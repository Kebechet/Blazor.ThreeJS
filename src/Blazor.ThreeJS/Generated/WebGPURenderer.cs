// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.WebGPURenderer</c>.</summary>
public sealed class WebGPURenderer : ThreeObject
{
	private bool _autoClear = true;
	private bool _autoClearColor = true;
	private bool _autoClearDepth = true;
	private bool _autoClearStencil = true;
	private bool _alpha = true;
	private string _outputColorSpace = string.Empty;
	private ToneMapping _toneMapping = ToneMapping.NoToneMapping;
	private float _toneMappingExposure = 1f;
	private bool _sortObjects = true;
	private bool _depth = true;
	private bool _stencil = false;
	private Lighting? _lighting;
	private bool _transparent = true;
	private bool _opaque = true;
	private InspectorBase? _inspector;
	private bool _highPrecision;
	private bool _isAutoClearWritten;
	private bool _isAutoClearColorWritten;
	private bool _isAutoClearDepthWritten;
	private bool _isAutoClearStencilWritten;
	private bool _isAlphaWritten;
	private bool _isOutputColorSpaceWritten;
	private bool _isToneMappingWritten;
	private bool _isToneMappingExposureWritten;
	private bool _isSortObjectsWritten;
	private bool _isDepthWritten;
	private bool _isStencilWritten;
	private bool _isLightingWritten;
	private bool _isTransparentWritten;
	private bool _isOpaqueWritten;
	private bool _isInspectorWritten;
	private bool _isHighPrecisionWritten;

	/// <summary>Initializes a new <see cref="WebGPURenderer"/>.</summary>
	public WebGPURenderer()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>WebGPURenderer</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal WebGPURenderer(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WebGPURenderer</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WebGPURenderer"; }
	}

	/// <summary>
	/// Whether the renderer should automatically clear the current rendering target before execute a
	/// <c>render()</c> call. The target can be the canvas (default framebuffer) or the current bound
	/// render target (custom framebuffer). Writing it records a <c>autoClear</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool AutoClear
	{
		get { return _autoClear; }
		set
		{
			if (_autoClear == value)
			{
				return;
			}

			_autoClear = value;
			_isAutoClearWritten = true;
			RecordSet("autoClear", value);
		}
	}

	/// <summary>
	/// When <c>autoClear</c> is set to <c>true</c>, this property defines whether the renderer should
	/// clear the color buffer. Writing it records a <c>autoClearColor</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public bool AutoClearColor
	{
		get { return _autoClearColor; }
		set
		{
			if (_autoClearColor == value)
			{
				return;
			}

			_autoClearColor = value;
			_isAutoClearColorWritten = true;
			RecordSet("autoClearColor", value);
		}
	}

	/// <summary>
	/// When <c>autoClear</c> is set to <c>true</c>, this property defines whether the renderer should
	/// clear the depth buffer. Writing it records a <c>autoClearDepth</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public bool AutoClearDepth
	{
		get { return _autoClearDepth; }
		set
		{
			if (_autoClearDepth == value)
			{
				return;
			}

			_autoClearDepth = value;
			_isAutoClearDepthWritten = true;
			RecordSet("autoClearDepth", value);
		}
	}

	/// <summary>
	/// When <c>autoClear</c> is set to <c>true</c>, this property defines whether the renderer should
	/// clear the stencil buffer. Writing it records a <c>autoClearStencil</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public bool AutoClearStencil
	{
		get { return _autoClearStencil; }
		set
		{
			if (_autoClearStencil == value)
			{
				return;
			}

			_autoClearStencil = value;
			_isAutoClearStencilWritten = true;
			RecordSet("autoClearStencil", value);
		}
	}

	/// <summary>
	/// Whether the default framebuffer should be transparent or opaque. Writing it records a
	/// <c>alpha</c> property write once this object is attached; writing the value already held records
	/// nothing.
	/// </summary>
	public bool Alpha
	{
		get { return _alpha; }
		set
		{
			if (_alpha == value)
			{
				return;
			}

			_alpha = value;
			_isAlphaWritten = true;
			RecordSet("alpha", value);
		}
	}

	/// <summary>
	/// Defines the output color space of the renderer. Writing it records a <c>outputColorSpace</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public string OutputColorSpace
	{
		get { return _outputColorSpace; }
		set
		{
			if (_outputColorSpace == value)
			{
				return;
			}

			_outputColorSpace = value;
			_isOutputColorSpaceWritten = true;
			RecordSet("outputColorSpace", value);
		}
	}

	/// <summary>
	/// Defines the tone mapping of the renderer. Writing it records a <c>toneMapping</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public ToneMapping ToneMapping
	{
		get { return _toneMapping; }
		set
		{
			if (_toneMapping == value)
			{
				return;
			}

			_toneMapping = value;
			_isToneMappingWritten = true;
			RecordSet("toneMapping", value);
		}
	}

	/// <summary>
	/// Defines the tone mapping exposure. Writing it records a <c>toneMappingExposure</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float ToneMappingExposure
	{
		get { return _toneMappingExposure; }
		set
		{
			if (_toneMappingExposure == value)
			{
				return;
			}

			_toneMappingExposure = value;
			_isToneMappingExposureWritten = true;
			RecordSet("toneMappingExposure", value);
		}
	}

	/// <summary>
	/// Whether the renderer should sort its render lists or not. Note: Sorting is used to attempt to
	/// properly render objects that have some degree of transparency. By definition, sorting objects
	/// may not work in all cases. Depending on the needs of application, it may be necessary to turn
	/// off sorting and use other methods to deal with transparency rendering e.g. manually determining
	/// each object's rendering order. Writing it records a <c>sortObjects</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public bool SortObjects
	{
		get { return _sortObjects; }
		set
		{
			if (_sortObjects == value)
			{
				return;
			}

			_sortObjects = value;
			_isSortObjectsWritten = true;
			RecordSet("sortObjects", value);
		}
	}

	/// <summary>
	/// Whether the default framebuffer should have a depth buffer or not. Writing it records a
	/// <c>depth</c> property write once this object is attached; writing the value already held records
	/// nothing.
	/// </summary>
	public bool Depth
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
	/// Whether the default framebuffer should have a stencil buffer or not. Writing it records a
	/// <c>stencil</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool Stencil
	{
		get { return _stencil; }
		set
		{
			if (_stencil == value)
			{
				return;
			}

			_stencil = value;
			_isStencilWritten = true;
			RecordSet("stencil", value);
		}
	}

	/// <summary>
	/// A map-like data structure for managing lights. Writing it records a <c>lighting</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Lighting? Lighting
	{
		get { return _lighting; }
		set
		{
			if (ReferenceEquals(_lighting, value))
			{
				return;
			}

			_lighting = value;
			_isLightingWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("lighting", value);
		}
	}

	/// <summary>
	/// Whether the renderer should render transparent render objects or not. Writing it records a
	/// <c>transparent</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool Transparent
	{
		get { return _transparent; }
		set
		{
			if (_transparent == value)
			{
				return;
			}

			_transparent = value;
			_isTransparentWritten = true;
			RecordSet("transparent", value);
		}
	}

	/// <summary>
	/// Whether the renderer should render opaque render objects or not. Writing it records a
	/// <c>opaque</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool Opaque
	{
		get { return _opaque; }
		set
		{
			if (_opaque == value)
			{
				return;
			}

			_opaque = value;
			_isOpaqueWritten = true;
			RecordSet("opaque", value);
		}
	}

	/// <summary>
	/// The <c>inspector</c> property of the JavaScript-side object. Writing it records a
	/// <c>inspector</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public InspectorBase? Inspector
	{
		get { return _inspector; }
		set
		{
			if (ReferenceEquals(_inspector, value))
			{
				return;
			}

			_inspector = value;
			_isInspectorWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("inspector", value);
		}
	}

	/// <summary>
	/// Enables or disables high precision for model-view and normal-view matrices. When enabled, will
	/// use CPU 64-bit precision for higher precision instead of GPU 32-bit for higher performance.
	/// NOTE: 64-bit precision is not compatible with <c>InstancedMesh</c> and <c>SkinnedMesh</c>.
	/// Writing it records a <c>highPrecision</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public bool HighPrecision
	{
		get { return _highPrecision; }
		set
		{
			if (_highPrecision == value)
			{
				return;
			}

			_highPrecision = value;
			_isHighPrecisionWritten = true;
			RecordSet("highPrecision", value);
		}
	}

	/// <summary>
	/// Renders the scene or 3D object with the given camera. This method can only be called if the
	/// renderer has been initialized. When using <c>render()</c> inside an animation loop, it's
	/// guaranteed the renderer will be initialized. The animation loop must be defined with
	/// <c>Renderer#setAnimationLoop</c> though. For all other use cases (like when using on-demand
	/// rendering), you must call <c>Renderer#init</c> before rendering. The target of the method is the
	/// default framebuffer (meaning the canvas) or alternatively a render target when specified via
	/// <c>setRenderTarget()</c>.
	/// </summary>
	/// <param name="scene">The scene or 3D object to render.</param>
	/// <param name="camera">The camera to render the scene with.</param>
	public void Render(Object3D scene, Camera camera)
	{
		RecordCall("render", scene, camera);
	}

	/// <summary>Sets the given pixel ratio and resizes the canvas if necessary.</summary>
	/// <param name="value">The pixel ratio.</param>
	public void SetPixelRatio(float value = 1f)
	{
		RecordCall("setPixelRatio", value);
	}

	/// <summary>
	/// This method allows to define the drawing buffer size by specifying width, height and pixel ratio
	/// all at once. The size of the drawing buffer is computed with this formula:.
	/// </summary>
	/// <param name="width">The width in logical pixels.</param>
	/// <param name="height">The height in logical pixels.</param>
	/// <param name="pixelRatio">The pixel ratio.</param>
	public void SetDrawingBufferSize(float width, float height, float pixelRatio)
	{
		RecordCall("setDrawingBufferSize", width, height, pixelRatio);
	}

	/// <summary>Sets the size of the renderer.</summary>
	/// <param name="width">The width in logical pixels.</param>
	/// <param name="height">The height in logical pixels.</param>
	/// <param name="updateStyle">Whether to update the <c>style</c> attribute of the canvas or not.</param>
	public void SetSize(float width, float height, bool updateStyle = true)
	{
		RecordCall("setSize", width, height, updateStyle);
	}

	/// <summary>Defines the scissor rectangle.</summary>
	/// <param name="x">Value forwarded to the <c>x</c> argument.</param>
	public void SetScissor(Vector4 x)
	{
		RecordCall("setScissor", x);
	}

	/// <summary>Defines the scissor test.</summary>
	/// <param name="boolean">Whether the scissor test should be enabled or not.</param>
	public void SetScissorTest(bool boolean)
	{
		RecordCall("setScissorTest", boolean);
	}

	/// <summary>Defines the viewport.</summary>
	/// <param name="x">Value forwarded to the <c>x</c> argument.</param>
	public void SetViewport(Vector4 x)
	{
		RecordCall("setViewport", x);
	}

	/// <summary>Defines the clear color and optionally the clear alpha.</summary>
	/// <param name="color">The clear color.</param>
	/// <param name="alpha">The clear alpha.</param>
	public void SetClearColor(Color color, float alpha = 1f)
	{
		RecordCall("setClearColor", color, alpha);
	}

	/// <summary>Defines the clear alpha.</summary>
	/// <param name="alpha">The clear alpha.</param>
	public void SetClearAlpha(float alpha)
	{
		RecordCall("setClearAlpha", alpha);
	}

	/// <summary>Defines the clear depth.</summary>
	/// <param name="depth">The clear depth.</param>
	public void SetClearDepth(float depth)
	{
		RecordCall("setClearDepth", depth);
	}

	/// <summary>Defines the clear stencil.</summary>
	/// <param name="stencil">The clear stencil.</param>
	public void SetClearStencil(float stencil)
	{
		RecordCall("setClearStencil", stencil);
	}

	/// <summary>Performs a manual clear operation. This method ignores <c>autoClear</c> properties.</summary>
	/// <param name="color">Whether the color buffer should be cleared or not.</param>
	/// <param name="depth">Whether the depth buffer should be cleared or not.</param>
	/// <param name="stencil">Whether the stencil buffer should be cleared or not.</param>
	public void Clear(bool color = true, bool depth = true, bool stencil = true)
	{
		RecordCall("clear", color, depth, stencil);
	}

	/// <summary>
	/// Performs a manual clear operation of the color buffer. This method ignores <c>autoClear</c>
	/// properties.
	/// </summary>
	public void ClearColor()
	{
		RecordCall("clearColor");
	}

	/// <summary>
	/// Performs a manual clear operation of the depth buffer. This method ignores <c>autoClear</c>
	/// properties.
	/// </summary>
	public void ClearDepth()
	{
		RecordCall("clearDepth");
	}

	/// <summary>
	/// Performs a manual clear operation of the stencil buffer. This method ignores <c>autoClear</c>
	/// properties.
	/// </summary>
	public void ClearStencil()
	{
		RecordCall("clearStencil");
	}

	/// <summary>
	/// Frees all internal resources of the renderer. Call this method if the renderer is no longer in
	/// use by your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Sets the given render target. Calling this method means the renderer does not target the default
	/// framebuffer (meaning the canvas) anymore but a custom framebuffer. Use <c>null</c> as the first
	/// argument to reset the state.
	/// </summary>
	/// <param name="renderTarget">The render target to set.</param>
	/// <param name="activeCubeFace">The active cube face.</param>
	/// <param name="activeMipmapLevel">The active mipmap level.</param>
	public void SetRenderTarget(RenderTarget? renderTarget, float activeCubeFace = 0f, float activeMipmapLevel = 0f)
	{
		RecordCall("setRenderTarget", renderTarget, activeCubeFace, activeMipmapLevel);
	}

	/// <summary>Sets the output render target for the renderer.</summary>
	/// <param name="renderTarget">The render target to set as the output target.</param>
	public void SetOutputRenderTarget(RenderTarget? renderTarget)
	{
		RecordCall("setOutputRenderTarget", renderTarget);
	}

	/// <summary>
	/// Initializes the given texture. Useful for preloading a texture rather than waiting until first
	/// render (which can cause noticeable lags due to decode and GPU upload overhead). This method can
	/// only be used if the renderer has been initialized.
	/// </summary>
	/// <param name="texture">The texture.</param>
	public void InitTexture(Texture texture)
	{
		RecordCall("initTexture", texture);
	}

	/// <summary>Initializes the given render target.</summary>
	/// <param name="renderTarget">The render target to intialize.</param>
	public void InitRenderTarget(RenderTarget renderTarget)
	{
		RecordCall("initRenderTarget", renderTarget);
	}

	/// <summary>Copies the current bound framebuffer into the given texture.</summary>
	/// <param name="framebufferTexture">The texture.</param>
	public void CopyFramebufferToTexture(FramebufferTexture framebufferTexture)
	{
		RecordCall("copyFramebufferToTexture", framebufferTexture);
	}

	/// <summary>Copies data of the given source texture into a destination texture.</summary>
	/// <param name="srcTexture">The source texture.</param>
	/// <param name="dstTexture">The destination texture.</param>
	public void CopyTextureToTexture(Texture srcTexture, Texture dstTexture)
	{
		RecordCall("copyTextureToTexture", srcTexture, dstTexture);
	}

	/// <summary>
	/// Reads <c>isWebGPURenderer</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isWebGPURenderer</c> held.
	/// </summary>
	/// <returns>The value <c>isWebGPURenderer</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsWebGPURendererAsync()
	{
		return GetAsync<bool>("isWebGPURenderer");
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isRenderer</c> held.
	/// </summary>
	/// <returns>The value <c>isRenderer</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsRendererAsync()
	{
		return GetAsync<bool>("isRenderer");
	}

	/// <summary>
	/// Whether logarithmic depth buffer is enabled or not. Read-only in three.js, so it is read on
	/// demand rather than mirrored: records a get op, sends it behind every write already pending, and
	/// completes with the value <c>logarithmicDepthBuffer</c> held.
	/// </summary>
	/// <returns>The value <c>logarithmicDepthBuffer</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> LogarithmicDepthBufferAsync()
	{
		return GetAsync<bool>("logarithmicDepthBuffer");
	}

	/// <summary>
	/// Whether reversed depth buffer is enabled or not. Read-only in three.js, so it is read on demand
	/// rather than mirrored: records a get op, sends it behind every write already pending, and
	/// completes with the value <c>reversedDepthBuffer</c> held.
	/// </summary>
	/// <returns>The value <c>reversedDepthBuffer</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> ReversedDepthBufferAsync()
	{
		return GetAsync<bool>("reversedDepthBuffer");
	}

	/// <summary>
	/// The coordinate system of the renderer. The value of this property depends on the selected
	/// backend. Either <c>THREE.WebGLCoordinateSystem</c> or <c>THREE.WebGPUCoordinateSystem</c>.
	/// Read-only in three.js, so it is read on demand rather than mirrored: records a get op, sends it
	/// behind every write already pending, and completes with the value <c>coordinateSystem</c> held.
	/// </summary>
	/// <returns>The value <c>coordinateSystem</c> held, once the JavaScript side has answered.</returns>
	public Task<CoordinateSystem> CoordinateSystemAsync()
	{
		return GetAsync<CoordinateSystem>("coordinateSystem");
	}

	/// <summary>
	/// Returns whether the renderer has been initialized or not. Read-only in three.js, so it is read
	/// on demand rather than mirrored: records a get op, sends it behind every write already pending,
	/// and completes with the value <c>initialized</c> held.
	/// </summary>
	/// <returns>The value <c>initialized</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> InitializedAsync()
	{
		return GetAsync<bool>("initialized");
	}

	/// <summary>
	/// Returns <c>true</c> if a framebuffer target is needed to perform tone mapping or color space
	/// conversion. If this is the case, the renderer allocates an internal render target for that
	/// purpose. Read-only in three.js, so it is read on demand rather than mirrored: records a get op,
	/// sends it behind every write already pending, and completes with the value
	/// <c>needsFrameBufferTarget</c> held.
	/// </summary>
	/// <returns>The value <c>needsFrameBufferTarget</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> NeedsFrameBufferTargetAsync()
	{
		return GetAsync<bool>("needsFrameBufferTarget");
	}

	/// <summary>
	/// The number of samples used for multi-sample anti-aliasing (MSAA). Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>samples</c> held.
	/// </summary>
	/// <returns>The value <c>samples</c> held, once the JavaScript side has answered.</returns>
	public Task<float> SamplesAsync()
	{
		return GetAsync<float>("samples");
	}

	/// <summary>
	/// The current number of samples used for multi-sample anti-aliasing (MSAA). When rendering to a
	/// custom render target, the number of samples of that render target is used. If the renderer needs
	/// an internal framebuffer target for tone mapping or color space conversion, the number of samples
	/// is set to 0. Read-only in three.js, so it is read on demand rather than mirrored: records a get
	/// op, sends it behind every write already pending, and completes with the value
	/// <c>currentSamples</c> held.
	/// </summary>
	/// <returns>The value <c>currentSamples</c> held, once the JavaScript side has answered.</returns>
	public Task<float> CurrentSamplesAsync()
	{
		return GetAsync<float>("currentSamples");
	}

	/// <summary>
	/// The current tone mapping of the renderer. When not producing screen output, the tone mapping is
	/// always <c>NoToneMapping</c>. Read-only in three.js, so it is read on demand rather than
	/// mirrored: records a get op, sends it behind every write already pending, and completes with the
	/// value <c>currentToneMapping</c> held.
	/// </summary>
	/// <returns>The value <c>currentToneMapping</c> held, once the JavaScript side has answered.</returns>
	public Task<ToneMapping> CurrentToneMappingAsync()
	{
		return GetAsync<ToneMapping>("currentToneMapping");
	}

	/// <summary>
	/// The current color space of the renderer. When not producing screen output, the color space is
	/// always the working color space. Read-only in three.js, so it is read on demand rather than
	/// mirrored: records a get op, sends it behind every write already pending, and completes with the
	/// value <c>currentColorSpace</c> held.
	/// </summary>
	/// <returns>The value <c>currentColorSpace</c> held, once the JavaScript side has answered.</returns>
	public Task<string> CurrentColorSpaceAsync()
	{
		return GetAsync<string>("currentColorSpace");
	}

	/// <summary>
	/// Returns <c>true</c> if the rendering settings are set to screen output. Read-only in three.js,
	/// so it is read on demand rather than mirrored: records a get op, sends it behind every write
	/// already pending, and completes with the value <c>isOutputTarget</c> held.
	/// </summary>
	/// <returns>The value <c>isOutputTarget</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsOutputTargetAsync()
	{
		return GetAsync<bool>("isOutputTarget");
	}

	/// <summary>
	/// Initializes the renderer so it is ready for usage. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>init</c> returned.
	/// </summary>
	/// <returns>The value <c>init</c> returned, once the JavaScript side has answered.</returns>
	public Task<WebGPURenderer?> InitAsync()
	{
		return RecordReadObject<WebGPURenderer>("init", (adoptedBatch, adoptedHandle) => new WebGPURenderer(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Compiles all materials in the given scene. This can be useful to avoid a phenomenon which is
	/// called "shader compilation stutter", which occurs when rendering an object with a new shader for
	/// the first time. If you want to add a 3D object to an existing scene, use the third optional
	/// parameter for applying the target scene. Note that the (target) scene's lighting and environment
	/// must be configured before calling this method. Answers nothing, and is awaited for when rather
	/// than for what: records a read op, sends it behind every write already pending, and completes
	/// once the promise <c>compileAsync</c> returned has settled.
	/// </summary>
	/// <param name="scene">The scene or 3D object to precompile.</param>
	/// <param name="camera">The camera that is used to render the scene.</param>
	/// <param name="targetScene">
	/// If the first argument is a 3D object, this parameter must represent the scene the 3D object is
	/// going to be added.
	/// </param>
	/// <returns>A task that completes once <c>compileAsync</c> has finished.</returns>
	public Task CompileAsync(Object3D scene, Camera camera, Scene? targetScene)
	{
		return RecordRead<object?>("compileAsync", scene, camera, targetScene);
	}

	/// <summary>
	/// Renders the scene in an async fashion. Answers nothing, and is awaited for when rather than for
	/// what: records a read op, sends it behind every write already pending, and completes once the
	/// promise <c>renderAsync</c> returned has settled.
	/// </summary>
	/// <param name="scene">The scene or 3D object to render.</param>
	/// <param name="camera">The camera.</param>
	/// <returns>A task that completes once <c>renderAsync</c> has finished.</returns>
	public Task RenderAsync(Object3D scene, Camera camera)
	{
		return RecordRead<object?>("renderAsync", scene, camera);
	}

	/// <summary>
	/// Can be used to synchronize CPU operations with GPU tasks. So when this method is called, the CPU
	/// waits for the GPU to complete its operation (e.g. a compute task). Answers nothing, and is
	/// awaited for when rather than for what: records a read op, sends it behind every write already
	/// pending, and completes once the promise <c>waitForGPU</c> returned has settled.
	/// </summary>
	/// <returns>A task that completes once <c>waitForGPU</c> has finished.</returns>
	public Task WaitForGPUAsync()
	{
		return RecordRead<object?>("waitForGPU");
	}

	/// <summary>
	/// Returns the output buffer type. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>getOutputBufferType</c> returned.
	/// </summary>
	/// <returns>The value <c>getOutputBufferType</c> returned, once the JavaScript side has answered.</returns>
	public Task<TextureDataType> GetOutputBufferTypeAsync()
	{
		return RecordRead<TextureDataType>("getOutputBufferType");
	}

	/// <summary>
	/// Returns the output buffer type. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>getColorBufferType</c> returned.
	/// </summary>
	/// <returns>The value <c>getColorBufferType</c> returned, once the JavaScript side has answered.</returns>
	public Task<TextureDataType> GetColorBufferTypeAsync()
	{
		return RecordRead<TextureDataType>("getColorBufferType");
	}

	/// <summary>
	/// Returns the maximum available anisotropy for texture filtering. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>getMaxAnisotropy</c> returned.
	/// </summary>
	/// <returns>The value <c>getMaxAnisotropy</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetMaxAnisotropyAsync()
	{
		return RecordRead<float>("getMaxAnisotropy");
	}

	/// <summary>
	/// Returns the active cube face. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>getActiveCubeFace</c> returned.
	/// </summary>
	/// <returns>The value <c>getActiveCubeFace</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetActiveCubeFaceAsync()
	{
		return RecordRead<float>("getActiveCubeFace");
	}

	/// <summary>
	/// Returns the active mipmap level. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>getActiveMipmapLevel</c> returned.
	/// </summary>
	/// <returns>The value <c>getActiveMipmapLevel</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetActiveMipmapLevelAsync()
	{
		return RecordRead<float>("getActiveMipmapLevel");
	}

	/// <summary>
	/// Returns the pixel ratio. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getPixelRatio</c> returned.
	/// </summary>
	/// <returns>The value <c>getPixelRatio</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetPixelRatioAsync()
	{
		return RecordRead<float>("getPixelRatio");
	}

	/// <summary>
	/// Returns the drawing buffer size in physical pixels. This method honors the pixel ratio. Records
	/// a read op, sends it behind every write already pending, and completes with what
	/// <c>getDrawingBufferSize</c> returned.
	/// </summary>
	/// <param name="target">The method writes the result in this target object.</param>
	/// <returns>The value <c>getDrawingBufferSize</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector2> GetDrawingBufferSizeAsync(Vector2 target)
	{
		return RecordRead<Vector2>("getDrawingBufferSize", target);
	}

	/// <summary>
	/// Returns the renderer's size in logical pixels. This method does not honor the pixel ratio.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>getSize</c> returned.
	/// </summary>
	/// <param name="target">The method writes the result in this target object.</param>
	/// <returns>The value <c>getSize</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector2> GetSizeAsync(Vector2 target)
	{
		return RecordRead<Vector2>("getSize", target);
	}

	/// <summary>
	/// Returns the scissor rectangle. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>getScissor</c> returned.
	/// </summary>
	/// <param name="target">The method writes the result in this target object.</param>
	/// <returns>The value <c>getScissor</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector4> GetScissorAsync(Vector4 target)
	{
		return RecordRead<Vector4>("getScissor", target);
	}

	/// <summary>
	/// Returns the scissor test value. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>getScissorTest</c> returned.
	/// </summary>
	/// <returns>The value <c>getScissorTest</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> GetScissorTestAsync()
	{
		return RecordRead<bool>("getScissorTest");
	}

	/// <summary>
	/// Returns the viewport definition. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>getViewport</c> returned.
	/// </summary>
	/// <param name="target">The method writes the result in this target object.</param>
	/// <returns>The value <c>getViewport</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector4> GetViewportAsync(Vector4 target)
	{
		return RecordRead<Vector4>("getViewport", target);
	}

	/// <summary>
	/// Returns the clear color. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getClearColor</c> returned.
	/// </summary>
	/// <param name="target">The method writes the result in this target object.</param>
	/// <returns>The value <c>getClearColor</c> returned, once the JavaScript side has answered.</returns>
	public Task<Color> GetClearColorAsync(Color target)
	{
		return RecordRead<Color>("getClearColor", target);
	}

	/// <summary>
	/// Returns the clear alpha. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getClearAlpha</c> returned.
	/// </summary>
	/// <returns>The value <c>getClearAlpha</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetClearAlphaAsync()
	{
		return RecordRead<float>("getClearAlpha");
	}

	/// <summary>
	/// Returns the clear depth. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getClearDepth</c> returned.
	/// </summary>
	/// <returns>The value <c>getClearDepth</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetClearDepthAsync()
	{
		return RecordRead<float>("getClearDepth");
	}

	/// <summary>
	/// Returns the clear stencil. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getClearStencil</c> returned.
	/// </summary>
	/// <returns>The value <c>getClearStencil</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetClearStencilAsync()
	{
		return RecordRead<float>("getClearStencil");
	}

	/// <summary>
	/// This method performs an occlusion query for the given 3D object. It returns <c>true</c> if the
	/// given 3D object is fully occluded by other 3D objects in the scene. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>isOccluded</c> returned.
	/// </summary>
	/// <param name="object">The 3D object to test.</param>
	/// <returns>The value <c>isOccluded</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> IsOccludedAsync(Object3D @object)
	{
		return RecordRead<bool>("isOccluded", @object);
	}

	/// <summary>
	/// Async version of <c>Renderer#clear</c>. Answers nothing, and is awaited for when rather than for
	/// what: records a read op, sends it behind every write already pending, and completes once the
	/// promise <c>clearAsync</c> returned has settled.
	/// </summary>
	/// <param name="color">Whether the color buffer should be cleared or not.</param>
	/// <param name="depth">Whether the depth buffer should be cleared or not.</param>
	/// <param name="stencil">Whether the stencil buffer should be cleared or not.</param>
	/// <returns>A task that completes once <c>clearAsync</c> has finished.</returns>
	public Task ClearAsync(bool color = true, bool depth = true, bool stencil = true)
	{
		return RecordRead<object?>("clearAsync", color, depth, stencil);
	}

	/// <summary>
	/// Async version of <c>Renderer#clearColor</c>. Answers nothing, and is awaited for when rather
	/// than for what: records a read op, sends it behind every write already pending, and completes
	/// once the promise <c>clearColorAsync</c> returned has settled.
	/// </summary>
	/// <returns>A task that completes once <c>clearColorAsync</c> has finished.</returns>
	public Task ClearColorAsync()
	{
		return RecordRead<object?>("clearColorAsync");
	}

	/// <summary>
	/// Async version of <c>Renderer#clearDepth</c>. Answers nothing, and is awaited for when rather
	/// than for what: records a read op, sends it behind every write already pending, and completes
	/// once the promise <c>clearDepthAsync</c> returned has settled.
	/// </summary>
	/// <returns>A task that completes once <c>clearDepthAsync</c> has finished.</returns>
	public Task ClearDepthAsync()
	{
		return RecordRead<object?>("clearDepthAsync");
	}

	/// <summary>
	/// Async version of <c>Renderer#clearStencil</c>. Answers nothing, and is awaited for when rather
	/// than for what: records a read op, sends it behind every write already pending, and completes
	/// once the promise <c>clearStencilAsync</c> returned has settled.
	/// </summary>
	/// <returns>A task that completes once <c>clearStencilAsync</c> has finished.</returns>
	public Task ClearStencilAsync()
	{
		return RecordRead<object?>("clearStencilAsync");
	}

	/// <summary>
	/// Returns the current render target. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getRenderTarget</c> returned.
	/// </summary>
	/// <returns>The value <c>getRenderTarget</c> returned, once the JavaScript side has answered.</returns>
	public Task<RenderTarget?> GetRenderTargetAsync()
	{
		return RecordReadObject<RenderTarget>("getRenderTarget", (adoptedBatch, adoptedHandle) => new RenderTarget(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns the current output target. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getOutputRenderTarget</c> returned.
	/// </summary>
	/// <returns>The value <c>getOutputRenderTarget</c> returned, once the JavaScript side has answered.</returns>
	public Task<RenderTarget?> GetOutputRenderTargetAsync()
	{
		return RecordReadObject<RenderTarget>("getOutputRenderTarget", (adoptedBatch, adoptedHandle) => new RenderTarget(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns the current canvas target. Answers with a three.js object no generated class mirrors:
	/// records a read op, sends it behind every write already pending, and completes with what
	/// <c>getCanvasTarget</c> returned, under its own handle, as an untyped <see cref="Primitive"/>.
	/// The mirror learns nothing from it — its members are reached by their three.js names, and nothing
	/// here checks them.
	/// </summary>
	/// <returns>
	/// The object <c>getCanvasTarget</c> returned, under its own handle, or <see langword="null"/> when
	/// it returned none.
	/// </returns>
	public Task<Primitive?> GetCanvasTargetAsync()
	{
		return CallObjectAsync("getCanvasTarget");
	}

	/// <summary>
	/// Checks if the given feature is supported by the selected backend. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>hasFeatureAsync</c> returned.
	/// </summary>
	/// <param name="name">The feature's name.</param>
	/// <returns>The value <c>hasFeatureAsync</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> HasFeatureAsyncAsync(string name)
	{
		return RecordRead<bool>("hasFeatureAsync", name);
	}

	/// <summary>
	/// Checks if the given feature is supported by the selected backend. If the renderer has not been
	/// initialized, this method always returns <c>false</c>. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>hasFeature</c> returned.
	/// </summary>
	/// <param name="name">The feature's name.</param>
	/// <returns>The value <c>hasFeature</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> HasFeatureAsync(string name)
	{
		return RecordRead<bool>("hasFeature", name);
	}

	/// <summary>
	/// Returns <c>true</c> when the renderer has been initialized. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>hasInitialized</c> returned.
	/// </summary>
	/// <returns>The value <c>hasInitialized</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> HasInitializedAsync()
	{
		return RecordRead<bool>("hasInitialized");
	}

	/// <summary>
	/// Initializes the given textures. Useful for preloading a texture rather than waiting until first
	/// render (which can cause noticeable lags due to decode and GPU upload overhead). Answers nothing,
	/// and is awaited for when rather than for what: records a read op, sends it behind every write
	/// already pending, and completes once the promise <c>initTextureAsync</c> returned has settled.
	/// </summary>
	/// <param name="texture">The texture.</param>
	/// <returns>A task that completes once <c>initTextureAsync</c> has finished.</returns>
	public Task InitTextureAsync(Texture texture)
	{
		return RecordRead<object?>("initTextureAsync", texture);
	}

	/// <summary>
	/// Reads pixel data from the given render target. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>readRenderTargetPixelsAsync</c> returned.
	/// </summary>
	/// <param name="renderTarget">The render target to read from.</param>
	/// <param name="x">The <c>x</c> coordinate of the copy region's origin.</param>
	/// <param name="y">The <c>y</c> coordinate of the copy region's origin.</param>
	/// <param name="width">The width of the copy region.</param>
	/// <param name="height">The height of the copy region.</param>
	/// <param name="textureIndex">The texture index of a MRT render target.</param>
	/// <param name="faceIndex">The active cube face index.</param>
	/// <returns>
	/// The value <c>readRenderTargetPixelsAsync</c> returned, once the JavaScript side has answered.
	/// </returns>
	public Task<TypedArray> ReadRenderTargetPixelsAsync(
		RenderTarget renderTarget,
		float x,
		float y,
		float width,
		float height,
		int textureIndex = 0,
		int faceIndex = 0)
	{
		return RecordRead<TypedArray>("readRenderTargetPixelsAsync", renderTarget, x, y, width, height, textureIndex, faceIndex);
	}

	/// <summary>
	/// Checks if the given compatibility is supported by the selected backend. Records a read op, sends
	/// it behind every write already pending, and completes with what <c>hasCompatibility</c> returned.
	/// </summary>
	/// <param name="name">The compatibility's name.</param>
	/// <returns>The value <c>hasCompatibility</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> HasCompatibilityAsync(string name)
	{
		return RecordRead<bool>("hasCompatibility", name);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.WebGPURenderer</c>, then replays every property written before
	/// this object was attached. A replayed value that is itself a mirrored object is attached first,
	/// so its create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isAutoClearWritten)
		{
			batch.Set(Handle, "autoClear", ThreeValue.Encode(_autoClear));
		}

		if (_isAutoClearColorWritten)
		{
			batch.Set(Handle, "autoClearColor", ThreeValue.Encode(_autoClearColor));
		}

		if (_isAutoClearDepthWritten)
		{
			batch.Set(Handle, "autoClearDepth", ThreeValue.Encode(_autoClearDepth));
		}

		if (_isAutoClearStencilWritten)
		{
			batch.Set(Handle, "autoClearStencil", ThreeValue.Encode(_autoClearStencil));
		}

		if (_isAlphaWritten)
		{
			batch.Set(Handle, "alpha", ThreeValue.Encode(_alpha));
		}

		if (_isOutputColorSpaceWritten)
		{
			batch.Set(Handle, "outputColorSpace", ThreeValue.Encode(_outputColorSpace));
		}

		if (_isToneMappingWritten)
		{
			batch.Set(Handle, "toneMapping", ThreeValue.Encode(_toneMapping));
		}

		if (_isToneMappingExposureWritten)
		{
			batch.Set(Handle, "toneMappingExposure", ThreeValue.Encode(_toneMappingExposure));
		}

		if (_isSortObjectsWritten)
		{
			batch.Set(Handle, "sortObjects", ThreeValue.Encode(_sortObjects));
		}

		if (_isDepthWritten)
		{
			batch.Set(Handle, "depth", ThreeValue.Encode(_depth));
		}

		if (_isStencilWritten)
		{
			batch.Set(Handle, "stencil", ThreeValue.Encode(_stencil));
		}

		if (_isLightingWritten)
		{
			_lighting?.AttachTo(batch);
			batch.Set(Handle, "lighting", ThreeValue.Encode(_lighting));
		}

		if (_isTransparentWritten)
		{
			batch.Set(Handle, "transparent", ThreeValue.Encode(_transparent));
		}

		if (_isOpaqueWritten)
		{
			batch.Set(Handle, "opaque", ThreeValue.Encode(_opaque));
		}

		if (_isInspectorWritten)
		{
			_inspector?.AttachTo(batch);
			batch.Set(Handle, "inspector", ThreeValue.Encode(_inspector));
		}

		if (_isHighPrecisionWritten)
		{
			batch.Set(Handle, "highPrecision", ThreeValue.Encode(_highPrecision));
		}
	}
}
