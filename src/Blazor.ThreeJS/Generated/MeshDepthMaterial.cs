// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A material for drawing geometry by depth. Depth is based off of the camera near and far plane.
/// White is nearest, black is farthest. The JavaScript-side <c>THREE.MeshDepthMaterial</c>.
/// </summary>
public sealed class MeshDepthMaterial : Material
{
	private DepthPackingStrategies _depthPacking = DepthPackingStrategies.BasicDepthPacking;
	private float _displacementScale = 0f;
	private float _displacementBias = 0f;
	private bool _wireframe = false;
	private float _wireframeLinewidth = 1f;
	private bool _isDepthPackingWritten;
	private bool _isDisplacementScaleWritten;
	private bool _isDisplacementBiasWritten;
	private bool _isWireframeWritten;
	private bool _isWireframeLinewidthWritten;

	/// <summary>Constructs a new mesh depth material.</summary>
	public MeshDepthMaterial()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshDepthMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshDepthMaterial"; }
	}

	/// <summary>
	/// Type for depth packing. Writing it records a <c>depthPacking</c> property write once this object
	/// is attached; writing the value already held records nothing.
	/// </summary>
	public DepthPackingStrategies DepthPacking
	{
		get { return _depthPacking; }
		set
		{
			if (_depthPacking == value)
			{
				return;
			}

			_depthPacking = value;
			_isDepthPackingWritten = true;
			RecordSet("depthPacking", value);
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
	/// Emits the create op for <c>THREE.MeshDepthMaterial</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isDepthPackingWritten)
		{
			batch.Set(Handle, "depthPacking", ThreeValue.Encode(_depthPacking));
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
	}
}
