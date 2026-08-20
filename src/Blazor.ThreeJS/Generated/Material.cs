// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Abstract base class for materials. Materials define the appearance of renderable 3D objects. The
/// JavaScript-side <c>THREE.Material</c>.
/// </summary>
public class Material : EventDispatcher
{
	private string _type = string.Empty;
	private bool _needsUpdate = false;
	private string _name = string.Empty;
	private Blending _blending = Blending.NormalBlending;
	private Side _side = Side.FrontSide;
	private bool _vertexColors = false;
	private float _opacity = 1f;
	private bool _transparent = false;
	private bool _alphaHash = false;
	private BlendingDstFactor _blendDst = BlendingDstFactor.OneMinusSrcAlphaFactor;
	private BlendingEquation _blendEquation = BlendingEquation.AddEquation;
	private BlendingDstFactor? _blendDstAlpha = null;
	private BlendingEquation? _blendEquationAlpha = null;
	private float _blendAlpha = 0f;
	private DepthModes _depthFunc = DepthModes.LessEqualDepth;
	private bool _depthTest = true;
	private bool _depthWrite = true;
	private float _stencilWriteMask;
	private StencilFunc _stencilFunc = StencilFunc.AlwaysStencilFunc;
	private float _stencilRef = 0f;
	private float _stencilFuncMask;
	private StencilOp _stencilFail = StencilOp.KeepStencilOp;
	private StencilOp _stencilZFail = StencilOp.KeepStencilOp;
	private StencilOp _stencilZPass = StencilOp.KeepStencilOp;
	private bool _stencilWrite = false;
	private Plane[]? _clippingPlanes = null;
	private bool _clipIntersection = false;
	private bool _clipShadows = false;
	private Side? _shadowSide = null;
	private bool _colorWrite = true;
	private ShaderPrecision? _precision = null;
	private bool _polygonOffset = false;
	private float _polygonOffsetFactor = 0f;
	private float _polygonOffsetUnits = 0f;
	private bool _dithering = false;
	private bool _alphaToCoverage = false;
	private bool _premultipliedAlpha = false;
	private bool _forceSinglePass = false;
	private bool _allowOverride = true;
	private bool _visible = true;
	private bool _toneMapped = true;
	private float _alphaTest;
	private bool _isTypeWritten;
	private bool _isNeedsUpdateWritten;
	private bool _isNameWritten;
	private bool _isBlendingWritten;
	private bool _isSideWritten;
	private bool _isVertexColorsWritten;
	private bool _isOpacityWritten;
	private bool _isTransparentWritten;
	private bool _isAlphaHashWritten;
	private bool _isBlendDstWritten;
	private bool _isBlendEquationWritten;
	private bool _isBlendDstAlphaWritten;
	private bool _isBlendEquationAlphaWritten;
	private bool _isBlendColorWritten;
	private bool _isBlendAlphaWritten;
	private bool _isDepthFuncWritten;
	private bool _isDepthTestWritten;
	private bool _isDepthWriteWritten;
	private bool _isStencilWriteMaskWritten;
	private bool _isStencilFuncWritten;
	private bool _isStencilRefWritten;
	private bool _isStencilFuncMaskWritten;
	private bool _isStencilFailWritten;
	private bool _isStencilZFailWritten;
	private bool _isStencilZPassWritten;
	private bool _isStencilWriteWritten;
	private bool _isClippingPlanesWritten;
	private bool _isClipIntersectionWritten;
	private bool _isClipShadowsWritten;
	private bool _isShadowSideWritten;
	private bool _isColorWriteWritten;
	private bool _isPrecisionWritten;
	private bool _isPolygonOffsetWritten;
	private bool _isPolygonOffsetFactorWritten;
	private bool _isPolygonOffsetUnitsWritten;
	private bool _isDitheringWritten;
	private bool _isAlphaToCoverageWritten;
	private bool _isPremultipliedAlphaWritten;
	private bool _isForceSinglePassWritten;
	private bool _isAllowOverrideWritten;
	private bool _isVisibleWritten;
	private bool _isToneMappedWritten;
	private bool _isAlphaTestWritten;

	/// <summary>
	/// Represents the RGB values of the constant blend color. This property has only an effect when
	/// using custom blending with <c>ConstantColor</c> or <c>OneMinusConstantColor</c>. Mirrored as an
	/// instance this object owns: mutating it records a write of <c>blendColor</c>.
	/// </summary>
	public Color BlendColor { get; }

	/// <summary>Initializes a new <see cref="Material"/>.</summary>
	public Material()
	{
		BlendColor = new Color(0f, 0f, 0f);
		BlendColor.OnChange = () =>
		{
			_isBlendColorWritten = true;
			RecordSet("blendColor", BlendColor);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Material</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Material(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		BlendColor = new Color(0f, 0f, 0f);
		BlendColor.OnChange = () =>
		{
			_isBlendColorWritten = true;
			RecordSet("blendColor", BlendColor);
		};

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Material</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Material"; }
	}

	/// <summary>
	/// The type property is used for detecting the object type in context of
	/// serialization/deserialization. Writing it records a <c>type</c> property write once this object
	/// is attached; writing the value already held records nothing.
	/// </summary>
	public string Type
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
	/// Setting this property to <c>true</c> indicates the engine the material needs to be recompiled.
	/// Writing it records a <c>needsUpdate</c> property write once this object is attached; writing the
	/// value already held records nothing.
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
	/// The name of the material. Writing it records a <c>name</c> property write once this object is
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
	/// Defines the blending type of the material. It must be set to <c>CustomBlending</c> if custom
	/// blending properties like <c>Material#blendSrc</c>, <c>Material#blendDst</c> or
	/// <c>Material#blendEquation</c> should have any effect. Writing it records a <c>blending</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Blending Blending
	{
		get { return _blending; }
		set
		{
			if (_blending == value)
			{
				return;
			}

			_blending = value;
			_isBlendingWritten = true;
			RecordSet("blending", value);
		}
	}

	/// <summary>
	/// Defines which side of faces will be rendered - front, back or both. Writing it records a
	/// <c>side</c> property write once this object is attached; writing the value already held records
	/// nothing.
	/// </summary>
	public Side Side
	{
		get { return _side; }
		set
		{
			if (_side == value)
			{
				return;
			}

			_side = value;
			_isSideWritten = true;
			RecordSet("side", value);
		}
	}

	/// <summary>
	/// If set to <c>true</c>, vertex colors should be used. The engine supports RGB and RGBA vertex
	/// colors depending on whether a three (RGB) or four (RGBA) component color buffer attribute is
	/// used. Writing it records a <c>vertexColors</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public bool VertexColors
	{
		get { return _vertexColors; }
		set
		{
			if (_vertexColors == value)
			{
				return;
			}

			_vertexColors = value;
			_isVertexColorsWritten = true;
			RecordSet("vertexColors", value);
		}
	}

	/// <summary>
	/// Defines how transparent the material is. A value of <c>0.0</c> indicates fully transparent,
	/// <c>1.0</c> is fully opaque. If the <c>Material#transparent</c> is not set to <c>true</c>, the
	/// material will remain fully opaque and this value will only affect its color. Writing it records
	/// a <c>opacity</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Opacity
	{
		get { return _opacity; }
		set
		{
			if (_opacity == value)
			{
				return;
			}

			_opacity = value;
			_isOpacityWritten = true;
			RecordSet("opacity", value);
		}
	}

	/// <summary>
	/// Defines whether this material is transparent. This has an effect on rendering as transparent
	/// objects need special treatment and are rendered after non-transparent objects. When set to true,
	/// the extent to which the material is transparent is controlled by <c>Material#opacity</c>.
	/// Writing it records a <c>transparent</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public bool Transparent
	{
		get { return _transparent; }
		set
		{
			if (_transparent == value)
			{
				return;
			}

			_transparent = value;
			_isTransparentWritten = true;
			RecordSet("transparent", value);
		}
	}

	/// <summary>
	/// Enables alpha hashed transparency, an alternative to <c>Material#transparent</c> or
	/// <c>Material#alphaTest</c>. The material will not be rendered if opacity is lower than a random
	/// threshold. Randomization introduces some grain or noise, but approximates alpha blending without
	/// the associated problems of sorting. Using TAA can reduce the resulting noise. Writing it records
	/// a <c>alphaHash</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool AlphaHash
	{
		get { return _alphaHash; }
		set
		{
			if (_alphaHash == value)
			{
				return;
			}

			_alphaHash = value;
			_isAlphaHashWritten = true;
			RecordSet("alphaHash", value);
		}
	}

	/// <summary>
	/// Defines the blending destination factor. Writing it records a <c>blendDst</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public BlendingDstFactor BlendDst
	{
		get { return _blendDst; }
		set
		{
			if (_blendDst == value)
			{
				return;
			}

			_blendDst = value;
			_isBlendDstWritten = true;
			RecordSet("blendDst", value);
		}
	}

	/// <summary>
	/// Defines the blending equation. Writing it records a <c>blendEquation</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public BlendingEquation BlendEquation
	{
		get { return _blendEquation; }
		set
		{
			if (_blendEquation == value)
			{
				return;
			}

			_blendEquation = value;
			_isBlendEquationWritten = true;
			RecordSet("blendEquation", value);
		}
	}

	/// <summary>
	/// Defines the blending destination alpha factor. Writing it records a <c>blendDstAlpha</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public BlendingDstFactor? BlendDstAlpha
	{
		get { return _blendDstAlpha; }
		set
		{
			if (_blendDstAlpha == value)
			{
				return;
			}

			_blendDstAlpha = value;
			_isBlendDstAlphaWritten = true;
			RecordSet("blendDstAlpha", value);
		}
	}

	/// <summary>
	/// Defines the blending equation of the alpha channel. Writing it records a
	/// <c>blendEquationAlpha</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public BlendingEquation? BlendEquationAlpha
	{
		get { return _blendEquationAlpha; }
		set
		{
			if (_blendEquationAlpha == value)
			{
				return;
			}

			_blendEquationAlpha = value;
			_isBlendEquationAlphaWritten = true;
			RecordSet("blendEquationAlpha", value);
		}
	}

	/// <summary>
	/// Represents the alpha value of the constant blend color. This property has only an effect when
	/// using custom blending with <c>ConstantAlpha</c> or <c>OneMinusConstantAlpha</c>. Writing it
	/// records a <c>blendAlpha</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float BlendAlpha
	{
		get { return _blendAlpha; }
		set
		{
			if (_blendAlpha == value)
			{
				return;
			}

			_blendAlpha = value;
			_isBlendAlphaWritten = true;
			RecordSet("blendAlpha", value);
		}
	}

	/// <summary>
	/// Defines the depth function. Writing it records a <c>depthFunc</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public DepthModes DepthFunc
	{
		get { return _depthFunc; }
		set
		{
			if (_depthFunc == value)
			{
				return;
			}

			_depthFunc = value;
			_isDepthFuncWritten = true;
			RecordSet("depthFunc", value);
		}
	}

	/// <summary>
	/// Whether to have depth test enabled when rendering this material. When the depth test is
	/// disabled, the depth write will also be implicitly disabled. Writing it records a
	/// <c>depthTest</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool DepthTest
	{
		get { return _depthTest; }
		set
		{
			if (_depthTest == value)
			{
				return;
			}

			_depthTest = value;
			_isDepthTestWritten = true;
			RecordSet("depthTest", value);
		}
	}

	/// <summary>
	/// Whether rendering this material has any effect on the depth buffer. When drawing 2D overlays it
	/// can be useful to disable the depth writing in order to layer several things together without
	/// creating z-index artifacts. Writing it records a <c>depthWrite</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public bool DepthWrite
	{
		get { return _depthWrite; }
		set
		{
			if (_depthWrite == value)
			{
				return;
			}

			_depthWrite = value;
			_isDepthWriteWritten = true;
			RecordSet("depthWrite", value);
		}
	}

	/// <summary>
	/// The bit mask to use when writing to the stencil buffer. Writing it records a
	/// <c>stencilWriteMask</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float StencilWriteMask
	{
		get { return _stencilWriteMask; }
		set
		{
			if (_stencilWriteMask == value)
			{
				return;
			}

			_stencilWriteMask = value;
			_isStencilWriteMaskWritten = true;
			RecordSet("stencilWriteMask", value);
		}
	}

	/// <summary>
	/// The stencil comparison function to use. Writing it records a <c>stencilFunc</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public StencilFunc StencilFunc
	{
		get { return _stencilFunc; }
		set
		{
			if (_stencilFunc == value)
			{
				return;
			}

			_stencilFunc = value;
			_isStencilFuncWritten = true;
			RecordSet("stencilFunc", value);
		}
	}

	/// <summary>
	/// The value to use when performing stencil comparisons or stencil operations. Writing it records a
	/// <c>stencilRef</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float StencilRef
	{
		get { return _stencilRef; }
		set
		{
			if (_stencilRef == value)
			{
				return;
			}

			_stencilRef = value;
			_isStencilRefWritten = true;
			RecordSet("stencilRef", value);
		}
	}

	/// <summary>
	/// The bit mask to use when comparing against the stencil buffer. Writing it records a
	/// <c>stencilFuncMask</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float StencilFuncMask
	{
		get { return _stencilFuncMask; }
		set
		{
			if (_stencilFuncMask == value)
			{
				return;
			}

			_stencilFuncMask = value;
			_isStencilFuncMaskWritten = true;
			RecordSet("stencilFuncMask", value);
		}
	}

	/// <summary>
	/// Which stencil operation to perform when the comparison function returns <c>false</c>. Writing it
	/// records a <c>stencilFail</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public StencilOp StencilFail
	{
		get { return _stencilFail; }
		set
		{
			if (_stencilFail == value)
			{
				return;
			}

			_stencilFail = value;
			_isStencilFailWritten = true;
			RecordSet("stencilFail", value);
		}
	}

	/// <summary>
	/// Which stencil operation to perform when the comparison function returns <c>true</c> but the
	/// depth test fails. Writing it records a <c>stencilZFail</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public StencilOp StencilZFail
	{
		get { return _stencilZFail; }
		set
		{
			if (_stencilZFail == value)
			{
				return;
			}

			_stencilZFail = value;
			_isStencilZFailWritten = true;
			RecordSet("stencilZFail", value);
		}
	}

	/// <summary>
	/// Which stencil operation to perform when the comparison function returns <c>true</c> and the
	/// depth test passes. Writing it records a <c>stencilZPass</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public StencilOp StencilZPass
	{
		get { return _stencilZPass; }
		set
		{
			if (_stencilZPass == value)
			{
				return;
			}

			_stencilZPass = value;
			_isStencilZPassWritten = true;
			RecordSet("stencilZPass", value);
		}
	}

	/// <summary>
	/// Whether stencil operations are performed against the stencil buffer. In order to perform writes
	/// or comparisons against the stencil buffer this value must be <c>true</c>. Writing it records a
	/// <c>stencilWrite</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool StencilWrite
	{
		get { return _stencilWrite; }
		set
		{
			if (_stencilWrite == value)
			{
				return;
			}

			_stencilWrite = value;
			_isStencilWriteWritten = true;
			RecordSet("stencilWrite", value);
		}
	}

	/// <summary>
	/// User-defined clipping planes specified as THREE.Plane objects in world space. These planes apply
	/// to the objects this material is attached to. Points in space whose signed distance to the plane
	/// is negative are clipped (not rendered). This requires <c>WebGLRenderer#localClippingEnabled</c>
	/// to be <c>true</c>. Writing it records a <c>clippingPlanes</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public Plane[]? ClippingPlanes
	{
		get { return _clippingPlanes; }
		set
		{
			if (_clippingPlanes == value)
			{
				return;
			}

			_clippingPlanes = value;
			_isClippingPlanesWritten = true;
			RecordSet("clippingPlanes", value);
		}
	}

	/// <summary>
	/// Changes the behavior of clipping planes so that only their intersection is clipped, rather than
	/// their union. Writing it records a <c>clipIntersection</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public bool ClipIntersection
	{
		get { return _clipIntersection; }
		set
		{
			if (_clipIntersection == value)
			{
				return;
			}

			_clipIntersection = value;
			_isClipIntersectionWritten = true;
			RecordSet("clipIntersection", value);
		}
	}

	/// <summary>
	/// Defines whether to clip shadows according to the clipping planes specified on this material.
	/// Writing it records a <c>clipShadows</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public bool ClipShadows
	{
		get { return _clipShadows; }
		set
		{
			if (_clipShadows == value)
			{
				return;
			}

			_clipShadows = value;
			_isClipShadowsWritten = true;
			RecordSet("clipShadows", value);
		}
	}

	/// <summary>
	/// Defines which side of faces cast shadows. If <c>null</c>, the side casting shadows is determined
	/// as follows: - When <c>Material#side</c> is set to <c>FrontSide</c>, the back side cast shadows.
	/// - When <c>Material#side</c> is set to <c>BackSide</c>, the front side cast shadows. - When
	/// <c>Material#side</c> is set to <c>DoubleSide</c>, both sides cast shadows. Writing it records a
	/// <c>shadowSide</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public Side? ShadowSide
	{
		get { return _shadowSide; }
		set
		{
			if (_shadowSide == value)
			{
				return;
			}

			_shadowSide = value;
			_isShadowSideWritten = true;
			RecordSet("shadowSide", value);
		}
	}

	/// <summary>
	/// Whether to render the material's color. This can be used in conjunction with
	/// <c>Object3D#renderOder</c> to create invisible objects that occlude other objects. Writing it
	/// records a <c>colorWrite</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public bool ColorWrite
	{
		get { return _colorWrite; }
		set
		{
			if (_colorWrite == value)
			{
				return;
			}

			_colorWrite = value;
			_isColorWriteWritten = true;
			RecordSet("colorWrite", value);
		}
	}

	/// <summary>
	/// Override the renderer's default precision for this material. Writing it records a
	/// <c>precision</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public ShaderPrecision? Precision
	{
		get { return _precision; }
		set
		{
			if (_precision == value)
			{
				return;
			}

			_precision = value;
			_isPrecisionWritten = true;
			RecordSet("precision", value);
		}
	}

	/// <summary>
	/// Whether to use polygon offset or not. When enabled, each fragment's depth value will be offset
	/// after it is interpolated from the depth values of the appropriate vertices. The offset is added
	/// before the depth test is performed and before the value is written into the depth buffer. Can be
	/// useful for rendering hidden-line images, for applying decals to surfaces, and for rendering
	/// solids with highlighted edges. Writing it records a <c>polygonOffset</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool PolygonOffset
	{
		get { return _polygonOffset; }
		set
		{
			if (_polygonOffset == value)
			{
				return;
			}

			_polygonOffset = value;
			_isPolygonOffsetWritten = true;
			RecordSet("polygonOffset", value);
		}
	}

	/// <summary>
	/// Specifies a scale factor that is used to create a variable depth offset for each polygon.
	/// Writing it records a <c>polygonOffsetFactor</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public float PolygonOffsetFactor
	{
		get { return _polygonOffsetFactor; }
		set
		{
			if (_polygonOffsetFactor == value)
			{
				return;
			}

			_polygonOffsetFactor = value;
			_isPolygonOffsetFactorWritten = true;
			RecordSet("polygonOffsetFactor", value);
		}
	}

	/// <summary>
	/// Is multiplied by an implementation-specific value to create a constant depth offset. Writing it
	/// records a <c>polygonOffsetUnits</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public float PolygonOffsetUnits
	{
		get { return _polygonOffsetUnits; }
		set
		{
			if (_polygonOffsetUnits == value)
			{
				return;
			}

			_polygonOffsetUnits = value;
			_isPolygonOffsetUnitsWritten = true;
			RecordSet("polygonOffsetUnits", value);
		}
	}

	/// <summary>
	/// Whether to apply dithering to the color to remove the appearance of banding. Writing it records
	/// a <c>dithering</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool Dithering
	{
		get { return _dithering; }
		set
		{
			if (_dithering == value)
			{
				return;
			}

			_dithering = value;
			_isDitheringWritten = true;
			RecordSet("dithering", value);
		}
	}

	/// <summary>
	/// Whether alpha to coverage should be enabled or not. Can only be used with MSAA-enabled contexts
	/// (meaning when the renderer was created with *antialias* parameter set to <c>true</c>). Enabling
	/// this will smooth aliasing on clip plane edges and alphaTest-clipped edges. Writing it records a
	/// <c>alphaToCoverage</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public bool AlphaToCoverage
	{
		get { return _alphaToCoverage; }
		set
		{
			if (_alphaToCoverage == value)
			{
				return;
			}

			_alphaToCoverage = value;
			_isAlphaToCoverageWritten = true;
			RecordSet("alphaToCoverage", value);
		}
	}

	/// <summary>
	/// Whether to premultiply the alpha (transparency) value. Writing it records a
	/// <c>premultipliedAlpha</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public bool PremultipliedAlpha
	{
		get { return _premultipliedAlpha; }
		set
		{
			if (_premultipliedAlpha == value)
			{
				return;
			}

			_premultipliedAlpha = value;
			_isPremultipliedAlphaWritten = true;
			RecordSet("premultipliedAlpha", value);
		}
	}

	/// <summary>
	/// Whether double-sided, transparent objects should be rendered with a single pass or not. The
	/// engine renders double-sided, transparent objects with two draw calls (back faces first, then
	/// front faces) to mitigate transparency artifacts. There are scenarios however where this approach
	/// produces no quality gains but still doubles draw calls e.g. when rendering flat vegetation like
	/// grass sprites. In these cases, set the <c>forceSinglePass</c> flag to <c>true</c> to disable the
	/// two pass rendering to avoid performance issues. Writing it records a <c>forceSinglePass</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool ForceSinglePass
	{
		get { return _forceSinglePass; }
		set
		{
			if (_forceSinglePass == value)
			{
				return;
			}

			_forceSinglePass = value;
			_isForceSinglePassWritten = true;
			RecordSet("forceSinglePass", value);
		}
	}

	/// <summary>
	/// Whether it's possible to override the material with <c>Scene#overrideMaterial</c> or not.
	/// Writing it records a <c>allowOverride</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public bool AllowOverride
	{
		get { return _allowOverride; }
		set
		{
			if (_allowOverride == value)
			{
				return;
			}

			_allowOverride = value;
			_isAllowOverrideWritten = true;
			RecordSet("allowOverride", value);
		}
	}

	/// <summary>
	/// Defines whether 3D objects using this material are visible. Writing it records a <c>visible</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Visible
	{
		get { return _visible; }
		set
		{
			if (_visible == value)
			{
				return;
			}

			_visible = value;
			_isVisibleWritten = true;
			RecordSet("visible", value);
		}
	}

	/// <summary>
	/// Defines whether this material is tone mapped according to the renderer's tone mapping setting.
	/// It is ignored when rendering to a render target or using post processing or when using
	/// <c>WebGPURenderer</c>. In all these cases, all materials are honored by tone mapping. Writing it
	/// records a <c>toneMapped</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public bool ToneMapped
	{
		get { return _toneMapped; }
		set
		{
			if (_toneMapped == value)
			{
				return;
			}

			_toneMapped = value;
			_isToneMappedWritten = true;
			RecordSet("toneMapped", value);
		}
	}

	/// <summary>
	/// The <c>alphaTest</c> property of the JavaScript-side object. Writing it records a
	/// <c>alphaTest</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float AlphaTest
	{
		get { return _alphaTest; }
		set
		{
			if (_alphaTest == value)
			{
				return;
			}

			_alphaTest = value;
			_isAlphaTestWritten = true;
			RecordSet("alphaTest", value);
		}
	}

	/// <summary>Copies the values of the given material to this instance.</summary>
	/// <param name="source">The material to copy.</param>
	public void Copy(Material source)
	{
		RecordCall("copy", source);
	}

	/// <summary>
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsMaterialAsync()
	{
		return GetAsync<bool>("isMaterial");
	}

	/// <summary>
	/// The UUID of the material. Read-only in three.js, so it is read on demand rather than mirrored:
	/// records a get op, sends it behind every write already pending, and completes with the value
	/// <c>uuid</c> held.
	/// </summary>
	/// <returns>The value <c>uuid</c> held, once the JavaScript side has answered.</returns>
	public Task<string> UuidAsync()
	{
		return GetAsync<string>("uuid");
	}

	/// <summary>
	/// This starts at <c>0</c> and counts how many times <c>Material#needsUpdate</c> is set to
	/// <c>true</c>. Read-only in three.js, so it is read on demand rather than mirrored: records a get
	/// op, sends it behind every write already pending, and completes with the value <c>version</c>
	/// held.
	/// </summary>
	/// <returns>The value <c>version</c> held, once the JavaScript side has answered.</returns>
	public Task<float> VersionAsync()
	{
		return GetAsync<float>("version");
	}

	/// <summary>
	/// In case <c>Material#onBeforeCompile</c> is used, this callback can be used to identify values of
	/// settings used in <c>onBeforeCompile()</c>, so three.js can reuse a cached shader or recompile
	/// the shader for this material as needed. This method can only be used when rendering with
	/// <c>WebGLRenderer</c>. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>customProgramCacheKey</c> returned.
	/// </summary>
	/// <returns>The value <c>customProgramCacheKey</c> returned, once the JavaScript side has answered.</returns>
	public Task<string> CustomProgramCacheKeyAsync()
	{
		return RecordRead<string>("customProgramCacheKey");
	}

	/// <summary>
	/// Returns a new material with copied values from this instance. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<Material?> CloneAsync()
	{
		return RecordReadObject<Material>("clone", (adoptedBatch, adoptedHandle) => new Material(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Material</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isTypeWritten)
		{
			batch.Set(Handle, "type", ThreeValue.Encode(_type));
		}

		if (_isNeedsUpdateWritten)
		{
			batch.Set(Handle, "needsUpdate", ThreeValue.Encode(_needsUpdate));
		}

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isBlendingWritten)
		{
			batch.Set(Handle, "blending", ThreeValue.Encode(_blending));
		}

		if (_isSideWritten)
		{
			batch.Set(Handle, "side", ThreeValue.Encode(_side));
		}

		if (_isVertexColorsWritten)
		{
			batch.Set(Handle, "vertexColors", ThreeValue.Encode(_vertexColors));
		}

		if (_isOpacityWritten)
		{
			batch.Set(Handle, "opacity", ThreeValue.Encode(_opacity));
		}

		if (_isTransparentWritten)
		{
			batch.Set(Handle, "transparent", ThreeValue.Encode(_transparent));
		}

		if (_isAlphaHashWritten)
		{
			batch.Set(Handle, "alphaHash", ThreeValue.Encode(_alphaHash));
		}

		if (_isBlendDstWritten)
		{
			batch.Set(Handle, "blendDst", ThreeValue.Encode(_blendDst));
		}

		if (_isBlendEquationWritten)
		{
			batch.Set(Handle, "blendEquation", ThreeValue.Encode(_blendEquation));
		}

		if (_isBlendDstAlphaWritten)
		{
			batch.Set(Handle, "blendDstAlpha", ThreeValue.Encode(_blendDstAlpha));
		}

		if (_isBlendEquationAlphaWritten)
		{
			batch.Set(Handle, "blendEquationAlpha", ThreeValue.Encode(_blendEquationAlpha));
		}

		if (_isBlendColorWritten)
		{
			batch.Set(Handle, "blendColor", ThreeValue.Encode(BlendColor));
		}

		if (_isBlendAlphaWritten)
		{
			batch.Set(Handle, "blendAlpha", ThreeValue.Encode(_blendAlpha));
		}

		if (_isDepthFuncWritten)
		{
			batch.Set(Handle, "depthFunc", ThreeValue.Encode(_depthFunc));
		}

		if (_isDepthTestWritten)
		{
			batch.Set(Handle, "depthTest", ThreeValue.Encode(_depthTest));
		}

		if (_isDepthWriteWritten)
		{
			batch.Set(Handle, "depthWrite", ThreeValue.Encode(_depthWrite));
		}

		if (_isStencilWriteMaskWritten)
		{
			batch.Set(Handle, "stencilWriteMask", ThreeValue.Encode(_stencilWriteMask));
		}

		if (_isStencilFuncWritten)
		{
			batch.Set(Handle, "stencilFunc", ThreeValue.Encode(_stencilFunc));
		}

		if (_isStencilRefWritten)
		{
			batch.Set(Handle, "stencilRef", ThreeValue.Encode(_stencilRef));
		}

		if (_isStencilFuncMaskWritten)
		{
			batch.Set(Handle, "stencilFuncMask", ThreeValue.Encode(_stencilFuncMask));
		}

		if (_isStencilFailWritten)
		{
			batch.Set(Handle, "stencilFail", ThreeValue.Encode(_stencilFail));
		}

		if (_isStencilZFailWritten)
		{
			batch.Set(Handle, "stencilZFail", ThreeValue.Encode(_stencilZFail));
		}

		if (_isStencilZPassWritten)
		{
			batch.Set(Handle, "stencilZPass", ThreeValue.Encode(_stencilZPass));
		}

		if (_isStencilWriteWritten)
		{
			batch.Set(Handle, "stencilWrite", ThreeValue.Encode(_stencilWrite));
		}

		if (_isClippingPlanesWritten)
		{
			batch.Set(Handle, "clippingPlanes", ThreeValue.Encode(_clippingPlanes));
		}

		if (_isClipIntersectionWritten)
		{
			batch.Set(Handle, "clipIntersection", ThreeValue.Encode(_clipIntersection));
		}

		if (_isClipShadowsWritten)
		{
			batch.Set(Handle, "clipShadows", ThreeValue.Encode(_clipShadows));
		}

		if (_isShadowSideWritten)
		{
			batch.Set(Handle, "shadowSide", ThreeValue.Encode(_shadowSide));
		}

		if (_isColorWriteWritten)
		{
			batch.Set(Handle, "colorWrite", ThreeValue.Encode(_colorWrite));
		}

		if (_isPrecisionWritten)
		{
			batch.Set(Handle, "precision", ThreeValue.Encode(_precision));
		}

		if (_isPolygonOffsetWritten)
		{
			batch.Set(Handle, "polygonOffset", ThreeValue.Encode(_polygonOffset));
		}

		if (_isPolygonOffsetFactorWritten)
		{
			batch.Set(Handle, "polygonOffsetFactor", ThreeValue.Encode(_polygonOffsetFactor));
		}

		if (_isPolygonOffsetUnitsWritten)
		{
			batch.Set(Handle, "polygonOffsetUnits", ThreeValue.Encode(_polygonOffsetUnits));
		}

		if (_isDitheringWritten)
		{
			batch.Set(Handle, "dithering", ThreeValue.Encode(_dithering));
		}

		if (_isAlphaToCoverageWritten)
		{
			batch.Set(Handle, "alphaToCoverage", ThreeValue.Encode(_alphaToCoverage));
		}

		if (_isPremultipliedAlphaWritten)
		{
			batch.Set(Handle, "premultipliedAlpha", ThreeValue.Encode(_premultipliedAlpha));
		}

		if (_isForceSinglePassWritten)
		{
			batch.Set(Handle, "forceSinglePass", ThreeValue.Encode(_forceSinglePass));
		}

		if (_isAllowOverrideWritten)
		{
			batch.Set(Handle, "allowOverride", ThreeValue.Encode(_allowOverride));
		}

		if (_isVisibleWritten)
		{
			batch.Set(Handle, "visible", ThreeValue.Encode(_visible));
		}

		if (_isToneMappedWritten)
		{
			batch.Set(Handle, "toneMapped", ThreeValue.Encode(_toneMapped));
		}

		if (_isAlphaTestWritten)
		{
			batch.Set(Handle, "alphaTest", ThreeValue.Encode(_alphaTest));
		}
	}
}
