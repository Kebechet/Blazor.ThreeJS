using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

public class ThreeErrorTests
{
	[Fact]
	public void ThreeError_MemberAbsentInJson_DeserialisesAsNull()
	{
		// Arrange
		const string json = """{"handle":1,"message":"Unknown three.js type."}""";

		// Act
		var error = JsonSerializer.Deserialize<ThreeError>(json);

		// Assert
		error.ShouldNotBeNull();
		error.Member.ShouldBeNull();
	}
}
