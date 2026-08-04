// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Camera that uses [perspective
/// projection](https://en.wikipedia.org/wiki/Perspective_(graphical)). This projection mode is
/// designed to mimic the way the human eye sees. It is the most common projection mode used for
/// rendering a 3D scene. The JavaScript-side <c>THREE.PerspectiveCamera</c>.
/// </summary>
public class PerspectiveCamera : Camera
{
	private float _fov;
	private float _aspect;
	private float _near;
	private float _far;
	private float _zoom = 1f;
	private float _focus = 10f;
	private float _filmGauge = 35f;
	private float _filmOffset = 0f;
	private bool _isFovWritten;
	private bool _isZoomWritten;
	private bool _isNearWritten;
	private bool _isFarWritten;
	private bool _isFocusWritten;
	private bool _isAspectWritten;
	private bool _isFilmGaugeWritten;
	private bool _isFilmOffsetWritten;

	/// <summary>Constructs a new perspective camera.</summary>
	/// <param name="fov">The vertical field of view.</param>
	/// <param name="aspect">The aspect ratio.</param>
	/// <param name="near">The camera's near plane.</param>
	/// <param name="far">The camera's far plane.</param>
	public PerspectiveCamera(float fov = 50f, float aspect = 1f, float near = 0.1f, float far = 2000f)
	{
		_fov = fov;
		_aspect = aspect;
		_near = near;
		_far = far;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PerspectiveCamera</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PerspectiveCamera"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.PerspectiveCamera</c>: fov, aspect, near, far.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_fov, _aspect, _near, _far]; }
	}

	/// <summary>
	/// The vertical field of view, from bottom to top of view, in degrees. Writing it records a
	/// <c>fov</c> property write once this object is attached; writing the value already held records
	/// nothing.
	/// </summary>
	public float Fov
	{
		get { return _fov; }
		set
		{
			if (_fov == value)
			{
				return;
			}

			_fov = value;
			_isFovWritten = true;
			RecordSet("fov", value);
		}
	}

	/// <summary>
	/// The zoom factor of the camera. Writing it records a <c>zoom</c> property write once this object
	/// is attached; writing the value already held records nothing.
	/// </summary>
	public float Zoom
	{
		get { return _zoom; }
		set
		{
			if (_zoom == value)
			{
				return;
			}

			_zoom = value;
			_isZoomWritten = true;
			RecordSet("zoom", value);
		}
	}

	/// <summary>
	/// The camera's near plane. The valid range is greater than <c>0</c> and less than the current
	/// value of <c>PerspectiveCamera#far</c>. Note that, unlike for the
	/// <see cref="OrthographicCamera"/>, <c>0</c> is &lt;em&gt;not&lt;/em&gt; a valid value for a
	/// perspective camera's near plane. Writing it records a <c>near</c> property write once this
	/// object is attached; writing the value already held records nothing.
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
	/// The camera's far plane. Must be greater than the current value of <c>PerspectiveCamera#near</c>.
	/// Writing it records a <c>far</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float Far
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
	/// Object distance used for stereoscopy and depth-of-field effects. This parameter does not
	/// influence the projection matrix unless a <see cref="StereoCamera"/> is being used. Writing it
	/// records a <c>focus</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float Focus
	{
		get { return _focus; }
		set
		{
			if (_focus == value)
			{
				return;
			}

			_focus = value;
			_isFocusWritten = true;
			RecordSet("focus", value);
		}
	}

	/// <summary>
	/// The aspect ratio, usually the canvas width / canvas height. Writing it records a <c>aspect</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Aspect
	{
		get { return _aspect; }
		set
		{
			if (_aspect == value)
			{
				return;
			}

			_aspect = value;
			_isAspectWritten = true;
			RecordSet("aspect", value);
		}
	}

	/// <summary>
	/// Film size used for the larger axis. Default is <c>35</c> (millimeters). This parameter does not
	/// influence the projection matrix unless <c>PerspectiveCamera#filmOffset</c> is set to a nonzero
	/// value. Writing it records a <c>filmGauge</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public float FilmGauge
	{
		get { return _filmGauge; }
		set
		{
			if (_filmGauge == value)
			{
				return;
			}

			_filmGauge = value;
			_isFilmGaugeWritten = true;
			RecordSet("filmGauge", value);
		}
	}

	/// <summary>
	/// Horizontal off-center offset in the same unit as <c>PerspectiveCamera#filmGauge</c>. Writing it
	/// records a <c>filmOffset</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float FilmOffset
	{
		get { return _filmOffset; }
		set
		{
			if (_filmOffset == value)
			{
				return;
			}

			_filmOffset = value;
			_isFilmOffsetWritten = true;
			RecordSet("filmOffset", value);
		}
	}

	/// <summary>
	/// Sets the FOV by focal length in respect to the current <c>PerspectiveCamera#filmGauge</c>. The
	/// default film gauge is 35, so that the focal length can be specified for a 35mm (full frame)
	/// camera.
	/// </summary>
	/// <param name="focalLength">Values for focal length and film gauge must have the same unit.</param>
	public void SetFocalLength(float focalLength)
	{
		RecordCall("setFocalLength", focalLength);
	}

	/// <summary>
	/// Sets an offset in a larger frustum. This is useful for multi-window or
	/// multi-monitor/multi-machine setups. For example, if you have 3x2 monitors and each monitor is
	/// 1920x1080 and the monitors are in grid like this then for each monitor you would call it like
	/// this: Note there is no reason monitors have to be the same size or in a grid.
	/// </summary>
	/// <param name="fullWidth">The full width of multiview setup.</param>
	/// <param name="fullHeight">The full height of multiview setup.</param>
	/// <param name="x">The horizontal offset of the subcamera.</param>
	/// <param name="y">The vertical offset of the subcamera.</param>
	/// <param name="width">The width of subcamera.</param>
	/// <param name="height">The height of subcamera.</param>
	public void SetViewOffset(float fullWidth, float fullHeight, float x, float y, float width, float height)
	{
		RecordCall("setViewOffset", fullWidth, fullHeight, x, y, width, height);
	}

	/// <summary>Removes the view offset from the projection matrix.</summary>
	public void ClearViewOffset()
	{
		RecordCall("clearViewOffset");
	}

	/// <summary>
	/// Updates the camera's projection matrix. Must be called after any change of camera properties.
	/// </summary>
	public void UpdateProjectionMatrix()
	{
		RecordCall("updateProjectionMatrix");
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

		if (_isFovWritten)
		{
			batch.Set(Handle, "fov", ThreeValue.Encode(_fov));
		}

		if (_isZoomWritten)
		{
			batch.Set(Handle, "zoom", ThreeValue.Encode(_zoom));
		}

		if (_isNearWritten)
		{
			batch.Set(Handle, "near", ThreeValue.Encode(_near));
		}

		if (_isFarWritten)
		{
			batch.Set(Handle, "far", ThreeValue.Encode(_far));
		}

		if (_isFocusWritten)
		{
			batch.Set(Handle, "focus", ThreeValue.Encode(_focus));
		}

		if (_isAspectWritten)
		{
			batch.Set(Handle, "aspect", ThreeValue.Encode(_aspect));
		}

		if (_isFilmGaugeWritten)
		{
			batch.Set(Handle, "filmGauge", ThreeValue.Encode(_filmGauge));
		}

		if (_isFilmOffsetWritten)
		{
			batch.Set(Handle, "filmOffset", ThreeValue.Encode(_filmOffset));
		}
	}
}
