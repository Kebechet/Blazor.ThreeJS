namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// A point in spherical coordinates: a radius, a polar angle from the positive Y axis, and an
/// equatorial angle in the XZ plane. three.js uses this to orbit a camera around a target without
/// accumulating error in Cartesian space.
/// </summary>
public sealed class Spherical
{
	private float _radius = 1f;
	private float _phi;
	private float _theta;

	/// <summary>Raised whenever any component changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a point at radius one, on the positive Y axis.
	/// </summary>
	public Spherical()
	{
	}

	/// <summary>
	/// Initializes a point with the given spherical coordinates.
	/// </summary>
	/// <param name="radius">Distance from the origin.</param>
	/// <param name="phi">Polar angle in radians, measured from the positive Y axis.</param>
	/// <param name="theta">Equatorial angle in radians, in the XZ plane measured from the positive Z axis.</param>
	public Spherical(float radius, float phi, float theta)
	{
		_radius = radius;
		_phi = phi;
		_theta = theta;
	}

	/// <summary>Gets or sets the distance from the origin.</summary>
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

	/// <summary>Gets or sets the polar angle in radians, measured from the positive Y axis.</summary>
	public float Phi
	{
		get { return _phi; }
		set
		{
			if (_phi == value)
			{
				return;
			}

			_phi = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Gets or sets the equatorial angle in radians, in the XZ plane measured from the positive Z axis.
	/// </summary>
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

	/// <summary>
	/// Sets all three coordinates and triggers the change callback once.
	/// </summary>
	/// <param name="radius">Distance from the origin.</param>
	/// <param name="phi">Polar angle in radians.</param>
	/// <param name="theta">Equatorial angle in radians.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Spherical Set(float radius, float phi, float theta)
	{
		if (_radius == radius && _phi == phi && _theta == theta)
		{
			return this;
		}

		_radius = radius;
		_phi = phi;
		_theta = theta;
		OnChange?.Invoke();
		return this;
	}

	/// <summary>Copies the coordinates from another instance, mutating this one in place.</summary>
	/// <param name="other">The instance to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Spherical Copy(Spherical other)
	{
		return Set(other._radius, other._phi, other._theta);
	}

	/// <summary>
	/// Sets these coordinates from a Cartesian point, mutating this instance in place.
	/// </summary>
	/// <param name="vector">The Cartesian point to convert.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Spherical SetFromVector3(Vector3 vector)
	{
		var radius = vector.Length();
		if (radius == 0f)
		{
			return Set(0f, 0f, 0f);
		}

		return Set(
			radius,
			MathF.Acos(System.Math.Clamp(vector.Y / radius, -1f, 1f)),
			MathF.Atan2(vector.X, vector.Z));
	}

	/// <summary>
	/// Converts these coordinates to a Cartesian point.
	/// </summary>
	/// <returns>A new vector holding the equivalent Cartesian position.</returns>
	public Vector3 ToVector3()
	{
		var sinPhiRadius = MathF.Sin(_phi) * _radius;
		return new Vector3(
			sinPhiRadius * MathF.Sin(_theta),
			MathF.Cos(_phi) * _radius,
			sinPhiRadius * MathF.Cos(_theta));
	}

	/// <summary>Extracts the coordinates into an array.</summary>
	/// <returns>A new array containing [radius, phi, theta].</returns>
	public float[] ToArray()
	{
		return [_radius, _phi, _theta];
	}

	/// <summary>Copies three values from an array into these coordinates, mutating this instance in place.</summary>
	/// <param name="values">An array with at least three elements, in [radius, phi, theta] order.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Spherical FromArray(float[] values)
	{
		return Set(values[0], values[1], values[2]);
	}

	/// <summary>Creates a copy with the same coordinates and no change callback.</summary>
	/// <returns>A new instance with the same coordinates.</returns>
	public Spherical Clone()
	{
		return new Spherical(_radius, _phi, _theta);
	}
}
