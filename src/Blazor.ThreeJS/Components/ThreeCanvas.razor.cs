using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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

	/// <summary>
	/// A scene written as a component tree — a camera, some lights, some meshes. Supplying it builds
	/// and renders that scene; leaving it out leaves the canvas entirely to
	/// <see cref="OnReady"/> and the imperative API, which is what it was before this existed.
	/// <para>
	/// The two compose: the declarative scene is built and made active before <see cref="OnReady"/>
	/// fires, so a handler receives a context whose scene is already running — enough to flush, to
	/// subscribe to <see cref="ThreeContext.OnError"/>, or to attach <c>OrbitControls</c>.
	/// </para>
	/// <para>
	/// ⚠️ The context is <b>not</b> a route to the declarative scene itself: the scene root is internal,
	/// and <see cref="ThreeContext.SetActiveSceneAsync"/> would <b>replace</b> the scene the markup
	/// built rather than let a caller into it. To drive a declaratively built object imperatively, put
	/// an <c>@ref</c> on the component that owns it and use its <c>Object</c> property — the mirrored
	/// object itself, built by the time this callback runs. Note that a parameter written in the markup
	/// is re-applied on every render, so leave the parameter off for state C# means to drive.
	/// </para>
	/// </summary>
	[Parameter] public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Raised when the JavaScript-side context could not be created, with the exception that stopped
	/// it. <see cref="OnReady"/> never fires in that case, so this is the callback that says why.
	/// <para>
	/// Handling it is optional: the component already replaces the canvas with a message naming the
	/// failure, which is the part that matters to whoever is looking at the page. Subscribe when the
	/// page has its own state to correct — controls to disable, a retry to offer, telemetry to send.
	/// </para>
	/// </summary>
	[Parameter] public EventCallback<Exception> OnInitializationFailed { get; set; }

	private const string ModulePath = "./_content/Kebechet.Blazor.ThreeJS/three-interop.js";

	private ElementReference _canvasElement;
	private IJSObjectReference? _module;
	private ThreeContext? _threeContext;
	private Task? _initializationTask;
	private bool _isDisposed;

	/// <summary>
	/// What stopped <see cref="CreateContextAsync"/>, once something has. Rendered in place of the
	/// canvas, because a canvas with no renderer behind it is indistinguishable from one whose scene
	/// simply has nothing in it — and that silence is what turns a failed renderer into a page whose
	/// every control does nothing for no visible reason.
	/// </summary>
	private Exception? _initializationError;

	/// <summary>
	/// The declarative scene's root, present only while <see cref="ChildContent"/> is supplied. Captured
	/// as the component is rendered, so it is set by the time the first render has been applied.
	/// </summary>
	private ThreeSceneRoot? _sceneRoot;

	/// <summary>
	/// The declarative scene's root, as a fragment the markup renders, or <see langword="null"/> when no
	/// scene was written. Built here rather than written as a tag because <see cref="ThreeSceneRoot"/> is
	/// internal — it is machinery this component owns, not a component a consumer writes — and Razor only
	/// resolves a tag to a public type.
	/// </summary>
	private RenderFragment? _declarativeScene
	{
		get
		{
			if (ChildContent is null)
			{
				return null;
			}

			return BuildSceneRoot;
		}
	}

	/// <summary>Renders the scene root around the consumer's markup, capturing it for the build below.</summary>
	/// <param name="builder">Builder for this component's render tree.</param>
	private void BuildSceneRoot(RenderTreeBuilder builder)
	{
		builder.OpenComponent<ThreeSceneRoot>(0);
		builder.AddAttribute(1, "ChildContent", ChildContent);
		builder.AddComponentReferenceCapture(2, x => _sceneRoot = (ThreeSceneRoot) x);
		builder.CloseComponent();
	}

	/// <summary>
	/// What lets the browser call back into this component, handed to <c>createContext</c> so the
	/// applier can report a pointer hit. Released by <see cref="DisposeAsync"/>: an undisposed one
	/// keeps this component, its context and its whole scene graph alive for as long as the circuit
	/// lives, because the JS-side reference table holds it.
	/// </summary>
	private DotNetObjectReference<ThreeCanvas>? _selfReference;

	/// <summary>
	/// Creates the JavaScript-side context on the first render only, since the canvas element does not
	/// exist yet during earlier lifecycle stages, then — unless disposal raced it — hands the result to
	/// <see cref="OnReady"/> and flushes it. <see cref="_initializationTask"/> covers only
	/// <see cref="CreateContextAsync"/>, not this method's own continuation: that keeps it bounded by
	/// framework-owned JS interop (which reliably faults with <see cref="JSDisconnectedException"/> on
	/// a dead circuit) rather than by arbitrary, possibly-unbounded consumer code in
	/// <see cref="OnReady"/>, so <see cref="DisposeAsync"/> can safely await it without risking a hang.
	/// <para>
	/// A declarative scene is built here too, before <see cref="OnReady"/>. This is the moment the whole
	/// component tree has rendered, so the attach it triggers reaches a complete object graph and emits
	/// it in dependency order — a geometry's create op ahead of the mesh that holds it — rather than
	/// piecemeal as each component initialized. It sits in this method's continuation rather than in
	/// <see cref="_initializationTask"/> for the same reason <see cref="OnReady"/> does.
	/// </para>
	/// </summary>
	/// <param name="firstRender">Whether this is the first time the component has rendered.</param>
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (!firstRender)
		{
			return;
		}

		_initializationTask = CreateContextAsync();

		try
		{
			await _initializationTask;
		}
		catch (Exception exception)
		{
			await ReportInitializationFailureAsync(exception);
			return;
		}

		var threeContext = _threeContext;
		if (_isDisposed || threeContext is null)
		{
			return;
		}

		if (_sceneRoot is not null)
		{
			await _sceneRoot.BuildAsync(threeContext);
		}

		await OnReady.InvokeAsync(threeContext);
		await threeContext.FlushAsync();
	}

	/// <summary>
	/// Imports the interop module and creates the JavaScript-side context, assigning
	/// <see cref="_threeContext"/> once <c>createContext</c> returns. Deliberately does not check for
	/// a disposal racing it and does not touch <see cref="OnReady"/> — both belong to
	/// <see cref="OnAfterRenderAsync"/>'s continuation, which resumes after this task completes. That
	/// split is what lets <see cref="DisposeAsync"/> await this task unconditionally: every step in it
	/// is framework JS interop, so a dead circuit faults it rather than hanging it. The module
	/// reference is stored before <c>createContext</c> runs so it is still releasable if that call
	/// throws — it does when WebGL is unavailable or the browser's live-context limit is reached. The
	/// reference to this component is stored before the call for the same reason.
	/// </summary>
	/// <summary>
	/// Shows a failed <c>createContext</c> where the canvas was and hands it to
	/// <see cref="OnInitializationFailed"/>, rather than letting it reach the renderer as an unhandled
	/// exception.
	/// <para>
	/// ⚠️ Swallowing it is the point, and it is a narrow swallow: this covers the package's own
	/// initialization, not a caller's <see cref="OnReady"/>, which still faults the way any other
	/// component's callback does. Left unhandled, this one surfaces as the framework's generic error
	/// banner — the same wording for a dead renderer as for any other fault, naming neither the canvas
	/// nor the cause, while every control the page wired to the scene goes quietly inert.
	/// </para>
	/// <para>
	/// Nothing is reported for a disposed component or a dead circuit: there is no page left to tell.
	/// </para>
	/// </summary>
	/// <param name="exception">What <see cref="CreateContextAsync"/> threw.</param>
	private async Task ReportInitializationFailureAsync(Exception exception)
	{
		if (_isDisposed || exception is JSDisconnectedException)
		{
			return;
		}

		_initializationError = exception;
		StateHasChanged();

		await OnInitializationFailed.InvokeAsync(exception);
	}

	private async Task CreateContextAsync()
	{
		var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
		_module = module;

		_selfReference = DotNetObjectReference.Create(this);
		var contextId = await module.InvokeAsync<int>("createContext", _canvasElement, _selfReference);
		_threeContext = new ThreeContext(module, contextId);
	}

	/// <summary>
	/// Receives a pointer hit from the JavaScript applier and raises it on the object the ray met.
	/// Public and <c>[JSInvokable]</c> because the browser has to be able to reach it; it is not part
	/// of the surface a consumer calls — subscribe to <see cref="Object3D.OnClick"/> instead.
	/// <para>
	/// The flush afterwards is what lets a handler be an ordinary synchronous <see cref="Action"/>: it
	/// mutates the scene graph and the changes go out with no awaiting on its part. It costs nothing
	/// when the handler changed nothing, because a flush with an empty batch makes no interop call.
	/// </para>
	/// <para>
	/// No dispatching is done here, and none is needed. A JS-to-.NET call arrives on the renderer's
	/// synchronization context on every hosting model this package supports — the circuit's on Blazor
	/// Server, the single thread on WebAssembly, and the UI thread on MAUI Hybrid, where
	/// <c>BlazorWebView</c> marshals JS interop onto it — so a handler may touch component state and
	/// call <c>StateHasChanged</c> directly.
	/// </para>
	/// <para>
	/// A hit that arrives after disposal raises nothing: the context is detached before its teardown
	/// begins, and clears its own target table on top of that.
	/// </para>
	/// </summary>
	/// <param name="handle">Handle of the object the ray met.</param>
	/// <param name="x">X coordinate, in world space, of the point where the ray met it.</param>
	/// <param name="y">Y coordinate, in world space, of the point where the ray met it.</param>
	/// <param name="z">Z coordinate, in world space, of the point where the ray met it.</param>
	/// <param name="distance">Distance in world units from the camera to that point.</param>
	[JSInvokable]
	public async Task DispatchPointerEventAsync(int handle, float x, float y, float z, float distance)
	{
		var threeContext = _threeContext;
		if (threeContext is null)
		{
			return;
		}

		threeContext.DispatchPointerEvent(handle, new ThreePointerEvent
		{
			Point = new Vector3(x, y, z),
			Distance = distance
		});

		await threeContext.FlushAsync();
	}

	/// <summary>
	/// Waits for <see cref="CreateContextAsync"/> to reach a terminal state — whether or not it
	/// started, and regardless of how it ended — then releases whatever it left behind: the full
	/// <see cref="ThreeContext"/> if it got that far, otherwise just the module reference, otherwise
	/// nothing, and in every one of those cases <see cref="_selfReference"/> if it was created.
	/// Because teardown always runs after that task has settled, the JavaScript-side context —
	/// its WebGL renderer, <c>ResizeObserver</c>, and render loop — is torn down through
	/// <c>disposeContext</c> whenever <c>createContext</c> ever returned successfully, even if disposal
	/// was requested while that call was still in flight.
	/// <para>
	/// Setting <see cref="_isDisposed"/> first, before awaiting anything, is what lets
	/// <see cref="OnAfterRenderAsync"/>'s continuation see disposal even if its own await of
	/// <see cref="_initializationTask"/> happens to resume before this method's does — the flag flips
	/// synchronously the instant this method is called, with no dependency on continuation ordering. If
	/// disposal instead lands after <see cref="_threeContext"/> already exists and while
	/// <see cref="OnReady"/> is still running, this method proceeds immediately: the still-running
	/// <see cref="OnAfterRenderAsync"/> continuation may then call <c>FlushAsync</c> against a context
	/// whose module this call just disposed, which is exactly what <see cref="ThreeContext.FlushAsync"/>'s
	/// own <see cref="ObjectDisposedException"/> guard is for.
	/// </para>
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		_isDisposed = true;
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

		// The reference goes last, in a finally, so it is released however the teardown ends and only
		// once the JavaScript side can no longer reach it: disposeContext takes the click listener off
		// the canvas, and until that has run a pointer hit could still be on its way to it.
		try
		{
			if (_threeContext is not null)
			{
				await DisposeThreeContextAsync();
				return;
			}

			await DisposeModuleAsync();
		}
		finally
		{
			var selfReference = _selfReference;
			_selfReference = null;
			selfReference?.Dispose();
		}
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
