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
// It then drives the two ops that hand a value back end to end — the method read and the property
// read — proves that a class with no generated wrapper is still constructible, mutable and readable
// by name, and closes with the two contracts this bundle settles: that every generated class is a
// name the bundle actually exports, and that every class the coverage table calls reachable is one
// too, neither more nor fewer.

import assert from 'node:assert/strict';
import { createReadStream, readFileSync } from 'node:fs';
import { createServer } from 'node:http';
import { fileURLToPath } from 'node:url';
import { Worker as NodeWorker } from 'node:worker_threads';

import {
    applyOp,
    attachOrbitControlsTo,
    createContextAround,
    detachControls,
    dispatchPointerHit,
    disposeDecoders,
    loadGltfInto,
    runOps
} from '../src/Blazor.ThreeJS/wwwroot/three-interop.js';
import * as THREE from '../src/Blazor.ThreeJS/wwwroot/three.webgpu.min.js';

// A browser global three.js's FileLoader reports download progress through, and which Node has not
// got. A gap in the test host rather than in anything this package ships: every browser it targets
// has had ProgressEvent for twenty years. Only reached when a response body is read, which is why
// assigning it after the imports is soon enough.
globalThis.ProgressEvent ??= class ProgressEvent extends Event {
    constructor(type, init = {}) {
        super(type);
        this.lengthComputable = init.lengthComputable ?? false;
        this.loaded = init.loaded ?? 0;
        this.total = init.total ?? 0;
    }
};

// Another gap in the test host, reached only by the DRACO section below. DRACOLoader resolves its
// decoder assets relative to three-interop.js's own import.meta.url, which is an http(s) URL in every
// browser that ever loads this module and a file:// one here, since this file imports three-interop.js
// straight off disk. Node's fetch refuses the file:// scheme outright rather than falling back to the
// filesystem, so the decoder path this package computes for the browser is the one this polyfill has
// to make Node honour too.
const originalFetch = globalThis.fetch;
globalThis.fetch = async (input, init) => {
    const requestUrl = typeof input === 'string' ? input : (input?.url ?? String(input));
    if (!requestUrl.startsWith('file://')) {
        return originalFetch(input, init);
    }

    const bytes = readFileSync(fileURLToPath(requestUrl));
    return new Response(bytes, { status: 200 });
};

// A third gap: DRACOLoader decodes off a Worker, which Node's global scope has none of.
// `node:worker_threads` ships a Worker too, but it is an EventEmitter that runs a file or an eval
// string, not an EventTarget that runs a blob: URL - so both DRACOLoader's own worker script and the
// way DRACOLoader talks to it need a shim. This is only the sliver DRACOLoader actually reaches:
// postMessage/onmessage at the top of the worker's own global scope (which the script, written for a
// browser, addresses through `self` - a global Node defines nowhere on its own), and
// postMessage/onmessage plus arbitrary property storage on the handle held here, which is where
// DRACOLoader keeps its own callback table.
globalThis.Worker ??= class Worker {
    constructor(sourceUrl) {
        this._ready = fetch(sourceUrl)
            .then(response => response.text())
            .then(source => {
                const bootstrap =
                    "const { parentPort } = require('node:worker_threads');" +
                    'globalThis.self = globalThis;' +
                    'globalThis.postMessage = (message, transfer) => parentPort.postMessage(message, transfer);' +
                    "Object.defineProperty(globalThis, 'onmessage', " +
                    '{ set: handler => parentPort.on("message", data => handler({ data })) });';

                this._worker = new NodeWorker(`${bootstrap}\n${source}`, { eval: true });
                // Unref'd: a decoder worker outliving the load it served must not be what keeps this
                // one-shot script's process open, the same way it costs a browser tab nothing once the
                // page has moved on.
                this._worker.unref();
                this._worker.on('message', message => this.onmessage?.({ data: message }));
                this._worker.on('error', error => this.onerror?.(error));
            });
    }

    postMessage(message, transfer) {
        this._ready.then(() => this._worker.postMessage(message, transfer));
    }

    terminate() {
        this._ready.then(() => this._worker.terminate());
    }
};

const { DoubleSide } = THREE;

// Kept in step with ThreeOpKind. Only the kinds this file names are spelled out.
const OP_CREATE = 0;
const OP_SET = 1;
const OP_CALL = 2;
const OP_ADD = 3;
const OP_DISPOSE = 5;
const OP_READ = 6;
const OP_PICK = 7;
const OP_GET = 8;

// The handle every context registers its renderer under, matching ThreeWireFormat.RendererHandle. The
// browser's own allocator counts down from below it, so nothing it mints can ever take it.
const RESERVED_RENDERER_HANDLE = -1;

const ops = JSON.parse(readFileSync(new URL('./wire-format-fixture.json', import.meta.url), 'utf8'));
const context = { objects: new Map() };

// The material-reassignment ops (a fresh Create plus a $ref Set) are appended after the original
// fixture sequence. The batch is applied in two passes so the first pass can pin the
// constructor-time $ref still resolving to the original material, before the second pass rebinds
// it — applying everything in one loop would make that constructor-time assertion false by the
// time it runs.
//
// The fixture's two value-producing ops are held out of both passes: applyOp returns their value
// rather than mutating anything, so only runOps turns them into the result rows the C# side actually
// consumes.
const reassignmentStartIndex = ops.findIndex(op => op.h === 5);
const opsBeforeReassignment = ops.slice(0, reassignmentStartIndex);
const opsFromReassignment = ops.slice(reassignmentStartIndex).filter(op => op.k !== OP_READ && op.k !== OP_GET);
const fixtureReadOps = ops.filter(op => op.k === OP_READ);
const fixtureGetOps = ops.filter(op => op.k === OP_GET);

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

// The fixture's own get op, the second of the two shapes C# serializes that answer with a value. It
// names the same handle 6 and reads `fov` as a *property*, which no read op can do — that one insists
// the member is a function — so the 50 it comes back with is three.js's own default, read off the
// object rather than computed by it. Run here, before the ordering batch below writes a new fov.
const fixtureGetResponse = runOps(context, fixtureGetOps);
assert.deepEqual(fixtureGetResponse.e, [], 'the fixture get should not have been rejected');
assert.equal(fixtureGetResponse.r.length, 1, 'one get op should produce exactly one result row');
assert.equal(fixtureGetResponse.r[0].i, 2, 'the result row should echo its own request id back');
assert.equal(fixtureGetResponse.r[0].v, 50, "a get should answer with the property's own value, three.js's default fov here");

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

// ---------------------------------------------------------------------------------------------
// Tagged math values, both directions, against the real three.js.
//
// C# and JavaScript each carry their own hand-written component layout per type - ToArray/FromArray
// on one side, read/build on the other - with no compiler relationship between them. A pair that
// disagrees on component order still round-trips within each language, so neither language's own
// tests can catch it: a transposed matrix or a swapped Box3 corner would reach production silently.
//
// math-values-fixture.json is written by the C# encoder and asserted against it by
// MathValueWireFormatTests. Decoding each entry here into a real three.js instance and encoding it
// straight back is what pins the two layouts to each other.
// ---------------------------------------------------------------------------------------------

const mathValues = JSON.parse(readFileSync(new URL('./math-values-fixture.json', import.meta.url), 'utf8'));

let mathValueCount = 0;
for (const [typeName, encoded] of Object.entries(mathValues)) {
    // Through the applier's own ops rather than by calling decode/encode directly: a Set writes the
    // value onto a real three.js object and a Get reads it back, which is the path a mirrored
    // property actually takes.
    const carrier = {};
    context.objects.set(90, carrier);
    runOps(context, [{ k: OP_SET, h: 90, m: 'value', v: encoded }]);

    const instance = carrier.value;
    assert.equal(
        instance instanceof THREE[typeName],
        true,
        `a '${typeName}' tagged value must decode to a real THREE.${typeName}, not to a plain object`);

    const readBack = runOps(context, [{ k: OP_GET, h: 90, m: 'value', i: 80 }]).r[0].v;
    assert.deepEqual(
        readBack,
        encoded,
        `${typeName} components disagree between C# and JavaScript - one side's layout has drifted`);

    context.objects.delete(90);
    mathValueCount++;
}

assert.equal(mathValueCount, 20, 'every hand-written math type should be in the fixture');

// Infinity has no JSON number, and the two runtimes fail differently and quietly if left alone:
// Utf8JsonWriter throws in C#, and JSON.stringify(Infinity) yields null here. three.js seeds an empty
// Box3 at ±Infinity, so this is that type's default state rather than an edge case.
const emptyBox = { $t: 'Box3', v: ['Infinity', 'Infinity', 'Infinity', '-Infinity', '-Infinity', '-Infinity'] };
const boxCarrier = {};
context.objects.set(91, boxCarrier);
runOps(context, [{ k: OP_SET, h: 91, m: 'value', v: emptyBox }]);
assert.equal(boxCarrier.value.isEmpty(), true, 'an infinite Box3 must decode to a box three.js itself calls empty');
assert.deepEqual(
    runOps(context, [{ k: OP_GET, h: 91, m: 'value', i: 81 }]).r[0].v,
    emptyBox,
    'a non-finite component must survive being read back, rather than being flattened to null by JSON.stringify');
context.objects.delete(91);

console.log(`# Math values OK - ${mathValueCount} tagged types round-tripped through the applier, non-finite components included.`);

// ---------------------------------------------------------------------------------------------
// Arrays and typed arrays.
//
// The end-to-end case is a custom BufferGeometry: C# sends vertex data as a $ta typed array, the
// applier rebuilds the real Float32Array, and three.js uploads it. Anything less than the real
// constructor fails here - BufferAttribute reads `array.length` and hands the buffer to WebGL, so a
// plain Array of numbers gets through the constructor and breaks at draw time instead.
// ---------------------------------------------------------------------------------------------

const geometryContext = { objects: new Map() };
const TRIANGLE = [0, 0, 0, 1, 0, 0, 0, 1, 0];

runOps(geometryContext, [
    { k: OP_CREATE, h: 1, t: 'BufferAttribute', a: [{ $ta: 'Float32Array', v: TRIANGLE }, 3] },
    { k: OP_CREATE, h: 2, t: 'BufferGeometry', a: [] },
    { k: OP_CALL, h: 2, m: 'setAttribute', a: ['position', { $ref: 1 }] }
]);

const positionAttribute = geometryContext.objects.get(1);
assert.equal(positionAttribute.array instanceof Float32Array, true, 'a $ta value must rebuild the real typed array, not a plain Array');
assert.equal(positionAttribute.count, 3, 'three vertices of item size three');
assert.equal(
    geometryContext.objects.get(2).getAttribute('position'),
    positionAttribute,
    'a $ref argument must resolve to the object the handle names');

// Read back: a typed array returns in the same $ta form C# sends, so BufferGeometry data round-trips.
const attributeReadBack = runOps(geometryContext, [{ k: OP_GET, h: 1, m: 'array', i: 82 }]).r[0].v;
assert.deepEqual(
    attributeReadBack,
    { $ta: 'Float32Array', v: TRIANGLE },
    'a typed array must read back under the same tag it was sent with');

// An array of handles is sendable but deliberately NOT readable: encoding one would need a minted
// handle per element, which no op does. It has to fail loudly rather than serialize the objects.
geometryContext.objects.set(3, { bones: [positionAttribute] });
const unreadableArray = runOps(geometryContext, [{ k: OP_GET, h: 3, m: 'bones', i: 83 }]);
assert.match(
    unreadableArray.r[0].e,
    /no wire encoding/,
    'an array of three.js objects must be refused on the way back, not serialized element by element');

// Plain arrays, both directions, including one of tagged values.
geometryContext.objects.set(4, {});
runOps(geometryContext, [
    { k: OP_SET, h: 4, m: 'numbers', v: [1, 2, 3] },
    { k: OP_SET, h: 4, m: 'points', v: [{ $t: 'Vector3', v: [1, 2, 3] }, { $t: 'Vector3', v: [4, 5, 6] }] }
]);

const arrayCarrier = geometryContext.objects.get(4);
assert.deepEqual(arrayCarrier.numbers, [1, 2, 3], 'a plain array must decode element by element');
assert.equal(arrayCarrier.points.length, 2, 'an array of tagged values must keep every element');
assert.equal(arrayCarrier.points[0].isVector3, true, 'each element of a tagged array must become a real three.js value');
assert.deepEqual(
    runOps(geometryContext, [{ k: OP_GET, h: 4, m: 'points', i: 84 }]).r[0].v,
    [{ $t: 'Vector3', v: [1, 2, 3] }, { $t: 'Vector3', v: [4, 5, 6] }],
    'an array of math values must read back tagged element by element');

console.log('# Arrays OK - a custom BufferGeometry built from a $ta typed array, arrays round-tripped, handle arrays refused on read.');

// ---------------------------------------------------------------------------------------------
// Minting a handle for a member whose result is an object.
//
// The property this rests on is that an object the table already knows answers with the handle it
// already has. three.js returns `this` from most of its mutators, and the declared return type cannot
// tell those apart from the ones that allocate — Object3D.clone and BufferGeometry.center both say
// `this`. Minting blindly would give an object C# already mirrors a second handle, and a write through
// one would be invisible through the other.
// ---------------------------------------------------------------------------------------------

const mintContext = { objects: new Map() };
runOps(mintContext, [{ k: OP_CREATE, h: 1, t: 'BufferGeometry', a: [] }]);
const mintGeometry = mintContext.objects.get(1);

// `center()` returns the receiver. C# created it, so the handle that comes back must be C#'s own
// positive one and NOT a freshly minted negative one.
const selfReturn = runOps(mintContext, [{ k: OP_READ, h: 1, m: 'center', a: [], i: 90, n: true }]).r[0].v;
assert.equal(selfReturn.$ref, 1, 'a method returning the receiver must answer with the handle it already has');
assert.equal(mintContext.objects.size, 1, 'answering with a known handle must not register a second entry');

// `clone()` allocates. That one is genuinely new, so it gets a minted handle of its own.
const cloned = runOps(mintContext, [{ k: OP_READ, h: 1, m: 'clone', a: [], i: 91, n: true }]).r[0].v;
assert.notEqual(cloned.$ref, 1, 'a method that allocates must not answer with the receiver handle');
assert.ok(cloned.$ref < 0, 'a minted handle must be negative');
assert.equal(cloned.t, 'BufferGeometry', 'the reference must carry the constructor name three.js reports');
assert.notEqual(mintContext.objects.get(cloned.$ref), mintGeometry, 'the clone must be a different object');

// Asking twice for the same object answers the same handle both times.
const firstAsk = runOps(mintContext, [{ k: OP_GET, h: 1, m: 'attributes', i: 92, n: true }]).r[0].v;
const secondAsk = runOps(mintContext, [{ k: OP_GET, h: 1, m: 'attributes', i: 93, n: true }]).r[0].v;
assert.equal(firstAsk.$ref, secondAsk.$ref, 'reading the same object twice must answer with one handle, not two');

// A value still answers as a value under `n`: handing back a handle to a Vector3 would make the caller
// round-trip again for components C# can hold directly.
const valueUnderMint = runOps(mintContext, [{ k: OP_GET, h: 1, m: 'boundingSphere', i: 94, n: true }]).r[0].v;
assert.equal(valueUnderMint, null, 'an unset property answers null even when a handle was asked for');

runOps(mintContext, [{ k: OP_CREATE, h: 2, t: 'Vector3', a: [1, 2, 3] }]);
mintContext.objects.set(3, { position: mintContext.objects.get(2) });
const mathUnderMint = runOps(mintContext, [{ k: OP_GET, h: 3, m: 'position', i: 95, n: true }]).r[0].v;
assert.deepEqual(mathUnderMint, { $t: 'Vector3', v: [1, 2, 3] }, 'a math value must answer as a value even when a handle was asked for');

// Without `n`, an object result is still refused rather than serialized.
const refusedWithoutMint = runOps(mintContext, [{ k: OP_READ, h: 1, m: 'clone', a: [], i: 96 }]);
assert.match(
    refusedWithoutMint.r[0].e,
    /no wire encoding/,
    'an object result must still be refused when the caller did not ask for a handle');

// The renderer the context registers for itself has to be remembered in both directions like anything
// else, and it is the one object nothing in C# ever attached: `ThreeContext.Renderer` mirrors it under
// the reserved handle from the moment the context exists. `InspectorBase.getRenderer` is a generated
// member that answers with it - C#'s `GetRendererAsync` - so a renderer registered only in the forward
// table would come back here as a freshly minted handle, and C# would build a second, untyped mirror of
// the renderer it already holds with no way to notice.
const rendererContext = createContextAround({ isFakeRenderer: true }, createRecordingDotNetRef());
applyOp(rendererContext, { k: OP_CREATE, h: 1, t: 'InspectorBase', a: [] });
applyOp(rendererContext, { k: OP_CALL, h: 1, m: 'setRenderer', a: [{ $ref: RESERVED_RENDERER_HANDLE }] });

const objectCountBeforeRendererRead = rendererContext.objects.size;
const readBackRenderer = applyOp(rendererContext, { k: OP_READ, h: 1, m: 'getRenderer', a: [], n: true });

assert.equal(
    readBackRenderer.$ref, RESERVED_RENDERER_HANDLE,
    'a member answering with the renderer must answer with the reserved handle C# already mirrors it under');
assert.equal(
    rendererContext.objects.size, objectCountBeforeRendererRead,
    'answering with the renderer must not register a second entry for it');

console.log('# Handle minting OK - the receiver keeps its handle, a clone gets a new one, values stay values.');

// ---------------------------------------------------------------------------------------------
// The escape hatch. Everything above is the typed surface proving itself; this is the other half of
// the coverage claim - that a class the generator does NOT wrap is still reachable, by name, with no
// C# type behind it.
//
// The subject is asserted to be a class with no generated wrapper, read out of the same
// api-coverage.json the README's table is rendered from. If it ever becomes generated, this stops
// proving anything and says so rather than passing quietly.
// ---------------------------------------------------------------------------------------------

const coverage = JSON.parse(readFileSync(new URL('../generator/api-coverage.json', import.meta.url), 'utf8'));

const UNWRAPPED_CLASS = 'Vector2';
const unwrappedEntry = coverage.classes.find(entry => entry.name === UNWRAPPED_CLASS);
assert.ok(unwrappedEntry, `${UNWRAPPED_CLASS} should be one of the classes in the coverage report`);
assert.notEqual(
    unwrappedEntry.status,
    'emittable',
    'the escape-hatch subject must be a class with no generated wrapper, or this section proves nothing');
assert.equal(unwrappedEntry.isReachable, true, 'the escape-hatch subject must be one the coverage report calls reachable');

// Construct, mutate and read back a class with no generated wrapper, in one batch, through the same
// four ops the untyped C# surface records: Create names it, Get reads a property, Read invokes a
// method, Set writes a property and Call invokes a command.
const escapeHatchResponse = runOps(context, [
    { k: OP_CREATE, h: 70, t: UNWRAPPED_CLASS, a: [3, 4] },
    { k: OP_GET, h: 70, m: 'x', i: 51 },
    { k: OP_READ, h: 70, m: 'length', a: [], i: 52 },
    { k: OP_SET, h: 70, m: 'x', v: 6 },
    { k: OP_CALL, h: 70, m: 'multiplyScalar', a: [2] },
    { k: OP_GET, h: 70, m: 'y', i: 53 }
]);

const escapeHatchRow = requestId => escapeHatchResponse.r.find(row => row.i === requestId);
assert.deepEqual(escapeHatchResponse.e, [], 'no op of the escape-hatch batch should have been rejected');
assert.ok(context.objects.get(70).isVector2, `a Create naming '${UNWRAPPED_CLASS}' should build the real three.js class`);
assert.equal(escapeHatchRow(51).v, 3, 'a get should read the property the constructor argument landed in');
assert.ok(Math.abs(escapeHatchRow(52).v - 5) < 1e-9, 'a read should invoke the method, and (3, 4) is 5 units long');
assert.equal(escapeHatchRow(53).v, 8, 'the get behind the Set and the Call should observe both: 4 doubled is 8');
assert.equal(context.objects.get(70).x, 12, 'the Set and the Call should both have reached the object: 6 doubled is 12');

// A get reads the member; it does not invoke it. `length` is a method on Vector2, so asking for it
// as a property yields the function itself, which has no wire encoding - where the read op above
// invoked the same name and got 5 back. Without this the two op kinds could quietly become one.
const getRefusalResponse = runOps(context, [
    { k: OP_GET, h: 70, m: 'length', i: 54 },
    { k: OP_GET, h: 70, m: 'notAProperty', i: 55 },
    { k: OP_GET, h: 3, m: 'geometry', i: 56 }
]);

assert.deepEqual(getRefusalResponse.e, [], 'a failed get must not be reported on the batch error channel');
assert.match(
    getRefusalResponse.r.find(row => row.i === 54).e,
    /no wire encoding/,
    'a get naming a method must refuse the function rather than invoking it');
assert.match(
    getRefusalResponse.r.find(row => row.i === 55).e,
    /not a property/,
    'a get naming a property the object has not got must fail rather than answering with undefined');
assert.match(
    getRefusalResponse.r.find(row => row.i === 56).e,
    /no wire encoding/,
    'a get of a property holding a three.js object must be refused, not serialized as a plain object');

// ---------------------------------------------------------------------------------------------
// Pointer picking, end to end against the vendored three.js. The read op above proves a value C#
// asked for came back; this proves a call C# never asked for goes out - and, just as importantly,
// that nothing goes out when it should not.
//
// A real THREE.Raycaster, a real THREE.Mesh and a real box geometry do the work; only the two things
// Node genuinely has not got are stood in for. The canvas is a recorder of addEventListener /
// removeEventListener calls, which is what makes "no pointer-move listener exists" an assertion
// rather than a claim, and the .NET reference is a recorder of invokeMethodAsync calls, which is what
// makes "exactly one callback" countable.
// ---------------------------------------------------------------------------------------------

// The fixture's own pick op first, applied above with the rest of the batch, so the shape C#
// serializes is the shape that actually registers a candidate. That context has no renderer and no
// .NET reference, which is the other half of what this proves: opting an object in registers it
// without touching the DOM at all.
assert.ok(context.pointerTargets.has(3), 'the fixture pick op should have registered the mesh as a hit-test candidate');
assert.equal(context.pointerListener, undefined, 'a context with no .NET reference must not have grown a listener');

function createRecordingCanvas() {
    const registrations = [];
    return {
        registrations,
        addEventListener(type, listener) {
            registrations.push({ type, listener });
        },
        removeEventListener(type, listener) {
            const index = registrations.findIndex(registration => registration.type === type && registration.listener === listener);
            if (index >= 0) {
                registrations.splice(index, 1);
            }
        }
    };
}

function createRecordingDotNetRef() {
    const invocations = [];
    return {
        invocations,
        invokeMethodAsync(...args) {
            invocations.push(args);
            return Promise.resolve();
        }
    };
}

const NEAR_MESH_HANDLE = 63;
const FAR_MESH_HANDLE = 64;

const pointerCanvas = createRecordingCanvas();
const pointerDotNetRef = createRecordingDotNetRef();
const pickingContext = {
    objects: new Map(),
    renderer: { domElement: pointerCanvas },
    dotNetRef: pointerDotNetRef,
    cameraHandle: 60
};

// A camera at the origin looking down -Z, and two boxes straight in front of it at 2 and 5 units.
// The world matrices are updated by an explicit Call op because nothing else will: in the browser the
// render loop does it every frame, and intersectObjects never does it itself.
for (const op of [
    { k: OP_CREATE, h: 60, t: 'PerspectiveCamera', a: [50, 1, 0.1, 100] },
    { k: OP_CREATE, h: 61, t: 'BoxGeometry', a: [1, 1, 1] },
    { k: OP_CREATE, h: 62, t: 'MeshStandardMaterial', a: [] },
    { k: OP_CREATE, h: NEAR_MESH_HANDLE, t: 'Mesh', a: [{ $ref: 61 }, { $ref: 62 }] },
    { k: OP_SET, h: NEAR_MESH_HANDLE, m: 'position', v: { $t: 'Vector3', v: [0, 0, -2] } },
    { k: OP_CALL, h: NEAR_MESH_HANDLE, m: 'updateMatrixWorld', a: [] },
    { k: OP_CREATE, h: FAR_MESH_HANDLE, t: 'Mesh', a: [{ $ref: 61 }, { $ref: 62 }] },
    { k: OP_SET, h: FAR_MESH_HANDLE, m: 'position', v: { $t: 'Vector3', v: [0, 0, -5] } },
    { k: OP_CALL, h: FAR_MESH_HANDLE, m: 'updateMatrixWorld', a: [] },
    { k: OP_CALL, h: 60, m: 'updateMatrixWorld', a: [] }
]) {
    applyOp(pickingContext, op);
}

// Nothing has opted in yet, so there is nothing on the canvas to hear a pointer at all. This is the
// zero-cost property the whole design rests on: an idle scene is not a scene whose listener does
// little, it is a scene with no listener.
assert.deepEqual(pointerCanvas.registrations, [], 'a context with nothing opted in must register no DOM listener');
assert.equal(dispatchPointerHit(pickingContext, 0, 0), null, 'a pointer over a scene with nothing opted in must hit nothing');
assert.deepEqual(pointerDotNetRef.invocations, [], 'a pointer over a scene with nothing opted in must send nothing to C#');

// The opt-in op, which is the only thing that puts a listener on the canvas.
applyOp(pickingContext, { k: OP_PICK, h: FAR_MESH_HANDLE, v: true });

assert.deepEqual(
    pointerCanvas.registrations.map(registration => registration.type),
    ['click'],
    'opting an object in must register the click listener and nothing else');

// ⚠️ The property most likely to be violated quietly, asserted rather than reasoned about: no
// pointer-move listener of any spelling exists, so moving the pointer runs no code and therefore
// costs no interop, whatever the scene contains.
assert.deepEqual(
    pointerCanvas.registrations.filter(registration => /move|over|out|enter|leave/i.test(registration.type)),
    [],
    'no pointer-movement listener may be registered while only OnClick is subscribed');

// A ray straight down the middle meets the opted-in box, and reports it once.
assert.deepEqual(pointerDotNetRef.invocations, [], 'no callback should have been sent before the first pointer event');
const farHit = dispatchPointerHit(pickingContext, 0, 0);
assert.ok(farHit, 'a pointer over the opted-in box should have hit it');
assert.equal(pointerDotNetRef.invocations.length, 1, 'one pointer event over one object must produce exactly one callback');

const [callbackName, hitHandle, hitX, hitY, hitZ, hitDistance] = pointerDotNetRef.invocations[0];
assert.equal(callbackName, 'DispatchPointerEventAsync', 'the callback must name the [JSInvokable] method on ThreeCanvas');
assert.equal(hitHandle, FAR_MESH_HANDLE, 'the callback must carry the handle of the object the ray met');
assert.equal(hitX, 0, 'the hit point should be on the ray, which runs down the camera axis');
assert.equal(hitY, 0, 'the hit point should be on the ray, which runs down the camera axis');
// The front face of a 1x1x1 box centred 5 units away is at z = -4.5, which is 4.5 from the camera at
// the origin. Numbers three.js computed from its own geometry, not ones this file could arrange.
assert.ok(Math.abs(hitZ - -4.5) < 1e-6, 'the hit point should be on the front face of the box, not at its origin');
assert.ok(Math.abs(hitDistance - 4.5) < 1e-6, 'the distance should be from the camera to the front face');

// A pointer over empty space. The ray still runs, and still meets nothing, so nothing crosses the
// boundary - the outcome a hover-style API would have had to report and this one does not.
pointerDotNetRef.invocations.length = 0;
assert.equal(dispatchPointerHit(pickingContext, 0.9, 0.9), null, 'a pointer over empty space must hit nothing');
assert.deepEqual(pointerDotNetRef.invocations, [], 'a pointer over empty space must send nothing to C#');

// Two opted-in objects on the same ray: the nearer one wins and the further one is not also
// reported, so a stack of clickable objects is still exactly one callback.
applyOp(pickingContext, { k: OP_PICK, h: NEAR_MESH_HANDLE, v: true });
assert.equal(pointerCanvas.registrations.length, 1, 'a second opt-in must not register a second listener, or one click would report twice');
const nearHit = dispatchPointerHit(pickingContext, 0, 0);
assert.equal(pointerDotNetRef.invocations.length, 1, 'a ray through two opted-in objects must still produce exactly one callback');
assert.equal(nearHit.handle, NEAR_MESH_HANDLE, 'the nearest object on the ray must be the one reported');
assert.ok(Math.abs(pointerDotNetRef.invocations[0][5] - 1.5) < 1e-6, 'the reported distance should be to the nearer box');

// An object with no subscriber is not a candidate, however solidly it sits on the ray. Opting the
// near box back out hands the same click to the far one.
pointerDotNetRef.invocations.length = 0;
applyOp(pickingContext, { k: OP_PICK, h: NEAR_MESH_HANDLE, v: false });
assert.equal(dispatchPointerHit(pickingContext, 0, 0).handle, FAR_MESH_HANDLE, 'an opted-out object must stop being hit-testable');
assert.equal(pointerDotNetRef.invocations.length, 1, 'the click should have been reported once, by the object still opted in');

// Disposing an opted-in object must take it out of the candidate set too, or the applier would go on
// holding and hit-testing an object three.js has released.
pointerDotNetRef.invocations.length = 0;
applyOp(pickingContext, { k: OP_DISPOSE, h: FAR_MESH_HANDLE });
assert.equal(dispatchPointerHit(pickingContext, 0, 0), null, 'a disposed object must stop being hit-testable');
assert.deepEqual(pointerDotNetRef.invocations, [], 'a disposed object must not report a hit');

// And with the last candidate gone the listener comes off the canvas, so the scene is back to
// costing nothing.
assert.deepEqual(pointerCanvas.registrations, [], 'the click listener must be removed once nothing is opted in');

// A context with no .NET reference has nowhere to report a hit, so it never listens for one either -
// the other half of the gate, and what a consumer creating a context outside ThreeCanvas gets.
const unreachableCanvas = createRecordingCanvas();
const unreachableContext = {
    objects: new Map(pickingContext.objects),
    renderer: { domElement: unreachableCanvas },
    dotNetRef: null,
    cameraHandle: 60
};
applyOp(unreachableContext, { k: OP_PICK, h: NEAR_MESH_HANDLE, v: true });
assert.deepEqual(unreachableCanvas.registrations, [], 'a context with no .NET reference must register no listener');

// ---------------------------------------------------------------------------------------------
// The addons: GLTFLoader and OrbitControls, driven as vendored modules against the vendored bundle.
//
// This is the only place either one is proved to *resolve*. They live in `examples/jsm`, which the
// three.js bundle does not include, so each ships as its own static asset with its bare `three`
// imports rewritten - and an ES module that imports a sibling it cannot find builds green, passes
// every C# test, and fails in the browser the first time a consumer loads a model. A mocked test
// would prove the plumbing and miss exactly that. `generator/vendor-addons.mjs --check` guards the
// import closure statically; this guards it by running it.
//
// The model is the demo's own `figure.gltf`, served over a real HTTP server on a free port, so the
// fetch, the parse and the handle minting are all the real thing - and the file the demo ships is
// proved to be a file that loads.
// ---------------------------------------------------------------------------------------------

const modelPath = fileURLToPath(new URL('../demo/Blazor.ThreeJS.Demo.Stories/wwwroot/models/figure.gltf', import.meta.url));
const animatedFixturePath = fileURLToPath(new URL('./animated-fixture.gltf', import.meta.url));
const dracoFixturePath = fileURLToPath(new URL('../demo/Blazor.ThreeJS.Demo.Stories/wwwroot/models/box-draco.gltf', import.meta.url));
const dracoFixtureBinPath = fileURLToPath(new URL('../demo/Blazor.ThreeJS.Demo.Stories/wwwroot/models/box-draco.bin', import.meta.url));
const fixturesByRoute = new Map([
    ['/figure.gltf', modelPath],
    ['/animated-fixture.gltf', animatedFixturePath],
    ['/box-draco.gltf', dracoFixturePath],
    ['/box-draco.bin', dracoFixtureBinPath]
]);

const modelServer = createServer((request, response) => {
    const filePath = fixturesByRoute.get(request.url);
    if (!filePath) {
        response.writeHead(404);
        response.end();
        return;
    }

    response.writeHead(200, { 'Content-Type': 'model/gltf+json' });
    createReadStream(filePath).pipe(response);
});

await new Promise(resolve => modelServer.listen(0, '127.0.0.1', resolve));
const modelUrl = `http://127.0.0.1:${modelServer.address().port}/figure.gltf`;
const animatedFixtureUrl = `http://127.0.0.1:${modelServer.address().port}/animated-fixture.gltf`;
const dracoFixtureUrl = `http://127.0.0.1:${modelServer.address().port}/box-draco.gltf`;

const loadingContext = { objects: new Map() };

// Stands in for the DotNetObjectReference a caller passing an IProgress supplies. This is the only
// JavaScript-to-C# call the package makes during an operation, and it is bounded by the fetch — a
// handful of events for a model, never per frame.
const progressReports = [];
const progressRef = {
    invokeMethodAsync: (method, loaded, total) => {
        progressReports.push({ method, loaded, total });
        return Promise.resolve();
    }
};

const loadResponse = await loadGltfInto(loadingContext, modelUrl, progressRef);
const loadedNodes = loadResponse.n;

assert.ok(progressReports.length > 0, 'a load with a progress reference should report at least once');
assert.equal(
    progressReports.every(report => report.method === 'ReportProgress'),
    true,
    'progress must be delivered to the [JSInvokable] name the C# reporter declares');
assert.ok(
    progressReports.every(report => typeof report.loaded === 'number' && typeof report.total === 'number'),
    'both progress figures must be numbers, since C# declares them long');

// A progress reference that throws must not fail an otherwise-fine load: the usual cause is a circuit
// that went away mid-download, and the model is still going to arrive.
const faultingContext = { objects: new Map() };
const faultingRef = { invokeMethodAsync: () => Promise.reject(new Error('circuit gone')) };
const survivedResponse = await loadGltfInto(faultingContext, modelUrl, faultingRef);
assert.ok(survivedResponse.n.length > 1, 'a failing progress report must not fail the load');

assert.ok(loadedNodes.length > 1, 'the loaded graph should report a root and its named descendants');
assert.deepEqual(loadResponse.a, [], 'a model with no animations should report an empty clip list');

// Handle minting, and the property the whole design rests on: the browser allocates downwards and C#
// upwards from 1, so neither allocator can ever produce a handle the other already used. ThreeObject
// rejects a non-negative handle offered as browser-minted, which is the same rule from the other side.
const mintedHandles = loadedNodes.map(node => node.h);
assert.deepEqual(
    mintedHandles.filter(handle => handle >= 0),
    [],
    'every handle the browser mints must be negative, or it would collide with one C# allocated');
assert.equal(new Set(mintedHandles).size, mintedHandles.length, 'no two loaded nodes may share a handle');

// -1 is reserved for the context's own renderer, so minting starts below it. A loaded node handed -1
// would answer to the same handle C# addresses the renderer by, and writes meant for one would land
// on the other.
assert.deepEqual(mintedHandles.slice(0, 3), [-2, -3, -4], 'minting should count down from below the reserved renderer handle');
assert.equal(mintedHandles.includes(RESERVED_RENDERER_HANDLE), false, 'no minted handle may take the reserved renderer handle');

for (const node of loadedNodes) {
    assert.ok(loadingContext.objects.has(node.h), `handle ${node.h} should be registered in the object table`);
}

// Only *named* nodes are mirrored, and every row carries the transform the loader gave the node -
// which is what makes the C# mirror's Position honest the moment it is handed over, rather than a
// zero it would have to read back to correct.
assert.deepEqual(
    loadedNodes.filter(node => node !== loadedNodes[0] && !node.n),
    [],
    'no unnamed node may be mirrored');

const head = loadedNodes.find(node => node.n === 'Head');
assert.ok(head, 'the demo figure should carry a node named Head');
assert.equal(head.t, 'Mesh', 'the head node should be reported as the Mesh three.js built for it');
assert.equal(head.p.$t, 'Vector3', 'a node position should travel under the same Vector3 tag C# sends one in');
assert.deepEqual(head.p.v, [0, 0.95, 0], 'the head position should be the one the file declares, read off the loaded object');
assert.deepEqual(head.s.v, [0.3, 0.32, 0.3], 'the head scale should be the one the file declares');
assert.equal(head.r.$t, 'Euler', 'a node rotation should travel under the Euler tag');
assert.equal(head.v, true, 'a node the loader left visible should be reported visible');
assert.equal(loadingContext.objects.get(head.h).isMesh, true, 'the registered object should be the real three.js Mesh');

// A loaded node has to work like any other object in the graph, or the mirror would only be able to
// name it. Adding it to a scene by handle, hiding it and hit-testing it are the three things a
// consumer actually does with one.
applyOp(loadingContext, { k: OP_CREATE, h: 100, t: 'Scene', a: [] });
applyOp(loadingContext, { k: OP_ADD, h: 100, c: loadedNodes[0].h });
assert.equal(
    loadingContext.objects.get(100).children.length,
    1,
    'a loaded root must be addable to a C#-created scene by its minted handle');

applyOp(loadingContext, { k: OP_SET, h: head.h, m: 'visible', v: false });
assert.equal(loadingContext.objects.get(head.h).visible, false, 'a Set op must reach a loaded node like any other');
applyOp(loadingContext, { k: OP_SET, h: head.h, m: 'visible', v: true });

applyOp(loadingContext, { k: OP_PICK, h: head.h, v: true });
assert.ok(loadingContext.pointerTargets.has(head.h), 'a loaded node must be able to opt into hit-testing');

// A JS-side read that returns a loaded node must answer with the handle the load already minted for
// it, not a fresh one - this is what registerObject inside describeLoadedNode buys: without it,
// handleFor would have no record of the object at all and would mint a duplicate every time.
const objectCountBeforeReread = loadingContext.objects.size;
const rereadHead = applyOp(loadingContext, {
    k: OP_READ, h: loadedNodes[0].h, m: 'getObjectByName', a: ['Head'], n: true
});
assert.equal(
    rereadHead.$ref, head.h,
    'reading back an already-loaded node must answer with the handle load minted for it');
assert.equal(
    loadingContext.objects.size, objectCountBeforeReread,
    'answering with an already-minted handle must not register a second entry for the same object');

// Disposal. The geometries, materials and textures a loaded file brings in are the resources nothing
// else releases: a Mesh has no dispose, so the generic arm never reaches them, and C# never created
// them so no dispose op will ever name them either. Disposing the root has to release the graph.
const releasedResources = new Set();
loadingContext.objects.get(loadedNodes[0].h).traverse(object => {
    for (const resource of [object.geometry, object.material]) {
        if (resource) {
            resource.addEventListener('dispose', () => releasedResources.add(resource));
        }
    }
});

applyOp(loadingContext, { k: OP_DISPOSE, h: loadedNodes[0].h });

assert.ok(releasedResources.size >= 2, 'disposing a loaded root must release the geometries and materials it brought in');
assert.deepEqual(
    mintedHandles.filter(handle => loadingContext.objects.has(handle)),
    [],
    'disposing a loaded root must retire every handle minted for its graph');
assert.equal(loadingContext.pointerTargets.size, 0, 'a disposed loaded node must stop being hit-testable');

// A model that does carry animations reports one row per clip, minted after every node handle so the
// wire test's [-2,-3,-4] pin on the demo figure stays untouched by files that bring clips along.
const animatedLoadingContext = { objects: new Map() };
const animatedResponse = await loadGltfInto(animatedLoadingContext, animatedFixtureUrl, undefined);

assert.equal(animatedResponse.a.length, 1, 'the animated fixture should report exactly one clip');

const clipRow = animatedResponse.a[0];
assert.ok(clipRow.h < 0, 'a clip handle must come from the browser-minted half of the space');
assert.equal(
    animatedLoadingContext.objects.has(clipRow.h),
    true,
    `clip handle ${clipRow.h} should be registered in the object table`);

const registeredClip = animatedLoadingContext.objects.get(clipRow.h);
assert.equal(
    typeof registeredClip.duration,
    'number',
    'the registered object should be the real three.js AnimationClip, which carries a numeric duration');
assert.equal(registeredClip.name, 'Spin', 'the registered clip should be named the way the fixture names it');
assert.equal(clipRow.n, 'Spin', 'the wire row should carry the clip name under n');
assert.equal(typeof clipRow.d, 'number', 'the wire row should carry the clip duration under d');

const animatedNodeHandles = animatedResponse.n.map(node => node.h);
assert.ok(
    animatedNodeHandles.every(nodeHandle => clipRow.h < nodeHandle),
    'the clip handle must be minted after every node handle, since minting counts further down each time');

// The clip half of what registerObject buys, and the one member that reads a clip back:
// AnimationAction.getClip. Without the reverse entry, handleFor would have no record of the clip
// object and would mint a second handle for the clip C# already mirrors, so a write through one mirror
// would be invisible through the other.
applyOp(animatedLoadingContext, {
    k: OP_CREATE, h: 300, t: 'AnimationMixer', a: [{ $ref: animatedResponse.n[0].h }]
});

const clipAction = applyOp(animatedLoadingContext, {
    k: OP_READ, h: 300, m: 'clipAction', a: [{ $ref: clipRow.h }], n: true
});

const objectCountBeforeClipReread = animatedLoadingContext.objects.size;
const rereadClip = applyOp(animatedLoadingContext, {
    k: OP_READ, h: clipAction.$ref, m: 'getClip', a: [], n: true
});

assert.equal(
    rereadClip.$ref, clipRow.h,
    'reading a clip back off its action must answer with the handle the load minted for it');
assert.equal(
    animatedLoadingContext.objects.size, objectCountBeforeClipReread,
    'answering with an already-minted clip handle must not register a second entry for the same clip');

applyOp(animatedLoadingContext, { k: OP_DISPOSE, h: animatedResponse.n[0].h });
assert.equal(
    animatedLoadingContext.objects.has(clipRow.h),
    false,
    'disposing the loaded root must retire the clip handle along with the node handles');

// ---------------------------------------------------------------------------------------------
// DRACO decoding, opt-in. `box-draco.gltf` is Khronos's own CC-BY-4.0 Box sample, re-exported with
// KHR_draco_mesh_compression, which is documented (README, GLTFLoader's own class doc) as rejecting
// unless a caller opts in. Both halves of that documented behaviour are proved here: the file loads
// and mirrors its root when `{draco:true}` wires a DRACOLoader in, and the exact same file rejects
// with GLTFLoader's own message when no options are given, since that is the failure a caller who
// skips the opt-in is meant to see.
// ---------------------------------------------------------------------------------------------

const dracoRejectedContext = { objects: new Map() };
await assert.rejects(
    () => loadGltfInto(dracoRejectedContext, dracoFixtureUrl, undefined),
    /DRACOLoader/,
    'a Draco-compressed file must reject the load when no DRACOLoader is wired in');

const dracoLoadingContext = { objects: new Map() };
const dracoResponse = await loadGltfInto(dracoLoadingContext, dracoFixtureUrl, undefined, { draco: true });

assert.ok(dracoResponse.n.length > 0, 'the Draco fixture should report at least its mirrored root');
assert.ok(
    dracoLoadingContext.objects.has(dracoResponse.n[0].h),
    'the mirrored root should be registered in the object table');

// Not just that the load resolved - that the worker actually decoded the compressed buffer back into
// the exact vertex data Box.gltf's own accessors declare, which is the only proof that the wasm
// decoder ran at all rather than a promise that happened to settle.
let decodedMesh = null;
dracoLoadingContext.objects.get(dracoResponse.n[0].h).traverse(object => {
    if (object.isMesh) {
        decodedMesh = object;
    }
});
assert.ok(decodedMesh, 'the decoded graph should contain the mesh Draco compressed');
assert.equal(
    decodedMesh.geometry.attributes.position.count, 24,
    "the decoded geometry should carry the Box sample's 24 vertices");
assert.equal(
    decodedMesh.geometry.index.count, 36,
    "the decoded geometry should carry the Box sample's 36 indices");

// A second {draco:true} load on the same context must reuse the cached decoder rather than building a
// fresh one - and the worker pool it owns - per load. This is what getDracoLoader's cache on the
// context exists to guarantee; without it, a SPA loading ten compressed models would leak ten worker
// pools, one per load, none of which anything but disposeContext could ever reach.
const dracoLoaderAfterFirstLoad = dracoLoadingContext.dracoLoader;
assert.ok(dracoLoaderAfterFirstLoad, 'the context should cache the DRACOLoader it built for the first load');

await loadGltfInto(dracoLoadingContext, dracoFixtureUrl, undefined, { draco: true });
assert.equal(
    dracoLoadingContext.dracoLoader, dracoLoaderAfterFirstLoad,
    'a second opt-in load on the same context must reuse the cached DRACOLoader instance rather than building another');

// Disposal - disposeDecoders is the exact function disposeContext itself calls, not a copy of its
// logic - must tear down the worker pool the two loads above built up and clear the cache, or a
// context that goes on loading compressed models past this point would find a disposed decoder still
// sitting there instead of building a fresh one.
disposeDecoders(dracoLoadingContext);
assert.equal(
    dracoLoaderAfterFirstLoad.workerPool.length, 0,
    'disposing the cached decoder must terminate every worker in its pool');
assert.equal(
    dracoLoadingContext.dracoLoader, undefined,
    'disposing the cached decoder must clear it off the context');

// Two opt-in loads racing to be the *first* one on a brand-new context - unlike the sequential reuse
// above, neither has anything cached to reuse yet when it calls getDracoLoader. Both still end up
// sharing the one decoder getDracoLoader's promise cache hands out, and a load afterwards still reuses
// it, which is what a context that raced two DRACOLoaders into existence would fail to do once the
// loser's instance got overwritten and orphaned.
const racedDracoContext = { objects: new Map() };
const [racedFirst, racedSecond] = await Promise.all([
    loadGltfInto(racedDracoContext, dracoFixtureUrl, undefined, { draco: true }),
    loadGltfInto(racedDracoContext, dracoFixtureUrl, undefined, { draco: true })
]);
assert.ok(
    racedFirst.n.length > 0 && racedSecond.n.length > 0,
    'both racing opt-in loads should still decode successfully');

const racedDracoLoader = racedDracoContext.dracoLoader;
assert.ok(racedDracoLoader, 'a racing pair of first opt-in loads should still leave a decoder cached');

await loadGltfInto(racedDracoContext, dracoFixtureUrl, undefined, { draco: true });
assert.equal(
    racedDracoContext.dracoLoader, racedDracoLoader,
    'a load after the race must still reuse the one decoder the race settled on');

disposeDecoders(racedDracoContext);

// ---------------------------------------------------------------------------------------------
// KTX2, opt-in. There is no small KTX2-compressed sample in this repository's fixtures, so what is
// proved here is the half that does not need one: `{ktx2:true}` on a file that carries no
// KHR_texture_basisu texture still loads exactly as it would with no options at all - the loader is
// wired in and never asked to decode anything - and getKtx2Loader's caching and disposal behave the
// same way getDracoLoader's do, proven the same way just above.
// ---------------------------------------------------------------------------------------------

const ktx2LoadingContext = { objects: new Map() };
const ktx2Response = await loadGltfInto(ktx2LoadingContext, modelUrl, undefined, { ktx2: true });
assert.ok(
    ktx2Response.n.length > 1,
    'a file with no KTX2 texture must still load fully with {ktx2:true} - the loader is attached but never asked to decode anything');

const ktx2LoaderAfterFirstLoad = ktx2LoadingContext.ktx2Loader;
assert.ok(ktx2LoaderAfterFirstLoad, 'the context should cache the KTX2Loader it built for the first opt-in load');

await loadGltfInto(ktx2LoadingContext, modelUrl, undefined, { ktx2: true });
assert.equal(
    ktx2LoadingContext.ktx2Loader, ktx2LoaderAfterFirstLoad,
    'a second opt-in load on the same context must reuse the cached KTX2Loader instance rather than building another');

disposeDecoders(ktx2LoadingContext);
assert.equal(
    ktx2LoadingContext.ktx2Loader, undefined,
    'disposing the cached decoder must clear the KTX2Loader off the context');

// A transient failure building the decoder must not brick every later opt-in load on the same context
// with the same stale rejection forever. DRACOLoader/KTX2Loader's own module path is hardcoded, so it
// cannot be made to fail here, but detectSupport can: a `renderer` with none of the shape KTX2Loader
// expects makes it throw synchronously inside getKtx2Loader's promise, before context.ktx2Loader is
// ever assigned - a real failure on the one branch this test host can actually reach into.
const retryContext = { objects: new Map(), renderer: {} };
let failedLoadError = null;
try {
    await loadGltfInto(retryContext, modelUrl, undefined, { ktx2: true });
} catch (error) {
    failedLoadError = error;
}

assert.ok(
    failedLoadError,
    'a KTX2Loader that fails to detect support against a malformed renderer must reject the load');
assert.equal(
    retryContext.ktx2LoaderPromise, undefined,
    'a rejected decoder promise must clear itself off the context rather than staying cached forever');
assert.equal(
    retryContext.ktx2Loader, undefined,
    'a failed decoder build must never have set the resolved instance');

// The same context, retried once the cause of the failure is gone, must get a real second attempt
// rather than replaying the first attempt's cached rejection.
retryContext.renderer = undefined;
const retriedResponse = await loadGltfInto(retryContext, modelUrl, undefined, { ktx2: true });
assert.ok(
    retriedResponse.n.length > 1,
    'a context must be able to retry and succeed on the same opt-in flag once the earlier failure clears');
assert.ok(retryContext.ktx2Loader, 'a successful retry must leave a decoder cached same as any other first load');

disposeDecoders(retryContext);

modelServer.close();

// ---------------------------------------------------------------------------------------------
// A raycast. Every intersection three.js reports names the object it hit, so a structure the read
// answers with may itself carry a three.js object - and that object has identity a copy of its fields
// would lose. Driven against a real Raycaster and a real Mesh.
// ---------------------------------------------------------------------------------------------

const raycastContext = createContextAround({ isRaycastStandIn: true });
const hitMesh = new THREE.Mesh(new THREE.BoxGeometry(2, 2, 2), new THREE.MeshBasicMaterial());
raycastContext.objects.set(40, hitMesh);

const raycaster = new THREE.Raycaster();
raycaster.set(new THREE.Vector3(0, 0, 5), new THREE.Vector3(0, 0, -1));
raycastContext.objects.set(41, raycaster);

const hits = runOps(raycastContext, [
    { k: OP_READ, h: 41, m: 'intersectObject', a: [{ $ref: 40 }], i: 80, n: true }
]);

assert.equal(hits.r.length, 1, 'the raycast should produce one result row');
const encodedHits = hits.r[0].v;
assert.ok(Array.isArray(encodedHits) && encodedHits.length > 0, 'the ray should hit the box it was aimed at');

const firstHit = encodedHits[0].$o;
assert.ok(firstHit, 'an intersection is a plain object, so it answers under the structure tag');
assert.ok(typeof firstHit.distance === 'number', 'and carries its distance as a value');
assert.ok(firstHit.point.$t === 'Vector3', 'and its point as a tagged math value');

// The object it hit comes back as a reference to the handle this context already holds for it, not as
// a second registration - which is what makes the C# side able to hand back the same mirror.
// Minted here rather than reused, because this stand-in put the mesh in the object table directly
// instead of through the applier's own registration - which is the reverse lookup handleFor reads.
// What matters is that it is a *reference* rather than a copy of the mesh's fields.
assert.equal(typeof firstHit.object.$ref, 'number', 'the hit object should come back as a handle reference');
assert.ok(raycastContext.objects.get(firstHit.object.$ref) === hitMesh, 'and that handle should name the mesh that was hit');
assert.equal(firstHit.object.t, 'Mesh', 'and name the type three.js knows it by');

// ---------------------------------------------------------------------------------------------
// A static. three.js's utility classes - AnimationUtils, ShapeUtils, DataUtils - hang their work off
// the class rather than off any object, so there is no handle to address them by. The op carries the
// three.js type name instead, under the same `t` the create op already uses.
// ---------------------------------------------------------------------------------------------

const staticContext = createContextAround({ isStaticStandIn: true });

const halfFloat = runOps(staticContext, [
    { k: OP_READ, t: 'DataUtils', m: 'toHalfFloat', a: [1], i: 70 }
]);

assert.equal(halfFloat.r.length, 1, 'a static read should produce one result row');
assert.equal(
    halfFloat.r[0].v,
    THREE.DataUtils.toHalfFloat(1),
    'the static should run on the class three.js exports, and answer what three.js answers');

// The whole point of naming the class: no handle is involved, and none is invented.
assert.equal(staticContext.objects.size, 1, 'a static read must not register anything - only the renderer is held');

// A type the bundle does not carry fails by name rather than resolving to undefined and throwing
// something opaque from inside three.js.
const unknownStatic = runOps(staticContext, [
    { k: OP_READ, t: 'NotAThreeJsClass', m: 'whatever', a: [], i: 71 }
]);

assert.match(unknownStatic.r[0].e ?? '', /Unknown three.js type/, 'an unknown class should be named in the failure');

// ---------------------------------------------------------------------------------------------
// A structural value. three.js describes some of what it hands back with an interface rather than a
// class - `BufferGeometry.groups` is `{ start, count, materialIndex }[]` - and those have no identity,
// so they travel as their own members rather than behind a handle.
//
// ⚠️ Only a *plain* object. A three.js instance flattened into JSON would reach C# as a plausible bag
// of numbers, which is the refusal this arm narrows rather than replaces.
// ---------------------------------------------------------------------------------------------

const structureContext = createContextAround({ isStructureStandIn: true });
structureContext.objects.set(1, new THREE.BufferGeometry());
structureContext.objects.set(2, { holder: null });

// Read: three.js's own groups array, produced by three.js rather than by anything sent here.
runOps(structureContext, [{ k: OP_CALL, h: 1, m: 'addGroup', a: [0, 3, 1] }]);
const groupsRead = runOps(structureContext, [{ k: OP_GET, h: 1, m: 'groups', i: 90 }]);

assert.equal(groupsRead.r.length, 1, 'reading groups should produce one row');
assert.deepEqual(
    groupsRead.r[0].v,
    [{ $o: { start: 0, count: 3, materialIndex: 1 } }],
    'a plain object answers as its own members under the structure tag, so C# can bind them by name');

// Write: a structure decodes member by member, so a tagged value nested inside one becomes a real
// three.js value rather than the wire shape of one.
runOps(structureContext, [{
    k: OP_SET,
    h: 2,
    m: 'holder',
    v: { $o: { point: { $t: 'Vector3', v: ['1', '2', '3'] }, count: 7 } }
}]);

const written = structureContext.objects.get(2).holder;
assert.ok(written.point.isVector3, 'a math value nested in a structure decodes to a real three.js one');
assert.equal(written.point.x, 1, 'and carries its components');
assert.equal(written.count, 7, 'a plain member beside it passes through');

// A three.js instance is still refused. This is the rule the plain-object arm narrows, and the reason
// the wire form is tagged at all.
const instanceRead = runOps(structureContext, [{ k: OP_READ, h: 1, m: 'clone', a: [], i: 91 }]);
assert.match(
    instanceRead.r[0].e ?? '',
    /has no wire encoding/,
    'a three.js instance must still be refused rather than serialized as a bag of numbers');

// ---------------------------------------------------------------------------------------------
// A promise answer. three.js's WebGPU renderer resolves half its API - renderAsync, clearAsync,
// readRenderTargetPixelsAsync - when the GPU is done rather than when the call returns, so the
// applier has to wait for the promise before filling in the row. Driven against a stand-in object
// rather than a real renderer, because no WebGPU device exists under Node and what is being pinned
// is the wire behaviour, not the renderer.
// ---------------------------------------------------------------------------------------------

const promiseContext = createContextAround({ isPromiseStandIn: true });
let sideEffectOrder = [];
promiseContext.objects.set(1, {
    settleAfter: 0,
    slowAnswer(value) {
        sideEffectOrder.push('called');
        return new Promise(resolve => setTimeout(() => resolve(value), this.settleAfter));
    },
    failsLater() {
        return Promise.reject(new Error('the GPU said no'));
    },
    fastAnswer() {
        return 7;
    },
    marker: 0
});

// A batch with no promise in it answers synchronously, which is what keeps a per-frame flush off the
// microtask queue. Asserted as "not a thenable" rather than by awaiting, since awaiting would pass
// either way.
const syncResponse = runOps(promiseContext, [{ k: OP_READ, h: 1, m: 'fastAnswer', a: [], i: 1 }]);
assert.equal(typeof syncResponse.then, 'undefined', 'a batch with no promise answer must not become a promise itself');
assert.equal(syncResponse.r[0].v, 7, 'a plain answer should still come back as itself');

// A promise answer makes the whole response a promise, and the row carries what it settled to rather
// than the promise object.
sideEffectOrder = [];
const promised = runOps(promiseContext, [
    { k: OP_READ, h: 1, m: 'slowAnswer', a: [42], i: 2 },
    { k: OP_SET, h: 1, m: 'marker', v: 5 }
]);

assert.equal(typeof promised.then, 'function', 'a batch carrying a promise answer must answer with a promise');
assert.equal(promiseContext.objects.get(1).marker, 5,
    'ops behind a promise answer must still apply in order, without waiting for it');

const promisedResponse = await promised;
assert.deepEqual(promisedResponse.e, [], 'a settled promise answer is not an error');
assert.equal(promisedResponse.r.length, 1, 'the promise answer should fill in exactly one row');
assert.equal(promisedResponse.r[0].i, 2, 'the row should still carry its own request id');
assert.equal(promisedResponse.r[0].v, 42, 'the row should carry what the promise settled to, not the promise');

// A rejected promise faults the one row that asked, on the same channel a throwing read uses, and
// leaves the batch's error channel alone - exactly one C# task is awaiting it.
const rejectedResponse = await runOps(promiseContext, [{ k: OP_READ, h: 1, m: 'failsLater', a: [], i: 3 }]);
assert.deepEqual(rejectedResponse.e, [], 'a rejected read must not also be announced on the batch error channel');
assert.equal(rejectedResponse.r.length, 1, 'a rejected read still produces its own row');
assert.equal(rejectedResponse.r[0].v, undefined, 'a rejected row must not carry a value');
assert.match(rejectedResponse.r[0].e, /the GPU said no/, "the row should carry the promise's own rejection message");

// Two promise answers in one batch both settle before the response resolves, whichever order they
// settle in, and each row still answers its own request.
promiseContext.objects.get(1).settleAfter = 0;
const slowThenFast = await runOps(promiseContext, [
    { k: OP_READ, h: 1, m: 'slowAnswer', a: ['first'], i: 4 },
    { k: OP_READ, h: 1, m: 'slowAnswer', a: ['second'], i: 5 }
]);

assert.equal(slowThenFast.r.find(row => row.i === 4).v, 'first', 'request 4 should carry its own answer');
assert.equal(slowThenFast.r.find(row => row.i === 5).v, 'second', 'request 5 should carry its own answer');

// ---------------------------------------------------------------------------------------------
// OrbitControls. The camera moves every frame on this side of the boundary, which is the whole
// point, so what is asserted here is that it moves *and* that nothing crosses the boundary while it
// does - and that every listener the addon registered comes back off on detach.
// ---------------------------------------------------------------------------------------------

function createRecordingElement() {
    const registrations = [];
    const listenTo = scope => ({
        addEventListener(type, listener) {
            registrations.push({ scope, type, listener });
        },
        removeEventListener(type, listener) {
            const index = registrations.findIndex(x => x.scope === scope && x.type === type && x.listener === listener);
            if (index >= 0) {
                registrations.splice(index, 1);
            }
        }
    });

    const ownerDocument = listenTo('document');
    return {
        registrations,
        ownerDocument,
        style: {},
        ...listenTo('element'),
        getRootNode() {
            return ownerDocument;
        }
    };
}

const controlsElement = createRecordingElement();
const controlsDotNetRef = createRecordingDotNetRef();
const controlsContext = {
    objects: new Map(),
    renderer: { domElement: controlsElement },
    dotNetRef: controlsDotNetRef,
    cameraHandle: 200
};

applyOp(controlsContext, { k: OP_CREATE, h: 200, t: 'PerspectiveCamera', a: [50, 1, 0.1, 100] });
applyOp(controlsContext, { k: OP_SET, h: 200, m: 'position', v: { $t: 'Vector3', v: [0, 0, 5] } });

assert.deepEqual(controlsElement.registrations, [], 'a canvas with no controls attached must carry none of their listeners');

const controlsHandle = await attachOrbitControlsTo(controlsContext, 200);

assert.ok(controlsHandle < 0, 'the controls handle must come out of the browser-minted half of the space');
assert.equal(controlsContext.objects.get(controlsHandle).object, controlsContext.objects.get(200), 'the controls must be bound to the camera at the handle they were given');
assert.ok(controlsElement.registrations.length > 0, 'attaching controls must put their listeners on the canvas');

// Properties reach the controls through ordinary Set ops, on the minted handle, with no op kind of
// their own - which is what lets them coalesce and flush exactly like every other mirrored property.
applyOp(controlsContext, { k: OP_SET, h: controlsHandle, m: 'autoRotate', v: true });
applyOp(controlsContext, { k: OP_SET, h: controlsHandle, m: 'target', v: { $t: 'Vector3', v: [0, 1, 0] } });
assert.equal(controlsContext.objects.get(controlsHandle).autoRotate, true, 'a Set op must reach the controls');
assert.deepEqual(
    controlsContext.objects.get(controlsHandle).target.toArray(),
    [0, 1, 0],
    'a tagged Vector3 Set must be copied into the target the controls already hold');

// The frame loop, a hundred and twenty times over. The camera has to actually move, or the assertion
// below would be vacuous; and nothing may reach C# while it does, or a drag would be one SignalR
// message per frame for as long as the user holds the mouse down.
const cameraPositionBeforeFrames = controlsContext.objects.get(200).position.toArray();
for (let frame = 0; frame < 120; frame++) {
    controlsContext.objects.get(controlsHandle).update();
}

assert.notDeepEqual(
    controlsContext.objects.get(200).position.toArray(),
    cameraPositionBeforeFrames,
    'the controls should have moved the camera, or the zero-interop assertion below proves nothing');
assert.deepEqual(
    controlsDotNetRef.invocations,
    [],
    'a hundred and twenty frames of camera movement must send nothing to C#');

// Detach has to take back every listener it added - on the canvas and on the document it hangs off -
// or the canvas would go on driving a camera nothing renders.
detachControls(controlsContext);

assert.deepEqual(controlsElement.registrations, [], 'detaching controls must remove every listener they registered');
assert.equal(controlsContext.objects.has(controlsHandle), false, 'detaching controls must retire their handle');
assert.equal(controlsContext.controls, null, 'detaching controls must leave nothing for the render loop to update');

// The floor under the README's headline. `generator/api-coverage.json` is what that number is
// rendered from, and what it claims is that each of those classes is a class a consumer can create -
// which nothing upstream of here checks against the bundle that will actually be asked for it.
// `three-interop.js` resolves a Create op as `THREE[op.t]`, so a name the bundle does not export
// throws `Unknown three.js type` at runtime with a green build and a passing emit:check behind it.
// The 46 classes that shipped in exactly that state were the reason this assertion exists.
const generatedClassNames = coverage.classes.filter(entry => entry.status === 'emittable').map(entry => entry.name);
assert.ok(generatedClassNames.length > 0, 'api-coverage.json should list emittable classes');
const unconstructible = generatedClassNames.filter(name => typeof THREE[name] !== 'function');
assert.deepEqual(
    unconstructible,
    [],
    'every generated class must be a constructor on the vendored three.js namespace');

// The floor under the *second* claim the README makes about that table: that the classes it does not
// generate are still reachable, by name, through Primitive. It is asserted from both sides, because
// each side fails differently and both would be a lie.
//
// Overstating is the dangerous one: a class the table calls reachable but the bundle does not export
// sends a consumer down a path that throws `Unknown three.js type`. Understating is the quiet one: a
// class the bundle does export while the table leaves it out of the count makes the reachable figure
// too small, and a coverage number that drifts either way is exactly what this file exists to stop.
const reachableClassNames = coverage.classes.filter(entry => entry.isReachable).map(entry => entry.name);
assert.equal(
    coverage.totals.reachableClasses,
    reachableClassNames.length,
    'the reachable total must be the number of classes the report marks reachable');

const overstatedReachable = reachableClassNames.filter(name => typeof THREE[name] !== 'function');
assert.deepEqual(
    overstatedReachable,
    [],
    'every class the coverage report calls reachable must be a constructor on the vendored three.js namespace');

const understatedReachable = coverage.classes
    .filter(entry => !entry.isReachable && typeof THREE[entry.name] === 'function')
    .map(entry => entry.name);

assert.deepEqual(
    understatedReachable,
    [],
    'a class the bundle exports as a constructor must be counted as reachable, or the figure understates itself');

assert.ok(
    reachableClassNames.length > generatedClassNames.length,
    'the reachable set must be strictly larger than the generated one, or the escape hatch reaches nothing new');

console.log(`Wire contract OK - ${ops.length} ops applied against the vendored three.js.`);
console.log('Read op OK - values, tagged math values, correlation, ordering and refusals round-tripped.');
console.log('Picking OK - one callback per hit, none for a miss, and no pointer-movement listener at all.');
console.log(`GLTFLoader OK - the demo's own model fetched, parsed and mirrored as ${loadedNodes.length} nodes on browser-minted handles, then released.`);
console.log('DRACO decoding OK - the same compressed file rejects with no options, decodes with {draco:true}, reuses its cached decoder on a second load and on a racing first pair, and releases it on dispose.');
console.log('KTX2 opt-in OK - {ktx2:true} still loads a file with no KTX2 texture, reuses its cached decoder on a second load, releases it on dispose, and clears a failed build so a retry gets a real second attempt.');
console.log('Raycasting OK - an intersection answers as a structure whose hit object references the handle the context already held.');
console.log('Statics OK - a utility class runs by name, answers what three.js answers, registers no handle, and an unknown class is named in the failure.');
console.log('Structures OK - a plain object round-trips as its own members, a nested math value decodes, a three.js instance is still refused.');
console.log('Promise answers OK - a batch with none stays synchronous, one with them waits for each, a rejection faults only its own row.');
console.log('OrbitControls OK - attached to the real canvas, 120 frames of camera movement, zero interop, every listener removed on detach.');
console.log(`Generated surface OK - ${generatedClassNames.length} generated classes are constructors on the vendored three.js.`);
console.log(`Escape hatch OK - '${UNWRAPPED_CLASS}' has no generated wrapper and was still constructed, mutated and read back; ${reachableClassNames.length} classes are reachable, from both sides.`);
