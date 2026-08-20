using System.Text.Json;
using Blazor.ThreeJS.Tests.Core;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// Pins the C# half of the structural wire form: a plain data value three.js describes with an
/// interface rather than a class. It has no identity on the JavaScript side, so it travels as its own
/// members instead of behind a handle — which <c>tests/wire-format.test.mjs</c> pins from the other
/// end, against a real <c>BufferGeometry</c>.
/// </summary>
public class StructureValueTests
{
	[Fact]
	public void GeometryGroup_Written_SendsItsMembersUnderThreeJsOwnNames()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BufferGeometry();
		geometry.AttachTo(batch);
		batch.Drain();

		// Act
		geometry.Groups = [new GeometryGroup { Start = 0, Count = 3, MaterialIndex = 1 }];
		var op = batch.Drain().Single(x => x.Kind == ThreeOpKind.Set && x.Member == "groups");

		// Assert
		var encoded = op.Value.ShouldBeOfType<object?[]>();
		var members = encoded.Single().ShouldBeOfType<ThreeValue.StructureValue>().Members;
		members["start"].ShouldBe(0);
		members["count"].ShouldBe(3);
		members["materialIndex"].ShouldBe(1);
	}

	[Fact]
	public void GeometryGroup_MaterialIndexLeftUnset_IsOmittedRatherThanSentAsNull()
	{
		// three.js applies its own default to a member an object literal never mentioned. Sending null
		// instead would assign null, which is a value it never chose.
		var batch = new ThreeBatch();
		var geometry = new BufferGeometry();
		geometry.AttachTo(batch);
		batch.Drain();

		geometry.Groups = [new GeometryGroup { Start = 0, Count = 3 }];
		var op = batch.Drain().Single(x => x.Kind == ThreeOpKind.Set && x.Member == "groups");

		var encoded = op.Value.ShouldBeOfType<object?[]>();
		var members = encoded.Single().ShouldBeOfType<ThreeValue.StructureValue>().Members;
		members.ShouldNotContainKey("materialIndex");
	}

	[Fact]
	public void GeometryGroup_ReadBack_BindsTheMembersTheApplierSent()
	{
		// Arrange
		var element = JsonDocument.Parse("""{"$o":{"start":4,"count":9,"materialIndex":2}}""").RootElement;

		// Act
		var group = ThreeValue.Decode<GeometryGroup>(element);

		// Assert
		group.ShouldNotBeNull();
		group.Start.ShouldBe(4);
		group.Count.ShouldBe(9);
		group.MaterialIndex.ShouldBe(2);
	}

	[Fact]
	public void GeometryGroup_ReadBackWithoutAnOptionalMember_LeavesItUnset()
	{
		var element = JsonDocument.Parse("""{"$o":{"start":4,"count":9}}""").RootElement;

		var group = ThreeValue.Decode<GeometryGroup>(element);

		group.MaterialIndex.ShouldBeNull();
	}

	[Fact]
	public void GeometryGroup_TwoWithTheSameMembers_AreEqual()
	{
		// A value with no identity: three.js keeps no reference to a group, so two holding the same
		// numbers are the same group. The record's own equality is what says so.
		var first = new GeometryGroup { Start = 0, Count = 3, MaterialIndex = 1 };
		var second = new GeometryGroup { Start = 0, Count = 3, MaterialIndex = 1 };

		first.ShouldBe(second);
	}

	[Fact]
	public async Task Fog_ToJson_ReadsAStructureBackThroughTheQueryChannel()
	{
		// Arrange: reading is the direction that matters most for these - a geometry group or a
		// serialised fog is an answer three.js produces, not a value a caller sends.
		var module = new RecordingJsObjectReference
		{
			RespondToBatch = ops => new ThreeBatchResponse
			{
				Results = ops
					.Where(x => x.Kind is ThreeOpKind.Read or ThreeOpKind.Get)
					.Select(x => new ThreeReadResult
					{
						RequestId = x.RequestId,
						Value = JsonDocument.Parse("""{"$o":{"type":"Fog","name":"","color":255,"near":1,"far":1000}}""").RootElement
					})
					.ToList()
			}
		};

		var context = new ThreeContext(module, contextId: 1);
		var fog = new Fog(new Color(1f, 1f, 1f));
		context.Attach(fog);

		// Act
		var json = await fog.ToJSONAsync();

		// Assert
		json.ShouldNotBeNull();
		json.Type.ShouldBe("Fog");
		json.Near.ShouldBe(1f);
		json.Far.ShouldBe(1000f);
	}

	[Fact]
	public void Dictionary_Written_TravelsAsAPlainObjectKeyedByThreeJsOwnKeys()
	{
		// An index signature is a dictionary and nothing else, and a dictionary is already what a plain
		// object is on the wire - so the structure tag carries it with no form of its own.
		var batch = new ThreeBatch();
		var loader = new Loader();
		loader.AttachTo(batch);
		batch.Drain();

		loader.RequestHeader = new Dictionary<string, string> { ["Accept"] = "model/gltf+json" };
		var op = batch.Drain().Single(x => x.Kind == ThreeOpKind.Set && x.Member == "requestHeader");

		var members = op.Value.ShouldBeOfType<ThreeValue.StructureValue>().Members;
		members["Accept"].ShouldBe("model/gltf+json");
	}

	[Fact]
	public void Dictionary_ReadBack_BindsEveryKeyTheApplierSent()
	{
		var element = JsonDocument.Parse("""{"$o":{"first":0,"second":1}}""").RootElement;

		var decoded = ThreeValue.Decode<Dictionary<string, float>>(element);

		decoded.ShouldNotBeNull();
		decoded["first"].ShouldBe(0f);
		decoded["second"].ShouldBe(1f);
	}

	[Fact]
	public void FrenetFrames_IsOneRecordSharedByEveryCurveThatComputesIt()
	{
		// three.js writes this shape without a name, on ten curve classes. Keyed by the shape rather than
		// by the member, so they share one record instead of getting ten identical ones - and the name
		// says what the shape is rather than which class was read first.
		var element = JsonDocument.Parse(
			"""{"$o":{"tangents":[{"$t":"Vector3","v":[1,0,0]}],"normals":[],"binormals":[]}}""").RootElement;

		var frames = ThreeValue.Decode<FrenetFrames>(element);

		frames.ShouldNotBeNull();
		frames.Tangents.ShouldNotBeNull();
		frames.Tangents!.Single().X.ShouldBe(1f);
	}

	[Fact]
	public void BoxGeometryParameters_KeepsItsClassInTheName_BecauseSixteenGeometriesEachHaveTheirOwn()
	{
		// `parameters` alone would collide sixteen ways, so the class is part of the name. The shape is
		// three.js echoing back the arguments the geometry was built from.
		var element = JsonDocument.Parse(
			"""{"$o":{"width":2,"height":3,"depth":4,"widthSegments":1,"heightSegments":1,"depthSegments":1}}""").RootElement;

		var parameters = ThreeValue.Decode<BoxGeometryParameters>(element);

		parameters.ShouldNotBeNull();
		parameters.Width.ShouldBe(2f);
		parameters.Depth.ShouldBe(4f);
	}
}
