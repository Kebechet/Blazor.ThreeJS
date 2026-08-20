// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This holds a reference to a real property in the scene graph; used internally. The
/// JavaScript-side <c>THREE.PropertyBinding</c>.
/// </summary>
public sealed class PropertyBinding : ThreeObject
{
	private ThreeObject _rootNode;
	private string _path;
	private bool _isPathWritten;
	private bool _isRootNodeWritten;

	/// <summary>Constructs a new property binding.</summary>
	/// <param name="rootNode">The root node.</param>
	/// <param name="path">The path.</param>
	public PropertyBinding(ThreeObject rootNode, string path)
	{
		_rootNode = rootNode;
		_path = path;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>PropertyBinding</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal PropertyBinding(ThreeBatch batch, int handle)
		: base(handle)
	{
		_rootNode = default!;
		_path = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PropertyBinding</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PropertyBinding"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.PropertyBinding</c>: rootNode, path.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_rootNode, _path]; }
	}

	/// <summary>
	/// The object path to the animated property. Writing it records a <c>path</c> property write once
	/// this object is attached; writing the value already held records nothing.
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
	/// The root node. Writing it records a <c>rootNode</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public ThreeObject RootNode
	{
		get { return _rootNode; }
		set
		{
			if (ReferenceEquals(_rootNode, value))
			{
				return;
			}

			_rootNode = value;
			_isRootNodeWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("rootNode", value);
		}
	}

	/// <summary>Creates a getter / setter pair for the property tracked by this binding.</summary>
	public void Bind()
	{
		RecordCall("bind");
	}

	/// <summary>Unbinds the property.</summary>
	public void Unbind()
	{
		RecordCall("unbind");
	}

	/// <summary>
	/// Replaces spaces with underscores and removes unsupported characters from node names, to ensure
	/// compatibility with parseTrackName(). Records a read op, sends it behind every write already
	/// pending, and completes with what <c>sanitizeNodeName</c> returned.
	/// </summary>
	/// <param name="name">Node name to be sanitized.</param>
	/// <returns>The value <c>sanitizeNodeName</c> returned, once the JavaScript side has answered.</returns>
	public static Task<string> SanitizeNodeNameAsync(ThreeContext context, string name)
	{
		return context.CallStaticAsync<string>("PropertyBinding", "sanitizeNodeName", name);
	}

	/// <summary>
	/// Parses the given track name (an object path to an animated property) and returns an object with
	/// information about the path. Matches strings in the following forms: - nodeName.property -
	/// nodeName.property[accessor] - nodeName.material.property[accessor] - uuid.property[accessor] -
	/// uuid.objectName[objectIndex].propertyName[propertyIndex] - parentName/nodeName.property -
	/// parentName/parentName/nodeName.property[index] - .bone[Armature.DEF_cog].position -
	/// scene:helium_balloon_model:helium_balloon_model.position. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>parseTrackName</c> returned.
	/// </summary>
	/// <param name="trackName">The track name to parse.</param>
	/// <returns>The value <c>parseTrackName</c> returned, once the JavaScript side has answered.</returns>
	public static Task<ParseTrackNameResults> ParseTrackNameAsync(ThreeContext context, string trackName)
	{
		return context.CallStaticAsync<ParseTrackNameResults>("PropertyBinding", "parseTrackName", trackName);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.PropertyBinding</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own. A
	/// replayed value that is itself a mirrored object is attached first, so its create op reaches the
	/// batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_rootNode.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isPathWritten)
		{
			batch.Set(Handle, "path", ThreeValue.Encode(_path));
		}

		if (_isRootNodeWritten)
		{
			_rootNode.AttachTo(batch);
			batch.Set(Handle, "rootNode", ThreeValue.Encode(_rootNode));
		}
	}
}
