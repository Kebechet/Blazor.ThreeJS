#!/usr/bin/env node
import fs from "node:fs";
import path from "node:path";
import url from "node:url";

/**
 * Vendors the three.js addons this package wraps into `src/Blazor.ThreeJS/wwwroot/addons/`, and
 * proves the copy is complete.
 *
 * The addons live in `examples/jsm`, outside the three.js bundle the package already ships, so each
 * one has to be copied in as its own static asset. Copying only the entry point is the failure this
 * script exists to prevent: an ES module that imports a sibling it cannot resolve builds green,
 * passes every C# test, and fails in the browser with `ERR_MODULE_NOT_FOUND` the first time a
 * consumer actually loads a model. So every file's imports are read after it is copied, and a
 * relative import that names a file outside the manifest fails the run by name.
 *
 *     node generator/vendor-addons.mjs           rewrite the vendored copies
 *     node generator/vendor-addons.mjs --check   fail if they differ from upstream
 */

const HERE = path.dirname(url.fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(HERE, "..");
const UPSTREAM_ROOT = path.join(REPO_ROOT, "node_modules", "three", "examples", "jsm");
const VENDOR_ROOT = path.join(REPO_ROOT, "src", "Blazor.ThreeJS", "wwwroot", "addons");
/** The vendored three.js bundle every addon's bare `three` import is rewritten to point at. */
const BUNDLE_FILE_NAME = "three.module.js";
/** Bare module specifier the addons import three.js under, which no browser can resolve unaided. */
const BARE_THREE_SPECIFIER = "three";

/**
 * The addon files that ship, as paths under `examples/jsm`. The two entry points are the loader and
 * the controls; the two utils below them are there because `GLTFLoader` imports them, which is a
 * fact about upstream rather than a choice - `verifyImportClosure` re-derives it on every run and
 * fails if this list stops being closed under imports.
 */
const VENDORED_FILES = [
    "controls/OrbitControls.js",
    "loaders/GLTFLoader.js",
    "utils/BufferGeometryUtils.js",
    "utils/SkeletonUtils.js"
];

/**
 * Matches the specifier of a static `import ... from '...'` / `export ... from '...'` clause.
 * Anchored to the start of a line, which is what keeps it off the two things in three.js's own
 * source that read like imports and are not: the `@three_import` line every addon carries in its
 * JSDoc header, and a `from "..."` inside a template literal. Both are indented, so neither can
 * start a line with `import` or `export`. `[^;]` rather than `.` so a clause broken over sixty lines
 * - which `GLTFLoader`'s is - still matches to its `from`.
 */
const IMPORT_SPECIFIER_PATTERN = /^(?:import|export)\b[^;]*?\bfrom\s*['"]([^'"]+)['"]/gm;
/** Matches a dynamic `import('...')`, which resolves at runtime and so has to be vendored too. */
const DYNAMIC_IMPORT_PATTERN = /\bimport\s*\(\s*['"]([^'"]+)['"]\s*\)/g;

const isCheckOnly = process.argv.includes("--check");

const vendoredContentsByFile = new Map();
for (const relativePath of VENDORED_FILES) {
    vendoredContentsByFile.set(relativePath, vendorOne(relativePath));
}

verifyImportClosure(vendoredContentsByFile);

const drift = [];
for (const [relativePath, contents] of vendoredContentsByFile) {
    const targetPath = path.join(VENDOR_ROOT, relativePath);
    const existing = fs.existsSync(targetPath) ? fs.readFileSync(targetPath, "utf8") : null;
    if (existing === contents) {
        continue;
    }

    drift.push(relativePath);
    if (isCheckOnly) {
        continue;
    }

    fs.mkdirSync(path.dirname(targetPath), { recursive: true });
    fs.writeFileSync(targetPath, contents);
}

if (isCheckOnly && drift.length > 0) {
    console.error(
        `The vendored addons are out of date with node_modules/three: ${drift.join(", ")}.\n` +
        "Run `npm run vendor` and commit the result.");
    process.exit(1);
}

const verb = isCheckOnly ? "checked" : "vendored";
console.log(`Addons OK - ${VENDORED_FILES.length} files ${verb}, import closure complete.`);

/**
 * Reads one upstream addon and rewrites its bare `three` imports to the vendored bundle. Relative
 * imports are left exactly as they are, which is why the vendored tree mirrors the upstream
 * directory layout: `../utils/BufferGeometryUtils.js` then resolves without being touched, so the
 * only edit this script makes to three.js's own source is the one a browser cannot do without.
 */
function vendorOne(relativePath) {
    const source = fs.readFileSync(path.join(UPSTREAM_ROOT, relativePath), "utf8");
    const depth = relativePath.split("/").length - 1;
    const bundleSpecifier = `${"../".repeat(depth + 1)}${BUNDLE_FILE_NAME}`;
    return source.replaceAll(`from '${BARE_THREE_SPECIFIER}'`, `from '${bundleSpecifier}'`);
}

/**
 * Walks the imports of every vendored file and fails if one names something that will not be there.
 * A relative specifier has to be in the manifest, and the only bare specifier allowed is the
 * rewritten bundle - anything else would resolve against a bare-module map the browser has not got.
 */
function verifyImportClosure(contentsByFile) {
    const missing = [];
    for (const [relativePath, contents] of contentsByFile) {
        for (const specifier of readImportSpecifiers(contents)) {
            if (!specifier.startsWith(".")) {
                missing.push(`${relativePath} imports the bare specifier '${specifier}', which no browser can resolve`);
                continue;
            }

            const resolved = path.posix.normalize(path.posix.join(path.posix.dirname(relativePath), specifier));
            if (resolved === `../${BUNDLE_FILE_NAME}` || contentsByFile.has(resolved)) {
                continue;
            }

            missing.push(`${relativePath} imports '${specifier}', which resolves to '${resolved}' and is not vendored`);
        }
    }

    if (missing.length === 0) {
        return;
    }

    console.error(
        "The vendored addon set is not closed under its own imports:\n  " + missing.join("\n  ") + "\n" +
        `Add the missing files to VENDORED_FILES in ${path.relative(REPO_ROOT, url.fileURLToPath(import.meta.url))}.`);
    process.exit(1);
}

/** Every module specifier one file imports, static and dynamic alike. */
function readImportSpecifiers(contents) {
    const specifiers = [];
    for (const pattern of [IMPORT_SPECIFIER_PATTERN, DYNAMIC_IMPORT_PATTERN]) {
        pattern.lastIndex = 0;
        let match = pattern.exec(contents);
        while (match !== null) {
            specifiers.push(match[1]);
            match = pattern.exec(contents);
        }
    }

    return specifiers;
}
