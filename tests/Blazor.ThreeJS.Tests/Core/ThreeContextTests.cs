using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.JSInterop;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

public class ThreeContextTests
{
	[Fact]
	public void ThreeContext_AttachCalledWithAScene_RecordsCreateOpForTheSceneAndItsChildren()
	{
		// Arrange
		var module = Substitute.For<IJSObjectReference>();
		var context = new ThreeContext(module, contextId: 1);
		var scene = new Scene();
		var camera = new PerspectiveCamera();
		scene.Add(camera);

		// Act
		context.Attach(scene);
		var ops = context.Batch.Drain();

		// Assert
		ops.ShouldContain(x => x.Kind == ThreeOpKind.Create && x.Handle == scene.Handle && x.Type == nameof(Scene));
		ops.ShouldContain(x => x.Kind == ThreeOpKind.Create && x.Handle == camera.Handle && x.Type == nameof(PerspectiveCamera));
		ops.ShouldContain(x => x.Kind == ThreeOpKind.Add && x.Handle == scene.Handle && x.ChildHandle == camera.Handle);
	}

	[Fact]
	public async Task ThreeContext_DisposeWhenCircuitDisconnected_DoesNotThrow()
	{
		// Arrange
		var module = Substitute.For<IJSObjectReference>();
		module.DisposeAsync().Returns(ValueTask.FromException(new JSDisconnectedException("Circuit disconnected.")));
		var context = new ThreeContext(module, contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => context.DisposeAsync().AsTask());

		// Assert
		exception.ShouldBeNull();
	}

	[Fact]
	public async Task ThreeContext_DisposeWhenModuleAlreadyDisposed_DoesNotThrow()
	{
		// Arrange
		var context = new ThreeContext(new AlreadyDisposedJsObjectReference(), contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => context.DisposeAsync().AsTask());

		// Assert
		exception.ShouldBeNull();
	}

	[Fact]
	public async Task ThreeContext_FlushWithNoPendingOps_MakesNoInteropCall()
	{
		// Arrange
		var module = Substitute.For<IJSObjectReference>();
		var context = new ThreeContext(module, contextId: 1);

		// Act
		await context.FlushAsync();

		// Assert
		module.ReceivedCalls().ShouldBeEmpty();
	}

	[Fact]
	public async Task ThreeContext_FlushWithApplierErrors_RaisesOnError()
	{
		// Arrange
		var applierErrors = new List<ThreeError> { new() { Handle = 1, Member = "roughness", Message = "Invalid roughness value." } };
		var module = Substitute.For<IJSObjectReference>();
		module.InvokeAsync<List<ThreeError>>(Arg.Any<string>(), Arg.Any<object[]>()).Returns(applierErrors);
		var context = new ThreeContext(module, contextId: 1);
		context.Batch.Set(1, "visible", true);
		IReadOnlyList<ThreeError>? raisedErrors = null;
		context.OnError += errors => raisedErrors = errors;

		// Act
		await context.FlushAsync();

		// Assert
		raisedErrors.ShouldBe(applierErrors);
	}

	[Fact]
	public async Task ThreeContext_FlushWithNoApplierErrors_DoesNotRaiseOnError()
	{
		// Arrange
		var module = Substitute.For<IJSObjectReference>();
		module.InvokeAsync<List<ThreeError>>(Arg.Any<string>(), Arg.Any<object[]>()).Returns([]);
		var context = new ThreeContext(module, contextId: 1);
		context.Batch.Set(1, "visible", true);
		var wasOnErrorRaised = false;
		context.OnError += _ => wasOnErrorRaised = true;

		// Act
		await context.FlushAsync();

		// Assert
		wasOnErrorRaised.ShouldBeFalse();
	}

	[Fact]
	public async Task ThreeContext_FlushWhenCircuitDisconnected_DoesNotThrow()
	{
		// Arrange
		var module = Substitute.For<IJSObjectReference>();
		module.InvokeAsync<List<ThreeError>>(Arg.Any<string>(), Arg.Any<object[]>())
			.ThrowsAsync(new JSDisconnectedException("Circuit disconnected."));
		var context = new ThreeContext(module, contextId: 1);
		context.Batch.Set(1, "visible", true);

		// Act
		var exception = await Record.ExceptionAsync(() => context.FlushAsync());

		// Assert
		exception.ShouldBeNull();
	}

	[Fact]
	public async Task ThreeContext_SetActiveSceneWithAnUnattachedCamera_SendsItsCreateOpBeforeTheSetActiveSceneCall()
	{
		// Arrange
		var module = new RecordingJsObjectReference();
		var context = new ThreeContext(module, contextId: 1);
		var scene = new Scene();
		var camera = new PerspectiveCamera();

		// Act
		await context.SetActiveSceneAsync(scene, camera);

		// Assert
		var applyBatchInvocation = module.Invocations.Single(x => x.Identifier == "applyBatch");
		var ops = applyBatchInvocation.Arguments.OfType<IReadOnlyList<ThreeOp>>().Single();

		ops.ShouldContain(x => x.Kind == ThreeOpKind.Create && x.Handle == scene.Handle);
		ops.ShouldContain(x => x.Kind == ThreeOpKind.Create && x.Handle == camera.Handle);
		module.Invocations.IndexOf(applyBatchInvocation)
			.ShouldBeLessThan(module.Invocations.FindIndex(x => x.Identifier == "setActiveScene"));
	}

	[Fact]
	public async Task ThreeContext_SetActiveSceneWithAnAlreadyAttachedGraph_DoesNotReemitItsCreateOps()
	{
		// Arrange
		var module = new RecordingJsObjectReference();
		var context = new ThreeContext(module, contextId: 1);
		var scene = new Scene();
		var camera = new PerspectiveCamera();
		scene.Add(camera);
		context.Attach(scene);
		await context.FlushAsync();

		// Act
		await context.SetActiveSceneAsync(scene, camera);

		// Assert
		module.Invocations.Count(x => x.Identifier == "applyBatch").ShouldBe(1);
	}

	[Fact]
	public async Task ThreeContext_SetActiveSceneWithACameraFromAnotherContext_Throws()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var otherContext = new ThreeContext(new RecordingJsObjectReference(), contextId: 2);
		var scene = new Scene();
		var camera = new PerspectiveCamera();
		otherContext.Attach(camera);

		// Act
		var exception = await Record.ExceptionAsync(() => context.SetActiveSceneAsync(scene, camera));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task ThreeContext_SetActiveSceneWhenCircuitDisconnected_DoesNotThrow()
	{
		// Arrange
		var context = new ThreeContext(new ThrowingJsObjectReference(), contextId: 1);
		var scene = new Scene();
		var camera = new PerspectiveCamera();

		// Act
		var exception = await Record.ExceptionAsync(() => context.SetActiveSceneAsync(scene, camera));

		// Assert
		exception.ShouldBeNull();
	}
}

/// <summary>
/// Fake <see cref="IJSObjectReference"/> that fails every call with <see cref="JSDisconnectedException"/>.
/// NSubstitute cannot stand in for this: the interop path under test goes through
/// <c>InvokeVoidAsync</c>, which closes over <c>Microsoft.JSInterop.Infrastructure.IJSVoidResult</c> —
/// a type internal to the JSInterop assembly, so a substitute cannot be configured against that
/// generic instantiation from this test project. Implementing the interface directly sidesteps the
/// problem, since the generic type argument never has to be named by the caller.
/// </summary>
internal sealed class ThrowingJsObjectReference : IJSObjectReference
{
	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		return ValueTask.FromException<TValue>(new JSDisconnectedException("Circuit disconnected."));
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		return ValueTask.FromException<TValue>(new JSDisconnectedException("Circuit disconnected."));
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}

/// <summary>
/// Fake <see cref="IJSObjectReference"/> that records every invocation and answers each one with an
/// empty result. Lets a test assert what actually crossed the interop boundary and in what order,
/// which is the only place the ops a context sent can be observed once <c>FlushAsync</c> has drained
/// the batch. Implemented directly rather than substituted for the reason spelled out on
/// <see cref="ThrowingJsObjectReference"/>.
/// </summary>
internal sealed class RecordingJsObjectReference : IJSObjectReference
{
	/// <summary>Every invocation received so far, in the order it arrived.</summary>
	public List<JsInvocation> Invocations { get; } = [];

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		Invocations.Add(new JsInvocation { Identifier = identifier, Arguments = args ?? [] });
		if (typeof(TValue) == typeof(List<ThreeError>))
		{
			return ValueTask.FromResult((TValue) (object) new List<ThreeError>());
		}

		return default;
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		return InvokeAsync<TValue>(identifier, args);
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}

/// <summary>A single invocation recorded by <see cref="RecordingJsObjectReference"/>.</summary>
internal sealed class JsInvocation
{
	/// <summary>Name of the JavaScript function that was invoked.</summary>
	public required string Identifier { get; init; }

	/// <summary>Arguments the invocation carried.</summary>
	public required object?[] Arguments { get; init; }
}

/// <summary>
/// Fake <see cref="IJSObjectReference"/> that fails every call with <see cref="ObjectDisposedException"/>,
/// standing in for a module reference that was already disposed by a racing <c>ThreeCanvas</c> disposal
/// before <see cref="ThreeContext.DisposeAsync"/> gets to use it. Real
/// <c>Microsoft.JSInterop.Implementation.JSObjectReference</c> throws exactly this from
/// <c>InvokeAsync</c>/<c>InvokeVoidAsync</c> once disposed, per <c>ThrowIfDisposed()</c>.
/// </summary>
internal sealed class AlreadyDisposedJsObjectReference : IJSObjectReference
{
	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		return ValueTask.FromException<TValue>(new ObjectDisposedException(nameof(AlreadyDisposedJsObjectReference)));
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		return ValueTask.FromException<TValue>(new ObjectDisposedException(nameof(AlreadyDisposedJsObjectReference)));
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}
