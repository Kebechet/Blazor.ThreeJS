using Blazor.ThreeJS.Tests.Addons;
using Kebechet.Blazor.ThreeJS.Addons;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Core;

/// <summary>
/// Covers the rule that lets two allocators share one handle space without ever talking to each
/// other: C# counts up from 1 and the browser counts down from -1. Everything the loaded-graph and
/// controls mirrors do rests on it, so both directions are checked rather than assumed - the
/// JavaScript half is pinned in <c>tests/wire-format.test.mjs</c>.
/// </summary>
public class ThreeHandlePartitionTests
{
	[Fact]
	public void ThreeObject_HandleAllocatedForAMirroredObject_IsPositive()
	{
		// Arrange & Act
		var mesh = new Mesh();

		// Assert
		mesh.Handle.ShouldBeGreaterThan(0);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(int.MinValue)]
	public void ThreeObject_AllocatorProducingANonPositiveHandle_Throws(int wrappedHandle)
	{
		// Arrange & Act
		var exception = Record.Exception(() => ThreeObject.ThrowIfNotMirrorAllocated(wrappedHandle));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public void ThreeObject_AllocatorProducingAPositiveHandle_ReturnsIt()
	{
		// Arrange & Act
		var handle = ThreeObject.ThrowIfNotMirrorAllocated(7);

		// Assert
		handle.ShouldBe(7);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(int.MaxValue)]
	public void ThreeObject_BrowserMintedHandleInTheMirrorsHalfOfTheSpace_Throws(int collidingHandle)
	{
		// Arrange & Act
		var exception = Record.Exception(() => ThreeObject.ThrowIfNotBrowserMinted(collidingHandle));

		// Assert
		exception.ShouldBeOfType<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void ThreeObject_BrowserMintedNegativeHandle_ReturnsIt()
	{
		// Arrange & Act
		var handle = ThreeObject.ThrowIfNotBrowserMinted(-7);

		// Assert
		handle.ShouldBe(-7);
	}

	[Fact]
	public async Task LoadedObject3D_HandleTheBrowserReportedAsPositive_IsRejectedAtConstruction()
	{
		// Arrange
		var module = new AddonJsObjectReference
		{
			LoadResponse = new GLTFLoadResponse { Nodes = [LoadedNode.Describe(handle: 1, "Forged", "Group")] }
		};

		var context = new ThreeContext(module, contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => new GLTFLoader(context).LoadAsync("model.gltf"));

		// Assert
		exception.ShouldBeOfType<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void ThreeObject_AMirroredObjectAndALoadedOne_NeverShareAHandle()
	{
		// Arrange
		var module = new AddonJsObjectReference();
		var context = new ThreeContext(module, contextId: 1);
		var loadedNode = new LoadedObject3D(context.Batch, LoadedNode.Describe(handle: -1, "Head"));

		// Act
		var mirroredHandles = Enumerable.Range(0, 50)
			.Select(_ => new Mesh().Handle)
			.ToList();

		// Assert
		mirroredHandles.ShouldNotContain(loadedNode.Handle);
		mirroredHandles.ShouldAllBe(x => x > 0);
	}
}
