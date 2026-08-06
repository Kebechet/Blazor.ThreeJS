// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates a torus knot, the particular shape of which is defined by a pair of coprime integers, p
/// and q If p and q are not coprime, the result will be a torus link. The JavaScript-side
/// <c>THREE.TorusKnotGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/TorusKnotGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/TorusKnotGeometry.js">Source</seealso>
public sealed class TorusKnotGeometry : BufferGeometry
{
	private readonly float _radius;
	private readonly float _tube;
	private readonly int _tubularSegments;
	private readonly int _radialSegments;
	private readonly int _p;
	private readonly int _q;

	/// <summary>Create a new instance of <see cref="TorusKnotGeometry"/>.</summary>
	/// <param name="radius">Radius of the torus.</param>
	/// <param name="tube"></param>
	/// <param name="tubularSegments"></param>
	/// <param name="radialSegments"></param>
	/// <param name="p">
	/// This value determines, how many times the geometry winds around its axis of rotational symmetry.
	/// </param>
	/// <param name="q">
	/// This value determines, how many times the geometry winds around a circle in the interior of the
	/// torus.
	/// </param>
	public TorusKnotGeometry(
		float radius = 1f,
		float tube = 0.4f,
		int tubularSegments = 64,
		int radialSegments = 8,
		int p = 2,
		int q = 3)
	{
		_radius = radius;
		_tube = tube;
		_tubularSegments = tubularSegments;
		_radialSegments = radialSegments;
		_p = p;
		_q = q;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>TorusKnotGeometry</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal TorusKnotGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_radius = default!;
		_tube = default!;
		_tubularSegments = default!;
		_radialSegments = default!;
		_p = default!;
		_q = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.TorusKnotGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "TorusKnotGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.TorusKnotGeometry</c>: radius, tube,
	/// tubularSegments, radialSegments, p, q.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_radius, _tube, _tubularSegments, _radialSegments, _p, _q]; }
	}
}
