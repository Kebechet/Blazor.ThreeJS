namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// A line segment between two points. Unlike <see cref="Ray"/> it is bounded at both ends, so
/// distances along it clamp to the segment.
/// </summary>
public sealed class Line3
{
	/// <summary>The point the segment starts at.</summary>
	public Vector3 Start { get; }

	/// <summary>The point the segment ends at.</summary>
	public Vector3 End { get; }

	/// <summary>Raised whenever either endpoint changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>Initializes a degenerate segment with both endpoints at the origin.</summary>
	public Line3()
		: this(new Vector3(), new Vector3())
	{
	}

	/// <summary>
	/// Initializes a segment between two points.
	/// </summary>
	/// <param name="start">The starting point. Copied; the instance is not retained.</param>
	/// <param name="end">The ending point. Copied; the instance is not retained.</param>
	/// <remarks>
	/// Both are copied rather than aliased. See <see cref="Box3(Vector3, Vector3)"/> for why.
	/// </remarks>
	public Line3(Vector3 start, Vector3 end)
	{
		Start = start.Clone();
		End = end.Clone();
		Start.OnChange = RaiseChanged;
		End.OnChange = RaiseChanged;
	}

	/// <summary>Sets both endpoints together, triggering the change callback once.</summary>
	/// <param name="start">The new starting point, copied by value.</param>
	/// <param name="end">The new ending point, copied by value.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Line3 Set(Vector3 start, Vector3 end)
	{
		return SetComponents(start.X, start.Y, start.Z, end.X, end.Y, end.Z);
	}

	/// <summary>Copies both endpoints from another segment, mutating this one in place.</summary>
	/// <param name="other">The segment to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Line3 Copy(Line3 other)
	{
		return SetComponents(other.Start.X, other.Start.Y, other.Start.Z, other.End.X, other.End.Y, other.End.Z);
	}

	/// <summary>Computes the midpoint of the segment.</summary>
	/// <returns>A new vector halfway between the endpoints.</returns>
	public Vector3 GetCenter()
	{
		return new Vector3(
			(Start.X + End.X) * 0.5f,
			(Start.Y + End.Y) * 0.5f,
			(Start.Z + End.Z) * 0.5f);
	}

	/// <summary>Computes the vector from the start of the segment to its end.</summary>
	/// <returns>A new vector holding the delta.</returns>
	public Vector3 Delta()
	{
		return new Vector3(End.X - Start.X, End.Y - Start.Y, End.Z - Start.Z);
	}

	/// <summary>Computes the squared length of the segment.</summary>
	/// <returns>The squared distance between the endpoints.</returns>
	public float DistanceSq()
	{
		return Delta().LengthSq();
	}

	/// <summary>Computes the length of the segment.</summary>
	/// <returns>The distance between the endpoints.</returns>
	public float Distance()
	{
		return Delta().Length();
	}

	/// <summary>
	/// Computes the point a fraction of the way along the segment.
	/// </summary>
	/// <param name="t">The fraction, where 0 is <see cref="Start"/> and 1 is <see cref="End"/>.</param>
	/// <returns>A new vector at that point.</returns>
	public Vector3 At(float t)
	{
		var delta = Delta();
		return new Vector3(
			Start.X + (delta.X * t),
			Start.Y + (delta.Y * t),
			Start.Z + (delta.Z * t));
	}

	/// <summary>
	/// Finds how far along the segment a point projects, as a fraction clamped to [0, 1] so the
	/// result always names a point on the segment rather than on its infinite extension.
	/// </summary>
	/// <param name="point">The point to project.</param>
	/// <returns>The clamped fraction along the segment.</returns>
	public float ClosestPointToPointParameter(Vector3 point)
	{
		var delta = Delta();
		var lengthSq = delta.LengthSq();
		if (lengthSq == 0f)
		{
			return 0f;
		}

		var offset = new Vector3(point.X - Start.X, point.Y - Start.Y, point.Z - Start.Z);
		return System.Math.Clamp(offset.Dot(delta) / lengthSq, 0f, 1f);
	}

	/// <summary>Computes the closest point on this segment to an arbitrary point.</summary>
	/// <param name="point">The point to approach.</param>
	/// <returns>A new vector at the closest point on the segment.</returns>
	public Vector3 ClosestPointToPoint(Vector3 point)
	{
		return At(ClosestPointToPointParameter(point));
	}

	/// <summary>Extracts both endpoints into an array.</summary>
	/// <returns>A new array containing [startX, startY, startZ, endX, endY, endZ].</returns>
	public float[] ToArray()
	{
		return [Start.X, Start.Y, Start.Z, End.X, End.Y, End.Z];
	}

	/// <summary>Copies six values from an array into this segment, mutating it in place.</summary>
	/// <param name="values">An array with at least six elements: the start followed by the end.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Line3 FromArray(float[] values)
	{
		return SetComponents(values[0], values[1], values[2], values[3], values[4], values[5]);
	}

	/// <summary>Creates a copy of this segment with the same endpoints and no change callback.</summary>
	/// <returns>A new segment between the same points.</returns>
	public Line3 Clone()
	{
		return new Line3(Start, End);
	}

	/// <summary>Writes both endpoints together, raising the change callback at most once.</summary>
	private Line3 SetComponents(float startX, float startY, float startZ, float endX, float endY, float endZ)
	{
		var hasChanged = Start.X != startX || Start.Y != startY || Start.Z != startZ ||
			End.X != endX || End.Y != endY || End.Z != endZ;

		if (!hasChanged)
		{
			return this;
		}

		Start.OnChange = null;
		End.OnChange = null;
		Start.Set(startX, startY, startZ);
		End.Set(endX, endY, endZ);
		Start.OnChange = RaiseChanged;
		End.OnChange = RaiseChanged;

		OnChange?.Invoke();
		return this;
	}

	private void RaiseChanged()
	{
		OnChange?.Invoke();
	}
}
