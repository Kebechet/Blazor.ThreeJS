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
import { DoubleSide } from '../src/Blazor.ThreeJS/wwwroot/three.module.js';

const ops = JSON.parse(readFileSync(new URL('./wire-format-fixture.json', import.meta.url), 'utf8'));
const context = { objects: new Map() };

// The material-reassignment ops (a fresh Create plus a $ref Set) are appended after the original
// fixture sequence. The batch is applied in two passes so the first pass can pin the
// constructor-time $ref still resolving to the original material, before the second pass rebinds
// it — applying everything in one loop would make that constructor-time assertion false by the
// time it runs.
const reassignmentStartIndex = ops.findIndex(op => op.h === 5);
const opsBeforeReassignment = ops.slice(0, reassignmentStartIndex);
const opsFromReassignment = ops.slice(reassignmentStartIndex);

for (const op of opsBeforeReassignment) {
    applyOp(context, op);
}

const material = context.objects.get(2);
const mesh = context.objects.get(3);
const scene = context.objects.get(4);

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

for (const op of opsFromReassignment) {
    applyOp(context, op);
}

const geometry = context.objects.get(1);
const reassignedMaterial = context.objects.get(5);

assert.equal(geometry, undefined, 'the Dispose op should have removed handle 1 from the object table');
assert.ok(reassignedMaterial.isMeshStandardMaterial, 'handle 5 should be a freshly created MeshStandardMaterial');

// The $ref Set must rebind mesh.material to the new instance, never deep-copy it, and must not
// disturb the material the mesh no longer references.
assert.equal(mesh.material, reassignedMaterial, 'the $ref Set value should rebind mesh.material to the newly created material');
assert.notEqual(mesh.material, material, 'mesh.material should no longer be the original constructor-time material');

// An enum Set must decode as the plain number three.js expects for Material.side, not a string.
assert.equal(material.side, DoubleSide, 'the enum Set value should decode into material.side as THREE.DoubleSide');

// The $undef sentinel, end to end. THREE.PerspectiveCamera(fov = 50, aspect = 1, near = 0.1,
// far = 2000): the fixture supplies the sentinel for fov and near, and real values for aspect and
// far. A parameter default only fires for undefined, so fov landing on 50 and near on 0.1 is proof
// that the sentinel arrived as a genuine undefined and not as null, in a *middle* position that
// trimming the argument list could never have reached.
const cameraWithUnspecifiedArguments = context.objects.get(6);
assert.equal(cameraWithUnspecifiedArguments.fov, 50, 'the $undef sentinel in argument 0 should let three.js apply its own fov default');
assert.equal(cameraWithUnspecifiedArguments.near, 0.1, 'the $undef sentinel in argument 2 should let three.js apply its own near default');
assert.equal(cameraWithUnspecifiedArguments.aspect, 2, 'the supplied aspect should still land in its own position');
assert.equal(cameraWithUnspecifiedArguments.far, 1000, 'the supplied far should still land after two sentinels');

// The control. The same constructor given JSON null for fov keeps the null, which is exactly the
// bug the sentinel exists to fix - and is what makes the four assertions above mean something.
const cameraWithNullArgument = context.objects.get(7);
assert.equal(cameraWithNullArgument.fov, null, 'a JSON null must NOT trigger the upstream default, or the sentinel would be redundant');
assert.equal(cameraWithNullArgument.far, 2000, 'omitting a trailing argument entirely should still apply the upstream default');

// A tagged math value in a constructor argument, which only the generated classes produce: the
// hand-written AmbientLight this replaced converted its colour to a hex integer in C# first. Proving
// the tagged form reaches THREE.AmbientLight as a real THREE.Color is what makes that change safe.
const ambientLight = context.objects.get(8);
assert.ok(ambientLight.isAmbientLight, 'handle 8 should be an AmbientLight');
assert.equal(ambientLight.color.getHex(), 0xff0000, 'a $t-tagged Color constructor argument should reach three.js as a real THREE.Color');
assert.ok(Math.abs(ambientLight.intensity - 0.4) < 1e-6, 'the intensity supplied after the tagged colour should still land in its own position');

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
