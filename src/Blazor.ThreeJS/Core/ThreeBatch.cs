namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Accumulates pending operations and hands them over in one drain. Property writes coalesce so a
/// burst of assignments in a single tick costs one op, which is what keeps a scene affordable over
/// a Blazor Server circuit.
/// </summary>
internal sealed class ThreeBatch
{
	private readonly List<ThreeOp> _ops = [];
	private readonly Dictionary<(int Handle, string Member), int> _setIndexesByTarget = [];

	/// <summary>Whether any op is currently pending a <see cref="Drain"/>.</summary>
	public bool HasPendingOps
	{
		get { return _ops.Any(); }
	}

	/// <summary>
	/// Records an instantiation of a new three.js object under <paramref name="handle"/>. Also acts as
	/// a coalescing barrier for <b>every</b> handle, not just this one: a create invalidates every
	/// pending <c>Set</c> slot, because rewriting one afterwards could point it at a handle created
	/// later in the batch, and the applier rejects such an op with an unknown-handle error.
	/// <para>
	/// This costs nothing in steady state — an animation batch contains no create ops at all, so
	/// per-frame coalescing is untouched. Only a batch that mixes creates with property writes gives
	/// up some coalescing, and it gives it up for at most one extra <c>Set</c> op.
	/// </para>
	/// </summary>
	/// <param name="handle">Handle the created object is registered under.</param>
	/// <param name="type">Name of the three.js type to instantiate.</param>
	/// <param name="args">Positional constructor arguments.</param>
	public void Create(int handle, string type, object?[] args)
	{
		_setIndexesByTarget.Clear();
		_ops.Add(new ThreeOp
		{
			Kind = ThreeOpKind.Create,
			Handle = handle,
			Type = type,
			Args = args
		});
	}

	/// <summary>
	/// Records a property write on <paramref name="handle"/>. A second call for the same
	/// (<paramref name="handle"/>, <paramref name="member"/>) pair replaces the earlier op in
	/// place instead of appending, so repeated writes in one tick still cost a single op.
	/// Coalescing only holds within a run of writes: recording a <see cref="Call"/> or
	/// <see cref="Dispose"/> on this handle acts as a barrier, because a call can observe the
	/// object's property state at the point it runs — a <c>Set</c> recorded afterward always
	/// appends a new op instead of overwriting a value the call may already have read. A
	/// <see cref="Create"/> is a barrier too, for every handle at once, so a rewritten value can
	/// never reference an object created later in the batch.
	/// </summary>
	/// <param name="handle">Handle of the object to write to.</param>
	/// <param name="member">Name of the property being written.</param>
	/// <param name="value">The value to write.</param>
	/// <exception cref="InvalidOperationException">
	/// Thrown when <paramref name="value"/> is <see cref="ThreeValue.Unspecified"/>. See the remarks
	/// on why that is a defect rather than a value.
	/// </exception>
	public void Set(int handle, string member, object? value)
	{
		// The not-supplied sentinel is constructor-arguments-only. It says "the caller did not pass
		// this", which a constructor can act on by letting three.js apply its own parameter default;
		// a property write cannot, because assigning `undefined` to a property of a retained object
		// means setting it to undefined, not leaving it alone. A generated class that has nothing to
		// say about a property simply does not write it. Reaching here is a generator defect, and it
		// fails loudly rather than shipping `undefined` into a live three.js instance.
		if (ReferenceEquals(value, ThreeValue.Unspecified))
		{
			throw new InvalidOperationException(
				$"The '{nameof(ThreeValue)}.{nameof(ThreeValue.Unspecified)}' sentinel reached a {nameof(ThreeOpKind.Set)} op for member '{member}' on handle {handle}. " +
				$"It is only meaningful in constructor arguments, where it lets three.js apply its own parameter default. " +
				$"A property the mirror has no value for is left unwritten instead.");
		}

		var target = (handle, member);
		if (_setIndexesByTarget.TryGetValue(target, out var existingIndex))
		{
			_ops[existingIndex] = new ThreeOp
			{
				Kind = ThreeOpKind.Set,
				Handle = handle,
				Member = member,
				Value = value
			};
			return;
		}

		_setIndexesByTarget[target] = _ops.Count;
		_ops.Add(new ThreeOp
		{
			Kind = ThreeOpKind.Set,
			Handle = handle,
			Member = member,
			Value = value
		});
	}

	/// <summary>
	/// Records a method invocation on <paramref name="handle"/>. Unlike <see cref="Set"/>, calls are
	/// never coalesced — two identical calls are two ops, since a method call is not idempotent.
	/// Also acts as a coalescing barrier for this handle: it leaves any <c>Set</c> op already
	/// recorded untouched, but drops the coalescing entry so a <c>Set</c> recorded afterward on the
	/// same handle appends a new op instead of overwriting a value this call may have observed.
	/// </summary>
	/// <param name="handle">Handle of the object to invoke the method on.</param>
	/// <param name="member">Name of the method to invoke.</param>
	/// <param name="args">Positional arguments to pass to the method.</param>
	public void Call(int handle, string member, object?[] args)
	{
		InvalidateSetCoalescing(handle);
		_ops.Add(new ThreeOp
		{
			Kind = ThreeOpKind.Call,
			Handle = handle,
			Member = member,
			Args = args
		});
	}

	/// <summary>
	/// Records attaching a child object to a parent object.
	/// </summary>
	/// <param name="parentHandle">Handle of the parent object.</param>
	/// <param name="childHandle">Handle of the child object to attach.</param>
	public void Add(int parentHandle, int childHandle)
	{
		_ops.Add(new ThreeOp
		{
			Kind = ThreeOpKind.Add,
			Handle = parentHandle,
			ChildHandle = childHandle
		});
	}

	/// <summary>
	/// Records detaching a child object from a parent object.
	/// </summary>
	/// <param name="parentHandle">Handle of the parent object.</param>
	/// <param name="childHandle">Handle of the child object to detach.</param>
	public void Remove(int parentHandle, int childHandle)
	{
		_ops.Add(new ThreeOp
		{
			Kind = ThreeOpKind.Remove,
			Handle = parentHandle,
			ChildHandle = childHandle
		});
	}

	/// <summary>
	/// Records releasing an object and its JavaScript-side resources. Also acts as a coalescing
	/// barrier for this handle, for the same reason as <see cref="Call"/>: coalescing a later
	/// <c>Set</c> back into a pre-dispose op would be meaningless once the object is gone.
	/// </summary>
	/// <param name="handle">Handle of the object to dispose.</param>
	public void Dispose(int handle)
	{
		InvalidateSetCoalescing(handle);
		_ops.Add(new ThreeOp
		{
			Kind = ThreeOpKind.Dispose,
			Handle = handle
		});
	}

	/// <summary>
	/// Returns all pending ops in recording order and clears the batch, including the coalescing
	/// index, so a second drain with no intervening writes returns an empty list.
	/// </summary>
	/// <returns>The ops that were pending before this call.</returns>
	public IReadOnlyList<ThreeOp> Drain()
	{
		var drained = _ops.ToList();
		_ops.Clear();
		_setIndexesByTarget.Clear();
		return drained;
	}

	/// <summary>
	/// Drops any pending <c>Set</c> coalescing entries for <paramref name="handle"/>. Does not
	/// touch already-recorded ops — only stops a future <c>Set</c> on this handle from overwriting
	/// one of them.
	/// </summary>
	/// <param name="handle">Handle whose coalescing entries should be invalidated.</param>
	private void InvalidateSetCoalescing(int handle)
	{
		var staleTargets = _setIndexesByTarget.Keys
			.Where(x => x.Handle == handle)
			.ToList();

		foreach (var staleTarget in staleTargets)
		{
			_setIndexesByTarget.Remove(staleTarget);
		}
	}
}
