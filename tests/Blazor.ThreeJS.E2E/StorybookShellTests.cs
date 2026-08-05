using Microsoft.Playwright;
using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// The storybook shell — the page a human actually lands on. The story canvases are addressed
/// directly everywhere else, which is faster and steadier but leaves the sidebar and its story tree
/// entirely uncovered.
/// </summary>
[Collection(DemoCollectionDefinition.Name)]
public sealed class StorybookShellTests(DemoFixture fixture)
{
	/// <summary>How long the shell is given to boot and build its story tree.</summary>
	private const float SidebarTimeout = 60_000;

	/// <summary>
	/// BlazingStory keys its story tree by the <c>[Stories]</c> path string, so two files declaring
	/// the same path leave only whichever registered last — silently, with a green build and no
	/// console error. Only the rendered sidebar shows it, which is why this assertion is here and
	/// nowhere else.
	/// </summary>
	[Fact]
	public async Task Sidebar_ShellOpened_ListsEveryStoryExactlyOnce()
	{
		// Arrange
		var browserContext = await fixture.OpenShellContextAsync();
		await using var _ = browserContext;
		var page = await browserContext.NewPageAsync();

		// Act
		await page.GotoAsync(fixture.BaseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
		var storyLinks = page.Locator("a[href*='/story/']");
		await storyLinks.First.WaitForAsync(new LocatorWaitForOptions { Timeout = SidebarTimeout });

		var listedStoryIds = (await storyLinks.EvaluateAllAsync<string[]>("links => links.map(link => link.getAttribute('href'))"))
			.Select(x => x.Split("/story/").Last())
			.ToArray();

		// Assert
		listedStoryIds.ShouldBeUnique();
		foreach (var storyId in Stories.All)
		{
			listedStoryIds.ShouldContain(storyId);
		}

		listedStoryIds.Length.ShouldBe(Stories.All.Count);
	}
}
