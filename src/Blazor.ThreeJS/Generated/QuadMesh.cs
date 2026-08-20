// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.QuadMesh</c>.</summary>
public sealed class QuadMesh : Mesh
{
	private readonly Material? _material;
	private OrthographicCamera? _camera;
	private bool _isCameraWritten;

	/// <summary>Initializes a new <see cref="QuadMesh"/>.</summary>
	/// <param name="material">Value forwarded to the <c>material</c> constructor argument.</param>
	public QuadMesh(Material? material = null)
		: base(material: material)
	{
		_material = material;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>QuadMesh</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal QuadMesh(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.QuadMesh</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "QuadMesh"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.QuadMesh</c>: material. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_material)]); }
	}

	/// <summary>
	/// The <c>camera</c> property of the JavaScript-side object. Writing it records a <c>camera</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public OrthographicCamera? Camera
	{
		get { return _camera; }
		set
		{
			if (ReferenceEquals(_camera, value))
			{
				return;
			}

			_camera = value;
			_isCameraWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("camera", value);
		}
	}

	/// <summary>
	/// Reads <c>isQuadMesh</c> back from the JavaScript-side object. Read-only in three.js, so it is
	/// read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isQuadMesh</c> held.
	/// </summary>
	/// <returns>The value <c>isQuadMesh</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsQuadMeshAsync()
	{
		return GetAsync<bool>("isQuadMesh");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.QuadMesh</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_material?.AttachTo(batch);

		base.EmitCreate(batch);
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that. A replayed
	/// value that is itself a mirrored object is attached first, so its create op reaches the batch
	/// before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isCameraWritten)
		{
			_camera?.AttachTo(batch);
			batch.Set(Handle, "camera", ThreeValue.Encode(_camera));
		}
	}
}
