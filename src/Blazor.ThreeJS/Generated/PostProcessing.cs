// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.PostProcessing</c>.</summary>
public sealed class PostProcessing : RenderPipeline
{
	private readonly Renderer _renderer;

	/// <summary>Constructs a new post processing management module.</summary>
	/// <param name="renderer">A reference to the renderer.</param>
	public PostProcessing(Renderer renderer)
		: base(renderer: renderer)
	{
		_renderer = renderer;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>PostProcessing</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal PostProcessing(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_renderer = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PostProcessing</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PostProcessing"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.PostProcessing</c>: renderer.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_renderer]; }
	}

	/// <summary>
	/// Attaches the objects <c>THREE.PostProcessing</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_renderer.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
