using Microsoft.Playwright;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// One BlazingStory story open in its own browser context, with everything the browser said about it
/// while it loaded — console errors, failed requests, the URL of every response — recorded from
/// before the first navigation.
/// </summary>
internal sealed class StoryPage : IAsyncDisposable
{
	/// <summary>How long a story is given to create its WebGL context and size its drawing buffer.</summary>
	private static readonly TimeSpan ReadyTimeout = TimeSpan.FromSeconds(60);

	/// <summary>Gap between polls in <see cref="WaitUntilAsync"/>.</summary>
	private static readonly TimeSpan ConditionPollInterval = TimeSpan.FromMilliseconds(100);

	/// <summary>
	/// Reads the canvas back out of the live drawing buffer, scaled into a rectangle of the caller's
	/// choosing.
	/// </summary>
	/// <remarks>
	/// The renderer is created without <c>preserveDrawingBuffer</c>, so the buffer is cleared as the
	/// frame is composited and anything read after that is blank. Animation-frame callbacks run
	/// before compositing, and the interop module re-arms its own callback from inside its current
	/// one — so a callback registered here is always behind the module's for the frame that follows,
	/// and therefore runs after that frame has been rendered and before it has been thrown away. The
	/// nesting is what puts this callback in the right frame rather than the one already in flight.
	/// </remarks>
	private const string CaptureScript = """
		([sourceX, sourceY, sourceWidth, sourceHeight, targetWidth, targetHeight]) => new Promise(resolve => {
			const canvas = document.querySelector('canvas');
			requestAnimationFrame(() => requestAnimationFrame(() => {
				const scratch = document.createElement('canvas');
				scratch.width = targetWidth;
				scratch.height = targetHeight;
				const context = scratch.getContext('2d', { willReadFrequently: true });
				context.clearRect(0, 0, targetWidth, targetHeight);
				context.drawImage(canvas, sourceX, sourceY, sourceWidth, sourceHeight, 0, 0, targetWidth, targetHeight);
				resolve(Array.from(context.getImageData(0, 0, targetWidth, targetHeight).data));
			}));
		})
		""";

	/// <summary>
	/// Asks the canvas for the context three.js already put on it. <c>getContext</c> hands back the
	/// existing one rather than making a second, so this reports what the library created rather than
	/// what this script could create.
	/// </summary>
	private const string ProbeScript = """
		() => {
			const canvas = document.querySelector('canvas');
			if (!canvas) {
				return null;
			}

			const box = canvas.getBoundingClientRect();
			const gl = canvas.getContext('webgl2') ?? canvas.getContext('webgl');
			let renderer = '';
			if (gl) {
				const debugInfo = gl.getExtension('WEBGL_debug_renderer_info');
				renderer = debugInfo ? gl.getParameter(debugInfo.UNMASKED_RENDERER_WEBGL) : gl.getParameter(gl.RENDERER);
			}

			return {
				hasWebGlContext: !!gl,
				contextVersion: gl ? gl.getParameter(gl.VERSION) : '',
				renderer: renderer,
				drawingBufferWidth: gl ? gl.drawingBufferWidth : 0,
				drawingBufferHeight: gl ? gl.drawingBufferHeight : 0,
				canvasWidth: canvas.width,
				canvasHeight: canvas.height,
				cssWidth: box.width,
				cssHeight: box.height,
				devicePixelRatio: window.devicePixelRatio
			};
		}
		""";

	/// <summary>
	/// Readiness: a canvas exists, three.js put a WebGL context on it, and something has sized the
	/// drawing buffer away from the 300x150 default a bare <c>&lt;canvas&gt;</c> starts with.
	/// </summary>
	private const string ReadyScript = """
		() => {
			const canvas = document.querySelector('canvas');
			if (!canvas) {
				return false;
			}

			const gl = canvas.getContext('webgl2') ?? canvas.getContext('webgl');
			return !!gl && !(canvas.width === 300 && canvas.height === 150);
		}
		""";

	private readonly IBrowserContext _browserContext;
	private readonly List<string> _consoleErrors = [];
	private readonly List<string> _failedRequests = [];
	private readonly List<ResponseRecord> _responses = [];

	/// <summary>The page the story is open in.</summary>
	public IPage Page { get; }

	/// <summary>The story's canvas element.</summary>
	public ILocator Canvas
	{
		get
		{
			return Page.Locator("canvas");
		}
	}

	/// <summary>Console errors and unhandled exceptions the page reported, noise already filtered out.</summary>
	public IReadOnlyList<string> ConsoleErrors
	{
		get
		{
			lock (_consoleErrors)
			{
				return _consoleErrors.Distinct().ToArray();
			}
		}
	}

	/// <summary>Requests the browser could not complete at all, as "url: reason".</summary>
	public IReadOnlyList<string> FailedRequests
	{
		get
		{
			lock (_failedRequests)
			{
				return _failedRequests.Distinct().ToArray();
			}
		}
	}

	/// <summary>Every response the page received, in arrival order.</summary>
	public IReadOnlyList<ResponseRecord> Responses
	{
		get
		{
			lock (_responses)
			{
				return _responses.ToArray();
			}
		}
	}

	/// <summary>
	/// What the page itself is admitting went wrong, in the two places a failure can end up on screen
	/// without failing anything else a test looks at: the framework's own error banner, raised by any
	/// unhandled exception a story's scene-building throws, and the canvas's failure box, rendered when
	/// the renderer never started. Both leave the story looking merely empty — the console is the only
	/// other trace, and a story that swallows its own exception does not even leave that.
	/// </summary>
	/// <returns>One line per visible failure, empty when the page is admitting nothing.</returns>
	public async Task<IReadOnlyList<string>> ReadVisibleFailuresAsync()
	{
		var failures = new List<string>();

		var errorBanner = Page.Locator("#blazor-error-ui");
		if (await errorBanner.CountAsync() > 0 && await errorBanner.First.IsVisibleAsync())
		{
			failures.Add("the framework's unhandled-error banner is showing");
		}

		var canvasFailure = Page.Locator("[data-testid=three-canvas-error]");
		if (await canvasFailure.CountAsync() > 0)
		{
			failures.Add($"the canvas reported no renderer: {(await canvasFailure.First.TextContentAsync())?.Trim()}");
		}

		return failures;
	}

	private StoryPage(IBrowserContext browserContext, IPage page)
	{
		_browserContext = browserContext;
		Page = page;
	}

	/// <summary>
	/// Navigates to a story canvas and returns once it is rendering.
	/// </summary>
	/// <param name="browserContext">Context the story gets a page in, and which this takes ownership of.</param>
	/// <param name="baseUrl">Root URL the demo is served from.</param>
	/// <param name="storyId">BlazingStory story id.</param>
	public static async Task<StoryPage> OpenAsync(IBrowserContext browserContext, string baseUrl, string storyId)
	{
		var page = await browserContext.NewPageAsync();
		var storyPage = new StoryPage(browserContext, page);
		storyPage.Subscribe();

		// The story canvas rather than the shell: it is the same component under test with none of
		// the sidebar, panels and cross-frame hops in front of it.
		await page.GotoAsync(
			$"{baseUrl}/iframe.html?viewMode=story&id={storyId}",
			new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

		await page.WaitForFunctionAsync(ReadyScript, null, new PageWaitForFunctionOptions { Timeout = (float) ReadyTimeout.TotalMilliseconds });
		return storyPage;
	}

	/// <summary>Starts recording what the browser says, before anything has been navigated to.</summary>
	private void Subscribe()
	{
		// Every console error is recorded, with no exclusion list. A filter wide enough to drop a
		// stray "Failed to load resource" is exactly wide enough to hide a vendored module chunk that
		// went missing, which is the defect these assertions exist for; the one message that did turn
		// up unrelated - a favicon the demo does not ship - is answered at its source instead.
		Page.Console += (_, message) =>
		{
			if (message.Type != "error")
			{
				return;
			}

			// The location, not just the text: a failed subresource logs the same line whatever it
			// was, and without the URL a failure here says nothing about which file went missing.
			lock (_consoleErrors)
			{
				_consoleErrors.Add($"{FirstLine(message.Text)} [{message.Location}]");
			}
		};

		Page.PageError += (_, error) =>
		{
			lock (_consoleErrors)
			{
				_consoleErrors.Add(FirstLine(error));
			}
		};

		Page.RequestFailed += (_, request) =>
		{
			lock (_failedRequests)
			{
				_failedRequests.Add($"{request.Url}: {request.Failure}");
			}
		};

		Page.Response += (_, response) =>
		{
			lock (_responses)
			{
				_responses.Add(new ResponseRecord { Url = response.Url, Status = response.Status });
			}
		};
	}

	/// <summary>Reads what the browser made of the canvas: its context, its buffer and its CSS box.</summary>
	public async Task<CanvasProbe> ProbeAsync()
	{
		var probe = await Page.EvaluateAsync<CanvasProbe?>(ProbeScript);
		return probe ?? throw new InvalidOperationException("The story has no canvas element.");
	}

	/// <summary>
	/// Samples the whole canvas, scaled down to the given grid.
	/// </summary>
	/// <param name="columns">Grid width.</param>
	/// <param name="rows">Grid height.</param>
	public async Task<CanvasSample> CaptureWholeCanvasAsync(int columns, int rows)
	{
		var probe = await ProbeAsync();
		return await CaptureAsync(0, 0, probe.CanvasWidth, probe.CanvasHeight, columns, rows);
	}

	/// <summary>
	/// Samples a square of drawing-buffer pixels centred on a point given in viewport coordinates,
	/// which is the space clicks are expressed in.
	/// </summary>
	/// <param name="viewportPoint">Centre of the square, in viewport CSS pixels.</param>
	/// <param name="radiusInCssPixels">Half the square's side, in CSS pixels.</param>
	public async Task<CanvasSample> CaptureAroundAsync(ViewportPoint viewportPoint, int radiusInCssPixels)
	{
		var probe = await ProbeAsync();
		var box = await Canvas.BoundingBoxAsync() ?? throw new InvalidOperationException("The canvas has no layout box.");
		var bufferScale = probe.CanvasWidth / box.Width;

		var sourceX = (int) Math.Round((viewportPoint.X - box.X - radiusInCssPixels) * bufferScale);
		var sourceY = (int) Math.Round((viewportPoint.Y - box.Y - radiusInCssPixels) * bufferScale);
		var side = (int) Math.Round(radiusInCssPixels * 2 * bufferScale);
		return await CaptureAsync(sourceX, sourceY, side, side, side, side);
	}

	private async Task<CanvasSample> CaptureAsync(int sourceX, int sourceY, int sourceWidth, int sourceHeight, int targetWidth, int targetHeight)
	{
		var channels = await Page.EvaluateAsync<int[]>(
			CaptureScript,
			new[] { sourceX, sourceY, sourceWidth, sourceHeight, targetWidth, targetHeight });

		return new CanvasSample
		{
			Width = targetWidth,
			Height = targetHeight,
			Pixels = channels.Select(x => (byte) x).ToArray()
		};
	}

	/// <summary>
	/// Where a world-space point on the camera's focal plane lands in the viewport, for a perspective
	/// camera looking down -Z from a known distance. Derived from the canvas's measured box rather
	/// than from the aspect the story passed to the camera constructor, because the interop module
	/// overwrites that aspect with the canvas's own the moment the scene becomes active.
	/// </summary>
	/// <param name="worldX">World X, in the plane the camera is focused on.</param>
	/// <param name="worldY">World Y, in the plane the camera is focused on.</param>
	/// <param name="cameraDistance">Distance from the camera to that plane, in world units.</param>
	/// <param name="fieldOfViewDegrees">The camera's vertical field of view.</param>
	public async Task<ViewportPoint> ProjectAsync(double worldX, double worldY, double cameraDistance, double fieldOfViewDegrees)
	{
		var box = await Canvas.BoundingBoxAsync() ?? throw new InvalidOperationException("The canvas has no layout box.");
		var halfHeightInWorldUnits = cameraDistance * Math.Tan(fieldOfViewDegrees * Math.PI / 360);
		var halfWidthInWorldUnits = halfHeightInWorldUnits * (box.Width / box.Height);

		var normalizedX = worldX / halfWidthInWorldUnits;
		var normalizedY = worldY / halfHeightInWorldUnits;
		return new ViewportPoint(
			box.X + (normalizedX + 1) / 2 * box.Width,
			box.Y + (1 - normalizedY) / 2 * box.Height);
	}

	/// <summary>The centre of the canvas, in viewport coordinates.</summary>
	public async Task<ViewportPoint> CanvasCentreAsync()
	{
		var box = await Canvas.BoundingBoxAsync() ?? throw new InvalidOperationException("The canvas has no layout box.");
		return new ViewportPoint(box.X + box.Width / 2, box.Y + box.Height / 2);
	}

	/// <summary>Clicks a point in viewport coordinates with a real mouse press and release.</summary>
	/// <param name="viewportPoint">Point to click.</param>
	public async Task ClickAsync(ViewportPoint viewportPoint)
	{
		await Page.Mouse.ClickAsync((float) viewportPoint.X, (float) viewportPoint.Y);
	}

	/// <summary>
	/// Polls a condition until it holds or the timeout elapses, then fails with the caller's own
	/// description of what never happened.
	/// </summary>
	/// <param name="conditionAsync">Condition to poll.</param>
	/// <param name="timeout">How long to keep polling.</param>
	/// <param name="description">What the condition means, for the failure message.</param>
	public async Task WaitUntilAsync(Func<Task<bool>> conditionAsync, TimeSpan timeout, string description)
	{
		var deadline = DateTime.UtcNow + timeout;
		while (DateTime.UtcNow < deadline)
		{
			if (await conditionAsync())
			{
				return;
			}

			await Task.Delay(ConditionPollInterval);
		}

		throw new TimeoutException($"Timed out after {timeout} waiting until {description}.");
	}

	private static string FirstLine(string text)
	{
		var lines = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
		return lines.Length == 0 ? text : lines.First();
	}

	/// <summary>Closes the page and the context it was opened in.</summary>
	public async ValueTask DisposeAsync()
	{
		await _browserContext.CloseAsync();
	}
}

/// <summary>A point in viewport CSS pixels, which is the space Playwright's mouse works in.</summary>
/// <param name="X">Horizontal position.</param>
/// <param name="Y">Vertical position.</param>
internal sealed record ViewportPoint(double X, double Y);

/// <summary>One response the page received.</summary>
internal sealed record ResponseRecord
{
	/// <summary>URL that was fetched.</summary>
	public required string Url { get; init; }

	/// <summary>HTTP status the server answered with.</summary>
	public required int Status { get; init; }
}

/// <summary>What the browser reports about the canvas the library set up.</summary>
internal sealed record CanvasProbe
{
	/// <summary>Whether the canvas carries a WebGL context at all.</summary>
	public required bool HasWebGlContext { get; init; }

	/// <summary>The context's <c>VERSION</c> string, for example <c>WebGL 2.0 (OpenGL ES 3.0 Chromium)</c>.</summary>
	public required string ContextVersion { get; init; }

	/// <summary>The unmasked renderer string, which names the rasteriser actually in use.</summary>
	public required string Renderer { get; init; }

	/// <summary>Width of the context's drawing buffer, in device pixels.</summary>
	public required int DrawingBufferWidth { get; init; }

	/// <summary>Height of the context's drawing buffer, in device pixels.</summary>
	public required int DrawingBufferHeight { get; init; }

	/// <summary>The canvas element's <c>width</c> attribute, in device pixels.</summary>
	public required int CanvasWidth { get; init; }

	/// <summary>The canvas element's <c>height</c> attribute, in device pixels.</summary>
	public required int CanvasHeight { get; init; }

	/// <summary>Width of the canvas's CSS box, in CSS pixels.</summary>
	public required double CssWidth { get; init; }

	/// <summary>Height of the canvas's CSS box, in CSS pixels.</summary>
	public required double CssHeight { get; init; }

	/// <summary>The page's device pixel ratio.</summary>
	public required double DevicePixelRatio { get; init; }
}
