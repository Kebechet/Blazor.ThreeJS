// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// For use with a texture's <c>THREE.Texture.internalFormat</c> property, these define how elements
/// of a <c>THREE.Texture</c>, or texels, are stored on the GPU. - <c>R8</c> stores the red
/// component on 8 bits. - <c>R8_SNORM</c> stores the red component on 8 bits. The component is
/// stored as normalized. - <c>R8I</c> stores the red component on 8 bits. The component is stored
/// as an integer. - <c>R8UI</c> stores the red component on 8 bits. The component is stored as an
/// unsigned integer. - <c>R16I</c> stores the red component on 16 bits. The component is stored as
/// an integer. - <c>R16UI</c> stores the red component on 16 bits. The component is stored as an
/// unsigned integer. - <c>R16F</c> stores the red component on 16 bits. The component is stored as
/// floating point. - <c>R32I</c> stores the red component on 32 bits. The component is stored as an
/// integer. - <c>R32UI</c> stores the red component on 32 bits. The component is stored as an
/// unsigned integer. - <c>R32F</c> stores the red component on 32 bits. The component is stored as
/// floating point. - <c>RG8</c> stores the red and green components on 8 bits each. -
/// <c>RG8_SNORM</c> stores the red and green components on 8 bits each. Every component is stored
/// as normalized. - <c>RG8I</c> stores the red and green components on 8 bits each. Every component
/// is stored as an integer. - <c>RG8UI</c> stores the red and green components on 8 bits each.
/// Every component is stored as an unsigned integer. - <c>RG16I</c> stores the red and green
/// components on 16 bits each. Every component is stored as an integer. - <c>RG16UI</c> stores the
/// red and green components on 16 bits each. Every component is stored as an unsigned integer. -
/// <c>RG16F</c> stores the red and green components on 16 bits each. Every component is stored as
/// floating point. - <c>RG32I</c> stores the red and green components on 32 bits each. Every
/// component is stored as an integer. - <c>RG32UI</c> stores the red and green components on 32
/// bits. Every component is stored as an unsigned integer. - <c>RG32F</c> stores the red and green
/// components on 32 bits. Every component is stored as floating point. - <c>RGB8</c> stores the
/// red, green, and blue components on 8 bits each. RGB8_SNORM<c> stores the red, green, and blue
/// components on 8 bits each. Every component is stored as normalized. - </c>RGB8I<c> stores the
/// red, green, and blue components on 8 bits each. Every component is stored as an integer. -
/// </c>RGB8UI<c> stores the red, green, and blue components on 8 bits each. Every component is
/// stored as an unsigned integer. - </c>RGB16I<c> stores the red, green, and blue components on 16
/// bits each. Every component is stored as an integer. - </c>RGB16UI<c> stores the red, green, and
/// blue components on 16 bits each. Every component is stored as an unsigned integer. -
/// </c>RGB16F<c> stores the red, green, and blue components on 16 bits each. Every component is
/// stored as floating point - </c>RGB32I<c> stores the red, green, and blue components on 32 bits
/// each. Every component is stored as an integer. - </c>RGB32UI<c> stores the red, green, and blue
/// components on 32 bits each. Every component is stored as an unsigned integer. - </c>RGB32F<c>
/// stores the red, green, and blue components on 32 bits each. Every component is stored as
/// floating point - </c>R11F_G11F_B10F<c> stores the red, green, and blue components respectively
/// on 11 bits, 11 bits, and 10bits. Every component is stored as floating point. - </c>RGB565<c>
/// stores the red, green, and blue components respectively on 5 bits, 6 bits, and 5 bits. -
/// </c>RGB9_E5<c> stores the red, green, and blue components on 9 bits each. - </c>RGBA8<c> stores
/// the red, green, blue, and alpha components on 8 bits each. - </c>RGBA8_SNORM<c> stores the red,
/// green, blue, and alpha components on 8 bits. Every component is stored as normalized. -
/// </c>RGBA8I<c> stores the red, green, blue, and alpha components on 8 bits each. Every component
/// is stored as an integer. - </c>RGBA8UI<c> stores the red, green, blue, and alpha components on 8
/// bits. Every component is stored as an unsigned integer. - </c>RGBA16I<c> stores the red, green,
/// blue, and alpha components on 16 bits. Every component is stored as an integer. -
/// </c>RGBA16UI<c> stores the red, green, blue, and alpha components on 16 bits. Every component is
/// stored as an unsigned integer. - </c>RGBA16F<c> stores the red, green, blue, and alpha
/// components on 16 bits. Every component is stored as floating point. - </c>RGBA32I<c> stores the
/// red, green, blue, and alpha components on 32 bits. Every component is stored as an integer. -
/// </c>RGBA32UI<c> stores the red, green, blue, and alpha components on 32 bits. Every component is
/// stored as an unsigned integer. - </c>RGBA32F<c> stores the red, green, blue, and alpha
/// components on 32 bits. Every component is stored as floating point. - </c>RGB5_A1<c> stores the
/// red, green, blue, and alpha components respectively on 5 bits, 5 bits, 5 bits, and 1 bit. -
/// </c>RGB10_A2<c> stores the red, green, blue, and alpha components respectively on 10 bits, 10
/// bits, 10 bits and 2 bits. - </c>RGB10_A2UI<c> stores the red, green, blue, and alpha components
/// respectively on 10 bits, 10 bits, 10 bits and 2 bits. Every component is stored as an unsigned
/// integer. - </c>SRGB8<c> stores the red, green, and blue components on 8 bits each. -
/// </c>SRGB8_ALPHA8<c> stores the red, green, blue, and alpha components on 8 bits each. -
/// </c>DEPTH_COMPONENT16<c> stores the depth component on 16bits. - </c>DEPTH_COMPONENT24<c> stores
/// the depth component on 24bits. - </c>DEPTH_COMPONENT32F<c> stores the depth component on 32bits.
/// The component is stored as floating point. - </c>DEPTH24_STENCIL8<c> stores the depth, and
/// stencil components respectively on 24 bits and 8 bits. The stencil component is stored as an
/// unsigned integer. - </c>DEPTH32F_STENCIL8` stores the depth, and stencil components respectively
/// on 32 bits and 8 bits. The depth component is stored as floating point, and the stencil
/// component as an unsigned integer. Encoded on the wire as the string three.js compares against,
/// not as the C# value, which is only a position.
/// </summary>
public enum PixelFormatGPU : byte
{
	/// <summary>Matches <c>THREE.ALPHA</c>. Sent as <c>"ALPHA"</c>.</summary>
	ALPHA = 0,

	/// <summary>Matches <c>THREE.RGB</c>. Sent as <c>"RGB"</c>.</summary>
	RGB = 1,

	/// <summary>Matches <c>THREE.RGBA</c>. Sent as <c>"RGBA"</c>.</summary>
	RGBA = 2,

	/// <summary>Matches <c>THREE.LUMINANCE</c>. Sent as <c>"LUMINANCE"</c>.</summary>
	LUMINANCE = 3,

	/// <summary>Matches <c>THREE.LUMINANCE_ALPHA</c>. Sent as <c>"LUMINANCE_ALPHA"</c>.</summary>
	LUMINANCE_ALPHA = 4,

	/// <summary>Matches <c>THREE.RED_INTEGER</c>. Sent as <c>"RED_INTEGER"</c>.</summary>
	RED_INTEGER = 5,

	/// <summary>Matches <c>THREE.R8</c>. Sent as <c>"R8"</c>.</summary>
	R8 = 6,

	/// <summary>Matches <c>THREE.R8_SNORM</c>. Sent as <c>"R8_SNORM"</c>.</summary>
	R8_SNORM = 7,

	/// <summary>Matches <c>THREE.R8I</c>. Sent as <c>"R8I"</c>.</summary>
	R8I = 8,

	/// <summary>Matches <c>THREE.R8UI</c>. Sent as <c>"R8UI"</c>.</summary>
	R8UI = 9,

	/// <summary>Matches <c>THREE.R16I</c>. Sent as <c>"R16I"</c>.</summary>
	R16I = 10,

	/// <summary>Matches <c>THREE.R16UI</c>. Sent as <c>"R16UI"</c>.</summary>
	R16UI = 11,

	/// <summary>Matches <c>THREE.R16F</c>. Sent as <c>"R16F"</c>.</summary>
	R16F = 12,

	/// <summary>Matches <c>THREE.R32I</c>. Sent as <c>"R32I"</c>.</summary>
	R32I = 13,

	/// <summary>Matches <c>THREE.R32UI</c>. Sent as <c>"R32UI"</c>.</summary>
	R32UI = 14,

	/// <summary>Matches <c>THREE.R32F</c>. Sent as <c>"R32F"</c>.</summary>
	R32F = 15,

	/// <summary>Matches <c>THREE.RG8</c>. Sent as <c>"RG8"</c>.</summary>
	RG8 = 16,

	/// <summary>Matches <c>THREE.RG8_SNORM</c>. Sent as <c>"RG8_SNORM"</c>.</summary>
	RG8_SNORM = 17,

	/// <summary>Matches <c>THREE.RG8I</c>. Sent as <c>"RG8I"</c>.</summary>
	RG8I = 18,

	/// <summary>Matches <c>THREE.RG8UI</c>. Sent as <c>"RG8UI"</c>.</summary>
	RG8UI = 19,

	/// <summary>Matches <c>THREE.RG16I</c>. Sent as <c>"RG16I"</c>.</summary>
	RG16I = 20,

	/// <summary>Matches <c>THREE.RG16UI</c>. Sent as <c>"RG16UI"</c>.</summary>
	RG16UI = 21,

	/// <summary>Matches <c>THREE.RG16F</c>. Sent as <c>"RG16F"</c>.</summary>
	RG16F = 22,

	/// <summary>Matches <c>THREE.RG32I</c>. Sent as <c>"RG32I"</c>.</summary>
	RG32I = 23,

	/// <summary>Matches <c>THREE.RG32UI</c>. Sent as <c>"RG32UI"</c>.</summary>
	RG32UI = 24,

	/// <summary>Matches <c>THREE.RG32F</c>. Sent as <c>"RG32F"</c>.</summary>
	RG32F = 25,

	/// <summary>Matches <c>THREE.RGB565</c>. Sent as <c>"RGB565"</c>.</summary>
	RGB565 = 26,

	/// <summary>Matches <c>THREE.RGB8</c>. Sent as <c>"RGB8"</c>.</summary>
	RGB8 = 27,

	/// <summary>Matches <c>THREE.RGB8_SNORM</c>. Sent as <c>"RGB8_SNORM"</c>.</summary>
	RGB8_SNORM = 28,

	/// <summary>Matches <c>THREE.RGB8I</c>. Sent as <c>"RGB8I"</c>.</summary>
	RGB8I = 29,

	/// <summary>Matches <c>THREE.RGB8UI</c>. Sent as <c>"RGB8UI"</c>.</summary>
	RGB8UI = 30,

	/// <summary>Matches <c>THREE.RGB16I</c>. Sent as <c>"RGB16I"</c>.</summary>
	RGB16I = 31,

	/// <summary>Matches <c>THREE.RGB16UI</c>. Sent as <c>"RGB16UI"</c>.</summary>
	RGB16UI = 32,

	/// <summary>Matches <c>THREE.RGB16F</c>. Sent as <c>"RGB16F"</c>.</summary>
	RGB16F = 33,

	/// <summary>Matches <c>THREE.RGB32I</c>. Sent as <c>"RGB32I"</c>.</summary>
	RGB32I = 34,

	/// <summary>Matches <c>THREE.RGB32UI</c>. Sent as <c>"RGB32UI"</c>.</summary>
	RGB32UI = 35,

	/// <summary>Matches <c>THREE.RGB32F</c>. Sent as <c>"RGB32F"</c>.</summary>
	RGB32F = 36,

	/// <summary>Matches <c>THREE.RGB9_E5</c>. Sent as <c>"RGB9_E5"</c>.</summary>
	RGB9_E5 = 37,

	/// <summary>Matches <c>THREE.SRGB8</c>. Sent as <c>"SRGB8"</c>.</summary>
	SRGB8 = 38,

	/// <summary>Matches <c>THREE.R11F_G11F_B10F</c>. Sent as <c>"R11F_G11F_B10F"</c>.</summary>
	R11F_G11F_B10F = 39,

	/// <summary>Matches <c>THREE.RGBA4</c>. Sent as <c>"RGBA4"</c>.</summary>
	RGBA4 = 40,

	/// <summary>Matches <c>THREE.RGBA8</c>. Sent as <c>"RGBA8"</c>.</summary>
	RGBA8 = 41,

	/// <summary>Matches <c>THREE.RGBA8_SNORM</c>. Sent as <c>"RGBA8_SNORM"</c>.</summary>
	RGBA8_SNORM = 42,

	/// <summary>Matches <c>THREE.RGBA8I</c>. Sent as <c>"RGBA8I"</c>.</summary>
	RGBA8I = 43,

	/// <summary>Matches <c>THREE.RGBA8UI</c>. Sent as <c>"RGBA8UI"</c>.</summary>
	RGBA8UI = 44,

	/// <summary>Matches <c>THREE.RGBA16I</c>. Sent as <c>"RGBA16I"</c>.</summary>
	RGBA16I = 45,

	/// <summary>Matches <c>THREE.RGBA16UI</c>. Sent as <c>"RGBA16UI"</c>.</summary>
	RGBA16UI = 46,

	/// <summary>Matches <c>THREE.RGBA16F</c>. Sent as <c>"RGBA16F"</c>.</summary>
	RGBA16F = 47,

	/// <summary>Matches <c>THREE.RGBA32I</c>. Sent as <c>"RGBA32I"</c>.</summary>
	RGBA32I = 48,

	/// <summary>Matches <c>THREE.RGBA32UI</c>. Sent as <c>"RGBA32UI"</c>.</summary>
	RGBA32UI = 49,

	/// <summary>Matches <c>THREE.RGBA32F</c>. Sent as <c>"RGBA32F"</c>.</summary>
	RGBA32F = 50,

	/// <summary>Matches <c>THREE.RGB5_A1</c>. Sent as <c>"RGB5_A1"</c>.</summary>
	RGB5_A1 = 51,

	/// <summary>Matches <c>THREE.RGB10_A2</c>. Sent as <c>"RGB10_A2"</c>.</summary>
	RGB10_A2 = 52,

	/// <summary>Matches <c>THREE.RGB10_A2UI</c>. Sent as <c>"RGB10_A2UI"</c>.</summary>
	RGB10_A2UI = 53,

	/// <summary>Matches <c>THREE.SRGB8_ALPHA8</c>. Sent as <c>"SRGB8_ALPHA8"</c>.</summary>
	SRGB8_ALPHA8 = 54,

	/// <summary>Matches <c>THREE.DEPTH_COMPONENT16</c>. Sent as <c>"DEPTH_COMPONENT16"</c>.</summary>
	DEPTH_COMPONENT16 = 56,

	/// <summary>Matches <c>THREE.DEPTH_COMPONENT24</c>. Sent as <c>"DEPTH_COMPONENT24"</c>.</summary>
	DEPTH_COMPONENT24 = 57,

	/// <summary>Matches <c>THREE.DEPTH_COMPONENT32F</c>. Sent as <c>"DEPTH_COMPONENT32F"</c>.</summary>
	DEPTH_COMPONENT32F = 58,

	/// <summary>Matches <c>THREE.DEPTH24_STENCIL8</c>. Sent as <c>"DEPTH24_STENCIL8"</c>.</summary>
	DEPTH24_STENCIL8 = 59,

	/// <summary>Matches <c>THREE.DEPTH32F_STENCIL8</c>. Sent as <c>"DEPTH32F_STENCIL8"</c>.</summary>
	DEPTH32F_STENCIL8 = 60
}
