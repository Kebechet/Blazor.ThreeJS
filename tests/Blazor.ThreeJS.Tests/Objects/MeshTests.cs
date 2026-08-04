using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

public class MeshTests
{
	[Fact]
	public void Mesh_MaterialPropertyWrittenAfterAttach_RecordsSetOp()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		var mesh = new Mesh(new BoxGeometry(), material);
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		material.Roughness = 0.25f;
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(1);
		ops.Single().Kind.ShouldBe(ThreeOpKind.Set);
		ops.Single().Member.ShouldBe("roughness");
	}

	[Fact]
	public void Mesh_SharedGeometryAcrossTwoMeshes_EmitsSingleCreateOpPerHandle()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BoxGeometry();
		var material = new MeshStandardMaterial();
		var mesh1 = new Mesh(geometry, material);
		var mesh2 = new Mesh(geometry, material);

		// Act
		mesh1.AttachTo(batch);
		mesh2.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var createOps = ops.Where(x => x.Kind == ThreeOpKind.Create).ToList();
		createOps.Count(x => x.Handle == geometry.Handle).ShouldBe(1);
		createOps.Count(x => x.Handle == material.Handle).ShouldBe(1);
	}

	[Fact]
	public void Mesh_AttachedToBatch_EmitsGeometryAndMaterialCreateBeforeMeshCreate()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BoxGeometry();
		var material = new MeshStandardMaterial();
		var mesh = new Mesh(geometry, material);

		// Act
		mesh.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var createHandlesInOrder = ops
			.Where(x => x.Kind == ThreeOpKind.Create)
			.Select(x => x.Handle)
			.ToList();

		createHandlesInOrder.IndexOf(geometry.Handle).ShouldBeLessThan(createHandlesInOrder.IndexOf(mesh.Handle));
		createHandlesInOrder.IndexOf(material.Handle).ShouldBeLessThan(createHandlesInOrder.IndexOf(mesh.Handle));
	}
}
