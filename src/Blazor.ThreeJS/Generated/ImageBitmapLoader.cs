// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.ImageBitmapLoader</c>.</summary>
public sealed class ImageBitmapLoader : Loader
{
	private readonly LoadingManager? _manager;

	/// <summary>Initializes a new <see cref="ImageBitmapLoader"/>.</summary>
	/// <param name="manager">Value forwarded to the <c>manager</c> constructor argument.</param>
	public ImageBitmapLoader(LoadingManager? manager = null)
	{
		_manager = manager;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ImageBitmapLoader</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ImageBitmapLoader(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ImageBitmapLoader</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ImageBitmapLoader"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ImageBitmapLoader</c>: manager. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_manager)]); }
	}

	/// <summary>
	/// Reads <c>isImageBitmapLoader</c> back from the JavaScript-side object. Read-only in three.js, so
	/// it is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isImageBitmapLoader</c> held.
	/// </summary>
	/// <returns>The value <c>isImageBitmapLoader</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsImageBitmapLoaderAsync()
	{
		return GetAsync<bool>("isImageBitmapLoader");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.ImageBitmapLoader</c> is constructed from, so their create ops
	/// reach the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_manager?.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
