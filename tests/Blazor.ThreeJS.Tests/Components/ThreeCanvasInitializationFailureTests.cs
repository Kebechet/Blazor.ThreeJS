using Bunit;
using Kebechet.Blazor.ThreeJS.Components;
using Kebechet.Blazor.ThreeJS.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Components;

/// <summary>
/// What a canvas does when the renderer behind it never starts. <c>createContext</c> throws for
/// reasons a page cannot control — no WebGPU or WebGL in this browser, the live-context limit already
/// reached — and until this was handled, the failure surfaced only as the framework's generic error
/// banner while the canvas stayed blank and every control the page had wired to the scene did nothing
/// at all, silently, for no visible reason.
/// </summary>
public class ThreeCanvasInitializationFailureTests
{
	private const string FailureMessage = "WebGPU is unavailable and no WebGL fallback could be created.";

	[Fact]
	public void ThreeCanvas_CreateContextThrows_ShowsTheFailureWhereTheCanvasWas()
	{
		// Arrange
		using var bunitContext = new BunitContext();
		bunitContext.Services.AddSingleton<IJSRuntime>(new FailingCreateContextJsRuntime(new InvalidOperationException(FailureMessage)));

		// Act
		var rendered = bunitContext.Render<ThreeCanvas>(parameters => parameters.Add(x => x.Style, "width: 640px;"));

		// Assert
		rendered.WaitForAssertion(() =>
		{
			var failure = rendered.Find("[data-testid=three-canvas-error]");
			failure.TextContent.ShouldContain(FailureMessage);
		});

		rendered.Markup.ShouldNotContain("<canvas", Case.Insensitive);
		rendered.Find("[data-testid=three-canvas-error]").GetAttribute("style").ShouldBe("width: 640px;");
	}

	[Fact]
	public void ThreeCanvas_CreateContextThrows_RaisesOnInitializationFailedAndNeverFiresOnReady()
	{
		// Arrange
		using var bunitContext = new BunitContext();
		var failure = new InvalidOperationException(FailureMessage);
		bunitContext.Services.AddSingleton<IJSRuntime>(new FailingCreateContextJsRuntime(failure));
		Exception? reported = null;
		var readyCallCount = 0;

		// Act
		var rendered = bunitContext.Render<ThreeCanvas>(parameters => parameters
			.Add(x => x.OnInitializationFailed, exception => reported = exception)
			.Add(x => x.OnReady, _ => readyCallCount++));

		// Assert
		rendered.WaitForAssertion(() => reported.ShouldNotBeNull());
		reported.ShouldBeSameAs(failure);
		readyCallCount.ShouldBe(0);
	}

	/// <summary>
	/// A dead circuit is not a canvas failure: there is no page left to tell, and saying so would put a
	/// renderer's obituary on a screen nobody is looking at.
	/// </summary>
	[Fact]
	public void ThreeCanvas_CircuitDiesDuringCreateContext_ReportsNothing()
	{
		// Arrange
		using var bunitContext = new BunitContext();
		bunitContext.Services.AddSingleton<IJSRuntime>(new FailingCreateContextJsRuntime(new JSDisconnectedException("The circuit is gone.")));
		Exception? reported = null;

		// Act
		var rendered = bunitContext.Render<ThreeCanvas>(parameters => parameters.Add(x => x.OnInitializationFailed, exception => reported = exception));

		// Assert
		rendered.WaitForState(() => rendered.Instance is not null);
		reported.ShouldBeNull();
		rendered.FindAll("[data-testid=three-canvas-error]").ShouldBeEmpty();
	}

	/// <summary>The path that already worked, so the failure branch cannot quietly take it over.</summary>
	[Fact]
	public void ThreeCanvas_CreateContextSucceeds_RendersTheCanvasAndSaysNothingAboutFailure()
	{
		// Arrange
		using var bunitContext = new BunitContext();
		bunitContext.Services.AddSingleton<IJSRuntime>(new RecordingThreeJsRuntime(new RecordingThreeJsModule()));
		var readyCallCount = 0;

		// Act
		var rendered = bunitContext.Render<ThreeCanvas>(parameters => parameters.Add(x => x.OnReady, _ => readyCallCount++));

		// Assert
		rendered.WaitForAssertion(() => readyCallCount.ShouldBe(1));
		rendered.Markup.ShouldContain("<canvas", Case.Insensitive);
		rendered.FindAll("[data-testid=three-canvas-error]").ShouldBeEmpty();
	}
}

/// <summary>
/// Fake <see cref="IJSRuntime"/> whose interop module fails the <c>createContext</c> call with a given
/// exception, standing in for a browser that cannot give this canvas a renderer.
/// </summary>
internal sealed class FailingCreateContextJsRuntime : IJSRuntime
{
	private readonly FailingCreateContextModule _module;

	public FailingCreateContextJsRuntime(Exception failure)
	{
		_module = new FailingCreateContextModule(failure);
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

/// <summary>The imported interop module, for a browser where creating the context throws.</summary>
internal sealed class FailingCreateContextModule : IJSObjectReference
{
	private readonly Exception _failure;

	public FailingCreateContextModule(Exception failure)
	{
		_failure = failure;
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		if (identifier == "createContext")
		{
			throw _failure;
		}

		throw new NotSupportedException($"No fake behaviour configured for interop call '{identifier}'.");
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
