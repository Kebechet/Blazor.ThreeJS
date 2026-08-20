// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A loader for loading an image. Unlike other loaders, this one emits events instead of using
/// predefined callbacks. So if you're interested in getting notified when things happen, you need
/// to add listeners to the object. The JavaScript-side <c>THREE.ImageLoader</c>.
/// </summary>
public sealed class ImageLoader : Loader
{
	private readonly LoadingManager? _manager;

	/// <summary>Initializes a new <see cref="ImageLoader"/>.</summary>
	/// <param name="manager">Value forwarded to the <c>manager</c> constructor argument.</param>
	public ImageLoader(LoadingManager? manager = null)
		: base(manager: manager)
	{
		_manager = manager;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ImageLoader</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ImageLoader(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ImageLoader</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ImageLoader"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ImageLoader</c>: manager. An argument the caller
	/// left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_manager)]); }
	}

	/// <summary>
	/// Attaches the objects <c>THREE.ImageLoader</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_manager?.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
