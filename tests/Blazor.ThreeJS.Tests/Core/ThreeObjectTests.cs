using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

public class ThreeObjectTests
{
	[Fact]
	public void ThreeObject_AttachToCalledTwice_EmitsSingleCreateOp()
	{
		// Arrange
		var batch = new ThreeBatch();
		var geometry = new BoxGeometry();

		// Act
		geometry.AttachTo(batch);
		geometry.AttachTo(batch);
		var ops = batch.Drain();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Create).ShouldBe(1);
	}
}
