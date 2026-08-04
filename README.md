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

> **Generated from `@types/three`.** 189 of three.js's 309 classes and 32 of its constant groups are wrapped, mechanically, from the upstream type declarations - so the property names, constructor argument order and documentation are three.js's own rather than a paraphrase. The `Vector3` / `Euler` / `Quaternion` / `Color` / `Matrix4` math types and the `Object3D` scene-graph base are hand-written. What is not covered, and why, is listed per class and per member in [`generator/api-coverage.md`](generator/api-coverage.md); [Wrapping a type yourself](#wrapping-a-type-yourself) is how to reach the rest in the meantime.

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

### Animating

Mutate the scene graph as you would in three.js, then call `FlushAsync` once per frame. Property
writes coalesce per object and member, so a burst of assignments in one tick costs one op - and
writing a value that is already set costs nothing at all.

```csharp
mesh.Rotation.Y += 0.01f;
await threeContext.FlushAsync();
```

Reads never leave C#, and a tick that changes nothing makes no interop call.

## Wrapping a type yourself

60 of three.js's classes are not wrapped - typed-array buffer attributes, abstract bases, types
taking a DOM handle - and you are not blocked by any of them: the three.js bundle this package ships
is the complete library, and every hook the generated types use is `protected`, so you can add the
wrapper in your own project. Derive from `Object3D` for anything that belongs in the scene graph and
it attaches, batches, and flushes exactly like a generated type.

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
package. `Position`, `Rotation`, `Scale`, and `IsVisible` are inherited from `Object3D` and are
replayed as usual, whenever you write them.

Values you pass to `RecordSet` and `RecordCall` may be any primitive, `string`, `enum`, `null`,
another wrapped object (sent as a handle reference), or one of the `Vector3` / `Euler` /
`Quaternion` / `Color` / `Matrix4` math types. Any other reference type throws
`NotSupportedException` at the call rather than silently shipping its serialized shape over a live
three.js instance.

A type outside the scene graph works the same way: derive from the generated `BufferGeometry` or
`Material` (or from `ThreeObject` directly) and assign it to `mesh.Geometry` / `mesh.Material`, which
attaches it to the same context before the property write that references it. The one thing a wrapper
outside this package cannot do is attach dependencies of its own - the hook for that is internal - so
a custom type that has to construct another wrapped object first is still out of reach.

## License

[MIT](LICENSE)
