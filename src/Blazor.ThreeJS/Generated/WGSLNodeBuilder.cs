// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.WGSLNodeBuilder</c>.</summary>
public sealed class WGSLNodeBuilder : ThreeObject
{
	private readonly Object3D _object;
	private readonly Renderer _renderer;

	/// <summary>Initializes a new <see cref="WGSLNodeBuilder"/>.</summary>
	/// <param name="object">Value forwarded to the <c>object</c> constructor argument.</param>
	/// <param name="renderer">Value forwarded to the <c>renderer</c> constructor argument.</param>
	public WGSLNodeBuilder(Object3D @object, Renderer renderer)
	{
		_object = @object;
		_renderer = renderer;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>WGSLNodeBuilder</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal WGSLNodeBuilder(ThreeBatch batch, int handle)
		: base(handle)
	{
		_object = default!;
		_renderer = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WGSLNodeBuilder</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WGSLNodeBuilder"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.WGSLNodeBuilder</c>: object, renderer.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_object, _renderer]; }
	}

	/// <summary>
	/// Attaches the objects <c>THREE.WGSLNodeBuilder</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_object.AttachTo(batch);
		_renderer.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
