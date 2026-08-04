// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>Base class for implementing loaders. The JavaScript-side <c>THREE.Loader</c>.</summary>
public class Loader : ThreeObject
{
	private LoadingManager? _manager;
	private string _crossOrigin = string.Empty;
	private bool _withCredentials = false;
	private string _path = string.Empty;
	private string _resourcePath = string.Empty;
	private bool _isCrossOriginWritten;
	private bool _isWithCredentialsWritten;
	private bool _isPathWritten;
	private bool _isResourcePathWritten;
	private bool _isManagerWritten;

	/// <summary>Initializes a new <see cref="Loader"/>.</summary>
	/// <param name="manager">Value forwarded to the <c>manager</c> constructor argument.</param>
	public Loader(LoadingManager? manager = null)
	{
		_manager = manager;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Loader</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Loader"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Loader</c>: manager. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_manager)]); }
	}

	/// <summary>
	/// The <c>crossOrigin</c> property of the JavaScript-side object. Writing it records a
	/// <c>crossOrigin</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public string CrossOrigin
	{
		get { return _crossOrigin; }
		set
		{
			if (_crossOrigin == value)
			{
				return;
			}

			_crossOrigin = value;
			_isCrossOriginWritten = true;
			RecordSet("crossOrigin", value);
		}
	}

	/// <summary>
	/// The <c>withCredentials</c> property of the JavaScript-side object. Writing it records a
	/// <c>withCredentials</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public bool WithCredentials
	{
		get { return _withCredentials; }
		set
		{
			if (_withCredentials == value)
			{
				return;
			}

			_withCredentials = value;
			_isWithCredentialsWritten = true;
			RecordSet("withCredentials", value);
		}
	}

	/// <summary>
	/// The <c>path</c> property of the JavaScript-side object. Writing it records a <c>path</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public string Path
	{
		get { return _path; }
		set
		{
			if (_path == value)
			{
				return;
			}

			_path = value;
			_isPathWritten = true;
			RecordSet("path", value);
		}
	}

	/// <summary>
	/// The <c>resourcePath</c> property of the JavaScript-side object. Writing it records a
	/// <c>resourcePath</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public string ResourcePath
	{
		get { return _resourcePath; }
		set
		{
			if (_resourcePath == value)
			{
				return;
			}

			_resourcePath = value;
			_isResourcePathWritten = true;
			RecordSet("resourcePath", value);
		}
	}

	/// <summary>
	/// The <c>manager</c> property of the JavaScript-side object. Writing it records a <c>manager</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public LoadingManager? Manager
	{
		get { return _manager; }
		set
		{
			if (ReferenceEquals(_manager, value))
			{
				return;
			}

			_manager = value;
			_isManagerWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("manager", value);
		}
	}

	/// <summary>Records a call to <c>setCrossOrigin</c> on the JavaScript-side object.</summary>
	/// <param name="crossOrigin">Value forwarded to the <c>crossOrigin</c> argument.</param>
	public void SetCrossOrigin(string crossOrigin)
	{
		RecordCall("setCrossOrigin", crossOrigin);
	}

	/// <summary>Records a call to <c>setWithCredentials</c> on the JavaScript-side object.</summary>
	/// <param name="value">Value forwarded to the <c>value</c> argument.</param>
	public void SetWithCredentials(bool value)
	{
		RecordCall("setWithCredentials", value);
	}

	/// <summary>Records a call to <c>setPath</c> on the JavaScript-side object.</summary>
	/// <param name="path">Value forwarded to the <c>path</c> argument.</param>
	public void SetPath(string path)
	{
		RecordCall("setPath", path);
	}

	/// <summary>Records a call to <c>setResourcePath</c> on the JavaScript-side object.</summary>
	/// <param name="resourcePath">Value forwarded to the <c>resourcePath</c> argument.</param>
	public void SetResourcePath(string resourcePath)
	{
		RecordCall("setResourcePath", resourcePath);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.Loader</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_manager?.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isCrossOriginWritten)
		{
			batch.Set(Handle, "crossOrigin", ThreeValue.Encode(_crossOrigin));
		}

		if (_isWithCredentialsWritten)
		{
			batch.Set(Handle, "withCredentials", ThreeValue.Encode(_withCredentials));
		}

		if (_isPathWritten)
		{
			batch.Set(Handle, "path", ThreeValue.Encode(_path));
		}

		if (_isResourcePathWritten)
		{
			batch.Set(Handle, "resourcePath", ThreeValue.Encode(_resourcePath));
		}

		if (_isManagerWritten)
		{
			batch.Set(Handle, "manager", ThreeValue.Encode(_manager));
		}
	}
}
