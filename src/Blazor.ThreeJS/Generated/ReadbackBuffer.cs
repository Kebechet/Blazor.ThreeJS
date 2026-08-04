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
