// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// An instance of <c>AnimationAction</c> schedules the playback of an animation which is stored in
/// <see cref="AnimationClip"/>. The JavaScript-side <c>THREE.AnimationAction</c>.
/// </summary>
public sealed class AnimationAction : ThreeObject
{
	private readonly AnimationMixer _mixer;
	private readonly AnimationClip _clip;
	private readonly Object3D? _localRoot;
	private AnimationBlendMode? _blendMode;
	private AnimationActionLoopStyles _loop = AnimationActionLoopStyles.LoopRepeat;
	private float _time;
	private float _timeScale = 1f;
	private float _weight = 1f;
	private float _repetitions;
	private bool _paused = false;
	private bool _enabled = true;
	private bool _clampWhenFinished = false;
	private bool _zeroSlopeAtStart = true;
	private bool _zeroSlopeAtEnd = true;
	private bool _isBlendModeWritten;
	private bool _isLoopWritten;
	private bool _isTimeWritten;
	private bool _isTimeScaleWritten;
	private bool _isWeightWritten;
	private bool _isRepetitionsWritten;
	private bool _isPausedWritten;
	private bool _isEnabledWritten;
	private bool _isClampWhenFinishedWritten;
	private bool _isZeroSlopeAtStartWritten;
	private bool _isZeroSlopeAtEndWritten;

	/// <summary>Constructs a new animation action.</summary>
	/// <param name="mixer">The mixer that is controlled by this action.</param>
	/// <param name="clip">The animation clip that holds the actual keyframes.</param>
	/// <param name="localRoot">The root object on which this action is performed.</param>
	/// <param name="blendMode">The blend mode.</param>
	public AnimationAction(
		AnimationMixer mixer,
		AnimationClip clip,
		Object3D? localRoot = null,
		AnimationBlendMode? blendMode = null)
	{
		_mixer = mixer;
		_clip = clip;
		_localRoot = localRoot;
		_blendMode = blendMode;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>AnimationAction</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal AnimationAction(ThreeBatch batch, int handle)
		: base(handle)
	{
		_mixer = default!;
		_clip = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AnimationAction</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AnimationAction"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.AnimationAction</c>: mixer, clip, localRoot,
	/// blendMode. An argument the caller left unspecified travels as the wire's not-supplied sentinel,
	/// or is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_mixer,
				_clip,
				ThreeValue.OrUnspecified(_localRoot),
				ThreeValue.OrUnspecified(_blendMode)
			]);
		}
	}

	/// <summary>
	/// Defines how the animation is blended/combined when two or more animations are simultaneously
	/// played. Writing it records a <c>blendMode</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public AnimationBlendMode? BlendMode
	{
		get { return _blendMode; }
		set
		{
			if (_blendMode == value)
			{
				return;
			}

			_blendMode = value;
			_isBlendModeWritten = true;
			RecordSet("blendMode", value);
		}
	}

	/// <summary>
	/// The loop mode, set via <c>AnimationAction#setLoop</c>. Writing it records a <c>loop</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public AnimationActionLoopStyles Loop
	{
		get { return _loop; }
		set
		{
			if (_loop == value)
			{
				return;
			}

			_loop = value;
			_isLoopWritten = true;
			RecordSet("loop", value);
		}
	}

	/// <summary>
	/// The local time of this action (in seconds, starting with <c>0</c>). The value gets clamped or
	/// wrapped to <c>[0,clip.duration]</c> (according to the loop state). Writing it records a
	/// <c>time</c> property write once this object is attached; writing the value already held records
	/// nothing.
	/// </summary>
	public float Time
	{
		get { return _time; }
		set
		{
			if (_time == value)
			{
				return;
			}

			_time = value;
			_isTimeWritten = true;
			RecordSet("time", value);
		}
	}

	/// <summary>
	/// Scaling factor for the <c>AnimationAction#time</c>. A value of <c>0</c> causes the animation to
	/// pause. Negative values cause the animation to play backwards. Writing it records a
	/// <c>timeScale</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float TimeScale
	{
		get { return _timeScale; }
		set
		{
			if (_timeScale == value)
			{
				return;
			}

			_timeScale = value;
			_isTimeScaleWritten = true;
			RecordSet("timeScale", value);
		}
	}

	/// <summary>
	/// The degree of influence of this action (in the interval <c>[0, 1]</c>). Values between <c>0</c>
	/// (no impact) and <c>1</c> (full impact) can be used to blend between several actions. Writing it
	/// records a <c>weight</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float Weight
	{
		get { return _weight; }
		set
		{
			if (_weight == value)
			{
				return;
			}

			_weight = value;
			_isWeightWritten = true;
			RecordSet("weight", value);
		}
	}

	/// <summary>
	/// The number of repetitions of the performed clip over the course of this action. Can be set via
	/// <c>AnimationAction#setLoop</c>. Setting this number has no effect if <c>AnimationAction#loop</c>
	/// is set to <c>THREE:LoopOnce</c>. Writing it records a <c>repetitions</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Repetitions
	{
		get { return _repetitions; }
		set
		{
			if (_repetitions == value)
			{
				return;
			}

			_repetitions = value;
			_isRepetitionsWritten = true;
			RecordSet("repetitions", value);
		}
	}

	/// <summary>
	/// If set to <c>true</c>, the playback of the action is paused. Writing it records a <c>paused</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Paused
	{
		get { return _paused; }
		set
		{
			if (_paused == value)
			{
				return;
			}

			_paused = value;
			_isPausedWritten = true;
			RecordSet("paused", value);
		}
	}

	/// <summary>
	/// If set to <c>false</c>, the action is disabled so it has no impact. When the action is
	/// re-enabled, the animation continues from its current time (setting <c>enabled</c> to
	/// <c>false</c> doesn't reset the action). Writing it records a <c>enabled</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Enabled
	{
		get { return _enabled; }
		set
		{
			if (_enabled == value)
			{
				return;
			}

			_enabled = value;
			_isEnabledWritten = true;
			RecordSet("enabled", value);
		}
	}

	/// <summary>
	/// If set to true the animation will automatically be paused on its last frame. If set to false,
	/// <c>AnimationAction#enabled</c> will automatically be switched to <c>false</c> when the last loop
	/// of the action has finished, so that this action has no further impact. Note: This member has no
	/// impact if the action is interrupted (it has only an effect if its last loop has really
	/// finished). Writing it records a <c>clampWhenFinished</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public bool ClampWhenFinished
	{
		get { return _clampWhenFinished; }
		set
		{
			if (_clampWhenFinished == value)
			{
				return;
			}

			_clampWhenFinished = value;
			_isClampWhenFinishedWritten = true;
			RecordSet("clampWhenFinished", value);
		}
	}

	/// <summary>
	/// Enables smooth interpolation without separate clips for start, loop and end. Writing it records
	/// a <c>zeroSlopeAtStart</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public bool ZeroSlopeAtStart
	{
		get { return _zeroSlopeAtStart; }
		set
		{
			if (_zeroSlopeAtStart == value)
			{
				return;
			}

			_zeroSlopeAtStart = value;
			_isZeroSlopeAtStartWritten = true;
			RecordSet("zeroSlopeAtStart", value);
		}
	}

	/// <summary>
	/// Enables smooth interpolation without separate clips for start, loop and end. Writing it records
	/// a <c>zeroSlopeAtEnd</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public bool ZeroSlopeAtEnd
	{
		get { return _zeroSlopeAtEnd; }
		set
		{
			if (_zeroSlopeAtEnd == value)
			{
				return;
			}

			_zeroSlopeAtEnd = value;
			_isZeroSlopeAtEndWritten = true;
			RecordSet("zeroSlopeAtEnd", value);
		}
	}

	/// <summary>Defines the time when the animation should start.</summary>
	/// <param name="time">The start time in seconds.</param>
	public void StartAt(float time)
	{
		RecordCall("startAt", time);
	}

	/// <summary>
	/// Configures the loop settings for this action. This writes the same three.js state as
	/// <see cref="Loop"/> and the mirror does not learn from it: afterwards <c>Loop</c> still reports
	/// its previous value, and writing that value back records nothing at all. Where the property
	/// exists, write the property.
	/// </summary>
	/// <param name="mode">The loop mode.</param>
	/// <param name="repetitions">The number of repetitions.</param>
	public void SetLoop(AnimationActionLoopStyles mode, float repetitions)
	{
		RecordCall("setLoop", mode, repetitions);
	}

	/// <summary>
	/// Sets the effective weight of this action. An action has no effect and thus an effective weight
	/// of zero when the action is disabled.
	/// </summary>
	/// <param name="weight">The weight to set.</param>
	public void SetEffectiveWeight(float weight)
	{
		RecordCall("setEffectiveWeight", weight);
	}

	/// <summary>
	/// Fades the animation in by increasing its weight gradually from <c>0</c> to <c>1</c>, within the
	/// passed time interval.
	/// </summary>
	/// <param name="duration">The duration of the fade.</param>
	public void FadeIn(float duration)
	{
		RecordCall("fadeIn", duration);
	}

	/// <summary>
	/// Fades the animation out by decreasing its weight gradually from <c>1</c> to <c>0</c>, within the
	/// passed time interval.
	/// </summary>
	/// <param name="duration">The duration of the fade.</param>
	public void FadeOut(float duration)
	{
		RecordCall("fadeOut", duration);
	}

	/// <summary>
	/// Causes this action to fade in and the given action to fade out, within the passed time interval.
	/// </summary>
	/// <param name="fadeOutAction">The animation action to fade out.</param>
	/// <param name="duration">The duration of the fade.</param>
	/// <param name="warp">Whether warping should be used or not.</param>
	public void CrossFadeFrom(AnimationAction fadeOutAction, float duration, bool warp = false)
	{
		RecordCall("crossFadeFrom", fadeOutAction, duration, warp);
	}

	/// <summary>
	/// Causes this action to fade out and the given action to fade in, within the passed time interval.
	/// </summary>
	/// <param name="fadeInAction">The animation action to fade in.</param>
	/// <param name="duration">The duration of the fade.</param>
	/// <param name="warp">Whether warping should be used or not.</param>
	public void CrossFadeTo(AnimationAction fadeInAction, float duration, bool warp = false)
	{
		RecordCall("crossFadeTo", fadeInAction, duration, warp);
	}

	/// <summary>
	/// Sets the effective time scale of this action. An action has no effect and thus an effective time
	/// scale of zero when the action is paused.
	/// </summary>
	/// <param name="timeScale">The time scale to set.</param>
	public void SetEffectiveTimeScale(float timeScale)
	{
		RecordCall("setEffectiveTimeScale", timeScale);
	}

	/// <summary>Sets the duration for a single loop of this action.</summary>
	/// <param name="duration">The duration to set.</param>
	public void SetDuration(float duration)
	{
		RecordCall("setDuration", duration);
	}

	/// <summary>Synchronizes this action with the passed other action.</summary>
	/// <param name="action">The action to sync with.</param>
	public void SyncWith(AnimationAction action)
	{
		RecordCall("syncWith", action);
	}

	/// <summary>Decelerates this animation's speed to <c>0</c> within the passed time interval.</summary>
	/// <param name="duration">The duration.</param>
	public void Halt(float duration)
	{
		RecordCall("halt", duration);
	}

	/// <summary>
	/// Changes the playback speed, within the passed time interval, by modifying
	/// <c>AnimationAction#timeScale</c> gradually from <c>startTimeScale</c> to <c>endTimeScale</c>.
	/// </summary>
	/// <param name="startTimeScale">The start time scale.</param>
	/// <param name="endTimeScale">The end time scale.</param>
	/// <param name="duration">The duration.</param>
	public void Warp(float startTimeScale, float endTimeScale, float duration)
	{
		RecordCall("warp", startTimeScale, endTimeScale, duration);
	}

	/// <summary>Records a call to <c>_scheduleFading</c> on the JavaScript-side object.</summary>
	/// <param name="duration">Value forwarded to the <c>duration</c> argument.</param>
	/// <param name="weightNow">Value forwarded to the <c>weightNow</c> argument.</param>
	/// <param name="weightThen">Value forwarded to the <c>weightThen</c> argument.</param>
	public void _scheduleFading(float duration, float weightNow, float weightThen)
	{
		RecordCall("_scheduleFading", duration, weightNow, weightThen);
	}

	/// <summary>
	/// Starts the playback of the animation. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>play</c> returned.
	/// </summary>
	/// <returns>The value <c>play</c> returned, once the JavaScript side has answered.</returns>
	public Task<AnimationAction?> PlayAsync()
	{
		return RecordReadObject<AnimationAction>("play", (adoptedBatch, adoptedHandle) => new AnimationAction(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Stops the playback of the animation. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>stop</c> returned.
	/// </summary>
	/// <returns>The value <c>stop</c> returned, once the JavaScript side has answered.</returns>
	public Task<AnimationAction?> StopAsync()
	{
		return RecordReadObject<AnimationAction>("stop", (adoptedBatch, adoptedHandle) => new AnimationAction(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Resets the playback of the animation. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>reset</c> returned.
	/// </summary>
	/// <returns>The value <c>reset</c> returned, once the JavaScript side has answered.</returns>
	public Task<AnimationAction?> ResetAsync()
	{
		return RecordReadObject<AnimationAction>("reset", (adoptedBatch, adoptedHandle) => new AnimationAction(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns <c>true</c> if the animation is running. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>isRunning</c> returned.
	/// </summary>
	/// <returns>The value <c>isRunning</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> IsRunningAsync()
	{
		return RecordRead<bool>("isRunning");
	}

	/// <summary>
	/// Returns <c>true</c> when <c>AnimationAction#play</c> has been called. Records a read op, sends
	/// it behind every write already pending, and completes with what <c>isScheduled</c> returned.
	/// </summary>
	/// <returns>The value <c>isScheduled</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> IsScheduledAsync()
	{
		return RecordRead<bool>("isScheduled");
	}

	/// <summary>
	/// Returns the effective weight of this action. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>getEffectiveWeight</c> returned.
	/// </summary>
	/// <returns>The value <c>getEffectiveWeight</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetEffectiveWeightAsync()
	{
		return RecordRead<float>("getEffectiveWeight");
	}

	/// <summary>
	/// Stops any fading which is applied to this action. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>stopFading</c> returned.
	/// </summary>
	/// <returns>The value <c>stopFading</c> returned, once the JavaScript side has answered.</returns>
	public Task<AnimationAction?> StopFadingAsync()
	{
		return RecordReadObject<AnimationAction>("stopFading", (adoptedBatch, adoptedHandle) => new AnimationAction(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns the effective time scale of this action. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>getEffectiveTimeScale</c> returned.
	/// </summary>
	/// <returns>The value <c>getEffectiveTimeScale</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetEffectiveTimeScaleAsync()
	{
		return RecordRead<float>("getEffectiveTimeScale");
	}

	/// <summary>
	/// Stops any scheduled warping which is applied to this action. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>stopWarping</c> returned.
	/// </summary>
	/// <returns>The value <c>stopWarping</c> returned, once the JavaScript side has answered.</returns>
	public Task<AnimationAction?> StopWarpingAsync()
	{
		return RecordReadObject<AnimationAction>("stopWarping", (adoptedBatch, adoptedHandle) => new AnimationAction(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns the animation mixer of this animation action. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>getMixer</c> returned.
	/// </summary>
	/// <returns>The value <c>getMixer</c> returned, once the JavaScript side has answered.</returns>
	public Task<AnimationMixer?> GetMixerAsync()
	{
		return RecordReadObject<AnimationMixer>("getMixer", (adoptedBatch, adoptedHandle) => new AnimationMixer(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns the animation clip of this animation action. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>getClip</c> returned.
	/// </summary>
	/// <returns>The value <c>getClip</c> returned, once the JavaScript side has answered.</returns>
	public Task<AnimationClip?> GetClipAsync()
	{
		return RecordReadObject<AnimationClip>("getClip", (adoptedBatch, adoptedHandle) => new AnimationClip(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns the root object of this animation action. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>getRoot</c> returned.
	/// </summary>
	/// <returns>The value <c>getRoot</c> returned, once the JavaScript side has answered.</returns>
	public Task<Object3D?> GetRootAsync()
	{
		return RecordReadObject<Object3D>("getRoot", (adoptedBatch, adoptedHandle) => new PrimitiveObject3D(adoptedBatch, adoptedHandle, "Object3D"));
	}

	/// <summary>
	/// Attaches the objects <c>THREE.AnimationAction</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_mixer.AttachTo(batch);
		_clip.AttachTo(batch);
		_localRoot?.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isBlendModeWritten)
		{
			batch.Set(Handle, "blendMode", ThreeValue.Encode(_blendMode));
		}

		if (_isLoopWritten)
		{
			batch.Set(Handle, "loop", ThreeValue.Encode(_loop));
		}

		if (_isTimeWritten)
		{
			batch.Set(Handle, "time", ThreeValue.Encode(_time));
		}

		if (_isTimeScaleWritten)
		{
			batch.Set(Handle, "timeScale", ThreeValue.Encode(_timeScale));
		}

		if (_isWeightWritten)
		{
			batch.Set(Handle, "weight", ThreeValue.Encode(_weight));
		}

		if (_isRepetitionsWritten)
		{
			batch.Set(Handle, "repetitions", ThreeValue.Encode(_repetitions));
		}

		if (_isPausedWritten)
		{
			batch.Set(Handle, "paused", ThreeValue.Encode(_paused));
		}

		if (_isEnabledWritten)
		{
			batch.Set(Handle, "enabled", ThreeValue.Encode(_enabled));
		}

		if (_isClampWhenFinishedWritten)
		{
			batch.Set(Handle, "clampWhenFinished", ThreeValue.Encode(_clampWhenFinished));
		}

		if (_isZeroSlopeAtStartWritten)
		{
			batch.Set(Handle, "zeroSlopeAtStart", ThreeValue.Encode(_zeroSlopeAtStart));
		}

		if (_isZeroSlopeAtEndWritten)
		{
			batch.Set(Handle, "zeroSlopeAtEnd", ThreeValue.Encode(_zeroSlopeAtEnd));
		}
	}
}
