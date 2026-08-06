// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Abstract base class for cameras. This class should always be inherited when you build a new
/// camera. The JavaScript-side <c>THREE.Camera</c>.
/// </summary>
public class Camera : Object3D
{
	private CoordinateSystem _coordinateSystem;
	private bool _isCoordinateSystemWritten;
	private bool _isViewportWritten;

	/// <summary>
	/// The <c>viewport</c> property of the JavaScript-side object. Mirrored as an instance this object
	/// owns: mutating it records a write of <c>viewport</c>.
	/// </summary>
	public Vector4 Viewport { get; }

	/// <summary>Initializes a new <see cref="Camera"/>.</summary>
	public Camera()
	{
		Viewport = new Vector4();
		Viewport.OnChange = () =>
		{
			_isViewportWritten = true;
			RecordSet("viewport", Viewport);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Camera</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Camera(ThreeBatch batch, int handle)
		: base(handle)
	{
		Viewport = new Vector4();
		Viewport.OnChange = () =>
		{
			_isViewportWritten = true;
			RecordSet("viewport", Viewport);
		};

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Camera</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Camera"; }
	}

	/// <summary>
	/// The coordinate system in which the camera is used. Writing it records a <c>coordinateSystem</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public CoordinateSystem CoordinateSystem
	{
		get { return _coordinateSystem; }
		set
		{
			if (_coordinateSystem == value)
			{
				return;
			}

			_coordinateSystem = value;
			_isCoordinateSystemWritten = true;
			RecordSet("coordinateSystem", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isCamera</c> held.
	/// </summary>
	/// <returns>The value <c>isCamera</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsCameraAsync()
	{
		return GetAsync<bool>("isCamera");
	}

	/// <summary>
	/// The flag that indicates whether the camera uses a reversed depth buffer. Read-only in three.js,
	/// so it is read on demand rather than mirrored: records a get op, sends it behind every write
	/// already pending, and completes with the value <c>reversedDepth</c> held.
	/// </summary>
	/// <returns>The value <c>reversedDepth</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> ReversedDepthAsync()
	{
		return GetAsync<bool>("reversedDepth");
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isCoordinateSystemWritten)
		{
			batch.Set(Handle, "coordinateSystem", ThreeValue.Encode(_coordinateSystem));
		}

		if (_isViewportWritten)
		{
			batch.Set(Handle, "viewport", ThreeValue.Encode(Viewport));
		}
	}
}
