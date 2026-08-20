// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>BatchedMeshGeometryRange</c>. A plain value rather than a
/// handle-backed object: three.js declares it as a shape, and nothing on either side keeps a
/// reference to one. It travels as its own members, under three.js's names for them.
/// </summary>
public sealed record BatchedMeshGeometryRange : IThreeStructure
{
	/// <summary>three.js's <c>vertexStart</c>.</summary>
	public float VertexStart { get; init; }

	/// <summary>three.js's <c>vertexCount</c>.</summary>
	public int VertexCount { get; init; }

	/// <summary>three.js's <c>reservedVertexCount</c>.</summary>
	public int ReservedVertexCount { get; init; }

	/// <summary>three.js's <c>indexStart</c>.</summary>
	public float IndexStart { get; init; }

	/// <summary>three.js's <c>indexCount</c>.</summary>
	public int IndexCount { get; init; }

	/// <summary>three.js's <c>reservedIndexCount</c>.</summary>
	public int ReservedIndexCount { get; init; }

	/// <summary>three.js's <c>start</c>.</summary>
	public float Start { get; init; }

	/// <summary>three.js's <c>count</c>.</summary>
	public int Count { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["vertexStart"] = VertexStart;
		members["vertexCount"] = VertexCount;
		members["reservedVertexCount"] = ReservedVertexCount;
		members["indexStart"] = IndexStart;
		members["indexCount"] = IndexCount;
		members["reservedIndexCount"] = ReservedIndexCount;
		members["start"] = Start;
		members["count"] = Count;

		return members;
	}

	/// <summary>
	/// Builds a <c>BatchedMeshGeometryRange</c> from the members the applier sent back. A member
	/// three.js did not carry keeps this instance's own value, which for the blank instance the decoder
	/// builds is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new BatchedMeshGeometryRange
		{
			VertexStart = members.TryGetValue("vertexStart", out var vertexStartElement) ? ThreeValue.Decode<float>(vertexStartElement, context) : VertexStart,
			VertexCount = members.TryGetValue("vertexCount", out var vertexCountElement) ? ThreeValue.Decode<int>(vertexCountElement, context) : VertexCount,
			ReservedVertexCount = members.TryGetValue("reservedVertexCount", out var reservedVertexCountElement) ? ThreeValue.Decode<int>(reservedVertexCountElement, context) : ReservedVertexCount,
			IndexStart = members.TryGetValue("indexStart", out var indexStartElement) ? ThreeValue.Decode<float>(indexStartElement, context) : IndexStart,
			IndexCount = members.TryGetValue("indexCount", out var indexCountElement) ? ThreeValue.Decode<int>(indexCountElement, context) : IndexCount,
			ReservedIndexCount = members.TryGetValue("reservedIndexCount", out var reservedIndexCountElement) ? ThreeValue.Decode<int>(reservedIndexCountElement, context) : ReservedIndexCount,
			Start = members.TryGetValue("start", out var startElement) ? ThreeValue.Decode<float>(startElement, context) : Start,
			Count = members.TryGetValue("count", out var countElement) ? ThreeValue.Decode<int>(countElement, context) : Count
		};
	}
}
