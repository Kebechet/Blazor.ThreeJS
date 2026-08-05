using System.Text.Json.Serialization;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// A single instruction in the batch sent to the JavaScript applier. This is a fixed wire format
/// shared by <c>ThreeBatch</c> and <c>three-interop.js</c>; the numeric values of
/// <see cref="ThreeOpKind"/>, the JSON property names below, and which of them may be omitted must
/// change together on both sides.
/// <para>
/// Every op carries only the fields its kind actually uses. The properties an unrelated kind leaves
/// unset are omitted rather than written as <c>null</c> or <c>0</c>, because the padding was roughly
/// 40% of a batch's bytes and every one of those bytes crosses a Blazor Server circuit. The applier
/// already reads the omitted ones defensively (<c>op.a ?? []</c>, <c>op.m ?? op.t</c>), so absence
/// and the previous explicit null are equivalent to it.
/// </para>
/// </summary>
internal sealed class ThreeOp
{
	/// <summary>The kind of instruction, serialized as the short numeric property <c>"k"</c>.</summary>
	[JsonPropertyName("k")]
	public required ThreeOpKind Kind { get; init; }

	/// <summary>Handle of the object this op targets. Always serialized: every kind targets a handle.</summary>
	[JsonPropertyName("h")]
	public int Handle { get; init; }

	/// <summary>Name of the three.js type to instantiate. Only set for <see cref="ThreeOpKind.Create"/>, and omitted from the payload otherwise.</summary>
	[JsonPropertyName("t")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Type { get; init; }

	/// <summary>Name of the property or method this op writes, invokes or reads. Set for <see cref="ThreeOpKind.Set"/>, <see cref="ThreeOpKind.Call"/>, <see cref="ThreeOpKind.Read"/> and <see cref="ThreeOpKind.Get"/>, and omitted from the payload otherwise.</summary>
	[JsonPropertyName("m")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Member { get; init; }

	/// <summary>Positional arguments. Set for <see cref="ThreeOpKind.Create"/> and <see cref="ThreeOpKind.Call"/>, and omitted from the payload otherwise.</summary>
	[JsonPropertyName("a")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public object?[]? Args { get; init; }

	/// <summary>
	/// The value being written, or — for <see cref="ThreeOpKind.Pick"/> — whether the object is opting
	/// into pointer hit-testing or out of it. Set for those two kinds.
	/// <para>
	/// Always serialized, including when it is <see langword="null"/>. Writing <c>null</c> over a
	/// property is a legitimate instruction, and omitting the key would make it indistinguishable
	/// from <c>undefined</c> on the JavaScript side.
	/// </para>
	/// </summary>
	[JsonPropertyName("v")]
	public object? Value { get; init; }

	/// <summary>Handle of the child object. Set for <see cref="ThreeOpKind.Add"/> and <see cref="ThreeOpKind.Remove"/>, and omitted when zero — handles are allocated from 1 upwards, so 0 is never a real handle.</summary>
	[JsonPropertyName("c")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public int ChildHandle { get; init; }

	/// <summary>
	/// Identifies which pending read a returned value answers. Only set for the two kinds that produce
	/// one, <see cref="ThreeOpKind.Read"/> and <see cref="ThreeOpKind.Get"/>, and omitted when zero —
	/// request ids are allocated from 1 upwards, so 0 is never a real one.
	/// <para>
	/// The applier echoes it back on the result row rather than relying on position, so a batch
	/// carrying several reads still matches each value to the request that asked for it, and a
	/// response missing a row is detectable instead of silently answering with the wrong value.
	/// </para>
	/// </summary>
	[JsonPropertyName("i")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public int RequestId { get; init; }
}

/// <summary>
/// Discriminates what a <see cref="ThreeOp"/> instructs the JavaScript applier to do. The numeric
/// values are part of the wire contract with <c>three-interop.js</c> and must not be renumbered.
/// </summary>
internal enum ThreeOpKind : byte
{
	/// <summary>Instantiate a new three.js object and register it under a handle. Also acts as a barrier that stops any later Set from coalescing into one recorded before it, on any handle.</summary>
	Create = 0,

	/// <summary>Write a property on an existing object. Coalesces per (handle, member) within a batch, unless a Call or Dispose on the same handle, or a Create on any handle, was recorded since the last Set on that handle.</summary>
	Set = 1,

	/// <summary>Invoke a method on an existing object. Never coalesces, and acts as a barrier that stops a later Set on the same handle from coalescing into an earlier one.</summary>
	Call = 2,

	/// <summary>Attach a child object to a parent object.</summary>
	Add = 3,

	/// <summary>Detach a child object from a parent object.</summary>
	Remove = 4,

	/// <summary>Release an object and its JavaScript-side resources.</summary>
	Dispose = 5,

	/// <summary>
	/// Invoke a method on an existing object and send its return value back. The only op that produces
	/// a value; every other kind is one-directional. Never coalesces, and acts as the same barrier a
	/// <see cref="Call"/> does, since a read observes the object's property state at the point it runs.
	/// </summary>
	Read = 6,

	/// <summary>
	/// Opt an object into JavaScript-side pointer hit-testing, or back out of it, carried in
	/// <see cref="ThreeOp.Value"/>. The only op that makes the browser send anything C# did not ask
	/// for: an opted-in object that a click's ray meets produces a callback with no request behind it.
	/// <para>
	/// Touches no three.js state — it adds the object to, or removes it from, the applier's own set of
	/// hit-test candidates — so unlike <see cref="Call"/> it is not a coalescing barrier.
	/// </para>
	/// </summary>
	Pick = 7,

	/// <summary>
	/// Read a property off an existing object and send its value back. The second op that produces a
	/// value, and the mirror image of <see cref="Set"/> — where <see cref="Read"/> invokes a method,
	/// this one reads a member, which is what puts three.js's read-only properties within reach.
	/// <para>
	/// A kind of its own rather than a relaxation of <see cref="Read"/>: the applier rejects a
	/// <see cref="Read"/> whose member is not a function, and letting it fall back to a property read
	/// would turn a mistyped method name into a silent <see langword="default"/> instead of an error.
	/// Never coalesces, and acts as the same barrier a <see cref="Read"/> does.
	/// </para>
	/// </summary>
	Get = 8
}
