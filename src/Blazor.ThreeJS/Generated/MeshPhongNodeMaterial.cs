// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="MeshPhongMaterial"/>. The JavaScript-side
/// <c>THREE.MeshPhongNodeMaterial</c>.
/// </summary>
public sealed class MeshPhongNodeMaterial : NodeMaterial
{
	private float _shininess = 30f;
	private Texture? _map = null;
	private Texture? _lightMap = null;
	private float _lightMapIntensity = 1f;
	private Texture? _aoMap = null;
	private float _aoMapIntensity = 1f;
	private float _emissiveIntensity = 1f;
	private Texture? _emissiveMap = null;
	private Texture? _bumpMap = null;
	private float _bumpScale = 1f;
	private Texture? _normalMap = null;
	private NormalMapTypes _normalMapType = NormalMapTypes.TangentSpaceNormalMap;
	private Texture? _displacementMap = null;
	private float _displacementScale = 0f;
	private float _displacementBias = 0f;
	private Texture? _specularMap = null;
	private Texture? _alphaMap = null;
	private Texture? _envMap = null;
	private Combine _combine = Combine.MultiplyOperation;
	private float _reflectivity = 1f;
	private float _envMapIntensity = 1f;
	private float _refractionRatio = 0.98f;
	private bool _wireframe = false;
	private float _wireframeLinewidth = 1f;
	private LineJoin _wireframeLinecap;
	private LineJoin _wireframeLinejoin;
	private bool _flatShading = false;
	private bool _isColorWritten;
	private bool _isSpecularWritten;
	private bool _isShininessWritten;
	private bool _isMapWritten;
	private bool _isLightMapWritten;
	private bool _isLightMapIntensityWritten;
	private bool _isAoMapWritten;
	private bool _isAoMapIntensityWritten;
	private bool _isEmissiveWritten;
	private bool _isEmissiveIntensityWritten;
	private bool _isEmissiveMapWritten;
	private bool _isBumpMapWritten;
	private bool _isBumpScaleWritten;
	private bool _isNormalMapWritten;
	private bool _isNormalMapTypeWritten;
	private bool _isNormalScaleWritten;
	private bool _isDisplacementMapWritten;
	private bool _isDisplacementScaleWritten;
	private bool _isDisplacementBiasWritten;
	private bool _isSpecularMapWritten;
	private bool _isAlphaMapWritten;
	private bool _isEnvMapWritten;
	private bool _isEnvMapRotationWritten;
	private bool _isCombineWritten;
	private bool _isReflectivityWritten;
	private bool _isEnvMapIntensityWritten;
	private bool _isRefractionRatioWritten;
	private bool _isWireframeWritten;
	private bool _isWireframeLinewidthWritten;
	private bool _isWireframeLinecapWritten;
	private bool _isWireframeLinejoinWritten;
	private bool _isFlatShadingWritten;

	/// <summary>
	/// Color of the material. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>
	/// Specular color of the material. The default color is set to <c>0x111111</c> (very dark grey)
	/// This defines how shiny the material is and the color of its shine. Mirrored as an instance this
	/// object owns: mutating it records a write of <c>specular</c>.
	/// </summary>
	public Color Specular { get; }

	/// <summary>
	/// Emissive (light) color of the material, essentially a solid color unaffected by other lighting.
	/// Mirrored as an instance this object owns: mutating it records a write of <c>emissive</c>.
	/// </summary>
	public Color Emissive { get; }

	/// <summary>
	/// How much the normal map affects the material. Typical value range is <c>[0,1]</c>. Mirrored as
	/// an instance this object owns: mutating it records a write of <c>normalScale</c>.
	/// </summary>
	public Vector2 NormalScale { get; }

	/// <summary>
	/// The rotation of the environment map in radians. Mirrored as an instance this object owns:
	/// mutating it records a write of <c>envMapRotation</c>.
	/// </summary>
	public Euler EnvMapRotation { get; }

	/// <summary>Constructs a new mesh lambert node material.</summary>
	public MeshPhongNodeMaterial()
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};

		Specular = new Color();
		Specular.OnChange = () =>
		{
			_isSpecularWritten = true;
			RecordSet("specular", Specular);
		};

		Emissive = new Color(0f, 0f, 0f);
		Emissive.OnChange = () =>
		{
			_isEmissiveWritten = true;
			RecordSet("emissive", Emissive);
		};

		NormalScale = new Vector2();
		NormalScale.OnChange = () =>
		{
			_isNormalScaleWritten = true;
			RecordSet("normalScale", NormalScale);
		};

		EnvMapRotation = new Euler();
		EnvMapRotation.OnChange = () =>
		{
			_isEnvMapRotationWritten = true;
			RecordSet("envMapRotation", EnvMapRotation);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>MeshPhongNodeMaterial</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal MeshPhongNodeMaterial(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};

		Specular = new Color();
		Specular.OnChange = () =>
		{
			_isSpecularWritten = true;
			RecordSet("specular", Specular);
		};

		Emissive = new Color(0f, 0f, 0f);
		Emissive.OnChange = () =>
		{
			_isEmissiveWritten = true;
			RecordSet("emissive", Emissive);
		};

		NormalScale = new Vector2();
		NormalScale.OnChange = () =>
		{
			_isNormalScaleWritten = true;
			RecordSet("normalScale", NormalScale);
		};

		EnvMapRotation = new Euler();
		EnvMapRotation.OnChange = () =>
		{
			_isEnvMapRotationWritten = true;
			RecordSet("envMapRotation", EnvMapRotation);
		};

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshPhongNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshPhongNodeMaterial"; }
	}

	/// <summary>
	/// How shiny the specular highlight is; a higher value gives a sharper highlight. Writing it
	/// records a <c>shininess</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float Shininess
	{
		get { return _shininess; }
		set
		{
			if (_shininess == value)
			{
				return;
			}

			_shininess = value;
			_isShininessWritten = true;
			RecordSet("shininess", value);
		}
	}

	/// <summary>
	/// The color map. May optionally include an alpha channel, typically combined with
	/// <c>Material#transparent</c> or <c>Material#alphaTest</c>. The texture map color is modulated by
	/// the diffuse <c>color</c>. <c>map</c> represents color data, and the texture must be assigned a
	/// <c>Texture#colorSpace</c>. Most <c>map</c> textures set <c>texture.colorSpace =
	/// SRGBColorSpace</c>. Writing it records a <c>map</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public Texture? Map
	{
		get { return _map; }
		set
		{
			if (ReferenceEquals(_map, value))
			{
				return;
			}

			_map = value;
			_isMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("map", value);
		}
	}

	/// <summary>
	/// The light map. Requires a second set of UVs. <c>lightMap</c> represents pre-baked illuminance
	/// data, and the texture must be assigned a <c>Texture#colorSpace</c>. Most <c>lightMap</c>
	/// textures set <c>texture.colorSpace = LinearSRGBColorSpace</c> and use float-type formats such as
	/// <c>.exr</c> or <c>.hdr</c>. Writing it records a <c>lightMap</c> property write once this object
	/// is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? LightMap
	{
		get { return _lightMap; }
		set
		{
			if (ReferenceEquals(_lightMap, value))
			{
				return;
			}

			_lightMap = value;
			_isLightMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("lightMap", value);
		}
	}

	/// <summary>
	/// Intensity of the baked light. Writing it records a <c>lightMapIntensity</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float LightMapIntensity
	{
		get { return _lightMapIntensity; }
		set
		{
			if (_lightMapIntensity == value)
			{
				return;
			}

			_lightMapIntensity = value;
			_isLightMapIntensityWritten = true;
			RecordSet("lightMapIntensity", value);
		}
	}

	/// <summary>
	/// The red channel of this texture is used as the ambient occlusion map. Requires a second set of
	/// UVs. <c>aoMap</c> represents non-color data. Any texture assigned must have
	/// <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a <c>aoMap</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? AoMap
	{
		get { return _aoMap; }
		set
		{
			if (ReferenceEquals(_aoMap, value))
			{
				return;
			}

			_aoMap = value;
			_isAoMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("aoMap", value);
		}
	}

	/// <summary>
	/// Intensity of the ambient occlusion effect. Range is <c>[0,1]</c>, where <c>0</c> disables
	/// ambient occlusion. Where intensity is <c>1</c> and the AO map's red channel is also <c>1</c>,
	/// ambient light is fully occluded on a surface. Writing it records a <c>aoMapIntensity</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float AoMapIntensity
	{
		get { return _aoMapIntensity; }
		set
		{
			if (_aoMapIntensity == value)
			{
				return;
			}

			_aoMapIntensity = value;
			_isAoMapIntensityWritten = true;
			RecordSet("aoMapIntensity", value);
		}
	}

	/// <summary>
	/// Intensity of the emissive light. Modulates the emissive color. Writing it records a
	/// <c>emissiveIntensity</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float EmissiveIntensity
	{
		get { return _emissiveIntensity; }
		set
		{
			if (_emissiveIntensity == value)
			{
				return;
			}

			_emissiveIntensity = value;
			_isEmissiveIntensityWritten = true;
			RecordSet("emissiveIntensity", value);
		}
	}

	/// <summary>
	/// Set emissive (glow) map. The emissive map color is modulated by the emissive color and the
	/// emissive intensity. If you have an emissive map, be sure to set the emissive color to something
	/// other than black. <c>emissiveMap</c> represents color data, and the texture must be assigned a
	/// <c>Texture#colorSpace</c>. Most <c>emissiveMap</c> textures set <c>texture.colorSpace =
	/// SRGBColorSpace</c>. Writing it records a <c>emissiveMap</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public Texture? EmissiveMap
	{
		get { return _emissiveMap; }
		set
		{
			if (ReferenceEquals(_emissiveMap, value))
			{
				return;
			}

			_emissiveMap = value;
			_isEmissiveMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("emissiveMap", value);
		}
	}

	/// <summary>
	/// The texture to create a bump map. The black and white values map to the perceived depth in
	/// relation to the lights. Bump doesn't actually affect the geometry of the object, only the
	/// lighting. If a normal map is defined this will be ignored. <c>bumpMap</c> represents non-color
	/// data. Any texture assigned must have <c>texture.colorSpace = NoColorSpace</c> (default). Writing
	/// it records a <c>bumpMap</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public Texture? BumpMap
	{
		get { return _bumpMap; }
		set
		{
			if (ReferenceEquals(_bumpMap, value))
			{
				return;
			}

			_bumpMap = value;
			_isBumpMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("bumpMap", value);
		}
	}

	/// <summary>
	/// How much the bump map affects the material. Typical range is <c>[0,1]</c>. Writing it records a
	/// <c>bumpScale</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float BumpScale
	{
		get { return _bumpScale; }
		set
		{
			if (_bumpScale == value)
			{
				return;
			}

			_bumpScale = value;
			_isBumpScaleWritten = true;
			RecordSet("bumpScale", value);
		}
	}

	/// <summary>
	/// The texture to create a normal map. The RGB values affect the surface normal for each pixel
	/// fragment and change the way the color is lit. Normal maps do not change the actual shape of the
	/// surface, only the lighting. In case the material has a normal map authored using the left handed
	/// convention, the <c>y</c> component of <c>normalScale</c> should be negated to compensate for the
	/// different handedness. <c>normalMap</c> represents non-color data. Any texture assigned must have
	/// <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a <c>normalMap</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? NormalMap
	{
		get { return _normalMap; }
		set
		{
			if (ReferenceEquals(_normalMap, value))
			{
				return;
			}

			_normalMap = value;
			_isNormalMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("normalMap", value);
		}
	}

	/// <summary>
	/// The type of normal map. Writing it records a <c>normalMapType</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public NormalMapTypes NormalMapType
	{
		get { return _normalMapType; }
		set
		{
			if (_normalMapType == value)
			{
				return;
			}

			_normalMapType = value;
			_isNormalMapTypeWritten = true;
			RecordSet("normalMapType", value);
		}
	}

	/// <summary>
	/// The displacement map affects the position of the mesh's vertices. Unlike other maps which only
	/// affect the light and shade of the material the displaced vertices can cast shadows, block other
	/// objects, and otherwise act as real geometry. The displacement texture is an image where the
	/// value of each pixel (white being the highest) is mapped against, and repositions, the vertices
	/// of the mesh. For best results, pair a displacement map with a matching normal map, since the
	/// renderer can not recompute surface normals from the displaced vertices. <c>displacementMap</c>
	/// represents non-color data. Any texture assigned must have <c>texture.colorSpace =
	/// NoColorSpace</c> (default). Writing it records a <c>displacementMap</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? DisplacementMap
	{
		get { return _displacementMap; }
		set
		{
			if (ReferenceEquals(_displacementMap, value))
			{
				return;
			}

			_displacementMap = value;
			_isDisplacementMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("displacementMap", value);
		}
	}

	/// <summary>
	/// How much the displacement map affects the mesh (where black is no displacement, and white is
	/// maximum displacement). Without a displacement map set, this value is not applied. Writing it
	/// records a <c>displacementScale</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public float DisplacementScale
	{
		get { return _displacementScale; }
		set
		{
			if (_displacementScale == value)
			{
				return;
			}

			_displacementScale = value;
			_isDisplacementScaleWritten = true;
			RecordSet("displacementScale", value);
		}
	}

	/// <summary>
	/// The offset of the displacement map's values on the mesh's vertices. The bias is added to the
	/// scaled sample of the displacement map. Without a displacement map set, this value is not
	/// applied. Writing it records a <c>displacementBias</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float DisplacementBias
	{
		get { return _displacementBias; }
		set
		{
			if (_displacementBias == value)
			{
				return;
			}

			_displacementBias = value;
			_isDisplacementBiasWritten = true;
			RecordSet("displacementBias", value);
		}
	}

	/// <summary>
	/// The specular map value affects both how much the specular surface highlight contributes and how
	/// much of the environment map affects the surface. <c>specularMap</c> represents color data, and
	/// the texture must be assigned a <c>Texture#colorSpace</c>. Most <c>specularMap</c> textures set
	/// <c>texture.colorSpace = SRGBColorSpace</c>. Writing it records a <c>specularMap</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? SpecularMap
	{
		get { return _specularMap; }
		set
		{
			if (ReferenceEquals(_specularMap, value))
			{
				return;
			}

			_specularMap = value;
			_isSpecularMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("specularMap", value);
		}
	}

	/// <summary>
	/// The alpha map is a grayscale texture that controls the opacity across the surface (black: fully
	/// transparent; white: fully opaque). Only the color of the texture is used, ignoring the alpha
	/// channel if one exists. For RGB and RGBA textures, the renderer will use the green channel when
	/// sampling this texture due to the extra bit of precision provided for green in DXT-compressed and
	/// uncompressed RGB 565 formats. Luminance-only and luminance/alpha textures will also still work
	/// as expected. <c>alphaMap</c> represents non-color data. Any texture assigned must have
	/// <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a <c>alphaMap</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? AlphaMap
	{
		get { return _alphaMap; }
		set
		{
			if (ReferenceEquals(_alphaMap, value))
			{
				return;
			}

			_alphaMap = value;
			_isAlphaMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("alphaMap", value);
		}
	}

	/// <summary>
	/// The environment map. <c>envMap</c> represents luminance data, and the texture must be assigned a
	/// <c>Texture#colorSpace</c>. Most <c>envMap</c> textures set <c>texture.colorSpace =
	/// LinearSRGBColorSpace</c> and use float-type formats such as <c>.exr</c> or <c>.hdr</c>. Writing
	/// it records a <c>envMap</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public Texture? EnvMap
	{
		get { return _envMap; }
		set
		{
			if (ReferenceEquals(_envMap, value))
			{
				return;
			}

			_envMap = value;
			_isEnvMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("envMap", value);
		}
	}

	/// <summary>
	/// How to combine the result of the surface's color with the environment map, if any. When set to
	/// <c>MixOperation</c>, the <c>MeshBasicMaterial#reflectivity</c> is used to blend between the two
	/// colors. Writing it records a <c>combine</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public Combine Combine
	{
		get { return _combine; }
		set
		{
			if (_combine == value)
			{
				return;
			}

			_combine = value;
			_isCombineWritten = true;
			RecordSet("combine", value);
		}
	}

	/// <summary>
	/// How much the environment map affects the surface. The valid range is between <c>0</c> (no
	/// reflections) and <c>1</c> (full reflections). Writing it records a <c>reflectivity</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Reflectivity
	{
		get { return _reflectivity; }
		set
		{
			if (_reflectivity == value)
			{
				return;
			}

			_reflectivity = value;
			_isReflectivityWritten = true;
			RecordSet("reflectivity", value);
		}
	}

	/// <summary>
	/// Scales the effect of the environment map by multiplying its color. Writing it records a
	/// <c>envMapIntensity</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float EnvMapIntensity
	{
		get { return _envMapIntensity; }
		set
		{
			if (_envMapIntensity == value)
			{
				return;
			}

			_envMapIntensity = value;
			_isEnvMapIntensityWritten = true;
			RecordSet("envMapIntensity", value);
		}
	}

	/// <summary>
	/// The index of refraction (IOR) of air (approximately 1) divided by the index of refraction of the
	/// material. It is used with environment mapping modes <c>CubeRefractionMapping</c> and
	/// <c>EquirectangularRefractionMapping</c>. The refraction ratio should not exceed <c>1</c>.
	/// Writing it records a <c>refractionRatio</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public float RefractionRatio
	{
		get { return _refractionRatio; }
		set
		{
			if (_refractionRatio == value)
			{
				return;
			}

			_refractionRatio = value;
			_isRefractionRatioWritten = true;
			RecordSet("refractionRatio", value);
		}
	}

	/// <summary>
	/// Renders the geometry as a wireframe. Writing it records a <c>wireframe</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Wireframe
	{
		get { return _wireframe; }
		set
		{
			if (_wireframe == value)
			{
				return;
			}

			_wireframe = value;
			_isWireframeWritten = true;
			RecordSet("wireframe", value);
		}
	}

	/// <summary>
	/// Controls the thickness of the wireframe. Can only be used with <c>SVGRenderer</c>. Writing it
	/// records a <c>wireframeLinewidth</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public float WireframeLinewidth
	{
		get { return _wireframeLinewidth; }
		set
		{
			if (_wireframeLinewidth == value)
			{
				return;
			}

			_wireframeLinewidth = value;
			_isWireframeLinewidthWritten = true;
			RecordSet("wireframeLinewidth", value);
		}
	}

	/// <summary>
	/// Defines appearance of wireframe ends. Can only be used with <c>SVGRenderer</c>. Writing it
	/// records a <c>wireframeLinecap</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public LineJoin WireframeLinecap
	{
		get { return _wireframeLinecap; }
		set
		{
			if (_wireframeLinecap == value)
			{
				return;
			}

			_wireframeLinecap = value;
			_isWireframeLinecapWritten = true;
			RecordSet("wireframeLinecap", value);
		}
	}

	/// <summary>
	/// Defines appearance of wireframe joints. Can only be used with <c>SVGRenderer</c>. Writing it
	/// records a <c>wireframeLinejoin</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public LineJoin WireframeLinejoin
	{
		get { return _wireframeLinejoin; }
		set
		{
			if (_wireframeLinejoin == value)
			{
				return;
			}

			_wireframeLinejoin = value;
			_isWireframeLinejoinWritten = true;
			RecordSet("wireframeLinejoin", value);
		}
	}

	/// <summary>
	/// Whether the material is rendered with flat shading or not. Writing it records a
	/// <c>flatShading</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool FlatShading
	{
		get { return _flatShading; }
		set
		{
			if (_flatShading == value)
			{
				return;
			}

			_flatShading = value;
			_isFlatShadingWritten = true;
			RecordSet("flatShading", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isMeshPhongNodeMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isMeshPhongNodeMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsMeshPhongNodeMaterialAsync()
	{
		return GetAsync<bool>("isMeshPhongNodeMaterial");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.MeshPhongNodeMaterial</c>, then replays every property written
	/// before this object was attached. A replayed value that is itself a mirrored object is attached
	/// first, so its create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isColorWritten)
		{
			batch.Set(Handle, "color", ThreeValue.Encode(Color));
		}

		if (_isSpecularWritten)
		{
			batch.Set(Handle, "specular", ThreeValue.Encode(Specular));
		}

		if (_isShininessWritten)
		{
			batch.Set(Handle, "shininess", ThreeValue.Encode(_shininess));
		}

		if (_isMapWritten)
		{
			_map?.AttachTo(batch);
			batch.Set(Handle, "map", ThreeValue.Encode(_map));
		}

		if (_isLightMapWritten)
		{
			_lightMap?.AttachTo(batch);
			batch.Set(Handle, "lightMap", ThreeValue.Encode(_lightMap));
		}

		if (_isLightMapIntensityWritten)
		{
			batch.Set(Handle, "lightMapIntensity", ThreeValue.Encode(_lightMapIntensity));
		}

		if (_isAoMapWritten)
		{
			_aoMap?.AttachTo(batch);
			batch.Set(Handle, "aoMap", ThreeValue.Encode(_aoMap));
		}

		if (_isAoMapIntensityWritten)
		{
			batch.Set(Handle, "aoMapIntensity", ThreeValue.Encode(_aoMapIntensity));
		}

		if (_isEmissiveWritten)
		{
			batch.Set(Handle, "emissive", ThreeValue.Encode(Emissive));
		}

		if (_isEmissiveIntensityWritten)
		{
			batch.Set(Handle, "emissiveIntensity", ThreeValue.Encode(_emissiveIntensity));
		}

		if (_isEmissiveMapWritten)
		{
			_emissiveMap?.AttachTo(batch);
			batch.Set(Handle, "emissiveMap", ThreeValue.Encode(_emissiveMap));
		}

		if (_isBumpMapWritten)
		{
			_bumpMap?.AttachTo(batch);
			batch.Set(Handle, "bumpMap", ThreeValue.Encode(_bumpMap));
		}

		if (_isBumpScaleWritten)
		{
			batch.Set(Handle, "bumpScale", ThreeValue.Encode(_bumpScale));
		}

		if (_isNormalMapWritten)
		{
			_normalMap?.AttachTo(batch);
			batch.Set(Handle, "normalMap", ThreeValue.Encode(_normalMap));
		}

		if (_isNormalMapTypeWritten)
		{
			batch.Set(Handle, "normalMapType", ThreeValue.Encode(_normalMapType));
		}

		if (_isNormalScaleWritten)
		{
			batch.Set(Handle, "normalScale", ThreeValue.Encode(NormalScale));
		}

		if (_isDisplacementMapWritten)
		{
			_displacementMap?.AttachTo(batch);
			batch.Set(Handle, "displacementMap", ThreeValue.Encode(_displacementMap));
		}

		if (_isDisplacementScaleWritten)
		{
			batch.Set(Handle, "displacementScale", ThreeValue.Encode(_displacementScale));
		}

		if (_isDisplacementBiasWritten)
		{
			batch.Set(Handle, "displacementBias", ThreeValue.Encode(_displacementBias));
		}

		if (_isSpecularMapWritten)
		{
			_specularMap?.AttachTo(batch);
			batch.Set(Handle, "specularMap", ThreeValue.Encode(_specularMap));
		}

		if (_isAlphaMapWritten)
		{
			_alphaMap?.AttachTo(batch);
			batch.Set(Handle, "alphaMap", ThreeValue.Encode(_alphaMap));
		}

		if (_isEnvMapWritten)
		{
			_envMap?.AttachTo(batch);
			batch.Set(Handle, "envMap", ThreeValue.Encode(_envMap));
		}

		if (_isEnvMapRotationWritten)
		{
			batch.Set(Handle, "envMapRotation", ThreeValue.Encode(EnvMapRotation));
		}

		if (_isCombineWritten)
		{
			batch.Set(Handle, "combine", ThreeValue.Encode(_combine));
		}

		if (_isReflectivityWritten)
		{
			batch.Set(Handle, "reflectivity", ThreeValue.Encode(_reflectivity));
		}

		if (_isEnvMapIntensityWritten)
		{
			batch.Set(Handle, "envMapIntensity", ThreeValue.Encode(_envMapIntensity));
		}

		if (_isRefractionRatioWritten)
		{
			batch.Set(Handle, "refractionRatio", ThreeValue.Encode(_refractionRatio));
		}

		if (_isWireframeWritten)
		{
			batch.Set(Handle, "wireframe", ThreeValue.Encode(_wireframe));
		}

		if (_isWireframeLinewidthWritten)
		{
			batch.Set(Handle, "wireframeLinewidth", ThreeValue.Encode(_wireframeLinewidth));
		}

		if (_isWireframeLinecapWritten)
		{
			batch.Set(Handle, "wireframeLinecap", ThreeValue.Encode(_wireframeLinecap));
		}

		if (_isWireframeLinejoinWritten)
		{
			batch.Set(Handle, "wireframeLinejoin", ThreeValue.Encode(_wireframeLinejoin));
		}

		if (_isFlatShadingWritten)
		{
			batch.Set(Handle, "flatShading", ThreeValue.Encode(_flatShading));
		}
	}
}
