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
public class DepthTexture : EventDispatcher
{
	private readonly float? _width;
	private readonly float? _height;
	private TextureDataType? _type;
	private readonly Mapping? _mapping;
	private Wrapping? _wrapS;
	private Wrapping? _wrapT;
	private MagnificationTextureFilter? _magFilter;
	private MinificationTextureFilter? _minFilter;
	private float? _anisotropy;
	private DepthTexturePixelFormat? _format;
	private readonly float _depth;
	private bool _flipY = false;
	private bool _generateMipmaps = false;
	private TextureComparisonFunction? _compareFunction;
	private string _uuid = string.Empty;
	private string _name = string.Empty;
	private float _channel;
	private bool _matrixAutoUpdate = true;
	private float _rotation = 0f;
	private bool _premultiplyAlpha = false;
	private float _unpackAlignment = 4f;
	private string _colorSpace = string.Empty;
	private bool _isRenderTargetTexture = false;
	private bool _isArrayTexture = false;
	private int _version = 0;
	private float _pmremVersion;
	private bool _normalized = false;
	private bool _needsUpdate;
	private bool _needsPMREMUpdate = false;
	private RenderTarget? _renderTarget;
	private bool _isFlipYWritten;
	private bool _isMagFilterWritten;
	private bool _isMinFilterWritten;
	private bool _isGenerateMipmapsWritten;
	private bool _isFormatWritten;
	private bool _isTypeWritten;
	private bool _isCompareFunctionWritten;
	private bool _isUuidWritten;
	private bool _isNameWritten;
	private bool _isChannelWritten;
	private bool _isWrapSWritten;
	private bool _isWrapTWritten;
	private bool _isAnisotropyWritten;
	private bool _isMatrixAutoUpdateWritten;
	private bool _isRotationWritten;
	private bool _isPremultiplyAlphaWritten;
	private bool _isUnpackAlignmentWritten;
	private bool _isColorSpaceWritten;
	private bool _isIsRenderTargetTextureWritten;
	private bool _isIsArrayTextureWritten;
	private bool _isVersionWritten;
	private bool _isPmremVersionWritten;
	private bool _isNormalizedWritten;
	private bool _isNeedsUpdateWritten;
	private bool _isNeedsPMREMUpdateWritten;
	private bool _isRenderTargetWritten;

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
	/// The <c>flipY</c> property of the JavaScript-side object. Writing it records a <c>flipY</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool FlipY
	{
		get { return _flipY; }
		set
		{
			if (_flipY == value)
			{
				return;
			}

			_flipY = value;
			_isFlipYWritten = true;
			RecordSet("flipY", value);
		}
	}

	/// <summary>
	/// The <c>magFilter</c> property of the JavaScript-side object. Writing it records a
	/// <c>magFilter</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public MagnificationTextureFilter? MagFilter
	{
		get { return _magFilter; }
		set
		{
			if (_magFilter == value)
			{
				return;
			}

			_magFilter = value;
			_isMagFilterWritten = true;
			RecordSet("magFilter", value);
		}
	}

	/// <summary>
	/// The <c>minFilter</c> property of the JavaScript-side object. Writing it records a
	/// <c>minFilter</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public MinificationTextureFilter? MinFilter
	{
		get { return _minFilter; }
		set
		{
			if (_minFilter == value)
			{
				return;
			}

			_minFilter = value;
			_isMinFilterWritten = true;
			RecordSet("minFilter", value);
		}
	}

	/// <summary>
	/// The <c>generateMipmaps</c> property of the JavaScript-side object. Writing it records a
	/// <c>generateMipmaps</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public bool GenerateMipmaps
	{
		get { return _generateMipmaps; }
		set
		{
			if (_generateMipmaps == value)
			{
				return;
			}

			_generateMipmaps = value;
			_isGenerateMipmapsWritten = true;
			RecordSet("generateMipmaps", value);
		}
	}

	/// <summary>
	/// The <c>format</c> property of the JavaScript-side object. Writing it records a <c>format</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public DepthTexturePixelFormat? Format
	{
		get { return _format; }
		set
		{
			if (_format == value)
			{
				return;
			}

			_format = value;
			_isFormatWritten = true;
			RecordSet("format", value);
		}
	}

	/// <summary>
	/// The <c>type</c> property of the JavaScript-side object. Writing it records a <c>type</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public TextureDataType? Type
	{
		get { return _type; }
		set
		{
			if (_type == value)
			{
				return;
			}

			_type = value;
			_isTypeWritten = true;
			RecordSet("type", value);
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
	/// <see href="http://en.wikipedia.org/wiki/Universally_unique_identifier">UUID</see> of this object
	/// instance. Writing it records a <c>uuid</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public string Uuid
	{
		get { return _uuid; }
		set
		{
			if (_uuid == value)
			{
				return;
			}

			_uuid = value;
			_isUuidWritten = true;
			RecordSet("uuid", value);
		}
	}

	/// <summary>
	/// Optional name of the object. Writing it records a <c>name</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public string Name
	{
		get { return _name; }
		set
		{
			if (_name == value)
			{
				return;
			}

			_name = value;
			_isNameWritten = true;
			RecordSet("name", value);
		}
	}

	/// <summary>
	/// Lets you select the uv attribute to map the texture to. <c>0</c> for <c>uv</c>, <c>1</c> for
	/// <c>uv1</c>, <c>2</c> for <c>uv2</c> and <c>3</c> for <c>uv3</c>. Writing it records a
	/// <c>channel</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Channel
	{
		get { return _channel; }
		set
		{
			if (_channel == value)
			{
				return;
			}

			_channel = value;
			_isChannelWritten = true;
			RecordSet("channel", value);
		}
	}

	/// <summary>
	/// This defines how the <c>Texture</c> is wrapped *horizontally* and corresponds to **U** in UV
	/// mapping. Writing it records a <c>wrapS</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public Wrapping? WrapS
	{
		get { return _wrapS; }
		set
		{
			if (_wrapS == value)
			{
				return;
			}

			_wrapS = value;
			_isWrapSWritten = true;
			RecordSet("wrapS", value);
		}
	}

	/// <summary>
	/// This defines how the <c>Texture</c> is wrapped *vertically* and corresponds to **V** in UV
	/// mapping. Writing it records a <c>wrapT</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public Wrapping? WrapT
	{
		get { return _wrapT; }
		set
		{
			if (_wrapT == value)
			{
				return;
			}

			_wrapT = value;
			_isWrapTWritten = true;
			RecordSet("wrapT", value);
		}
	}

	/// <summary>
	/// The number of samples taken along the axis through the pixel that has the highest density of
	/// texels. Writing it records a <c>anisotropy</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public float? Anisotropy
	{
		get { return _anisotropy; }
		set
		{
			if (_anisotropy == value)
			{
				return;
			}

			_anisotropy = value;
			_isAnisotropyWritten = true;
			RecordSet("anisotropy", value);
		}
	}

	/// <summary>
	/// Whether is to update the texture's uv-transform <c>.matrix</c>. Writing it records a
	/// <c>matrixAutoUpdate</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public bool MatrixAutoUpdate
	{
		get { return _matrixAutoUpdate; }
		set
		{
			if (_matrixAutoUpdate == value)
			{
				return;
			}

			_matrixAutoUpdate = value;
			_isMatrixAutoUpdateWritten = true;
			RecordSet("matrixAutoUpdate", value);
		}
	}

	/// <summary>
	/// How much the texture is rotated around the center point, in radians. Writing it records a
	/// <c>rotation</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Rotation
	{
		get { return _rotation; }
		set
		{
			if (_rotation == value)
			{
				return;
			}

			_rotation = value;
			_isRotationWritten = true;
			RecordSet("rotation", value);
		}
	}

	/// <summary>
	/// If set to <c>true</c>, the alpha channel, if present, is multiplied into the color channels when
	/// the texture is uploaded to the GPU. Writing it records a <c>premultiplyAlpha</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool PremultiplyAlpha
	{
		get { return _premultiplyAlpha; }
		set
		{
			if (_premultiplyAlpha == value)
			{
				return;
			}

			_premultiplyAlpha = value;
			_isPremultiplyAlphaWritten = true;
			RecordSet("premultiplyAlpha", value);
		}
	}

	/// <summary>
	/// Specifies the alignment requirements for the start of each pixel row in memory. Writing it
	/// records a <c>unpackAlignment</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float UnpackAlignment
	{
		get { return _unpackAlignment; }
		set
		{
			if (_unpackAlignment == value)
			{
				return;
			}

			_unpackAlignment = value;
			_isUnpackAlignmentWritten = true;
			RecordSet("unpackAlignment", value);
		}
	}

	/// <summary>
	/// The <c>{@link Texture</c> constants} page for details of other color spaces. Writing it records
	/// a <c>colorSpace</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public string ColorSpace
	{
		get { return _colorSpace; }
		set
		{
			if (_colorSpace == value)
			{
				return;
			}

			_colorSpace = value;
			_isColorSpaceWritten = true;
			RecordSet("colorSpace", value);
		}
	}

	/// <summary>
	/// Indicates whether a texture belongs to a render target or not. Writing it records a
	/// <c>isRenderTargetTexture</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public bool IsRenderTargetTexture
	{
		get { return _isRenderTargetTexture; }
		set
		{
			if (_isRenderTargetTexture == value)
			{
				return;
			}

			_isRenderTargetTexture = value;
			_isIsRenderTargetTextureWritten = true;
			RecordSet("isRenderTargetTexture", value);
		}
	}

	/// <summary>
	/// Indicates if a texture should be handled like a texture array. Writing it records a
	/// <c>isArrayTexture</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public bool IsArrayTexture
	{
		get { return _isArrayTexture; }
		set
		{
			if (_isArrayTexture == value)
			{
				return;
			}

			_isArrayTexture = value;
			_isIsArrayTextureWritten = true;
			RecordSet("isArrayTexture", value);
		}
	}

	/// <summary>
	/// This starts at <c>0</c> and counts how many times <c>.needsUpdate</c> is set to <c>true</c>.
	/// Writing it records a <c>version</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public int Version
	{
		get { return _version; }
		set
		{
			if (_version == value)
			{
				return;
			}

			_version = value;
			_isVersionWritten = true;
			RecordSet("version", value);
		}
	}

	/// <summary>
	/// Indicates whether this texture should be processed by PMREMGenerator or not (only relevant for
	/// render target textures). Writing it records a <c>pmremVersion</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float PmremVersion
	{
		get { return _pmremVersion; }
		set
		{
			if (_pmremVersion == value)
			{
				return;
			}

			_pmremVersion = value;
			_isPmremVersionWritten = true;
			RecordSet("pmremVersion", value);
		}
	}

	/// <summary>
	/// Whether the texture should use one of the 16 bit integer formats which are normalized to [0, 1]
	/// or [-1, 1] (depending on signed/unsigned) when sampled. Writing it records a <c>normalized</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Normalized
	{
		get { return _normalized; }
		set
		{
			if (_normalized == value)
			{
				return;
			}

			_normalized = value;
			_isNormalizedWritten = true;
			RecordSet("normalized", value);
		}
	}

	/// <summary>
	/// Set this to <c>true</c> to trigger an update next time the texture is used. Particularly
	/// important for setting the wrap mode. Writing it records a <c>needsUpdate</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool NeedsUpdate
	{
		get { return _needsUpdate; }
		set
		{
			if (_needsUpdate == value)
			{
				return;
			}

			_needsUpdate = value;
			_isNeedsUpdateWritten = true;
			RecordSet("needsUpdate", value);
		}
	}

	/// <summary>
	/// Indicates whether this texture should be processed by <c>THREE.PMREMGenerator</c> or not.
	/// Writing it records a <c>needsPMREMUpdate</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public bool NeedsPMREMUpdate
	{
		get { return _needsPMREMUpdate; }
		set
		{
			if (_needsPMREMUpdate == value)
			{
				return;
			}

			_needsPMREMUpdate = value;
			_isNeedsPMREMUpdateWritten = true;
			RecordSet("needsPMREMUpdate", value);
		}
	}

	/// <summary>
	/// The <c>renderTarget</c> property of the JavaScript-side object. Writing it records a
	/// <c>renderTarget</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public RenderTarget? RenderTarget
	{
		get { return _renderTarget; }
		set
		{
			if (ReferenceEquals(_renderTarget, value))
			{
				return;
			}

			_renderTarget = value;
			_isRenderTargetWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("renderTarget", value);
		}
	}

	/// <summary>
	/// Update the texture's **UV-transform** <c>.matrix</c> from the texture properties <c>.offset</c>,
	/// <c>.repeat</c>, <c>.rotation</c> and <c>.center</c>.
	/// </summary>
	public void UpdateMatrix()
	{
		RecordCall("updateMatrix");
	}

	/// <summary>Adds a range of data in the data texture to be updated on the GPU.</summary>
	/// <param name="start">Position at which to start update.</param>
	/// <param name="count">The number of components to update.</param>
	public void AddUpdateRange(float start, int count)
	{
		RecordCall("addUpdateRange", start, count);
	}

	/// <summary>Clears the update ranges.</summary>
	public void ClearUpdateRanges()
	{
		RecordCall("clearUpdateRanges");
	}

	/// <summary>Frees the GPU-related resources allocated by this instance.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.DepthTexture</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isFlipYWritten)
		{
			batch.Set(Handle, "flipY", ThreeValue.Encode(_flipY));
		}

		if (_isMagFilterWritten)
		{
			batch.Set(Handle, "magFilter", ThreeValue.Encode(_magFilter));
		}

		if (_isMinFilterWritten)
		{
			batch.Set(Handle, "minFilter", ThreeValue.Encode(_minFilter));
		}

		if (_isGenerateMipmapsWritten)
		{
			batch.Set(Handle, "generateMipmaps", ThreeValue.Encode(_generateMipmaps));
		}

		if (_isFormatWritten)
		{
			batch.Set(Handle, "format", ThreeValue.Encode(_format));
		}

		if (_isTypeWritten)
		{
			batch.Set(Handle, "type", ThreeValue.Encode(_type));
		}

		if (_isCompareFunctionWritten)
		{
			batch.Set(Handle, "compareFunction", ThreeValue.Encode(_compareFunction));
		}

		if (_isUuidWritten)
		{
			batch.Set(Handle, "uuid", ThreeValue.Encode(_uuid));
		}

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isChannelWritten)
		{
			batch.Set(Handle, "channel", ThreeValue.Encode(_channel));
		}

		if (_isWrapSWritten)
		{
			batch.Set(Handle, "wrapS", ThreeValue.Encode(_wrapS));
		}

		if (_isWrapTWritten)
		{
			batch.Set(Handle, "wrapT", ThreeValue.Encode(_wrapT));
		}

		if (_isAnisotropyWritten)
		{
			batch.Set(Handle, "anisotropy", ThreeValue.Encode(_anisotropy));
		}

		if (_isMatrixAutoUpdateWritten)
		{
			batch.Set(Handle, "matrixAutoUpdate", ThreeValue.Encode(_matrixAutoUpdate));
		}

		if (_isRotationWritten)
		{
			batch.Set(Handle, "rotation", ThreeValue.Encode(_rotation));
		}

		if (_isPremultiplyAlphaWritten)
		{
			batch.Set(Handle, "premultiplyAlpha", ThreeValue.Encode(_premultiplyAlpha));
		}

		if (_isUnpackAlignmentWritten)
		{
			batch.Set(Handle, "unpackAlignment", ThreeValue.Encode(_unpackAlignment));
		}

		if (_isColorSpaceWritten)
		{
			batch.Set(Handle, "colorSpace", ThreeValue.Encode(_colorSpace));
		}

		if (_isIsRenderTargetTextureWritten)
		{
			batch.Set(Handle, "isRenderTargetTexture", ThreeValue.Encode(_isRenderTargetTexture));
		}

		if (_isIsArrayTextureWritten)
		{
			batch.Set(Handle, "isArrayTexture", ThreeValue.Encode(_isArrayTexture));
		}

		if (_isVersionWritten)
		{
			batch.Set(Handle, "version", ThreeValue.Encode(_version));
		}

		if (_isPmremVersionWritten)
		{
			batch.Set(Handle, "pmremVersion", ThreeValue.Encode(_pmremVersion));
		}

		if (_isNormalizedWritten)
		{
			batch.Set(Handle, "normalized", ThreeValue.Encode(_normalized));
		}

		if (_isNeedsUpdateWritten)
		{
			batch.Set(Handle, "needsUpdate", ThreeValue.Encode(_needsUpdate));
		}

		if (_isNeedsPMREMUpdateWritten)
		{
			batch.Set(Handle, "needsPMREMUpdate", ThreeValue.Encode(_needsPMREMUpdate));
		}

		if (_isRenderTargetWritten)
		{
			batch.Set(Handle, "renderTarget", ThreeValue.Encode(_renderTarget));
		}
	}
}
