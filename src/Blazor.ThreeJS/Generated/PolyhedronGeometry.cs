// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A polyhedron is a solid in three dimensions with flat faces. The JavaScript-side
/// <c>THREE.PolyhedronGeometry</c>.
/// </summary>
/// <remarks>
/// This class will take an array of vertices, project them onto a sphere, and then divide them up
/// to the desired level of detail This class is used by <c>DodecahedronGeometry</c>,
/// <c>IcosahedronGeometry</c>, <c>OctahedronGeometry</c>, and <c>TetrahedronGeometry</c> to
/// generate their respective geometries.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/PolyhedronGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/PolyhedronGeometry.js">Source</seealso>
public class PolyhedronGeometry : BufferGeometry
{
	private readonly float[]? _vertices;
	private readonly float[]? _indices;
	private readonly float _radius;
	private readonly int _detail;

	/// <summary>Create a new instance of <see cref="PolyhedronGeometry"/>.</summary>
	/// <param name="vertices">Array of points of the form [1,1,1, -1,-1,-1, ... ].</param>
	/// <param name="indices">Array of indices that make up the faces of the form [0,1,2, 2,3,0, ... ].</param>
	/// <param name="radius">[page:The radius of the final shape.</param>
	/// <param name="detail">
	/// [page:How many levels to subdivide the geometry. The more detail, the smoother the shape.
	/// </param>
	public PolyhedronGeometry(float[]? vertices = null, float[]? indices = null, float radius = 1f, int detail = 0)
	{
		_vertices = vertices;
		_indices = indices;
		_radius = radius;
		_detail = detail;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>PolyhedronGeometry</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal PolyhedronGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_radius = default!;
		_detail = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PolyhedronGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PolyhedronGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.PolyhedronGeometry</c>: vertices, indices, radius,
	/// detail. An argument the caller left unspecified travels as the wire's not-supplied sentinel, or
	/// is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_vertices),
				ThreeValue.OrUnspecified(_indices),
				_radius,
				_detail
			]);
		}
	}
}
