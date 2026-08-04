// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates meshes with axial symmetry like vases. The JavaScript-side <c>THREE.LatheGeometry</c>.
/// </summary>
/// <remarks>The lathe rotates around the Y axis.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/LatheGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/LatheGeometry.js">Source</seealso>
public sealed class LatheGeometry : BufferGeometry
{
	/// <summary>This creates a <see cref="LatheGeometry"/> based on the parameters.</summary>
	public LatheGeometry()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LatheGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LatheGeometry"; }
	}
}
