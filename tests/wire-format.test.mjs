// JavaScript half of the C#<->JS wire-contract test. Run from the repository root:
//
//     node tests/wire-format.test.mjs
//
// No npm install is needed: this drives the three.js bundle vendored into wwwroot, which is the one
// that actually ships, rather than the copy in node_modules.
//
// The fixture it reads is the same file ThreeWireFormatTests asserts the C# serializer produces, so
// a change to either side that the other does not follow fails here. applyOp is driven directly
// instead of applyBatch: applyBatch swallows an unknown context id and returns [], which would make
// every assertion below pass without a single op being applied.

import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { applyOp } from '../src/Blazor.ThreeJS/wwwroot/three-interop.js';

const ops = JSON.parse(readFileSync(new URL('./wire-format-fixture.json', import.meta.url), 'utf8'));
const context = { objects: new Map() };

for (const op of ops) {
    applyOp(context, op);
}

const geometry = context.objects.get(1);
const material = context.objects.get(2);
const mesh = context.objects.get(3);
const scene = context.objects.get(4);

assert.equal(geometry, undefined, 'the Dispose op should have removed handle 1 from the object table');
assert.ok(material.isMeshStandardMaterial, 'handle 2 should be a MeshStandardMaterial');
assert.ok(mesh.isMesh, 'handle 3 should be a Mesh');
assert.ok(scene.isScene, 'handle 4 should be a Scene');

// A $ref must rebind to the referenced instance, never deep-copy it.
assert.equal(mesh.material, material, 'the $ref constructor arg should resolve to the same material instance');
assert.equal(mesh.geometry.type, 'BoxGeometry', 'the $ref constructor arg should resolve to the created geometry');

assert.deepEqual(mesh.position.toArray(), [1, 2, 3], 'the $t Vector3 value should decode into position');
assert.equal(mesh.rotation.order, 'YXZ', 'the Euler order byte 1 should decode to YXZ');
assert.ok(mesh.quaternion.isQuaternion, 'the $t Quaternion value should decode into quaternion');
// The fixture writes the matrix after the lookAt Call deliberately: three.js recomputes
// Object3D.matrix from position/quaternion/scale inside lookAt, which would overwrite an earlier
// explicit matrix write and make this assertion test three.js rather than the decoder.
assert.deepEqual(
    Array.from(mesh.matrix.elements),
    [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1],
    'the $t Matrix4 value should decode column-major, untransposed');
assert.equal(mesh.visible, true, 'a primitive Set value should assign directly');

assert.equal(material.color.r, 1, 'the $t Color value should decode into the material colour');
assert.equal(material.color.g, 0, 'the $t Color value should decode into the material colour');
assert.equal(material.color.b, 0, 'the $t Color value should decode into the material colour');
assert.equal(material.map, null, 'an explicit null Set value should assign null, not be skipped');

// The preceding Set wrote the identity quaternion, so any deviation from it proves the Call op
// actually reached Mesh.lookAt rather than being dropped.
assert.notDeepEqual(mesh.quaternion.toArray(), [0, 0, 0, 1], 'the lookAt Call should have reoriented the mesh');

assert.equal(mesh.parent, null, 'the Add op followed by the Remove op should leave the mesh detached');
assert.equal(scene.children.length, 0, 'the Remove op should have detached the mesh from the scene');

// The failure mode this whole test exists for: if the C# side ever serializes the op kind as a
// string - a JsonStringEnumConverter reaching the interop options is all it takes - every op lands
// in the applier's default arm. Pin that it is loud rather than silent.
assert.throws(
    () => applyOp(context, { k: 'Set', h: 3, m: 'visible', v: false }),
    /Unknown op kind/,
    'a string op kind must throw, not be silently applied');
assert.throws(
    () => applyOp(context, { k: 99, h: 3 }),
    /Unknown op kind/,
    'an unrecognised op kind must throw');

console.log(`Wire contract OK - ${ops.length} ops applied against the vendored three.js.`);
