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
	private bool _isDisposed;

	/// <summary>
	/// Imports the interop module and creates the JavaScript-side context on the first render only,
	/// since the canvas element does not exist yet during initialization. The module reference is
	/// stored before <c>createContext</c> runs so it is still releasable if that call throws — it
	/// does when WebGL is unavailable or the browser's live-context limit is reached. Disposal can
	/// also land while this method is awaiting, so <c>_isDisposed</c> is re-checked at every
	/// suspension point: a context created after the component is gone would otherwise keep a WebGL
	/// context and a requestAnimationFrame loop alive for the lifetime of the page.
	/// </summary>
	/// <param name="firstRender">Whether this is the first time the component has rendered.</param>
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender)
		{
			return;
		}

		var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
		_module = module;
		if (_isDisposed)
		{
			await DisposeModuleAsync();
			return;
		}

		var contextId = await module.InvokeAsync<int>("createContext", _canvasElement, null);
		var threeContext = new ThreeContext(module, contextId);
		_threeContext = threeContext;
		if (_isDisposed)
		{
			await DisposeThreeContextAsync();
			return;
		}

		await OnReady.InvokeAsync(threeContext);
		await threeContext.FlushAsync();
	}

	/// <summary>
	/// Releases the JavaScript-side context and every three.js object it owns. Safe to call while
	/// <see cref="OnAfterRenderAsync"/> is still awaiting: the flag it sets makes that continuation
	/// clean up whatever it has already created instead of orphaning it.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		_isDisposed = true;
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
