using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

public class PointsTests
{
	[Fact]
	public void Points_AttachedToBatch_EmitsGeometryAndMaterialCreateBeforePointsCreate()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BoxGeometry();
		var material = new PointsMaterial();
		var points = new Points(geometry, material);

		// Act
		points.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var createHandlesInOrder = ops
			.Where(x => x.Kind == ThreeOpKind.Create)
			.Select(x => x.Handle)
			.ToList();

		createHandlesInOrder.IndexOf(geometry.Handle).ShouldBeLessThan(createHandlesInOrder.IndexOf(points.Handle));
		createHandlesInOrder.IndexOf(material.Handle).ShouldBeLessThan(createHandlesInOrder.IndexOf(points.Handle));
	}

	[Fact]
	public void Points_SharedMaterialAcrossTwoPointsObjects_EmitsSingleCreateOpPerHandle()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new PointsMaterial();
		var points1 = new Points(new BoxGeometry(), material);
		var points2 = new Points(new BoxGeometry(), material);

		// Act
		points1.AttachTo(batch);
		points2.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Create && x.Handle == material.Handle).ShouldBe(1);
	}

	[Fact]
	public void Points_SizePropertyWrittenAfterAttach_RecordsSetOp()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new PointsMaterial();
		var points = new Points(new BoxGeometry(), material);
		points.AttachTo(batch);
		batch.Drain();

		// Act
		material.Size = 2.5f;
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(1);
		ops.Single().Kind.ShouldBe(ThreeOpKind.Set);
		ops.Single().Member.ShouldBe("size");
	}

	[Fact]
	public void Points_AttachedWithUntouchedMaterial_ReplaysColorAndSize()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new PointsMaterial();
		var points = new Points(new BoxGeometry(), material);

		// Act
		points.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var materialSetMembers = ops
			.Where(x => x.Kind == ThreeOpKind.Set && x.Handle == material.Handle)
			.Select(x => x.Member)
			.ToList();
		materialSetMembers.ShouldBe(["color", "size"], ignoreOrder: true);
	}
}
