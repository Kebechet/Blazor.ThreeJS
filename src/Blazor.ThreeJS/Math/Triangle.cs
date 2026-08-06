namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// A triangle defined by its three corner points, in the winding order three.js treats as
/// front-facing (counter-clockwise when viewed from the side the normal points to).
/// </summary>
public sealed class Triangle
{
	/// <summary>The first corner.</summary>
	public Vector3 A { get; }

	/// <summary>The second corner.</summary>
	public Vector3 B { get; }

	/// <summary>The third corner.</summary>
	public Vector3 C { get; }

	/// <summary>Raised whenever any corner changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>Initializes a degenerate triangle with all three corners at the origin.</summary>
	public Triangle()
		: this(new Vector3(), new Vector3(), new Vector3())
	{
	}

	/// <summary>
	/// Initializes a triangle from its three corners.
	/// </summary>
	/// <param name="a">The first corner. Copied; the instance is not retained.</param>
	/// <param name="b">The second corner. Copied; the instance is not retained.</param>
	/// <param name="c">The third corner. Copied; the instance is not retained.</param>
	/// <remarks>
	/// All three are copied rather than aliased. See <see cref="Box3(Vector3, Vector3)"/> for why.
	/// </remarks>
	public Triangle(Vector3 a, Vector3 b, Vector3 c)
	{
		A = a.Clone();
		B = b.Clone();
		C = c.Clone();
		A.OnChange = RaiseChanged;
		B.OnChange = RaiseChanged;
		C.OnChange = RaiseChanged;
	}

	/// <summary>Sets all three corners together, triggering the change callback once.</summary>
	/// <param name="a">The new first corner, copied by value.</param>
	/// <param name="b">The new second corner, copied by value.</param>
	/// <param name="c">The new third corner, copied by value.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Triangle Set(Vector3 a, Vector3 b, Vector3 c)
	{
		return FromArray([a.X, a.Y, a.Z, b.X, b.Y, b.Z, c.X, c.Y, c.Z]);
	}

	/// <summary>Copies all three corners from another triangle, mutating this one in place.</summary>
	/// <param name="other">The triangle to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Triangle Copy(Triangle other)
	{
		return FromArray(other.ToArray());
	}

	/// <summary>Computes the area of the triangle.</summary>
	/// <returns>The area, which is zero for a degenerate triangle.</returns>
	public float GetArea()
	{
		var edge1 = new Vector3(C.X - B.X, C.Y - B.Y, C.Z - B.Z);
		var edge2 = new Vector3(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
		return edge1.Cross(edge2).Length() * 0.5f;
	}

	/// <summary>Computes the centroid, the average of the three corners.</summary>
	/// <returns>A new vector at the midpoint of the triangle.</returns>
	public Vector3 GetMidpoint()
	{
		const float third = 1f / 3f;
		return new Vector3(
			(A.X + B.X + C.X) * third,
			(A.Y + B.Y + C.Y) * third,
			(A.Z + B.Z + C.Z) * third);
	}

	/// <summary>
	/// Computes the unit-length surface normal. A degenerate triangle has no defined normal and
	/// yields the zero vector, which is what three.js returns.
	/// </summary>
	/// <returns>A new vector holding the normal.</returns>
	public Vector3 GetNormal()
	{
		var edge1 = new Vector3(C.X - B.X, C.Y - B.Y, C.Z - B.Z);
		var edge2 = new Vector3(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
		var normal = edge1.Cross(edge2);
		return normal.LengthSq() > 0f
			? normal.Normalize()
			: new Vector3();
	}

	/// <summary>Computes the plane this triangle lies in.</summary>
	/// <returns>A new plane through the three corners.</returns>
	public Plane GetPlane()
	{
		return new Plane().SetFromNormalAndCoplanarPoint(GetNormal(), A);
	}

	/// <summary>Extracts all three corners into an array.</summary>
	/// <returns>A new array containing the corners in A, B, C order, three components each.</returns>
	public float[] ToArray()
	{
		return [A.X, A.Y, A.Z, B.X, B.Y, B.Z, C.X, C.Y, C.Z];
	}

	/// <summary>Copies nine values from an array into the three corners, mutating this triangle in place.</summary>
	/// <param name="values">An array with at least nine elements, three per corner in A, B, C order.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Triangle FromArray(float[] values)
	{
		var current = ToArray();
		var hasChanged = false;
		for (var index = 0; index < current.Length; index++)
		{
			if (current[index] != values[index])
			{
				hasChanged = true;
				break;
			}
		}

		if (!hasChanged)
		{
			return this;
		}

		A.OnChange = null;
		B.OnChange = null;
		C.OnChange = null;
		A.Set(values[0], values[1], values[2]);
		B.Set(values[3], values[4], values[5]);
		C.Set(values[6], values[7], values[8]);
		A.OnChange = RaiseChanged;
		B.OnChange = RaiseChanged;
		C.OnChange = RaiseChanged;

		OnChange?.Invoke();
		return this;
	}

	/// <summary>Creates a copy of this triangle with the same corners and no change callback.</summary>
	/// <returns>A new triangle with the same corners.</returns>
	public Triangle Clone()
	{
		return new Triangle(A, B, C);
	}

	private void RaiseChanged()
	{
		OnChange?.Invoke();
	}
}
