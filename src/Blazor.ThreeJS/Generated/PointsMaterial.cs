// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A material for rendering point primitives. Materials define the appearance of renderable 3D
/// objects. The JavaScript-side <c>THREE.PointsMaterial</c>.
/// </summary>
public sealed class PointsMaterial : Material
{
	private float _size = 1f;
	private bool _sizeAttenuation = true;
	private bool _fog = true;
	private bool _lights = false;
	private bool _isColorWritten;
	private bool _isSizeWritten;
	private bool _isSizeAttenuationWritten;
	private bool _isFogWritten;
	private bool _isLightsWritten;

	/// <summary>
	/// Color of the material. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>Constructs a new points material.</summary>
	public PointsMaterial()
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PointsMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PointsMaterial"; }
	}

	/// <summary>
	/// Defines the size of the points in pixels. Might be capped if the value exceeds hardware
	/// dependent parameters like
	/// [gl.ALIASED_POINT_SIZE_RANGE](https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/getParamete).
	/// Writing it records a <c>size</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float Size
	{
		get { return _size; }
		set
		{
			if (_size == value)
			{
				return;
			}

			_size = value;
			_isSizeWritten = true;
			RecordSet("size", value);
		}
	}

	/// <summary>
	/// Specifies whether size of individual points is attenuated by the camera depth (perspective
	/// camera only). Writing it records a <c>sizeAttenuation</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public bool SizeAttenuation
	{
		get { return _sizeAttenuation; }
		set
		{
			if (_sizeAttenuation == value)
			{
				return;
			}

			_sizeAttenuation = value;
			_isSizeAttenuationWritten = true;
			RecordSet("sizeAttenuation", value);
		}
	}

	/// <summary>
	/// Whether the material is affected by fog or not. Writing it records a <c>fog</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Fog
	{
		get { return _fog; }
		set
		{
			if (_fog == value)
			{
				return;
			}

			_fog = value;
			_isFogWritten = true;
			RecordSet("fog", value);
		}
	}

	/// <summary>
	/// Whether this material is affected by lights or not. Writing it records a <c>lights</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Lights
	{
		get { return _lights; }
		set
		{
			if (_lights == value)
			{
				return;
			}

			_lights = value;
			_isLightsWritten = true;
			RecordSet("lights", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.PointsMaterial</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isColorWritten)
		{
			batch.Set(Handle, "color", ThreeValue.Encode(Color));
		}

		if (_isSizeWritten)
		{
			batch.Set(Handle, "size", ThreeValue.Encode(_size));
		}

		if (_isSizeAttenuationWritten)
		{
			batch.Set(Handle, "sizeAttenuation", ThreeValue.Encode(_sizeAttenuation));
		}

		if (_isFogWritten)
		{
			batch.Set(Handle, "fog", ThreeValue.Encode(_fog));
		}

		if (_isLightsWritten)
		{
			batch.Set(Handle, "lights", ThreeValue.Encode(_lights));
		}
	}
}
