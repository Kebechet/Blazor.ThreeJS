using Microsoft.Playwright;
using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// The TSL escape hatch, which is the only route custom shading has under this renderer. Every claim
/// here needs a real browser: the shader is compiled by three.js's node builder from a module the page
/// imports at runtime, so nothing about it can be checked without loading it.
/// </summary>
[Collection(DemoCollectionDefinition.Name)]
public sealed class TslShaderTests(DemoFixture fixture)
{
	private const int SignatureColumns = 96;
	private const int SignatureRows = 32;

	/// <summary>Speed the story starts at, which the readout shows before anything is clicked.</summary>
	private const string InitialSpeedText = "1.5";

	/// <summary>Speed the <c>Fast</c> button asks for.</summary>
	private const string FastSpeedText = "4.0";

	private static readonly TimeSpan DrawTimeout = TimeSpan.FromSeconds(20);

	/// <summary>
	/// Mean absolute per-channel difference above which two samples show different things. The suite
	/// rasterises in software, which is deterministic frame to frame, so an unchanging scene sits at 0.
	/// </summary>
	private const double ChangedThreshold = 1.0;

	/// <summary>
	/// A material whose colour and vertex position both come from a TSL module draws at all. This is
	/// the whole pipeline in one assertion: the vendored TSL bundle resolves, the consumer's module
	/// imports it, <c>LoadNodeAsync</c> mints handles for the nodes it returns, and the node builder
	/// accepts them on a <c>MeshStandardNodeMaterial</c>.
	/// </summary>
	[Fact]
	public async Task TslNodes_ShaderStoryOpened_DrawIntoTheCanvas()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.CatalogueShaders);

		// Act
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows)).CoveredFraction > 0.1,
			DrawTimeout,
			"the shaded sphere was drawn");

		// Assert
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>
	/// The sweep advances with no interop behind it. <c>time</c> is a node three.js increments inside
	/// its own frame loop, so the canvas has to keep changing while C# sends nothing at all — which is
	/// what makes a TSL animation free under this package's batching model.
	/// </summary>
	[Fact]
	public async Task TslTimeNode_NothingSentFromCSharp_KeepsChangingTheCanvas()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.CatalogueShaders);
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows)).CoveredFraction > 0.1,
			DrawTimeout,
			"the shaded sphere was drawn");

		// Act
		var firstFrame = await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows);
		await storyPage.Page.WaitForTimeoutAsync(500);
		var laterFrame = await storyPage.CaptureWholeCanvasAsync(SignatureColumns, SignatureRows);

		// Assert
		laterFrame.MeanAbsoluteDifferenceFrom(firstFrame).ShouldBeGreaterThan(ChangedThreshold);
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>
	/// A uniform node adopted from JavaScript is writable from C#, and the value that comes back is the
	/// browser's rather than the mirror's. The story reads the uniform after each write instead of
	/// echoing the number it just sent, so a write that never reached the running shader would leave
	/// the readout showing the old value.
	/// </summary>
	[Fact]
	public async Task TslUniform_SpeedButtonClicked_ReadsBackTheValueCSharpWrote()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.CatalogueShaders);
		var speedReadout = storyPage.Page.GetByTestId("shader-speed");
		await speedReadout.WaitForAsync();
		(await speedReadout.InnerTextAsync()).ShouldBe(InitialSpeedText);

		// Act
		await storyPage.Page
			.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Fast" })
			.ClickAsync();

		// Assert
		await storyPage.WaitUntilAsync(
			async () => await speedReadout.InnerTextAsync() == FastSpeedText,
			DrawTimeout,
			"the shader reported the speed C# wrote");
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}
}
