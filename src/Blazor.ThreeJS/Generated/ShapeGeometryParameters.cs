// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>ShapeGeometryParameters</c>. A plain value rather than a
/// handle-backed object: three.js declares it as a shape, and nothing on either side keeps a
/// reference to one. It travels as its own members, under three.js's names for them.
/// </summary>
public sealed record ShapeGeometryParameters : IThreeStructure
{
	/// <summary>three.js's <c>shapes</c>.</summary>
	public Shape Shapes { get; init; }

	/// <summary>three.js's <c>curveSegments</c>.</summary>
	public int CurveSegments { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["shapes"] = Shapes;
		members["curveSegments"] = CurveSegments;

		return members;
	}

	/// <summary>
	/// Builds a <c>ShapeGeometryParameters</c> from the members the applier sent back. A member
	/// three.js did not carry keeps this instance's own value, which for the blank instance the decoder
	/// builds is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new ShapeGeometryParameters
		{
			Shapes = members.TryGetValue("shapes", out var shapesElement) ? ThreeObject.AdoptStructureMember<Shape>(ThreeStructure.RequireContext(context, "shapes"), "shapes", ThreeValue.Decode<ThreeObjectReference?>(shapesElement), (adoptedBatch, adoptedHandle) => new Shape(adoptedBatch, adoptedHandle)) : Shapes,
			CurveSegments = members.TryGetValue("curveSegments", out var curveSegmentsElement) ? ThreeValue.Decode<int>(curveSegmentsElement, context) : CurveSegments
		};
	}
}
