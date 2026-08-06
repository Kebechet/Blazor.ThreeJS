namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// A sphere defined by a centre point and a radius. three.js uses it as a geometry's cheap bounding
/// volume and as the first stage of frustum culling.
/// </summary>
public sealed class Sphere
{
	private float _radius = -1f;

	/// <summary>
	/// The centre point. Mutating it in place notifies this sphere's owner, so
	/// <c>sphere.Center.X = 0f</c> is tracked exactly as <see cref="Set"/> would be.
	/// </summary>
	public Vector3 Center { get; }

	/// <summary>Raised whenever the centre or the radius changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes an empty sphere at the origin. The radius of -1 is three.js's own default and is
	/// what makes <see cref="IsEmpty"/> true before any geometry has been measured.
	/// </summary>
	public Sphere()
		: this(new Vector3(), -1f)
	{
	}

	/// <summary>
	/// Initializes a sphere with the given centre and radius.
	/// </summary>
	/// <param name="center">The centre point. Its values are copied; the instance is not retained.</param>
	/// <param name="radius">The radius. A negative radius marks the sphere empty.</param>
	/// <remarks>
	/// The centre is copied rather than aliased. See <see cref="Box3(Vector3, Vector3)"/> for why:
	/// this sphere hangs a change callback off the centre, and retaining a caller's instance would
	/// overwrite whatever callback that instance already carried.
	/// </remarks>
	public Sphere(Vector3 center, float radius)
	{
		Center = center.Clone();
		Center.OnChange = RaiseChanged;
		_radius = radius;
	}

	/// <summary>Gets or sets the radius. A negative radius marks the sphere empty.</summary>
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

	/// <summary>Sets the centre and radius together, triggering the change callback once.</summary>
	/// <param name="center">The new centre, copied by value.</param>
	/// <param name="radius">The new radius.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Sphere Set(Vector3 center, float radius)
	{
		return SetComponents(center.X, center.Y, center.Z, radius);
	}

	/// <summary>Copies the centre and radius from another sphere, mutating this one in place.</summary>
	/// <param name="other">The sphere to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Sphere Copy(Sphere other)
	{
		return SetComponents(other.Center.X, other.Center.Y, other.Center.Z, other._radius);
	}

	/// <summary>Resets this sphere to empty, at the origin with a radius of -1.</summary>
	/// <returns>This instance, for method chaining.</returns>
	public Sphere MakeEmpty()
	{
		return SetComponents(0f, 0f, 0f, -1f);
	}

	/// <summary>
	/// Whether this sphere encloses no volume, which three.js signals with a negative radius. A radius
	/// of exactly zero is <b>not</b> empty: it is the degenerate sphere containing just its centre.
	/// </summary>
	/// <returns><see langword="true"/> when the radius is negative.</returns>
	public bool IsEmpty()
	{
		return _radius < 0f;
	}

	/// <summary>Whether a point lies inside this sphere or on its surface.</summary>
	/// <param name="point">The point to test.</param>
	/// <returns><see langword="true"/> when the point is contained.</returns>
	public bool ContainsPoint(Vector3 point)
	{
		return DistanceSquaredToCenter(point) <= _radius * _radius;
	}

	/// <summary>
	/// Computes the signed distance from a point to this sphere's surface: negative inside, zero on
	/// the surface, positive outside.
	/// </summary>
	/// <param name="point">The point to measure from.</param>
	/// <returns>The signed distance to the surface.</returns>
	public float DistanceToPoint(Vector3 point)
	{
		return MathF.Sqrt(DistanceSquaredToCenter(point)) - _radius;
	}

	/// <summary>Whether this sphere shares any volume with another.</summary>
	/// <param name="other">The sphere to test against.</param>
	/// <returns><see langword="true"/> when the two spheres overlap or touch.</returns>
	public bool IntersectsSphere(Sphere other)
	{
		var reach = _radius + other._radius;
		return DistanceSquaredToCenter(other.Center) <= reach * reach;
	}

	/// <summary>Extracts the centre and radius into an array.</summary>
	/// <returns>A new array containing [centerX, centerY, centerZ, radius].</returns>
	public float[] ToArray()
	{
		return [Center.X, Center.Y, Center.Z, _radius];
	}

	/// <summary>Copies four values from an array into this sphere, mutating it in place.</summary>
	/// <param name="values">An array with at least four elements, in [centerX, centerY, centerZ, radius] order.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Sphere FromArray(float[] values)
	{
		return SetComponents(values[0], values[1], values[2], values[3]);
	}

	/// <summary>Creates a copy of this sphere with the same centre and radius and no change callback.</summary>
	/// <returns>A new sphere with the same definition.</returns>
	public Sphere Clone()
	{
		return new Sphere(Center, _radius);
	}

	private float DistanceSquaredToCenter(Vector3 point)
	{
		var deltaX = point.X - Center.X;
		var deltaY = point.Y - Center.Y;
		var deltaZ = point.Z - Center.Z;
		return (deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ);
	}

	/// <summary>Writes centre and radius together, raising the change callback at most once.</summary>
	private Sphere SetComponents(float centerX, float centerY, float centerZ, float radius)
	{
		var hasChanged = Center.X != centerX || Center.Y != centerY || Center.Z != centerZ || _radius != radius;
		if (!hasChanged)
		{
			return this;
		}

		Center.OnChange = null;
		Center.Set(centerX, centerY, centerZ);
		Center.OnChange = RaiseChanged;
		_radius = radius;

		OnChange?.Invoke();
		return this;
	}

	private void RaiseChanged()
	{
		OnChange?.Invoke();
	}
}
