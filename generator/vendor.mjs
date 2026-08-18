#!/usr/bin/env node
import fs from "node:fs";
import path from "node:path";
import url from "node:url";

/**
 * Vendors everything this package ships out of `node_modules/three` into
 * `src/Blazor.ThreeJS/wwwroot/`, and proves the copy is complete.
 *
 * Two kinds of file go in. The bundle is three.js itself, which every consumer loads; the addons live
 * in `examples/jsm`, outside that bundle, so each one has to be copied in as its own static asset.
 *
 * Copying only an entry point is the failure this script exists to prevent: an ES module that imports
 * a sibling it cannot resolve builds green, passes every C# test, and fails in the browser with
 * `ERR_MODULE_NOT_FOUND` the first time a consumer actually renders. So every file's imports are read
 * after it is copied, and a relative import that names a file outside the manifest fails the run.
 *
 *     node generator/vendor.mjs           rewrite the vendored copies
 *     node generator/vendor.mjs --check   fail if they differ from upstream
 */

const HERE = path.dirname(url.fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(HERE, "..");
const UPSTREAM_ROOT = path.join(REPO_ROOT, "node_modules", "three");
const VENDOR_ROOT = path.join(REPO_ROOT, "src", "Blazor.ThreeJS", "wwwroot");
/** The vendored three.js bundle every addon's bare `three` import is rewritten to point at. */
const BUNDLE_FILE_NAME = "three.webgpu.min.js";
/**
 * Matches the bare specifiers upstream imports three.js under, none of which a browser can resolve
 * unaided. Both name the same vendored bundle: the addons ask for `three`, and `three.tsl.min.js`
 * asks for `three/webgpu`, which is upstream's own name for the build this package already ships.
 * The quote style is captured and replayed because the addons are readable source using single
 * quotes and the TSL bundle is minified using double.
 */
const BARE_THREE_SPECIFIER_PATTERN = /\bfrom\s*(['"])(?:three|three\/webgpu)\1/g;
/** The hand-written interop module, checked to import the same bundle this script vendors. */
const INTEROP_FILE_NAME = "three-interop.js";

/**
 * Every file that ships, as a path under `wwwroot` mapped to its path under `node_modules/three`.
 *
 * The minified builds are deliberate: they are half the transfer of the readable ones over the wire
 * and a third of the bytes to parse, three.js publishes no source maps for either, and the code a
 * consumer actually debugs is `three-interop.js` — which is ours, is small, and stays readable.
 * `three.webgpu.min.js` imports `three.core.min.js` by name, so the pair travels together.
 *
 * The WebGPU build rather than the classic one. It carries the whole classic surface — `Mesh`,
 * `MeshStandardMaterial`, `AnimationMixer` and the rest — plus the node material system, and swaps
 * `WebGLRenderer` for `WebGPURenderer`. That renderer runs on a WebGPU backend where the browser has
 * one and falls back to a WebGL2 backend where it does not, so nothing stops rendering; what changes
 * is that materials become node graphs and compute shaders become reachable. It costs about 93 KB
 * more over the wire once compressed, which is the whole price.
 *
 * `three.tsl.min.js` is the shader-authoring half of that node system, and it ships because nothing
 * else can reach it: its 638 exports are free functions (`vec3`, `positionLocal`, `uniform`, `Fn`)
 * registered onto node prototypes at runtime by `addMethodChaining`, so they are absent from the
 * WebGPU bundle's exports and undescribable as C# members. Consumers write TSL in a small JavaScript
 * module of their own and hand the resulting node back through `ThreeContext.LoadNodeAsync`.
 *
 * `BufferGeometryUtils` and `SkeletonUtils` are here because `GLTFLoader` imports them; `DRACOLoader`
 * and `KTX2Loader` and everything below them are here because they decode compressed glTF geometry
 * and textures when a caller opts in (`GLTFLoadOptions`) — none of which is a choice this script
 * makes: `verifyImportClosure` re-derives the closure on every run and fails if this list stops being
 * closed under imports. The `.wasm` decoder payloads carry no imports of their own; `verifyImportClosure`
 * skips them for that reason, not because they are exempt from being vendored.
 *
 * `draco_decoder.js` — DRACOLoader's ~500 KB pure-JavaScript fallback decoder — is the one deliberate
 * exception: it is not vendored at all. DRACOLoader only reaches for it when `WebAssembly` is not an
 * `object`, and this package sets no `decoderConfig.type` that would force JS decoding on a browser
 * that does have WebAssembly; every browser target this package supports has had WebAssembly for
 * years. Vendoring it would ship 500 KB neither this package nor a supported browser can ever fetch.
 * `draco_wasm_wrapper.js` and `draco_decoder.wasm`, the path every real load takes, still ship below.
 */
const VENDORED_FILES = new Map([
    ["three.webgpu.min.js", "build/three.webgpu.min.js"],
    ["three.core.min.js", "build/three.core.min.js"],
    ["three.tsl.min.js", "build/three.tsl.min.js"],
    ["addons/controls/OrbitControls.js", "examples/jsm/controls/OrbitControls.js"],
    ["addons/loaders/GLTFLoader.js", "examples/jsm/loaders/GLTFLoader.js"],
    ["addons/utils/BufferGeometryUtils.js", "examples/jsm/utils/BufferGeometryUtils.js"],
    ["addons/utils/SkeletonUtils.js", "examples/jsm/utils/SkeletonUtils.js"],
    ["addons/loaders/DRACOLoader.js", "examples/jsm/loaders/DRACOLoader.js"],
    ["addons/loaders/KTX2Loader.js", "examples/jsm/loaders/KTX2Loader.js"],
    ["addons/utils/WorkerPool.js", "examples/jsm/utils/WorkerPool.js"],
    ["addons/libs/ktx-parse.module.js", "examples/jsm/libs/ktx-parse.module.js"],
    ["addons/libs/zstddec.module.js", "examples/jsm/libs/zstddec.module.js"],
    ["addons/math/ColorSpaces.js", "examples/jsm/math/ColorSpaces.js"],
    ["addons/libs/draco/gltf/draco_decoder.wasm", "examples/jsm/libs/draco/gltf/draco_decoder.wasm"],
    ["addons/libs/draco/gltf/draco_wasm_wrapper.js", "examples/jsm/libs/draco/gltf/draco_wasm_wrapper.js"],
    ["addons/libs/basis/basis_transcoder.js", "examples/jsm/libs/basis/basis_transcoder.js"],
    ["addons/libs/basis/basis_transcoder.wasm", "examples/jsm/libs/basis/basis_transcoder.wasm"]
]);

/**
 * Vendored files that used to ship and must not be left behind. A stale bundle in `wwwroot` is served
 * as a static asset whether or not anything imports it, so it would go on padding every consumer's
 * publish output with megabytes nothing loads.
 */
const RETIRED_FILES = [
    "three.module.js",
    "three.core.js",
    "three.module.min.js",
    "addons/libs/draco/gltf/draco_decoder.js"
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
for (const [vendoredPath, upstreamPath] of VENDORED_FILES) {
    vendoredContentsByFile.set(vendoredPath, vendorOne(vendoredPath, upstreamPath));
}

verifyImportClosure(vendoredContentsByFile);
verifyInteropImportsTheBundle();

const drift = [];
for (const [vendoredPath, contents] of vendoredContentsByFile) {
    const targetPath = path.join(VENDOR_ROOT, vendoredPath);
    const isBinary = Buffer.isBuffer(contents);
    const existing = fs.existsSync(targetPath)
        ? fs.readFileSync(targetPath, isBinary ? undefined : "utf8")
        : null;
    const unchanged = isBinary ? (Buffer.isBuffer(existing) && existing.equals(contents)) : existing === contents;
    if (unchanged) {
        continue;
    }

    drift.push(vendoredPath);
    if (isCheckOnly) {
        continue;
    }

    fs.mkdirSync(path.dirname(targetPath), { recursive: true });
    fs.writeFileSync(targetPath, contents);
}

for (const retiredFile of RETIRED_FILES) {
    const retiredPath = path.join(VENDOR_ROOT, retiredFile);
    if (!fs.existsSync(retiredPath)) {
        continue;
    }

    drift.push(`${retiredFile} (retired, should be deleted)`);
    if (!isCheckOnly) {
        fs.rmSync(retiredPath);
    }
}

if (isCheckOnly && drift.length > 0) {
    console.error(
        `The vendored three.js files are out of date with node_modules/three: ${drift.join(", ")}.\n` +
        "Run `npm run vendor` and commit the result.");
    process.exit(1);
}

const verb = isCheckOnly ? "checked" : "vendored";
console.log(`three.js OK - ${VENDORED_FILES.size} files ${verb}, import closure complete.`);

/**
 * Reads one upstream file and rewrites its bare `three` imports to the vendored bundle. Relative
 * imports are left exactly as they are, which is why the vendored tree mirrors the upstream
 * directory layout: `../utils/BufferGeometryUtils.js` then resolves without being touched, so the
 * only edit this script makes to three.js's own source is the one a browser cannot do without.
 *
 * `.wasm` files are read and returned as a raw `Buffer` instead: they are not text, carry no import
 * specifiers to rewrite, and decoding/re-encoding them as utf8 corrupts the bytes a WASM runtime
 * instantiates.
 */
function vendorOne(vendoredPath, upstreamPath) {
    const upstreamFullPath = path.join(UPSTREAM_ROOT, upstreamPath);
    if (vendoredPath.endsWith(".wasm")) {
        return fs.readFileSync(upstreamFullPath);
    }

    const source = fs.readFileSync(upstreamFullPath, "utf8");
    const depth = vendoredPath.split("/").length - 1;
    const bundleSpecifier = `${"../".repeat(depth) || "./"}${BUNDLE_FILE_NAME}`;
    return source.replace(BARE_THREE_SPECIFIER_PATTERN, (_, quote) => `from ${quote}${bundleSpecifier}${quote}`);
}

/**
 * Walks the imports of every vendored file and fails if one names something that will not be there.
 * A relative specifier has to resolve to another vendored file - anything bare would resolve against
 * a bare-module map the browser has not got.
 */
function verifyImportClosure(contentsByFile) {
    const missing = [];
    for (const [vendoredPath, contents] of contentsByFile) {
        if (Buffer.isBuffer(contents)) {
            continue;
        }

        for (const specifier of readImportSpecifiers(contents)) {
            if (!specifier.startsWith(".")) {
                missing.push(`${vendoredPath} imports the bare specifier '${specifier}', which no browser can resolve`);
                continue;
            }

            const resolved = path.posix.normalize(path.posix.join(path.posix.dirname(vendoredPath), specifier));
            if (contentsByFile.has(resolved)) {
                continue;
            }

            missing.push(`${vendoredPath} imports '${specifier}', which resolves to '${resolved}' and is not vendored`);
        }
    }

    if (missing.length === 0) {
        return;
    }

    console.error(
        "The vendored file set is not closed under its own imports:\n  " + missing.join("\n  ") + "\n" +
        `Add the missing files to VENDORED_FILES in ${path.relative(REPO_ROOT, url.fileURLToPath(import.meta.url))}.`);
    process.exit(1);
}

/**
 * Fails if the hand-written interop module imports a bundle this script does not vendor. Nothing else
 * would catch it: swapping which build ships is one edit here and one there, and a mismatch leaves a
 * green build that 404s on the module the moment a canvas mounts.
 */
function verifyInteropImportsTheBundle() {
    const interopPath = path.join(VENDOR_ROOT, INTEROP_FILE_NAME);
    const contents = fs.readFileSync(interopPath, "utf8");
    const specifiers = readImportSpecifiers(contents);
    if (specifiers.includes(`./${BUNDLE_FILE_NAME}`)) {
        return;
    }

    console.error(
        `${INTEROP_FILE_NAME} does not import './${BUNDLE_FILE_NAME}', which is the bundle this script vendors.\n` +
        `It imports: ${specifiers.join(", ")}`);
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
