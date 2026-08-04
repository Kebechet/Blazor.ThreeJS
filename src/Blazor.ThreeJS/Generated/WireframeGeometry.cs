// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This can be used as a helper object to view a <see cref="BufferGeometry">geometry</see> as a
/// wireframe. The JavaScript-side <c>THREE.WireframeGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl_helpers">helpers</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/WireframeGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/WireframeGeometry.js">Source</seealso>
public sealed class WireframeGeometry : BufferGeometry
{
	private readonly BufferGeometry? _geometry;

	/// <summary>Create a new instance of <see cref="WireframeGeometry"/>.</summary>
	/// <param name="geometry">Any geometry object.</param>
	public WireframeGeometry(BufferGeometry? geometry = null)
	{
		_geometry = geometry;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WireframeGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WireframeGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.WireframeGeometry</c>: geometry. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_geometry)]); }
	}

	/// <summary>
	/// Attaches the objects <c>THREE.WireframeGeometry</c> is constructed from, so their create ops
	/// reach the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_geometry?.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
