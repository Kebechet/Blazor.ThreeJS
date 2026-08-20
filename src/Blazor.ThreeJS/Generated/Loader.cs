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
	private Dictionary<string, string> _requestHeader;
	private bool _isCrossOriginWritten;
	private bool _isWithCredentialsWritten;
	private bool _isPathWritten;
	private bool _isResourcePathWritten;
	private bool _isManagerWritten;
	private bool _isRequestHeaderWritten;

	/// <summary>Initializes a new <see cref="Loader"/>.</summary>
	/// <param name="manager">Value forwarded to the <c>manager</c> constructor argument.</param>
	public Loader(LoadingManager? manager = null)
	{
		_manager = manager;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Loader</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Loader(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
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

	/// <summary>
	/// The <c>requestHeader</c> property of the JavaScript-side object. Writing it records a
	/// <c>requestHeader</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public Dictionary<string, string> RequestHeader
	{
		get { return _requestHeader; }
		set
		{
			if (_requestHeader == value)
			{
				return;
			}

			_requestHeader = value;
			_isRequestHeaderWritten = true;
			RecordSet("requestHeader", value);
		}
	}

	/// <summary>
	/// Records a call to <c>setCrossOrigin</c> on the JavaScript-side object. This writes the same
	/// three.js state as <see cref="CrossOrigin"/> and the mirror does not learn from it: afterwards
	/// <c>CrossOrigin</c> still reports its previous value, and writing that value back records nothing
	/// at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="crossOrigin">Value forwarded to the <c>crossOrigin</c> argument.</param>
	public void SetCrossOrigin(string crossOrigin)
	{
		RecordCall("setCrossOrigin", crossOrigin);
	}

	/// <summary>
	/// Records a call to <c>setWithCredentials</c> on the JavaScript-side object. This writes the same
	/// three.js state as <see cref="WithCredentials"/> and the mirror does not learn from it:
	/// afterwards <c>WithCredentials</c> still reports its previous value, and writing that value back
	/// records nothing at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="value">Value forwarded to the <c>value</c> argument.</param>
	public void SetWithCredentials(bool value)
	{
		RecordCall("setWithCredentials", value);
	}

	/// <summary>
	/// Records a call to <c>setPath</c> on the JavaScript-side object. This writes the same three.js
	/// state as <see cref="Path"/> and the mirror does not learn from it: afterwards <c>Path</c> still
	/// reports its previous value, and writing that value back records nothing at all. Where the
	/// property exists, write the property.
	/// </summary>
	/// <param name="path">Value forwarded to the <c>path</c> argument.</param>
	public void SetPath(string path)
	{
		RecordCall("setPath", path);
	}

	/// <summary>
	/// Records a call to <c>setResourcePath</c> on the JavaScript-side object. This writes the same
	/// three.js state as <see cref="ResourcePath"/> and the mirror does not learn from it: afterwards
	/// <c>ResourcePath</c> still reports its previous value, and writing that value back records
	/// nothing at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="resourcePath">Value forwarded to the <c>resourcePath</c> argument.</param>
	public void SetResourcePath(string resourcePath)
	{
		RecordCall("setResourcePath", resourcePath);
	}

	/// <summary>
	/// Records a call to <c>setRequestHeader</c> on the JavaScript-side object. This writes the same
	/// three.js state as <see cref="RequestHeader"/> and the mirror does not learn from it: afterwards
	/// <c>RequestHeader</c> still reports its previous value, and writing that value back records
	/// nothing at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="requestHeader">Value forwarded to the <c>requestHeader</c> argument.</param>
	public void SetRequestHeader(Dictionary<string, string> requestHeader)
	{
		RecordCall("setRequestHeader", requestHeader);
	}

	/// <summary>
	/// Reads <c>abort</c> back from the JavaScript-side object. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>abort</c> returned.
	/// </summary>
	/// <returns>The value <c>abort</c> returned, once the JavaScript side has answered.</returns>
	public Task<Loader?> AbortAsync()
	{
		return RecordReadObject<Loader>("abort", (adoptedBatch, adoptedHandle) => new Loader(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Attaches the objects <c>THREE.Loader</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own. A replayed
	/// value that is itself a mirrored object is attached first, so its create op reaches the batch
	/// before the write that references it by handle.
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
			_manager?.AttachTo(batch);
			batch.Set(Handle, "manager", ThreeValue.Encode(_manager));
		}

		if (_isRequestHeaderWritten)
		{
			batch.Set(Handle, "requestHeader", ThreeValue.Encode(_requestHeader));
		}
	}
}
