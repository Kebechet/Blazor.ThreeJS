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

	/// <summary>
	/// Adopts an existing JavaScript-side <c>WebGL3DRenderTarget</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal WebGL3DRenderTarget(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_width = default!;
		_height = default!;
		_depth = default!;

		Batch = batch;
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

	/// <summary>
	/// Reads <c>isWebGL3DRenderTarget</c> back from the JavaScript-side object. Read-only in three.js,
	/// so it is read on demand rather than mirrored: records a get op, sends it behind every write
	/// already pending, and completes with the value <c>isWebGL3DRenderTarget</c> held.
	/// </summary>
	/// <returns>The value <c>isWebGL3DRenderTarget</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsWebGL3DRenderTargetAsync()
	{
		return GetAsync<bool>("isWebGL3DRenderTarget");
	}
}
