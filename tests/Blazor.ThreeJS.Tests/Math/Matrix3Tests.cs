using Kebechet.Blazor.ThreeJS.Math;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Math;

public class Matrix3Tests
{
	[Fact]
	public void Matrix3_Constructed_IsIdentity()
	{
		// Arrange & Act
		var matrix = new Matrix3();

		// Assert
		matrix.Elements.ShouldBe([1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f]);
	}

	[Fact]
	public void Matrix3_SetInRowMajorReadingOrder_StoresColumnMajor()
	{
		// Arrange
		var matrix = new Matrix3();

		// Act
		matrix.Set(
			1f, 2f, 3f,
			4f, 5f, 6f,
			7f, 8f, 9f);

		// Assert
		// three.js stores column-major: element[0..2] is the FIRST COLUMN, i.e. 1,4,7.
		matrix.Elements.ShouldBe([1f, 4f, 7f, 2f, 5f, 8f, 3f, 6f, 9f]);
	}

	[Fact]
	public void Matrix3_FromArray_DoesNotTranspose()
	{
		// Arrange
		var matrix = new Matrix3();
		float[] columnMajor = [1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f];

		// Act
		matrix.FromArray(columnMajor);

		// Assert
		// The wire carries components in the same column-major order Elements stores them, so a
		// round trip through FromArray/ToArray must be the identity. Routing either through Set
		// would transpose every matrix that crossed the boundary.
		matrix.ToArray().ShouldBe(columnMajor);
	}

	[Fact]
	public void Matrix3_SetFromMatrix4_TakesTheUpperLeftBlock()
	{
		// Arrange
		var source = new Matrix4();
		source.Set(
			1f, 2f, 3f, 99f,
			4f, 5f, 6f, 99f,
			7f, 8f, 9f, 99f,
			99f, 99f, 99f, 99f);

		// Act
		var matrix = new Matrix3().SetFromMatrix4(source);

		// Assert
		matrix.Elements.ShouldBe([1f, 4f, 7f, 2f, 5f, 8f, 3f, 6f, 9f]);
	}

	[Fact]
	public void Matrix3_MultiplyByIdentity_LeavesTheMatrixUnchanged()
	{
		// Arrange
		var matrix = new Matrix3();
		matrix.Set(
			1f, 2f, 3f,
			4f, 5f, 6f,
			7f, 8f, 9f);

		var before = matrix.ToArray();

		// Act
		matrix.Multiply(new Matrix3());

		// Assert
		matrix.ToArray().ShouldBe(before);
	}

	[Fact]
	public void Matrix3_Transpose_ExchangesRowsAndColumns()
	{
		// Arrange
		var matrix = new Matrix3();
		matrix.Set(
			1f, 2f, 3f,
			4f, 5f, 6f,
			7f, 8f, 9f);

		// Act
		matrix.Transpose();

		// Assert
		matrix.ToArray().ShouldBe([1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f]);
	}

	[Fact]
	public void Matrix3_DeterminantOfIdentity_IsOne()
	{
		// Arrange & Act
		var determinant = new Matrix3().Determinant();

		// Assert
		determinant.ShouldBe(1f);
	}
}
