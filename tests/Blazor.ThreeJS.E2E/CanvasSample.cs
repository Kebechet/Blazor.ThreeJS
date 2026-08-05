namespace Blazor.ThreeJS.E2E;

/// <summary>
/// A rectangle of pixels read back out of the live drawing buffer, as straight RGBA bytes.
/// </summary>
/// <remarks>
/// Nothing here is compared against a stored baseline. Every comparison this suite makes is between
/// two samples taken from the same browser, seconds apart — before and after a click, one frame and
/// the next — so a difference can only come from the thing the test did. A committed baseline image
/// would instead be compared against whatever rasteriser the machine has, which is how a suite ends
/// up red on CI for reasons no one can act on.
/// </remarks>
internal sealed record CanvasSample
{
	/// <summary>Width of the sampled rectangle, in pixels.</summary>
	public required int Width { get; init; }

	/// <summary>Height of the sampled rectangle, in pixels.</summary>
	public required int Height { get; init; }

	/// <summary>Straight RGBA bytes, four per pixel, in row-major order.</summary>
	public required byte[] Pixels { get; init; }

	/// <summary>
	/// Fraction of pixels the renderer actually covered. The canvas is created with
	/// <c>alpha: true</c> and nothing clears it to a colour, so anything the scene did not draw stays
	/// fully transparent — which makes this the honest answer to "did it render anything".
	/// </summary>
	public double CoveredFraction
	{
		get
		{
			var coveredCount = 0;
			for (var index = 3; index < Pixels.Length; index += 4)
			{
				if (Pixels.ElementAt(index) > 0)
				{
					coveredCount++;
				}
			}

			return (double) coveredCount / (Width * Height);
		}
	}

	/// <summary>Mean of every channel over the whole rectangle.</summary>
	public CanvasColor AverageColor
	{
		get
		{
			double red = 0;
			double green = 0;
			double blue = 0;
			double alpha = 0;
			for (var index = 0; index < Pixels.Length; index += 4)
			{
				red += Pixels.ElementAt(index);
				green += Pixels.ElementAt(index + 1);
				blue += Pixels.ElementAt(index + 2);
				alpha += Pixels.ElementAt(index + 3);
			}

			var pixelCount = (double) (Width * Height);
			return new CanvasColor(red / pixelCount, green / pixelCount, blue / pixelCount, alpha / pixelCount);
		}
	}

	/// <summary>
	/// Mean absolute per-channel difference against another sample of the same shape, on the 0-255
	/// scale. This is the tolerance knob: "changed" and "unchanged" are both judged against it, so
	/// neither is a demand for identical bytes.
	/// </summary>
	/// <param name="other">Sample to compare against.</param>
	public double MeanAbsoluteDifferenceFrom(CanvasSample other)
	{
		if (Width != other.Width || Height != other.Height)
		{
			throw new ArgumentException($"Cannot compare a {Width}x{Height} sample with a {other.Width}x{other.Height} one.", nameof(other));
		}

		double total = 0;
		for (var index = 0; index < Pixels.Length; index++)
		{
			total += Math.Abs(Pixels.ElementAt(index) - other.Pixels.ElementAt(index));
		}

		return total / Pixels.Length;
	}
}

/// <summary>Mean colour of a sampled rectangle, one component per channel on the 0-255 scale.</summary>
/// <param name="Red">Mean red channel.</param>
/// <param name="Green">Mean green channel.</param>
/// <param name="Blue">Mean blue channel.</param>
/// <param name="Alpha">Mean alpha channel.</param>
internal sealed record CanvasColor(double Red, double Green, double Blue, double Alpha)
{
	/// <summary>
	/// Mean absolute difference across the three colour channels, ignoring alpha, on the 0-255 scale.
	/// </summary>
	/// <param name="other">Colour to compare against.</param>
	public double MeanChannelDifferenceFrom(CanvasColor other)
	{
		var total = Math.Abs(Red - other.Red) + Math.Abs(Green - other.Green) + Math.Abs(Blue - other.Blue);
		return total / 3;
	}
}
