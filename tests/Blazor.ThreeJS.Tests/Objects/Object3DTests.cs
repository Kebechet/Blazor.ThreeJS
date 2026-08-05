using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

public class Object3DTests
{
	[Fact]
	public void Object3D_PropertiesNobodyWrote_AreNotReplayedOnAttach()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();

		// Act
		group.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var setMembers = ops
			.Where(x => x.Kind == ThreeOpKind.Set)
			.Select(x => x.Member)
			.ToList();
		setMembers.ShouldBe(["position", "rotation", "scale", "visible"], ignoreOrder: true);
	}

	[Fact]
	public void Object3D_PropertiesWrittenBeforeAttach_AreReplayedWithTheirValues()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group
		{
			Name = "rig",
			CastShadow = true,
			ReceiveShadow = true,
			FrustumCulled = false,
			RenderOrder = 3f,
			MatrixAutoUpdate = false,
			MatrixWorldAutoUpdate = false,
			MatrixWorldNeedsUpdate = true
		};

		// Act
		group.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var setOps = ops
			.Where(x => x.Kind == ThreeOpKind.Set)
			.ToDictionary(x => x.Member!, x => x.Value);

		setOps["name"].ShouldBe("rig");
		setOps["castShadow"].ShouldBe(true);
		setOps["receiveShadow"].ShouldBe(true);
		setOps["frustumCulled"].ShouldBe(false);
		setOps["renderOrder"].ShouldBe(3f);
		setOps["matrixAutoUpdate"].ShouldBe(false);
		setOps["matrixWorldAutoUpdate"].ShouldBe(false);
		setOps["matrixWorldNeedsUpdate"].ShouldBe(true);
	}

	[Fact]
	public void Object3D_PropertiesRewrittenWithTheirExistingValues_ProducesNoOps()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		group.AttachTo(batch);
		batch.Drain();

		// Act
		group.Name = string.Empty;
		group.CastShadow = false;
		group.ReceiveShadow = false;
		group.FrustumCulled = true;
		group.RenderOrder = 0f;
		group.MatrixAutoUpdate = true;
		group.MatrixWorldAutoUpdate = true;
		group.MatrixWorldNeedsUpdate = false;
		var ops = batch.Drain();

		// Assert
		ops.ShouldBeEmpty();
	}

	[Fact]
	public void Object3D_PropertiesChangedAfterAttach_ProducesOneOpEach()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		group.AttachTo(batch);
		batch.Drain();

		// Act
		group.CastShadow = true;
		group.ReceiveShadow = true;
		group.RenderOrder = 2f;
		var ops = batch.Drain();

		// Assert
		var setMembers = ops
			.Where(x => x.Kind == ThreeOpKind.Set)
			.Select(x => x.Member)
			.ToList();
		setMembers.ShouldBe(["castShadow", "receiveShadow", "renderOrder"], ignoreOrder: true);
	}

	[Fact]
	public void Object3D_UpMutatedBeforeAttach_ReplaysItAsAVector()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		group.Up.Set(0f, 0f, 1f);

		// Act
		group.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var upOp = ops.Single(x => x.Kind == ThreeOpKind.Set && x.Member == "up");
		var upValue = upOp.Value.ShouldBeOfType<ThreeValue.TaggedValue>();
		upValue.Values.ShouldBe([0f, 0f, 1f]);
	}

	[Fact]
	public void Object3D_UpNeverMutated_HoldsTheThreeJsDefault()
	{
		// Arrange
		var group = new Group();

		// Act
		var up = group.Up;

		// Assert
		up.ToArray().ShouldBe([0f, 1f, 0f]);
	}

	/// <summary>
	/// three.js derives <c>rotation</c> and <c>quaternion</c> from each other, so whichever is applied
	/// last wins. The transform is replayed unconditionally and the quaternion only when written, so
	/// the caller's own write has to be the one that lands second.
	/// </summary>
	[Fact]
	public void Object3D_QuaternionWrittenBeforeAttach_IsReplayedAfterRotation()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		group.Quaternion.Set(0.1f, 0.2f, 0.3f, 0.9f);

		// Act
		group.AttachTo(batch);
		var opsList = batch.Drain().ToList();

		// Assert
		var rotationIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "rotation");
		var quaternionIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "quaternion");

		rotationIndex.ShouldBeGreaterThanOrEqualTo(0);
		rotationIndex.ShouldBeLessThan(quaternionIndex);

		var quaternionValue = opsList[quaternionIndex].Value.ShouldBeOfType<ThreeValue.TaggedValue>();
		quaternionValue.Values.ShouldBe([0.1f, 0.2f, 0.3f, 0.9f]);
	}

	[Fact]
	public void Object3D_PivotMutatedAfterAttach_RecordsAWriteOfPivot()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		group.AttachTo(batch);
		batch.Drain();

		// Act
		group.Pivot.Set(1f, 0f, 0f);
		var ops = batch.Drain();

		// Assert
		var pivotOp = ops.Single(x => x.Kind == ThreeOpKind.Set && x.Member == "pivot");
		var pivotValue = pivotOp.Value.ShouldBeOfType<ThreeValue.TaggedValue>();
		pivotValue.Values.ShouldBe([1f, 0f, 0f]);
	}

	[Fact]
	public void Object3D_LayersAssignedBeforeAttach_CreatesThemBeforeTheWriteThatReferencesThem()
	{
		// Arrange
		var batch = new ThreeBatch();
		var layers = new Layers { Mask = 5 };
		var group = new Group { Layers = layers };

		// Act
		group.AttachTo(batch);
		var opsList = batch.Drain().ToList();

		// Assert
		var createIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == layers.Handle);
		var setIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "layers");

		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(setIndex);

		var reference = opsList[setIndex].Value.ShouldBeOfType<ThreeValue.HandleReference>();
		reference.Handle.ShouldBe(layers.Handle);
	}

	[Fact]
	public void Object3D_CustomDepthMaterialAssignedAfterAttach_AttachesItAndRecordsTheWrite()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		group.AttachTo(batch);
		batch.Drain();
		var material = new MeshDepthMaterial();

		// Act
		group.CustomDepthMaterial = material;
		var opsList = batch.Drain().ToList();

		// Assert
		var createIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == material.Handle);
		var setIndex = opsList.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "customDepthMaterial");

		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(setIndex);

		var reference = opsList[setIndex].Value.ShouldBeOfType<ThreeValue.HandleReference>();
		reference.Handle.ShouldBe(material.Handle);
	}

	/// <summary>
	/// Pins the documented sharp edge on <c>LookAt</c> rather than describing it: the command writes
	/// three.js's <c>rotation</c> and <c>quaternion</c> and the mirror never learns, so the typed
	/// property's own "value unchanged, record nothing" guard leaves three.js holding the <c>lookAt</c>
	/// orientation.
	/// </summary>
	[Fact]
	public void Object3D_LookAtThenRewritingTheRotationTheMirrorStillHolds_RecordsNothing()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		group.Rotation.Set(0.4f, 0f, 0f, EulerOrder.XYZ);
		group.AttachTo(batch);
		batch.Drain();

		// Act
		group.LookAt(1f, 2f, 3f);
		group.Rotation.Set(0.4f, 0f, 0f, EulerOrder.XYZ);
		var ops = batch.Drain().ToList();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Call && x.Member == "lookAt").ShouldBe(1);
		var transformWrites = ops
			.Where(x => x.Kind == ThreeOpKind.Set)
			.Select(x => x.Member)
			.ToList();
		transformWrites.ShouldNotContain("rotation");
		transformWrites.ShouldNotContain("quaternion");
		group.Rotation.ToArray().ShouldBe([0.4f, 0f, 0f]);
	}

	/// <summary>
	/// The other half of the same edge: a rebuild replays the mirror, and the mirror never saw the
	/// <c>lookAt</c>, so the replayed <c>rotation</c> is the orientation from before the call.
	/// </summary>
	[Fact]
	public void Object3D_LookAtCalledBeforeAttach_ReplaysThePreCallRotation()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		group.Rotation.Set(0.4f, 0f, 0f, EulerOrder.XYZ);

		// Act
		group.LookAt(1f, 2f, 3f);
		group.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var rotationOp = ops.Single(x => x.Kind == ThreeOpKind.Set && x.Member == "rotation");
		var rotationValue = rotationOp.Value.ShouldBeOfType<ThreeValue.TaggedValue>();
		rotationValue.Values.ShouldBe([0.4f, 0f, 0f]);
	}

	[Fact]
	public void Object3D_NewPropertiesOnASubclass_AreInheritedRatherThanRedeclared()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial())
		{
			CastShadow = true,
			ReceiveShadow = true
		};

		// Act
		mesh.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var meshSetOps = ops
			.Where(x => x.Kind == ThreeOpKind.Set && x.Handle == mesh.Handle)
			.ToDictionary(x => x.Member!, x => x.Value);

		meshSetOps["castShadow"].ShouldBe(true);
		meshSetOps["receiveShadow"].ShouldBe(true);
	}
}
