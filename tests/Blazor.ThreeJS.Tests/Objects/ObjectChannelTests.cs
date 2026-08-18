using System.Text.Json;
using Blazor.ThreeJS.Tests.Core;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// Pins the generated half of the object-valued read channel: a member whose answer is a three.js
/// object rather than a value. Three shapes reach it — a read-only property adopted into its declared
/// type, a method adopted into its declared type, and either of those where no generated class
/// describes the result and an untyped <see cref="Primitive"/> carries it instead.
/// <para>
/// What is asserted is the wire: the op kind, the flag that asks the applier for a handle, and the
/// member name three.js will be asked for. That the applier honours the flag is pinned end to end
/// against the vendored three.js by <c>tests/wire-format.test.mjs</c>, which a fake module cannot show.
/// </para>
/// </summary>
public class ObjectChannelTests
{
	private const int AnsweredHandle = -5;

	[Fact]
	public async Task Audio_ListenerRead_SendsAGetOpThatAsksForAHandle()
	{
		// Arrange
		var module = AnswerEveryQueryWith(AnsweredHandle, "AudioListener");
		var context = new ThreeContext(module, contextId: 1);
		var audio = new Audio(new AudioListener());
		context.Attach(audio);

		// Act
		await audio.ListenerAsync();

		// Assert
		var op = SentOps(module).Single(x => x.Kind == ThreeOpKind.Get);
		op.Member.ShouldBe("listener");
		op.Handle.ShouldBe(audio.Handle);
		op.MintsHandle.ShouldBeTrue();
	}

	[Fact]
	public async Task Audio_ListenerRead_AdoptsTheAnsweredHandleAsTheTypeTheSignatureDeclares()
	{
		// Arrange
		var module = AnswerEveryQueryWith(AnsweredHandle, "AudioListener");
		var context = new ThreeContext(module, contextId: 1);
		var audio = new Audio(new AudioListener());
		context.Attach(audio);

		// Act
		var listener = await audio.ListenerAsync();

		// Assert
		listener.ShouldNotBeNull();
		listener.Handle.ShouldBe(AnsweredHandle);
	}

	[Fact]
	public async Task Audio_ListenerAnsweringAHandleThisContextAlreadyMirrors_ResolvesToThatMirror()
	{
		// Arrange
		var ownListener = new AudioListener();
		var audio = new Audio(ownListener);
		var module = new RecordingJsObjectReference();
		var context = new ThreeContext(module, contextId: 1);
		context.Attach(audio);
		module.RespondToBatch = ops => AnswerObjectQueries(ops, ownListener.Handle, "AudioListener");

		// Act
		var listener = await audio.ListenerAsync();

		// Assert
		listener.ShouldBeSameAs(ownListener);
	}

	[Fact]
	public async Task Audio_ListenerHoldingNothing_AnswersWithNull()
	{
		// Arrange
		var module = new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Results = ops
					.Where(IsQuery)
					.Select(x => new ThreeReadResult { RequestId = x.RequestId, Value = JsonDocument.Parse("null").RootElement })
					.ToList()
			}
		};

		var context = new ThreeContext(module, contextId: 1);
		var audio = new Audio(new AudioListener());
		context.Attach(audio);

		// Act
		var listener = await audio.ListenerAsync();

		// Assert
		listener.ShouldBeNull();
	}

	[Fact]
	public async Task BufferGeometry_GetIndirectRead_SendsAReadOpThatAsksForAHandle()
	{
		// Arrange
		var module = AnswerEveryQueryWith(AnsweredHandle, "IndirectStorageBufferAttribute");
		var context = new ThreeContext(module, contextId: 1);
		var geometry = new BufferGeometry();
		context.Attach(geometry);

		// Act
		await geometry.GetIndirectAsync();

		// Assert
		var op = SentOps(module).Single(x => x.Kind == ThreeOpKind.Read);
		op.Member.ShouldBe("getIndirect");
		op.Handle.ShouldBe(geometry.Handle);
		op.MintsHandle.ShouldBeTrue();
	}

	[Fact]
	public async Task BufferGeometry_GetIndirectRead_AnswersWithAPrimitiveCarryingThreeJsOwnTypeName()
	{
		// Arrange
		var module = AnswerEveryQueryWith(AnsweredHandle, "IndirectStorageBufferAttribute");
		var context = new ThreeContext(module, contextId: 1);
		var geometry = new BufferGeometry();
		context.Attach(geometry);

		// Act
		var indirect = await geometry.GetIndirectAsync();

		// Assert
		indirect.ShouldNotBeNull();
		indirect.Handle.ShouldBe(AnsweredHandle);
		indirect.ThreeType.ShouldBe("IndirectStorageBufferAttribute");
	}

	[Fact]
	public async Task ThreeObject_ReadOnlyPropertyHoldingAnUntypedObject_SendsAGetOpThatAsksForAHandle()
	{
		// Arrange: the shape the generator emits for a read-only property no generated class types,
		// reached here through the escape hatch the generated member delegates to.
		var module = AnswerEveryQueryWith(AnsweredHandle, "WebGLShadowMap");
		var context = new ThreeContext(module, contextId: 1);
		var renderer = new Primitive("WebGPURenderer");
		context.Attach(renderer);

		// Act
		var shadowMap = await renderer.GetObjectAsync("shadowMap");

		// Assert
		var op = SentOps(module).Single(x => x.Kind == ThreeOpKind.Get);
		op.Member.ShouldBe("shadowMap");
		op.MintsHandle.ShouldBeTrue();
		shadowMap.ShouldNotBeNull();
		shadowMap.ThreeType.ShouldBe("WebGLShadowMap");
	}

	[Fact]
	public async Task MeshStandardMaterial_UuidRead_SendsAGetOpThatDoesNotAskForAHandle()
	{
		// Arrange: the value channel is unchanged by the object one, and a get that asked for a handle
		// where a string was wanted would decode a reference out of an answer three.js sent as text.
		var module = new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Results = ops
					.Where(IsQuery)
					.Select(x => new ThreeReadResult { RequestId = x.RequestId, Value = JsonSerializer.SerializeToElement("f0d2") })
					.ToList()
			}
		};

		var context = new ThreeContext(module, contextId: 1);
		var material = new MeshStandardMaterial();
		context.Attach(material);

		// Act
		var uuid = await material.UuidAsync();

		// Assert
		var op = SentOps(module).Single(x => x.Kind == ThreeOpKind.Get);
		op.Member.ShouldBe("uuid");
		op.MintsHandle.ShouldBeFalse();
		uuid.ShouldBe("f0d2");
	}

	/// <summary>Whether an op is one of the two kinds that produce a result row.</summary>
	/// <param name="op">The op the applier received.</param>
	/// <returns><see langword="true"/> for a read and a get.</returns>
	private static bool IsQuery(ThreeOp op)
	{
		return op.Kind is ThreeOpKind.Read or ThreeOpKind.Get;
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
	/// A module that answers every read and get with the reference shape the applier sends for an op
	/// marked <c>n:true</c>. Written as literal JSON rather than serialized from the C# type it decodes
	/// into, so a rename of either wire key fails the test instead of travelling through it.
	/// </summary>
	/// <param name="handle">Handle the applier registered the object under.</param>
	/// <param name="threeTypeName">three.js's own <c>constructor.name</c> for it.</param>
	/// <returns>The fake module.</returns>
	private static RecordingJsObjectReference AnswerEveryQueryWith(int handle, string threeTypeName)
	{
		return new RecordingJsObjectReference
		{
			RespondToBatch = ops => AnswerObjectQueries(ops, handle, threeTypeName)
		};
	}

	/// <summary>Builds the response body <see cref="AnswerEveryQueryWith"/> sends.</summary>
	/// <param name="ops">The batch the applier received.</param>
	/// <param name="handle">Handle the applier registered the object under.</param>
	/// <param name="threeTypeName">three.js's own <c>constructor.name</c> for it.</param>
	/// <returns>One result row per read and get.</returns>
	private static ThreeBatchResponse AnswerObjectQueries(IReadOnlyList<ThreeOp> ops, int handle, string threeTypeName)
	{
		var reference = JsonDocument.Parse($"{{\"$ref\":{handle},\"t\":\"{threeTypeName}\"}}").RootElement;
		return new ThreeBatchResponse
		{
			Results = ops
				.Where(IsQuery)
				.Select(x => new ThreeReadResult { RequestId = x.RequestId, Value = reference })
				.ToList()
		};
	}
}
