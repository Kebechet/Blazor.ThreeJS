// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class is designed to assist with
/// <see href="https://en.wikipedia.org/wiki/Ray_casting">raycasting</see>. The JavaScript-side
/// <c>THREE.Raycaster</c>.
/// </summary>
/// <remarks>
/// Raycasting is used for mouse picking (working out what objects in the 3d space the mouse is
/// over) amongst other things.
/// </remarks>
/// <seealso href="https://threejs.org/examples/#webgl_interactive_cubes">Raycasting to a Mesh</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_interactive_cubes_ortho">Raycasting to a Mesh in using an OrthographicCamera</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_interactive_buffergeometry">Raycasting to a Mesh with BufferGeometry</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_instancing_raycast">Raycasting to a InstancedMesh</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_interactive_lines">Raycasting to a Line</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_interactive_raycasting_points">Raycasting to Points</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_geometry_terrain_raycast">Terrain raycasting</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_interactive_voxelpainter">Raycasting to paint voxels</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_raycaster_texture">Raycast to a Texture</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/Raycaster">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/Raycaster.js">Source</seealso>
public sealed class Raycaster : ThreeObject
{
	private readonly Vector3? _origin;
	private readonly Vector3? _direction;
	private float _near;
	private float? _far;
	private Camera? _camera;
	private Layers? _layers;
	private bool _isNearWritten;
	private bool _isFarWritten;
	private bool _isCameraWritten;
	private bool _isLayersWritten;

	/// <summary>This creates a new <see cref="Raycaster"/> object.</summary>
	/// <param name="origin">The origin vector where the ray casts from.</param>
	/// <param name="direction">The direction vector that gives direction to the ray. Should be normalized.</param>
	/// <param name="near">All results returned are further away than near. Near can't be negative.</param>
	/// <param name="far">All results returned are closer than far. Far can't be lower than near.</param>
	public Raycaster(Vector3? origin = null, Vector3? direction = null, float near = 0f, float? far = null)
	{
		_origin = origin;
		_direction = direction;
		_near = near;
		_far = far;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Raycaster</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Raycaster"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Raycaster</c>: origin, direction, near, far. An
	/// argument the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed
	/// when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_origin),
				ThreeValue.OrUnspecified(_direction),
				_near,
				ThreeValue.OrUnspecified(_far)
			]);
		}
	}

	/// <summary>
	/// The near factor of the raycaster. This value indicates which objects can be discarded based on
	/// the distance. This value shouldn't be negative and should be smaller than the far property.
	/// Writing it records a <c>near</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float Near
	{
		get { return _near; }
		set
		{
			if (_near == value)
			{
				return;
			}

			_near = value;
			_isNearWritten = true;
			RecordSet("near", value);
		}
	}

	/// <summary>
	/// The far factor of the raycaster. This value indicates which objects can be discarded based on
	/// the distance. This value shouldn't be negative and should be larger than the near property.
	/// Writing it records a <c>far</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float? Far
	{
		get { return _far; }
		set
		{
			if (_far == value)
			{
				return;
			}

			_far = value;
			_isFarWritten = true;
			RecordSet("far", value);
		}
	}

	/// <summary>
	/// The camera to use when raycasting against view-dependent objects such as billboarded objects
	/// like <c>Sprites</c>. This field can be set manually or is set when calling <c>setFromCamera</c>.
	/// Writing it records a <c>camera</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public Camera? Camera
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
	/// Used by <see cref="Raycaster"/> to selectively ignore 3D objects when performing intersection
	/// tests. The following code example ensures that only 3D objects on layer <c>1</c> will be honored
	/// by the instance of Raycaster. Writing it records a <c>layers</c> property write once this object
	/// is attached; writing the value already held records nothing.
	/// </summary>
	public Layers? Layers
	{
		get { return _layers; }
		set
		{
			if (ReferenceEquals(_layers, value))
			{
				return;
			}

			_layers = value;
			_isLayersWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("layers", value);
		}
	}

	/// <summary>Updates the ray with a new origin and direction.</summary>
	/// <param name="origin">The origin vector where the ray casts from.</param>
	/// <param name="direction">The normalized direction vector that gives direction to the ray.</param>
	public void Set(Vector3 origin, Vector3 direction)
	{
		RecordCall("set", origin, direction);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Raycaster</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isNearWritten)
		{
			batch.Set(Handle, "near", ThreeValue.Encode(_near));
		}

		if (_isFarWritten)
		{
			batch.Set(Handle, "far", ThreeValue.Encode(_far));
		}

		if (_isCameraWritten)
		{
			batch.Set(Handle, "camera", ThreeValue.Encode(_camera));
		}

		if (_isLayersWritten)
		{
			batch.Set(Handle, "layers", ThreeValue.Encode(_layers));
		}
	}
}
