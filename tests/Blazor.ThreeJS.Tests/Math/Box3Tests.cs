using Kebechet.Blazor.ThreeJS.Math;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Math;

/// <summary>
/// Covers the composite math types through <see cref="Box3"/>, which is the representative case: it
/// owns child math values, it copies rather than aliases them, and its default state is the one that
/// forced the non-finite wire encoding.
/// </summary>
public class Box3Tests
{
	[Fact]
	public void Box3_Constructed_IsEmptyAtInfiniteBounds()
	{
		// Arrange & Act
		var box = new Box3();

		// Assert
		// Not a zero-sized box at the origin: three.js seeds an empty box inverted at ±infinity so
		// that expanding it by any point yields exactly that point.
		box.Min.X.ShouldBe(float.PositiveInfinity);
		box.Max.X.ShouldBe(float.NegativeInfinity);
		box.IsEmpty().ShouldBeTrue();
	}

	[Fact]
	public void Box3_ExpandedByPointFromEmpty_BoundsExactlyThatPoint()
	{
		// Arrange
		var box = new Box3();

		// Act
		box.ExpandByPoint(new Vector3(2f, 3f, 4f));

		// Assert
		box.Min.ToArray().ShouldBe([2f, 3f, 4f]);
		box.Max.ToArray().ShouldBe([2f, 3f, 4f]);
		box.IsEmpty().ShouldBeFalse();
	}

	[Fact]
	public void Box3_ConstructedFromCorners_CopiesRatherThanAliasesThem()
	{
		// Arrange
		var min = new Vector3(0f, 0f, 0f);
		var max = new Vector3(1f, 1f, 1f);
		var box = new Box3(min, max);

		// Act
		min.X = -100f;

		// Assert
		// A deliberate divergence from three.js, which retains the instances. The box hangs its own
		// change callback off each corner, so retaining a caller's instance would overwrite whatever
		// callback that instance already carried - silently unhooking the object it belonged to.
		box.Min.X.ShouldBe(0f);
	}

	[Fact]
	public void Box3_CornerMutatedInPlace_RaisesTheChangeCallback()
	{
		// Arrange
		var box = new Box3(new Vector3(), new Vector3(1f, 1f, 1f));
		var changeCount = 0;
		box.OnChange = () => changeCount++;

		// Act
		box.Min.X = -1f;

		// Assert
		changeCount.ShouldBe(1);
	}

	[Fact]
	public void Box3_SetToNewCorners_RaisesTheChangeCallbackExactlyOnce()
	{
		// Arrange
		var box = new Box3(new Vector3(), new Vector3(1f, 1f, 1f));
		var changeCount = 0;
		box.OnChange = () => changeCount++;

		// Act
		box.Set(new Vector3(-1f, -2f, -3f), new Vector3(4f, 5f, 6f));

		// Assert
		// Six components moved, but a batched write is one change, not six.
		changeCount.ShouldBe(1);
	}

	[Fact]
	public void Box3_SetToTheValuesItAlreadyHolds_RaisesNothing()
	{
		// Arrange
		var box = new Box3(new Vector3(), new Vector3(1f, 1f, 1f));
		var changeCount = 0;
		box.OnChange = () => changeCount++;

		// Act
		box.Set(new Vector3(), new Vector3(1f, 1f, 1f));

		// Assert
		changeCount.ShouldBe(0);
	}

	[Fact]
	public void Box3_ContainsPoint_IncludesTheBoundary()
	{
		// Arrange
		var box = new Box3(new Vector3(), new Vector3(1f, 1f, 1f));

		// Act & Assert
		box.ContainsPoint(new Vector3(0.5f, 0.5f, 0.5f)).ShouldBeTrue();
		box.ContainsPoint(new Vector3(1f, 1f, 1f)).ShouldBeTrue();
		box.ContainsPoint(new Vector3(1.1f, 0.5f, 0.5f)).ShouldBeFalse();
	}

	[Fact]
	public void Box3_GetCenterAndSize_DescribeTheBounds()
	{
		// Arrange
		var box = new Box3(new Vector3(-1f, -2f, -3f), new Vector3(1f, 2f, 3f));

		// Act
		var center = box.GetCenter();
		var size = box.GetSize();

		// Assert
		center.ToArray().ShouldBe([0f, 0f, 0f]);
		size.ToArray().ShouldBe([2f, 4f, 6f]);
	}

	[Fact]
	public void Box3_SetFromPoints_BoundsAllOfThem()
	{
		// Arrange
		var box = new Box3();

		// Act
		box.SetFromPoints([new Vector3(1f, 1f, 1f), new Vector3(-1f, 5f, 0f), new Vector3(3f, 0f, -2f)]);

		// Assert
		box.Min.ToArray().ShouldBe([-1f, 0f, -2f]);
		box.Max.ToArray().ShouldBe([3f, 5f, 1f]);
	}

	[Fact]
	public void Box3_CloneOfAnEmptyBox_IsAlsoEmpty()
	{
		// Arrange
		var box = new Box3();

		// Act
		var clone = box.Clone();

		// Assert
		// Clone routes through the constructor, so the infinite bounds have to survive a Vector3 copy.
		clone.IsEmpty().ShouldBeTrue();
		clone.Min.X.ShouldBe(float.PositiveInfinity);
	}
}
