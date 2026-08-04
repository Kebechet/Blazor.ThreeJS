using System.Text.Json.Serialization;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// A single instruction in the batch sent to the JavaScript applier. This is a fixed wire format
/// shared by <c>ThreeBatch</c> and <c>three-interop.js</c>; the numeric values of
/// <see cref="ThreeOpKind"/> and the JSON property names below must change together on both sides.
/// </summary>
public sealed class ThreeOp
{
	/// <summary>The kind of instruction, serialized as the short numeric property <c>"k"</c>.</summary>
	[JsonPropertyName("k")]
	public required ThreeOpKind Kind { get; init; }

	/// <summary>Handle of the object this op targets.</summary>
	[JsonPropertyName("h")]
	public int Handle { get; init; }

	/// <summary>Name of the three.js type to instantiate. Only set for <see cref="ThreeOpKind.Create"/>.</summary>
	[JsonPropertyName("t")]
	public string? Type { get; init; }

	/// <summary>Name of the property or method this op writes or invokes. Set for <see cref="ThreeOpKind.Set"/> and <see cref="ThreeOpKind.Call"/>.</summary>
	[JsonPropertyName("m")]
	public string? Member { get; init; }

	/// <summary>Positional arguments. Set for <see cref="ThreeOpKind.Create"/> and <see cref="ThreeOpKind.Call"/>.</summary>
	[JsonPropertyName("a")]
	public object?[]? Args { get; init; }

	/// <summary>The value being written. Only set for <see cref="ThreeOpKind.Set"/>.</summary>
	[JsonPropertyName("v")]
	public object? Value { get; init; }

	/// <summary>Handle of the child object. Set for <see cref="ThreeOpKind.Add"/> and <see cref="ThreeOpKind.Remove"/>.</summary>
	[JsonPropertyName("c")]
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
