// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>Creates a tube that extrudes along a 3d curve. The JavaScript-side <c>THREE.TubeGeometry</c>.</summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/TubeGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/TubeGeometry.js">Source</seealso>
public sealed class TubeGeometry : BufferGeometry
{
	/// <summary>Create a new instance of <see cref="TubeGeometry"/>.</summary>
	public TubeGeometry()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.TubeGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "TubeGeometry"; }
	}
}
