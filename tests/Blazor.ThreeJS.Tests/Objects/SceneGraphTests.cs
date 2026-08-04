using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

public class SceneGraphTests
{
	[Fact]
	public void SceneGraph_MeshAddedToAttachedScene_EmitsCreateAndAddOps()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		scene.AttachTo(batch);
		batch.Drain();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());

		// Act
		scene.Add(mesh);
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Create).ShouldBe(3);
		ops.Count(x => x.Kind == ThreeOpKind.Add).ShouldBe(1);
	}

	[Fact]
	public void SceneGraph_PositionWrittenBeforeAttach_IsReplayedOnAttach()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		scene.Position.Set(1f, 2f, 3f);

		// Act
		scene.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		ops.ShouldContain(x => x.Kind == ThreeOpKind.Set && x.Member == "position");
	}

	[Fact]
	public void SceneGraph_PositionWrittenRepeatedly_CoalescesToOneSetOp()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		scene.AttachTo(batch);
		batch.Drain();

		// Act
		scene.Position.X = 1f;
		scene.Position.X = 2f;
		scene.Position.X = 3f;
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Member == "position").ShouldBe(1);
	}

	[Fact]
	public void SceneGraph_NothingChanged_ProducesNoOps()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		scene.AttachTo(batch);
		batch.Drain();

		// Act
		var ops = batch.Drain();

		// Assert
		ops.ShouldBeEmpty();
	}

	[Fact]
	public void SceneGraph_TransformRewrittenWithItsExistingValues_ProducesNoOps()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.Position.Set(1f, 2f, 3f);
		mesh.Rotation.Set(0.4f, 0.8f, 0f, EulerOrder.YXZ);
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		mesh.Position.Set(1f, 2f, 3f);
		mesh.Position.X = 1f;
		mesh.Rotation.Set(0.4f, 0.8f, 0f, EulerOrder.YXZ);
		mesh.Scale.Set(1f, 1f, 1f);
		var ops = batch.Drain();

		// Assert
		ops.ShouldBeEmpty();
	}

	[Fact]
	public void SceneGraph_NonMathPropertiesRewrittenWithTheirExistingValues_ProducesNoOps()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial { Roughness = 0.4f, Metalness = 0.2f };
		var mesh = new Mesh(new BoxGeometry(), material);
		mesh.IsVisible = false;
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		mesh.IsVisible = false;
		material.Roughness = 0.4f;
		material.Metalness = 0.2f;
		var ops = batch.Drain();

		// Assert
		ops.ShouldBeEmpty();
	}

	[Fact]
	public void SceneGraph_NonMathPropertiesActuallyChanged_ProducesOneOpEach()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial { Roughness = 0.4f, Metalness = 0.2f };
		var mesh = new Mesh(new BoxGeometry(), material);
		mesh.IsVisible = false;
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		mesh.IsVisible = true;
		material.Roughness = 0.5f;
		material.Metalness = 0.3f;
		var ops = batch.Drain();

		// Assert
		var setMembers = ops
			.Where(x => x.Kind == ThreeOpKind.Set)
			.Select(x => x.Member)
			.ToList();
		setMembers.ShouldBe(["visible", "roughness", "metalness"], ignoreOrder: true);
	}

	[Fact]
	public void SceneGraph_ObjectWithAnUntouchedTransformAttached_StillReplaysItsFullState()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();

		// Act
		scene.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var setMembers = ops
			.Where(x => x.Kind == ThreeOpKind.Set)
			.Select(x => x.Member)
			.ToList();
		setMembers.ShouldBe(["position", "rotation", "scale", "visible"], ignoreOrder: true);
	}

	[Fact]
	public void SceneGraph_TransformWrittenBeforeAttach_ReplaysRotationScaleAndVisibility()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		scene.Rotation.Set(0.1f, 0.2f, 0.3f, EulerOrder.YXZ);
		scene.Scale.Set(2f, 3f, 4f);
		scene.IsVisible = false;

		// Act
		scene.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var rotationOp = ops.Single(x => x.Kind == ThreeOpKind.Set && x.Member == "rotation");
		var scaleOp = ops.Single(x => x.Kind == ThreeOpKind.Set && x.Member == "scale");
		var visibleOp = ops.Single(x => x.Kind == ThreeOpKind.Set && x.Member == "visible");

		var rotationValue = rotationOp.Value.ShouldBeOfType<ThreeValue.TaggedValue>();
		rotationValue.Values.ShouldBe([0.1f, 0.2f, 0.3f]);
		rotationValue.Order.ShouldBe((byte) EulerOrder.YXZ);

		var scaleValue = scaleOp.Value.ShouldBeOfType<ThreeValue.TaggedValue>();
		scaleValue.Values.ShouldBe([2f, 3f, 4f]);

		visibleOp.Value.ShouldBe(false);
	}

	[Fact]
	public void SceneGraph_MaterialPropertiesWrittenBeforeAttach_AreReplayedOnAttach()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BoxGeometry();
		var material = new MeshStandardMaterial();
		material.Color.Set(0.2f, 0.4f, 0.6f);
		material.Roughness = 0.25f;
		material.Metalness = 0.75f;
		var mesh = new Mesh(geometry, material);

		// Act
		mesh.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var materialSetOps = ops
			.Where(x => x.Kind == ThreeOpKind.Set && x.Handle == material.Handle)
			.ToList();

		var colorOp = materialSetOps.Single(x => x.Member == "color");
		var colorValue = colorOp.Value.ShouldBeOfType<ThreeValue.TaggedValue>();
		colorValue.Values.ShouldBe([0.2f, 0.4f, 0.6f]);

		materialSetOps.Single(x => x.Member == "roughness").Value.ShouldBe(0.25f);
		materialSetOps.Single(x => x.Member == "metalness").Value.ShouldBe(0.75f);
	}

	[Fact]
	public void SceneGraph_ChildAddedBeforeParentAttach_EmitsChildCreateThenAddEdge()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		scene.Add(mesh);

		// Act
		scene.AttachTo(batch);
		var opsList = batch.Drain().ToList();

		// Assert
		var meshCreateIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == mesh.Handle);
		var addEdgeIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Add && x.Handle == scene.Handle && x.ChildHandle == mesh.Handle);

		meshCreateIndex.ShouldBeGreaterThanOrEqualTo(0);
		addEdgeIndex.ShouldBeGreaterThanOrEqualTo(0);
		meshCreateIndex.ShouldBeLessThan(addEdgeIndex);
	}

	[Fact]
	public void SceneGraph_ChildAddedAfterParentAttach_EmitsChildCreateThenAddEdge()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		scene.AttachTo(batch);
		batch.Drain();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());

		// Act
		scene.Add(mesh);
		var opsList = batch.Drain().ToList();

		// Assert
		var meshCreateIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == mesh.Handle);
		var addEdgeIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Add && x.Handle == scene.Handle && x.ChildHandle == mesh.Handle);

		meshCreateIndex.ShouldBeGreaterThanOrEqualTo(0);
		addEdgeIndex.ShouldBeGreaterThanOrEqualTo(0);
		meshCreateIndex.ShouldBeLessThan(addEdgeIndex);
	}
}
