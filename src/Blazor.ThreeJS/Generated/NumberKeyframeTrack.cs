// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A track for numeric keyframe values. The JavaScript-side <c>THREE.NumberKeyframeTrack</c>.</summary>
public sealed class NumberKeyframeTrack : ThreeObject
{
	private string _name;
	private float[] _times;
	private float[] _values;
	private readonly InterpolationModes? _interpolation;
	private string _valueTypeName = string.Empty;
	private InterpolationModes _defaultInterpolation = InterpolationModes.InterpolateLinear;
	private bool _isNameWritten;
	private bool _isTimesWritten;
	private bool _isValuesWritten;
	private bool _isValueTypeNameWritten;
	private bool _isDefaultInterpolationWritten;

	/// <summary>Constructs a new number keyframe track.</summary>
	/// <param name="name">The keyframe track's name.</param>
	/// <param name="times">A list of keyframe times.</param>
	/// <param name="values">A list of keyframe values.</param>
	/// <param name="interpolation">The interpolation type.</param>
	public NumberKeyframeTrack(string name, float[] times, float[] values, InterpolationModes? interpolation = null)
	{
		_name = name;
		_times = times;
		_values = values;
		_interpolation = interpolation;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>NumberKeyframeTrack</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal NumberKeyframeTrack(ThreeBatch batch, int handle)
		: base(handle)
	{
		_name = default!;
		_times = default!;
		_values = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.NumberKeyframeTrack</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "NumberKeyframeTrack"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.NumberKeyframeTrack</c>: name, times, values,
	/// interpolation. An argument the caller left unspecified travels as the wire's not-supplied
	/// sentinel, or is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_name,
				_times,
				_values,
				ThreeValue.OrUnspecified(_interpolation)
			]);
		}
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
	public float[] Values
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
	public Task<NumberKeyframeTrack?> OptimizeAsync()
	{
		return RecordReadObject<NumberKeyframeTrack>("optimize", (adoptedBatch, adoptedHandle) => new NumberKeyframeTrack(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns a new keyframe track with copied values from this instance. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<NumberKeyframeTrack?> CloneAsync()
	{
		return RecordReadObject<NumberKeyframeTrack>("clone", (adoptedBatch, adoptedHandle) => new NumberKeyframeTrack(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.NumberKeyframeTrack</c>, then replays every property written
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
