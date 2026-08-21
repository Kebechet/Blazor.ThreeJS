using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Addons;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Addons;

/// <summary>
/// Covers the C# half of orbit controls: what attaching costs, that configuring them goes through
/// the ordinary batch, and that the staleness the camera's mirror takes on has an answer a caller can
/// ask for. That the controls actually move the camera - and that a hundred and twenty frames of it
/// send nothing to C# - is pinned against the vendored addon by <c>tests/wire-format.test.mjs</c>.
/// </summary>
public class OrbitControlsTests
{
	[Fact]
	public async Task OrbitControls_Attach_SendsTheCameraHandleAfterFlushingItsCreateOp()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 3);
		var camera = new PerspectiveCamera();

		// Act
		await OrbitControls.AttachAsync(context, camera);

		// Assert
		// The browser has to be handed the real three.js camera, so its create op must already have
		// landed by the time the attach call goes out.
		var identifiers = module.Invocations
			.Select(x => x.Identifier)
			.ToList();

		identifiers.ShouldBe(["applyBatch", "attachOrbitControls"]);
		module.AllOps.ShouldContain(x => x.Kind == ThreeOpKind.Create && x.Handle == camera.Handle);
		module.Invocations.Last().Arguments.ShouldBe([3, camera.Handle]);
	}

	[Fact]
	public async Task OrbitControls_Attach_AdoptsTheHandleTheBrowserMinted()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var camera = new PerspectiveCamera();

		// Act
		var controls = await OrbitControls.AttachAsync(context, camera);

		// Assert
		controls.Handle.ShouldBe(-9);
		controls.Camera.ShouldBeSameAs(camera);
	}

	[Fact]
	public async Task OrbitControls_AttachedAndLeftAlone_MakesNoFurtherInteropCall()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var camera = new PerspectiveCamera();
		var controls = await OrbitControls.AttachAsync(context, camera);
		var invocationsAfterAttach = module.Invocations.Count;

		// Act
		// Every frame the controls move the camera on the JavaScript side. Nothing on this side is
		// asked to do anything about it, so flushing repeatedly costs nothing.
		controls.ShouldNotBeNull();
		await context.FlushAsync();
		await context.FlushAsync();

		// Assert
		module.Invocations.Count.ShouldBe(invocationsAfterAttach);
	}

	[Fact]
	public async Task OrbitControls_PropertyWrite_RecordsASetOnTheMintedHandleUnderThreeJsOwnName()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());

		// Act
		controls.IsDampingEnabled = true;
		controls.MaxDistance = 12f;
		controls.Target.Set(0f, 1f, 0f);
		await context.FlushAsync();

		// Assert
		var setOps = module.AllOps
			.Where(x => x.Kind == ThreeOpKind.Set && x.Handle == -9)
			.Select(x => x.Member)
			.ToList();

		setOps.ShouldBe(["enableDamping", "maxDistance", "target"]);
	}

	[Fact]
	public async Task OrbitControls_PropertyWrittenWithTheValueItAlreadyHolds_RecordsNothing()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());
		var invocationsAfterAttach = module.Invocations.Count;

		// Act
		// three.js's own defaults, written again. The mirror already holds them, so none may go out.
		controls.IsEnabled = true;
		controls.DampingFactor = 0.05f;
		controls.RotateSpeed = 1f;
		controls.MaxPolarAngle = MathF.PI;
		await context.FlushAsync();

		// Assert
		module.Invocations.Count.ShouldBe(invocationsAfterAttach);
	}

	[Fact]
	public async Task OrbitControls_Command_RecordsACallOnTheMintedHandle()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());

		// Act
		controls.SaveState();
		controls.Reset();
		await context.FlushAsync();

		// Assert
		var callOps = module.AllOps
			.Where(x => x.Kind == ThreeOpKind.Call && x.Handle == -9)
			.Select(x => x.Member)
			.ToList();

		callOps.ShouldBe(["saveState", "reset"]);
	}

	[Fact]
	public async Task OrbitControls_NaNLimit_ThrowsAtTheAssignment()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());

		// Act
		// three.js clamps against these bounds, every comparison with NaN is false, and the controls
		// would silently stop honouring the bound - so the assignment is where it has to fail.
		var exception = Record.Exception(() => controls.MinDistance = float.NaN);

		// Assert
		exception.ShouldBeOfType<ArgumentOutOfRangeException>();
	}

	[Theory]
	[InlineData(float.PositiveInfinity)]
	[InlineData(float.NegativeInfinity)]
	public async Task OrbitControls_InfiniteLimit_RecordsTheWriteAndSurvivesTheFlush(float value)
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());

		// Act
		// Infinity is meaningful to these bounds - it is three.js's own default for maxDistance - and
		// the wire spells it as a tagged token, so restoring the unbounded default has to work.
		controls.MaxDistance = 50f;
		controls.MaxDistance = value;
		await context.FlushAsync();

		// Assert
		// One op, not two: consecutive sets of one member coalesce, and the survivor carries the
		// infinity. That it serializes as the tagged token is pinned by the wire-format tests.
		controls.MaxDistance.ShouldBe(value);
		module.AllOps
			.Where(x => x.Kind == ThreeOpKind.Set && x.Handle == -9 && x.Member == "maxDistance")
			.Count()
			.ShouldBe(1);
	}

	[Fact]
	public async Task OrbitControls_MaxDistanceWrittenBackToItsOwnInfiniteDefault_RecordsNothingAndDoesNotThrow()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());
		var invocationsAfterAttach = module.Invocations.Count;

		// Act
		// three.js's own default for this one is infinity, so the mirror reports infinity - and the
		// unchanged-value guard elides the write, exactly as it does for any other value written back
		// unchanged.
		var exception = Record.Exception(() => controls.MaxDistance = float.PositiveInfinity);
		await context.FlushAsync();

		// Assert
		exception.ShouldBeNull();
		controls.MaxDistance.ShouldBe(float.PositiveInfinity);
		module.Invocations.Count.ShouldBe(invocationsAfterAttach);
	}

	[Fact]
	public async Task OrbitControls_ReadOfAnOrbitAngle_IssuesAReadOnTheControlsHandle()
	{
		// Arrange
		var module = new AddonJsObjectReference
		{
			OrbitControlsHandle = -9,
			ReadValue = JsonSerializer.SerializeToElement(1.25f)
		};

		var context = new ThreeContext(module, contextId: 1);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());

		// Act
		var polarAngle = await controls.GetPolarAngleAsync();

		// Assert
		polarAngle.ShouldBe(1.25f);
		var readOp = module.AllOps.Single(x => x.Kind == ThreeOpKind.Read);
		readOp.Handle.ShouldBe(-9);
		readOp.Member.ShouldBe("getPolarAngle");
	}

	[Fact]
	public async Task OrbitControls_ReadOfTheCameraPosition_IssuesAReadOnTheCameraRatherThanTheControls()
	{
		// Arrange
		var module = new AddonJsObjectReference
		{
			OrbitControlsHandle = -9,
			ReadValue = AddonJsObjectReference.TaggedValue("Vector3", 3f, 4f, 5f)
		};

		var context = new ThreeContext(module, contextId: 1);
		var camera = new PerspectiveCamera();
		var controls = await OrbitControls.AttachAsync(context, camera);

		// Act
		// The camera's own mirror is stale while the controls are driving it, so this is the only
		// honest way to answer "where is the camera now" - and it costs one call, when the caller asks.
		var cameraPosition = await controls.GetCameraPositionAsync();

		// Assert
		cameraPosition.ToArray().ShouldBe([3f, 4f, 5f]);
		var readOp = module.AllOps.Single(x => x.Kind == ThreeOpKind.Read);
		readOp.Handle.ShouldBe(camera.Handle);
		readOp.Member.ShouldBe("getWorldPosition");
	}

	[Fact]
	public async Task OrbitControls_CameraPositionWhileControlsAreAttached_StillReportsWhatCSharpLastWrote()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var camera = new PerspectiveCamera();
		camera.Position.Set(0f, 0f, 5f);

		// Act
		await OrbitControls.AttachAsync(context, camera);

		// Assert
		// Documented rather than fixed. The browser moves the camera every frame and nothing reads it
		// back, because reading it back per frame is exactly the traffic the controls exist to avoid.
		camera.Position.ToArray().ShouldBe([0f, 0f, 5f]);
	}

	[Fact]
	public async Task OrbitControls_Detach_TellsTheBrowserAndStopsRecording()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 7);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());

		// Act
		await controls.DetachAsync();
		controls.IsDampingEnabled = true;
		controls.Target.Set(1f, 1f, 1f);
		await context.FlushAsync();

		// Assert
		var detach = module.Invocations.Single(x => x.Identifier == "detachOrbitControls");
		detach.Arguments.ShouldBe([7]);
		module.AllOps.ShouldNotContain(x => x.Handle == -9);
	}

	[Fact]
	public async Task OrbitControls_ReadAfterDetach_ThrowsRatherThanAddressingAHandleTheBrowserRetired()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());
		await controls.DetachAsync();

		// Act
		var exception = await Record.ExceptionAsync(() => controls.GetPolarAngleAsync());

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task OrbitControls_AskedToEmitItsCreateOp_Throws()
	{
		// Arrange
		var module = new AddonJsObjectReference { OrbitControlsHandle = -9 };
		var context = new ThreeContext(module, contextId: 1);
		var controls = await OrbitControls.AttachAsync(context, new PerspectiveCamera());

		// Act
		// The controls were bound to a camera and a canvas by the browser, and a create op carries
		// neither, so a rebuild from the mirror would produce a set that drives nothing.
		var exception = Record.Exception(() => controls.EmitCreate(context.Batch));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task OrbitControls_AttachWithNoCamera_Throws()
	{
		// Arrange
		var context = new ThreeContext(new AddonJsObjectReference(), contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => OrbitControls.AttachAsync(context, camera: null!));

		// Assert
		exception.ShouldBeOfType<ArgumentNullException>();
	}
}
