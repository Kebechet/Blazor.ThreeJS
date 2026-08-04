using System.Text.Json.Serialization;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// A single op rejected by the JavaScript applier. Property names match the object literal
/// pushed by <c>three-interop.js</c>.
/// </summary>
public sealed class ThreeError
{
	/// <summary>Handle of the object the failing op targeted.</summary>
	[JsonPropertyName("handle")]
	public int Handle { get; init; }

	/// <summary>
	/// Name of the property or method the op targeted, or <see langword="null"/> for an Add, Remove,
	/// or Dispose op, since those target no member.
	/// </summary>
	[JsonPropertyName("member")]
	public string? Member { get; init; }

	/// <summary>Message describing why the applier rejected the op.</summary>
	[JsonPropertyName("message")]
	public required string Message { get; init; }
}
