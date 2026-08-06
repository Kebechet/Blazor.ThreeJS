// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates a texture from a
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/canvas">canvas
/// element</see>. The JavaScript-side <c>THREE.CanvasTexture</c>.
/// </summary>
/// <remarks>
/// This is almost the same as the base <see cref="Texture">Texture</see> class, except that it sets
/// <c>needsUpdate</c> to <c>true</c> immediately.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/textures/CanvasTexture">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/textures/CanvasTexture.js">Source</seealso>
public sealed class CanvasTexture : Texture
{
	/// <summary>This creates a new <c>CanvasTexture</c> object.</summary>
	public CanvasTexture()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CanvasTexture</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CanvasTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CanvasTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CanvasTexture"; }
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="CanvasTexture"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isCanvasTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isCanvasTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsCanvasTextureAsync()
	{
		return GetAsync<bool>("isCanvasTexture");
	}
}
