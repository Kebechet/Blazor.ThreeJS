// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="MeshLambertMaterial"/>. The JavaScript-side
/// <c>THREE.MeshLambertNodeMaterial</c>.
/// </summary>
public sealed class MeshLambertNodeMaterial : NodeMaterial
{
	private float _lightMapIntensity = 1f;
	private float _aoMapIntensity = 1f;
	private float _emissiveIntensity = 1f;
	private float _bumpScale = 1f;
	private NormalMapTypes _normalMapType = NormalMapTypes.TangentSpaceNormalMap;
	private float _displacementScale = 0f;
	private float _displacementBias = 0f;
	private Combine _combine = Combine.MultiplyOperation;
	private float _reflectivity = 1f;
	private float _envMapIntensity = 1f;
	private float _refractionRatio = 0.98f;
	private bool _wireframe = false;
	private float _wireframeLinewidth = 1f;
	private bool _flatShading = false;
	private bool _isColorWritten;
	private bool _isLightMapIntensityWritten;
	private bool _isAoMapIntensityWritten;
	private bool _isEmissiveWritten;
	private bool _isEmissiveIntensityWritten;
	private bool _isBumpScaleWritten;
	private bool _isNormalMapTypeWritten;
	private bool _isDisplacementScaleWritten;
	private bool _isDisplacementBiasWritten;
	private bool _isEnvMapRotationWritten;
	private bool _isCombineWritten;
	private bool _isReflectivityWritten;
	private bool _isEnvMapIntensityWritten;
	private bool _isRefractionRatioWritten;
	private bool _isWireframeWritten;
	private bool _isWireframeLinewidthWritten;
	private bool _isFlatShadingWritten;

	/// <summary>
	/// Color of the material. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>
	/// Emissive (light) color of the material, essentially a solid color unaffected by other lighting.
	/// Mirrored as an instance this object owns: mutating it records a write of <c>emissive</c>.
	/// </summary>
	public Color Emissive { get; }

	/// <summary>
	/// The rotation of the environment map in radians. Mirrored as an instance this object owns:
	/// mutating it records a write of <c>envMapRotation</c>.
	/// </summary>
	public Euler EnvMapRotation { get; }

	/// <summary>Constructs a new mesh lambert node material.</summary>
	public MeshLambertNodeMaterial()
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};

		Emissive = new Color(0f, 0f, 0f);
		Emissive.OnChange = () =>
		{
			_isEmissiveWritten = true;
			RecordSet("emissive", Emissive);
		};

		EnvMapRotation = new Euler();
		EnvMapRotation.OnChange = () =>
		{
			_isEnvMapRotationWritten = true;
			RecordSet("envMapRotation", EnvMapRotation);
		};
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshLambertNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshLambertNodeMaterial"; }
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
	/// Emits the create op for <c>THREE.MeshLambertNodeMaterial</c>, then replays every property
	/// written before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isColorWritten)
		{
			batch.Set(Handle, "color", ThreeValue.Encode(Color));
		}

		if (_isLightMapIntensityWritten)
		{
			batch.Set(Handle, "lightMapIntensity", ThreeValue.Encode(_lightMapIntensity));
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

		if (_isBumpScaleWritten)
		{
			batch.Set(Handle, "bumpScale", ThreeValue.Encode(_bumpScale));
		}

		if (_isNormalMapTypeWritten)
		{
			batch.Set(Handle, "normalMapType", ThreeValue.Encode(_normalMapType));
		}

		if (_isDisplacementScaleWritten)
		{
			batch.Set(Handle, "displacementScale", ThreeValue.Encode(_displacementScale));
		}

		if (_isDisplacementBiasWritten)
		{
			batch.Set(Handle, "displacementBias", ThreeValue.Encode(_displacementBias));
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

		if (_isFlatShadingWritten)
		{
			batch.Set(Handle, "flatShading", ThreeValue.Encode(_flatShading));
		}
	}
}
