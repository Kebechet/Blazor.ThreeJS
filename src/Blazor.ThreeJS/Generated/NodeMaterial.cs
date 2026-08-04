// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>Base class for all node materials. The JavaScript-side <c>THREE.NodeMaterial</c>.</summary>
public class NodeMaterial : Material
{
	private bool _fog = true;
	private bool _lights = false;
	private bool _isFogWritten;
	private bool _isLightsWritten;

	/// <summary>Initializes a new <see cref="NodeMaterial"/>.</summary>
	public NodeMaterial()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.NodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "NodeMaterial"; }
	}

	/// <summary>
	/// Whether this material is affected by fog or not. Writing it records a <c>fog</c> property write
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
	/// Most classic material types have a node pendant e.g. for <c>MeshBasicMaterial</c> there is
	/// <c>MeshBasicNodeMaterial</c>. This utility method is intended for defining all material
	/// properties of the classic type in the node type.
	/// </summary>
	/// <param name="material">The material to copy properties with their values to this node material.</param>
	public void SetDefaultValues(Material material)
	{
		if (Batch is not null)
		{
			material.AttachTo(Batch);
		}

		RecordCall("setDefaultValues", material);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.NodeMaterial</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

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
