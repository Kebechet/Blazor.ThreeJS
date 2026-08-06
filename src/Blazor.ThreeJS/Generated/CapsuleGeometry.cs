// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// <see cref="CapsuleGeometry"/> is a geometry class for a capsule with given radii and height. The
/// JavaScript-side <c>THREE.CapsuleGeometry</c>.
/// </summary>
/// <remarks>It is constructed using a lathe.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/CapsuleGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/CapsuleGeometry.js">Source</seealso>
public sealed class CapsuleGeometry : BufferGeometry
{
	private readonly float _radius;
	private readonly float _height;
	private readonly int _capSegments;
	private readonly int _radialSegments;
	private readonly int? _heightSegments;

	/// <summary>Create a new instance of <see cref="CapsuleGeometry"/>.</summary>
	/// <param name="radius">Radius of the capsule.</param>
	/// <param name="height">Height of the middle section.</param>
	/// <param name="capSegments">Number of curve segments used to build the caps.</param>
	/// <param name="radialSegments">Number of segmented faces around the circumference of the capsule.</param>
	/// <param name="heightSegments">
	/// Number of rows of faces along the height of the capsule. Optional; defaults to <c>1</c>.
	/// </param>
	public CapsuleGeometry(
		float radius = 1f,
		float height = 1f,
		int capSegments = 4,
		int radialSegments = 8,
		int? heightSegments = null)
	{
		_radius = radius;
		_height = height;
		_capSegments = capSegments;
		_radialSegments = radialSegments;
		_heightSegments = heightSegments;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CapsuleGeometry</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CapsuleGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_radius = default!;
		_height = default!;
		_capSegments = default!;
		_radialSegments = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CapsuleGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CapsuleGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.CapsuleGeometry</c>: radius, height, capSegments,
	/// radialSegments, heightSegments. An argument the caller left unspecified travels as the wire's
	/// not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js applies its
	/// own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_radius,
				_height,
				_capSegments,
				_radialSegments,
				ThreeValue.OrUnspecified(_heightSegments)
			]);
		}
	}
}
