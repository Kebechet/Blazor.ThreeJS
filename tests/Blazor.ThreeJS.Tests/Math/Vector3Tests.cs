using Kebechet.Blazor.ThreeJS.Math;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Math;

public class Vector3Tests
{
	[Fact]
	public void Vector3_SetCalledWithComponents_MutatesInPlaceAndReturnsSelf()
	{
		// Arrange
		var vector = new Vector3();

		// Act
		var returned = vector.Set(1f, 2f, 3f);

		// Assert
		vector.X.ShouldBe(1f);
		vector.Y.ShouldBe(2f);
		vector.Z.ShouldBe(3f);
		returned.ShouldBeSameAs(vector);
	}

	[Fact]
	public void Vector3_AddCalledWithVector_AddsComponentwise()
	{
		// Arrange
		var vector = new Vector3(1f, 2f, 3f);
		var addend = new Vector3(10f, 20f, 30f);

		// Act
		vector.Add(addend);

		// Assert
		vector.X.ShouldBe(11f);
		vector.Y.ShouldBe(22f);
		vector.Z.ShouldBe(33f);
	}

	[Fact]
	public void Vector3_LengthOnThreeFourFive_ReturnsPythagoreanResult()
	{
		// Arrange
		var vector = new Vector3(3f, 4f, 0f);

		// Act
		var length = vector.Length();

		// Assert
		length.ShouldBe(5f);
	}

	[Fact]
	public void Vector3_NormalizeOnNonUnitVector_ProducesUnitLength()
	{
		// Arrange
		var vector = new Vector3(0f, 3f, 4f);

		// Act
		vector.Normalize();

		// Assert
		vector.Length().ShouldBe(1f, 0.0001f);
	}

	[Fact]
	public void Vector3_CrossOfXAndYAxes_ReturnsZAxis()
	{
		// Arrange
		var vector = new Vector3(1f, 0f, 0f);
		var other = new Vector3(0f, 1f, 0f);

		// Act
		vector.Cross(other);

		// Assert
		vector.X.ShouldBe(0f, 0.0001f);
		vector.Y.ShouldBe(0f, 0.0001f);
		vector.Z.ShouldBe(1f, 0.0001f);
	}

	[Fact]
	public void Vector3_ComponentAssigned_InvokesOnChange()
	{
		// Arrange
		var vector = new Vector3();
		var changeCount = 0;
		vector.OnChange = () => changeCount++;

		// Act
		vector.X = 5f;

		// Assert
		changeCount.ShouldBe(1);
	}

	[Fact]
	public void Vector3_SetCalled_InvokesOnChangeExactlyOnce()
	{
		// Arrange
		var vector = new Vector3();
		var changeCount = 0;
		vector.OnChange = () => changeCount++;

		// Act
		vector.Set(1f, 2f, 3f);

		// Assert
		changeCount.ShouldBe(1);
	}

	[Fact]
	public void Vector3_ComponentAssignedItsExistingValue_DoesNotInvokeOnChange()
	{
		// Arrange
		var vector = new Vector3(1f, 2f, 3f);
		var changeCount = 0;
		vector.OnChange = () => changeCount++;

		// Act
		vector.X = 1f;

		// Assert
		changeCount.ShouldBe(0);
	}

	[Fact]
	public void Vector3_SetCalledWithItsExistingValues_DoesNotInvokeOnChange()
	{
		// Arrange
		var vector = new Vector3(1f, 2f, 3f);
		var changeCount = 0;
		vector.OnChange = () => changeCount++;

		// Act
		vector.Set(1f, 2f, 3f);

		// Assert
		changeCount.ShouldBe(0);
	}

	[Fact]
	public void Vector3_SetCalledWithOneChangedComponent_InvokesOnChange()
	{
		// Arrange
		var vector = new Vector3(1f, 2f, 3f);
		var changeCount = 0;
		vector.OnChange = () => changeCount++;

		// Act
		vector.Set(1f, 2f, 4f);

		// Assert
		changeCount.ShouldBe(1);
	}
}
