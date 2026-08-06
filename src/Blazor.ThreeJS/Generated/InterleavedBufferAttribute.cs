// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.InterleavedBufferAttribute</c>.</summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/InterleavedBufferAttribute">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/InterleavedBufferAttribute.js">Source</seealso>
public sealed class InterleavedBufferAttribute : ThreeObject
{
	private readonly InterleavedBuffer _interleavedBuffer;
	private float _itemSize;
	private float _offset;
	private bool _normalized;
	private string _name = string.Empty;
	private InterleavedBuffer? _data;
	private bool _needsUpdate;
	private bool _isNameWritten;
	private bool _isDataWritten;
	private bool _isItemSizeWritten;
	private bool _isOffsetWritten;
	private bool _isNormalizedWritten;
	private bool _isNeedsUpdateWritten;

	/// <summary>Create a new instance of <c>InterleavedBufferAttribute</c>.</summary>
	/// <param name="interleavedBuffer">
	/// Value forwarded to the <c>interleavedBuffer</c> constructor argument.
	/// </param>
	/// <param name="itemSize">Value forwarded to the <c>itemSize</c> constructor argument.</param>
	/// <param name="offset">Value forwarded to the <c>offset</c> constructor argument.</param>
	/// <param name="normalized"></param>
	public InterleavedBufferAttribute(
		InterleavedBuffer interleavedBuffer,
		float itemSize,
		float offset,
		bool normalized = false)
	{
		_interleavedBuffer = interleavedBuffer;
		_itemSize = itemSize;
		_offset = offset;
		_normalized = normalized;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>InterleavedBufferAttribute</c> under the handle the
	/// browser minted for it. No create op is emitted: the object already exists, and this mirror's job
	/// is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal InterleavedBufferAttribute(ThreeBatch batch, int handle)
		: base(handle)
	{
		_interleavedBuffer = default!;
		_itemSize = default!;
		_offset = default!;
		_normalized = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.InterleavedBufferAttribute</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "InterleavedBufferAttribute"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.InterleavedBufferAttribute</c>: interleavedBuffer,
	/// itemSize, offset, normalized.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_interleavedBuffer, _itemSize, _offset, _normalized]; }
	}

	/// <summary>
	/// Optional name for this attribute instance. Writing it records a <c>name</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public string Name
	{
		get { return _name; }
		set
		{
			if (_name == value)
			{
				return;
			}

			_name = value;
			_isNameWritten = true;
			RecordSet("name", value);
		}
	}

	/// <summary>
	/// The <see cref="InterleavedBuffer">InterleavedBuffer</see> instance passed in the constructor.
	/// Writing it records a <c>data</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public InterleavedBuffer? Data
	{
		get { return _data; }
		set
		{
			if (ReferenceEquals(_data, value))
			{
				return;
			}

			_data = value;
			_isDataWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("data", value);
		}
	}

	/// <summary>
	/// How many values make up each item. Writing it records a <c>itemSize</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float ItemSize
	{
		get { return _itemSize; }
		set
		{
			if (_itemSize == value)
			{
				return;
			}

			_itemSize = value;
			_isItemSizeWritten = true;
			RecordSet("itemSize", value);
		}
	}

	/// <summary>
	/// The offset in the underlying array buffer where an item starts. Writing it records a
	/// <c>offset</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Offset
	{
		get { return _offset; }
		set
		{
			if (_offset == value)
			{
				return;
			}

			_offset = value;
			_isOffsetWritten = true;
			RecordSet("offset", value);
		}
	}

	/// <summary>
	/// The <c>normalized</c> property of the JavaScript-side object. Writing it records a
	/// <c>normalized</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool Normalized
	{
		get { return _normalized; }
		set
		{
			if (_normalized == value)
			{
				return;
			}

			_normalized = value;
			_isNormalizedWritten = true;
			RecordSet("normalized", value);
		}
	}

	/// <summary>
	/// Flag to indicate that the <c>.data</c> (<see cref="InterleavedBuffer"/>) attribute has changed
	/// and should be re-sent to the GPU. Writing it records a <c>needsUpdate</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool NeedsUpdate
	{
		get { return _needsUpdate; }
		set
		{
			if (_needsUpdate == value)
			{
				return;
			}

			_needsUpdate = value;
			_isNeedsUpdateWritten = true;
			RecordSet("needsUpdate", value);
		}
	}

	/// <summary>
	/// Applies matrix <see cref="Matrix4">m</see> to every Vector3 element of this
	/// InterleavedBufferAttribute.
	/// </summary>
	/// <param name="m">Value forwarded to the <c>m</c> argument.</param>
	public void ApplyMatrix4(Matrix4 m)
	{
		RecordCall("applyMatrix4", m);
	}

	/// <summary>
	/// Applies normal matrix <see cref="Matrix3">m</see> to every Vector3 element of this
	/// InterleavedBufferAttribute.
	/// </summary>
	/// <param name="m">Value forwarded to the <c>m</c> argument.</param>
	public void ApplyNormalMatrix(Matrix3 m)
	{
		RecordCall("applyNormalMatrix", m);
	}

	/// <summary>
	/// Applies matrix <see cref="Matrix4">m</see> to every Vector3 element of this
	/// InterleavedBufferAttribute, interpreting the elements as a direction vectors.
	/// </summary>
	/// <param name="m">Value forwarded to the <c>m</c> argument.</param>
	public void TransformDirection(Matrix4 m)
	{
		RecordCall("transformDirection", m);
	}

	/// <summary>Sets the given component of the vector at the given index.</summary>
	/// <param name="index">Value forwarded to the <c>index</c> argument.</param>
	/// <param name="component">Value forwarded to the <c>component</c> argument.</param>
	/// <param name="value">Value forwarded to the <c>value</c> argument.</param>
	public void SetComponent(int index, float component, float value)
	{
		RecordCall("setComponent", index, component, value);
	}

	/// <summary>Sets the x component of the item at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="x"></param>
	public void SetX(int index, float x)
	{
		RecordCall("setX", index, x);
	}

	/// <summary>Sets the y component of the item at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="y"></param>
	public void SetY(int index, float y)
	{
		RecordCall("setY", index, y);
	}

	/// <summary>Sets the z component of the item at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="z"></param>
	public void SetZ(int index, float z)
	{
		RecordCall("setZ", index, z);
	}

	/// <summary>Sets the w component of the item at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="z"></param>
	public void SetW(int index, float z)
	{
		RecordCall("setW", index, z);
	}

	/// <summary>Sets the x and y components of the item at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public void SetXY(int index, float x, float y)
	{
		RecordCall("setXY", index, x, y);
	}

	/// <summary>Sets the x, y and z components of the item at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public void SetXYZ(int index, float x, float y, float z)
	{
		RecordCall("setXYZ", index, x, y, z);
	}

	/// <summary>Sets the x, y, z and w components of the item at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <param name="w"></param>
	public void SetXYZW(int index, float x, float y, float z, float w)
	{
		RecordCall("setXYZW", index, x, y, z, w);
	}

	/// <summary>
	/// The value of <c>.data</c>.<c>count</c>. If the buffer is storing a 3-component item (such as a
	/// _position, normal, or color_), then this will count the number of such items stored. Read-only
	/// in three.js, so it is read on demand rather than mirrored: records a get op, sends it behind
	/// every write already pending, and completes with the value <c>count</c> held.
	/// </summary>
	/// <returns>The value <c>count</c> held, once the JavaScript side has answered.</returns>
	public Task<int> CountAsync()
	{
		return GetAsync<int>("count");
	}

	/// <summary>
	/// The value of <c>data</c>.<c>array</c>. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>array</c> held.
	/// </summary>
	/// <returns>The value <c>array</c> held, once the JavaScript side has answered.</returns>
	public Task<TypedArray> ArrayAsync()
	{
		return GetAsync<TypedArray>("array");
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="InterleavedBufferAttribute"/>.
	/// Read-only in three.js, so it is read on demand rather than mirrored: records a get op, sends it
	/// behind every write already pending, and completes with the value
	/// <c>isInterleavedBufferAttribute</c> held.
	/// </summary>
	/// <returns>
	/// The value <c>isInterleavedBufferAttribute</c> held, once the JavaScript side has answered.
	/// </returns>
	public Task<bool> IsInterleavedBufferAttributeAsync()
	{
		return GetAsync<bool>("isInterleavedBufferAttribute");
	}

	/// <summary>
	/// Returns the given component of the vector at the given index. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>getComponent</c> returned.
	/// </summary>
	/// <param name="index">Value forwarded to the <c>index</c> argument.</param>
	/// <param name="component">Value forwarded to the <c>component</c> argument.</param>
	/// <returns>The value <c>getComponent</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetComponentAsync(int index, float component)
	{
		return RecordRead<float>("getComponent", index, component);
	}

	/// <summary>
	/// Returns the x component of the item at the given index. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>getX</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The value <c>getX</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetXAsync(int index)
	{
		return RecordRead<float>("getX", index);
	}

	/// <summary>
	/// Returns the y component of the item at the given index. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>getY</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The value <c>getY</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetYAsync(int index)
	{
		return RecordRead<float>("getY", index);
	}

	/// <summary>
	/// Returns the z component of the item at the given index. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>getZ</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The value <c>getZ</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetZAsync(int index)
	{
		return RecordRead<float>("getZ", index);
	}

	/// <summary>
	/// Returns the w component of the item at the given index. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>getW</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The value <c>getW</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetWAsync(int index)
	{
		return RecordRead<float>("getW", index);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.InterleavedBufferAttribute</c> is constructed from, so their
	/// create ops reach the batch before the one that references them by handle, then emits this
	/// object's own. A replayed value that is itself a mirrored object is attached first, so its create
	/// op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_interleavedBuffer.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isDataWritten)
		{
			_data?.AttachTo(batch);
			batch.Set(Handle, "data", ThreeValue.Encode(_data));
		}

		if (_isItemSizeWritten)
		{
			batch.Set(Handle, "itemSize", ThreeValue.Encode(_itemSize));
		}

		if (_isOffsetWritten)
		{
			batch.Set(Handle, "offset", ThreeValue.Encode(_offset));
		}

		if (_isNormalizedWritten)
		{
			batch.Set(Handle, "normalized", ThreeValue.Encode(_normalized));
		}

		if (_isNeedsUpdateWritten)
		{
			batch.Set(Handle, "needsUpdate", ThreeValue.Encode(_needsUpdate));
		}
	}
}
