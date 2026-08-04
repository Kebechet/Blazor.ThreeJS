// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A class for generating torus geometries. The JavaScript-side <c>THREE.TorusGeometry</c>.</summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/TorusGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/TorusGeometry.js">Source</seealso>
public sealed class TorusGeometry : BufferGeometry
{
	private readonly float _radius;
	private readonly float _tube;
	private readonly int _radialSegments;
	private readonly int _tubularSegments;
	private readonly float? _arc;
	private readonly float _thetaStart;
	private readonly float? _thetaLength;

	/// <summary>Initializes a new <see cref="TorusGeometry"/>.</summary>
	/// <param name="radius">Radius of the torus, from the center of the torus to the center of the tube.</param>
	/// <param name="tube">Radius of the tube. Must be smaller than <c>radius</c>. Default is <c>0.4</c>.</param>
	/// <param name="radialSegments">Default is <c>12</c>.</param>
	/// <param name="tubularSegments">Default is <c>48</c>.</param>
	/// <param name="arc">Central angle. Default is Math.PI * 2.</param>
	/// <param name="thetaStart">Start of the tubular sweep in radians.</param>
	/// <param name="thetaLength">times 2] - Length of the tubular sweep in radians.</param>
	public TorusGeometry(
		float radius = 1f,
		float tube = 0.4f,
		int radialSegments = 12,
		int tubularSegments = 48,
		float? arc = null,
		float thetaStart = 0f,
		float? thetaLength = null)
	{
		_radius = radius;
		_tube = tube;
		_radialSegments = radialSegments;
		_tubularSegments = tubularSegments;
		_arc = arc;
		_thetaStart = thetaStart;
		_thetaLength = thetaLength;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.TorusGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "TorusGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.TorusGeometry</c>: radius, tube, radialSegments,
	/// tubularSegments, arc, thetaStart, thetaLength. An argument the caller left unspecified travels
	/// as the wire's not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js
	/// applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_radius,
				_tube,
				_radialSegments,
				_tubularSegments,
				ThreeValue.OrUnspecified(_arc),
				_thetaStart,
				ThreeValue.OrUnspecified(_thetaLength)
			]);
		}
	}
}
