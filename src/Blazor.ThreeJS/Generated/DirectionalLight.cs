// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A light that gets emitted in a specific direction. This light will behave as though it is
/// infinitely far away and the rays produced from it are all parallel. The common use case for this
/// is to simulate daylight; the sun is far enough away that its position can be considered to be
/// infinite, and all light rays coming from it are parallel. A common point of confusion for
/// directional lights is that setting the rotation has no effect. This is because three.js's
/// DirectionalLight is the equivalent to what is often called a 'Target Direct Light' in other
/// applications. This means that its direction is calculated as pointing from the light's
/// <c>Object3D#position</c> to the <c>DirectionalLight#target</c> position (as opposed to a 'Free
/// Direct Light' that just has a rotation component). This light can cast shadows - see the
/// <c>DirectionalLightShadow</c> for details. The JavaScript-side <c>THREE.DirectionalLight</c>.
/// </summary>
public sealed class DirectionalLight : Object3D
{
	private readonly Color? _color;
	private float _intensity;
	private Object3D? _target;
	private bool _isTargetWritten;
	private bool _isIntensityWritten;

	/// <summary>Constructs a new directional light.</summary>
	/// <param name="color">The light's color.</param>
	/// <param name="intensity">The light's strength/intensity.</param>
	public DirectionalLight(Color? color = null, float intensity = 1f)
	{
		_color = color;
		_intensity = intensity;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>DirectionalLight</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal DirectionalLight(ThreeBatch batch, int handle)
		: base(handle)
	{
		_intensity = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.DirectionalLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "DirectionalLight"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.DirectionalLight</c>: color, intensity. An argument
	/// the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when
	/// nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_color), _intensity]); }
	}

	/// <summary>
	/// The directional light points from its position to the target's position. For the target's
	/// position to be changed to anything other than the default, it must be added to the scene. It is
	/// also possible to set the target to be another 3D object in the scene. The light will now track
	/// the target object. Writing it records a <c>target</c> property write once this object is
	/// attached; writing the value already held records nothing.
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
	/// the value <c>isDirectionalLight</c> held.
	/// </summary>
	/// <returns>The value <c>isDirectionalLight</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsDirectionalLightAsync()
	{
		return GetAsync<bool>("isDirectionalLight");
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

		if (_isIntensityWritten)
		{
			batch.Set(Handle, "intensity", ThreeValue.Encode(_intensity));
		}
	}
}
