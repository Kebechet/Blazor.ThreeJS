// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>DistanceModel</c>. Encoded on the wire as the string three.js
/// compares against, not as the C# value, which is only a position.
/// </summary>
public enum DistanceModel : byte
{
	/// <summary>Matches <c>THREE.Linear</c>. Sent as <c>"linear"</c>.</summary>
	Linear = 0,

	/// <summary>Matches <c>THREE.Inverse</c>. Sent as <c>"inverse"</c>.</summary>
	Inverse = 1,

	/// <summary>Matches <c>THREE.Exponential</c>. Sent as <c>"exponential"</c>.</summary>
	Exponential = 2
}
