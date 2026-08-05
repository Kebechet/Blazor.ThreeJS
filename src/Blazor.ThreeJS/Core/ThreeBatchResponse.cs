using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// What <c>applyBatch</c> hands back after running one batch. A fixed wire format shared with
/// <c>three-interop.js</c>: the two short property names below, and those of
/// <see cref="ThreeReadResult"/>, must change together on both sides.
/// <para>
/// The two lists carry different kinds of failure on purpose. <see cref="Errors"/> holds the ops
/// whose failure has nowhere else to go — a write nobody is awaiting — and is what
/// <c>ThreeContext.OnError</c> publishes. A read that failed is reported on its own result row
/// instead, so it faults the one task that asked for it rather than being announced to every
/// <c>OnError</c> subscriber as well.
/// </para>
/// </summary>
internal sealed class ThreeBatchResponse
{
	/// <summary>Ops the applier rejected, excluding reads, which report on their own result row.</summary>
	[JsonPropertyName("e")]
	public List<ThreeError> Errors { get; init; } = [];

	/// <summary>One row per read op in the batch, in the order the applier ran them.</summary>
	[JsonPropertyName("r")]
	public List<ThreeReadResult> Results { get; init; } = [];
}

/// <summary>
/// The outcome of one read op: either the value the method returned, or why it could not be produced.
/// </summary>
internal sealed class ThreeReadResult
{
	/// <summary>Request id of the read op this answers, echoed back from <c>ThreeOp.RequestId</c>.</summary>
	[JsonPropertyName("i")]
	public int RequestId { get; init; }

	/// <summary>
	/// The returned value in wire form, left undeserialized so <c>ThreeValue.Decode</c> can settle
	/// between a tagged math value and a plain one against the type the query declares.
	/// </summary>
	[JsonPropertyName("v")]
	public JsonElement? Value { get; init; }

	/// <summary>
	/// Why the read failed, when it did: the method threw, the handle is unknown, or the value it
	/// returned has no wire encoding. <see langword="null"/> on a successful read.
	/// </summary>
	[JsonPropertyName("e")]
	public string? Message { get; init; }
}
