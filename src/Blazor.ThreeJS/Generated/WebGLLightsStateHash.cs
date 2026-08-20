// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>WebGLLightsStateHash</c>. A plain value rather than a handle-backed
/// object: three.js declares it as a shape, and nothing on either side keeps a reference to one. It
/// travels as its own members, under three.js's names for them.
/// </summary>
public sealed record WebGLLightsStateHash : IThreeStructure
{
	/// <summary>three.js's <c>directionalLength</c>.</summary>
	public float DirectionalLength { get; init; }

	/// <summary>three.js's <c>pointLength</c>.</summary>
	public float PointLength { get; init; }

	/// <summary>three.js's <c>spotLength</c>.</summary>
	public float SpotLength { get; init; }

	/// <summary>three.js's <c>rectAreaLength</c>.</summary>
	public float RectAreaLength { get; init; }

	/// <summary>three.js's <c>hemiLength</c>.</summary>
	public float HemiLength { get; init; }

	/// <summary>three.js's <c>numDirectionalShadows</c>.</summary>
	public float NumDirectionalShadows { get; init; }

	/// <summary>three.js's <c>numPointShadows</c>.</summary>
	public float NumPointShadows { get; init; }

	/// <summary>three.js's <c>numSpotShadows</c>.</summary>
	public float NumSpotShadows { get; init; }

	/// <summary>three.js's <c>numSpotMaps</c>.</summary>
	public float NumSpotMaps { get; init; }

	/// <summary>three.js's <c>numLightProbes</c>.</summary>
	public float NumLightProbes { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["directionalLength"] = DirectionalLength;
		members["pointLength"] = PointLength;
		members["spotLength"] = SpotLength;
		members["rectAreaLength"] = RectAreaLength;
		members["hemiLength"] = HemiLength;
		members["numDirectionalShadows"] = NumDirectionalShadows;
		members["numPointShadows"] = NumPointShadows;
		members["numSpotShadows"] = NumSpotShadows;
		members["numSpotMaps"] = NumSpotMaps;
		members["numLightProbes"] = NumLightProbes;

		return members;
	}

	/// <summary>
	/// Builds a <c>WebGLLightsStateHash</c> from the members the applier sent back. A member three.js
	/// did not carry keeps this instance's own value, which for the blank instance the decoder builds
	/// is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new WebGLLightsStateHash
		{
			DirectionalLength = members.TryGetValue("directionalLength", out var directionalLengthElement) ? ThreeValue.Decode<float>(directionalLengthElement, context) : DirectionalLength,
			PointLength = members.TryGetValue("pointLength", out var pointLengthElement) ? ThreeValue.Decode<float>(pointLengthElement, context) : PointLength,
			SpotLength = members.TryGetValue("spotLength", out var spotLengthElement) ? ThreeValue.Decode<float>(spotLengthElement, context) : SpotLength,
			RectAreaLength = members.TryGetValue("rectAreaLength", out var rectAreaLengthElement) ? ThreeValue.Decode<float>(rectAreaLengthElement, context) : RectAreaLength,
			HemiLength = members.TryGetValue("hemiLength", out var hemiLengthElement) ? ThreeValue.Decode<float>(hemiLengthElement, context) : HemiLength,
			NumDirectionalShadows = members.TryGetValue("numDirectionalShadows", out var numDirectionalShadowsElement) ? ThreeValue.Decode<float>(numDirectionalShadowsElement, context) : NumDirectionalShadows,
			NumPointShadows = members.TryGetValue("numPointShadows", out var numPointShadowsElement) ? ThreeValue.Decode<float>(numPointShadowsElement, context) : NumPointShadows,
			NumSpotShadows = members.TryGetValue("numSpotShadows", out var numSpotShadowsElement) ? ThreeValue.Decode<float>(numSpotShadowsElement, context) : NumSpotShadows,
			NumSpotMaps = members.TryGetValue("numSpotMaps", out var numSpotMapsElement) ? ThreeValue.Decode<float>(numSpotMapsElement, context) : NumSpotMaps,
			NumLightProbes = members.TryGetValue("numLightProbes", out var numLightProbesElement) ? ThreeValue.Decode<float>(numLightProbesElement, context) : NumLightProbes
		};
	}
}
