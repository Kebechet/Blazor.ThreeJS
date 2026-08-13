// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Create a smooth **3D** spline curve from a series of points using the
/// <see href="https://en.wikipedia.org/wiki/Centripetal_Catmull-Rom_spline">Catmull-Rom</see>
/// algorithm. The JavaScript-side <c>THREE.CatmullRomCurve3</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl_geometry_extrude_splines">WebGL / geometry / extrude / splines</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/curves/CatmullRomCurve3">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/curves/CatmullRomCurve3.js">Source</seealso>
public sealed class CatmullRomCurve3 : ThreeObject
{
	private Vector3[]? _points;
	private bool _closed;
	private CurveType? _curveType;
	private float _tension;
	private int _arcLengthDivisions = 200;
	private bool _isClosedWritten;
	private bool _isPointsWritten;
	private bool _isCurveTypeWritten;
	private bool _isTensionWritten;
	private bool _isArcLengthDivisionsWritten;

	/// <summary>This constructor creates a new <see cref="CatmullRomCurve3"/>.</summary>
	/// <param name="points">An array of <c>Vector3</c> points.</param>
	/// <param name="closed">Whether the curve is closed.</param>
	/// <param name="curveType">Type of the curve.</param>
	/// <param name="tension">Tension of the curve.</param>
	public CatmullRomCurve3(
		Vector3[]? points = null,
		bool closed = false,
		CurveType? curveType = null,
		float tension = 0.5f)
	{
		_points = points;
		_closed = closed;
		_curveType = curveType;
		_tension = tension;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CatmullRomCurve3</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CatmullRomCurve3(ThreeBatch batch, int handle)
		: base(handle)
	{
		_closed = default!;
		_tension = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CatmullRomCurve3</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CatmullRomCurve3"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.CatmullRomCurve3</c>: points, closed, curveType,
	/// tension. An argument the caller left unspecified travels as the wire's not-supplied sentinel, or
	/// is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_points),
				_closed,
				ThreeValue.OrUnspecified(_curveType),
				_tension
			]);
		}
	}

	/// <summary>
	/// The curve will loop back onto itself when this is true. Writing it records a <c>closed</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Closed
	{
		get { return _closed; }
		set
		{
			if (_closed == value)
			{
				return;
			}

			_closed = value;
			_isClosedWritten = true;
			RecordSet("closed", value);
		}
	}

	/// <summary>
	/// The array of <c>Vector3</c> points that define the curve. Writing it records a <c>points</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Vector3[]? Points
	{
		get { return _points; }
		set
		{
			if (_points == value)
			{
				return;
			}

			_points = value;
			_isPointsWritten = true;
			RecordSet("points", value);
		}
	}

	/// <summary>
	/// Possible values are <c>centripetal</c>, <c>chordal</c> and <c>catmullrom</c>. Writing it records
	/// a <c>curveType</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public CurveType? CurveType
	{
		get { return _curveType; }
		set
		{
			if (_curveType == value)
			{
				return;
			}

			_curveType = value;
			_isCurveTypeWritten = true;
			RecordSet("curveType", value);
		}
	}

	/// <summary>
	/// When <c>.curveType</c> is <c>catmullrom</c>, defines catmullrom's tension. Writing it records a
	/// <c>tension</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Tension
	{
		get { return _tension; }
		set
		{
			if (_tension == value)
			{
				return;
			}

			_tension = value;
			_isTensionWritten = true;
			RecordSet("tension", value);
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
	public Task<Vector3> GetPointAsync(float t, Vector3 optionalTarget)
	{
		return RecordRead<Vector3>("getPoint", t, optionalTarget);
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
	public Task<Vector3> GetPointAtAsync(float u, Vector3 optionalTarget)
	{
		return RecordRead<Vector3>("getPointAt", u, optionalTarget);
	}

	/// <summary>
	/// Returns a set of divisions <c>+1</c> points using <c>getPoint(t)</c>. Records a read op, sends
	/// it behind every write already pending, and completes with what <c>getPoints</c> returned.
	/// </summary>
	/// <param name="divisions">Number of pieces to divide the <c>Curve</c> into.</param>
	/// <returns>The value <c>getPoints</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector3[]> GetPointsAsync(int divisions = 5)
	{
		return RecordRead<Vector3[]>("getPoints", divisions);
	}

	/// <summary>
	/// Returns a set of divisions <c>+1</c> equi-spaced points using <c>getPointAt(u)</c>. Records a
	/// read op, sends it behind every write already pending, and completes with what
	/// <c>getSpacedPoints</c> returned.
	/// </summary>
	/// <param name="divisions">Number of pieces to divide the <c>Curve</c> into.</param>
	/// <returns>The value <c>getSpacedPoints</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector3[]> GetSpacedPointsAsync(int divisions = 5)
	{
		return RecordRead<Vector3[]>("getSpacedPoints", divisions);
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
	public Task<Vector3> GetTangentAsync(float t, Vector3 optionalTarget)
	{
		return RecordRead<Vector3>("getTangent", t, optionalTarget);
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
	public Task<Vector3> GetTangentAtAsync(float u, Vector3 optionalTarget)
	{
		return RecordRead<Vector3>("getTangentAt", u, optionalTarget);
	}

	/// <summary>
	/// Creates a clone of this instance. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<CatmullRomCurve3?> CloneAsync()
	{
		return RecordReadObject<CatmullRomCurve3>("clone", (adoptedBatch, adoptedHandle) => new CatmullRomCurve3(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.CatmullRomCurve3</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isClosedWritten)
		{
			batch.Set(Handle, "closed", ThreeValue.Encode(_closed));
		}

		if (_isPointsWritten)
		{
			batch.Set(Handle, "points", ThreeValue.Encode(_points));
		}

		if (_isCurveTypeWritten)
		{
			batch.Set(Handle, "curveType", ThreeValue.Encode(_curveType));
		}

		if (_isTensionWritten)
		{
			batch.Set(Handle, "tension", ThreeValue.Encode(_tension));
		}

		if (_isArcLengthDivisionsWritten)
		{
			batch.Set(Handle, "arcLengthDivisions", ThreeValue.Encode(_arcLengthDivisions));
		}
	}
}
