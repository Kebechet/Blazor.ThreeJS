using System.Text.Json;
using Blazor.ThreeJS.Tests.Core;
using Kebechet.Blazor.ThreeJS.Addons;
using Kebechet.Blazor.ThreeJS.Core;
using Microsoft.JSInterop;

namespace Blazor.ThreeJS.Tests.Addons;

/// <summary>
/// Stands in for the interop module on the two calls the addons make that no other test needs:
/// <c>loadGltf</c>, which answers with a description of a graph, and <c>attachOrbitControls</c>, which
/// answers with a minted handle. Batches are answered too, since both addons flush around their call.
/// <para>
/// What is asserted against this is the C# half only — which ops were recorded, on which handles, and
/// what the mirror knows afterwards. That the addons resolve and run at all is pinned end to end
/// against the vendored bundle by <c>tests/wire-format.test.mjs</c>, which no stub could prove.
/// </para>
/// </summary>
internal sealed class AddonJsObjectReference : IJSObjectReference
{
	/// <summary>Every invocation received so far, in the order it arrived.</summary>
	public List<JsInvocation> Invocations { get; } = [];

	/// <summary>What <c>loadGltf</c> answers with.</summary>
	public GLTFLoadResponse LoadResponse { get; set; } = new();

	/// <summary>The handle <c>attachOrbitControls</c> reports it minted.</summary>
	public int OrbitControlsHandle { get; set; } = -1;

	/// <summary>
	/// Value every read op in a batch is answered with. Left unset, a batch comes back with no result
	/// rows, which is what the write-only tests expect.
	/// </summary>
	public JsonElement? ReadValue { get; set; }

	/// <summary>Ops from every batch that was sent, in the order they were sent.</summary>
	public List<IReadOnlyList<ThreeOp>> AppliedBatches { get; } = [];

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
	{
		Invocations.Add(new JsInvocation { Identifier = identifier, Arguments = args ?? [] });

		if (typeof(TValue) == typeof(GLTFLoadResponse))
		{
			return ValueTask.FromResult((TValue) (object) LoadResponse);
		}

		if (typeof(TValue) == typeof(int))
		{
			return ValueTask.FromResult((TValue) (object) OrbitControlsHandle);
		}

		if (typeof(TValue) != typeof(ThreeBatchResponse))
		{
			return default;
		}

		var ops = args?.OfType<IReadOnlyList<ThreeOp>>().FirstOrDefault() ?? [];
		AppliedBatches.Add(ops);
		var results = ops
			.Where(x => x.Kind == ThreeOpKind.Read)
			.Select(x => new ThreeReadResult { RequestId = x.RequestId, Value = ReadValue })
			.ToList();

		return ValueTask.FromResult((TValue) (object) new ThreeBatchResponse { Results = results });
	}

	public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		return InvokeAsync<TValue>(identifier, args);
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}

	/// <summary>Every op sent so far, flattened across batches.</summary>
	public IReadOnlyList<ThreeOp> AllOps
	{
		get
		{
			return AppliedBatches
				.SelectMany(x => x)
				.ToList();
		}
	}

	/// <summary>Builds a tagged math value the way the applier's encoder sends one back.</summary>
	/// <param name="tag">The <c>$t</c> tag, e.g. <c>Vector3</c>.</param>
	/// <param name="components">The value's raw components.</param>
	/// <returns>The tagged value as a <see cref="JsonElement"/>.</returns>
	public static JsonElement TaggedValue(string tag, params float[] components)
	{
		return JsonSerializer.SerializeToElement(new Dictionary<string, object>
		{
			["$t"] = tag,
			["v"] = components
		});
	}
}
