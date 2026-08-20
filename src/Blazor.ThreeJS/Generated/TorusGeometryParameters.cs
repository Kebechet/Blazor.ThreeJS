// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>TorusGeometryParameters</c>. A plain value rather than a
/// handle-backed object: three.js declares it as a shape, and nothing on either side keeps a
/// reference to one. It travels as its own members, under three.js's names for them.
/// </summary>
public sealed record TorusGeometryParameters : IThreeStructure
{
	/// <summary>three.js's <c>radius</c>.</summary>
	public float Radius { get; init; }

	/// <summary>three.js's <c>tube</c>.</summary>
	public float Tube { get; init; }

	/// <summary>three.js's <c>radialSegments</c>.</summary>
	public int RadialSegments { get; init; }

	/// <summary>three.js's <c>tubularSegments</c>.</summary>
	public int TubularSegments { get; init; }

	/// <summary>three.js's <c>arc</c>.</summary>
	public float Arc { get; init; }

	/// <summary>three.js's <c>thetaStart</c>.</summary>
	public float ThetaStart { get; init; }

	/// <summary>three.js's <c>thetaLength</c>.</summary>
	public float ThetaLength { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["radius"] = Radius;
		members["tube"] = Tube;
		members["radialSegments"] = RadialSegments;
		members["tubularSegments"] = TubularSegments;
		members["arc"] = Arc;
		members["thetaStart"] = ThetaStart;
		members["thetaLength"] = ThetaLength;

		return members;
	}

	/// <summary>
	/// Builds a <c>TorusGeometryParameters</c> from the members the applier sent back. A member
	/// three.js did not carry keeps this instance's own value, which for the blank instance the decoder
	/// builds is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members)
	{
		return new TorusGeometryParameters
		{
			Radius = members.TryGetValue("radius", out var radiusElement) ? ThreeValue.Decode<float>(radiusElement) : Radius,
			Tube = members.TryGetValue("tube", out var tubeElement) ? ThreeValue.Decode<float>(tubeElement) : Tube,
			RadialSegments = members.TryGetValue("radialSegments", out var radialSegmentsElement) ? ThreeValue.Decode<int>(radialSegmentsElement) : RadialSegments,
			TubularSegments = members.TryGetValue("tubularSegments", out var tubularSegmentsElement) ? ThreeValue.Decode<int>(tubularSegmentsElement) : TubularSegments,
			Arc = members.TryGetValue("arc", out var arcElement) ? ThreeValue.Decode<float>(arcElement) : Arc,
			ThetaStart = members.TryGetValue("thetaStart", out var thetaStartElement) ? ThreeValue.Decode<float>(thetaStartElement) : ThetaStart,
			ThetaLength = members.TryGetValue("thetaLength", out var thetaLengthElement) ? ThreeValue.Decode<float>(thetaLengthElement) : ThetaLength
		};
	}
}
