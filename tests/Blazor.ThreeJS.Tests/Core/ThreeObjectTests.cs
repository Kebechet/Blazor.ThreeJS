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

	[Fact]
	public void ThreeObject_AttachedToASecondBatch_Throws()
	{
		// Arrange
		var geometry = new BoxGeometry();
		geometry.AttachTo(new ThreeBatch());

		// Act
		var exception = Record.Exception(() => geometry.AttachTo(new ThreeBatch()));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public void Object3D_AttachedToASecondBatch_Throws()
	{
		// Arrange
		var scene = new Scene();
		scene.AttachTo(new ThreeBatch());

		// Act
		var exception = Record.Exception(() => scene.AttachTo(new ThreeBatch()));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public void ThreeObject_AttachedTwiceToTheSameBatch_DoesNotThrow()
	{
		// Arrange
		var batch = new ThreeBatch();
		var scene = new Scene();
		scene.AttachTo(batch);

		// Act
		var exception = Record.Exception(() => scene.AttachTo(batch));

		// Assert
		exception.ShouldBeNull();
	}
}
