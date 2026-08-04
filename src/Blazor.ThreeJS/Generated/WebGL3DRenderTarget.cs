// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Represents a three-dimensional render target. The JavaScript-side
/// <c>THREE.WebGL3DRenderTarget</c>.
/// </summary>
public sealed class WebGL3DRenderTarget : WebGLRenderTarget
{
	private readonly float _width;
	private readonly float _height;
	private readonly float _depth;

	/// <summary>Creates a new WebGL3DRenderTarget.</summary>
	/// <param name="width">the width of the render target, in pixels. Default is <c>1</c>.</param>
	/// <param name="height">the height of the render target, in pixels. Default is <c>1</c>.</param>
	/// <param name="depth">the depth of the render target. Default is <c>1</c>.</param>
	public WebGL3DRenderTarget(float width = 1f, float height = 1f, float depth = 1f)
	{
		_width = width;
		_height = height;
		_depth = depth;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WebGL3DRenderTarget</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WebGL3DRenderTarget"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.WebGL3DRenderTarget</c>: width, height, depth.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_width, _height, _depth]; }
	}
}
