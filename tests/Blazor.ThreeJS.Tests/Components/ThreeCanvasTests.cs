using System.Reflection;
using Kebechet.Blazor.ThreeJS.Components;
using Kebechet.Blazor.ThreeJS.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Components;

public class ThreeCanvasTests
{
	private static readonly PropertyInfo JsRuntimeProperty = typeof(ThreeCanvas)
		.GetProperty("_jsRuntime", BindingFlags.NonPublic | BindingFlags.Instance)
		?? throw new InvalidOperationException("ThreeCanvas no longer has an injected '_jsRuntime' property.");

	private static readonly MethodInfo OnAfterRenderAsyncMethod = typeof(ThreeCanvas)
		.GetMethod("OnAfterRenderAsync", BindingFlags.NonPublic | BindingFlags.Instance)
		?? throw new InvalidOperationException("ThreeCanvas no longer declares a protected 'OnAfterRenderAsync' method.");

	[Fact]
	public async Task ThreeCanvas_DisposeWhileCreateContextAwaits_TearsDownJsContextExactlyOnce()
	{
		// Arrange
		var createContextGate = new TaskCompletionSource<int>();
		var module = new GatedCreateContextJsObjectReference(createContextGate);
		var canvas = new ThreeCanvas();
		JsRuntimeProperty.SetValue(canvas, new SingleModuleJsRuntime(module));
		var onAfterRenderResult = OnAfterRenderAsyncMethod.Invoke(canvas, [true])
			?? throw new InvalidOperationException("OnAfterRenderAsync returned null instead of a Task.");
		var onAfterRenderTask = (Task) onAfterRenderResult;

		// Act
		var disposeTask = canvas.DisposeAsync();
		createContextGate.SetResult(1);
		var disposeException = await Record.ExceptionAsync(async () => await disposeTask);
		var initializationException = await Record.ExceptionAsync(() => onAfterRenderTask);

		// Assert
		disposeException.ShouldBeNull();
		initializationException.ShouldBeNull();
		module.DisposeContextCallCount.ShouldBe(1);
		module.DisposeCallCount.ShouldBe(1);
	}

	[Fact]
	public async Task ThreeCanvas_DisposeWhileOnReadyHangs_CompletesAndTearsDownJsContextOnce()
	{
		// Arrange
		var createContextGate = new TaskCompletionSource<int>();
		createContextGate.SetResult(1);
		var module = new GatedCreateContextJsObjectReference(createContextGate);
		var canvas = new ThreeCanvas();
		JsRuntimeProperty.SetValue(canvas, new SingleModuleJsRuntime(module));
		var onReadyGate = new TaskCompletionSource();
#pragma warning disable BL0005 // Test drives ThreeCanvas outside the normal parameter-binding pipeline, same as the _jsRuntime field above.
		canvas.OnReady = EventCallback.Factory.Create<ThreeContext>(this, threeContext =>
		{
			threeContext.Batch.Set(1, "visible", true);
			return onReadyGate.Task;
		});
#pragma warning restore BL0005
		var onAfterRenderResult = OnAfterRenderAsyncMethod.Invoke(canvas, [true])
			?? throw new InvalidOperationException("OnAfterRenderAsync returned null instead of a Task.");
		var onAfterRenderTask = (Task) onAfterRenderResult;

		// Act
		var disposeTask = canvas.DisposeAsync().AsTask();
		var firstCompletedTask = await Task.WhenAny(disposeTask, Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken));

		// Assert
		firstCompletedTask.ShouldBeSameAs(disposeTask);
		var disposeException = await Record.ExceptionAsync(() => disposeTask);
		disposeException.ShouldBeNull();
		module.DisposeContextCallCount.ShouldBe(1);
		onAfterRenderTask.IsCompleted.ShouldBeFalse();

		onReadyGate.SetResult();
		var initializationException = await Record.ExceptionAsync(() => onAfterRenderTask);
		initializationException.ShouldBeNull();
	}
}

/// <summary>
/// Fake <see cref="IJSRuntime"/> whose only configured behaviour is returning a fixed
/// <see cref="IJSObjectReference"/> for the module import call, so <c>ThreeCanvas</c>'s
/// <c>OnAfterRenderAsync</c> can be driven under test without a full DI/JS interop host.
/// </summary>
internal sealed class SingleModuleJsRuntime : IJSRuntime
{
	private readonly IJSObjectReference _module;

	public SingleModuleJsRuntime(IJSObjectReference module)
	{
		_module = module;
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		return ValueTask.FromResult((TValue) (object) _module);
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		return InvokeAsync<TValue>(identifier, args);
	}
}

/// <summary>
/// Fake <see cref="IJSObjectReference"/> standing in for the imported interop module. Its
/// <c>createContext</c> call does not complete until the <see cref="TaskCompletionSource{TResult}"/>
/// passed to the constructor is signalled, so a test can start disposing the owning <c>ThreeCanvas</c>
/// while that call is still in flight and then deterministically release it — reproducing the exact
/// interleaving traced in the Plan 1.5 disposal race. It also tracks <c>disposeContext</c> calls, so a
/// test can assert the JavaScript-side context is actually torn down rather than merely that no
/// exception escaped. NSubstitute is not used here for the same reason <c>ThrowingJsObjectReference</c>
/// in <c>ThreeContextTests</c> is hand-written: matching real
/// <c>Microsoft.JSInterop.Implementation.JSObjectReference</c> semantics (an in-flight call completes
/// normally even after the reference is disposed; only a call started after disposal throws
/// <see cref="ObjectDisposedException"/>) is not something a substitute can be configured to do.
/// </summary>
internal sealed class GatedCreateContextJsObjectReference : IJSObjectReference
{
	private readonly TaskCompletionSource<int> _createContextGate;

	public int DisposeContextCallCount { get; private set; }

	public int DisposeCallCount { get; private set; }

	public GatedCreateContextJsObjectReference(TaskCompletionSource<int> createContextGate)
	{
		_createContextGate = createContextGate;
	}

	public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		if (DisposeCallCount > 0)
		{
			throw new ObjectDisposedException(nameof(GatedCreateContextJsObjectReference));
		}

		switch (identifier)
		{
			case "createContext":
				var contextId = await _createContextGate.Task;
				return (TValue) (object) contextId;
			case "disposeContext":
				DisposeContextCallCount++;
				return default!;
			default:
				throw new NotSupportedException($"No fake behaviour configured for interop call '{identifier}'.");
		}
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		return InvokeAsync<TValue>(identifier, args);
	}

	public ValueTask DisposeAsync()
	{
		DisposeCallCount++;
		return ValueTask.CompletedTask;
	}
}
