// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class can only be used in combination with <c>WebGLRenderer.copyFramebufferToTexture()</c>.
/// The JavaScript-side <c>THREE.FramebufferTexture</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl_framebuffer_texture">webgl_framebuffer_texture</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/textures/FramebufferTexture">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/textures/FramebufferTexture.js">Source</seealso>
public sealed class FramebufferTexture : Texture
{
	private readonly float _width;
	private readonly float _height;

	/// <summary>Create a new instance of <see cref="FramebufferTexture"/>.</summary>
	/// <param name="width">The width of the texture.</param>
	/// <param name="height">The height of the texture.</param>
	public FramebufferTexture(float width, float height)
	{
		_width = width;
		_height = height;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>FramebufferTexture</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal FramebufferTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_width = default!;
		_height = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.FramebufferTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "FramebufferTexture"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.FramebufferTexture</c>: width, height.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_width, _height]; }
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="FramebufferTexture"/>. Read-only
	/// in three.js, so it is read on demand rather than mirrored: records a get op, sends it behind
	/// every write already pending, and completes with the value <c>isFramebufferTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isFramebufferTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsFramebufferTextureAsync()
	{
		return GetAsync<bool>("isFramebufferTexture");
	}
}
