// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A class for generating cylinder geometries. The JavaScript-side <c>THREE.CylinderGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/CylinderGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/CylinderGeometry.js">Source</seealso>
public class CylinderGeometry : BufferGeometry
{
	private readonly float _radiusTop;
	private readonly float _radiusBottom;
	private readonly float _height;
	private readonly int _radialSegments;
	private readonly int _heightSegments;
	private readonly bool _openEnded;
	private readonly float _thetaStart;
	private readonly float? _thetaLength;

	/// <summary>Create a new instance of <see cref="CylinderGeometry"/>.</summary>
	/// <param name="radiusTop">Radius of the cylinder at the top.</param>
	/// <param name="radiusBottom">Radius of the cylinder at the bottom.</param>
	/// <param name="height">Height of the cylinder.</param>
	/// <param name="radialSegments">Number of segmented faces around the circumference of the cylinder.</param>
	/// <param name="heightSegments">Number of rows of faces along the height of the cylinder.</param>
	/// <param name="openEnded">
	/// A Boolean indicating whether the ends of the cylinder are open or capped. Default <c>false</c>,
	/// _meaning capped_.
	/// </param>
	/// <param name="thetaStart">
	/// Start angle for first segment. Default <c>0</c>, _(three o'clock position)_.
	/// </param>
	/// <param name="thetaLength">
	/// The central angle, often called theta, of the circular sector. Default <c>Math.PI * 2</c>,
	/// _which makes for a complete cylinder.
	/// </param>
	public CylinderGeometry(
		float radiusTop = 1f,
		float radiusBottom = 1f,
		float height = 1f,
		int radialSegments = 32,
		int heightSegments = 1,
		bool openEnded = false,
		float thetaStart = 0f,
		float? thetaLength = null)
	{
		_radiusTop = radiusTop;
		_radiusBottom = radiusBottom;
		_height = height;
		_radialSegments = radialSegments;
		_heightSegments = heightSegments;
		_openEnded = openEnded;
		_thetaStart = thetaStart;
		_thetaLength = thetaLength;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CylinderGeometry</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CylinderGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_radiusTop = default!;
		_radiusBottom = default!;
		_height = default!;
		_radialSegments = default!;
		_heightSegments = default!;
		_openEnded = default!;
		_thetaStart = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CylinderGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CylinderGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.CylinderGeometry</c>: radiusTop, radiusBottom,
	/// height, radialSegments, heightSegments, openEnded, thetaStart, thetaLength. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_radiusTop,
				_radiusBottom,
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
