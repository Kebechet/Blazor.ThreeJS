namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Accumulates pending operations and hands them over in one drain. Property writes coalesce so a
/// burst of assignments in a single tick costs one op, which is what keeps a scene affordable over
/// a Blazor Server circuit.
/// </summary>
public sealed class ThreeBatch
{
	private readonly List<ThreeOp> _ops = [];
	private readonly Dictionary<(int Handle, string Member), int> _setIndexesByTarget = [];

	/// <summary>Whether any op is currently pending a <see cref="Drain"/>.</summary>
	public bool HasPendingOps
	{
		get { return _ops.Any(); }
	}

	/// <summary>
	/// Records an instantiation of a new three.js object under <paramref name="handle"/>.
	/// </summary>
	/// <param name="handle">Handle the created object is registered under.</param>
	/// <param name="type">Name of the three.js type to instantiate.</param>
	/// <param name="args">Positional constructor arguments.</param>
	public void Create(int handle, string type, object?[] args)
	{
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
	/// </summary>
	/// <param name="handle">Handle of the object to write to.</param>
	/// <param name="member">Name of the property being written.</param>
	/// <param name="value">The value to write.</param>
	public void Set(int handle, string member, object? value)
	{
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
	/// </summary>
	/// <param name="handle">Handle of the object to invoke the method on.</param>
	/// <param name="member">Name of the method to invoke.</param>
	/// <param name="args">Positional arguments to pass to the method.</param>
	public void Call(int handle, string member, object?[] args)
	{
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
	/// Records releasing an object and its JavaScript-side resources.
	/// </summary>
	/// <param name="handle">Handle of the object to dispose.</param>
	public void Dispose(int handle)
	{
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
}
