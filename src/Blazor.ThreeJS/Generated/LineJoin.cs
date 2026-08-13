// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>LineJoin</c>. Encoded on the wire as the string three.js
/// compares against, not as the C# value, which is only a position.
/// </summary>
public enum LineJoin : byte
{
	/// <summary>Matches <c>THREE.Round</c>. Sent as <c>"round"</c>.</summary>
	Round = 0,

	/// <summary>Matches <c>THREE.Bevel</c>. Sent as <c>"bevel"</c>.</summary>
	Bevel = 1,

	/// <summary>Matches <c>THREE.Miter</c>. Sent as <c>"miter"</c>.</summary>
	Miter = 2
}
