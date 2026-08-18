import * as THREE from './three.webgpu.min.js';

// Wire format shared with ThreeOp.cs — the numeric kinds, the short property names (k/h/t/m/a/v/c/i),
// the response shape ({e, r} with rows of {i, v} or {i, e}) and the tagged-value keys ($t/$ref/$undef)
// are a contract and must be changed on both sides together. The C# half of each literal lives in
// ThreeWireFormat.cs, ThreeOp.cs and ThreeBatchResponse.cs.
const OP_CREATE = 0;
const OP_SET = 1;
const OP_CALL = 2;
const OP_ADD = 3;
const OP_REMOVE = 4;
const OP_DISPOSE = 5;
const OP_READ = 6;
const OP_PICK = 7;
const OP_GET = 8;

// Name of the [JSInvokable] method a pointer hit is delivered to, on the ThreeCanvas the
// DotNetObjectReference passed to createContext wraps. Part of the same contract as the op kinds
// above: renaming the C# method without changing this string breaks delivery silently.
const POINTER_HIT_CALLBACK = 'DispatchPointerEventAsync';

// Name of the [JSInvokable] method a model-load progress event is delivered to, on the
// GltfProgressReporter the caller supplied. Part of the same contract as the op kinds: renaming the
// C# method without changing this string breaks delivery silently.
const LOAD_PROGRESS_CALLBACK = 'ReportProgress';

// The one DOM event this module listens for. Deliberately not a pointer-move event: see
// syncPointerListener.
const POINTER_EVENT_NAME = 'click';

const EULER_ORDERS = ['XYZ', 'YXZ', 'ZXY', 'ZYX', 'YZX', 'XZY'];

// Tokens carrying a non-finite component of a tagged math value, shared with ThreeWireFormat.cs.
// JSON has no numeric form for one, and both runtimes get it wrong differently if left alone: C#'s
// Utf8JsonWriter throws on the way out, and JSON.stringify(Infinity) here silently yields null on the
// way back. three.js seeds an empty Box3 at ±Infinity, so this is the default case for that type, not
// an edge one. The spellings are JavaScript's own String(Infinity) etc., which is what lets
// fromComponents convert with a plain Number().
const POSITIVE_INFINITY_TOKEN = 'Infinity';
const NEGATIVE_INFINITY_TOKEN = '-Infinity';
const NAN_TOKEN = 'NaN';

// Key naming which typed array a component list should be rebuilt as, shared with ThreeWireFormat.cs.
// A plain JSON array cannot say it: three.js hands a BufferAttribute's array straight to WebGL, which
// needs the real Float32Array rather than an Array of numbers.
const TYPED_ARRAY_KEY = '$ta';

// Key of a lone non-finite number, shared with ThreeWireFormat.cs. JSON.stringify(Infinity) is null,
// so a bare number cannot carry one in either direction.
const NON_FINITE_KEY = '$n';

// Handle each context registers its renderer under, shared with ThreeWireFormat.cs. Reserved rather
// than minted so C# can address the renderer without asking what handle it got; mintHandle seeds its
// allocator below this, so nothing else can ever be given it.
const RENDERER_HANDLE = -1;

// Builds a Vector3 out of three consecutive components, which is how every composite math value below
// carries its points.
function vector3At(components, offset) {
    return new THREE.Vector3(components[offset], components[offset + 1], components[offset + 2]);
}

// Every tagged math value except Euler, which carries a rotation order as well as components and so
// stays a special case at both ends. `is` identifies one read off a three.js object, `read` flattens
// it to components, and `build` reconstructs it — the same component order as the type's C# ToArray
// and FromArray, which is the contract the two halves share.
//
// The six without an `isX` flag are matched with instanceof: three.js gives Ray, Line3, Triangle,
// Spherical, Cylindrical and Frustum no runtime tag. Note also that Box3.isEmpty and
// Triangle.isFrontFacing are prototype *methods*, so nothing here may match on an `is`-prefixed name
// without knowing it is a flag.
const MATH_VALUES = [
    { tag: 'Vector3', is: value => value.isVector3, read: value => [value.x, value.y, value.z], build: c => vector3At(c, 0) },
    { tag: 'Quaternion', is: value => value.isQuaternion, read: value => [value.x, value.y, value.z, value.w], build: c => new THREE.Quaternion(c[0], c[1], c[2], c[3]) },
    { tag: 'Color', is: value => value.isColor, read: value => [value.r, value.g, value.b], build: c => new THREE.Color(c[0], c[1], c[2]) },
    // three.js stores matrix elements column-major and C#'s Elements does too, so neither direction
    // may transpose.
    { tag: 'Matrix4', is: value => value.isMatrix4, read: value => Array.from(value.elements), build: c => new THREE.Matrix4().fromArray(c) },
    { tag: 'Matrix3', is: value => value.isMatrix3, read: value => Array.from(value.elements), build: c => new THREE.Matrix3().fromArray(c) },
    { tag: 'Vector2', is: value => value.isVector2, read: value => [value.x, value.y], build: c => new THREE.Vector2(c[0], c[1]) },
    { tag: 'Vector4', is: value => value.isVector4, read: value => [value.x, value.y, value.z, value.w], build: c => new THREE.Vector4(c[0], c[1], c[2], c[3]) },
    {
        tag: 'Box3',
        is: value => value.isBox3,
        read: value => [value.min.x, value.min.y, value.min.z, value.max.x, value.max.y, value.max.z],
        build: c => new THREE.Box3(vector3At(c, 0), vector3At(c, 3))
    },
    {
        tag: 'Box2',
        is: value => value.isBox2,
        read: value => [value.min.x, value.min.y, value.max.x, value.max.y],
        build: c => new THREE.Box2(new THREE.Vector2(c[0], c[1]), new THREE.Vector2(c[2], c[3]))
    },
    {
        tag: 'Sphere',
        is: value => value.isSphere,
        read: value => [value.center.x, value.center.y, value.center.z, value.radius],
        build: c => new THREE.Sphere(vector3At(c, 0), c[3])
    },
    {
        tag: 'Plane',
        is: value => value.isPlane,
        read: value => [value.normal.x, value.normal.y, value.normal.z, value.constant],
        build: c => new THREE.Plane(vector3At(c, 0), c[3])
    },
    {
        tag: 'Ray',
        is: value => value instanceof THREE.Ray,
        read: value => [value.origin.x, value.origin.y, value.origin.z, value.direction.x, value.direction.y, value.direction.z],
        build: c => new THREE.Ray(vector3At(c, 0), vector3At(c, 3))
    },
    {
        tag: 'Line3',
        is: value => value instanceof THREE.Line3,
        read: value => [value.start.x, value.start.y, value.start.z, value.end.x, value.end.y, value.end.z],
        build: c => new THREE.Line3(vector3At(c, 0), vector3At(c, 3))
    },
    {
        tag: 'Triangle',
        is: value => value instanceof THREE.Triangle,
        read: value => [value.a.x, value.a.y, value.a.z, value.b.x, value.b.y, value.b.z, value.c.x, value.c.y, value.c.z],
        build: c => new THREE.Triangle(vector3At(c, 0), vector3At(c, 3), vector3At(c, 6))
    },
    {
        tag: 'Spherical',
        is: value => value instanceof THREE.Spherical,
        read: value => [value.radius, value.phi, value.theta],
        build: c => new THREE.Spherical(c[0], c[1], c[2])
    },
    {
        tag: 'Cylindrical',
        is: value => value instanceof THREE.Cylindrical,
        read: value => [value.radius, value.theta, value.y],
        build: c => new THREE.Cylindrical(c[0], c[1], c[2])
    },
    {
        tag: 'Frustum',
        is: value => value instanceof THREE.Frustum,
        read: value => value.planes.flatMap(plane => [plane.normal.x, plane.normal.y, plane.normal.z, plane.constant]),
        build: c => new THREE.Frustum(...Array.from({ length: 6 }, (unused, index) =>
            new THREE.Plane(vector3At(c, index * 4), c[(index * 4) + 3])))
    },
    {
        tag: 'SphericalHarmonics3',
        is: value => value.isSphericalHarmonics3,
        read: value => value.coefficients.flatMap(coefficient => [coefficient.x, coefficient.y, coefficient.z]),
        build: c => {
            const harmonics = new THREE.SphericalHarmonics3();
            harmonics.coefficients.forEach((coefficient, index) => coefficient.set(c[index * 3], c[(index * 3) + 1], c[(index * 3) + 2]));
            return harmonics;
        }
    }
];

const MATH_VALUES_BY_TAG = new Map(MATH_VALUES.map(entry => [entry.tag, entry]));

// Turns components off the wire into numbers, restoring the non-finite ones from their tokens.
function fromComponents(components) {
    return components.map(component => typeof component === 'string' ? Number(component) : component);
}

// Turns components into their wire form, spelling the non-finite ones as tokens so JSON.stringify
// cannot quietly flatten them to null.
function toComponents(values) {
    return values.map(value => {
        if (Number.isFinite(value)) {
            return value;
        }

        if (value === Infinity) {
            return POSITIVE_INFINITY_TOKEN;
        }

        return value === -Infinity ? NEGATIVE_INFINITY_TOKEN : NAN_TOKEN;
    });
}

// The addons this module wraps. They live outside the three.js bundle, ship as their own static
// assets under wwwroot/addons, and are imported dynamically: a consumer who never loads a model
// never fetches 115 KB of loader, a caller who never opts into a decoder never fetches it either, and
// a canvas with no controls never fetches the controls either. The paths are relative to this module,
// which is what makes them resolve identically from the package's own `_content/` folder and from the
// demo.
const GLTF_LOADER_MODULE = './addons/loaders/GLTFLoader.js';
const ORBIT_CONTROLS_MODULE = './addons/controls/OrbitControls.js';
const DRACO_LOADER_MODULE = './addons/loaders/DRACOLoader.js';
const KTX2_LOADER_MODULE = './addons/loaders/KTX2Loader.js';

const contexts = new Map();
let nextContextId = 1;

// Async because WebGPURenderer's backend is: requesting a GPU adapter and device is a Promise, and
// `render()` throws outright until it has resolved. The loop below reschedules itself in a `finally`,
// so starting it before init would spin forever throwing once per frame with nothing to show for it.
//
// Where the browser has no WebGPU, the renderer falls back to a WebGL2 backend on its own and logs a
// warning saying so. Nothing here has to choose: the scene renders either way.
export async function createContext(canvas, dotNetRef) {
    const renderer = new THREE.WebGPURenderer({ canvas, antialias: true, alpha: true });
    renderer.setPixelRatio(globalThis.devicePixelRatio || 1);
    await renderer.init();

    const context = {
        renderer,
        dotNetRef,
        objects: new Map(),
        sceneHandle: 0,
        cameraHandle: 0,
        isRunning: true
    };

    // Registered like any other mirrored object so C# can write to it through the ordinary ops - which
    // is the only way to reach shadow maps, tone mapping or the clear colour, none of which is
    // addressable from a scene object.
    context.objects.set(RENDERER_HANDLE, renderer);

    applySize(context, canvas.clientWidth, canvas.clientHeight);

    const resizeObserver = new ResizeObserver(entries => {
        for (const entry of entries) {
            applySize(context, entry.contentRect.width, entry.contentRect.height);
        }
    });
    resizeObserver.observe(canvas);
    context.resizeObserver = resizeObserver;

    const contextId = nextContextId++;
    contexts.set(contextId, context);

    const renderLoop = () => {
        if (!context.isRunning) {
            return;
        }

        // Re-scheduling in a finally keeps the loop alive across a failed frame. render() can throw
        // on a shader compile failure, a lost WebGL context, or a malformed material; if the next
        // frame were only requested after a successful render, one such throw would stop the canvas
        // permanently with no signal to C#, since render-time failures never reach applyBatch's
        // error channel.
        try {
            // Controls run entirely on this side of the boundary, every frame, with no interop at
            // all: they read the pointer from the DOM and write straight into the camera three.js
            // already holds. Round-tripping that through C# would put one message per frame on a
            // Blazor Server circuit for something the browser can do alone. The cost when no
            // controls are attached is this one truthy test.
            if (context.controls) {
                context.controls.update();
            }

            const scene = context.objects.get(context.sceneHandle);
            const camera = context.objects.get(context.cameraHandle);
            if (scene && camera) {
                renderer.render(scene, camera);
            }
        } finally {
            context.frameRequest = requestAnimationFrame(renderLoop);
        }
    };

    context.frameRequest = requestAnimationFrame(renderLoop);
    return contextId;
}

export function applyBatch(contextId, ops) {
    const context = contexts.get(contextId);
    if (!context) {
        return { e: [], r: [] };
    }

    return runOps(context, ops);
}

// Runs a whole batch against an already-resolved context and reports both halves of the outcome:
// `e` carries the ops the applier rejected, `r` one row per read op.
//
// The two are kept apart because they are answered differently on the C# side. A rejected write has
// nobody awaiting it, so it goes to the OnError event; a rejected read faults the one task that
// asked for the value, and announcing it on OnError as well would report the same failure twice.
//
// Exported for the same reason applyOp is: the wire-contract test needs to drive it against a plain
// `{ objects: new Map() }`, and going through applyBatch would be vacuous, since an unknown context
// id makes it return an empty response without applying anything.
export function runOps(context, ops) {
    const errors = [];
    const results = [];
    for (const op of ops) {
        try {
            const value = applyOp(context, op);
            if (producesValue(op)) {
                results.push({ i: op.i, v: value });
            }
        } catch (error) {
            const message = String(error && error.message ? error.message : error);
            if (producesValue(op)) {
                results.push({ i: op.i, e: message });
                continue;
            }

            errors.push({ handle: op.h, member: op.m ?? op.t, message });
        }
    }

    return { e: errors, r: results };
}

// Whether an op answers with a value, and therefore belongs on a result row rather than on the error
// channel. The two kinds that do are the method read and the property read; every other kind is an
// instruction nobody is awaiting.
function producesValue(op) {
    return op.k === OP_READ || op.k === OP_GET;
}

// Exported so the wire-contract test can drive the applier directly. It only ever touches
// `context.objects`, never the renderer, so a plain `{ objects: new Map() }` is enough to run every
// op kind under Node against the vendored three.js — no WebGL, no canvas.
//
// Returns the encoded value for the two ops that produce one - a method read and a property read -
// and nothing for every other kind.
export function applyOp(context, op) {
    switch (op.k) {
        case OP_CREATE: {
            const ctor = THREE[op.t];
            if (typeof ctor !== 'function') {
                throw new Error(`Unknown three.js type '${op.t}'`);
            }

            const args = (op.a ?? []).map(value => decode(context, value).value);
            const created = new ctor(...args);
            context.objects.set(op.h, created);
            rememberHandle(context, op.h, created);
            break;
        }
        case OP_SET: {
            const target = resolveHandle(context, op.h);
            assign(context, target, op.m, op.v);
            break;
        }
        case OP_CALL: {
            const target = resolveHandle(context, op.h);
            const args = (op.a ?? []).map(value => decode(context, value).value);
            target[op.m](...args);
            break;
        }
        case OP_ADD: {
            resolveHandle(context, op.h).add(resolveHandle(context, op.c));
            break;
        }
        case OP_REMOVE: {
            resolveHandle(context, op.h).remove(resolveHandle(context, op.c));
            break;
        }
        case OP_DISPOSE: {
            const target = context.objects.get(op.h);
            if (target && typeof target.dispose === 'function') {
                target.dispose();
            }

            // A loaded root owns geometries, materials and textures that C# never created and has no
            // handle for, so nothing else would ever release them. Disposing the root releases the
            // whole graph it brought in, and retires every handle minted for it.
            releaseLoadedGraph(context, op.h);

            context.objects.delete(op.h);

            // A disposed object must stop being hit-testable, and the pointer-target map must not go
            // on holding the only reference to it.
            setPointerTarget(context, op.h, null);
            break;
        }
        case OP_PICK: {
            const target = resolveHandle(context, op.h);
            setPointerTarget(context, op.h, op.v === true ? target : null);
            break;
        }
        case OP_READ: {
            const target = resolveHandle(context, op.h);
            if (typeof target[op.m] !== 'function') {
                throw new Error(`'${op.m}' is not a method on the object at handle '${op.h}'`);
            }

            const args = (op.a ?? []).map(value => decode(context, value).value);
            return encodeResult(context, op, target[op.m](...args));
        }
        case OP_GET: {
            const target = resolveHandle(context, op.h);

            // `in` rather than an undefined check, and it walks the prototype chain, which is where a
            // three.js class declares its accessors. A property the object has not got would otherwise
            // read as undefined, encode to null, and reach C# as default(T) - a fabricated answer to a
            // question three.js never understood. A property that genuinely holds undefined still
            // answers null, which is the one case the two cannot be told apart, and is what `null`
            // means everywhere else on this wire.
            if (!(op.m in target)) {
                throw new Error(`'${op.m}' is not a property on the object at handle '${op.h}'`);
            }

            return encodeResult(context, op, target[op.m]);
        }
        default:
            throw new Error(`Unknown op kind '${op.k}'`);
    }
}

// Answers a read or a get. Normally that is the value itself; when the op asked for a handle (`n`), a
// three.js object is registered and referenced instead of being refused.
//
// Values still answer as values even under `n`: a number, a string or a tagged math value has an exact
// wire form, and handing back a handle to a Vector3 would make the caller round-trip again to read
// components C# can already hold. Only what `encode` has no form for becomes a reference.
function encodeResult(context, op, value) {
    if (op.n !== true || value === null || typeof value !== 'object' || isEncodableValue(value)) {
        return encode(value);
    }

    return {
        $ref: handleFor(context, value),
        t: describeType(value)
    };
}

// three.js's own name for an object, so C# can label what it adopted. The declared type is often a
// base and a loader may return a subclass, so this is read rather than assumed.
//
// ⚠️ `type` first, and `constructor.name` only as a fallback. This package ships three.js's MINIFIED
// build, where class names are mangled — `new BufferGeometry().constructor.name` is `'Wn'`. `type` is
// a string literal three.js assigns, so the minifier cannot touch it. Anything without one is a plain
// object, which has no meaningful name to report.
function describeType(value) {
    if (typeof value.type === 'string' && value.type.length > 0) {
        return value.type;
    }

    return value.constructor ? value.constructor.name : 'Object';
}

// Whether `encode` has a wire form for this object, which is what decides between answering with the
// value and answering with a handle to it.
function isEncodableValue(value) {
    return value.isEuler === true ||
        ArrayBuffer.isView(value) ||
        Array.isArray(value) ||
        MATH_VALUES.some(mathValue => mathValue.is(value));
}

function resolveHandle(context, handle) {
    const target = context.objects.get(handle);
    if (!target) {
        throw new Error(`Unknown handle '${handle}'`);
    }

    return target;
}

// Writes a decoded math value into the existing instance where three.js exposes one, so setting a
// position does not allocate a fresh Vector3 on every flush. A $ref handle or a primitive is always
// a plain assignment — rebinding a reference (e.g. mesh.geometry) must never fall into the copy
// branch, or it would deep-copy the referenced object instead of pointing at it.
function assign(context, target, member, encoded) {
    const decoded = decode(context, encoded);
    const current = target[member];

    if (decoded.isMathValue && current && typeof current.copy === 'function') {
        current.copy(decoded.value);
        return;
    }

    target[member] = decoded.value;
}

// Decodes a wire value and tags whether it came from a $t-tagged math value, so the caller
// (assign) knows which case it is holding instead of re-inferring it from the runtime object.
function decode(context, value) {
    if (value === null || typeof value !== 'object') {
        return { value, isMathValue: false };
    }

    // Element-wise rather than passed through whole, so an array of handles becomes an array of the
    // objects they name and an array of tagged values becomes an array of three.js instances. Never a
    // math value itself: `assign` must not copy an array over an existing one, since three.js's array
    // properties are reassigned rather than mutated in place.
    if (Array.isArray(value)) {
        return { value: value.map(element => decode(context, element).value), isMathValue: false };
    }

    if (Object.prototype.hasOwnProperty.call(value, '$ref')) {
        return { value: resolveHandle(context, value.$ref), isMathValue: false };
    }

    // A lone non-finite number. Tagged rather than sent as a bare string, or it could not be told from
    // a genuine string value.
    if (Object.prototype.hasOwnProperty.call(value, NON_FINITE_KEY)) {
        return { value: Number(value[NON_FINITE_KEY]), isMathValue: false };
    }

    // A typed array names the exact constructor three.js needs: BufferAttribute and DataTexture reject
    // a plain Array, because WebGL uploads the buffer straight to the GPU.
    if (Object.prototype.hasOwnProperty.call(value, TYPED_ARRAY_KEY)) {
        const typedArray = globalThis[value[TYPED_ARRAY_KEY]];
        if (typeof typedArray !== 'function') {
            throw new Error(`'${value[TYPED_ARRAY_KEY]}' is not a typed array constructor available on this runtime`);
        }

        return { value: typedArray.from(fromComponents(value.v)), isMathValue: false };
    }

    // The "not supplied" sentinel, which only C#'s generated constructors send. It has to decode to a
    // genuine undefined and not to null: a JavaScript parameter default applies only to undefined, so
    // `new PerspectiveCamera(null, 2)` leaves fov null where `new PerspectiveCamera(undefined, 2)`
    // leaves it at three.js's own 50. Trailing arguments are trimmed on the C# side instead, so this
    // is what carries an unsupplied argument that has a supplied one after it.
    if (Object.prototype.hasOwnProperty.call(value, '$undef')) {
        return { value: undefined, isMathValue: false };
    }

    if (value.$t === undefined) {
        return { value, isMathValue: false };
    }

    const components = fromComponents(value.v);

    // Euler is the one tagged value carrying more than components, so it is built here rather than
    // from the MATH_VALUES table.
    if (value.$t === 'Euler') {
        return { value: new THREE.Euler(components[0], components[1], components[2], EULER_ORDERS[value.o ?? 0]), isMathValue: true };
    }

    const mathValue = MATH_VALUES_BY_TAG.get(value.$t);
    if (!mathValue) {
        return { value, isMathValue: false };
    }

    return { value: mathValue.build(components), isMathValue: true };
}

// Turns a value a read op produced into the wire form C#'s ThreeValue.Decode understands: a
// primitive passes through, any of the hand-written math types in MATH_VALUES becomes the same
// $t-tagged shape C# sends in the other direction, and undefined becomes null, since JSON has no
// undefined.
//
// Anything else throws rather than being serialized. A three.js object serialized as a plain JSON
// object would arrive in C# as a bag of numbers that deserializes onto whichever fields happen to
// match — a fabricated value where the caller expects a read one. The refusal mirrors the same rule
// on the C# encode side.
function encode(value) {
    if (value === undefined || value === null) {
        return null;
    }

    const type = typeof value;
    if (type === 'number') {
        // JSON.stringify turns a non-finite number into null, which would reach C# as a value nobody
        // sent. Tagged, it survives.
        return Number.isFinite(value) ? value : { [NON_FINITE_KEY]: String(value) };
    }

    if (type === 'boolean' || type === 'string') {
        return value;
    }

    // ArrayBuffer.isView is true for every typed array and for DataView; the byteLength test excludes
    // the latter, which carries no elements to hand back.
    if (ArrayBuffer.isView(value) && typeof value.length === 'number') {
        return { [TYPED_ARRAY_KEY]: value.constructor.name, v: toComponents(Array.from(value)) };
    }

    if (Array.isArray(value)) {
        return value.map(encode);
    }

    if (value.isEuler) {
        const order = EULER_ORDERS.indexOf(value.order);
        if (order < 0) {
            throw new Error(`Euler order '${value.order}' is not one of ${EULER_ORDERS.join(', ')}`);
        }

        return { $t: 'Euler', v: toComponents([value.x, value.y, value.z]), o: order };
    }

    for (const mathValue of MATH_VALUES) {
        if (mathValue.is(value)) {
            return { $t: mathValue.tag, v: toComponents(mathValue.read(value)) };
        }
    }

    throw new Error(`A '${value.constructor ? value.constructor.name : type}' value has no wire encoding, so it cannot be read back`);
}

// Adds or removes one opted-in object, then brings the listener into line with the result. `target`
// is the three.js instance to hit-test, or null to opt the handle out.
//
// The map is created on the first opt-in rather than in createContext, so a scene nobody made
// clickable never allocates it — which is also what makes "no opted-in objects" the cheap path in
// every function below, since they all test the map first.
function setPointerTarget(context, handle, target) {
    if (target) {
        if (!context.pointerTargets) {
            context.pointerTargets = new Map();
        }

        context.pointerTargets.set(handle, target);
    } else if (context.pointerTargets) {
        context.pointerTargets.delete(handle);
    }

    syncPointerListener(context);
}

// Attaches the DOM listener exactly when there is both something to hit and somewhere to report it,
// and removes it the moment either stops being true. A context with nothing opted in has no listener
// at all, so an idle scene costs nothing — not one interop call, not one raycast, not one DOM
// callback.
//
// The event listened for is `click` and nothing else. A pointer-move listener is what hover would
// need, and hover reports every boundary crossing: on a Blazor Server circuit each of those is a
// SignalR message, at a rate the user's mouse sets rather than one this module can bound. A click is
// a deliberate, rare act, so it has no such ceiling problem. That is why moving the pointer over this
// canvas costs nothing at all rather than merely costing little — there is no code on that path.
function syncPointerListener(context) {
    const shouldListen = Boolean(context.dotNetRef) && Boolean(context.pointerTargets) && context.pointerTargets.size > 0;
    if (shouldListen === Boolean(context.pointerListener)) {
        return;
    }

    const canvas = context.renderer.domElement;
    if (!shouldListen) {
        canvas.removeEventListener(POINTER_EVENT_NAME, context.pointerListener);
        context.pointerListener = null;
        return;
    }

    context.pointerListener = event => {
        const bounds = canvas.getBoundingClientRect();
        dispatchPointerHit(
            context,
            ((event.clientX - bounds.left) / bounds.width) * 2 - 1,
            -((event.clientY - bounds.top) / bounds.height) * 2 + 1);
    };

    canvas.addEventListener(POINTER_EVENT_NAME, context.pointerListener);
}

// Hit-tests the opted-in objects against a ray through the given normalized device coordinates and
// answers with the nearest hit, or null when the ray met nothing.
//
// Exported for the same reason applyOp is: the wire-contract test drives it against the vendored
// three.js under Node, where there is no WebGL and no canvas, to prove a real ray meets a real mesh.
//
// Two properties come out of how the intersection is run. The raycaster is given only the opted-in
// objects, so the cost is set by how many objects the consumer made clickable rather than by how
// large the scene is; and the search is non-recursive, so a hit is always one of those objects
// itself, never a descendant of one. Opting an object in makes that object's own geometry clickable
// and says nothing about its children.
export function pickNearest(context, ndcX, ndcY) {
    const camera = context.objects.get(context.cameraHandle);
    if (!camera || !context.pointerTargets || context.pointerTargets.size === 0) {
        return null;
    }

    if (!context.raycaster) {
        context.raycaster = new THREE.Raycaster();
    }

    // World matrices are already current: the render loop calls renderer.render every frame, which
    // updates them for the whole graph, and a DOM event can only arrive between frames.
    // intersectObjects does not update them itself, so a caller driving this outside a render loop
    // has to.
    context.raycaster.setFromCamera(new THREE.Vector2(ndcX, ndcY), camera);

    const entries = Array.from(context.pointerTargets.entries());
    const intersections = context.raycaster.intersectObjects(entries.map(entry => entry[1]), false);
    if (intersections.length === 0) {
        return null;
    }

    // intersectObjects sorts by distance, so taking the first row is what makes a ray passing
    // through several opted-in objects produce exactly one hit rather than one per object.
    const nearest = intersections[0];
    const hitEntry = entries.find(entry => entry[1] === nearest.object);
    return { handle: hitEntry[0], point: nearest.point, distance: nearest.distance };
}

// Picks, and reports the hit to C# if there was one. A ray that met nothing sends nothing: the
// pointer crossing empty space is not news, and inventing a "missed" message for it would put
// traffic on the circuit for the most common outcome of all.
//
// Exported so the wire-contract test can drive the whole path — ray, hit, and the call C# would
// receive — against a recording stand-in for the .NET reference.
export function dispatchPointerHit(context, ndcX, ndcY) {
    const hit = pickNearest(context, ndcX, ndcY);
    if (!hit) {
        return null;
    }

    // The returned promise is deliberately left alone. It rejects for two reasons — the consumer's
    // own handler threw, or the .NET reference went away mid-teardown — and the browser console
    // naming the rejection is the only signal the first one has anywhere.
    context.dotNetRef.invokeMethodAsync(
        POINTER_HIT_CALLBACK,
        hit.handle,
        hit.point.x,
        hit.point.y,
        hit.point.z,
        hit.distance);

    return hit;
}

// ---------------------------------------------------------------------------------------------
// Handles the browser mints.
//
// Every other object in this module was created by an op C# sent, under a handle C# allocated. The
// two addons below break that: a glTF scene graph and an OrbitControls instance are built here, by
// JavaScript, and C# has to be able to name them afterwards.
//
// The two allocators are kept apart by sign rather than by agreement. C# allocates upwards from 1
// (Interlocked.Increment) and this one allocates downwards from -1, so no negotiation, no reserved
// block and no round trip is needed for them never to collide - and a handle's sign says outright
// which side made the object. ThreeObject rejects a positive handle offered as browser-minted and a
// non-positive one produced by its own allocator, so the partition is enforced on both sides rather
// than assumed.
// ---------------------------------------------------------------------------------------------

function mintHandle(context) {
    // Seeded from the reserved renderer handle rather than from zero, so the first minted handle is
    // the one below it and nothing can collide with the renderer — in a context that has no renderer
    // too, which is what the wire-format test drives.
    context.nextMintedHandle = (context.nextMintedHandle ?? RENDERER_HANDLE) - 1;
    return context.nextMintedHandle;
}

// The handle an object already answers to, minting one only if it has none.
//
// The reverse lookup is what makes minting safe on an arbitrary member. three.js returns `this` from
// most of its mutators — `action.play()`, `geometry.center()` — and the declared return type cannot be
// told apart from one that allocates: Object3D.clone and BufferGeometry.center both say `this`. Minting
// blindly would give an object C# already mirrors a second handle, and writes through one would not be
// seen through the other. Asking the table first makes both cases correct without guessing which is
// which.
//
// A WeakMap so a handle registered here never keeps an object alive: the forward table is what owns
// the reference, and when an op disposes a handle the entry here goes with the object.
function handleFor(context, object) {
    if (!context.handlesByObject) {
        context.handlesByObject = new WeakMap();
    }

    const existing = context.handlesByObject.get(object);
    if (existing !== undefined) {
        return existing;
    }

    const handle = mintHandle(context);
    context.objects.set(handle, object);
    context.handlesByObject.set(object, handle);
    return handle;
}

// Records the handle an object was created or registered under, so a later read that returns the same
// object answers with it rather than minting a second one.
function rememberHandle(context, handle, object) {
    if (!context.handlesByObject) {
        context.handlesByObject = new WeakMap();
    }

    context.handlesByObject.set(object, handle);
}

// Imports a consumer's own JavaScript module and adopts the TSL node one of its exports produces.
// This is the only route custom shading has under this renderer, which converts every material into a
// node graph and rejects `ShaderMaterial` outright.
//
// TSL is deliberately not mirrored as C# members. Its authoring surface is ~638 free functions in a
// separate bundle, and the operators that make an expression readable — `.add`, `.mul`, the swizzles —
// do not exist on any class: `addMethodChaining` grafts them onto node prototypes at runtime, and
// TypeScript only sees them through `declare module` augmentations of a `NodeElements` interface.
// Generating that surface would also erase the one distinction TSL exists to enforce, because the
// float-versus-vec3 typing lives in template-literal generics (`Node<'vec3'>`) that C# cannot carry.
// Letting the consumer write those few lines in JavaScript keeps the whole language, exactly typed by
// three.js's own definitions, at the cost of one file.
//
// `modulePath` resolves against the document rather than against this module, so a consumer names
// their shader file the way they name any other static asset of their app.
//
// Arguments go through the same decoder as an op's, so a uniform can be handed a value C# computed or
// a handle to an object C# already mirrors.
export async function loadNode(contextId, modulePath, exportName, args) {
    const context = contexts.get(contextId);
    if (!context) {
        throw new Error(`Unknown context '${contextId}'`);
    }

    const module = await import(new URL(modulePath, document.baseURI).href);
    const exported = module[exportName];
    if (exported === undefined) {
        throw new Error(`Module '${modulePath}' has no export named '${exportName}'`);
    }

    const decodedArgs = (args ?? []).map(value => decode(context, value).value);
    const node = typeof exported === 'function' ? exported(...decodedArgs) : exported;
    if (node === null || node === undefined || node.isNode !== true) {
        const produced = node === null || node === undefined ? String(node) : describeType(node);
        throw new Error(
            `Export '${exportName}' of '${modulePath}' produced ${produced} rather than a TSL node. ` +
            "A shader export returns the result of a TSL expression, or is one.");
    }

    return {
        $ref: handleFor(context, node),
        t: describeType(node)
    };
}

// Loads a glTF or GLB file and registers the graph it produced, so C# can hold the result.
//
// What comes back is one row per mirrored node: the root always, plus every named descendant.
// Unnamed nodes are left unmirrored on purpose - a name is glTF's own way of addressing a node, and
// an index into a traversal changes the moment the artist re-exports, so mirroring the unnamed ones
// would hand C# an identifier that silently stops meaning the same thing.
//
// Each row carries the node's transform read off the object the loader built, encoded exactly as a
// read op encodes one, so the C# mirror starts out holding the loader's own values rather than
// three.js's constructor defaults.
//
// Alongside the nodes, one row per animation clip the file brought along: every clip GLTFLoader
// produced is mirrored, since - unlike a node - a clip has no "unnamed and therefore unaddressable"
// case to guard against; three.js names every clip it builds from a glTF animation.
export async function loadGltf(contextId, url, progressRef, options) {
    const context = contexts.get(contextId);
    if (!context) {
        throw new Error(`Unknown context '${contextId}'`);
    }

    return loadGltfInto(context, url, progressRef, options);
}

// Exported for the same reason runOps is: the wire-contract test drives it against a plain
// `{ objects: new Map() }` and a real HTTP URL, so the fetch, the parse and the minting are all the
// real thing. Going through loadGltf would be impossible there — createContext needs a WebGL
// renderer, which Node has not got, so no context id would ever resolve.
//
// `options` opts into the two compressed-asset extensions GLTFLoader does not handle on its own:
// `options.draco` wires a DRACOLoader for `KHR_draco_mesh_compression`, `options.ktx2` wires a
// KTX2Loader for `KHR_texture_basisu`. Left undefined (the caller asked for neither), a compressed
// file rejects with GLTFLoader's own message — the decoder modules are fetched only when asked for,
// same as the loader itself, and (see getDracoLoader/getKtx2Loader) cached on the context — decoder
// instance and worker pool alike — and reused by every later opt-in load on it, rather than rebuilt
// per load.
export async function loadGltfInto(context, url, progressRef, options) {
    const { GLTFLoader } = await import(GLTF_LOADER_MODULE);
    const loader = new GLTFLoader();

    if (options?.draco) {
        loader.setDRACOLoader(await getDracoLoader(context));
    }

    if (options?.ktx2) {
        loader.setKTX2Loader(await getKtx2Loader(context));
    }

    // The only JavaScript-to-C# call the package makes during an operation, and bounded by the fetch:
    // a handful of events for a model, never per frame. Failures are swallowed on purpose — a circuit
    // that went away mid-download must not fault a load that is otherwise still going to succeed.
    const onProgress = progressRef
        ? event => {
            progressRef.invokeMethodAsync(
                LOAD_PROGRESS_CALLBACK,
                event.loaded ?? 0,
                event.lengthComputable ? (event.total ?? 0) : 0)
                .catch(() => {});
        }
        : undefined;

    const gltf = await loader.loadAsync(url, onProgress);
    return registerLoadedGraph(context, gltf);
}

// A DRACOLoader owns a worker pool (up to workerLimit workers) that only .dispose() tears down, so a
// fresh instance per load - the first shape this took - leaked one worker pool per compressed load for
// as long as the context lived. Cached on the context instead, the same place every other browser
// resource a context owns lives, and reused by every later opt-in load on that context; disposeContext
// is what retires it. Lazy rather than built alongside the context itself, so a context that never
// loads a compressed model never pays for the decoder or its worker at all.
//
// What is cached is the in-flight *promise*, written before the first await inside it rather than
// after. Two opt-in loads racing to be first on the same context both call this synchronously, so
// whichever reaches the assignment first is the only one that ever builds a DRACOLoader; the other
// sees the promise already sitting there and awaits that instead of a second `import()` + `new
// DRACOLoader()` of its own. Caching the resolved instance directly, as this first did, left a window
// between the check and the assignment - both racing calls would see nothing cached, both would build
// one, and the loser's instance, worker pool included, would be overwritten and never reached again.
function getDracoLoader(context) {
    if (!context.dracoLoaderPromise) {
        context.dracoLoaderPromise = (async () => {
            const { DRACOLoader } = await import(DRACO_LOADER_MODULE);
            context.dracoLoader = new DRACOLoader()
                .setDecoderPath(new URL('./addons/libs/draco/gltf/', import.meta.url).href);
            return context.dracoLoader;
        })();
    }

    return context.dracoLoaderPromise;
}

// Mirrors getDracoLoader: the in-flight promise is cached and reused, for the same reason - KTX2Loader
// owns a worker pool too, and the same race would orphan one. Feature detection needs a renderer to
// query, which the wire-contract test's plain `{ objects: new Map() }` context has not got; a file
// that never carries a KTX2 texture never reaches the code path that needs this, so skipping detection
// there costs nothing real.
function getKtx2Loader(context) {
    if (!context.ktx2LoaderPromise) {
        context.ktx2LoaderPromise = (async () => {
            const { KTX2Loader } = await import(KTX2_LOADER_MODULE);
            const ktx2Loader = new KTX2Loader()
                .setTranscoderPath(new URL('./addons/libs/basis/', import.meta.url).href);

            if (context.renderer) {
                ktx2Loader.detectSupport(context.renderer);
            }

            context.ktx2Loader = ktx2Loader;
            return ktx2Loader;
        })();
    }

    return context.ktx2LoaderPromise;
}

// Tears down whichever decoders getDracoLoader/getKtx2Loader cached on a context, if a compressed
// load ever ran one up. Neither lives in context.objects - they are not mirrored, browser-only state -
// so disposeContext's generic dispose loop would never reach them, and each owns a worker pool only
// .dispose() tears down. Its own function, called from disposeContext below, rather than inlined
// there: the wire-contract test drives this exact path against a plain context that never went
// through createContext, and a copy of the logic could drift from what actually runs in the browser.
//
// Checked and cleared by the resolved instance, not the promise: by the time anything calls this, a
// getDracoLoader/getKtx2Loader call that is ever going to finish already has, since nothing disposes a
// context out from under a load still in flight. Clearing both fields lets a context that goes on
// loading compressed models past this point build a fresh decoder rather than finding a disposed one.
export function disposeDecoders(context) {
    if (context.dracoLoader) {
        context.dracoLoader.dispose();
        context.dracoLoader = undefined;
        context.dracoLoaderPromise = undefined;
    }

    if (context.ktx2Loader) {
        context.ktx2Loader.dispose();
        context.ktx2Loader = undefined;
        context.ktx2LoaderPromise = undefined;
    }
}

// Mints a handle for the loaded root, for each of its named descendants, and for each animation clip
// the file brought along - in that order, so a node's handle never depends on whether the file happens
// to carry any clips. Describes all of them.
function registerLoadedGraph(context, gltf) {
    const root = gltf.scene;
    const handles = [];
    const nodes = [describeLoadedNode(context, root, handles)];
    root.traverse(object => {
        if (object === root || !object.name) {
            return;
        }

        nodes.push(describeLoadedNode(context, object, handles));
    });

    const clips = (gltf.animations ?? []).map(clip => describeLoadedClip(context, clip, handles));

    if (!context.loadedGraphs) {
        context.loadedGraphs = new Map();
    }

    context.loadedGraphs.set(nodes[0].h, { root, handles });
    return { n: nodes, a: clips };
}

// Mints a handle for one animation clip the file brought along, and describes it. A clip is not part
// of the node graph `root.traverse` walks - GLTFLoader hands the whole array back separately - so it is
// registered the same way a node is (a handle, added to the same `handles` list a graph's dispose call
// retires) without being reachable by walking `root`.
function describeLoadedClip(context, clip, handles) {
    const handle = mintHandle(context);
    context.objects.set(handle, clip);
    rememberHandle(context, handle, clip);
    handles.push(handle);
    return {
        h: handle,
        n: clip.name,
        d: clip.duration
    };
}

function describeLoadedNode(context, object, handles) {
    const handle = mintHandle(context);
    context.objects.set(handle, object);
    rememberHandle(context, handle, object);
    handles.push(handle);
    return {
        h: handle,
        n: object.name,
        t: object.type,
        p: encode(object.position),
        r: encode(object.rotation),
        s: encode(object.scale),
        v: object.visible === true
    };
}

// Releases everything one loaded graph brought in, if `handle` names a loaded root: the GPU
// resources hanging off it, and the handles minted for its nodes and its animation clips - both live
// in the same `handles` list, so one loop retires them together.
//
// The geometries, materials and textures are the part nothing else covers. A Mesh has no `dispose`,
// so the generic arm in the dispose op never reaches them, and C# never created them so no dispose
// op will ever name them either. Materials are walked property by property rather than by a list of
// map names, so a texture slot three.js adds in a later release is still released.
function releaseLoadedGraph(context, handle) {
    const graph = context.loadedGraphs?.get(handle);
    if (!graph) {
        return;
    }

    const released = new Set();
    graph.root.traverse(object => {
        releaseOnce(released, object.geometry);
        const materials = Array.isArray(object.material) ? object.material : [object.material];
        for (const material of materials) {
            releaseMaterial(released, material);
        }
    });

    for (const nodeHandle of graph.handles) {
        context.objects.delete(nodeHandle);
        setPointerTarget(context, nodeHandle, null);
    }

    context.loadedGraphs.delete(handle);
}

function releaseMaterial(released, material) {
    if (!material || released.has(material)) {
        return;
    }

    for (const value of Object.values(material)) {
        if (value && value.isTexture) {
            releaseOnce(released, value);
        }
    }

    releaseOnce(released, material);
}

function releaseOnce(released, resource) {
    if (!resource || released.has(resource) || typeof resource.dispose !== 'function') {
        return;
    }

    released.add(resource);
    resource.dispose();
}

// Attaches OrbitControls to the camera at `cameraHandle` and to this context's canvas, and mints a
// handle for the controls so C# can write their properties through ordinary Set ops.
//
// Replacing an existing set is a detach followed by an attach rather than two live sets fighting
// over the same camera: two OrbitControls on one canvas both consume the same pointer events.
export async function attachOrbitControls(contextId, cameraHandle) {
    const context = contexts.get(contextId);
    if (!context) {
        throw new Error(`Unknown context '${contextId}'`);
    }

    return attachOrbitControlsTo(context, cameraHandle);
}

// Exported for the same reason loadGltfInto is: the wire-contract test drives the real addon against
// a recording stand-in for the canvas, which is what makes "every listener it registered comes back
// off on detach" an assertion rather than a claim.
export async function attachOrbitControlsTo(context, cameraHandle) {
    const camera = resolveHandle(context, cameraHandle);
    const { OrbitControls } = await import(ORBIT_CONTROLS_MODULE);
    detachControls(context);

    const controls = new OrbitControls(camera, context.renderer.domElement);
    const handle = mintHandle(context);
    context.objects.set(handle, controls);
    context.controls = controls;
    context.controlsHandle = handle;
    return handle;
}

export function detachOrbitControls(contextId) {
    const context = contexts.get(contextId);
    if (!context) {
        return;
    }

    detachControls(context);
}

// Takes the controls off the canvas. `dispose` is what removes the pointer, wheel, context-menu and
// key listeners OrbitControls registered - on the canvas and on its owning document - so skipping it
// would leave the canvas driving a camera nothing renders any more.
//
// Exported for the same reason applyOp is: the wire-contract test drives attach and detach against
// the vendored addon to prove the listeners really come back off.
export function detachControls(context) {
    if (!context.controls) {
        return;
    }

    context.controls.dispose();
    context.objects.delete(context.controlsHandle);
    context.controls = null;
    context.controlsHandle = 0;
}

export function setActiveScene(contextId, sceneHandle, cameraHandle) {
    const context = contexts.get(contextId);
    if (!context) {
        return;
    }

    context.sceneHandle = sceneHandle;
    context.cameraHandle = cameraHandle;

    // The camera handle is only known here. createContext sized the drawing buffer while
    // context.objects was still empty and cameraHandle was 0, so it could not derive the aspect,
    // and whether the ResizeObserver happens to fire again once the camera is registered depends
    // on when the host's layout settles relative to the C# round trip — a race that lands
    // differently on WebAssembly and on Server. Re-applying the size here makes the aspect
    // deterministic; without it the projection can keep whatever aspect the consumer guessed when
    // constructing the camera.
    const canvas = context.renderer.domElement;
    applySize(context, canvas.clientWidth, canvas.clientHeight);
}

// Keeps the renderer's drawing buffer in step with the canvas element's laid-out size. CSS
// controls the displayed size, so setSize's third argument stays false — three.js must never
// overwrite the element's inline style and fight the consumer's own CSS. A canvas inside a
// display:none panel, or observed before layout, reports 0x0; setSize(0, 0) would produce a
// broken buffer and aspect = width / 0 would corrupt the projection matrix with Infinity, so
// either dimension being 0 skips the update entirely rather than applying it.
function applySize(context, width, height) {
    if (width === 0 || height === 0) {
        return;
    }

    context.renderer.setSize(width, height, false);
    const camera = context.objects.get(context.cameraHandle);
    if (camera && camera.isPerspectiveCamera) {
        camera.aspect = width / height;
        camera.updateProjectionMatrix();
    }
}

export function disposeContext(contextId) {
    const context = contexts.get(contextId);
    if (!context) {
        return;
    }

    context.isRunning = false;
    cancelAnimationFrame(context.frameRequest);
    context.resizeObserver.disconnect();

    // Before the renderer goes, since removing the listener needs its canvas. Emptying the map is
    // what takes the listener off it, through the same path an opt-out uses. OrbitControls registers
    // listeners of its own, on the canvas and on its owning document, and comes off the same way.
    if (context.pointerTargets) {
        context.pointerTargets.clear();
    }

    syncPointerListener(context);
    detachControls(context);

    // Loaded graphs first: their geometries, materials and textures hang off objects the loop below
    // cannot reach, because a Mesh has no dispose and only the nodes C# asked to mirror are in the
    // object table at all.
    if (context.loadedGraphs) {
        for (const rootHandle of Array.from(context.loadedGraphs.keys())) {
            releaseLoadedGraph(context, rootHandle);
        }
    }

    disposeDecoders(context);

    for (const object of context.objects.values()) {
        if (object && typeof object.dispose === 'function') {
            object.dispose();
        }
    }

    context.objects.clear();
    context.renderer.dispose();
    contexts.delete(contextId);
}
