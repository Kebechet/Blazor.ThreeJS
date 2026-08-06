// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class emits light uniformly across the face a rectangular plane. This light type can be
/// used to simulate light sources such as bright windows or strip lighting. Important Notes: -
/// There is no shadow support. - Only PBR materials are supported. - You have to include
/// <c>RectAreaLightUniformsLib</c> (<c>WebGLRenderer</c>) or <c>RectAreaLightTexturesLib</c>
/// (<c>WebGPURenderer</c>) into your app and init the uniforms/textures. The JavaScript-side
/// <c>THREE.RectAreaLight</c>.
/// </summary>
public sealed class RectAreaLight : Object3D
{
	private readonly Color? _color;
	private float _intensity;
	private float _width;
	private float _height;
	private float _power;
	private bool _isWidthWritten;
	private bool _isHeightWritten;
	private bool _isPowerWritten;
	private bool _isIntensityWritten;

	/// <summary>Constructs a new area light.</summary>
	/// <param name="color">The light's color.</param>
	/// <param name="intensity">The light's strength/intensity.</param>
	/// <param name="width">The width of the light.</param>
	/// <param name="height">The height of the light.</param>
	public RectAreaLight(Color? color = null, float intensity = 1f, float width = 10f, float height = 10f)
	{
		_color = color;
		_intensity = intensity;
		_width = width;
		_height = height;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>RectAreaLight</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal RectAreaLight(ThreeBatch batch, int handle)
		: base(handle)
	{
		_intensity = default!;
		_width = default!;
		_height = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.RectAreaLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "RectAreaLight"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.RectAreaLight</c>: color, intensity, width, height.
	/// An argument the caller left unspecified travels as the wire's not-supplied sentinel, or is
	/// trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_color), _intensity, _width, _height]); }
	}

	/// <summary>
	/// The width of the light. Writing it records a <c>width</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float Width
	{
		get { return _width; }
		set
		{
			if (_width == value)
			{
				return;
			}

			_width = value;
			_isWidthWritten = true;
			RecordSet("width", value);
		}
	}

	/// <summary>
	/// The height of the light. Writing it records a <c>height</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float Height
	{
		get { return _height; }
		set
		{
			if (_height == value)
			{
				return;
			}

			_height = value;
			_isHeightWritten = true;
			RecordSet("height", value);
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
	/// the value <c>isRectAreaLight</c> held.
	/// </summary>
	/// <returns>The value <c>isRectAreaLight</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsRectAreaLightAsync()
	{
		return GetAsync<bool>("isRectAreaLight");
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

		if (_isWidthWritten)
		{
			batch.Set(Handle, "width", ThreeValue.Encode(_width));
		}

		if (_isHeightWritten)
		{
			batch.Set(Handle, "height", ThreeValue.Encode(_height));
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
