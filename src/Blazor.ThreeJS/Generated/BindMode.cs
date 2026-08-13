// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>BindMode</c>. Encoded on the wire as the string three.js
/// compares against, not as the C# value, which is only a position.
/// </summary>
public enum BindMode : byte
{
	/// <summary>Matches <c>THREE.AttachedBindMode</c>. Sent as <c>"attached"</c>.</summary>
	AttachedBindMode = 0,

	/// <summary>Matches <c>THREE.DetachedBindMode</c>. Sent as <c>"detached"</c>.</summary>
	DetachedBindMode = 1
}
