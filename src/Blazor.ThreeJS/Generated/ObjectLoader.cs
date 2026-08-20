// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.ObjectLoader</c>.</summary>
public class ObjectLoader : Loader
{
	private readonly LoadingManager? _manager;

	/// <summary>Initializes a new <see cref="ObjectLoader"/>.</summary>
	/// <param name="manager">Value forwarded to the <c>manager</c> constructor argument.</param>
	public ObjectLoader(LoadingManager? manager = null)
		: base(manager: manager)
	{
		_manager = manager;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ObjectLoader</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ObjectLoader(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ObjectLoader</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ObjectLoader"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ObjectLoader</c>: manager. An argument the caller
	/// left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_manager)]); }
	}

	/// <summary>
	/// Reads <c>parse</c> back from the JavaScript-side object. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>parse</c> returned.
	/// </summary>
	/// <param name="json">Value forwarded to the <c>json</c> argument.</param>
	/// <returns>The value <c>parse</c> returned, once the JavaScript side has answered.</returns>
	public Task<Object3D?> ParseAsync(object? json)
	{
		return RecordReadObject<Object3D>("parse", (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D"), json);
	}

	/// <summary>
	/// Reads <c>parseAsync</c> back from the JavaScript-side object. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>parseAsync</c> returned.
	/// </summary>
	/// <param name="json">Value forwarded to the <c>json</c> argument.</param>
	/// <returns>The value <c>parseAsync</c> returned, once the JavaScript side has answered.</returns>
	public Task<Object3D?> ParseAsyncAsync(object? json)
	{
		return RecordReadObject<Object3D>("parseAsync", (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D"), json);
	}

	/// <summary>
	/// Reads <c>parseGeometries</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>parseGeometries</c> returned.
	/// </summary>
	/// <param name="json">Value forwarded to the <c>json</c> argument.</param>
	/// <returns>The value <c>parseGeometries</c> returned, once the JavaScript side has answered.</returns>
	public Task<Dictionary<string, ThreeObject>> ParseGeometriesAsync(object? json)
	{
		return RecordReadHandles<Dictionary<string, ThreeObject>>("parseGeometries", json);
	}

	/// <summary>
	/// Reads <c>parseMaterials</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>parseMaterials</c> returned.
	/// </summary>
	/// <param name="json">Value forwarded to the <c>json</c> argument.</param>
	/// <param name="textures">Value forwarded to the <c>textures</c> argument.</param>
	/// <returns>The value <c>parseMaterials</c> returned, once the JavaScript side has answered.</returns>
	public Task<Dictionary<string, Material>> ParseMaterialsAsync(object? json, Dictionary<string, Texture> textures)
	{
		return RecordReadHandles<Dictionary<string, Material>>("parseMaterials", json, textures);
	}

	/// <summary>
	/// Reads <c>parseAnimations</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>parseAnimations</c> returned.
	/// </summary>
	/// <param name="json">Value forwarded to the <c>json</c> argument.</param>
	/// <returns>The value <c>parseAnimations</c> returned, once the JavaScript side has answered.</returns>
	public Task<Dictionary<string, AnimationClip>> ParseAnimationsAsync(object? json)
	{
		return RecordReadHandles<Dictionary<string, AnimationClip>>("parseAnimations", json);
	}

	/// <summary>
	/// Reads <c>parseObject</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>parseObject</c> returned.
	/// </summary>
	/// <param name="data">Value forwarded to the <c>data</c> argument.</param>
	/// <param name="geometries">Value forwarded to the <c>geometries</c> argument.</param>
	/// <param name="materials">Value forwarded to the <c>materials</c> argument.</param>
	/// <param name="animations">Value forwarded to the <c>animations</c> argument.</param>
	/// <returns>The value <c>parseObject</c> returned, once the JavaScript side has answered.</returns>
	public Task<Object3D?> ParseObjectAsync(
		object? data,
		Dictionary<string, ThreeObject> geometries,
		Dictionary<string, Material> materials,
		Dictionary<string, AnimationClip> animations)
	{
		return RecordReadObject<Object3D>("parseObject", (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D"), data, geometries, materials, animations);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.ObjectLoader</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_manager?.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
