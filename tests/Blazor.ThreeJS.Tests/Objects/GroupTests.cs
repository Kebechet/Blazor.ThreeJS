using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

public class GroupTests
{
	[Fact]
	public void Group_AttachedToBatch_EmitsCreateOpWithNoConstructorArgs()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();

		// Act
		group.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var createOp = ops.Single(x => x.Kind == ThreeOpKind.Create && x.Handle == group.Handle);
		createOp.Type.ShouldBe("Group");
		createOp.Args.ShouldBeEmpty();
	}

	[Fact]
	public void Group_MeshAddedBeforeAttach_EmitsChildCreateThenAddEdge()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		group.Add(mesh);

		// Act
		group.AttachTo(batch);
		var opsList = batch.Drain().ToList();

		// Assert
		var meshCreateIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == mesh.Handle);
		var addEdgeIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Add && x.Handle == group.Handle && x.ChildHandle == mesh.Handle);

		meshCreateIndex.ShouldBeGreaterThanOrEqualTo(0);
		addEdgeIndex.ShouldBeGreaterThanOrEqualTo(0);
		meshCreateIndex.ShouldBeLessThan(addEdgeIndex);
	}
}
