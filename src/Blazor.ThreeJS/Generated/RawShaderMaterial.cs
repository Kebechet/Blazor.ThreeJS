// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class works just like <see cref="ShaderMaterial"/>, except that definitions of built-in
/// uniforms and attributes are not automatically prepended to the GLSL shader code.
/// <c>RawShaderMaterial</c> can only be used with <see cref="WebGLRenderer"/>. The JavaScript-side
/// <c>THREE.RawShaderMaterial</c>.
/// </summary>
public sealed class RawShaderMaterial : ShaderMaterial
{
	/// <summary>Initializes a new <see cref="RawShaderMaterial"/>.</summary>
	public RawShaderMaterial()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.RawShaderMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "RawShaderMaterial"; }
	}
}
