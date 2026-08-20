// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Manages the global audio context in the engine. The JavaScript-side <c>THREE.AudioContext</c>.
/// </summary>
public sealed class AudioContext : ThreeObject
{
	/// <summary>Initializes a new <see cref="AudioContext"/>.</summary>
	public AudioContext()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>AudioContext</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal AudioContext(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AudioContext</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AudioContext"; }
	}

	/// <summary>Allows to set the global native audio context from outside.</summary>
	/// <param name="value">The native context to set.</param>
	public void SetContext(AudioContext value)
	{
		RecordCall("setContext", value);
	}

	/// <summary>
	/// Returns the global native audio context. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getContext</c> returned.
	/// </summary>
	/// <param name="context">
	/// Context the call belongs to; a static has no object of its own to record through.
	/// </param>
	/// <returns>The value <c>getContext</c> returned, once the JavaScript side has answered.</returns>
	public static Task<AudioContext?> GetContextAsync(ThreeContext context)
	{
		return context.CallStaticAsync<AudioContext>("AudioContext", "getContext");
	}
}
