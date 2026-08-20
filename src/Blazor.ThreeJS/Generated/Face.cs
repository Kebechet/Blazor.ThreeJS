// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>Face</c>. A plain value rather than a handle-backed object: three.js
/// declares it as a shape, and nothing on either side keeps a reference to one. It travels as its
/// own members, under three.js's names for them.
/// </summary>
public sealed record Face : IThreeStructure
{
	/// <summary>three.js's <c>a</c>.</summary>
	public float A { get; init; }

	/// <summary>three.js's <c>b</c>.</summary>
	public float B { get; init; }

	/// <summary>three.js's <c>c</c>.</summary>
	public float C { get; init; }

	/// <summary>three.js's <c>normal</c>.</summary>
	public Vector3 Normal { get; init; }

	/// <summary>three.js's <c>materialIndex</c>.</summary>
	public int MaterialIndex { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["a"] = A;
		members["b"] = B;
		members["c"] = C;
		members["normal"] = Normal;
		members["materialIndex"] = MaterialIndex;

		return members;
	}

	/// <summary>
	/// Builds a <c>Face</c> from the members the applier sent back. A member three.js did not carry
	/// keeps this instance's own value, which for the blank instance the decoder builds is the C#
	/// default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new Face
		{
			A = members.TryGetValue("a", out var aElement) ? ThreeValue.Decode<float>(aElement, context) : A,
			B = members.TryGetValue("b", out var bElement) ? ThreeValue.Decode<float>(bElement, context) : B,
			C = members.TryGetValue("c", out var cElement) ? ThreeValue.Decode<float>(cElement, context) : C,
			Normal = members.TryGetValue("normal", out var normalElement) ? ThreeValue.Decode<Vector3>(normalElement, context) : Normal,
			MaterialIndex = members.TryGetValue("materialIndex", out var materialIndexElement) ? ThreeValue.Decode<int>(materialIndexElement, context) : MaterialIndex
		};
	}
}
