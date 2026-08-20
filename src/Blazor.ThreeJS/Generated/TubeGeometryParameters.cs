// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>TubeGeometryParameters</c>. A plain value rather than a
/// handle-backed object: three.js declares it as a shape, and nothing on either side keeps a
/// reference to one. It travels as its own members, under three.js's names for them.
/// </summary>
public sealed record TubeGeometryParameters : IThreeStructure
{
	/// <summary>three.js's <c>path</c>.</summary>
	public ThreeObject Path { get; init; }

	/// <summary>three.js's <c>tubularSegments</c>.</summary>
	public int TubularSegments { get; init; }

	/// <summary>three.js's <c>radius</c>.</summary>
	public float Radius { get; init; }

	/// <summary>three.js's <c>radialSegments</c>.</summary>
	public int RadialSegments { get; init; }

	/// <summary>three.js's <c>closed</c>.</summary>
	public bool Closed { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["path"] = Path;
		members["tubularSegments"] = TubularSegments;
		members["radius"] = Radius;
		members["radialSegments"] = RadialSegments;
		members["closed"] = Closed;

		return members;
	}

	/// <summary>
	/// Builds a <c>TubeGeometryParameters</c> from the members the applier sent back. A member three.js
	/// did not carry keeps this instance's own value, which for the blank instance the decoder builds
	/// is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new TubeGeometryParameters
		{
			Path = members.TryGetValue("path", out var pathElement) ? ThreeObject.AdoptStructureMember<ThreeObject>(ThreeStructure.RequireContext(context, "path"), "path", ThreeValue.Decode<ThreeObjectReference?>(pathElement), (adoptedBatch, adoptedHandle) => new Primitive(adoptedBatch, adoptedHandle, "ThreeObject")) : Path,
			TubularSegments = members.TryGetValue("tubularSegments", out var tubularSegmentsElement) ? ThreeValue.Decode<int>(tubularSegmentsElement, context) : TubularSegments,
			Radius = members.TryGetValue("radius", out var radiusElement) ? ThreeValue.Decode<float>(radiusElement, context) : Radius,
			RadialSegments = members.TryGetValue("radialSegments", out var radialSegmentsElement) ? ThreeValue.Decode<int>(radialSegmentsElement, context) : RadialSegments,
			Closed = members.TryGetValue("closed", out var closedElement) ? ThreeValue.Decode<bool>(closedElement, context) : Closed
		};
	}
}
