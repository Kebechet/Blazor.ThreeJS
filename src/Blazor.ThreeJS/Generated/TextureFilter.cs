// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Texture all Magnification and Minification Filter Modes. Encoded on the wire as the numeric
/// value three.js itself uses, not as the member name.
/// </summary>
public enum TextureFilter : ushort
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
	LinearFilter = 1006,

	/// <summary>
	/// <c>NearestMipmapNearestFilter</c> chooses the mipmap that most closely matches the size of the
	/// pixel being textured and uses the <c>NearestFilter</c> criterion (the texel nearest to the
	/// center of the pixel) to produce a texture value.
	/// </summary>
	NearestMipmapNearestFilter = 1004,

	/// <summary>
	/// <c>NearestMipmapNearestFilter</c> chooses the mipmap that most closely matches the size of the
	/// pixel being textured and uses the <c>NearestFilter</c> criterion (the texel nearest to the
	/// center of the pixel) to produce a texture value. An alternative spelling three.js gives the same
	/// value as <see cref="NearestMipmapNearestFilter"/>.
	/// </summary>
	NearestMipMapNearestFilter = NearestMipmapNearestFilter,

	/// <summary>
	/// <c>NearestMipmapLinearFilter</c> chooses the two mipmaps that most closely match the size of the
	/// pixel being textured and uses the <c>NearestFilter</c> criterion to produce a texture value from
	/// each mipmap. The final texture value is a weighted average of those two values.
	/// </summary>
	NearestMipmapLinearFilter = 1005,

	/// <summary>
	/// <c>NearestMipMapLinearFilter</c> chooses the two mipmaps that most closely match the size of the
	/// pixel being textured and uses the <c>NearestFilter</c> criterion to produce a texture value from
	/// each mipmap. The final texture value is a weighted average of those two values. An alternative
	/// spelling three.js gives the same value as <see cref="NearestMipmapLinearFilter"/>.
	/// </summary>
	NearestMipMapLinearFilter = NearestMipmapLinearFilter,

	/// <summary>
	/// <c>LinearMipmapNearestFilter</c> chooses the mipmap that most closely matches the size of the
	/// pixel being textured and uses the <c>LinearFilter</c> criterion (a weighted average of the four
	/// texels that are closest to the center of the pixel) to produce a texture value.
	/// </summary>
	LinearMipmapNearestFilter = 1007,

	/// <summary>
	/// <c>LinearMipMapNearestFilter</c> chooses the mipmap that most closely matches the size of the
	/// pixel being textured and uses the <c>LinearFilter</c> criterion (a weighted average of the four
	/// texels that are closest to the center of the pixel) to produce a texture value. An alternative
	/// spelling three.js gives the same value as <see cref="LinearMipmapNearestFilter"/>.
	/// </summary>
	LinearMipMapNearestFilter = LinearMipmapNearestFilter,

	/// <summary>
	/// <c>LinearMipmapLinearFilter</c> is the default and chooses the two mipmaps that most closely
	/// match the size of the pixel being textured and uses the <c>LinearFilter</c> criterion to produce
	/// a texture value from each mipmap. The final texture value is a weighted average of those two
	/// values.
	/// </summary>
	LinearMipmapLinearFilter = 1008,

	/// <summary>
	/// <c>LinearMipMapLinearFilter</c> is the default and chooses the two mipmaps that most closely
	/// match the size of the pixel being textured and uses the <c>LinearFilter</c> criterion to produce
	/// a texture value from each mipmap. The final texture value is a weighted average of those two
	/// values. An alternative spelling three.js gives the same value as
	/// <see cref="LinearMipmapLinearFilter"/>.
	/// </summary>
	LinearMipMapLinearFilter = LinearMipmapLinearFilter
}
