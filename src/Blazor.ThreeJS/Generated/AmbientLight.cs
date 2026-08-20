// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This light globally illuminates all objects in the scene equally. It cannot be used to cast
/// shadows as it does not have a direction. The JavaScript-side <c>THREE.AmbientLight</c>.
/// </summary>
public sealed class AmbientLight : Light
{
	private readonly Color? _color;
	private readonly float _intensity;

	/// <summary>Constructs a new ambient light.</summary>
	/// <param name="color">The light's color.</param>
	/// <param name="intensity">The light's strength/intensity.</param>
	public AmbientLight(Color? color = null, float intensity = 1f)
		: base(color: color, intensity: intensity)
	{
		_color = color;
		_intensity = intensity;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>AmbientLight</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal AmbientLight(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_intensity = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AmbientLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AmbientLight"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.AmbientLight</c>: color, intensity. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_color), _intensity]); }
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isAmbientLight</c> held.
	/// </summary>
	/// <returns>The value <c>isAmbientLight</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsAmbientLightAsync()
	{
		return GetAsync<bool>("isAmbientLight");
	}
}
