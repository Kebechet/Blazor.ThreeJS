// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The WebGL renderer displays your beautifully crafted scenes using WebGL, if your device supports
/// it. This renderer has way better performance than CanvasRenderer. see
/// <see href="https://github.com/mrdoob/three.js/blob/master/src/renderers/WebGLRenderer.js">src/renderers/WebGLRenderer.js</see>.
/// The JavaScript-side <c>THREE.WebGLRenderer</c>.
/// </summary>
public sealed class WebGLRenderer : ThreeObject
{
	private bool _autoClear = true;
	private bool _autoClearColor = true;
	private bool _autoClearDepth = true;
	private bool _autoClearStencil = true;
	private bool _sortObjects = true;
	private bool _localClippingEnabled = false;
	private string _outputColorSpace = string.Empty;
	private ToneMapping _toneMapping;
	private float _toneMappingExposure = 1f;
	private float _transmissionResolutionScale;
	private WebGLCapabilities? _capabilities;
	private bool _isAutoClearWritten;
	private bool _isAutoClearColorWritten;
	private bool _isAutoClearDepthWritten;
	private bool _isAutoClearStencilWritten;
	private bool _isSortObjectsWritten;
	private bool _isLocalClippingEnabledWritten;
	private bool _isOutputColorSpaceWritten;
	private bool _isToneMappingWritten;
	private bool _isToneMappingExposureWritten;
	private bool _isTransmissionResolutionScaleWritten;
	private bool _isCapabilitiesWritten;

	/// <summary>
	/// parameters is an optional object with properties defining the renderer's behavior. The
	/// constructor also accepts no parameters at all. In all cases, it will assume sane defaults when
	/// parameters are missing.
	/// </summary>
	public WebGLRenderer()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WebGLRenderer</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WebGLRenderer"; }
	}

	/// <summary>
	/// Defines whether the renderer should automatically clear its output before rendering. Writing it
	/// records a <c>autoClear</c> property write once this object is attached; writing the value
	/// already held records nothing.
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
	/// If autoClear is true, defines whether the renderer should clear the color buffer. Default is
	/// true. Writing it records a <c>autoClearColor</c> property write once this object is attached;
	/// writing the value already held records nothing.
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
	/// If autoClear is true, defines whether the renderer should clear the depth buffer. Default is
	/// true. Writing it records a <c>autoClearDepth</c> property write once this object is attached;
	/// writing the value already held records nothing.
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
	/// If autoClear is true, defines whether the renderer should clear the stencil buffer. Default is
	/// true. Writing it records a <c>autoClearStencil</c> property write once this object is attached;
	/// writing the value already held records nothing.
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
	/// Defines whether the renderer should sort objects. Default is true. Writing it records a
	/// <c>sortObjects</c> property write once this object is attached; writing the value already held
	/// records nothing.
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
	/// The <c>localClippingEnabled</c> property of the JavaScript-side object. Writing it records a
	/// <c>localClippingEnabled</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public bool LocalClippingEnabled
	{
		get { return _localClippingEnabled; }
		set
		{
			if (_localClippingEnabled == value)
			{
				return;
			}

			_localClippingEnabled = value;
			_isLocalClippingEnabledWritten = true;
			RecordSet("localClippingEnabled", value);
		}
	}

	/// <summary>
	/// Color space used for output to HTMLCanvasElement. Supported values are <c>SRGBColorSpace</c> and
	/// <c>LinearSRGBColorSpace</c>. Writing it records a <c>outputColorSpace</c> property write once
	/// this object is attached; writing the value already held records nothing.
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
	/// The <c>toneMapping</c> property of the JavaScript-side object. Writing it records a
	/// <c>toneMapping</c> property write once this object is attached; writing the value already held
	/// records nothing.
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
	/// The <c>toneMappingExposure</c> property of the JavaScript-side object. Writing it records a
	/// <c>toneMappingExposure</c> property write once this object is attached; writing the value
	/// already held records nothing.
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
	/// The normalized resolution scale for the transmission render target, measured in percentage of
	/// viewport dimensions. Lowering this value can result in significant improvements to
	/// <see cref="MeshPhysicalMaterial"/> transmission performance. Default is <c>1</c>. Writing it
	/// records a <c>transmissionResolutionScale</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public float TransmissionResolutionScale
	{
		get { return _transmissionResolutionScale; }
		set
		{
			if (_transmissionResolutionScale == value)
			{
				return;
			}

			_transmissionResolutionScale = value;
			_isTransmissionResolutionScaleWritten = true;
			RecordSet("transmissionResolutionScale", value);
		}
	}

	/// <summary>
	/// The <c>capabilities</c> property of the JavaScript-side object. Writing it records a
	/// <c>capabilities</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public WebGLCapabilities? Capabilities
	{
		get { return _capabilities; }
		set
		{
			if (ReferenceEquals(_capabilities, value))
			{
				return;
			}

			_capabilities = value;
			_isCapabilitiesWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("capabilities", value);
		}
	}

	/// <summary>Records a call to <c>forceContextLoss</c> on the JavaScript-side object.</summary>
	public void ForceContextLoss()
	{
		RecordCall("forceContextLoss");
	}

	/// <summary>Records a call to <c>forceContextRestore</c> on the JavaScript-side object.</summary>
	public void ForceContextRestore()
	{
		RecordCall("forceContextRestore");
	}

	/// <summary>Records a call to <c>setPixelRatio</c> on the JavaScript-side object.</summary>
	/// <param name="value">Value forwarded to the <c>value</c> argument.</param>
	public void SetPixelRatio(float value)
	{
		RecordCall("setPixelRatio", value);
	}

	/// <summary>
	/// Resizes the output canvas to (width, height), and also sets the viewport to fit that size,
	/// starting in (0, 0).
	/// </summary>
	/// <param name="width">Value forwarded to the <c>width</c> argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> argument.</param>
	/// <param name="updateStyle">Value forwarded to the <c>updateStyle</c> argument.</param>
	public void SetSize(float width, float height, bool updateStyle)
	{
		RecordCall("setSize", width, height, updateStyle);
	}

	/// <summary>Records a call to <c>setDrawingBufferSize</c> on the JavaScript-side object.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> argument.</param>
	/// <param name="pixelRatio">Value forwarded to the <c>pixelRatio</c> argument.</param>
	public void SetDrawingBufferSize(float width, float height, float pixelRatio)
	{
		RecordCall("setDrawingBufferSize", width, height, pixelRatio);
	}

	/// <summary>
	/// Enable the scissor test. When this is enabled, only the pixels within the defined scissor area
	/// will be affected by further renderer actions.
	/// </summary>
	/// <param name="enable">Value forwarded to the <c>enable</c> argument.</param>
	public void SetScissorTest(bool enable)
	{
		RecordCall("setScissorTest", enable);
	}

	/// <summary>Sets the clear color, using color for the color and alpha for the opacity.</summary>
	/// <param name="color">Value forwarded to the <c>color</c> argument.</param>
	/// <param name="alpha">Value forwarded to the <c>alpha</c> argument.</param>
	public void SetClearColor(Color color, float alpha)
	{
		RecordCall("setClearColor", color, alpha);
	}

	/// <summary>Records a call to <c>setClearAlpha</c> on the JavaScript-side object.</summary>
	/// <param name="alpha">Value forwarded to the <c>alpha</c> argument.</param>
	public void SetClearAlpha(float alpha)
	{
		RecordCall("setClearAlpha", alpha);
	}

	/// <summary>
	/// Tells the renderer to clear its color, depth or stencil drawing buffer(s). Arguments default to
	/// true.
	/// </summary>
	/// <param name="color">Value forwarded to the <c>color</c> argument.</param>
	/// <param name="depth">Value forwarded to the <c>depth</c> argument.</param>
	/// <param name="stencil">Value forwarded to the <c>stencil</c> argument.</param>
	public void Clear(bool color, bool depth, bool stencil)
	{
		RecordCall("clear", color, depth, stencil);
	}

	/// <summary>Records a call to <c>clearColor</c> on the JavaScript-side object.</summary>
	public void ClearColor()
	{
		RecordCall("clearColor");
	}

	/// <summary>Records a call to <c>clearDepth</c> on the JavaScript-side object.</summary>
	public void ClearDepth()
	{
		RecordCall("clearDepth");
	}

	/// <summary>Records a call to <c>clearStencil</c> on the JavaScript-side object.</summary>
	public void ClearStencil()
	{
		RecordCall("clearStencil");
	}

	/// <summary>Records a call to <c>dispose</c> on the JavaScript-side object.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Render a scene or an object using a camera. The render is done to a previously specified
	/// <c>.renderTarget</c> set by calling <c>.setRenderTarget</c> or to the canvas as usual. By
	/// default render buffers are cleared before rendering but you can prevent this by setting the
	/// property <c>autoClear</c> to false. If you want to prevent only certain buffers being cleared
	/// you can set either the <c>autoClearColor</c>, <c>autoClearStencil</c> or <c>autoClearDepth</c>
	/// properties to false. To forcibly clear one ore more buffers call <c>.clear</c>.
	/// </summary>
	/// <param name="scene">Value forwarded to the <c>scene</c> argument.</param>
	/// <param name="camera">Value forwarded to the <c>camera</c> argument.</param>
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

	/// <summary>
	/// Initializes the given WebGLRenderTarget memory. Useful for initializing a render target so data
	/// can be copied into it using <c>WebGLRenderer.copyTextureToTexture</c> before it has been
	/// rendered to.
	/// </summary>
	/// <param name="target">Value forwarded to the <c>target</c> argument.</param>
	public void InitRenderTarget(WebGLRenderTarget target)
	{
		if (Batch is not null)
		{
			target.AttachTo(Batch);
		}

		RecordCall("initRenderTarget", target);
	}

	/// <summary>Can be used to reset the internal WebGL state.</summary>
	public void ResetState()
	{
		RecordCall("resetState");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.WebGLRenderer</c>, then replays every property written before
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

		if (_isSortObjectsWritten)
		{
			batch.Set(Handle, "sortObjects", ThreeValue.Encode(_sortObjects));
		}

		if (_isLocalClippingEnabledWritten)
		{
			batch.Set(Handle, "localClippingEnabled", ThreeValue.Encode(_localClippingEnabled));
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

		if (_isTransmissionResolutionScaleWritten)
		{
			batch.Set(Handle, "transmissionResolutionScale", ThreeValue.Encode(_transmissionResolutionScale));
		}

		if (_isCapabilitiesWritten)
		{
			batch.Set(Handle, "capabilities", ThreeValue.Encode(_capabilities));
		}
	}
}
