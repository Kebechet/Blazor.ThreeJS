// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This type of render target represents an array of textures. The JavaScript-side
/// <c>THREE.WebGLArrayRenderTarget</c>.
/// </summary>
public sealed class WebGLArrayRenderTarget : WebGLRenderTarget
{
	private readonly float _width;
	private readonly float _height;
	private readonly float _depth;
	private readonly RenderTargetOptions? _options;

	/// <summary>Creates a new WebGLArrayRenderTarget.</summary>
	/// <param name="width">the width of the render target, in pixels. Default is <c>1</c>.</param>
	/// <param name="height">the height of the render target, in pixels. Default is <c>1</c>.</param>
	/// <param name="depth">the depth/layer count of the render target. Default is <c>1</c>.</param>
	/// <param name="options">
	/// optional object that holds texture parameters for an auto-generated target texture and
	/// depthBuffer/stencilBuffer booleans. See <see cref="WebGLRenderTarget"/> for details.
	/// </param>
	public WebGLArrayRenderTarget(
		float width = 1f,
		float height = 1f,
		float depth = 1f,
		RenderTargetOptions? options = null)
		: base(options: options)
	{
		_width = width;
		_height = height;
		_depth = depth;
		_options = options;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>WebGLArrayRenderTarget</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal WebGLArrayRenderTarget(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_width = default!;
		_height = default!;
		_depth = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WebGLArrayRenderTarget</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WebGLArrayRenderTarget"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.WebGLArrayRenderTarget</c>: width, height, depth,
	/// options. An argument the caller left unspecified travels as the wire's not-supplied sentinel, or
	/// is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([_width, _height, _depth, ThreeValue.OrUnspecified(_options)]); }
	}

	/// <summary>
	/// Reads <c>isWebGLArrayRenderTarget</c> back from the JavaScript-side object. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isWebGLArrayRenderTarget</c> held.
	/// </summary>
	/// <returns>The value <c>isWebGLArrayRenderTarget</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsWebGLArrayRenderTargetAsync()
	{
		return GetAsync<bool>("isWebGLArrayRenderTarget");
	}
}
