namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// 3x3 matrix, used by three.js for normal matrices and for texture transforms.
/// <see cref="Elements"/> is stored <b>column-major</b> to match three.js and WebGL, while
/// <see cref="Set"/> accepts arguments in visual row-major reading order. Confusing the two produces
/// silently wrong transforms rather than an error, so the storage order is pinned by
/// <c>Matrix3Tests.Matrix3_SetInRowMajorReadingOrder_StoresColumnMajor</c>.
/// </summary>
public sealed class Matrix3
{
	/// <summary>
	/// The 9 matrix components, stored <b>column-major</b>: elements 0-2 are the first column,
	/// 3-5 the second, 6-8 the third. This matches three.js and is what WebGL expects directly - do
	/// not assume row-major indexing when reading this array.
	/// </summary>
	public float[] Elements { get; } = [1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f];

	/// <summary>
	/// Resets this matrix to the identity matrix, mutating it in place.
	/// </summary>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix3 Identity()
	{
		return Set(
			1f, 0f, 0f,
			0f, 1f, 0f,
			0f, 0f, 1f);
	}

	/// <summary>
	/// Sets all 9 components, mutating this instance in place. The 9 arguments are given in visual
	/// <b>row-major reading order</b> (the order you would read a 3x3 matrix on paper), but are stored
	/// into <see cref="Elements"/> transposed, i.e. <b>column-major</b>, exactly as three.js does.
	/// <c>n</c>+row+column naming mirrors the upstream three.js source.
	/// </summary>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix3 Set(
		float n11, float n12, float n13,
		float n21, float n22, float n23,
		float n31, float n32, float n33)
	{
		var elements = Elements;
		elements[0] = n11; elements[3] = n12; elements[6] = n13;
		elements[1] = n21; elements[4] = n22; elements[7] = n23;
		elements[2] = n31; elements[5] = n32; elements[8] = n33;
		return this;
	}

	/// <summary>
	/// Copies the components from another matrix into this matrix, mutating it in place.
	/// </summary>
	/// <param name="other">The matrix to copy from.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix3 Copy(Matrix3 other)
	{
		Array.Copy(other.Elements, Elements, 9);
		return this;
	}

	/// <summary>
	/// Multiplies this matrix by another matrix (<c>this = this * other</c>), mutating this instance in place.
	/// </summary>
	/// <param name="other">The matrix to multiply by.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix3 Multiply(Matrix3 other)
	{
		var a = Elements;
		var b = other.Elements;
		var result = new float[9];

		for (var column = 0; column < 3; column++)
		{
			for (var row = 0; row < 3; row++)
			{
				var sum = 0f;
				for (var k = 0; k < 3; k++)
				{
					sum += a[(k * 3) + row] * b[(column * 3) + k];
				}

				result[(column * 3) + row] = sum;
			}
		}

		Array.Copy(result, Elements, 9);
		return this;
	}

	/// <summary>
	/// Transposes this matrix in place, exchanging its rows and columns.
	/// </summary>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix3 Transpose()
	{
		var elements = Elements;
		(elements[1], elements[3]) = (elements[3], elements[1]);
		(elements[2], elements[6]) = (elements[6], elements[2]);
		(elements[5], elements[7]) = (elements[7], elements[5]);
		return this;
	}

	/// <summary>
	/// Computes the determinant of this matrix.
	/// </summary>
	/// <returns>The determinant.</returns>
	public float Determinant()
	{
		var e = Elements;
		var a = e[0];
		var b = e[1];
		var c = e[2];
		var d = e[3];
		var f = e[4];
		var g = e[5];
		var h = e[6];
		var i = e[7];
		var j = e[8];
		return (a * f * j) - (a * g * i) - (b * d * j) + (b * g * h) + (c * d * i) - (c * f * h);
	}

	/// <summary>
	/// Sets this matrix to the upper-left 3x3 block of a 4x4 matrix, mutating it in place. This is how
	/// three.js derives a rotation-and-scale matrix from a full transform.
	/// </summary>
	/// <param name="matrix">The 4x4 matrix to take the upper-left block of.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix3 SetFromMatrix4(Matrix4 matrix)
	{
		var m = matrix.Elements;
		return Set(
			m[0], m[4], m[8],
			m[1], m[5], m[9],
			m[2], m[6], m[10]);
	}

	/// <summary>
	/// Extracts the 9 components of this matrix into a new array, in the same <b>column-major</b>
	/// order they are stored in. The array is a copy, so a later mutation of this matrix cannot
	/// change a payload already handed to the wire encoder.
	/// </summary>
	/// <returns>A new array containing the 9 column-major components.</returns>
	public float[] ToArray()
	{
		var values = new float[9];
		Array.Copy(Elements, values, 9);
		return values;
	}

	/// <summary>
	/// Copies 9 <b>column-major</b> components from an array into this matrix, mutating it in place.
	/// </summary>
	/// <param name="values">An array with at least nine elements, in column-major order.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix3 FromArray(float[] values)
	{
		Array.Copy(values, Elements, 9);
		return this;
	}

	/// <summary>
	/// Creates a copy of this matrix with the same components.
	/// </summary>
	/// <returns>A new matrix with the same components.</returns>
	public Matrix3 Clone()
	{
		return new Matrix3().FromArray(Elements);
	}
}
