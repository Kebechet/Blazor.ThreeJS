// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>CurveType</c>. Encoded on the wire as the string three.js
/// compares against, not as the C# value, which is only a position.
/// </summary>
public enum CurveType : byte
{
	/// <summary>Matches <c>THREE.Centripetal</c>. Sent as <c>"centripetal"</c>.</summary>
	Centripetal = 0,

	/// <summary>Matches <c>THREE.Chordal</c>. Sent as <c>"chordal"</c>.</summary>
	Chordal = 1,

	/// <summary>Matches <c>THREE.Catmullrom</c>. Sent as <c>"catmullrom"</c>.</summary>
	Catmullrom = 2
}
