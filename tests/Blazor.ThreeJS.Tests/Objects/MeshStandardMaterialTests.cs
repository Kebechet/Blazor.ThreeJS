using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

public class MeshStandardMaterialTests
{
	[Fact]
	public void MeshStandardMaterial_AttachedWithUntouchedSide_ReplaysFrontSideAsANumber()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();

		// Act
		material.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var sideOp = ops.Single(x => x.Kind == ThreeOpKind.Set && x.Member == "side");
		sideOp.Value.ShouldBe((int) Side.FrontSide);
	}

	[Fact]
	public void MeshStandardMaterial_SideChangedAfterAttach_RecordsSetOpWithTheNewValue()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		material.AttachTo(batch);
		batch.Drain();

		// Act
		material.Side = Side.DoubleSide;
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(1);
		ops.Single().Member.ShouldBe("side");
		ops.Single().Value.ShouldBe((int) Side.DoubleSide);
	}

	[Fact]
	public void MeshStandardMaterial_SideRewrittenWithItsExistingValue_ProducesNoOps()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial { Side = Side.BackSide };
		material.AttachTo(batch);
		batch.Drain();

		// Act
		material.Side = Side.BackSide;
		var ops = batch.Drain();

		// Assert
		ops.ShouldBeEmpty();
	}
}
