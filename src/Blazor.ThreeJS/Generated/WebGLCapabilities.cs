// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A WebGL 2 backend utility module for managing the device's capabilities. The JavaScript-side
/// <c>THREE.WebGLCapabilities</c>.
/// </summary>
public sealed class WebGLCapabilities : ThreeObject
{
	private WebGLBackend _backend;
	private float? _maxAnisotropy = null;
	private float? _maxUniformBlockSize = null;
	private bool _isBackendWritten;
	private bool _isMaxAnisotropyWritten;
	private bool _isMaxUniformBlockSizeWritten;

	/// <summary>Constructs a new utility object.</summary>
	/// <param name="backend">The WebGL 2 backend.</param>
	public WebGLCapabilities(WebGLBackend backend)
	{
		_backend = backend;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.WebGLCapabilities</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "WebGLCapabilities"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.WebGLCapabilities</c>: backend.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_backend]; }
	}

	/// <summary>
	/// A reference to the WebGL 2 backend. Writing it records a <c>backend</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public WebGLBackend Backend
	{
		get { return _backend; }
		set
		{
			if (ReferenceEquals(_backend, value))
			{
				return;
			}

			_backend = value;
			_isBackendWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("backend", value);
		}
	}

	/// <summary>
	/// This value holds the cached max anisotropy value. Writing it records a <c>maxAnisotropy</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float? MaxAnisotropy
	{
		get { return _maxAnisotropy; }
		set
		{
			if (_maxAnisotropy == value)
			{
				return;
			}

			_maxAnisotropy = value;
			_isMaxAnisotropyWritten = true;
			RecordSet("maxAnisotropy", value);
		}
	}

	/// <summary>
	/// This value holds the cached max uniform block size value. Writing it records a
	/// <c>maxUniformBlockSize</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float? MaxUniformBlockSize
	{
		get { return _maxUniformBlockSize; }
		set
		{
			if (_maxUniformBlockSize == value)
			{
				return;
			}

			_maxUniformBlockSize = value;
			_isMaxUniformBlockSizeWritten = true;
			RecordSet("maxUniformBlockSize", value);
		}
	}

	/// <summary>
	/// Attaches the objects <c>THREE.WebGLCapabilities</c> is constructed from, so their create ops
	/// reach the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_backend.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isBackendWritten)
		{
			batch.Set(Handle, "backend", ThreeValue.Encode(_backend));
		}

		if (_isMaxAnisotropyWritten)
		{
			batch.Set(Handle, "maxAnisotropy", ThreeValue.Encode(_maxAnisotropy));
		}

		if (_isMaxUniformBlockSizeWritten)
		{
			batch.Set(Handle, "maxUniformBlockSize", ThreeValue.Encode(_maxUniformBlockSize));
		}
	}
}
