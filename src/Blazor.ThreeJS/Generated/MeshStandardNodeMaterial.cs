// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="MeshStandardMaterial"/>. The JavaScript-side
/// <c>THREE.MeshStandardNodeMaterial</c>.
/// </summary>
public class MeshStandardNodeMaterial : NodeMaterial
{
	private float _roughness = 1f;
	private float _metalness = 0f;
	private float _lightMapIntensity = 1f;
	private float _aoMapIntensity = 1f;
	private float _emissiveIntensity = 1f;
	private float _bumpScale = 1f;
	private NormalMapTypes _normalMapType = NormalMapTypes.TangentSpaceNormalMap;
	private float _displacementScale = 0f;
	private float _displacementBias = 0f;
	private float _envMapIntensity = 1f;
	private bool _wireframe = false;
	private float _wireframeLinewidth = 1f;
	private bool _flatShading = false;
	private bool _isColorWritten;
	private bool _isRoughnessWritten;
	private bool _isMetalnessWritten;
	private bool _isLightMapIntensityWritten;
	private bool _isAoMapIntensityWritten;
	private bool _isEmissiveWritten;
	private bool _isEmissiveIntensityWritten;
	private bool _isBumpScaleWritten;
	private bool _isNormalMapTypeWritten;
	private bool _isDisplacementScaleWritten;
	private bool _isDisplacementBiasWritten;
	private bool _isEnvMapRotationWritten;
	private bool _isEnvMapIntensityWritten;
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

	/// <summary>Constructs a new mesh standard node material.</summary>
	public MeshStandardNodeMaterial()
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

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshStandardNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshStandardNodeMaterial"; }
	}

	/// <summary>
	/// How rough the material appears. <c>0.0</c> means a smooth mirror reflection, <c>1.0</c> means
	/// fully diffuse. If <c>roughnessMap</c> is also provided, both values are multiplied. Writing it
	/// records a <c>roughness</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float Roughness
	{
		get { return _roughness; }
		set
		{
			if (_roughness == value)
			{
				return;
			}

			_roughness = value;
			_isRoughnessWritten = true;
			RecordSet("roughness", value);
		}
	}

	/// <summary>
	/// How much the material is like a metal. Non-metallic materials such as wood or stone use
	/// <c>0.0</c>, metallic use <c>1.0</c>, with nothing (usually) in between. A value between
	/// <c>0.0</c> and <c>1.0</c> could be used for a rusty metal look. If <c>metalnessMap</c> is also
	/// provided, both values are multiplied. Writing it records a <c>metalness</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Metalness
	{
		get { return _metalness; }
		set
		{
			if (_metalness == value)
			{
				return;
			}

			_metalness = value;
			_isMetalnessWritten = true;
			RecordSet("metalness", value);
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

	/// <summary>Setups the specular related node variables.</summary>
	public void SetupSpecular()
	{
		RecordCall("setupSpecular");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.MeshStandardNodeMaterial</c>, then replays every property
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

		if (_isRoughnessWritten)
		{
			batch.Set(Handle, "roughness", ThreeValue.Encode(_roughness));
		}

		if (_isMetalnessWritten)
		{
			batch.Set(Handle, "metalness", ThreeValue.Encode(_metalness));
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

		if (_isEnvMapIntensityWritten)
		{
			batch.Set(Handle, "envMapIntensity", ThreeValue.Encode(_envMapIntensity));
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
