// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.RenderTarget</c>.</summary>
public class RenderTarget : EventDispatcher
{
	private float? _width;
	private float? _height;
	private readonly RenderTargetOptions? _options;
	private float _depth;
	private bool _scissorTest = false;
	private Texture?[] _textures = [];
	private bool _depthBuffer = true;
	private bool _stencilBuffer = false;
	private bool _resolveDepthBuffer = true;
	private bool _resolveStencilBuffer = true;
	private float _samples = 0f;
	private bool _multiview = false;
	private bool _useArrayDepthTexture = false;
	private Texture? _texture;
	private DepthTexture? _depthTexture;
	private bool _isWidthWritten;
	private bool _isHeightWritten;
	private bool _isDepthWritten;
	private bool _isScissorWritten;
	private bool _isScissorTestWritten;
	private bool _isViewportWritten;
	private bool _isTexturesWritten;
	private bool _isDepthBufferWritten;
	private bool _isStencilBufferWritten;
	private bool _isResolveDepthBufferWritten;
	private bool _isResolveStencilBufferWritten;
	private bool _isSamplesWritten;
	private bool _isMultiviewWritten;
	private bool _isUseArrayDepthTextureWritten;
	private bool _isTextureWritten;
	private bool _isDepthTextureWritten;

	/// <summary>
	/// The <c>scissor</c> property of the JavaScript-side object. Mirrored as an instance this object
	/// owns: mutating it records a write of <c>scissor</c>.
	/// </summary>
	public Vector4 Scissor { get; }

	/// <summary>
	/// The <c>viewport</c> property of the JavaScript-side object. Mirrored as an instance this object
	/// owns: mutating it records a write of <c>viewport</c>.
	/// </summary>
	public Vector4 Viewport { get; }

	/// <summary>Initializes a new <see cref="RenderTarget"/>.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> constructor argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> constructor argument.</param>
	/// <param name="options">Value forwarded to the <c>options</c> constructor argument.</param>
	public RenderTarget(float? width = null, float? height = null, RenderTargetOptions? options = null)
	{
		_width = width;
		_height = height;
		_options = options;

		Scissor = new Vector4();
		Scissor.OnChange = () =>
		{
			_isScissorWritten = true;
			RecordSet("scissor", Scissor);
		};

		Viewport = new Vector4();
		Viewport.OnChange = () =>
		{
			_isViewportWritten = true;
			RecordSet("viewport", Viewport);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>RenderTarget</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal RenderTarget(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Scissor = new Vector4();
		Scissor.OnChange = () =>
		{
			_isScissorWritten = true;
			RecordSet("scissor", Scissor);
		};

		Viewport = new Vector4();
		Viewport.OnChange = () =>
		{
			_isViewportWritten = true;
			RecordSet("viewport", Viewport);
		};

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.RenderTarget</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "RenderTarget"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.RenderTarget</c>: width, height, options. An
	/// argument the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed
	/// when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_width),
				ThreeValue.OrUnspecified(_height),
				ThreeValue.OrUnspecified(_options)
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
	/// The <c>textures</c> property of the JavaScript-side object. Writing it records a <c>textures</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture?[] Textures
	{
		get { return _textures; }
		set
		{
			if (_textures == value)
			{
				return;
			}

			_textures = value;
			_isTexturesWritten = true;
			AttachEach(Batch, value);

			RecordSet("textures", value);
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
	/// The <c>texture</c> property of the JavaScript-side object. Writing it records a <c>texture</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? Texture
	{
		get { return _texture; }
		set
		{
			if (ReferenceEquals(_texture, value))
			{
				return;
			}

			_texture = value;
			_isTextureWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("texture", value);
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
	/// Reads <c>isRenderTarget</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isRenderTarget</c> held.
	/// </summary>
	/// <returns>The value <c>isRenderTarget</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsRenderTargetAsync()
	{
		return GetAsync<bool>("isRenderTarget");
	}

	/// <summary>
	/// Reads <c>clone</c> back from the JavaScript-side object. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<RenderTarget?> CloneAsync()
	{
		return RecordReadObject<RenderTarget>("clone", (adoptedBatch, adoptedHandle) => new RenderTarget(adoptedBatch, adoptedHandle));
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

		if (_isScissorWritten)
		{
			batch.Set(Handle, "scissor", ThreeValue.Encode(Scissor));
		}

		if (_isScissorTestWritten)
		{
			batch.Set(Handle, "scissorTest", ThreeValue.Encode(_scissorTest));
		}

		if (_isViewportWritten)
		{
			batch.Set(Handle, "viewport", ThreeValue.Encode(Viewport));
		}

		if (_isTexturesWritten)
		{
			AttachEach(batch, _textures);
			batch.Set(Handle, "textures", ThreeValue.Encode(_textures));
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

		if (_isTextureWritten)
		{
			_texture?.AttachTo(batch);
			batch.Set(Handle, "texture", ThreeValue.Encode(_texture));
		}

		if (_isDepthTextureWritten)
		{
			_depthTexture?.AttachTo(batch);
			batch.Set(Handle, "depthTexture", ThreeValue.Encode(_depthTexture));
		}
	}
}
