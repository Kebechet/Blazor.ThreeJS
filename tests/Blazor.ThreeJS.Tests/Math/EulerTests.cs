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
}
