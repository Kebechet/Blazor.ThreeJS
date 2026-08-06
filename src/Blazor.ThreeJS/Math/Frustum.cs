namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// The six-sided volume a camera can see, held as six inward-facing planes. three.js tests an
/// object's bounding sphere against it to decide whether the object is worth drawing.
/// </summary>
public sealed class Frustum
{
	/// <summary>How many planes bound a frustum: left, right, bottom, top, near and far.</summary>
	public const int PlaneCount = 6;

	private readonly Plane[] _planes;

	/// <summary>
	/// The six bounding planes, each with its normal pointing into the volume. Mutating one in place
	/// notifies this frustum's owner.
	/// </summary>
	public IReadOnlyList<Plane> Planes
	{
		get { return _planes; }
	}

	/// <summary>Raised whenever any plane changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a frustum whose six planes are all the default <see cref="Plane"/>. This is
	/// three.js's own default and does not describe a usable volume until
	/// <see cref="SetFromProjectionMatrix"/> has filled it in.
	/// </summary>
	public Frustum()
	{
		_planes = new Plane[PlaneCount];
		for (var index = 0; index < PlaneCount; index++)
		{
			var plane = new Plane();
			plane.OnChange = RaiseChanged;
			_planes[index] = plane;
		}
	}

	/// <summary>
	/// Initializes a frustum from six planes, in three.js's own order: right, left, bottom, top, far,
	/// near.
	/// </summary>
	/// <param name="planes">Exactly six planes. Their values are copied; the instances are not retained.</param>
	/// <exception cref="ArgumentException">Thrown when the count is not exactly six.</exception>
	public Frustum(IReadOnlyList<Plane> planes)
		: this()
	{
		if (planes.Count != PlaneCount)
		{
			throw new ArgumentException($"A frustum is bounded by exactly {PlaneCount} planes, but {planes.Count} were given.", nameof(planes));
		}

		for (var index = 0; index < PlaneCount; index++)
		{
			_planes[index].Copy(planes[index]);
		}
	}

	/// <summary>Copies all six planes from another frustum, mutating this one in place.</summary>
	/// <param name="other">The frustum to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Frustum Copy(Frustum other)
	{
		return FromArray(other.ToArray());
	}

	/// <summary>
	/// Derives the six planes from a camera's combined projection-view matrix, mutating this frustum
	/// in place. The plane order matches three.js: right, left, bottom, top, far, near.
	/// </summary>
	/// <param name="matrix">The projection matrix multiplied by the camera's inverse world matrix.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Frustum SetFromProjectionMatrix(Matrix4 matrix)
	{
		var m = matrix.Elements;
		var m0 = m[0]; var m1 = m[1]; var m2 = m[2]; var m3 = m[3];
		var m4 = m[4]; var m5 = m[5]; var m6 = m[6]; var m7 = m[7];
		var m8 = m[8]; var m9 = m[9]; var m10 = m[10]; var m11 = m[11];
		var m12 = m[12]; var m13 = m[13]; var m14 = m[14]; var m15 = m[15];

		var values = new float[PlaneCount * 4];
		WritePlane(values, 0, m3 - m0, m7 - m4, m11 - m8, m15 - m12);
		WritePlane(values, 1, m3 + m0, m7 + m4, m11 + m8, m15 + m12);
		WritePlane(values, 2, m3 + m1, m7 + m5, m11 + m9, m15 + m13);
		WritePlane(values, 3, m3 - m1, m7 - m5, m11 - m9, m15 - m13);
		WritePlane(values, 4, m3 - m2, m7 - m6, m11 - m10, m15 - m14);
		WritePlane(values, 5, m3 + m2, m7 + m6, m11 + m10, m15 + m14);
		return FromArray(values);
	}

	/// <summary>
	/// Whether a sphere lies at least partly inside the frustum. A sphere entirely behind any one
	/// plane is outside, which is the test that makes frustum culling cheap.
	/// </summary>
	/// <param name="sphere">The bounding sphere to test.</param>
	/// <returns><see langword="true"/> when the sphere is not fully outside any plane.</returns>
	public bool IntersectsSphere(Sphere sphere)
	{
		foreach (var plane in _planes)
		{
			if (plane.DistanceToPoint(sphere.Center) < -sphere.Radius)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>Whether a point lies inside the frustum.</summary>
	/// <param name="point">The point to test.</param>
	/// <returns><see langword="true"/> when the point is on the inward side of every plane.</returns>
	public bool ContainsPoint(Vector3 point)
	{
		foreach (var plane in _planes)
		{
			if (plane.DistanceToPoint(point) < 0f)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Whether an axis-aligned box lies at least partly inside the frustum. The test picks, for each
	/// plane, the box corner furthest along that plane's normal - if even that corner is behind the
	/// plane, no part of the box is inside.
	/// </summary>
	/// <param name="box">The bounding box to test.</param>
	/// <returns><see langword="true"/> when the box is not fully outside any plane.</returns>
	public bool IntersectsBox(Box3 box)
	{
		foreach (var plane in _planes)
		{
			var farthestX = plane.Normal.X > 0f ? box.Max.X : box.Min.X;
			var farthestY = plane.Normal.Y > 0f ? box.Max.Y : box.Min.Y;
			var farthestZ = plane.Normal.Z > 0f ? box.Max.Z : box.Min.Z;
			if (plane.DistanceToPoint(new Vector3(farthestX, farthestY, farthestZ)) < 0f)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>Extracts all six planes into an array, four components each.</summary>
	/// <returns>A new array of 24 values: normal X, Y, Z and constant, per plane.</returns>
	public float[] ToArray()
	{
		var values = new float[PlaneCount * 4];
		for (var index = 0; index < PlaneCount; index++)
		{
			var plane = _planes[index];
			values[index * 4] = plane.Normal.X;
			values[(index * 4) + 1] = plane.Normal.Y;
			values[(index * 4) + 2] = plane.Normal.Z;
			values[(index * 4) + 3] = plane.Constant;
		}

		return values;
	}

	/// <summary>Copies 24 values from an array into the six planes, mutating this frustum in place.</summary>
	/// <param name="values">An array with at least 24 elements: normal X, Y, Z and constant, per plane.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Frustum FromArray(float[] values)
	{
		var hasChanged = false;
		var current = ToArray();
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

		foreach (var plane in _planes)
		{
			plane.OnChange = null;
		}

		for (var index = 0; index < PlaneCount; index++)
		{
			_planes[index].Set(
				new Vector3(values[index * 4], values[(index * 4) + 1], values[(index * 4) + 2]),
				values[(index * 4) + 3]);
		}

		foreach (var plane in _planes)
		{
			plane.OnChange = RaiseChanged;
		}

		OnChange?.Invoke();
		return this;
	}

	/// <summary>Creates a copy of this frustum with the same planes and no change callback.</summary>
	/// <returns>A new frustum bounded by the same planes.</returns>
	public Frustum Clone()
	{
		return new Frustum().FromArray(ToArray());
	}

	/// <summary>
	/// Writes one normalized plane into the flat component array. The normal is scaled to unit length
	/// here rather than by the caller, because a plane derived from a projection matrix comes out
	/// scaled by an arbitrary factor and every distance it reports would inherit that scale.
	/// </summary>
	private static void WritePlane(float[] values, int planeIndex, float normalX, float normalY, float normalZ, float constant)
	{
		var length = MathF.Sqrt((normalX * normalX) + (normalY * normalY) + (normalZ * normalZ));
		var inverse = length == 0f ? 0f : 1f / length;
		values[planeIndex * 4] = normalX * inverse;
		values[(planeIndex * 4) + 1] = normalY * inverse;
		values[(planeIndex * 4) + 2] = normalZ * inverse;
		values[(planeIndex * 4) + 3] = constant * inverse;
	}

	private void RaiseChanged()
	{
		OnChange?.Invoke();
	}
}
