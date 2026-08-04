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
	public ThreeBatch Batch { get; } = new();

	/// <summary>
	/// Raised when the applier rejected one or more ops. Without this, a shader compile failure or
	/// an unknown type name would vanish into the browser console with no C#-side signal.
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
	/// Drains <see cref="Batch"/> and sends the pending ops to the JavaScript applier in a single
	/// interop call. A no-op when nothing is pending. Raises <see cref="OnError"/> if any op failed.
	/// Silently no-ops if the circuit has already disconnected.
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
			// could have been delivered anyway. Only this exception type is swallowed — a genuine
			// applier error still surfaces through OnError.
		}
	}

	/// <summary>
	/// Flushes any pending ops, then tells the renderer which scene and camera to render each frame.
	/// Silently no-ops if the circuit has already disconnected.
	/// </summary>
	/// <param name="sceneHandle">Handle of the scene object to render.</param>
	/// <param name="cameraHandle">Handle of the camera to render through.</param>
	public async Task SetActiveSceneAsync(int sceneHandle, int cameraHandle)
	{
		await FlushAsync();
		try
		{
			await _module.InvokeVoidAsync("setActiveScene", _contextId, sceneHandle, cameraHandle);
		}
		catch (JSDisconnectedException)
		{
			// A disconnected circuit is not recoverable and not an application bug; nothing pending
			// could have been delivered anyway. Only this exception type is swallowed — a genuine
			// applier error still surfaces through OnError.
		}
	}

	/// <summary>
	/// Stops the render loop, disposes every JavaScript-side three.js object owned by this context,
	/// and releases the module reference. Disposal during a Blazor Server circuit disconnect — the
	/// standard teardown path there — is not an error and completes without throwing.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		try
		{
			await _module.InvokeVoidAsync("disposeContext", _contextId);
			await _module.DisposeAsync();
		}
		catch (JSDisconnectedException)
		{
			// The JS side is already gone; there is nothing left to dispose and nothing recoverable.
		}
	}
}
