using Microsoft.Playwright;
using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// That each light type actually lights something.
/// <para>
/// ⚠️ This exists because every other assertion in the suite passed while a light was completely
/// broken. A <c>HemisphereLight</c> used to render the whole scene black, and the suite was happy:
/// the canvas still had coverage, because the geometry was drawn - just black - and nothing was
/// written to the console. "Something was drawn" and "no errors" are both true of a black frame, so
/// neither can tell a working renderer from a dead one. This one looks at the brightness.
/// </para>
/// </summary>
[Collection(DemoCollectionDefinition.Name)]
public sealed class LightRenderingTests(DemoFixture fixture, ITestOutputHelper output)
{
	private const int SignatureColumns = 96;
	private const int SignatureRows = 32;

	private static readonly TimeSpan DrawTimeout = TimeSpan.FromSeconds(20);

	/// <summary>
	/// Mean channel value over the whole canvas, on the 0-255 scale, below which the scene is treated
	/// as unlit. A liveness floor, not a rendering baseline: it only has to separate a black frame from
	/// a lit one.
	/// <para>
	/// Measured on this suite's software rasteriser: ambient 77, directional 90, hemisphere 92,
	/// point 44, spot 7.7. A spot light is the low one by a wide margin because its pool covers a small
	/// part of the frame and this average is over all of it — so the floor sits below that rather than
	/// near the others. A black frame measures essentially zero, which is what this catches.
	/// </para>
	/// </summary>
	private const double LitFloor = 3;

	[Theory]
	[InlineData("AmbientLight")]
	[InlineData("DirectionalLight")]
	[InlineData("HemisphereLight")]
	[InlineData("PointLight")]
	[InlineData("SpotLight")]
	public async Task Light_SelectedInTheLightsStory_ActuallyLightsTheScene(string lightName)
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.CatalogueLights);
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows)).CoveredFraction > 0.1,
			DrawTimeout,
			"the stage was drawn");

		// Act
		// The story disables the button for whichever light is already showing, so the one it opens on
		// needs no click and cannot take one.
		var button = storyPage.Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = lightName, Exact = true });
		if (await button.IsEnabledAsync())
		{
			await button.ClickAsync();
			await storyPage.WaitUntilAsync(
				async () => await button.IsEnabledAsync() == false,
				DrawTimeout,
				$"{lightName} became the shown light");
		}

		// A frame after the switch, since selecting a light is two property writes and the draw that
		// follows them.
		await storyPage.Page.WaitForTimeoutAsync(500);
		var average = (await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows)).AverageColor;
		var brightness = (average.Red + average.Green + average.Blue) / 3;

		// Assert
		output.WriteLine($"{lightName}: mean channel {brightness:0.0}");
		brightness.ShouldBeGreaterThan(
			LitFloor,
			$"{lightName} left the scene at a mean channel value of {brightness:0.0}, which is an unlit frame.");
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}
}
