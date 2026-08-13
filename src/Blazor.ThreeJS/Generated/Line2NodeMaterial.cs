// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This node material can be used to render lines with a size larger than one by representing them
/// as instanced meshes. The JavaScript-side <c>THREE.Line2NodeMaterial</c>.
/// </summary>
public sealed class Line2NodeMaterial : NodeMaterial
{
	private float _dashOffset = 0f;
	private bool _worldUnits;
	private bool _dashed;
	private float _scale = 1f;
	private float _dashSize = 3f;
	private float _gapSize = 1f;
	private Texture? _map = null;
	private float _linewidth = 1f;
	private LineCap _linecap;
	private LineJoin _linejoin;
	private bool _isDashOffsetWritten;
	private bool _isWorldUnitsWritten;
	private bool _isDashedWritten;
	private bool _isScaleWritten;
	private bool _isDashSizeWritten;
	private bool _isGapSizeWritten;
	private bool _isColorWritten;
	private bool _isMapWritten;
	private bool _isLinewidthWritten;
	private bool _isLinecapWritten;
	private bool _isLinejoinWritten;

	/// <summary>
	/// Color of the material. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>Constructs a new node material for wide line rendering.</summary>
	public Line2NodeMaterial()
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Line2NodeMaterial</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Line2NodeMaterial(ThreeBatch batch, int handle)
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

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Line2NodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Line2NodeMaterial"; }
	}

	/// <summary>
	/// The dash offset. Writing it records a <c>dashOffset</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float DashOffset
	{
		get { return _dashOffset; }
		set
		{
			if (_dashOffset == value)
			{
				return;
			}

			_dashOffset = value;
			_isDashOffsetWritten = true;
			RecordSet("dashOffset", value);
		}
	}

	/// <summary>
	/// The <c>worldUnits</c> property of the JavaScript-side object. Writing it records a
	/// <c>worldUnits</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool WorldUnits
	{
		get { return _worldUnits; }
		set
		{
			if (_worldUnits == value)
			{
				return;
			}

			_worldUnits = value;
			_isWorldUnitsWritten = true;
			RecordSet("worldUnits", value);
		}
	}

	/// <summary>
	/// The <c>dashed</c> property of the JavaScript-side object. Writing it records a <c>dashed</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Dashed
	{
		get { return _dashed; }
		set
		{
			if (_dashed == value)
			{
				return;
			}

			_dashed = value;
			_isDashedWritten = true;
			RecordSet("dashed", value);
		}
	}

	/// <summary>
	/// The scale of the dashed part of a line. Writing it records a <c>scale</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Scale
	{
		get { return _scale; }
		set
		{
			if (_scale == value)
			{
				return;
			}

			_scale = value;
			_isScaleWritten = true;
			RecordSet("scale", value);
		}
	}

	/// <summary>
	/// The size of the dash. This is both the gap with the stroke. Writing it records a <c>dashSize</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float DashSize
	{
		get { return _dashSize; }
		set
		{
			if (_dashSize == value)
			{
				return;
			}

			_dashSize = value;
			_isDashSizeWritten = true;
			RecordSet("dashSize", value);
		}
	}

	/// <summary>
	/// The size of the gap. Writing it records a <c>gapSize</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float GapSize
	{
		get { return _gapSize; }
		set
		{
			if (_gapSize == value)
			{
				return;
			}

			_gapSize = value;
			_isGapSizeWritten = true;
			RecordSet("gapSize", value);
		}
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
	/// Defines appearance of line ends. Can only be used with <c>SVGRenderer</c>. Writing it records a
	/// <c>linecap</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public LineCap Linecap
	{
		get { return _linecap; }
		set
		{
			if (_linecap == value)
			{
				return;
			}

			_linecap = value;
			_isLinecapWritten = true;
			RecordSet("linecap", value);
		}
	}

	/// <summary>
	/// Defines appearance of line joints. Can only be used with <c>SVGRenderer</c>. Writing it records
	/// a <c>linejoin</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public LineJoin Linejoin
	{
		get { return _linejoin; }
		set
		{
			if (_linejoin == value)
			{
				return;
			}

			_linejoin = value;
			_isLinejoinWritten = true;
			RecordSet("linejoin", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isLine2NodeMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isLine2NodeMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsLine2NodeMaterialAsync()
	{
		return GetAsync<bool>("isLine2NodeMaterial");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Line2NodeMaterial</c>, then replays every property written
	/// before this object was attached. A replayed value that is itself a mirrored object is attached
	/// first, so its create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isDashOffsetWritten)
		{
			batch.Set(Handle, "dashOffset", ThreeValue.Encode(_dashOffset));
		}

		if (_isWorldUnitsWritten)
		{
			batch.Set(Handle, "worldUnits", ThreeValue.Encode(_worldUnits));
		}

		if (_isDashedWritten)
		{
			batch.Set(Handle, "dashed", ThreeValue.Encode(_dashed));
		}

		if (_isScaleWritten)
		{
			batch.Set(Handle, "scale", ThreeValue.Encode(_scale));
		}

		if (_isDashSizeWritten)
		{
			batch.Set(Handle, "dashSize", ThreeValue.Encode(_dashSize));
		}

		if (_isGapSizeWritten)
		{
			batch.Set(Handle, "gapSize", ThreeValue.Encode(_gapSize));
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

		if (_isLinewidthWritten)
		{
			batch.Set(Handle, "linewidth", ThreeValue.Encode(_linewidth));
		}

		if (_isLinecapWritten)
		{
			batch.Set(Handle, "linecap", ThreeValue.Encode(_linecap));
		}

		if (_isLinejoinWritten)
		{
			batch.Set(Handle, "linejoin", ThreeValue.Encode(_linejoin));
		}
	}
}
