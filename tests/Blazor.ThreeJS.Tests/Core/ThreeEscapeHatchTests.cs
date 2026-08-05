using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

/// <summary>
/// Covers the untyped surface — <c>Set</c>, <c>Call</c>, <c>CallAsync</c>, <c>GetAsync</c> and the two
/// primitive types — and specifically the invariants it could bypass but must not: an object-valued
/// write attaches what it references first, an op recorded before an attach is replayed rather than
/// dropped, and the coalescing barriers stay exactly where a typed member puts them.
/// <para>
/// That the untyped path reaches real three.js at all is pinned end to end against the vendored bundle
/// by <c>tests/wire-format.test.mjs</c>, which constructs, mutates and reads back a class this package
/// does not generate.
/// </para>
/// </summary>
public class ThreeEscapeHatchTests
{
	[Fact]
	public void ThreeObject_RawSetOfAMirroredValue_EmitsItsCreateBeforeTheReference()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.AttachTo(batch);
		batch.Drain();
		var replacementMaterial = new MeshStandardMaterial();

		// Act
		mesh.Set("customDepthMaterial", replacementMaterial);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == replacementMaterial.Handle);
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "customDepthMaterial");
		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		setIndex.ShouldBeGreaterThan(createIndex);
	}

	[Fact]
	public void ThreeObject_RawSetOfAMirroredValueBeforeAttach_StillEmitsItsCreateBeforeTheReference()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		var replacementMaterial = new MeshStandardMaterial();
		mesh.Set("customDepthMaterial", replacementMaterial);

		// Act
		mesh.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == replacementMaterial.Handle);
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "customDepthMaterial");
		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		setIndex.ShouldBeGreaterThan(createIndex);
	}

	[Fact]
	public void ThreeObject_RawSetOfAMirroredValueAlreadyInTheGraph_DoesNotCreateItTwice()
	{
		// Arrange
		var batch = new ThreeBatch();
		var sharedMaterial = new MeshStandardMaterial();
		var mesh = new Mesh(new BoxGeometry(), sharedMaterial);
		mesh.AttachTo(batch);

		// Act
		mesh.Set("customDepthMaterial", sharedMaterial);
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Create && x.Handle == sharedMaterial.Handle).ShouldBe(1);
	}

	[Fact]
	public void ThreeObject_RawSetBeforeAttach_ReplaysItAfterTheCreate()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		material.Set("alphaHash", true);

		// Act
		material.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == material.Handle);
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "alphaHash");
		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		setIndex.ShouldBeGreaterThan(createIndex);
		ops.ElementAt(setIndex).Value.ShouldBe(true);
	}

	[Fact]
	public void ThreeObject_RawSetBeforeAttach_IsNotDroppedTheWayARecordSetWouldBe()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();

		// Act
		material.Set("alphaHash", true);
		material.AttachTo(batch);

		// Assert
		batch.Drain().ShouldContain(x => x.Kind == ThreeOpKind.Set && x.Member == "alphaHash");
	}

	[Fact]
	public void ThreeObject_RawCallBeforeAttach_ReplaysItAfterTheStateReplay()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.Position.Set(1f, 2f, 3f);
		mesh.Call("updateMatrixWorld");

		// Act
		mesh.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == mesh.Handle);
		var positionIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Handle == mesh.Handle && x.Member == "position");
		var callIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Call && x.Member == "updateMatrixWorld");
		callIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(positionIndex);
		positionIndex.ShouldBeLessThan(callIndex);
	}

	[Fact]
	public void ThreeObject_RawSetsAndCallsBeforeAttach_ReplayInInvocationOrder()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		material.Set("alphaHash", true);
		material.Call("dispose");
		material.Set("alphaHash", false);

		// Act
		material.AttachTo(batch);
		var replayed = batch.Drain()
			.Where(x => x.Handle == material.Handle && x.Member is "alphaHash" or "dispose")
			.Select(x => $"{x.Kind}:{x.Member}")
			.ToList();

		// Assert
		replayed.ShouldBe(["Set:alphaHash", "Call:dispose", "Set:alphaHash"]);
	}

	[Fact]
	public void ThreeObject_RawSetsOfTheSameMember_CoalesceIntoOneOp()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		material.AttachTo(batch);
		batch.Drain();

		// Act
		material.Set("alphaHash", true);
		material.Set("alphaHash", false);
		var ops = batch.Drain();

		// Assert
		ops.Count.ShouldBe(1);
		ops.Single().Value.ShouldBe(false);
	}

	[Fact]
	public void ThreeObject_RawSetAfterARawCall_DoesNotCoalesceIntoTheSetBeforeIt()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		material.AttachTo(batch);
		batch.Drain();

		// Act
		material.Set("alphaHash", true);
		material.Call("dispose");
		material.Set("alphaHash", false);
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Set && x.Member == "alphaHash").ShouldBe(2);
	}

	[Fact]
	public void ThreeObject_RawSetAndATypedWriteOfTheSameMember_CoalesceTogether()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.AttachTo(batch);
		batch.Drain();

		// Act
		mesh.Set("visible", false);
		mesh.IsVisible = false;
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Set && x.Member == "visible").ShouldBe(1);
	}

	/// <summary>
	/// Pins the documented sharp edge rather than describing it: a raw write does not reach the C#
	/// field behind a typed property, so the typed property's own "value unchanged, record nothing"
	/// guard can leave three.js holding the raw value. Mixing the two spellings of one property is what
	/// the escape hatch's documentation tells a caller not to do, and this is why.
	/// </summary>
	[Fact]
	public void ThreeObject_TypedWriteOfTheValueARawSetAlreadyChanged_RecordsNothing()
	{
		// Arrange
		var batch = new ThreeBatch();
		var mesh = new Mesh(new BoxGeometry(), new MeshStandardMaterial());
		mesh.AttachTo(batch);
		batch.Drain();
		mesh.Set("visible", false);
		batch.Drain();

		// Act
		mesh.IsVisible = true;
		var ops = batch.Drain();

		// Assert
		mesh.IsVisible.ShouldBeTrue();
		ops.ShouldBeEmpty();
	}

	[Fact]
	public void ThreeObject_RawSetOfAnUnencodableValue_ThrowsNamingTheMemberAndTheType()
	{
		// Arrange
		var material = new MeshStandardMaterial();
		material.AttachTo(new ThreeBatch());

		// Act
		var exception = Record.Exception(() => material.Set("userData", new UnencodableValue()));

		// Assert
		exception.ShouldBeOfType<NotSupportedException>();
		exception.Message.ShouldContain("userData");
		exception.Message.ShouldContain(nameof(UnencodableValue));
		exception.Message.ShouldContain(nameof(MeshStandardMaterial));
	}

	[Fact]
	public void ThreeObject_RawSetOfAnUnencodableValueBeforeAttach_ThrowsAtTheCallRatherThanAtTheAttach()
	{
		// Arrange
		var material = new MeshStandardMaterial();

		// Act
		var exception = Record.Exception(() => material.Set("userData", new UnencodableValue()));

		// Assert
		exception.ShouldBeOfType<NotSupportedException>();
	}

	[Fact]
	public void ThreeObject_RawCallWithAnUnencodableArgument_ThrowsNamingTheMember()
	{
		// Arrange
		var material = new MeshStandardMaterial();
		material.AttachTo(new ThreeBatch());

		// Act
		var exception = Record.Exception(() => material.Call("setValues", new UnencodableValue()));

		// Assert
		exception.ShouldBeOfType<NotSupportedException>();
		exception.Message.ShouldContain("setValues");
	}

	[Fact]
	public void ThreeObject_RawSetOfAMathValue_EncodesItAsATaggedValue()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		material.AttachTo(batch);
		batch.Drain();

		// Act
		material.Set("emissive", Color.Red);
		var recorded = batch.Drain().Single();

		// Assert
		recorded.Value.ShouldBeOfType<ThreeValue.TaggedValue>().Tag.ShouldBe(ThreeWireFormat.ColorTag);
	}

	[Fact]
	public async Task ThreeObject_GetBeforeAttach_Throws()
	{
		// Arrange
		var material = new MeshStandardMaterial();

		// Act
		var exception = await Record.ExceptionAsync(() => material.GetAsync<float>("roughness"));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task ThreeObject_GetOnABatchWithNoContext_Throws()
	{
		// Arrange
		var material = new MeshStandardMaterial();
		material.AttachTo(new ThreeBatch());

		// Act
		var exception = await Record.ExceptionAsync(() => material.GetAsync<float>("roughness"));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task ThreeObject_Get_RecordsAGetOpNamingTheProperty()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = ops => AnswerEveryValueRequest(ops, 0.5f) };
		var context = new ThreeContext(module, contextId: 1);
		var material = new MeshStandardMaterial();
		context.Attach(material);

		// Act
		await material.GetAsync<float>("roughness");

		// Assert
		var getOp = SentOps(module).Single(x => x.Kind == ThreeOpKind.Get);
		getOp.Member.ShouldBe("roughness");
		getOp.Handle.ShouldBe(material.Handle);
	}

	[Fact]
	public async Task ThreeObject_GetAfterAPendingWrite_SendsBothInOneOrderedBatch()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = ops => AnswerEveryValueRequest(ops, 0.5f) };
		var context = new ThreeContext(module, contextId: 1);
		var material = new MeshStandardMaterial();
		context.Attach(material);
		material.Roughness = 0.25f;

		// Act
		await material.GetAsync<float>("roughness");

		// Assert
		var ops = SentOps(module);
		var writeIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "roughness");
		var getIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Get);
		module.Invocations.Count.ShouldBe(1);
		writeIndex.ShouldBeGreaterThanOrEqualTo(0);
		getIndex.ShouldBeGreaterThan(writeIndex);
	}

	[Fact]
	public async Task ThreeObject_GetAnsweredWithAValue_DecodesIt()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = ops => AnswerEveryValueRequest(ops, 0.75f) };
		var context = new ThreeContext(module, contextId: 1);
		var material = new MeshStandardMaterial();
		context.Attach(material);

		// Act
		var roughness = await material.GetAsync<float>("roughness");

		// Assert
		roughness.ShouldBe(0.75f);
	}

	[Fact]
	public async Task ThreeObject_GetAnsweredWithATaggedMathValue_DecodesIt()
	{
		// Arrange
		var module = new RecordingJsObjectReference
		{
			RespondToBatch = ops => AnswerEveryValueRequest(ops, ThreeValue.Encode(new Vector3(1f, 2f, 3f)))
		};

		var context = new ThreeContext(module, contextId: 1);
		var material = new MeshStandardMaterial();
		context.Attach(material);

		// Act
		var scale = await material.GetAsync<Vector3>("normalScale");

		// Assert
		scale.ToArray().ShouldBe([1f, 2f, 3f]);
	}

	/// <summary>
	/// The type-safety decision at the untyped boundary, pinned: a value the declared type cannot hold
	/// faults the awaiting task. Answering with <see langword="default"/> would be a value the browser
	/// never sent, which is the one outcome a read must never produce, and <c>OnError</c> is for
	/// failures with nobody waiting on them.
	/// </summary>
	[Fact]
	public async Task ThreeObject_GetAnsweredWithAValueTheDeclaredTypeCannotHold_Faults()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = ops => AnswerEveryValueRequest(ops, "MeshStandardMaterial") };
		var context = new ThreeContext(module, contextId: 1);
		var material = new MeshStandardMaterial();
		context.Attach(material);

		// Act
		var exception = await Record.ExceptionAsync(() => material.GetAsync<float>("type"));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
		exception!.Message.ShouldContain("type");
		exception.Message.ShouldContain(typeof(float).FullName!);
	}

	[Fact]
	public async Task ThreeObject_GetAnsweredWithAValueTheDeclaredTypeCannotHold_DoesNotRaiseOnError()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = ops => AnswerEveryValueRequest(ops, "MeshStandardMaterial") };
		var context = new ThreeContext(module, contextId: 1);
		var material = new MeshStandardMaterial();
		context.Attach(material);
		var wasOnErrorRaised = false;
		context.OnError += _ => wasOnErrorRaised = true;

		// Act
		await Record.ExceptionAsync(() => material.GetAsync<float>("type"));

		// Assert
		wasOnErrorRaised.ShouldBeFalse();
	}

	[Fact]
	public async Task ThreeObject_GetRejectedByTheApplier_Faults()
	{
		// Arrange
		var module = new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Results = ops
					.Where(x => x.Kind == ThreeOpKind.Get)
					.Select(x => new ThreeReadResult { RequestId = x.RequestId, Message = "'nope' is not a property on the object at handle '1'" })
					.ToList()
			}
		};

		var context = new ThreeContext(module, contextId: 1);
		var material = new MeshStandardMaterial();
		context.Attach(material);

		// Act
		var exception = await Record.ExceptionAsync(() => material.GetAsync<float>("nope"));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
		exception!.Message.ShouldContain("is not a property");
	}

	[Fact]
	public async Task ThreeObject_CallAsync_RecordsAReadOpNamingTheMethod()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = ops => AnswerEveryValueRequest(ops, 2f) };
		var context = new ThreeContext(module, contextId: 1);
		var curve = new Primitive("LineCurve3");
		context.Attach(curve);

		// Act
		var length = await curve.CallAsync<float>("getLength");

		// Assert
		SentOps(module).Single(x => x.Kind == ThreeOpKind.Read).Member.ShouldBe("getLength");
		length.ShouldBe(2f);
	}

	[Fact]
	public void ThreeBatch_SetRecordedAfterAGet_DoesNotOverwriteTheSetBeforeIt()
	{
		// Arrange
		var batch = new ThreeBatch();
		batch.Set(1, "fov", 50f);
		batch.Get(1, "fov");

		// Act
		batch.Set(1, "fov", 90f);
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Set && x.Member == "fov").ShouldBe(2);
	}

	[Fact]
	public void ThreeBatch_AGetAndARead_DrawFromOneRequestIdSpace()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		var readRequestId = batch.Read(1, "getFocalLength", []);
		var getRequestId = batch.Get(1, "fov");

		// Assert
		getRequestId.ShouldNotBe(readRequestId);
	}

	[Fact]
	public void Primitive_Attached_EmitsACreateOpNamingTheTypeAndItsArguments()
	{
		// Arrange
		var batch = new ThreeBatch();
		var vector = new Primitive("Vector2", 0.5f, 0.25f);

		// Act
		vector.AttachTo(batch);
		var created = batch.Drain().Single();

		// Assert
		created.Kind.ShouldBe(ThreeOpKind.Create);
		created.Type.ShouldBe("Vector2");
		created.Args.ShouldBe([0.5f, 0.25f]);
	}

	[Fact]
	public void Primitive_WithAMirroredConstructorArgument_EmitsItsCreateFirst()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BoxGeometry();
		var primitive = new Primitive("WireframeGeometry", geometry);

		// Act
		primitive.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var argumentCreateIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == geometry.Handle);
		var primitiveCreateIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == primitive.Handle);
		argumentCreateIndex.ShouldBeGreaterThanOrEqualTo(0);
		primitiveCreateIndex.ShouldBeGreaterThan(argumentCreateIndex);
	}

	[Fact]
	public void Primitive_WithAnUnencodableConstructorArgument_ThrowsAtConstruction()
	{
		// Arrange & Act
		var exception = Record.Exception(() => new Primitive("Vector2", new UnencodableValue()));

		// Assert
		exception.ShouldBeOfType<NotSupportedException>();
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Primitive_WithABlankTypeName_Throws(string threeTypeName)
	{
		// Arrange & Act
		var exception = Record.Exception(() => new Primitive(threeTypeName));

		// Assert
		exception.ShouldBeOfType<ArgumentException>();
	}

	[Fact]
	public void Primitive_PassedToARawSet_TravelsAsAHandleReference()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial();
		material.AttachTo(batch);
		batch.Drain();
		var normalScale = new Primitive("Vector2", 0.5f, 0.5f);

		// Act
		material.Set("normalScale", normalScale);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Type == "Vector2");
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "normalScale");
		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		setIndex.ShouldBeGreaterThan(createIndex);
		ops.ElementAt(setIndex).Value.ShouldBeOfType<ThreeValue.HandleReference>().Handle.ShouldBe(normalScale.Handle);
	}

	[Fact]
	public void PrimitiveObject3D_AddedToAScene_IsCreatedAndParented()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		var helper = new PrimitiveObject3D("Box3Helper");
		scene.Add(helper);

		// Act
		scene.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Type == "Box3Helper");
		var addIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Add && x.ChildHandle == helper.Handle);
		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		addIndex.ShouldBeGreaterThan(createIndex);
	}

	[Fact]
	public void PrimitiveObject3D_ConfiguredBeforeBeingAdded_ReplaysBothTheTransformAndTheRawWrite()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		var audio = new PrimitiveObject3D("PositionalAudio");
		audio.Position.Set(0f, 1f, 0f);
		audio.Set("refDistance", 2f);
		audio.Call("play");
		scene.Add(audio);

		// Act
		scene.AttachTo(batch);
		var recorded = batch.Drain()
			.Where(x => x.Handle == audio.Handle)
			.Select(x => $"{x.Kind}:{x.Member ?? x.Type}")
			.ToList();

		// Assert
		recorded.ShouldBe(
		[
			"Create:PositionalAudio",
			"Set:position",
			"Set:rotation",
			"Set:scale",
			"Set:visible",
			"Set:refDistance",
			"Call:play"
		]);
	}

	/// <summary>
	/// Every op the batch carried on the single interop call the module received.
	/// </summary>
	/// <param name="module">The recording stand-in for the JavaScript module.</param>
	/// <returns>The ops, in the order they were recorded.</returns>
	private static List<ThreeOp> SentOps(RecordingJsObjectReference module)
	{
		return module.Invocations
			.Single(x => x.Identifier == "applyBatch")
			.Arguments
			.OfType<IReadOnlyList<ThreeOp>>()
			.Single()
			.ToList();
	}

	/// <summary>
	/// Answers every value-producing op in a batch with the same value, whichever of the two kinds it
	/// is, which is all a test that only cares about the ops that were sent needs.
	/// </summary>
	/// <param name="ops">The batch the applier received.</param>
	/// <param name="value">The value to answer with, serialized as the applier would send it.</param>
	/// <returns>A response with one result row per read.</returns>
	private static ThreeBatchResponse AnswerEveryValueRequest(IReadOnlyList<ThreeOp> ops, object? value)
	{
		return new ThreeBatchResponse
		{
			Results = ops
				.Where(x => x.Kind is ThreeOpKind.Read or ThreeOpKind.Get)
				.Select(x => new ThreeReadResult
				{
					RequestId = x.RequestId,
					Value = JsonSerializer.SerializeToElement(value, new JsonSerializerOptions(JsonSerializerDefaults.Web))
				})
				.ToList()
		};
	}

	/// <summary>
	/// A reference type the wire has no encoding for, standing in for whatever a caller might reach for
	/// — a POCO, a delegate's target, a DOM wrapper. Declared here rather than using <c>new object()</c>
	/// so the failure message can be asserted to name the offending type.
	/// </summary>
	private sealed class UnencodableValue
	{
	}
}
