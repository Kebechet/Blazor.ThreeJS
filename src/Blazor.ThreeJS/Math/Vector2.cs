namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// Two-component vector. Mirrors three.js semantics: mutators mutate in place and return
/// <c>this</c> for chaining, so upstream three.js documentation and examples translate directly.
/// </summary>
public sealed class Vector2
{
	private float _x;
	private float _y;

	/// <summary>
	/// Raised whenever any component changes. Set by an owning object so that writing
	/// <c>material.Repeat.X</c> marks the owner dirty without the owner observing each component.
	/// </summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a new vector with both components set to zero.
	/// </summary>
	public Vector2()
	{
	}

	/// <summary>
	/// Initializes a new vector with the given component values.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	public Vector2(float x, float y)
	{
		_x = x;
		_y = y;
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
	/// Gets or sets the X component under three.js's alternate name for a vector used as a size.
	/// The same storage as <see cref="X"/>.
	/// </summary>
	public float Width
	{
		get { return X; }
		set { X = value; }
	}

	/// <summary>
	/// Gets or sets the Y component under three.js's alternate name for a vector used as a size.
	/// The same storage as <see cref="Y"/>.
	/// </summary>
	public float Height
	{
		get { return Y; }
		set { Y = value; }
	}

	/// <summary>
	/// Sets both components and triggers the <c>OnChange</c> callback once (not per component).
	/// Writing the values this vector already holds changes nothing and raises nothing, so a consumer
	/// loop that reassigns unchanged state every frame costs no interop.
	/// </summary>
	/// <param name="x">The new X component.</param>
	/// <param name="y">The new Y component.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector2 Set(float x, float y)
	{
		if (_x == x && _y == y)
		{
			return this;
		}

		_x = x;
		_y = y;
		OnChange?.Invoke();
		return this;
	}

	/// <summary>
	/// Copies the components from another vector into this vector, mutating it in place.
	/// </summary>
	/// <param name="other">The vector to copy from.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector2 Copy(Vector2 other)
	{
		return Set(other._x, other._y);
	}

	/// <summary>
	/// Adds another vector's components to this vector's components, mutating this vector in place.
	/// </summary>
	/// <param name="other">The vector to add.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector2 Add(Vector2 other)
	{
		return Set(_x + other._x, _y + other._y);
	}

	/// <summary>
	/// Subtracts another vector's components from this vector's components, mutating this vector in place.
	/// </summary>
	/// <param name="other">The vector to subtract.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector2 Sub(Vector2 other)
	{
		return Set(_x - other._x, _y - other._y);
	}

	/// <summary>
	/// Multiplies each component of this vector by a scalar value, mutating this vector in place.
	/// </summary>
	/// <param name="scalar">The scalar to multiply by.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector2 MultiplyScalar(float scalar)
	{
		return Set(_x * scalar, _y * scalar);
	}

	/// <summary>
	/// Computes the squared length (magnitude squared) of this vector. This is useful for comparisons
	/// to avoid the cost of computing the square root when only relative magnitude matters.
	/// </summary>
	/// <returns>The squared length of this vector.</returns>
	public float LengthSq()
	{
		return (_x * _x) + (_y * _y);
	}

	/// <summary>
	/// Computes the length (magnitude) of this vector: sqrt(x² + y²).
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
	public Vector2 Normalize()
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
	/// <returns>The dot product: (this.x * other.x) + (this.y * other.y).</returns>
	public float Dot(Vector2 other)
	{
		return (_x * other._x) + (_y * other._y);
	}

	/// <summary>
	/// Computes the cross product of this vector with another vector. In two dimensions the result is
	/// the scalar z-component of the equivalent three-dimensional cross product.
	/// </summary>
	/// <param name="other">The other vector.</param>
	/// <returns>The scalar cross product: (this.x * other.y) - (this.y * other.x).</returns>
	public float Cross(Vector2 other)
	{
		return (_x * other._y) - (_y * other._x);
	}

	/// <summary>
	/// Computes the angle of this vector in radians, measured counter-clockwise from the positive
	/// X axis, in the range [0, 2π).
	/// </summary>
	/// <returns>The angle in radians.</returns>
	public float Angle()
	{
		var angle = MathF.Atan2(-_y, -_x) + MathF.PI;
		return angle;
	}

	/// <summary>
	/// Rotates this vector around a centre point by the given angle, mutating it in place.
	/// </summary>
	/// <param name="center">The point to rotate around.</param>
	/// <param name="angle">The rotation angle in radians, counter-clockwise.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector2 RotateAround(Vector2 center, float angle)
	{
		var cos = MathF.Cos(angle);
		var sin = MathF.Sin(angle);
		var deltaX = _x - center._x;
		var deltaY = _y - center._y;
		return Set(
			(deltaX * cos) - (deltaY * sin) + center._x,
			(deltaX * sin) + (deltaY * cos) + center._y);
	}

	/// <summary>
	/// Computes the distance from this vector to another vector.
	/// </summary>
	/// <param name="other">The other vector.</param>
	/// <returns>The Euclidean distance between the two points represented by these vectors.</returns>
	public float DistanceTo(Vector2 other)
	{
		var deltaX = _x - other._x;
		var deltaY = _y - other._y;
		return MathF.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
	}

	/// <summary>
	/// Extracts the components of this vector into an array.
	/// </summary>
	/// <returns>A new array containing [x, y].</returns>
	public float[] ToArray()
	{
		return [_x, _y];
	}

	/// <summary>
	/// Copies two values from an array into this vector's components, mutating it in place.
	/// </summary>
	/// <param name="values">An array with at least two elements. The first two elements are copied.</param>
	/// <returns>This vector, for method chaining.</returns>
	public Vector2 FromArray(float[] values)
	{
		return Set(values[0], values[1]);
	}

	/// <summary>
	/// Creates a shallow copy of this vector with the same component values.
	/// The copy is independent and has no <c>OnChange</c> callback set.
	/// </summary>
	/// <returns>A new vector with the same components.</returns>
	public Vector2 Clone()
	{
		return new Vector2(_x, _y);
	}
}
