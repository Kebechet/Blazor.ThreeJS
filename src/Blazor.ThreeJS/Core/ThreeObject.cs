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
	internal int Handle { get; }

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
	/// Idempotent — calling this a second time with the same batch is a no-op, which is what lets two
	/// objects (e.g. two meshes) share the same geometry or material instance without emitting a
	/// duplicate create for it. Virtual so a subclass with its own attachment concerns (replaying
	/// transform state, attaching children) can extend it while this attach-once guard stays the
	/// single source of truth for whether the create op was already emitted.
	/// </summary>
	/// <param name="batch">The batch to attach this object to.</param>
	/// <exception cref="InvalidOperationException">
	/// Thrown when this object is already attached to a different batch. See
	/// <see cref="ThrowIfAttachedToAnotherBatch"/>.
	/// </exception>
	internal virtual void AttachTo(ThreeBatch batch)
	{
		ThrowIfAttachedToAnotherBatch(batch);
		if (Batch is not null)
		{
			return;
		}

		Batch = batch;
		EmitCreate(batch);
	}

	/// <summary>
	/// Rejects an attempt to attach this object to a second batch. An object records its writes into
	/// exactly one batch, so sharing it between two contexts would emit a handle reference into a
	/// context that never created the object — an unknown-handle failure in the browser with no
	/// C#-side signal. Failing at the call site instead names the object and the mistake.
	/// </summary>
	/// <param name="batch">The batch this object is being attached to.</param>
	/// <exception cref="InvalidOperationException">Thrown when this object is already attached to a different batch.</exception>
	private protected void ThrowIfAttachedToAnotherBatch(ThreeBatch batch)
	{
		if (Batch is null || ReferenceEquals(Batch, batch))
		{
			return;
		}

		throw new InvalidOperationException(
			$"'{GetType().Name}' (handle {Handle}) is already attached to another {nameof(ThreeContext)} and cannot be attached to a second one. " +
			$"Handles are per-context, so the other context would receive a reference to an object it never created. " +
			$"Build one object graph per {nameof(ThreeContext)}.");
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
	/// <para>
	/// An override records every value through <see cref="ThreeValue.Encode"/>, including primitives
	/// that round-trip unchanged. One unconditional rule costs nothing and leaves no per-property
	/// judgement call about which values need encoding.
	/// </para>
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
