// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates extruded geometry from a path shape. The JavaScript-side <c>THREE.ExtrudeGeometry</c>.
/// </summary>
public sealed class ExtrudeGeometry : BufferGeometry
{
	private readonly Shape? _shapes;

	/// <summary>Create a new instance of <see cref="ExtrudeGeometry"/>.</summary>
	/// <param name="shapes">Shape or an array of shapes.</param>
	public ExtrudeGeometry(Shape? shapes = null)
	{
		_shapes = shapes;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ExtrudeGeometry</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ExtrudeGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ExtrudeGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ExtrudeGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ExtrudeGeometry</c>: shapes. An argument the caller
	/// left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_shapes)]); }
	}

	/// <summary>
	/// Attaches the objects <c>THREE.ExtrudeGeometry</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_shapes?.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
