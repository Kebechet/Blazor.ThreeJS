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
public sealed class LightProbe : Light
{
	private readonly SphericalHarmonics3? _sh;
	private readonly float _intensity;

	/// <summary>Constructs a new light probe.</summary>
	/// <param name="sh">The spherical harmonics which represents encoded lighting information.</param>
	/// <param name="intensity">The light's strength/intensity.</param>
	public LightProbe(SphericalHarmonics3? sh = null, float intensity = 1f)
		: base(intensity: intensity)
	{
		_sh = sh;
		_intensity = intensity;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>LightProbe</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal LightProbe(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_intensity = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LightProbe</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LightProbe"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.LightProbe</c>: sh, intensity. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_sh), _intensity]); }
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isLightProbe</c> held.
	/// </summary>
	/// <returns>The value <c>isLightProbe</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsLightProbeAsync()
	{
		return GetAsync<bool>("isLightProbe");
	}
}
