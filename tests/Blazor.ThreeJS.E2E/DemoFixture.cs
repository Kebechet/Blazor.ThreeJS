using Microsoft.Playwright;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// The one collection every test in this suite belongs to, so a single demo process and a single
/// browser are shared and the tests run one at a time. They contend for a GPU-less software
/// rasteriser and each one drives real pointer input, neither of which parallelises usefully.
/// </summary>
[CollectionDefinition(Name)]
public sealed class DemoCollectionDefinition : ICollectionFixture<DemoFixture>
{
	/// <summary>Name shared by the collection definition and every <c>[Collection]</c> attribute.</summary>
	public const string Name = "Demo storybook";
}

/// <summary>
/// The same storybook again, hosted over a Blazor Server circuit instead of compiled to WebAssembly.
/// <para>
/// Its own collection, so it gets its own demo process and its own browser: the two hosts cannot share
/// a port, and the tests in either one run against a rendering surface they do not share with anything.
/// </para>
/// </summary>
[CollectionDefinition(Name)]
public sealed class ServerDemoCollectionDefinition : ICollectionFixture<ServerDemoFixture>
{
	/// <summary>Name shared by the collection definition and every <c>[Collection]</c> attribute.</summary>
	public const string Name = "Demo storybook over a Blazor Server circuit";
}

/// <summary>
/// Owns the demo storybook process and the browser that drives it, and hands each test a freshly
/// isolated page.
/// </summary>
public class DemoFixture : IAsyncLifetime
{
	/// <summary>
	/// Flags that pin WebGL to SwiftShader, ANGLE's software rasteriser, rather than whatever GPU
	/// and driver the machine happens to have. A suite that renders on the host GPU produces
	/// different pixels on a developer's machine and on a CI runner, and a test that only fails on
	/// one of them teaches people to ignore red.
	/// <para>
	/// The flag names are not trusted: <c>Rendering_BrowserLaunched_ReportsSwiftShaderAsTheRenderer</c>
	/// reads the renderer string back out of a live context and fails if Chrome quietly ignored them.
	/// <c>--enable-unsafe-swiftshader</c> is what current Chrome needs before it will hand a WebGL
	/// context to a software rasteriser at all; without it <c>getContext</c> returns null.
	/// </para>
	/// </summary>
	private static readonly string[] SoftwareRenderingArgs =
	[
		"--disable-gpu",
		"--use-gl=angle",
		"--use-angle=swiftshader",
		"--enable-unsafe-swiftshader",
		"--disable-dev-shm-usage"
	];

	private static readonly ViewportSize DefaultViewport = new() { Width = 1280, Height = 900 };

	private readonly DemoServer _demoServer;
	private IPlaywright? _playwright;
	private IBrowser? _browser;

	/// <summary>Runs the WebAssembly host, which is what the deployed storybook is.</summary>
	public DemoFixture()
		: this(DemoServer.WebAssemblyHost)
	{
	}

	/// <summary>Runs one of the two demo hosts.</summary>
	/// <param name="projectPath">Project file, as path segments below the repository root.</param>
	private protected DemoFixture(string[] projectPath)
	{
		_demoServer = new DemoServer(projectPath);
	}

	/// <summary>Root URL the demo storybook is served from.</summary>
	public string BaseUrl
	{
		get
		{
			return _demoServer.BaseUrl;
		}
	}

	/// <summary>
	/// Starts the demo and the browser. Tears down whatever it managed to start if any step fails,
	/// so a half-started fixture never leaves the demo holding its port.
	/// </summary>
	public async ValueTask InitializeAsync()
	{
		var isInitialized = false;
		try
		{
			await _demoServer.StartAsync();

			_playwright = await Playwright.CreateAsync();
			_browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
			{
				// The installed Google Chrome rather than a downloaded Chromium: it is already on
				// this machine and on the CI image, so the suite costs no browser download, and the
				// rendering it does is pinned by the flags above rather than by which build it is.
				Channel = "chrome",
				Headless = true,
				Args = SoftwareRenderingArgs
			});

			isInitialized = true;
		}
		finally
		{
			if (!isInitialized)
			{
				await DisposeAsync();
			}
		}
	}

	/// <summary>
	/// Opens one BlazingStory story in its own browser context and waits until the canvas is
	/// rendering, then hands back the page for a test to drive.
	/// </summary>
	/// <param name="storyId">BlazingStory story id, for example <c>components-threecanvas--rotating-cube</c>.</param>
	/// <param name="devicePixelRatio">Device pixel ratio the page should report.</param>
	/// <param name="colorScheme">Colour scheme the page reports a preference for, or null for the default.</param>
	internal async Task<StoryPage> OpenStoryAsync(string storyId, double devicePixelRatio = 1, ColorScheme? colorScheme = null)
	{
		var browserContext = await CreateContextAsync(devicePixelRatio, colorScheme);
		var storyPage = await StoryPage.OpenAsync(browserContext, BaseUrl, storyId);
		return storyPage;
	}

	/// <summary>
	/// Opens the storybook shell — the page a human actually lands on — rather than a story canvas.
	/// </summary>
	/// <param name="colorScheme">Colour scheme the page reports a preference for, or null for the default.</param>
	public async Task<IBrowserContext> OpenShellContextAsync(ColorScheme? colorScheme = null)
	{
		return await CreateContextAsync(devicePixelRatio: 1, colorScheme);
	}

	/// <summary>
	/// Creates an isolated context and answers the browser's favicon request itself.
	/// </summary>
	/// <remarks>
	/// The demo ships no <c>favicon.ico</c>, and headless Chrome asks for one only sometimes — so
	/// without this the request 404s on an unpredictable subset of runs and the console assertions
	/// fail for a reason that has nothing to do with the library. Answered here rather than filtered
	/// out of the console text: a filter broad enough to catch it would also catch a real missing
	/// asset, which is the one thing these tests exist to notice.
	/// </remarks>
	/// <param name="devicePixelRatio">Device pixel ratio the pages in this context report.</param>
	/// <param name="colorScheme">Colour scheme the pages report a preference for, or null for the default.</param>
	private async Task<IBrowserContext> CreateContextAsync(double devicePixelRatio, ColorScheme? colorScheme = null)
	{
		var browser = _browser ?? throw new InvalidOperationException("The browser was not started.");
		var browserContext = await browser.NewContextAsync(new BrowserNewContextOptions
		{
			ViewportSize = DefaultViewport,
			DeviceScaleFactor = (float) devicePixelRatio,
			ColorScheme = colorScheme
		});

		await browserContext.RouteAsync("**/favicon.ico", route => route.FulfillAsync(new RouteFulfillOptions
		{
			Status = 200,
			ContentType = "image/x-icon",
			Body = string.Empty
		}));

		return browserContext;
	}

	/// <summary>
	/// Closes the browser and stops the demo. Safe to call on a fixture that never finished starting:
	/// every step is skipped when the thing it releases was never created.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		if (_browser is not null)
		{
			await _browser.CloseAsync();
			_browser = null;
		}

		_playwright?.Dispose();
		_playwright = null;

		await _demoServer.DisposeAsync();
	}
}

/// <summary>
/// The demo storybook served from a Blazor Server host: the same stories, from the same
/// <c>Blazor.ThreeJS.Demo.Stories</c> library, rendered by a circuit over SignalR rather than by a runtime
/// in the browser.
/// <para>
/// It exists because Server is the harder of the two hosts to be right on and the one nothing else
/// here exercises. Every op crosses a real network boundary, every callback into C# is a SignalR
/// message, and a batch that is merely correct in WebAssembly can still be too chatty to use over a
/// circuit. What the tests in this collection check is that it works at all, and that the interop
/// count per frame is the same as it is in the browser.
/// </para>
/// </summary>
public sealed class ServerDemoFixture : DemoFixture
{
	/// <summary>Runs the Blazor Server host.</summary>
	public ServerDemoFixture()
		: base(DemoServer.ServerHost)
	{
	}
}
