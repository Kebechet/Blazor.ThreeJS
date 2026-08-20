// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Create a smooth **2D**
/// <see href="http://en.wikipedia.org/wiki/B%C3%A9zier_curve#mediaviewer/File:Bezier_curve.svg">cubic
/// bezier curve</see>, defined by a start point, endpoint and two control points. The
/// JavaScript-side <c>THREE.CubicBezierCurve</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/curves/CubicBezierCurve">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/curves/CubicBezierCurve.js">Source</seealso>
public sealed class CubicBezierCurve : ThreeObject
{
	private readonly Vector2? _v0;
	private readonly Vector2? _v1;
	private readonly Vector2? _v2;
	private readonly Vector2? _v3;
	private int _arcLengthDivisions = 200;
	private bool _isArcLengthDivisionsWritten;

	/// <summary>This constructor creates a new <see cref="CubicBezierCurve"/>.</summary>
	/// <param name="v0">The starting point. Default is <c>new THREE.Vector2()</c>.</param>
	/// <param name="v1">The first control point. Default is <c>new THREE.Vector2()</c>.</param>
	/// <param name="v2">The second control point. Default is <c>new THREE.Vector2()</c>.</param>
	/// <param name="v3">The ending point. Default is <c>new THREE.Vector2()</c>.</param>
	public CubicBezierCurve(Vector2? v0 = null, Vector2? v1 = null, Vector2? v2 = null, Vector2? v3 = null)
	{
		_v0 = v0;
		_v1 = v1;
		_v2 = v2;
		_v3 = v3;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CubicBezierCurve</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CubicBezierCurve(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CubicBezierCurve</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CubicBezierCurve"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.CubicBezierCurve</c>: v0, v1, v2, v3. An argument
	/// the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when
	/// nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_v0),
				ThreeValue.OrUnspecified(_v1),
				ThreeValue.OrUnspecified(_v2),
				ThreeValue.OrUnspecified(_v3)
			]);
		}
	}

	/// <summary>
	/// This value determines the amount of divisions when calculating the cumulative segment lengths of
	/// a <c>Curve</c> via <c>.getLengths</c>. To ensure precision when using methods like
	/// <c>.getSpacedPoints</c>, it is recommended to increase <c>.arcLengthDivisions</c> if the
	/// <c>Curve</c> is very large. Writing it records a <c>arcLengthDivisions</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public int ArcLengthDivisions
	{
		get { return _arcLengthDivisions; }
		set
		{
			if (_arcLengthDivisions == value)
			{
				return;
			}

			_arcLengthDivisions = value;
			_isArcLengthDivisionsWritten = true;
			RecordSet("arcLengthDivisions", value);
		}
	}

	/// <summary>Update the cumulative segment distance cache.</summary>
	public void UpdateArcLengths()
	{
		RecordCall("updateArcLengths");
	}

	/// <summary>Copies the data from the given JSON object to this instance.</summary>
	/// <param name="json">Value forwarded to the <c>json</c> argument.</param>
	public void FromJSON(CurveJSON json)
	{
		RecordCall("fromJSON", json);
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="CubicBezierCurve"/>. Read-only
	/// in three.js, so it is read on demand rather than mirrored: records a get op, sends it behind
	/// every write already pending, and completes with the value <c>isCubicBezierCurve</c> held.
	/// </summary>
	/// <returns>The value <c>isCubicBezierCurve</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsCubicBezierCurveAsync()
	{
		return GetAsync<bool>("isCubicBezierCurve");
	}

	/// <summary>
	/// A Read-only _string_ to check if <c>this</c> object type. Read-only in three.js, so it is read
	/// on demand rather than mirrored: records a get op, sends it behind every write already pending,
	/// and completes with the value <c>type</c> held.
	/// </summary>
	/// <returns>The value <c>type</c> held, once the JavaScript side has answered.</returns>
	public Task<string> TypeAsync()
	{
		return GetAsync<string>("type");
	}

	/// <summary>
	/// Returns a vector for a given position on the curve. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>getPoint</c> returned.
	/// </summary>
	/// <param name="t">A position on the curve. Must be in the range <c>[ 0, 1 ]</c>.</param>
	/// <param name="optionalTarget">
	/// If specified, the result will be copied into this Vector, otherwise a new Vector will be
	/// created.
	/// </param>
	/// <returns>The value <c>getPoint</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector2> GetPointAsync(float t, Vector2 optionalTarget)
	{
		return RecordRead<Vector2>("getPoint", t, optionalTarget);
	}

	/// <summary>
	/// Returns a vector for a given position on the <c>Curve</c> according to the arc length. Records a
	/// read op, sends it behind every write already pending, and completes with what <c>getPointAt</c>
	/// returned.
	/// </summary>
	/// <param name="u">
	/// A position on the <c>Curve</c> according to the arc length. Must be in the range <c>[ 0, 1
	/// ]</c>.
	/// </param>
	/// <param name="optionalTarget">
	/// If specified, the result will be copied into this Vector, otherwise a new Vector will be
	/// created.
	/// </param>
	/// <returns>The value <c>getPointAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector2> GetPointAtAsync(float u, Vector2 optionalTarget)
	{
		return RecordRead<Vector2>("getPointAt", u, optionalTarget);
	}

	/// <summary>
	/// Returns a set of divisions <c>+1</c> points using <c>getPoint(t)</c>. Records a read op, sends
	/// it behind every write already pending, and completes with what <c>getPoints</c> returned.
	/// </summary>
	/// <param name="divisions">Number of pieces to divide the <c>Curve</c> into.</param>
	/// <returns>The value <c>getPoints</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector2[]> GetPointsAsync(int divisions = 5)
	{
		return RecordRead<Vector2[]>("getPoints", divisions);
	}

	/// <summary>
	/// Returns a set of divisions <c>+1</c> equi-spaced points using <c>getPointAt(u)</c>. Records a
	/// read op, sends it behind every write already pending, and completes with what
	/// <c>getSpacedPoints</c> returned.
	/// </summary>
	/// <param name="divisions">Number of pieces to divide the <c>Curve</c> into.</param>
	/// <returns>The value <c>getSpacedPoints</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector2[]> GetSpacedPointsAsync(int divisions = 5)
	{
		return RecordRead<Vector2[]>("getSpacedPoints", divisions);
	}

	/// <summary>
	/// Get total <c>Curve</c> arc length. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getLength</c> returned.
	/// </summary>
	/// <returns>The value <c>getLength</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetLengthAsync()
	{
		return RecordRead<float>("getLength");
	}

	/// <summary>
	/// Get list of cumulative segment lengths. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getLengths</c> returned.
	/// </summary>
	/// <param name="divisions"></param>
	/// <returns>The value <c>getLengths</c> returned, once the JavaScript side has answered.</returns>
	public Task<float[]> GetLengthsAsync(int divisions)
	{
		return RecordRead<float[]>("getLengths", divisions);
	}

	/// <summary>
	/// Given u in the range <c>[ 0, 1 ]</c>,. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getUtoTmapping</c> returned.
	/// </summary>
	/// <param name="u"></param>
	/// <param name="distance"></param>
	/// <returns>The value <c>getUtoTmapping</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetUtoTmappingAsync(float u, float distance)
	{
		return RecordRead<float>("getUtoTmapping", u, distance);
	}

	/// <summary>
	/// Returns a unit vector tangent at t. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getTangent</c> returned.
	/// </summary>
	/// <param name="t">A position on the curve. Must be in the range <c>[ 0, 1 ]</c>.</param>
	/// <param name="optionalTarget">
	/// If specified, the result will be copied into this Vector, otherwise a new Vector will be
	/// created.
	/// </param>
	/// <returns>The value <c>getTangent</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector2> GetTangentAsync(float t, Vector2 optionalTarget)
	{
		return RecordRead<Vector2>("getTangent", t, optionalTarget);
	}

	/// <summary>
	/// Returns tangent at a point which is equidistant to the ends of the <c>Curve</c> from the point
	/// given in <c>.getTangent</c>. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getTangentAt</c> returned.
	/// </summary>
	/// <param name="u">
	/// A position on the <c>Curve</c> according to the arc length. Must be in the range <c>[ 0, 1
	/// ]</c>.
	/// </param>
	/// <param name="optionalTarget">
	/// If specified, the result will be copied into this Vector, otherwise a new Vector will be
	/// created.
	/// </param>
	/// <returns>The value <c>getTangentAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector2> GetTangentAtAsync(float u, Vector2 optionalTarget)
	{
		return RecordRead<Vector2>("getTangentAt", u, optionalTarget);
	}

	/// <summary>
	/// Generates the Frenet Frames. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>computeFrenetFrames</c> returned.
	/// </summary>
	/// <param name="segments"></param>
	/// <param name="closed">Value forwarded to the <c>closed</c> argument.</param>
	/// <returns>The value <c>computeFrenetFrames</c> returned, once the JavaScript side has answered.</returns>
	public Task<FrenetFrames> ComputeFrenetFramesAsync(int segments, bool closed)
	{
		return RecordRead<FrenetFrames>("computeFrenetFrames", segments, closed);
	}

	/// <summary>
	/// Creates a clone of this instance. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<CubicBezierCurve?> CloneAsync()
	{
		return RecordReadObject<CubicBezierCurve>("clone", (adoptedBatch, adoptedHandle) => new CubicBezierCurve(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns a JSON object representation of this instance. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>toJSON</c> returned.
	/// </summary>
	/// <returns>The value <c>toJSON</c> returned, once the JavaScript side has answered.</returns>
	public Task<CurveJSON> ToJSONAsync()
	{
		return RecordRead<CurveJSON>("toJSON");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.CubicBezierCurve</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isArcLengthDivisionsWritten)
		{
			batch.Set(Handle, "arcLengthDivisions", ThreeValue.Encode(_arcLengthDivisions));
		}
	}
}
