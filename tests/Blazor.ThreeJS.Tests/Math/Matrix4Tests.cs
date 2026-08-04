using Kebechet.Blazor.ThreeJS.Math;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Math;

public class Matrix4Tests
{
	[Fact]
	public void Matrix4_Constructed_IsIdentity()
	{
		// Arrange & Act
		var matrix = new Matrix4();

		// Assert
		matrix.Elements.ShouldBe([1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f]);
	}

	[Fact]
	public void Matrix4_SetInRowMajorReadingOrder_StoresColumnMajor()
	{
		// Arrange
		var matrix = new Matrix4();

		// Act
		matrix.Set(
			1f, 2f, 3f, 4f,
			5f, 6f, 7f, 8f,
			9f, 10f, 11f, 12f,
			13f, 14f, 15f, 16f);

		// Assert
		// three.js stores column-major: element[0..3] is the FIRST COLUMN, i.e. 1,5,9,13.
		matrix.Elements.ShouldBe([1f, 5f, 9f, 13f, 2f, 6f, 10f, 14f, 3f, 7f, 11f, 15f, 4f, 8f, 12f, 16f]);
	}

	[Fact]
	public void Matrix4_ComposeWithTranslationOnly_PlacesPositionInLastColumn()
	{
		// Arrange
		var matrix = new Matrix4();
		var position = new Vector3(7f, 8f, 9f);
		var quaternion = new Quaternion();
		var scale = new Vector3(1f, 1f, 1f);

		// Act
		matrix.Compose(position, quaternion, scale);

		// Assert
		// Column-major: translation occupies elements 12, 13, 14.
		matrix.Elements[12].ShouldBe(7f);
		matrix.Elements[13].ShouldBe(8f);
		matrix.Elements[14].ShouldBe(9f);
		matrix.Elements[15].ShouldBe(1f);
	}

	[Fact]
	public void Matrix4_MultiplyByIdentity_LeavesMatrixUnchanged()
	{
		// Arrange
		var matrix = new Matrix4();
		matrix.Set(
			1f, 2f, 3f, 4f,
			5f, 6f, 7f, 8f,
			9f, 10f, 11f, 12f,
			13f, 14f, 15f, 16f);
		var expected = (float[]) matrix.Elements.Clone();

		// Act
		matrix.Multiply(new Matrix4());

		// Assert
		matrix.Elements.ShouldBe(expected);
	}
}
