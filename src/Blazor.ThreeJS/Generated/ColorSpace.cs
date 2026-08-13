// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>ColorSpace</c>. Encoded on the wire as the string three.js
/// compares against, not as the C# value, which is only a position.
/// </summary>
public enum ColorSpace : byte
{
	/// <summary>
	/// Matches <c>THREE.NoColorSpace</c>. Sent as the empty string, which is what three.js uses for
	/// this.
	/// </summary>
	NoColorSpace = 0,

	/// <summary>Matches <c>THREE.SRGBColorSpace</c>. Sent as <c>"srgb"</c>.</summary>
	SRGBColorSpace = 1,

	/// <summary>Matches <c>THREE.LinearSRGBColorSpace</c>. Sent as <c>"srgb-linear"</c>.</summary>
	LinearSRGBColorSpace = 2
}
