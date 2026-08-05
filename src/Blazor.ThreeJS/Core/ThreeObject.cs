namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Base for every mirrored three.js object. Holds the handle that identifies this object on the
/// JavaScript side and routes writes into the batch. Reads never leave C#.
/// </summary>
public abstract class ThreeObject
{
	private static int _nextHandle;

	/// <summary>
	/// Commands invoked before this object reached a batch, held until <see cref="AttachTo"/> can
	/// replay them. Stays <see langword="null"/> until the first such command, so an object built the
	/// usual way — attached, then driven — never allocates it.
	/// </summary>
	private List<PendingCall>? _pendingCalls;

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
	/// Attaches this object to a batch: assigns <see cref="Batch"/>, emits the create op, replays the
	/// state written before the attach, then replays the commands invoked before it. That order is the
	/// whole contract — a replayed command observes the object three.js would have had at the moment it
	/// was invoked, which for <c>lookAt</c> and its kind means the replayed property values rather than
	/// three.js's constructor defaults.
	/// Idempotent — calling this a second time with the same batch is a no-op, which is what lets two
	/// objects (e.g. two meshes) share the same geometry or material instance without emitting a
	/// duplicate create for it. Virtual so a subclass with its own attachment concerns (attaching
	/// children) can extend it while this attach-once guard stays the single source of truth for
	/// whether the create op was already emitted.
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
		EmitState(batch);
		ReplayPendingCalls(batch);
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
	/// first and attaching any argument that is itself a mirrored object, so its create op reaches the
	/// batch before the call that references it by handle.
	/// <para>
	/// Invoked before this object is attached, the call is held instead and replayed by
	/// <see cref="AttachTo"/> once the create op and the state replay have gone in. Dropping it — which
	/// is what a bare no-op would do — is the one outcome that has no signal anywhere: properties
	/// already survive a pre-attach write by being replayed from fields, so a command that silently did
	/// not happen would be the only member kind where construction order changes the result.
	/// </para>
	/// <para>
	/// Arguments are encoded at invocation time, not at replay time, so a held call carries the values
	/// it was given rather than whatever a mutable argument was later changed to.
	/// </para>
	/// </summary>
	/// <param name="member">Name of the method to invoke.</param>
	/// <param name="args">Positional arguments to pass to the method.</param>
	protected void RecordCall(string member, params object?[] args)
	{
		var encodedArgs = args
			.Select(ThreeValue.Encode)
			.ToArray();

		if (Batch is null)
		{
			_pendingCalls ??= [];
			_pendingCalls.Add(new PendingCall
			{
				Member = member,
				Arguments = args,
				EncodedArguments = encodedArgs
			});
			return;
		}

		AttachMirroredArguments(Batch, args);
		Batch.Call(Handle, member, encodedArgs);
	}

	/// <summary>
	/// Invokes a method on this object and hands its return value back, which is the one thing the rest
	/// of this class cannot do: every other member records an instruction and returns immediately.
	/// <para>
	/// The read is recorded into <see cref="Batch"/> behind everything already pending and the whole
	/// batch is sent in one call, so the value is taken after the writes the caller has already made.
	/// </para>
	/// <para>
	/// Unlike <see cref="RecordCall"/>, a read on an unattached object is <b>not</b> held for replay: a
	/// held write eventually happens, whereas a held read has no value to give the caller now, and there
	/// is no JavaScript object to ask yet. It fails at the call site instead.
	/// </para>
	/// </summary>
	/// <typeparam name="TValue">C# type the query declares it returns.</typeparam>
	/// <param name="member">Name of the three.js method to invoke.</param>
	/// <param name="args">Positional arguments to pass to the method.</param>
	/// <returns>The value three.js returned, decoded into <typeparamref name="TValue"/>.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when this object is not attached to a <see cref="ThreeContext"/>, so there is nothing to
	/// read from.
	/// </exception>
	protected Task<TValue> RecordRead<TValue>(string member, params object?[] args)
	{
		var batch = Batch;
		if (batch?.Context is not { } context)
		{
			throw new InvalidOperationException(
				$"'{GetType().Name}' (handle {Handle}) is not attached to a {nameof(ThreeContext)}, so '{member}' has nothing to read from. " +
				$"A property written before an attach is replayed once it happens, but a read cannot be: there is no JavaScript object to ask, " +
				$"and no value to hand back now. Attach the object graph to a context first.");
		}

		AttachMirroredArguments(batch, args);
		var encodedArgs = args
			.Select(ThreeValue.Encode)
			.ToArray();

		return context.ReadAsync<TValue>(Handle, member, encodedArgs);
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

	/// <summary>
	/// Replays the state written before this object was attached. Empty here, because a class that
	/// folds its own replay into <see cref="EmitCreate"/> has nothing left to say. The hook exists so
	/// that <see cref="AttachTo"/> has a named slot between the create op and the command replay for
	/// the classes that do replay separately — <c>Object3D</c>, which has a subtree to attach after
	/// its own state — and so that a held command can be guaranteed to land after both.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal virtual void EmitState(ThreeBatch batch)
	{
	}

	/// <summary>Positional arguments to pass to the three.js constructor when this object is created.</summary>
	protected virtual object?[] ConstructorArgs
	{
		get { return []; }
	}

	/// <summary>
	/// Records every command held while this object had no batch, in invocation order, and releases the
	/// queue. Only ever reached once per object: <see cref="AttachTo"/> returns early on an object that
	/// already has a batch, and every later command records straight into it.
	/// </summary>
	/// <param name="batch">Batch to record the call ops into.</param>
	private void ReplayPendingCalls(ThreeBatch batch)
	{
		if (_pendingCalls is null)
		{
			return;
		}

		foreach (var pendingCall in _pendingCalls)
		{
			AttachMirroredArguments(batch, pendingCall.Arguments);
			batch.Call(Handle, pendingCall.Member, pendingCall.EncodedArguments);
		}

		_pendingCalls = null;
	}

	/// <summary>
	/// Attaches every argument that is itself a mirrored object. A <c>$ref</c> to a handle the applier
	/// has never seen is an unknown-handle failure in the browser, and an argument passed to a command
	/// invoked before this object reached a batch has not been attached by anything else. Attaching
	/// rather than emitting the create op directly is what keeps a shared instance from being created
	/// twice.
	/// </summary>
	/// <param name="batch">Batch to attach the arguments to.</param>
	/// <param name="args">The command's positional arguments, unencoded.</param>
	private static void AttachMirroredArguments(ThreeBatch batch, object?[] args)
	{
		foreach (var arg in args)
		{
			if (arg is ThreeObject mirroredArgument)
			{
				mirroredArgument.AttachTo(batch);
			}
		}
	}

	/// <summary>A command invoked before its object was attached, held until the attach can replay it.</summary>
	private sealed class PendingCall
	{
		/// <summary>Name of the three.js method to invoke.</summary>
		public required string Member { get; init; }

		/// <summary>The arguments as the caller passed them, kept so the mirrored ones can be attached on replay.</summary>
		public required object?[] Arguments { get; init; }

		/// <summary>The same arguments in wire form, encoded at invocation time.</summary>
		public required object?[] EncodedArguments { get; init; }
	}
}
