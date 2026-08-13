// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates a texture directly from raw data, width and height. The JavaScript-side
/// <c>THREE.DataTexture</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/textures/DataTexture">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/textures/DataTexture.js">Source</seealso>
public sealed class DataTexture : Texture
{
	private readonly TypedArray? _data;
	private readonly float _width;
	private readonly float _height;
	private readonly PixelFormat? _format;
	private readonly TextureDataType? _type;
	private readonly Mapping? _mapping;
	private readonly Wrapping? _wrapS;
	private readonly Wrapping? _wrapT;
	private readonly MagnificationTextureFilter? _magFilter;
	private readonly MinificationTextureFilter? _minFilter;
	private readonly float? _anisotropy;
	private readonly ColorSpace? _colorSpace;

	/// <summary>Initializes a new <see cref="DataTexture"/>.</summary>
	/// <param name="data">
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/ArrayBufferView">ArrayBufferView</see>
	/// of the texture.
	/// </param>
	/// <param name="width">Width of the texture.</param>
	/// <param name="height">Height of the texture.</param>
	/// <param name="format">See <c>.format</c>. Default <c>THREE.RGBAFormat</c>.</param>
	/// <param name="type">See <c>.type</c>. Default <c>THREE.UnsignedByteType</c>.</param>
	/// <param name="mapping">See <c>.mapping</c>. Default <c>THREE.Texture.DEFAULT_MAPPING</c>.</param>
	/// <param name="wrapS">See <c>.wrapS</c>. Default <c>THREE.ClampToEdgeWrapping</c>.</param>
	/// <param name="wrapT">See <c>.wrapT</c>. Default <c>THREE.ClampToEdgeWrapping</c>.</param>
	/// <param name="magFilter">See <c>.magFilter</c>. Default <c>THREE.NearestFilter</c>.</param>
	/// <param name="minFilter">See <c>.minFilter</c>. Default <c>THREE.NearestFilter</c>.</param>
	/// <param name="anisotropy">See <c>.anisotropy</c>. Default <c>THREE.Texture.DEFAULT_ANISOTROPY</c>.</param>
	/// <param name="colorSpace">See <c>.colorSpace</c>. Default <c>NoColorSpace</c>.</param>
	public DataTexture(
		TypedArray? data = null,
		float width = 1f,
		float height = 1f,
		PixelFormat? format = null,
		TextureDataType? type = null,
		Mapping? mapping = null,
		Wrapping? wrapS = null,
		Wrapping? wrapT = null,
		MagnificationTextureFilter? magFilter = null,
		MinificationTextureFilter? minFilter = null,
		float? anisotropy = null,
		ColorSpace? colorSpace = null)
	{
		_data = data;
		_width = width;
		_height = height;
		_format = format;
		_type = type;
		_mapping = mapping;
		_wrapS = wrapS;
		_wrapT = wrapT;
		_magFilter = magFilter;
		_minFilter = minFilter;
		_anisotropy = anisotropy;
		_colorSpace = colorSpace;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>DataTexture</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal DataTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_width = default!;
		_height = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.DataTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "DataTexture"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.DataTexture</c>: data, width, height, format, type,
	/// mapping, wrapS, wrapT, magFilter, minFilter, anisotropy, colorSpace. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_data),
				_width,
				_height,
				ThreeValue.OrUnspecified(_format),
				ThreeValue.OrUnspecified(_type),
				ThreeValue.OrUnspecified(_mapping),
				ThreeValue.OrUnspecified(_wrapS),
				ThreeValue.OrUnspecified(_wrapT),
				ThreeValue.OrUnspecified(_magFilter),
				ThreeValue.OrUnspecified(_minFilter),
				ThreeValue.OrUnspecified(_anisotropy),
				ThreeValue.OrUnspecified(_colorSpace)
			]);
		}
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="DataTexture"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isDataTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isDataTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsDataTextureAsync()
	{
		return GetAsync<bool>("isDataTexture");
	}
}
