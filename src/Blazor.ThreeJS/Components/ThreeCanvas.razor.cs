using Kebechet.Blazor.ThreeJS.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Hosts a three.js renderer inside a <c>&lt;canvas&gt;</c> element. Creates the JavaScript-side
/// context on first render and hands the caller a <see cref="ThreeContext"/> to build a scene with.
/// </summary>
public partial class ThreeCanvas
{
	/// <summary>
	/// Raised once the JavaScript-side context has been created, with the <see cref="ThreeContext"/>
	/// to use for building and flushing the scene.
	/// </summary>
	[Parameter] public EventCallback<ThreeContext> OnReady { get; set; }

	/// <summary>CSS class applied to the underlying <c>&lt;canvas&gt;</c> element.</summary>
	[Parameter] public string? Class { get; set; }

	/// <summary>Inline style applied to the underlying <c>&lt;canvas&gt;</c> element.</summary>
	[Parameter] public string? Style { get; set; }

	private const string ModulePath = "./_content/Kebechet.Blazor.ThreeJS/three-interop.js";

	private ElementReference _canvasElement;
	private IJSObjectReference? _module;
	private ThreeContext? _threeContext;
	private Task? _initializationTask;

	/// <summary>
	/// Starts <see cref="InitializeAsync"/> on the first render only, since the canvas element does
	/// not exist yet during earlier lifecycle stages, and remembers the task so
	/// <see cref="DisposeAsync"/> can wait for it to settle before tearing anything down. Returning
	/// the same task lets the renderer's own lifecycle-exception handling see initialization failures
	/// exactly as it would have before this method existed.
	/// </summary>
	/// <param name="firstRender">Whether this is the first time the component has rendered.</param>
	protected override Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender)
		{
			return Task.CompletedTask;
		}

		_initializationTask = InitializeAsync();
		return _initializationTask;
	}

	/// <summary>
	/// Imports the interop module, creates the JavaScript-side context, and hands it to
	/// <see cref="OnReady"/>. Runs to a terminal state unconditionally — it does not check for a
	/// disposal racing it — so <see cref="DisposeAsync"/> can rely on <c>_module</c>/<c>_threeContext</c>
	/// always reflecting a fully-settled outcome (nothing created, only the module, or a complete
	/// <see cref="ThreeContext"/>) once this task completes, rather than a state caught mid-creation.
	/// The module reference is stored before <c>createContext</c> runs so it is still releasable if
	/// that call throws — it does when WebGL is unavailable or the browser's live-context limit is
	/// reached.
	/// </summary>
	private async Task InitializeAsync()
	{
		var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
		_module = module;

		var contextId = await module.InvokeAsync<int>("createContext", _canvasElement, null);
		var threeContext = new ThreeContext(module, contextId);
		_threeContext = threeContext;

		await OnReady.InvokeAsync(threeContext);
		await threeContext.FlushAsync();
	}

	/// <summary>
	/// Waits for <see cref="InitializeAsync"/> to reach a terminal state — whether or not it started,
	/// and regardless of how it ended — then releases whatever it left behind: the full
	/// <see cref="ThreeContext"/> if it got that far, otherwise just the module reference, otherwise
	/// nothing. Because teardown always runs after initialization has settled, the JavaScript-side
	/// context — its WebGL renderer, <c>ResizeObserver</c>, and render loop — is torn down through
	/// <c>disposeContext</c> whenever <c>createContext</c> ever returned successfully, even if
	/// disposal was requested while that call was still in flight.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		if (_initializationTask is not null)
		{
			try
			{
				await _initializationTask;
			}
			catch (Exception)
			{
				// Already surfaced through the same task OnAfterRenderAsync returned to the
				// renderer; disposal only needs initialization to have finished, not succeeded.
			}
		}

		if (_threeContext is not null)
		{
			await DisposeThreeContextAsync();
			return;
		}

		await DisposeModuleAsync();
	}

	/// <summary>
	/// Disposes the context, which also releases the module reference it owns.
	/// </summary>
	private async ValueTask DisposeThreeContextAsync()
	{
		var threeContext = _threeContext;
		_threeContext = null;
		_module = null;
		if (threeContext is not null)
		{
			await threeContext.DisposeAsync();
		}
	}

	/// <summary>
	/// Releases the module reference directly, for the window in which it has been imported but no
	/// <see cref="ThreeContext"/> owns it yet.
	/// </summary>
	private async ValueTask DisposeModuleAsync()
	{
		var module = _module;
		_module = null;
		if (module is null)
		{
			return;
		}

		try
		{
			await module.DisposeAsync();
		}
		catch (JSDisconnectedException)
		{
			// The circuit is already gone, so the module reference died with it.
		}
	}
}
