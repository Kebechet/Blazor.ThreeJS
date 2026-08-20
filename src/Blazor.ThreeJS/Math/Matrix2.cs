namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// 2x2 matrix. <see cref="Elements"/> is stored <b>column-major</b> to match three.js and WebGL, while
/// <see cref="Set"/> accepts arguments in visual row-major reading order. Confusing the two produces
/// silently wrong transforms rather than an error, so the storage order is pinned by
/// <c>Matrix2Tests.Matrix2_SetInRowMajorReadingOrder_StoresColumnMajor</c>.
/// </summary>
public sealed class Matrix2
{
	/// <summary>
	/// The 4 matrix components, stored <b>column-major</b>: elements 0-1 are the first column and 2-3
	/// the second. This matches three.js and is what WebGL expects directly - do not assume row-major
	/// indexing when reading this array.
	/// </summary>
	public float[] Elements { get; } = [1f, 0f, 0f, 1f];

	/// <summary>Resets this matrix to the identity matrix, mutating it in place.</summary>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix2 Identity()
	{
		return Set(
			1f, 0f,
			0f, 1f);
	}

	/// <summary>
	/// Sets all 4 components, mutating this instance in place. The arguments are given in visual
	/// <b>row-major reading order</b> (the order you would read a 2x2 matrix on paper), but are stored
	/// into <see cref="Elements"/> transposed, i.e. <b>column-major</b>, exactly as three.js does.
	/// <c>n</c>+row+column naming mirrors the upstream three.js source.
	/// </summary>
	/// <param name="n11">Row 1, column 1.</param>
	/// <param name="n12">Row 1, column 2.</param>
	/// <param name="n21">Row 2, column 1.</param>
	/// <param name="n22">Row 2, column 2.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix2 Set(float n11, float n12, float n21, float n22)
	{
		var elements = Elements;
		elements[0] = n11; elements[2] = n12;
		elements[1] = n21; elements[3] = n22;
		return this;
	}

	/// <summary>Copies the components from another matrix into this one, mutating it in place.</summary>
	/// <param name="other">The matrix to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix2 Copy(Matrix2 other)
	{
		Array.Copy(other.Elements, Elements, 4);
		return this;
	}

	/// <summary>
	/// Multiplies this matrix by another (<c>this = this * other</c>), mutating this instance in place.
	/// </summary>
	/// <param name="other">The matrix to multiply by.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix2 Multiply(Matrix2 other)
	{
		var a = Elements;
		var b = other.Elements;
		var result = new float[4];

		for (var column = 0; column < 2; column++)
		{
			for (var row = 0; row < 2; row++)
			{
				var sum = 0f;
				for (var k = 0; k < 2; k++)
				{
					sum += a[(k * 2) + row] * b[(column * 2) + k];
				}

				result[(column * 2) + row] = sum;
			}
		}

		Array.Copy(result, Elements, 4);
		return this;
	}

	/// <summary>Transposes this matrix in place, exchanging its rows and columns.</summary>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix2 Transpose()
	{
		var elements = Elements;
		(elements[1], elements[2]) = (elements[2], elements[1]);
		return this;
	}

	/// <summary>Computes the determinant of this matrix.</summary>
	/// <returns>The determinant.</returns>
	public float Determinant()
	{
		var e = Elements;
		return (e[0] * e[3]) - (e[2] * e[1]);
	}

	/// <summary>The components, column-major, as a new array.</summary>
	/// <returns>A copy of <see cref="Elements"/>.</returns>
	public float[] ToArray()
	{
		var values = new float[4];
		Array.Copy(Elements, values, 4);
		return values;
	}

	/// <summary>Sets every component from a column-major array, mutating this instance in place.</summary>
	/// <param name="values">Four components, column-major.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix2 FromArray(float[] values)
	{
		Array.Copy(values, Elements, 4);
		return this;
	}

	/// <summary>Creates a copy of this matrix with the same components.</summary>
	/// <returns>A new matrix with the same components.</returns>
	public Matrix2 Clone()
	{
		return new Matrix2().FromArray(Elements);
	}
}
