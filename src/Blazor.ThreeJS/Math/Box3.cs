namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// An axis-aligned bounding box in three dimensions, defined by its minimum and maximum corners.
/// <para>
/// A default-constructed box is <b>empty</b>, not zero-sized: three.js seeds the minimum at positive
/// infinity and the maximum at negative infinity so that <see cref="ExpandByPoint"/> over any set of
/// points yields their true bounds without the origin being counted as a member. Those infinities
/// travel over the wire as named tokens - see <c>ThreeWireFormat.PositiveInfinityToken</c> - because
/// JSON has no numeric form for them.
/// </para>
/// </summary>
public sealed class Box3
{
	/// <summary>
	/// The corner with the smallest coordinate on each axis. Mutating it in place notifies this box's
	/// owner, so <c>box.Min.X = 0f</c> is tracked exactly as <c>box.Set(...)</c> would be.
	/// </summary>
	public Vector3 Min { get; }

	/// <summary>The corner with the largest coordinate on each axis.</summary>
	public Vector3 Max { get; }

	/// <summary>Raised whenever either corner changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes an empty box, whose minimum is positive infinity on every axis and whose maximum
	/// is negative infinity. This is three.js's own default and is what makes expansion work.
	/// </summary>
	public Box3()
		: this(
			new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity),
			new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity))
	{
	}

	/// <summary>
	/// Initializes a box spanning the two given corners.
	/// </summary>
	/// <param name="min">The minimum corner. Its values are copied; the instance is not retained.</param>
	/// <param name="max">The maximum corner. Its values are copied; the instance is not retained.</param>
	/// <remarks>
	/// Both corners are copied rather than aliased, which is where this departs from three.js. This box
	/// hangs a change callback off each corner so that mutating one is observable; retaining a caller's
	/// instance would overwrite whatever callback that instance already carried, silently unhooking the
	/// object it belonged to.
	/// </remarks>
	public Box3(Vector3 min, Vector3 max)
	{
		Min = min.Clone();
		Max = max.Clone();
		Min.OnChange = RaiseChanged;
		Max.OnChange = RaiseChanged;
	}

	/// <summary>
	/// Sets both corners and triggers the change callback once.
	/// </summary>
	/// <param name="min">The new minimum corner, copied by value.</param>
	/// <param name="max">The new maximum corner, copied by value.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box3 Set(Vector3 min, Vector3 max)
	{
		return SetComponents(min.X, min.Y, min.Z, max.X, max.Y, max.Z);
	}

	/// <summary>
	/// Resets this box to empty, so that expanding it by a point yields exactly that point.
	/// </summary>
	/// <returns>This instance, for method chaining.</returns>
	public Box3 MakeEmpty()
	{
		return SetComponents(
			float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity,
			float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
	}

	/// <summary>
	/// Whether this box encloses no volume. True for a default-constructed box, and for any box whose
	/// maximum has fallen below its minimum on an axis.
	/// </summary>
	/// <returns><see langword="true"/> when the box is empty.</returns>
	public bool IsEmpty()
	{
		return Max.X < Min.X || Max.Y < Min.Y || Max.Z < Min.Z;
	}

	/// <summary>Copies both corners from another box, mutating this one in place.</summary>
	/// <param name="other">The box to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box3 Copy(Box3 other)
	{
		return SetComponents(other.Min.X, other.Min.Y, other.Min.Z, other.Max.X, other.Max.Y, other.Max.Z);
	}

	/// <summary>
	/// Grows this box just enough to contain the given point, mutating it in place.
	/// </summary>
	/// <param name="point">The point to include.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box3 ExpandByPoint(Vector3 point)
	{
		return SetComponents(
			MathF.Min(Min.X, point.X), MathF.Min(Min.Y, point.Y), MathF.Min(Min.Z, point.Z),
			MathF.Max(Max.X, point.X), MathF.Max(Max.Y, point.Y), MathF.Max(Max.Z, point.Z));
	}

	/// <summary>
	/// Sets this box to the bounds of a set of points, mutating it in place. An empty sequence leaves
	/// the box empty.
	/// </summary>
	/// <param name="points">The points to enclose.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box3 SetFromPoints(IEnumerable<Vector3> points)
	{
		MakeEmpty();
		foreach (var point in points)
		{
			ExpandByPoint(point);
		}

		return this;
	}

	/// <summary>Whether a point lies inside this box or on its boundary.</summary>
	/// <param name="point">The point to test.</param>
	/// <returns><see langword="true"/> when the point is contained.</returns>
	public bool ContainsPoint(Vector3 point)
	{
		return point.X >= Min.X && point.X <= Max.X &&
			point.Y >= Min.Y && point.Y <= Max.Y &&
			point.Z >= Min.Z && point.Z <= Max.Z;
	}

	/// <summary>Whether this box shares any volume with another.</summary>
	/// <param name="other">The box to test against.</param>
	/// <returns><see langword="true"/> when the two boxes overlap or touch.</returns>
	public bool IntersectsBox(Box3 other)
	{
		return other.Max.X >= Min.X && other.Min.X <= Max.X &&
			other.Max.Y >= Min.Y && other.Min.Y <= Max.Y &&
			other.Max.Z >= Min.Z && other.Min.Z <= Max.Z;
	}

	/// <summary>Grows this box to also contain another box, mutating it in place.</summary>
	/// <param name="other">The box to absorb.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box3 Union(Box3 other)
	{
		return SetComponents(
			MathF.Min(Min.X, other.Min.X), MathF.Min(Min.Y, other.Min.Y), MathF.Min(Min.Z, other.Min.Z),
			MathF.Max(Max.X, other.Max.X), MathF.Max(Max.Y, other.Max.Y), MathF.Max(Max.Z, other.Max.Z));
	}

	/// <summary>
	/// Computes the point at the centre of this box. An empty box has no meaningful centre and yields
	/// the origin, which is what three.js returns.
	/// </summary>
	/// <returns>A new vector at the centre.</returns>
	public Vector3 GetCenter()
	{
		if (IsEmpty())
		{
			return new Vector3();
		}

		return new Vector3(
			(Min.X + Max.X) * 0.5f,
			(Min.Y + Max.Y) * 0.5f,
			(Min.Z + Max.Z) * 0.5f);
	}

	/// <summary>
	/// Computes the extent of this box on each axis. An empty box yields the zero vector.
	/// </summary>
	/// <returns>A new vector holding the width, height and depth.</returns>
	public Vector3 GetSize()
	{
		if (IsEmpty())
		{
			return new Vector3();
		}

		return new Vector3(Max.X - Min.X, Max.Y - Min.Y, Max.Z - Min.Z);
	}

	/// <summary>Extracts both corners into an array.</summary>
	/// <returns>A new array containing [minX, minY, minZ, maxX, maxY, maxZ].</returns>
	public float[] ToArray()
	{
		return [Min.X, Min.Y, Min.Z, Max.X, Max.Y, Max.Z];
	}

	/// <summary>Copies six values from an array into both corners, mutating this box in place.</summary>
	/// <param name="values">An array with at least six elements, in [minX, minY, minZ, maxX, maxY, maxZ] order.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box3 FromArray(float[] values)
	{
		return SetComponents(values[0], values[1], values[2], values[3], values[4], values[5]);
	}

	/// <summary>Creates a copy of this box with the same corners and no change callback.</summary>
	/// <returns>A new box spanning the same bounds.</returns>
	public Box3 Clone()
	{
		return new Box3(Min, Max);
	}

	/// <summary>
	/// Writes all six components, raising the change callback at most once however many of them moved.
	/// The corner writes go through the fields' own setters, which are silenced first so that a
	/// six-component update cannot announce itself six times.
	/// </summary>
	private Box3 SetComponents(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
	{
		var hasChanged = Min.X != minX || Min.Y != minY || Min.Z != minZ ||
			Max.X != maxX || Max.Y != maxY || Max.Z != maxZ;

		if (!hasChanged)
		{
			return this;
		}

		Min.OnChange = null;
		Max.OnChange = null;
		Min.Set(minX, minY, minZ);
		Max.Set(maxX, maxY, maxZ);
		Min.OnChange = RaiseChanged;
		Max.OnChange = RaiseChanged;

		OnChange?.Invoke();
		return this;
	}

	private void RaiseChanged()
	{
		OnChange?.Invoke();
	}
}
