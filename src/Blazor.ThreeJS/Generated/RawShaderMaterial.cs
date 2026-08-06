// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class works just like <see cref="ShaderMaterial"/>, except that definitions of built-in
/// uniforms and attributes are not automatically prepended to the GLSL shader code.
/// <c>RawShaderMaterial</c> can only be used with <c>WebGLRenderer</c>. The JavaScript-side
/// <c>THREE.RawShaderMaterial</c>.
/// </summary>
public sealed class RawShaderMaterial : ShaderMaterial
{
	/// <summary>Initializes a new <see cref="RawShaderMaterial"/>.</summary>
	public RawShaderMaterial()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>RawShaderMaterial</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal RawShaderMaterial(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.RawShaderMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "RawShaderMaterial"; }
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isRawShaderMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isRawShaderMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsRawShaderMaterialAsync()
	{
		return GetAsync<bool>("isRawShaderMaterial");
	}
}
