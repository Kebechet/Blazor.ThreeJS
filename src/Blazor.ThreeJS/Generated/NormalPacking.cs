// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>NormalPacking</c>. Encoded on the wire as the string three.js
/// compares against, not as the C# value, which is only a position.
/// </summary>
public enum NormalPacking : byte
{
	/// <summary>
	/// Matches <c>THREE.NoNormalPacking</c>. Sent as the empty string, which is what three.js uses for
	/// this.
	/// </summary>
	NoNormalPacking = 0,

	/// <summary>Matches <c>THREE.NormalRGPacking</c>. Sent as <c>"rg"</c>.</summary>
	NormalRGPacking = 1,

	/// <summary>Matches <c>THREE.NormalGAPacking</c>. Sent as <c>"ga"</c>.</summary>
	NormalGAPacking = 2
}
