// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>InfoMemory</c>. A plain value rather than a handle-backed object:
/// three.js declares it as a shape, and nothing on either side keeps a reference to one. It travels
/// as its own members, under three.js's names for them.
/// </summary>
public sealed record InfoMemory : IThreeStructure
{
	/// <summary>three.js's <c>attributes</c>.</summary>
	public float Attributes { get; init; }

	/// <summary>three.js's <c>attributesSize</c>.</summary>
	public float AttributesSize { get; init; }

	/// <summary>three.js's <c>geometries</c>.</summary>
	public float Geometries { get; init; }

	/// <summary>three.js's <c>indexAttributes</c>.</summary>
	public float IndexAttributes { get; init; }

	/// <summary>three.js's <c>indexAttributesSize</c>.</summary>
	public float IndexAttributesSize { get; init; }

	/// <summary>three.js's <c>indirectStorageAttributes</c>.</summary>
	public float IndirectStorageAttributes { get; init; }

	/// <summary>three.js's <c>indirectStorageAttributesSize</c>.</summary>
	public float IndirectStorageAttributesSize { get; init; }

	/// <summary>three.js's <c>programs</c>.</summary>
	public float Programs { get; init; }

	/// <summary>three.js's <c>programsSize</c>.</summary>
	public float ProgramsSize { get; init; }

	/// <summary>three.js's <c>readbackBuffers</c>.</summary>
	public float ReadbackBuffers { get; init; }

	/// <summary>three.js's <c>readbackBuffersSize</c>.</summary>
	public float ReadbackBuffersSize { get; init; }

	/// <summary>three.js's <c>renderTargets</c>.</summary>
	public float RenderTargets { get; init; }

	/// <summary>three.js's <c>storageAttributes</c>.</summary>
	public float StorageAttributes { get; init; }

	/// <summary>three.js's <c>storageAttributesSize</c>.</summary>
	public float StorageAttributesSize { get; init; }

	/// <summary>three.js's <c>textures</c>.</summary>
	public float Textures { get; init; }

	/// <summary>three.js's <c>texturesSize</c>.</summary>
	public float TexturesSize { get; init; }

	/// <summary>three.js's <c>uniformBuffers</c>.</summary>
	public float UniformBuffers { get; init; }

	/// <summary>three.js's <c>uniformBuffersSize</c>.</summary>
	public float UniformBuffersSize { get; init; }

	/// <summary>three.js's <c>total</c>.</summary>
	public float Total { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["attributes"] = Attributes;
		members["attributesSize"] = AttributesSize;
		members["geometries"] = Geometries;
		members["indexAttributes"] = IndexAttributes;
		members["indexAttributesSize"] = IndexAttributesSize;
		members["indirectStorageAttributes"] = IndirectStorageAttributes;
		members["indirectStorageAttributesSize"] = IndirectStorageAttributesSize;
		members["programs"] = Programs;
		members["programsSize"] = ProgramsSize;
		members["readbackBuffers"] = ReadbackBuffers;
		members["readbackBuffersSize"] = ReadbackBuffersSize;
		members["renderTargets"] = RenderTargets;
		members["storageAttributes"] = StorageAttributes;
		members["storageAttributesSize"] = StorageAttributesSize;
		members["textures"] = Textures;
		members["texturesSize"] = TexturesSize;
		members["uniformBuffers"] = UniformBuffers;
		members["uniformBuffersSize"] = UniformBuffersSize;
		members["total"] = Total;

		return members;
	}

	/// <summary>
	/// Builds a <c>InfoMemory</c> from the members the applier sent back. A member three.js did not
	/// carry keeps this instance's own value, which for the blank instance the decoder builds is the C#
	/// default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new InfoMemory
		{
			Attributes = members.TryGetValue("attributes", out var attributesElement) ? ThreeValue.Decode<float>(attributesElement, context) : Attributes,
			AttributesSize = members.TryGetValue("attributesSize", out var attributesSizeElement) ? ThreeValue.Decode<float>(attributesSizeElement, context) : AttributesSize,
			Geometries = members.TryGetValue("geometries", out var geometriesElement) ? ThreeValue.Decode<float>(geometriesElement, context) : Geometries,
			IndexAttributes = members.TryGetValue("indexAttributes", out var indexAttributesElement) ? ThreeValue.Decode<float>(indexAttributesElement, context) : IndexAttributes,
			IndexAttributesSize = members.TryGetValue("indexAttributesSize", out var indexAttributesSizeElement) ? ThreeValue.Decode<float>(indexAttributesSizeElement, context) : IndexAttributesSize,
			IndirectStorageAttributes = members.TryGetValue("indirectStorageAttributes", out var indirectStorageAttributesElement) ? ThreeValue.Decode<float>(indirectStorageAttributesElement, context) : IndirectStorageAttributes,
			IndirectStorageAttributesSize = members.TryGetValue("indirectStorageAttributesSize", out var indirectStorageAttributesSizeElement) ? ThreeValue.Decode<float>(indirectStorageAttributesSizeElement, context) : IndirectStorageAttributesSize,
			Programs = members.TryGetValue("programs", out var programsElement) ? ThreeValue.Decode<float>(programsElement, context) : Programs,
			ProgramsSize = members.TryGetValue("programsSize", out var programsSizeElement) ? ThreeValue.Decode<float>(programsSizeElement, context) : ProgramsSize,
			ReadbackBuffers = members.TryGetValue("readbackBuffers", out var readbackBuffersElement) ? ThreeValue.Decode<float>(readbackBuffersElement, context) : ReadbackBuffers,
			ReadbackBuffersSize = members.TryGetValue("readbackBuffersSize", out var readbackBuffersSizeElement) ? ThreeValue.Decode<float>(readbackBuffersSizeElement, context) : ReadbackBuffersSize,
			RenderTargets = members.TryGetValue("renderTargets", out var renderTargetsElement) ? ThreeValue.Decode<float>(renderTargetsElement, context) : RenderTargets,
			StorageAttributes = members.TryGetValue("storageAttributes", out var storageAttributesElement) ? ThreeValue.Decode<float>(storageAttributesElement, context) : StorageAttributes,
			StorageAttributesSize = members.TryGetValue("storageAttributesSize", out var storageAttributesSizeElement) ? ThreeValue.Decode<float>(storageAttributesSizeElement, context) : StorageAttributesSize,
			Textures = members.TryGetValue("textures", out var texturesElement) ? ThreeValue.Decode<float>(texturesElement, context) : Textures,
			TexturesSize = members.TryGetValue("texturesSize", out var texturesSizeElement) ? ThreeValue.Decode<float>(texturesSizeElement, context) : TexturesSize,
			UniformBuffers = members.TryGetValue("uniformBuffers", out var uniformBuffersElement) ? ThreeValue.Decode<float>(uniformBuffersElement, context) : UniformBuffers,
			UniformBuffersSize = members.TryGetValue("uniformBuffersSize", out var uniformBuffersSizeElement) ? ThreeValue.Decode<float>(uniformBuffersSizeElement, context) : UniformBuffersSize,
			Total = members.TryGetValue("total", out var totalElement) ? ThreeValue.Decode<float>(totalElement, context) : Total
		};
	}
}
