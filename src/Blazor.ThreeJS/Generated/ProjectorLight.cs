// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A projector light version of <see cref="SpotLight"/>. Can only be used with
/// <see cref="WebGPURenderer"/>. The JavaScript-side <c>THREE.ProjectorLight</c>.
/// </summary>
public sealed class ProjectorLight : SpotLight
{
	private readonly Color? _color;
	private readonly float _intensity;
	private readonly float _distance;
	private readonly float? _angle;
	private readonly float _penumbra;
	private readonly float _decay;
	private float? _aspect = null;
	private bool _isAspectWritten;

	/// <summary>Initializes a new <see cref="ProjectorLight"/>.</summary>
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
	public ProjectorLight(
		Color? color = null,
		float intensity = 1f,
		float distance = 0f,
		float? angle = null,
		float penumbra = 0f,
		float decay = 2f)
		: base(color: color, intensity: intensity, distance: distance, angle: angle, penumbra: penumbra, decay: decay)
	{
		_color = color;
		_intensity = intensity;
		_distance = distance;
		_angle = angle;
		_penumbra = penumbra;
		_decay = decay;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ProjectorLight</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ProjectorLight(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_intensity = default!;
		_distance = default!;
		_penumbra = default!;
		_decay = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ProjectorLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ProjectorLight"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ProjectorLight</c>: color, intensity, distance,
	/// angle, penumbra, decay. An argument the caller left unspecified travels as the wire's
	/// not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js applies its
	/// own default.
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
	/// Aspect ratio of the light. Set to <c>null</c> to use the texture aspect ratio. Writing it
	/// records a <c>aspect</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float? Aspect
	{
		get { return _aspect; }
		set
		{
			if (_aspect == value)
			{
				return;
			}

			_aspect = value;
			_isAspectWritten = true;
			RecordSet("aspect", value);
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

		if (_isAspectWritten)
		{
			batch.Set(Handle, "aspect", ThreeValue.Encode(_aspect));
		}
	}
}
