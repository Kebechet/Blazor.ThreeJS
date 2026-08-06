// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A readback buffer is used to transfer data from the GPU to the CPU. It is primarily used to read
/// back compute shader results. The JavaScript-side <c>THREE.ReadbackBuffer</c>.
/// </summary>
public sealed class ReadbackBuffer : EventDispatcher
{
	private float _maxByteLength;
	private string _name = string.Empty;
	private bool _isNameWritten;
	private bool _isMaxByteLengthWritten;

	/// <summary>Constructs a new readback buffer.</summary>
	/// <param name="maxByteLength">The maximum size of the buffer to be read back.</param>
	public ReadbackBuffer(float maxByteLength)
	{
		_maxByteLength = maxByteLength;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ReadbackBuffer</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ReadbackBuffer(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_maxByteLength = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ReadbackBuffer</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ReadbackBuffer"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.ReadbackBuffer</c>: maxByteLength.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_maxByteLength]; }
	}

	/// <summary>
	/// Name used for debugging purposes. Writing it records a <c>name</c> property write once this
	/// object is attached; writing the value already held records nothing.
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
	/// The maximum size of the buffer to be read back. Writing it records a <c>maxByteLength</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float MaxByteLength
	{
		get { return _maxByteLength; }
		set
		{
			if (_maxByteLength == value)
			{
				return;
			}

			_maxByteLength = value;
			_isMaxByteLengthWritten = true;
			RecordSet("maxByteLength", value);
		}
	}

	/// <summary>
	/// Releases the mapped buffer data so the GPU buffer can be used by the GPU again. Note: Any
	/// <c>ArrayBuffer</c> data associated with this readback buffer are removed and no longer
	/// accessible after calling this method.
	/// </summary>
	public void Release()
	{
		RecordCall("release");
	}

	/// <summary>Frees internal resources.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isReadbackBuffer</c> held.
	/// </summary>
	/// <returns>The value <c>isReadbackBuffer</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsReadbackBufferAsync()
	{
		return GetAsync<bool>("isReadbackBuffer");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.ReadbackBuffer</c>, then replays every property written before
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

		if (_isMaxByteLengthWritten)
		{
			batch.Set(Handle, "maxByteLength", ThreeValue.Encode(_maxByteLength));
		}
	}
}
