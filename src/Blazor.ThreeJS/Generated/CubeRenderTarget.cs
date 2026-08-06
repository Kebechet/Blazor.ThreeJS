// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class represents a cube render target. It is a special version of
/// <c>WebGLCubeRenderTarget</c> which is compatible with <c>WebGPURenderer</c>. The JavaScript-side
/// <c>THREE.CubeRenderTarget</c>.
/// </summary>
public sealed class CubeRenderTarget : RenderTarget
{
	private readonly float _size;

	/// <summary>Constructs a new cube render target.</summary>
	/// <param name="size">The size of the render target.</param>
	public CubeRenderTarget(float size = 1f)
	{
		_size = size;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CubeRenderTarget</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CubeRenderTarget(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_size = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CubeRenderTarget</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CubeRenderTarget"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.CubeRenderTarget</c>: size.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_size]; }
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isCubeRenderTarget</c> held.
	/// </summary>
	/// <returns>The value <c>isCubeRenderTarget</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsCubeRenderTargetAsync()
	{
		return GetAsync<bool>("isCubeRenderTarget");
	}
}
