// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>ShaderMaterialDefaultAttributeValues</c>. A plain value rather than
/// a handle-backed object: three.js declares it as a shape, and nothing on either side keeps a
/// reference to one. It travels as its own members, under three.js's names for them.
/// </summary>
public sealed record ShaderMaterialDefaultAttributeValues : IThreeStructure
{
	/// <summary>three.js's <c>color</c>.</summary>
	public float[] Color { get; init; }

	/// <summary>three.js's <c>uv</c>.</summary>
	public float[] Uv { get; init; }

	/// <summary>three.js's <c>uv1</c>.</summary>
	public float[] Uv1 { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["color"] = Color;
		members["uv"] = Uv;
		members["uv1"] = Uv1;

		return members;
	}

	/// <summary>
	/// Builds a <c>ShaderMaterialDefaultAttributeValues</c> from the members the applier sent back. A
	/// member three.js did not carry keeps this instance's own value, which for the blank instance the
	/// decoder builds is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new ShaderMaterialDefaultAttributeValues
		{
			Color = members.TryGetValue("color", out var colorElement) ? ThreeValue.Decode<float[]>(colorElement, context) : Color,
			Uv = members.TryGetValue("uv", out var uvElement) ? ThreeValue.Decode<float[]>(uvElement, context) : Uv,
			Uv1 = members.TryGetValue("uv1", out var uv1Element) ? ThreeValue.Decode<float[]>(uv1Element, context) : Uv1
		};
	}
}
