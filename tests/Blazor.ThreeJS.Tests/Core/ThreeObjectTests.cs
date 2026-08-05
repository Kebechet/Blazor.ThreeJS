using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

public class ThreeObjectTests
{
	[Fact]
	public void ThreeObject_AttachToCalledTwice_EmitsSingleCreateOp()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BoxGeometry();

		// Act
		geometry.AttachTo(batch);
		geometry.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Create).ShouldBe(1);
	}

	[Fact]
	public void ThreeObject_AttachedToASecondBatch_Throws()
	{
		// Arrange
		var geometry = new BoxGeometry();
		geometry.AttachTo(new ThreeBatch());

		// Act
		var exception = Record.Exception(() => geometry.AttachTo(new ThreeBatch()));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public void Object3D_AttachedToASecondBatch_Throws()
	{
		// Arrange
		var scene = new Scene();
		scene.AttachTo(new ThreeBatch());

		// Act
		var exception = Record.Exception(() => scene.AttachTo(new ThreeBatch()));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public void ThreeObject_AttachedTwiceToTheSameBatch_DoesNotThrow()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		scene.AttachTo(batch);

		// Act
		var exception = Record.Exception(() => scene.AttachTo(batch));

		// Assert
		exception.ShouldBeNull();
	}

	[Fact]
	public void ThreeObject_CommandInvokedBeforeAttach_RecordsItAfterTheCreateAndThePropertyReplay()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.Position.Set(1f, 2f, 3f);
		mesh.LookAt(0f, 0f, 0f);

		// Act
		mesh.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == mesh.Handle);
		var positionSetIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Handle == mesh.Handle && x.Member == "position");
		var lookAtIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Call && x.Handle == mesh.Handle && x.Member == "lookAt");

		lookAtIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(positionSetIndex);
		positionSetIndex.ShouldBeLessThan(lookAtIndex);
		ops.ElementAt(lookAtIndex).Args.ShouldBe([0f, 0f, 0f]);
	}

	[Fact]
	public void ThreeObject_CommandInvokedAfterAttach_RecordsASingleCallOp()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		mesh.LookAt(1f, 2f, 3f);
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(1);
		ops.Single().Kind.ShouldBe(ThreeOpKind.Call);
		ops.Single().Member.ShouldBe("lookAt");
		ops.Single().Args.ShouldBe([1f, 2f, 3f]);
	}

	[Fact]
	public void ThreeObject_SeveralCommandsInvokedBeforeAttach_ReplaysThemInInvocationOrder()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.LookAt(0f, 0f, 0f);
		mesh.UpdateMorphTargets();
		mesh.LookAt(4f, 5f, 6f);

		// Act
		mesh.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var replayedMembers = ops
			.Where(x => x.Kind == ThreeOpKind.Call && x.Handle == mesh.Handle)
			.Select(x => x.Member)
			.ToList();

		replayedMembers.ShouldBe(["lookAt", "updateMorphTargets", "lookAt"]);
	}

	[Fact]
	public void ThreeObject_CommandWithAMirroredArgumentInvokedBeforeAttach_EmitsTheArgumentCreateFirst()
	{
		// Arrange
		var batch = new ThreeBatch();
		var levelOfDetail = new LOD();
		var level = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		levelOfDetail.AddLevel(level, 10f);

		// Act
		levelOfDetail.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var levelCreateIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == level.Handle);
		var addLevelIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Call && x.Handle == levelOfDetail.Handle && x.Member == "addLevel");

		addLevelIndex.ShouldBeGreaterThanOrEqualTo(0);
		levelCreateIndex.ShouldBeGreaterThanOrEqualTo(0);
		levelCreateIndex.ShouldBeLessThan(addLevelIndex);
	}

	[Fact]
	public void ThreeObject_AttachedTwiceAfterAPreAttachCommand_ReplaysThatCommandOnce()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.LookAt(0f, 0f, 0f);

		// Act
		mesh.AttachTo(batch);
		mesh.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Call && x.Member == "lookAt").ShouldBe(1);
	}

	[Fact]
	public void ThreeObject_CommandInvokedBeforeAttachOnANonSceneGraphObject_RecordsItAfterThePropertyReplay()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		material.Roughness = 0.25f;
		material.Copy(new MeshStandardMaterial());

		// Act
		material.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var roughnessSetIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Handle == material.Handle && x.Member == "roughness");
		var copyIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Call && x.Handle == material.Handle && x.Member == "copy");

		copyIndex.ShouldBeGreaterThanOrEqualTo(0);
		roughnessSetIndex.ShouldBeGreaterThanOrEqualTo(0);
		roughnessSetIndex.ShouldBeLessThan(copyIndex);
	}
}
