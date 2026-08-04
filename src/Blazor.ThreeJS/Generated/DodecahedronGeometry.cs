// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A class for generating a dodecahedron geometries. The JavaScript-side
/// <c>THREE.DodecahedronGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/DodecahedronGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/DodecahedronGeometry.js">Source</seealso>
public sealed class DodecahedronGeometry : PolyhedronGeometry
{
	private readonly float _radius;
	private readonly int _detail;

	/// <summary>Create a new instance of <see cref="DodecahedronGeometry"/>.</summary>
	/// <param name="radius">Radius of the dodecahedron.</param>
	/// <param name="detail">
	/// Setting this to a value greater than 0 adds vertices making it no longer a dodecahedron.
	/// </param>
	public DodecahedronGeometry(float radius = 1f, int detail = 0)
	{
		_radius = radius;
		_detail = detail;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.DodecahedronGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "DodecahedronGeometry"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.DodecahedronGeometry</c>: radius, detail.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_radius, _detail]; }
	}
}
