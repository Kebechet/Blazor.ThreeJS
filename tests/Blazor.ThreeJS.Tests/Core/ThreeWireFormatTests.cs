using System.Text.Json;
using System.Text.Json.Nodes;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

// three.js has its own `Path` — a 2D curve — and the generated mirror carries it, so the unqualified
// name is ambiguous in any file importing both this namespace and System.IO.
using Path = System.IO.Path;

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
	public void ThreeOp_ReadOpSerialized_MatchesWireContract()
	{
		// Arrange
		var readOp = BuildFixtureOps().First(x => x.Kind == ThreeOpKind.Read);

		// Act
		var json = JsonSerializer.Serialize(readOp, _webOptions);

		// Assert
		json.ShouldBe("""{"k":6,"h":6,"m":"getFocalLength","a":[],"v":null,"i":1}""");
	}

	[Fact]
	public void ThreeOp_PickOpSerialized_MatchesWireContract()
	{
		// Arrange
		var pickOp = BuildFixtureOps().First(x => x.Kind == ThreeOpKind.Pick);

		// Act
		var json = JsonSerializer.Serialize(pickOp, _webOptions);

		// Assert
		json.ShouldBe("""{"k":7,"h":3,"v":true}""");
	}

	[Fact]
	public void ThreeOp_NonReadOp_OmitsTheRequestIdKey()
	{
		// Arrange
		var setOp = BuildFixtureOps().First(x => x.Member == "visible");

		// Act
		var serializedKeys = JsonNode.Parse(JsonSerializer.Serialize(setOp, _webOptions))!
			.AsObject()
			.Select(x => x.Key)
			.ToList();

		// Assert
		serializedKeys.ShouldNotContain("i");
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
	public void ThreeOp_EveryOpKind_AppearsInTheFixtureBatch()
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

	[Fact]
	public void ThreeValue_SideEnumEncoded_SerializesAsANumber()
	{
		// Arrange
		var encoded = ThreeValue.Encode(Side.DoubleSide);

		// Act
		var valueKind = JsonNode.Parse(JsonSerializer.Serialize(encoded, _webOptions))!.GetValueKind();

		// Assert
		valueKind.ShouldBe(JsonValueKind.Number);
	}

	[Fact]
	public void ThreeValue_SideEnumEncoded_MatchesItsThreeJsNumericConstant()
	{
		// Arrange & Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(Side.DoubleSide), _webOptions);

		// Assert
		json.ShouldBe("2");
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
	public void ThreeValue_WideBackedEnumEncoded_KeepsItsFullNumericValue()
	{
		// Arrange
		var wideValue = GeneratedEnumBackingShape.RepeatWrapping;

		// Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(wideValue), _webOptions);

		// Assert
		json.ShouldBe("1000");
	}

	[Fact]
	public void ThreeValue_UnspecifiedEncoded_MatchesWireContract()
	{
		// Arrange & Act
		var json = JsonSerializer.Serialize(ThreeValue.Encode(ThreeValue.Unspecified), _webOptions);

		// Assert
		json.ShouldBe($$"""{"{{ThreeWireFormat.UndefinedKey}}":true}""");
	}

	[Fact]
	public void ThreeValue_UnspecifiedEncoded_StaysDistinctFromAnIntentionalNull()
	{
		// Arrange
		var unspecified = JsonSerializer.Serialize(ThreeValue.Encode(ThreeValue.Unspecified), _webOptions);

		// Act
		var intentionalNull = JsonSerializer.Serialize(ThreeValue.Encode(null), _webOptions);

		// Assert
		intentionalNull.ShouldBe("null");
		unspecified.ShouldNotBe(intentionalNull);
	}

	[Fact]
	public void ThreeValue_OrUnspecifiedGivenNull_ReturnsTheSentinel()
	{
		// Arrange & Act
		var substituted = ThreeValue.OrUnspecified(null);

		// Assert
		substituted.ShouldBeSameAs(ThreeValue.Unspecified);
	}

	[Fact]
	public void ThreeValue_OrUnspecifiedGivenAValue_ReturnsItUnchanged()
	{
		// Arrange & Act
		var substituted = ThreeValue.OrUnspecified(512f);

		// Assert
		substituted.ShouldBe(512f);
	}

	[Fact]
	public void ThreeValue_TrimUnspecifiedTail_DropsOnlyTheTrailingRun()
	{
		// Arrange
		object?[] args = [ThreeValue.Unspecified, 2f, ThreeValue.Unspecified, ThreeValue.Unspecified];

		// Act
		var trimmed = ThreeValue.TrimUnspecifiedTail(args);

		// Assert
		trimmed.ShouldBe([ThreeValue.Unspecified, 2f]);
	}

	[Fact]
	public void ThreeValue_TrimUnspecifiedTail_KeepsATrailingIntentionalNull()
	{
		// Arrange
		object?[] args = [1f, null];

		// Act
		var trimmed = ThreeValue.TrimUnspecifiedTail(args);

		// Assert
		trimmed.ShouldBe([1f, null]);
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

	[Theory]
	[InlineData(1.5f)]
	[InlineData(true)]
	[InlineData("someString")]
	public void ThreeValue_PrimitiveOrStringDecoded_ReturnsItUnchanged(object value)
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(value, _webOptions);

		// Act
		var decoded = ThreeValue.Decode<object>(element);

		// Assert
		decoded.ToString().ShouldBe(value.ToString());
	}

	[Fact]
	public void ThreeValue_Vector3Decoded_RoundTripsWhatTheEncoderProduced()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(new Vector3(1f, 2f, 3f)), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<Vector3>(element);

		// Assert
		decoded.ToArray().ShouldBe([1f, 2f, 3f]);
	}

	[Fact]
	public void ThreeValue_EulerDecoded_RestoresItsRotationOrder()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(new Euler(0.5f, 0f, 0f, EulerOrder.YXZ)), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<Euler>(element);

		// Assert
		decoded.Order.ShouldBe(EulerOrder.YXZ);
	}

	[Fact]
	public void ThreeValue_QuaternionDecoded_RoundTripsAllFourComponents()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(new Quaternion(0.1f, 0.2f, 0.3f, 0.4f)), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<Quaternion>(element);

		// Assert
		decoded.ToArray().ShouldBe([0.1f, 0.2f, 0.3f, 0.4f]);
	}

	[Fact]
	public void ThreeValue_ColorDecoded_RoundTripsWhatTheEncoderProduced()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(Color.Red), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<Color>(element);

		// Assert
		decoded.ToArray().ShouldBe([1f, 0f, 0f]);
	}

	[Fact]
	public void ThreeValue_Matrix4Decoded_KeepsTheColumnMajorOrderItWasSentIn()
	{
		// Arrange
		var matrix = new Matrix4().Set(
			1f, 2f, 3f, 4f,
			5f, 6f, 7f, 8f,
			9f, 10f, 11f, 12f,
			13f, 14f, 15f, 16f);

		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(matrix), _webOptions);

		// Act
		var decoded = ThreeValue.Decode<Matrix4>(element);

		// Assert
		decoded.Elements.ShouldBe(matrix.Elements);
	}

	[Fact]
	public void ThreeValue_DecodeGivenNoValue_ReturnsTheDefault()
	{
		// Arrange & Act
		var decoded = ThreeValue.Decode<float>(null);

		// Assert
		decoded.ShouldBe(0f);
	}

	[Fact]
	public void ThreeValue_DecodeGivenAJsonNull_ReturnsTheDefault()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement<object?>(null, _webOptions);

		// Act
		var decoded = ThreeValue.Decode<string>(element);

		// Assert
		decoded.ShouldBeNull();
	}

	[Fact]
	public void ThreeValue_DecodeGivenATagTheTargetTypeCannotHold_Throws()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(ThreeValue.Encode(new Vector3(1f, 2f, 3f)), _webOptions);

		// Act
		var exception = Record.Exception(() => ThreeValue.Decode<Color>(element));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public void ThreeValue_DecodeGivenAnUnknownTag_Throws()
	{
		// Arrange
		var element = JsonSerializer.SerializeToElement(new { t = "Box3", v = new[] { 0f } }, _webOptions);
		var retagged = JsonDocument.Parse(
			element.GetRawText().Replace("\"t\":", $"\"{ThreeWireFormat.TagKey}\":", StringComparison.Ordinal)).RootElement;

		// Act
		var exception = Record.Exception(() => ThreeValue.Decode<Vector3>(retagged));

		// Assert
		exception.ShouldBeOfType<NotSupportedException>();
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
	/// Builds one op of every <see cref="ThreeOpKind"/>, one encoded value of every
	/// <see cref="ThreeWireFormat"/> tag, an enum-valued <c>Set</c>, a <c>$ref</c>-valued
	/// <c>Set</c> that reassigns a mesh's material after it was already attached, and the pair of
	/// <c>PerspectiveCamera</c> creates that prove the <c>$undef</c> sentinel reaches JavaScript as a
	/// real <c>undefined</c>, an <c>AmbientLight</c> create carrying a tagged math value as a
	/// constructor argument, the <c>Read</c> op that hands a value back, and the <c>Pick</c> op that
	/// opts an object into pointer hit-testing — as a batch the JavaScript applier can run end to end.
	/// Kept in step with
	/// <c>tests/wire-format-fixture.json</c> by <c>ThreeOp_FixtureBatchSerialized_MatchesTheSharedFixtureFile</c>.
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
			new ThreeOp { Kind = ThreeOpKind.Create, Handle = 5, Type = "MeshStandardMaterial", Args = [] },
			new ThreeOp { Kind = ThreeOpKind.Set, Handle = 3, Member = "material", Value = new ThreeValue.HandleReference { Handle = 5 } },
			new ThreeOp { Kind = ThreeOpKind.Set, Handle = 2, Member = "side", Value = ThreeValue.Encode(Side.DoubleSide) },
			new ThreeOp { Kind = ThreeOpKind.Dispose, Handle = 1 },
			new ThreeOp
			{
				Kind = ThreeOpKind.Create,
				Handle = 6,
				Type = "PerspectiveCamera",
				Args = [ThreeValue.Encode(ThreeValue.Unspecified), 2f, ThreeValue.Encode(ThreeValue.Unspecified), 1000f]
			},

			// The control for the op above. THREE.PerspectiveCamera defaults fov to 50 and near to 0.1,
			// and this op supplies JSON null for fov instead of the sentinel — which the JavaScript half
			// asserts leaves fov null. Without it, the sentinel assertions would also pass if the applier
			// were quietly reading null as "absent".
			new ThreeOp { Kind = ThreeOpKind.Create, Handle = 7, Type = "PerspectiveCamera", Args = [null, 2f] },

			// A tagged math value as a *constructor* argument rather than as a Set value. The generated
			// AmbientLight forwards its Color straight through, where the hand-written one it replaced
			// converted to a hex integer first, so this is the one shape in the batch that only the
			// generated classes produce.
			new ThreeOp { Kind = ThreeOpKind.Create, Handle = 8, Type = "AmbientLight", Args = [ThreeValue.Encode(Color.Red), 0.4f] },

			// The only op that produces a value. It targets handle 6, the camera whose fov was left to
			// the $undef sentinel, so the focal length the JavaScript half reads back is one three.js
			// computed from its own default rather than from anything C# sent.
			new ThreeOp { Kind = ThreeOpKind.Read, Handle = 6, Member = "getFocalLength", Args = [], RequestId = 1 },

			// The op behind the only traffic C# never asked for: it opts handle 3, the mesh, into
			// pointer hit-testing, and the JavaScript half asserts the applier reads this exact shape as
			// a candidate registration.
			new ThreeOp { Kind = ThreeOpKind.Pick, Handle = 3, Value = true }
		];
	}

	/// <summary>
	/// Stands in for the generated enums whose values do not fit in a <see cref="byte"/> — three.js's
	/// WebGL constants are in the thousands, so the generator backs those with <see cref="ushort"/>.
	/// Declared here rather than referencing a generated enum so the encoder is pinned against the
	/// backing type itself, not against whichever enum happens to carry it in the current snapshot.
	/// </summary>
	private enum GeneratedEnumBackingShape : ushort
	{
		/// <summary>Mirrors <c>THREE.RepeatWrapping</c>, the widest shape a generated enum takes today.</summary>
		RepeatWrapping = 1000
	}
}
