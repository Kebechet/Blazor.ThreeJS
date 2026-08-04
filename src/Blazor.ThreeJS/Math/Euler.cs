namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// Euler angles in radians. Order matters: the same three angles describe different rotations
/// under different orders, which is why three.js carries the order on the value itself.
/// </summary>
public sealed class Euler
{
	private float _x;
	private float _y;
	private float _z;
	private EulerOrder _order = EulerOrder.XYZ;

	/// <summary>
	/// Raised whenever any component or the order changes. Set by an owning <c>Object3D</c> so that
	/// writing <c>mesh.Rotation.Y</c> marks the owner dirty without the owner observing each component.
	/// </summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a new set of Euler angles with all components set to zero and <see cref="EulerOrder.XYZ"/> order.
	/// </summary>
	public Euler()
	{
	}

	/// <summary>
	/// Initializes a new set of Euler angles with the given component values and order.
	/// </summary>
	/// <param name="x">The rotation about the X axis, in radians.</param>
	/// <param name="y">The rotation about the Y axis, in radians.</param>
	/// <param name="z">The rotation about the Z axis, in radians.</param>
	/// <param name="order">The order in which the axis rotations are applied. Defaults to <see cref="EulerOrder.XYZ"/>.</param>
	public Euler(float x, float y, float z, EulerOrder order = EulerOrder.XYZ)
	{
		_x = x;
		_y = y;
		_z = z;
		_order = order;
	}

	/// <summary>
	/// Gets or sets the rotation about the X axis, in radians. Setting this component triggers the
	/// <c>OnChange</c> callback, unless the value is unchanged.
	/// </summary>
	public float X
	{
		get { return _x; }
		set
		{
			if (_x == value)
			{
				return;
			}

			_x = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Gets or sets the rotation about the Y axis, in radians. Setting this component triggers the
	/// <c>OnChange</c> callback, unless the value is unchanged.
	/// </summary>
	public float Y
	{
		get { return _y; }
		set
		{
			if (_y == value)
			{
				return;
			}

			_y = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Gets or sets the rotation about the Z axis, in radians. Setting this component triggers the
	/// <c>OnChange</c> callback, unless the value is unchanged.
	/// </summary>
	public float Z
	{
		get { return _z; }
		set
		{
			if (_z == value)
			{
				return;
			}

			_z = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Gets or sets the order in which the axis rotations are applied. The same X/Y/Z values produce a
	/// different final rotation under a different order, so this is not just metadata. Setting this
	/// property triggers the <c>OnChange</c> callback, unless the value is unchanged.
	/// </summary>
	public EulerOrder Order
	{
		get { return _order; }
		set
		{
			if (_order == value)
			{
				return;
			}

			_order = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Sets all three components and the order, mutating this instance in place and triggering the
	/// <c>OnChange</c> callback once (not per component). Writing the values this instance already
	/// holds changes nothing and raises nothing, so a consumer loop that reassigns unchanged state
	/// every frame costs no interop.
	/// </summary>
	/// <param name="x">The new rotation about the X axis, in radians.</param>
	/// <param name="y">The new rotation about the Y axis, in radians.</param>
	/// <param name="z">The new rotation about the Z axis, in radians.</param>
	/// <param name="order">The new rotation order.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Euler Set(float x, float y, float z, EulerOrder order)
	{
		if (_x == x && _y == y && _z == z && _order == order)
		{
			return this;
		}

		_x = x;
		_y = y;
		_z = z;
		_order = order;
		OnChange?.Invoke();
		return this;
	}

	/// <summary>
	/// Extracts the angle components of this instance into an array. The order is not included.
	/// </summary>
	/// <returns>A new array containing [x, y, z].</returns>
	public float[] ToArray()
	{
		return [_x, _y, _z];
	}
}

/// <summary>
/// The order in which axis rotations are applied when composing a set of Euler angles into a rotation.
/// </summary>
public enum EulerOrder : byte
{
	/// <summary>Rotate about X, then Y, then Z.</summary>
	XYZ,

	/// <summary>Rotate about Y, then X, then Z.</summary>
	YXZ,

	/// <summary>Rotate about Z, then X, then Y.</summary>
	ZXY,

	/// <summary>Rotate about Z, then Y, then X.</summary>
	ZYX,

	/// <summary>Rotate about Y, then Z, then X.</summary>
	YZX,

	/// <summary>Rotate about X, then Z, then Y.</summary>
	XZY
}
