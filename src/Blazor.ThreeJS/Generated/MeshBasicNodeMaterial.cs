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
	private float _lightMapIntensity = 1f;
	private float _aoMapIntensity = 1f;
	private Combine _combine = Combine.MultiplyOperation;
	private float _reflectivity = 1f;
	private float _refractionRatio = 0.98f;
	private bool _wireframe = false;
	private float _wireframeLinewidth = 1f;
	private bool _isColorWritten;
	private bool _isLightMapIntensityWritten;
	private bool _isAoMapIntensityWritten;
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

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshBasicNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshBasicNodeMaterial"; }
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
	/// Emits the create op for <c>THREE.MeshBasicNodeMaterial</c>, then replays every property written
	/// before this object was attached.
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
