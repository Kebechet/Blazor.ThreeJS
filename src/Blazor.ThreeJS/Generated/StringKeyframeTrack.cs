// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A track for string keyframe values. The JavaScript-side <c>THREE.StringKeyframeTrack</c>.</summary>
public sealed class StringKeyframeTrack : ThreeObject
{
	private string _name;
	private float[] _times;
	private string[] _values;
	private string _valueTypeName = string.Empty;
	private InterpolationModes _defaultInterpolation = InterpolationModes.InterpolateLinear;
	private bool _isNameWritten;
	private bool _isTimesWritten;
	private bool _isValuesWritten;
	private bool _isValueTypeNameWritten;
	private bool _isDefaultInterpolationWritten;

	/// <summary>
	/// Constructs a new string keyframe track. This keyframe track type has no <c>interpolation</c>
	/// parameter because the interpolation is always discrete.
	/// </summary>
	/// <param name="name">The keyframe track's name.</param>
	/// <param name="times">A list of keyframe times.</param>
	/// <param name="values">A list of keyframe values.</param>
	public StringKeyframeTrack(string name, float[] times, string[] values)
	{
		_name = name;
		_times = times;
		_values = values;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>StringKeyframeTrack</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal StringKeyframeTrack(ThreeBatch batch, int handle)
		: base(handle)
	{
		_name = default!;
		_times = default!;
		_values = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.StringKeyframeTrack</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "StringKeyframeTrack"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.StringKeyframeTrack</c>: name, times, values.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_name, _times, _values]; }
	}

	/// <summary>
	/// The track's name can refer to morph targets or bones or possibly other values within an animated
	/// object. See <c>PropertyBinding#parseTrackName</c> for the forms of strings that can be parsed
	/// for property binding. Writing it records a <c>name</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public string Name
	{
		get { return _name; }
		set
		{
			if (_name == value)
			{
				return;
			}

			_name = value;
			_isNameWritten = true;
			RecordSet("name", value);
		}
	}

	/// <summary>
	/// The keyframe times. Writing it records a <c>times</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float[] Times
	{
		get { return _times; }
		set
		{
			if (_times == value)
			{
				return;
			}

			_times = value;
			_isTimesWritten = true;
			RecordSet("times", value);
		}
	}

	/// <summary>
	/// The keyframe values. Writing it records a <c>values</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public string[] Values
	{
		get { return _values; }
		set
		{
			if (_values == value)
			{
				return;
			}

			_values = value;
			_isValuesWritten = true;
			RecordSet("values", value);
		}
	}

	/// <summary>
	/// The value type name. Writing it records a <c>ValueTypeName</c> property write once this object
	/// is attached; writing the value already held records nothing.
	/// </summary>
	public string ValueTypeName
	{
		get { return _valueTypeName; }
		set
		{
			if (_valueTypeName == value)
			{
				return;
			}

			_valueTypeName = value;
			_isValueTypeNameWritten = true;
			RecordSet("ValueTypeName", value);
		}
	}

	/// <summary>
	/// The default interpolation type of this keyframe track. Writing it records a
	/// <c>DefaultInterpolation</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public InterpolationModes DefaultInterpolation
	{
		get { return _defaultInterpolation; }
		set
		{
			if (_defaultInterpolation == value)
			{
				return;
			}

			_defaultInterpolation = value;
			_isDefaultInterpolationWritten = true;
			RecordSet("DefaultInterpolation", value);
		}
	}

	/// <summary>Defines the interpolation factor method for this keyframe track.</summary>
	/// <param name="interpolation">The interpolation type.</param>
	public void SetInterpolation(InterpolationModes interpolation)
	{
		RecordCall("setInterpolation", interpolation);
	}

	/// <summary>Moves all keyframes either forward or backward in time.</summary>
	/// <param name="timeOffset">The offset to move the time values.</param>
	public void Shift(float timeOffset)
	{
		RecordCall("shift", timeOffset);
	}

	/// <summary>Scale all keyframe times by a factor (useful for frame - seconds conversions).</summary>
	/// <param name="timeScale">The time scale.</param>
	public void Scale(float timeScale)
	{
		RecordCall("scale", timeScale);
	}

	/// <summary>
	/// Removes keyframes before and after animation without changing any values within the defined time
	/// range. Note: The method does not shift around keys to the start of the track time, because for
	/// interpolated keys this will change their values.
	/// </summary>
	/// <param name="startTime">The start time.</param>
	/// <param name="endTime">The end time.</param>
	public void Trim(float startTime, float endTime)
	{
		RecordCall("trim", startTime, endTime);
	}

	/// <summary>
	/// Factory method for creating a new discrete interpolant. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>InterpolantFactoryMethodDiscrete</c> returned.
	/// </summary>
	/// <param name="result">The result buffer.</param>
	/// <returns>
	/// The value <c>InterpolantFactoryMethodDiscrete</c> returned, once the JavaScript side has
	/// answered.
	/// </returns>
	public Task<DiscreteInterpolant?> InterpolantFactoryMethodDiscreteAsync(TypedArray result)
	{
		return RecordReadObject<DiscreteInterpolant>("InterpolantFactoryMethodDiscrete", (adoptedBatch, adoptedHandle) => new DiscreteInterpolant(adoptedBatch, adoptedHandle), result);
	}

	/// <summary>
	/// Factory method for creating a new linear interpolant. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>InterpolantFactoryMethodLinear</c> returned.
	/// </summary>
	/// <param name="result">The result buffer.</param>
	/// <returns>
	/// The value <c>InterpolantFactoryMethodLinear</c> returned, once the JavaScript side has answered.
	/// </returns>
	public Task<LinearInterpolant?> InterpolantFactoryMethodLinearAsync(TypedArray result)
	{
		return RecordReadObject<LinearInterpolant>("InterpolantFactoryMethodLinear", (adoptedBatch, adoptedHandle) => new LinearInterpolant(adoptedBatch, adoptedHandle), result);
	}

	/// <summary>
	/// Factory method for creating a new smooth interpolant. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>InterpolantFactoryMethodSmooth</c> returned.
	/// </summary>
	/// <param name="result">The result buffer.</param>
	/// <returns>
	/// The value <c>InterpolantFactoryMethodSmooth</c> returned, once the JavaScript side has answered.
	/// </returns>
	public Task<CubicInterpolant?> InterpolantFactoryMethodSmoothAsync(TypedArray result)
	{
		return RecordReadObject<CubicInterpolant>("InterpolantFactoryMethodSmooth", (adoptedBatch, adoptedHandle) => new CubicInterpolant(adoptedBatch, adoptedHandle), result);
	}

	/// <summary>
	/// Factory method for creating a new Bezier interpolant. The Bezier interpolant requires tangent
	/// data to be set via the <c>settings</c> property on the track before creating the interpolant.
	/// The settings should contain: - <c>inTangents</c>: Float32Array with [time, value] pairs per
	/// keyframe per component - <c>outTangents</c>: Float32Array with [time, value] pairs per keyframe
	/// per component. Records a read op, sends it behind every write already pending, and completes
	/// with what <c>InterpolantFactoryMethodBezier</c> returned.
	/// </summary>
	/// <param name="result">The result buffer.</param>
	/// <returns>
	/// The value <c>InterpolantFactoryMethodBezier</c> returned, once the JavaScript side has answered.
	/// </returns>
	public Task<BezierInterpolant?> InterpolantFactoryMethodBezierAsync(TypedArray result)
	{
		return RecordReadObject<BezierInterpolant>("InterpolantFactoryMethodBezier", (adoptedBatch, adoptedHandle) => new BezierInterpolant(adoptedBatch, adoptedHandle), result);
	}

	/// <summary>
	/// Returns the current interpolation type. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getInterpolation</c> returned.
	/// </summary>
	/// <returns>The value <c>getInterpolation</c> returned, once the JavaScript side has answered.</returns>
	public Task<InterpolationModes> GetInterpolationAsync()
	{
		return RecordRead<InterpolationModes>("getInterpolation");
	}

	/// <summary>
	/// Returns the value size. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getValueSize</c> returned.
	/// </summary>
	/// <returns>The value <c>getValueSize</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetValueSizeAsync()
	{
		return RecordRead<float>("getValueSize");
	}

	/// <summary>
	/// Performs minimal validation on the keyframe track. Returns <c>true</c> if the values are valid.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>validate</c> returned.
	/// </summary>
	/// <returns>The value <c>validate</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> ValidateAsync()
	{
		return RecordRead<bool>("validate");
	}

	/// <summary>
	/// Optimizes this keyframe track by removing equivalent sequential keys (which are common in morph
	/// target sequences). Records a read op, sends it behind every write already pending, and completes
	/// with what <c>optimize</c> returned.
	/// </summary>
	/// <returns>The value <c>optimize</c> returned, once the JavaScript side has answered.</returns>
	public Task<StringKeyframeTrack?> OptimizeAsync()
	{
		return RecordReadObject<StringKeyframeTrack>("optimize", (adoptedBatch, adoptedHandle) => new StringKeyframeTrack(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns a new keyframe track with copied values from this instance. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<StringKeyframeTrack?> CloneAsync()
	{
		return RecordReadObject<StringKeyframeTrack>("clone", (adoptedBatch, adoptedHandle) => new StringKeyframeTrack(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.StringKeyframeTrack</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isTimesWritten)
		{
			batch.Set(Handle, "times", ThreeValue.Encode(_times));
		}

		if (_isValuesWritten)
		{
			batch.Set(Handle, "values", ThreeValue.Encode(_values));
		}

		if (_isValueTypeNameWritten)
		{
			batch.Set(Handle, "ValueTypeName", ThreeValue.Encode(_valueTypeName));
		}

		if (_isDefaultInterpolationWritten)
		{
			batch.Set(Handle, "DefaultInterpolation", ThreeValue.Encode(_defaultInterpolation));
		}
	}
}
