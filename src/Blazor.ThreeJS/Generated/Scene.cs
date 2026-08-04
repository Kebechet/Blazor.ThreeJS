// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Scenes allow you to set up what is to be rendered and where by three.js. This is where you place
/// 3D objects like meshes, lines or lights. The JavaScript-side <c>THREE.Scene</c>.
/// </summary>
public sealed class Scene : Object3D
{
	private float _backgroundBlurriness = 0f;
	private float _backgroundIntensity = 1f;
	private float _environmentIntensity = 1f;
	private Material? _overrideMaterial = null;
	private bool _isBackgroundBlurrinessWritten;
	private bool _isBackgroundIntensityWritten;
	private bool _isBackgroundRotationWritten;
	private bool _isEnvironmentIntensityWritten;
	private bool _isEnvironmentRotationWritten;
	private bool _isOverrideMaterialWritten;

	/// <summary>
	/// The rotation of the background in radians. Only influences environment maps assigned to
	/// <c>Scene#background</c>. Mirrored as an instance this object owns: mutating it records a write
	/// of <c>backgroundRotation</c>.
	/// </summary>
	public Euler BackgroundRotation { get; }

	/// <summary>
	/// The rotation of the environment map in radians. Only influences physical materials in the scene
	/// when <c>Scene#environment</c> is used. Mirrored as an instance this object owns: mutating it
	/// records a write of <c>environmentRotation</c>.
	/// </summary>
	public Euler EnvironmentRotation { get; }

	/// <summary>Initializes a new <see cref="Scene"/>.</summary>
	public Scene()
	{
		BackgroundRotation = new Euler();
		BackgroundRotation.OnChange = () =>
		{
			_isBackgroundRotationWritten = true;
			RecordSet("backgroundRotation", BackgroundRotation);
		};

		EnvironmentRotation = new Euler();
		EnvironmentRotation.OnChange = () =>
		{
			_isEnvironmentRotationWritten = true;
			RecordSet("environmentRotation", EnvironmentRotation);
		};
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Scene</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Scene"; }
	}

	/// <summary>
	/// Sets the blurriness of the background. Only influences environment maps assigned to
	/// <c>Scene#background</c>. Valid input is a float between <c>0</c> and <c>1</c>. Writing it
	/// records a <c>backgroundBlurriness</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public float BackgroundBlurriness
	{
		get { return _backgroundBlurriness; }
		set
		{
			if (_backgroundBlurriness == value)
			{
				return;
			}

			_backgroundBlurriness = value;
			_isBackgroundBlurrinessWritten = true;
			RecordSet("backgroundBlurriness", value);
		}
	}

	/// <summary>
	/// Attenuates the color of the background. Only applies to background textures. Writing it records
	/// a <c>backgroundIntensity</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float BackgroundIntensity
	{
		get { return _backgroundIntensity; }
		set
		{
			if (_backgroundIntensity == value)
			{
				return;
			}

			_backgroundIntensity = value;
			_isBackgroundIntensityWritten = true;
			RecordSet("backgroundIntensity", value);
		}
	}

	/// <summary>
	/// Attenuates the color of the environment. Only influences environment maps assigned to
	/// <c>Scene#environment</c>. Writing it records a <c>environmentIntensity</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float EnvironmentIntensity
	{
		get { return _environmentIntensity; }
		set
		{
			if (_environmentIntensity == value)
			{
				return;
			}

			_environmentIntensity = value;
			_isEnvironmentIntensityWritten = true;
			RecordSet("environmentIntensity", value);
		}
	}

	/// <summary>
	/// Forces everything in the scene to be rendered with the defined material. It is possible to
	/// exclude materials from override by setting <c>Material#allowOverride</c> to <c>false</c>.
	/// Writing it records a <c>overrideMaterial</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public Material? OverrideMaterial
	{
		get { return _overrideMaterial; }
		set
		{
			if (ReferenceEquals(_overrideMaterial, value))
			{
				return;
			}

			_overrideMaterial = value;
			_isOverrideMaterialWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("overrideMaterial", value);
		}
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isBackgroundBlurrinessWritten)
		{
			batch.Set(Handle, "backgroundBlurriness", ThreeValue.Encode(_backgroundBlurriness));
		}

		if (_isBackgroundIntensityWritten)
		{
			batch.Set(Handle, "backgroundIntensity", ThreeValue.Encode(_backgroundIntensity));
		}

		if (_isBackgroundRotationWritten)
		{
			batch.Set(Handle, "backgroundRotation", ThreeValue.Encode(BackgroundRotation));
		}

		if (_isEnvironmentIntensityWritten)
		{
			batch.Set(Handle, "environmentIntensity", ThreeValue.Encode(_environmentIntensity));
		}

		if (_isEnvironmentRotationWritten)
		{
			batch.Set(Handle, "environmentRotation", ThreeValue.Encode(EnvironmentRotation));
		}

		if (_isOverrideMaterialWritten)
		{
			batch.Set(Handle, "overrideMaterial", ThreeValue.Encode(_overrideMaterial));
		}
	}
}
