// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.WebGLCubeRenderTarget</c>.</summary>
public sealed class WebGLCubeRenderTarget : WebGLRenderTarget
{
	private readonly float? _size;

	/// <summary>Initializes a new <see cref="WebGLCubeRenderTarget"/>.</summary>
	/// <param name="size">Value forwarded to the <c>size</c> constructor argument.</param>
	public WebGLCubeRenderTarget(float? size = null)
	{
		_size = size;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WebGLCubeRenderTarget</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WebGLCubeRenderTarget"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.WebGLCubeRenderTarget</c>: size. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_size)]); }
	}

	/// <summary>Records a call to <c>clear</c> on the JavaScript-side object.</summary>
	/// <param name="renderer">Value forwarded to the <c>renderer</c> argument.</param>
	/// <param name="color">Value forwarded to the <c>color</c> argument.</param>
	/// <param name="depth">Value forwarded to the <c>depth</c> argument.</param>
	/// <param name="stencil">Value forwarded to the <c>stencil</c> argument.</param>
	public void Clear(WebGLRenderer renderer, bool color, bool depth, bool stencil)
	{
		if (Batch is not null)
		{
			renderer.AttachTo(Batch);
		}

		RecordCall("clear", renderer, color, depth, stencil);
	}
}
