// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.FileLoader</c>.</summary>
public sealed class FileLoader : Loader
{
	private readonly LoadingManager? _manager;
	private string _mimeType = string.Empty;
	private string _responseType = string.Empty;
	private bool _isMimeTypeWritten;
	private bool _isResponseTypeWritten;

	/// <summary>Initializes a new <see cref="FileLoader"/>.</summary>
	/// <param name="manager">Value forwarded to the <c>manager</c> constructor argument.</param>
	public FileLoader(LoadingManager? manager = null)
	{
		_manager = manager;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>FileLoader</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal FileLoader(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.FileLoader</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "FileLoader"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.FileLoader</c>: manager. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_manager)]); }
	}

	/// <summary>
	/// The <c>mimeType</c> property of the JavaScript-side object. Writing it records a <c>mimeType</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public string MimeType
	{
		get { return _mimeType; }
		set
		{
			if (_mimeType == value)
			{
				return;
			}

			_mimeType = value;
			_isMimeTypeWritten = true;
			RecordSet("mimeType", value);
		}
	}

	/// <summary>
	/// The <c>responseType</c> property of the JavaScript-side object. Writing it records a
	/// <c>responseType</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public string ResponseType
	{
		get { return _responseType; }
		set
		{
			if (_responseType == value)
			{
				return;
			}

			_responseType = value;
			_isResponseTypeWritten = true;
			RecordSet("responseType", value);
		}
	}

	/// <summary>
	/// Records a call to <c>setResponseType</c> on the JavaScript-side object. This writes the same
	/// three.js state as <see cref="ResponseType"/> and the mirror does not learn from it: afterwards
	/// <c>ResponseType</c> still reports its previous value, and writing that value back records
	/// nothing at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="value">Value forwarded to the <c>value</c> argument.</param>
	public void SetResponseType(string value)
	{
		RecordCall("setResponseType", value);
	}

	/// <summary>
	/// Records a call to <c>setMimeType</c> on the JavaScript-side object. This writes the same
	/// three.js state as <see cref="MimeType"/> and the mirror does not learn from it: afterwards
	/// <c>MimeType</c> still reports its previous value, and writing that value back records nothing at
	/// all. Where the property exists, write the property.
	/// </summary>
	/// <param name="value">Value forwarded to the <c>value</c> argument.</param>
	public void SetMimeType(string value)
	{
		RecordCall("setMimeType", value);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.FileLoader</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_manager?.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isMimeTypeWritten)
		{
			batch.Set(Handle, "mimeType", ThreeValue.Encode(_mimeType));
		}

		if (_isResponseTypeWritten)
		{
			batch.Set(Handle, "responseType", ThreeValue.Encode(_responseType));
		}
	}
}
