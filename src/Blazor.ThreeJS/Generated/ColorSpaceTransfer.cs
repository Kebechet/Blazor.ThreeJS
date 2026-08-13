// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>ColorSpaceTransfer</c>. Encoded on the wire as the string
/// three.js compares against, not as the C# value, which is only a position.
/// </summary>
public enum ColorSpaceTransfer : byte
{
	/// <summary>Matches <c>THREE.LinearTransfer</c>. Sent as <c>"linear"</c>.</summary>
	LinearTransfer = 0,

	/// <summary>Matches <c>THREE.SRGBTransfer</c>. Sent as <c>"srgb"</c>.</summary>
	SRGBTransfer = 1
}
