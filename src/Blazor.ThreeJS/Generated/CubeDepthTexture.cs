// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.CubeDepthTexture</c>.</summary>
public sealed class CubeDepthTexture : DepthTexture
{
	private readonly float _size;
	private readonly TextureDataType? _type;
	private readonly Mapping? _mapping;
	private readonly Wrapping? _wrapS;
	private readonly Wrapping? _wrapT;
	private readonly MagnificationTextureFilter? _magFilter;
	private readonly MinificationTextureFilter? _minFilter;
	private readonly float? _anisotropy;
	private readonly DepthTexturePixelFormat? _format;

	/// <summary>Initializes a new <see cref="CubeDepthTexture"/>.</summary>
	/// <param name="size">Value forwarded to the <c>size</c> constructor argument.</param>
	/// <param name="type">Value forwarded to the <c>type</c> constructor argument.</param>
	/// <param name="mapping">Value forwarded to the <c>mapping</c> constructor argument.</param>
	/// <param name="wrapS">Value forwarded to the <c>wrapS</c> constructor argument.</param>
	/// <param name="wrapT">Value forwarded to the <c>wrapT</c> constructor argument.</param>
	/// <param name="magFilter">Value forwarded to the <c>magFilter</c> constructor argument.</param>
	/// <param name="minFilter">Value forwarded to the <c>minFilter</c> constructor argument.</param>
	/// <param name="anisotropy">Value forwarded to the <c>anisotropy</c> constructor argument.</param>
	/// <param name="format">Value forwarded to the <c>format</c> constructor argument.</param>
	public CubeDepthTexture(
		float size,
		TextureDataType? type = null,
		Mapping? mapping = null,
		Wrapping? wrapS = null,
		Wrapping? wrapT = null,
		MagnificationTextureFilter? magFilter = null,
		MinificationTextureFilter? minFilter = null,
		float? anisotropy = null,
		DepthTexturePixelFormat? format = null)
	{
		_size = size;
		_type = type;
		_mapping = mapping;
		_wrapS = wrapS;
		_wrapT = wrapT;
		_magFilter = magFilter;
		_minFilter = minFilter;
		_anisotropy = anisotropy;
		_format = format;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CubeDepthTexture</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CubeDepthTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_size = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CubeDepthTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CubeDepthTexture"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.CubeDepthTexture</c>: size, type, mapping, wrapS,
	/// wrapT, magFilter, minFilter, anisotropy, format. An argument the caller left unspecified travels
	/// as the wire's not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js
	/// applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_size,
				ThreeValue.OrUnspecified(_type),
				ThreeValue.OrUnspecified(_mapping),
				ThreeValue.OrUnspecified(_wrapS),
				ThreeValue.OrUnspecified(_wrapT),
				ThreeValue.OrUnspecified(_magFilter),
				ThreeValue.OrUnspecified(_minFilter),
				ThreeValue.OrUnspecified(_anisotropy),
				ThreeValue.OrUnspecified(_format)
			]);
		}
	}

	/// <summary>
	/// Reads <c>isCubeDepthTexture</c> back from the JavaScript-side object. Read-only in three.js, so
	/// it is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isCubeDepthTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isCubeDepthTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsCubeDepthTextureAsync()
	{
		return GetAsync<bool>("isCubeDepthTexture");
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
