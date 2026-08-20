using System.Text.Json;
using Blazor.ThreeJS.Tests.Core;
using Kebechet.Blazor.ThreeJS.Core;
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
}
