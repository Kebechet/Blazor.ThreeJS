// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>AudioSourceType</c>. Encoded on the wire as the string
/// three.js compares against, not as the C# value, which is only a position.
/// </summary>
public enum AudioSourceType : byte
{
	/// <summary>Matches <c>THREE.Empty</c>. Sent as <c>"empty"</c>.</summary>
	Empty = 0,

	/// <summary>Matches <c>THREE.AudioNode</c>. Sent as <c>"audioNode"</c>.</summary>
	AudioNode = 1,

	/// <summary>Matches <c>THREE.MediaNode</c>. Sent as <c>"mediaNode"</c>.</summary>
	MediaNode = 2,

	/// <summary>Matches <c>THREE.MediaStreamNode</c>. Sent as <c>"mediaStreamNode"</c>.</summary>
	MediaStreamNode = 3,

	/// <summary>Matches <c>THREE.Buffer</c>. Sent as <c>"buffer"</c>.</summary>
	Buffer = 4
}
