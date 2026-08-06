namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// A point in cylindrical coordinates: a radius out from the Y axis, an angle around it, and a
/// height along it.
/// </summary>
public sealed class Cylindrical
{
	private float _radius = 1f;
	private float _theta;
	private float _y;

	/// <summary>Raised whenever any component changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a point at radius one, angle zero, height zero.
	/// </summary>
	public Cylindrical()
	{
	}

	/// <summary>
	/// Initializes a point with the given cylindrical coordinates.
	/// </summary>
	/// <param name="radius">Distance from the Y axis.</param>
	/// <param name="theta">Angle in radians around the Y axis, measured from the positive Z axis.</param>
	/// <param name="y">Height along the Y axis.</param>
	public Cylindrical(float radius, float theta, float y)
	{
		_radius = radius;
		_theta = theta;
		_y = y;
	}

	/// <summary>Gets or sets the distance from the Y axis.</summary>
	public float Radius
	{
		get { return _radius; }
		set
		{
			if (_radius == value)
			{
				return;
			}

			_radius = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>Gets or sets the angle in radians around the Y axis, measured from the positive Z axis.</summary>
	public float Theta
	{
		get { return _theta; }
		set
		{
			if (_theta == value)
			{
				return;
			}

			_theta = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>Gets or sets the height along the Y axis.</summary>
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
	/// Sets all three coordinates and triggers the change callback once.
	/// </summary>
	/// <param name="radius">Distance from the Y axis.</param>
	/// <param name="theta">Angle in radians around the Y axis.</param>
	/// <param name="y">Height along the Y axis.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Cylindrical Set(float radius, float theta, float y)
	{
		if (_radius == radius && _theta == theta && _y == y)
		{
			return this;
		}

		_radius = radius;
		_theta = theta;
		_y = y;
		OnChange?.Invoke();
		return this;
	}

	/// <summary>Copies the coordinates from another instance, mutating this one in place.</summary>
	/// <param name="other">The instance to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Cylindrical Copy(Cylindrical other)
	{
		return Set(other._radius, other._theta, other._y);
	}

	/// <summary>
	/// Sets these coordinates from a Cartesian point, mutating this instance in place.
	/// </summary>
	/// <param name="vector">The Cartesian point to convert.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Cylindrical SetFromVector3(Vector3 vector)
	{
		return Set(
			MathF.Sqrt((vector.X * vector.X) + (vector.Z * vector.Z)),
			MathF.Atan2(vector.X, vector.Z),
			vector.Y);
	}

	/// <summary>
	/// Converts these coordinates to a Cartesian point.
	/// </summary>
	/// <returns>A new vector holding the equivalent Cartesian position.</returns>
	public Vector3 ToVector3()
	{
		return new Vector3(
			_radius * MathF.Sin(_theta),
			_y,
			_radius * MathF.Cos(_theta));
	}

	/// <summary>Extracts the coordinates into an array.</summary>
	/// <returns>A new array containing [radius, theta, y].</returns>
	public float[] ToArray()
	{
		return [_radius, _theta, _y];
	}

	/// <summary>Copies three values from an array into these coordinates, mutating this instance in place.</summary>
	/// <param name="values">An array with at least three elements, in [radius, theta, y] order.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Cylindrical FromArray(float[] values)
	{
		return Set(values[0], values[1], values[2]);
	}

	/// <summary>Creates a copy with the same coordinates and no change callback.</summary>
	/// <returns>A new instance with the same coordinates.</returns>
	public Cylindrical Clone()
	{
		return new Cylindrical(_radius, _theta, _y);
	}
}
