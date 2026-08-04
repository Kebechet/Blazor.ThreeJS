using Kebechet.Blazor.ThreeJS.Math;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Math;

public class ColorTests
{
	[Fact]
	public void Color_SetHexPureRed_ProducesUnitRedChannel()
	{
		// Arrange
		var color = new Color();

		// Act
		color.SetHex(0xff0000);

		// Assert
		color.R.ShouldBe(1f, 0.0001f);
		color.G.ShouldBe(0f, 0.0001f);
		color.B.ShouldBe(0f, 0.0001f);
	}

	[Fact]
	public void Color_GetHexAfterSetHex_RoundTripsExactly()
	{
		// Arrange
		var color = new Color();
		color.SetHex(0x3366cc);

		// Act
		var hex = color.GetHex();

		// Assert
		hex.ShouldBe(0x3366cc);
	}

	[Fact]
	public void Color_RedPresetRequestedTwice_ReturnsDistinctInstances()
	{
		// Arrange & Act
		var first = Color.Red;
		var second = Color.Red;

		// Assert
		first.ShouldNotBeSameAs(second);
	}

	[Fact]
	public void Color_ChannelAssigned_InvokesOnChange()
	{
		// Arrange
		var color = new Color();
		var changeCount = 0;
		color.OnChange = () => changeCount++;

		// Act
		color.G = 0.5f;

		// Assert
		changeCount.ShouldBe(1);
	}

	[Fact]
	public void Color_ChannelAssignedItsExistingValue_DoesNotInvokeOnChange()
	{
		// Arrange
		var color = new Color(0.25f, 0.5f, 0.75f);
		var changeCount = 0;
		color.OnChange = () => changeCount++;

		// Act
		color.G = 0.5f;

		// Assert
		changeCount.ShouldBe(0);
	}

	[Fact]
	public void Color_SetHexCalledWithItsExistingValue_DoesNotInvokeOnChange()
	{
		// Arrange
		var color = Color.FromHex(0x3366cc);
		var changeCount = 0;
		color.OnChange = () => changeCount++;

		// Act
		color.SetHex(0x3366cc);

		// Assert
		changeCount.ShouldBe(0);
	}
}
