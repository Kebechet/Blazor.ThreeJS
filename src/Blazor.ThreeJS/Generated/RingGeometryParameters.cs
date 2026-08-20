// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>RingGeometryParameters</c>. A plain value rather than a
/// handle-backed object: three.js declares it as a shape, and nothing on either side keeps a
/// reference to one. It travels as its own members, under three.js's names for them.
/// </summary>
public sealed record RingGeometryParameters : IThreeStructure
{
	/// <summary>three.js's <c>innerRadius</c>.</summary>
	public float InnerRadius { get; init; }

	/// <summary>three.js's <c>outerRadius</c>.</summary>
	public float OuterRadius { get; init; }

	/// <summary>three.js's <c>thetaSegments</c>.</summary>
	public int ThetaSegments { get; init; }

	/// <summary>three.js's <c>phiSegments</c>.</summary>
	public int PhiSegments { get; init; }

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
		members["innerRadius"] = InnerRadius;
		members["outerRadius"] = OuterRadius;
		members["thetaSegments"] = ThetaSegments;
		members["phiSegments"] = PhiSegments;
		members["thetaStart"] = ThetaStart;
		members["thetaLength"] = ThetaLength;

		return members;
	}

	/// <summary>
	/// Builds a <c>RingGeometryParameters</c> from the members the applier sent back. A member three.js
	/// did not carry keeps this instance's own value, which for the blank instance the decoder builds
	/// is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members)
	{
		return new RingGeometryParameters
		{
			InnerRadius = members.TryGetValue("innerRadius", out var innerRadiusElement) ? ThreeValue.Decode<float>(innerRadiusElement) : InnerRadius,
			OuterRadius = members.TryGetValue("outerRadius", out var outerRadiusElement) ? ThreeValue.Decode<float>(outerRadiusElement) : OuterRadius,
			ThetaSegments = members.TryGetValue("thetaSegments", out var thetaSegmentsElement) ? ThreeValue.Decode<int>(thetaSegmentsElement) : ThetaSegments,
			PhiSegments = members.TryGetValue("phiSegments", out var phiSegmentsElement) ? ThreeValue.Decode<int>(phiSegmentsElement) : PhiSegments,
			ThetaStart = members.TryGetValue("thetaStart", out var thetaStartElement) ? ThreeValue.Decode<float>(thetaStartElement) : ThetaStart,
			ThetaLength = members.TryGetValue("thetaLength", out var thetaLengthElement) ? ThreeValue.Decode<float>(thetaLengthElement) : ThetaLength
		};
	}
}
