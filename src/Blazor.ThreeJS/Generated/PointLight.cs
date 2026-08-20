// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A light that gets emitted from a single point in all directions. A common use case for this is
/// to replicate the light emitted from a bare lightbulb. This light can cast shadows - see the
/// <c>PointLightShadow</c> for details. The JavaScript-side <c>THREE.PointLight</c>.
/// </summary>
public sealed class PointLight : Light
{
	private readonly Color? _color;
	private readonly float _intensity;
	private float _distance;
	private float _decay;
	private float _power;
	private bool _isDistanceWritten;
	private bool _isDecayWritten;
	private bool _isPowerWritten;

	/// <summary>Constructs a new point light.</summary>
	/// <param name="color">The light's color.</param>
	/// <param name="intensity">The light's strength/intensity measured in candela (cd).</param>
	/// <param name="distance">Maximum range of the light. <c>0</c> means no limit.</param>
	/// <param name="decay">The amount the light dims along the distance of the light.</param>
	public PointLight(Color? color = null, float intensity = 1f, float distance = 0f, float decay = 2f)
		: base(color: color, intensity: intensity)
	{
		_color = color;
		_intensity = intensity;
		_distance = distance;
		_decay = decay;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>PointLight</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal PointLight(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_intensity = default!;
		_distance = default!;
		_decay = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PointLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PointLight"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.PointLight</c>: color, intensity, distance, decay.
	/// An argument the caller left unspecified travels as the wire's not-supplied sentinel, or is
	/// trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_color),
				_intensity,
				_distance,
				_decay
			]);
		}
	}

	/// <summary>
	/// When distance is zero, light will attenuate according to inverse-square law to infinite
	/// distance. When distance is non-zero, light will attenuate according to inverse-square law until
	/// near the distance cutoff, where it will then attenuate quickly and smoothly to 0. Inherently,
	/// cutoffs are not physically correct. Writing it records a <c>distance</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Distance
	{
		get { return _distance; }
		set
		{
			if (_distance == value)
			{
				return;
			}

			_distance = value;
			_isDistanceWritten = true;
			RecordSet("distance", value);
		}
	}

	/// <summary>
	/// The amount the light dims along the distance of the light. In context of physically-correct
	/// rendering the default value should not be changed. Writing it records a <c>decay</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Decay
	{
		get { return _decay; }
		set
		{
			if (_decay == value)
			{
				return;
			}

			_decay = value;
			_isDecayWritten = true;
			RecordSet("decay", value);
		}
	}

	/// <summary>
	/// The <c>power</c> property of the JavaScript-side object. Writing it records a <c>power</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Power
	{
		get { return _power; }
		set
		{
			if (_power == value)
			{
				return;
			}

			_power = value;
			_isPowerWritten = true;
			RecordSet("power", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isPointLight</c> held.
	/// </summary>
	/// <returns>The value <c>isPointLight</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsPointLightAsync()
	{
		return GetAsync<bool>("isPointLight");
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

		if (_isDistanceWritten)
		{
			batch.Set(Handle, "distance", ThreeValue.Encode(_distance));
		}

		if (_isDecayWritten)
		{
			batch.Set(Handle, "decay", ThreeValue.Encode(_decay));
		}

		if (_isPowerWritten)
		{
			batch.Set(Handle, "power", ThreeValue.Encode(_power));
		}
	}
}
