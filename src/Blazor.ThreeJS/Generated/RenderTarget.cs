// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.RenderTarget</c>.</summary>
public class RenderTarget : EventDispatcher
{
	private float? _width;
	private float? _height;
	private float _depth;
	private bool _scissorTest = false;
	private bool _depthBuffer = true;
	private bool _stencilBuffer = false;
	private bool _resolveDepthBuffer = true;
	private bool _resolveStencilBuffer = true;
	private float _samples = 0f;
	private bool _multiview = false;
	private bool _useArrayDepthTexture = false;
	private DepthTexture? _depthTexture;
	private bool _isWidthWritten;
	private bool _isHeightWritten;
	private bool _isDepthWritten;
	private bool _isScissorTestWritten;
	private bool _isDepthBufferWritten;
	private bool _isStencilBufferWritten;
	private bool _isResolveDepthBufferWritten;
	private bool _isResolveStencilBufferWritten;
	private bool _isSamplesWritten;
	private bool _isMultiviewWritten;
	private bool _isUseArrayDepthTextureWritten;
	private bool _isDepthTextureWritten;

	/// <summary>Initializes a new <see cref="RenderTarget"/>.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> constructor argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> constructor argument.</param>
	public RenderTarget(float? width = null, float? height = null)
	{
		_width = width;
		_height = height;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.RenderTarget</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "RenderTarget"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.RenderTarget</c>: width, height. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_width),
				ThreeValue.OrUnspecified(_height)
			]);
		}
	}

	/// <summary>
	/// The <c>width</c> property of the JavaScript-side object. Writing it records a <c>width</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float? Width
	{
		get { return _width; }
		set
		{
			if (_width == value)
			{
				return;
			}

			_width = value;
			_isWidthWritten = true;
			RecordSet("width", value);
		}
	}

	/// <summary>
	/// The <c>height</c> property of the JavaScript-side object. Writing it records a <c>height</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float? Height
	{
		get { return _height; }
		set
		{
			if (_height == value)
			{
				return;
			}

			_height = value;
			_isHeightWritten = true;
			RecordSet("height", value);
		}
	}

	/// <summary>
	/// The <c>depth</c> property of the JavaScript-side object. Writing it records a <c>depth</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Depth
	{
		get { return _depth; }
		set
		{
			if (_depth == value)
			{
				return;
			}

			_depth = value;
			_isDepthWritten = true;
			RecordSet("depth", value);
		}
	}

	/// <summary>
	/// The <c>scissorTest</c> property of the JavaScript-side object. Writing it records a
	/// <c>scissorTest</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool ScissorTest
	{
		get { return _scissorTest; }
		set
		{
			if (_scissorTest == value)
			{
				return;
			}

			_scissorTest = value;
			_isScissorTestWritten = true;
			RecordSet("scissorTest", value);
		}
	}

	/// <summary>
	/// The <c>depthBuffer</c> property of the JavaScript-side object. Writing it records a
	/// <c>depthBuffer</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool DepthBuffer
	{
		get { return _depthBuffer; }
		set
		{
			if (_depthBuffer == value)
			{
				return;
			}

			_depthBuffer = value;
			_isDepthBufferWritten = true;
			RecordSet("depthBuffer", value);
		}
	}

	/// <summary>
	/// The <c>stencilBuffer</c> property of the JavaScript-side object. Writing it records a
	/// <c>stencilBuffer</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool StencilBuffer
	{
		get { return _stencilBuffer; }
		set
		{
			if (_stencilBuffer == value)
			{
				return;
			}

			_stencilBuffer = value;
			_isStencilBufferWritten = true;
			RecordSet("stencilBuffer", value);
		}
	}

	/// <summary>
	/// Defines whether the depth buffer should be resolved when rendering into a multisampled render
	/// target. Writing it records a <c>resolveDepthBuffer</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public bool ResolveDepthBuffer
	{
		get { return _resolveDepthBuffer; }
		set
		{
			if (_resolveDepthBuffer == value)
			{
				return;
			}

			_resolveDepthBuffer = value;
			_isResolveDepthBufferWritten = true;
			RecordSet("resolveDepthBuffer", value);
		}
	}

	/// <summary>
	/// Defines whether the stencil buffer should be resolved when rendering into a multisampled render
	/// target. This property has no effect when <c>.resolveDepthBuffer</c> is set to <c>false</c>.
	/// Writing it records a <c>resolveStencilBuffer</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public bool ResolveStencilBuffer
	{
		get { return _resolveStencilBuffer; }
		set
		{
			if (_resolveStencilBuffer == value)
			{
				return;
			}

			_resolveStencilBuffer = value;
			_isResolveStencilBufferWritten = true;
			RecordSet("resolveStencilBuffer", value);
		}
	}

	/// <summary>
	/// Defines the count of MSAA samples. Can only be used with WebGL 2. Default is **0**. Writing it
	/// records a <c>samples</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float Samples
	{
		get { return _samples; }
		set
		{
			if (_samples == value)
			{
				return;
			}

			_samples = value;
			_isSamplesWritten = true;
			RecordSet("samples", value);
		}
	}

	/// <summary>
	/// Whether to this target is used in multiview rendering. Writing it records a <c>multiview</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Multiview
	{
		get { return _multiview; }
		set
		{
			if (_multiview == value)
			{
				return;
			}

			_multiview = value;
			_isMultiviewWritten = true;
			RecordSet("multiview", value);
		}
	}

	/// <summary>
	/// Whether to create the depth texture as an array texture for per-layer depth testing. This is
	/// separate from multiview so layered render targets can use array depth without the multiview
	/// extension. Writing it records a <c>useArrayDepthTexture</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public bool UseArrayDepthTexture
	{
		get { return _useArrayDepthTexture; }
		set
		{
			if (_useArrayDepthTexture == value)
			{
				return;
			}

			_useArrayDepthTexture = value;
			_isUseArrayDepthTextureWritten = true;
			RecordSet("useArrayDepthTexture", value);
		}
	}

	/// <summary>
	/// The <c>depthTexture</c> property of the JavaScript-side object. Writing it records a
	/// <c>depthTexture</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public DepthTexture? DepthTexture
	{
		get { return _depthTexture; }
		set
		{
			if (ReferenceEquals(_depthTexture, value))
			{
				return;
			}

			_depthTexture = value;
			_isDepthTextureWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("depthTexture", value);
		}
	}

	/// <summary>Records a call to <c>setSize</c> on the JavaScript-side object.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> argument.</param>
	/// <param name="depth">Value forwarded to the <c>depth</c> argument.</param>
	public void SetSize(float width, float height, float depth)
	{
		RecordCall("setSize", width, height, depth);
	}

	/// <summary>Records a call to <c>copy</c> on the JavaScript-side object.</summary>
	/// <param name="source">Value forwarded to the <c>source</c> argument.</param>
	public void Copy(RenderTarget source)
	{
		RecordCall("copy", source);
	}

	/// <summary>Records a call to <c>dispose</c> on the JavaScript-side object.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.RenderTarget</c>, then replays every property written before
	/// this object was attached. A replayed value that is itself a mirrored object is attached first,
	/// so its create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isWidthWritten)
		{
			batch.Set(Handle, "width", ThreeValue.Encode(_width));
		}

		if (_isHeightWritten)
		{
			batch.Set(Handle, "height", ThreeValue.Encode(_height));
		}

		if (_isDepthWritten)
		{
			batch.Set(Handle, "depth", ThreeValue.Encode(_depth));
		}

		if (_isScissorTestWritten)
		{
			batch.Set(Handle, "scissorTest", ThreeValue.Encode(_scissorTest));
		}

		if (_isDepthBufferWritten)
		{
			batch.Set(Handle, "depthBuffer", ThreeValue.Encode(_depthBuffer));
		}

		if (_isStencilBufferWritten)
		{
			batch.Set(Handle, "stencilBuffer", ThreeValue.Encode(_stencilBuffer));
		}

		if (_isResolveDepthBufferWritten)
		{
			batch.Set(Handle, "resolveDepthBuffer", ThreeValue.Encode(_resolveDepthBuffer));
		}

		if (_isResolveStencilBufferWritten)
		{
			batch.Set(Handle, "resolveStencilBuffer", ThreeValue.Encode(_resolveStencilBuffer));
		}

		if (_isSamplesWritten)
		{
			batch.Set(Handle, "samples", ThreeValue.Encode(_samples));
		}

		if (_isMultiviewWritten)
		{
			batch.Set(Handle, "multiview", ThreeValue.Encode(_multiview));
		}

		if (_isUseArrayDepthTextureWritten)
		{
			batch.Set(Handle, "useArrayDepthTexture", ThreeValue.Encode(_useArrayDepthTexture));
		}

		if (_isDepthTextureWritten)
		{
			_depthTexture?.AttachTo(batch);
			batch.Set(Handle, "depthTexture", ThreeValue.Encode(_depthTexture));
		}
	}
}
