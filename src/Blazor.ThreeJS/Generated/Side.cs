// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Defines which side of faces will be rendered - front, back or both. Default is <c>FrontSide</c>.
/// Encoded on the wire as the numeric value three.js itself uses, not as the member name.
/// </summary>
public enum Side : byte
{
	/// <summary>Matches <c>THREE.FrontSide</c>.</summary>
	FrontSide = 0,

	/// <summary>Matches <c>THREE.BackSide</c>.</summary>
	BackSide = 1,

	/// <summary>Matches <c>THREE.DoubleSide</c>.</summary>
	DoubleSide = 2
}
