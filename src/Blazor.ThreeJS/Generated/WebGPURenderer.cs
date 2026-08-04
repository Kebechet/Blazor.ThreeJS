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
	private Info? _info;
	private NodeLibrary? _library;
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
	private bool _isInfoWritten;
	private bool _isLibraryWritten;
	private bool _isLightingWritten;
	private bool _isTransparentWritten;
	private bool _isOpaqueWritten;
	private bool _isInspectorWritten;
	private bool _isHighPrecisionWritten;

	/// <summary>Initializes a new <see cref="WebGPURenderer"/>.</summary>
	public WebGPURenderer()
	{
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
	/// Holds a series of statistical information about the GPU memory and the rendering process. Useful
	/// for debugging and monitoring. Writing it records a <c>info</c> property write once this object
	/// is attached; writing the value already held records nothing.
	/// </summary>
	public Info? Info
	{
		get { return _info; }
		set
		{
			if (ReferenceEquals(_info, value))
			{
				return;
			}

			_info = value;
			_isInfoWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("info", value);
		}
	}

	/// <summary>
	/// The node library defines how certain library objects like materials, lights or tone mapping
	/// functions are mapped to node types. This is required since although instances of classes like
	/// <c>MeshBasicMaterial</c> or <c>PointLight</c> can be part of the scene graph, they are
	/// internally represented as nodes for further processing. Writing it records a <c>library</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public NodeLibrary? Library
	{
		get { return _library; }
		set
		{
			if (ReferenceEquals(_library, value))
			{
				return;
			}

			_library = value;
			_isLibraryWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("library", value);
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
		if (Batch is not null)
		{
			scene.AttachTo(Batch);
		}

		if (Batch is not null)
		{
			camera.AttachTo(Batch);
		}

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

	/// <summary>Defines the scissor test.</summary>
	/// <param name="boolean">Whether the scissor test should be enabled or not.</param>
	public void SetScissorTest(bool boolean)
	{
		RecordCall("setScissorTest", boolean);
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
		if (Batch is not null && renderTarget is not null)
		{
			renderTarget.AttachTo(Batch);
		}

		RecordCall("setRenderTarget", renderTarget, activeCubeFace, activeMipmapLevel);
	}

	/// <summary>Sets the output render target for the renderer.</summary>
	/// <param name="renderTarget">The render target to set as the output target.</param>
	public void SetOutputRenderTarget(RenderTarget? renderTarget)
	{
		if (Batch is not null && renderTarget is not null)
		{
			renderTarget.AttachTo(Batch);
		}

		RecordCall("setOutputRenderTarget", renderTarget);
	}

	/// <summary>Initializes the given render target.</summary>
	/// <param name="renderTarget">The render target to intialize.</param>
	public void InitRenderTarget(RenderTarget renderTarget)
	{
		if (Batch is not null)
		{
			renderTarget.AttachTo(Batch);
		}

		RecordCall("initRenderTarget", renderTarget);
	}

	/// <summary>Copies the current bound framebuffer into the given texture.</summary>
	/// <param name="framebufferTexture">The texture.</param>
	public void CopyFramebufferToTexture(FramebufferTexture framebufferTexture)
	{
		if (Batch is not null)
		{
			framebufferTexture.AttachTo(Batch);
		}

		RecordCall("copyFramebufferToTexture", framebufferTexture);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.WebGPURenderer</c>, then replays every property written before
	/// this object was attached.
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

		if (_isInfoWritten)
		{
			batch.Set(Handle, "info", ThreeValue.Encode(_info));
		}

		if (_isLibraryWritten)
		{
			batch.Set(Handle, "library", ThreeValue.Encode(_library));
		}

		if (_isLightingWritten)
		{
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
			batch.Set(Handle, "inspector", ThreeValue.Encode(_inspector));
		}

		if (_isHighPrecisionWritten)
		{
			batch.Set(Handle, "highPrecision", ThreeValue.Encode(_highPrecision));
		}
	}
}
