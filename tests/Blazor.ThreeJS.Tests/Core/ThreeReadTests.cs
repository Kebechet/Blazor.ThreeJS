using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.JSInterop;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

/// <summary>
/// Covers the one op that carries a value back. Everything asserted here is a property of the C#
/// half; that a value survives the boundary at all is pinned end to end against the vendored three.js
/// by <c>tests/wire-format.test.mjs</c>, which a mocked module could never prove.
/// </summary>
public class ThreeReadTests
{
	[Fact]
	public async Task ThreeContext_ReadAfterAPendingWrite_SendsTheWriteAndTheReadInOneOrderedBatch()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = AnswerEveryRead };
		var context = new ThreeContext(module, contextId: 1);
		var camera = new PerspectiveCamera();
		context.Attach(camera);
		camera.Fov = 90f;

		// Act
		await context.ReadAsync<float>(camera.Handle, "getFocalLength", []);

		// Assert
		var ops = module.Invocations
			.Single(x => x.Identifier == "applyBatch")
			.Arguments
			.OfType<IReadOnlyList<ThreeOp>>()
			.Single();

		var writeIndex = ops.ToList().FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "fov");
		var readIndex = ops.ToList().FindIndex(x => x.Kind == ThreeOpKind.Read);
		writeIndex.ShouldBeGreaterThanOrEqualTo(0);
		readIndex.ShouldBeGreaterThan(writeIndex);
	}

	[Fact]
	public async Task ThreeContext_ReadWithPendingWrites_MakesASingleInteropCall()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = AnswerEveryRead };
		var context = new ThreeContext(module, contextId: 1);
		var camera = new PerspectiveCamera();
		context.Attach(camera);
		camera.Fov = 90f;

		// Act
		await context.ReadAsync<float>(camera.Handle, "getFocalLength", []);

		// Assert
		module.Invocations.Count.ShouldBe(1);
	}

	[Fact]
	public async Task ThreeContext_TwoConcurrentReadsAnsweredOutOfOrder_EachResolveToTheirOwnResult()
	{
		// Arrange
		var module = new DeferringJsObjectReference();
		var context = new ThreeContext(module, contextId: 1);

		// Act
		var firstRead = context.ReadAsync<float>(handle: 3, "getFocalLength", []);
		var secondRead = context.ReadAsync<float>(handle: 4, "getEffectiveFOV", []);
		module.PendingBatches.Count.ShouldBe(2);
		module.PendingBatches.Last().AnswerEveryReadWith(90f);
		module.PendingBatches.First().AnswerEveryReadWith(18f);

		// Assert
		(await firstRead).ShouldBe(18f);
		(await secondRead).ShouldBe(90f);
	}

	[Fact]
	public async Task ThreeContext_TwoConcurrentReads_CarryDistinctRequestIds()
	{
		// Arrange
		var module = new DeferringJsObjectReference();
		var context = new ThreeContext(module, contextId: 1);

		// Act
		_ = context.ReadAsync<float>(handle: 3, "getFocalLength", []);
		_ = context.ReadAsync<float>(handle: 4, "getEffectiveFOV", []);

		// Assert
		var requestIds = module.PendingBatches
			.SelectMany(x => x.Ops)
			.Where(x => x.Kind == ThreeOpKind.Read)
			.Select(x => x.RequestId)
			.ToList();

		requestIds.Distinct().Count().ShouldBe(requestIds.Count);
		await Task.CompletedTask;
	}

	[Fact]
	public async Task ThreeContext_ReadWhenCircuitDisconnected_Faults()
	{
		// Arrange
		var context = new ThreeContext(new ThrowingJsObjectReference(), contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => context.ReadAsync<float>(handle: 3, "getFocalLength", []));

		// Assert
		exception.ShouldBeOfType<JSDisconnectedException>();
	}

	[Fact]
	public async Task ThreeContext_ReadWhenModuleAlreadyDisposed_Faults()
	{
		// Arrange
		var context = new ThreeContext(new AlreadyDisposedJsObjectReference(), contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => context.ReadAsync<float>(handle: 3, "getFocalLength", []));

		// Assert
		exception.ShouldBeOfType<ObjectDisposedException>();
	}

	[Fact]
	public async Task ThreeContext_ReadThatNeverGetsAnswered_FaultsWithTimeoutRatherThanHanging()
	{
		// Arrange
		var context = new ThreeContext(new DeferringJsObjectReference(), contextId: 1)
		{
			ReadTimeout = TimeSpan.FromMilliseconds(50)
		};

		// Act
		var exception = await Record.ExceptionAsync(() => context.ReadAsync<float>(handle: 3, "getFocalLength", []));

		// Assert
		exception.ShouldBeOfType<TimeoutException>();
	}

	[Fact]
	public async Task ThreeContext_ReadRejectedByTheApplier_Faults()
	{
		// Arrange
		var module = new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Results = ops
					.Where(x => x.Kind == ThreeOpKind.Read)
					.Select(x => new ThreeReadResult { RequestId = x.RequestId, Message = "Unknown handle '3'" })
					.ToList()
			}
		};

		var context = new ThreeContext(module, contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => context.ReadAsync<float>(handle: 3, "getFocalLength", []));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
		exception.Message.ShouldContain("Unknown handle '3'");
	}

	[Fact]
	public async Task ThreeContext_ReadRejectedByTheApplier_DoesNotAlsoRaiseOnError()
	{
		// Arrange
		var module = new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Results = ops
					.Where(x => x.Kind == ThreeOpKind.Read)
					.Select(x => new ThreeReadResult { RequestId = x.RequestId, Message = "Unknown handle '3'" })
					.ToList()
			}
		};

		var context = new ThreeContext(module, contextId: 1);
		var wasOnErrorRaised = false;
		context.OnError += _ => wasOnErrorRaised = true;

		// Act
		await Record.ExceptionAsync(() => context.ReadAsync<float>(handle: 3, "getFocalLength", []));

		// Assert
		wasOnErrorRaised.ShouldBeFalse();
	}

	[Fact]
	public async Task ThreeContext_ReadInABatchWhoseWritesFailed_StillRaisesOnErrorForThoseWrites()
	{
		// Arrange
		var applierErrors = new List<ThreeError> { new() { Handle = 1, Member = "roughness", Message = "Invalid roughness value." } };
		var module = new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Errors = applierErrors,
				Results = ops
					.Where(x => x.Kind == ThreeOpKind.Read)
					.Select(x => new ThreeReadResult { RequestId = x.RequestId, Value = JsonSerializer.SerializeToElement(1f) })
					.ToList()
			}
		};

		var context = new ThreeContext(module, contextId: 1);
		IReadOnlyList<ThreeError>? raisedErrors = null;
		context.OnError += errors => raisedErrors = errors;

		// Act
		await context.ReadAsync<float>(handle: 3, "getFocalLength", []);

		// Assert
		raisedErrors.ShouldBe(applierErrors);
	}

	[Fact]
	public async Task ThreeContext_ReadAnsweredWithoutItsResultRow_Faults()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = _ => new ThreeBatchResponse() };
		var context = new ThreeContext(module, contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => context.ReadAsync<float>(handle: 3, "getFocalLength", []));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task ThreeContext_ReadThatDrainedTheBatch_LeavesNothingPending()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = AnswerEveryRead };
		var context = new ThreeContext(module, contextId: 1);
		var camera = new PerspectiveCamera();
		context.Attach(camera);

		// Act
		await context.ReadAsync<float>(camera.Handle, "getFocalLength", []);

		// Assert
		context.Batch.HasPendingOps.ShouldBeFalse();
	}

	[Fact]
	public async Task ThreeObject_ReadBeforeBeingAttached_Throws()
	{
		// Arrange
		var readable = new ReadableTestObject();

		// Act
		var exception = await Record.ExceptionAsync(() => readable.ReadFloatAsync("getFocalLength"));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task ThreeObject_ReadOnABatchWithNoContext_Throws()
	{
		// Arrange
		var readable = new ReadableTestObject();
		readable.AttachTo(new ThreeBatch());

		// Act
		var exception = await Record.ExceptionAsync(() => readable.ReadFloatAsync("getFocalLength"));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task ThreeObject_ReadWithAMirroredArgument_AttachesItBeforeTheReadOp()
	{
		// Arrange
		var module = new RecordingJsObjectReference { RespondToBatch = AnswerEveryRead };
		var context = new ThreeContext(module, contextId: 1);
		var readable = new ReadableTestObject();
		readable.AttachTo(context.Batch);
		var geometry = new BoxGeometry();

		// Act
		await readable.ReadFloatAsync("measure", geometry);

		// Assert
		var ops = module.Invocations
			.Single(x => x.Identifier == "applyBatch")
			.Arguments
			.OfType<IReadOnlyList<ThreeOp>>()
			.Single()
			.ToList();

		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == geometry.Handle);
		var readIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Read);
		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		readIndex.ShouldBeGreaterThan(createIndex);
	}

	[Fact]
	public async Task ThreeObject_ReadReturningATaggedMathValue_DecodesIt()
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
						Value = JsonSerializer.SerializeToElement(ThreeValue.Encode(new Vector3(1f, 2f, 3f)))
					})
					.ToList()
			}
		};

		var context = new ThreeContext(module, contextId: 1);
		var readable = new ReadableTestObject();
		readable.AttachTo(context.Batch);

		// Act
		var position = await readable.ReadVectorAsync("getVertexPosition");

		// Assert
		position.ToArray().ShouldBe([1f, 2f, 3f]);
	}

	[Fact]
	public void ThreeBatch_SetRecordedAfterARead_DoesNotOverwriteTheSetBeforeIt()
	{
		// Arrange
		var batch = new ThreeBatch();
		batch.Set(1, "fov", 50f);
		batch.Read(1, "getFocalLength", []);

		// Act
		batch.Set(1, "fov", 90f);
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Set && x.Member == "fov").ShouldBe(2);
	}

	[Fact]
	public void ThreeBatch_TwoReads_GetDistinctRequestIds()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		var firstRequestId = batch.Read(1, "getFocalLength", []);
		var secondRequestId = batch.Read(1, "getEffectiveFOV", []);

		// Assert
		firstRequestId.ShouldNotBe(secondRequestId);
	}

	[Fact]
	public void ThreeBatch_ReadAfterADrain_DoesNotReuseAnEarlierRequestId()
	{
		// Arrange
		var batch = new ThreeBatch();
		var firstRequestId = batch.Read(1, "getFocalLength", []);
		batch.Drain();

		// Act
		var secondRequestId = batch.Read(1, "getFocalLength", []);

		// Assert
		secondRequestId.ShouldNotBe(firstRequestId);
	}

	[Fact]
	public void ThreeBatch_TwoReadsOfTheSameMember_AreTwoOps()
	{
		// Arrange
		var batch = new ThreeBatch();

		// Act
		batch.Read(1, "getFocalLength", []);
		batch.Read(1, "getFocalLength", []);

		// Assert
		batch.Drain().Count(x => x.Kind == ThreeOpKind.Read).ShouldBe(2);
	}

	/// <summary>
	/// Answers every read op in a batch with the same number, which is all a test that only cares about
	/// the ops that were sent needs.
	/// </summary>
	/// <param name="ops">The batch the applier received.</param>
	/// <returns>A response with one result row per read.</returns>
	private static ThreeBatchResponse AnswerEveryRead(IReadOnlyList<ThreeOp> ops)
	{
		return new ThreeBatchResponse
		{
			Results = ops
				.Where(x => x.Kind == ThreeOpKind.Read)
				.Select(x => new ThreeReadResult { RequestId = x.RequestId, Value = JsonSerializer.SerializeToElement(1f) })
				.ToList()
		};
	}

	/// <summary>
	/// A minimal mirrored object with the read helper exposed, so the base class's own read behaviour
	/// can be tested without depending on which generated class happens to carry a query today.
	/// </summary>
	private sealed class ReadableTestObject : ThreeObject
	{
		/// <summary>Name of the three.js constructor this stands in for.</summary>
		protected override string ThreeTypeName
		{
			get { return "Object3D"; }
		}

		/// <summary>Reads a number back.</summary>
		/// <param name="member">Method to invoke.</param>
		/// <param name="args">Positional arguments.</param>
		/// <returns>The number three.js returned.</returns>
		public Task<float> ReadFloatAsync(string member, params object?[] args)
		{
			return RecordRead<float>(member, args);
		}

		/// <summary>Reads a vector back.</summary>
		/// <param name="member">Method to invoke.</param>
		/// <returns>The vector three.js returned.</returns>
		public Task<Vector3> ReadVectorAsync(string member)
		{
			return RecordRead<Vector3>(member);
		}
	}
}

/// <summary>
/// Fake <see cref="IJSObjectReference"/> that never answers a batch on its own, so a test can decide
/// when — and in which order — each in-flight call completes. Nothing else can produce two genuinely
/// concurrent reads: a fake that answers synchronously finishes the first read before the second one
/// starts, which would make a correlation assertion vacuous. Implemented directly rather than
/// substituted for the reason spelled out on <c>ThrowingJsObjectReference</c>.
/// </summary>
internal sealed class DeferringJsObjectReference : IJSObjectReference
{
	private readonly List<DeferredBatch> _pendingBatches = [];

	/// <summary>Every batch received and not yet answered, in the order it arrived.</summary>
	public IReadOnlyList<DeferredBatch> PendingBatches
	{
		get { return _pendingBatches; }
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		var ops = args?.OfType<IReadOnlyList<ThreeOp>>().FirstOrDefault() ?? [];
		var deferred = new DeferredBatch { Ops = ops };
		_pendingBatches.Add(deferred);
		return new ValueTask<TValue>(CastAsync<TValue>(deferred.Completion.Task));
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		// Deliberately ignores the token. A consumer-supplied IJSObjectReference is free to do the same,
		// and the read timeout has to hold anyway.
		return InvokeAsync<TValue>(identifier, args);
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}

	private static async Task<TValue> CastAsync<TValue>(Task<ThreeBatchResponse> response)
	{
		return (TValue) (object) await response;
	}
}

/// <summary>One batch a <see cref="DeferringJsObjectReference"/> received and has not answered yet.</summary>
internal sealed class DeferredBatch
{
	/// <summary>The ops the batch carried.</summary>
	public required IReadOnlyList<ThreeOp> Ops { get; init; }

	/// <summary>Completes the interop call this batch belongs to.</summary>
	public TaskCompletionSource<ThreeBatchResponse> Completion { get; } = new();

	/// <summary>Answers every read op in this batch with one number, matched by request id.</summary>
	/// <param name="value">The number to answer with.</param>
	public void AnswerEveryReadWith(float value)
	{
		Completion.SetResult(new ThreeBatchResponse
		{
			Results = Ops
				.Where(x => x.Kind == ThreeOpKind.Read)
				.Select(x => new ThreeReadResult { RequestId = x.RequestId, Value = JsonSerializer.SerializeToElement(value) })
				.ToList()
		});
	}
}
