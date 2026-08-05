using Bunit;
using Kebechet.Blazor.ThreeJS.Components;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Blazor.ThreeJS.Tests.Components;

/// <summary>
/// Drives a declarative scene through a real Blazor renderer and records every batch that reaches the
/// JavaScript side.
/// <para>
/// bUnit rather than a hand-driven lifecycle, because the questions this file asks are about the
/// renderer's own behaviour — when a cascading value reaches a component, when a re-render actually
/// happens, and in which order a removed subtree is disposed. Substituting for any of that would make
/// the answers the test's own rather than Blazor's.
/// </para>
/// </summary>
internal sealed class DeclarativeSceneHost : IAsyncDisposable
{
	private readonly BunitContext _bunitContext = new();
	private IRenderedComponent<DeclarativeSceneTrigger>? _rootComponent;
	private ThreeCanvas? _canvas;

	/// <summary>Every batch that reached the JavaScript side, and how many calls it took.</summary>
	public RecordingThreeJsModule Module { get; } = new();

	/// <summary>The scene to render inside the canvas. Left null, the canvas renders no scene at all.</summary>
	public RenderFragment? Scene { get; set; }

	/// <summary>Position a test scene gives its mesh, so a test can change one parameter and nothing else.</summary>
	public Vector3? MeshPosition { get; set; }

	/// <summary>Whether a test scene renders its optional subtree, so a test can add or remove one.</summary>
	public bool IsSubtreeRendered { get; set; }

	/// <summary>Width a test scene gives its box geometry, which three.js cannot change after construction.</summary>
	public float BoxWidth { get; set; } = 1f;

	/// <summary>The component wrapping the canvas, which is what a test re-renders through.</summary>
	public IRenderedComponent<DeclarativeSceneTrigger> RootComponent
	{
		get
		{
			return _rootComponent ?? throw new InvalidOperationException($"Nothing has been rendered yet — call {nameof(Render)} first.");
		}
	}

	/// <summary>The canvas under test.</summary>
	public ThreeCanvas Canvas
	{
		get
		{
			return _canvas ?? throw new InvalidOperationException($"Nothing has been rendered yet — call {nameof(Render)} first.");
		}
	}

	/// <summary>Registers the recording runtime in place of bUnit's own, so batches can be read back as ops.</summary>
	public DeclarativeSceneHost()
	{
		_bunitContext.Services.AddSingleton<IJSRuntime>(new RecordingThreeJsRuntime(Module));
	}

	/// <summary>Renders the canvas with whatever <see cref="Scene"/> currently holds.</summary>
	public void Render()
	{
		_rootComponent = _bunitContext.Render<DeclarativeSceneTrigger>(builder =>
		{
			builder.OpenComponent<DeclarativeSceneTrigger>(0);
			builder.AddAttribute(1, "ChildContent", (RenderFragment) BuildCanvas);
			builder.CloseComponent();
		});
	}

	/// <summary>
	/// Re-renders everything without changing a parameter, which is what a parent re-render does to a
	/// scene graph: child content is a <see cref="RenderFragment"/>, so nothing below is skipped.
	/// </summary>
	/// <returns>A task that completes once the render and its after-render work have finished.</returns>
	public Task ForceRenderAsync()
	{
		return RootComponent.InvokeAsync(() => RootComponent.Instance.ForceRender());
	}

	/// <summary>
	/// Waits for the renderer to catch an exception thrown while rendering, which is where a failure
	/// raised by a lifecycle method ends up rather than on the task that triggered the render.
	/// </summary>
	/// <returns>The exception, or <see langword="null"/> when none was raised.</returns>
	public async Task<Exception?> WaitForUnhandledExceptionAsync()
	{
		var unhandledException = _bunitContext.Renderer.UnhandledException;
		var firstToFinish = await Task.WhenAny(unhandledException, Task.Delay(TimeSpan.FromSeconds(5)));
		if (firstToFinish != unhandledException)
		{
			return null;
		}

		return await unhandledException;
	}

	/// <summary>Tears the rendered tree down, so the disposal path runs under the test's control.</summary>
	/// <returns>A task that completes once bUnit has disposed everything it rendered.</returns>
	public async ValueTask DisposeAsync()
	{
		await _bunitContext.DisposeAsync();
	}

	/// <summary>Renders the canvas, capturing it so a test can reach its JavaScript-invokable surface.</summary>
	/// <param name="builder">Builder for the trigger's child content.</param>
	private void BuildCanvas(RenderTreeBuilder builder)
	{
		builder.OpenComponent<ThreeCanvas>(0);
		if (Scene is not null)
		{
			builder.AddAttribute(1, "ChildContent", Scene);
		}

		builder.AddComponentReferenceCapture(2, x => _canvas = (ThreeCanvas) x);
		builder.CloseComponent();
	}
}

/// <summary>
/// A component whose only job is to re-render its child content on demand, standing in for the
/// consumer component a scene is written inside.
/// </summary>
internal sealed class DeclarativeSceneTrigger : ComponentBase
{
	/// <summary>The canvas and its scene.</summary>
	[Parameter] public RenderFragment? ChildContent { get; set; }

	/// <summary>Re-renders, with nothing about the component changed.</summary>
	public void ForceRender()
	{
		StateHasChanged();
	}

	/// <summary>Renders the child content directly, adding nothing of its own.</summary>
	/// <param name="builder">Builder for this component's render tree.</param>
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.AddContent(0, ChildContent);
	}
}

/// <summary>
/// Hands out one <see cref="RecordingThreeJsModule"/> for the interop module import, which is the only
/// call a <c>ThreeCanvas</c> makes on the runtime itself.
/// </summary>
internal sealed class RecordingThreeJsRuntime : IJSRuntime
{
	private readonly RecordingThreeJsModule _module;

	public RecordingThreeJsRuntime(RecordingThreeJsModule module)
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
/// Stands in for the imported interop module and keeps every op that was sent through it, so a test
/// can assert both what the batch contained and — for the paths that must cost nothing — that no call
/// was made at all.
/// </summary>
internal sealed class RecordingThreeJsModule : IJSObjectReference
{
	/// <summary>Ops of every batch applied so far, one entry per <c>applyBatch</c> call.</summary>
	public List<IReadOnlyList<ThreeOp>> AppliedBatches { get; } = [];

	/// <summary>Every call made on this module, whatever it was for.</summary>
	public int CallCount { get; private set; }

	/// <summary>How many times a scene was made active.</summary>
	public int SetActiveSceneCallCount { get; private set; }

	/// <summary>Every op sent so far, in order, across all batches.</summary>
	public IReadOnlyList<ThreeOp> AllOps
	{
		get { return AppliedBatches.SelectMany(x => x).ToList(); }
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		CallCount++;
		switch (identifier)
		{
			case "createContext":
				return ValueTask.FromResult((TValue) (object) 1);
			case "applyBatch":
				AppliedBatches.Add(args?.OfType<IReadOnlyList<ThreeOp>>().FirstOrDefault() ?? []);
				return ValueTask.FromResult((TValue) (object) new ThreeBatchResponse());
			case "setActiveScene":
				SetActiveSceneCallCount++;
				return ValueTask.FromResult<TValue>(default!);
			case "disposeContext":
				return ValueTask.FromResult<TValue>(default!);
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
		return ValueTask.CompletedTask;
	}
}
