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
/// <see cref="DirectionalLightShadow"/> for details. The JavaScript-side
/// <c>THREE.DirectionalLight</c>.
/// </summary>
public sealed class DirectionalLight : Object3D
{
	private readonly Color? _color;
	private float _intensity;
	private Object3D? _target;
	private DirectionalLightShadow? _shadow;
	private bool _isTargetWritten;
	private bool _isShadowWritten;
	private bool _isIntensityWritten;

	/// <summary>Constructs a new directional light.</summary>
	/// <param name="color">The light's color.</param>
	/// <param name="intensity">The light's strength/intensity.</param>
	public DirectionalLight(Color? color = null, float intensity = 1f)
	{
		_color = color;
		_intensity = intensity;
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
	/// This property holds the light's shadow configuration. Writing it records a <c>shadow</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public DirectionalLightShadow? Shadow
	{
		get { return _shadow; }
		set
		{
			if (ReferenceEquals(_shadow, value))
			{
				return;
			}

			_shadow = value;
			_isShadowWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("shadow", value);
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
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isTargetWritten)
		{
			batch.Set(Handle, "target", ThreeValue.Encode(_target));
		}

		if (_isShadowWritten)
		{
			batch.Set(Handle, "shadow", ThreeValue.Encode(_shadow));
		}

		if (_isIntensityWritten)
		{
			batch.Set(Handle, "intensity", ThreeValue.Encode(_intensity));
		}
	}
}
