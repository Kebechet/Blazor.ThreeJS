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

	[Fact]
	public void ThreeBatch_SetCallSetOnSameHandle_DoesNotCoalesceAcrossTheCall()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Set(1, "position", "A");
		batch.Call(1, "lookAt", [1f]);
		batch.Set(1, "position", "B");
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(3);
		ops.First().Kind.ShouldBe(ThreeOpKind.Set);
		ops.First().Value.ShouldBe("A");
		ops.ElementAt(1).Kind.ShouldBe(ThreeOpKind.Call);
		ops.Last().Kind.ShouldBe(ThreeOpKind.Set);
		ops.Last().Value.ShouldBe("B");
	}

	[Fact]
	public void ThreeBatch_SetCallSetOnDifferentHandles_StillCoalesces()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Set(1, "position", "A");
		batch.Call(2, "lookAt", [1f]);
		batch.Set(1, "position", "B");
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(2);
		ops.Single(x => x.Kind == ThreeOpKind.Set).Value.ShouldBe("B");
	}

	[Fact]
	public void ThreeBatch_SetCreateSetOnSameTarget_AppendsInsteadOfRewritingTheEarlierSet()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Set(1, "material", "A");
		batch.Create(2, "MeshStandardMaterial", []);
		batch.Set(1, "material", "B");
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(3);
		ops.First().Kind.ShouldBe(ThreeOpKind.Set);
		ops.First().Value.ShouldBe("A");
		ops.ElementAt(1).Kind.ShouldBe(ThreeOpKind.Create);
		ops.Last().Kind.ShouldBe(ThreeOpKind.Set);
		ops.Last().Value.ShouldBe("B");
	}

	/// <summary>
	/// A <c>Get</c> observes the object's property state at the point it runs, so a later <c>Set</c>
	/// folded back into the op in front of it would change a value the read already answered with.
	/// </summary>
	[Fact]
	public void ThreeBatch_SetGetSetOnSameHandle_DoesNotCoalesceAcrossTheGet()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Set(1, "roughness", "A");
		batch.Get(1, "roughness");
		batch.Set(1, "roughness", "B");
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(3);
		ops.First().Kind.ShouldBe(ThreeOpKind.Set);
		ops.First().Value.ShouldBe("A");
		ops.ElementAt(1).Kind.ShouldBe(ThreeOpKind.Get);
		ops.Last().Kind.ShouldBe(ThreeOpKind.Set);
		ops.Last().Value.ShouldBe("B");
	}

	/// <summary>
	/// Without the barrier a write recorded after a release is folded silently backwards into the op in
	/// front of the <c>Dispose</c> and vanishes; with it, the write reaches the applier as its own op and
	/// fails loudly against a handle the browser has already retired. A loud failure is the whole point.
	/// </summary>
	[Fact]
	public void ThreeBatch_SetDisposeSetOnSameHandle_DoesNotCoalesceAcrossTheDispose()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Set(1, "visible", "A");
		batch.Dispose(1);
		batch.Set(1, "visible", "B");
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(3);
		ops.First().Kind.ShouldBe(ThreeOpKind.Set);
		ops.First().Value.ShouldBe("A");
		ops.ElementAt(1).Kind.ShouldBe(ThreeOpKind.Dispose);
		ops.Last().Kind.ShouldBe(ThreeOpKind.Set);
		ops.Last().Value.ShouldBe("B");
	}

	[Fact]
	public void ThreeBatch_SetGivenTheNotSuppliedSentinel_Throws()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		var exception = Record.Exception(() => batch.Set(1, "roughness", ThreeValue.Unspecified));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public void ThreeBatch_SetTwiceThenCall_CoalescesTheSetsBeforeTheCall()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Set(1, "position", "A");
		batch.Set(1, "position", "B");
		batch.Call(1, "lookAt", [1f]);
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(2);
		ops.First().Kind.ShouldBe(ThreeOpKind.Set);
		ops.First().Value.ShouldBe("B");
		ops.Last().Kind.ShouldBe(ThreeOpKind.Call);
	}
}
