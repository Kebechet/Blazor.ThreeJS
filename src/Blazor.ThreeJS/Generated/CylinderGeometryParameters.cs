// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>CylinderGeometryParameters</c>. A plain value rather than a
/// handle-backed object: three.js declares it as a shape, and nothing on either side keeps a
/// reference to one. It travels as its own members, under three.js's names for them.
/// </summary>
public sealed record CylinderGeometryParameters : IThreeStructure
{
	/// <summary>three.js's <c>radiusTop</c>.</summary>
	public float RadiusTop { get; init; }

	/// <summary>three.js's <c>radiusBottom</c>.</summary>
	public float RadiusBottom { get; init; }

	/// <summary>three.js's <c>height</c>.</summary>
	public float Height { get; init; }

	/// <summary>three.js's <c>radialSegments</c>.</summary>
	public int RadialSegments { get; init; }

	/// <summary>three.js's <c>heightSegments</c>.</summary>
	public int HeightSegments { get; init; }

	/// <summary>three.js's <c>openEnded</c>.</summary>
	public bool OpenEnded { get; init; }

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
		members["radiusTop"] = RadiusTop;
		members["radiusBottom"] = RadiusBottom;
		members["height"] = Height;
		members["radialSegments"] = RadialSegments;
		members["heightSegments"] = HeightSegments;
		members["openEnded"] = OpenEnded;
		members["thetaStart"] = ThetaStart;
		members["thetaLength"] = ThetaLength;

		return members;
	}

	/// <summary>
	/// Builds a <c>CylinderGeometryParameters</c> from the members the applier sent back. A member
	/// three.js did not carry keeps this instance's own value, which for the blank instance the decoder
	/// builds is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new CylinderGeometryParameters
		{
			RadiusTop = members.TryGetValue("radiusTop", out var radiusTopElement) ? ThreeValue.Decode<float>(radiusTopElement, context) : RadiusTop,
			RadiusBottom = members.TryGetValue("radiusBottom", out var radiusBottomElement) ? ThreeValue.Decode<float>(radiusBottomElement, context) : RadiusBottom,
			Height = members.TryGetValue("height", out var heightElement) ? ThreeValue.Decode<float>(heightElement, context) : Height,
			RadialSegments = members.TryGetValue("radialSegments", out var radialSegmentsElement) ? ThreeValue.Decode<int>(radialSegmentsElement, context) : RadialSegments,
			HeightSegments = members.TryGetValue("heightSegments", out var heightSegmentsElement) ? ThreeValue.Decode<int>(heightSegmentsElement, context) : HeightSegments,
			OpenEnded = members.TryGetValue("openEnded", out var openEndedElement) ? ThreeValue.Decode<bool>(openEndedElement, context) : OpenEnded,
			ThetaStart = members.TryGetValue("thetaStart", out var thetaStartElement) ? ThreeValue.Decode<float>(thetaStartElement, context) : ThetaStart,
			ThetaLength = members.TryGetValue("thetaLength", out var thetaLengthElement) ? ThreeValue.Decode<float>(thetaLengthElement, context) : ThetaLength
		};
	}
}
