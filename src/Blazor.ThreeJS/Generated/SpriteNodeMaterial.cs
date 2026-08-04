// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="SpriteMaterial"/>. The JavaScript-side
/// <c>THREE.SpriteNodeMaterial</c>.
/// </summary>
public class SpriteNodeMaterial : NodeMaterial
{
	private bool _sizeAttenuation;
	private float _rotation = 0f;
	private bool _isSizeAttenuationWritten;
	private bool _isColorWritten;
	private bool _isRotationWritten;

	/// <summary>
	/// Color of the material. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>Constructs a new sprite node material.</summary>
	public SpriteNodeMaterial()
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.SpriteNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "SpriteNodeMaterial"; }
	}

	/// <summary>
	/// The <c>sizeAttenuation</c> property of the JavaScript-side object. Writing it records a
	/// <c>sizeAttenuation</c> property write once this object is attached; writing the value already
	/// held records nothing.
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
	/// The rotation of the sprite in radians. Writing it records a <c>rotation</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Rotation
	{
		get { return _rotation; }
		set
		{
			if (_rotation == value)
			{
				return;
			}

			_rotation = value;
			_isRotationWritten = true;
			RecordSet("rotation", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.SpriteNodeMaterial</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isSizeAttenuationWritten)
		{
			batch.Set(Handle, "sizeAttenuation", ThreeValue.Encode(_sizeAttenuation));
		}

		if (_isColorWritten)
		{
			batch.Set(Handle, "color", ThreeValue.Encode(Color));
		}

		if (_isRotationWritten)
		{
			batch.Set(Handle, "rotation", ThreeValue.Encode(_rotation));
		}
	}
}
