import * as THREE from './three.module.js';

// Wire format shared with ThreeOp.cs — the numeric kinds and the short property names
// (k/h/t/m/a/v/c) are a contract and must be changed on both sides together.
const OP_CREATE = 0;
const OP_SET = 1;
const OP_CALL = 2;
const OP_ADD = 3;
const OP_REMOVE = 4;
const OP_DISPOSE = 5;

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

    const contextId = nextContextId++;
    contexts.set(contextId, context);

    const renderLoop = () => {
        if (!context.isRunning) {
            return;
        }

        const scene = context.objects.get(context.sceneHandle);
        const camera = context.objects.get(context.cameraHandle);
        if (scene && camera) {
            renderer.render(scene, camera);
        }

        context.frameRequest = requestAnimationFrame(renderLoop);
    };

    context.frameRequest = requestAnimationFrame(renderLoop);
    return contextId;
}

export function applyBatch(contextId, ops) {
    const context = contexts.get(contextId);
    if (!context) {
        return [];
    }

    const errors = [];
    for (const op of ops) {
        try {
            applyOp(context, op);
        } catch (error) {
            errors.push({ handle: op.h, member: op.m ?? op.t, message: String(error && error.message ? error.message : error) });
        }
    }

    return errors;
}

function applyOp(context, op) {
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

    switch (value.$t) {
        case 'Vector3':
            return { value: new THREE.Vector3(value.v[0], value.v[1], value.v[2]), isMathValue: true };
        case 'Euler':
            return { value: new THREE.Euler(value.v[0], value.v[1], value.v[2], EULER_ORDERS[value.o ?? 0]), isMathValue: true };
        case 'Quaternion':
            return { value: new THREE.Quaternion(value.v[0], value.v[1], value.v[2], value.v[3]), isMathValue: true };
        case 'Color':
            return { value: new THREE.Color(value.v[0], value.v[1], value.v[2]), isMathValue: true };
        default:
            return { value, isMathValue: false };
    }
}

export function setActiveScene(contextId, sceneHandle, cameraHandle) {
    const context = contexts.get(contextId);
    if (!context) {
        return;
    }

    context.sceneHandle = sceneHandle;
    context.cameraHandle = cameraHandle;
}

export function resize(contextId, width, height) {
    const context = contexts.get(contextId);
    if (!context) {
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
    for (const object of context.objects.values()) {
        if (object && typeof object.dispose === 'function') {
            object.dispose();
        }
    }

    context.objects.clear();
    context.renderer.dispose();
    contexts.delete(contextId);
}
