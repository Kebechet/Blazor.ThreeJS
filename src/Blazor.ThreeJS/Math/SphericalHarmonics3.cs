namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// Third-order spherical harmonics: nine RGB coefficients that approximate the light arriving at a
/// point from every direction. This is what a <c>LightProbe</c> carries, and it is why a probe can
/// stand in for a whole environment at the cost of nine vectors.
/// </summary>
public sealed class SphericalHarmonics3
{
	/// <summary>How many coefficients a third-order expansion carries.</summary>
	public const int CoefficientCount = 9;

	private readonly Vector3[] _coefficients;

	/// <summary>
	/// The nine coefficients, each holding an RGB triple rather than a position. Mutating one in place
	/// notifies this instance's owner.
	/// </summary>
	public IReadOnlyList<Vector3> Coefficients
	{
		get { return _coefficients; }
	}

	/// <summary>Raised whenever any coefficient changes, so an owner can mark itself dirty.</summary>
	internal Action? OnChange { get; set; }

	/// <summary>Initializes all nine coefficients to zero, describing no light at all.</summary>
	public SphericalHarmonics3()
	{
		_coefficients = new Vector3[CoefficientCount];
		for (var index = 0; index < CoefficientCount; index++)
		{
			var coefficient = new Vector3();
			coefficient.OnChange = RaiseChanged;
			_coefficients[index] = coefficient;
		}
	}

	/// <summary>Copies all nine coefficients from another instance, mutating this one in place.</summary>
	/// <param name="other">The instance to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public SphericalHarmonics3 Copy(SphericalHarmonics3 other)
	{
		return FromArray(other.ToArray());
	}

	/// <summary>Resets all nine coefficients to zero.</summary>
	/// <returns>This instance, for method chaining.</returns>
	public SphericalHarmonics3 Zero()
	{
		return FromArray(new float[CoefficientCount * 3]);
	}

	/// <summary>
	/// Scales every coefficient by a factor, mutating this instance in place. Scaling the whole
	/// expansion scales the light it describes.
	/// </summary>
	/// <param name="scalar">The factor to multiply by.</param>
	/// <returns>This instance, for method chaining.</returns>
	public SphericalHarmonics3 Scale(float scalar)
	{
		var values = ToArray();
		for (var index = 0; index < values.Length; index++)
		{
			values[index] *= scalar;
		}

		return FromArray(values);
	}

	/// <summary>
	/// Adds another expansion's coefficients to this one's, mutating this instance in place. Light is
	/// additive, so summing two expansions describes both sources together.
	/// </summary>
	/// <param name="other">The expansion to add.</param>
	/// <returns>This instance, for method chaining.</returns>
	public SphericalHarmonics3 Add(SphericalHarmonics3 other)
	{
		var values = ToArray();
		var addend = other.ToArray();
		for (var index = 0; index < values.Length; index++)
		{
			values[index] += addend[index];
		}

		return FromArray(values);
	}

	/// <summary>Extracts all nine coefficients into an array, three components each.</summary>
	/// <returns>A new array of 27 values, in coefficient order.</returns>
	public float[] ToArray()
	{
		var values = new float[CoefficientCount * 3];
		for (var index = 0; index < CoefficientCount; index++)
		{
			var coefficient = _coefficients[index];
			values[index * 3] = coefficient.X;
			values[(index * 3) + 1] = coefficient.Y;
			values[(index * 3) + 2] = coefficient.Z;
		}

		return values;
	}

	/// <summary>Copies 27 values from an array into the nine coefficients, mutating this instance in place.</summary>
	/// <param name="values">An array with at least 27 elements, three per coefficient.</param>
	/// <returns>This instance, for method chaining.</returns>
	public SphericalHarmonics3 FromArray(float[] values)
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

		foreach (var coefficient in _coefficients)
		{
			coefficient.OnChange = null;
		}

		for (var index = 0; index < CoefficientCount; index++)
		{
			_coefficients[index].Set(values[index * 3], values[(index * 3) + 1], values[(index * 3) + 2]);
		}

		foreach (var coefficient in _coefficients)
		{
			coefficient.OnChange = RaiseChanged;
		}

		OnChange?.Invoke();
		return this;
	}

	/// <summary>Creates a copy with the same coefficients and no change callback.</summary>
	/// <returns>A new instance holding the same expansion.</returns>
	public SphericalHarmonics3 Clone()
	{
		return new SphericalHarmonics3().FromArray(ToArray());
	}

	private void RaiseChanged()
	{
		OnChange?.Invoke();
	}
}
