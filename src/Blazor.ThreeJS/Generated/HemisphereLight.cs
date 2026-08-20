// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A light source positioned directly above the scene, with color fading from the sky color to the
/// ground color. This light cannot be used to cast shadows. The JavaScript-side
/// <c>THREE.HemisphereLight</c>.
/// </summary>
public sealed class HemisphereLight : Light
{
	private readonly Color? _skyColor;
	private readonly Color? _groundColor;
	private readonly float _intensity;

	/// <summary>Constructs a new hemisphere light.</summary>
	/// <param name="skyColor">The light's sky color.</param>
	/// <param name="groundColor">The light's ground color.</param>
	/// <param name="intensity">The light's strength/intensity.</param>
	public HemisphereLight(Color? skyColor = null, Color? groundColor = null, float intensity = 1f)
		: base(intensity: intensity)
	{
		_skyColor = skyColor;
		_groundColor = groundColor;
		_intensity = intensity;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>HemisphereLight</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal HemisphereLight(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_intensity = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.HemisphereLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "HemisphereLight"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.HemisphereLight</c>: skyColor, groundColor,
	/// intensity. An argument the caller left unspecified travels as the wire's not-supplied sentinel,
	/// or is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_skyColor),
				ThreeValue.OrUnspecified(_groundColor),
				_intensity
			]);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isHemisphereLight</c> held.
	/// </summary>
	/// <returns>The value <c>isHemisphereLight</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsHemisphereLightAsync()
	{
		return GetAsync<bool>("isHemisphereLight");
	}
}
