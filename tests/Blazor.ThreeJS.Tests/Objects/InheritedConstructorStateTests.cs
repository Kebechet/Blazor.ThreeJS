using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// A generated subclass forwards the constructor arguments it shares with its base, so the base half
/// of the mirror holds what the JavaScript object holds. These pin what the caller reads back through
/// a property the base declares, and what reaches the wire for a class that only exists because its
/// base constructor is now reachable.
/// </summary>
public class InheritedConstructorStateTests
{
	[Fact]
	public void ArcCurve_ConstructedWithACentre_ReportsThatCentreThroughTheInheritedProperty()
	{
		// Arrange & Act
		var curve = new ArcCurve(aX: 5f, aY: 7f);

		// Assert
		curve.AX.ShouldBe(5f);
		curve.AY.ShouldBe(7f);
	}

	[Fact]
	public void ArcCurve_ConstructedWithACentre_SendsThatCentreOnTheWire()
	{
		// Arrange
		var batch = new ThreeBatch();
		var curve = new ArcCurve(aX: 5f, aY: 7f);

		// Act
		curve.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var create = ops.Single(x => x.Kind == ThreeOpKind.Create && x.Handle == curve.Handle);
		create.Args.ShouldNotBeNull();
		create.Args[0].ShouldBe(5f);
		create.Args[1].ShouldBe(7f);
	}

	[Fact]
	public void ArcCurve_ConstructedWithARadius_LeavesTheBaseRadiiAtTheirOwnDefaults()
	{
		// three.js passes ArcCurve's one radius to both of EllipseCurve's, but nothing in the types says
		// so, and a mirror that guessed it would be asserting something it was never told. The base
		// keeps its documented default instead.
		var curve = new ArcCurve(aRadius: 3f);

		curve.XRadius.ShouldBe(1f);
		curve.YRadius.ShouldBe(1f);
	}

	[Fact]
	public void PositionalAudio_ConstructedWithAListener_CreatesTheListenerBeforeItself()
	{
		// Arrange: PositionalAudio declares no constructor of its own, so its listener comes from the
		// one Audio declares - which is also why it could not be generated until the base was reachable.
		var batch = new ThreeBatch();
		var listener = new AudioListener();
		var audio = new PositionalAudio(listener);

		// Act
		audio.AttachTo(batch);
		var ops = batch.Drain().ToList();

		// Assert
		var listenerCreate = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == listener.Handle);
		var ownCreate = ops.FindIndex(x => x.Kind == ThreeOpKind.Create && x.Handle == audio.Handle);

		listenerCreate.ShouldBeGreaterThanOrEqualTo(0);
		ownCreate.ShouldBeGreaterThan(listenerCreate);
		ops.ElementAt(ownCreate).Type.ShouldBe("PositionalAudio");
	}

	[Fact]
	public void InstancedBufferAttribute_ConstructedWithItsOwnArguments_ReportsThemThroughTheInheritedProperties()
	{
		// Arrange & Act
		var attribute = new InstancedBufferAttribute(new Float32Array([1f, 2f, 3f]), itemSize: 3f, normalized: true);

		// Assert
		attribute.ItemSize.ShouldBe(3f);
		attribute.Normalized.ShouldBeTrue();
	}
}
