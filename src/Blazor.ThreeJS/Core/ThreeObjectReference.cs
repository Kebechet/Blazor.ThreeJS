using System.Text.Json.Serialization;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// What the applier answers with when a read or a get was asked for a handle rather than a value:
/// the handle the object is registered under, and three.js's own name for it.
/// <para>
/// The same <c>$ref</c> shape C# sends in the other direction, with the type name added. It
/// deserializes through the ordinary reader rather than a tagged-value arm, because it carries no
/// <see cref="ThreeWireFormat.TagKey"/> — a plain object with known property names is exactly what
/// <see cref="System.Text.Json"/> is for.
/// </para>
/// </summary>
internal sealed class ThreeObjectReference
{
	/// <summary>Handle the object is registered under, negative because the browser made it.</summary>
	[JsonPropertyName(ThreeWireFormat.HandleReferenceKey)]
	public required int Handle { get; init; }

	/// <summary>
	/// three.js's own <c>constructor.name</c>. Reported rather than assumed: a declared return type is
	/// often a base, and what actually comes back may be a subclass.
	/// </summary>
	[JsonPropertyName("t")]
	public string? ThreeTypeName { get; init; }
}
