namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// Four-component vector. Mirrors three.js semantics: mutators mutate in place and return
/// <c>this</c> for chaining, so upstream three.js documentation and examples translate directly.
/// </summary>
public sealed class Vector4
{
	private float _x;
	private float _y;
	private float _z;
	private float _w = 1f;

	/// <summary>
	/// Raised whenever any component changes. Set by an owning object so that writing
	/// <c>renderer.Viewport.X</c> marks the owner dirty without the owner observing each component.
	/// </summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a new vector at (0, 0, 0, 1). The <c>W</c> default of one is three.js's own, and
	/// is what makes an unset vector behave as a position rather than a direction under a homogeneous
	/// transform.
	/// </summary>
	public Vector4()
	{
	}

	/// <summary>
	/// Initializes a new vector with the given component values.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <param name="w">The W component.</param>
	public Vector4(float x, float y, float z, float w)
	{
		_x = x;
		_y = y;
		_z = z;
		_w = w;
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
	/// Gets or sets the W component. Setting this component triggers the <c>OnChange</c> callback,
	/// unless the value is unchanged.
	/// </summary>
	public float W
	{
		get { return _w; }
		set
		{
			if (_w == value)
			{
				return;
			}

			_w = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Sets all four components and triggers the <c>OnChange</c> callback once (not per component).
	/// Writing the values this vector already holds changes nothing and raises nothing.
	/// </summary>
	/// <param name="x">The new X component.</param>
	/// <param name="y">The new Y component.</param>
	/// <param name="z">The new Z component.</param>
	/// <param name="w">The new W component.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector4 Set(float x, float y, float z, float w)
	{
		if (_x == x && _y == y && _z == z && _w == w)
		{
			return this;
		}

		_x = x;
		_y = y;
		_z = z;
		_w = w;
		OnChange?.Invoke();
		return this;
	}

	/// <summary>
	/// Copies the components from another vector into this vector, mutating it in place.
	/// </summary>
	/// <param name="other">The vector to copy from.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector4 Copy(Vector4 other)
	{
		return Set(other._x, other._y, other._z, other._w);
	}

	/// <summary>
	/// Adds another vector's components to this vector's components, mutating this vector in place.
	/// </summary>
	/// <param name="other">The vector to add.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector4 Add(Vector4 other)
	{
		return Set(_x + other._x, _y + other._y, _z + other._z, _w + other._w);
	}

	/// <summary>
	/// Subtracts another vector's components from this vector's components, mutating this vector in place.
	/// </summary>
	/// <param name="other">The vector to subtract.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector4 Sub(Vector4 other)
	{
		return Set(_x - other._x, _y - other._y, _z - other._z, _w - other._w);
	}

	/// <summary>
	/// Multiplies each component of this vector by a scalar value, mutating this vector in place.
	/// </summary>
	/// <param name="scalar">The scalar to multiply by.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector4 MultiplyScalar(float scalar)
	{
		return Set(_x * scalar, _y * scalar, _z * scalar, _w * scalar);
	}

	/// <summary>
	/// Computes the squared length (magnitude squared) of this vector.
	/// </summary>
	/// <returns>The squared length of this vector.</returns>
	public float LengthSq()
	{
		return (_x * _x) + (_y * _y) + (_z * _z) + (_w * _w);
	}

	/// <summary>
	/// Computes the length (magnitude) of this vector: sqrt(x² + y² + z² + w²).
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
	public Vector4 Normalize()
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
	/// <returns>The dot product of the two vectors.</returns>
	public float Dot(Vector4 other)
	{
		return (_x * other._x) + (_y * other._y) + (_z * other._z) + (_w * other._w);
	}

	/// <summary>
	/// Extracts the components of this vector into an array.
	/// </summary>
	/// <returns>A new array containing [x, y, z, w].</returns>
	public float[] ToArray()
	{
		return [_x, _y, _z, _w];
	}

	/// <summary>
	/// Copies four values from an array into this vector's components, mutating it in place.
	/// </summary>
	/// <param name="values">An array with at least four elements. The first four elements are copied.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector4 FromArray(float[] values)
	{
		return Set(values[0], values[1], values[2], values[3]);
	}

	/// <summary>
	/// Creates a shallow copy of this vector with the same component values.
	/// The copy is independent and has no <c>OnChange</c> callback set.
	/// </summary>
	/// <returns>A new vector with the same components.</returns>
	public Vector4 Clone()
	{
		return new Vector4(_x, _y, _z, _w);
	}
}
