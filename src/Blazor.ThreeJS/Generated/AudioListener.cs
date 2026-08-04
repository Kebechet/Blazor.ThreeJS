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
}
