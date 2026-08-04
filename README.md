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

> **Foundation release.** This version covers a static lit scene: `Scene`, `Group`, `PerspectiveCamera`, `Mesh`, `Points`, `BoxGeometry`, `MeshStandardMaterial`, `PointsMaterial`, `AmbientLight`, `DirectionalLight`, the `Side` enum, and the `Vector3` / `Euler` / `Quaternion` / `Color` / `Matrix4` math types. The rest of the three.js surface is not wrapped yet - see [Wrapping a type yourself](#wrapping-a-type-yourself) for how to reach it in the meantime.

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

The wrapped surface is small, but you are not blocked by it: the three.js bundle this package ships
is the complete library, and every hook needed to drive an unwrapped type is `protected`, so you can
add the wrapper in your own project. Derive from `Object3D` for anything that belongs in the scene
graph - a light, a camera, a helper - and it attaches, batches, and flushes like a built-in type.

Four members are all it takes. `ThreeTypeName` names the export on the `THREE` namespace,
`ConstructorArgs` supplies its constructor arguments, `RecordSet` writes a property, and `RecordCall`
invokes a method - each recorded into the same batch as the built-in types, so your wrapper coalesces
and flushes exactly like they do.

```csharp
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;

public sealed class SpotLight : Object3D
{
    private readonly Color _color;
    private readonly float _intensity;
    private float _angle = 1.05f;

    public SpotLight(Color color, float intensity)
    {
        _color = color;
        _intensity = intensity;
    }

    protected override string ThreeTypeName
    {
        get { return "SpotLight"; }
    }

    protected override object?[] ConstructorArgs
    {
        get { return [_color.GetHex(), _intensity]; }
    }

    public float Angle
    {
        get { return _angle; }
        set
        {
            if (_angle == value)
            {
                return;
            }

            _angle = value;
            RecordSet("angle", value);
        }
    }

    public void LookAt(float x, float y, float z)
    {
        RecordCall("lookAt", x, y, z);
    }
}
```

Add it to a scene like any built-in type: `scene.Add(new SpotLight(Color.White, 2f));`.

The guard on the setter is the same one every built-in property uses - writing a value that is
already set records nothing, which is what keeps a per-frame assignment free. Property names and
constructor argument order are three.js's own, so the
[three.js documentation](https://threejs.org/docs/) is the reference for both.

One difference from a built-in type: give a custom type its initial state through `ConstructorArgs`,
not through a property write before it is attached. `RecordSet` only records once the object has
reached a context, and the replay-on-attach that covers built-in properties is internal to this
package. `Position`, `Rotation`, `Scale`, and `IsVisible` are inherited from `Object3D` and are
replayed as usual, whenever you write them.

Values you pass to `RecordSet` and `RecordCall` may be any primitive, `string`, `enum`, `null`,
another wrapped object (sent as a handle reference), or one of the `Vector3` / `Euler` /
`Quaternion` / `Color` / `Matrix4` math types. Any other reference type throws
`NotSupportedException` at the call rather than silently shipping its serialized shape over a live
three.js instance.

What this does not yet cover is a type that is not part of the scene graph - a geometry, a material,
a texture. Those reach the renderer only through the object that owns them, and that wiring is
internal to this package, so a custom one cannot be attached from outside yet.

## License

[MIT](LICENSE)
