// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This special type of texture is intended for compute shaders. It can be used to compute the data
/// of a texture with a compute shader. Note: This type of texture can only be used with
/// <c>WebGPURenderer</c> and a WebGPU backend. The JavaScript-side <c>THREE.Storage3DTexture</c>.
/// </summary>
public sealed class Storage3DTexture : EventDispatcher
{
	private readonly float _width;
	private readonly float _height;
	private readonly float _depth;
	private Wrapping _wrapR;
	private bool _is3DTexture;
	private string _uuid = string.Empty;
	private string _name = string.Empty;
	private float _channel;
	private Wrapping _wrapS;
	private Wrapping _wrapT;
	private MagnificationTextureFilter _magFilter;
	private MinificationTextureFilter _minFilter;
	private float _anisotropy;
	private TextureDataType _type;
	private bool _matrixAutoUpdate = true;
	private float _rotation = 0f;
	private bool _generateMipmaps = true;
	private bool _premultiplyAlpha = false;
	private bool _flipY = true;
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
	private bool _isWrapRWritten;
	private bool _isIs3DTextureWritten;
	private bool _isUuidWritten;
	private bool _isNameWritten;
	private bool _isChannelWritten;
	private bool _isWrapSWritten;
	private bool _isWrapTWritten;
	private bool _isMagFilterWritten;
	private bool _isMinFilterWritten;
	private bool _isAnisotropyWritten;
	private bool _isTypeWritten;
	private bool _isMatrixAutoUpdateWritten;
	private bool _isRotationWritten;
	private bool _isGenerateMipmapsWritten;
	private bool _isPremultiplyAlphaWritten;
	private bool _isFlipYWritten;
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

	/// <summary>Constructs a new storage texture.</summary>
	/// <param name="width">The storage texture's width.</param>
	/// <param name="height">The storage texture's height.</param>
	/// <param name="depth">The storage texture's depth.</param>
	public Storage3DTexture(float width = 1f, float height = 1f, float depth = 1f)
	{
		_width = width;
		_height = height;
		_depth = depth;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Storage3DTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Storage3DTexture"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.Storage3DTexture</c>: width, height, depth.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_width, _height, _depth]; }
	}

	/// <summary>
	/// This defines how the texture is wrapped in the depth direction and corresponds to *W* in UVW
	/// mapping. Writing it records a <c>wrapR</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public Wrapping WrapR
	{
		get { return _wrapR; }
		set
		{
			if (_wrapR == value)
			{
				return;
			}

			_wrapR = value;
			_isWrapRWritten = true;
			RecordSet("wrapR", value);
		}
	}

	/// <summary>
	/// Indicates whether this texture is a 3D texture. Writing it records a <c>is3DTexture</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Is3DTexture
	{
		get { return _is3DTexture; }
		set
		{
			if (_is3DTexture == value)
			{
				return;
			}

			_is3DTexture = value;
			_isIs3DTextureWritten = true;
			RecordSet("is3DTexture", value);
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
	public Wrapping WrapS
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
	public Wrapping WrapT
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
	/// How the <c>Texture</c> is sampled when a texel covers more than one pixel. Writing it records a
	/// <c>magFilter</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public MagnificationTextureFilter MagFilter
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
	/// How the <c>Texture</c> is sampled when a texel covers less than one pixel. Writing it records a
	/// <c>minFilter</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public MinificationTextureFilter MinFilter
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
	/// The number of samples taken along the axis through the pixel that has the highest density of
	/// texels. Writing it records a <c>anisotropy</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public float Anisotropy
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
	/// This must correspond to the <c>.format</c>. Writing it records a <c>type</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public TextureDataType Type
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
	/// Whether to generate mipmaps, _(if possible)_ for a texture. Writing it records a
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
	/// If set to <c>true</c>, the texture is flipped along the vertical axis when uploaded to the GPU.
	/// Writing it records a <c>flipY</c> property write once this object is attached; writing the value
	/// already held records nothing.
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

	/// <summary>Records a call to <c>setSize</c> on the JavaScript-side object.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> argument.</param>
	/// <param name="depth">Value forwarded to the <c>depth</c> argument.</param>
	public void SetSize(float width, float height, float depth)
	{
		RecordCall("setSize", width, height, depth);
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
	/// Emits the create op for <c>THREE.Storage3DTexture</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isWrapRWritten)
		{
			batch.Set(Handle, "wrapR", ThreeValue.Encode(_wrapR));
		}

		if (_isIs3DTextureWritten)
		{
			batch.Set(Handle, "is3DTexture", ThreeValue.Encode(_is3DTexture));
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

		if (_isMagFilterWritten)
		{
			batch.Set(Handle, "magFilter", ThreeValue.Encode(_magFilter));
		}

		if (_isMinFilterWritten)
		{
			batch.Set(Handle, "minFilter", ThreeValue.Encode(_minFilter));
		}

		if (_isAnisotropyWritten)
		{
			batch.Set(Handle, "anisotropy", ThreeValue.Encode(_anisotropy));
		}

		if (_isTypeWritten)
		{
			batch.Set(Handle, "type", ThreeValue.Encode(_type));
		}

		if (_isMatrixAutoUpdateWritten)
		{
			batch.Set(Handle, "matrixAutoUpdate", ThreeValue.Encode(_matrixAutoUpdate));
		}

		if (_isRotationWritten)
		{
			batch.Set(Handle, "rotation", ThreeValue.Encode(_rotation));
		}

		if (_isGenerateMipmapsWritten)
		{
			batch.Set(Handle, "generateMipmaps", ThreeValue.Encode(_generateMipmaps));
		}

		if (_isPremultiplyAlphaWritten)
		{
			batch.Set(Handle, "premultiplyAlpha", ThreeValue.Encode(_premultiplyAlpha));
		}

		if (_isFlipYWritten)
		{
			batch.Set(Handle, "flipY", ThreeValue.Encode(_flipY));
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
