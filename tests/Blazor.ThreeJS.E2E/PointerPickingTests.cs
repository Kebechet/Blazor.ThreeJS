using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// The full pointer loop, which no other layer can run end to end: a real mouse click on the canvas,
/// a raycast in JavaScript, a <c>[JSInvokable]</c> hop into C#, a handler writing to the mirrored
/// scene graph, the batch that carries the change back, and the applier that draws it.
/// </summary>
[Collection(DemoCollectionDefinition.Name)]
public sealed class PointerPickingTests(DemoFixture fixture)
{
	/// <summary>World X of the two cubes that have a click handler; the third sits at zero.</summary>
	private const double OuterCubeOffsetX = 2.2;

	/// <summary>Distance from the camera to the plane the cubes sit in, in world units.</summary>
	private const double CubeCameraDistance = 6;

	/// <summary>The cubes' camera's vertical field of view, in degrees.</summary>
	private const double CubeFieldOfViewDegrees = 75;

	/// <summary>
	/// Half the side of the square sampled over a cube, in CSS pixels. A cube covers roughly forty
	/// pixels at this viewport, so this stays well inside one even after its rotation.
	/// </summary>
	private const int CubeSampleRadius = 10;

	/// <summary>How long a story is given to draw its first frame.</summary>
	private static readonly TimeSpan DrawTimeout = TimeSpan.FromSeconds(20);

	/// <summary>How long a recolour is given to make it back to the canvas.</summary>
	private static readonly TimeSpan RecolourTimeout = TimeSpan.FromSeconds(10);

	/// <summary>
	/// How long a click that should change nothing is watched for, before the scene is declared
	/// unchanged. Long enough to cover the interop round trip a real hit would have taken.
	/// </summary>
	private static readonly TimeSpan SettleDelay = TimeSpan.FromSeconds(2);

	/// <summary>
	/// Mean channel difference, on the 0-255 scale, above which a sampled square counts as recoloured.
	/// The palette step is from <c>0x3366cc</c> to <c>0xcc3366</c>, which is a hundred-odd levels on
	/// two channels, so this threshold is nowhere near it and nowhere near rasteriser noise either.
	/// </summary>
	private const double RecolouredThreshold = 12;

	/// <summary>
	/// Difference below which a sampled square counts as untouched. Software rasterisation of a
	/// static scene is bit-identical frame to frame, so this only has to allow for the sampling
	/// itself.
	/// </summary>
	private const double UnchangedThreshold = 1;

	[Fact]
	public async Task PointerPicking_OuterCubeClickedInTheImperativeStory_RecoloursThatCubeAndLeavesTheOthers()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeClickToRecolour);
		var leftCube = await storyPage.ProjectAsync(-OuterCubeOffsetX, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		var middleCube = await storyPage.ProjectAsync(0, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		var rightCube = await storyPage.ProjectAsync(OuterCubeOffsetX, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		await WaitUntilCubesAreDrawnAsync(storyPage, leftCube, middleCube, rightCube);

		var leftBefore = await storyPage.CaptureAroundAsync(leftCube, CubeSampleRadius);
		var middleBefore = await storyPage.CaptureAroundAsync(middleCube, CubeSampleRadius);
		var rightBefore = await storyPage.CaptureAroundAsync(rightCube, CubeSampleRadius);

		// Act
		await storyPage.ClickAsync(leftCube);

		// Assert
		await storyPage.WaitUntilAsync(
			async () => await HasRecolouredAsync(storyPage, leftCube, leftBefore),
			RecolourTimeout,
			"the clicked cube was recoloured");

		var middleAfter = await storyPage.CaptureAroundAsync(middleCube, CubeSampleRadius);
		var rightAfter = await storyPage.CaptureAroundAsync(rightCube, CubeSampleRadius);
		middleAfter.MeanAbsoluteDifferenceFrom(middleBefore).ShouldBeLessThan(UnchangedThreshold);
		rightAfter.MeanAbsoluteDifferenceFrom(rightBefore).ShouldBeLessThan(UnchangedThreshold);
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>
	/// The middle cube never subscribes, so it is never registered as a pointer target and the ray
	/// passes straight through it. The second half of the test is what makes the first half mean
	/// something: without it, a click machinery that was simply dead would pass.
	/// </summary>
	[Fact]
	public async Task PointerPicking_MiddleCubeWithNoHandlerClicked_ChangesNothingWhileTheOuterCubesStillRespond()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeClickToRecolour);
		var leftCube = await storyPage.ProjectAsync(-OuterCubeOffsetX, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		var middleCube = await storyPage.ProjectAsync(0, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		var rightCube = await storyPage.ProjectAsync(OuterCubeOffsetX, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		await WaitUntilCubesAreDrawnAsync(storyPage, leftCube, middleCube, rightCube);

		var middleBefore = await storyPage.CaptureAroundAsync(middleCube, CubeSampleRadius);
		var rightBefore = await storyPage.CaptureAroundAsync(rightCube, CubeSampleRadius);

		// Act
		await storyPage.ClickAsync(middleCube);
		await Task.Delay(SettleDelay, TestContext.Current.CancellationToken);

		// Assert
		var middleAfter = await storyPage.CaptureAroundAsync(middleCube, CubeSampleRadius);
		middleAfter.MeanAbsoluteDifferenceFrom(middleBefore).ShouldBeLessThan(UnchangedThreshold);

		await storyPage.ClickAsync(rightCube);
		await storyPage.WaitUntilAsync(
			async () => await HasRecolouredAsync(storyPage, rightCube, rightBefore),
			RecolourTimeout,
			"an outer cube was still able to recolour after the middle one was clicked");
	}

	/// <summary>
	/// The declarative path to the same place: the handler changes a field, Blazor re-renders the
	/// component tree, and the diff of that tree is what reaches the canvas.
	/// </summary>
	[Fact]
	public async Task PointerPicking_OuterCubeClickedInTheDeclarativeStory_RecoloursThatCubeThroughARerender()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.DeclarativeClickToRecolour);
		var leftCube = await storyPage.ProjectAsync(-OuterCubeOffsetX, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		var middleCube = await storyPage.ProjectAsync(0, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		var rightCube = await storyPage.ProjectAsync(OuterCubeOffsetX, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		await WaitUntilCubesAreDrawnAsync(storyPage, leftCube, middleCube, rightCube);

		var rightBefore = await storyPage.CaptureAroundAsync(rightCube, CubeSampleRadius);
		var leftBefore = await storyPage.CaptureAroundAsync(leftCube, CubeSampleRadius);

		// Act
		await storyPage.ClickAsync(rightCube);

		// Assert
		await storyPage.WaitUntilAsync(
			async () => await HasRecolouredAsync(storyPage, rightCube, rightBefore),
			RecolourTimeout,
			"the clicked cube was recoloured");

		var leftAfter = await storyPage.CaptureAroundAsync(leftCube, CubeSampleRadius);
		leftAfter.MeanAbsoluteDifferenceFrom(leftBefore).ShouldBeLessThan(UnchangedThreshold);
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>
	/// Waits until all three cubes are on screen, which doubles as a check that the projection the
	/// clicks use actually lands on them: a click aimed at empty space would otherwise fail later as
	/// "nothing was recoloured", which is the same symptom as a broken pointer path.
	/// </summary>
	/// <param name="storyPage">Story to wait on.</param>
	/// <param name="cubePoints">Where each cube is expected to be, in viewport coordinates.</param>
	private static async Task WaitUntilCubesAreDrawnAsync(StoryPage storyPage, params ViewportPoint[] cubePoints)
	{
		foreach (var cubePoint in cubePoints)
		{
			await storyPage.WaitUntilAsync(
				async () => (await storyPage.CaptureAroundAsync(cubePoint, CubeSampleRadius)).CoveredFraction > 0.99,
				DrawTimeout,
				$"a cube was drawn at ({cubePoint.X:0}, {cubePoint.Y:0})");
		}
	}

	/// <summary>Whether the square around a cube now shows a different colour than it did.</summary>
	/// <param name="storyPage">Story to sample.</param>
	/// <param name="cubePoint">Where the cube is, in viewport coordinates.</param>
	/// <param name="before">Sample taken before the click.</param>
	private static async Task<bool> HasRecolouredAsync(StoryPage storyPage, ViewportPoint cubePoint, CanvasSample before)
	{
		var after = await storyPage.CaptureAroundAsync(cubePoint, CubeSampleRadius);
		return after.AverageColor.MeanChannelDifferenceFrom(before.AverageColor) > RecolouredThreshold;
	}
}
