// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// An instanced version of <c>BufferAttribute</c>. The JavaScript-side
/// <c>THREE.InstancedBufferAttribute</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/InstancedBufferAttribute">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/InstancedBufferAttribute.js">Source</seealso>
public class InstancedBufferAttribute : BufferAttribute
{
	private readonly TypedArray _array;
	private readonly float _itemSize;
	private readonly bool? _normalized;
	private float? _meshPerAttribute;
	private bool _isMeshPerAttributeWritten;

	/// <summary>Create a new instance of <c>InstancedBufferAttribute</c>.</summary>
	/// <param name="array">Value forwarded to the <c>array</c> constructor argument.</param>
	/// <param name="itemSize">Value forwarded to the <c>itemSize</c> constructor argument.</param>
	/// <param name="normalized">Value forwarded to the <c>normalized</c> constructor argument.</param>
	/// <param name="meshPerAttribute">Value forwarded to the <c>meshPerAttribute</c> constructor argument.</param>
	public InstancedBufferAttribute(
		TypedArray array,
		float itemSize,
		bool? normalized = null,
		float? meshPerAttribute = null)
		: base(array: array, itemSize: itemSize, normalized: normalized ?? false)
	{
		_array = array;
		_itemSize = itemSize;
		_normalized = normalized;
		_meshPerAttribute = meshPerAttribute;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>InstancedBufferAttribute</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal InstancedBufferAttribute(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_array = default!;
		_itemSize = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.InstancedBufferAttribute</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "InstancedBufferAttribute"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.InstancedBufferAttribute</c>: array, itemSize,
	/// normalized, meshPerAttribute. An argument the caller left unspecified travels as the wire's
	/// not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js applies its
	/// own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_array,
				_itemSize,
				ThreeValue.OrUnspecified(_normalized),
				ThreeValue.OrUnspecified(_meshPerAttribute)
			]);
		}
	}

	/// <summary>
	/// Defines how often a value of this buffer attribute should be repeated. A value of one means that
	/// each value of the instanced attribute is used for a single instance. A value of two means that
	/// each value is used for two consecutive instances (and so on). Writing it records a
	/// <c>meshPerAttribute</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float? MeshPerAttribute
	{
		get { return _meshPerAttribute; }
		set
		{
			if (_meshPerAttribute == value)
			{
				return;
			}

			_meshPerAttribute = value;
			_isMeshPerAttributeWritten = true;
			RecordSet("meshPerAttribute", value);
		}
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="InstancedBufferAttribute"/>.
	/// Read-only in three.js, so it is read on demand rather than mirrored: records a get op, sends it
	/// behind every write already pending, and completes with the value
	/// <c>isInstancedBufferAttribute</c> held.
	/// </summary>
	/// <returns>The value <c>isInstancedBufferAttribute</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsInstancedBufferAttributeAsync()
	{
		return GetAsync<bool>("isInstancedBufferAttribute");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.InstancedBufferAttribute</c>, then replays every property
	/// written before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isMeshPerAttributeWritten)
		{
			batch.Set(Handle, "meshPerAttribute", ThreeValue.Encode(_meshPerAttribute));
		}
	}
}
