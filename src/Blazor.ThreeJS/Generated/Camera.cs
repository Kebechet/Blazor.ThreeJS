// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Abstract base class for cameras. This class should always be inherited when you build a new
/// camera. The JavaScript-side <c>THREE.Camera</c>.
/// </summary>
public class Camera : Object3D
{
	private CoordinateSystem _coordinateSystem;
	private bool _isCoordinateSystemWritten;

	/// <summary>Initializes a new <see cref="Camera"/>.</summary>
	public Camera()
	{
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
	}
}
