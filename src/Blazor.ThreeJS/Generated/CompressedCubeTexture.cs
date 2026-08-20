// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.CompressedCubeTexture</c>.</summary>
public sealed class CompressedCubeTexture : CompressedTexture
{
	private readonly CompressedTextureImageData[] _images;

	/// <summary>Initializes a new <see cref="CompressedCubeTexture"/>.</summary>
	/// <param name="images">Value forwarded to the <c>images</c> constructor argument.</param>
	public CompressedCubeTexture(CompressedTextureImageData[] images)
	{
		_images = images;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CompressedCubeTexture</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CompressedCubeTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_images = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CompressedCubeTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CompressedCubeTexture"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.CompressedCubeTexture</c>: images.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_images]; }
	}

	/// <summary>
	/// Reads <c>isCompressedCubeTexture</c> back from the JavaScript-side object. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isCompressedCubeTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isCompressedCubeTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsCompressedCubeTextureAsync()
	{
		return GetAsync<bool>("isCompressedCubeTexture");
	}

	/// <summary>
	/// Reads <c>isCubeTexture</c> back from the JavaScript-side object. Read-only in three.js, so it is
	/// read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isCubeTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isCubeTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsCubeTextureAsync()
	{
		return GetAsync<bool>("isCubeTexture");
	}
}
