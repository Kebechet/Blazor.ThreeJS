using System.Text.Json;
using Blazor.ThreeJS.Tests.Core;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// Pins the C# half of three.js's statics. Its utility classes — <c>AnimationUtils</c>,
/// <c>ShapeUtils</c>, <c>DataUtils</c> — hang their work off the class rather than off any object, so
/// there is no handle to address them by and no <c>Batch</c> for them to record into. They are emitted
/// as C# statics taking the context explicitly, and the op names the class instead.
/// <para>
/// That the applier runs them against the real class is pinned from the other end by
/// <c>tests/wire-format.test.mjs</c>, against the vendored three.js.
/// </para>
/// </summary>
public class StaticMemberTests
{
	[Fact]
	public async Task DataUtils_ToHalfFloat_SendsAReadNamingTheClassRatherThanAHandle()
	{
		// Arrange
		var module = AnswerEveryQueryWith("1");
		var context = new ThreeContext(module, contextId: 1);

		// Act
		await DataUtils.ToHalfFloatAsync(context, 1f);

		// Assert
		var op = SentOps(module).Single(x => x.Kind == ThreeOpKind.Read);
		op.Type.ShouldBe("DataUtils");
		op.Member.ShouldBe("toHalfFloat");
		op.Handle.ShouldBe(0);
	}

	[Fact]
	public async Task DataUtils_ToHalfFloat_AnswersWithWhatTheApplierSent()
	{
		var module = AnswerEveryQueryWith("15360");
		var context = new ThreeContext(module, contextId: 1);

		var half = await DataUtils.ToHalfFloatAsync(context, 1f);

		half.ShouldBe(15360f);
	}

	[Fact]
	public async Task AnimationUtils_Subclip_AttachesAMirroredArgumentBeforeTheReadThatNamesIt()
	{
		// A static takes mirrored objects like any other member, and the object it is handed still has
		// to exist on the JavaScript side before the op that references it by handle.
		var module = AnswerEveryQueryWith("null");
		var context = new ThreeContext(module, contextId: 1);
		var clip = new AnimationClip("walk", 1f);

		await AnimationUtils.SubclipAsync(context, clip, "run", 0f, 10f);

		var ops = SentOps(module);
		var createIndex = ops.ToList().FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == clip.Handle);
		var readIndex = ops.ToList().FindIndex(x => x.Kind == ThreeOpKind.Read && x.Type == "AnimationUtils");

		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		readIndex.ShouldBeGreaterThan(createIndex);
	}

	[Fact]
	public void Texture_StaticProperty_IsStillNotMirrored()
	{
		// A static *method* is reachable; a static *property* is not, and deliberately. `DEFAULT_ANISOTROPY`
		// is a global three.js setting rather than state any one mirror owns, so there is nothing to write
		// through and no object whose value it would be.
		typeof(Texture).GetProperty("DEFAULT_ANISOTROPY").ShouldBeNull();
	}

	private static RecordingJsObjectReference AnswerEveryQueryWith(string json)
	{
		var value = JsonDocument.Parse(json).RootElement;
		return new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Results = ops
					.Where(x => x.Kind is ThreeOpKind.Read or ThreeOpKind.Get)
					.Select(x => new ThreeReadResult { RequestId = x.RequestId, Value = value })
					.ToList()
			}
		};
	}

	private static IReadOnlyList<ThreeOp> SentOps(RecordingJsObjectReference module)
	{
		return module.Invocations
			.Where(x => x.Identifier == "applyBatch")
			.SelectMany(x => x.Arguments.OfType<IReadOnlyList<ThreeOp>>().Single())
			.ToList();
	}
}
