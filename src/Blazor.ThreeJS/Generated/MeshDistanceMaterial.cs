// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A material used internally for implementing shadow mapping with point lights. Can also be used
/// to customize the shadow casting of an object by assigning an instance of
/// <c>MeshDistanceMaterial</c> to <c>Object3D#customDistanceMaterial</c>. The following examples
/// demonstrates this approach in order to ensure transparent parts of objects do not cast shadows.
/// The JavaScript-side <c>THREE.MeshDistanceMaterial</c>.
/// </summary>
public sealed class MeshDistanceMaterial : Material
{
	private float _displacementScale = 0f;
	private float _displacementBias = 0f;
	private bool _isDisplacementScaleWritten;
	private bool _isDisplacementBiasWritten;

	/// <summary>Constructs a new mesh distance material.</summary>
	public MeshDistanceMaterial()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshDistanceMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshDistanceMaterial"; }
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
	/// Emits the create op for <c>THREE.MeshDistanceMaterial</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isDisplacementScaleWritten)
		{
			batch.Set(Handle, "displacementScale", ThreeValue.Encode(_displacementScale));
		}

		if (_isDisplacementBiasWritten)
		{
			batch.Set(Handle, "displacementBias", ThreeValue.Encode(_displacementBias));
		}
	}
}
