// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A material for rendering line primitives. Materials define the appearance of renderable 3D
/// objects. The JavaScript-side <c>THREE.LineBasicMaterial</c>.
/// </summary>
public class LineBasicMaterial : Material
{
	private Texture? _map = null;
	private float _linewidth = 1f;
	private bool _fog = true;
	private bool _lights = false;
	private bool _isColorWritten;
	private bool _isMapWritten;
	private bool _isLinewidthWritten;
	private bool _isFogWritten;
	private bool _isLightsWritten;

	/// <summary>
	/// Color of the material. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>Constructs a new line basic material.</summary>
	public LineBasicMaterial()
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>LineBasicMaterial</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal LineBasicMaterial(ThreeBatch batch, int handle)
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

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LineBasicMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LineBasicMaterial"; }
	}

	/// <summary>
	/// Sets the color of the lines using data from a texture. The texture map color is modulated by the
	/// diffuse <c>color</c>. <c>map</c> represents color data, and the texture must be assigned a
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
	/// Controls line thickness or lines. Can only be used with <c>SVGRenderer</c>. WebGL and WebGPU
	/// ignore this setting and always render line primitives with a width of one pixel. Writing it
	/// records a <c>linewidth</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float Linewidth
	{
		get { return _linewidth; }
		set
		{
			if (_linewidth == value)
			{
				return;
			}

			_linewidth = value;
			_isLinewidthWritten = true;
			RecordSet("linewidth", value);
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
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isLineBasicMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isLineBasicMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsLineBasicMaterialAsync()
	{
		return GetAsync<bool>("isLineBasicMaterial");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.LineBasicMaterial</c>, then replays every property written
	/// before this object was attached. A replayed value that is itself a mirrored object is attached
	/// first, so its create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isColorWritten)
		{
			batch.Set(Handle, "color", ThreeValue.Encode(Color));
		}

		if (_isMapWritten)
		{
			_map?.AttachTo(batch);
			batch.Set(Handle, "map", ThreeValue.Encode(_map));
		}

		if (_isLinewidthWritten)
		{
			batch.Set(Handle, "linewidth", ThreeValue.Encode(_linewidth));
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
