// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// **"Interleaved"** means that multiple attributes, possibly of different types, (e.g., _position,
/// normal, uv, color_) are packed into a single array buffer. An introduction into interleaved
/// arrays can be found here:
/// <see href="https://blog.tojicode.com/2011/05/interleaved-array-basics.html">Interleaved array
/// basics</see>. The JavaScript-side <c>THREE.InterleavedBuffer</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl_buffergeometry_points_interleaved">webgl / buffergeometry / points / interleaved</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/InterleavedBuffer">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/InterleavedBuffer.js">Source</seealso>
public class InterleavedBuffer : ThreeObject
{
	private TypedArray _array;
	private int _stride;
	private Usage _usage;
	private BufferAttributeUpdateRanges[] _updateRanges = [];
	private int _version = 0;
	private int _count = 0;
	private bool _needsUpdate;
	private string _uuid = string.Empty;
	private bool _isArrayWritten;
	private bool _isStrideWritten;
	private bool _isUsageWritten;
	private bool _isUpdateRangesWritten;
	private bool _isVersionWritten;
	private bool _isCountWritten;
	private bool _isNeedsUpdateWritten;
	private bool _isUuidWritten;

	/// <summary>Create a new instance of <see cref="InterleavedBuffer"/>.</summary>
	/// <param name="array">
	/// A
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/TypedArray">TypedArray</see>
	/// with a shared buffer. Stores the geometry data.
	/// </param>
	/// <param name="stride">The number of typed-array elements per vertex.</param>
	public InterleavedBuffer(TypedArray array, int stride)
	{
		_array = array;
		_stride = stride;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>InterleavedBuffer</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal InterleavedBuffer(ThreeBatch batch, int handle)
		: base(handle)
	{
		_array = default!;
		_stride = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.InterleavedBuffer</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "InterleavedBuffer"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.InterleavedBuffer</c>: array, stride.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_array, _stride]; }
	}

	/// <summary>
	/// A
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/TypedArray">TypedArray</see>
	/// with a shared buffer. Stores the geometry data. Writing it records a <c>array</c> property write
	/// once this object is attached; writing the value already held records nothing.
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
	/// The number of
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/TypedArray">TypedArray</see>
	/// elements per vertex. Writing it records a <c>stride</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public int Stride
	{
		get { return _stride; }
		set
		{
			if (_stride == value)
			{
				return;
			}

			_stride = value;
			_isStrideWritten = true;
			RecordSet("stride", value);
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
	/// This can be used to only update some components of stored data. Use the <c>.addUpdateRange</c>
	/// function to add ranges to this array. Writing it records a <c>updateRanges</c> property write
	/// once this object is attached; writing the value already held records nothing.
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
	/// Gives the total number of elements in the array. Writing it records a <c>count</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public int Count
	{
		get { return _count; }
		set
		{
			if (_count == value)
			{
				return;
			}

			_count = value;
			_isCountWritten = true;
			RecordSet("count", value);
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
	/// <see href="http://en.wikipedia.org/wiki/Universally_unique_identifier">UUID</see> of this object
	/// instance. Writing it records a <c>uuid</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public string Uuid
	{
		get { return _uuid; }
		set
		{
			if (_uuid == value)
			{
				return;
			}

			_uuid = value;
			_isUuidWritten = true;
			RecordSet("uuid", value);
		}
	}

	/// <summary>
	/// Calls
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/TypedArray/set">TypedArray.set</see>(
	/// <c>value</c>, <c>offset</c> ) on the <c>array</c>.
	/// </summary>
	/// <param name="value">The source <c>TypedArray</c>.</param>
	/// <param name="offset">index of the <c>array</c> at which to start copying.</param>
	public void Set(float[] value, int offset = 0)
	{
		RecordCall("set", value, offset);
	}

	/// <summary>
	/// Set <c>usage</c>. This writes the same three.js state as <see cref="Usage"/> and the mirror does
	/// not learn from it: afterwards <c>Usage</c> still reports its previous value, and writing that
	/// value back records nothing at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="value">
	/// Corresponds to the <c>usage</c> parameter of
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/bufferData">WebGLRenderingContext.bufferData</see>.
	/// </param>
	public void SetUsage(Usage value)
	{
		RecordCall("setUsage", value);
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

	/// <summary>
	/// Copies another <see cref="InterleavedBuffer"/> to this <see cref="InterleavedBuffer"/> instance.
	/// </summary>
	/// <param name="source">Value forwarded to the <c>source</c> argument.</param>
	public void Copy(InterleavedBuffer source)
	{
		RecordCall("copy", source);
	}

	/// <summary>Copies data from <c>attribute</c>[<c>index2</c>] to <c>array</c>[<c>index1</c>].</summary>
	/// <param name="index1"></param>
	/// <param name="attribute">Value forwarded to the <c>attribute</c> argument.</param>
	/// <param name="index2"></param>
	public void CopyAt(int index1, InterleavedBufferAttribute attribute, int index2)
	{
		RecordCall("copyAt", index1, attribute, index2);
	}

	/// <summary>
	/// Reads <c>isInterleavedBuffer</c> back from the JavaScript-side object. Read-only in three.js, so
	/// it is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isInterleavedBuffer</c> held.
	/// </summary>
	/// <returns>The value <c>isInterleavedBuffer</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsInterleavedBufferAsync()
	{
		return GetAsync<bool>("isInterleavedBuffer");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.InterleavedBuffer</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isArrayWritten)
		{
			batch.Set(Handle, "array", ThreeValue.Encode(_array));
		}

		if (_isStrideWritten)
		{
			batch.Set(Handle, "stride", ThreeValue.Encode(_stride));
		}

		if (_isUsageWritten)
		{
			batch.Set(Handle, "usage", ThreeValue.Encode(_usage));
		}

		if (_isUpdateRangesWritten)
		{
			batch.Set(Handle, "updateRanges", ThreeValue.Encode(_updateRanges));
		}

		if (_isVersionWritten)
		{
			batch.Set(Handle, "version", ThreeValue.Encode(_version));
		}

		if (_isCountWritten)
		{
			batch.Set(Handle, "count", ThreeValue.Encode(_count));
		}

		if (_isNeedsUpdateWritten)
		{
			batch.Set(Handle, "needsUpdate", ThreeValue.Encode(_needsUpdate));
		}

		if (_isUuidWritten)
		{
			batch.Set(Handle, "uuid", ThreeValue.Encode(_uuid));
		}
	}
}
