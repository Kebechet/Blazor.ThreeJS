// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Object for keeping track of time. This uses
/// [performance.now]<see href="https://developer.mozilla.org/en-US/docs/Web/API/Performance/now">https://developer.mozilla.org/en-US/docs/Web/API/Performance/now</see>.
/// The JavaScript-side <c>THREE.Clock</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/Clock">https://threejs.org/docs/index.html#api/en/core/Clock</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/Clock.js">https://github.com/mrdoob/three.js/blob/master/src/core/Clock.js</seealso>
public sealed class Clock : ThreeObject
{
	private bool _autoStart;
	private float _startTime = 0f;
	private float _oldTime = 0f;
	private float _elapsedTime = 0f;
	private bool _running = false;
	private bool _isAutoStartWritten;
	private bool _isStartTimeWritten;
	private bool _isOldTimeWritten;
	private bool _isElapsedTimeWritten;
	private bool _isRunningWritten;

	/// <summary>Create a new instance of <c>Clock</c>.</summary>
	/// <param name="autoStart">
	/// Whether to automatically start the clock when <c>.getDelta()</c> is called for the first time.
	/// </param>
	public Clock(bool autoStart = true)
	{
		_autoStart = autoStart;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Clock</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Clock"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.Clock</c>: autoStart.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_autoStart]; }
	}

	/// <summary>
	/// If set, starts the clock automatically when <c>.getDelta()</c> is called for the first time.
	/// Writing it records a <c>autoStart</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public bool AutoStart
	{
		get { return _autoStart; }
		set
		{
			if (_autoStart == value)
			{
				return;
			}

			_autoStart = value;
			_isAutoStartWritten = true;
			RecordSet("autoStart", value);
		}
	}

	/// <summary>
	/// Holds the time at which the clock's <c>.start()</c> method was last called. Writing it records a
	/// <c>startTime</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float StartTime
	{
		get { return _startTime; }
		set
		{
			if (_startTime == value)
			{
				return;
			}

			_startTime = value;
			_isStartTimeWritten = true;
			RecordSet("startTime", value);
		}
	}

	/// <summary>
	/// Holds the time at which the clock's <c>.start()</c>, <c>.getElapsedTime()</c> or
	/// <c>.getDelta()</c> methods were last called. Writing it records a <c>oldTime</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float OldTime
	{
		get { return _oldTime; }
		set
		{
			if (_oldTime == value)
			{
				return;
			}

			_oldTime = value;
			_isOldTimeWritten = true;
			RecordSet("oldTime", value);
		}
	}

	/// <summary>
	/// Keeps track of the total time that the clock has been running. Writing it records a
	/// <c>elapsedTime</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float ElapsedTime
	{
		get { return _elapsedTime; }
		set
		{
			if (_elapsedTime == value)
			{
				return;
			}

			_elapsedTime = value;
			_isElapsedTimeWritten = true;
			RecordSet("elapsedTime", value);
		}
	}

	/// <summary>
	/// Whether the clock is running or not. Writing it records a <c>running</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Running
	{
		get { return _running; }
		set
		{
			if (_running == value)
			{
				return;
			}

			_running = value;
			_isRunningWritten = true;
			RecordSet("running", value);
		}
	}

	/// <summary>Starts clock.</summary>
	public void Start()
	{
		RecordCall("start");
	}

	/// <summary>Stops clock and sets <c>oldTime</c> to the current time.</summary>
	public void Stop()
	{
		RecordCall("stop");
	}

	/// <summary>
	/// Get the seconds passed since the clock started and sets <c>.oldTime</c> to the current time.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>getElapsedTime</c> returned.
	/// </summary>
	/// <returns>The value <c>getElapsedTime</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetElapsedTimeAsync()
	{
		return RecordRead<float>("getElapsedTime");
	}

	/// <summary>
	/// Get the seconds passed since the time <c>.oldTime</c> was set and sets <c>.oldTime</c> to the
	/// current time. Records a read op, sends it behind every write already pending, and completes with
	/// what <c>getDelta</c> returned.
	/// </summary>
	/// <returns>The value <c>getDelta</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetDeltaAsync()
	{
		return RecordRead<float>("getDelta");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Clock</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isAutoStartWritten)
		{
			batch.Set(Handle, "autoStart", ThreeValue.Encode(_autoStart));
		}

		if (_isStartTimeWritten)
		{
			batch.Set(Handle, "startTime", ThreeValue.Encode(_startTime));
		}

		if (_isOldTimeWritten)
		{
			batch.Set(Handle, "oldTime", ThreeValue.Encode(_oldTime));
		}

		if (_isElapsedTimeWritten)
		{
			batch.Set(Handle, "elapsedTime", ThreeValue.Encode(_elapsedTime));
		}

		if (_isRunningWritten)
		{
			batch.Set(Handle, "running", ThreeValue.Encode(_running));
		}
	}
}
