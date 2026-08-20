// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A class for generating a two-dimensional ring geometry. The JavaScript-side
/// <c>THREE.RingGeometry</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/RingGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/RingGeometry.js">Source</seealso>
public sealed class RingGeometry : BufferGeometry
{
	private readonly float _innerRadius;
	private readonly float _outerRadius;
	private readonly int _thetaSegments;
	private readonly int _phiSegments;
	private readonly float _thetaStart;
	private readonly float? _thetaLength;

	/// <summary>Create a new instance of <see cref="RingGeometry"/>.</summary>
	/// <param name="innerRadius"></param>
	/// <param name="outerRadius"></param>
	/// <param name="thetaSegments">
	/// Number of segments. A higher number means the ring will be more round. Minimum is 3.
	/// </param>
	/// <param name="phiSegments">Number of segments per ring segment. Minimum is <c>1</c>.</param>
	/// <param name="thetaStart">Starting angle.</param>
	/// <param name="thetaLength">Central angle.</param>
	public RingGeometry(
		float innerRadius = 0.5f,
		float outerRadius = 1f,
		int thetaSegments = 32,
		int phiSegments = 1,
		float thetaStart = 0f,
		float? thetaLength = null)
	{
		_innerRadius = innerRadius;
		_outerRadius = outerRadius;
		_thetaSegments = thetaSegments;
		_phiSegments = phiSegments;
		_thetaStart = thetaStart;
		_thetaLength = thetaLength;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>RingGeometry</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal RingGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_innerRadius = default!;
		_outerRadius = default!;
		_thetaSegments = default!;
		_phiSegments = default!;
		_thetaStart = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.RingGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "RingGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.RingGeometry</c>: innerRadius, outerRadius,
	/// thetaSegments, phiSegments, thetaStart, thetaLength. An argument the caller left unspecified
	/// travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied follows it, so
	/// three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_innerRadius,
				_outerRadius,
				_thetaSegments,
				_phiSegments,
				_thetaStart,
				ThreeValue.OrUnspecified(_thetaLength)
			]);
		}
	}

	/// <summary>
	/// An object with a property for each of the constructor parameters. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>parameters</c> held.
	/// </summary>
	/// <returns>The value <c>parameters</c> held, once the JavaScript side has answered.</returns>
	public Task<RingGeometryParameters> ParametersAsync()
	{
		return GetAsync<RingGeometryParameters>("parameters");
	}
}
