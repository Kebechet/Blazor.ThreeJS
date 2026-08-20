// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// An instanced version of <c>InterleavedBuffer</c>. The JavaScript-side
/// <c>THREE.InstancedInterleavedBuffer</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/InstancedInterleavedBuffer">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/InstancedInterleavedBuffer.js">Source</seealso>
public sealed class InstancedInterleavedBuffer : InterleavedBuffer
{
	private readonly TypedArray _array;
	private readonly int _stride;
	private float? _meshPerAttribute;
	private bool _isMeshPerAttributeWritten;

	/// <summary>Create a new instance of <see cref="InstancedInterleavedBuffer"/>.</summary>
	/// <param name="array">Value forwarded to the <c>array</c> constructor argument.</param>
	/// <param name="stride">Value forwarded to the <c>stride</c> constructor argument.</param>
	/// <param name="meshPerAttribute">Value forwarded to the <c>meshPerAttribute</c> constructor argument.</param>
	public InstancedInterleavedBuffer(TypedArray array, int stride, float? meshPerAttribute = null)
		: base(array: array, stride: stride)
	{
		_array = array;
		_stride = stride;
		_meshPerAttribute = meshPerAttribute;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>InstancedInterleavedBuffer</c> under the handle the
	/// browser minted for it. No create op is emitted: the object already exists, and this mirror's job
	/// is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal InstancedInterleavedBuffer(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_array = default!;
		_stride = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.InstancedInterleavedBuffer</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "InstancedInterleavedBuffer"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.InstancedInterleavedBuffer</c>: array, stride,
	/// meshPerAttribute. An argument the caller left unspecified travels as the wire's not-supplied
	/// sentinel, or is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([_array, _stride, ThreeValue.OrUnspecified(_meshPerAttribute)]); }
	}

	/// <summary>
	/// The <c>meshPerAttribute</c> property of the JavaScript-side object. Writing it records a
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
	/// Emits the create op for <c>THREE.InstancedInterleavedBuffer</c>, then replays every property
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
