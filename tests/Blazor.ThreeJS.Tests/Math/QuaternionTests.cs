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

	// The expected values below for all six EulerOrder arms were harvested from real three.js
	// r185 (`Quaternion.setFromEuler`) with input Euler(0.3, 0.5, 0.7), using a throwaway Node
	// script (not committed) as an independent oracle. Distinct non-zero, non-equal angles are
	// used deliberately: zero or equal angles make several orders coincide, which would let a
	// wrong arm pass unnoticed.

	[Fact]
	public void Quaternion_SetFromEulerXyzOrder_MatchesThreeJsReferenceValues()
	{
		// Arrange
		var quaternion = new Quaternion();
		var euler = new Euler(0.3f, 0.5f, 0.7f, EulerOrder.XYZ);

		// Act
		quaternion.SetFromEuler(euler);

		// Assert
		quaternion.X.ShouldBe(0.2198958f, 0.0001f);
		quaternion.Y.ShouldBe(0.1801459f, 0.0001f);
		quaternion.Z.ShouldBe(0.3632374f, 0.0001f);
		quaternion.W.ShouldBe(0.8872722f, 0.0001f);
	}

	[Fact]
	public void Quaternion_SetFromEulerYxzOrder_MatchesThreeJsReferenceValues()
	{
		// Arrange
		var quaternion = new Quaternion();
		var euler = new Euler(0.3f, 0.5f, 0.7f, EulerOrder.YXZ);

		// Act
		quaternion.SetFromEuler(euler);

		// Assert
		quaternion.X.ShouldBe(0.2198958f, 0.0001f);
		quaternion.Y.ShouldBe(0.1801459f, 0.0001f);
		quaternion.Z.ShouldBe(0.2937772f, 0.0001f);
		quaternion.W.ShouldBe(0.9126271f, 0.0001f);
	}

	[Fact]
	public void Quaternion_SetFromEulerZxyOrder_MatchesThreeJsReferenceValues()
	{
		// Arrange
		var quaternion = new Quaternion();
		var euler = new Euler(0.3f, 0.5f, 0.7f, EulerOrder.ZXY);

		// Act
		quaternion.SetFromEuler(euler);

		// Assert
		quaternion.X.ShouldBe(0.0521324f, 0.0001f);
		quaternion.Y.ShouldBe(0.2794439f, 0.0001f);
		quaternion.Z.ShouldBe(0.3632374f, 0.0001f);
		quaternion.W.ShouldBe(0.8872722f, 0.0001f);
	}

	[Fact]
	public void Quaternion_SetFromEulerZyxOrder_MatchesThreeJsReferenceValues()
	{
		// Arrange
		var quaternion = new Quaternion();
		var euler = new Euler(0.3f, 0.5f, 0.7f, EulerOrder.ZYX);

		// Act
		quaternion.SetFromEuler(euler);

		// Assert
		quaternion.X.ShouldBe(0.0521324f, 0.0001f);
		quaternion.Y.ShouldBe(0.2794439f, 0.0001f);
		quaternion.Z.ShouldBe(0.2937772f, 0.0001f);
		quaternion.W.ShouldBe(0.9126271f, 0.0001f);
	}

	[Fact]
	public void Quaternion_SetFromEulerYzxOrder_MatchesThreeJsReferenceValues()
	{
		// Arrange
		var quaternion = new Quaternion();
		var euler = new Euler(0.3f, 0.5f, 0.7f, EulerOrder.YZX);

		// Act
		quaternion.SetFromEuler(euler);

		// Assert
		quaternion.X.ShouldBe(0.2198958f, 0.0001f);
		quaternion.Y.ShouldBe(0.2794439f, 0.0001f);
		quaternion.Z.ShouldBe(0.2937772f, 0.0001f);
		quaternion.W.ShouldBe(0.8872722f, 0.0001f);
	}

	[Fact]
	public void Quaternion_SetFromEulerXzyOrder_MatchesThreeJsReferenceValues()
	{
		// Arrange
		var quaternion = new Quaternion();
		var euler = new Euler(0.3f, 0.5f, 0.7f, EulerOrder.XZY);

		// Act
		quaternion.SetFromEuler(euler);

		// Assert
		quaternion.X.ShouldBe(0.0521324f, 0.0001f);
		quaternion.Y.ShouldBe(0.1801459f, 0.0001f);
		quaternion.Z.ShouldBe(0.3632374f, 0.0001f);
		quaternion.W.ShouldBe(0.9126271f, 0.0001f);
	}
}
