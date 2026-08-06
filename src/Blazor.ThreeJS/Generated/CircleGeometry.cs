// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// <see cref="CircleGeometry"/> is a simple shape of Euclidean geometry. The JavaScript-side
/// <c>THREE.CircleGeometry</c>.
/// </summary>
/// <remarks>
/// It is constructed from a number of triangular segments that are oriented around a central point
/// and extend as far out as a given radius It is built counter-clockwise from a start angle and a
/// given central angle It can also be used to create regular polygons, where the number of segments
/// determines the number of sides.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/CircleGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/CircleGeometry.js">Source</seealso>
public sealed class CircleGeometry : BufferGeometry
{
	private readonly float _radius;
	private readonly int _segments;
	private readonly float _thetaStart;
	private readonly float? _thetaLength;

	/// <summary>Create a new instance of <see cref="CircleGeometry"/>.</summary>
	/// <param name="radius">Radius of the circle.</param>
	/// <param name="segments">Number of segments (triangles). Expects a <c>Integer</c>. Minimum <c>3</c>.</param>
	/// <param name="thetaStart">
	/// Start angle for first segment. Expects a <c>Float</c>. Default <c>0</c>, _(three o'clock
	/// position)_.
	/// </param>
	/// <param name="thetaLength">
	/// The central angle, often called theta, of the circular sector. Expects a <c>Float</c>. Default
	/// <c>Math.PI * 2</c>, _which makes for a complete circle_.
	/// </param>
	public CircleGeometry(float radius = 1f, int segments = 32, float thetaStart = 0f, float? thetaLength = null)
	{
		_radius = radius;
		_segments = segments;
		_thetaStart = thetaStart;
		_thetaLength = thetaLength;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CircleGeometry</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CircleGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_radius = default!;
		_segments = default!;
		_thetaStart = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CircleGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CircleGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.CircleGeometry</c>: radius, segments, thetaStart,
	/// thetaLength. An argument the caller left unspecified travels as the wire's not-supplied
	/// sentinel, or is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_radius,
				_segments,
				_thetaStart,
				ThreeValue.OrUnspecified(_thetaLength)
			]);
		}
	}
}
