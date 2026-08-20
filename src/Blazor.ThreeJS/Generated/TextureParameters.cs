// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>TextureParameters</c>. A plain value rather than a handle-backed
/// object: three.js declares it as a shape, and nothing on either side keeps a reference to one. It
/// travels as its own members, under three.js's names for them.
/// </summary>
public sealed record TextureParameters : IThreeStructure
{
	/// <summary>three.js's <c>mapping</c>.</summary>
	public AnyMapping? Mapping { get; init; }

	/// <summary>three.js's <c>wrapS</c>.</summary>
	public Wrapping? WrapS { get; init; }

	/// <summary>three.js's <c>wrapT</c>.</summary>
	public Wrapping? WrapT { get; init; }

	/// <summary>three.js's <c>wrapR</c>.</summary>
	public Wrapping? WrapR { get; init; }

	/// <summary>three.js's <c>format</c>.</summary>
	public PixelFormat? Format { get; init; }

	/// <summary>three.js's <c>internalFormat</c>.</summary>
	public PixelFormatGPU? InternalFormat { get; init; }

	/// <summary>three.js's <c>type</c>.</summary>
	public TextureDataType? Type { get; init; }

	/// <summary>three.js's <c>colorSpace</c>.</summary>
	public ColorSpace? ColorSpace { get; init; }

	/// <summary>three.js's <c>magFilter</c>.</summary>
	public MagnificationTextureFilter? MagFilter { get; init; }

	/// <summary>three.js's <c>minFilter</c>.</summary>
	public MinificationTextureFilter? MinFilter { get; init; }

	/// <summary>three.js's <c>anisotropy</c>.</summary>
	public float? Anisotropy { get; init; }

	/// <summary>three.js's <c>flipY</c>.</summary>
	public bool? FlipY { get; init; }

	/// <summary>three.js's <c>generateMipmaps</c>.</summary>
	public bool? GenerateMipmaps { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		if (Mapping is not null)
		{
			members["mapping"] = Mapping;
		}

		if (WrapS is not null)
		{
			members["wrapS"] = WrapS;
		}

		if (WrapT is not null)
		{
			members["wrapT"] = WrapT;
		}

		if (WrapR is not null)
		{
			members["wrapR"] = WrapR;
		}

		if (Format is not null)
		{
			members["format"] = Format;
		}

		if (InternalFormat is not null)
		{
			members["internalFormat"] = InternalFormat;
		}

		if (Type is not null)
		{
			members["type"] = Type;
		}

		if (ColorSpace is not null)
		{
			members["colorSpace"] = ColorSpace;
		}

		if (MagFilter is not null)
		{
			members["magFilter"] = MagFilter;
		}

		if (MinFilter is not null)
		{
			members["minFilter"] = MinFilter;
		}

		if (Anisotropy is not null)
		{
			members["anisotropy"] = Anisotropy;
		}

		if (FlipY is not null)
		{
			members["flipY"] = FlipY;
		}

		if (GenerateMipmaps is not null)
		{
			members["generateMipmaps"] = GenerateMipmaps;
		}

		return members;
	}

	/// <summary>
	/// Builds a <c>TextureParameters</c> from the members the applier sent back. A member three.js did
	/// not carry keeps this instance's own value, which for the blank instance the decoder builds is
	/// the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <param name="context">Context a member that is itself a mirrored object is adopted into.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members, ThreeContext? context)
	{
		return new TextureParameters
		{
			Mapping = members.TryGetValue("mapping", out var mappingElement) ? ThreeValue.Decode<AnyMapping?>(mappingElement, context) : Mapping,
			WrapS = members.TryGetValue("wrapS", out var wrapSElement) ? ThreeValue.Decode<Wrapping?>(wrapSElement, context) : WrapS,
			WrapT = members.TryGetValue("wrapT", out var wrapTElement) ? ThreeValue.Decode<Wrapping?>(wrapTElement, context) : WrapT,
			WrapR = members.TryGetValue("wrapR", out var wrapRElement) ? ThreeValue.Decode<Wrapping?>(wrapRElement, context) : WrapR,
			Format = members.TryGetValue("format", out var formatElement) ? ThreeValue.Decode<PixelFormat?>(formatElement, context) : Format,
			InternalFormat = members.TryGetValue("internalFormat", out var internalFormatElement) ? ThreeValue.Decode<PixelFormatGPU?>(internalFormatElement, context) : InternalFormat,
			Type = members.TryGetValue("type", out var typeElement) ? ThreeValue.Decode<TextureDataType?>(typeElement, context) : Type,
			ColorSpace = members.TryGetValue("colorSpace", out var colorSpaceElement) ? ThreeValue.Decode<ColorSpace?>(colorSpaceElement, context) : ColorSpace,
			MagFilter = members.TryGetValue("magFilter", out var magFilterElement) ? ThreeValue.Decode<MagnificationTextureFilter?>(magFilterElement, context) : MagFilter,
			MinFilter = members.TryGetValue("minFilter", out var minFilterElement) ? ThreeValue.Decode<MinificationTextureFilter?>(minFilterElement, context) : MinFilter,
			Anisotropy = members.TryGetValue("anisotropy", out var anisotropyElement) ? ThreeValue.Decode<float?>(anisotropyElement, context) : Anisotropy,
			FlipY = members.TryGetValue("flipY", out var flipYElement) ? ThreeValue.Decode<bool?>(flipYElement, context) : FlipY,
			GenerateMipmaps = members.TryGetValue("generateMipmaps", out var generateMipmapsElement) ? ThreeValue.Decode<bool?>(generateMipmapsElement, context) : GenerateMipmaps
		};
	}
}
