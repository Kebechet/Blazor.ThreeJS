[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/kebechet)

# Blazor.ThreeJS
[![NuGet Version](https://img.shields.io/nuget/v/Kebechet.Blazor.ThreeJS)](https://www.nuget.org/packages/Kebechet.Blazor.ThreeJS/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Kebechet.Blazor.ThreeJS)](https://www.nuget.org/packages/Kebechet.Blazor.ThreeJS/)
![Last updated](https://img.shields.io/github/last-commit/Kebechet/Blazor.ThreeJS/main?label=last%20updated)
[![Twitter](https://img.shields.io/twitter/url/https/twitter.com/samuel_sidor.svg?style=social&label=Follow%20samuel_sidor)](https://x.com/samuel_sidor)

<!-- Restore these once the infrastructure they point at exists:
[![Build](https://github.com/Kebechet/Blazor.ThreeJS/actions/workflows/build.yml/badge.svg)](https://github.com/Kebechet/Blazor.ThreeJS/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/Kebechet/Blazor.ThreeJS/graph/badge.svg)](https://codecov.io/gh/Kebechet/Blazor.ThreeJS)
[![Storybook](https://img.shields.io/badge/storybook-live%20demo-ff4785)](https://kebechet.github.io/Blazor.ThreeJS/)
-->

Blazor wrapper for three.js. Typed C# scene graph with batched interop, safe on WebAssembly, Server, and MAUI Hybrid. Ships the three.js bundle - no npm, no CDN, no manual script tags.

> **Foundation release.** This version covers a static lit scene: `Scene`, `PerspectiveCamera`, `Mesh`, `BoxGeometry`, `MeshStandardMaterial`, `AmbientLight`, `DirectionalLight`, and the `Vector3` / `Euler` / `Quaternion` / `Color` / `Matrix4` math types. The rest of the three.js surface is not wrapped yet.

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
        scene.AttachTo(threeContext.Batch);

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

        await threeContext.SetActiveSceneAsync(scene.Handle, camera.Handle);
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

## License

[MIT](LICENSE)
