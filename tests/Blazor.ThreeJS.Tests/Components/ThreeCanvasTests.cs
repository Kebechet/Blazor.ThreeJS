using System.Reflection;
using Kebechet.Blazor.ThreeJS.Components;
using Microsoft.JSInterop;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Components;

public class ThreeCanvasTests
{
	private static readonly PropertyInfo JsRuntimeProperty = typeof(ThreeCanvas)
		.GetProperty("_jsRuntime", BindingFlags.NonPublic | BindingFlags.Instance)!;

	private static readonly MethodInfo OnAfterRenderAsyncMethod = typeof(ThreeCanvas)
		.GetMethod("OnAfterRenderAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;

	[Fact]
	public async Task ThreeCanvas_DisposeWhileCreateContextAwaits_DoesNotThrowAndDisposesModuleOnce()
	{
		// Arrange
		var createContextGate = new TaskCompletionSource<int>();
		var module = new GatedCreateContextJsObjectReference(createContextGate);
		var canvas = new ThreeCanvas();
		JsRuntimeProperty.SetValue(canvas, new SingleModuleJsRuntime(module));
		var onAfterRenderTask = (Task) OnAfterRenderAsyncMethod.Invoke(canvas, [true])!;

		// Act
		await canvas.DisposeAsync();
		createContextGate.SetResult(1);
		var exception = await Record.ExceptionAsync(() => onAfterRenderTask);

		// Assert
		exception.ShouldBeNull();
		module.DisposeCallCount.ShouldBe(1);
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
/// passed to the constructor is signalled, so a test can dispose the owning <c>ThreeCanvas</c> while
/// that call is still in flight and then deterministically release it — reproducing the exact
/// interleaving traced in the Plan 1.5 disposal race. NSubstitute is not used here for the same
/// reason <c>ThrowingJsObjectReference</c> in <c>ThreeContextTests</c> is hand-written: matching real
/// <c>Microsoft.JSInterop.Implementation.JSObjectReference</c> semantics (an in-flight call completes
/// normally even after the reference is disposed; disposal itself is idempotent) is not something a
/// substitute can be configured to do.
/// </summary>
internal sealed class GatedCreateContextJsObjectReference : IJSObjectReference
{
	private readonly TaskCompletionSource<int> _createContextGate;

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

		if (identifier != "createContext")
		{
			throw new NotSupportedException($"No fake behaviour configured for interop call '{identifier}'.");
		}

		var contextId = await _createContextGate.Task;
		return (TValue) (object) contextId;
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
