// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This helps with visualizing what a camera contains in its frustum. It visualizes the frustum of
/// a camera using a line segments. Based on frustum visualization in [lightgl.js shadowmap
/// example](https://github.com/evanw/lightgl.js/blob/master/tests/shadowmap.html).
/// <c>CameraHelper</c> must be a child of the scene. When the camera is transformed or its
/// projection matrix is changed, it's necessary to call the <c>update()</c> method of the
/// respective helper. The JavaScript-side <c>THREE.CameraHelper</c>.
/// </summary>
public sealed class CameraHelper : LineSegments
{
	private Camera _camera;
	private bool _isCameraWritten;

	/// <summary>Constructs a new arrow helper.</summary>
	/// <param name="camera">The camera to visualize.</param>
	public CameraHelper(Camera camera)
	{
		_camera = camera;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CameraHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CameraHelper"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.CameraHelper</c>: camera.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_camera]; }
	}

	/// <summary>
	/// The camera being visualized. Writing it records a <c>camera</c> property write once this object
	/// is attached; writing the value already held records nothing.
	/// </summary>
	public Camera Camera
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

	/// <summary>Defines the colors of the helper.</summary>
	/// <param name="frustum">The frustum line color.</param>
	/// <param name="cone">The cone line color.</param>
	/// <param name="up">The up line color.</param>
	/// <param name="target">The target line color.</param>
	/// <param name="cross">The cross line color.</param>
	public void SetColors(Color frustum, Color cone, Color up, Color target, Color cross)
	{
		RecordCall("setColors", frustum, cone, up, target, cross);
	}

	/// <summary>Updates the helper based on the projection matrix of the camera.</summary>
	public void Update()
	{
		RecordCall("update");
	}

	/// <summary>
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.CameraHelper</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_camera.AttachTo(batch);

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
			_camera.AttachTo(batch);
			batch.Set(Handle, "camera", ThreeValue.Encode(_camera));
		}
	}
}
