// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Light probes are an alternative way of adding light to a 3D scene. Unlike classical light
/// sources (e.g. directional, point or spot lights), light probes do not emit light. Instead they
/// store information about light passing through 3D space. During rendering, the light that hits a
/// 3D object is approximated by using the data from the light probe. Light probes are usually
/// created from (radiance) environment maps. The class <c>LightProbeGenerator</c> can be used to
/// create light probes from cube textures or render targets. However, light estimation data could
/// also be provided in other forms e.g. by WebXR. This enables the rendering of augmented reality
/// content that reacts to real world lighting. The current probe implementation in three.js
/// supports so-called diffuse light probes. This type of light probe is functionally equivalent to
/// an irradiance environment map. The JavaScript-side <c>THREE.LightProbe</c>.
/// </summary>
public sealed class LightProbe : Object3D
{
	private float _intensity = 1f;
	private bool _isColorWritten;
	private bool _isIntensityWritten;

	/// <summary>
	/// The light's color. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>Constructs a new light probe.</summary>
	public LightProbe()
	{
		Color = new Color();
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LightProbe</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LightProbe"; }
	}

	/// <summary>
	/// The light's intensity. Writing it records a <c>intensity</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float Intensity
	{
		get { return _intensity; }
		set
		{
			if (_intensity == value)
			{
				return;
			}

			_intensity = value;
			_isIntensityWritten = true;
			RecordSet("intensity", value);
		}
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
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isColorWritten)
		{
			batch.Set(Handle, "color", ThreeValue.Encode(Color));
		}

		if (_isIntensityWritten)
		{
			batch.Set(Handle, "intensity", ThreeValue.Encode(_intensity));
		}
	}
}
