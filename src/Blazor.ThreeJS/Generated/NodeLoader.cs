// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.NodeLoader</c>.</summary>
public sealed class NodeLoader : Loader
{
	private readonly LoadingManager? _manager;
	private Dictionary<string, Texture> _textures;
	private bool _isTexturesWritten;

	/// <summary>Initializes a new <see cref="NodeLoader"/>.</summary>
	/// <param name="manager">Value forwarded to the <c>manager</c> constructor argument.</param>
	public NodeLoader(LoadingManager? manager = null)
		: base(manager: manager)
	{
		_manager = manager;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>NodeLoader</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal NodeLoader(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.NodeLoader</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "NodeLoader"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.NodeLoader</c>: manager. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_manager)]); }
	}

	/// <summary>
	/// The <c>textures</c> property of the JavaScript-side object. Writing it records a <c>textures</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Dictionary<string, Texture> Textures
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
			RecordSet("textures", value);
		}
	}

	/// <summary>
	/// Records a call to <c>setTextures</c> on the JavaScript-side object. This writes the same
	/// three.js state as <see cref="Textures"/> and the mirror does not learn from it: afterwards
	/// <c>Textures</c> still reports its previous value, and writing that value back records nothing at
	/// all. Where the property exists, write the property.
	/// </summary>
	/// <param name="textures">Value forwarded to the <c>textures</c> argument.</param>
	public void SetTextures(Dictionary<string, Texture> textures)
	{
		RecordCall("setTextures", textures);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.NodeLoader</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_manager?.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isTexturesWritten)
		{
			batch.Set(Handle, "textures", ThreeValue.Encode(_textures));
		}
	}
}
