using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;
using Path = Kebechet.Blazor.ThreeJS.Objects.Path;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// The same ordering invariant as <see cref="GeneratedReferencePropertyTests"/>, for a property or a
/// constructor argument holding an <em>array</em> of mirrored objects. Each element travels as its own
/// handle reference, so each one has to be created before the op that names it — and unlike the scalar
/// case there is no single field to attach, which is what made this the arm that got missed.
/// </summary>
public class GeneratedSequenceReferenceTests
{
	[Fact]
	public void ArrayCamera_CamerasPassedToConstructor_CreatesEachCameraBeforeTheCreateThatReferencesThem()
	{
		// Arrange
		var batch = new ThreeBatch();
		var first = new PerspectiveCamera();
		var second = new PerspectiveCamera();
		var arrayCamera = new ArrayCamera([first, second]);

		// Act
		arrayCamera.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var ownCreateIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == arrayCamera.Handle);
		ownCreateIndex.ShouldBeGreaterThanOrEqualTo(0);

		foreach (var camera in new[] { first, second })
		{
			var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == camera.Handle);
			createIndex.ShouldBeGreaterThanOrEqualTo(0);
			createIndex.ShouldBeLessThan(ownCreateIndex);
		}
	}

	[Fact]
	public void ArrayCamera_CamerasWrittenBeforeAttach_CreatesEachCameraBeforeTheWriteThatReferencesThem()
	{
		// Arrange
		var batch = new ThreeBatch();
		var first = new PerspectiveCamera();
		var second = new PerspectiveCamera();
		var arrayCamera = new ArrayCamera { Cameras = [first, second] };

		// Act
		arrayCamera.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "cameras");
		setIndex.ShouldBeGreaterThanOrEqualTo(0);

		foreach (var camera in new[] { first, second })
		{
			var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == camera.Handle);
			createIndex.ShouldBeGreaterThanOrEqualTo(0);
			createIndex.ShouldBeLessThan(setIndex);
		}

		var encoded = ops.ElementAt(setIndex).Value.ShouldBeOfType<object?[]>();
		encoded.Length.ShouldBe(2);
		encoded[0].ShouldBeOfType<ThreeValue.HandleReference>().Handle.ShouldBe(first.Handle);
		encoded[1].ShouldBeOfType<ThreeValue.HandleReference>().Handle.ShouldBe(second.Handle);
	}

	[Fact]
	public void ArrayCamera_CamerasWrittenAfterAttach_CreatesEachCameraBeforeTheWriteThatReferencesThem()
	{
		// Arrange
		var batch = new ThreeBatch();
		var arrayCamera = new ArrayCamera();
		arrayCamera.AttachTo(batch);
		batch.Drain();
		var first = new PerspectiveCamera();
		var second = new PerspectiveCamera();

		// Act
		arrayCamera.Cameras = [first, second];
		var ops = batch.Drain().ToList();

		// Assert
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "cameras");
		setIndex.ShouldBeGreaterThanOrEqualTo(0);

		foreach (var camera in new[] { first, second })
		{
			var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == camera.Handle);
			createIndex.ShouldBeGreaterThanOrEqualTo(0);
			createIndex.ShouldBeLessThan(setIndex);
		}
	}

	[Fact]
	public void ArrayCamera_CameraAlreadyAttached_IsNotCreatedASecondTime()
	{
		// Arrange
		var batch = new ThreeBatch();
		var camera = new PerspectiveCamera();
		camera.AttachTo(batch);
		var arrayCamera = new ArrayCamera { Cameras = [camera] };

		// Act
		arrayCamera.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		ops.Count(x => x.Kind == ThreeOpKind.Create && x.Handle == camera.Handle).ShouldBe(1);
	}

	[Fact]
	public void ArrayCamera_CamerasHoldingNull_LeavesTheNullInPlaceRatherThanFaulting()
	{
		// Arrange
		var batch = new ThreeBatch();
		var camera = new PerspectiveCamera();
		var arrayCamera = new ArrayCamera { Cameras = [null, camera] };

		// Act
		arrayCamera.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "cameras");
		var encoded = ops.ElementAt(setIndex).Value.ShouldBeOfType<object?[]>();
		encoded[0].ShouldBeNull();
		encoded[1].ShouldBeOfType<ThreeValue.HandleReference>().Handle.ShouldBe(camera.Handle);
	}

	[Fact]
	public void Shape_HolesWrittenBeforeAttach_CreatesEachHoleBeforeTheWriteThatReferencesThem()
	{
		// Arrange
		var batch = new ThreeBatch();
		var hole = new Path();
		var shape = new Shape { Holes = [hole] };

		// Act
		shape.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "holes");
		var createIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == hole.Handle);

		setIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeGreaterThanOrEqualTo(0);
		createIndex.ShouldBeLessThan(setIndex);
	}

	[Fact]
	public void Material_ClippingPlanesWrittenBeforeAttach_TravelsAsTaggedValuesRatherThanHandles()
	{
		// Arrange
		var batch = new ThreeBatch();
		var material = new MeshStandardMaterial { ClippingPlanes = [new Plane(new Vector3(0f, 1f, 0f), 0f)] };

		// Act
		material.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var setIndex = ops.FindIndex(x => x.Kind == ThreeOpKind.Set && x.Member == "clippingPlanes");
		setIndex.ShouldBeGreaterThanOrEqualTo(0);

		// A Plane is a hand-written math value, not a handle-backed object, so it needs no create op of
		// its own — it encodes inline the way a Vector3 does.
		var encoded = ops.ElementAt(setIndex).Value.ShouldBeOfType<object?[]>();
		encoded.Length.ShouldBe(1);
		encoded[0].ShouldBeOfType<ThreeValue.TaggedValue>().Tag.ShouldBe(ThreeWireFormat.PlaneTag);
	}
}
