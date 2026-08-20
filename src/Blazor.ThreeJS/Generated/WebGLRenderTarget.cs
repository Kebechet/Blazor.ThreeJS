// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.WebGLRenderTarget</c>.</summary>
public class WebGLRenderTarget : RenderTarget
{
	private readonly float? _width;
	private readonly float? _height;
	private readonly RenderTargetOptions? _options;

	/// <summary>Initializes a new <see cref="WebGLRenderTarget"/>.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> constructor argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> constructor argument.</param>
	/// <param name="options">Value forwarded to the <c>options</c> constructor argument.</param>
	public WebGLRenderTarget(float? width = null, float? height = null, RenderTargetOptions? options = null)
		: base(width: width, height: height, options: options)
	{
		_width = width;
		_height = height;
		_options = options;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>WebGLRenderTarget</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal WebGLRenderTarget(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WebGLRenderTarget</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WebGLRenderTarget"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.WebGLRenderTarget</c>: width, height, options. An
	/// argument the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed
	/// when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_width),
				ThreeValue.OrUnspecified(_height),
				ThreeValue.OrUnspecified(_options)
			]);
		}
	}

	/// <summary>
	/// Reads <c>isWebGLRenderTarget</c> back from the JavaScript-side object. Read-only in three.js, so
	/// it is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isWebGLRenderTarget</c> held.
	/// </summary>
	/// <returns>The value <c>isWebGLRenderTarget</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsWebGLRenderTargetAsync()
	{
		return GetAsync<bool>("isWebGLRenderTarget");
	}
}
