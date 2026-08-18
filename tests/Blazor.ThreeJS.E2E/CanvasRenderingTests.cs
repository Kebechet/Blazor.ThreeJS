using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// What only a real browser can answer: whether a WebGL context was created at all, whether the
/// drawing buffer is the size the CSS box and the device pixel ratio say it should be, and whether
/// anything was drawn into it.
/// </summary>
[Collection(DemoCollectionDefinition.Name)]
public sealed class CanvasRenderingTests(DemoFixture fixture, ITestOutputHelper output)
{
	/// <summary>Grid a whole-canvas sample is scaled into. Coarse on purpose: this is not a baseline.</summary>
	private const int SignatureColumns = 96;

	/// <summary>Rows in a whole-canvas sample.</summary>
	private const int SignatureRows = 32;

	/// <summary>How long a story is given to draw its first frame.</summary>
	private static readonly TimeSpan DrawTimeout = TimeSpan.FromSeconds(20);

	/// <summary>
	/// Mean absolute per-channel difference, on the 0-255 scale, above which two samples are treated
	/// as showing different things. Software rasterisation is deterministic frame to frame, so the
	/// floor for "identical" is 0 and this only has to clear the noise a changing scene never sits in.
	/// </summary>
	private const double ChangedThreshold = 1.0;

	[Fact]
	public async Task WebGlContext_RotatingCubeStoryOpened_IsCreatedOnTheCanvasElement()
	{
		// Arrange & Act
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeRotatingCube);
		var probe = await storyPage.ProbeAsync();

		// Assert
		probe.HasWebGlContext.ShouldBeTrue();
		probe.ContextVersion.ShouldStartWith("WebGL");
		probe.DrawingBufferWidth.ShouldBeGreaterThan(0);
		probe.DrawingBufferHeight.ShouldBeGreaterThan(0);
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>
	/// The determinism guarantee this suite rests on, checked rather than assumed. The launch flags
	/// are only a request; if a Chrome release stops honouring one of them the suite silently starts
	/// rendering on whatever GPU the machine has, and every pixel comparison below becomes
	/// machine-dependent without anything going red.
	/// </summary>
	[Fact]
	public async Task Rendering_BrowserLaunched_ReportsSwiftShaderAsTheRenderer()
	{
		// Arrange & Act
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeRotatingCube);
		var probe = await storyPage.ProbeAsync();

		// Assert
		output.WriteLine($"renderer: {probe.Renderer}");
		output.WriteLine($"context: {probe.ContextVersion}");
		probe.Renderer.ShouldContain("SwiftShader");
	}

	/// <summary>
	/// The 300x150 defect: a canvas whose drawing buffer never left the HTML default renders a
	/// stretched, low-resolution scene while every unit test still passes, because nothing outside a
	/// browser has a layout box to compare against.
	/// </summary>
	/// <param name="devicePixelRatio">Device pixel ratio the page reports.</param>
	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	public async Task DrawingBuffer_StoryOpenedAtAGivenDevicePixelRatio_MatchesTheCssBoxTimesThatRatio(double devicePixelRatio)
	{
		// Arrange & Act
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeRotatingCube, devicePixelRatio);
		var probe = await storyPage.ProbeAsync();

		// Assert
		probe.DevicePixelRatio.ShouldBe(devicePixelRatio);
		probe.CssWidth.ShouldBeGreaterThan(300);

		// Rounding of a fractional CSS box differs by a pixel between the browser's own device-pixel
		// conversion and this arithmetic, which says nothing about the library.
		var expectedWidth = probe.CssWidth * devicePixelRatio;
		var expectedHeight = probe.CssHeight * devicePixelRatio;
		Math.Abs(probe.CanvasWidth - expectedWidth).ShouldBeLessThanOrEqualTo(1);
		Math.Abs(probe.CanvasHeight - expectedHeight).ShouldBeLessThanOrEqualTo(1);
		probe.DrawingBufferWidth.ShouldBe(probe.CanvasWidth);
		probe.DrawingBufferHeight.ShouldBe(probe.CanvasHeight);
	}

	/// <summary>
	/// The canvas is created with <c>alpha: true</c> and never cleared to a colour, so a pixel with
	/// any opacity at all is one the scene drew. That makes "did anything render" a question about
	/// coverage rather than about a particular colour, which no rasteriser can disagree about.
	/// </summary>
	[Fact]
	public async Task RenderedFrame_RotatingCubeStoryOpened_CoversTheMiddleAndLeavesTheCornersClear()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeRotatingCube);
		var centre = await storyPage.CanvasCentreAsync();

		// Act
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureAroundAsync(centre, 20)).CoveredFraction > 0.5,
			DrawTimeout,
			"the cube covered the middle of the canvas");
		var wholeCanvas = await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows);

		// Assert
		var centreSample = await storyPage.CaptureAroundAsync(centre, 20);
		centreSample.CoveredFraction.ShouldBeGreaterThan(0.9);
		centreSample.AverageColor.Alpha.ShouldBeGreaterThan(200);

		// A scene that filled the whole buffer would pass the check above for the wrong reason - a
		// failed clear, say. The cube is small, so most of the canvas must still be untouched.
		wholeCanvas.CoveredFraction.ShouldBeLessThan(0.5);
		wholeCanvas.CoveredFraction.ShouldBeGreaterThan(0);
	}

	/// <summary>
	/// The whole animation path in one assertion: a C# timer changes a rotation, the batch carries it
	/// over interop, the applier writes it, and the render loop draws the result. Any break in that
	/// chain leaves two identical frames.
	/// </summary>
	[Fact]
	public async Task RenderLoop_RotatingCubeStoryLeftRunning_DrawsADifferentFrameAsTheCubeTurns()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeRotatingCube);
		var centre = await storyPage.CanvasCentreAsync();
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureAroundAsync(centre, 20)).CoveredFraction > 0.5,
			DrawTimeout,
			"the cube covered the middle of the canvas");

		// Act
		// Sampled over the cube rather than over the whole canvas: the cube covers a few percent of
		// the buffer, so a whole-canvas mean dilutes a real turn down into the same range as nothing
		// happening at all.
		var firstFrame = await storyPage.CaptureAroundAsync(centre, 20);
		await Task.Delay(TimeSpan.FromMilliseconds(500), TestContext.Current.CancellationToken);
		var laterFrame = await storyPage.CaptureAroundAsync(centre, 20);

		// Assert
		laterFrame.MeanAbsoluteDifferenceFrom(firstFrame).ShouldBeGreaterThan(ChangedThreshold);
	}

	/// <summary>
	/// The declarative scene draws the same picture as the imperative one it mirrors. Compared as
	/// coverage rather than as pixels: both are the same cube at the same rotation, but they are two
	/// separate render loops and demanding identical frames would be demanding they be in phase.
	/// </summary>
	[Fact]
	public async Task DeclarativeScene_ClickToRecolourStoryOpened_DrawsTheSameThreeCubesAsTheImperativeOne()
	{
		// Arrange
		await using var imperativePage = await fixture.OpenStoryAsync(Stories.ImperativeClickToRecolour);
		await using var declarativePage = await fixture.OpenStoryAsync(Stories.DeclarativeClickToRecolour);

		// Act
		var imperativeCoverage = await WaitForDrawnCoverageAsync(imperativePage);
		var declarativeCoverage = await WaitForDrawnCoverageAsync(declarativePage);

		// Assert
		declarativeCoverage.ShouldBe(imperativeCoverage, tolerance: 0.01);
		imperativePage.ConsoleErrors.ShouldBeEmpty();
		declarativePage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>
	/// Every story loads with a clean console. This is the check that would have caught a vendored
	/// addon missing a sibling chunk, an interop signature drifting from its JavaScript counterpart,
	/// or a Blazor render exception the UI otherwise swallows.
	/// </summary>
	/// <param name="storyId">Story to open.</param>
	[Theory]
	[MemberData(nameof(EveryStory))]
	public async Task Console_StoryOpened_ReportsNoErrorsAndNoFailedRequests(string storyId)
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(storyId);

		// Act
		await WaitForDrawnCoverageAsync(storyPage);

		// Assert
		// The unserved URLs first: a subresource that 404s also shows up as a console error, and the
		// console line does not name the file while this does.
		storyPage.Responses.Where(x => x.Status >= 400).Select(x => $"{x.Status} {x.Url}").ShouldBeEmpty();
		storyPage.FailedRequests.ShouldBeEmpty();
		storyPage.ConsoleErrors.ShouldBeEmpty();

		// A story whose scene-building throws leaves its controls rendered and inert: nothing here
		// fails, the canvas is merely empty, and the only sign is a banner no other assertion reads.
		(await storyPage.ReadVisibleFailuresAsync()).ShouldBeEmpty();
	}

	/// <summary>Every story id in the demo, as theory data.</summary>
	public static TheoryData<string> EveryStory
	{
		get
		{
			var data = new TheoryData<string>();
			foreach (var storyId in Stories.All)
			{
				data.Add(storyId);
			}

			return data;
		}
	}

	/// <summary>
	/// Waits until the story has drawn something and answers with how much of the canvas it covers.
	/// </summary>
	/// <param name="storyPage">Story to wait on.</param>
	private static async Task<double> WaitForDrawnCoverageAsync(StoryPage storyPage)
	{
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows)).CoveredFraction > 0,
			DrawTimeout,
			"the scene drew something");

		var sample = await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows);
		return sample.CoveredFraction;
	}
}
