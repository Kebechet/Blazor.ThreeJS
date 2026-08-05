// JavaScript half of the C#<->JS wire-contract test. Run from the repository root:
//
//     node tests/wire-format.test.mjs
//
// No npm install is needed: this drives the three.js bundle vendored into wwwroot, which is the one
// that actually ships, rather than the copy in node_modules.
//
// The fixture it reads is the same file ThreeWireFormatTests asserts the C# serializer produces, so
// a change to either side that the other does not follow fails here. applyOp and runOps are driven
// directly instead of applyBatch: applyBatch swallows an unknown context id and returns an empty
// response, which would make every assertion below pass without a single op being applied.
//
// It then drives the read op end to end — the only op that hands a value back — and closes with the
// other contract this bundle settles: that every generated class is a name the bundle actually
// exports, which is what the README's coverage headline claims about them.

import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { applyOp, runOps } from '../src/Blazor.ThreeJS/wwwroot/three-interop.js';
import * as THREE from '../src/Blazor.ThreeJS/wwwroot/three.module.js';

const { DoubleSide } = THREE;

// Kept in step with ThreeOpKind. Only the kinds this file names are spelled out.
const OP_CREATE = 0;
const OP_SET = 1;
const OP_READ = 6;

const ops = JSON.parse(readFileSync(new URL('./wire-format-fixture.json', import.meta.url), 'utf8'));
const context = { objects: new Map() };

// The material-reassignment ops (a fresh Create plus a $ref Set) are appended after the original
// fixture sequence. The batch is applied in two passes so the first pass can pin the
// constructor-time $ref still resolving to the original material, before the second pass rebinds
// it — applying everything in one loop would make that constructor-time assertion false by the
// time it runs.
//
// The fixture's read op is held out of both passes: applyOp returns its value rather than mutating
// anything, so only runOps turns it into the result row the C# side actually consumes.
const reassignmentStartIndex = ops.findIndex(op => op.h === 5);
const opsBeforeReassignment = ops.slice(0, reassignmentStartIndex);
const opsFromReassignment = ops.slice(reassignmentStartIndex).filter(op => op.k !== OP_READ);
const fixtureReadOps = ops.filter(op => op.k === OP_READ);

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

// ---------------------------------------------------------------------------------------------
// The read op, end to end against the vendored three.js. Everything above proves an instruction
// reached the browser; this proves a *value* came back from it.
// ---------------------------------------------------------------------------------------------

// The fixture's own read op first, so the shape C# serializes is the shape that actually runs.
// Handle 6 is the camera whose fov was left to the $undef sentinel, so 18.76… is three.js's own
// computation from its own 50° default and the aspect of 2 the fixture supplied — a number nothing
// in C# could have produced.
const fixtureReadResponse = runOps(context, fixtureReadOps);
assert.deepEqual(fixtureReadResponse.e, [], 'the fixture read should not have been rejected');
assert.equal(fixtureReadResponse.r.length, 1, 'one read op should produce exactly one result row');
assert.equal(fixtureReadResponse.r[0].i, 1, 'the result row should echo the request id back');
assert.ok(
    Math.abs(fixtureReadResponse.r[0].v - context.objects.get(6).getFocalLength()) < 1e-9,
    'the read should return the focal length three.js computes, not a placeholder');
assert.ok(Math.abs(fixtureReadResponse.r[0].v - 18.76443555445864) < 1e-9,
    'the focal length of a default 50° camera at aspect 2 should be 18.76…');

// Ordering: a Set and a Read in one batch, on the same handle. The read must observe the write,
// because the applier runs the batch in order — this is what makes a read unable to see stale
// state without any flush discipline on the C# side.
const focalLengthBefore = context.objects.get(6).getFocalLength();
const orderedResponse = runOps(context, [
    { k: OP_SET, h: 6, m: 'fov', v: 90 },
    { k: OP_READ, h: 6, m: 'getFocalLength', a: [], i: 7 }
]);

assert.equal(orderedResponse.r.length, 1, 'the ordered batch should produce one result row');
assert.notEqual(orderedResponse.r[0].v, focalLengthBefore,
    'a read behind a Set in the same batch must observe the Set, not the value from before it');
assert.ok(Math.abs(orderedResponse.r[0].v - 8.75) < 1e-9,
    'the focal length should be the one three.js computes for the fov written earlier in the same batch');

// Correlation: two reads in one batch come back as two rows, each carrying the id of the request it
// answers. Matching by id rather than by position is what keeps several in-flight reads apart.
const correlatedResponse = runOps(context, [
    { k: OP_READ, h: 6, m: 'getEffectiveFOV', a: [], i: 11 },
    { k: OP_READ, h: 6, m: 'getFilmWidth', a: [], i: 12 }
]);

assert.deepEqual(correlatedResponse.r.map(row => row.i), [11, 12], 'each result row should carry its own request id');
assert.equal(correlatedResponse.r.find(row => row.i === 11).v, 90, 'request 11 should answer with the effective fov');
assert.equal(correlatedResponse.r.find(row => row.i === 12).v, 35, 'request 12 should answer with the film width');

// A math value read back, tagged exactly as C# sends one in the other direction. getVertexPosition
// reads a real vertex out of the BoxGeometry's buffer, which C# holds nothing of.
const mathReadResponse = runOps(context, [
    { k: OP_CREATE, h: 20, t: 'BoxGeometry', a: [2, 2, 2] },
    { k: OP_CREATE, h: 21, t: 'MeshStandardMaterial', a: [] },
    { k: OP_CREATE, h: 22, t: 'Mesh', a: [{ $ref: 20 }, { $ref: 21 }] },
    { k: OP_READ, h: 22, m: 'getVertexPosition', a: [0, { $t: 'Vector3', v: [0, 0, 0] }], i: 21 },
    { k: OP_CREATE, h: 23, t: 'InstancedMesh', a: [{ $ref: 20 }, { $ref: 21 }, 4] },
    { k: OP_READ, h: 23, m: 'getMatrixAt', a: [0, { $t: 'Matrix4', v: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0] }], i: 22 }
]);

assert.deepEqual(mathReadResponse.e, [], 'the math read batch should not have been rejected');
const vertexPosition = mathReadResponse.r.find(row => row.i === 21).v;
assert.equal(vertexPosition.$t, 'Vector3', 'a Vector3 return should come back under the Vector3 tag');
assert.deepEqual(vertexPosition.v, [1, 1, 1], 'the first vertex of a 2x2x2 box is (1, 1, 1)');

const instanceMatrix = mathReadResponse.r.find(row => row.i === 22).v;
assert.equal(instanceMatrix.$t, 'Matrix4', 'a Matrix4 return should come back under the Matrix4 tag');
assert.deepEqual(
    instanceMatrix.v,
    [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1],
    'the matrix should come back column-major and untransposed, the same order C# sends one in');

// A failed read reports on its own result row and stays out of the error list, because the C# side
// faults the one task awaiting it rather than announcing it to every OnError subscriber.
const failedReadResponse = runOps(context, [
    { k: OP_SET, h: 6, m: 'fov', v: 45 },
    { k: OP_READ, h: 999, m: 'getFocalLength', a: [], i: 31 },
    { k: OP_READ, h: 6, m: 'notAMethod', a: [], i: 32 }
]);

assert.deepEqual(failedReadResponse.e, [], 'a failed read must not be reported on the batch error channel');
assert.equal(failedReadResponse.r.length, 2, 'a failed read still produces a result row');
assert.match(failedReadResponse.r.find(row => row.i === 31).e, /Unknown handle/, 'an unknown handle should be named on the result row');
assert.match(failedReadResponse.r.find(row => row.i === 32).e, /not a method/, 'a missing method should be named on the result row');
assert.equal(failedReadResponse.r.find(row => row.i === 31).v, undefined, 'a failed read must not also carry a value');

// The failure mode the encoder exists to prevent: a method returning a three.js object has no wire
// encoding, and serializing its public shape would hand C# a plausible-looking bag of numbers.
// `clone()` is the exact case the member classifier refuses for this reason, so pin that the applier
// refuses it too rather than relying on the generator never emitting one.
const unencodableReadResponse = runOps(context, [{ k: OP_READ, h: 6, m: 'clone', a: [], i: 41 }]);
assert.match(
    unencodableReadResponse.r[0].e,
    /no wire encoding/,
    'a read returning a three.js object must be refused, not serialized as a plain object');

// The floor under the README's headline. `generator/api-coverage.json` is what that number is
// rendered from, and what it claims is that each of those classes is a class a consumer can create -
// which nothing upstream of here checks against the bundle that will actually be asked for it.
// `three-interop.js` resolves a Create op as `THREE[op.t]`, so a name the bundle does not export
// throws `Unknown three.js type` at runtime with a green build and a passing emit:check behind it.
// The 46 classes that shipped in exactly that state were the reason this assertion exists.
const coverage = JSON.parse(readFileSync(new URL('../generator/api-coverage.json', import.meta.url), 'utf8'));
const generatedClassNames = coverage.classes.filter(entry => entry.status === 'emittable').map(entry => entry.name);
assert.ok(generatedClassNames.length > 0, 'api-coverage.json should list emittable classes');
const unconstructible = generatedClassNames.filter(name => typeof THREE[name] !== 'function');
assert.deepEqual(
    unconstructible,
    [],
    'every generated class must be a constructor on the vendored three.js namespace');

console.log(`Wire contract OK - ${ops.length} ops applied against the vendored three.js.`);
console.log('Read op OK - values, tagged math values, correlation, ordering and refusals round-tripped.');
console.log(`Generated surface OK - ${generatedClassNames.length} generated classes are constructors on the vendored three.js.`);
