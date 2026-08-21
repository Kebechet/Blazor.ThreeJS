using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;

namespace Kebechet.Blazor.ThreeJS.Addons;

/// <summary>
/// Orbit, zoom and pan a camera with the pointer, wrapping three.js's own <c>OrbitControls</c> addon.
/// <para>
/// The controls run entirely in the browser. They read the pointer from the DOM and write straight
/// into the camera three.js already holds, once per frame, with no interop at all — which is the only
/// way this can work: a drag is one message per frame, and on a Blazor Server circuit that is a
/// message every 16 ms for as long as the user holds the mouse down.
/// </para>
/// <para>
/// ⚠️ <b>That makes the camera's mirror stale, on purpose.</b> While controls are attached, the
/// camera's transform is JavaScript's to write. <c>camera.Position</c> goes on reporting whatever C#
/// last wrote to it — the value you passed before attaching, most likely — and not where the camera
/// actually is. A C# value is authoritative exactly when C# last wrote it through a typed member, and
/// this is the case where JavaScript wrote it instead. It is not papered over: nothing reads the
/// camera back per frame, because doing so would reintroduce exactly the per-frame traffic the
/// controls exist to avoid. When you need the real figures, ask for them:
/// <see cref="GetCameraPositionAsync"/>, <see cref="GetDistanceAsync"/>,
/// <see cref="GetPolarAngleAsync"/> and <see cref="GetAzimuthalAngleAsync"/> each cost one interop
/// call, at a moment you choose.
/// </para>
/// <para>
/// Writing <c>camera.Position</c> while controls are attached still works and still reaches three.js;
/// the controls simply move the camera again on the next frame. Detach first if you mean to place the
/// camera yourself.
/// </para>
/// <para>
/// Properties below record into the context's batch exactly like any other mirrored property, so they
/// travel with the next flush. The addon module is fetched on the first attach and not before.
/// </para>
/// </summary>
public sealed class OrbitControls : ThreeObject
{
	private readonly ThreeContext _context;
	private readonly Object3D _camera;
	private bool _isEnabled = true;
	private float _minDistance;
	private float _maxDistance = float.PositiveInfinity;
	private float _minPolarAngle;
	private float _maxPolarAngle = MathF.PI;
	private bool _isDampingEnabled;
	private float _dampingFactor = 0.05f;
	private bool _isZoomEnabled = true;
	private float _zoomSpeed = 1f;
	private bool _isRotateEnabled = true;
	private float _rotateSpeed = 1f;
	private bool _isPanEnabled = true;
	private float _panSpeed = 1f;
	private bool _isAutoRotateEnabled;
	private float _autoRotateSpeed = 2f;

	/// <summary>The camera these controls drive.</summary>
	public Object3D Camera
	{
		get { return _camera; }
	}

	/// <summary>
	/// The point the camera orbits around and looks at. Mirrored as an instance these controls own:
	/// mutating it records a write of <c>target</c>. Starts at the origin, as three.js's does.
	/// </summary>
	public Vector3 Target { get; } = new();

	/// <summary>
	/// Whether the controls respond to the pointer at all. Setting this to <see langword="false"/>
	/// freezes the camera where it is without detaching, which also stops the mirror drifting further.
	/// </summary>
	public bool IsEnabled
	{
		get { return _isEnabled; }
		set
		{
			if (_isEnabled == value)
			{
				return;
			}

			_isEnabled = value;
			RecordSet("enabled", value);
		}
	}

	/// <summary>How close the camera may be dollied in. Defaults to 0.</summary>
	public float MinDistance
	{
		get { return _minDistance; }
		set
		{
			if (_minDistance == value)
			{
				return;
			}

			_minDistance = ThrowIfNaN(value, nameof(MinDistance));
			RecordSet("minDistance", value);
		}
	}

	/// <summary>
	/// How far the camera may be dollied out. Defaults to <see cref="float.PositiveInfinity"/>, as
	/// three.js's does, and writing infinity back restores that unbounded default: the wire carries it
	/// as a tagged token, so it survives the trip JSON alone could not spell.
	/// </summary>
	public float MaxDistance
	{
		get { return _maxDistance; }
		set
		{
			if (_maxDistance == value)
			{
				return;
			}

			_maxDistance = ThrowIfNaN(value, nameof(MaxDistance));
			RecordSet("maxDistance", value);
		}
	}

	/// <summary>How far the camera may orbit upwards, in radians. Defaults to 0.</summary>
	public float MinPolarAngle
	{
		get { return _minPolarAngle; }
		set
		{
			if (_minPolarAngle == value)
			{
				return;
			}

			_minPolarAngle = ThrowIfNaN(value, nameof(MinPolarAngle));
			RecordSet("minPolarAngle", value);
		}
	}

	/// <summary>How far the camera may orbit downwards, in radians. Defaults to <see cref="MathF.PI"/>.</summary>
	public float MaxPolarAngle
	{
		get { return _maxPolarAngle; }
		set
		{
			if (_maxPolarAngle == value)
			{
				return;
			}

			_maxPolarAngle = ThrowIfNaN(value, nameof(MaxPolarAngle));
			RecordSet("maxPolarAngle", value);
		}
	}

	/// <summary>
	/// Whether the camera keeps drifting to a stop after the pointer is released. Off by default, as in
	/// three.js. Costs nothing extra here: the per-frame update the damping needs already runs.
	/// </summary>
	public bool IsDampingEnabled
	{
		get { return _isDampingEnabled; }
		set
		{
			if (_isDampingEnabled == value)
			{
				return;
			}

			_isDampingEnabled = value;
			RecordSet("enableDamping", value);
		}
	}

	/// <summary>How quickly the drift decays when <see cref="IsDampingEnabled"/> is set. Defaults to 0.05.</summary>
	public float DampingFactor
	{
		get { return _dampingFactor; }
		set
		{
			if (_dampingFactor == value)
			{
				return;
			}

			_dampingFactor = ThrowIfNaN(value, nameof(DampingFactor));
			RecordSet("dampingFactor", value);
		}
	}

	/// <summary>Whether the wheel and pinch gestures dolly the camera. Defaults to <see langword="true"/>.</summary>
	public bool IsZoomEnabled
	{
		get { return _isZoomEnabled; }
		set
		{
			if (_isZoomEnabled == value)
			{
				return;
			}

			_isZoomEnabled = value;
			RecordSet("enableZoom", value);
		}
	}

	/// <summary>How far one wheel notch dollies. Defaults to 1.</summary>
	public float ZoomSpeed
	{
		get { return _zoomSpeed; }
		set
		{
			if (_zoomSpeed == value)
			{
				return;
			}

			_zoomSpeed = ThrowIfNaN(value, nameof(ZoomSpeed));
			RecordSet("zoomSpeed", value);
		}
	}

	/// <summary>Whether dragging orbits the camera. Defaults to <see langword="true"/>.</summary>
	public bool IsRotateEnabled
	{
		get { return _isRotateEnabled; }
		set
		{
			if (_isRotateEnabled == value)
			{
				return;
			}

			_isRotateEnabled = value;
			RecordSet("enableRotate", value);
		}
	}

	/// <summary>How far a drag orbits. Defaults to 1.</summary>
	public float RotateSpeed
	{
		get { return _rotateSpeed; }
		set
		{
			if (_rotateSpeed == value)
			{
				return;
			}

			_rotateSpeed = ThrowIfNaN(value, nameof(RotateSpeed));
			RecordSet("rotateSpeed", value);
		}
	}

	/// <summary>Whether a right-drag or two-finger drag pans the camera. Defaults to <see langword="true"/>.</summary>
	public bool IsPanEnabled
	{
		get { return _isPanEnabled; }
		set
		{
			if (_isPanEnabled == value)
			{
				return;
			}

			_isPanEnabled = value;
			RecordSet("enablePan", value);
		}
	}

	/// <summary>How far a pan drag moves the camera. Defaults to 1.</summary>
	public float PanSpeed
	{
		get { return _panSpeed; }
		set
		{
			if (_panSpeed == value)
			{
				return;
			}

			_panSpeed = ThrowIfNaN(value, nameof(PanSpeed));
			RecordSet("panSpeed", value);
		}
	}

	/// <summary>
	/// Whether the camera orbits on its own. Off by default. Turning it on makes the camera move every
	/// frame with no pointer involved, which drifts the mirror exactly as a drag does.
	/// </summary>
	public bool IsAutoRotateEnabled
	{
		get { return _isAutoRotateEnabled; }
		set
		{
			if (_isAutoRotateEnabled == value)
			{
				return;
			}

			_isAutoRotateEnabled = value;
			RecordSet("autoRotate", value);
		}
	}

	/// <summary>How fast the camera orbits when <see cref="IsAutoRotateEnabled"/> is set, in degrees per second at 60 fps. Defaults to 2.</summary>
	public float AutoRotateSpeed
	{
		get { return _autoRotateSpeed; }
		set
		{
			if (_autoRotateSpeed == value)
			{
				return;
			}

			_autoRotateSpeed = ThrowIfNaN(value, nameof(AutoRotateSpeed));
			RecordSet("autoRotateSpeed", value);
		}
	}

	/// <inheritdoc/>
	protected override string ThreeTypeName
	{
		get { return "OrbitControls"; }
	}

	/// <summary>
	/// Adopts the controls the browser built, under the handle it minted for them.
	/// </summary>
	/// <param name="handle">The negative handle the JavaScript side registered the controls under.</param>
	/// <param name="context">The context whose canvas and camera the controls are bound to.</param>
	/// <param name="camera">The camera the controls drive.</param>
	private OrbitControls(int handle, ThreeContext context, Object3D camera)
		: base(handle)
	{
		_context = context;
		_camera = camera;
		Batch = context.Batch;
		Target.OnChange = () => RecordSet("target", Target);
	}

	/// <summary>
	/// Attaches orbit controls to <paramref name="camera"/> and to this context's canvas.
	/// <para>
	/// The camera is attached to the context and the batch flushed first, so the browser is given a
	/// handle it already knows — the controls have to be handed the real three.js camera, not a promise
	/// of one. A camera already attached costs nothing here, and one attached to another context throws,
	/// which is the case that really is a mistake.
	/// </para>
	/// <para>
	/// Attaching a second set replaces the first rather than stacking: two OrbitControls on one canvas
	/// both consume the same pointer events and fight over the same camera. The replaced set is
	/// disposed on the JavaScript side, so its listeners come off; the C# object for it, if you kept
	/// one, records nothing further.
	/// </para>
	/// </summary>
	/// <param name="context">The context whose canvas the controls listen on.</param>
	/// <param name="camera">The camera to orbit.</param>
	/// <returns>The controls, ready to configure.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> or <paramref name="camera"/> is <see langword="null"/>.</exception>
	public static async Task<OrbitControls> AttachAsync(ThreeContext context, Object3D camera)
	{
		if (context is null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		if (camera is null)
		{
			throw new ArgumentNullException(nameof(camera));
		}

		context.Attach(camera);
		await context.FlushAsync();

		var handle = await context.AttachOrbitControlsAsync(camera.Handle);
		var controls = new OrbitControls(handle, context, camera);

		// The browser holds one controls slot per canvas, and attaching evicted whatever occupied it.
		// Spending the evicted mirror here is what keeps its writes from addressing the handle the
		// browser just retired.
		context.ReplaceActiveOrbitControls(controls);
		return controls;
	}

	/// <summary>
	/// Takes the controls off the canvas, which is what removes the pointer, wheel, context-menu and
	/// key listeners three.js registered for them, and leaves the camera wherever the last frame put it.
	/// <para>
	/// This instance is spent afterwards. It is detached from the batch, so a property write on it
	/// records nothing rather than addressing a handle the browser no longer knows, and a read on it
	/// fails at the call site. Attach a new set when you want controls again.
	/// </para>
	/// <para>
	/// Disposing the context detaches too, so this is only needed to stop orbiting while the canvas
	/// lives on.
	/// </para>
	/// <para>
	/// A set that a later <see cref="AttachAsync"/> already replaced only spends itself here: the
	/// browser detached it at the moment of the replacement, and reaching for the canvas now would
	/// take the replacement's listeners off instead.
	/// </para>
	/// </summary>
	public async Task DetachAsync()
	{
		Batch = null;
		if (!_context.IsActiveOrbitControls(this))
		{
			return;
		}

		_context.ClearActiveOrbitControls(this);
		await _context.DetachOrbitControlsAsync();
	}

	/// <summary>
	/// Reads where the camera actually is, in world space — the honest answer to the staleness this
	/// class's own remarks describe. Costs one interop call, and observes every write already pending,
	/// because the read travels inside the batch carrying them.
	/// </summary>
	/// <returns>The camera's world position at the moment the browser ran the read.</returns>
	public Task<Vector3> GetCameraPositionAsync()
	{
		return _context.ReadAsync<Vector3>(_camera.Handle, "getWorldPosition", [ThreeValue.Encode(new Vector3())]);
	}

	/// <summary>Reads the current distance from the camera to <see cref="Target"/>.</summary>
	/// <returns>The distance in world units.</returns>
	public Task<float> GetDistanceAsync()
	{
		return RecordRead<float>("getDistance");
	}

	/// <summary>Reads the camera's current vertical orbit angle, in radians.</summary>
	/// <returns>The polar angle.</returns>
	public Task<float> GetPolarAngleAsync()
	{
		return RecordRead<float>("getPolarAngle");
	}

	/// <summary>Reads the camera's current horizontal orbit angle, in radians.</summary>
	/// <returns>The azimuthal angle.</returns>
	public Task<float> GetAzimuthalAngleAsync()
	{
		return RecordRead<float>("getAzimuthalAngle");
	}

	/// <summary>
	/// Remembers the camera's current position and target, so <see cref="Reset"/> can come back to
	/// them. Recorded like any other command and travels with the next flush.
	/// </summary>
	public void SaveState()
	{
		RecordCall("saveState");
	}

	/// <summary>
	/// Returns the camera to the state <see cref="SaveState"/> last captured, or to where it was when
	/// the controls were attached. Recorded like any other command and travels with the next flush.
	/// </summary>
	public void Reset()
	{
		RecordCall("reset");
	}

	/// <summary>
	/// Refuses to emit a create op. Unreachable in the normal course of things — these controls have
	/// had a batch since they were constructed, and <c>AttachTo</c> returns early on an object that
	/// does — and reachable only through a rebuild after <see cref="DetachAsync"/> has cleared the
	/// batch. There is nothing to rebuild: the controls were bound to a camera and a canvas by the
	/// browser, and a create op carries neither. Attach a new set instead.
	/// </summary>
	/// <param name="batch">Batch the create op would have been recorded into.</param>
	/// <exception cref="InvalidOperationException">Always.</exception>
	internal override void EmitCreate(ThreeBatch batch)
	{
		throw new InvalidOperationException(
			$"{nameof(OrbitControls)} (handle {Handle}) was created by the browser and bound to a camera and a canvas there, " +
			$"so it cannot be recreated from the C# mirror. Call {nameof(AttachAsync)} again to get a new set.");
	}

	/// <summary>
	/// Rejects NaN, the one number no bound or speed can mean anything by: three.js compares against
	/// it and every comparison is false, so the controls would quietly stop clamping with no error
	/// anywhere. Infinity passes — the wire spells it as a tagged token the applier reads back exactly,
	/// and it is three.js's own default for <see cref="MaxDistance"/>, so writing it back has to work.
	/// </summary>
	/// <param name="value">The value being assigned.</param>
	/// <param name="member">Name of the property, for the failure message.</param>
	/// <returns><paramref name="value"/>, when it is a number.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the value is NaN.</exception>
	private static float ThrowIfNaN(float value, string member)
	{
		if (!float.IsNaN(value))
		{
			return value;
		}

		throw new ArgumentOutOfRangeException(
			nameof(value),
			value,
			$"'{member}' cannot be set to NaN: three.js clamps against this value, every comparison with NaN is false, " +
			$"and the controls would silently stop honouring the bound.");
	}
}
