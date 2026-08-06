// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>Creates a cube texture made up of six images. The JavaScript-side <c>THREE.CubeTexture</c>.</summary>
/// <remarks>
/// <see cref="CubeTexture"/> is almost equivalent in functionality and usage to
/// <see cref="Texture"/>. The only differences are that the images are an array of _6_ images as
/// opposed to a single image, and the mapping options are <c>THREE.CubeReflectionMapping</c>
/// (default) or <c>THREE.CubeRefractionMapping</c>.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/textures/CubeTexture">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/textures/CubeTexture.js">Source</seealso>
public sealed class CubeTexture : Texture
{
	/// <summary>This creates a new <c>CubeTexture</c> object.</summary>
	public CubeTexture()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CubeTexture</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CubeTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CubeTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CubeTexture"; }
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="CubeTexture"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isCubeTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isCubeTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsCubeTextureAsync()
	{
		return GetAsync<bool>("isCubeTexture");
	}
}
