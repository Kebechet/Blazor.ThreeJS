namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// An infinite plane in three dimensions, held in Hessian normal form: a unit normal and a signed
/// distance from the origin along it. A point <c>p</c> lies on the plane when
/// <c>normal · p + constant == 0</c>.
/// </summary>
public sealed class Plane
{
	private float _constant;

	/// <summary>
	/// The plane's normal, expected to be unit length. Mutating it in place notifies this plane's
	/// owner; it is not re-normalized automatically, matching three.js.
	/// </summary>
	public Vector3 Normal { get; }

	/// <summary>Raised whenever the normal or the constant changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes the plane <c>x = 0</c>, with a normal along the positive X axis, which is
	/// three.js's own default.
	/// </summary>
	public Plane()
		: this(new Vector3(1f, 0f, 0f), 0f)
	{
	}

	/// <summary>
	/// Initializes a plane from a normal and a constant.
	/// </summary>
	/// <param name="normal">The plane normal, expected to be unit length. Copied; the instance is not retained.</param>
	/// <param name="constant">The signed distance from the origin along the normal.</param>
	/// <remarks>
	/// The normal is copied rather than aliased. See <see cref="Box3(Vector3, Vector3)"/> for why.
	/// </remarks>
	public Plane(Vector3 normal, float constant)
	{
		Normal = normal.Clone();
		Normal.OnChange = RaiseChanged;
		_constant = constant;
	}

	/// <summary>Gets or sets the signed distance from the origin along the normal.</summary>
	public float Constant
	{
		get { return _constant; }
		set
		{
			if (_constant == value)
			{
				return;
			}

			_constant = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>Sets the normal and constant together, triggering the change callback once.</summary>
	/// <param name="normal">The new normal, copied by value.</param>
	/// <param name="constant">The new constant.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Plane Set(Vector3 normal, float constant)
	{
		return SetComponents(normal.X, normal.Y, normal.Z, constant);
	}

	/// <summary>Copies the normal and constant from another plane, mutating this one in place.</summary>
	/// <param name="other">The plane to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Plane Copy(Plane other)
	{
		return SetComponents(other.Normal.X, other.Normal.Y, other.Normal.Z, other._constant);
	}

	/// <summary>
	/// Sets this plane from a normal and a point known to lie on it, mutating it in place.
	/// </summary>
	/// <param name="normal">The plane normal, expected to be unit length.</param>
	/// <param name="point">A point on the plane.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Plane SetFromNormalAndCoplanarPoint(Vector3 normal, Vector3 point)
	{
		return SetComponents(normal.X, normal.Y, normal.Z, -normal.Dot(point));
	}

	/// <summary>
	/// Computes the signed distance from a point to this plane: positive on the side the normal points
	/// to, negative on the other, zero on the plane.
	/// </summary>
	/// <param name="point">The point to measure.</param>
	/// <returns>The signed distance.</returns>
	public float DistanceToPoint(Vector3 point)
	{
		return Normal.Dot(point) + _constant;
	}

	/// <summary>
	/// Scales the normal to unit length and the constant with it, so the plane is unmoved but its
	/// distances become true Euclidean distances. A zero-length normal is left alone rather than
	/// producing NaN.
	/// </summary>
	/// <returns>This instance, for method chaining.</returns>
	public Plane Normalize()
	{
		var length = Normal.Length();
		if (length == 0f)
		{
			return this;
		}

		var inverse = 1f / length;
		return SetComponents(
			Normal.X * inverse,
			Normal.Y * inverse,
			Normal.Z * inverse,
			_constant * inverse);
	}

	/// <summary>Flips the plane to face the other way, leaving its position unchanged.</summary>
	/// <returns>This instance, for method chaining.</returns>
	public Plane Negate()
	{
		return SetComponents(-Normal.X, -Normal.Y, -Normal.Z, -_constant);
	}

	/// <summary>Projects a point onto this plane.</summary>
	/// <param name="point">The point to project.</param>
	/// <returns>A new vector at the closest point on the plane.</returns>
	public Vector3 ProjectPoint(Vector3 point)
	{
		var distance = DistanceToPoint(point);
		return new Vector3(
			point.X - (Normal.X * distance),
			point.Y - (Normal.Y * distance),
			point.Z - (Normal.Z * distance));
	}

	/// <summary>Extracts the normal and constant into an array.</summary>
	/// <returns>A new array containing [normalX, normalY, normalZ, constant].</returns>
	public float[] ToArray()
	{
		return [Normal.X, Normal.Y, Normal.Z, _constant];
	}

	/// <summary>Copies four values from an array into this plane, mutating it in place.</summary>
	/// <param name="values">An array with at least four elements, in [normalX, normalY, normalZ, constant] order.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Plane FromArray(float[] values)
	{
		return SetComponents(values[0], values[1], values[2], values[3]);
	}

	/// <summary>Creates a copy of this plane with the same definition and no change callback.</summary>
	/// <returns>A new plane with the same normal and constant.</returns>
	public Plane Clone()
	{
		return new Plane(Normal, _constant);
	}

	/// <summary>Writes normal and constant together, raising the change callback at most once.</summary>
	private Plane SetComponents(float normalX, float normalY, float normalZ, float constant)
	{
		var hasChanged = Normal.X != normalX || Normal.Y != normalY || Normal.Z != normalZ || _constant != constant;
		if (!hasChanged)
		{
			return this;
		}

		Normal.OnChange = null;
		Normal.Set(normalX, normalY, normalZ);
		Normal.OnChange = RaiseChanged;
		_constant = constant;

		OnChange?.Invoke();
		return this;
	}

	private void RaiseChanged()
	{
		OnChange?.Invoke();
	}
}
