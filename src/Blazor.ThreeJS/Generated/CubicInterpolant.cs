// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Fast and simple cubic spline interpolant. It was derived from a Hermitian construction setting
/// the first derivative at each sample position to the linear slope between neighboring positions
/// over their parameter interval. The JavaScript-side <c>THREE.CubicInterpolant</c>.
/// </summary>
public sealed class CubicInterpolant : ThreeObject
{
	private TypedArray _parameterPositions;
	private TypedArray _sampleValues;
	private readonly float _sampleSize;
	private TypedArray? _resultBuffer;
	private float _valueSize;
	private bool _isParameterPositionsWritten;
	private bool _isResultBufferWritten;
	private bool _isSampleValuesWritten;
	private bool _isValueSizeWritten;

	/// <summary>Initializes a new <see cref="CubicInterpolant"/>.</summary>
	/// <param name="parameterPositions">The parameter positions hold the interpolation factors.</param>
	/// <param name="sampleValues">The sample values.</param>
	/// <param name="sampleSize">The sample size.</param>
	/// <param name="resultBuffer">The result buffer.</param>
	public CubicInterpolant(
		TypedArray parameterPositions,
		TypedArray sampleValues,
		float sampleSize,
		TypedArray? resultBuffer = null)
	{
		_parameterPositions = parameterPositions;
		_sampleValues = sampleValues;
		_sampleSize = sampleSize;
		_resultBuffer = resultBuffer;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CubicInterpolant</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CubicInterpolant(ThreeBatch batch, int handle)
		: base(handle)
	{
		_parameterPositions = default!;
		_sampleValues = default!;
		_sampleSize = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CubicInterpolant</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CubicInterpolant"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.CubicInterpolant</c>: parameterPositions,
	/// sampleValues, sampleSize, resultBuffer. An argument the caller left unspecified travels as the
	/// wire's not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js
	/// applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_parameterPositions,
				_sampleValues,
				_sampleSize,
				ThreeValue.OrUnspecified(_resultBuffer)
			]);
		}
	}

	/// <summary>
	/// The parameter positions. Writing it records a <c>parameterPositions</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public TypedArray ParameterPositions
	{
		get { return _parameterPositions; }
		set
		{
			if (_parameterPositions == value)
			{
				return;
			}

			_parameterPositions = value;
			_isParameterPositionsWritten = true;
			RecordSet("parameterPositions", value);
		}
	}

	/// <summary>
	/// The result buffer. Writing it records a <c>resultBuffer</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public TypedArray? ResultBuffer
	{
		get { return _resultBuffer; }
		set
		{
			if (_resultBuffer == value)
			{
				return;
			}

			_resultBuffer = value;
			_isResultBufferWritten = true;
			RecordSet("resultBuffer", value);
		}
	}

	/// <summary>
	/// The sample values. Writing it records a <c>sampleValues</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public TypedArray SampleValues
	{
		get { return _sampleValues; }
		set
		{
			if (_sampleValues == value)
			{
				return;
			}

			_sampleValues = value;
			_isSampleValuesWritten = true;
			RecordSet("sampleValues", value);
		}
	}

	/// <summary>
	/// The value size. Writing it records a <c>valueSize</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float ValueSize
	{
		get { return _valueSize; }
		set
		{
			if (_valueSize == value)
			{
				return;
			}

			_valueSize = value;
			_isValueSizeWritten = true;
			RecordSet("valueSize", value);
		}
	}

	/// <summary>Records a call to <c>intervalChanged_</c> on the JavaScript-side object.</summary>
	/// <param name="i1">Value forwarded to the <c>i1</c> argument.</param>
	/// <param name="t0">Value forwarded to the <c>t0</c> argument.</param>
	/// <param name="t1">Value forwarded to the <c>t1</c> argument.</param>
	public void IntervalChanged_(float i1, float t0, float t1)
	{
		RecordCall("intervalChanged_", i1, t0, t1);
	}

	/// <summary>
	/// Reads <c>interpolate_</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>interpolate_</c> returned.
	/// </summary>
	/// <param name="i1">Value forwarded to the <c>i1</c> argument.</param>
	/// <param name="t0">Value forwarded to the <c>t0</c> argument.</param>
	/// <param name="t">Value forwarded to the <c>t</c> argument.</param>
	/// <param name="t1">Value forwarded to the <c>t1</c> argument.</param>
	/// <returns>The value <c>interpolate_</c> returned, once the JavaScript side has answered.</returns>
	public Task<TypedArray> Interpolate_Async(float i1, float t0, float t, float t1)
	{
		return RecordRead<TypedArray>("interpolate_", i1, t0, t, t1);
	}

	/// <summary>
	/// Evaluate the interpolant at position <c>t</c>. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>evaluate</c> returned.
	/// </summary>
	/// <param name="t">The interpolation factor.</param>
	/// <returns>The value <c>evaluate</c> returned, once the JavaScript side has answered.</returns>
	public Task<TypedArray> EvaluateAsync(float t)
	{
		return RecordRead<TypedArray>("evaluate", t);
	}

	/// <summary>
	/// Copies a sample value to the result buffer. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>copySampleValue_</c> returned.
	/// </summary>
	/// <param name="index">An index into the sample value buffer.</param>
	/// <returns>The value <c>copySampleValue_</c> returned, once the JavaScript side has answered.</returns>
	public Task<TypedArray> CopySampleValue_Async(int index)
	{
		return RecordRead<TypedArray>("copySampleValue_", index);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.CubicInterpolant</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isParameterPositionsWritten)
		{
			batch.Set(Handle, "parameterPositions", ThreeValue.Encode(_parameterPositions));
		}

		if (_isResultBufferWritten)
		{
			batch.Set(Handle, "resultBuffer", ThreeValue.Encode(_resultBuffer));
		}

		if (_isSampleValuesWritten)
		{
			batch.Set(Handle, "sampleValues", ThreeValue.Encode(_sampleValues));
		}

		if (_isValueSizeWritten)
		{
			batch.Set(Handle, "valueSize", ThreeValue.Encode(_valueSize));
		}
	}
}
