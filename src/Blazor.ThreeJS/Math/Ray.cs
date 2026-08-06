namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// A half-line: an origin and a direction it travels in forever. This is what a
/// <c>Raycaster</c> casts, and what a pick test intersects geometry with.
/// </summary>
public sealed class Ray
{
	/// <summary>The point the ray starts from.</summary>
	public Vector3 Origin { get; }

	/// <summary>
	/// The direction the ray travels, expected to be unit length. It is not re-normalized
	/// automatically, matching three.js - a non-unit direction rescales every distance the ray reports.
	/// </summary>
	public Vector3 Direction { get; }

	/// <summary>Raised whenever the origin or direction changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a ray at the origin pointing down the negative Z axis, which is three.js's own
	/// default and the direction a default camera looks.
	/// </summary>
	public Ray()
		: this(new Vector3(), new Vector3(0f, 0f, -1f))
	{
	}

	/// <summary>
	/// Initializes a ray from an origin and a direction.
	/// </summary>
	/// <param name="origin">The starting point. Copied; the instance is not retained.</param>
	/// <param name="direction">The travel direction, expected to be unit length. Copied; the instance is not retained.</param>
	/// <remarks>
	/// Both are copied rather than aliased. See <see cref="Box3(Vector3, Vector3)"/> for why.
	/// </remarks>
	public Ray(Vector3 origin, Vector3 direction)
	{
		Origin = origin.Clone();
		Direction = direction.Clone();
		Origin.OnChange = RaiseChanged;
		Direction.OnChange = RaiseChanged;
	}

	/// <summary>Sets the origin and direction together, triggering the change callback once.</summary>
	/// <param name="origin">The new origin, copied by value.</param>
	/// <param name="direction">The new direction, copied by value.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Ray Set(Vector3 origin, Vector3 direction)
	{
		return SetComponents(origin.X, origin.Y, origin.Z, direction.X, direction.Y, direction.Z);
	}

	/// <summary>Copies the origin and direction from another ray, mutating this one in place.</summary>
	/// <param name="other">The ray to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Ray Copy(Ray other)
	{
		return SetComponents(
			other.Origin.X, other.Origin.Y, other.Origin.Z,
			other.Direction.X, other.Direction.Y, other.Direction.Z);
	}

	/// <summary>
	/// Points this ray from its origin towards a target, mutating it in place and normalizing the
	/// resulting direction.
	/// </summary>
	/// <param name="target">The point to aim at.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Ray LookAt(Vector3 target)
	{
		var direction = new Vector3(target.X - Origin.X, target.Y - Origin.Y, target.Z - Origin.Z).Normalize();
		return SetComponents(Origin.X, Origin.Y, Origin.Z, direction.X, direction.Y, direction.Z);
	}

	/// <summary>Computes the point a given distance along this ray.</summary>
	/// <param name="distance">How far along the direction to travel.</param>
	/// <returns>A new vector at that point.</returns>
	public Vector3 At(float distance)
	{
		return new Vector3(
			Origin.X + (Direction.X * distance),
			Origin.Y + (Direction.Y * distance),
			Origin.Z + (Direction.Z * distance));
	}

	/// <summary>
	/// Computes the closest point on this ray to an arbitrary point. Points behind the origin clamp to
	/// the origin, since a ray is a half-line rather than a full line.
	/// </summary>
	/// <param name="point">The point to approach.</param>
	/// <returns>A new vector at the closest point on the ray.</returns>
	public Vector3 ClosestPointToPoint(Vector3 point)
	{
		var offset = new Vector3(point.X - Origin.X, point.Y - Origin.Y, point.Z - Origin.Z);
		var travel = offset.Dot(Direction);
		if (travel < 0f)
		{
			return Origin.Clone();
		}

		return At(travel);
	}

	/// <summary>Computes the distance from this ray to a point.</summary>
	/// <param name="point">The point to measure to.</param>
	/// <returns>The distance from the point to the nearest point on the ray.</returns>
	public float DistanceToPoint(Vector3 point)
	{
		return ClosestPointToPoint(point).DistanceTo(point);
	}

	/// <summary>Extracts the origin and direction into an array.</summary>
	/// <returns>A new array containing [originX, originY, originZ, directionX, directionY, directionZ].</returns>
	public float[] ToArray()
	{
		return [Origin.X, Origin.Y, Origin.Z, Direction.X, Direction.Y, Direction.Z];
	}

	/// <summary>Copies six values from an array into this ray, mutating it in place.</summary>
	/// <param name="values">An array with at least six elements: the origin followed by the direction.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Ray FromArray(float[] values)
	{
		return SetComponents(values[0], values[1], values[2], values[3], values[4], values[5]);
	}

	/// <summary>Creates a copy of this ray with the same definition and no change callback.</summary>
	/// <returns>A new ray with the same origin and direction.</returns>
	public Ray Clone()
	{
		return new Ray(Origin, Direction);
	}

	/// <summary>Writes origin and direction together, raising the change callback at most once.</summary>
	private Ray SetComponents(float originX, float originY, float originZ, float directionX, float directionY, float directionZ)
	{
		var hasChanged = Origin.X != originX || Origin.Y != originY || Origin.Z != originZ ||
			Direction.X != directionX || Direction.Y != directionY || Direction.Z != directionZ;

		if (!hasChanged)
		{
			return this;
		}

		Origin.OnChange = null;
		Direction.OnChange = null;
		Origin.Set(originX, originY, originZ);
		Direction.Set(directionX, directionY, directionZ);
		Origin.OnChange = RaiseChanged;
		Direction.OnChange = RaiseChanged;

		OnChange?.Invoke();
		return this;
	}

	private void RaiseChanged()
	{
		OnChange?.Invoke();
	}
}
