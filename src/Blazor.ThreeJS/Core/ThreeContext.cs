using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Addons;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.JSInterop;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Owns the batch and the JavaScript module for a single canvas. A single <see cref="FlushAsync"/>
/// call drains every op pending in <see cref="Batch"/> into one interop call, no matter how many
/// property writes accumulated since the previous flush.
/// </summary>
public sealed class ThreeContext : IAsyncDisposable
{
	/// <summary>
	/// How long a read waits for the applier before it gives up. Long enough that a slow first frame
	/// or a busy Blazor Server circuit never trips it, short enough that a caller awaiting a value the
	/// browser will never send is not left waiting forever.
	/// </summary>
	private static readonly TimeSpan _defaultReadTimeout = TimeSpan.FromSeconds(30);

	private readonly IJSObjectReference _module;
	private readonly int _contextId;

	/// <summary>
	/// Objects that have a pointer-event subscriber, keyed by the handle the browser names in a
	/// callback. Only these can be the target of one: the applier hit-tests exactly the same set, and
	/// a handle absent from here is a callback with nowhere to go rather than a lookup that guesses.
	/// </summary>
	private readonly Dictionary<int, Object3D> _pointerTargetsByHandle = [];

	/// <summary>
	/// Accumulates pending create/set/call/add/remove/dispose/read ops for this context until the next
	/// <see cref="FlushAsync"/> or read.
	/// </summary>
	internal ThreeBatch Batch { get; } = new();

	/// <summary>
	/// How long a read started through this context waits for the applier's answer before faulting with
	/// a <see cref="TimeoutException"/>. Defaults to 30 seconds.
	/// </summary>
	public TimeSpan ReadTimeout { get; set; } = _defaultReadTimeout;

	/// <summary>
	/// How many interop calls this context has made since it was created. The retained mirror's central
	/// claim is that a scene nobody is changing costs nothing to keep on screen, and this is the number
	/// that shows it: the JavaScript side owns the render loop, so a still scene leaves this flat no
	/// matter how many frames go by.
	/// <para>
	/// Counts calls that returned. A batch lost to a dead circuit never reached the browser and is not
	/// counted as though it had.
	/// </para>
	/// </summary>
	public long SentBatchCount { get; private set; }

	/// <summary>
	/// How many individual ops this context has delivered to the browser since it was created. Divided
	/// by <see cref="SentBatchCount"/> this is the coalescing ratio — how many property writes a single
	/// interop call carried.
	/// </summary>
	public long SentOpCount { get; private set; }

	/// <summary>
	/// Raised when the applier rejected one or more ops while running a batch: an unknown three.js
	/// type name, an unknown handle, or a property write or method call that threw. Without this,
	/// those failures would vanish into the browser console with no C#-side signal.
	/// <para>
	/// This covers the <see cref="FlushAsync"/> path only. Failures raised while <b>rendering</b> a
	/// frame — a shader that fails to compile, a lost WebGL context — happen inside the JavaScript
	/// render loop, which has no channel back to C#, and are not reported here.
	/// </para>
	/// <para>
	/// A failed <b>read</b> is not reported here either. This event carries the failures that have
	/// nowhere else to go, and a read has somewhere: it faults the task the caller is awaiting, with
	/// the applier's own message.
	/// </para>
	/// </summary>
	public event Action<IReadOnlyList<ThreeError>>? OnError;

	/// <summary>
	/// Wraps an already-created JavaScript-side context. Called by <c>ThreeCanvas</c> once
	/// <c>createContext</c> has returned a context id for its canvas.
	/// </summary>
	/// <param name="module">The imported <c>three-interop.js</c> module instance.</param>
	/// <param name="contextId">Identifier of the JavaScript-side context returned by <c>createContext</c>.</param>
	internal ThreeContext(IJSObjectReference module, int contextId)
	{
		_module = module;
		_contextId = contextId;
		Batch.Context = this;
		Renderer = new WebGPURenderer(Batch, ThreeWireFormat.RendererHandle);
	}

	/// <summary>
	/// The renderer drawing this canvas, which the browser built and this mirror only names.
	/// <para>
	/// A <c>WebGPURenderer</c>, which runs on a WebGPU backend where the browser has one and falls back
	/// to a WebGL2 backend where it does not. Both are the same C# type and the same scene API; what
	/// differs is only what it talks to underneath.
	/// </para>
	/// <para>
	/// Everything else in a scene is reachable from the scene graph; the renderer is not, so without
	/// this there is no way to enable shadow maps, choose a tone mapping, or set the clear colour from
	/// C# at all. Writes go through the same batch as any other object's.
	/// </para>
	/// <para>
	/// ⚠️ Adopted, not created: it carries no create op, and the mirror knows only what C# has written
	/// to it. Reading a property back that C# never set answers with the C# default rather than what
	/// three.js holds — use <c>GetAsync</c> for the browser's own value.
	/// </para>
	/// </summary>
	public WebGPURenderer Renderer { get; }

	/// <summary>
	/// Every mirrored object this context has attached, by handle. The C# counterpart of the applier's
	/// own object-to-handle map, and needed for the same reason: a member whose result is an object
	/// answers with a handle, and that handle is often one C# already has a mirror for —
	/// <c>mesh.geometry</c> answers with the geometry the caller passed in. Building a second C# object
	/// for it would leave two mirrors of one three.js object, and a write through either invisible to
	/// the other.
	/// <para>
	/// Weak, so registering an object here never keeps it alive: the scene graph owns the references,
	/// and an entry whose object has been collected resolves to nothing rather than resurrecting it.
	/// </para>
	/// </summary>
	private readonly Dictionary<int, WeakReference<ThreeObject>> _mirroredObjectsByHandle = [];

	/// <summary>Records a mirrored object so a handle answered by a read can resolve back to it.</summary>
	/// <param name="mirroredObject">The object being attached.</param>
	internal void Register(ThreeObject mirroredObject)
	{
		_mirroredObjectsByHandle[mirroredObject.Handle] = new WeakReference<ThreeObject>(mirroredObject);
	}

	/// <summary>Finds the mirror already holding a handle, if this context has one.</summary>
	/// <param name="handle">Handle the applier answered with.</param>
	/// <returns>The existing mirror, or <see langword="null"/> when nothing here holds that handle.</returns>
	internal ThreeObject? Resolve(int handle)
	{
		if (!_mirroredObjectsByHandle.TryGetValue(handle, out var reference))
		{
			return null;
		}

		if (reference.TryGetTarget(out var mirroredObject))
		{
			return mirroredObject;
		}

		// Collected since it was registered, so the entry is dead weight rather than an answer.
		_mirroredObjectsByHandle.Remove(handle);
		return null;
	}

	/// <summary>
	/// Attaches an object graph to this context's <see cref="Batch"/>: emits a create op for
	/// <paramref name="root"/> and every object already added under it, then replays any property
	/// writes made before this call. Attaching is idempotent - calling this again on an
	/// already-attached root is a no-op.
	/// <para>
	/// Takes any mirrored object rather than only a scene-graph one. Most objects reach a context by
	/// being referenced from one that is already attached - a geometry through the mesh that holds it -
	/// but an object nothing in the graph references, a <c>Clock</c> or a curve being measured, has no
	/// such route and would otherwise have no way in at all.
	/// </para>
	/// </summary>
	/// <param name="root">Root of the object graph to attach, typically a <see cref="Scene"/>.</param>
	public void Attach(ThreeObject root)
	{
		root.AttachTo(Batch);
	}

	/// <summary>
	/// Drains <see cref="Batch"/> and sends the pending ops to the JavaScript applier in a single
	/// interop call. A no-op when nothing is pending. Raises <see cref="OnError"/> if any op failed.
	/// Silently no-ops if the circuit has already disconnected or the module reference has already
	/// been disposed.
	/// </summary>
	public async Task FlushAsync()
	{
		if (!Batch.HasPendingOps)
		{
			return;
		}

		var ops = Batch.Drain();
		try
		{
			var response = await _module.InvokeAsync<ThreeBatchResponse>("applyBatch", _contextId, ops);
			RecordSend(ops.Count);
			RaiseErrors(response);
		}
		catch (JSDisconnectedException)
		{
			// A disconnected circuit is not recoverable and not an application bug; nothing pending
			// could have been delivered anyway. Only this exception type and ObjectDisposedException
			// are swallowed — a genuine applier error still surfaces through OnError.
		}
		catch (ObjectDisposedException)
		{
			// Reachable from ThreeCanvas itself: if disposal lands after ThreeContext already exists
			// but while OnReady is still running, ThreeCanvas.DisposeAsync tears this context down
			// immediately rather than waiting for OnReady to finish, since only the framework-owned
			// import/createContext work is awaited before disposal proceeds. The still-running OnReady
			// continuation can then reach this FlushAsync call after the module is already disposed.
			// Also stays narrow and swallowed for any other caller racing a concurrent disposal.
			// Nothing pending could have been delivered anyway.
		}
	}

	/// <summary>
	/// Runs one read: records the read op behind everything already pending, sends the whole batch in a
	/// single interop call, and decodes the value that comes back.
	/// <para>
	/// The read travels <b>inside</b> the batch rather than on a call of its own, which is what makes it
	/// observe the writes made before it: the applier runs the ops in order, so the value it reads is
	/// taken after those writes have landed. A caller cannot get stale data by forgetting to flush,
	/// because there is no separate flush to forget.
	/// </para>
	/// <para>
	/// Unlike <see cref="FlushAsync"/>, a dead circuit is <b>not</b> swallowed here. A write that could
	/// not be delivered has nobody waiting on it; a read does, and answering it with a fabricated
	/// default would be worse than failing. <see cref="JSDisconnectedException"/> and
	/// <see cref="ObjectDisposedException"/> therefore fault the returned task, as does a response that
	/// never arrives.
	/// </para>
	/// </summary>
	/// <typeparam name="TValue">C# type the query declares it returns.</typeparam>
	/// <param name="handle">Handle of the object to read from.</param>
	/// <param name="member">Name of the three.js method to invoke.</param>
	/// <param name="encodedArgs">Positional arguments, already in wire form.</param>
	/// <param name="mintsHandle">Whether the applier should answer with a handle instead of a value.</param>
	/// <returns>The decoded return value.</returns>
	/// <exception cref="TimeoutException">Thrown when no response arrives within <see cref="ReadTimeout"/>.</exception>
	/// <exception cref="InvalidOperationException">
	/// Thrown when the applier rejected the read, or answered the batch without a row for it.
	/// </exception>
	internal Task<TValue> ReadAsync<TValue>(int handle, string member, object?[] encodedArgs, bool mintsHandle = false)
	{
		return AwaitValueAsync<TValue>(Batch.Read(handle, member, encodedArgs, mintsHandle), handle, member);
	}

	/// <summary>
	/// Runs one property read, on exactly the terms <see cref="ReadAsync{TValue}"/> runs a method one:
	/// recorded behind everything already pending, sent in the same interop call, and answered on its
	/// own result row.
	/// <para>
	/// This is the escape hatch's half of the read channel. The generated classes have no use for it —
	/// a three.js property they mirror is state C# already holds — but a property nothing mirrors, on a
	/// class nothing wraps, has no method to route through <see cref="ReadAsync{TValue}"/> and would
	/// otherwise be unreachable.
	/// </para>
	/// </summary>
	/// <typeparam name="TValue">C# type the caller declares the property holds.</typeparam>
	/// <param name="handle">Handle of the object to read from.</param>
	/// <param name="member">Name of the three.js property to read.</param>
	/// <param name="mintsHandle">Whether the applier should answer with a handle instead of a value.</param>
	/// <returns>The decoded value.</returns>
	/// <exception cref="TimeoutException">Thrown when no response arrives within <see cref="ReadTimeout"/>.</exception>
	/// <exception cref="InvalidOperationException">
	/// Thrown when the applier rejected the read, answered the batch without a row for it, or sent back
	/// a value <typeparamref name="TValue"/> cannot hold.
	/// </exception>
	internal Task<TValue> GetAsync<TValue>(int handle, string member, bool mintsHandle = false)
	{
		return AwaitValueAsync<TValue>(Batch.Get(handle, member, mintsHandle), handle, member);
	}

	/// <summary>
	/// Sends the batch carrying a value-producing op and completes with the value that comes back.
	/// Shared by <see cref="ReadAsync{TValue}"/> and <see cref="GetAsync{TValue}"/>, which differ only
	/// in the op they record — everything from the drain onwards is the same correlation, the same
	/// timeout and the same refusal to answer with a value nobody sent.
	/// </summary>
	/// <typeparam name="TValue">C# type the caller declares the value has.</typeparam>
	/// <param name="requestId">Id the recorded op will be answered under.</param>
	/// <param name="handle">Handle the read targeted, named in any failure.</param>
	/// <param name="member">Member the read named, named in any failure.</param>
	/// <returns>The decoded value.</returns>
	private async Task<TValue> AwaitValueAsync<TValue>(int requestId, int handle, string member)
	{
		var ops = Batch.Drain();

		using var timeout = new CancellationTokenSource(ReadTimeout);
		var invocation = _module.InvokeAsync<ThreeBatchResponse>("applyBatch", timeout.Token, _contextId, ops).AsTask();

		ThreeBatchResponse response;
		try
		{
			// Raced against the timeout rather than simply awaited with the token. Every JSRuntime
			// implementation the framework ships does honour the token, but IJSObjectReference is an
			// interface a consumer may implement, and a caller awaiting a value must not be able to hang
			// on one that ignores it. The token is still passed, so a cooperative runtime also stops
			// waiting on its side rather than only here.
			var firstToFinish = await Task.WhenAny(invocation, Task.Delay(Timeout.InfiniteTimeSpan, timeout.Token));
			if (firstToFinish != invocation)
			{
				// The abandoned invocation may still fault later with nobody awaiting it.
				ObserveAbandoned(invocation);
				throw BuildTimeout(handle, member);
			}

			response = await invocation;
		}
		catch (OperationCanceledException) when (timeout.IsCancellationRequested)
		{
			throw BuildTimeout(handle, member);
		}

		RecordSend(ops.Count);
		RaiseErrors(response);

		var result = response.Results.FirstOrDefault(x => x.RequestId == requestId);
		if (result is null)
		{
			throw new InvalidOperationException(
				$"The applier answered the batch carrying the read of '{member}' from handle {handle} without a result for request {requestId}. " +
				$"Every read op must produce exactly one result row, so this is a wire-format disagreement with three-interop.js.");
		}

		if (result.Message is not null)
		{
			throw new InvalidOperationException(
				$"The applier could not read '{member}' from handle {handle}: {result.Message}");
		}

		try
		{
			return ThreeValue.Decode<TValue>(result.Value);
		}
		catch (JsonException exception)
		{
			// Only reachable when what three.js actually holds is not what the caller declared - a
			// string read as a float, a number read as a bool. Faulting is the whole policy for a read:
			// the alternative is answering with default(TValue), which is a value the browser never
			// sent. The wrap exists because the raw deserializer message names neither the member nor
			// the object, and this failure is one an escape-hatch caller reaches by getting a type wrong.
			throw new InvalidOperationException(
				$"The value of '{member}' on handle {handle} cannot be held as '{typeof(TValue).FullName}': {exception.Message} " +
				$"A read faults rather than answering with a default the browser never sent.",
				exception);
		}
	}

	/// <summary>
	/// Asks the browser to load a glTF or GLB file and to report the graph it built.
	/// <para>
	/// This is not a batch op, and deliberately so. Every op kind is an instruction in an ordered
	/// stream with no answer of its own; this is a request that mints JavaScript-side state and
	/// answers with a description of it, which is a different shape of call. Routing it through the
	/// batch would need an eighth op kind whose only purpose was to carry a payload the applier
	/// invents, and would tie a load that may take seconds to the flush of a scene that is ready now.
	/// </para>
	/// <para>
	/// Nothing is flushed first. The load reads no mirrored state, so there is nothing pending it could
	/// observe; the handles it mints are registered the moment it returns, so a batch op referencing one
	/// resolves whenever it is eventually flushed.
	/// </para>
	/// <para>
	/// No timeout is imposed. A read has one because a value that never arrives leaves a caller waiting
	/// on something the applier may simply not send; a load is a network fetch the browser is already
	/// bounding, and a big model over a slow connection is slow rather than broken.
	/// </para>
	/// </summary>
	/// <param name="url">URL of the file, as the browser will fetch it.</param>
	/// <param name="progressReference">
	/// Reference the browser reports fetch progress to, or <see langword="null"/> when the caller asked
	/// for none. Owned by the caller, which disposes it once the load has settled.
	/// </param>
	/// <returns>One row per mirrored node of the loaded graph.</returns>
	internal async Task<GLTFLoadResponse> LoadGltfAsync(string url, DotNetObjectReference<GltfProgressReporter>? progressReference)
	{
		return await _module.InvokeAsync<GLTFLoadResponse>("loadGltf", _contextId, url, progressReference);
	}

	/// <summary>
	/// Asks the browser to bind <c>OrbitControls</c> to the camera at <paramref name="cameraHandle"/>
	/// and to this context's canvas, and hands back the handle it minted for them.
	/// </summary>
	/// <param name="cameraHandle">Handle of the camera the controls should drive.</param>
	/// <returns>The negative handle the controls were registered under.</returns>
	internal async Task<int> AttachOrbitControlsAsync(int cameraHandle)
	{
		return await _module.InvokeAsync<int>("attachOrbitControls", _contextId, cameraHandle);
	}

	/// <summary>
	/// Takes any attached <c>OrbitControls</c> off this context's canvas, releasing the DOM listeners
	/// three.js registered for them. Silently no-ops if the circuit has already disconnected or the
	/// module reference has already been disposed — unlike a load, nothing is waiting on a value here,
	/// and a canvas that is gone has no listeners left to remove.
	/// </summary>
	internal async Task DetachOrbitControlsAsync()
	{
		try
		{
			await _module.InvokeVoidAsync("detachOrbitControls", _contextId);
		}
		catch (JSDisconnectedException)
		{
			// A disconnected circuit is not recoverable and not an application bug; the canvas the
			// listeners were on is gone with it.
		}
		catch (ObjectDisposedException)
		{
			// See the matching catch in FlushAsync — reachable when a consumer detaches while
			// ThreeCanvas concurrently disposes the context.
		}
	}

	/// <summary>
	/// Records that <paramref name="pointerTarget"/> can be the target of a pointer callback. Called by
	/// <see cref="Object3D"/> when it opts in, alongside the op that tells the applier the same thing.
	/// </summary>
	/// <param name="pointerTarget">The object that opted in.</param>
	internal void RegisterPointerTarget(Object3D pointerTarget)
	{
		_pointerTargetsByHandle[pointerTarget.Handle] = pointerTarget;
	}

	/// <summary>
	/// Drops <paramref name="pointerTarget"/> from the table, so a callback still in flight for it
	/// finds nothing rather than raising an event nobody is subscribed to any more.
	/// </summary>
	/// <param name="pointerTarget">The object that opted out.</param>
	internal void UnregisterPointerTarget(Object3D pointerTarget)
	{
		_pointerTargetsByHandle.Remove(pointerTarget.Handle);
	}

	/// <summary>
	/// Raises the pointer event on the object the browser hit. Unlike every other path in this class,
	/// nothing in C# asked for this call: the applier sends it when a click's ray meets an opted-in
	/// object, and it arrives through <c>ThreeCanvas</c>'s <c>[JSInvokable]</c> method.
	/// <para>
	/// A handle with no entry is ignored rather than reported. The one way to reach it is a callback
	/// that crossed the boundary while the object was opting out or the context was being torn down,
	/// and the event it would have raised has by then been unsubscribed on purpose.
	/// </para>
	/// </summary>
	/// <param name="handle">Handle of the object the ray met.</param>
	/// <param name="pointerEvent">Where the ray met it.</param>
	internal void DispatchPointerEvent(int handle, ThreePointerEvent pointerEvent)
	{
		if (!_pointerTargetsByHandle.TryGetValue(handle, out var pointerTarget))
		{
			return;
		}

		pointerTarget.RaiseClick(pointerEvent);
	}

	/// <summary>
	/// Records one delivered interop call and the ops it carried. Called only after the call returned,
	/// so a batch the circuit swallowed is not counted as traffic that happened.
	/// </summary>
	/// <param name="opCount">How many ops the delivered call carried.</param>
	private void RecordSend(int opCount)
	{
		SentBatchCount++;
		SentOpCount += opCount;
	}

	/// <summary>
	/// Publishes the ops the applier rejected, if any. Read failures are not among them: they travel on
	/// their own result row and fault the task that asked for the value.
	/// </summary>
	/// <param name="response">What the applier returned for one batch.</param>
	private void RaiseErrors(ThreeBatchResponse response)
	{
		if (response.Errors.Any())
		{
			OnError?.Invoke(response.Errors);
		}
	}

	/// <summary>Builds the failure a read that never got an answer reports.</summary>
	/// <param name="handle">Handle the read targeted.</param>
	/// <param name="member">Method the read invoked.</param>
	/// <returns>The exception to throw.</returns>
	private TimeoutException BuildTimeout(int handle, string member)
	{
		return new TimeoutException(
			$"Reading '{member}' from handle {handle} got no answer within {ReadTimeout}. " +
			$"The JavaScript side never completed the batch carrying the read.");
	}

	/// <summary>
	/// Reads the fault off an invocation nothing awaits any more, so a later failure on it does not
	/// surface as an unobserved task exception long after the read it belonged to already gave up.
	/// </summary>
	/// <param name="invocation">The abandoned interop call.</param>
	private static void ObserveAbandoned(Task invocation)
	{
		_ = invocation.ContinueWith(
			x => _ = x.Exception,
			CancellationToken.None,
			TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
			TaskScheduler.Default);
	}

	/// <summary>
	/// Attaches both objects to this context, flushes any pending ops, then tells the renderer which
	/// scene and camera to render each frame. Silently no-ops if the circuit has already disconnected
	/// or the module reference has already been disposed.
	/// <para>
	/// Attaching here rather than demanding it of the caller is what makes the idiomatic three.js
	/// arrangement work: a camera does not have to sit in the scene graph, so an unattached one reaches
	/// this call in perfectly ordinary code. Both attaches happen before the flush so their create ops
	/// travel in the same interop call as the handles below. Attaching is idempotent, so a graph
	/// already passed to <see cref="Attach"/> costs nothing here — while an object belonging to another
	/// context still throws, which is the case that really is a mistake.
	/// </para>
	/// </summary>
	/// <param name="scene">The scene object to render.</param>
	/// <param name="camera">The camera to render through.</param>
	/// <exception cref="InvalidOperationException">
	/// Thrown when <paramref name="scene"/> or <paramref name="camera"/> is already attached to a
	/// different <see cref="ThreeContext"/>.
	/// </exception>
	public async Task SetActiveSceneAsync(Object3D scene, Object3D camera)
	{
		Attach(scene);
		Attach(camera);

		await FlushAsync();
		try
		{
			await _module.InvokeVoidAsync("setActiveScene", _contextId, scene.Handle, camera.Handle);
		}
		catch (JSDisconnectedException)
		{
			// A disconnected circuit is not recoverable and not an application bug; nothing pending
			// could have been delivered anyway. Only this exception type and ObjectDisposedException
			// are swallowed — a genuine applier error still surfaces through OnError.
		}
		catch (ObjectDisposedException)
		{
			// See the matching catch in FlushAsync — reachable from ThreeCanvas itself when disposal
			// lands while OnReady is still running, not just from an unrelated caller.
		}
	}

	/// <summary>
	/// Stops the render loop, disposes every JavaScript-side three.js object owned by this context,
	/// and releases the module reference. Disposal during a Blazor Server circuit disconnect — the
	/// standard teardown path there — is not an error and completes without throwing. Nor is disposing
	/// a context whose module reference was already disposed elsewhere — defence in depth for a
	/// consumer that disposes this context itself while <c>ThreeCanvas</c> concurrently disposes it too.
	/// <para>
	/// The module is released in a <c>finally</c> so that a <c>disposeContext</c> failure of any
	/// kind still gives the reference back. Leaking it would pin the imported module for the
	/// lifetime of the circuit on Blazor Server.
	/// </para>
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		// Emptied before anything is awaited, so a pointer callback that crosses the boundary during
		// the teardown finds no target and raises nothing. disposeContext takes the listener off the
		// canvas as well, but only once it runs.
		_pointerTargetsByHandle.Clear();
		try
		{
			await _module.InvokeVoidAsync("disposeContext", _contextId);
		}
		catch (JSDisconnectedException)
		{
			// The JS side is already gone; there is nothing left to dispose and nothing recoverable.
		}
		catch (ObjectDisposedException)
		{
			// The module reference was already disposed elsewhere; there is nothing left to dispose
			// and nothing recoverable. Only this exception type and JSDisconnectedException are
			// swallowed here — a genuine applier error still surfaces through OnError.
		}
		finally
		{
			try
			{
				await _module.DisposeAsync();
			}
			catch (JSDisconnectedException)
			{
				// The circuit died with the module reference, so there is nothing left to release.
			}
		}
	}
}
