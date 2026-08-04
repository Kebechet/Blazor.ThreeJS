// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// An instanced version of <c>BufferGeometry</c>. The JavaScript-side
/// <c>THREE.InstancedBufferGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/InstancedBufferGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/InstancedBufferGeometry.js">Source</seealso>
public sealed class InstancedBufferGeometry : BufferGeometry
{
	private int _instanceCount;
	private bool _isInstanceCountWritten;

	/// <summary>Create a new instance of <see cref="InstancedBufferGeometry"/>.</summary>
	public InstancedBufferGeometry()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.InstancedBufferGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "InstancedBufferGeometry"; }
	}

	/// <summary>
	/// The <c>instanceCount</c> property of the JavaScript-side object. Writing it records a
	/// <c>instanceCount</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public int InstanceCount
	{
		get { return _instanceCount; }
		set
		{
			if (_instanceCount == value)
			{
				return;
			}

			_instanceCount = value;
			_isInstanceCountWritten = true;
			RecordSet("instanceCount", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.InstancedBufferGeometry</c>, then replays every property
	/// written before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isInstanceCountWritten)
		{
			batch.Set(Handle, "instanceCount", ThreeValue.Encode(_instanceCount));
		}
	}
}
