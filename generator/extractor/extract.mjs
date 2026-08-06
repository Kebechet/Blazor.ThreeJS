#!/usr/bin/env node
import ts from "typescript";
import fs from "node:fs";
import path from "node:path";
import url from "node:url";
import { createTypeMapper } from "./types.mjs";
import { getDoc, getParameterDocs, memberDocText, numericKindFrom } from "./jsdoc.mjs";

/**
 * Extracts the three.js public API from `@types/three` into `generator/three-api.json`.
 * See `generator/IR-SCHEMA.md` for the contract this produces.
 *
 * Everything TypeScript-specific lives here so the C# emitter never has to parse `.d.ts`.
 */

const IR_VERSION = 1;
const HERE = path.dirname(url.fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(HERE, "..", "..");
const TYPES_PACKAGE = path.join(REPO_ROOT, "node_modules", "@types", "three");
const SOURCE_ROOT = path.join(TYPES_PACKAGE, "src");
/**
 * Directories under `src/` that are out of scope: the TSL node stack.
 *
 * ⚠️ The reason changed with the WebGPU build and is no longer "the bundle does not carry them".
 * It does. Including them was tried and measured: 118 classes come into scope, 59 generate, and they
 * carry 89 members between them — `AONode` emits a constructor and one boolean query, and nothing
 * else. A TSL node is written by *composing* it (`positionLocal.add(vec3(0, 1, 0))`), and the whole
 * composition API is 638 free **functions** — `vec3`, `float`, `uniform`, `Fn` — which live in a
 * separate bundle (`three.tsl.min.js`) that this package does not vendor, and which the emitter could
 * not generate anyway: it emits classes and enums, not free functions.
 *
 * So the classes alone are constructible shells with nothing to do. Reaching TSL properly means
 * vendoring that second bundle, teaching the emitter free functions, and modelling the chaining API —
 * at which point this exclusion comes off. Until then it stays, and the cost of removing it is a
 * `three-api.json` that doubles to ~10 MB for no reachable capability.
 */
const EXCLUDED_DIRECTORIES = ["nodes"];
/**
 * The addons - `GLTFLoader`, `OrbitControls`, every post-processing pass. They live outside `src/`,
 * ship as separate modules three.js's own bundle does not include, and are never extracted. Measured
 * anyway, so the coverage table can state the size of the exclusion rather than assert it.
 */
const ADDONS_DIRECTORY = "examples/jsm";
const DEFAULT_OUTPUT = path.join(REPO_ROOT, "generator", "three-api.json");
/**
 * The public export surface of the build this package ships.
 * <p>
 * This is `Three.WebGPU.d.ts` rather than `Three.d.ts` because the vendored bundle is
 * `three.webgpu.min.js`. The two barrels describe genuinely different surfaces: the WebGPU one adds
 * `WebGPURenderer`, the node materials and the TSL stack, and drops `WebGLRenderer`. Reading the wrong
 * one produces a coverage report that is confidently wrong in both directions — classes marked
 * unreachable that the bundle exports, and classes marked reachable that it does not.
 */
const PUBLIC_BARREL = "Three.WebGPU.d.ts";
/**
 * The three.js bundle that ships inside the package. `three-interop.js` resolves every constructor
 * against this module's namespace (`THREE[op.t]`), so a class absent from it cannot be created at
 * runtime no matter what `@types/three` declares.
 */
const RUNTIME_BUNDLE_PATH = path.join(REPO_ROOT, "src", "Blazor.ThreeJS", "wwwroot", "three.webgpu.min.js");

function byText(a, b) {
	if (a === b) {
		return 0;
	}
	return a < b ? -1 : 1;
}

function toPosix(filePath) {
	return filePath.split(path.sep).join("/");
}

function relativeToPackage(fileName) {
	return toPosix(path.relative(TYPES_PACKAGE, path.resolve(fileName)));
}

function discoverSourceFiles(directory, acc = []) {
	const entries = fs.readdirSync(directory, { withFileTypes: true }).sort((a, b) => byText(a.name, b.name));
	for (const entry of entries) {
		const name = entry.name;
		const full = path.join(directory, name);
		if (entry.isDirectory()) {
			discoverSourceFiles(full, acc);
			continue;
		}
		if (name.endsWith(".d.ts")) {
			acc.push(full);
		}
	}
	return acc;
}

/**
 * Counts the `.d.ts` files and class declarations under a directory the extractor never parses into
 * the IR. A standalone `createSourceFile` is deliberate: nothing here needs the checker, and adding
 * these files to the program would pull the excluded surface back into scope.
 *
 * A missing path throws rather than counting zero. These counts reach the README as a completeness
 * claim ("the node stack is 118 classes we do not wrap"), so an upstream rename would otherwise
 * publish `| … | 0 |` — while the 215 files that exclusion was holding back flood the IR in the same
 * run, with a green build and a passing `emit:check`.
 */
function countDeclarations(root) {
	if (!fs.existsSync(root)) {
		throw new Error(
			`Declared exclusion path '${toPosix(root)}' does not exist. The README states its size as a ` +
				"completeness claim, and counting a missing directory as zero would publish a false one. " +
				"Upstream has probably moved or renamed it: update EXCLUDED_DIRECTORIES / ADDONS_DIRECTORY.",
		);
	}
	const files = discoverSourceFiles(root).sort(byText);
	let classes = 0;
	for (const fileName of files) {
		const sourceFile = ts.createSourceFile(
			fileName,
			fs.readFileSync(fileName, "utf8"),
			ts.ScriptTarget.ESNext,
			false,
			ts.ScriptKind.TS,
		);
		const visit = (node) => {
			if (ts.isClassDeclaration(node)) {
				classes++;
			}
			ts.forEachChild(node, visit);
		};
		visit(sourceFile);
	}
	return { files: files.length, classes };
}

function isExcluded(fileName) {
	const relative = toPosix(path.relative(SOURCE_ROOT, path.resolve(fileName)));
	if (relative.startsWith("..")) {
		return false;
	}
	return EXCLUDED_DIRECTORIES.some((dir) => relative === dir || relative.startsWith(`${dir}/`));
}

function hasModifier(node, kind) {
	return node.modifiers?.some((modifier) => modifier.kind === kind) ?? false;
}

function memberName(node) {
	if (node.name === undefined) {
		return "<anonymous>";
	}
	if (ts.isIdentifier(node.name) || ts.isPrivateIdentifier(node.name)) {
		return node.name.text;
	}
	if (ts.isStringLiteral(node.name) || ts.isNumericLiteral(node.name)) {
		return node.name.text;
	}
	return node.name.getText();
}

function visibilityOf(node) {
	if (hasModifier(node, ts.SyntaxKind.PrivateKeyword)) {
		return "private";
	}
	if (hasModifier(node, ts.SyntaxKind.ProtectedKeyword)) {
		return "protected";
	}
	return undefined;
}

function assignIfDefined(target, key, value) {
	if (value !== undefined) {
		target[key] = value;
	}
}

/**
 * The names the shipped three.js bundle actually puts on `THREE`. Read by importing the vendored
 * module, which is the same object `three-interop.js` indexes, so this is the runtime truth rather
 * than a second reading of the types.
 */
async function readRuntimeExportNames() {
	if (!fs.existsSync(RUNTIME_BUNDLE_PATH)) {
		throw new Error(`The vendored three.js bundle is missing at '${toPosix(RUNTIME_BUNDLE_PATH)}'.`);
	}
	const bundle = await import(url.pathToFileURL(RUNTIME_BUNDLE_PATH).href);
	return new Set(Object.keys(bundle));
}

async function main() {
	const outputIndex = process.argv.indexOf("--out");
	const isCheck = process.argv.includes("--check");
	const outputPath = outputIndex >= 0 ? path.resolve(process.argv[outputIndex + 1]) : DEFAULT_OUTPUT;

	if (!fs.existsSync(SOURCE_ROOT)) {
		console.error(`@types/three not found at ${SOURCE_ROOT}. Run 'npm install' first.`);
		process.exit(1);
	}

	const runtimeExportNames = await readRuntimeExportNames();

	const allFiles = discoverSourceFiles(SOURCE_ROOT).sort(byText);
	const inScopeFiles = allFiles.filter((x) => !isExcluded(x));

	const program = ts.createProgram(inScopeFiles, {
		target: ts.ScriptTarget.ESNext,
		module: ts.ModuleKind.ESNext,
		moduleResolution: ts.ModuleResolutionKind.Bundler,
		skipLibCheck: true,
		strict: false,
		// Pinned so the run does not depend on whatever other @types packages happen to be installed.
		types: [],
	});
	const checker = program.getTypeChecker();

	const inScopePaths = new Set(inScopeFiles.map((x) => path.resolve(x)));

	function classifyDeclarationFile(fileName) {
		const resolved = path.resolve(fileName);
		if (inScopePaths.has(resolved)) {
			return { origin: "in-scope", file: relativeToPackage(resolved) };
		}
		if (isExcluded(resolved)) {
			return { origin: "excluded", file: relativeToPackage(resolved) };
		}
		if (toPosix(resolved).includes("/node_modules/typescript/lib/")) {
			return { origin: "lib" };
		}
		if (toPosix(resolved).startsWith(toPosix(TYPES_PACKAGE))) {
			return { origin: "package", file: relativeToPackage(resolved) };
		}
		return { origin: "external" };
	}

	let numericKindMarkers = 0;

	/**
	 * Records a numeric-kind marker on the IR and counts it. Every `Expects a \`Float\`` /
	 * `Expects an \`Integer\`` marker the extractor reads goes through here, so the count in
	 * `meta.counts` is exactly what a reader reproduces by grepping the snapshot for the field.
	 */
	function assignNumericKind(target, key, numericKind) {
		if (numericKind === undefined) {
			return;
		}
		target[key] = numericKind;
		numericKindMarkers++;
	}

	const mapper = createTypeMapper({ checker, classifyDeclarationFile, assignNumericKind });
	const { typeToIr, parameterToIr, typeParametersToIr } = mapper;

	/** The `.d.ts` a module specifier resolves to, using the same resolution the program was built with. */
	function moduleSourceFileOf(moduleSpecifier) {
		const symbol = checker.getSymbolAtLocation(moduleSpecifier);
		return (symbol?.declarations ?? []).find((x) => ts.isSourceFile(x));
	}

	/**
	 * Whether a name reaches a **value** rather than only a type. `export type { X }` and
	 * `export { type X }` publish a name a consumer can annotate with but never construct, and an
	 * `interface` re-exported without the `type` keyword is the same thing said differently.
	 */
	function isValueName(nameNode) {
		let symbol = checker.getSymbolAtLocation(nameNode);
		if (symbol === undefined) {
			return false;
		}
		if ((symbol.flags & ts.SymbolFlags.Alias) !== 0) {
			try {
				symbol = checker.getAliasedSymbol(symbol);
			} catch {
				/* keep the alias symbol */
			}
		}
		return (symbol.flags & ts.SymbolFlags.Value) !== 0;
	}

	function declaredNameNodes(statement) {
		if (ts.isVariableStatement(statement)) {
			return statement.declarationList.declarations.map((x) => x.name);
		}
		return statement.name === undefined ? [] : [statement.name];
	}

	/**
	 * One `export … from` / `export { … }` statement as the IR records it. The five barrel files are
	 * nothing but these, so without them they contribute no entry at all while still counting towards
	 * `filesScanned`.
	 */
	function exportDeclarationIr(statement, file) {
		const ir = { file };
		assignIfDefined(ir, "module", statement.moduleSpecifier?.text);
		if (statement.moduleSpecifier !== undefined) {
			const target = moduleSourceFileOf(statement.moduleSpecifier);
			if (target !== undefined) {
				const placement = classifyDeclarationFile(target.fileName);
				ir.targetOrigin = placement.origin;
				assignIfDefined(ir, "targetFile", placement.file);
			}
		}
		if (statement.isTypeOnly) {
			ir.isTypeOnly = true;
		}
		if (statement.exportClause === undefined) {
			ir.kind = "star";
			return ir;
		}
		if (ts.isNamespaceExport(statement.exportClause)) {
			ir.kind = "namespace";
			ir.names = [{ name: statement.exportClause.name.text, isValue: !statement.isTypeOnly }];
			return ir;
		}
		ir.kind = "named";
		ir.names = statement.exportClause.elements.map((element) => {
			const isTypeOnly = statement.isTypeOnly || element.isTypeOnly;
			const entry = { name: element.name.text };
			if (element.propertyName !== undefined) {
				entry.localName = element.propertyName.text;
			}
			entry.isValue = !isTypeOnly && isValueName(element.name);
			return entry;
		});
		return ir;
	}

	/**
	 * Walks the barrel graph out of the package's public entry point and returns the names three.js
	 * publishes, split by whether the name reaches a value.
	 *
	 * This is what makes `isExported` mean "three.js exports this to users". Read per file, the answer
	 * is always yes — every `.d.ts` in `src/` uses the `export` keyword — which says only that a sibling
	 * module could import it, not that it appears on `THREE`.
	 */
	function resolvePublicSurface(barrelFileName) {
		const barrel = program.getSourceFile(barrelFileName);
		if (barrel === undefined) {
			throw new Error(`The public barrel '${relativeToPackage(barrelFileName)}' is not in the program.`);
		}

		const valueExports = new Set();
		const typeOnlyExports = new Set();
		const files = new Set();
		const visited = new Set();

		function record(name, isValue) {
			if (isValue) {
				valueExports.add(name);
				return;
			}
			typeOnlyExports.add(name);
		}

		// A file is walked once per type-only context: reaching it through `export type * from` publishes
		// a strictly weaker set than reaching it through a plain `export * from`, and a barrel can do both.
		function walk(sourceFile, isTypeOnlyContext) {
			const key = `${sourceFile.fileName}|${isTypeOnlyContext}`;
			if (visited.has(key)) {
				return;
			}
			visited.add(key);
			files.add(relativeToPackage(sourceFile.fileName));

			for (const statement of sourceFile.statements) {
				if (ts.isExportDeclaration(statement)) {
					const isTypeOnly = isTypeOnlyContext || statement.isTypeOnly;
					if (statement.exportClause === undefined) {
						const target = moduleSourceFileOf(statement.moduleSpecifier);
						if (target === undefined) {
							throw new Error(
								`'${relativeToPackage(sourceFile.fileName)}' re-exports ${statement.moduleSpecifier.getText()}, ` +
									"which does not resolve. The public surface would silently lose everything behind it.",
							);
						}
						walk(target, isTypeOnly);
						continue;
					}
					if (ts.isNamespaceExport(statement.exportClause)) {
						record(statement.exportClause.name.text, !isTypeOnly);
						continue;
					}
					for (const element of statement.exportClause.elements) {
						record(element.name.text, !(isTypeOnly || element.isTypeOnly) && isValueName(element.name));
					}
					continue;
				}
				if (!hasModifier(statement, ts.SyntaxKind.ExportKeyword)) {
					continue;
				}
				// `export default class X` publishes `default`, not `X`, and `export * from` does not carry a
				// default across. Only an explicit `export { default as … } from` republishes it, and that
				// arrives through the named-export branch above under the name it is given there.
				if (hasModifier(statement, ts.SyntaxKind.DefaultKeyword)) {
					continue;
				}
				for (const nameNode of declaredNameNodes(statement)) {
					record(nameNode.getText(), !isTypeOnlyContext && isValueName(nameNode));
				}
			}
		}

		walk(barrel, false);

		// A name published both ways - `export * from` reaching the declaration and a sibling
		// `export type { … }` naming it - is a value. The weaker spelling does not take it away.
		for (const name of valueExports) {
			typeOnlyExports.delete(name);
		}

		return {
			barrel: relativeToPackage(barrelFileName),
			files: [...files].sort(byText),
			valueExports,
			typeOnlyExports,
		};
	}

	const publicSurface = resolvePublicSurface(path.join(SOURCE_ROOT, PUBLIC_BARREL));

	const classes = [];
	const interfaces = [];
	const enums = [];
	const constants = [];
	const typeAliases = [];
	const functions = [];
	const namespaces = [];
	const moduleAugmentations = [];
	const reExports = [];
	let constantsSkippedOutOfScope = 0;

	/**
	 * Whether the public barrel publishes a **value** under this name - the question that decides
	 * whether the applier can reach it on `THREE`. An `export type { X }` re-export publishes a name a
	 * consumer can annotate with and never construct, which is how `@types/three` spells the three
	 * `LightShadow` subclasses three.js keeps internal.
	 */
	function isPublicValue(name) {
		return publicSurface.valueExports.has(name);
	}

	/** Whether the public barrel publishes this name at all, for declarations that are only ever types. */
	function isPublicType(name) {
		return publicSurface.valueExports.has(name) || publicSurface.typeOnlyExports.has(name);
	}

	/**
	 * `@param` tags are matched by name. Some upstream docs drift from the signature
	 * (`Path.absarc` documents `x`/`y` for parameters named `aX`/`aY`), which would silently drop the
	 * numeric kind. Fall back to position only when the tag count matches the parameter count and
	 * every name that *does* match sits at its own index, so name and position never disagree.
	 * Parameters resolved this way are marked `docSource: "position"` so the inference stays auditable.
	 */
	function resolveParameterDocs(node, parameterNames) {
		const documented = getParameterDocs(node);
		const byName = new Map();
		for (const [index, name] of parameterNames.entries()) {
			const entry = documented.get(name);
			if (entry !== undefined) {
				byName.set(index, entry);
			}
		}
		if (byName.size === parameterNames.length || documented.size !== parameterNames.length) {
			return byName;
		}
		const documentedNames = [...documented.keys()];
		const namesAgreeWithPositions = [...byName.keys()].every(
			(index) => documentedNames[index] === parameterNames[index],
		);
		if (!namesAgreeWithPositions) {
			return byName;
		}
		for (const [index, name] of documentedNames.entries()) {
			if (byName.has(index)) {
				continue;
			}
			byName.set(index, { ...documented.get(name), docSource: "position" });
		}
		return byName;
	}

	function signatureIr(node, options = {}) {
		const doc = getDoc(node);
		const parameterIrs = node.parameters.map(parameterToIr);
		const parameterDocs = resolveParameterDocs(node, parameterIrs.map((x) => x.name));
		const parameters = parameterIrs.map((parameterIr, index) => {
			const documented = parameterDocs.get(index);
			if (documented !== undefined) {
				assignNumericKind(parameterIr, "numericKind", documented.numericKind);
				assignIfDefined(parameterIr, "defaultValue", documented.defaultValue);
				assignIfDefined(parameterIr, "doc", documented.text);
				assignIfDefined(parameterIr, "docSource", documented.docSource);
			}
			return parameterIr;
		});

		const ir = { parameters };
		const typeParameters = typeParametersToIr(node.typeParameters);
		assignIfDefined(ir, "typeParameters", typeParameters);
		if (options.hasReturnType) {
			assignIfDefined(ir, "returnType", typeToIr(node.type));
			assignNumericKind(ir, "returnNumericKind", numericKindFrom(doc?.returns));
		}
		assignIfDefined(ir, "doc", doc);
		return ir;
	}

	function propertyIr(node, kind) {
		const doc = getDoc(node);
		const ir = { name: memberName(node) };
		if (kind !== "property") {
			ir.accessor = kind;
		}
		if (hasModifier(node, ts.SyntaxKind.StaticKeyword)) {
			ir.isStatic = true;
		}
		if (hasModifier(node, ts.SyntaxKind.ReadonlyKeyword)) {
			ir.isReadonly = true;
		}
		if (hasModifier(node, ts.SyntaxKind.OverrideKeyword)) {
			ir.isOverride = true;
		}
		if (hasModifier(node, ts.SyntaxKind.AbstractKeyword)) {
			ir.isAbstract = true;
		}
		if (node.questionToken !== undefined) {
			ir.isOptional = true;
		}
		assignIfDefined(ir, "visibility", visibilityOf(node));
		const typeNode = ts.isSetAccessorDeclaration(node) ? node.parameters[0]?.type : node.type;
		assignIfDefined(ir, "type", typeToIr(typeNode));
		assignNumericKind(ir, "numericKind", numericKindFrom(memberDocText(doc)));
		assignIfDefined(ir, "defaultValue", doc?.defaultValue);
		assignIfDefined(ir, "doc", doc);
		return ir;
	}

	/** Class and interface members share a shape; overloads are grouped under one entry. */
	function collectMembers(members) {
		const properties = [];
		const methodsByKey = new Map();
		const methodOrder = [];
		const indexSignatures = [];
		const callSignatures = [];
		const constructSignatures = [];
		const constructors = [];
		const accessorsByName = new Map();

		for (const member of members) {
			if (ts.isConstructorDeclaration(member)) {
				constructors.push(signatureIr(member, { hasReturnType: false }));
				continue;
			}
			if (ts.isPropertyDeclaration(member) || ts.isPropertySignature(member)) {
				properties.push(propertyIr(member, "property"));
				continue;
			}
			if (ts.isGetAccessorDeclaration(member) || ts.isSetAccessorDeclaration(member)) {
				const name = memberName(member);
				const isGetter = ts.isGetAccessorDeclaration(member);
				const existing = accessorsByName.get(name);
				if (existing !== undefined) {
					existing.accessor = "get-set";
					delete existing.isReadonly;
					if (isGetter && existing.type === undefined) {
						assignIfDefined(existing, "type", typeToIr(member.type));
					}
					continue;
				}
				const ir = propertyIr(member, isGetter ? "get" : "set");
				if (isGetter) {
					ir.isReadonly = true;
				}
				accessorsByName.set(name, ir);
				properties.push(ir);
				continue;
			}
			if (ts.isMethodDeclaration(member) || ts.isMethodSignature(member)) {
				const name = memberName(member);
				const isStatic = hasModifier(member, ts.SyntaxKind.StaticKeyword);
				const key = `${isStatic ? "static " : ""}${name}`;
				let entry = methodsByKey.get(key);
				if (entry === undefined) {
					entry = { name, overloads: [] };
					if (isStatic) {
						entry.isStatic = true;
					}
					if (hasModifier(member, ts.SyntaxKind.OverrideKeyword)) {
						entry.isOverride = true;
					}
					if (hasModifier(member, ts.SyntaxKind.AbstractKeyword)) {
						entry.isAbstract = true;
					}
					if (member.questionToken !== undefined) {
						entry.isOptional = true;
					}
					assignIfDefined(entry, "visibility", visibilityOf(member));
					methodsByKey.set(key, entry);
					methodOrder.push(entry);
				}
				entry.overloads.push(signatureIr(member, { hasReturnType: true }));
				continue;
			}
			if (ts.isIndexSignatureDeclaration(member)) {
				indexSignatures.push(signatureIr(member, { hasReturnType: true }));
				continue;
			}
			if (ts.isCallSignatureDeclaration(member)) {
				callSignatures.push(signatureIr(member, { hasReturnType: true }));
				continue;
			}
			if (ts.isConstructSignatureDeclaration(member)) {
				constructSignatures.push(signatureIr(member, { hasReturnType: true }));
			}
		}

		return { constructors, properties, methods: methodOrder, indexSignatures, callSignatures, constructSignatures };
	}

	function buildExportMap(sourceFile) {
		const map = new Map();
		const moduleSymbol = checker.getSymbolAtLocation(sourceFile);
		if (moduleSymbol === undefined) {
			return map;
		}
		for (const exported of checker.getExportsOfModule(moduleSymbol)) {
			let target = exported;
			if ((exported.flags & ts.SymbolFlags.Alias) !== 0) {
				try {
					target = checker.getAliasedSymbol(exported);
				} catch {
					/* keep the alias symbol */
				}
			}
			for (const declaration of target.declarations ?? []) {
				if (!map.has(declaration)) {
					map.set(declaration, exported.name);
				}
			}
		}
		return map;
	}

	for (const fileName of inScopeFiles) {
		const sourceFile = program.getSourceFile(fileName);
		if (sourceFile === undefined) {
			throw new Error(`TypeScript did not load ${fileName}`);
		}
		const file = relativeToPackage(sourceFile.fileName);
		const exportMap = buildExportMap(sourceFile);

		for (const statement of sourceFile.statements) {
			if (ts.isExportDeclaration(statement)) {
				reExports.push(exportDeclarationIr(statement, file));
				continue;
			}

			const exportName = exportMap.get(statement);

			if (ts.isClassDeclaration(statement)) {
				const members = collectMembers(statement.members);
				const name = statement.name?.text ?? "<anonymous>";
				const ir = {
					name,
					file,
					isExported: isPublicValue(name),
					isRuntimeExport: runtimeExportNames.has(name),
				};
				if (exportName !== undefined && exportName !== ir.name) {
					ir.exportName = exportName;
				}
				if (hasModifier(statement, ts.SyntaxKind.DefaultKeyword)) {
					ir.isDefaultExport = true;
				}
				if (hasModifier(statement, ts.SyntaxKind.AbstractKeyword)) {
					ir.isAbstract = true;
				}
				assignIfDefined(ir, "typeParameters", typeParametersToIr(statement.typeParameters));

				for (const clause of statement.heritageClauses ?? []) {
					if (clause.token === ts.SyntaxKind.ExtendsKeyword && clause.types.length > 0) {
						ir.extends = typeToIr(clause.types[0]);
					} else if (clause.token === ts.SyntaxKind.ImplementsKeyword) {
						ir.implements = clause.types.map(typeToIr);
					}
				}

				assignIfDefined(ir, "doc", getDoc(statement));
				ir.constructors = members.constructors;
				ir.properties = members.properties;
				ir.methods = members.methods;
				if (members.indexSignatures.length > 0) {
					ir.indexSignatures = members.indexSignatures;
				}
				classes.push(ir);
				continue;
			}

			if (ts.isInterfaceDeclaration(statement)) {
				const members = collectMembers(statement.members);
				const ir = { name: statement.name.text, file, isExported: isPublicType(statement.name.text) };
				assignIfDefined(ir, "typeParameters", typeParametersToIr(statement.typeParameters));
				const extended = (statement.heritageClauses ?? [])
					.filter((clause) => clause.token === ts.SyntaxKind.ExtendsKeyword)
					.flatMap((clause) => clause.types.map(typeToIr));
				if (extended.length > 0) {
					ir.extends = extended;
				}
				assignIfDefined(ir, "doc", getDoc(statement));
				ir.properties = members.properties;
				ir.methods = members.methods;
				if (members.indexSignatures.length > 0) {
					ir.indexSignatures = members.indexSignatures;
				}
				if (members.callSignatures.length > 0) {
					ir.callSignatures = members.callSignatures;
				}
				if (members.constructSignatures.length > 0) {
					ir.constructSignatures = members.constructSignatures;
				}
				interfaces.push(ir);
				continue;
			}

			if (ts.isEnumDeclaration(statement)) {
				const ir = {
					name: statement.name.text,
					file,
					isExported: isPublicValue(statement.name.text),
					isConst: hasModifier(statement, ts.SyntaxKind.ConstKeyword),
				};
				assignIfDefined(ir, "doc", getDoc(statement));
				ir.members = statement.members.map((member) => {
					const memberIr = { name: memberName(member) };
					const value = checker.getConstantValue(member);
					if (value !== undefined) {
						memberIr.value = value;
					}
					if (member.initializer !== undefined) {
						memberIr.initializerText = member.initializer.getText();
					}
					assignIfDefined(memberIr, "doc", getDoc(member));
					return memberIr;
				});
				enums.push(ir);
				continue;
			}

			if (ts.isTypeAliasDeclaration(statement)) {
				const ir = { name: statement.name.text, file, isExported: isPublicType(statement.name.text) };
				assignIfDefined(ir, "typeParameters", typeParametersToIr(statement.typeParameters));
				ir.type = typeToIr(statement.type);
				const group = constantGroupOf(ir.type);
				if (group !== undefined) {
					ir.constantGroup = group;
				}
				assignIfDefined(ir, "doc", getDoc(statement));
				typeAliases.push(ir);
				continue;
			}

			if (ts.isFunctionDeclaration(statement)) {
				const name = statement.name?.text ?? "<anonymous>";
				let entry = functions.find((x) => x.name === name && x.file === file);
				if (entry === undefined) {
					entry = { name, file, isExported: isPublicValue(name), overloads: [] };
					assignIfDefined(entry, "doc", getDoc(statement));
					functions.push(entry);
				}
				entry.overloads.push(signatureIr(statement, { hasReturnType: true }));
				continue;
			}

			if (ts.isVariableStatement(statement)) {
				for (const declaration of statement.declarationList.declarations) {
					const type = typeToIr(declaration.type);
					if (type?.kind === "typeQuery" && type.target?.origin === "excluded") {
						constantsSkippedOutOfScope++;
						continue;
					}
					const ir = {
						name: declaration.name.getText(),
						file,
						isExported: isPublicValue(declaration.name.getText()),
						isConst: (statement.declarationList.flags & ts.NodeFlags.Const) !== 0,
					};
					assignIfDefined(ir, "type", type);
					assignIfDefined(ir, "doc", getDoc(declaration));
					constants.push(ir);
				}
				continue;
			}

			if (ts.isModuleDeclaration(statement) && statement.body !== undefined && ts.isModuleBlock(statement.body)) {
				if (ts.isStringLiteral(statement.name)) {
					moduleAugmentations.push(moduleAugmentationIr(statement, file));
					continue;
				}
				const ir = { name: statement.name.getText(), file, isExported: isPublicValue(statement.name.getText()) };
				assignIfDefined(ir, "doc", getDoc(statement));
				ir.members = statement.body.statements
					.flatMap((member) => {
						if (ts.isTypeAliasDeclaration(member) || ts.isInterfaceDeclaration(member) || ts.isClassDeclaration(member)) {
							return [{ name: member.name?.getText() ?? "<anonymous>", declarationKind: ts.SyntaxKind[member.kind] }];
						}
						if (ts.isVariableStatement(member)) {
							return member.declarationList.declarations.map((x) => ({
								name: x.name.getText(),
								declarationKind: "VariableDeclaration",
							}));
						}
						if (ts.isFunctionDeclaration(member)) {
							return [{ name: member.name?.text ?? "<anonymous>", declarationKind: "FunctionDeclaration" }];
						}
						if (ts.isExportDeclaration(member) && member.exportClause !== undefined && ts.isNamedExports(member.exportClause)) {
							return member.exportClause.elements.map((element) => ({
								name: element.name.text,
								declarationKind: "ExportSpecifier",
							}));
						}
						return [{ name: "<unknown>", declarationKind: ts.SyntaxKind[member.kind] }];
					})
					.sort((a, b) => byText(a.name, b.name) || byText(a.declarationKind, b.declarationKind));
				namespaces.push(ir);
			}
		}
	}

	/**
	 * `declare module "../../scenes/Scene.js" { interface Scene { ... } }` - declaration merging that
	 * bolts extra members onto a type declared elsewhere. In three.js these come from the node stack,
	 * so the emitter must see them to know a class's real member set is not just its own declaration.
	 */
	function moduleAugmentationIr(statement, file) {
		const ir = { file, targetModule: statement.name.text };
		const moduleSymbol = checker.getSymbolAtLocation(statement.name);
		const targetSourceFile = (moduleSymbol?.declarations ?? []).find((x) => ts.isSourceFile(x));
		if (targetSourceFile !== undefined) {
			const placement = classifyDeclarationFile(targetSourceFile.fileName);
			ir.targetOrigin = placement.origin;
			assignIfDefined(ir, "targetFile", placement.file);
		}
		ir.augments = statement.body.statements
			.filter((member) => ts.isInterfaceDeclaration(member) || ts.isClassDeclaration(member))
			.map((member) => {
				const members = collectMembers(member.members);
				const augmented = { name: member.name?.getText() ?? "<anonymous>" };
				const extended = (member.heritageClauses ?? [])
					.filter((clause) => clause.token === ts.SyntaxKind.ExtendsKeyword)
					.flatMap((clause) => clause.types.map(typeToIr));
				if (extended.length > 0) {
					augmented.extends = extended;
				}
				if (members.properties.length > 0) {
					augmented.properties = members.properties;
				}
				if (members.methods.length > 0) {
					augmented.methods = members.methods;
				}
				return augmented;
			});
		return ir;
	}

	/** `type Side = typeof FrontSide | typeof BackSide` - the grouping signal for loose constants. */
	function constantGroupOf(type) {
		if (type === undefined) {
			return undefined;
		}
		const members = type.kind === "union" ? type.types : [type];
		if (type.kind !== "union" || members.length < 2) {
			return undefined;
		}
		const names = [];
		for (const member of members) {
			if (member.kind !== "typeQuery" || member.target?.origin !== "in-scope" || member.target.refKind !== "constant") {
				return undefined;
			}
			names.push(member.name);
		}
		return names;
	}

	const sortByNameThenFile = (a, b) => byText(a.name, b.name) || byText(a.file, b.file);
	classes.sort(sortByNameThenFile);
	interfaces.sort(sortByNameThenFile);
	enums.sort(sortByNameThenFile);
	constants.sort(sortByNameThenFile);
	typeAliases.sort(sortByNameThenFile);
	functions.sort(sortByNameThenFile);
	namespaces.sort(sortByNameThenFile);
	moduleAugmentations.sort((a, b) => byText(a.targetModule, b.targetModule) || byText(a.file, b.file));
	reExports.sort((a, b) => byText(a.file, b.file) || byText(a.module ?? "", b.module ?? "") || byText(a.kind, b.kind));

	const duplicateClassNames = [];
	const filesByClassName = new Map();
	for (const entry of classes) {
		filesByClassName.set(entry.name, [...(filesByClassName.get(entry.name) ?? []), entry.file]);
	}
	for (const [name, files] of [...filesByClassName].sort((a, b) => byText(a[0], b[0]))) {
		if (files.length > 1) {
			duplicateClassNames.push({ name, files });
		}
	}

	const typesPackageJson = JSON.parse(fs.readFileSync(path.join(TYPES_PACKAGE, "package.json"), "utf8"));
	const ir = {
		meta: {
			irVersion: IR_VERSION,
			generator: "generator/extractor/extract.mjs",
			schema: "generator/IR-SCHEMA.md",
			typesPackage: typesPackageJson.name,
			typesVersion: typesPackageJson.version,
			typescriptVersion: ts.version,
			sourceRoot: "src",
			excludedDirectories: EXCLUDED_DIRECTORIES.map((x) => ({
				path: `src/${x}`,
				...countDeclarations(path.join(SOURCE_ROOT, x)),
			})),
			addons: { path: ADDONS_DIRECTORY, ...countDeclarations(path.join(TYPES_PACKAGE, ADDONS_DIRECTORY)) },
			publicSurface: {
				barrel: publicSurface.barrel,
				runtimeBundle: toPosix(path.relative(REPO_ROOT, RUNTIME_BUNDLE_PATH)),
				barrelFiles: publicSurface.files.length,
				valueExports: publicSurface.valueExports.size,
				typeOnlyExports: publicSurface.typeOnlyExports.size,
				runtimeExports: runtimeExportNames.size,
				valueExportsAbsentFromRuntime: [...publicSurface.valueExports].filter((x) => !runtimeExportNames.has(x)).sort(byText),
				runtimeExportsAbsentFromBarrel: [...runtimeExportNames].filter((x) => !publicSurface.valueExports.has(x)).sort(byText),
			},
			counts: {
				filesScanned: inScopeFiles.length,
				filesExcluded: allFiles.length - inScopeFiles.length,
				classes: classes.length,
				interfaces: interfaces.length,
				enums: enums.length,
				constants: constants.length,
				constantsSkippedOutOfScope,
				typeAliases: typeAliases.length,
				functions: functions.length,
				namespaces: namespaces.length,
				moduleAugmentations: moduleAugmentations.length,
				reExports: reExports.length,
				// The sole source of the float/integer distinction, and prose rather than syntax: a
				// DefinitelyTyped reflow of `Expects a \`Float\`` costs nothing else and would silently
				// collapse every numeric onto the project-wide `float` default. Counted so a run that
				// finds none is a `243 -> 0` line in the IR diff instead of a normal-looking success.
				numericKindMarkers,
			},
			duplicateClassNames,
		},
		classes,
		interfaces,
		enums,
		constants,
		typeAliases,
		functions,
		namespaces,
		moduleAugmentations,
		reExports,
	};

	// Diagnostics gate the write. TypeScript recovers from syntax it cannot parse by dropping the
	// statement, so new syntax in a future three.js release yields a quietly truncated snapshot; a
	// warning printed after the file is on disk is a warning nobody sees in a green CI run.
	const syntacticDiagnostics = program.getSyntacticDiagnostics();
	if (syntacticDiagnostics.length > 0) {
		console.error(`${syntacticDiagnostics.length} syntactic diagnostic(s) while parsing ${typesPackageJson.name}. Nothing was written.`);
		for (const diagnostic of syntacticDiagnostics.slice(0, 10)) {
			const location = diagnostic.file === undefined
				? ""
				: `${relativeToPackage(diagnostic.file.fileName)}: `;
			console.error(`  ${location}${ts.flattenDiagnosticMessageText(diagnostic.messageText, " ")}`);
		}
		process.exit(1);
	}

	const rendered = `${JSON.stringify(ir, null, 2)}\n`;
	if (isCheck) {
		return check(outputPath, rendered);
	}

	fs.mkdirSync(path.dirname(outputPath), { recursive: true });
	fs.writeFileSync(outputPath, rendered, "utf8");
	console.log(`wrote ${toPosix(path.relative(REPO_ROOT, outputPath))}`);
	console.log(JSON.stringify(ir.meta.counts, null, 2));
}

/**
 * The golden check for the extractor, mirroring `emit:check` one level upstream: re-extracts in
 * memory and fails if the result differs from the committed snapshot. Without it nothing guards
 * sources to IR - a change to the extractor, or to the pinned `@types/three`, could land with a
 * stale `three-api.json` and a green build.
 */
function check(outputPath, rendered) {
	const relative = toPosix(path.relative(REPO_ROOT, outputPath));
	if (!fs.existsSync(outputPath)) {
		console.error(`MISSING ${relative} - the extractor produces it but it is not committed.`);
		process.exit(1);
	}

	// Line endings are normalized rather than compared: the repository has no .gitattributes and
	// core.autocrlf is enabled on Windows, so the committed file can arrive with CRLF on one machine
	// and LF on another without a single extracted character having changed.
	const committed = fs.readFileSync(outputPath, "utf8").replace(/\r\n/g, "\n");
	if (committed === rendered) {
		console.log(`ok      ${relative} matches what the extractor produces`);
		return;
	}

	const committedLines = committed.split("\n");
	const renderedLines = rendered.split("\n");
	console.error(`DRIFT   ${relative}`);
	for (let index = 0; index < Math.max(committedLines.length, renderedLines.length); index++) {
		const committedLine = index < committedLines.length ? committedLines[index] : "<end of file>";
		const renderedLine = index < renderedLines.length ? renderedLines[index] : "<end of file>";
		if (committedLine === renderedLine) {
			continue;
		}
		console.error(`        first difference at line ${index + 1}:`);
		console.error(`          committed:   ${committedLine.trim()}`);
		console.error(`          regenerated: ${renderedLine.trim()}`);
		break;
	}
	console.error("");
	console.error("The extractor's output no longer matches the committed snapshot. Review the change, then run");
	console.error("`npm run extract` to accept it.");
	process.exit(1);
}

await main();
