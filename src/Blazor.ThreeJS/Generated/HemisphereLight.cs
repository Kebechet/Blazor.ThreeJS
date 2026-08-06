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
public sealed class HemisphereLight : Object3D
{
	private readonly Color? _skyColor;
	private readonly Color? _groundColor;
	private float _intensity;
	private bool _isColorWritten;
	private bool _isIntensityWritten;

	/// <summary>
	/// The light's color. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>Constructs a new hemisphere light.</summary>
	/// <param name="skyColor">The light's sky color.</param>
	/// <param name="groundColor">The light's ground color.</param>
	/// <param name="intensity">The light's strength/intensity.</param>
	public HemisphereLight(Color? skyColor = null, Color? groundColor = null, float intensity = 1f)
	{
		_skyColor = skyColor;
		_groundColor = groundColor;
		_intensity = intensity;

		Color = new Color();
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>HemisphereLight</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal HemisphereLight(ThreeBatch batch, int handle)
		: base(handle)
	{
		_intensity = default!;

		Color = new Color();
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};

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
	/// the value <c>isHemisphereLight</c> held.
	/// </summary>
	/// <returns>The value <c>isHemisphereLight</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsHemisphereLightAsync()
	{
		return GetAsync<bool>("isHemisphereLight");
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
