using System.Text.Json;
using Blazor.ThreeJS.Tests.Core;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// Pins the generated shape of a three.js method that returns a promise. The read op already answers
/// on a task, so the C# signature is the one the awaited type would have given anyway — what the
/// promise adds is that the applier waits for it, which
/// <c>tests/wire-format.test.mjs</c> pins against the vendored three.js. What is asserted here is that
/// such a method reaches the wire at all, as a read rather than as a fire-and-forget call.
/// </summary>
public class AwaitedQueryTests
{
	[Fact]
	public async Task WebGPURenderer_ClearAsync_RecordsAReadRatherThanACall()
	{
		// Arrange: `clearAsync` answers nothing, and is awaited for when rather than for what. A call op
		// would apply just as well and complete immediately, which would take away the only thing the
		// method offers.
		var module = AnswerEveryQueryWith("null");
		var context = new ThreeContext(module, contextId: 1);
		var renderer = new WebGPURenderer();
		context.Attach(renderer);

		// Act
		await renderer.ClearAsync();

		// Assert
		var op = SentOps(module).Single(x => x.Kind == ThreeOpKind.Read);
		op.Member.ShouldBe("clearAsync");
		op.Handle.ShouldBe(renderer.Handle);
		op.MintsHandle.ShouldBeFalse();
	}

	[Fact]
	public async Task WebGPURenderer_WaitForGpu_KeepsTheMirrorsOwnSuffixWhereThreeJsHasNone()
	{
		// Arrange
		var module = AnswerEveryQueryWith("null");
		var context = new ThreeContext(module, contextId: 1);
		var renderer = new WebGPURenderer();
		context.Attach(renderer);

		// Act
		await renderer.WaitForGPUAsync();

		// Assert
		SentOps(module).Single(x => x.Kind == ThreeOpKind.Read).Member.ShouldBe("waitForGPU");
	}

	[Fact]
	public async Task WebGPURenderer_HasFeature_AndHasFeatureAsync_AreTwoMethodsReachingTwoThreeJsMembers()
	{
		// three.js declares both, and the mirror's own `Async` suffix collides with three.js's. Doubling
		// the suffix on the one that already carries it is ugly; dropping either method would be a
		// missing feature, which is worse.
		var module = AnswerEveryQueryWith("null");
		var context = new ThreeContext(module, contextId: 1);
		var renderer = new WebGPURenderer();
		context.Attach(renderer);

		// Act
		await renderer.HasFeatureAsync("depth-clip-control");
		await renderer.HasFeatureAsyncAsync("depth-clip-control");

		// Assert
		SentOps(module)
			.Where(x => x.Kind == ThreeOpKind.Read)
			.Select(x => x.Member)
			.ShouldBe(["hasFeature", "hasFeatureAsync"]);
	}

	/// <summary>
	/// A module that answers every read with one literal. <c>null</c> is what an awaited void actually
	/// sends: three.js resolves the promise with <c>undefined</c>, and the applier encodes that as null.
	/// </summary>
	/// <param name="json">The JSON literal every result row carries.</param>
	/// <returns>The recording module.</returns>
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
