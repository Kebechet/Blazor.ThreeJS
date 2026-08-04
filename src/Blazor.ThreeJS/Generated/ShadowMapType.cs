// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>ShadowMapType</c>. Encoded on the wire as the numeric value
/// three.js itself uses, not as the member name.
/// </summary>
public enum ShadowMapType : byte
{
	/// <summary>Matches <c>THREE.BasicShadowMap</c>.</summary>
	BasicShadowMap = 0,

	/// <summary>Matches <c>THREE.PCFShadowMap</c>.</summary>
	PCFShadowMap = 1,

	/// <summary>Matches <c>THREE.PCFSoftShadowMap</c>.</summary>
	PCFSoftShadowMap = 2,

	/// <summary>Matches <c>THREE.VSMShadowMap</c>.</summary>
	VSMShadowMap = 3
}
