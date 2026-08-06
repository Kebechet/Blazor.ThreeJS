// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.WebGPUBackend</c>.</summary>
public sealed class WebGPUBackend : ThreeObject
{
	/// <summary>Initializes a new <see cref="WebGPUBackend"/>.</summary>
	public WebGPUBackend()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>WebGPUBackend</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal WebGPUBackend(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WebGPUBackend</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WebGPUBackend"; }
	}

	/// <summary>
	/// Reads <c>isWebGPUBackend</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isWebGPUBackend</c> held.
	/// </summary>
	/// <returns>The value <c>isWebGPUBackend</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsWebGPUBackendAsync()
	{
		return GetAsync<bool>("isWebGPUBackend");
	}

	/// <summary>
	/// Reads <c>coordinateSystem</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>coordinateSystem</c> held.
	/// </summary>
	/// <returns>The value <c>coordinateSystem</c> held, once the JavaScript side has answered.</returns>
	public Task<CoordinateSystem> CoordinateSystemAsync()
	{
		return GetAsync<CoordinateSystem>("coordinateSystem");
	}

	/// <summary>
	/// Reads <c>getMaxAnisotropy</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>getMaxAnisotropy</c> returned.
	/// </summary>
	/// <returns>The value <c>getMaxAnisotropy</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetMaxAnisotropyAsync()
	{
		return RecordRead<float>("getMaxAnisotropy");
	}
}
