[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/kebechet)

# Blazor.ThreeJS
[![NuGet Version](https://img.shields.io/nuget/v/Kebechet.Blazor.ThreeJS)](https://www.nuget.org/packages/Kebechet.Blazor.ThreeJS/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Kebechet.Blazor.ThreeJS)](https://www.nuget.org/packages/Kebechet.Blazor.ThreeJS/)
![Last updated](https://img.shields.io/github/last-commit/Kebechet/Blazor.ThreeJS/main?label=last%20updated)
[![Twitter](https://img.shields.io/twitter/url/https/twitter.com/samuel_sidor.svg?style=social&label=Follow%20samuel_sidor)](https://x.com/samuel_sidor)
[![Build](https://github.com/Kebechet/Blazor.ThreeJS/actions/workflows/build.yml/badge.svg)](https://github.com/Kebechet/Blazor.ThreeJS/actions/workflows/build.yml)
[![Storybook](https://img.shields.io/badge/storybook-live%20demo-ff4785)](https://kebechet.github.io/Blazor.ThreeJS/)

<!-- Restore once this repo has a Codecov token:
[![codecov](https://codecov.io/gh/Kebechet/Blazor.ThreeJS/graph/badge.svg)](https://codecov.io/gh/Kebechet/Blazor.ThreeJS)
-->

Blazor wrapper for three.js. Typed C# scene graph with batched interop, safe on WebAssembly, Server, and MAUI Hybrid. Ships the three.js bundle - no npm, no CDN, no manual script tags.

> **Generated from `@types/three`.** Most of three.js's class surface is wrapped mechanically from the upstream type declarations, so the property names, constructor argument order and documentation are three.js's own rather than a paraphrase. The `Vector3` / `Euler` / `Quaternion` / `Color` / `Matrix4` math types, the `Object3D` scene-graph base and the two addon wrappers (`GLTFLoader`, `OrbitControls`) are hand-written. [Coverage](#coverage) is exactly how much is wrapped, what is not, and how those numbers were arrived at; [Reaching a class the mirror does not wrap](#reaching-a-class-the-mirror-does-not-wrap) is how to reach the rest - untyped in three lines, or with a wrapper of your own.

## Installation

```bash
dotnet add package Kebechet.Blazor.ThreeJS
```

## Usage

Drop a `<ThreeCanvas>` on a page and build the scene in its `OnReady` callback. Everything you write
into the scene graph accumulates in the context's batch; `SetActiveSceneAsync` flushes it in a single
interop call and tells the renderer what to draw.

```razor
@using Kebechet.Blazor.ThreeJS.Components
@using Kebechet.Blazor.ThreeJS.Core
@using Kebechet.Blazor.ThreeJS.Math
@using Kebechet.Blazor.ThreeJS.Objects

<ThreeCanvas Style="width: 100%; height: 400px;" OnReady="BuildSceneAsync" />

@code {
    private async Task BuildSceneAsync(ThreeContext threeContext)
    {
        var scene = new Scene();
        threeContext.Attach(scene);

        var camera = new PerspectiveCamera(75f, 16f / 9f, 0.1f, 1000f);
        camera.Position.Set(0f, 0f, 5f);
        scene.Add(camera);

        scene.Add(new AmbientLight(Color.White, 0.4f));

        var light = new DirectionalLight(Color.White, 1f);
        light.Position.Set(5f, 5f, 5f);
        scene.Add(light);

        var material = new MeshStandardMaterial();
        material.Color.SetHex(0x3366cc);
        material.Roughness = 0.4f;

        var mesh = new Mesh(new BoxGeometry(1f, 1f, 1f), material);
        mesh.Rotation.Set(0.4f, 0.8f, 0f, EulerOrder.XYZ);
        scene.Add(mesh);

        await threeContext.SetActiveSceneAsync(scene, camera);
    }
}
```

The camera's `aspect` is re-derived from the canvas when the scene goes active and again whenever the
canvas is resized, so the value you pass to the constructor is only a starting point.

### Writing the scene as components

The same scene can be written inside the canvas instead, as a component tree. Blazor's render tree
becomes the scene graph: a component is added to whatever it is written inside, so a geometry written
inside a mesh fills that mesh's geometry slot and a mesh written inside a group becomes the group's
child. Nothing declares where it belongs - where it is written is where it goes.

```razor
@using Kebechet.Blazor.ThreeJS.Components
@using Kebechet.Blazor.ThreeJS.Math

<ThreeCanvas Style="width: 100%; height: 400px;">
    <ThreePerspectiveCamera Fov="75f" Aspect="16f / 9f" Near="0.1f" Far="1000f" Position="new(0f, 0f, 5f)" />
    <ThreeAmbientLight Color="Color.White" Intensity="0.4f" />
    <ThreeDirectionalLight Color="Color.White" Intensity="1f" Position="new(5f, 5f, 5f)" />
    <ThreeMesh Rotation="new(0.4f, _rotationY, 0f)" OnClick="Recolour">
        <ThreeBoxGeometry Width="1f" Height="1f" Depth="1f" />
        <ThreeMeshStandardMaterial Color="@_cubeColor" Roughness="0.4f" />
    </ThreeMesh>
</ThreeCanvas>
```

Animating is then an ordinary re-render: change a field, call `StateHasChanged`, and the changed
parameter is the only thing that reaches the browser. Everything the imperative API guarantees still
holds underneath, because the components write into the same mirror - **a re-render in which no
parameter changed produces no interop call at all**, which matters here because child content is a
`RenderFragment` and Blazor therefore cannot skip re-rendering a scene graph.

The two styles compose. Leaving `OnReady` on a `<ThreeCanvas>` that also has a component tree gives
you the context once the declarative scene is built and running, so you can drive it imperatively from
there.

What is shipped as components today: `<ThreeGroup>`, `<ThreeMesh>`, `<ThreePoints>`,
`<ThreePerspectiveCamera>`, `<ThreeOrthographicCamera>`, `<ThreeAmbientLight>`,
`<ThreeDirectionalLight>`, `<ThreePointLight>`, `<ThreeBoxGeometry>`, `<ThreeSphereGeometry>`,
`<ThreePlaneGeometry>`, `<ThreeMeshStandardMaterial>`, `<ThreeMeshBasicMaterial>` and
`<ThreePointsMaterial>`. Every wrapped class is still reachable imperatively through `OnReady`,
whether or not it has a component; the component set is deliberately smaller than the 143 wrapped
classes, and adding one is a matter of deriving from `ThreeNode<T>` or `ThreeObject3DNode<T>`.

Three rules are worth knowing:

- **A camera is required.** The first camera component in the markup is the one the canvas renders
  through. A scene with none has no point of view, and says so rather than rendering black.
- **A parameter three.js only takes as a constructor argument cannot change.** A box's `Width` builds
  vertices once; changing it afterwards throws rather than being quietly ignored. Put an `@key` on the
  component so a change builds a fresh object.
- **Fill a slot one way or the other.** `<ThreeMesh Geometry="@_shared">` and a `<ThreeBoxGeometry>`
  written inside it both target the same slot, and the parameter is re-applied on every render.

### Animating

Mutate the scene graph as you would in three.js, then call `FlushAsync` once per frame. Property
writes coalesce per object and member, so a burst of assignments in one tick costs one op - and
writing a value that is already set costs nothing at all.

```csharp
mesh.Rotation.Y += 0.01f;
await threeContext.FlushAsync();
```

Reads never leave C#, and a tick that changes nothing makes no interop call.

### Clicking objects

Subscribe to an object's `OnClick` and the browser starts hit-testing it. The ray is cast in
JavaScript, where the scene already lives, so C# hears only about clicks that actually landed on one
of your objects. A click on empty space, or on an object nobody subscribed to, crosses no wire at
all.

```csharp
mesh.OnClick += pointerEvent =>
{
    material.Color.SetHex(0xcc3366);
    Console.WriteLine($"hit {pointerEvent.Distance} units away, at {pointerEvent.Point.X}, {pointerEvent.Point.Y}, {pointerEvent.Point.Z}");
};
```

Subscribing is the opt-in and unsubscribing the last handler is the opt-out. Both record an op that
travels with the next flush, exactly like a property write, so subscribe while you are building the
scene - before `SetActiveSceneAsync` - or flush afterwards. Changes the handler makes to the scene
graph are flushed for you once it returns, which is what lets it be an ordinary synchronous
delegate; a handler that changes nothing costs nothing, because a flush with an empty batch makes no
call.

Only the object you subscribe on is hit-tested. Subscribing on a `Group` does not make its children
clickable - subscribe on the meshes that carry the geometry you want clicked.

**Moving the pointer costs nothing at all.** There is no `OnPointerEnter` / `OnPointerLeave`, and no
pointer-movement listener is registered anywhere: hover has to report every boundary crossing, which
on a Blazor Server circuit is one message per crossing, at a rate the user's mouse sets rather than
one this package can bound. A click is deliberate and rare, so it has no such ceiling. And a scene
with no subscriber has no click listener on the canvas either, so it costs nothing whether the
pointer moves or not.

### Loading a glTF model

`GLTFLoader` wraps three.js's own addon. The browser fetches and parses the file - that is the only
place it can happen, since parsing glTF means building buffers, geometries, materials and textures,
and none of those have a wire form. What comes back to C# is a description of the graph, so the
objects the browser made can be named from here.

```csharp
var model = await new GLTFLoader(threeContext).LoadAsync("models/figure.gltf");
scene.Add(model.Scene);

var head = model.FindNode("Head");
if (head is not null)
{
    head.Position.Z += 0.35f;
    head.OnClick += _ => Console.WriteLine("clicked the head");
}

await threeContext.FlushAsync();
```

`model.Scene` is the loaded root; `model.Nodes` is every **named** node beneath it. Nothing else is
mirrored, and that is the trade: a name is glTF's own way of addressing a node, while an unnamed one
can only be identified by its place in a traversal, which changes the next time the artist exports.
So the cost of a load is set by how much of the file its author chose to name rather than by how much
geometry it holds - and everything unmirrored still renders, it just cannot be addressed from C#.

**What a loaded object knows.** `head.Position` returns the position the **loader** gave it, not
zero: the transform of every mirrored node is read off the object in the browser and seeded into the
mirror at load time, so it is accurate the moment you receive it and mirrored normally from then on.
What the mirror was never told - the geometry, the material, the textures - has no C# object at all.
`Children` is empty on every loaded node too, including the root, because C# did not build that graph
and rebuilding it with `Add` would re-parent nodes three.js has already placed. Use `FindNode`.

**Handles.** These objects were created by the browser, so the browser allocates their handles: C#
counts up from 1 and JavaScript counts down from -1, which is why the two never collide without
having to agree on anything. Disposing the loaded root releases the whole graph it brought in -
geometries, materials and textures included, which nothing else would ever free.

Compressed-mesh and compressed-texture extensions (`KHR_draco_mesh_compression`,
`KHR_texture_basisu`) are not wired up; a file using one fails the load rather than quietly arriving
without its geometry.

### Orbiting the camera

```csharp
var controls = await OrbitControls.AttachAsync(threeContext, camera);
controls.IsDampingEnabled = true;
controls.MinDistance = 1.5f;
controls.MaxDistance = 8f;
controls.Target.Set(0f, 0.2f, 0f);
await threeContext.FlushAsync();
```

Drag to orbit, scroll to zoom, right-drag to pan. The controls run entirely in the browser and move
the camera every frame **with no interop at all**, which is the only way this can work: a drag is one
message per frame, and on a Blazor Server circuit that is a message every 16 ms for as long as the
user holds the mouse down.

**⚠️ That makes `camera.Position` stale, on purpose.** While controls are attached the camera's
transform belongs to JavaScript, and the C# mirror goes on reporting the last value C# wrote to it -
the one you passed before attaching. This is the one place in the package where the mirror is
knowingly not authoritative, and it is not papered over: nothing reads the camera back per frame,
because that is exactly the traffic the controls exist to avoid. Ask when you need to know:

```csharp
var cameraPosition = await controls.GetCameraPositionAsync();
var distance = await controls.GetDistanceAsync();
```

Each costs one interop call, at a moment you choose. `GetPolarAngleAsync` and
`GetAzimuthalAngleAsync` answer the same way. Writing `camera.Position` while controls are attached
still reaches three.js; the controls simply move the camera again on the next frame, so detach first
if you mean to place it yourself.

`DetachAsync` removes the DOM listeners three.js registered and spends the instance - further writes
to it record nothing rather than addressing a handle the browser has retired. Disposing the context
detaches too.

### `Path` and `Timer` collide with the BCL

three.js has a `Path` (a 2D curve) and a `Timer`, and so does .NET. Both wrapped types live in
`Kebechet.Blazor.ThreeJS.Objects`, so a file that imports the mirror and uses either bare name gets
`CS0104: 'Path' is an ambiguous reference` - `System.IO` and `System.Threading` are in the implicit
usings of every project, so you do not have to have imported them yourself.

The names are three.js's, and this package mirrors three.js, so they stay. One alias per file fixes
it, and only in the files that need it:

```csharp
using Kebechet.Blazor.ThreeJS.Objects;
using Path = System.IO.Path;
```

Swap the alias round - `using Path = Kebechet.Blazor.ThreeJS.Objects.Path;` - in a file that wants
the curve rather than the BCL type.

## Coverage

<!-- coverage:begin - generated by `npm run emit`; edit generator/emitter/Map/CoverageReport.cs, not this section -->

Generated from `@types/three@0.185.3`: **143 of three.js's 309 core classes** and
**32** of its constant groups, carrying 1015 public members and 171 enum members. Property names,
constructor argument order and documentation are three.js's own rather than a paraphrase - and so is
everything below, which is what that same generator run measured about itself.

A further 72 classes have no generated type and are still **reachable** untyped, by name -
[how](#reaching-what-is-not-generated). What is out of reach is what three.js does not export at all.

### Classes

| | classes |
|---|---|
| **generated** | **143** |
| blocked on something the mirror cannot express yet | 106 |
| deliberately out of the mirrored surface | 60 |
| **three.js core total** | **309** |

The blocked ones, by what blocks them:

| obstacle | classes |
|---|---|
| three.js's public barrel does not re-export it as a value, so the applier cannot reach it on `THREE` | 63 |
| a union of several real alternatives, which one C# parameter cannot express | 14 |
| a TypeScript lib or DOM type; C# holds no browser object and the wire has no encoding for one | 9 |
| a type alias that is neither a constant group nor a rename of a mapped type | 5 |
| the class is abstract, so it has no constructor to mirror | 4 |
| an array or tuple; `ThreeValue.Encode` has no array arm | 3 |
| a `src/math/**` value type beyond the five that are hand-written | 2 |
| the types re-export it but the shipped three.js bundle has no such runtime value to construct | 1 |
| the class declares more than one constructor | 1 |
| two classes share a name, and a C# namespace holds one type of a given name | 1 |
| a type parameter with neither a default nor a constraint to erase to | 1 |
| its C# base requires constructor arguments the generated class has nothing to supply | 1 |
| declared `any` / `unknown`, or with no type at all | 1 |

### Not wrapped, and not counted above

These are outside the class total, because the extractor never reads them:

| | classes | |
|---|---|---|
| addons (`examples/jsm`) | 383 | **2 wrapped by hand**: `GLTFLoader`, `OrbitControls`, each vendored as its own static asset beside the bundle. The other 381 are not - no post-processing passes, no exporters, no other loaders or controls. The generator reads none of them either way, which is why they sit outside the class total |
| the TSL / WebGPU node stack (`src/nodes`) | 118 | this package ships three.js's WebGL build, which does not include them |

And inside the total, deliberately out of the mirrored surface:

| | classes | |
|---|---|---|
| renderer internals (`src/renderers/webgl/**`, `src/renderers/webxr/**`) | 32 | the types consumers actually name (`WebGL3DRenderTarget`, `WebGLArrayRenderTarget`, `WebGLCubeRenderTarget`, `WebGLRenderTarget`, `WebGLRenderer`) are outside those directories and are generated |
| `src/math/**` value types | 27 | 5 of them ship, hand-ported (`Color`, `Euler`, `Matrix4`, `Quaternion`, `Vector3`); the other 22 - `Vector2`, `Box3`, `Plane`, `Ray` and the rest - do not. A math value is arithmetic rather than a signature: the generator has their members but not their behaviour, so each one waits on a hand port |
| `Object3D` | 1 | hand-written: it carries the scene-graph machinery - attachment, the transform, pre-attach state replay - which is behaviour rather than surface |

### ⚠️ What reads back, and what does not

The wire format has a seventh op kind, **read**: it invokes a three.js method inside the batch it
travels in and sends the return value back, so a read always observes the writes made before it. It
carries **values** - numbers, booleans, strings, and the five hand-written math types, tagged exactly as
they are sent in the other direction.

On the generated classes that reaches **58 methods**: focal length and effective field of view, elapsed
time, curve lengths, instance matrices and colours, vertex positions, layer tests. Each is emitted as
`…Async` and returns a `Task<T>`.

It does **not** carry handles, and that is what still puts members out of reach:

- **49 methods returning a three.js object** - `clone`, `getObjectByName`, `getRenderTarget`,
  `createMaterialFromType`. No op mints a handle for an object the browser created, and serializing one
  as a plain object would hand C# a plausible bag of numbers instead of a value. `GLTFLoader` is the one
  thing in the package that does mint handles for browser-made objects, and it is deliberately not an op:
  the loader reports the graph it built and C# mirrors the nodes it names. Nothing generalises that to an
  arbitrary method return, which would have to describe an object nobody asked it to describe.
- **38 methods returning or taking an array** - which is why **`Raycaster.intersectObjects` is still not
  callable**: it answers with `Intersection[]`, and the wire has no array encoding in either direction.
- **99 read-only properties** are not generated as members. The read op invokes a method, so a
  property has nothing to route through it, and exposing one as an async method would change the shape of
  the API rather than its coverage. They are still readable: `GetAsync` below reads any property by name.

A read is caller-initiated and costs one interop call. An idle scene still costs **zero** - nothing polls,
and no callback runs per frame.

### Where a generated type is narrower than three.js

- **47 of the 143 generated classes have a narrower constructor.** 113 trailing optional parameters
  whose type does not map are dropped; calling the JavaScript constructor with fewer arguments is what
  three.js is built for, so the result is a faithful subset rather than a guess. A **required** parameter
  that does not map blocks the whole class instead.
- **A colour is a `Color`.** three.js also accepts a CSS string or a hex number wherever a colour is
  taken; the hex form is covered by `Color.FromHex`, the string form is not exposed.
- **`T | T[]` maps to `T`** in 6 declared types, so a mesh with several materials is not expressible.

### Reaching what is not generated

Everything above is a limit of the **typed** surface. None of it is a limit of the package, because a
class the generator refuses is still a class three.js has:

- **`Primitive` / `PrimitiveObject3D`** construct any class the shipped bundle exports, by its three.js
  name - the same `new THREE[name](…)` the applier runs for a generated one.
- **`Set` / `Call` / `CallAsync` / `GetAsync`** reach any member of any object you hold, generated or
  not, by its three.js name. `GetAsync` reads a **property**, which is what puts the read-only ones above
  within reach.

| | classes |
|---|---|
| **generated, and typed** | **143** |
| reachable by name, untyped | 72 |
| **reachable at all** | **215** |
| not exported by three.js, so reachable by nothing | 94 |
| **three.js core total** | **309** |

⚠️ The last row is not a gap this package can close. Those 94 classes are ones three.js's own barrel does
not publish as values, so `THREE[name]` is `undefined` in the browser and there is nothing to construct -
by this package or by any other consumer of the same bundle. They are counted separately rather than
folded into the claim.

⚠️ **The escape hatch is sharper than the typed surface, on purpose.** It bypasses the generated types,
so it bypasses what they know:

- **Nothing checks the names.** A misspelled type, member or argument list is three.js's to reject, and it
  does so when the batch runs - through `OnError` for a write, and by faulting the task for a read.
- **The mirror does not learn from a raw `Set`.** `mesh.Set("visible", false)` leaves `mesh.IsVisible`
  reporting `true`, and the next typed write of `true` then records nothing. Where a typed property exists,
  use it.
- **A raw `Set` made before the object is attached replays after every typed property**, whichever order the
  two were written in. A typed property is replayed from its field, which does not know when it was set.

What it does **not** bypass: an object-valued write still attaches the object it references before the
op that names it, a call recorded before an attach is still replayed rather than dropped, writes still
coalesce per member and a call is still a barrier to that coalescing, and a value with no wire encoding is
still refused rather than shipped as a plain object. Those are properties of the batch, and the escape
hatch goes through it rather than around it.

### How this was measured

- **Classes**: every class declaration under `@types/three@0.185.3`'s `src/`, minus the excluded
  directories above, extracted into `generator/three-api.json`. three.js also exports 65 top-level
  functions, which are not classes, are not counted in the total, and are not wrapped either.
  `npm run extract:check` fails if that snapshot differs from what `@types/three` says today.
- **Generated is a class you can construct**: every one of them is a constructor on the `src/Blazor.ThreeJS/wwwroot/three.module.js`
  bundle that ships in this package, which `tests/wire-format.test.mjs` asserts name by name. A class
  three.js declares in its types but does not put on `THREE` is **blocked**, not counted.
- **Generated**: the files in `src/Blazor.ThreeJS/Generated/`, one per class or enum. `npm run emit:check`
  fails if any of them differs from what the generator produces today, or if one is left behind.
- **Reachable is a name the bundle exports**: the extractor imports `src/Blazor.ThreeJS/wwwroot/three.module.js`
  and records, per class, whether three.js puts that name on `THREE` - the runtime itself rather than a
  second reading of the types. `tests/wire-format.test.mjs` asserts the figure from **both** sides: every
  class called reachable is a constructor on that bundle, and no class it leaves out is one, so the number
  can neither overstate nor understate itself.
- **Public members**: `grep -c "^\tpublic " src/Blazor.ThreeJS/Generated/*.cs`, summed over the class files.
- **Everything else**: `generator/api-coverage.json`, written by the run that wrote this section. The
  per-class and per-member detail behind every figure, including each blocked class and each skipped
  member with its obstacle named, is in [`generator/api-coverage.md`](generator/api-coverage.md).

This section is generated by `npm run emit` and rewritten in place between its markers. Editing it by
hand is pointless: the next run overwrites it, and `npm run emit:check` fails until it matches.

<!-- coverage:end -->

## Reaching a class the mirror does not wrap

Some of three.js's classes have no generated C# type - typed-array buffer attributes, abstract bases,
types taking a DOM handle - and none of them blocks you. The three.js bundle this package ships is the
complete library, and the applier resolves a class by **name** off it, so anything three.js exports is
constructible whether a C# type exists for it or not.

`Primitive` names the class. `Set`, `Call`, `CallAsync` and `GetAsync` reach its members, by their
three.js names. All four live on `ThreeObject`, so they work on the generated types too - a property
three.js added last week is a `Set` away rather than a package release away.

```csharp
// Vector2 is a math type this package has not hand-ported, so a material's normalScale has no typed
// spelling. Passing a Primitive as a value sends it as a handle reference, and attaches it first.
var normalScale = new Primitive("Vector2", 0.5f, 0.5f);
material.Set("normalScale", normalScale);

// PositionalAudio has no generated type either: the generator refuses it because its base needs a
// constructor argument a generated subclass has nothing to supply. It belongs in the scene graph, so
// it gets the transform, the parenting and OnClick that come with Object3D.
var positionalAudio = new PrimitiveObject3D("PositionalAudio", listener);
positionalAudio.Position.Set(0f, 1f, 0f);
positionalAudio.Set("refDistance", 2f);
positionalAudio.Call("play");
scene.Add(positionalAudio);

await threeContext.FlushAsync();

// Reading back works on any object, generated or not: GetAsync reads a property, CallAsync invokes a
// method and hands its return value over.
var refDistance = await positionalAudio.GetAsync<float>("refDistance");
var roughness = await material.GetAsync<float>("roughness");
```

Use `PrimitiveObject3D` for a class that belongs in the scene graph and `Primitive` for everything
else - a geometry, a material, a texture, a curve. The difference matters: `PrimitiveObject3D` replays
a transform when it attaches, which on something that is not an `Object3D` would write `position` and
`scale` onto an object three.js never gave them to.

An object nothing in the scene graph references - a curve you only want to measure - reaches a context
through `threeContext.Attach(…)`, which takes any mirrored object rather than only a scene-graph one.

**⚠️ The escape hatch is sharper than the typed surface, and knowingly so:**

- **Nothing checks the names.** A misspelled type, member or argument list is three.js's to reject, and
  it does so when the batch runs: through `OnError` for a write, and by faulting the task for a read.
- **The mirror does not learn from a raw `Set`.** `mesh.Set("visible", false)` leaves `mesh.IsVisible`
  reporting `true`, and the next typed write of `true` then records nothing at all. Where a typed
  property exists, use it.
- **A raw `Set` made before the object is attached replays after every typed property**, whichever order
  the two were written in - a typed property is replayed from its field, which does not know when it was
  assigned.
- **A middle constructor argument cannot be left unsupplied.** The generated constructors have a wire
  sentinel for "not supplied"; this does not expose it, so pass three.js's documented default
  explicitly when an argument after it has to be supplied.
- **`GetAsync<T>` cannot check `T`.** If what three.js holds is not what you declared, the task faults -
  it never answers with a `default` the browser did not send.

What it does **not** give up, because it goes through the same batch the typed surface does: an
object-valued write still attaches the object it references before the op that names it, a call
recorded before an attach is still replayed rather than dropped, writes still coalesce per member and
a call is still a barrier to that coalescing, and a value with no wire encoding is still refused at the
call site rather than shipped as a plain object over a live three.js instance.

Only values come back over the wire - numbers, booleans, strings, and the five math types. A property
or method whose value is a three.js **object** is refused rather than serialized, so `GetAsync` reaches
`fov` but not `geometry`. And a class three.js does not export at all is out of reach for this too:
`THREE[name]` is `undefined` in the browser, and there is nothing there to construct.

## Wrapping a type yourself

When you want the class to have a real C# type - properties with names, a compiler that checks them,
IntelliSense - write the wrapper instead. Every hook the generated types use is `protected`, so it
lives in your own project. Derive from `Object3D` for anything that belongs in the scene graph and it
attaches, batches, and flushes exactly like a generated type.

Four members are all it takes. `ThreeTypeName` names the export on the `THREE` namespace,
`ConstructorArgs` supplies its constructor arguments, `RecordSet` writes a property, and `RecordCall`
invokes a method - each recorded into the same batch as the generated types, so your wrapper
coalesces and flushes exactly like they do.

`PositionalAudio` is a real example: the generator refuses it because its base class needs a
constructor argument that a generated subclass has nothing to pass, but by hand it is straightforward.

```csharp
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;

public sealed class PositionalAudio : Object3D
{
    private readonly AudioListener _listener;
    private float _refDistance = 1f;

    public PositionalAudio(AudioListener listener)
    {
        _listener = listener;
    }

    protected override string ThreeTypeName
    {
        get { return "PositionalAudio"; }
    }

    protected override object?[] ConstructorArgs
    {
        get { return [_listener]; }
    }

    public float RefDistance
    {
        get { return _refDistance; }
        set
        {
            if (_refDistance == value)
            {
                return;
            }

            _refDistance = value;
            RecordSet("refDistance", value);
        }
    }

    public void Play()
    {
        RecordCall("play");
    }
}
```

Add it to a scene like any generated type: `scene.Add(new PositionalAudio(listener));`. Passing a
generated object as a constructor argument - `_listener` above - sends it as a handle reference, so
attach it to the same context first.

The guard on the setter is the same one every generated property uses - writing a value that is
already set records nothing, which is what keeps a per-frame assignment free. Property names and
constructor argument order are three.js's own, so the
[three.js documentation](https://threejs.org/docs/) is the reference for both.

One difference from a generated type: give a custom type its initial state through `ConstructorArgs`,
not through a property write before it is attached. `RecordSet` only records once the object has
reached a context, and the replay-on-attach that covers generated properties is internal to this
package - the public `Set` is the exception, since a write with no field behind it is held and
replayed. Everything `Object3D` itself carries - `Position`, `Rotation`, `Scale`, `Quaternion`, `Up`,
`IsVisible`, `Name`, `CastShadow`, `ReceiveShadow`, `FrustumCulled`, `RenderOrder`, `Layers`, the
matrix-update flags - is inherited and replayed as usual, whenever you write it.

Values you pass to `RecordSet` and `RecordCall` may be any primitive, `string`, `enum`, `null`,
another wrapped object (sent as a handle reference), or one of the `Vector3` / `Euler` /
`Quaternion` / `Color` / `Matrix4` math types. Any other reference type throws
`NotSupportedException` at the call rather than silently shipping its serialized shape over a live
three.js instance.

A type outside the scene graph works the same way: derive from the generated `BufferGeometry` or
`Material` (or from `ThreeObject` directly) and assign it to `mesh.Geometry` / `mesh.Material`, which
attaches it to the same context before the property write that references it. What a wrapper outside
this package cannot do is attach its **own** constructor dependencies - the hook for that is internal -
so attach them yourself first with `threeContext.Attach(dependency)`, which takes any mirrored object,
or reach for `Primitive`, which does it for you.

## License

[MIT](LICENSE)
