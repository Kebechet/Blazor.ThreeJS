// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A class for generating an octahedron geometry. The JavaScript-side
/// <c>THREE.OctahedronGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/OctahedronGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/OctahedronGeometry.js">Source</seealso>
public sealed class OctahedronGeometry : PolyhedronGeometry
{
	private readonly float _radius;
	private readonly int _detail;

	/// <summary>Create a new instance of <see cref="OctahedronGeometry"/>.</summary>
	/// <param name="radius">Radius of the octahedron.</param>
	/// <param name="detail">
	/// Setting this to a value greater than zero add vertices making it no longer an octahedron.
	/// </param>
	public OctahedronGeometry(float radius = 1f, int detail = 0)
		: base(radius: radius, detail: detail)
	{
		_radius = radius;
		_detail = detail;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>OctahedronGeometry</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal OctahedronGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_radius = default!;
		_detail = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.OctahedronGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "OctahedronGeometry"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.OctahedronGeometry</c>: radius, detail.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_radius, _detail]; }
	}
}
