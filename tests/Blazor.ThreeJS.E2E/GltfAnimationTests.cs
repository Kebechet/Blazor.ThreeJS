using Microsoft.Playwright;
using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// A glTF file's own animation clip, surfaced by <c>GLTFLoader</c> and played back through the
/// ordinary <c>AnimationMixer</c>. What is asserted here needs a real browser and a real clock: the
/// canvas has to keep changing on its own while the clip plays - nothing in C# is writing a transform
/// every frame - and stop changing the moment <c>Stop</c> is clicked, which no lower test layer can show.
/// </summary>
[Collection(DemoCollectionDefinition.Name)]
public sealed class GltfAnimationTests(DemoFixture fixture)
{
	private const int SignatureColumns = 96;
	private const int SignatureRows = 32;

	private static readonly TimeSpan ModelTimeout = TimeSpan.FromSeconds(30);

	/// <summary>Gap between the two captures a "did it change" assertion compares.</summary>
	private static readonly TimeSpan CaptureGap = TimeSpan.FromMilliseconds(600);

	/// <summary>
	/// Mean absolute per-channel difference above which two samples show different things. The suite
	/// rasterises in software, which is deterministic frame to frame, so an unchanging scene sits at 0.
	/// Shared with <see cref="TslShaderTests"/>'s threshold for the same reason it picked this value.
	/// </summary>
	private const double ChangedThreshold = 1.0;

	[Fact]
	public async Task GltfAnimation_ModelLoaded_KeepsChangingTheCanvasWithNothingSentFromCSharpEveryFrame()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.AnimatedModel);
		await WaitForModelAsync(storyPage);

		// Act
		var firstFrame = await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows);
		await storyPage.Page.WaitForTimeoutAsync((float) CaptureGap.TotalMilliseconds);
		var laterFrame = await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows);

		// Assert
		// The story starts the clip playing the moment the model is ready, so no click is needed before
		// this pair of captures - only AnimationMixer.Update, called every frame from C#'s own loop, is
		// what advances it.
		laterFrame.MeanAbsoluteDifferenceFrom(firstFrame).ShouldBeGreaterThan(ChangedThreshold);
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	[Fact]
	public async Task GltfAnimation_StopClicked_FreezesTheCanvas()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.AnimatedModel);
		await WaitForModelAsync(storyPage);

		// Act
		await storyPage.Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Stop" }).ClickAsync();
		// One frame for the stop to reach the mixer and the next draw to reflect it, before the pair of
		// captures that has to show nothing moving from here on.
		await storyPage.Page.WaitForTimeoutAsync(200);
		var firstFrame = await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows);
		await storyPage.Page.WaitForTimeoutAsync((float) CaptureGap.TotalMilliseconds);
		var laterFrame = await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows);

		// Assert
		laterFrame.MeanAbsoluteDifferenceFrom(firstFrame).ShouldBeLessThan(ChangedThreshold);
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>Waits until the model has been fetched, built and drawn onto the canvas.</summary>
	/// <param name="storyPage">Story to wait on.</param>
	private static async Task WaitForModelAsync(StoryPage storyPage)
	{
		// Whole-canvas coverage rather than a window centred on the canvas, unlike
		// GltfFigureTests.WaitForModelAsync: the inner box of this model rises up out of a hollow outer
		// one, so the exact centre pixel is sometimes background depending on where the clip's animation
		// happens to be the instant this polls. The outer box's walls are visible somewhere in the frame
		// regardless of clip phase, which is all readiness needs to prove.
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows)).CoveredFraction > 0.05,
			ModelTimeout,
			"the animated model was drawn onto the canvas");
	}
}
