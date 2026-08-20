// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>LODLevels</c>. A plain value rather than a handle-backed object:
/// three.js declares it as a shape, and nothing on either side keeps a reference to one. It travels
/// as its own members, under three.js's names for them.
/// </summary>
public sealed record LODLevels : IThreeStructure
{
	/// <summary>The Object3D to display at this level.</summary>
	public Object3D Object { get; init; }

	/// <summary>The distance at which to display this level of detail. Expects a <c>Float</c>.</summary>
	public float Distance { get; init; }

	/// <summary>
	/// Threshold used to avoid flickering at LOD boundaries, as a fraction of distance. Expects a
	/// <c>Float</c>.
	/// </summary>
	public float Hysteresis { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["object"] = Object;
		members["distance"] = Distance;
		members["hysteresis"] = Hysteresis;

		return members;
	}

	/// <summary>
	/// Builds a <c>LODLevels</c> from the members the applier sent back. A member three.js did not
	/// carry keeps this instance's own value, which for the blank instance the decoder builds is the C#
	/// default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new LODLevels
		{
			Object = members.TryGetValue("object", out var objectElement) ? ThreeObject.AdoptStructureMember<Object3D>(ThreeStructure.RequireContext(context, "object"), "object", ThreeValue.Decode<ThreeObjectReference?>(objectElement), (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D")) : Object,
			Distance = members.TryGetValue("distance", out var distanceElement) ? ThreeValue.Decode<float>(distanceElement, context) : Distance,
			Hysteresis = members.TryGetValue("hysteresis", out var hysteresisElement) ? ThreeValue.Decode<float>(hysteresisElement, context) : Hysteresis
		};
	}
}
