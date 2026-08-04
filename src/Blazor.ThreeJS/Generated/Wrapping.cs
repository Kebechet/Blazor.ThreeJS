// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Texture Wrapping Modes. Encoded on the wire as the numeric value three.js itself uses, not as
/// the member name.
/// </summary>
public enum Wrapping : ushort
{
	/// <summary>With <c>RepeatWrapping</c> the texture will simply repeat to infinity.</summary>
	RepeatWrapping = 1000,

	/// <summary>
	/// With <c>ClampToEdgeWrapping</c> the last pixel of the texture stretches to the edge of the mesh.
	/// </summary>
	ClampToEdgeWrapping = 1001,

	/// <summary>
	/// With <c>MirroredRepeatWrapping</c> the texture will repeats to infinity, mirroring on each
	/// repeat.
	/// </summary>
	MirroredRepeatWrapping = 1002
}
