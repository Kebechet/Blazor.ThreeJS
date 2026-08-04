using Kebechet.Blazor.ThreeJS.Core;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

public class ThreeBatchTests
{
	[Fact]
	public void ThreeBatch_Constructed_HasNoPendingOps()
	{
		// Arrange & Act
		var batch = new ThreeBatch();

		// Assert
		batch.HasPendingOps.ShouldBeFalse();
	}

	[Fact]
	public void ThreeBatch_SetSameMemberTwice_CoalescesToLatestValue()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Set(1, "visible", true);
		batch.Set(1, "visible", false);
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(1);
		ops.Single().Value.ShouldBe(false);
	}

	[Fact]
	public void ThreeBatch_SetDifferentMembers_KeepsBothOps()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Set(1, "visible", true);
		batch.Set(1, "castShadow", true);
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(2);
	}

	[Fact]
	public void ThreeBatch_CreateThenSet_EmitsCreateFirst()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Create(1, "Mesh", []);
		batch.Set(1, "visible", false);
		var ops = batch.Drain();

		// Assert
		ops.First().Kind.ShouldBe(ThreeOpKind.Create);
		ops.Last().Kind.ShouldBe(ThreeOpKind.Set);
	}

	[Fact]
	public void ThreeBatch_Drained_ClearsPendingOps()
	{
		// Arrange
		var batch = new ThreeBatch();
		batch.Set(1, "visible", true);

		// Act
		batch.Drain();

		// Assert
		batch.HasPendingOps.ShouldBeFalse();
	}

	[Fact]
	public void ThreeBatch_CallRecordedTwice_DoesNotCoalesce()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Call(1, "translateX", [1f]);
		batch.Call(1, "translateX", [1f]);
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(2);
	}
}
