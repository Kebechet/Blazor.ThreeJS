using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// The addon story: a glTF model the browser loads and mints handles for, orbit controls that drive
/// the camera without C# in the loop, and a read that goes and asks the browser what it actually has.
/// </summary>
[Collection(DemoCollectionDefinition.Name)]
public sealed class GltfFigureTests(DemoFixture fixture)
{
	/// <summary>
	/// Every module chunk the figure story must fetch. The addon files are hand-vendored and import
	/// each other, and a sibling left behind by a three.js bump is invisible to every other test
	/// layer: the build stays green, the C# tests stay green, and the browser fails at
	/// <c>ERR_MODULE_NOT_FOUND</c> on the one page that imports it.
	/// </summary>
	private static readonly string[] RequiredModuleChunks =
	[
		"/_content/Kebechet.Blazor.ThreeJS/three-interop.js",
		"/_content/Kebechet.Blazor.ThreeJS/three.webgpu.min.js",
		"/_content/Kebechet.Blazor.ThreeJS/three.core.min.js",
		"/_content/Kebechet.Blazor.ThreeJS/addons/loaders/GLTFLoader.js",
		"/_content/Kebechet.Blazor.ThreeJS/addons/controls/OrbitControls.js",
		"/_content/Kebechet.Blazor.ThreeJS/addons/utils/BufferGeometryUtils.js",
		"/_content/Kebechet.Blazor.ThreeJS/addons/utils/SkeletonUtils.js"
	];

	/// <summary>The model the story loads, resolved against the app's base href.</summary>
	private const string ModelUrl = "/models/figure.gltf";

	/// <summary>
	/// The glTF node the camera looks straight at. The camera sits at (0, 0.4, 3.4) with the orbit
	/// target at (0, 0.2, 0), and the torso spans that point.
	/// </summary>
	private const string CentreBodyPartName = "Torso";

	/// <summary>What the story shows before anything has been clicked.</summary>
	private const string NothingClickedYet = "nothing yet";

	/// <summary>Label of the story's button for asking the browser where the camera is.</summary>
	private const string ReadCameraButtonName = "Where is the camera?";

	/// <summary>How long the model is given to download, parse and draw.</summary>
	private static readonly TimeSpan ModelTimeout = TimeSpan.FromSeconds(30);

	/// <summary>How long a click is given to reach C# and come back out as rendered text.</summary>
	private static readonly TimeSpan ClickTimeout = TimeSpan.FromSeconds(10);

	/// <summary>Reads the two positions out of the story's camera readout line.</summary>
	private static readonly Regex CameraReadoutPattern = new(
		@"mirror says \((?<mirror>[^)]*)\), browser says \((?<browser>[^)]*)\)",
		RegexOptions.Compiled);

	[Fact]
	public async Task StaticAssets_FigureStoryOpened_ServeEveryModuleChunkIncludingTheVendoredAddonSiblings()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.FigureWithOrbitControls);

		// Act
		await WaitForModelAsync(storyPage);

		// Assert
		var fetchedUrls = string.Join(Environment.NewLine, storyPage.Responses.Select(x => $"{x.Status} {x.Url}"));
		foreach (var chunk in RequiredModuleChunks.Append(ModelUrl))
		{
			var pattern = FingerprintedAssets.UrlPattern(chunk);
			var responses = storyPage.Responses.Where(x => pattern.IsMatch(x.Url)).ToArray();
			responses.ShouldNotBeEmpty($"The browser never fetched {chunk}. It fetched:{Environment.NewLine}{fetchedUrls}");
			responses.Select(x => x.Status).ShouldAllBe(status => status < 400);
		}

		storyPage.FailedRequests.ShouldBeEmpty();
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>
	/// The model's meshes are built by the browser and mirrored back as handles C# never created. A
	/// click on one has to resolve to the right handle and raise on the right object, which the
	/// story reports by name.
	/// </summary>
	[Fact]
	public async Task PointerPicking_LoadedBodyPartClicked_ReportsItsNameThroughABrowserMintedHandle()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.FigureWithOrbitControls);
		await WaitForModelAsync(storyPage);
		// By test id rather than by element name: the story's prose is free to grow another <strong>
		// without silently retargeting this assertion at the wrong one.
		var lastClickedPart = storyPage.Page.Locator("[data-testid='last-clicked-part']");
		(await lastClickedPart.InnerTextAsync()).ShouldBe(NothingClickedYet);

		// Act
		await storyPage.ClickAsync(await storyPage.CanvasCentreAsync());

		// Assert
		await storyPage.WaitUntilAsync(
			async () => await lastClickedPart.InnerTextAsync() != NothingClickedYet,
			ClickTimeout,
			"the clicked body part was reported");
		(await lastClickedPart.InnerTextAsync()).ShouldBe(CentreBodyPartName);
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>
	/// Orbit controls move the camera entirely on the browser side, so the C# mirror goes stale the
	/// moment they do. The story's read button is the only way to see both numbers at once, and it is
	/// the read path's end-to-end proof: the value comes back from the browser, not from the mirror.
	/// </summary>
	[Fact]
	public async Task OrbitControls_CameraDraggedThenRead_ReportAPositionTheMirrorNoLongerHas()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.FigureWithOrbitControls);
		await WaitForModelAsync(storyPage);
		var readButton = storyPage.Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = ReadCameraButtonName });

		// Act
		await readButton.ClickAsync();
		var beforeOrbit = await ReadCameraPositionsAsync(storyPage);

		var canvasCentre = await storyPage.CanvasCentreAsync();
		await storyPage.Page.Mouse.MoveAsync((float) canvasCentre.X, (float) canvasCentre.Y);
		await storyPage.Page.Mouse.DownAsync();
		await storyPage.Page.Mouse.MoveAsync((float) canvasCentre.X + 120, (float) canvasCentre.Y, new() { Steps = 12 });
		await storyPage.Page.Mouse.UpAsync();

		await readButton.ClickAsync();
		var afterOrbit = await ReadCameraPositionsAsync(storyPage);

		// Assert
		beforeOrbit.Browser.ShouldBe(beforeOrbit.Mirror);
		afterOrbit.Mirror.ShouldBe(beforeOrbit.Mirror);
		afterOrbit.Browser.ShouldNotBe(afterOrbit.Mirror);
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>Waits until the model has been fetched, built and drawn onto the canvas.</summary>
	/// <param name="storyPage">Story to wait on.</param>
	private static async Task WaitForModelAsync(StoryPage storyPage)
	{
		var centre = await storyPage.CanvasCentreAsync();
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureAroundAsync(centre, 12)).CoveredFraction > 0.99,
			ModelTimeout,
			"the loaded model was drawn in the middle of the canvas");
	}

	/// <summary>
	/// Parses the story's readout line into the position C# last wrote and the one the browser
	/// answered with.
	/// </summary>
	/// <param name="storyPage">Story showing the readout.</param>
	private static async Task<CameraReadout> ReadCameraPositionsAsync(StoryPage storyPage)
	{
		var text = await storyPage.Page.Locator("body").InnerTextAsync();
		var match = CameraReadoutPattern.Match(text);
		match.Success.ShouldBeTrue($"The story never printed a camera readout. Page text was:{Environment.NewLine}{text}");
		return new CameraReadout
		{
			Mirror = ParsePosition(match.Groups["mirror"].Value),
			Browser = ParsePosition(match.Groups["browser"].Value)
		};
	}

	/// <summary>Turns "0.00, 0.40, 3.40" into three numbers.</summary>
	/// <param name="text">The formatted components.</param>
	private static IReadOnlyList<double> ParsePosition(string text)
	{
		return text
			.Split(',')
			.Select(x => double.Parse(x.Trim(), CultureInfo.InvariantCulture))
			.ToArray();
	}

	/// <summary>The two positions the story prints side by side.</summary>
	private sealed record CameraReadout
	{
		/// <summary>What C# last wrote into its mirrored camera.</summary>
		public required IReadOnlyList<double> Mirror { get; init; }

		/// <summary>What the browser answered when asked for the live camera.</summary>
		public required IReadOnlyList<double> Browser { get; init; }
	}
}
