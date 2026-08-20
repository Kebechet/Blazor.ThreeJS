// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>BufferAttributeJSON</c>. A plain value rather than a handle-backed
/// object: three.js declares it as a shape, and nothing on either side keeps a reference to one. It
/// travels as its own members, under three.js's names for them.
/// </summary>
public sealed record BufferAttributeJSON : IThreeStructure
{
	/// <summary>three.js's <c>itemSize</c>.</summary>
	public float ItemSize { get; init; }

	/// <summary>three.js's <c>type</c>.</summary>
	public string Type { get; init; }

	/// <summary>three.js's <c>array</c>.</summary>
	public float[] Array { get; init; }

	/// <summary>three.js's <c>normalized</c>.</summary>
	public bool Normalized { get; init; }

	/// <summary>three.js's <c>name</c>.</summary>
	public string? Name { get; init; }

	/// <summary>three.js's <c>usage</c>.</summary>
	public Usage? Usage { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["itemSize"] = ItemSize;
		members["type"] = Type;
		members["array"] = Array;
		members["normalized"] = Normalized;

		if (Name is not null)
		{
			members["name"] = Name;
		}

		if (Usage is not null)
		{
			members["usage"] = Usage;
		}

		return members;
	}

	/// <summary>
	/// Builds a <c>BufferAttributeJSON</c> from the members the applier sent back. A member three.js
	/// did not carry keeps this instance's own value, which for the blank instance the decoder builds
	/// is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members)
	{
		return new BufferAttributeJSON
		{
			ItemSize = members.TryGetValue("itemSize", out var itemSizeElement) ? ThreeValue.Decode<float>(itemSizeElement) : ItemSize,
			Type = members.TryGetValue("type", out var typeElement) ? ThreeValue.Decode<string>(typeElement) : Type,
			Array = members.TryGetValue("array", out var arrayElement) ? ThreeValue.Decode<float[]>(arrayElement) : Array,
			Normalized = members.TryGetValue("normalized", out var normalizedElement) ? ThreeValue.Decode<bool>(normalizedElement) : Normalized,
			Name = members.TryGetValue("name", out var nameElement) ? ThreeValue.Decode<string?>(nameElement) : Name,
			Usage = members.TryGetValue("usage", out var usageElement) ? ThreeValue.Decode<Usage?>(usageElement) : Usage
		};
	}
}
