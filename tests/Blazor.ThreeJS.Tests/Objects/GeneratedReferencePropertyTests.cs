using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// Pins the ordering invariant for generated properties whose value is itself a mirrored object and
/// which three.js does not also take as a constructor argument. The write travels as a handle
/// reference, so the referenced object's create op has to reach the batch first — through the setter
/// when the owner is already attached, and through the replay when it is not.
/// </summary>
public class GeneratedReferencePropertyTests
{
	[Fact]
	public void Scene_OverrideMaterialWrittenBeforeAttach_CreatesTheMaterialBeforeTheWriteThatReferencesIt()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		var scene = new Scene { OverrideMaterial = material };

		// Act
		scene.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == material.Handle);
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "overrideMaterial");

		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		setIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(setIndex);

		var reference = ops.ElementAt(setIndex).Value.ShouldBeOfType<ThreeValue.HandleReference>();
		reference.Handle.ShouldBe(material.Handle);
	}

	[Fact]
	public void Scene_OverrideMaterialWrittenAfterAttach_CreatesTheMaterialBeforeTheWriteThatReferencesIt()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		scene.AttachTo(batch);
		batch.Drain();
		var material = new MeshStandardMaterial();

		// Act
		scene.OverrideMaterial = material;
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == material.Handle);
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "overrideMaterial");

		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		setIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(setIndex);

		var reference = ops.ElementAt(setIndex).Value.ShouldBeOfType<ThreeValue.HandleReference>();
		reference.Handle.ShouldBe(material.Handle);
	}

	[Fact]
	public void Scene_OverrideMaterialAlreadyAttached_IsNotCreatedASecondTime()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		var mesh = new Mesh(new BoxGeometry(), material);
		mesh.AttachTo(batch);
		var scene = new Scene { OverrideMaterial = material };

		// Act
		scene.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var materialCreateCount = ops.Count(x => x.Kind == ThreeOpKind.Create && x.Handle == material.Handle);
		materialCreateCount.ShouldBe(1);

		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == material.Handle);
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "overrideMaterial");
		createIndex.ShouldBeLessThan(setIndex);
	}

	[Fact]
	public void Raycaster_CameraWrittenBeforeAttach_CreatesTheCameraBeforeTheWriteThatReferencesIt()
	{
		// Arrange
		var batch = new ThreeBatch();
		var camera = new PerspectiveCamera();
		var raycaster = new Raycaster { Camera = camera };

		// Act
		raycaster.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == camera.Handle);
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "camera");

		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		setIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(setIndex);

		var reference = ops.ElementAt(setIndex).Value.ShouldBeOfType<ThreeValue.HandleReference>();
		reference.Handle.ShouldBe(camera.Handle);
	}

	[Fact]
	public void SpotLight_TargetWrittenBeforeAttach_CreatesTheTargetBeforeTheWriteThatReferencesIt()
	{
		// Arrange
		var batch = new ThreeBatch();
		var target = new Group();
		var spotLight = new SpotLight { Target = target };

		// Act
		spotLight.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == target.Handle);
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "target");

		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		setIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(setIndex);

		var reference = ops.ElementAt(setIndex).Value.ShouldBeOfType<ThreeValue.HandleReference>();
		reference.Handle.ShouldBe(target.Handle);
	}
}
