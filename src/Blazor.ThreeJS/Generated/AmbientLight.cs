// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This light globally illuminates all objects in the scene equally. It cannot be used to cast
/// shadows as it does not have a direction. The JavaScript-side <c>THREE.AmbientLight</c>.
/// </summary>
public sealed class AmbientLight : Object3D
{
	private readonly Color? _color;
	private float _intensity;
	private bool _isIntensityWritten;

	/// <summary>Constructs a new ambient light.</summary>
	/// <param name="color">The light's color.</param>
	/// <param name="intensity">The light's strength/intensity.</param>
	public AmbientLight(Color? color = null, float intensity = 1f)
	{
		_color = color;
		_intensity = intensity;
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

		if (_isIntensityWritten)
		{
			batch.Set(Handle, "intensity", ThreeValue.Encode(_intensity));
		}
	}
}
