using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

/// <summary>
/// The enums three.js spells as strings, across the wire they actually cross.
/// <para>
/// Every failure this file guards against is silent. A string-valued enum sent as its C# ordinal is
/// a number three.js compares against its own strings, matches against nothing, and shrugs off by
/// keeping whatever default the object already had — no exception, no console error, just a texture
/// that is quietly in the wrong colour space. So each assertion is on the serialized JSON rather than
/// on the encoder's return value, because only the JSON shows whether the browser receives
/// <c>"srgb"</c> or <c>1</c>.
/// </para>
/// </summary>
public class StringEnumWireFormatTests
{
	private static readonly JsonSerializerOptions _webOptions = new(JsonSerializerDefaults.Web);

	[Fact]
	public void ThreeValue_StringValuedEnumEncoded_IsTheTokenThreeJsCompares()
	{
		// Arrange & Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(ColorSpace.SRGBColorSpace), _webOptions);

		// Assert
		json.ShouldBe("\"srgb\"");
	}

	/// <summary>
	/// three.js spells "no colour space" as the empty string, and a C# enum spells it as 0. Those are
	/// the same member and completely different wire values, so the member whose ordinal collides with
	/// a plausible number is the one most worth pinning.
	/// </summary>
	[Fact]
	public void ThreeValue_StringValuedEnumWithEmptyToken_IsAnEmptyStringNotZero()
	{
		// Arrange & Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(ColorSpace.NoColorSpace), _webOptions);

		// Assert
		json.ShouldBe("\"\"");
		json.ShouldNotBe("0");
	}

	/// <summary>
	/// The numeric enums must keep crossing as numbers. Encoding reaches both kinds through one
	/// <c>case Enum</c> arm, so a change that sent tokens unconditionally would still compile and would
	/// break every side, blending mode and texture filter in the package at once.
	/// </summary>
	[Fact]
	public void ThreeValue_NumericEnumEncoded_IsStillTheNumberThreeJsUses()
	{
		// Arrange & Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(Side.DoubleSide), _webOptions);

		// Assert
		json.ShouldBe(((int) Side.DoubleSide).ToString());
	}

	[Fact]
	public void ThreeValue_StringValuedEnumRoundTripped_ComesBackAsTheSameMember()
	{
		// Arrange
		var json = JsonSerializer.Serialize(ThreeValue.Encode(ColorSpace.LinearSRGBColorSpace), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<ColorSpace>(JsonSerializer.Deserialize<JsonElement>(json));

		// Assert
		decoded.ShouldBe(ColorSpace.LinearSRGBColorSpace);
	}

	/// <summary>
	/// <c>GLSLVersion.GLSL1</c>'s token is <c>"100"</c> — a string that reads as a number. Decoding has
	/// to resolve it by token, because anything that parsed the string as a number would land on
	/// whichever member happened to have that ordinal, or on none.
	/// </summary>
	[Fact]
	public void ThreeValue_TokenThatLooksNumericDecoded_ResolvesByTokenNotByValue()
	{
		// Arrange
		var json = JsonSerializer.Serialize(ThreeValue.Encode(GLSLVersion.GLSL1), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<GLSLVersion>(JsonSerializer.Deserialize<JsonElement>(json));

		// Assert
		json.ShouldBe("\"100\"");
		decoded.ShouldBe(GLSLVersion.GLSL1);
		((int) GLSLVersion.GLSL1).ShouldNotBe(100);
	}

	/// <summary>
	/// An enum the table knows at all, it knows completely. The enums and the token table are produced
	/// by two separate emitters from one catalog, so a member could reach the enum and not the table —
	/// and that member alone would silently cross as its ordinal while its neighbours crossed correctly.
	/// <para>
	/// Discovered by reflection rather than listed, so an enum added upstream is covered the moment it
	/// generates instead of when someone remembers to extend an <c>InlineData</c> list.
	/// </para>
	/// </summary>
	[Fact]
	public void ThreeStringEnum_EveryEnumItKnows_HasATokenForEveryMemberThatRoundTrips()
	{
		// Arrange
		var stringValuedEnumTypes = typeof(ColorSpace).Assembly
			.GetExportedTypes()
			.Where(x => x.IsEnum)
			.Where(x => Enum.GetValues(x).Cast<Enum>().Any(value => ThreeStringEnum.TokenFor(value) is not null))
			.ToList();

		// Act & Assert
		// Named rather than merely counted: discovery asks the table what it knows, so an enum the
		// table forgot entirely would not be discovered and every assertion below would pass over it.
		// This is the floor that turns that silence into a failure.
		stringValuedEnumTypes
			.Select(x => x.Name)
			.OrderBy(x => x, StringComparer.Ordinal)
			.ShouldBe([
				"BindMode", "ColorSpace", "ColorSpaceTransfer", "CurveType", "GLSLVersion",
				"LineCap", "LineJoin", "NormalPacking", "PixelFormatGPU"
			]);

		foreach (var enumType in stringValuedEnumTypes)
		{
			foreach (var member in Enum.GetValues(enumType).Cast<Enum>())
			{
				var token = ThreeStringEnum.TokenFor(member);
				token.ShouldNotBeNull($"{enumType.Name}.{member} has no wire token, so it would cross as its ordinal.");
				ThreeStringEnum.FromToken(enumType, token).ShouldBe(member);
			}
		}
	}

	/// <summary>
	/// The enums synthesised from inline string-literal unions are shared by token set, not minted per
	/// member. three.js declares <c>"round" | "bevel" | "miter"</c> on fourteen materials; one enum per
	/// declaration site would be fourteen incompatible types for one set of three values.
	/// </summary>
	[Fact]
	public void SynthesisedEnum_SameTokenSetOnDifferentMembers_IsOneSharedType()
	{
		// Arrange
		var lineMaterial = new LineBasicMaterial();
		var meshMaterial = new MeshBasicMaterial();

		// Act & Assert
		lineMaterial.Linejoin.GetType().ShouldBe(meshMaterial.WireframeLinejoin.GetType());
		lineMaterial.Linecap.GetType().ShouldBe(typeof(LineCap));
	}

	/// <summary>
	/// <c>@types/three</c> lists <c>"SRGB8"</c> twice in the <c>PixelFormatGPU</c> union. One token
	/// repeated is upstream saying the same thing twice, not two members, and treating it as a second
	/// spelling emits <c>SRGB8 = SRGB8</c> — which does not compile, so the whole package fails to
	/// build the moment any enum hits it.
	/// </summary>
	[Fact]
	public void SynthesisedEnum_TokenRepeatedUpstream_YieldsOneMember()
	{
		// Arrange & Act
		var members = Enum.GetNames<PixelFormatGPU>();

		// Assert
		members.Count(x => x == nameof(PixelFormatGPU.SRGB8)).ShouldBe(1);
		members.ShouldContain(nameof(PixelFormatGPU.SRGB8_ALPHA8));
		ThreeStringEnum.TokenFor(PixelFormatGPU.SRGB8).ShouldBe("SRGB8");
	}

	/// <summary>
	/// A string-literal union stays an enum and does not decay to <see cref="string"/>.
	/// <para>
	/// The mapper collapses a union whose arms all resolve to one C# type, on the grounds that such a
	/// union is TypeScript spelling one thing twice. Every arm of <c>"round" | "bevel" | "miter"</c>
	/// does resolve to <c>string</c>, so without an explicit carve-out that rule swallows the closed
	/// set: the enums here silently become strings, and a new set upstream never reaches the coverage
	/// report as a decision to make. Asserting the property's declared type is what catches that,
	/// because the member count goes <em>up</em> when it happens.
	/// </para>
	/// </summary>
	[Fact]
	public void SynthesisedEnum_ArmsAllResolvingToString_StaysAnEnumRatherThanCollapsing()
	{
		// Arrange & Act
		var wireframeLinejoin = typeof(MeshStandardMaterial).GetProperty(nameof(MeshStandardMaterial.WireframeLinejoin));
		var linecap = typeof(LineBasicMaterial).GetProperty(nameof(LineBasicMaterial.Linecap));

		// Assert
		wireframeLinejoin?.PropertyType.ShouldBe(typeof(LineJoin));
		linecap?.PropertyType.ShouldBe(typeof(LineCap));
	}

	/// <summary>
	/// A synthesised enum crosses as its token like any other string-valued one. Worth its own
	/// assertion because these reach the wire by a different route — mapped from an inline union rather
	/// than resolved from a named alias — and only the encoder's behaviour proves the two meet.
	/// </summary>
	[Fact]
	public void ThreeValue_SynthesisedEnumEncoded_IsTheTokenThreeJsCompares()
	{
		// Arrange & Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(LineJoin.Bevel), _webOptions);

		// Assert
		json.ShouldBe("\"bevel\"");
	}
}
