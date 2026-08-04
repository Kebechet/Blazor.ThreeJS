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
	/// <summary>Create a new instance of <see cref="PolyhedronGeometry"/>.</summary>
	public PolyhedronGeometry()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PolyhedronGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PolyhedronGeometry"; }
	}
}
