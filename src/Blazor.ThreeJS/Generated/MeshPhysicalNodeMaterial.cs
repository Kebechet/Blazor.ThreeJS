// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="MeshPhysicalMaterial"/>. The JavaScript-side
/// <c>THREE.MeshPhysicalNodeMaterial</c>.
/// </summary>
public class MeshPhysicalNodeMaterial : MeshStandardNodeMaterial
{
	private float _anisotropyRotation = 1f;
	private Texture? _anisotropyMap = null;
	private Texture? _clearcoatMap = null;
	private float _clearcoatRoughness = 0f;
	private Texture? _clearcoatRoughnessMap = null;
	private Texture? _clearcoatNormalMap = null;
	private float _ior = 1.5f;
	private float _reflectivity = 0.5f;
	private Texture? _iridescenceMap = null;
	private float _iridescenceIOR = 1.3f;
	private Texture? _iridescenceThicknessMap = null;
	private Texture? _sheenColorMap = null;
	private float _sheenRoughness = 1f;
	private Texture? _sheenRoughnessMap = null;
	private Texture? _transmissionMap = null;
	private float _thickness = 0f;
	private Texture? _thicknessMap = null;
	private float _attenuationDistance;
	private float _specularIntensity = 1f;
	private Texture? _specularIntensityMap = null;
	private Texture? _specularColorMap = null;
	private float _anisotropy;
	private float _clearcoat;
	private float _iridescence;
	private float _dispersion;
	private float _sheen;
	private float _transmission;
	private bool _isAnisotropyRotationWritten;
	private bool _isAnisotropyMapWritten;
	private bool _isClearcoatMapWritten;
	private bool _isClearcoatRoughnessWritten;
	private bool _isClearcoatRoughnessMapWritten;
	private bool _isClearcoatNormalScaleWritten;
	private bool _isClearcoatNormalMapWritten;
	private bool _isIorWritten;
	private bool _isReflectivityWritten;
	private bool _isIridescenceMapWritten;
	private bool _isIridescenceIORWritten;
	private bool _isIridescenceThicknessMapWritten;
	private bool _isSheenColorWritten;
	private bool _isSheenColorMapWritten;
	private bool _isSheenRoughnessWritten;
	private bool _isSheenRoughnessMapWritten;
	private bool _isTransmissionMapWritten;
	private bool _isThicknessWritten;
	private bool _isThicknessMapWritten;
	private bool _isAttenuationDistanceWritten;
	private bool _isAttenuationColorWritten;
	private bool _isSpecularIntensityWritten;
	private bool _isSpecularIntensityMapWritten;
	private bool _isSpecularColorWritten;
	private bool _isSpecularColorMapWritten;
	private bool _isAnisotropyWritten;
	private bool _isClearcoatWritten;
	private bool _isIridescenceWritten;
	private bool _isDispersionWritten;
	private bool _isSheenWritten;
	private bool _isTransmissionWritten;

	/// <summary>
	/// How much <c>clearcoatNormalMap</c> affects the clear coat layer, from <c>(0,0)</c> to
	/// <c>(1,1)</c>. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>clearcoatNormalScale</c>.
	/// </summary>
	public Vector2 ClearcoatNormalScale { get; }

	/// <summary>
	/// The sheen tint. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>sheenColor</c>.
	/// </summary>
	public Color SheenColor { get; }

	/// <summary>
	/// The color that white light turns into due to absorption when reaching the attenuation distance.
	/// Mirrored as an instance this object owns: mutating it records a write of
	/// <c>attenuationColor</c>.
	/// </summary>
	public Color AttenuationColor { get; }

	/// <summary>
	/// Tints the specular reflection at normal incidence for non-metals only. Mirrored as an instance
	/// this object owns: mutating it records a write of <c>specularColor</c>.
	/// </summary>
	public Color SpecularColor { get; }

	/// <summary>Initializes a new <see cref="MeshPhysicalNodeMaterial"/>.</summary>
	public MeshPhysicalNodeMaterial()
	{
		ClearcoatNormalScale = new Vector2();
		ClearcoatNormalScale.OnChange = () =>
		{
			_isClearcoatNormalScaleWritten = true;
			RecordSet("clearcoatNormalScale", ClearcoatNormalScale);
		};

		SheenColor = new Color(0f, 0f, 0f);
		SheenColor.OnChange = () =>
		{
			_isSheenColorWritten = true;
			RecordSet("sheenColor", SheenColor);
		};

		AttenuationColor = new Color(1f, 1f, 1f);
		AttenuationColor.OnChange = () =>
		{
			_isAttenuationColorWritten = true;
			RecordSet("attenuationColor", AttenuationColor);
		};

		SpecularColor = new Color(1f, 1f, 1f);
		SpecularColor.OnChange = () =>
		{
			_isSpecularColorWritten = true;
			RecordSet("specularColor", SpecularColor);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>MeshPhysicalNodeMaterial</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal MeshPhysicalNodeMaterial(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		ClearcoatNormalScale = new Vector2();
		ClearcoatNormalScale.OnChange = () =>
		{
			_isClearcoatNormalScaleWritten = true;
			RecordSet("clearcoatNormalScale", ClearcoatNormalScale);
		};

		SheenColor = new Color(0f, 0f, 0f);
		SheenColor.OnChange = () =>
		{
			_isSheenColorWritten = true;
			RecordSet("sheenColor", SheenColor);
		};

		AttenuationColor = new Color(1f, 1f, 1f);
		AttenuationColor.OnChange = () =>
		{
			_isAttenuationColorWritten = true;
			RecordSet("attenuationColor", AttenuationColor);
		};

		SpecularColor = new Color(1f, 1f, 1f);
		SpecularColor.OnChange = () =>
		{
			_isSpecularColorWritten = true;
			RecordSet("specularColor", SpecularColor);
		};

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshPhysicalNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshPhysicalNodeMaterial"; }
	}

	/// <summary>
	/// The rotation of the anisotropy in tangent, bitangent space, measured in radians
	/// counter-clockwise from the tangent. When <c>anisotropyMap</c> is present, this property provides
	/// additional rotation to the vectors in the texture. Writing it records a
	/// <c>anisotropyRotation</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float AnisotropyRotation
	{
		get { return _anisotropyRotation; }
		set
		{
			if (_anisotropyRotation == value)
			{
				return;
			}

			_anisotropyRotation = value;
			_isAnisotropyRotationWritten = true;
			RecordSet("anisotropyRotation", value);
		}
	}

	/// <summary>
	/// Red and green channels represent the anisotropy direction in <c>[-1, 1]</c> tangent, bitangent
	/// space, to be rotated by <c>anisotropyRotation</c>. The blue channel contains strength as <c>[0,
	/// 1]</c> to be multiplied by <c>anisotropy</c>. <c>anisotropyMap</c> represents non-color data.
	/// Any texture assigned must have <c>texture.colorSpace = NoColorSpace</c> (default). Writing it
	/// records a <c>anisotropyMap</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public Texture? AnisotropyMap
	{
		get { return _anisotropyMap; }
		set
		{
			if (ReferenceEquals(_anisotropyMap, value))
			{
				return;
			}

			_anisotropyMap = value;
			_isAnisotropyMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("anisotropyMap", value);
		}
	}

	/// <summary>
	/// The red channel of this texture is multiplied against <c>clearcoat</c>, for per-pixel control
	/// over a coating's intensity. <c>clearcoatMap</c> represents non-color data. Any texture assigned
	/// must have <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a
	/// <c>clearcoatMap</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public Texture? ClearcoatMap
	{
		get { return _clearcoatMap; }
		set
		{
			if (ReferenceEquals(_clearcoatMap, value))
			{
				return;
			}

			_clearcoatMap = value;
			_isClearcoatMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("clearcoatMap", value);
		}
	}

	/// <summary>
	/// Roughness of the clear coat layer, from <c>0.0</c> to <c>1.0</c>. Writing it records a
	/// <c>clearcoatRoughness</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float ClearcoatRoughness
	{
		get { return _clearcoatRoughness; }
		set
		{
			if (_clearcoatRoughness == value)
			{
				return;
			}

			_clearcoatRoughness = value;
			_isClearcoatRoughnessWritten = true;
			RecordSet("clearcoatRoughness", value);
		}
	}

	/// <summary>
	/// The green channel of this texture is multiplied against <c>clearcoatRoughness</c>, for per-pixel
	/// control over a coating's roughness. <c>clearcoatRoughnessMap</c> represents non-color data. Any
	/// texture assigned must have <c>texture.colorSpace = NoColorSpace</c> (default). Writing it
	/// records a <c>clearcoatRoughnessMap</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public Texture? ClearcoatRoughnessMap
	{
		get { return _clearcoatRoughnessMap; }
		set
		{
			if (ReferenceEquals(_clearcoatRoughnessMap, value))
			{
				return;
			}

			_clearcoatRoughnessMap = value;
			_isClearcoatRoughnessMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("clearcoatRoughnessMap", value);
		}
	}

	/// <summary>
	/// Can be used to enable independent normals for the clear coat layer. <c>clearcoatNormalMap</c>
	/// represents non-color data. Any texture assigned must have <c>texture.colorSpace =
	/// NoColorSpace</c> (default). Writing it records a <c>clearcoatNormalMap</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? ClearcoatNormalMap
	{
		get { return _clearcoatNormalMap; }
		set
		{
			if (ReferenceEquals(_clearcoatNormalMap, value))
			{
				return;
			}

			_clearcoatNormalMap = value;
			_isClearcoatNormalMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("clearcoatNormalMap", value);
		}
	}

	/// <summary>
	/// Index-of-refraction for non-metallic materials, from <c>1.0</c> to <c>2.333</c>. Writing it
	/// records a <c>ior</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Ior
	{
		get { return _ior; }
		set
		{
			if (_ior == value)
			{
				return;
			}

			_ior = value;
			_isIorWritten = true;
			RecordSet("ior", value);
		}
	}

	/// <summary>
	/// Degree of reflectivity, from <c>0.0</c> to <c>1.0</c>. Default is <c>0.5</c>, which corresponds
	/// to an index-of-refraction of <c>1.5</c>. This models the reflectivity of non-metallic materials.
	/// It has no effect when <c>metalness</c> is <c>1.0</c>. Writing it records a <c>reflectivity</c>
	/// property write once this object is attached; writing the value already held records nothing.
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
	/// The red channel of this texture is multiplied against <c>iridescence</c>, for per-pixel control
	/// over iridescence. <c>iridescenceMap</c> represents non-color data. Any texture assigned must
	/// have <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a
	/// <c>iridescenceMap</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public Texture? IridescenceMap
	{
		get { return _iridescenceMap; }
		set
		{
			if (ReferenceEquals(_iridescenceMap, value))
			{
				return;
			}

			_iridescenceMap = value;
			_isIridescenceMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("iridescenceMap", value);
		}
	}

	/// <summary>
	/// Strength of the iridescence RGB color shift effect, represented by an index-of-refraction.
	/// Between <c>1.0</c> to <c>2.333</c>. Writing it records a <c>iridescenceIOR</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float IridescenceIOR
	{
		get { return _iridescenceIOR; }
		set
		{
			if (_iridescenceIOR == value)
			{
				return;
			}

			_iridescenceIOR = value;
			_isIridescenceIORWritten = true;
			RecordSet("iridescenceIOR", value);
		}
	}

	/// <summary>
	/// A texture that defines the thickness of the iridescence layer, stored in the green channel.
	/// Minimum and maximum values of thickness are defined by <c>iridescenceThicknessRange</c> array: -
	/// <c>0.0</c> in the green channel will result in thickness equal to first element of the array. -
	/// <c>1.0</c> in the green channel will result in thickness equal to second element of the array. -
	/// Values in-between will linearly interpolate between the elements of the array.
	/// <c>iridescenceThicknessMap</c> represents non-color data. Any texture assigned must have
	/// <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a
	/// <c>iridescenceThicknessMap</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public Texture? IridescenceThicknessMap
	{
		get { return _iridescenceThicknessMap; }
		set
		{
			if (ReferenceEquals(_iridescenceThicknessMap, value))
			{
				return;
			}

			_iridescenceThicknessMap = value;
			_isIridescenceThicknessMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("iridescenceThicknessMap", value);
		}
	}

	/// <summary>
	/// The RGB channels of this texture are multiplied against <c>sheenColor</c>, for per-pixel control
	/// over sheen tint. <c>sheenColorMap</c> represents color data, and the texture must be assigned a
	/// <c>Texture#colorSpace</c>. Most <c>sheenColorMap</c> textures set <c>texture.colorSpace =
	/// SRGBColorSpace</c>. Writing it records a <c>sheenColorMap</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public Texture? SheenColorMap
	{
		get { return _sheenColorMap; }
		set
		{
			if (ReferenceEquals(_sheenColorMap, value))
			{
				return;
			}

			_sheenColorMap = value;
			_isSheenColorMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("sheenColorMap", value);
		}
	}

	/// <summary>
	/// Roughness of the sheen layer, from <c>0.0</c> to <c>1.0</c>. Writing it records a
	/// <c>sheenRoughness</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float SheenRoughness
	{
		get { return _sheenRoughness; }
		set
		{
			if (_sheenRoughness == value)
			{
				return;
			}

			_sheenRoughness = value;
			_isSheenRoughnessWritten = true;
			RecordSet("sheenRoughness", value);
		}
	}

	/// <summary>
	/// The alpha channel of this texture is multiplied against <c>sheenRoughness</c>, for per-pixel
	/// control over sheen roughness. <c>sheenRoughnessMap</c> represents non-color data. Any texture
	/// assigned must have <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a
	/// <c>sheenRoughnessMap</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public Texture? SheenRoughnessMap
	{
		get { return _sheenRoughnessMap; }
		set
		{
			if (ReferenceEquals(_sheenRoughnessMap, value))
			{
				return;
			}

			_sheenRoughnessMap = value;
			_isSheenRoughnessMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("sheenRoughnessMap", value);
		}
	}

	/// <summary>
	/// The red channel of this texture is multiplied against <c>transmission</c>, for per-pixel control
	/// over optical transparency. <c>transmissionMap</c> represents non-color data. Any texture
	/// assigned must have <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a
	/// <c>transmissionMap</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public Texture? TransmissionMap
	{
		get { return _transmissionMap; }
		set
		{
			if (ReferenceEquals(_transmissionMap, value))
			{
				return;
			}

			_transmissionMap = value;
			_isTransmissionMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("transmissionMap", value);
		}
	}

	/// <summary>
	/// The thickness of the volume beneath the surface. The value is given in the coordinate space of
	/// the mesh. If the value is <c>0</c> the material is thin-walled. Otherwise the material is a
	/// volume boundary. Writing it records a <c>thickness</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float Thickness
	{
		get { return _thickness; }
		set
		{
			if (_thickness == value)
			{
				return;
			}

			_thickness = value;
			_isThicknessWritten = true;
			RecordSet("thickness", value);
		}
	}

	/// <summary>
	/// A texture that defines the thickness, stored in the green channel. This will be multiplied by
	/// <c>thickness</c>. <c>thicknessMap</c> represents non-color data. Any texture assigned must have
	/// <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a <c>thicknessMap</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? ThicknessMap
	{
		get { return _thicknessMap; }
		set
		{
			if (ReferenceEquals(_thicknessMap, value))
			{
				return;
			}

			_thicknessMap = value;
			_isThicknessMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("thicknessMap", value);
		}
	}

	/// <summary>
	/// Density of the medium given as the average distance that light travels in the medium before
	/// interacting with a particle. The value is given in world space units, and must be greater than
	/// zero. Writing it records a <c>attenuationDistance</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float AttenuationDistance
	{
		get { return _attenuationDistance; }
		set
		{
			if (_attenuationDistance == value)
			{
				return;
			}

			_attenuationDistance = value;
			_isAttenuationDistanceWritten = true;
			RecordSet("attenuationDistance", value);
		}
	}

	/// <summary>
	/// A float that scales the amount of specular reflection for non-metals only. When set to zero, the
	/// model is effectively Lambertian. From <c>0.0</c> to <c>1.0</c>. Writing it records a
	/// <c>specularIntensity</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float SpecularIntensity
	{
		get { return _specularIntensity; }
		set
		{
			if (_specularIntensity == value)
			{
				return;
			}

			_specularIntensity = value;
			_isSpecularIntensityWritten = true;
			RecordSet("specularIntensity", value);
		}
	}

	/// <summary>
	/// The alpha channel of this texture is multiplied against <c>specularIntensity</c>, for per-pixel
	/// control over specular intensity. <c>specularIntensityMap</c> represents non-color data. Any
	/// texture assigned must have <c>texture.colorSpace = NoColorSpace</c> (default). Writing it
	/// records a <c>specularIntensityMap</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public Texture? SpecularIntensityMap
	{
		get { return _specularIntensityMap; }
		set
		{
			if (ReferenceEquals(_specularIntensityMap, value))
			{
				return;
			}

			_specularIntensityMap = value;
			_isSpecularIntensityMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("specularIntensityMap", value);
		}
	}

	/// <summary>
	/// The RGB channels of this texture are multiplied against <c>specularColor</c>, for per-pixel
	/// control over specular color. <c>specularColorMap</c> represents color data, and the texture must
	/// be assigned a <c>Texture#colorSpace</c>. Most <c>specularColorMap</c> textures set
	/// <c>texture.colorSpace = SRGBColorSpace</c>. Writing it records a <c>specularColorMap</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? SpecularColorMap
	{
		get { return _specularColorMap; }
		set
		{
			if (ReferenceEquals(_specularColorMap, value))
			{
				return;
			}

			_specularColorMap = value;
			_isSpecularColorMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("specularColorMap", value);
		}
	}

	/// <summary>
	/// The <c>anisotropy</c> property of the JavaScript-side object. Writing it records a
	/// <c>anisotropy</c> property write once this object is attached; writing the value already held
	/// records nothing.
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
	/// The <c>clearcoat</c> property of the JavaScript-side object. Writing it records a
	/// <c>clearcoat</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Clearcoat
	{
		get { return _clearcoat; }
		set
		{
			if (_clearcoat == value)
			{
				return;
			}

			_clearcoat = value;
			_isClearcoatWritten = true;
			RecordSet("clearcoat", value);
		}
	}

	/// <summary>
	/// The <c>iridescence</c> property of the JavaScript-side object. Writing it records a
	/// <c>iridescence</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Iridescence
	{
		get { return _iridescence; }
		set
		{
			if (_iridescence == value)
			{
				return;
			}

			_iridescence = value;
			_isIridescenceWritten = true;
			RecordSet("iridescence", value);
		}
	}

	/// <summary>
	/// The <c>dispersion</c> property of the JavaScript-side object. Writing it records a
	/// <c>dispersion</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Dispersion
	{
		get { return _dispersion; }
		set
		{
			if (_dispersion == value)
			{
				return;
			}

			_dispersion = value;
			_isDispersionWritten = true;
			RecordSet("dispersion", value);
		}
	}

	/// <summary>
	/// The <c>sheen</c> property of the JavaScript-side object. Writing it records a <c>sheen</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Sheen
	{
		get { return _sheen; }
		set
		{
			if (_sheen == value)
			{
				return;
			}

			_sheen = value;
			_isSheenWritten = true;
			RecordSet("sheen", value);
		}
	}

	/// <summary>
	/// The <c>transmission</c> property of the JavaScript-side object. Writing it records a
	/// <c>transmission</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Transmission
	{
		get { return _transmission; }
		set
		{
			if (_transmission == value)
			{
				return;
			}

			_transmission = value;
			_isTransmissionWritten = true;
			RecordSet("transmission", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isMeshPhysicalNodeMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isMeshPhysicalNodeMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsMeshPhysicalNodeMaterialAsync()
	{
		return GetAsync<bool>("isMeshPhysicalNodeMaterial");
	}

	/// <summary>
	/// Whether the lighting model should use clearcoat or not. Read-only in three.js, so it is read on
	/// demand rather than mirrored: records a get op, sends it behind every write already pending, and
	/// completes with the value <c>useClearcoat</c> held.
	/// </summary>
	/// <returns>The value <c>useClearcoat</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> UseClearcoatAsync()
	{
		return GetAsync<bool>("useClearcoat");
	}

	/// <summary>
	/// Whether the lighting model should use iridescence or not. Read-only in three.js, so it is read
	/// on demand rather than mirrored: records a get op, sends it behind every write already pending,
	/// and completes with the value <c>useIridescence</c> held.
	/// </summary>
	/// <returns>The value <c>useIridescence</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> UseIridescenceAsync()
	{
		return GetAsync<bool>("useIridescence");
	}

	/// <summary>
	/// Whether the lighting model should use sheen or not. Read-only in three.js, so it is read on
	/// demand rather than mirrored: records a get op, sends it behind every write already pending, and
	/// completes with the value <c>useSheen</c> held.
	/// </summary>
	/// <returns>The value <c>useSheen</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> UseSheenAsync()
	{
		return GetAsync<bool>("useSheen");
	}

	/// <summary>
	/// Whether the lighting model should use anisotropy or not. Read-only in three.js, so it is read on
	/// demand rather than mirrored: records a get op, sends it behind every write already pending, and
	/// completes with the value <c>useAnisotropy</c> held.
	/// </summary>
	/// <returns>The value <c>useAnisotropy</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> UseAnisotropyAsync()
	{
		return GetAsync<bool>("useAnisotropy");
	}

	/// <summary>
	/// Whether the lighting model should use transmission or not. Read-only in three.js, so it is read
	/// on demand rather than mirrored: records a get op, sends it behind every write already pending,
	/// and completes with the value <c>useTransmission</c> held.
	/// </summary>
	/// <returns>The value <c>useTransmission</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> UseTransmissionAsync()
	{
		return GetAsync<bool>("useTransmission");
	}

	/// <summary>
	/// Whether the lighting model should use dispersion or not. Read-only in three.js, so it is read on
	/// demand rather than mirrored: records a get op, sends it behind every write already pending, and
	/// completes with the value <c>useDispersion</c> held.
	/// </summary>
	/// <returns>The value <c>useDispersion</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> UseDispersionAsync()
	{
		return GetAsync<bool>("useDispersion");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.MeshPhysicalNodeMaterial</c>, then replays every property
	/// written before this object was attached. A replayed value that is itself a mirrored object is
	/// attached first, so its create op reaches the batch before the write that references it by
	/// handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isAnisotropyRotationWritten)
		{
			batch.Set(Handle, "anisotropyRotation", ThreeValue.Encode(_anisotropyRotation));
		}

		if (_isAnisotropyMapWritten)
		{
			_anisotropyMap?.AttachTo(batch);
			batch.Set(Handle, "anisotropyMap", ThreeValue.Encode(_anisotropyMap));
		}

		if (_isClearcoatMapWritten)
		{
			_clearcoatMap?.AttachTo(batch);
			batch.Set(Handle, "clearcoatMap", ThreeValue.Encode(_clearcoatMap));
		}

		if (_isClearcoatRoughnessWritten)
		{
			batch.Set(Handle, "clearcoatRoughness", ThreeValue.Encode(_clearcoatRoughness));
		}

		if (_isClearcoatRoughnessMapWritten)
		{
			_clearcoatRoughnessMap?.AttachTo(batch);
			batch.Set(Handle, "clearcoatRoughnessMap", ThreeValue.Encode(_clearcoatRoughnessMap));
		}

		if (_isClearcoatNormalScaleWritten)
		{
			batch.Set(Handle, "clearcoatNormalScale", ThreeValue.Encode(ClearcoatNormalScale));
		}

		if (_isClearcoatNormalMapWritten)
		{
			_clearcoatNormalMap?.AttachTo(batch);
			batch.Set(Handle, "clearcoatNormalMap", ThreeValue.Encode(_clearcoatNormalMap));
		}

		if (_isIorWritten)
		{
			batch.Set(Handle, "ior", ThreeValue.Encode(_ior));
		}

		if (_isReflectivityWritten)
		{
			batch.Set(Handle, "reflectivity", ThreeValue.Encode(_reflectivity));
		}

		if (_isIridescenceMapWritten)
		{
			_iridescenceMap?.AttachTo(batch);
			batch.Set(Handle, "iridescenceMap", ThreeValue.Encode(_iridescenceMap));
		}

		if (_isIridescenceIORWritten)
		{
			batch.Set(Handle, "iridescenceIOR", ThreeValue.Encode(_iridescenceIOR));
		}

		if (_isIridescenceThicknessMapWritten)
		{
			_iridescenceThicknessMap?.AttachTo(batch);
			batch.Set(Handle, "iridescenceThicknessMap", ThreeValue.Encode(_iridescenceThicknessMap));
		}

		if (_isSheenColorWritten)
		{
			batch.Set(Handle, "sheenColor", ThreeValue.Encode(SheenColor));
		}

		if (_isSheenColorMapWritten)
		{
			_sheenColorMap?.AttachTo(batch);
			batch.Set(Handle, "sheenColorMap", ThreeValue.Encode(_sheenColorMap));
		}

		if (_isSheenRoughnessWritten)
		{
			batch.Set(Handle, "sheenRoughness", ThreeValue.Encode(_sheenRoughness));
		}

		if (_isSheenRoughnessMapWritten)
		{
			_sheenRoughnessMap?.AttachTo(batch);
			batch.Set(Handle, "sheenRoughnessMap", ThreeValue.Encode(_sheenRoughnessMap));
		}

		if (_isTransmissionMapWritten)
		{
			_transmissionMap?.AttachTo(batch);
			batch.Set(Handle, "transmissionMap", ThreeValue.Encode(_transmissionMap));
		}

		if (_isThicknessWritten)
		{
			batch.Set(Handle, "thickness", ThreeValue.Encode(_thickness));
		}

		if (_isThicknessMapWritten)
		{
			_thicknessMap?.AttachTo(batch);
			batch.Set(Handle, "thicknessMap", ThreeValue.Encode(_thicknessMap));
		}

		if (_isAttenuationDistanceWritten)
		{
			batch.Set(Handle, "attenuationDistance", ThreeValue.Encode(_attenuationDistance));
		}

		if (_isAttenuationColorWritten)
		{
			batch.Set(Handle, "attenuationColor", ThreeValue.Encode(AttenuationColor));
		}

		if (_isSpecularIntensityWritten)
		{
			batch.Set(Handle, "specularIntensity", ThreeValue.Encode(_specularIntensity));
		}

		if (_isSpecularIntensityMapWritten)
		{
			_specularIntensityMap?.AttachTo(batch);
			batch.Set(Handle, "specularIntensityMap", ThreeValue.Encode(_specularIntensityMap));
		}

		if (_isSpecularColorWritten)
		{
			batch.Set(Handle, "specularColor", ThreeValue.Encode(SpecularColor));
		}

		if (_isSpecularColorMapWritten)
		{
			_specularColorMap?.AttachTo(batch);
			batch.Set(Handle, "specularColorMap", ThreeValue.Encode(_specularColorMap));
		}

		if (_isAnisotropyWritten)
		{
			batch.Set(Handle, "anisotropy", ThreeValue.Encode(_anisotropy));
		}

		if (_isClearcoatWritten)
		{
			batch.Set(Handle, "clearcoat", ThreeValue.Encode(_clearcoat));
		}

		if (_isIridescenceWritten)
		{
			batch.Set(Handle, "iridescence", ThreeValue.Encode(_iridescence));
		}

		if (_isDispersionWritten)
		{
			batch.Set(Handle, "dispersion", ThreeValue.Encode(_dispersion));
		}

		if (_isSheenWritten)
		{
			batch.Set(Handle, "sheen", ThreeValue.Encode(_sheen));
		}

		if (_isTransmissionWritten)
		{
			batch.Set(Handle, "transmission", ThreeValue.Encode(_transmission));
		}
	}
}
