// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Represents a non-positional ( global ) audio object. This and related audio modules make use of
/// the [Web Audio API](https://www.w3.org/TR/webaudio-1.1/). The JavaScript-side
/// <c>THREE.Audio</c>.
/// </summary>
public class Audio : Object3D
{
	private readonly AudioListener _listener;
	private bool _autoplay;
	private float _loopStart = 0f;
	private float _loopEnd = 0f;
	private float _offset = 0f;
	private float? _duration;
	private bool _isAutoplayWritten;
	private bool _isLoopStartWritten;
	private bool _isLoopEndWritten;
	private bool _isOffsetWritten;
	private bool _isDurationWritten;

	/// <summary>Constructs a new audio.</summary>
	/// <param name="listener">The global audio listener.</param>
	public Audio(AudioListener listener)
	{
		_listener = listener;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Audio</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Audio(ThreeBatch batch, int handle)
		: base(handle)
	{
		_listener = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Audio</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Audio"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.Audio</c>: listener.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_listener]; }
	}

	/// <summary>
	/// Whether to start playback automatically or not. Writing it records a <c>autoplay</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Autoplay
	{
		get { return _autoplay; }
		set
		{
			if (_autoplay == value)
			{
				return;
			}

			_autoplay = value;
			_isAutoplayWritten = true;
			RecordSet("autoplay", value);
		}
	}

	/// <summary>
	/// Defines where in the audio buffer the replay should start, in seconds. Writing it records a
	/// <c>loopStart</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float LoopStart
	{
		get { return _loopStart; }
		set
		{
			if (_loopStart == value)
			{
				return;
			}

			_loopStart = value;
			_isLoopStartWritten = true;
			RecordSet("loopStart", value);
		}
	}

	/// <summary>
	/// Defines where in the audio buffer the replay should stop, in seconds. Writing it records a
	/// <c>loopEnd</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float LoopEnd
	{
		get { return _loopEnd; }
		set
		{
			if (_loopEnd == value)
			{
				return;
			}

			_loopEnd = value;
			_isLoopEndWritten = true;
			RecordSet("loopEnd", value);
		}
	}

	/// <summary>
	/// An offset to the time within the audio buffer the playback should begin, in seconds. Writing it
	/// records a <c>offset</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float Offset
	{
		get { return _offset; }
		set
		{
			if (_offset == value)
			{
				return;
			}

			_offset = value;
			_isOffsetWritten = true;
			RecordSet("offset", value);
		}
	}

	/// <summary>
	/// Overrides the default duration of the audio. Writing it records a <c>duration</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float? Duration
	{
		get { return _duration; }
		set
		{
			if (_duration == value)
			{
				return;
			}

			_duration = value;
			_isDurationWritten = true;
			RecordSet("duration", value);
		}
	}

	/// <summary>Defines the detuning of oscillation in cents.</summary>
	/// <param name="value">The detuning of oscillation in cents.</param>
	public void SetDetune(float value)
	{
		RecordCall("setDetune", value);
	}

	/// <summary>Automatically called when playback finished.</summary>
	public void OnEnded()
	{
		RecordCall("onEnded");
	}

	/// <summary>
	/// Sets the loop start value which defines where in the audio buffer the replay should start, in
	/// seconds. This writes the same three.js state as <see cref="LoopStart"/> and the mirror does not
	/// learn from it: afterwards <c>LoopStart</c> still reports its previous value, and writing that
	/// value back records nothing at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="value">The loop start value.</param>
	public void SetLoopStart(float value)
	{
		RecordCall("setLoopStart", value);
	}

	/// <summary>
	/// Sets the loop end value which defines where in the audio buffer the replay should stop, in
	/// seconds. This writes the same three.js state as <see cref="LoopEnd"/> and the mirror does not
	/// learn from it: afterwards <c>LoopEnd</c> still reports its previous value, and writing that
	/// value back records nothing at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="value">The loop end value.</param>
	public void SetLoopEnd(float value)
	{
		RecordCall("setLoopEnd", value);
	}

	/// <summary>Sets the volume.</summary>
	/// <param name="value">The volume to set.</param>
	public void SetVolume(float value)
	{
		RecordCall("setVolume", value);
	}

	/// <summary>
	/// The global audio listener. Read-only in three.js, so it is read on demand rather than mirrored:
	/// records a get op, sends it behind every write already pending, and completes with the value
	/// <c>listener</c> held.
	/// </summary>
	/// <returns>The value <c>listener</c> held, once the JavaScript side has answered.</returns>
	public Task<AudioListener?> ListenerAsync()
	{
		return RecordGetObject<AudioListener>("listener", (adoptedBatch, adoptedHandle) => new AudioListener(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Modify pitch, measured in cents. +/- 100 is a semitone. +/- 1200 is an octave. Defined via
	/// <c>Audio#setDetune</c>. Read-only in three.js, so it is read on demand rather than mirrored:
	/// records a get op, sends it behind every write already pending, and completes with the value
	/// <c>detune</c> held.
	/// </summary>
	/// <returns>The value <c>detune</c> held, once the JavaScript side has answered.</returns>
	public Task<float> DetuneAsync()
	{
		return GetAsync<float>("detune");
	}

	/// <summary>
	/// Whether the audio should loop or not. Defined via <c>Audio#setLoop</c>. Read-only in three.js,
	/// so it is read on demand rather than mirrored: records a get op, sends it behind every write
	/// already pending, and completes with the value <c>loop</c> held.
	/// </summary>
	/// <returns>The value <c>loop</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> LoopAsync()
	{
		return GetAsync<bool>("loop");
	}

	/// <summary>
	/// The playback speed. Defined via <c>Audio#setPlaybackRate</c>. Read-only in three.js, so it is
	/// read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>playbackRate</c> held.
	/// </summary>
	/// <returns>The value <c>playbackRate</c> held, once the JavaScript side has answered.</returns>
	public Task<float> PlaybackRateAsync()
	{
		return GetAsync<float>("playbackRate");
	}

	/// <summary>
	/// Indicates whether the audio is playing or not. This flag will be automatically set when using
	/// <c>Audio#play</c>, <c>Audio#pause</c>, <c>Audio#stop</c>. Read-only in three.js, so it is read
	/// on demand rather than mirrored: records a get op, sends it behind every write already pending,
	/// and completes with the value <c>isPlaying</c> held.
	/// </summary>
	/// <returns>The value <c>isPlaying</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsPlayingAsync()
	{
		return GetAsync<bool>("isPlaying");
	}

	/// <summary>
	/// Indicates whether the audio playback can be controlled with method like <c>Audio#play</c> or
	/// <c>Audio#pause</c>. This flag will be automatically set when audio sources are defined.
	/// Read-only in three.js, so it is read on demand rather than mirrored: records a get op, sends it
	/// behind every write already pending, and completes with the value <c>hasPlaybackControl</c> held.
	/// </summary>
	/// <returns>The value <c>hasPlaybackControl</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> HasPlaybackControlAsync()
	{
		return GetAsync<bool>("hasPlaybackControl");
	}

	/// <summary>
	/// Connects to the audio source. This is used internally on initialisation and when setting /
	/// removing filters. Records a read op, sends it behind every write already pending, and completes
	/// with what <c>connect</c> returned.
	/// </summary>
	/// <returns>The value <c>connect</c> returned, once the JavaScript side has answered.</returns>
	public Task<Audio?> ConnectAsync()
	{
		return RecordReadObject<Audio>("connect", (adoptedBatch, adoptedHandle) => new Audio(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns the detuning of oscillation in cents. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>getDetune</c> returned.
	/// </summary>
	/// <returns>The value <c>getDetune</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetDetuneAsync()
	{
		return RecordRead<float>("getDetune");
	}

	/// <summary>
	/// Returns the current playback rate. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getPlaybackRate</c> returned.
	/// </summary>
	/// <returns>The value <c>getPlaybackRate</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetPlaybackRateAsync()
	{
		return RecordRead<float>("getPlaybackRate");
	}

	/// <summary>
	/// Returns the loop flag. Can only be used with compatible audio sources that allow playback
	/// control. Records a read op, sends it behind every write already pending, and completes with what
	/// <c>getLoop</c> returned.
	/// </summary>
	/// <returns>The value <c>getLoop</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> GetLoopAsync()
	{
		return RecordRead<bool>("getLoop");
	}

	/// <summary>
	/// Returns the volume. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getVolume</c> returned.
	/// </summary>
	/// <returns>The value <c>getVolume</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetVolumeAsync()
	{
		return RecordRead<float>("getVolume");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.Audio</c> is constructed from, so their create ops reach the batch
	/// before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_listener.AttachTo(batch);

		base.EmitCreate(batch);
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isAutoplayWritten)
		{
			batch.Set(Handle, "autoplay", ThreeValue.Encode(_autoplay));
		}

		if (_isLoopStartWritten)
		{
			batch.Set(Handle, "loopStart", ThreeValue.Encode(_loopStart));
		}

		if (_isLoopEndWritten)
		{
			batch.Set(Handle, "loopEnd", ThreeValue.Encode(_loopEnd));
		}

		if (_isOffsetWritten)
		{
			batch.Set(Handle, "offset", ThreeValue.Encode(_offset));
		}

		if (_isDurationWritten)
		{
			batch.Set(Handle, "duration", ThreeValue.Encode(_duration));
		}
	}
}
