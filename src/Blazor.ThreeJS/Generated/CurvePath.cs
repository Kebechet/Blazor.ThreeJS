// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Curved Path - a curve path is simply a array of connected curves, but retains the api of a
/// curve. The JavaScript-side <c>THREE.CurvePath</c>.
/// </summary>
/// <remarks>
/// A <see cref="CurvePath"/> is simply an array of connected curves, but retains the api of a
/// curve.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/core/CurvePath">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/core/CurvePath.js">Source</seealso>
public class CurvePath : ThreeObject
{
	private ThreeObject?[] _curves = [];
	private bool _autoClose = false;
	private int _arcLengthDivisions = 200;
	private bool _isCurvesWritten;
	private bool _isAutoCloseWritten;
	private bool _isArcLengthDivisionsWritten;

	/// <summary>The constructor take no parameters.</summary>
	public CurvePath()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CurvePath</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CurvePath(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CurvePath</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CurvePath"; }
	}

	/// <summary>
	/// The array of <c>Curves</c>. Writing it records a <c>curves</c> property write once this object
	/// is attached; writing the value already held records nothing.
	/// </summary>
	public ThreeObject?[] Curves
	{
		get { return _curves; }
		set
		{
			if (_curves == value)
			{
				return;
			}

			_curves = value;
			_isCurvesWritten = true;
			AttachEach(Batch, value);

			RecordSet("curves", value);
		}
	}

	/// <summary>
	/// Whether or not to automatically close the path. Writing it records a <c>autoClose</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool AutoClose
	{
		get { return _autoClose; }
		set
		{
			if (_autoClose == value)
			{
				return;
			}

			_autoClose = value;
			_isAutoCloseWritten = true;
			RecordSet("autoClose", value);
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

	/// <summary>Add a curve to the <c>.curves</c> array.</summary>
	/// <param name="curve">Value forwarded to the <c>curve</c> argument.</param>
	public void Add(ThreeObject curve)
	{
		RecordCall("add", curve);
	}

	/// <summary>Records a call to <c>fromJSON</c> on the JavaScript-side object.</summary>
	/// <param name="json">Value forwarded to the <c>json</c> argument.</param>
	public void FromJSON(CurvePathJSON json)
	{
		RecordCall("fromJSON", json);
	}

	/// <summary>Update the cumulative segment distance cache.</summary>
	public void UpdateArcLengths()
	{
		RecordCall("updateArcLengths");
	}

	/// <summary>Copies another <c>Curve</c> object to this instance.</summary>
	/// <param name="source">Value forwarded to the <c>source</c> argument.</param>
	public void Copy(ThreeObject source)
	{
		RecordCall("copy", source);
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
	/// Adds a <see cref="LineCurve">lineCurve</see> to close the path. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>closePath</c> returned.
	/// </summary>
	/// <returns>The value <c>closePath</c> returned, once the JavaScript side has answered.</returns>
	public Task<CurvePath?> ClosePathAsync()
	{
		return RecordReadObject<CurvePath>("closePath", (adoptedBatch, adoptedHandle) => new CurvePath(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Get list of cumulative curve lengths of the curves in the <c>.curves</c> array. Records a read
	/// op, sends it behind every write already pending, and completes with what <c>getCurveLengths</c>
	/// returned.
	/// </summary>
	/// <returns>The value <c>getCurveLengths</c> returned, once the JavaScript side has answered.</returns>
	public Task<float[]> GetCurveLengthsAsync()
	{
		return RecordRead<float[]>("getCurveLengths");
	}

	/// <summary>
	/// Reads <c>toJSON</c> back from the JavaScript-side object. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>toJSON</c> returned.
	/// </summary>
	/// <returns>The value <c>toJSON</c> returned, once the JavaScript side has answered.</returns>
	public Task<CurvePathJSON> ToJSONAsync()
	{
		return RecordRead<CurvePathJSON>("toJSON");
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
	public Task<CurvePath?> CloneAsync()
	{
		return RecordReadObject<CurvePath>("clone", (adoptedBatch, adoptedHandle) => new CurvePath(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.CurvePath</c>, then replays every property written before this
	/// object was attached. A replayed value that is itself a mirrored object is attached first, so its
	/// create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isCurvesWritten)
		{
			AttachEach(batch, _curves);
			batch.Set(Handle, "curves", ThreeValue.Encode(_curves));
		}

		if (_isAutoCloseWritten)
		{
			batch.Set(Handle, "autoClose", ThreeValue.Encode(_autoClose));
		}

		if (_isArcLengthDivisionsWritten)
		{
			batch.Set(Handle, "arcLengthDivisions", ThreeValue.Encode(_arcLengthDivisions));
		}
	}
}
