// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A class for generating an icosahedron geometry. The JavaScript-side
/// <c>THREE.IcosahedronGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/IcosahedronGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/IcosahedronGeometry.js">Source</seealso>
public sealed class IcosahedronGeometry : PolyhedronGeometry
{
	private readonly float _radius;
	private readonly int _detail;

	/// <summary>Create a new instance of <see cref="IcosahedronGeometry"/>.</summary>
	/// <param name="radius"></param>
	/// <param name="detail">
	/// Setting this to a value greater than 0 adds more vertices making it no longer an icosahedron.
	/// When detail is greater than 1, it's effectively a sphere.
	/// </param>
	public IcosahedronGeometry(float radius = 1f, int detail = 0)
	{
		_radius = radius;
		_detail = detail;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.IcosahedronGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "IcosahedronGeometry"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.IcosahedronGeometry</c>: radius, detail.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_radius, _detail]; }
	}
}
