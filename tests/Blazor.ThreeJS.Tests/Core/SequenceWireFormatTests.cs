using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

/// <summary>
/// Pins how arrays and typed arrays cross the wire. Both are encoded element by element rather than
/// as their own serialized shape, which is what lets an array of mirrored objects travel as handles
/// and an array of math values as tagged values.
/// </summary>
public class SequenceWireFormatTests
{
	private static readonly JsonSerializerOptions _webOptions = new(JsonSerializerDefaults.Web);

	[Fact]
	public void ThreeValue_NumberArrayEncoded_IsAPlainJsonArray()
	{
		// Arrange
		float[] values = [1f, 2.5f, -3f];

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(values), _webOptions);

		// Assert
		json.ShouldBe("[1,2.5,-3]");
	}

	[Fact]
	public void ThreeValue_EmptyArrayEncoded_IsAnEmptyJsonArray()
	{
		// Arrange & Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(Array.Empty<float>()), _webOptions);

		// Assert
		json.ShouldBe("[]");
	}

	[Fact]
	public void ThreeValue_StringArrayEncoded_KeepsEachStringWhole()
	{
		// Arrange
		string[] names = ["head", "spine"];

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(names), _webOptions);

		// Assert
		// A string is itself a sequence of characters, so the string arm has to be reached before the
		// sequence arm or each name would arrive as an array of one-character strings.
		json.ShouldBe("""["head","spine"]""");
	}

	[Fact]
	public void ThreeValue_MathValueArrayEncoded_TagsEachElement()
	{
		// Arrange
		Vector3[] points = [new(1f, 2f, 3f), new(4f, 5f, 6f)];

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(points), _webOptions);

		// Assert
		json.ShouldBe("""[{"$t":"Vector3","v":[1,2,3]},{"$t":"Vector3","v":[4,5,6]}]""");
	}

	[Fact]
	public void ThreeValue_MirroredObjectArrayEncoded_ReferencesEachByHandle()
	{
		// Arrange
		var context = new ThreeContext(new RecordingJsObjectReference(), contextId: 1);
		var first = new Group();
		var second = new Group();
		context.Attach(first);
		context.Attach(second);

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(new[] { first, second }), _webOptions);

		// Assert
		// Handles, not the objects' own shape: the applier resolves each through its handle table, so
		// the array costs two integers however large the objects behind them are.
		json.ShouldBe($$"""[{"$ref":{{first.Handle}}},{"$ref":{{second.Handle}}}]""");
	}

	[Fact]
	public void ThreeValue_TypedArrayEncoded_NamesItsJavaScriptConstructor()
	{
		// Arrange
		var vertices = new Float32Array(0f, 1f, 2f);

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(vertices), _webOptions);

		// Assert
		// three.js hands a BufferAttribute's array straight to WebGL, so the applier has to rebuild the
		// exact constructor rather than a plain Array of numbers.
		json.ShouldBe("""{"$ta":"Float32Array","v":[0,1,2]}""");
	}

	[Fact]
	public void ThreeValue_TypedArrayWithNonFiniteElement_CarriesTheToken()
	{
		// Arrange
		var values = new Float64Array(1d, double.PositiveInfinity, double.NaN);

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(values), _webOptions);

		// Assert
		json.ShouldBe("""{"$ta":"Float64Array","v":[1,"Infinity","NaN"]}""");
	}

	[Fact]
	public void ThreeValue_TypedArrayMutatedAfterConstruction_DoesNotChangeWhatWasEncoded()
	{
		// Arrange
		float[] source = [1f, 2f];
		var vertices = new Float32Array(source);

		// Act
		source[0] = 99f;

		// Assert
		// The constructor copies, so a caller reusing its own buffer cannot retroactively change a
		// payload that has already been handed to the encoder.
		vertices.Values.ShouldBe([1f, 2f]);
	}

	[Fact]
	public void ThreeValue_TaggedArrayDecoded_RebuildsEachElement()
	{
		// Arrange
		// Exactly what `Curve.getPoints` answers with, which is where this was first missed: the array
		// arrived, the plain deserializer bound none of the tagged fields, and every point came back
		// (0, 0) with no error anywhere. A curve drawn from them collapses to a dot.
		var element = JsonSerializer.SerializeToElement(
			ThreeValue.Encode(new Vector2[] { new(1f, 2f), new(3f, 4f) }),
			_webOptions);

		// Act
		var decoded = ThreeValue.Decode<Vector2[]>(element);

		// Assert
		decoded.Length.ShouldBe(2);
		decoded[0].ToArray().ShouldBe([1f, 2f]);
		decoded[1].ToArray().ShouldBe([3f, 4f]);
	}

	[Fact]
	public void ThreeValue_NumberArrayDecoded_RebuildsTheElements()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(new[] { 1f, 2.5f }), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<float[]>(element);

		// Assert
		decoded.ShouldBe([1f, 2.5f]);
	}

	[Fact]
	public void ThreeValue_TypedArrayDecoded_RebuildsTheMatchingClass()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(new Float32Array(1f, 2f, 3f)), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<Float32Array>(element);

		// Assert
		decoded.Values.ShouldBe([1f, 2f, 3f]);
	}

	[Fact]
	public void ThreeValue_UnencodableElement_NamesTheElementTypeRatherThanTheArray()
	{
		// Arrange
		var values = new object[] { 1f, new StringBuilderStandIn() };

		// Act
		var exception = Record.Exception(() => ThreeValue.Encode(values));

		// Assert
		// Elements go through Encode individually, so the failure names the type that actually has no
		// encoding instead of the array that happened to contain it.
		exception.ShouldBeOfType<NotSupportedException>();
		exception.Message.ShouldContain(nameof(StringBuilderStandIn));
	}

	/// <summary>A reference type with no wire encoding, standing in for anything a caller might pass by mistake.</summary>
	private sealed class StringBuilderStandIn
	{
	}
}
