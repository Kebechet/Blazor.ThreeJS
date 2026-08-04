// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This renderer module provides a series of statistical information about the GPU memory and the
/// rendering process. Useful for debugging and monitoring. The JavaScript-side <c>THREE.Info</c>.
/// </summary>
public sealed class Info : ThreeObject
{
	private bool _autoReset = true;
	private bool _isAutoResetWritten;

	/// <summary>Initializes a new <see cref="Info"/>.</summary>
	public Info()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Info</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Info"; }
	}

	/// <summary>
	/// Whether frame related metrics should automatically be resetted or not. This property should be
	/// set to <c>false</c> by apps which manage their own animation loop. They must then call
	/// <c>renderer.info.reset()</c> once per frame manually. Writing it records a <c>autoReset</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool AutoReset
	{
		get { return _autoReset; }
		set
		{
			if (_autoReset == value)
			{
				return;
			}

			_autoReset = value;
			_isAutoResetWritten = true;
			RecordSet("autoReset", value);
		}
	}

	/// <summary>This method should be executed per draw call and updates the corresponding metrics.</summary>
	/// <param name="object">The 3D object that is going to be rendered.</param>
	/// <param name="count">The vertex or index count.</param>
	/// <param name="instanceCount">The instance count.</param>
	public void Update(Object3D @object, int count, int instanceCount)
	{
		if (Batch is not null)
		{
			@object.AttachTo(Batch);
		}

		RecordCall("update", @object, count, instanceCount);
	}

	/// <summary>Resets frame related metrics.</summary>
	public void Reset()
	{
		RecordCall("reset");
	}

	/// <summary>Performs a complete reset of the object.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>Tracks a readback buffer memory explicitly.</summary>
	/// <param name="readbackBuffer">The readback buffer to track.</param>
	public void CreateReadbackBuffer(ReadbackBuffer readbackBuffer)
	{
		if (Batch is not null)
		{
			readbackBuffer.AttachTo(Batch);
		}

		RecordCall("createReadbackBuffer", readbackBuffer);
	}

	/// <summary>Tracks a readback buffer memory explicitly.</summary>
	/// <param name="readbackBuffer">The readback buffer to track.</param>
	public void DestroyReadbackBuffer(ReadbackBuffer readbackBuffer)
	{
		if (Batch is not null)
		{
			readbackBuffer.AttachTo(Batch);
		}

		RecordCall("destroyReadbackBuffer", readbackBuffer);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Info</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isAutoResetWritten)
		{
			batch.Set(Handle, "autoReset", ThreeValue.Encode(_autoReset));
		}
	}
}
