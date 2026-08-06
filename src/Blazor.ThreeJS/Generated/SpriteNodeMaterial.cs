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
	private Texture? _map = null;
	private Texture? _alphaMap = null;
	private float _rotation = 0f;
	private bool _isSizeAttenuationWritten;
	private bool _isColorWritten;
	private bool _isMapWritten;
	private bool _isAlphaMapWritten;
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

	/// <summary>
	/// Adopts an existing JavaScript-side <c>SpriteNodeMaterial</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal SpriteNodeMaterial(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};

		Batch = batch;
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
	/// The color map. May optionally include an alpha channel, typically combined with
	/// <c>Material#transparent</c> or <c>Material#alphaTest</c>. The texture map color is modulated by
	/// the diffuse <c>color</c>. <c>map</c> represents color data, and the texture must be assigned a
	/// <c>Texture#colorSpace</c>. Most <c>map</c> textures set <c>texture.colorSpace =
	/// SRGBColorSpace</c>. Writing it records a <c>map</c> property write once this object is attached;
	/// writing the value already held records nothing.
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
	/// The alpha map is a grayscale texture that controls the opacity across the surface (black: fully
	/// transparent; white: fully opaque). Only the color of the texture is used, ignoring the alpha
	/// channel if one exists. For RGB and RGBA textures, the renderer will use the green channel when
	/// sampling this texture due to the extra bit of precision provided for green in DXT-compressed and
	/// uncompressed RGB 565 formats. Luminance-only and luminance/alpha textures will also still work
	/// as expected. <c>alphaMap</c> represents non-color data. Any texture assigned must have
	/// <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a <c>alphaMap</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? AlphaMap
	{
		get { return _alphaMap; }
		set
		{
			if (ReferenceEquals(_alphaMap, value))
			{
				return;
			}

			_alphaMap = value;
			_isAlphaMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("alphaMap", value);
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
	/// Reads <c>isSpriteMaterial</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isSpriteMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isSpriteMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsSpriteMaterialAsync()
	{
		return GetAsync<bool>("isSpriteMaterial");
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isSpriteNodeMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isSpriteNodeMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsSpriteNodeMaterialAsync()
	{
		return GetAsync<bool>("isSpriteNodeMaterial");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.SpriteNodeMaterial</c>, then replays every property written
	/// before this object was attached. A replayed value that is itself a mirrored object is attached
	/// first, so its create op reaches the batch before the write that references it by handle.
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

		if (_isMapWritten)
		{
			_map?.AttachTo(batch);
			batch.Set(Handle, "map", ThreeValue.Encode(_map));
		}

		if (_isAlphaMapWritten)
		{
			_alphaMap?.AttachTo(batch);
			batch.Set(Handle, "alphaMap", ThreeValue.Encode(_alphaMap));
		}

		if (_isRotationWritten)
		{
			batch.Set(Handle, "rotation", ThreeValue.Encode(_rotation));
		}
	}
}
