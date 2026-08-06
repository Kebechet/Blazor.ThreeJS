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

	/// <summary>
	/// Adopts an existing JavaScript-side <c>DodecahedronGeometry</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal DodecahedronGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_radius = default!;
		_detail = default!;

		Batch = batch;
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
