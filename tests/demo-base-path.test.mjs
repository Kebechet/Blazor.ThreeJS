// The demo is served from a sub-path on GitHub Pages - https://kebechet.github.io/Blazor.ThreeJS/ -
// while `dotnet run` serves it from the root. Every asset it fetches at runtime therefore has to be
// addressed relatively: a root-absolute URL resolves to the right file locally and to a 404 in
// production, which is invisible to every other test in this repository because they all run against
// the root.
//
// This is not hypothetical. `js/tsl-shaders.js` imported the three.js TSL bundle as
// `/_content/Kebechet.Blazor.ThreeJS/three.tsl.min.js`; the Shaders story worked locally, passed CI,
// and on the deployed site failed its dynamic import, left the scene unbuilt and every one of its
// buttons a silent no-op.
//
// ⚠️ Relative is necessary and not sufficient. `../_content/…` and `../../…` are both relative, and
// which one is right depends on how deep the *importing* file is served from - something that changed
// the day the stories moved into their own library and their assets moved to `_content/`. So the
// second test here resolves each relative specifier the way a browser would and insists it lands on a
// file that exists.
//
// Run with: node tests/demo-base-path.test.mjs
import assert from 'node:assert/strict';
import test from 'node:test';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const repositoryRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');

/**
 * Every web root the deployed site is assembled from, with the URL prefix the file is served under.
 * A Razor class library's `wwwroot` is published beneath `_content/<assembly name>/`, so a file's
 * depth on disk is not its depth in the URL space - and the depth in the URL space is what a
 * relative specifier is resolved against.
 */
const webRoots = [
	{ directory: path.join(repositoryRoot, 'demo', 'Blazor.ThreeJS.Demo', 'wwwroot'), urlPrefix: '' },
	{
		directory: path.join(repositoryRoot, 'demo', 'Blazor.ThreeJS.Stories', 'wwwroot'),
		urlPrefix: '_content/Blazor.ThreeJS.Stories/'
	},
	{
		directory: path.join(repositoryRoot, 'src', 'Blazor.ThreeJS', 'wwwroot'),
		urlPrefix: '_content/Kebechet.Blazor.ThreeJS/'
	}
];

/** Files the app's own code fetches at runtime, as opposed to build output under _framework. */
const scannedExtensions = new Set(['.js', '.mjs', '.html', '.css']);

/** Build output: written by `dotnet publish`, rewritten by the deploy workflow, not ours to lint. */
const skippedDirectories = new Set(['_framework', '_content']);

/**
 * A root-absolute URL in a place the browser resolves against the origin rather than the app base.
 * `<base href="/">` is deliberately excluded: the deploy workflow rewrites that one line, and it is
 * the mechanism that makes every *relative* URL correct under the sub-path.
 */
const offenders = [
	{ what: 'a static import', pattern: /\bfrom\s*['"](\/(?!\/)[^'"]*)['"]/g },
	{ what: 'a dynamic import', pattern: /\bimport\s*\(\s*['"](\/(?!\/)[^'"]*)['"]/g },
	{ what: 'a new URL(...)', pattern: /\bnew\s+URL\s*\(\s*['"](\/(?!\/)[^'"]*)['"]/g },
	{ what: 'a fetch(...)', pattern: /\bfetch\s*\(\s*['"](\/(?!\/)[^'"]*)['"]/g },
	{ what: 'a src attribute', pattern: /\bsrc\s*=\s*"(\/(?!\/)[^"]*)"/g },
	{ what: 'a stylesheet href', pattern: /<link\b[^>]*\bhref\s*=\s*"(\/(?!\/)[^"]*)"/g },
];

/** Specifiers that reach another file by walking up from this one, which is where a wrong depth hides. */
const relativeImports = [
	{ what: 'a static import', pattern: /\bfrom\s*['"](\.[^'"]*)['"]/g },
	{ what: 'a dynamic import', pattern: /\bimport\s*\(\s*['"](\.[^'"]*)['"]/g },
];

/** Every scannable file under one web root, recursively, with the URL it is served under. */
function collectFiles({ directory, urlPrefix }) {
	const found = [];
	if (!fs.existsSync(directory)) {
		return found;
	}

	const walk = (current, urlDirectory) => {
		for (const entry of fs.readdirSync(current, { withFileTypes: true })) {
			const entryPath = path.join(current, entry.name);

			if (entry.isDirectory()) {
				if (!skippedDirectories.has(entry.name)) {
					walk(entryPath, urlDirectory + entry.name + '/');
				}

				continue;
			}

			if (scannedExtensions.has(path.extname(entry.name))) {
				found.push({ entryPath, url: urlDirectory + entry.name });
			}
		}
	};

	walk(directory, urlPrefix);
	return found;
}

const allFiles = webRoots.flatMap(collectFiles);

/** Where a served URL comes from on disk, or null when nothing publishes it. */
function resolveServedUrl(url) {
	for (const { directory, urlPrefix } of webRoots) {
		if (!url.startsWith(urlPrefix)) {
			continue;
		}

		const candidate = path.join(directory, url.slice(urlPrefix.length));
		if (fs.existsSync(candidate)) {
			return candidate;
		}
	}

	return null;
}

test('every demo asset addresses the app relatively, so it survives a sub-path deploy', () => {
	const failures = [];

	for (const { entryPath } of allFiles) {
		const contents = fs.readFileSync(entryPath, 'utf8');
		const relativePath = path.relative(repositoryRoot, entryPath).replace(/\\/g, '/');

		for (const { what, pattern } of offenders) {
			for (const match of contents.matchAll(pattern)) {
				const lineNumber = contents.slice(0, match.index).split('\n').length;
				failures.push(`${relativePath}:${lineNumber} - ${what} of '${match[1]}'`);
			}
		}
	}

	assert.deepEqual(
		failures,
		[],
		'A root-absolute URL resolves against the origin, not the app base, so it 404s wherever the ' +
			'demo is served from a sub-path. Address it relatively instead:\n  ' + failures.join('\n  '));
});

test('every relative import resolves to a file that is actually published there', () => {
	const failures = [];

	for (const { entryPath, url } of allFiles) {
		const contents = fs.readFileSync(entryPath, 'utf8');
		const relativePath = path.relative(repositoryRoot, entryPath).replace(/\\/g, '/');

		for (const { what, pattern } of relativeImports) {
			for (const match of contents.matchAll(pattern)) {
				// Resolved against the URL the importing file is served under, which is what the browser
				// does - not against where it sits in the repository, which is a different depth.
				const resolved = new URL(match[1], new URL(url, 'https://host/')).pathname.replace(/^\//, '');
				if (resolveServedUrl(resolved) !== null) {
					continue;
				}

				const lineNumber = contents.slice(0, match.index).split('\n').length;
				failures.push(
					`${relativePath}:${lineNumber} - ${what} of '${match[1]}' resolves to '${resolved}', which nothing publishes`);
			}
		}
	}

	assert.deepEqual(
		failures,
		[],
		'A relative specifier with the wrong number of leading `..` is still relative, so the first ' +
			'test passes and the deployed site 404s. Count the segments against the URL the importing ' +
			'file is served under:\n  ' + failures.join('\n  '));
});
