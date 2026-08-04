// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Texture Magnification Filter Modes. For use with a texture's <c>magFilter</c> property, these
/// define the texture magnification function to be used when the pixel being textured maps to an
/// area less than or equal to one texture element (texel). Encoded on the wire as the numeric value
/// three.js itself uses, not as the member name.
/// </summary>
public enum MagnificationTextureFilter : ushort
{
	/// <summary>
	/// <c>NearestFilter</c> returns the value of the texture element that is nearest (in Manhattan
	/// distance) to the specified texture coordinates.
	/// </summary>
	NearestFilter = 1003,

	/// <summary>
	/// <c>LinearFilter</c> returns the weighted average of the four texture elements that are closest
	/// to the specified texture coordinates, and can include items wrapped or repeated from other parts
	/// of a texture, depending on the values of <c>wrapS</c> and <c>wrapT</c>, and on the exact
	/// mapping.
	/// </summary>
	LinearFilter = 1006
}
