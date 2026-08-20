// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>ShaderPrecision</c>. Encoded on the wire as the string
/// three.js compares against, not as the C# value, which is only a position.
/// </summary>
public enum ShaderPrecision : byte
{
	/// <summary>Matches <c>THREE.Highp</c>. Sent as <c>"highp"</c>.</summary>
	Highp = 0,

	/// <summary>Matches <c>THREE.Mediump</c>. Sent as <c>"mediump"</c>.</summary>
	Mediump = 1,

	/// <summary>Matches <c>THREE.Lowp</c>. Sent as <c>"lowp"</c>.</summary>
	Lowp = 2
}
