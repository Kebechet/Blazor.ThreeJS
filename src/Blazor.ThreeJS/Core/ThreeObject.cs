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
		Batch?.Call(Handle, member, args.Select(ThreeValue.Encode).ToArray());
	}

	/// <summary>
	/// Emits the create op plus every property this object currently holds. Called on attach, and
	/// again after a WebGL context loss to rebuild the scene from the C# mirror.
	/// </summary>
	/// <param name="batch">Batch to record the create op into.</param>
	internal virtual void EmitCreate(ThreeBatch batch)
	{
		batch.Create(Handle, ThreeTypeName, ConstructorArgs.Select(ThreeValue.Encode).ToArray());
	}

	/// <summary>Positional arguments to pass to the three.js constructor when this object is created.</summary>
	protected virtual object?[] ConstructorArgs
	{
		get { return []; }
	}
}
