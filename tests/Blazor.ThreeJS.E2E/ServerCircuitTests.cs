using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// The claim this suite exists to stop being a claim: that the package works over a Blazor Server
/// circuit, not only in a WebAssembly runtime that shares an address space with the browser.
/// <para>
/// Over Server every op is a SignalR message, every callback into C# is a round trip, and the
/// interop-per-frame figure the README quotes is the difference between usable and unusable. None of
/// that is exercised anywhere else here: every other test in this project runs against the
/// WebAssembly host, where a "round trip" is a function call.
/// </para>
/// <para>
/// The stories are the same files - both hosts reference <c>Blazor.ThreeJS.Demo.Stories</c> - so a story
/// that works in one and not the other is a hosting-model difference and nothing else.
/// </para>
/// </summary>
[Collection(ServerDemoCollectionDefinition.Name)]
public sealed class ServerCircuitTests(ServerDemoFixture fixture, ITestOutputHelper output)
{
	/// <summary>Grid a whole-canvas sample is scaled into, matching the WebAssembly suite's.</summary>
	private const int SignatureColumns = 96;

	/// <summary>Rows in a whole-canvas sample.</summary>
	private const int SignatureRows = 32;

	/// <summary>
	/// How long a story is given to draw. Longer than the WebAssembly suite's, because every op on
	/// this host crosses a network boundary and the first frame waits for the circuit as well as for
	/// the renderer.
	/// </summary>
	private static readonly TimeSpan DrawTimeout = TimeSpan.FromSeconds(30);

	/// <summary>Mean per-channel difference above which two samples are treated as showing different things.</summary>
	private const double ChangedThreshold = 1.0;

	/// <summary>
	/// Radius, in CSS pixels, of the sample taken over a cube.
	/// <para>
	/// ⚠️ Sampled over the cube rather than over the whole canvas, for the reason the WebAssembly suite
	/// records: a cube covers a few percent of the buffer, so a whole-canvas mean dilutes a real change
	/// down into the same range as nothing happening at all. Measured that way this reads 0.08 against a
	/// threshold of 1.0 while the cube is visibly turning.
	/// </para>
	/// </summary>
	private const int CubeSampleRadius = 20;

	/// <summary>Horizontal offset of the outer cubes in the click-to-recolour story, in world units.</summary>
	private const double OuterCubeOffsetX = 2.2;

	/// <summary>Distance the click-to-recolour story puts its camera at.</summary>
	private const double CubeCameraDistance = 6;

	/// <summary>Vertical field of view the click-to-recolour story's camera uses.</summary>
	private const double CubeFieldOfViewDegrees = 75;

	[Fact]
	public async Task WebGlContext_StoryOpenedOverACircuit_IsCreatedAndDrawnInto()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeRotatingCube);

		// Act
		var probe = await storyPage.ProbeAsync();

		// Assert: the module loaded from `_content`, the renderer was built, and the batch that built
		// the scene crossed the circuit - all three fail as one if Server hosting is broken.
		probe.HasWebGlContext.ShouldBeTrue();
		probe.ContextVersion.ShouldStartWith("WebGL");
		probe.DrawingBufferWidth.ShouldBeGreaterThan(0);
		storyPage.ConsoleErrors.ShouldBeEmpty();
		storyPage.FailedRequests.ShouldBeEmpty();
	}

	[Fact]
	public async Task Scene_AnimatedOverACircuit_KeepsChangingBetweenFrames()
	{
		// Arrange: the cube is turned from C#, one property write per frame, so a scene that stops
		// changing means the per-frame batch stopped arriving.
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeRotatingCube);
		var centre = await storyPage.CanvasCentreAsync();
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureAroundAsync(centre, CubeSampleRadius)).CoveredFraction > 0.5,
			DrawTimeout,
			"the cube to cover the middle of the canvas");

		// Act
		var first = await storyPage.CaptureAroundAsync(centre, CubeSampleRadius);
		await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
		var second = await storyPage.CaptureAroundAsync(centre, CubeSampleRadius);

		// Assert
		var difference = first.MeanAbsoluteDifferenceFrom(second);
		output.WriteLine($"mean per-channel difference over one second: {difference:F2}");
		difference.ShouldBeGreaterThan(ChangedThreshold);
	}

	[Fact]
	public async Task Pointer_ClickedOverACircuit_ReachesCSharpAndChangesTheScene()
	{
		// Arrange: picking is the one direction the wire runs the other way. The browser hit-tests and
		// calls back into C#, which over Server is a SignalR message rather than a function call.
		await using var storyPage = await fixture.OpenStoryAsync(Stories.ImperativeClickToRecolour);
		var leftCube = await storyPage.ProjectAsync(-OuterCubeOffsetX, 0, CubeCameraDistance, CubeFieldOfViewDegrees);
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureAroundAsync(leftCube, CubeSampleRadius)).CoveredFraction > 0.5,
			DrawTimeout,
			"the left cube to be drawn");

		var before = await storyPage.CaptureAroundAsync(leftCube, CubeSampleRadius);

		// Act
		await storyPage.ClickAsync(leftCube);
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureAroundAsync(leftCube, CubeSampleRadius))
				.MeanAbsoluteDifferenceFrom(before) > ChangedThreshold,
			DrawTimeout,
			"the click to reach C# and the recoloured cube to come back");

		// Assert
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	[Fact]
	public async Task GltfModel_LoadedOverACircuit_DrawsTheModelItFetched()
	{
		// Arrange: the loader runs in the browser and reports the graph it built back to C#, which
		// mirrors it under browser-minted handles. Over Server that report is a message, and the
		// handles it carries have to survive the trip.
		await using var storyPage = await fixture.OpenStoryAsync(Stories.FigureWithOrbitControls);

		// Act
		await WaitUntilDrawnAsync(storyPage);

		// Assert
		storyPage.ConsoleErrors.ShouldBeEmpty();
		storyPage.FailedRequests.ShouldBeEmpty();
		storyPage.Responses
			.Where(x => x.Status >= 400)
			.Select(x => $"{x.Url}: {x.Status}")
			.ShouldBeEmpty();
	}

	[Fact]
	public async Task Assets_ServedByTheServerHost_ComeFromTheLibrariesTheStoriesShareWithTheOtherHost()
	{
		// Arrange: a Razor class library's assets are published under `_content/<assembly>/`, and a
		// Server host only serves them if the static-asset manifest was loaded. Getting that wrong
		// answers every asset with a 500 while the page itself still returns 200 - so the storybook
		// boots, looks alive, and has no three.js in it.
		await using var storyPage = await fixture.OpenStoryAsync(Stories.CatalogueShaders);

		// Act
		await WaitUntilDrawnAsync(storyPage);

		// Assert
		var contentResponses = storyPage.Responses
			.Where(x => x.Url.Contains("/_content/", StringComparison.Ordinal))
			.ToList();

		contentResponses.ShouldNotBeEmpty();
		contentResponses.Where(x => x.Status >= 400).ShouldBeEmpty();
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>
	/// Every story the demo publishes, opened over the circuit and required to draw. The same list the
	/// WebAssembly suite sweeps, so a story that only works in one host is visible as a difference
	/// between two runs of the same set rather than as a gap in coverage.
	/// </summary>
	/// <param name="storyId">BlazingStory id of the story to open.</param>
	[Theory]
	[MemberData(nameof(EveryStory))]
	public async Task Story_OpenedOverACircuit_DrawsSomethingAndReportsNoError(string storyId)
	{
		// Arrange & Act
		await using var storyPage = await fixture.OpenStoryAsync(storyId);
		await WaitUntilDrawnAsync(storyPage);

		// Assert
		(await storyPage.ReadVisibleFailuresAsync()).ShouldBeEmpty();
		storyPage.ConsoleErrors.ShouldBeEmpty();
		storyPage.FailedRequests.ShouldBeEmpty();
	}

	/// <summary>The story ids, as xUnit theory data.</summary>
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

	/// <summary>Waits until the canvas holds something other than the colour it was cleared to.</summary>
	/// <param name="storyPage">The open story.</param>
	private static async Task WaitUntilDrawnAsync(StoryPage storyPage)
	{
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows)).CoveredFraction > 0,
			DrawTimeout,
			"the story to draw its first frame over the circuit");
	}
}
