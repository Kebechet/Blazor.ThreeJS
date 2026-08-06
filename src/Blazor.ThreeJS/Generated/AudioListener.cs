// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The class represents a virtual listener of the all positional and non-positional audio effects
/// in the scene. A three.js application usually creates a single listener. It is a mandatory
/// constructor parameter for audios entities like <see cref="Audio"/> and <c>PositionalAudio</c>.
/// In most cases, the listener object is a child of the camera. So the 3D transformation of the
/// camera represents the 3D transformation of the listener. The JavaScript-side
/// <c>THREE.AudioListener</c>.
/// </summary>
public sealed class AudioListener : Object3D
{
	/// <summary>Initializes a new <see cref="AudioListener"/>.</summary>
	public AudioListener()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>AudioListener</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal AudioListener(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AudioListener</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AudioListener"; }
	}

	/// <summary>
	/// Sets the applications master volume. This volume setting affects all audio nodes in the scene.
	/// </summary>
	/// <param name="value">The master volume to set.</param>
	public void SetMasterVolume(float value)
	{
		RecordCall("setMasterVolume", value);
	}

	/// <summary>
	/// Time delta values required for <c>linearRampToValueAtTime()</c> usage. Read-only in three.js, so
	/// it is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>timeDelta</c> held.
	/// </summary>
	/// <returns>The value <c>timeDelta</c> held, once the JavaScript side has answered.</returns>
	public Task<float> TimeDeltaAsync()
	{
		return GetAsync<float>("timeDelta");
	}

	/// <summary>
	/// Removes the current filter from this listener. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>removeFilter</c> returned.
	/// </summary>
	/// <returns>The value <c>removeFilter</c> returned, once the JavaScript side has answered.</returns>
	public Task<AudioListener?> RemoveFilterAsync()
	{
		return RecordReadObject<AudioListener>("removeFilter", (adoptedBatch, adoptedHandle) => new AudioListener(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns the applications master volume. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getMasterVolume</c> returned.
	/// </summary>
	/// <returns>The value <c>getMasterVolume</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetMasterVolumeAsync()
	{
		return RecordRead<float>("getMasterVolume");
	}
}
