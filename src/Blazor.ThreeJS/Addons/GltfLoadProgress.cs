using Microsoft.JSInterop;

namespace Kebechet.Blazor.ThreeJS.Addons;

/// <summary>
/// How far a model load has got, as the browser's own fetch reports it.
/// </summary>
public sealed class GltfLoadProgress
{
	/// <summary>Bytes fetched so far.</summary>
	public required long BytesLoaded { get; init; }

	/// <summary>
	/// Total bytes the response declared, or <see langword="null"/> when it declared none.
	/// <para>
	/// ⚠️ Absent more often than it looks. A server that streams the response, or compresses it without
	/// a <c>Content-Length</c>, reports a total of zero — which is not "an empty file" but "unknown", so
	/// it is surfaced as null rather than as a number a progress bar would divide by.
	/// </para>
	/// </summary>
	public required long? BytesTotal { get; init; }

	/// <summary>
	/// Fraction complete in the range 0 to 1, or <see langword="null"/> when
	/// <see cref="BytesTotal"/> is unknown. A caller with nothing to divide by wants an indeterminate
	/// progress bar, and this says so instead of answering zero forever.
	/// <para>
	/// ⚠️ Also null once the two numbers contradict each other, which a compressed response makes
	/// ordinary rather than exotic: <c>Content-Length</c> counts the bytes on the wire while the browser
	/// counts the bytes it decoded, so a gzipped model passes its own declared total long before it has
	/// finished — the demo's own figure reported 597%. A total the load has already overtaken is not a
	/// total, and the same rule applies as above: say unknown rather than answer with a number that is
	/// wrong in a direction nobody checks for.
	/// </para>
	/// </summary>
	public double? Fraction
	{
		get
		{
			return BytesTotal is > 0 && BytesLoaded <= BytesTotal.Value
				? (double) BytesLoaded / BytesTotal.Value
				: null;
		}
	}
}

/// <summary>
/// Receives progress events from the browser's fetch and forwards them to the caller's
/// <see cref="IProgress{T}"/>.
/// <para>
/// This is the only channel in the package that runs JavaScript-to-C# during an operation, and it is
/// deliberately the only one. Its call count is bounded by the fetch — a handful of events for a
/// model — so it cannot become per-frame traffic. three.js's per-frame hooks
/// (<c>Renderer.setOpaqueSort</c>, <c>onBeforeRender</c>) are refused for exactly that reason:
/// a delegate invoked per object per frame across this boundary is a network round trip per object on
/// a Blazor Server circuit, and an idle scene costing zero interop is the property the whole design
/// rests on.
/// </para>
/// </summary>
internal sealed class GltfProgressReporter
{
	private readonly IProgress<GltfLoadProgress> _progress;

	/// <summary>Wraps the caller's progress sink.</summary>
	/// <param name="progress">Where events are forwarded.</param>
	public GltfProgressReporter(IProgress<GltfLoadProgress> progress)
	{
		_progress = progress;
	}

	/// <summary>
	/// Called by the applier for each progress event the browser's fetch raises.
	/// <para>
	/// Public and <c>[JSInvokable]</c> because the browser has to be able to reach it; it is not part
	/// of the surface a consumer calls.
	/// </para>
	/// </summary>
	/// <param name="bytesLoaded">Bytes fetched so far.</param>
	/// <param name="bytesTotal">Bytes the response declared, or zero when it declared none.</param>
	[JSInvokable]
	public void ReportProgress(long bytesLoaded, long bytesTotal)
	{
		_progress.Report(new GltfLoadProgress
		{
			BytesLoaded = bytesLoaded,
			BytesTotal = bytesTotal > 0 ? bytesTotal : null
		});
	}
}
