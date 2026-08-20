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

> **Generated from `@types/three`.** Most of three.js's class surface is wrapped mechanically from the upstream type declarations, so the property names, constructor argument order and documentation are three.js's own rather than a paraphrase. The 19 math types in `Kebechet.Blazor.ThreeJS.Math` (`Vector3`, `Color` and the rest) and the two addon wrappers (`GLTFLoader`, `OrbitControls`) are hand-written; the `Object3D` scene-graph base is hybrid - hand-written for behaviour, with a generated command and query surface beside it. [Coverage](#coverage) is exactly how much is wrapped, what is not, and how those numbers were arrived at; [Reaching a class the mirror does not wrap](#reaching-a-class-the-mirror-does-not-wrap) is how to reach the rest - untyped in three lines, or with a wrapper of your own.

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

**The two styles compose through `@ref`, not through the context.** `OnReady` still fires on a canvas
that has a component tree - after the declarative scene is built and running - and it is how you reach
the `ThreeContext` to flush, to subscribe to `OnError`, or to attach `OrbitControls`. What it does not
hand you is the scene: the scene root is internal, and `SetActiveSceneAsync` would *replace* the
declarative scene rather than let you into it. The route into a declaratively built object is an
`@ref` on the component that owns it, whose `Object` property **is** the mirrored object.

```razor
@using Kebechet.Blazor.ThreeJS.Addons
@using Kebechet.Blazor.ThreeJS.Components
@using Kebechet.Blazor.ThreeJS.Core
@using Kebechet.Blazor.ThreeJS.Math

<ThreeCanvas Style="width: 100%; height: 400px;" OnReady="DriveTheSceneAsync">
    <ThreePerspectiveCamera @ref="_cameraNode" Fov="75f" Position="new(0f, 0f, 5f)" />
    <ThreeAmbientLight Color="Color.White" Intensity="0.6f" />
    <ThreeMesh @ref="_meshNode">
        <ThreeBoxGeometry Width="1f" Height="1f" Depth="1f" />
        <ThreeMeshStandardMaterial Color="Color.White" />
    </ThreeMesh>
</ThreeCanvas>

@code {
    private ThreePerspectiveCamera? _cameraNode;
    private ThreeMesh? _meshNode;

    private async Task DriveTheSceneAsync(ThreeContext threeContext)
    {
        if (_cameraNode is null || _meshNode is null)
        {
            return;
        }

        await OrbitControls.AttachAsync(threeContext, _cameraNode.Object);

        _meshNode.Object.Rotation.Y = 0.4f;
        await threeContext.FlushAsync();
    }
}
```

`Object` is built when the component initializes, so it is there by the time `OnReady` fires; reading
it earlier throws rather than answering with a placeholder. One thing to keep in mind: a parameter
written in the markup is re-applied on **every** render, so an imperative write to a value the markup
also sets is overwritten by the next one. Leave the parameter off - as `Rotation` is left off the mesh
above - for state you mean to drive from C#.

What is shipped as components today: `<ThreeGroup>`, `<ThreeMesh>`, `<ThreePoints>`,
`<ThreePerspectiveCamera>`, `<ThreeOrthographicCamera>`, `<ThreeAmbientLight>`,
`<ThreeDirectionalLight>`, `<ThreePointLight>`, `<ThreeBoxGeometry>`, `<ThreeSphereGeometry>`,
`<ThreePlaneGeometry>`, `<ThreeMeshStandardMaterial>`, `<ThreeMeshBasicMaterial>` and
`<ThreePointsMaterial>`. Every wrapped class is still reachable imperatively through `OnReady`,
whether or not it has a component; the component set is deliberately smaller than the 194 wrapped
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

### When a C# value is authoritative

The C# objects are a **mirror** of the three.js objects rather than a live view of them - nothing is
read back unless you ask for it - so it is worth being precise about when what C# reports is the truth.

> **A C# value is authoritative exactly when C# last wrote it through a typed member.** It is stale
> when JavaScript wrote it (`OrbitControls`), when C# wrote it by a route the mirror does not track (a
> raw `Set`, or a command like `lookAt`), or when nothing mirrored it at all (a loaded geometry).

Each of those three is documented where you meet it - [orbiting the camera](#orbiting-the-camera),
[the escape hatch](#reaching-a-class-the-mirror-does-not-wrap), and
[what a loaded object knows](#loading-a-gltf-model). What they have in common is the rule above.

⚠️ **A stale value is worse than a wrong reading, because writing it back records nothing.** Every
typed property elides a write of the value it already holds - that is what makes a per-frame
assignment free - so assigning the value C# still believes is current is a no-op, and three.js keeps
the one it got from elsewhere. Write a *different* value, or use the read-back the relevant section
names.

Inside the typed surface this has one shape: a three.js **command** that writes state the mirror also
holds as a **property**. `Object3D.LookAt` is the one on the scene-graph base - it orients the object
without touching `Rotation` or `Quaternion`. The generated classes carry nine more, each a `SetX`
beside an `X`: `Audio.SetLoopStart` / `LoopStart`, `Audio.SetLoopEnd` / `LoopEnd`,
`FileLoader.SetResponseType` / `ResponseType`, `FileLoader.SetMimeType` / `MimeType`,
`Loader.SetCrossOrigin` / `CrossOrigin`, `Loader.SetWithCredentials` / `WithCredentials`,
`Loader.SetPath` / `Path`, `Loader.SetResourcePath` / `ResourcePath`, and
`UniformsGroup.SetUsage` / `Usage`. Each one says so in its own documentation. Where a property
exists, write the property.

A glTF model is **not** a fourth case. `LoadedObject3D` seeds its transform from the browser at load
time and mirrors it normally from then on, so those values are authoritative; what it does not have is
a `Children` list, and that is a coverage limit rather than a staleness one.

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
geometries, materials and textures included, which nothing else would ever free. Every clip handle
under `model.Animations` is retired at the same time, for the same reason: nothing else would ever
release them either.

**Playing a clip the file brought along.** `model.Animations` is every `LoadedAnimationClip` the file
carries, and `model.FindClip("name")` finds one by its glTF name the way `FindNode` finds a node. No
new playback machinery is needed - a `LoadedAnimationClip.Clip` is a plain `AnimationClip`, so it plays
through the same `AnimationMixer` any other clip does:

```csharp
var mixer = new AnimationMixer(model.Scene);
threeContext.Attach(mixer);

var spin = model.FindClip("Spin");
if (spin is not null)
{
    var action = await mixer.ClipActionAsync(spin.Clip, model.Scene, AnimationBlendMode.NormalAnimationBlendMode);
    await action!.PlayAsync();
}
```

`mixer.Update(deltaTime)` still has to be called every frame to advance playback. A `PeriodicTimer`
loop that calls it and then `threeContext.FlushAsync()` - the pattern both `Animation.stories.razor`
and `AnimatedModel.stories.razor` use - costs no extra interop: the call rides along with whatever the
frame is already flushing, rather than adding a round trip of its own.

`LoadedAnimationClip` wraps rather than *is* an `AnimationClip`: its `Name` and `Duration` are plain
`string`/`float` properties read once from the load response, not the generated class's own `Name`/
`Duration`, because those are constructor-argument state on `AnimationClip` that an adopted instance
has no way to seed without writing them back to the browser as property sets - a round trip that could
only confirm values the browser just reported. Reading `spin.Name`/`spin.Duration` costs nothing;
`spin.Clip` is what a mixer or any other API expecting an `AnimationClip` takes.

**Compressed meshes and textures are opt-in.** `KHR_draco_mesh_compression` and `KHR_texture_basisu`
each need a decoder addon and its own worker/wasm payload - hundreds of kilobytes nobody should pay
for on a load that never uses them - so `GLTFLoader` decodes neither unless asked. Pass a
`GLTFLoadOptions` to opt in:

```csharp
var model = await new GLTFLoader(threeContext).LoadAsync(
    "models/box-draco.gltf",
    new GLTFLoadOptions { IsDracoEnabled = true });
```

`IsKtx2Enabled` opts into `KHR_texture_basisu` the same way, wiring a `KTX2Loader` instead of a
`DRACOLoader`. Each flag fetches its own decoder module only on the load that sets it. Leave both at
their default `false` and nothing changes from before: a compressed file still fails the load with the
browser's own message rather than quietly arriving without its geometry or textures -
`demo/Blazor.ThreeJS.Stories/Stories/CompressedModel.stories.razor` shows the opt-in that avoids it, and
`tests/wire-format.test.mjs` proves both the failure and the opt-in against the real decoder.

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
the one you passed before attaching. This is the JavaScript-wrote-it case of
[the authority rule](#when-a-c-value-is-authoritative), and it is not papered over: nothing reads the
camera back per frame, because that is exactly the traffic the controls exist to avoid. Ask when you
need to know:

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

Generated from `@types/three@0.185.3`: **227 of three.js's 309 core classes** and
**47** of its constant groups, carrying 2319 public members and 294 enum members. Property names,
constructor argument order and documentation are three.js's own rather than a paraphrase - and so is
everything below, which is what that same generator run measured about itself.

A further 37 public members are generated onto `Object3D`, which is hand-written and so is **not**
one of those 227 classes - its command and query surface is emitted as the other half of a
`partial class`. So `Generated/**` carries 2356 public members in all, and this headline claims
only the ones that sit on a class the generator made.

A further 37 classes have no generated type and are still **reachable** untyped, by name -
[how](#reaching-what-is-not-generated). What is out of reach is what three.js does not export at all.

### Classes

| | classes |
|---|---|
| **generated** | **227** |
| — of which abstract, so a base and a parameter type rather than something to construct | 3 |
| blocked on something the mirror cannot express yet | 29 |
| deliberately out of the mirrored surface | 53 |
| **three.js core total** | **309** |

The blocked ones, by what blocks them:

| obstacle | classes |
|---|---|
| three.js's public barrel does not re-export it as a value, so the applier cannot reach it on `THREE` | 15 |
| the class is abstract, so it has no constructor to mirror | 3 |
| a TypeScript lib or DOM type; C# holds no browser object and the wire has no encoding for one | 3 |
| two classes share a name, and a C# namespace holds one type of a given name | 3 |
| the types re-export it but the shipped three.js bundle has no such runtime value to construct | 1 |
| declared under `src/nodes/**`, the TSL / WebGPU node stack outside the extracted surface | 1 |
| a type parameter with neither a default nor a constraint to erase to | 1 |
| a type alias that is neither a constant group nor a rename of a mapped type | 1 |
| a union of several real alternatives in a position that holds one type — a property or a return type, since a required parameter becomes one overload per arm | 1 |

### Not wrapped, and not counted above

These are outside the class total, because the extractor never reads them:

| | classes | |
|---|---|---|
| addons (`examples/jsm`) | 383 | **2 wrapped by hand**: `GLTFLoader`, `OrbitControls`, each vendored as its own static asset beside the bundle. The other 381 are not wrapped - no post-processing passes, no exporters, no other controls. `DRACOLoader` and `KTX2Loader` are vendored too, but only as decoder dependencies `GLTFLoader` wires in when `GLTFLoadOptions` opts a load into one, never as a class a consumer constructs directly. The generator reads none of them either way, which is why they sit outside the class total |
| the TSL / WebGPU node stack (`src/nodes`) | 118 | the shipped bundle **does** carry them - the renderer is `WebGPURenderer` and every material it draws is a node graph - but they are outside the surface the extractor reads, and deliberately so: TSL's operators are grafted onto node prototypes at runtime and its typing lives in TypeScript generics no C# signature carries, so a mirror of it would be a lossy shadow. `ThreeContext.LoadNodeAsync` reaches the real thing instead |

And inside the total, deliberately out of the mirrored surface:

| | classes | |
|---|---|---|
| renderer internals (`src/renderers/webgl/**`, `src/renderers/webxr/**`) | 32 | the types consumers actually name (`CubeRenderTarget`, `RenderTarget`, `WebGL3DRenderTarget`, `WebGLArrayRenderTarget`, `WebGLRenderTarget`, `WebGPURenderer`) are outside those directories and are generated |
| `src/math/**` value types | 20 | 20 of them ship, hand-ported (`Box2`, `Box3`, `Color`, `Cylindrical`, `Euler`, `Frustum`, `Line3`, `Matrix2`, `Matrix3`, `Matrix4`, `Plane`, `Quaternion`, `Ray`, `Sphere`, `Spherical`, `SphericalHarmonics3`, `Triangle`, `Vector2`, `Vector3`, `Vector4`); the other 0 do not: none. A math value is arithmetic rather than a signature: the generator has their members but not their behaviour, so each one waits on a hand port |
| `Object3D` | 1 | **hybrid**: hand-written for behaviour, generated for surface. The hand-written part carries the scene-graph machinery - attachment, the transform, pre-attach state replay; a generated `partial` beside it carries three.js's `Object3D` commands and queries (`RotateX`, `Attach`, `GetObjectByNameAsync`, …). Not counted as generated above, because no generator makes the type itself |

### ⚠️ What reads back, and what does not

Two of the wire format's op kinds answer: **read** invokes a three.js method, and **get** reads a
property. Both travel inside the batch they were recorded in, so either always observes the writes made
before it, and both are generated as `…Async` methods returning a task.

They answer in two ways. A **value** comes back as itself - numbers, booleans, strings, and the 20
hand-written math types, tagged exactly as they are sent in the other direction. An **object** cannot:
serializing one would hand C# a plausible bag of numbers instead of a value. So the applier registers it
under a handle of its own and answers with a reference to that handle instead, which is what makes
`renderer.shadowMap` and `mesh.CloneAsync()` reachable at all.

On the generated classes that reaches **552 members**:

- **436 answer with a value** - 250 methods (focal length and effective field of view, elapsed time,
  curve lengths, instance matrices and colours, vertex positions, layer tests) and 186 read-only
  properties (`uuid`, `instanceCount`, and three.js's own `isMesh`-style type tags). A read-only property
  is read on demand rather than mirrored, because three.js is the only side that ever assigns it: a C#
  property would imply the mirror knew the value without asking.
- **105 answer with a mirrored object** - `Task<T?>` over the generated type, adopted under the handle the
  applier registered it beneath. A handle this context already mirrors resolves back to that same C#
  object rather than to a second wrapper of it - which is what makes a method returning its own
  receiver safe - and `null` means the member genuinely held none.
- **1 answer with an object no generated class mirrors** - `Task<Primitive?>`, the same untyped wrapper
  the escape hatch hands out. The handle is real and writable; nothing type-checks the members you name
  on it. Adoption dedupes here on the same terms as above, and a handle this context mirrors as
  something *other* than a `Primitive` faults instead of being wrapped a second time - that mirror is the
  better answer and the caller is already holding it.
- **10 answer nothing at all** - a bare `Task`, awaited for *when* rather than for what.
  three.js declares these as returning a promise (`renderer.clearAsync`, `renderer.waitForGPU`), and the
  promise settles when the GPU has finished rather than when the call returned - so the applier waits
  for it before answering the row. Recording them as call ops would apply just as well and complete
  immediately, which is the one thing their name says they do not do.

What remains out of reach is out for reasons a handle does not fix:

- **44 members taking or returning a JavaScript callback** - the wire carries ops in one direction only,
  so there is nothing to call back into C# with.
- **43 members typed as a DOM or TypeScript lib type** - C# holds no `HTMLCanvasElement` to hand over,
  and a handle names a three.js object rather than an arbitrary browser one.
- **27 members that are not instance API** - 16 of them static, which the mirror has no handle to
  address because a static belongs to the class rather than to any object it holds, and 11
  declared `protected` or `private`, which three.js does not offer a consumer in the first place.

A read is caller-initiated and costs one interop call. An idle scene still costs **zero** - nothing polls,
and no callback runs per frame.

### Blocked, but still reachable

The 29 blocked classes account for themselves as follows:

- **15 are absent from the shipped three.js bundle.** Not a mapping decision: `THREE[name]` is
  `undefined` for every one of them, so nothing could construct them — not a generated class, and not the
  escape hatch either.
- **9 lose no capability**, listed below. They are abstract bases whose concrete subclasses all
  generate, convenience subclasses that only rearrange constructor arguments, or classes the untyped
  escape hatch constructs by name.
- **5 are neither**, and are a genuine gap nobody has written a route to yet.

A blocked class is therefore not automatically a missing feature, and a count on its own implies it is.

| class | how to get the same result |
|---|---|
| `CompressedArrayTexture` | `new Primitive("CompressedArrayTexture", mipmaps, width, height, depth)` |
| `Controls` | abstract in three.js; `OrbitControls` ships as a hand-written addon |
| `Curve` | abstract in three.js; every concrete curve (`LineCurve`, `SplineCurve`, `CatmullRomCurve3`, …) generates |
| `GLBufferAttribute` | `new Primitive("GLBufferAttribute", buffer, type, itemSize, elementSize, count)` — it takes a raw WebGL buffer |
| `KeyframeTrack` | abstract in three.js; all six concrete tracks (`VectorKeyframeTrack`, `NumberKeyframeTrack`, …) generate |
| `PMREMGenerator` | `new Primitive("PMREMGenerator", renderer)` — two three.js classes share this name |
| `Source` | `new Primitive("Source", data)` |
| `VideoTexture` | `new Primitive("VideoTexture", videoElement)` — it takes an `HTMLVideoElement`, which C# never holds |

⚠️ 6 entries above describe classes that are no longer blocked and should be removed from
`EmitterConfig.BlockedClassWorkarounds`: `CompressedCubeTexture`, `CompressedTexture`, `DataTextureLoader`, `InstancedInterleavedBuffer`, `Light`, `Uniform`.

### Where a generated type is narrower than three.js

- **50 of the 227 generated classes have a narrower constructor.** 78 trailing optional parameters
  whose type does not map are dropped; calling the JavaScript constructor with fewer arguments is what
  three.js is built for, so the result is a faithful subset rather than a guess. A **required** parameter
  that does not map blocks the whole class instead.
- **2 generated methods declare more than one *TypeScript* overload upstream, and only the first
  signature is emitted.** Unrelated to the arm overloads below, which come from one signature.
- **2 constructor declarations on 2 classes could not chain to a generated base and
  are not emitted.** A union-armed parameter becomes one declaration per arm, and where the class
  passes that parameter up to a base, only the arms the base can hold survive. The class is emitted
  with the declarations that work rather than blocked over the ones that do not:
  - `StorageBufferAttribute(float array, float itemSize)`
  - `StorageInstancedBufferAttribute(float array, float itemSize)`
- **A colour is a `Color`.** three.js also accepts a CSS string or a hex number wherever a colour is
  taken; the hex form is covered by `Color.FromHex`, the string form is not exposed.
- **`T | T[]` maps to `T`** in 14 declared types, so a mesh with several materials is not expressible.
- **A union in a required parameter becomes one overload per arm** - 6 members carry
  12 signatures between them, so `BufferGeometry.SetIndex` takes either a `BufferAttribute` or an
  `int[]`. Two costs: an **optional** union parameter is dropped instead, because every overload would
  accept the same argument-omitting call and none could win it; and where two arms are reference types
  anything that converts to both is ambiguous (CS0121) — `SetIndex(null)` and `SetFromPoints([])` do not
  compile, and need a cast (`SetIndex((int[]?) null)`) or a named argument.

### Reaching what is not generated

Everything above is a limit of the **typed** surface. None of it is a limit of the package, because a
class the generator refuses is still a class three.js has:

- **`Primitive` / `PrimitiveObject3D`** construct any class the shipped bundle exports, by its three.js
  name - the same `new THREE[name](…)` the applier runs for a generated one.
- **`Set` / `Call` / `CallAsync` / `GetAsync`** reach any member of any object you hold, generated or
  not, by its three.js name. `GetAsync` reads a **property**, which is what puts the read-only ones above
  within reach.
- **`GetObjectAsync` / `CallObjectAsync`** are those two again for a member whose answer is an **object**:
  the applier registers it and hands back a `Primitive` you can write to, which is how a nested object no
  dotted path addresses - `renderer.shadowMap` - is reached.

| | classes |
|---|---|
| **generated, and typed** | **227** |
| reachable by name, untyped | 37 |
| **reachable at all** | **264** |
| not exported by three.js, so reachable by nothing | 45 |
| **three.js core total** | **309** |

⚠️ The last row is not a gap this package can close. Those 45 classes are ones three.js's own barrel does
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
- **⚠️ A lone array argument needs an `(object?)` cast.** `Call`, `CallAsync`, `CallObjectAsync`,
  `new Primitive(…)`, `new PrimitiveObject3D(…)` and `ThreeContext.LoadNodeAsync` all take
  `params object?[]`, and C# array covariance makes a **reference-type** array convertible to it — so
  `Call("setFromPoints", points)` binds `points` as the whole argument list and three.js receives one
  argument per point. Write `Call("setFromPoints", (object?) points)`. No overload can fix this: the
  non-expanded form wins on an identity conversion, so it would still be chosen. A **value-type** array
  (`float[]`, `int[]`) is unaffected, having no covariant conversion to `object?[]`. The generated
  classes carry the cast already; this is a limit of the escape hatch, and of the workaround column above.

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
- **Generated is a class the bundle carries**: every one of them is a constructor on the `src/Blazor.ThreeJS/wwwroot/three.webgpu.min.js`
  bundle that ships in this package, which `tests/wire-format.test.mjs` asserts name by name. A class
  three.js declares in its types but does not put on `THREE` is **blocked**, not counted. Most of them
  are also a class *you* can construct; the abstract ones counted above are not, and are emitted as
  abstract C# classes because a base and a parameter type is what they are for.
- **Generated**: the files in `src/Blazor.ThreeJS/Generated/`, one per class or enum. `npm run emit:check`
  fails if any of them differs from what the generator produces today, or if one is left behind.
- **Reachable is a name the bundle exports**: the extractor imports `src/Blazor.ThreeJS/wwwroot/three.webgpu.min.js`
  and records, per class, whether three.js puts that name on `THREE` - the runtime itself rather than a
  second reading of the types. `tests/wire-format.test.mjs` asserts the figure from **both** sides: every
  class called reachable is a constructor on that bundle, and no class it leaves out is one, so the number
  can neither overstate nor understate itself.
- **Public members**: `grep -c "^\tpublic " src/Blazor.ThreeJS/Generated/*.cs`, summed over the class files.
  The headline splits that sum, because one of those files is the generated half of a hand-written
  class rather than a class the generator made: its members are counted, and counted separately.
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

`Primitive` names the class. `Set`, `Call`, `CallAsync`, `GetAsync`, `GetObjectAsync` and
`CallObjectAsync` reach its members, by their three.js names. All six live on `ThreeObject`, so they
work on the generated types too - a property three.js added last week is a `Set` away rather than a
package release away.

```csharp
// PositionalAudio has no generated type: the generator refuses it because its base needs a
// constructor argument a generated subclass has nothing to supply. It belongs in the scene graph, so
// it gets the transform, the parenting and OnClick that come with Object3D. `listener` here is
// itself a mirrored object - passing it as a constructor argument sends it as a handle reference and
// attaches it first, the same way a Primitive passed as any other value would.
var positionalAudio = new PrimitiveObject3D("PositionalAudio", listener);
positionalAudio.Position.Set(0f, 1f, 0f);
positionalAudio.Set("refDistance", 2f);
positionalAudio.Call("play");
scene.Add(positionalAudio);

await threeContext.FlushAsync();

// Reading back works on any object, generated or not: GetAsync reads a property, CallAsync invokes a
// method and hands its return value over.
var refDistance = await positionalAudio.GetAsync<float>("refDistance");
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

Only values come back over the wire - numbers, booleans, strings, and the hand-ported math types in
`Kebechet.Blazor.ThreeJS.Math`. A property or method whose value is a three.js **object** is refused
rather than serialized, so `GetAsync` reaches `fov` but not `geometry` - `GetObjectAsync` and
`CallObjectAsync` reach `geometry` instead, answering with the object under its own handle as an
untyped `Primitive`. Reading the same member twice answers with the same wrapper both times, since a
handle this context already mirrors resolves back to that mirror rather than being wrapped again - and
where the mirror it resolves to is **not** a `Primitive`, because you built that object in C# or loaded
it from a glTF, the read faults instead of handing you an untyped second wrapper whose writes the typed
mirror would never see. And a class three.js does not export at all is out of reach for this too:
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
another wrapped object (sent as a handle reference), or one of the hand-written math types in
`Kebechet.Blazor.ThreeJS.Math` (`Vector3`, `Color` and the rest of the 19). Any other reference type
throws `NotSupportedException` at the call rather than silently shipping its serialized shape over a
live three.js instance.

A type outside the scene graph works the same way: derive from the generated `BufferGeometry` or
`Material` (or from `ThreeObject` directly) and assign it to `mesh.Geometry` / `mesh.Material`, which
attaches it to the same context before the property write that references it. What a wrapper outside
this package cannot do is attach its **own** constructor dependencies - the hook for that is internal -
so attach them yourself first with `threeContext.Attach(dependency)`, which takes any mirrored object,
or reach for `Primitive`, which does it for you.

## Running the storybook

Every example above is a story in [the live storybook](https://kebechet.github.io/Blazor.ThreeJS/),
which is the WebAssembly host published to GitHub Pages:

```powershell
dotnet run --project demo/Blazor.ThreeJS.Demo
```

The same stories run over a **Blazor Server** circuit from a second host:

```powershell
dotnet run --project demo/Blazor.ThreeJS.Demo.Server
```

Both reference `demo/Blazor.ThreeJS.Stories`, so the stories are one set of files and neither host can
drift from the other. The Server one is what makes "safe on Server" checkable rather than argued:
`tests/Blazor.ThreeJS.E2E/ServerCircuitTests.cs` sweeps every story over the circuit, where each op is
a SignalR message and every pointer callback is a round trip. Its own
[README](demo/Blazor.ThreeJS.Demo.Server/README.md) records what a Server host has to do differently,
and the one upstream BlazingStory bug that affects its shell page but not its stories.

## License

[MIT](LICENSE)
