using System.Reflection;
using Blazor.ThreeJS.Tests.Components;
using Kebechet.Blazor.ThreeJS.Components;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.JSInterop;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

/// <summary>
/// Covers the C# half of pointer picking: which ops a subscription records, how a handle in an
/// incoming callback finds its way back to the object whose event to raise, and what a disposed
/// component does with a callback that arrives anyway.
/// <para>
/// That a ray actually meets a mesh, that one hit produces one callback, and that no
/// pointer-movement listener exists at all are proved in <c>tests/wire-format.test.mjs</c> against
/// the three.js bundle that ships. A mocked module here could only ever prove the plumbing.
/// </para>
/// </summary>
public class ThreePickingTests
{
	private static readonly PropertyInfo JsRuntimeProperty = typeof(ThreeCanvas)
		.GetProperty("_jsRuntime", BindingFlags.NonPublic | BindingFlags.Instance)
		?? throw new InvalidOperationException("ThreeCanvas no longer has an injected '_jsRuntime' property.");

	private static readonly MethodInfo OnAfterRenderAsyncMethod = typeof(ThreeCanvas)
		.GetMethod("OnAfterRenderAsync", BindingFlags.NonPublic | BindingFlags.Instance)
		?? throw new InvalidOperationException("ThreeCanvas no longer declares a protected 'OnAfterRenderAsync' method.");

	[Fact]
	public void Object3D_NoPointerSubscriber_RecordsNoPickOp()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var mesh = new Mesh();

		// Act
		context.Attach(mesh);
		var ops = context.Batch.Drain();

		// Assert
		ops.ShouldNotContain(x => x.Kind == ThreeOpKind.Pick);
	}

	[Fact]
	public void Object3D_OnClickSubscribedWhileAttached_RecordsAnOptInPickOp()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var mesh = new Mesh();
		context.Attach(mesh);
		context.Batch.Drain();

		// Act
		mesh.OnClick += _ => { };
		var ops = context.Batch.Drain();

		// Assert
		ops.ShouldHaveSingleItem();
		ops.Single().Kind.ShouldBe(ThreeOpKind.Pick);
		ops.Single().Handle.ShouldBe(mesh.Handle);
		ops.Single().Value.ShouldBe(true);
	}

	[Fact]
	public void Object3D_OnClickSubscribedBeforeAttach_ReplaysTheOptInOnAttach()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var mesh = new Mesh();
		mesh.OnClick += _ => { };

		// Act
		context.Attach(mesh);
		var ops = context.Batch.Drain();

		// Assert
		ops.ShouldContain(x => x.Kind == ThreeOpKind.Pick && x.Handle == mesh.Handle && Equals(x.Value, true));
	}

	[Fact]
	public void Object3D_OnClickSubscribedTwice_RecordsTheOptInOnce()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var mesh = new Mesh();
		context.Attach(mesh);
		context.Batch.Drain();

		// Act
		mesh.OnClick += _ => { };
		mesh.OnClick += _ => { };
		var ops = context.Batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Pick).ShouldBe(1);
	}

	[Fact]
	public void Object3D_OnClickLastHandlerRemoved_RecordsAnOptOutPickOp()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var mesh = new Mesh();
		context.Attach(mesh);
		Action<ThreePointerEvent> handler = _ => { };
		mesh.OnClick += handler;
		context.Batch.Drain();

		// Act
		mesh.OnClick -= handler;
		var ops = context.Batch.Drain();

		// Assert
		ops.ShouldHaveSingleItem();
		ops.Single().Kind.ShouldBe(ThreeOpKind.Pick);
		ops.Single().Value.ShouldBe(false);
	}

	[Fact]
	public void Object3D_OnClickOneOfTwoHandlersRemoved_RecordsNothing()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var mesh = new Mesh();
		context.Attach(mesh);
		Action<ThreePointerEvent> firstHandler = _ => { };
		mesh.OnClick += firstHandler;
		mesh.OnClick += _ => { };
		context.Batch.Drain();

		// Act
		mesh.OnClick -= firstHandler;
		var ops = context.Batch.Drain();

		// Assert
		ops.ShouldBeEmpty();
	}

	[Fact]
	public void ThreeContext_PointerEventForASubscribedObject_RaisesOnClickExactlyOnce()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var mesh = new Mesh();
		context.Attach(mesh);
		var raisedEvents = new List<ThreePointerEvent>();
		mesh.OnClick += pointerEvent => raisedEvents.Add(pointerEvent);
		var pointerEvent = new ThreePointerEvent { Point = new Vector3(0f, 0f, -4.5f), Distance = 4.5f };

		// Act
		context.DispatchPointerEvent(mesh.Handle, pointerEvent);

		// Assert
		raisedEvents.ShouldHaveSingleItem();
		raisedEvents.Single().ShouldBeSameAs(pointerEvent);
	}

	[Fact]
	public void ThreeContext_PointerEventForAnUnsubscribedHandle_RaisesNothing()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var subscribedMesh = new Mesh();
		var unsubscribedMesh = new Mesh();
		context.Attach(subscribedMesh);
		context.Attach(unsubscribedMesh);
		var wasRaised = false;
		subscribedMesh.OnClick += _ => wasRaised = true;

		// Act
		context.DispatchPointerEvent(unsubscribedMesh.Handle, BuildPointerEvent());

		// Assert
		wasRaised.ShouldBeFalse();
	}

	[Fact]
	public void ThreeContext_PointerEventAfterTheObjectOptedOut_RaisesNothing()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var mesh = new Mesh();
		context.Attach(mesh);
		var raisedCount = 0;
		Action<ThreePointerEvent> handler = _ => raisedCount++;
		mesh.OnClick += handler;
		mesh.OnClick -= handler;

		// Act
		context.DispatchPointerEvent(mesh.Handle, BuildPointerEvent());

		// Assert
		raisedCount.ShouldBe(0);
	}

	[Fact]
	public async Task ThreeContext_PointerEventAfterDispose_RaisesNothing()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var mesh = new Mesh();
		context.Attach(mesh);
		var wasRaised = false;
		mesh.OnClick += _ => wasRaised = true;
		await context.DisposeAsync();

		// Act
		context.DispatchPointerEvent(mesh.Handle, BuildPointerEvent());

		// Assert
		wasRaised.ShouldBeFalse();
	}

	[Fact]
	public async Task ThreeCanvas_ContextCreated_PassesADotNetReferenceToItself()
	{
		// Arrange
		var module = BuildReadyModule();
		var canvas = BuildCanvas(module);

		// Act
		await InvokeOnAfterRenderAsync(canvas);

		// Assert
		var selfReference = module.CreateContextArguments
			?.OfType<DotNetObjectReference<ThreeCanvas>>()
			.SingleOrDefault();
		selfReference.ShouldNotBeNull();
		selfReference.Value.ShouldBeSameAs(canvas);
	}

	[Fact]
	public async Task ThreeCanvas_Disposed_DisposesTheDotNetReferenceItHandedToJavaScript()
	{
		// Arrange
		var module = BuildReadyModule();
		var canvas = BuildCanvas(module);
		await InvokeOnAfterRenderAsync(canvas);
		var selfReference = module.CreateContextArguments!
			.OfType<DotNetObjectReference<ThreeCanvas>>()
			.Single();

		// Act
		await canvas.DisposeAsync();

		// Assert
		// A live reference answers with the component it wraps; a disposed one throws instead, which is
		// the only observation of its state the type offers - and an undisposed one is exactly what
		// would pin this component in the JS reference table for the life of the circuit.
		Should.Throw<ObjectDisposedException>(() => selfReference.Value);
	}

	[Fact]
	public async Task ThreeCanvas_ContextCreationFailed_StillDisposesTheDotNetReference()
	{
		// Arrange
		var createContextGate = new TaskCompletionSource<int>();
		createContextGate.SetException(new InvalidOperationException("WebGL is unavailable."));
		var module = new GatedCreateContextJsObjectReference(createContextGate);
		var canvas = BuildCanvas(module);
		await Record.ExceptionAsync(() => InvokeOnAfterRenderAsync(canvas));
		var selfReference = module.CreateContextArguments!
			.OfType<DotNetObjectReference<ThreeCanvas>>()
			.Single();

		// Act
		await canvas.DisposeAsync();

		// Assert
		Should.Throw<ObjectDisposedException>(() => selfReference.Value);
	}

	[Fact]
	public async Task ThreeCanvas_PointerEventForASubscribedObject_RaisesOnClickAndFlushesWhatTheHandlerChanged()
	{
		// Arrange
		var module = BuildReadyModule();
		var canvas = BuildCanvas(module);
		await InvokeOnAfterRenderAsync(canvas);
		var context = GetThreeContext(canvas);
		var mesh = new Mesh();
		context.Attach(mesh);
		await context.FlushAsync();
		mesh.OnClick += _ => mesh.IsVisible = false;
		await context.FlushAsync();
		module.AppliedBatches.Clear();

		// Act
		await canvas.DispatchPointerEventAsync(mesh.Handle, 0f, 0f, -4.5f, 4.5f);

		// Assert
		mesh.IsVisible.ShouldBeFalse();
		var flushedOps = module.AppliedBatches.ShouldHaveSingleItem();
		flushedOps.ShouldContain(x => x.Kind == ThreeOpKind.Set && x.Handle == mesh.Handle && x.Member == "visible");
	}

	[Fact]
	public async Task ThreeCanvas_PointerEventWhoseHandlerChangesNothing_MakesNoInteropCall()
	{
		// Arrange
		var module = BuildReadyModule();
		var canvas = BuildCanvas(module);
		await InvokeOnAfterRenderAsync(canvas);
		var context = GetThreeContext(canvas);
		var mesh = new Mesh();
		context.Attach(mesh);
		mesh.OnClick += _ => { };
		await context.FlushAsync();
		module.AppliedBatches.Clear();

		// Act
		await canvas.DispatchPointerEventAsync(mesh.Handle, 0f, 0f, -4.5f, 4.5f);

		// Assert
		module.AppliedBatches.ShouldBeEmpty();
	}

	[Fact]
	public async Task ThreeCanvas_PointerEventAfterDispose_RaisesNothingAndMakesNoInteropCall()
	{
		// Arrange
		var module = BuildReadyModule();
		var canvas = BuildCanvas(module);
		await InvokeOnAfterRenderAsync(canvas);
		var context = GetThreeContext(canvas);
		var mesh = new Mesh();
		context.Attach(mesh);
		var wasRaised = false;
		mesh.OnClick += _ => wasRaised = true;
		await canvas.DisposeAsync();
		module.AppliedBatches.Clear();

		// Act
		var exception = await Record.ExceptionAsync(() => canvas.DispatchPointerEventAsync(mesh.Handle, 0f, 0f, -4.5f, 4.5f));

		// Assert
		exception.ShouldBeNull();
		wasRaised.ShouldBeFalse();
		module.AppliedBatches.ShouldBeEmpty();
	}

	/// <summary>A pointer event whose values no assertion depends on.</summary>
	/// <returns>An arbitrary but valid pointer event.</returns>
	private static ThreePointerEvent BuildPointerEvent()
	{
		return new ThreePointerEvent { Point = new Vector3(0f, 0f, -4.5f), Distance = 4.5f };
	}

	/// <summary>Builds a module fake whose <c>createContext</c> answers immediately.</summary>
	/// <returns>The module fake, with its create gate already signalled.</returns>
	private static GatedCreateContextJsObjectReference BuildReadyModule()
	{
		var createContextGate = new TaskCompletionSource<int>();
		createContextGate.SetResult(1);
		return new GatedCreateContextJsObjectReference(createContextGate);
	}

	/// <summary>
	/// Builds a canvas wired to a module fake, bypassing the DI and parameter-binding pipeline the same
	/// way <c>ThreeCanvasTests</c> does.
	/// </summary>
	/// <param name="module">The module fake to serve the import with.</param>
	/// <returns>The canvas under test.</returns>
	private static ThreeCanvas BuildCanvas(GatedCreateContextJsObjectReference module)
	{
		var canvas = new ThreeCanvas();
		JsRuntimeProperty.SetValue(canvas, new SingleModuleJsRuntime(module));
		return canvas;
	}

	/// <summary>Drives the first render, which is what creates the JavaScript-side context.</summary>
	/// <param name="canvas">The canvas under test.</param>
	private static async Task InvokeOnAfterRenderAsync(ThreeCanvas canvas)
	{
		var result = OnAfterRenderAsyncMethod.Invoke(canvas, [true])
			?? throw new InvalidOperationException("OnAfterRenderAsync returned null instead of a Task.");
		await (Task) result;
	}

	/// <summary>
	/// Reads the context the canvas built, which is the same instance its <c>OnReady</c> would have
	/// handed a consumer.
	/// </summary>
	/// <param name="canvas">The canvas under test.</param>
	/// <returns>The context.</returns>
	private static ThreeContext GetThreeContext(ThreeCanvas canvas)
	{
		var field = typeof(ThreeCanvas).GetField("_threeContext", BindingFlags.NonPublic | BindingFlags.Instance)
			?? throw new InvalidOperationException("ThreeCanvas no longer has a '_threeContext' field.");

		return field.GetValue(canvas) as ThreeContext
			?? throw new InvalidOperationException("ThreeCanvas has not created its context.");
	}
}
