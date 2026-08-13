// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>GLSLVersion</c>. Encoded on the wire as the string three.js
/// compares against, not as the C# value, which is only a position.
/// </summary>
public enum GLSLVersion : byte
{
	/// <summary>Matches <c>THREE.GLSL1</c>. Sent as <c>"100"</c>.</summary>
	GLSL1 = 0,

	/// <summary>Matches <c>THREE.GLSL3</c>. Sent as <c>"300 es"</c>.</summary>
	GLSL3 = 1
}
