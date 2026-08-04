namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// Rotation quaternion. Ported from three.js so that <see cref="SetFromEuler"/> reproduces
/// upstream's per-order term ordering exactly.
/// </summary>
public sealed class Quaternion
{
	private float _x;
	private float _y;
	private float _z;
	private float _w = 1f;

	/// <summary>
	/// Raised whenever any component changes. Set by an owning <c>Object3D</c> so that writing
	/// <c>mesh.Quaternion.W</c> marks the owner dirty without the owner observing each component.
	/// </summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a new quaternion representing the identity rotation (0, 0, 0, 1).
	/// </summary>
	public Quaternion()
	{
	}

	/// <summary>
	/// Initializes a new quaternion with the given component values.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <param name="w">The W (scalar) component.</param>
	public Quaternion(float x, float y, float z, float w)
	{
		_x = x;
		_y = y;
		_z = z;
		_w = w;
	}

	/// <summary>
	/// Gets or sets the X component. Setting this component triggers the <c>OnChange</c> callback.
	/// </summary>
	public float X
	{
		get { return _x; }
		set
		{
			_x = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Gets or sets the Y component. Setting this component triggers the <c>OnChange</c> callback.
	/// </summary>
	public float Y
	{
		get { return _y; }
		set
		{
			_y = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Gets or sets the Z component. Setting this component triggers the <c>OnChange</c> callback.
	/// </summary>
	public float Z
	{
		get { return _z; }
		set
		{
			_z = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Gets or sets the W (scalar) component. Setting this component triggers the <c>OnChange</c> callback.
	/// </summary>
	public float W
	{
		get { return _w; }
		set
		{
			_w = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Sets all four components, mutating this instance in place and triggering the <c>OnChange</c>
	/// callback once (not per component).
	/// </summary>
	/// <param name="x">The new X component.</param>
	/// <param name="y">The new Y component.</param>
	/// <param name="z">The new Z component.</param>
	/// <param name="w">The new W (scalar) component.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Quaternion Set(float x, float y, float z, float w)
	{
		_x = x;
		_y = y;
		_z = z;
		_w = w;
		OnChange?.Invoke();
		return this;
	}

	/// <summary>
	/// Computes the rotation quaternion equivalent to the given Euler angles, mutating this instance
	/// in place. The term ordering for each <see cref="EulerOrder"/> is ported directly from three.js
	/// so the resulting quaternion matches upstream bit-for-bit (modulo floating point rounding).
	/// </summary>
	/// <param name="euler">The Euler angles to convert.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Quaternion SetFromEuler(Euler euler)
	{
		var cosX = MathF.Cos(euler.X / 2f);
		var cosY = MathF.Cos(euler.Y / 2f);
		var cosZ = MathF.Cos(euler.Z / 2f);
		var sinX = MathF.Sin(euler.X / 2f);
		var sinY = MathF.Sin(euler.Y / 2f);
		var sinZ = MathF.Sin(euler.Z / 2f);

		switch (euler.Order)
		{
			case EulerOrder.XYZ:
				return Set(
					(sinX * cosY * cosZ) + (cosX * sinY * sinZ),
					(cosX * sinY * cosZ) - (sinX * cosY * sinZ),
					(cosX * cosY * sinZ) + (sinX * sinY * cosZ),
					(cosX * cosY * cosZ) - (sinX * sinY * sinZ));
			case EulerOrder.YXZ:
				return Set(
					(sinX * cosY * cosZ) + (cosX * sinY * sinZ),
					(cosX * sinY * cosZ) - (sinX * cosY * sinZ),
					(cosX * cosY * sinZ) - (sinX * sinY * cosZ),
					(cosX * cosY * cosZ) + (sinX * sinY * sinZ));
			case EulerOrder.ZXY:
				return Set(
					(sinX * cosY * cosZ) - (cosX * sinY * sinZ),
					(cosX * sinY * cosZ) + (sinX * cosY * sinZ),
					(cosX * cosY * sinZ) + (sinX * sinY * cosZ),
					(cosX * cosY * cosZ) - (sinX * sinY * sinZ));
			case EulerOrder.ZYX:
				return Set(
					(sinX * cosY * cosZ) - (cosX * sinY * sinZ),
					(cosX * sinY * cosZ) + (sinX * cosY * sinZ),
					(cosX * cosY * sinZ) - (sinX * sinY * cosZ),
					(cosX * cosY * cosZ) + (sinX * sinY * sinZ));
			case EulerOrder.YZX:
				return Set(
					(sinX * cosY * cosZ) + (cosX * sinY * sinZ),
					(cosX * sinY * cosZ) + (sinX * cosY * sinZ),
					(cosX * cosY * sinZ) - (sinX * sinY * cosZ),
					(cosX * cosY * cosZ) - (sinX * sinY * sinZ));
			case EulerOrder.XZY:
				return Set(
					(sinX * cosY * cosZ) - (cosX * sinY * sinZ),
					(cosX * sinY * cosZ) - (sinX * cosY * sinZ),
					(cosX * cosY * sinZ) + (sinX * sinY * cosZ),
					(cosX * cosY * cosZ) + (sinX * sinY * sinZ));
			default:
				throw new NotImplementedException($"{nameof(EulerOrder)} '{euler.Order}' is not handled.");
		}
	}

	/// <summary>
	/// Normalizes this quaternion to unit length, mutating it in place. A quaternion must be unit
	/// length to represent a valid rotation. If this quaternion has zero length, it is set to the
	/// identity rotation (0, 0, 0, 1) rather than producing NaN.
	/// </summary>
	/// <returns>This instance, for method chaining.</returns>
	public Quaternion Normalize()
	{
		var length = MathF.Sqrt((_x * _x) + (_y * _y) + (_z * _z) + (_w * _w));
		if (length == 0f)
		{
			return Set(0f, 0f, 0f, 1f);
		}

		return Set(_x / length, _y / length, _z / length, _w / length);
	}

	/// <summary>
	/// Extracts the components of this quaternion into an array.
	/// </summary>
	/// <returns>A new array containing [x, y, z, w].</returns>
	public float[] ToArray()
	{
		return [_x, _y, _z, _w];
	}
}
