// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.WebGLBackend</c>.</summary>
public sealed class WebGLBackend : ThreeObject
{
	/// <summary>Initializes a new <see cref="WebGLBackend"/>.</summary>
	public WebGLBackend()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>WebGLBackend</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal WebGLBackend(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WebGLBackend</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WebGLBackend"; }
	}

	/// <summary>
	/// Reads <c>isWebGLBackend</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isWebGLBackend</c> held.
	/// </summary>
	/// <returns>The value <c>isWebGLBackend</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsWebGLBackendAsync()
	{
		return GetAsync<bool>("isWebGLBackend");
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
