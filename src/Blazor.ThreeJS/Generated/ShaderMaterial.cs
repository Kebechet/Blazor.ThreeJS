// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A material rendered with custom shaders. A shader is a small program written in GLSL. that runs
/// on the GPU. You may want to use a custom shader if you need to implement an effect not included
/// with any of the built-in materials. There are the following notes to bear in mind when using a
/// <c>ShaderMaterial</c>: - <c>ShaderMaterial</c> can only be used with
/// <see cref="WebGLRenderer"/>. - Built in attributes and uniforms are passed to the shaders along
/// with your code. If you don't want that, use <see cref="RawShaderMaterial"/> instead. - You can
/// use the directive <c>#pragma unroll_loop_start</c> and <c>#pragma unroll_loop_end</c> in order
/// to unroll a <c>for</c> loop in GLSL by the shader preprocessor. The directive has to be placed
/// right above the loop. The loop formatting has to correspond to a defined standard. - The loop
/// has to be [normalized](https://en.wikipedia.org/wiki/Normalized_loop). - The loop variable has
/// to be *i*. - The value <c>UNROLLED_LOOP_INDEX</c> will be replaced with the explicitly value of
/// *i* for the given iteration and can be used in preprocessor statements. The JavaScript-side
/// <c>THREE.ShaderMaterial</c>.
/// </summary>
public class ShaderMaterial : Material
{
	private string _vertexShader = string.Empty;
	private string _fragmentShader = string.Empty;
	private float _linewidth = 1f;
	private bool _wireframe = false;
	private float _wireframeLinewidth = 1f;
	private bool _fog = false;
	private bool _lights = false;
	private bool _clipping = false;
	private string? _index0AttributeName;
	private bool _uniformsNeedUpdate = false;
	private bool _isVertexShaderWritten;
	private bool _isFragmentShaderWritten;
	private bool _isLinewidthWritten;
	private bool _isWireframeWritten;
	private bool _isWireframeLinewidthWritten;
	private bool _isFogWritten;
	private bool _isLightsWritten;
	private bool _isClippingWritten;
	private bool _isIndex0AttributeNameWritten;
	private bool _isUniformsNeedUpdateWritten;

	/// <summary>Constructs a new shader material.</summary>
	public ShaderMaterial()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ShaderMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ShaderMaterial"; }
	}

	/// <summary>
	/// Vertex shader GLSL code. This is the actual code for the shader. Writing it records a
	/// <c>vertexShader</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public string VertexShader
	{
		get { return _vertexShader; }
		set
		{
			if (_vertexShader == value)
			{
				return;
			}

			_vertexShader = value;
			_isVertexShaderWritten = true;
			RecordSet("vertexShader", value);
		}
	}

	/// <summary>
	/// Fragment shader GLSL code. This is the actual code for the shader. Writing it records a
	/// <c>fragmentShader</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public string FragmentShader
	{
		get { return _fragmentShader; }
		set
		{
			if (_fragmentShader == value)
			{
				return;
			}

			_fragmentShader = value;
			_isFragmentShaderWritten = true;
			RecordSet("fragmentShader", value);
		}
	}

	/// <summary>
	/// Controls line thickness or lines. WebGL and WebGPU ignore this setting and always render line
	/// primitives with a width of one pixel. Writing it records a <c>linewidth</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Linewidth
	{
		get { return _linewidth; }
		set
		{
			if (_linewidth == value)
			{
				return;
			}

			_linewidth = value;
			_isLinewidthWritten = true;
			RecordSet("linewidth", value);
		}
	}

	/// <summary>
	/// Renders the geometry as a wireframe. Writing it records a <c>wireframe</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Wireframe
	{
		get { return _wireframe; }
		set
		{
			if (_wireframe == value)
			{
				return;
			}

			_wireframe = value;
			_isWireframeWritten = true;
			RecordSet("wireframe", value);
		}
	}

	/// <summary>
	/// Controls the thickness of the wireframe. WebGL and WebGPU ignore this property and always render
	/// 1 pixel wide lines. Writing it records a <c>wireframeLinewidth</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float WireframeLinewidth
	{
		get { return _wireframeLinewidth; }
		set
		{
			if (_wireframeLinewidth == value)
			{
				return;
			}

			_wireframeLinewidth = value;
			_isWireframeLinewidthWritten = true;
			RecordSet("wireframeLinewidth", value);
		}
	}

	/// <summary>
	/// Defines whether the material color is affected by global fog settings; <c>true</c> to pass fog
	/// uniforms to the shader. Setting this property to <c>true</c> requires the definition of fog
	/// uniforms. It is recommended to use <c>UniformsUtils.merge()</c> to combine the custom shader
	/// uniforms with predefined fog uniforms. Writing it records a <c>fog</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Fog
	{
		get { return _fog; }
		set
		{
			if (_fog == value)
			{
				return;
			}

			_fog = value;
			_isFogWritten = true;
			RecordSet("fog", value);
		}
	}

	/// <summary>
	/// Defines whether this material uses lighting; <c>true</c> to pass uniform data related to
	/// lighting to this shader. Writing it records a <c>lights</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public bool Lights
	{
		get { return _lights; }
		set
		{
			if (_lights == value)
			{
				return;
			}

			_lights = value;
			_isLightsWritten = true;
			RecordSet("lights", value);
		}
	}

	/// <summary>
	/// Defines whether this material supports clipping; <c>true</c> to let the renderer pass the
	/// clippingPlanes uniform. Writing it records a <c>clipping</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public bool Clipping
	{
		get { return _clipping; }
		set
		{
			if (_clipping == value)
			{
				return;
			}

			_clipping = value;
			_isClippingWritten = true;
			RecordSet("clipping", value);
		}
	}

	/// <summary>
	/// If set, this calls
	/// [gl.bindAttribLocation](https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/bindAttribLocation)
	/// to bind a generic vertex index to an attribute variable. Writing it records a
	/// <c>index0AttributeName</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public string? Index0AttributeName
	{
		get { return _index0AttributeName; }
		set
		{
			if (_index0AttributeName == value)
			{
				return;
			}

			_index0AttributeName = value;
			_isIndex0AttributeNameWritten = true;
			RecordSet("index0AttributeName", value);
		}
	}

	/// <summary>
	/// Can be used to force a uniform update while changing uniforms in <c>Object3D#onBeforeRender</c>.
	/// Writing it records a <c>uniformsNeedUpdate</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public bool UniformsNeedUpdate
	{
		get { return _uniformsNeedUpdate; }
		set
		{
			if (_uniformsNeedUpdate == value)
			{
				return;
			}

			_uniformsNeedUpdate = value;
			_isUniformsNeedUpdateWritten = true;
			RecordSet("uniformsNeedUpdate", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.ShaderMaterial</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isVertexShaderWritten)
		{
			batch.Set(Handle, "vertexShader", ThreeValue.Encode(_vertexShader));
		}

		if (_isFragmentShaderWritten)
		{
			batch.Set(Handle, "fragmentShader", ThreeValue.Encode(_fragmentShader));
		}

		if (_isLinewidthWritten)
		{
			batch.Set(Handle, "linewidth", ThreeValue.Encode(_linewidth));
		}

		if (_isWireframeWritten)
		{
			batch.Set(Handle, "wireframe", ThreeValue.Encode(_wireframe));
		}

		if (_isWireframeLinewidthWritten)
		{
			batch.Set(Handle, "wireframeLinewidth", ThreeValue.Encode(_wireframeLinewidth));
		}

		if (_isFogWritten)
		{
			batch.Set(Handle, "fog", ThreeValue.Encode(_fog));
		}

		if (_isLightsWritten)
		{
			batch.Set(Handle, "lights", ThreeValue.Encode(_lights));
		}

		if (_isClippingWritten)
		{
			batch.Set(Handle, "clipping", ThreeValue.Encode(_clipping));
		}

		if (_isIndex0AttributeNameWritten)
		{
			batch.Set(Handle, "index0AttributeName", ThreeValue.Encode(_index0AttributeName));
		}

		if (_isUniformsNeedUpdateWritten)
		{
			batch.Set(Handle, "uniformsNeedUpdate", ThreeValue.Encode(_uniformsNeedUpdate));
		}
	}
}
