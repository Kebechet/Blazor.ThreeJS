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
	/// </summary>
	public async Task FlushAsync()
	{
		if (!Batch.HasPendingOps)
		{
			return;
		}

		var ops = Batch.Drain();
		var errors = await _module.InvokeAsync<List<ThreeError>>("applyBatch", _contextId, ops);
		if (errors.Any())
		{
			OnError?.Invoke(errors);
		}
	}

	/// <summary>
	/// Flushes any pending ops, then tells the renderer which scene and camera to render each frame.
	/// </summary>
	/// <param name="sceneHandle">Handle of the scene object to render.</param>
	/// <param name="cameraHandle">Handle of the camera to render through.</param>
	public async Task SetActiveSceneAsync(int sceneHandle, int cameraHandle)
	{
		await FlushAsync();
		await _module.InvokeVoidAsync("setActiveScene", _contextId, sceneHandle, cameraHandle);
	}

	/// <summary>
	/// Stops the render loop, disposes every JavaScript-side three.js object owned by this context,
	/// and releases the module reference.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		await _module.InvokeVoidAsync("disposeContext", _contextId);
		await _module.DisposeAsync();
	}
}
