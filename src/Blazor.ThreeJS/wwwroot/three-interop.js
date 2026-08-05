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
const OP_PICK = 7;

// Name of the [JSInvokable] method a pointer hit is delivered to, on the ThreeCanvas the
// DotNetObjectReference passed to createContext wraps. Part of the same contract as the op kinds
// above: renaming the C# method without changing this string breaks delivery silently.
const POINTER_HIT_CALLBACK = 'DispatchPointerEventAsync';

// The one DOM event this module listens for. Deliberately not a pointer-move event: see
// syncPointerListener.
const POINTER_EVENT_NAME = 'click';

const EULER_ORDERS = ['XYZ', 'YXZ', 'ZXY', 'ZYX', 'YZX', 'XZY'];

// The two addons this module wraps. They live outside the three.js bundle, ship as their own static
// assets under wwwroot/addons, and are imported dynamically: a consumer who never loads a model
// never fetches 115 KB of loader, and a canvas with no controls never fetches the controls either.
// The paths are relative to this module, which is what makes them resolve identically from the
// package's own `_content/` folder and from the demo.
const GLTF_LOADER_MODULE = './addons/loaders/GLTFLoader.js';
const ORBIT_CONTROLS_MODULE = './addons/controls/OrbitControls.js';

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
    context.nextMintedHandle = (context.nextMintedHandle ?? 0) - 1;
    return context.nextMintedHandle;
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
export async function loadGltf(contextId, url) {
    const context = contexts.get(contextId);
    if (!context) {
        throw new Error(`Unknown context '${contextId}'`);
    }

    return loadGltfInto(context, url);
}

// Exported for the same reason runOps is: the wire-contract test drives it against a plain
// `{ objects: new Map() }` and a real HTTP URL, so the fetch, the parse and the minting are all the
// real thing. Going through loadGltf would be impossible there — createContext needs a WebGL
// renderer, which Node has not got, so no context id would ever resolve.
export async function loadGltfInto(context, url) {
    const { GLTFLoader } = await import(GLTF_LOADER_MODULE);
    const gltf = await new GLTFLoader().loadAsync(url);
    return registerLoadedGraph(context, gltf.scene);
}

// Mints a handle for the loaded root and for each of its named descendants, and describes them.
function registerLoadedGraph(context, root) {
    const handles = [];
    const nodes = [describeLoadedNode(context, root, handles)];
    root.traverse(object => {
        if (object === root || !object.name) {
            return;
        }

        nodes.push(describeLoadedNode(context, object, handles));
    });

    if (!context.loadedGraphs) {
        context.loadedGraphs = new Map();
    }

    context.loadedGraphs.set(nodes[0].h, { root, handles });
    return { n: nodes };
}

function describeLoadedNode(context, object, handles) {
    const handle = mintHandle(context);
    context.objects.set(handle, object);
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
// resources hanging off it, and the handles minted for its nodes.
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

    for (const object of context.objects.values()) {
        if (object && typeof object.dispose === 'function') {
            object.dispose();
        }
    }

    context.objects.clear();
    context.renderer.dispose();
    contexts.delete(contextId);
}
