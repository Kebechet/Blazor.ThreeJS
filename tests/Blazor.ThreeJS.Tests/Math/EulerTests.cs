using Kebechet.Blazor.ThreeJS.Math;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Math;

public class EulerTests
{
	[Fact]
	public void Euler_Constructed_DefaultsToXyzOrder()
	{
		// Arrange & Act
		var euler = new Euler();

		// Assert
		euler.Order.ShouldBe(EulerOrder.XYZ);
	}

	[Fact]
	public void Euler_ComponentAssigned_InvokesOnChange()
	{
		// Arrange
		var euler = new Euler();
		var changeCount = 0;
		euler.OnChange = () => changeCount++;

		// Act
		euler.Y = 1f;

		// Assert
		changeCount.ShouldBe(1);
	}

	[Fact]
	public void Euler_ComponentAssignedItsExistingValue_DoesNotInvokeOnChange()
	{
		// Arrange
		var euler = new Euler(0f, 1f, 0f, EulerOrder.YXZ);
		var changeCount = 0;
		euler.OnChange = () => changeCount++;

		// Act
		euler.Y = 1f;

		// Assert
		changeCount.ShouldBe(0);
	}

	[Fact]
	public void Euler_OrderAssignedItsExistingValue_DoesNotInvokeOnChange()
	{
		// Arrange
		var euler = new Euler(0f, 1f, 0f, EulerOrder.YXZ);
		var changeCount = 0;
		euler.OnChange = () => changeCount++;

		// Act
		euler.Order = EulerOrder.YXZ;

		// Assert
		changeCount.ShouldBe(0);
	}

	[Fact]
	public void Euler_SetCalledWithTheSameAnglesButADifferentOrder_InvokesOnChange()
	{
		// Arrange
		var euler = new Euler(0f, 1f, 0f, EulerOrder.YXZ);
		var changeCount = 0;
		euler.OnChange = () => changeCount++;

		// Act
		euler.Set(0f, 1f, 0f, EulerOrder.ZXY);

		// Assert
		changeCount.ShouldBe(1);
	}

	[Fact]
	public void Euler_SetCalledWithItsExistingValues_DoesNotInvokeOnChange()
	{
		// Arrange
		var euler = new Euler(0f, 1f, 0f, EulerOrder.YXZ);
		var changeCount = 0;
		euler.OnChange = () => changeCount++;

		// Act
		euler.Set(0f, 1f, 0f, EulerOrder.YXZ);

		// Assert
		changeCount.ShouldBe(0);
	}
}
