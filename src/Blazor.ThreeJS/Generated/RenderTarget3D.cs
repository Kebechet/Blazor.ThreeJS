// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.RenderTarget3D</c>.</summary>
public sealed class RenderTarget3D : RenderTarget
{
	private readonly float? _width;
	private readonly float? _height;
	private readonly float? _depth;

	/// <summary>Initializes a new <see cref="RenderTarget3D"/>.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> constructor argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> constructor argument.</param>
	/// <param name="depth">Value forwarded to the <c>depth</c> constructor argument.</param>
	public RenderTarget3D(float? width = null, float? height = null, float? depth = null)
		: base(width: width, height: height)
	{
		_width = width;
		_height = height;
		_depth = depth;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>RenderTarget3D</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal RenderTarget3D(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.RenderTarget3D</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "RenderTarget3D"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.RenderTarget3D</c>: width, height, depth. An
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
				ThreeValue.OrUnspecified(_depth)
			]);
		}
	}

	/// <summary>
	/// Reads <c>isRenderTarget3D</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isRenderTarget3D</c> held.
	/// </summary>
	/// <returns>The value <c>isRenderTarget3D</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsRenderTarget3DAsync()
	{
		return GetAsync<bool>("isRenderTarget3D");
	}
}
