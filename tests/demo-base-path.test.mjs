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
// Run with: node tests/demo-base-path.test.mjs
import assert from 'node:assert/strict';
import test from 'node:test';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const repositoryRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const demoWebRoot = path.join(repositoryRoot, 'demo', 'wwwroot');

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

/** Every scannable file under the demo's web root, recursively. */
function collectFiles(directory) {
	const found = [];

	for (const entry of fs.readdirSync(directory, { withFileTypes: true })) {
		const entryPath = path.join(directory, entry.name);

		if (entry.isDirectory()) {
			if (!skippedDirectories.has(entry.name)) {
				found.push(...collectFiles(entryPath));
			}

			continue;
		}

		if (scannedExtensions.has(path.extname(entry.name))) {
			found.push(entryPath);
		}
	}

	return found;
}

test('every demo asset addresses the app relatively, so it survives a sub-path deploy', () => {
	const failures = [];

	for (const file of collectFiles(demoWebRoot)) {
		const contents = fs.readFileSync(file, 'utf8');
		const relativePath = path.relative(repositoryRoot, file).replace(/\\/g, '/');

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
