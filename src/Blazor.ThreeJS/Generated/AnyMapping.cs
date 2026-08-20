// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Texture Mapping Modes for any type of Textures. Encoded on the wire as the numeric value
/// three.js itself uses, not as the member name.
/// </summary>
public enum AnyMapping : ushort
{
	/// <summary>Maps the texture using the mesh's UV coordinates.</summary>
	UVMapping = 300,

	/// <summary>Matches <c>THREE.EquirectangularReflectionMapping</c>.</summary>
	EquirectangularReflectionMapping = 303,

	/// <summary>Matches <c>THREE.EquirectangularRefractionMapping</c>.</summary>
	EquirectangularRefractionMapping = 304,

	/// <summary>Matches <c>THREE.CubeReflectionMapping</c>.</summary>
	CubeReflectionMapping = 301,

	/// <summary>Matches <c>THREE.CubeRefractionMapping</c>.</summary>
	CubeRefractionMapping = 302,

	/// <summary>Matches <c>THREE.CubeUVReflectionMapping</c>.</summary>
	CubeUVReflectionMapping = 306
}
