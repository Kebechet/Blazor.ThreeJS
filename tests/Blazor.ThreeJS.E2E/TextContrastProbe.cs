using Microsoft.Playwright;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// Measures how readable a run of text actually is, from the pixels the browser painted rather than
/// from the styles that were meant to produce them.
/// </summary>
/// <remarks>
/// Reading <c>getComputedStyle</c> would answer a different question. Story prose renders inside a
/// deliberately transparent document that the storybook shell composites over its own background, so
/// the colour behind a paragraph belongs to an element in another frame and no computed style on the
/// paragraph mentions it. Reconstructing it means walking two ancestor chains applying the compositing
/// rules by hand — a model of the browser, which is exactly the thing that was wrong when this text
/// shipped unreadable. A screenshot is the browser's own answer.
/// </remarks>
internal static class TextContrastProbe
{
	/// <summary>
	/// Decodes a screenshot inside the browser and reports the two colours in it that matter.
	/// </summary>
	/// <remarks>
	/// The image round-trips back into the page because decoding it in C# would mean taking an image
	/// library as a dependency for one measurement. The browser already has a PNG decoder.
	/// <para>
	/// The most common colour in a clip of text is the background it sits on: glyph strokes cover a
	/// small fraction of a paragraph's box. The colour furthest from it is the ink. Antialiasing only
	/// produces blends between the two, so it can pull the furthest colour toward the background and
	/// understate the contrast - never overstate it, which is the direction that would let an
	/// unreadable page pass.
	/// </para>
	/// </remarks>
	private const string AnalyseScript = """
		async (dataUrl) => {
			const image = new Image();
			image.src = dataUrl;
			await image.decode();

			const scratch = document.createElement('canvas');
			scratch.width = image.naturalWidth;
			scratch.height = image.naturalHeight;
			const context = scratch.getContext('2d', { willReadFrequently: true });
			context.drawImage(image, 0, 0);
			const data = context.getImageData(0, 0, scratch.width, scratch.height).data;

			const countsByColor = new Map();
			for (let index = 0; index < data.length; index += 4) {
				const key = (data[index] << 16) | (data[index + 1] << 8) | data[index + 2];
				countsByColor.set(key, (countsByColor.get(key) ?? 0) + 1);
			}

			let backdropKey = 0;
			let backdropCount = -1;
			for (const [key, count] of countsByColor) {
				if (count > backdropCount) {
					backdropCount = count;
					backdropKey = key;
				}
			}

			const backdropRed = (backdropKey >> 16) & 255;
			const backdropGreen = (backdropKey >> 8) & 255;
			const backdropBlue = backdropKey & 255;

			let inkKey = backdropKey;
			let inkDistance = -1;
			for (const key of countsByColor.keys()) {
				const red = (key >> 16) & 255;
				const green = (key >> 8) & 255;
				const blue = key & 255;
				const distance = (red - backdropRed) ** 2 + (green - backdropGreen) ** 2 + (blue - backdropBlue) ** 2;
				if (distance > inkDistance) {
					inkDistance = distance;
					inkKey = key;
				}
			}

			const pixelCount = data.length / 4;
			return {
				backdropRed: backdropRed,
				backdropGreen: backdropGreen,
				backdropBlue: backdropBlue,
				inkRed: (inkKey >> 16) & 255,
				inkGreen: (inkKey >> 8) & 255,
				inkBlue: inkKey & 255,
				backdropFraction: backdropCount / pixelCount,
				distinctColorCount: countsByColor.size
			};
		}
		""";

	/// <summary>
	/// Screenshots one element and reports the contrast between its text and whatever was painted
	/// behind it.
	/// </summary>
	/// <param name="page">Top-level page to screenshot and to decode the result in. The element may
	/// live in a child frame of it — a screenshot clip is in top-level viewport coordinates, which is
	/// also what a locator's bounding box is expressed in.</param>
	/// <param name="textElement">The element whose box is sampled.</param>
	public static async Task<TextContrastSample> MeasureAsync(IPage page, ILocator textElement)
	{
		await textElement.ScrollIntoViewIfNeededAsync();
		var box = await textElement.BoundingBoxAsync() ?? throw new InvalidOperationException("The text element has no layout box.");
		if (box.Width < 1 || box.Height < 1)
		{
			throw new InvalidOperationException($"The text element's box is {box.Width}x{box.Height}, which has no pixels to sample.");
		}

		var png = await page.ScreenshotAsync(new PageScreenshotOptions
		{
			Type = ScreenshotType.Png,
			Clip = new Clip
			{
				X = box.X,
				Y = box.Y,
				Width = box.Width,
				Height = box.Height
			}
		});

		var dataUrl = $"data:image/png;base64,{Convert.ToBase64String(png)}";
		var sample = await page.EvaluateAsync<TextContrastSample?>(AnalyseScript, dataUrl);
		return sample ?? throw new InvalidOperationException("The screenshot could not be analysed.");
	}
}

/// <summary>
/// The two colours found in a clip of text, and how much of it each accounts for.
/// </summary>
internal sealed record TextContrastSample
{
	/// <summary>WCAG AA's contrast floor for text below 18pt, which all story prose is.</summary>
	public const double WcagAaNormalText = 4.5;

	/// <summary>Red channel of the most common colour in the clip.</summary>
	public required int BackdropRed { get; init; }

	/// <summary>Green channel of the most common colour in the clip.</summary>
	public required int BackdropGreen { get; init; }

	/// <summary>Blue channel of the most common colour in the clip.</summary>
	public required int BackdropBlue { get; init; }

	/// <summary>Red channel of the colour furthest from the backdrop.</summary>
	public required int InkRed { get; init; }

	/// <summary>Green channel of the colour furthest from the backdrop.</summary>
	public required int InkGreen { get; init; }

	/// <summary>Blue channel of the colour furthest from the backdrop.</summary>
	public required int InkBlue { get; init; }

	/// <summary>Share of the clip painted in exactly the backdrop colour.</summary>
	public required double BackdropFraction { get; init; }

	/// <summary>
	/// How many distinct colours the clip contains, which is how "nothing rendered" is told apart from
	/// "rendered unreadably". A clip with no glyphs in it is one flat colour; antialiasing means any
	/// text at all produces many, however close to the background it was painted.
	/// </summary>
	public required int DistinctColorCount { get; init; }

	/// <summary>WCAG 2.x contrast ratio between the ink and the backdrop, from 1 to 21.</summary>
	public double ContrastRatio
	{
		get
		{
			var inkLuminance = RelativeLuminance(InkRed, InkGreen, InkBlue);
			var backdropLuminance = RelativeLuminance(BackdropRed, BackdropGreen, BackdropBlue);
			var lighter = Math.Max(inkLuminance, backdropLuminance);
			var darker = Math.Min(inkLuminance, backdropLuminance);
			return (lighter + 0.05) / (darker + 0.05);
		}
	}

	/// <summary>Both colours and the ratio, as the failure message a person can act on.</summary>
	public string Description
	{
		get
		{
			return $"ink rgb({InkRed}, {InkGreen}, {InkBlue}) on backdrop rgb({BackdropRed}, {BackdropGreen}, {BackdropBlue}) " +
				$"is {ContrastRatio:F2}:1 (backdrop covers {BackdropFraction:P0} of the clip, which holds {DistinctColorCount} colours)";
		}
	}

	/// <summary>
	/// Relative luminance per WCAG 2.x, which is the sRGB transfer function undone and the channels
	/// weighted for human sensitivity.
	/// </summary>
	/// <param name="red">Red channel, 0-255.</param>
	/// <param name="green">Green channel, 0-255.</param>
	/// <param name="blue">Blue channel, 0-255.</param>
	private static double RelativeLuminance(int red, int green, int blue)
	{
		return 0.2126 * ToLinear(red) + 0.7152 * ToLinear(green) + 0.0722 * ToLinear(blue);
	}

	/// <param name="channel">One sRGB channel, 0-255.</param>
	private static double ToLinear(int channel)
	{
		var value = channel / 255.0;
		if (value <= 0.03928)
		{
			return value / 12.92;
		}

		return Math.Pow((value + 0.055) / 1.055, 2.4);
	}
}
