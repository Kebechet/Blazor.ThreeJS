// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.StorageTexture</c>.</summary>
public sealed class StorageTexture : Texture
{
	private readonly float? _width;
	private readonly float? _height;

	/// <summary>Initializes a new <see cref="StorageTexture"/>.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> constructor argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> constructor argument.</param>
	public StorageTexture(float? width = null, float? height = null)
	{
		_width = width;
		_height = height;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>StorageTexture</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal StorageTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.StorageTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "StorageTexture"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.StorageTexture</c>: width, height. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_width),
				ThreeValue.OrUnspecified(_height)
			]);
		}
	}

	/// <summary>Records a call to <c>setSize</c> on the JavaScript-side object.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> argument.</param>
	/// <param name="depth">Value forwarded to the <c>depth</c> argument.</param>
	public void SetSize(float width, float height, float depth)
	{
		RecordCall("setSize", width, height, depth);
	}
}
