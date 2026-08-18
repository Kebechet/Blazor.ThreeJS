using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kebechet.Blazor.ThreeJS.Addons;

/// <summary>
/// What <c>loadGltf</c> hands back after the browser has parsed a model. A fixed wire format shared
/// with <c>three-interop.js</c>: the short property names here and on <see cref="GLTFNodeDescription"/>
/// and <see cref="GLTFClipDescription"/> must change together on both sides.
/// </summary>
internal sealed class GLTFLoadResponse
{
	/// <summary>
	/// One row per mirrored node, the loaded root first. Every later row is a named descendant of it,
	/// in traversal order.
	/// </summary>
	[JsonPropertyName("n")]
	public List<GLTFNodeDescription> Nodes { get; init; } = [];

	/// <summary>
	/// One row per animation clip the file brought along, minted after every node handle. Absent from
	/// an old browser's cached response - a file with no <c>a</c> key deserializes to an empty list
	/// rather than a missing one, which is what keeps a stale cache reading as "no clips" instead of a
	/// deserialization failure.
	/// </summary>
	[JsonPropertyName("a")]
	public List<GLTFClipDescription> Animations { get; init; } = [];
}

/// <summary>
/// One animation clip of a loaded graph, as the browser found it: the handle it minted, and the name
/// and duration three.js reported for the clip.
/// </summary>
internal sealed class GLTFClipDescription
{
	/// <summary>Negative handle the JavaScript side registered this clip under.</summary>
	[JsonPropertyName("h")]
	public int Handle { get; init; }

	/// <summary>The clip's name, as three.js built it from the glTF animation.</summary>
	[JsonPropertyName("n")]
	public required string Name { get; init; }

	/// <summary>The clip's duration in seconds.</summary>
	[JsonPropertyName("d")]
	public float Duration { get; init; }
}

/// <summary>
/// One node of a loaded graph, as the browser found it: the handle it minted, what three.js called
/// the object, and the transform the loader gave it.
/// <para>
/// The transform travels in the same <c>$t</c>-tagged form a read op returns, rather than as three
/// bare arrays, so exactly one codec settles what a vector or an Euler looks like on the wire.
/// </para>
/// </summary>
internal sealed class GLTFNodeDescription
{
	/// <summary>Negative handle the JavaScript side registered this node under.</summary>
	[JsonPropertyName("h")]
	public int Handle { get; init; }

	/// <summary>The node's glTF name. Empty only for the loaded root, which is the one row not selected by name.</summary>
	[JsonPropertyName("n")]
	public required string Name { get; init; }

	/// <summary>three.js's own <c>type</c> for the object it built, e.g. <c>Mesh</c>, <c>Group</c>, <c>Bone</c>.</summary>
	[JsonPropertyName("t")]
	public required string Type { get; init; }

	/// <summary>The node's local position, tagged as a <c>Vector3</c>.</summary>
	[JsonPropertyName("p")]
	public JsonElement? Position { get; init; }

	/// <summary>The node's local rotation, tagged as an <c>Euler</c>.</summary>
	[JsonPropertyName("r")]
	public JsonElement? Rotation { get; init; }

	/// <summary>The node's local scale, tagged as a <c>Vector3</c>.</summary>
	[JsonPropertyName("s")]
	public JsonElement? Scale { get; init; }

	/// <summary>Whether the loader left the node visible.</summary>
	[JsonPropertyName("v")]
	public bool IsVisible { get; init; }
}
