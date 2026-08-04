// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// An implementation of the <see cref="Earcut"/> polygon triangulation algorithm. The
/// JavaScript-side <c>THREE.Earcut</c>.
/// </summary>
/// <remarks>The code is a port of <see href="https://github.com/mapbox/earcut">mapbox/earcut</see>.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/Earcut">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/Earcut.js">Source</seealso>
public sealed class Earcut : ThreeObject
{
	/// <summary>Initializes a new <see cref="Earcut"/>.</summary>
	public Earcut()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Earcut</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Earcut"; }
	}
}
