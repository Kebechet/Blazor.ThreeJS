// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>WebGLInfoRender</c>. A plain value rather than a handle-backed
/// object: three.js declares it as a shape, and nothing on either side keeps a reference to one. It
/// travels as its own members, under three.js's names for them.
/// </summary>
public sealed record WebGLInfoRender : IThreeStructure
{
	/// <summary>three.js's <c>calls</c>.</summary>
	public float Calls { get; init; }

	/// <summary>three.js's <c>frame</c>.</summary>
	public float Frame { get; init; }

	/// <summary>three.js's <c>lines</c>.</summary>
	public float Lines { get; init; }

	/// <summary>three.js's <c>points</c>.</summary>
	public float Points { get; init; }

	/// <summary>three.js's <c>triangles</c>.</summary>
	public float Triangles { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["calls"] = Calls;
		members["frame"] = Frame;
		members["lines"] = Lines;
		members["points"] = Points;
		members["triangles"] = Triangles;

		return members;
	}

	/// <summary>
	/// Builds a <c>WebGLInfoRender</c> from the members the applier sent back. A member three.js did
	/// not carry keeps this instance's own value, which for the blank instance the decoder builds is
	/// the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new WebGLInfoRender
		{
			Calls = members.TryGetValue("calls", out var callsElement) ? ThreeValue.Decode<float>(callsElement, context) : Calls,
			Frame = members.TryGetValue("frame", out var frameElement) ? ThreeValue.Decode<float>(frameElement, context) : Frame,
			Lines = members.TryGetValue("lines", out var linesElement) ? ThreeValue.Decode<float>(linesElement, context) : Lines,
			Points = members.TryGetValue("points", out var pointsElement) ? ThreeValue.Decode<float>(pointsElement, context) : Points,
			Triangles = members.TryGetValue("triangles", out var trianglesElement) ? ThreeValue.Decode<float>(trianglesElement, context) : Triangles
		};
	}
}
