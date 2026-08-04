// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// All Texture Pixel Formats Modes for <c>THREE.DepthTexture</c>. Encoded on the wire as the
/// numeric value three.js itself uses, not as the member name.
/// </summary>
public enum DepthTexturePixelFormat : ushort
{
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
	DepthStencilFormat = 1027
}
