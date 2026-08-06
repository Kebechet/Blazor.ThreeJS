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

	/// <summary>
	/// Adopts an existing JavaScript-side <c>NodeMaterial</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal NodeMaterial(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
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
		RecordCall("setDefaultValues", material);
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isNodeMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isNodeMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsNodeMaterialAsync()
	{
		return GetAsync<bool>("isNodeMaterial");
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
