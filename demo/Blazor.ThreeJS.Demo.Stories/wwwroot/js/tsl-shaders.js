// TSL shaders for the Catalogue/Shaders story, written in JavaScript because that is where TSL lives.
//
// Nothing here knows about Blazor. The module imports three.js's shader-authoring bundle exactly as
// three.js's own examples do, and each export hands back a node. C# loads them through
// `ThreeContext.LoadNodeAsync` and assigns the results to a material's node slots.
import {
    color,
    mix,
    normalLocal,
    positionLocal,
    sin,
    time,
    uniform,
    uv
// Relative, not root-absolute: the demo is served from the root by `dotnet run` and from
// /Blazor.ThreeJS/ on GitHub Pages, and only a relative specifier resolves correctly in both.
//
// ⚠️ Two levels, and the count is the whole correctness of this line. This file is served from
// `_content/Blazor.ThreeJS.Stories/js/`, so `..` reaches the library's own static-asset root and the
// second one reaches `_content/`, beside which the package's assets sit. A wrong count 404s only when
// deployed, never in a unit test - `tests/demo-base-path.test.mjs` is what fails instead.
} from '../../Kebechet.Blazor.ThreeJS/three.tsl.min.js';

// A uniform is a node whose `value` stays writable after the shader is built, which is what lets C#
// drive a live shader without recompiling it or paying interop per frame.
export function speedUniform(initialSpeed) {
    return uniform(initialSpeed);
}

// Bands of colour sweeping around the sphere. `uv().x` runs 0..1 around it, `time` advances itself on
// the GPU, and the uniform scales how fast - so C# writing that one value changes the animation.
export function auroraColour(speedNode) {
    const sweep = sin(uv().x.mul(18).add(time.mul(speedNode)))
        .mul(0.5)
        .add(0.5);

    return mix(color(0x0b1a4d), color(0x2ee6c8), sweep);
}

// The same sweep pushed along each vertex's normal, so the surface ripples rather than just recolours.
export function rippleSurface(speedNode) {
    const lift = sin(uv().x.mul(18).add(time.mul(speedNode)))
        .mul(0.08);

    return positionLocal.add(normalLocal.mul(lift));
}
