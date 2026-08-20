// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.VideoFrameTexture</c>.</summary>
public sealed class VideoFrameTexture : Texture
{
	private readonly Mapping? _mapping;
	private readonly Wrapping? _wrapS;
	private readonly Wrapping? _wrapT;
	private readonly MagnificationTextureFilter? _magFilter;
	private readonly MinificationTextureFilter? _minFilter;
	private readonly PixelFormat? _format;
	private readonly TextureDataType? _type;
	private readonly float? _anisotropy;

	/// <summary>Initializes a new <see cref="VideoFrameTexture"/>.</summary>
	/// <param name="mapping">Value forwarded to the <c>mapping</c> constructor argument.</param>
	/// <param name="wrapS">Value forwarded to the <c>wrapS</c> constructor argument.</param>
	/// <param name="wrapT">Value forwarded to the <c>wrapT</c> constructor argument.</param>
	/// <param name="magFilter">Value forwarded to the <c>magFilter</c> constructor argument.</param>
	/// <param name="minFilter">Value forwarded to the <c>minFilter</c> constructor argument.</param>
	/// <param name="format">Value forwarded to the <c>format</c> constructor argument.</param>
	/// <param name="type">Value forwarded to the <c>type</c> constructor argument.</param>
	/// <param name="anisotropy">Value forwarded to the <c>anisotropy</c> constructor argument.</param>
	public VideoFrameTexture(
		Mapping? mapping = null,
		Wrapping? wrapS = null,
		Wrapping? wrapT = null,
		MagnificationTextureFilter? magFilter = null,
		MinificationTextureFilter? minFilter = null,
		PixelFormat? format = null,
		TextureDataType? type = null,
		float? anisotropy = null)
		: base(mapping: mapping, wrapS: wrapS, wrapT: wrapT, magFilter: magFilter, minFilter: minFilter, format: format, type: type, anisotropy: anisotropy)
	{
		_mapping = mapping;
		_wrapS = wrapS;
		_wrapT = wrapT;
		_magFilter = magFilter;
		_minFilter = minFilter;
		_format = format;
		_type = type;
		_anisotropy = anisotropy;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>VideoFrameTexture</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal VideoFrameTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.VideoFrameTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "VideoFrameTexture"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.VideoFrameTexture</c>: mapping, wrapS, wrapT,
	/// magFilter, minFilter, format, type, anisotropy. An argument the caller left unspecified travels
	/// as the wire's not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js
	/// applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_mapping),
				ThreeValue.OrUnspecified(_wrapS),
				ThreeValue.OrUnspecified(_wrapT),
				ThreeValue.OrUnspecified(_magFilter),
				ThreeValue.OrUnspecified(_minFilter),
				ThreeValue.OrUnspecified(_format),
				ThreeValue.OrUnspecified(_type),
				ThreeValue.OrUnspecified(_anisotropy)
			]);
		}
	}

	/// <summary>Records a call to <c>setFrame</c> on the JavaScript-side object.</summary>
	/// <param name="frame">Value forwarded to the <c>frame</c> argument.</param>
	public void SetFrame(object? frame)
	{
		RecordCall("setFrame", frame);
	}

	/// <summary>
	/// This is called automatically and sets <c>.needsUpdate</c> to <c>true</c> every time a new frame
	/// is available.
	/// </summary>
	public void Update()
	{
		RecordCall("update");
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <c>VideoTexture</c>. Read-only in three.js,
	/// so it is read on demand rather than mirrored: records a get op, sends it behind every write
	/// already pending, and completes with the value <c>isVideoTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isVideoTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsVideoTextureAsync()
	{
		return GetAsync<bool>("isVideoTexture");
	}
}
