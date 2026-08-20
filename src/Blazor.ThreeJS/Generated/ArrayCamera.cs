// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This type of camera can be used in order to efficiently render a scene with a predefined set of
/// cameras. This is an important performance aspect for rendering VR scenes. An instance of
/// <c>ArrayCamera</c> always has an array of sub cameras. It's mandatory to define for each sub
/// camera the <c>viewport</c> property which determines the part of the viewport that is rendered
/// with this camera. The JavaScript-side <c>THREE.ArrayCamera</c>.
/// </summary>
public sealed class ArrayCamera : PerspectiveCamera
{
	private readonly PerspectiveCamera?[]? _array;
	private PerspectiveCamera?[] _cameras = [];
	private bool _isCamerasWritten;

	/// <summary>Constructs a new array camera.</summary>
	/// <param name="array">An array of perspective sub cameras.</param>
	public ArrayCamera(PerspectiveCamera?[]? array = null)
	{
		_array = array;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ArrayCamera</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ArrayCamera(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ArrayCamera</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ArrayCamera"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ArrayCamera</c>: array. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_array)]); }
	}

	/// <summary>
	/// An array of perspective sub cameras. Writing it records a <c>cameras</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public PerspectiveCamera?[] Cameras
	{
		get { return _cameras; }
		set
		{
			if (_cameras == value)
			{
				return;
			}

			_cameras = value;
			_isCamerasWritten = true;
			AttachEach(Batch, value);

			RecordSet("cameras", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isArrayCamera</c> held.
	/// </summary>
	/// <returns>The value <c>isArrayCamera</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsArrayCameraAsync()
	{
		return GetAsync<bool>("isArrayCamera");
	}

	/// <summary>
	/// Whether this camera is used with multiview rendering or not. Read-only in three.js, so it is
	/// read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isMultiViewCamera</c> held.
	/// </summary>
	/// <returns>The value <c>isMultiViewCamera</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsMultiViewCameraAsync()
	{
		return GetAsync<bool>("isMultiViewCamera");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.ArrayCamera</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		AttachEach(batch, _array);

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

		if (_isCamerasWritten)
		{
			AttachEach(batch, _cameras);
			batch.Set(Handle, "cameras", ThreeValue.Encode(_cameras));
		}
	}
}
