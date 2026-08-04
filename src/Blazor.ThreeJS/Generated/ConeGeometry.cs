// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A class for generating cone geometries. The JavaScript-side <c>THREE.ConeGeometry</c>.</summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/ConeGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/ConeGeometry.js">Source</seealso>
public sealed class ConeGeometry : CylinderGeometry
{
	private readonly float _radius;
	private readonly float _height;
	private readonly int _radialSegments;
	private readonly int _heightSegments;
	private readonly bool _openEnded;
	private readonly float _thetaStart;
	private readonly float? _thetaLength;

	/// <summary>Create a new instance of <see cref="ConeGeometry"/>.</summary>
	/// <param name="radius">Radius of the cone base.</param>
	/// <param name="height">Height of the cone.</param>
	/// <param name="radialSegments">Number of segmented faces around the circumference of the cone.</param>
	/// <param name="heightSegments">Number of rows of faces along the height of the cone.</param>
	/// <param name="openEnded">
	/// A Boolean indicating whether the base of the cone is open or capped. Default <c>false</c>,
	/// _meaning capped_.
	/// </param>
	/// <param name="thetaStart">
	/// Start angle for first segment. Expects a <c>Float</c>. Default <c>0</c>, _(three o'clock
	/// position)_.
	/// </param>
	/// <param name="thetaLength">
	/// The central angle, often called theta, of the circular sector. Expects a <c>Float</c>. Default
	/// <c>Math.PI * 2</c>, _which makes for a complete cone_.
	/// </param>
	public ConeGeometry(
		float radius = 1f,
		float height = 1f,
		int radialSegments = 32,
		int heightSegments = 1,
		bool openEnded = false,
		float thetaStart = 0f,
		float? thetaLength = null)
	{
		_radius = radius;
		_height = height;
		_radialSegments = radialSegments;
		_heightSegments = heightSegments;
		_openEnded = openEnded;
		_thetaStart = thetaStart;
		_thetaLength = thetaLength;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ConeGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ConeGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ConeGeometry</c>: radius, height, radialSegments,
	/// heightSegments, openEnded, thetaStart, thetaLength. An argument the caller left unspecified
	/// travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied follows it, so
	/// three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_radius,
				_height,
				_radialSegments,
				_heightSegments,
				_openEnded,
				_thetaStart,
				ThreeValue.OrUnspecified(_thetaLength)
			]);
		}
	}
}
