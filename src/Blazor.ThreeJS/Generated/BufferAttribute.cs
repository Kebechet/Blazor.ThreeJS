// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class stores data for an attribute (such as vertex positions, face indices, normals,
/// colors, UVs, and any custom attributes ) associated with a <c>BufferGeometry</c>, which allows
/// for more efficient passing of data to the GPU. The JavaScript-side <c>THREE.BufferAttribute</c>.
/// </summary>
/// <remarks>
/// When working with _vector-like_ data, the _<c>.fromBufferAttribute( attribute, index )</c>_
/// helper methods on <c>Vector2</c>, <c>Vector3</c>, <c>Vector4</c>, and <c>Color</c> classes may
/// be helpful.
/// </remarks>
/// <seealso href="https://threejs.org/examples/#webgl_buffergeometry">WebGL / BufferGeometry - Clean up Memory</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/BufferAttribute">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/BufferAttribute.js">Source</seealso>
public class BufferAttribute : EventDispatcher
{
	private TypedArray _array;
	private float _itemSize;
	private bool _normalized;
	private string _name = string.Empty;
	private Usage _usage;
	private AttributeGPUType _gpuType;
	private BufferAttributeUpdateRanges[] _updateRanges = [];
	private int _version = 0;
	private bool _needsUpdate;
	private bool _isNameWritten;
	private bool _isArrayWritten;
	private bool _isItemSizeWritten;
	private bool _isUsageWritten;
	private bool _isGpuTypeWritten;
	private bool _isUpdateRangesWritten;
	private bool _isVersionWritten;
	private bool _isNormalizedWritten;
	private bool _isNeedsUpdateWritten;

	/// <summary>This creates a new <c>GLBufferAttribute</c> object.</summary>
	/// <param name="array">
	/// Must be a <c>TypedArray</c>. Used to instantiate the buffer. This array should have <c>itemSize
	/// * numVertices</c> elements, where numVertices is the number of vertices in the associated
	/// <c>BufferGeometry</c>.
	/// </param>
	/// <param name="itemSize">
	/// the number of values of the <c>array</c> that should be associated with a particular vertex. For
	/// instance, if this attribute is storing a 3-component vector (such as a _position_, _normal_, or
	/// _color_), then itemSize should be <c>3</c>.
	/// </param>
	/// <param name="normalized">
	/// Applies to integer data only. Indicates how the underlying data in the buffer maps to the values
	/// in the GLSL code. For instance, if <c>array</c> is an instance of <c>UInt16Array</c>, and
	/// <c>normalized</c> is true, the values <c>0</c> - <c>+65535</c> in the array data will be mapped
	/// to <c>0.0f</c> - <c>+1.0f</c> in the GLSL attribute. An <c>Int16Array</c> (signed) would map
	/// from <c>-32768</c> - <c>+32767</c> to <c>-1.0f</c> - <c>+1.0f</c>. If normalized is false, the
	/// values will be converted to floats unmodified, i.e. <c>32767</c> becomes <c>32767.0f</c>.
	/// </param>
	public BufferAttribute(TypedArray array, float itemSize, bool normalized = false)
	{
		_array = array;
		_itemSize = itemSize;
		_normalized = normalized;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>BufferAttribute</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal BufferAttribute(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_array = default!;
		_itemSize = default!;
		_normalized = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BufferAttribute</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "BufferAttribute"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.BufferAttribute</c>: array, itemSize, normalized.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_array, _itemSize, _normalized]; }
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
	/// The
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/TypedArray">TypedArray</see>
	/// holding data stored in the buffer. Writing it records a <c>array</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public TypedArray Array
	{
		get { return _array; }
		set
		{
			if (_array == value)
			{
				return;
			}

			_array = value;
			_isArrayWritten = true;
			RecordSet("array", value);
		}
	}

	/// <summary>
	/// The length of vectors that are being stored in the <c>array</c>. Writing it records a
	/// <c>itemSize</c> property write once this object is attached; writing the value already held
	/// records nothing.
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
	/// Defines the intended usage pattern of the data store for optimization purposes. Corresponds to
	/// the <c>usage</c> parameter of
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/bufferData">WebGLRenderingContext.bufferData</see>.
	/// Writing it records a <c>usage</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public Usage Usage
	{
		get { return _usage; }
		set
		{
			if (_usage == value)
			{
				return;
			}

			_usage = value;
			_isUsageWritten = true;
			RecordSet("usage", value);
		}
	}

	/// <summary>
	/// Configures the bound GPU type for use in shaders. Either <c>FloatType</c> or <c>IntType</c>,
	/// default is <c>FloatType</c>. Note: this only has an effect for integer arrays and is not
	/// configurable for float arrays. For lower precision float types, see
	/// https://threejs.org/docs/#api/en/core/bufferAttributeTypes/BufferAttributeTypes. Writing it
	/// records a <c>gpuType</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public AttributeGPUType GpuType
	{
		get { return _gpuType; }
		set
		{
			if (_gpuType == value)
			{
				return;
			}

			_gpuType = value;
			_isGpuTypeWritten = true;
			RecordSet("gpuType", value);
		}
	}

	/// <summary>
	/// This can be used to only update some components of stored vectors (for example, just the
	/// component related to color). Use the <c>.addUpdateRange</c> function to add ranges to this
	/// array. Writing it records a <c>updateRanges</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public BufferAttributeUpdateRanges[] UpdateRanges
	{
		get { return _updateRanges; }
		set
		{
			if (_updateRanges == value)
			{
				return;
			}

			_updateRanges = value;
			_isUpdateRangesWritten = true;
			RecordSet("updateRanges", value);
		}
	}

	/// <summary>
	/// A version number, incremented every time the <c>needsUpdate</c> property is set to true. Writing
	/// it records a <c>version</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public int Version
	{
		get { return _version; }
		set
		{
			if (_version == value)
			{
				return;
			}

			_version = value;
			_isVersionWritten = true;
			RecordSet("version", value);
		}
	}

	/// <summary>
	/// Indicates how the underlying data in the buffer maps to the values in the GLSL shader code.
	/// Writing it records a <c>normalized</c> property write once this object is attached; writing the
	/// value already held records nothing.
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
	/// Flag to indicate that this attribute has changed and should be re-sent to the GPU. Set this to
	/// true when you modify the value of the array. Writing it records a <c>needsUpdate</c> property
	/// write once this object is attached; writing the value already held records nothing.
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
	/// Set <c>usage</c>. This writes the same three.js state as <see cref="Usage"/> and the mirror does
	/// not learn from it: afterwards <c>Usage</c> still reports its previous value, and writing that
	/// value back records nothing at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="usage">
	/// Corresponds to the <c>usage</c> parameter of
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/bufferData">WebGLRenderingContext.bufferData</see>.
	/// </param>
	public void SetUsage(Usage usage)
	{
		RecordCall("setUsage", usage);
	}

	/// <summary>
	/// Adds a range of data in the data array to be updated on the GPU. Adds an object describing the
	/// range to the <c>.updateRanges</c> array.
	/// </summary>
	/// <param name="start">Value forwarded to the <c>start</c> argument.</param>
	/// <param name="count">Value forwarded to the <c>count</c> argument.</param>
	public void AddUpdateRange(float start, int count)
	{
		RecordCall("addUpdateRange", start, count);
	}

	/// <summary>Clears the <c>.updateRanges</c> array.</summary>
	public void ClearUpdateRanges()
	{
		RecordCall("clearUpdateRanges");
	}

	/// <summary>Copies another <see cref="BufferAttribute"/> to this <see cref="BufferAttribute"/>.</summary>
	/// <param name="source">Value forwarded to the <c>source</c> argument.</param>
	public void Copy(BufferAttribute source)
	{
		RecordCall("copy", source);
	}

	/// <summary>Copy a vector from bufferAttribute[index2] to <c>array</c>[index1].</summary>
	/// <param name="index1">Value forwarded to the <c>index1</c> argument.</param>
	/// <param name="attribute">Value forwarded to the <c>attribute</c> argument.</param>
	/// <param name="index2">Value forwarded to the <c>index2</c> argument.</param>
	public void CopyAt(float index1, BufferAttribute attribute, float index2)
	{
		RecordCall("copyAt", index1, attribute, index2);
	}

	/// <summary>
	/// Copy the array given here (which can be a normal array or <c>TypedArray</c>) into <c>array</c>.
	/// </summary>
	/// <param name="array">Value forwarded to the <c>array</c> argument.</param>
	public void CopyArray(float[] array)
	{
		RecordCall("copyArray", (object?) array);
	}

	/// <summary>
	/// Applies matrix <see cref="Matrix3">m</see> to every Vector3 element of this
	/// <see cref="BufferAttribute"/>.
	/// </summary>
	/// <param name="m">Value forwarded to the <c>m</c> argument.</param>
	public void ApplyMatrix3(Matrix3 m)
	{
		RecordCall("applyMatrix3", m);
	}

	/// <summary>
	/// Applies matrix <see cref="Matrix4">m</see> to every Vector3 element of this
	/// <see cref="BufferAttribute"/>.
	/// </summary>
	/// <param name="m">Value forwarded to the <c>m</c> argument.</param>
	public void ApplyMatrix4(Matrix4 m)
	{
		RecordCall("applyMatrix4", m);
	}

	/// <summary>
	/// Applies normal matrix <see cref="Matrix3">m</see> to every Vector3 element of this
	/// <see cref="BufferAttribute"/>.
	/// </summary>
	/// <param name="m">Value forwarded to the <c>m</c> argument.</param>
	public void ApplyNormalMatrix(Matrix3 m)
	{
		RecordCall("applyNormalMatrix", m);
	}

	/// <summary>
	/// Applies matrix <see cref="Matrix4">m</see> to every Vector3 element of this
	/// <see cref="BufferAttribute"/>, interpreting the elements as a direction vectors.
	/// </summary>
	/// <param name="m">Value forwarded to the <c>m</c> argument.</param>
	public void TransformDirection(Matrix4 m)
	{
		RecordCall("transformDirection", m);
	}

	/// <summary>
	/// Calls
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/TypedArray/set">TypedArray.set</see>(
	/// <c>value</c>, <c>offset</c> ) on the <c>array</c>.
	/// </summary>
	/// <param name="value"><c>Array</c> or <c>TypedArray</c> from which to copy values.</param>
	/// <param name="offset">index of the <c>array</c> at which to start copying.</param>
	public void Set(float[] value, int offset = 0)
	{
		RecordCall("set", value, offset);
	}

	/// <summary>Sets the given component of the vector at the given index.</summary>
	/// <param name="index">Value forwarded to the <c>index</c> argument.</param>
	/// <param name="component">Value forwarded to the <c>component</c> argument.</param>
	/// <param name="value">Value forwarded to the <c>value</c> argument.</param>
	public void SetComponent(int index, float component, float value)
	{
		RecordCall("setComponent", index, component, value);
	}

	/// <summary>Sets the x component of the vector at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="x">Value forwarded to the <c>x</c> argument.</param>
	public void SetX(int index, float x)
	{
		RecordCall("setX", index, x);
	}

	/// <summary>Sets the y component of the vector at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="y">Value forwarded to the <c>y</c> argument.</param>
	public void SetY(int index, float y)
	{
		RecordCall("setY", index, y);
	}

	/// <summary>Sets the z component of the vector at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="z">Value forwarded to the <c>z</c> argument.</param>
	public void SetZ(int index, float z)
	{
		RecordCall("setZ", index, z);
	}

	/// <summary>Sets the w component of the vector at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="z">Value forwarded to the <c>z</c> argument.</param>
	public void SetW(int index, float z)
	{
		RecordCall("setW", index, z);
	}

	/// <summary>Sets the x and y components of the vector at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="x">Value forwarded to the <c>x</c> argument.</param>
	/// <param name="y">Value forwarded to the <c>y</c> argument.</param>
	public void SetXY(int index, float x, float y)
	{
		RecordCall("setXY", index, x, y);
	}

	/// <summary>Sets the x, y and z components of the vector at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="x">Value forwarded to the <c>x</c> argument.</param>
	/// <param name="y">Value forwarded to the <c>y</c> argument.</param>
	/// <param name="z">Value forwarded to the <c>z</c> argument.</param>
	public void SetXYZ(int index, float x, float y, float z)
	{
		RecordCall("setXYZ", index, x, y, z);
	}

	/// <summary>Sets the x, y, z and w components of the vector at the given index.</summary>
	/// <param name="index"></param>
	/// <param name="x">Value forwarded to the <c>x</c> argument.</param>
	/// <param name="y">Value forwarded to the <c>y</c> argument.</param>
	/// <param name="z">Value forwarded to the <c>z</c> argument.</param>
	/// <param name="w">Value forwarded to the <c>w</c> argument.</param>
	public void SetXYZW(int index, float x, float y, float z, float w)
	{
		RecordCall("setXYZW", index, x, y, z, w);
	}

	/// <summary>Disposes of the buffer attribute. Available only in <see cref="WebGPURenderer"/>.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Unique number for this attribute instance. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>id</c> held.
	/// </summary>
	/// <returns>The value <c>id</c> held, once the JavaScript side has answered.</returns>
	public Task<float> IdAsync()
	{
		return GetAsync<float>("id");
	}

	/// <summary>
	/// Represents the number of items this buffer attribute stores. It is internally computed by
	/// dividing the <c>array</c>'s length by the <c>itemSize</c>. Read-only property. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>count</c> held.
	/// </summary>
	/// <returns>The value <c>count</c> held, once the JavaScript side has answered.</returns>
	public Task<int> CountAsync()
	{
		return GetAsync<int>("count");
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="BufferAttribute"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isBufferAttribute</c> held.
	/// </summary>
	/// <returns>The value <c>isBufferAttribute</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsBufferAttributeAsync()
	{
		return GetAsync<bool>("isBufferAttribute");
	}

	/// <summary>
	/// Reads <c>clone</c> back from the JavaScript-side object. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<BufferAttribute?> CloneAsync()
	{
		return RecordReadObject<BufferAttribute>("clone", (adoptedBatch, adoptedHandle) => new BufferAttribute(adoptedBatch, adoptedHandle));
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
	/// Returns the x component of the vector at the given index. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>getX</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The value <c>getX</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetXAsync(int index)
	{
		return RecordRead<float>("getX", index);
	}

	/// <summary>
	/// Returns the y component of the vector at the given index. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>getY</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The value <c>getY</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetYAsync(int index)
	{
		return RecordRead<float>("getY", index);
	}

	/// <summary>
	/// Returns the z component of the vector at the given index. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>getZ</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The value <c>getZ</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetZAsync(int index)
	{
		return RecordRead<float>("getZ", index);
	}

	/// <summary>
	/// Returns the w component of the vector at the given index. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>getW</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The value <c>getW</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetWAsync(int index)
	{
		return RecordRead<float>("getW", index);
	}

	/// <summary>
	/// Convert this object to three.js to the <c>data.attributes</c> part of
	/// <see href="https://github.com/mrdoob/three.js/wiki/JSON-Geometry-format-4">JSON Geometry format
	/// v4</see>,. Records a read op, sends it behind every write already pending, and completes with
	/// what <c>toJSON</c> returned.
	/// </summary>
	/// <returns>The value <c>toJSON</c> returned, once the JavaScript side has answered.</returns>
	public Task<BufferAttributeJSON> ToJSONAsync()
	{
		return RecordRead<BufferAttributeJSON>("toJSON");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.BufferAttribute</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isArrayWritten)
		{
			batch.Set(Handle, "array", ThreeValue.Encode(_array));
		}

		if (_isItemSizeWritten)
		{
			batch.Set(Handle, "itemSize", ThreeValue.Encode(_itemSize));
		}

		if (_isUsageWritten)
		{
			batch.Set(Handle, "usage", ThreeValue.Encode(_usage));
		}

		if (_isGpuTypeWritten)
		{
			batch.Set(Handle, "gpuType", ThreeValue.Encode(_gpuType));
		}

		if (_isUpdateRangesWritten)
		{
			batch.Set(Handle, "updateRanges", ThreeValue.Encode(_updateRanges));
		}

		if (_isVersionWritten)
		{
			batch.Set(Handle, "version", ThreeValue.Encode(_version));
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
