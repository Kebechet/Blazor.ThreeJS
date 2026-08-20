// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>AnimationMixerStats</c>. A plain value rather than a handle-backed
/// object: three.js declares it as a shape, and nothing on either side keeps a reference to one. It
/// travels as its own members, under three.js's names for them.
/// </summary>
public sealed record AnimationMixerStats : IThreeStructure
{
	/// <summary>three.js's <c>actions</c>.</summary>
	public AnimationMixerStatsActions Actions { get; init; }

	/// <summary>three.js's <c>bindings</c>.</summary>
	public AnimationMixerStatsActions Bindings { get; init; }

	/// <summary>three.js's <c>controlInterpolants</c>.</summary>
	public AnimationMixerStatsActions ControlInterpolants { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["actions"] = Actions;
		members["bindings"] = Bindings;
		members["controlInterpolants"] = ControlInterpolants;

		return members;
	}

	/// <summary>
	/// Builds a <c>AnimationMixerStats</c> from the members the applier sent back. A member three.js
	/// did not carry keeps this instance's own value, which for the blank instance the decoder builds
	/// is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members)
	{
		return new AnimationMixerStats
		{
			Actions = members.TryGetValue("actions", out var actionsElement) ? ThreeValue.Decode<AnimationMixerStatsActions>(actionsElement) : Actions,
			Bindings = members.TryGetValue("bindings", out var bindingsElement) ? ThreeValue.Decode<AnimationMixerStatsActions>(bindingsElement) : Bindings,
			ControlInterpolants = members.TryGetValue("controlInterpolants", out var controlInterpolantsElement) ? ThreeValue.Decode<AnimationMixerStatsActions>(controlInterpolantsElement) : ControlInterpolants
		};
	}
}
