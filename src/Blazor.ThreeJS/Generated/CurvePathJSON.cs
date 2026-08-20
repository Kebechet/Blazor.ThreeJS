// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>CurvePathJSON</c>. A plain value rather than a handle-backed object:
/// three.js declares it as a shape, and nothing on either side keeps a reference to one. It travels
/// as its own members, under three.js's names for them.
/// </summary>
public sealed record CurvePathJSON : IThreeStructure
{
	/// <summary>three.js's <c>metadata</c>.</summary>
	public BufferGeometryJSONMetadata Metadata { get; init; }

	/// <summary>three.js's <c>arcLengthDivisions</c>.</summary>
	public float ArcLengthDivisions { get; init; }

	/// <summary>three.js's <c>type</c>.</summary>
	public string Type { get; init; }

	/// <summary>three.js's <c>autoClose</c>.</summary>
	public bool AutoClose { get; init; }

	/// <summary>three.js's <c>curves</c>.</summary>
	public CurveJSON[] Curves { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["metadata"] = Metadata;
		members["arcLengthDivisions"] = ArcLengthDivisions;
		members["type"] = Type;
		members["autoClose"] = AutoClose;
		members["curves"] = Curves;

		return members;
	}

	/// <summary>
	/// Builds a <c>CurvePathJSON</c> from the members the applier sent back. A member three.js did not
	/// carry keeps this instance's own value, which for the blank instance the decoder builds is the C#
	/// default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new CurvePathJSON
		{
			Metadata = members.TryGetValue("metadata", out var metadataElement) ? ThreeValue.Decode<BufferGeometryJSONMetadata>(metadataElement, context) : Metadata,
			ArcLengthDivisions = members.TryGetValue("arcLengthDivisions", out var arcLengthDivisionsElement) ? ThreeValue.Decode<float>(arcLengthDivisionsElement, context) : ArcLengthDivisions,
			Type = members.TryGetValue("type", out var typeElement) ? ThreeValue.Decode<string>(typeElement, context) : Type,
			AutoClose = members.TryGetValue("autoClose", out var autoCloseElement) ? ThreeValue.Decode<bool>(autoCloseElement, context) : AutoClose,
			Curves = members.TryGetValue("curves", out var curvesElement) ? ThreeValue.Decode<CurveJSON[]>(curvesElement, context) : Curves
		};
	}
}
