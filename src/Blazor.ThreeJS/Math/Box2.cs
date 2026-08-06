namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// An axis-aligned bounding box in two dimensions, defined by its minimum and maximum corners.
/// <para>
/// A default-constructed box is <b>empty</b>, not zero-sized: three.js seeds the minimum at positive
/// infinity and the maximum at negative infinity so that <see cref="ExpandByPoint"/> over any set of
/// points yields their true bounds without the origin being counted as a member.
/// </para>
/// </summary>
public sealed class Box2
{
	/// <summary>The corner with the smallest coordinate on each axis.</summary>
	public Vector2 Min { get; }

	/// <summary>The corner with the largest coordinate on each axis.</summary>
	public Vector2 Max { get; }

	/// <summary>Raised whenever either corner changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes an empty box, whose minimum is positive infinity on both axes and whose maximum is
	/// negative infinity.
	/// </summary>
	public Box2()
		: this(
			new Vector2(float.PositiveInfinity, float.PositiveInfinity),
			new Vector2(float.NegativeInfinity, float.NegativeInfinity))
	{
	}

	/// <summary>
	/// Initializes a box spanning the two given corners.
	/// </summary>
	/// <param name="min">The minimum corner. Its values are copied; the instance is not retained.</param>
	/// <param name="max">The maximum corner. Its values are copied; the instance is not retained.</param>
	/// <remarks>
	/// Both corners are copied rather than aliased. See <see cref="Box3(Vector3, Vector3)"/> for why:
	/// this box hangs a change callback off each corner, and retaining a caller's instance would
	/// overwrite whatever callback that instance already carried.
	/// </remarks>
	public Box2(Vector2 min, Vector2 max)
	{
		Min = min.Clone();
		Max = max.Clone();
		Min.OnChange = RaiseChanged;
		Max.OnChange = RaiseChanged;
	}

	/// <summary>Sets both corners and triggers the change callback once.</summary>
	/// <param name="min">The new minimum corner, copied by value.</param>
	/// <param name="max">The new maximum corner, copied by value.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box2 Set(Vector2 min, Vector2 max)
	{
		return SetComponents(min.X, min.Y, max.X, max.Y);
	}

	/// <summary>Resets this box to empty.</summary>
	/// <returns>This instance, for method chaining.</returns>
	public Box2 MakeEmpty()
	{
		return SetComponents(
			float.PositiveInfinity, float.PositiveInfinity,
			float.NegativeInfinity, float.NegativeInfinity);
	}

	/// <summary>Whether this box encloses no area.</summary>
	/// <returns><see langword="true"/> when the box is empty.</returns>
	public bool IsEmpty()
	{
		return Max.X < Min.X || Max.Y < Min.Y;
	}

	/// <summary>Copies both corners from another box, mutating this one in place.</summary>
	/// <param name="other">The box to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box2 Copy(Box2 other)
	{
		return SetComponents(other.Min.X, other.Min.Y, other.Max.X, other.Max.Y);
	}

	/// <summary>Grows this box just enough to contain the given point, mutating it in place.</summary>
	/// <param name="point">The point to include.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box2 ExpandByPoint(Vector2 point)
	{
		return SetComponents(
			MathF.Min(Min.X, point.X), MathF.Min(Min.Y, point.Y),
			MathF.Max(Max.X, point.X), MathF.Max(Max.Y, point.Y));
	}

	/// <summary>Sets this box to the bounds of a set of points, mutating it in place.</summary>
	/// <param name="points">The points to enclose.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box2 SetFromPoints(IEnumerable<Vector2> points)
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
	public bool ContainsPoint(Vector2 point)
	{
		return point.X >= Min.X && point.X <= Max.X &&
			point.Y >= Min.Y && point.Y <= Max.Y;
	}

	/// <summary>Whether this box shares any area with another.</summary>
	/// <param name="other">The box to test against.</param>
	/// <returns><see langword="true"/> when the two boxes overlap or touch.</returns>
	public bool IntersectsBox(Box2 other)
	{
		return other.Max.X >= Min.X && other.Min.X <= Max.X &&
			other.Max.Y >= Min.Y && other.Min.Y <= Max.Y;
	}

	/// <summary>Grows this box to also contain another box, mutating it in place.</summary>
	/// <param name="other">The box to absorb.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box2 Union(Box2 other)
	{
		return SetComponents(
			MathF.Min(Min.X, other.Min.X), MathF.Min(Min.Y, other.Min.Y),
			MathF.Max(Max.X, other.Max.X), MathF.Max(Max.Y, other.Max.Y));
	}

	/// <summary>Computes the point at the centre of this box; an empty box yields the origin.</summary>
	/// <returns>A new vector at the centre.</returns>
	public Vector2 GetCenter()
	{
		if (IsEmpty())
		{
			return new Vector2();
		}

		return new Vector2((Min.X + Max.X) * 0.5f, (Min.Y + Max.Y) * 0.5f);
	}

	/// <summary>Computes the extent of this box on each axis; an empty box yields the zero vector.</summary>
	/// <returns>A new vector holding the width and height.</returns>
	public Vector2 GetSize()
	{
		if (IsEmpty())
		{
			return new Vector2();
		}

		return new Vector2(Max.X - Min.X, Max.Y - Min.Y);
	}

	/// <summary>Extracts both corners into an array.</summary>
	/// <returns>A new array containing [minX, minY, maxX, maxY].</returns>
	public float[] ToArray()
	{
		return [Min.X, Min.Y, Max.X, Max.Y];
	}

	/// <summary>Copies four values from an array into both corners, mutating this box in place.</summary>
	/// <param name="values">An array with at least four elements, in [minX, minY, maxX, maxY] order.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Box2 FromArray(float[] values)
	{
		return SetComponents(values[0], values[1], values[2], values[3]);
	}

	/// <summary>Creates a copy of this box with the same corners and no change callback.</summary>
	/// <returns>A new box spanning the same bounds.</returns>
	public Box2 Clone()
	{
		return new Box2(Min, Max);
	}

	/// <summary>Writes all four components, raising the change callback at most once.</summary>
	private Box2 SetComponents(float minX, float minY, float maxX, float maxY)
	{
		var hasChanged = Min.X != minX || Min.Y != minY || Max.X != maxX || Max.Y != maxY;
		if (!hasChanged)
		{
			return this;
		}

		Min.OnChange = null;
		Max.OnChange = null;
		Min.Set(minX, minY);
		Max.Set(maxX, maxY);
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
