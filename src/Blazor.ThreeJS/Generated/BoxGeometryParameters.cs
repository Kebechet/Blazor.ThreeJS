// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>BoxGeometryParameters</c>. A plain value rather than a handle-backed
/// object: three.js declares it as a shape, and nothing on either side keeps a reference to one. It
/// travels as its own members, under three.js's names for them.
/// </summary>
public sealed record BoxGeometryParameters : IThreeStructure
{
	/// <summary>three.js's <c>width</c>.</summary>
	public float Width { get; init; }

	/// <summary>three.js's <c>height</c>.</summary>
	public float Height { get; init; }

	/// <summary>three.js's <c>depth</c>.</summary>
	public float Depth { get; init; }

	/// <summary>three.js's <c>widthSegments</c>.</summary>
	public int WidthSegments { get; init; }

	/// <summary>three.js's <c>heightSegments</c>.</summary>
	public int HeightSegments { get; init; }

	/// <summary>three.js's <c>depthSegments</c>.</summary>
	public int DepthSegments { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["width"] = Width;
		members["height"] = Height;
		members["depth"] = Depth;
		members["widthSegments"] = WidthSegments;
		members["heightSegments"] = HeightSegments;
		members["depthSegments"] = DepthSegments;

		return members;
	}

	/// <summary>
	/// Builds a <c>BoxGeometryParameters</c> from the members the applier sent back. A member three.js
	/// did not carry keeps this instance's own value, which for the blank instance the decoder builds
	/// is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new BoxGeometryParameters
		{
			Width = members.TryGetValue("width", out var widthElement) ? ThreeValue.Decode<float>(widthElement, context) : Width,
			Height = members.TryGetValue("height", out var heightElement) ? ThreeValue.Decode<float>(heightElement, context) : Height,
			Depth = members.TryGetValue("depth", out var depthElement) ? ThreeValue.Decode<float>(depthElement, context) : Depth,
			WidthSegments = members.TryGetValue("widthSegments", out var widthSegmentsElement) ? ThreeValue.Decode<int>(widthSegmentsElement, context) : WidthSegments,
			HeightSegments = members.TryGetValue("heightSegments", out var heightSegmentsElement) ? ThreeValue.Decode<int>(heightSegmentsElement, context) : HeightSegments,
			DepthSegments = members.TryGetValue("depthSegments", out var depthSegmentsElement) ? ThreeValue.Decode<int>(depthSegmentsElement, context) : DepthSegments
		};
	}
}
