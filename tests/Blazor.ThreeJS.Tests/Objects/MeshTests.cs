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

	[Fact]
	public void Mesh_MaterialSetToAFreshMaterial_EmitsItsCreateBeforeTheMaterialSetOp()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.AttachTo(batch);
		batch.Drain();
		var freshMaterial = new MeshStandardMaterial();

		// Act
		mesh.Material = freshMaterial;
		var opsList = batch.Drain().ToList();

		// Assert
		var createIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == freshMaterial.Handle);
		var setIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Handle == mesh.Handle && x.Member == "material");

		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		setIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(setIndex);
	}

	[Fact]
	public void Mesh_MaterialReassignedTwiceBeforeFlush_KeepsEachCreateBeforeTheSetReferencingIt()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		mesh.Material = new MeshStandardMaterial();
		mesh.Material = new MeshStandardMaterial();
		var opsList = batch.Drain().ToList();

		// Assert
		var materialSetOps = opsList
			.Where(x => x.Kind == ThreeOpKind.Set && x.Handle == mesh.Handle && x.Member == "material")
			.ToList();

		materialSetOps.Count.ShouldBe(2);
		foreach (var materialSetOp in materialSetOps)
		{
			var referencedHandle = materialSetOp.Value.ShouldBeOfType<ThreeValue.HandleReference>().Handle;
			var createIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == referencedHandle);

			createIndex.ShouldBeGreaterThanOrEqualTo(0);
			createIndex.ShouldBeLessThan(opsList.IndexOf(materialSetOp));
		}
	}

	[Fact]
	public void Mesh_MaterialSetToAnAlreadyAttachedMaterial_DoesNotReemitItsCreateOp()
	{
		// Arrange
		var batch = new ThreeBatch();
		var sharedMaterial = new MeshStandardMaterial();
		var otherMesh = new Mesh(new BoxGeometry(), sharedMaterial);
		otherMesh.AttachTo(batch);
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		mesh.Material = sharedMaterial;
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Create && x.Handle == sharedMaterial.Handle).ShouldBe(0);
		ops.ShouldContain(x => x.Kind == ThreeOpKind.Set && x.Handle == mesh.Handle && x.Member == "material");
	}

	[Fact]
	public void Mesh_MaterialSetToTheSameInstanceItAlreadyHolds_ProducesNoOps()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		var mesh = new Mesh(new BoxGeometry(), material);
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		mesh.Material = material;
		var ops = batch.Drain();

		// Assert
		ops.ShouldBeEmpty();
	}
}
