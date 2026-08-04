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
	private ThreeContext? _threeContext;

	/// <summary>
	/// Imports the interop module and creates the JavaScript-side context on the first render only,
	/// since the canvas element does not exist yet during initialization.
	/// </summary>
	/// <param name="firstRender">Whether this is the first time the component has rendered.</param>
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender)
		{
			return;
		}

		var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
		var contextId = await module.InvokeAsync<int>("createContext", _canvasElement, null);
		_threeContext = new ThreeContext(module, contextId);

		await OnReady.InvokeAsync(_threeContext);
		await _threeContext.FlushAsync();
	}

	/// <summary>
	/// Releases the JavaScript-side context and every three.js object it owns.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		if (_threeContext is not null)
		{
			await _threeContext.DisposeAsync();
		}
	}
}
