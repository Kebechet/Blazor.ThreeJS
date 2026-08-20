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
	private readonly object?[]? _images;
	private readonly CubeTextureMapping? _mapping;
	private readonly Wrapping? _wrapS;
	private readonly Wrapping? _wrapT;
	private readonly MagnificationTextureFilter? _magFilter;
	private readonly MinificationTextureFilter? _minFilter;
	private readonly PixelFormat? _format;
	private readonly TextureDataType? _type;
	private readonly float? _anisotropy;
	private readonly string? _colorSpace;

	/// <summary>This creates a new <c>CubeTexture</c> object.</summary>
	/// <param name="images">Value forwarded to the <c>images</c> constructor argument.</param>
	/// <param name="mapping">See <c>.mapping</c>. Default <c>THREE.CubeReflectionMapping</c>.</param>
	/// <param name="wrapS">See <c>.wrapS</c>. Default <c>THREE.ClampToEdgeWrapping</c>.</param>
	/// <param name="wrapT">See <c>.wrapT</c>. Default <c>THREE.ClampToEdgeWrapping</c>.</param>
	/// <param name="magFilter">See <c>.magFilter</c>. Default <c>THREE.LinearFilter</c>.</param>
	/// <param name="minFilter">See <c>.minFilter</c>. Default <c>THREE.LinearMipmapLinearFilter</c>.</param>
	/// <param name="format">See <c>.format</c>. Default <c>THREE.RGBAFormat</c>.</param>
	/// <param name="type">See <c>.type</c>. Default <c>THREE.UnsignedByteType</c>.</param>
	/// <param name="anisotropy">See <c>.anisotropy</c>. Default <c>THREE.Texture.DEFAULT_ANISOTROPY</c>.</param>
	/// <param name="colorSpace">See <c>.colorSpace</c>. Default <c>NoColorSpace</c>.</param>
	public CubeTexture(
		object?[]? images = null,
		CubeTextureMapping? mapping = null,
		Wrapping? wrapS = null,
		Wrapping? wrapT = null,
		MagnificationTextureFilter? magFilter = null,
		MinificationTextureFilter? minFilter = null,
		PixelFormat? format = null,
		TextureDataType? type = null,
		float? anisotropy = null,
		string? colorSpace = null)
		: base(wrapS: wrapS, wrapT: wrapT, magFilter: magFilter, minFilter: minFilter, format: format, type: type, anisotropy: anisotropy)
	{
		_images = images;
		_mapping = mapping;
		_wrapS = wrapS;
		_wrapT = wrapT;
		_magFilter = magFilter;
		_minFilter = minFilter;
		_format = format;
		_type = type;
		_anisotropy = anisotropy;
		_colorSpace = colorSpace;
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
	/// Constructor arguments forwarded to <c>THREE.CubeTexture</c>: images, mapping, wrapS, wrapT,
	/// magFilter, minFilter, format, type, anisotropy, colorSpace. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_images),
				ThreeValue.OrUnspecified(_mapping),
				ThreeValue.OrUnspecified(_wrapS),
				ThreeValue.OrUnspecified(_wrapT),
				ThreeValue.OrUnspecified(_magFilter),
				ThreeValue.OrUnspecified(_minFilter),
				ThreeValue.OrUnspecified(_format),
				ThreeValue.OrUnspecified(_type),
				ThreeValue.OrUnspecified(_anisotropy),
				ThreeValue.OrUnspecified(_colorSpace)
			]);
		}
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
