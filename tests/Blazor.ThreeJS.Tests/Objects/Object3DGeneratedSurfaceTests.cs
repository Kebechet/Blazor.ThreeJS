using System.Text.Json;
using Blazor.ThreeJS.Tests.Core;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// Pins the generated half of <c>Object3D</c>: the commands and queries that reach every node in the
/// scene graph because the base declares them. They are emitted rather than hand-written, so what is
/// asserted here is that the emitted shape behaves like the hand-written one it sits beside — the same
/// call op, the same pre-attach replay, the same handle-minting read.
/// </summary>
public class Object3DGeneratedSurfaceTests
{
	private const int AnsweredHandle = -7;

	[Fact]
	public void Object3D_RotateX_RecordsACallOpCarryingTheAngle()
	{
		// Arrange
		var batch = new ThreeBatch();
		var group = new Group();
		group.AttachTo(batch);

		// Act
		group.RotateX(0.5f);
		var ops = batch.Drain();

		// Assert
		var call = ops.Single(x => x.Kind == ThreeOpKind.Call);
		call.Handle.ShouldBe(group.Handle);
		call.Member.ShouldBe("rotateX");
		call.Args.ShouldBe([0.5f]);
	}

	[Fact]
	public void Object3D_RotateXInvokedBeforeAttach_ReplaysAfterTheCreateOpAndTheStateReplay()
	{
		// Arrange: the order is the contract - a held command has to observe the object three.js would
		// have had when it was invoked, which means the replayed properties rather than the constructor
		// defaults.
		var batch = new ThreeBatch();
		var group = new Group
		{
			Name = "rig"
		};

		// Act
		group.RotateX(0.5f);
		group.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		var kinds = ops
			.Where(x => x.Handle == group.Handle)
			.Select(x => (x.Kind, x.Member))
			.ToList();

		kinds.ShouldBe([
			(ThreeOpKind.Create, null),
			(ThreeOpKind.Set, "name"),
			(ThreeOpKind.Call, "rotateX")
		]);
	}

	[Fact]
	public void Object3D_RotateX_LeavesTheMirroredRotationReportingItsPreCallValue()
	{
		// Arrange: the authority caveat the story spells out. Nothing reads back, so the mirror is stale
		// afterwards and rewriting the value it still holds records nothing at all.
		var batch = new ThreeBatch();
		var group = new Group();
		group.AttachTo(batch);

		// Act
		group.RotateX(0.5f);
		group.Rotation.X = 0f;
		var ops = batch.Drain();

		// Assert
		group.Rotation.X.ShouldBe(0f);
		ops.ShouldNotContain(x => x.Kind == ThreeOpKind.Set && x.Member == "rotation");
	}

	[Fact]
	public async Task Object3D_GetObjectByNameRead_SendsAReadOpThatAsksForAHandle()
	{
		// Arrange
		var module = AnswerEveryReadWith(AnsweredHandle, "Mesh");
		var context = new ThreeContext(module, contextId: 1);
		var group = new Group();
		context.Attach(group);

		// Act
		var found = await group.GetObjectByNameAsync("hub");

		// Assert
		var op = SentOps(module).Single(x => x.Kind == ThreeOpKind.Read);
		op.Handle.ShouldBe(group.Handle);
		op.Member.ShouldBe("getObjectByName");
		op.Args.ShouldBe(["hub"]);
		op.MintsHandle.ShouldBeTrue();
		found.ShouldNotBeNull();
		found.Handle.ShouldBe(AnsweredHandle);
	}

	[Fact]
	public async Task Object3D_GetWorldPositionRead_DecodesTheTaggedVectorTheBrowserAnswersWith()
	{
		// Arrange
		var module = new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Results = ops
					.Where(x => x.Kind == ThreeOpKind.Read)
					.Select(x => new ThreeReadResult
					{
						RequestId = x.RequestId,
						Value = JsonSerializer.SerializeToElement(ThreeValue.Encode(new Vector3(4f, 5f, 6f)))
					})
					.ToList()
			}
		};

		var context = new ThreeContext(module, contextId: 1);
		var group = new Group();
		context.Attach(group);

		// Act
		var worldPosition = await group.GetWorldPositionAsync(new Vector3());

		// Assert
		var op = SentOps(module).Single(x => x.Kind == ThreeOpKind.Read);
		op.Member.ShouldBe("getWorldPosition");
		// A math value comes back tagged rather than by handle: asking for one would decode a reference
		// out of a `$t` tuple and fault.
		op.MintsHandle.ShouldBeFalse();
		worldPosition.ToArray().ShouldBe([4f, 5f, 6f]);
	}

	/// <summary>The ops that reached the applier in the single batch these tests send.</summary>
	/// <param name="module">The fake module.</param>
	/// <returns>The batch, in the order it was recorded.</returns>
	private static IReadOnlyList<ThreeOp> SentOps(RecordingJsObjectReference module)
	{
		return module.Invocations
			.Single(x => x.Identifier == "applyBatch")
			.Arguments
			.OfType<IReadOnlyList<ThreeOp>>()
			.Single();
	}

	/// <summary>
	/// A module that answers every read with the reference shape the applier sends for an op marked
	/// <c>n:true</c>. Written as literal JSON rather than serialized from the C# type it decodes into, so
	/// a rename of either wire key fails the test instead of travelling through it.
	/// </summary>
	/// <param name="handle">Handle the applier registered the object under.</param>
	/// <param name="threeTypeName">three.js's own <c>constructor.name</c> for it.</param>
	/// <returns>The fake module.</returns>
	private static RecordingJsObjectReference AnswerEveryReadWith(int handle, string threeTypeName)
	{
		var reference = JsonDocument.Parse($"{{\"$ref\":{handle},\"t\":\"{threeTypeName}\"}}").RootElement;
		return new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Results = ops
					.Where(x => x.Kind == ThreeOpKind.Read)
					.Select(x => new ThreeReadResult { RequestId = x.RequestId, Value = reference })
					.ToList()
			}
		};
	}
}
