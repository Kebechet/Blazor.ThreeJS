using Microsoft.Playwright;
using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// Whether a story's prose can actually be read, measured from painted pixels.
/// </summary>
/// <remarks>
/// Story text once shipped at roughly 1.1:1 against the shell's background — black on near-black,
/// invisible. Every canvas test addresses <c>iframe.html</c> directly, where the same text sat on the
/// browser's default white and read perfectly, so the whole suite stayed green. The defect existed
/// only in the composite, which nothing opened.
/// <para>
/// Both schemes are exercised deliberately. The preview document is transparent and declares
/// <c>color-scheme: light dark</c>, so its text colour and the background it lands on are chosen by
/// two different stylesheets responding to the same preference. Pinning one scheme would let the pair
/// fall out of step in the other and still pass — which is the state an earlier attempt at this fix
/// actually shipped in.
/// </para>
/// </remarks>
[Collection(DemoCollectionDefinition.Name)]
public sealed class StoryLegibilityTests(DemoFixture fixture)
{
	/// <summary>How long the storybook is given to boot and render a story's prose.</summary>
	private const float ProseTimeout = 60_000;

	/// <summary>
	/// Least of the clip one flat colour must account for before it is treated as the backdrop. Text
	/// covers a minority of its own box, so a real background wins by a wide margin; anything less
	/// means the clip was a gradient or an image and the measurement below would be meaningless.
	/// </summary>
	private const double MinimumBackdropFraction = 0.3;

	/// <summary>
	/// The story sampled. Any story carrying prose would do — the stylesheet under test is the
	/// preview document's, not any one story's.
	/// </summary>
	private const string ProseStoryId = Stories.ImperativeClickToRecolour;

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task StoryProse_OpenedInTheShell_StaysReadableAgainstTheShellBackground(bool isDarkSchemePreferred)
	{
		// Arrange
		var browserContext = await fixture.OpenShellContextAsync(SchemeFor(isDarkSchemePreferred));
		await using var _ = browserContext;
		var page = await browserContext.NewPageAsync();
		await GoToStoryAsync(page, ProseStoryId);

		var prose = page.FrameLocator("iframe[src*='iframe.html']").Locator("p:not(:empty)").First;
		await prose.WaitForAsync(new LocatorWaitForOptions { Timeout = ProseTimeout });

		// Act
		var sample = await TextContrastProbe.MeasureAsync(page, prose);

		// Assert
		ShouldBeReadable(sample);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task StoryProse_CanvasOpenedDirectly_StaysReadableAgainstThePageBackground(bool isDarkSchemePreferred)
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(ProseStoryId, colorScheme: SchemeFor(isDarkSchemePreferred));
		var prose = storyPage.Page.Locator("p:not(:empty)").First;
		await prose.WaitForAsync(new LocatorWaitForOptions { Timeout = ProseTimeout });

		// Act
		var sample = await TextContrastProbe.MeasureAsync(storyPage.Page, prose);

		// Assert
		ShouldBeReadable(sample);
	}

	/// <summary>
	/// Asserts a sample is text a person could read, checking the two things that would make that
	/// verdict meaningless before checking the verdict itself — so a clip that sampled a gradient, or
	/// one the story never drew into, says so instead of being reported as a contrast failure.
	/// </summary>
	/// <param name="sample">Measurement to judge.</param>
	private static void ShouldBeReadable(TextContrastSample sample)
	{
		sample.BackdropFraction.ShouldBeGreaterThan(MinimumBackdropFraction, sample.Description);
		sample.DistinctColorCount.ShouldBeGreaterThan(1, sample.Description);
		sample.ContrastRatio.ShouldBeGreaterThanOrEqualTo(TextContrastSample.WcagAaNormalText, sample.Description);
	}

	/// <summary>
	/// Navigates the shell to one story by following the link the sidebar publishes for it, so the
	/// test never has to know how BlazingStory spells a story URL.
	/// </summary>
	/// <param name="page">Page to navigate.</param>
	/// <param name="storyId">BlazingStory story id.</param>
	private async Task GoToStoryAsync(IPage page, string storyId)
	{
		await page.GotoAsync(fixture.BaseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

		var storyLink = page.Locator($"a[href$='/story/{storyId}']");

		// Attached rather than visible: the sidebar renders the whole story tree up front but keeps
		// unexpanded branches hidden, and only the link's href is wanted here.
		await storyLink.WaitForAsync(new LocatorWaitForOptions { Timeout = ProseTimeout, State = WaitForSelectorState.Attached });

		var storyUrl = await storyLink.EvaluateAsync<string>("link => link.href");
		await page.GotoAsync(storyUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
	}

	/// <param name="isDarkSchemePreferred">Whether the page should report preferring a dark scheme.</param>
	private static ColorScheme SchemeFor(bool isDarkSchemePreferred)
	{
		if (isDarkSchemePreferred)
		{
			return ColorScheme.Dark;
		}

		return ColorScheme.Light;
	}
}
