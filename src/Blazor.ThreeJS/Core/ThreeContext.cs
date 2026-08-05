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
	private readonly IJSObjectReference _module;
	private readonly int _contextId;

	/// <summary>
	/// Accumulates pending create/set/call/add/remove/dispose ops for this context until the next
	/// <see cref="FlushAsync"/>.
	/// </summary>
	internal ThreeBatch Batch { get; } = new();

	/// <summary>
	/// Raised when the applier rejected one or more ops while running a batch: an unknown three.js
	/// type name, an unknown handle, or a property write or method call that threw. Without this,
	/// those failures would vanish into the browser console with no C#-side signal.
	/// <para>
	/// This covers the <see cref="FlushAsync"/> path only. Failures raised while <b>rendering</b> a
	/// frame — a shader that fails to compile, a lost WebGL context — happen inside the JavaScript
	/// render loop, which has no channel back to C#, and are not reported here.
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
	}

	/// <summary>
	/// Attaches an object graph to this context's <see cref="Batch"/>: emits a create op for
	/// <paramref name="root"/> and every object already added under it, then replays any property
	/// writes made before this call. Attaching is idempotent - calling this again on an
	/// already-attached root is a no-op.
	/// </summary>
	/// <param name="root">Root of the object graph to attach, typically a <see cref="Scene"/>.</param>
	public void Attach(Object3D root)
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
			var errors = await _module.InvokeAsync<List<ThreeError>>("applyBatch", _contextId, ops);
			if (errors.Any())
			{
				OnError?.Invoke(errors);
			}
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
