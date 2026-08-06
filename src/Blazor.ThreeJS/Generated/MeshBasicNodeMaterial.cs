// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="MeshBasicMaterial"/>. The JavaScript-side
/// <c>THREE.MeshBasicNodeMaterial</c>.
/// </summary>
public sealed class MeshBasicNodeMaterial : NodeMaterial
{
	private Texture? _map = null;
	private Texture? _lightMap = null;
	private float _lightMapIntensity = 1f;
	private Texture? _aoMap = null;
	private float _aoMapIntensity = 1f;
	private Texture? _specularMap = null;
	private Texture? _alphaMap = null;
	private Texture? _envMap = null;
	private Combine _combine = Combine.MultiplyOperation;
	private float _reflectivity = 1f;
	private float _refractionRatio = 0.98f;
	private bool _wireframe = false;
	private float _wireframeLinewidth = 1f;
	private bool _isColorWritten;
	private bool _isMapWritten;
	private bool _isLightMapWritten;
	private bool _isLightMapIntensityWritten;
	private bool _isAoMapWritten;
	private bool _isAoMapIntensityWritten;
	private bool _isSpecularMapWritten;
	private bool _isAlphaMapWritten;
	private bool _isEnvMapWritten;
	private bool _isEnvMapRotationWritten;
	private bool _isCombineWritten;
	private bool _isReflectivityWritten;
	private bool _isRefractionRatioWritten;
	private bool _isWireframeWritten;
	private bool _isWireframeLinewidthWritten;

	/// <summary>
	/// Color of the material. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>
	/// The rotation of the environment map in radians. Mirrored as an instance this object owns:
	/// mutating it records a write of <c>envMapRotation</c>.
	/// </summary>
	public Euler EnvMapRotation { get; }

	/// <summary>Constructs a new mesh basic node material.</summary>
	public MeshBasicNodeMaterial()
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};

		EnvMapRotation = new Euler();
		EnvMapRotation.OnChange = () =>
		{
			_isEnvMapRotationWritten = true;
			RecordSet("envMapRotation", EnvMapRotation);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>MeshBasicNodeMaterial</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal MeshBasicNodeMaterial(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};

		EnvMapRotation = new Euler();
		EnvMapRotation.OnChange = () =>
		{
			_isEnvMapRotationWritten = true;
			RecordSet("envMapRotation", EnvMapRotation);
		};

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshBasicNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshBasicNodeMaterial"; }
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
	/// Specular map used by the material. <c>specularMap</c> represents color data, and the texture
	/// must be assigned a <c>Texture#colorSpace</c>. Most <c>specularMap</c> textures set
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
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isMeshBasicNodeMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isMeshBasicNodeMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsMeshBasicNodeMaterialAsync()
	{
		return GetAsync<bool>("isMeshBasicNodeMaterial");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.MeshBasicNodeMaterial</c>, then replays every property written
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
	}
}
