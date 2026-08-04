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
public sealed class ThreeOp
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

	/// <summary>Name of the property or method this op writes or invokes. Set for <see cref="ThreeOpKind.Set"/> and <see cref="ThreeOpKind.Call"/>, and omitted from the payload otherwise.</summary>
	[JsonPropertyName("m")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Member { get; init; }

	/// <summary>Positional arguments. Set for <see cref="ThreeOpKind.Create"/> and <see cref="ThreeOpKind.Call"/>, and omitted from the payload otherwise.</summary>
	[JsonPropertyName("a")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public object?[]? Args { get; init; }

	/// <summary>
	/// The value being written. Only set for <see cref="ThreeOpKind.Set"/>.
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
}

/// <summary>
/// Discriminates what a <see cref="ThreeOp"/> instructs the JavaScript applier to do. The numeric
/// values are part of the wire contract with <c>three-interop.js</c> and must not be renumbered.
/// </summary>
public enum ThreeOpKind : byte
{
	/// <summary>Instantiate a new three.js object and register it under a handle.</summary>
	Create = 0,

	/// <summary>Write a property on an existing object. Coalesces per (handle, member) within a batch, unless a Call or Dispose on the same handle was recorded since the last Set on that handle.</summary>
	Set = 1,

	/// <summary>Invoke a method on an existing object. Never coalesces, and acts as a barrier that stops a later Set on the same handle from coalescing into an earlier one.</summary>
	Call = 2,

	/// <summary>Attach a child object to a parent object.</summary>
	Add = 3,

	/// <summary>Detach a child object from a parent object.</summary>
	Remove = 4,

	/// <summary>Release an object and its JavaScript-side resources.</summary>
	Dispose = 5
}
