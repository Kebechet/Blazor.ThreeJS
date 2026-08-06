// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This can be used as a helper object to view the edges of a <c>geometry</c>. The JavaScript-side
/// <c>THREE.EdgesGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl_helpers">helpers</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/EdgesGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/EdgesGeometry.js">Source</seealso>
public sealed class EdgesGeometry : BufferGeometry
{
	private readonly BufferGeometry? _geometry;
	private readonly int _thresholdAngle;

	/// <summary>Create a new instance of <see cref="EdgesGeometry"/>.</summary>
	/// <param name="geometry">Any geometry object.</param>
	/// <param name="thresholdAngle">
	/// An edge is only rendered if the angle (in degrees) between the face normals of the adjoining
	/// faces exceeds this value. Expects a <c>Integer</c>. Default <c>1</c> _degree_.
	/// </param>
	public EdgesGeometry(BufferGeometry? geometry = null, int thresholdAngle = 1)
	{
		_geometry = geometry;
		_thresholdAngle = thresholdAngle;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>EdgesGeometry</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal EdgesGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_thresholdAngle = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.EdgesGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "EdgesGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.EdgesGeometry</c>: geometry, thresholdAngle. An
	/// argument the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed
	/// when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_geometry), _thresholdAngle]); }
	}

	/// <summary>
	/// Attaches the objects <c>THREE.EdgesGeometry</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_geometry?.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
