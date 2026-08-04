// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class represents a cube render target. It is a special version of
/// <c>WebGLCubeRenderTarget</c> which is compatible with <c>WebGPURenderer</c>. The JavaScript-side
/// <c>THREE.CubeRenderTarget</c>.
/// </summary>
public sealed class CubeRenderTarget : RenderTarget
{
	private readonly float _size;

	/// <summary>Constructs a new cube render target.</summary>
	/// <param name="size">The size of the render target.</param>
	public CubeRenderTarget(float size = 1f)
	{
		_size = size;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CubeRenderTarget</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CubeRenderTarget"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.CubeRenderTarget</c>: size.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_size]; }
	}
}
