// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// InspectorBase is the base class for all inspectors. The JavaScript-side
/// <c>THREE.InspectorBase</c>.
/// </summary>
public sealed class InspectorBase : EventDispatcher
{
	/// <summary>Initializes a new <see cref="InspectorBase"/>.</summary>
	public InspectorBase()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>InspectorBase</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal InspectorBase(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.InspectorBase</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "InspectorBase"; }
	}

	/// <summary>Initializes the inspector.</summary>
	public void Init()
	{
		RecordCall("init");
	}

	/// <summary>Called when a frame begins.</summary>
	public void Begin()
	{
		RecordCall("begin");
	}

	/// <summary>Called when a frame ends.</summary>
	public void Finish()
	{
		RecordCall("finish");
	}

	/// <summary>Called when a compute operation ends.</summary>
	/// <param name="uid">A unique identifier for the render context.</param>
	public void FinishCompute(string uid)
	{
		RecordCall("finishCompute", uid);
	}

	/// <summary>Called when a render operation begins.</summary>
	/// <param name="uid">A unique identifier for the render context.</param>
	/// <param name="scene">The scene being rendered.</param>
	/// <param name="camera">The camera being used for rendering.</param>
	/// <param name="renderTarget">The render target, if any.</param>
	public void BeginRender(string uid, Scene scene, Camera camera, RenderTarget renderTarget)
	{
		RecordCall("beginRender", uid, scene, camera, renderTarget);
	}

	/// <summary>Called when an animation loop ends.</summary>
	/// <param name="uid">A unique identifier for the render context.</param>
	public void FinishRender(string uid)
	{
		RecordCall("finishRender", uid);
	}

	/// <summary>Called when a texture copy operation is performed.</summary>
	/// <param name="srcTexture">The source texture.</param>
	/// <param name="dstTexture">The destination texture.</param>
	public void CopyTextureToTexture(Texture srcTexture, Texture dstTexture)
	{
		RecordCall("copyTextureToTexture", srcTexture, dstTexture);
	}

	/// <summary>Called when a framebuffer copy operation is performed.</summary>
	/// <param name="framebufferTexture">The texture associated with the framebuffer.</param>
	public void CopyFramebufferToTexture(Texture framebufferTexture)
	{
		RecordCall("copyFramebufferToTexture", framebufferTexture);
	}
}
