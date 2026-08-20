using System.Text.Json;
using System.Text.Json.Nodes;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Shouldly;

// three.js has its own `Path` — a 2D curve — and the generated mirror carries it, so the unqualified
// name is ambiguous in any file importing both this namespace and System.IO.
using Path = System.IO.Path;

namespace Blazor.ThreeJS.Tests.Core;

/// <summary>
/// Round-trips every hand-written math type through the wire encoding it will actually cross the
/// interop boundary in: <c>ThreeValue.Encode</c>, real JSON serialization, then
/// <c>ThreeValue.Decode</c>.
/// <para>
/// Encoding and decoding are separate switch statements with no compiler relationship, so a type with
/// an encode arm and no decode arm compiles, ships, and fails only when a consumer reads that member
/// back. <c>MathTypesUnderTest</c> is the list both halves are checked against, and
/// <c>ThreeValue_EveryHandWrittenMathType_IsUnderTest</c> fails when a new type is added to
/// <c>Math/</c> without being added here.
/// </para>
/// </summary>
public class MathValueWireFormatTests
{
	private static readonly JsonSerializerOptions _webOptions = new(JsonSerializerDefaults.Web);
	private const string FixtureFileName = "math-values-fixture.json";

	/// <summary>
	/// One populated instance of every math type, each carrying values distinct enough that a
	/// component written into the wrong slot changes the result.
	/// </summary>
	public static TheoryData<string, object> MathTypesUnderTest()
	{
		return new TheoryData<string, object>
		{
			{ nameof(Vector2), new Vector2(1f, 2f) },
			{ nameof(Vector3), new Vector3(1f, 2f, 3f) },
			{ nameof(Vector4), new Vector4(1f, 2f, 3f, 4f) },
			{ nameof(Quaternion), new Quaternion(0.1f, 0.2f, 0.3f, 0.4f) },
			{ nameof(Euler), new Euler(0.1f, 0.2f, 0.3f, EulerOrder.ZYX) },
			{ nameof(Color), new Color(0.25f, 0.5f, 0.75f) },
			{ nameof(Matrix2), new Matrix2().FromArray([1f, 2f, 3f, 4f]) },
			{ nameof(Matrix3), new Matrix3().FromArray([1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f]) },
			{ nameof(Matrix4), BuildMatrix4() },
			{ nameof(Box2), new Box2(new Vector2(-1f, -2f), new Vector2(3f, 4f)) },
			{ nameof(Box3), new Box3(new Vector3(-1f, -2f, -3f), new Vector3(4f, 5f, 6f)) },
			{ nameof(Sphere), new Sphere(new Vector3(1f, 2f, 3f), 4f) },
			{ nameof(Plane), new Plane(new Vector3(0f, 1f, 0f), 5f) },
			{ nameof(Ray), new Ray(new Vector3(1f, 2f, 3f), new Vector3(0f, 0f, 1f)) },
			{ nameof(Line3), new Line3(new Vector3(1f, 2f, 3f), new Vector3(4f, 5f, 6f)) },
			{ nameof(Triangle), new Triangle(new Vector3(1f, 2f, 3f), new Vector3(4f, 5f, 6f), new Vector3(7f, 8f, 9f)) },
			{ nameof(Spherical), new Spherical(1f, 2f, 3f) },
			{ nameof(Cylindrical), new Cylindrical(1f, 2f, 3f) },
			{ nameof(Frustum), BuildFrustum() },
			{ nameof(SphericalHarmonics3), BuildSphericalHarmonics() }
		};
	}

	[Theory]
	[MemberData(nameof(MathTypesUnderTest))]
	public void ThreeValue_MathValueEncodedAndDecoded_SurvivesTheRoundTrip(string typeName, object value)
	{
		// Arrange
		var encoded = ThreeValue.Encode(value);

		// Act
		var element = JsonSerializer.SerializeToElement(encoded, _webOptions);
		var decoded = Decode(value.GetType(), element);

		// Assert
		decoded.ShouldNotBeNull(typeName);
		decoded.GetType().ShouldBe(value.GetType());
		Components(decoded).ShouldBe(Components(value), $"{typeName} lost or reordered components on the wire");
	}

	[Theory]
	[MemberData(nameof(MathTypesUnderTest))]
	public void ThreeValue_MathValueEncoded_MatchesTheSharedFixtureFile(string typeName, object value)
	{
		// Arrange
		var fixtureJson = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, FixtureFileName));
		var expected = JsonNode.Parse(fixtureJson)![typeName];

		// Act
		var actual = JsonNode.Parse(JsonSerializer.Serialize(ThreeValue.Encode(value), _webOptions));

		// Assert
		// The JavaScript half of this contract test reads the same file and rebuilds each value against
		// the real three.js, so a component order that disagrees between the two sides fails there
		// rather than silently transposing a matrix on the way across.
		expected.ShouldNotBeNull($"{typeName} is missing from {FixtureFileName}");
		actual!.ToJsonString().ShouldBe(expected!.ToJsonString());
	}

	[Fact]
	public void ThreeValue_EveryHandWrittenMathType_IsUnderTest()
	{
		// Arrange
		var testedNames = MathTypesUnderTest()
			.Select(x => x.Data.Item1)
			.ToHashSet(StringComparer.Ordinal);

		// Act
		// Discovered from the assembly rather than listed, so a newly ported type is caught here
		// instead of shipping with no wire coverage at all.
		var shippedNames = typeof(Vector3).Assembly
			.GetTypes()
			.Where(x => x.IsPublic && x.Namespace == typeof(Vector3).Namespace && x.IsClass)
			.Select(x => x.Name)
			.ToList();

		// Assert
		shippedNames.ShouldNotBeEmpty();
		shippedNames.Where(x => !testedNames.Contains(x)).ShouldBeEmpty();
	}

	[Fact]
	public void ThreeValue_EmptyBox3Encoded_CarriesInfinityAsAToken()
	{
		// Arrange
		var box = new Box3();

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(box), _webOptions);

		// Assert
		// JSON has no numeric infinity, and Utf8JsonWriter throws rather than inventing one. A
		// default Box3 is exactly this case, so without the token encoding it could not be sent at all.
		json.ShouldBe("""{"$t":"Box3","v":["Infinity","Infinity","Infinity","-Infinity","-Infinity","-Infinity"]}""");
	}

	[Theory]
	[InlineData(float.PositiveInfinity)]
	[InlineData(float.NegativeInfinity)]
	[InlineData(float.NaN)]
	public void ThreeValue_NonFiniteComponentEncodedAndDecoded_SurvivesTheRoundTrip(float component)
	{
		// Arrange
		var vector = new Vector3(component, 1f, 2f);

		// Act
		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(vector), _webOptions);
		var decoded = ThreeValue.Decode<Vector3>(element);

		// Assert
		decoded.X.ShouldBe(component);
		decoded.Y.ShouldBe(1f);
	}

	[Theory]
	[InlineData(float.PositiveInfinity, "Infinity")]
	[InlineData(float.NegativeInfinity, "-Infinity")]
	[InlineData(float.NaN, "NaN")]
	public void ThreeValue_NonFiniteScalarEncoded_IsTaggedRatherThanBare(float value, string expectedToken)
	{
		// Arrange & Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(value), _webOptions);

		// Assert
		// Tagged, not a bare string: `Set("name", "Infinity")` is a legitimate string write, and a bare
		// token would be indistinguishable from it. Without this the write throws outright —
		// Utf8JsonWriter refuses a non-finite number — which is what `AnimationAction.repetitions`
		// hits, since infinity is three.js's own way of saying "loop forever".
		json.ShouldBe($$"""{"$n":"{{expectedToken}}"}""");
	}

	[Theory]
	[InlineData(float.PositiveInfinity)]
	[InlineData(float.NegativeInfinity)]
	[InlineData(float.NaN)]
	public void ThreeValue_NonFiniteScalarDecoded_SurvivesTheRoundTrip(float value)
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(value), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<float>(element);

		// Assert
		decoded.ShouldBe(value);
	}

	[Fact]
	public void ThreeValue_DecodeGivenAnUnknownComponentToken_Throws()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(new Dictionary<string, object>
		{
			["$t"] = "Vector3",
			["v"] = new object[] { "Enormous", 1f, 2f }
		}, _webOptions);

		// Act
		var exception = Record.Exception(() => ThreeValue.Decode<Vector3>(element));

		// Assert
		// A string component that names no token is a divergence between the two sides, not a value.
		// Guessing a number for it would fabricate an answer the caller cannot tell from a real one.
		exception.ShouldBeOfType<NotSupportedException>();
	}

	private static Matrix4 BuildMatrix4()
	{
		var matrix = new Matrix4();
		matrix.Set(
			1f, 2f, 3f, 4f,
			5f, 6f, 7f, 8f,
			9f, 10f, 11f, 12f,
			13f, 14f, 15f, 16f);

		return matrix;
	}

	private static Frustum BuildFrustum()
	{
		var values = new float[Frustum.PlaneCount * 4];
		for (var index = 0; index < values.Length; index++)
		{
			values[index] = index + 1;
		}

		return new Frustum().FromArray(values);
	}

	private static SphericalHarmonics3 BuildSphericalHarmonics()
	{
		var values = new float[SphericalHarmonics3.CoefficientCount * 3];
		for (var index = 0; index < values.Length; index++)
		{
			values[index] = index + 1;
		}

		return new SphericalHarmonics3().FromArray(values);
	}

	/// <summary>
	/// Decodes through the generic <c>ThreeValue.Decode&lt;T&gt;</c> the generated code calls, with the
	/// type supplied at run time so one theory can cover every math type.
	/// </summary>
	private static object? Decode(Type mathType, JsonElement element)
	{
		var decode = typeof(ThreeValue)
			.GetMethod(nameof(ThreeValue.Decode))!
			.MakeGenericMethod(mathType);

		// Both arguments, including the optional context: MethodInfo.Invoke binds positionally and does
		// not apply C# optional-parameter defaults. A math value needs no context - only a structure
		// carrying a mirrored object does - so it is passed as null.
		return decode.Invoke(null, [(JsonElement?) element, null]);
	}

	/// <summary>Reads a math value's components through its own <c>ToArray</c>, whatever its type.</summary>
	private static float[] Components(object value)
	{
		return (float[]) value.GetType().GetMethod("ToArray", Type.EmptyTypes)!.Invoke(value, null)!;
	}
}
