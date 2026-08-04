import ts from "typescript";

/**
 * JSDoc is prose, not TypeScript syntax: the compiler API gives us the tag structure,
 * but the micro-conventions inside a tag's text (``Expects a `Float` ``, ``Default `1` ``,
 * `[fov=50]`) are documentation dialect that only a text scan can read.
 */

const NUMERIC_KIND = /Expects\s+an?\s+`(Float|Integer)`/i;
const OLD_STYLE_DEFAULT = /Default(?:\s+is)?\s+`([^`]*)`/;
const BRACKETED_DEFAULT = /\[\s*[A-Za-z0-9_$.[\]]+\s*=\s*([^\]]*)\]/;
const LINK_WITH_URL = /\{@link(?:code|plain)?\s+(https?:\/\/[^\s|}]+)\s*(?:\|\s*([^}]*?))?\s*\}/g;
const BARE_URL = /(https?:\/\/\S+)/;
const LEADING_DASH = /^[-–—]\s+/;

/** Strips the `/**`, `*\/` and per-line ` * ` decoration from a raw comment slice. */
function stripCommentDecoration(raw) {
	return raw
		.replace(/^\s*\/\*\*?/, "")
		.replace(/\*\/\s*$/, "")
		.split("\n")
		.map((line) => line.replace(/^\s*\*\s?/, ""))
		.join("\n")
		.trim();
}

function entityNameToString(name) {
	if (name === undefined) {
		return "";
	}
	if (ts.isIdentifier(name)) {
		return name.text;
	}
	if (ts.isQualifiedName(name)) {
		return `${entityNameToString(name.left)}.${name.right.text}`;
	}
	return name.getText();
}

/**
 * Flattens a JSDoc comment (string or node array with `{@link}` parts) to text,
 * keeping `{@link ...}` markers intact so the emitter can rewrite them as `<see cref="..."/>`.
 */
function flattenComment(comment) {
	if (comment === undefined) {
		return "";
	}
	if (typeof comment === "string") {
		return comment.trim();
	}
	let text = "";
	for (const part of comment) {
		if (ts.isJSDocLink(part) || ts.isJSDocLinkCode(part) || ts.isJSDocLinkPlain(part)) {
			const target = entityNameToString(part.name);
			const label = (part.text ?? "").trim();
			text += `{@link ${[target, label].filter(Boolean).join(" ")}}`;
			continue;
		}
		text += part.text;
	}
	return text.trim();
}

function rawTagText(tag) {
	return stripCommentDecoration(tag.getText()).trim();
}

export function numericKindFrom(text) {
	const match = text ? NUMERIC_KIND.exec(text) : null;
	if (match === null) {
		return undefined;
	}
	return match[1].toLowerCase() === "float" ? "float" : "integer";
}

function unquoteDefault(text) {
	const trimmed = text.trim().replace(/[.,;]$/, "").trim();
	const backticked = /^`(.*)`$/.exec(trimmed);
	return (backticked ? backticked[1] : trimmed).trim();
}

function collectSeeLinks(rawText, into) {
	let found = false;
	for (const match of rawText.matchAll(LINK_WITH_URL)) {
		found = true;
		const entry = { url: match[1] };
		const label = (match[2] ?? "").trim();
		if (label) {
			entry.label = label;
		}
		into.push(entry);
	}
	if (found) {
		return;
	}
	const bare = BARE_URL.exec(rawText);
	if (bare !== null) {
		into.push({ url: bare[1].replace(/[.,)]+$/, "") });
	}
}

/**
 * Builds the doc IR for a declaration. Returns `undefined` when the declaration carries no JSDoc,
 * so the emitted JSON stays free of empty objects.
 */
export function getDoc(node) {
	const blocks = ts.getJSDocCommentsAndTags(node).filter((x) => ts.isJSDoc(x));
	const tags = ts.getJSDocTags(node);
	if (blocks.length === 0 && tags.length === 0) {
		return undefined;
	}

	const doc = {};
	const summaries = [];
	for (const block of blocks) {
		const text = flattenComment(block.comment);
		if (text) {
			summaries.push(text);
		}
	}
	if (summaries.length > 0) {
		doc.summary = summaries.join("\n\n");
	}

	const remarks = [];
	const examples = [];
	const see = [];
	const other = [];
	for (const tag of tags) {
		const name = tag.tagName.text;
		const text = flattenComment(tag.comment);
		switch (name) {
			case "param":
			case "parameter":
				break;
			case "remarks":
				if (text) {
					remarks.push(text);
				}
				break;
			case "example":
				examples.push(text);
				break;
			case "returns":
			case "return":
				if (text) {
					doc.returns = text;
				}
				break;
			case "see":
				collectSeeLinks(rawTagText(tag), see);
				break;
			case "default":
			case "defaultValue":
				doc.defaultValue = unquoteDefault(text || rawTagText(tag).replace(/^@\w+\s*/, ""));
				break;
			case "deprecated":
				doc.isDeprecated = true;
				if (text) {
					doc.deprecated = text;
				}
				break;
			case "internal":
				doc.isInternal = true;
				break;
			case "override":
			case "readonly":
			case "abstract":
			case "static":
				break;
			default:
				other.push(text ? { name, text } : { name });
				break;
		}
	}
	if (remarks.length > 0) {
		doc.remarks = remarks.join("\n\n");
	}
	if (examples.length > 0) {
		doc.examples = examples;
	}
	if (see.length > 0) {
		doc.see = see;
	}
	if (other.length > 0) {
		doc.tags = other;
	}
	return Object.keys(doc).length > 0 ? doc : undefined;
}

/**
 * The prose a member-level numeric-kind scan may look at: everything except `@param`/`@returns`,
 * so a method's integer parameter never makes the method itself look integer-typed.
 */
export function memberDocText(doc) {
	if (doc === undefined) {
		return "";
	}
	return [doc.summary, doc.remarks, doc.defaultValue].filter(Boolean).join("\n");
}

/** Maps parameter name to the documented text, numeric kind and default for one signature. */
export function getParameterDocs(node) {
	const byName = new Map();
	for (const tag of ts.getJSDocTags(node)) {
		if (!ts.isJSDocParameterTag(tag)) {
			continue;
		}
		const name = entityNameToString(tag.name);
		if (!name || byName.has(name)) {
			continue;
		}
		const text = flattenComment(tag.comment).replace(LEADING_DASH, "").trim();
		const raw = rawTagText(tag);
		const entry = {};
		if (text) {
			entry.text = text;
		}
		const numericKind = numericKindFrom(text) ?? numericKindFrom(raw);
		if (numericKind !== undefined) {
			entry.numericKind = numericKind;
		}
		const oldStyle = OLD_STYLE_DEFAULT.exec(text);
		const bracketed = BRACKETED_DEFAULT.exec(raw);
		if (oldStyle !== null) {
			entry.defaultValue = unquoteDefault(oldStyle[1]);
		} else if (bracketed !== null) {
			entry.defaultValue = unquoteDefault(bracketed[1]);
		}
		byName.set(name, entry);
	}
	return byName;
}
