// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates an one-sided polygonal geometry from one or more path shapes. The JavaScript-side
/// <c>THREE.ShapeGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/ShapeGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/ShapeGeometry.js">Source</seealso>
public sealed class ShapeGeometry : BufferGeometry
{
	private readonly Shape? _shapes;
	private readonly int _curveSegments;

	/// <summary>Create a new instance of <see cref="ShapeGeometry"/>.</summary>
	/// <param name="shapes">
	/// Array of shapes or a single <c>Shape</c>. Default <c>new Shape([new Vector2(0, 0.5), new
	/// Vector2(-0.5, -0.5), new Vector2(0.5, -0.5)])</c>, _a single triangle shape_.
	/// </param>
	/// <param name="curveSegments">Number of segments per shape.</param>
	public ShapeGeometry(Shape? shapes = null, int curveSegments = 12)
	{
		_shapes = shapes;
		_curveSegments = curveSegments;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ShapeGeometry</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ShapeGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_curveSegments = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ShapeGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ShapeGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ShapeGeometry</c>: shapes, curveSegments. An
	/// argument the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed
	/// when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_shapes), _curveSegments]); }
	}

	/// <summary>
	/// Attaches the objects <c>THREE.ShapeGeometry</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_shapes?.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
