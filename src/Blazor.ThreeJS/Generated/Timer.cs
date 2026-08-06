// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class is an alternative to <c>THREE.Clock</c> with a different API design and behavior The
/// goal is to avoid the conceptual flaws that became apparent in <c>THREE.Clock</c> over time. -
/// <see cref="Timer"/> has an <c>.update()</c> method that updates its internal state. That makes
/// it possible to call <c>.getDelta()</c> and <c>.getElapsed()</c> multiple times per simulation
/// step without getting different values. - The class can make use of the Page Visibility API to
/// avoid large time delta values when the app is inactive (e.g. tab switched or browser hidden).
/// The JavaScript-side <c>THREE.Timer</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl_morphtargets_sphere">https://threejs.org/examples/#webgl_morphtargets_sphere</seealso>
public sealed class Timer : ThreeObject
{
	/// <summary>Initializes a new <see cref="Timer"/>.</summary>
	public Timer()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Timer</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Timer(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Timer</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Timer"; }
	}

	/// <summary>
	/// Disconnects the timer from the DOM and also disables the usage of the Page Visibility API.
	/// </summary>
	public void Disconnect()
	{
		RecordCall("disconnect");
	}

	/// <summary>Sets a time scale that scales the time delta in <c>.update()</c>.</summary>
	/// <param name="timescale">Value forwarded to the <c>timescale</c> argument.</param>
	public void SetTimescale(float timescale)
	{
		RecordCall("setTimescale", timescale);
	}

	/// <summary>
	/// Can be used to free all internal resources. Usually called when the timer instance isn't
	/// required anymore.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Updates the internal state of the timer. This method should be called once per simulation step
	/// and before you perform queries against the timer (e.g. via <c>()</c>).
	/// </summary>
	/// <param name="timestamp">
	/// The current time in milliseconds. Can be obtained from the
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/window/requestAnimationFrame">requestAnimationFrame</see>
	/// callback argument. If not provided, the current time will be determined with
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Performance/now">performance.now</see>.
	/// </param>
	public void Update(float timestamp)
	{
		RecordCall("update", timestamp);
	}

	/// <summary>
	/// Returns the time delta in seconds. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getDelta</c> returned.
	/// </summary>
	/// <returns>The value <c>getDelta</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetDeltaAsync()
	{
		return RecordRead<float>("getDelta");
	}

	/// <summary>
	/// Returns the elapsed time in seconds. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getElapsed</c> returned.
	/// </summary>
	/// <returns>The value <c>getElapsed</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetElapsedAsync()
	{
		return RecordRead<float>("getElapsed");
	}

	/// <summary>
	/// Returns the time scale. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getTimescale</c> returned.
	/// </summary>
	/// <returns>The value <c>getTimescale</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetTimescaleAsync()
	{
		return RecordRead<float>("getTimescale");
	}

	/// <summary>
	/// Resets the time computation for the current simulation step. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>reset</c> returned.
	/// </summary>
	/// <returns>The value <c>reset</c> returned, once the JavaScript side has answered.</returns>
	public Task<Timer?> ResetAsync()
	{
		return RecordReadObject<Timer>("reset", (adoptedBatch, adoptedHandle) => new Timer(adoptedBatch, adoptedHandle));
	}
}
