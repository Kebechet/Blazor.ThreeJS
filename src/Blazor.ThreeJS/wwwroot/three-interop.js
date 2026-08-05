import * as THREE from './three.module.js';

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

const EULER_ORDERS = ['XYZ', 'YXZ', 'ZXY', 'ZYX', 'YZX', 'XZY'];

const contexts = new Map();
let nextContextId = 1;

export function createContext(canvas, dotNetRef) {
    const renderer = new THREE.WebGLRenderer({ canvas, antialias: true, alpha: true });
    renderer.setPixelRatio(globalThis.devicePixelRatio || 1);

    const context = {
        renderer,
        dotNetRef,
        objects: new Map(),
        sceneHandle: 0,
        cameraHandle: 0,
        isRunning: true
    };

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
            if (op.k === OP_READ) {
                results.push({ i: op.i, v: value });
            }
        } catch (error) {
            const message = String(error && error.message ? error.message : error);
            if (op.k === OP_READ) {
                results.push({ i: op.i, e: message });
                continue;
            }

            errors.push({ handle: op.h, member: op.m ?? op.t, message });
        }
    }

    return { e: errors, r: results };
}

// Exported so the wire-contract test can drive the applier directly. It only ever touches
// `context.objects`, never the renderer, so a plain `{ objects: new Map() }` is enough to run every
// op kind under Node against the vendored three.js — no WebGL, no canvas.
//
// Returns the encoded value for a read op, and nothing for every other kind.
export function applyOp(context, op) {
    switch (op.k) {
        case OP_CREATE: {
            const ctor = THREE[op.t];
            if (typeof ctor !== 'function') {
                throw new Error(`Unknown three.js type '${op.t}'`);
            }

            const args = (op.a ?? []).map(value => decode(context, value).value);
            context.objects.set(op.h, new ctor(...args));
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

            context.objects.delete(op.h);
            break;
        }
        case OP_READ: {
            const target = resolveHandle(context, op.h);
            if (typeof target[op.m] !== 'function') {
                throw new Error(`'${op.m}' is not a method on the object at handle '${op.h}'`);
            }

            const args = (op.a ?? []).map(value => decode(context, value).value);
            return encode(target[op.m](...args));
        }
        default:
            throw new Error(`Unknown op kind '${op.k}'`);
    }
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

    if (Object.prototype.hasOwnProperty.call(value, '$ref')) {
        return { value: resolveHandle(context, value.$ref), isMathValue: false };
    }

    // The "not supplied" sentinel, which only C#'s generated constructors send. It has to decode to a
    // genuine undefined and not to null: a JavaScript parameter default applies only to undefined, so
    // `new PerspectiveCamera(null, 2)` leaves fov null where `new PerspectiveCamera(undefined, 2)`
    // leaves it at three.js's own 50. Trailing arguments are trimmed on the C# side instead, so this
    // is what carries an unsupplied argument that has a supplied one after it.
    if (Object.prototype.hasOwnProperty.call(value, '$undef')) {
        return { value: undefined, isMathValue: false };
    }

    switch (value.$t) {
        case 'Vector3':
            return { value: new THREE.Vector3(value.v[0], value.v[1], value.v[2]), isMathValue: true };
        case 'Euler':
            return { value: new THREE.Euler(value.v[0], value.v[1], value.v[2], EULER_ORDERS[value.o ?? 0]), isMathValue: true };
        case 'Quaternion':
            return { value: new THREE.Quaternion(value.v[0], value.v[1], value.v[2], value.v[3]), isMathValue: true };
        case 'Color':
            return { value: new THREE.Color(value.v[0], value.v[1], value.v[2]), isMathValue: true };
        case 'Matrix4':
            // C# already stores its elements column-major, exactly as fromArray expects, so this
            // must not transpose.
            return { value: new THREE.Matrix4().fromArray(value.v), isMathValue: true };
        default:
            return { value, isMathValue: false };
    }
}

// Turns a value a read op produced into the wire form C#'s ThreeValue.Decode understands: a
// primitive passes through, one of the five hand-written math types becomes the same $t-tagged shape
// C# sends in the other direction, and undefined becomes null, since JSON has no undefined.
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
    if (type === 'number' || type === 'boolean' || type === 'string') {
        return value;
    }

    if (value.isVector3) {
        return { $t: 'Vector3', v: [value.x, value.y, value.z] };
    }

    if (value.isEuler) {
        const order = EULER_ORDERS.indexOf(value.order);
        if (order < 0) {
            throw new Error(`Euler order '${value.order}' is not one of ${EULER_ORDERS.join(', ')}`);
        }

        return { $t: 'Euler', v: [value.x, value.y, value.z], o: order };
    }

    if (value.isQuaternion) {
        return { $t: 'Quaternion', v: [value.x, value.y, value.z, value.w] };
    }

    if (value.isColor) {
        return { $t: 'Color', v: [value.r, value.g, value.b] };
    }

    if (value.isMatrix4) {
        // three.js stores elements column-major and C#'s Matrix4.Elements does too, so this must not
        // transpose — the same rule the Matrix4 decode arm above states from the other direction.
        return { $t: 'Matrix4', v: Array.from(value.elements) };
    }

    throw new Error(`A '${value.constructor ? value.constructor.name : type}' value has no wire encoding, so it cannot be read back`);
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
    for (const object of context.objects.values()) {
        if (object && typeof object.dispose === 'function') {
            object.dispose();
        }
    }

    context.objects.clear();
    context.renderer.dispose();
    contexts.delete(contextId);
}
