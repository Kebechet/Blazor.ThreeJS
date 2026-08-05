namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Base for every mirrored three.js object. Holds the handle that identifies this object on the
/// JavaScript side and routes writes into the batch. Reads never leave C#.
/// </summary>
public abstract class ThreeObject
{
	private static int _nextHandle;

	/// <summary>
	/// Commands invoked, and untyped property writes made, before this object reached a batch, held
	/// until <see cref="AttachTo"/> can replay them in order. Stays <see langword="null"/> until the
	/// first such op, so an object built the usual way — attached, then driven — never allocates it.
	/// <para>
	/// A typed property write is not in here and must not be: a generated class replays its own state
	/// from its fields, so queueing the write as well would send it twice.
	/// </para>
	/// </summary>
	private List<PendingOp>? _pendingOps;

	/// <summary>
	/// Handle identifying this object on the JavaScript side. Allocated monotonically in C# via
	/// <see cref="Interlocked.Increment(ref int)"/> at construction time, so creating an object
	/// never awaits and never round-trips to JavaScript.
	/// <para>
	/// Positive for an object C# created, negative for one the browser did. The two allocators share
	/// one handle space and never negotiate over it: this one counts up from 1 and the JavaScript one
	/// counts down from -1, so they cannot collide without a reserved block, a round trip, or an
	/// agreement either side could drift from. Both directions are enforced rather than assumed - see
	/// <see cref="ThrowIfNotMirrorAllocated"/> and <see cref="ThrowIfNotBrowserMinted"/>.
	/// </para>
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
		Handle = ThrowIfNotMirrorAllocated(Interlocked.Increment(ref _nextHandle));
	}

	/// <summary>
	/// Adopts a handle the browser minted for an object it created itself — a node of a loaded glTF
	/// graph, or an <c>OrbitControls</c> instance. No create op is ever emitted for such an object:
	/// it already exists, and the mirror's job is to name it, not to build it.
	/// <para>
	/// Not <see langword="protected"/>: a handle is only meaningful against the JavaScript object
	/// table it came from, so minting one is this assembly's business. A consumer deriving their own
	/// wrapper goes through the parameterless constructor and gets a C#-allocated handle with a
	/// create op behind it.
	/// </para>
	/// </summary>
	/// <param name="handle">The negative handle the JavaScript side registered the object under.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Thrown when <paramref name="handle"/> is not negative, which would put it in the half of the
	/// space this class's own allocator owns.
	/// </exception>
	private protected ThreeObject(int handle)
	{
		Handle = ThrowIfNotBrowserMinted(handle);
	}

	/// <summary>
	/// Checks a handle this class allocated is still in its own half of the space. Only reachable by
	/// exhausting <see cref="int"/>: <see cref="Interlocked.Increment(ref int)"/> wraps to
	/// <see cref="int.MinValue"/> rather than throwing, and a handle that wrapped would collide with a
	/// browser-minted one and silently address the wrong object. A long-lived Blazor Server process is
	/// where that is reachable at all, since the counter is static and never reset.
	/// </summary>
	/// <param name="handle">The freshly allocated handle.</param>
	/// <returns><paramref name="handle"/>, when it is valid.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the allocator has wrapped.</exception>
	internal static int ThrowIfNotMirrorAllocated(int handle)
	{
		if (handle > 0)
		{
			return handle;
		}

		throw new InvalidOperationException(
			$"The {nameof(ThreeObject)} handle allocator has run out of positive handles (it produced {handle}). " +
			$"Handles are allocated once per mirrored object for the life of the process and never reused, and negative ones " +
			$"belong to objects the browser created, so continuing would address one of those instead.");
	}

	/// <summary>
	/// Checks a handle offered as browser-minted really is one. A positive value here would mean the
	/// JavaScript side allocated out of the mirror's half of the space, which no amount of later
	/// checking could recover from — two objects would answer to the same handle.
	/// </summary>
	/// <param name="handle">The handle the JavaScript side reported.</param>
	/// <returns><paramref name="handle"/>, when it is valid.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the handle is not negative.</exception>
	internal static int ThrowIfNotBrowserMinted(int handle)
	{
		if (handle < 0)
		{
			return handle;
		}

		throw new ArgumentOutOfRangeException(
			nameof(handle),
			handle,
			$"A handle the browser minted must be negative. Positive handles are allocated by {nameof(ThreeObject)} for objects C# created, " +
			$"so accepting this one would let two objects answer to the same handle.");
	}

	/// <summary>
	/// Attaches this object to a batch: assigns <see cref="Batch"/>, emits the create op, replays the
	/// typed state written before the attach, then replays the ops held before it — the commands
	/// invoked and the untyped <see cref="Set"/> writes made. That order is the whole contract — a
	/// replayed command observes the object three.js would have had at the moment it was invoked, which
	/// for <c>lookAt</c> and its kind means the replayed property values rather than three.js's
	/// constructor defaults.
	/// <para>
	/// ⚠️ It also means a held <see cref="Set"/> lands <b>after</b> every typed property replay,
	/// whatever order the two were written in. A typed property is replayed from the field it lives in,
	/// which has no record of when it was assigned; only the held ops carry an order. Writing the same
	/// three.js property both ways before an attach is the one case where that is observable.
	/// </para>
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
		ReplayPendingOps(batch);
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
	/// Writes a property three.js has and this mirror does not, by its three.js name. The escape hatch
	/// for state: it reaches any property of any object, whether or not a generated class exposes one
	/// for it, and records the same <c>Set</c> op a typed property write records — so it coalesces,
	/// batches and flushes identically.
	/// <para>
	/// A value that is itself a mirrored object is attached first, so its create op reaches the batch
	/// before the reference that names it. Written before this object is attached, the write is held
	/// and replayed by <see cref="AttachTo"/>, so construction order does not matter here either.
	/// </para>
	/// <para>
	/// ⚠️ <b>The mirror does not learn from this.</b> Writing <c>visible</c> here leaves
	/// <c>Object3D.IsVisible</c> reporting whatever it reported before, and a later typed write of the
	/// value C# still believes is current records nothing at all — so the two spellings of one property
	/// must not be mixed. Where a typed property exists, use it.
	/// </para>
	/// </summary>
	/// <param name="member">Name of the three.js property to write, e.g. <c>"refDistance"</c>.</param>
	/// <param name="value">
	/// The new value: any primitive, <see cref="string"/>, <see cref="Enum"/>, <see langword="null"/>,
	/// another mirrored object, or one of the five hand-written math types.
	/// </param>
	/// <exception cref="NotSupportedException">
	/// Thrown for any other reference type. Such a value has no wire contract, and shipping its
	/// serialized public shape would let the applier assign that plain object over a live three.js
	/// instance with nothing raised anywhere.
	/// </exception>
	public void Set(string member, object? value)
	{
		var encodedValue = EncodeOrExplain(member, value);
		if (Batch is not { } batch)
		{
			_pendingOps ??= [];
			_pendingOps.Add(new PendingOp
			{
				Kind = ThreeOpKind.Set,
				Member = member,
				Arguments = [value],
				EncodedArguments = [encodedValue]
			});

			return;
		}

		// The invariant an untyped write is most likely to break: a $ref naming a handle the applier has
		// never seen is an unknown-handle failure in the browser, and nothing else on this path would
		// have created the referenced object. Attaching rather than emitting its create op directly is
		// what keeps a shared instance from being created twice.
		if (value is ThreeObject mirroredValue)
		{
			mirroredValue.AttachTo(batch);
		}

		batch.Set(Handle, member, encodedValue);
	}

	/// <summary>
	/// Invokes a method three.js has and this mirror does not, by its three.js name. The escape hatch
	/// for commands, recording the same <c>Call</c> op a generated method records — including the
	/// coalescing barrier that stops a later write from being folded into one this call may have
	/// observed.
	/// <para>
	/// Invoked before this object is attached, the call is held and replayed by <see cref="AttachTo"/>,
	/// exactly as a generated command is.
	/// </para>
	/// </summary>
	/// <param name="member">Name of the three.js method to invoke, e.g. <c>"play"</c>.</param>
	/// <param name="args">Positional arguments, under the same encoding rules as <see cref="Set"/>.</param>
	/// <exception cref="NotSupportedException">Thrown for an argument with no wire encoding.</exception>
	public void Call(string member, params object?[] args)
	{
		RecordCall(member, args);
	}

	/// <summary>
	/// Invokes a method three.js has and this mirror does not, and hands its return value back. The
	/// escape hatch for queries: same op, same batch and same correlation as a generated
	/// <c>…Async</c> method.
	/// </summary>
	/// <typeparam name="TValue">C# type the caller declares the method returns.</typeparam>
	/// <param name="member">Name of the three.js method to invoke.</param>
	/// <param name="args">Positional arguments, under the same encoding rules as <see cref="Set"/>.</param>
	/// <returns>The value three.js returned, decoded into <typeparamref name="TValue"/>.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when this object is not attached to a <see cref="ThreeContext"/>, when the applier
	/// rejected the call, or when what came back cannot be held as <typeparamref name="TValue"/>.
	/// </exception>
	public Task<TValue> CallAsync<TValue>(string member, params object?[] args)
	{
		return RecordRead<TValue>(member, args);
	}

	/// <summary>
	/// Reads a property off the three.js object, by its three.js name. The escape hatch for reading
	/// state back, and the only member here with no generated counterpart: the read op invokes a
	/// method, so three.js's read-only properties are on no generated class at all, and this is what
	/// reaches them.
	/// <para>
	/// Like every read it travels inside the batch, behind the writes already pending, so it observes
	/// them. Unlike a write it cannot be held for replay — an unattached object has no JavaScript side
	/// to ask and no value to hand back now — so it fails at the call site instead.
	/// </para>
	/// <para>
	/// ⚠️ Only values come back: numbers, booleans, strings, and the five hand-written math types. A
	/// property holding a three.js object is refused by the applier rather than serialized, because a
	/// plain JSON object would arrive here as a plausible bag of numbers instead of a value.
	/// </para>
	/// </summary>
	/// <typeparam name="TValue">C# type the caller declares the property holds.</typeparam>
	/// <param name="member">Name of the three.js property to read, e.g. <c>"fov"</c>.</param>
	/// <returns>The value the property held, decoded into <typeparamref name="TValue"/>.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when this object is not attached to a <see cref="ThreeContext"/>, when the object has no
	/// such property, or when its value cannot be held as <typeparamref name="TValue"/> — which faults
	/// rather than answering with a default the browser never sent.
	/// </exception>
	public Task<TValue> GetAsync<TValue>(string member)
	{
		return RequireContext(member).GetAsync<TValue>(Handle, member);
	}

	/// <summary>
	/// Records a property write on this object into <see cref="Batch"/>, encoding the value first.
	/// A no-op while <see cref="Batch"/> is unset, because the class that owns the property replays it
	/// from its own field on attach — which is what <see cref="Set"/>, having no field behind it,
	/// cannot rely on.
	/// </summary>
	/// <param name="member">Name of the property being written.</param>
	/// <param name="value">The new value.</param>
	protected void RecordSet(string member, object? value)
	{
		Batch?.Set(Handle, member, EncodeOrExplain(member, value));
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
			.Select(x => EncodeOrExplain(member, x))
			.ToArray();

		if (Batch is null)
		{
			_pendingOps ??= [];
			_pendingOps.Add(new PendingOp
			{
				Kind = ThreeOpKind.Call,
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
		var context = RequireContext(member);
		AttachMirroredArguments(context.Batch, args);
		var encodedArgs = args
			.Select(x => EncodeOrExplain(member, x))
			.ToArray();

		return context.ReadAsync<TValue>(Handle, member, encodedArgs);
	}

	/// <summary>
	/// The context this object records into, or the failure explaining why there is none. Shared by
	/// every member that has to answer with a value, because all of them fail the same way: a held
	/// write eventually happens, whereas a held read has nothing to hand back now.
	/// </summary>
	/// <param name="member">Member being read, named in the failure.</param>
	/// <returns>The context this object is attached to.</returns>
	/// <exception cref="InvalidOperationException">Thrown when this object is not attached to one.</exception>
	private ThreeContext RequireContext(string member)
	{
		if (Batch?.Context is not { } context)
		{
			throw new InvalidOperationException(
				$"'{GetType().Name}' (handle {Handle}) is not attached to a {nameof(ThreeContext)}, so '{member}' has nothing to read from. " +
				$"A property written before an attach is replayed once it happens, but a read cannot be: there is no JavaScript object to ask, " +
				$"and no value to hand back now. Attach the object graph to a context first.");
		}

		return context;
	}

	/// <summary>
	/// Encodes one value for the wire, naming what carried it when the encoder refuses. The refusal
	/// itself is <see cref="ThreeValue.Encode"/>'s and stays exactly as loud; its message is written for
	/// whoever extends the encoder, and a caller who reached it by passing the wrong thing to
	/// <see cref="Set"/> or <see cref="Call"/> needs to know which member of which object instead.
	/// </summary>
	/// <param name="member">Member the value was being written to, invoked on, or passed to.</param>
	/// <param name="value">The value to encode.</param>
	/// <returns>The wire-ready representation.</returns>
	/// <exception cref="NotSupportedException">Thrown for a reference type the encoder has no arm for.</exception>
	private object? EncodeOrExplain(string member, object? value)
	{
		try
		{
			return ThreeValue.Encode(value);
		}
		catch (NotSupportedException exception)
		{
			throw new NotSupportedException(
				$"'{GetType().Name}' (handle {Handle}) cannot send a '{value?.GetType().FullName}' value as '{member}'. {exception.Message}",
				exception);
		}
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
			.Select((x, index) => EncodeOrExplain($"constructor argument {index}", x))
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
	/// Records every op held while this object had no batch, in invocation order, and releases the
	/// queue. Only ever reached once per object: <see cref="AttachTo"/> returns early on an object that
	/// already has a batch, and every later op records straight into it.
	/// <para>
	/// The queue holds writes and calls interleaved, and replaying them in one pass is what keeps their
	/// relative order — a raw write made after a call must still land after it, since the call may have
	/// been what produced the value being corrected.
	/// </para>
	/// </summary>
	/// <param name="batch">Batch to record the held ops into.</param>
	private void ReplayPendingOps(ThreeBatch batch)
	{
		if (_pendingOps is null)
		{
			return;
		}

		foreach (var pendingOp in _pendingOps)
		{
			AttachMirroredArguments(batch, pendingOp.Arguments);
			if (pendingOp.Kind == ThreeOpKind.Set)
			{
				batch.Set(Handle, pendingOp.Member, pendingOp.EncodedArguments.First());
				continue;
			}

			batch.Call(Handle, pendingOp.Member, pendingOp.EncodedArguments);
		}

		_pendingOps = null;
	}

	/// <summary>
	/// Attaches every argument that is itself a mirrored object. A <c>$ref</c> to a handle the applier
	/// has never seen is an unknown-handle failure in the browser, and an argument passed to a command
	/// invoked before this object reached a batch has not been attached by anything else. Attaching
	/// rather than emitting the create op directly is what keeps a shared instance from being created
	/// twice.
	/// </summary>
	/// <param name="batch">Batch to attach the arguments to.</param>
	/// <param name="args">The op's positional arguments, or its single value, unencoded.</param>
	private protected static void AttachMirroredArguments(ThreeBatch batch, object?[] args)
	{
		foreach (var arg in args)
		{
			if (arg is ThreeObject mirroredArgument)
			{
				mirroredArgument.AttachTo(batch);
			}
		}
	}

	/// <summary>An op recorded before its object was attached, held until the attach can replay it.</summary>
	private sealed class PendingOp
	{
		/// <summary>
		/// Whether this is a property write or a method invocation. Only
		/// <see cref="ThreeOpKind.Set"/> and <see cref="ThreeOpKind.Call"/> are ever held: those are the
		/// two kinds whose effect can still be delivered later.
		/// </summary>
		public required ThreeOpKind Kind { get; init; }

		/// <summary>Name of the three.js property to write or method to invoke.</summary>
		public required string Member { get; init; }

		/// <summary>
		/// The arguments as the caller passed them — the single value for a write — kept so the mirrored
		/// ones can be attached on replay.
		/// </summary>
		public required object?[] Arguments { get; init; }

		/// <summary>The same values in wire form, encoded at invocation time.</summary>
		public required object?[] EncodedArguments { get; init; }
	}
}
