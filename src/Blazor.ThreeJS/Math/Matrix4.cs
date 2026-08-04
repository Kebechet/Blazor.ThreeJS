namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// 4x4 transform matrix. <see cref="Elements"/> is stored <b>column-major</b> to match three.js
/// and WebGL, while <see cref="Set"/> accepts arguments in visual row-major reading order.
/// Confusing the two produces silently wrong transforms rather than an error, so the storage
/// order is pinned by <c>Matrix4Tests.Matrix4_SetInRowMajorReadingOrder_StoresColumnMajor</c>.
/// </summary>
public sealed class Matrix4
{
	/// <summary>
	/// The 16 matrix components, stored <b>column-major</b>: elements 0-3 are the first column,
	/// 4-7 the second, and so on. This matches three.js and is what WebGL expects directly - do not
	/// assume row-major indexing when reading this array.
	/// </summary>
	public float[] Elements { get; } = [1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f];

	/// <summary>
	/// Resets this matrix to the identity matrix, mutating it in place.
	/// </summary>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix4 Identity()
	{
		return Set(
			1f, 0f, 0f, 0f,
			0f, 1f, 0f, 0f,
			0f, 0f, 1f, 0f,
			0f, 0f, 0f, 1f);
	}

	/// <summary>
	/// Sets all 16 components, mutating this instance in place. The 16 arguments are given in
	/// visual <b>row-major reading order</b> (the order you would read a 4x4 matrix on paper), but
	/// are stored into <see cref="Elements"/> transposed, i.e. <b>column-major</b>, exactly as
	/// three.js does. <c>n</c>+row+column naming mirrors the upstream three.js source.
	/// </summary>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix4 Set(
		float n11, float n12, float n13, float n14,
		float n21, float n22, float n23, float n24,
		float n31, float n32, float n33, float n34,
		float n41, float n42, float n43, float n44)
	{
		var elements = Elements;
		elements[0] = n11; elements[4] = n12; elements[8] = n13; elements[12] = n14;
		elements[1] = n21; elements[5] = n22; elements[9] = n23; elements[13] = n24;
		elements[2] = n31; elements[6] = n32; elements[10] = n33; elements[14] = n34;
		elements[3] = n41; elements[7] = n42; elements[11] = n43; elements[15] = n44;
		return this;
	}

	/// <summary>
	/// Multiplies this matrix by another matrix (<c>this = this * other</c>), mutating this instance in place.
	/// </summary>
	/// <param name="other">The matrix to multiply by.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix4 Multiply(Matrix4 other)
	{
		var a = Elements;
		var b = other.Elements;
		var result = new float[16];

		for (var column = 0; column < 4; column++)
		{
			for (var row = 0; row < 4; row++)
			{
				var sum = 0f;
				for (var k = 0; k < 4; k++)
				{
					sum += a[(k * 4) + row] * b[(column * 4) + k];
				}

				result[(column * 4) + row] = sum;
			}
		}

		Array.Copy(result, Elements, 16);
		return this;
	}

	/// <summary>
	/// Composes this matrix from a position, rotation quaternion and scale, mutating this instance
	/// in place. This is the standard way to build a transform matrix for an <c>Object3D</c>: the
	/// resulting matrix applies scale first, then rotation, then translation.
	/// </summary>
	/// <param name="position">The translation to place in the last column.</param>
	/// <param name="quaternion">The rotation to apply.</param>
	/// <param name="scale">The per-axis scale to apply.</param>
	/// <returns>This instance, for method chaining.</returns>
	public Matrix4 Compose(Vector3 position, Quaternion quaternion, Vector3 scale)
	{
		var x = quaternion.X;
		var y = quaternion.Y;
		var z = quaternion.Z;
		var w = quaternion.W;

		var x2 = x + x;
		var y2 = y + y;
		var z2 = z + z;
		var xx = x * x2;
		var xy = x * y2;
		var xz = x * z2;
		var yy = y * y2;
		var yz = y * z2;
		var zz = z * z2;
		var wx = w * x2;
		var wy = w * y2;
		var wz = w * z2;

		var elements = Elements;
		elements[0] = (1f - (yy + zz)) * scale.X;
		elements[1] = (xy + wz) * scale.X;
		elements[2] = (xz - wy) * scale.X;
		elements[3] = 0f;

		elements[4] = (xy - wz) * scale.Y;
		elements[5] = (1f - (xx + zz)) * scale.Y;
		elements[6] = (yz + wx) * scale.Y;
		elements[7] = 0f;

		elements[8] = (xz + wy) * scale.Z;
		elements[9] = (yz - wx) * scale.Z;
		elements[10] = (1f - (xx + yy)) * scale.Z;
		elements[11] = 0f;

		elements[12] = position.X;
		elements[13] = position.Y;
		elements[14] = position.Z;
		elements[15] = 1f;

		return this;
	}
}
