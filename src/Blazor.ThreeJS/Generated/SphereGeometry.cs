// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A class for generating sphere geometries. The JavaScript-side <c>THREE.SphereGeometry</c>.</summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/SphereGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/SphereGeometry.js">Source</seealso>
public sealed class SphereGeometry : BufferGeometry
{
	private readonly float _radius;
	private readonly int _widthSegments;
	private readonly int _heightSegments;
	private readonly float _phiStart;
	private readonly float? _phiLength;
	private readonly float _thetaStart;
	private readonly float? _thetaLength;

	/// <summary>Create a new instance of <see cref="SphereGeometry"/>.</summary>
	/// <param name="radius">Sphere radius.</param>
	/// <param name="widthSegments">Number of horizontal segments. Minimum value is 3, and the.</param>
	/// <param name="heightSegments">Number of vertical segments. Minimum value is 2, and the.</param>
	/// <param name="phiStart">Specify horizontal starting angle.</param>
	/// <param name="phiLength">Specify horizontal sweep angle size.</param>
	/// <param name="thetaStart">Specify vertical starting angle.</param>
	/// <param name="thetaLength">Specify vertical sweep angle size.</param>
	public SphereGeometry(
		float radius = 1f,
		int widthSegments = 32,
		int heightSegments = 16,
		float phiStart = 0f,
		float? phiLength = null,
		float thetaStart = 0f,
		float? thetaLength = null)
	{
		_radius = radius;
		_widthSegments = widthSegments;
		_heightSegments = heightSegments;
		_phiStart = phiStart;
		_phiLength = phiLength;
		_thetaStart = thetaStart;
		_thetaLength = thetaLength;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.SphereGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "SphereGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.SphereGeometry</c>: radius, widthSegments,
	/// heightSegments, phiStart, phiLength, thetaStart, thetaLength. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_radius,
				_widthSegments,
				_heightSegments,
				_phiStart,
				ThreeValue.OrUnspecified(_phiLength),
				_thetaStart,
				ThreeValue.OrUnspecified(_thetaLength)
			]);
		}
	}
}
