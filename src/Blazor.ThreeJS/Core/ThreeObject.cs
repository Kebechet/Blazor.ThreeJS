namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Base for every mirrored three.js object. Holds the handle that identifies this object on the
/// JavaScript side and routes writes into the batch. Reads never leave C#.
/// </summary>
public abstract class ThreeObject
{
	private static int _nextHandle;

	/// <summary>
	/// Handle identifying this object on the JavaScript side. Allocated monotonically in C# via
	/// <see cref="Interlocked.Increment(ref int)"/> at construction time, so creating an object
	/// never awaits and never round-trips to JavaScript.
	/// </summary>
	public int Handle { get; }

	/// <summary>
	/// Assigned when the object is attached to a context. Until then, writes are held in the
	/// object's own fields and replayed by <see cref="EmitCreate"/> on attach.
	/// </summary>
	internal ThreeBatch? Batch { get; set; }

	/// <summary>Name of the corresponding export on the three.js namespace, e.g. <c>"Mesh"</c>.</summary>
	protected abstract string ThreeTypeName { get; }

	/// <summary>
	/// Allocates this object's <see cref="Handle"/>. Does not touch <see cref="Batch"/> — an object
	/// can be constructed and configured before it is ever attached to a scene.
	/// </summary>
	protected ThreeObject()
	{
		Handle = Interlocked.Increment(ref _nextHandle);
	}

	/// <summary>
	/// Attaches this object to a batch: assigns <see cref="Batch"/> and emits the create op.
	/// Idempotent — calling this a second time on an already-attached object is a no-op, which is
	/// what lets two objects (e.g. two meshes) share the same geometry or material instance without
	/// emitting a duplicate create for it. Virtual so a subclass with its own attachment concerns
	/// (replaying transform state, attaching children) can extend it while this attach-once guard
	/// stays the single source of truth for whether the create op was already emitted.
	/// </summary>
	/// <param name="batch">The batch to attach this object to.</param>
	public virtual void AttachTo(ThreeBatch batch)
	{
		if (Batch is not null)
		{
			return;
		}

		Batch = batch;
		EmitCreate(batch);
	}

	/// <summary>
	/// Records a property write on this object into <see cref="Batch"/>, encoding the value first.
	/// A no-op while <see cref="Batch"/> is unset.
	/// </summary>
	/// <param name="member">Name of the property being written.</param>
	/// <param name="value">The new value.</param>
	protected void RecordSet(string member, object? value)
	{
		Batch?.Set(Handle, member, ThreeValue.Encode(value));
	}

	/// <summary>
	/// Records a method invocation on this object into <see cref="Batch"/>, encoding each argument
	/// first. A no-op while <see cref="Batch"/> is unset.
	/// </summary>
	/// <param name="member">Name of the method to invoke.</param>
	/// <param name="args">Positional arguments to pass to the method.</param>
	protected void RecordCall(string member, params object?[] args)
	{
		var encodedArgs = args
			.Select(ThreeValue.Encode)
			.ToArray();

		Batch?.Call(Handle, member, encodedArgs);
	}

	/// <summary>
	/// Emits the create op plus every property this object currently holds. Called on attach, and
	/// again after a WebGL context loss to rebuild the scene from the C# mirror.
	/// </summary>
	/// <param name="batch">Batch to record the create op into.</param>
	internal virtual void EmitCreate(ThreeBatch batch)
	{
		var encodedConstructorArgs = ConstructorArgs
			.Select(ThreeValue.Encode)
			.ToArray();

		batch.Create(Handle, ThreeTypeName, encodedConstructorArgs);
	}

	/// <summary>Positional arguments to pass to the three.js constructor when this object is created.</summary>
	protected virtual object?[] ConstructorArgs
	{
		get { return []; }
	}
}
