// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Camera that uses [orthographic
/// projection](https://en.wikipedia.org/wiki/Orthographic_projection). In this projection mode, an
/// object's size in the rendered image stays constant regardless of its distance from the camera.
/// This can be useful for rendering 2D scenes and UI elements, amongst other things. The
/// JavaScript-side <c>THREE.OrthographicCamera</c>.
/// </summary>
public sealed class OrthographicCamera : Camera
{
	private float _left;
	private float _right;
	private float _top;
	private float _bottom;
	private float _near;
	private float _far;
	private float _zoom = 1f;
	private OrthographicCameraView? _view = null;
	private bool _isZoomWritten;
	private bool _isViewWritten;
	private bool _isLeftWritten;
	private bool _isRightWritten;
	private bool _isTopWritten;
	private bool _isBottomWritten;
	private bool _isNearWritten;
	private bool _isFarWritten;

	/// <summary>Constructs a new orthographic camera.</summary>
	/// <param name="left">The left plane of the camera's frustum.</param>
	/// <param name="right">The right plane of the camera's frustum.</param>
	/// <param name="top">The top plane of the camera's frustum.</param>
	/// <param name="bottom">The bottom plane of the camera's frustum.</param>
	/// <param name="near">The camera's near plane.</param>
	/// <param name="far">The camera's far plane.</param>
	public OrthographicCamera(
		float left = -1f,
		float right = 1f,
		float top = 1f,
		float bottom = -1f,
		float near = 0.1f,
		float far = 2000f)
	{
		_left = left;
		_right = right;
		_top = top;
		_bottom = bottom;
		_near = near;
		_far = far;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>OrthographicCamera</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal OrthographicCamera(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_left = default!;
		_right = default!;
		_top = default!;
		_bottom = default!;
		_near = default!;
		_far = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.OrthographicCamera</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "OrthographicCamera"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.OrthographicCamera</c>: left, right, top, bottom,
	/// near, far.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_left, _right, _top, _bottom, _near, _far]; }
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
	/// Represents the frustum window specification. This property should not be edited directly but via
	/// <c>PerspectiveCamera#setViewOffset</c> and <c>PerspectiveCamera#clearViewOffset</c>. Writing it
	/// records a <c>view</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public OrthographicCameraView? View
	{
		get { return _view; }
		set
		{
			if (_view == value)
			{
				return;
			}

			_view = value;
			_isViewWritten = true;
			RecordSet("view", value);
		}
	}

	/// <summary>
	/// The left plane of the camera's frustum. Writing it records a <c>left</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Left
	{
		get { return _left; }
		set
		{
			if (_left == value)
			{
				return;
			}

			_left = value;
			_isLeftWritten = true;
			RecordSet("left", value);
		}
	}

	/// <summary>
	/// The right plane of the camera's frustum. Writing it records a <c>right</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Right
	{
		get { return _right; }
		set
		{
			if (_right == value)
			{
				return;
			}

			_right = value;
			_isRightWritten = true;
			RecordSet("right", value);
		}
	}

	/// <summary>
	/// The top plane of the camera's frustum. Writing it records a <c>top</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float Top
	{
		get { return _top; }
		set
		{
			if (_top == value)
			{
				return;
			}

			_top = value;
			_isTopWritten = true;
			RecordSet("top", value);
		}
	}

	/// <summary>
	/// The bottom plane of the camera's frustum. Writing it records a <c>bottom</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Bottom
	{
		get { return _bottom; }
		set
		{
			if (_bottom == value)
			{
				return;
			}

			_bottom = value;
			_isBottomWritten = true;
			RecordSet("bottom", value);
		}
	}

	/// <summary>
	/// The camera's near plane. The valid range is greater than <c>0</c> and less than the current
	/// value of <c>OrthographicCamera#far</c>. Note that, unlike for the
	/// <see cref="PerspectiveCamera"/>, <c>0</c> is a valid value for an orthographic camera's near
	/// plane. Writing it records a <c>near</c> property write once this object is attached; writing the
	/// value already held records nothing.
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
	/// The camera's far plane. Must be greater than the current value of
	/// <c>OrthographicCamera#near</c>. Writing it records a <c>far</c> property write once this object
	/// is attached; writing the value already held records nothing.
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
	/// Sets an offset in a larger frustum. This is useful for multi-window or
	/// multi-monitor/multi-machine setups.
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
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isOrthographicCamera</c> held.
	/// </summary>
	/// <returns>The value <c>isOrthographicCamera</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsOrthographicCameraAsync()
	{
		return GetAsync<bool>("isOrthographicCamera");
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

		if (_isZoomWritten)
		{
			batch.Set(Handle, "zoom", ThreeValue.Encode(_zoom));
		}

		if (_isViewWritten)
		{
			batch.Set(Handle, "view", ThreeValue.Encode(_view));
		}

		if (_isLeftWritten)
		{
			batch.Set(Handle, "left", ThreeValue.Encode(_left));
		}

		if (_isRightWritten)
		{
			batch.Set(Handle, "right", ThreeValue.Encode(_right));
		}

		if (_isTopWritten)
		{
			batch.Set(Handle, "top", ThreeValue.Encode(_top));
		}

		if (_isBottomWritten)
		{
			batch.Set(Handle, "bottom", ThreeValue.Encode(_bottom));
		}

		if (_isNearWritten)
		{
			batch.Set(Handle, "near", ThreeValue.Encode(_near));
		}

		if (_isFarWritten)
		{
			batch.Set(Handle, "far", ThreeValue.Encode(_far));
		}
	}
}
