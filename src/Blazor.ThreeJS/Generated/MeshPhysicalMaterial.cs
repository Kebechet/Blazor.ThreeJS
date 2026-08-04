// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// An extension of the <see cref="MeshStandardMaterial"/>, providing more advanced physically-based
/// rendering properties: - Anisotropy: Ability to represent the anisotropic property of materials
/// as observable with brushed metals. - Clearcoat: Some materials — like car paints, carbon fiber,
/// and wet surfaces — require a clear, reflective layer on top of another layer that may be
/// irregular or rough. Clearcoat approximates this effect, without the need for a separate
/// transparent surface. - Iridescence: Allows to render the effect where hue varies depending on
/// the viewing angle and illumination angle. This can be seen on soap bubbles, oil films, or on the
/// wings of many insects. - Physically-based transparency: One limitation of
/// <c>Material#opacity</c> is that highly transparent materials are less reflective.
/// Physically-based transmission provides a more realistic option for thin, transparent surfaces
/// like glass. - Advanced reflectivity: More flexible reflectivity for non-metallic materials. -
/// Sheen: Can be used for representing cloth and fabric materials. As a result of these complex
/// shading features, <c>MeshPhysicalMaterial</c> has a higher performance cost, per pixel, than
/// other three.js materials. Most effects are disabled by default, and add cost as they are
/// enabled. For best results, always specify an environment map when using this material. The
/// JavaScript-side <c>THREE.MeshPhysicalMaterial</c>.
/// </summary>
public sealed class MeshPhysicalMaterial : MeshStandardMaterial
{
	private float _anisotropyRotation = 1f;
	private float _clearcoatRoughness = 0f;
	private float _ior = 1.5f;
	private float _reflectivity = 0.5f;
	private float _iridescenceIOR = 1.3f;
	private float _sheenRoughness = 1f;
	private float _thickness = 0f;
	private float _attenuationDistance;
	private float _specularIntensity = 1f;
	private float _anisotropy;
	private float _clearcoat;
	private float _iridescence;
	private float _dispersion;
	private float _sheen;
	private float _transmission;
	private bool _isAnisotropyRotationWritten;
	private bool _isClearcoatRoughnessWritten;
	private bool _isIorWritten;
	private bool _isReflectivityWritten;
	private bool _isIridescenceIORWritten;
	private bool _isSheenColorWritten;
	private bool _isSheenRoughnessWritten;
	private bool _isThicknessWritten;
	private bool _isAttenuationDistanceWritten;
	private bool _isAttenuationColorWritten;
	private bool _isSpecularIntensityWritten;
	private bool _isSpecularColorWritten;
	private bool _isAnisotropyWritten;
	private bool _isClearcoatWritten;
	private bool _isIridescenceWritten;
	private bool _isDispersionWritten;
	private bool _isSheenWritten;
	private bool _isTransmissionWritten;

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

	/// <summary>Initializes a new <see cref="MeshPhysicalMaterial"/>.</summary>
	public MeshPhysicalMaterial()
	{
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

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshPhysicalMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshPhysicalMaterial"; }
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
	/// Emits the create op for <c>THREE.MeshPhysicalMaterial</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isAnisotropyRotationWritten)
		{
			batch.Set(Handle, "anisotropyRotation", ThreeValue.Encode(_anisotropyRotation));
		}

		if (_isClearcoatRoughnessWritten)
		{
			batch.Set(Handle, "clearcoatRoughness", ThreeValue.Encode(_clearcoatRoughness));
		}

		if (_isIorWritten)
		{
			batch.Set(Handle, "ior", ThreeValue.Encode(_ior));
		}

		if (_isReflectivityWritten)
		{
			batch.Set(Handle, "reflectivity", ThreeValue.Encode(_reflectivity));
		}

		if (_isIridescenceIORWritten)
		{
			batch.Set(Handle, "iridescenceIOR", ThreeValue.Encode(_iridescenceIOR));
		}

		if (_isSheenColorWritten)
		{
			batch.Set(Handle, "sheenColor", ThreeValue.Encode(SheenColor));
		}

		if (_isSheenRoughnessWritten)
		{
			batch.Set(Handle, "sheenRoughness", ThreeValue.Encode(_sheenRoughness));
		}

		if (_isThicknessWritten)
		{
			batch.Set(Handle, "thickness", ThreeValue.Encode(_thickness));
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

		if (_isSpecularColorWritten)
		{
			batch.Set(Handle, "specularColor", ThreeValue.Encode(SpecularColor));
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
