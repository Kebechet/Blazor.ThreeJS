// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This light gets emitted from a single point in one direction, along a cone that increases in
/// size the further from the light it gets. This light can cast shadows - see the
/// <c>SpotLightShadow</c> for details. The JavaScript-side <c>THREE.SpotLight</c>.
/// </summary>
public class SpotLight : Object3D
{
	private readonly Color? _color;
	private float _intensity;
	private float _distance;
	private float? _angle;
	private float _penumbra;
	private float _decay;
	private Object3D? _target;
	private Texture? _map = null;
	private float _power;
	private bool _isTargetWritten;
	private bool _isDistanceWritten;
	private bool _isAngleWritten;
	private bool _isPenumbraWritten;
	private bool _isDecayWritten;
	private bool _isMapWritten;
	private bool _isPowerWritten;
	private bool _isIntensityWritten;

	/// <summary>Constructs a new spot light.</summary>
	/// <param name="color">The light's color.</param>
	/// <param name="intensity">The light's strength/intensity measured in candela (cd).</param>
	/// <param name="distance">Maximum range of the light. <c>0</c> means no limit.</param>
	/// <param name="angle">
	/// Maximum angle of light dispersion from its direction whose upper bound is <c>Math.PI/2</c>.
	/// </param>
	/// <param name="penumbra">
	/// Percent of the spotlight cone that is attenuated due to penumbra. Value range is <c>[0,1]</c>.
	/// </param>
	/// <param name="decay">The amount the light dims along the distance of the light.</param>
	public SpotLight(
		Color? color = null,
		float intensity = 1f,
		float distance = 0f,
		float? angle = null,
		float penumbra = 0f,
		float decay = 2f)
	{
		_color = color;
		_intensity = intensity;
		_distance = distance;
		_angle = angle;
		_penumbra = penumbra;
		_decay = decay;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>SpotLight</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal SpotLight(ThreeBatch batch, int handle)
		: base(handle)
	{
		_intensity = default!;
		_distance = default!;
		_penumbra = default!;
		_decay = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.SpotLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "SpotLight"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.SpotLight</c>: color, intensity, distance, angle,
	/// penumbra, decay. An argument the caller left unspecified travels as the wire's not-supplied
	/// sentinel, or is trimmed when nothing supplied follows it, so three.js applies its own default.
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
				ThreeValue.OrUnspecified(_angle),
				_penumbra,
				_decay
			]);
		}
	}

	/// <summary>
	/// The spot light points from its position to the target's position. For the target's position to
	/// be changed to anything other than the default, it must be added to the scene. It is also
	/// possible to set the target to be another 3D object in the scene. The light will now track the
	/// target object. Writing it records a <c>target</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public Object3D? Target
	{
		get { return _target; }
		set
		{
			if (ReferenceEquals(_target, value))
			{
				return;
			}

			_target = value;
			_isTargetWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("target", value);
		}
	}

	/// <summary>
	/// Maximum range of the light. <c>0</c> means no limit. Writing it records a <c>distance</c>
	/// property write once this object is attached; writing the value already held records nothing.
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
	/// Maximum angle of light dispersion from its direction whose upper bound is <c>Math.PI/2</c>.
	/// Writing it records a <c>angle</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float? Angle
	{
		get { return _angle; }
		set
		{
			if (_angle == value)
			{
				return;
			}

			_angle = value;
			_isAngleWritten = true;
			RecordSet("angle", value);
		}
	}

	/// <summary>
	/// Percent of the spotlight cone that is attenuated due to penumbra. Value range is <c>[0,1]</c>.
	/// Writing it records a <c>penumbra</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public float Penumbra
	{
		get { return _penumbra; }
		set
		{
			if (_penumbra == value)
			{
				return;
			}

			_penumbra = value;
			_isPenumbraWritten = true;
			RecordSet("penumbra", value);
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
	/// A texture used to modulate the color of the light. The spot light color is mixed with the RGB
	/// value of this texture, with a ratio corresponding to its alpha value. The cookie-like masking
	/// effect is reproduced using pixel values (0, 0, 0, 1-cookie_value). *Warning*: This property is
	/// disabled if <c>Object3D#castShadow</c> is set to <c>false</c>. Writing it records a <c>map</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? Map
	{
		get { return _map; }
		set
		{
			if (ReferenceEquals(_map, value))
			{
				return;
			}

			_map = value;
			_isMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("map", value);
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
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isSpotLight</c> held.
	/// </summary>
	/// <returns>The value <c>isSpotLight</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsSpotLightAsync()
	{
		return GetAsync<bool>("isSpotLight");
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isLight</c> held.
	/// </summary>
	/// <returns>The value <c>isLight</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsLightAsync()
	{
		return GetAsync<bool>("isLight");
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that. A replayed
	/// value that is itself a mirrored object is attached first, so its create op reaches the batch
	/// before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isTargetWritten)
		{
			_target?.AttachTo(batch);
			batch.Set(Handle, "target", ThreeValue.Encode(_target));
		}

		if (_isDistanceWritten)
		{
			batch.Set(Handle, "distance", ThreeValue.Encode(_distance));
		}

		if (_isAngleWritten)
		{
			batch.Set(Handle, "angle", ThreeValue.Encode(_angle));
		}

		if (_isPenumbraWritten)
		{
			batch.Set(Handle, "penumbra", ThreeValue.Encode(_penumbra));
		}

		if (_isDecayWritten)
		{
			batch.Set(Handle, "decay", ThreeValue.Encode(_decay));
		}

		if (_isMapWritten)
		{
			_map?.AttachTo(batch);
			batch.Set(Handle, "map", ThreeValue.Encode(_map));
		}

		if (_isPowerWritten)
		{
			batch.Set(Handle, "power", ThreeValue.Encode(_power));
		}

		if (_isIntensityWritten)
		{
			batch.Set(Handle, "intensity", ThreeValue.Encode(_intensity));
		}
	}
}
