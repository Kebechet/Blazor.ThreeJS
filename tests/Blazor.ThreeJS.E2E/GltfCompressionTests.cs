using Shouldly;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// The opt-in DRACO decoder path. Nothing lower in the test pyramid drives the browser against the
/// real vendored decoder and a real network fetch of it, which is the only way to prove the story
/// actually asks for <c>DRACOLoader.js</c> and the decoder assets it wraps rather than silently falling
/// back to the undecoded, geometry-less load <see cref="GLTFLoaderTests"/> and
/// <c>tests/wire-format.test.mjs</c> already pin from the C# and JavaScript sides.
/// </summary>
[Collection(DemoCollectionDefinition.Name)]
public sealed class GltfCompressionTests(DemoFixture fixture)
{
	/// <summary>
	/// Every module chunk the compression story must fetch beyond what a plain, uncompressed load
	/// already needs: the DRACOLoader addon itself, and the two decoder files it actually asks for in
	/// a WebAssembly-capable browser - <c>draco_wasm_wrapper.js</c>, the glue Emscripten generates, and
	/// <c>draco_decoder.wasm</c>, the decoder itself. <c>draco_decoder.js</c>, the pure-JavaScript
	/// fallback DRACOLoader only reaches for when <c>WebAssembly</c> is unavailable, is vendored
	/// alongside them but deliberately left off this list: Chrome always has <c>WebAssembly</c>, so it
	/// is never fetched, and asserting a file this suite would never see would make the assertion a
	/// lie about what the load actually requests. A sibling any of these imports but this list omits
	/// is invisible to every other test layer: the build stays green, the C# tests stay green, and the
	/// browser fails at <c>ERR_MODULE_NOT_FOUND</c> or "No DRACOLoader instance provided" on the one
	/// page that actually opts in.
	/// </summary>
	private static readonly string[] RequiredModuleChunks =
	[
		"/_content/Kebechet.Blazor.ThreeJS/three-interop.js",
		"/_content/Kebechet.Blazor.ThreeJS/three.webgpu.min.js",
		"/_content/Kebechet.Blazor.ThreeJS/three.core.min.js",
		"/_content/Kebechet.Blazor.ThreeJS/addons/loaders/GLTFLoader.js",
		"/_content/Kebechet.Blazor.ThreeJS/addons/controls/OrbitControls.js",
		"/_content/Kebechet.Blazor.ThreeJS/addons/loaders/DRACOLoader.js",
		"/_content/Kebechet.Blazor.ThreeJS/addons/libs/draco/gltf/draco_decoder.wasm",
		"/_content/Kebechet.Blazor.ThreeJS/addons/libs/draco/gltf/draco_wasm_wrapper.js"
	];

	/// <summary>The Draco-compressed model the story loads, resolved against the app's base href.</summary>
	private const string ModelUrl = "/models/box-draco.gltf";

	/// <summary>The external buffer the compressed model's own glTF JSON references by URI.</summary>
	private const string ModelBufferUrl = "/models/box-draco.bin";

	/// <summary>How long the model is given to fetch, decode and draw.</summary>
	private static readonly TimeSpan ModelTimeout = TimeSpan.FromSeconds(30);

	[Fact]
	public async Task StaticAssets_CompressionStoryOpened_ServeTheDracoDecoderAlongsideTheLoaderChunks()
	{
		// Arrange
		await using var storyPage = await fixture.OpenStoryAsync(Stories.CompressedModel);

		// Act
		await WaitForModelAsync(storyPage);

		// Assert
		var fetchedUrls = string.Join(Environment.NewLine, storyPage.Responses.Select(x => $"{x.Status} {x.Url}"));
		foreach (var chunk in RequiredModuleChunks.Append(ModelUrl).Append(ModelBufferUrl))
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
	/// A file whose mesh only exists compressed has nothing to draw unless the decoder actually ran -
	/// so a covered canvas is standing proof the wasm decode, not just the fetch, succeeded.
	/// </summary>
	[Fact]
	public async Task CompressedModel_StoryOpened_DecodesAndDrawsTheGeometry()
	{
		// Arrange & Act
		await using var storyPage = await fixture.OpenStoryAsync(Stories.CompressedModel);
		await WaitForModelAsync(storyPage);

		// Assert
		storyPage.ConsoleErrors.ShouldBeEmpty();
	}

	/// <summary>Waits until the compressed model has been fetched, decoded and drawn onto the canvas.</summary>
	/// <param name="storyPage">Story to wait on.</param>
	private static async Task WaitForModelAsync(StoryPage storyPage)
	{
		var centre = await storyPage.CanvasCentreAsync();
		await storyPage.WaitUntilAsync(
			async () => (await storyPage.CaptureAroundAsync(centre, 12)).CoveredFraction > 0.5,
			ModelTimeout,
			"the decoded model was drawn in the middle of the canvas");
	}
}
