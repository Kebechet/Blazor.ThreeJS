// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// All Texture Pixel Formats Modes. Encoded on the wire as the numeric value three.js itself uses,
/// not as the member name.
/// </summary>
public enum PixelFormat : ushort
{
	/// <summary>
	/// <c>AlphaFormat</c> discards the red, green and blue components and reads just the alpha
	/// component.
	/// </summary>
	AlphaFormat = 1021,

	/// <summary>Matches <c>THREE.RGBFormat</c>.</summary>
	RGBFormat = 1022,

	/// <summary><c>RGBAFormat</c> is the default and reads the red, green, blue and alpha components.</summary>
	RGBAFormat = 1023,

	/// <summary>
	/// <c>DepthFormat</c> reads each element as a single depth value, converts it to floating point,
	/// and clamps to the range <c>[0,1]</c>.
	/// </summary>
	DepthFormat = 1026,

	/// <summary>
	/// <c>DepthStencilFormat</c> reads each element is a pair of depth and stencil values. The depth
	/// component of the pair is interpreted as in <c>DepthFormat</c>. The stencil component is
	/// interpreted based on the depth + stencil internal format.
	/// </summary>
	DepthStencilFormat = 1027,

	/// <summary><c>RedFormat</c> discards the green and blue components and reads just the red component.</summary>
	RedFormat = 1028,

	/// <summary>
	/// <c>RedIntegerFormat</c> discards the green and blue components and reads just the red component.
	/// The texels are read as integers instead of floating point.
	/// </summary>
	RedIntegerFormat = 1029,

	/// <summary>
	/// <c>RGFormat</c> discards the alpha, and blue components and reads the red, and green components.
	/// </summary>
	RGFormat = 1030,

	/// <summary>
	/// <c>RGIntegerFormat</c> discards the alpha, and blue components and reads the red, and green
	/// components. The texels are read as integers instead of floating point.
	/// </summary>
	RGIntegerFormat = 1031,

	/// <summary>
	/// <c>RGBIntegerFormat</c> discards the alpha components and reads the red, green, and blue
	/// components.
	/// </summary>
	RGBIntegerFormat = 1032,

	/// <summary><c>RGBAIntegerFormat</c> reads the red, green, blue and alpha component.</summary>
	RGBAIntegerFormat = 1033
}
