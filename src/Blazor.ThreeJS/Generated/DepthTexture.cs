// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class can be used to automatically save the depth information of a rendering into a
/// texture. The JavaScript-side <c>THREE.DepthTexture</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl_depth_texture">depth / texture</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/textures/DepthTexture">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/textures/DepthTexture.js">Source</seealso>
public class DepthTexture : Texture
{
	private readonly float? _width;
	private readonly float? _height;
	private readonly TextureDataType? _type;
	private readonly Mapping? _mapping;
	private readonly Wrapping? _wrapS;
	private readonly Wrapping? _wrapT;
	private readonly MagnificationTextureFilter? _magFilter;
	private readonly MinificationTextureFilter? _minFilter;
	private readonly float? _anisotropy;
	private readonly DepthTexturePixelFormat? _format;
	private readonly float _depth;
	private TextureComparisonFunction? _compareFunction;
	private bool _isCompareFunctionWritten;

	/// <summary>Create a new instance of <see cref="DepthTexture"/>.</summary>
	/// <param name="width">Width of the texture.</param>
	/// <param name="height">Height of the texture.</param>
	/// <param name="type">
	/// See <c>.type</c>. Default <c>THREE.UnsignedByteType</c> or <c>THREE.UnsignedInt248Type</c>.
	/// </param>
	/// <param name="mapping">See <c>.mapping</c>. Default <c>THREE.Texture.DEFAULT_MAPPING</c>.</param>
	/// <param name="wrapS">See <c>.wrapS</c>. Default <c>THREE.ClampToEdgeWrapping</c>.</param>
	/// <param name="wrapT">See <c>.wrapT</c>. Default <c>THREE.ClampToEdgeWrapping</c>.</param>
	/// <param name="magFilter">See <c>.magFilter</c>. Default <c>THREE.NearestFilter</c>.</param>
	/// <param name="minFilter">See <c>.minFilter</c>. Default <c>THREE.NearestFilter</c>.</param>
	/// <param name="anisotropy">See <c>.anisotropy</c>. Default <c>THREE.Texture.DEFAULT_ANISOTROPY</c>.</param>
	/// <param name="format">See <c>.format</c>. Default <c>THREE.DepthFormat</c>.</param>
	/// <param name="depth">The depth of the texture.</param>
	public DepthTexture(
		float? width = null,
		float? height = null,
		TextureDataType? type = null,
		Mapping? mapping = null,
		Wrapping? wrapS = null,
		Wrapping? wrapT = null,
		MagnificationTextureFilter? magFilter = null,
		MinificationTextureFilter? minFilter = null,
		float? anisotropy = null,
		DepthTexturePixelFormat? format = null,
		float depth = 1f)
	{
		_width = width;
		_height = height;
		_type = type;
		_mapping = mapping;
		_wrapS = wrapS;
		_wrapT = wrapT;
		_magFilter = magFilter;
		_minFilter = minFilter;
		_anisotropy = anisotropy;
		_format = format;
		_depth = depth;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>DepthTexture</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal DepthTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_depth = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.DepthTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "DepthTexture"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.DepthTexture</c>: width, height, type, mapping,
	/// wrapS, wrapT, magFilter, minFilter, anisotropy, format, depth. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_width),
				ThreeValue.OrUnspecified(_height),
				ThreeValue.OrUnspecified(_type),
				ThreeValue.OrUnspecified(_mapping),
				ThreeValue.OrUnspecified(_wrapS),
				ThreeValue.OrUnspecified(_wrapT),
				ThreeValue.OrUnspecified(_magFilter),
				ThreeValue.OrUnspecified(_minFilter),
				ThreeValue.OrUnspecified(_anisotropy),
				ThreeValue.OrUnspecified(_format),
				_depth
			]);
		}
	}

	/// <summary>
	/// This is used to define the comparison function used when comparing texels in the depth texture
	/// to the value in the depth buffer. Default is <c>null</c> which means comparison is disabled. See
	/// <c>THREE.TextureComparisonFunction</c> for functions. Writing it records a
	/// <c>compareFunction</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public TextureComparisonFunction? CompareFunction
	{
		get { return _compareFunction; }
		set
		{
			if (_compareFunction == value)
			{
				return;
			}

			_compareFunction = value;
			_isCompareFunctionWritten = true;
			RecordSet("compareFunction", value);
		}
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="DepthTexture"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isDepthTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isDepthTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsDepthTextureAsync()
	{
		return GetAsync<bool>("isDepthTexture");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.DepthTexture</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isCompareFunctionWritten)
		{
			batch.Set(Handle, "compareFunction", ThreeValue.Encode(_compareFunction));
		}
	}
}
