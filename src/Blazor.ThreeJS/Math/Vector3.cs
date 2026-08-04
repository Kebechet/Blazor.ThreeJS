namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// Three-component vector. Mirrors three.js semantics: mutators mutate in place and return
/// <c>this</c> for chaining, so upstream three.js documentation and examples translate directly.
/// </summary>
public sealed class Vector3
{
	private float _x;
	private float _y;
	private float _z;

	/// <summary>
	/// Raised whenever any component changes. Set by an owning <c>Object3D</c> so that writing
	/// <c>mesh.Position.X</c> marks the owner dirty without the owner observing each component.
	/// </summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a new vector with all components set to zero.
	/// </summary>
	public Vector3()
	{
	}

	/// <summary>
	/// Initializes a new vector with the given component values.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	public Vector3(float x, float y, float z)
	{
		_x = x;
		_y = y;
		_z = z;
	}

	/// <summary>
	/// Gets or sets the X component. Setting this component triggers the <c>OnChange</c> callback,
	/// unless the value is unchanged.
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
	/// Gets or sets the Y component. Setting this component triggers the <c>OnChange</c> callback,
	/// unless the value is unchanged.
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
	/// Gets or sets the Z component. Setting this component triggers the <c>OnChange</c> callback,
	/// unless the value is unchanged.
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
	/// Sets all three components and triggers the <c>OnChange</c> callback once (not per component).
	/// Writing the values this vector already holds changes nothing and raises nothing, so a consumer
	/// loop that reassigns unchanged state every frame costs no interop.
	/// </summary>
	/// <param name="x">The new X component.</param>
	/// <param name="y">The new Y component.</param>
	/// <param name="z">The new Z component.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector3 Set(float x, float y, float z)
	{
		if (_x == x && _y == y && _z == z)
		{
			return this;
		}

		_x = x;
		_y = y;
		_z = z;
		OnChange?.Invoke();
		return this;
	}

	/// <summary>
	/// Copies the components from another vector into this vector, mutating it in place.
	/// </summary>
	/// <param name="other">The vector to copy from.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector3 Copy(Vector3 other)
	{
		return Set(other._x, other._y, other._z);
	}

	/// <summary>
	/// Adds another vector's components to this vector's components, mutating this vector in place.
	/// </summary>
	/// <param name="other">The vector to add.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector3 Add(Vector3 other)
	{
		return Set(_x + other._x, _y + other._y, _z + other._z);
	}

	/// <summary>
	/// Subtracts another vector's components from this vector's components, mutating this vector in place.
	/// </summary>
	/// <param name="other">The vector to subtract.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector3 Sub(Vector3 other)
	{
		return Set(_x - other._x, _y - other._y, _z - other._z);
	}

	/// <summary>
	/// Multiplies each component of this vector by a scalar value, mutating this vector in place.
	/// </summary>
	/// <param name="scalar">The scalar to multiply by.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector3 MultiplyScalar(float scalar)
	{
		return Set(_x * scalar, _y * scalar, _z * scalar);
	}

	/// <summary>
	/// Computes the squared length (magnitude squared) of this vector. This is useful for comparisons
	/// to avoid the cost of computing the square root when only relative magnitude matters.
	/// </summary>
	/// <returns>The squared length of this vector.</returns>
	public float LengthSq()
	{
		return (_x * _x) + (_y * _y) + (_z * _z);
	}

	/// <summary>
	/// Computes the length (magnitude) of this vector: sqrt(x² + y² + z²).
	/// </summary>
	/// <returns>The length of this vector.</returns>
	public float Length()
	{
		return MathF.Sqrt(LengthSq());
	}

	/// <summary>
	/// Normalizes this vector to unit length (length = 1), mutating it in place.
	/// If this vector has zero length, it is left unchanged rather than producing NaN.
	/// </summary>
	/// <returns>This vector, for method chaining.</returns>
	public Vector3 Normalize()
	{
		var length = Length();
		if (length == 0f)
		{
			return this;
		}

		return MultiplyScalar(1f / length);
	}

	/// <summary>
	/// Computes the dot product of this vector with another vector.
	/// </summary>
	/// <param name="other">The other vector.</param>
	/// <returns>The dot product: (this.x * other.x) + (this.y * other.y) + (this.z * other.z).</returns>
	public float Dot(Vector3 other)
	{
		return (_x * other._x) + (_y * other._y) + (_z * other._z);
	}

	/// <summary>
	/// Computes the cross product of this vector with another vector, storing the result in this vector, mutating it in place.
	/// </summary>
	/// <param name="other">The other vector.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector3 Cross(Vector3 other)
	{
		return Set(
			(_y * other._z) - (_z * other._y),
			(_z * other._x) - (_x * other._z),
			(_x * other._y) - (_y * other._x));
	}

	/// <summary>
	/// Computes the distance from this vector to another vector.
	/// </summary>
	/// <param name="other">The other vector.</param>
	/// <returns>The Euclidean distance between the two points represented by these vectors.</returns>
	public float DistanceTo(Vector3 other)
	{
		var deltaX = _x - other._x;
		var deltaY = _y - other._y;
		var deltaZ = _z - other._z;
		return MathF.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
	}

	/// <summary>
	/// Extracts the components of this vector into an array.
	/// </summary>
	/// <returns>A new array containing [x, y, z].</returns>
	public float[] ToArray()
	{
		return [_x, _y, _z];
	}

	/// <summary>
	/// Copies three values from an array into this vector's components, mutating it in place.
	/// </summary>
	/// <param name="values">An array with at least three elements. The first three elements are copied.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector3 FromArray(float[] values)
	{
		return Set(values[0], values[1], values[2]);
	}

	/// <summary>
	/// Creates a shallow copy of this vector with the same component values.
	/// The copy is independent and has no <c>OnChange</c> callback set.
	/// </summary>
	/// <returns>A new vector with the same components.</returns>
	public Vector3 Clone()
	{
		return new Vector3(_x, _y, _z);
	}
}
