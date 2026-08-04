using System.Text.Json;
using System.Text.Json.Nodes;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

/// <summary>
/// Pins the JSON that crosses the interop boundary, byte for byte. Nothing else in the suite
/// serializes a <see cref="ThreeOp"/>, so without this the contract with <c>three-interop.js</c>
/// held only by coincidence: a string-enum converter reaching these options would turn
/// <c>"k":1</c> into <c>"k":"Set"</c>, every op would fall into the applier's <c>default</c> arm,
/// and the C# side would still compile and pass.
/// <para>
/// The batch built by <c>BuildFixtureOps</c> is mirrored by <c>tests/wire-format-fixture.json</c>,
/// which the JavaScript half of this contract test consumes. Run that half with:
/// <c>node tests/wire-format.test.mjs</c> (from the repository root; no npm install needed, it
/// drives the vendored three.js).
/// </para>
/// </summary>
public class ThreeWireFormatTests
{
	private static readonly JsonSerializerOptions _webOptions = new(JsonSerializerDefaults.Web);
	private const string FixtureFileName = "wire-format-fixture.json";

	[Fact]
	public void ThreeOp_CreateOpSerialized_MatchesWireContract()
	{
		// Arrange
		var createOp = BuildFixtureOps().First(x => x.Kind == ThreeOpKind.Create);

		// Act
		var json = JsonSerializer.Serialize(createOp, _webOptions);

		// Assert
		json.ShouldBe("""{"k":0,"h":1,"t":"BoxGeometry","a":[1,1,1],"v":null}""");
	}

	[Fact]
	public void ThreeOp_SetOpSerialized_MatchesWireContract()
	{
		// Arrange
		var setOp = BuildFixtureOps().First(x => x.Member == "visible");

		// Act
		var json = JsonSerializer.Serialize(setOp, _webOptions);

		// Assert
		json.ShouldBe("""{"k":1,"h":3,"m":"visible","v":true}""");
	}

	[Fact]
	public void ThreeOp_CallOpSerialized_MatchesWireContract()
	{
		// Arrange
		var callOp = BuildFixtureOps().First(x => x.Kind == ThreeOpKind.Call);

		// Act
		var json = JsonSerializer.Serialize(callOp, _webOptions);

		// Assert
		json.ShouldBe("""{"k":2,"h":3,"m":"lookAt","a":[0,1,0],"v":null}""");
	}

	[Fact]
	public void ThreeOp_AddOpSerialized_MatchesWireContract()
	{
		// Arrange
		var addOp = BuildFixtureOps().First(x => x.Kind == ThreeOpKind.Add);

		// Act
		var json = JsonSerializer.Serialize(addOp, _webOptions);

		// Assert
		json.ShouldBe("""{"k":3,"h":4,"v":null,"c":3}""");
	}

	[Fact]
	public void ThreeOp_RemoveOpSerialized_MatchesWireContract()
	{
		// Arrange
		var removeOp = BuildFixtureOps().First(x => x.Kind == ThreeOpKind.Remove);

		// Act
		var json = JsonSerializer.Serialize(removeOp, _webOptions);

		// Assert
		json.ShouldBe("""{"k":4,"h":4,"v":null,"c":3}""");
	}

	[Fact]
	public void ThreeOp_DisposeOpSerialized_MatchesWireContract()
	{
		// Arrange
		var disposeOp = BuildFixtureOps().First(x => x.Kind == ThreeOpKind.Dispose);

		// Act
		var json = JsonSerializer.Serialize(disposeOp, _webOptions);

		// Assert
		json.ShouldBe("""{"k":5,"h":1,"v":null}""");
	}

	[Fact]
	public void ThreeOp_EveryOpKind_SerializesKindAsANumber()
	{
		// Arrange
		var ops = BuildFixtureOps();

		// Act
		var kindValueKinds = JsonNode.Parse(JsonSerializer.Serialize(ops, _webOptions))!
			.AsArray()
			.Select(x => x!["k"]!.GetValueKind())
			.Distinct()
			.ToList();

		// Assert
		kindValueKinds.ShouldBe([JsonValueKind.Number]);
	}

	[Fact]
	public void ThreeOp_AllSixOpKinds_AppearInTheFixtureBatch()
	{
		// Arrange
		var allOpKinds = Enum.GetValues<ThreeOpKind>();

		// Act
		var coveredOpKinds = BuildFixtureOps()
			.Select(x => x.Kind)
			.Distinct()
			.ToList();

		// Assert
		coveredOpKinds.ShouldBe(allOpKinds, ignoreOrder: true);
	}

	[Fact]
	public void ThreeOp_SetOpWithANullValue_StillWritesTheValueKey()
	{
		// Arrange
		var nullValuedSetOp = BuildFixtureOps().First(x => x.Member == "map");

		// Act
		var json = JsonSerializer.Serialize(nullValuedSetOp, _webOptions);

		// Assert
		json.ShouldBe("""{"k":1,"h":2,"m":"map","v":null}""");
	}

	[Fact]
	public void ThreeOp_FieldsUnusedByAnOpKind_AreOmittedFromThePayload()
	{
		// Arrange
		var setOp = BuildFixtureOps().First(x => x.Member == "visible");

		// Act
		var serializedKeys = JsonNode.Parse(JsonSerializer.Serialize(setOp, _webOptions))!
			.AsObject()
			.Select(x => x.Key)
			.ToList();

		// Assert
		serializedKeys.ShouldBe(["k", "h", "m", "v"]);
	}

	[Fact]
	public void ThreeValue_Vector3Encoded_MatchesWireContract()
	{
		// Arrange
		var vector = new Vector3(1f, 2f, 3f);

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(vector), _webOptions);

		// Assert
		json.ShouldBe("""{"$t":"Vector3","v":[1,2,3]}""");
	}

	[Fact]
	public void ThreeValue_EulerEncoded_MatchesWireContract()
	{
		// Arrange
		var euler = new Euler(0.5f, 0f, 0f, EulerOrder.YXZ);

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(euler), _webOptions);

		// Assert
		json.ShouldBe("""{"$t":"Euler","v":[0.5,0,0],"o":1}""");
	}

	[Fact]
	public void ThreeValue_QuaternionEncoded_MatchesWireContract()
	{
		// Arrange
		var quaternion = new Quaternion(0f, 0f, 0f, 1f);

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(quaternion), _webOptions);

		// Assert
		json.ShouldBe("""{"$t":"Quaternion","v":[0,0,0,1]}""");
	}

	[Fact]
	public void ThreeValue_ColorEncoded_MatchesWireContract()
	{
		// Arrange
		var color = Color.Red;

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(color), _webOptions);

		// Assert
		json.ShouldBe("""{"$t":"Color","v":[1,0,0]}""");
	}

	[Fact]
	public void ThreeValue_Matrix4Encoded_MatchesWireContract()
	{
		// Arrange
		var matrix = new Matrix4();

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(matrix), _webOptions);

		// Assert
		json.ShouldBe("""{"$t":"Matrix4","v":[1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1]}""");
	}

	[Fact]
	public void ThreeValue_NonEulerTagEncoded_OmitsTheOrderKey()
	{
		// Arrange
		var vector = new Vector3(1f, 2f, 3f);

		// Act
		var serializedKeys = JsonNode.Parse(JsonSerializer.Serialize(ThreeValue.Encode(vector), _webOptions))!
			.AsObject()
			.Select(x => x.Key)
			.ToList();

		// Assert
		serializedKeys.ShouldBe(["$t", "v"]);
	}

	[Fact]
	public void ThreeValue_EulerTagEncoded_KeepsTheOrderKey()
	{
		// Arrange
		var euler = new Euler(0.5f, 0f, 0f, EulerOrder.YXZ);

		// Act
		var orderNode = JsonNode.Parse(JsonSerializer.Serialize(ThreeValue.Encode(euler), _webOptions))!
			.AsObject()["o"];

		// Assert
		orderNode.ShouldNotBeNull();
		orderNode.GetValue<byte>().ShouldBe((byte) EulerOrder.YXZ);
	}

	[Fact]
	public void ThreeValue_ThreeObjectEncoded_BecomesAHandleReference()
	{
		// Arrange
		var geometry = new BoxGeometry();

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(geometry), _webOptions);

		// Assert
		json.ShouldBe($$"""{"$ref":{{geometry.Handle}}}""");
	}

	[Theory]
	[InlineData(ThreeWireFormat.Vector3Tag)]
	[InlineData(ThreeWireFormat.EulerTag)]
	[InlineData(ThreeWireFormat.QuaternionTag)]
	[InlineData(ThreeWireFormat.ColorTag)]
	[InlineData(ThreeWireFormat.Matrix4Tag)]
	public void ThreeWireFormat_EveryTagConstant_IsEmittedByTheEncoder(string tag)
	{
		// Arrange
		var encodedMathValues = new object?[]
		{
			ThreeValue.Encode(new Vector3()),
			ThreeValue.Encode(new Euler()),
			ThreeValue.Encode(new Quaternion()),
			ThreeValue.Encode(new Color()),
			ThreeValue.Encode(new Matrix4())
		};

		// Act
		var emittedTags = encodedMathValues
			.Cast<ThreeValue.TaggedValue>()
			.Select(x => x.Tag)
			.ToList();

		// Assert
		emittedTags.ShouldContain(tag);
	}

	[Fact]
	public void ThreeValue_UnhandledReferenceTypeEncoded_Throws()
	{
		// Arrange
		var unhandledValue = new object();

		// Act
		var exception = Record.Exception(() => ThreeValue.Encode(unhandledValue));

		// Assert
		exception.ShouldBeOfType<NotSupportedException>();
	}

	[Theory]
	[InlineData(1.5f)]
	[InlineData(true)]
	[InlineData("someString")]
	public void ThreeValue_PrimitiveOrStringEncoded_PassesThroughUnchanged(object value)
	{
		// Arrange & Act
		var encoded = ThreeValue.Encode(value);

		// Assert
		encoded.ShouldBe(value);
	}

	[Fact]
	public void ThreeOp_FixtureBatchSerialized_MatchesTheSharedFixtureFile()
	{
		// Arrange
		var fixtureJson = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, FixtureFileName));
		var expected = JsonNode.Parse(fixtureJson);

		// Act
		var actual = JsonNode.Parse(JsonSerializer.Serialize(BuildFixtureOps(), _webOptions));

		// Assert
		JsonNode.DeepEquals(actual, expected).ShouldBeTrue(
			$"The serialized batch drifted from {FixtureFileName}, which the JavaScript half of this " +
			$"contract test consumes.{Environment.NewLine}Actual: {actual?.ToJsonString()}");
	}

	/// <summary>
	/// Builds one op of every <see cref="ThreeOpKind"/> and one encoded value of every
	/// <see cref="ThreeWireFormat"/> tag, as a batch the JavaScript applier can run end to end.
	/// Kept in step with <c>tests/wire-format-fixture.json</c> by
	/// <c>ThreeOp_FixtureBatchSerialized_MatchesTheSharedFixtureFile</c>.
	/// </summary>
	/// <returns>The fixture batch, in the order the applier receives it.</returns>
	private static List<ThreeOp> BuildFixtureOps()
	{
		return
		[
			new ThreeOp { Kind = ThreeOpKind.Create, Handle = 1, Type = "BoxGeometry", Args = [1f, 1f, 1f] },
			new ThreeOp { Kind = ThreeOpKind.Create, Handle = 2, Type = "MeshStandardMaterial", Args = [] },
			new ThreeOp
			{
				Kind = ThreeOpKind.Create,
				Handle = 3,
				Type = "Mesh",
				Args =
				[
					new ThreeValue.HandleReference { Handle = 1 },
					new ThreeValue.HandleReference { Handle = 2 }
				]
			},
			new ThreeOp { Kind = ThreeOpKind.Create, Handle = 4, Type = "Scene", Args = [] },
			new ThreeOp { Kind = ThreeOpKind.Set, Handle = 3, Member = "position", Value = ThreeValue.Encode(new Vector3(1f, 2f, 3f)) },
			new ThreeOp { Kind = ThreeOpKind.Set, Handle = 3, Member = "rotation", Value = ThreeValue.Encode(new Euler(0.5f, 0f, 0f, EulerOrder.YXZ)) },
			new ThreeOp { Kind = ThreeOpKind.Set, Handle = 3, Member = "quaternion", Value = ThreeValue.Encode(new Quaternion(0f, 0f, 0f, 1f)) },
			new ThreeOp { Kind = ThreeOpKind.Set, Handle = 2, Member = "color", Value = ThreeValue.Encode(Color.Red) },
			new ThreeOp { Kind = ThreeOpKind.Set, Handle = 3, Member = "visible", Value = true },
			new ThreeOp { Kind = ThreeOpKind.Set, Handle = 2, Member = "map", Value = null },
			new ThreeOp { Kind = ThreeOpKind.Call, Handle = 3, Member = "lookAt", Args = [0f, 1f, 0f] },
			new ThreeOp { Kind = ThreeOpKind.Set, Handle = 3, Member = "matrix", Value = ThreeValue.Encode(new Matrix4()) },
			new ThreeOp { Kind = ThreeOpKind.Add, Handle = 4, ChildHandle = 3 },
			new ThreeOp { Kind = ThreeOpKind.Remove, Handle = 4, ChildHandle = 3 },
			new ThreeOp { Kind = ThreeOpKind.Dispose, Handle = 1 }
		];
	}
}
