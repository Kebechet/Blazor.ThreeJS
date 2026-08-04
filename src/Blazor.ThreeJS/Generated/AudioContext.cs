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

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AudioContext</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AudioContext"; }
	}
}
