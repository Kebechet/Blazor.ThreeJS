// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="MeshNormalMaterial"/>. The JavaScript-side
/// <c>THREE.MeshNormalNodeMaterial</c>.
/// </summary>
public sealed class MeshNormalNodeMaterial : NodeMaterial
{
	private Texture? _bumpMap = null;
	private float _bumpScale = 1f;
	private Texture? _normalMap = null;
	private NormalMapTypes _normalMapType = NormalMapTypes.TangentSpaceNormalMap;
	private Texture? _displacementMap = null;
	private float _displacementScale = 0f;
	private float _displacementBias = 0f;
	private bool _wireframe = false;
	private float _wireframeLinewidth = 1f;
	private bool _flatShading = false;
	private bool _isBumpMapWritten;
	private bool _isBumpScaleWritten;
	private bool _isNormalMapWritten;
	private bool _isNormalMapTypeWritten;
	private bool _isNormalScaleWritten;
	private bool _isDisplacementMapWritten;
	private bool _isDisplacementScaleWritten;
	private bool _isDisplacementBiasWritten;
	private bool _isWireframeWritten;
	private bool _isWireframeLinewidthWritten;
	private bool _isFlatShadingWritten;

	/// <summary>
	/// How much the normal map affects the material. Typical value range is <c>[0,1]</c>. Mirrored as
	/// an instance this object owns: mutating it records a write of <c>normalScale</c>.
	/// </summary>
	public Vector2 NormalScale { get; }

	/// <summary>Constructs a new mesh normal node material.</summary>
	public MeshNormalNodeMaterial()
	{
		NormalScale = new Vector2();
		NormalScale.OnChange = () =>
		{
			_isNormalScaleWritten = true;
			RecordSet("normalScale", NormalScale);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>MeshNormalNodeMaterial</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal MeshNormalNodeMaterial(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		NormalScale = new Vector2();
		NormalScale.OnChange = () =>
		{
			_isNormalScaleWritten = true;
			RecordSet("normalScale", NormalScale);
		};

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshNormalNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshNormalNodeMaterial"; }
	}

	/// <summary>
	/// The texture to create a bump map. The black and white values map to the perceived depth in
	/// relation to the lights. Bump doesn't actually affect the geometry of the object, only the
	/// lighting. If a normal map is defined this will be ignored. <c>bumpMap</c> represents non-color
	/// data. Any texture assigned must have <c>texture.colorSpace = NoColorSpace</c> (default). Writing
	/// it records a <c>bumpMap</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public Texture? BumpMap
	{
		get { return _bumpMap; }
		set
		{
			if (ReferenceEquals(_bumpMap, value))
			{
				return;
			}

			_bumpMap = value;
			_isBumpMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("bumpMap", value);
		}
	}

	/// <summary>
	/// How much the bump map affects the material. Typical range is <c>[0,1]</c>. Writing it records a
	/// <c>bumpScale</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float BumpScale
	{
		get { return _bumpScale; }
		set
		{
			if (_bumpScale == value)
			{
				return;
			}

			_bumpScale = value;
			_isBumpScaleWritten = true;
			RecordSet("bumpScale", value);
		}
	}

	/// <summary>
	/// The texture to create a normal map. The RGB values affect the surface normal for each pixel
	/// fragment and change the way the color is lit. Normal maps do not change the actual shape of the
	/// surface, only the lighting. In case the material has a normal map authored using the left handed
	/// convention, the <c>y</c> component of <c>normalScale</c> should be negated to compensate for the
	/// different handedness. <c>normalMap</c> represents non-color data. Any texture assigned must have
	/// <c>texture.colorSpace = NoColorSpace</c> (default). Writing it records a <c>normalMap</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? NormalMap
	{
		get { return _normalMap; }
		set
		{
			if (ReferenceEquals(_normalMap, value))
			{
				return;
			}

			_normalMap = value;
			_isNormalMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("normalMap", value);
		}
	}

	/// <summary>
	/// The type of normal map. Writing it records a <c>normalMapType</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public NormalMapTypes NormalMapType
	{
		get { return _normalMapType; }
		set
		{
			if (_normalMapType == value)
			{
				return;
			}

			_normalMapType = value;
			_isNormalMapTypeWritten = true;
			RecordSet("normalMapType", value);
		}
	}

	/// <summary>
	/// The displacement map affects the position of the mesh's vertices. Unlike other maps which only
	/// affect the light and shade of the material the displaced vertices can cast shadows, block other
	/// objects, and otherwise act as real geometry. The displacement texture is an image where the
	/// value of each pixel (white being the highest) is mapped against, and repositions, the vertices
	/// of the mesh. For best results, pair a displacement map with a matching normal map, since the
	/// renderer can not recompute surface normals from the displaced vertices. Writing it records a
	/// <c>displacementMap</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public Texture? DisplacementMap
	{
		get { return _displacementMap; }
		set
		{
			if (ReferenceEquals(_displacementMap, value))
			{
				return;
			}

			_displacementMap = value;
			_isDisplacementMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("displacementMap", value);
		}
	}

	/// <summary>
	/// How much the displacement map affects the mesh (where black is no displacement, and white is
	/// maximum displacement). Without a displacement map set, this value is not applied. Writing it
	/// records a <c>displacementScale</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public float DisplacementScale
	{
		get { return _displacementScale; }
		set
		{
			if (_displacementScale == value)
			{
				return;
			}

			_displacementScale = value;
			_isDisplacementScaleWritten = true;
			RecordSet("displacementScale", value);
		}
	}

	/// <summary>
	/// The offset of the displacement map's values on the mesh's vertices. The bias is added to the
	/// scaled sample of the displacement map. Without a displacement map set, this value is not
	/// applied. Writing it records a <c>displacementBias</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float DisplacementBias
	{
		get { return _displacementBias; }
		set
		{
			if (_displacementBias == value)
			{
				return;
			}

			_displacementBias = value;
			_isDisplacementBiasWritten = true;
			RecordSet("displacementBias", value);
		}
	}

	/// <summary>
	/// Renders the geometry as a wireframe. Writing it records a <c>wireframe</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Wireframe
	{
		get { return _wireframe; }
		set
		{
			if (_wireframe == value)
			{
				return;
			}

			_wireframe = value;
			_isWireframeWritten = true;
			RecordSet("wireframe", value);
		}
	}

	/// <summary>
	/// Controls the thickness of the wireframe. WebGL and WebGPU ignore this property and always render
	/// 1 pixel wide lines. Writing it records a <c>wireframeLinewidth</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float WireframeLinewidth
	{
		get { return _wireframeLinewidth; }
		set
		{
			if (_wireframeLinewidth == value)
			{
				return;
			}

			_wireframeLinewidth = value;
			_isWireframeLinewidthWritten = true;
			RecordSet("wireframeLinewidth", value);
		}
	}

	/// <summary>
	/// Whether the material is rendered with flat shading or not. Writing it records a
	/// <c>flatShading</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool FlatShading
	{
		get { return _flatShading; }
		set
		{
			if (_flatShading == value)
			{
				return;
			}

			_flatShading = value;
			_isFlatShadingWritten = true;
			RecordSet("flatShading", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isMeshNormalNodeMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isMeshNormalNodeMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsMeshNormalNodeMaterialAsync()
	{
		return GetAsync<bool>("isMeshNormalNodeMaterial");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.MeshNormalNodeMaterial</c>, then replays every property written
	/// before this object was attached. A replayed value that is itself a mirrored object is attached
	/// first, so its create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isBumpMapWritten)
		{
			_bumpMap?.AttachTo(batch);
			batch.Set(Handle, "bumpMap", ThreeValue.Encode(_bumpMap));
		}

		if (_isBumpScaleWritten)
		{
			batch.Set(Handle, "bumpScale", ThreeValue.Encode(_bumpScale));
		}

		if (_isNormalMapWritten)
		{
			_normalMap?.AttachTo(batch);
			batch.Set(Handle, "normalMap", ThreeValue.Encode(_normalMap));
		}

		if (_isNormalMapTypeWritten)
		{
			batch.Set(Handle, "normalMapType", ThreeValue.Encode(_normalMapType));
		}

		if (_isNormalScaleWritten)
		{
			batch.Set(Handle, "normalScale", ThreeValue.Encode(NormalScale));
		}

		if (_isDisplacementMapWritten)
		{
			_displacementMap?.AttachTo(batch);
			batch.Set(Handle, "displacementMap", ThreeValue.Encode(_displacementMap));
		}

		if (_isDisplacementScaleWritten)
		{
			batch.Set(Handle, "displacementScale", ThreeValue.Encode(_displacementScale));
		}

		if (_isDisplacementBiasWritten)
		{
			batch.Set(Handle, "displacementBias", ThreeValue.Encode(_displacementBias));
		}

		if (_isWireframeWritten)
		{
			batch.Set(Handle, "wireframe", ThreeValue.Encode(_wireframe));
		}

		if (_isWireframeLinewidthWritten)
		{
			batch.Set(Handle, "wireframeLinewidth", ThreeValue.Encode(_wireframeLinewidth));
		}

		if (_isFlatShadingWritten)
		{
			batch.Set(Handle, "flatShading", ThreeValue.Encode(_flatShading));
		}
	}
}
