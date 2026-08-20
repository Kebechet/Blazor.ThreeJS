// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>AnimationClipJSON</c>. A plain value rather than a handle-backed
/// object: three.js declares it as a shape, and nothing on either side keeps a reference to one. It
/// travels as its own members, under three.js's names for them.
/// </summary>
public sealed record AnimationClipJSON : IThreeStructure
{
	/// <summary>three.js's <c>name</c>.</summary>
	public string Name { get; init; }

	/// <summary>three.js's <c>duration</c>.</summary>
	public float Duration { get; init; }

	/// <summary>three.js's <c>tracks</c>.</summary>
	public KeyframeTrackJSON[] Tracks { get; init; }

	/// <summary>three.js's <c>uuid</c>.</summary>
	public string Uuid { get; init; }

	/// <summary>three.js's <c>blendMode</c>.</summary>
	public AnimationBlendMode BlendMode { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["name"] = Name;
		members["duration"] = Duration;
		members["tracks"] = Tracks;
		members["uuid"] = Uuid;
		members["blendMode"] = BlendMode;

		return members;
	}

	/// <summary>
	/// Builds a <c>AnimationClipJSON</c> from the members the applier sent back. A member three.js did
	/// not carry keeps this instance's own value, which for the blank instance the decoder builds is
	/// the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members)
	{
		return new AnimationClipJSON
		{
			Name = members.TryGetValue("name", out var nameElement) ? ThreeValue.Decode<string>(nameElement) : Name,
			Duration = members.TryGetValue("duration", out var durationElement) ? ThreeValue.Decode<float>(durationElement) : Duration,
			Tracks = members.TryGetValue("tracks", out var tracksElement) ? ThreeValue.Decode<KeyframeTrackJSON[]>(tracksElement) : Tracks,
			Uuid = members.TryGetValue("uuid", out var uuidElement) ? ThreeValue.Decode<string>(uuidElement) : Uuid,
			BlendMode = members.TryGetValue("blendMode", out var blendModeElement) ? ThreeValue.Decode<AnimationBlendMode>(blendModeElement) : BlendMode
		};
	}
}
