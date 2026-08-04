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
	/// seconds.
	/// </summary>
	/// <param name="value">The loop start value.</param>
	public void SetLoopStart(float value)
	{
		RecordCall("setLoopStart", value);
	}

	/// <summary>
	/// Sets the loop end value which defines where in the audio buffer the replay should stop, in
	/// seconds.
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
