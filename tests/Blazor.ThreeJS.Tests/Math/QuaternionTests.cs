using Kebechet.Blazor.ThreeJS.Math;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Math;

public class QuaternionTests
{
	[Fact]
	public void Quaternion_Constructed_IsIdentityRotation()
	{
		// Arrange & Act
		var quaternion = new Quaternion();

		// Assert
		quaternion.X.ShouldBe(0f);
		quaternion.Y.ShouldBe(0f);
		quaternion.Z.ShouldBe(0f);
		quaternion.W.ShouldBe(1f);
	}

	[Fact]
	public void Quaternion_SetFromEulerHalfPiAboutY_MatchesThreeJsReferenceValues()
	{
		// Arrange
		var quaternion = new Quaternion();
		var euler = new Euler(0f, MathF.PI / 2f, 0f, EulerOrder.XYZ);

		// Act
		quaternion.SetFromEuler(euler);

		// Assert
		quaternion.X.ShouldBe(0f, 0.0001f);
		quaternion.Y.ShouldBe(0.7071068f, 0.0001f);
		quaternion.Z.ShouldBe(0f, 0.0001f);
		quaternion.W.ShouldBe(0.7071068f, 0.0001f);
	}

	[Fact]
	public void Quaternion_SetFromEulerZeroRotation_ProducesIdentity()
	{
		// Arrange
		var quaternion = new Quaternion(9f, 9f, 9f, 9f);
		var euler = new Euler(0f, 0f, 0f, EulerOrder.XYZ);

		// Act
		quaternion.SetFromEuler(euler);

		// Assert
		quaternion.W.ShouldBe(1f, 0.0001f);
	}
}
