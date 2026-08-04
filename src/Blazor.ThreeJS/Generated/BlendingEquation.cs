// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>BlendingEquation</c>. Encoded on the wire as the numeric
/// value three.js itself uses, not as the member name.
/// </summary>
public enum BlendingEquation : byte
{
	/// <summary>Matches <c>THREE.AddEquation</c>.</summary>
	AddEquation = 100,

	/// <summary>Matches <c>THREE.SubtractEquation</c>.</summary>
	SubtractEquation = 101,

	/// <summary>Matches <c>THREE.ReverseSubtractEquation</c>.</summary>
	ReverseSubtractEquation = 102,

	/// <summary>Matches <c>THREE.MinEquation</c>.</summary>
	MinEquation = 103,

	/// <summary>Matches <c>THREE.MaxEquation</c>.</summary>
	MaxEquation = 104
}
