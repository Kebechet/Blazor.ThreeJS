// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Maps the enums three.js spells as strings between their C# values and the tokens the browser
/// compares against. Generated rather than reflective so a trimmed WebAssembly build cannot lose
/// the mapping.
/// </summary>
internal static class ThreeStringEnum
{
	/// <summary>
	/// The token a value crosses the wire as, or <see langword="null"/> when its enum is one of the
	/// numeric ones and the number is already what three.js wants.
	/// </summary>
	/// <param name="value">Any enum value.</param>
	/// <returns>The token, or <see langword="null"/> for a numeric enum.</returns>
	public static string? TokenFor(Enum value)
	{
		return value switch
		{
			AudioSourceType audioSourceType => TokenFor(audioSourceType),
			BindMode bindMode => TokenFor(bindMode),
			ColorSpace colorSpace => TokenFor(colorSpace),
			ColorSpaceTransfer colorSpaceTransfer => TokenFor(colorSpaceTransfer),
			CurveType curveType => TokenFor(curveType),
			DistanceModel distanceModel => TokenFor(distanceModel),
			GLSLVersion gLSLVersion => TokenFor(gLSLVersion),
			LineCap lineCap => TokenFor(lineCap),
			LineJoin lineJoin => TokenFor(lineJoin),
			NormalPacking normalPacking => TokenFor(normalPacking),
			PixelFormatGPU pixelFormatGPU => TokenFor(pixelFormatGPU),
			ShaderPrecision shaderPrecision => TokenFor(shaderPrecision),
			_ => null
		};
	}

	/// <summary>The value a token names, for reading a string-valued enum back out of the browser.</summary>
	/// <param name="enumType">The enum the caller is expecting.</param>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The boxed value, or <see langword="null"/> when the type is not string-valued or the token is unknown.</returns>
	public static object? FromToken(Type enumType, string token)
	{
		if (enumType == typeof(AudioSourceType))
		{
			return AudioSourceTypeFromToken(token);
		}

		if (enumType == typeof(BindMode))
		{
			return BindModeFromToken(token);
		}

		if (enumType == typeof(ColorSpace))
		{
			return ColorSpaceFromToken(token);
		}

		if (enumType == typeof(ColorSpaceTransfer))
		{
			return ColorSpaceTransferFromToken(token);
		}

		if (enumType == typeof(CurveType))
		{
			return CurveTypeFromToken(token);
		}

		if (enumType == typeof(DistanceModel))
		{
			return DistanceModelFromToken(token);
		}

		if (enumType == typeof(GLSLVersion))
		{
			return GLSLVersionFromToken(token);
		}

		if (enumType == typeof(LineCap))
		{
			return LineCapFromToken(token);
		}

		if (enumType == typeof(LineJoin))
		{
			return LineJoinFromToken(token);
		}

		if (enumType == typeof(NormalPacking))
		{
			return NormalPackingFromToken(token);
		}

		if (enumType == typeof(PixelFormatGPU))
		{
			return PixelFormatGPUFromToken(token);
		}

		if (enumType == typeof(ShaderPrecision))
		{
			return ShaderPrecisionFromToken(token);
		}

		return null;
	}

	/// <summary>The token three.js compares a <see cref="AudioSourceType"/> against.</summary>
	/// <param name="audioSourceType">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(AudioSourceType audioSourceType)
	{
		return audioSourceType switch
		{
			AudioSourceType.Empty => "empty",
			AudioSourceType.AudioNode => "audioNode",
			AudioSourceType.MediaNode => "mediaNode",
			AudioSourceType.MediaStreamNode => "mediaStreamNode",
			AudioSourceType.Buffer => "buffer",
			_ => throw new NotImplementedException($"No three.js token is known for AudioSourceType '{audioSourceType}'.")
		};
	}

	/// <summary>The <see cref="AudioSourceType"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? AudioSourceTypeFromToken(string token)
	{
		return token switch
		{
			"empty" => AudioSourceType.Empty,
			"audioNode" => AudioSourceType.AudioNode,
			"mediaNode" => AudioSourceType.MediaNode,
			"mediaStreamNode" => AudioSourceType.MediaStreamNode,
			"buffer" => AudioSourceType.Buffer,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="BindMode"/> against.</summary>
	/// <param name="bindMode">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(BindMode bindMode)
	{
		return bindMode switch
		{
			BindMode.AttachedBindMode => "attached",
			BindMode.DetachedBindMode => "detached",
			_ => throw new NotImplementedException($"No three.js token is known for BindMode '{bindMode}'.")
		};
	}

	/// <summary>The <see cref="BindMode"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? BindModeFromToken(string token)
	{
		return token switch
		{
			"attached" => BindMode.AttachedBindMode,
			"detached" => BindMode.DetachedBindMode,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="ColorSpace"/> against.</summary>
	/// <param name="colorSpace">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(ColorSpace colorSpace)
	{
		return colorSpace switch
		{
			ColorSpace.NoColorSpace => "",
			ColorSpace.SRGBColorSpace => "srgb",
			ColorSpace.LinearSRGBColorSpace => "srgb-linear",
			_ => throw new NotImplementedException($"No three.js token is known for ColorSpace '{colorSpace}'.")
		};
	}

	/// <summary>The <see cref="ColorSpace"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? ColorSpaceFromToken(string token)
	{
		return token switch
		{
			"" => ColorSpace.NoColorSpace,
			"srgb" => ColorSpace.SRGBColorSpace,
			"srgb-linear" => ColorSpace.LinearSRGBColorSpace,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="ColorSpaceTransfer"/> against.</summary>
	/// <param name="colorSpaceTransfer">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(ColorSpaceTransfer colorSpaceTransfer)
	{
		return colorSpaceTransfer switch
		{
			ColorSpaceTransfer.LinearTransfer => "linear",
			ColorSpaceTransfer.SRGBTransfer => "srgb",
			_ => throw new NotImplementedException($"No three.js token is known for ColorSpaceTransfer '{colorSpaceTransfer}'.")
		};
	}

	/// <summary>The <see cref="ColorSpaceTransfer"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? ColorSpaceTransferFromToken(string token)
	{
		return token switch
		{
			"linear" => ColorSpaceTransfer.LinearTransfer,
			"srgb" => ColorSpaceTransfer.SRGBTransfer,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="CurveType"/> against.</summary>
	/// <param name="curveType">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(CurveType curveType)
	{
		return curveType switch
		{
			CurveType.Centripetal => "centripetal",
			CurveType.Chordal => "chordal",
			CurveType.Catmullrom => "catmullrom",
			_ => throw new NotImplementedException($"No three.js token is known for CurveType '{curveType}'.")
		};
	}

	/// <summary>The <see cref="CurveType"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? CurveTypeFromToken(string token)
	{
		return token switch
		{
			"centripetal" => CurveType.Centripetal,
			"chordal" => CurveType.Chordal,
			"catmullrom" => CurveType.Catmullrom,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="DistanceModel"/> against.</summary>
	/// <param name="distanceModel">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(DistanceModel distanceModel)
	{
		return distanceModel switch
		{
			DistanceModel.Linear => "linear",
			DistanceModel.Inverse => "inverse",
			DistanceModel.Exponential => "exponential",
			_ => throw new NotImplementedException($"No three.js token is known for DistanceModel '{distanceModel}'.")
		};
	}

	/// <summary>The <see cref="DistanceModel"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? DistanceModelFromToken(string token)
	{
		return token switch
		{
			"linear" => DistanceModel.Linear,
			"inverse" => DistanceModel.Inverse,
			"exponential" => DistanceModel.Exponential,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="GLSLVersion"/> against.</summary>
	/// <param name="gLSLVersion">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(GLSLVersion gLSLVersion)
	{
		return gLSLVersion switch
		{
			GLSLVersion.GLSL1 => "100",
			GLSLVersion.GLSL3 => "300 es",
			_ => throw new NotImplementedException($"No three.js token is known for GLSLVersion '{gLSLVersion}'.")
		};
	}

	/// <summary>The <see cref="GLSLVersion"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? GLSLVersionFromToken(string token)
	{
		return token switch
		{
			"100" => GLSLVersion.GLSL1,
			"300 es" => GLSLVersion.GLSL3,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="LineCap"/> against.</summary>
	/// <param name="lineCap">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(LineCap lineCap)
	{
		return lineCap switch
		{
			LineCap.Butt => "butt",
			LineCap.Round => "round",
			LineCap.Square => "square",
			_ => throw new NotImplementedException($"No three.js token is known for LineCap '{lineCap}'.")
		};
	}

	/// <summary>The <see cref="LineCap"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? LineCapFromToken(string token)
	{
		return token switch
		{
			"butt" => LineCap.Butt,
			"round" => LineCap.Round,
			"square" => LineCap.Square,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="LineJoin"/> against.</summary>
	/// <param name="lineJoin">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(LineJoin lineJoin)
	{
		return lineJoin switch
		{
			LineJoin.Round => "round",
			LineJoin.Bevel => "bevel",
			LineJoin.Miter => "miter",
			_ => throw new NotImplementedException($"No three.js token is known for LineJoin '{lineJoin}'.")
		};
	}

	/// <summary>The <see cref="LineJoin"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? LineJoinFromToken(string token)
	{
		return token switch
		{
			"round" => LineJoin.Round,
			"bevel" => LineJoin.Bevel,
			"miter" => LineJoin.Miter,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="NormalPacking"/> against.</summary>
	/// <param name="normalPacking">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(NormalPacking normalPacking)
	{
		return normalPacking switch
		{
			NormalPacking.NoNormalPacking => "",
			NormalPacking.NormalRGPacking => "rg",
			NormalPacking.NormalGAPacking => "ga",
			_ => throw new NotImplementedException($"No three.js token is known for NormalPacking '{normalPacking}'.")
		};
	}

	/// <summary>The <see cref="NormalPacking"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? NormalPackingFromToken(string token)
	{
		return token switch
		{
			"" => NormalPacking.NoNormalPacking,
			"rg" => NormalPacking.NormalRGPacking,
			"ga" => NormalPacking.NormalGAPacking,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="PixelFormatGPU"/> against.</summary>
	/// <param name="pixelFormatGPU">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(PixelFormatGPU pixelFormatGPU)
	{
		return pixelFormatGPU switch
		{
			PixelFormatGPU.ALPHA => "ALPHA",
			PixelFormatGPU.RGB => "RGB",
			PixelFormatGPU.RGBA => "RGBA",
			PixelFormatGPU.LUMINANCE => "LUMINANCE",
			PixelFormatGPU.LUMINANCE_ALPHA => "LUMINANCE_ALPHA",
			PixelFormatGPU.RED_INTEGER => "RED_INTEGER",
			PixelFormatGPU.R8 => "R8",
			PixelFormatGPU.R8_SNORM => "R8_SNORM",
			PixelFormatGPU.R8I => "R8I",
			PixelFormatGPU.R8UI => "R8UI",
			PixelFormatGPU.R16I => "R16I",
			PixelFormatGPU.R16UI => "R16UI",
			PixelFormatGPU.R16F => "R16F",
			PixelFormatGPU.R32I => "R32I",
			PixelFormatGPU.R32UI => "R32UI",
			PixelFormatGPU.R32F => "R32F",
			PixelFormatGPU.RG8 => "RG8",
			PixelFormatGPU.RG8_SNORM => "RG8_SNORM",
			PixelFormatGPU.RG8I => "RG8I",
			PixelFormatGPU.RG8UI => "RG8UI",
			PixelFormatGPU.RG16I => "RG16I",
			PixelFormatGPU.RG16UI => "RG16UI",
			PixelFormatGPU.RG16F => "RG16F",
			PixelFormatGPU.RG32I => "RG32I",
			PixelFormatGPU.RG32UI => "RG32UI",
			PixelFormatGPU.RG32F => "RG32F",
			PixelFormatGPU.RGB565 => "RGB565",
			PixelFormatGPU.RGB8 => "RGB8",
			PixelFormatGPU.RGB8_SNORM => "RGB8_SNORM",
			PixelFormatGPU.RGB8I => "RGB8I",
			PixelFormatGPU.RGB8UI => "RGB8UI",
			PixelFormatGPU.RGB16I => "RGB16I",
			PixelFormatGPU.RGB16UI => "RGB16UI",
			PixelFormatGPU.RGB16F => "RGB16F",
			PixelFormatGPU.RGB32I => "RGB32I",
			PixelFormatGPU.RGB32UI => "RGB32UI",
			PixelFormatGPU.RGB32F => "RGB32F",
			PixelFormatGPU.RGB9_E5 => "RGB9_E5",
			PixelFormatGPU.SRGB8 => "SRGB8",
			PixelFormatGPU.R11F_G11F_B10F => "R11F_G11F_B10F",
			PixelFormatGPU.RGBA4 => "RGBA4",
			PixelFormatGPU.RGBA8 => "RGBA8",
			PixelFormatGPU.RGBA8_SNORM => "RGBA8_SNORM",
			PixelFormatGPU.RGBA8I => "RGBA8I",
			PixelFormatGPU.RGBA8UI => "RGBA8UI",
			PixelFormatGPU.RGBA16I => "RGBA16I",
			PixelFormatGPU.RGBA16UI => "RGBA16UI",
			PixelFormatGPU.RGBA16F => "RGBA16F",
			PixelFormatGPU.RGBA32I => "RGBA32I",
			PixelFormatGPU.RGBA32UI => "RGBA32UI",
			PixelFormatGPU.RGBA32F => "RGBA32F",
			PixelFormatGPU.RGB5_A1 => "RGB5_A1",
			PixelFormatGPU.RGB10_A2 => "RGB10_A2",
			PixelFormatGPU.RGB10_A2UI => "RGB10_A2UI",
			PixelFormatGPU.SRGB8_ALPHA8 => "SRGB8_ALPHA8",
			PixelFormatGPU.DEPTH_COMPONENT16 => "DEPTH_COMPONENT16",
			PixelFormatGPU.DEPTH_COMPONENT24 => "DEPTH_COMPONENT24",
			PixelFormatGPU.DEPTH_COMPONENT32F => "DEPTH_COMPONENT32F",
			PixelFormatGPU.DEPTH24_STENCIL8 => "DEPTH24_STENCIL8",
			PixelFormatGPU.DEPTH32F_STENCIL8 => "DEPTH32F_STENCIL8",
			_ => throw new NotImplementedException($"No three.js token is known for PixelFormatGPU '{pixelFormatGPU}'.")
		};
	}

	/// <summary>The <see cref="PixelFormatGPU"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? PixelFormatGPUFromToken(string token)
	{
		return token switch
		{
			"ALPHA" => PixelFormatGPU.ALPHA,
			"RGB" => PixelFormatGPU.RGB,
			"RGBA" => PixelFormatGPU.RGBA,
			"LUMINANCE" => PixelFormatGPU.LUMINANCE,
			"LUMINANCE_ALPHA" => PixelFormatGPU.LUMINANCE_ALPHA,
			"RED_INTEGER" => PixelFormatGPU.RED_INTEGER,
			"R8" => PixelFormatGPU.R8,
			"R8_SNORM" => PixelFormatGPU.R8_SNORM,
			"R8I" => PixelFormatGPU.R8I,
			"R8UI" => PixelFormatGPU.R8UI,
			"R16I" => PixelFormatGPU.R16I,
			"R16UI" => PixelFormatGPU.R16UI,
			"R16F" => PixelFormatGPU.R16F,
			"R32I" => PixelFormatGPU.R32I,
			"R32UI" => PixelFormatGPU.R32UI,
			"R32F" => PixelFormatGPU.R32F,
			"RG8" => PixelFormatGPU.RG8,
			"RG8_SNORM" => PixelFormatGPU.RG8_SNORM,
			"RG8I" => PixelFormatGPU.RG8I,
			"RG8UI" => PixelFormatGPU.RG8UI,
			"RG16I" => PixelFormatGPU.RG16I,
			"RG16UI" => PixelFormatGPU.RG16UI,
			"RG16F" => PixelFormatGPU.RG16F,
			"RG32I" => PixelFormatGPU.RG32I,
			"RG32UI" => PixelFormatGPU.RG32UI,
			"RG32F" => PixelFormatGPU.RG32F,
			"RGB565" => PixelFormatGPU.RGB565,
			"RGB8" => PixelFormatGPU.RGB8,
			"RGB8_SNORM" => PixelFormatGPU.RGB8_SNORM,
			"RGB8I" => PixelFormatGPU.RGB8I,
			"RGB8UI" => PixelFormatGPU.RGB8UI,
			"RGB16I" => PixelFormatGPU.RGB16I,
			"RGB16UI" => PixelFormatGPU.RGB16UI,
			"RGB16F" => PixelFormatGPU.RGB16F,
			"RGB32I" => PixelFormatGPU.RGB32I,
			"RGB32UI" => PixelFormatGPU.RGB32UI,
			"RGB32F" => PixelFormatGPU.RGB32F,
			"RGB9_E5" => PixelFormatGPU.RGB9_E5,
			"SRGB8" => PixelFormatGPU.SRGB8,
			"R11F_G11F_B10F" => PixelFormatGPU.R11F_G11F_B10F,
			"RGBA4" => PixelFormatGPU.RGBA4,
			"RGBA8" => PixelFormatGPU.RGBA8,
			"RGBA8_SNORM" => PixelFormatGPU.RGBA8_SNORM,
			"RGBA8I" => PixelFormatGPU.RGBA8I,
			"RGBA8UI" => PixelFormatGPU.RGBA8UI,
			"RGBA16I" => PixelFormatGPU.RGBA16I,
			"RGBA16UI" => PixelFormatGPU.RGBA16UI,
			"RGBA16F" => PixelFormatGPU.RGBA16F,
			"RGBA32I" => PixelFormatGPU.RGBA32I,
			"RGBA32UI" => PixelFormatGPU.RGBA32UI,
			"RGBA32F" => PixelFormatGPU.RGBA32F,
			"RGB5_A1" => PixelFormatGPU.RGB5_A1,
			"RGB10_A2" => PixelFormatGPU.RGB10_A2,
			"RGB10_A2UI" => PixelFormatGPU.RGB10_A2UI,
			"SRGB8_ALPHA8" => PixelFormatGPU.SRGB8_ALPHA8,
			"DEPTH_COMPONENT16" => PixelFormatGPU.DEPTH_COMPONENT16,
			"DEPTH_COMPONENT24" => PixelFormatGPU.DEPTH_COMPONENT24,
			"DEPTH_COMPONENT32F" => PixelFormatGPU.DEPTH_COMPONENT32F,
			"DEPTH24_STENCIL8" => PixelFormatGPU.DEPTH24_STENCIL8,
			"DEPTH32F_STENCIL8" => PixelFormatGPU.DEPTH32F_STENCIL8,
			_ => null
		};
	}

	/// <summary>The token three.js compares a <see cref="ShaderPrecision"/> against.</summary>
	/// <param name="shaderPrecision">The value to send.</param>
	/// <returns>The token.</returns>
	private static string TokenFor(ShaderPrecision shaderPrecision)
	{
		return shaderPrecision switch
		{
			ShaderPrecision.Highp => "highp",
			ShaderPrecision.Mediump => "mediump",
			ShaderPrecision.Lowp => "lowp",
			_ => throw new NotImplementedException($"No three.js token is known for ShaderPrecision '{shaderPrecision}'.")
		};
	}

	/// <summary>The <see cref="ShaderPrecision"/> a token names.</summary>
	/// <param name="token">The token the browser sent.</param>
	/// <returns>The value, or <see langword="null"/> when three.js sent something this build does not know.</returns>
	private static object? ShaderPrecisionFromToken(string token)
	{
		return token switch
		{
			"highp" => ShaderPrecision.Highp,
			"mediump" => ShaderPrecision.Mediump,
			"lowp" => ShaderPrecision.Lowp,
			_ => null
		};
	}
}
