// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Helper object to graphically show the world-axis-aligned bounding box around an object. The
/// actual bounding box is handled with <c>Box3</c>, this is just a visual helper for debugging. It
/// can be automatically resized with <c>BoxHelper#update</c> when the object it's created from is
/// transformed. Note that the object must have a geometry for this to work, so it won't work with
/// sprites. The JavaScript-side <c>THREE.BoxHelper</c>.
/// </summary>
public sealed class BoxHelper : LineSegments
{
	private Object3D _object;
	private readonly Color? _color;
	private bool _isObjectWritten;

	/// <summary>Constructs a new box helper.</summary>
	/// <param name="object">The 3D object to show the world-axis-aligned bounding box.</param>
	/// <param name="color">The box's color.</param>
	public BoxHelper(Object3D @object, Color? color = null)
	{
		_object = @object;
		_color = color;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BoxHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "BoxHelper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.BoxHelper</c>: object, color. An argument the caller
	/// left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([_object, ThreeValue.OrUnspecified(_color)]); }
	}

	/// <summary>
	/// The 3D object being visualized. Writing it records a <c>object</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public Object3D Object
	{
		get { return _object; }
		set
		{
			if (ReferenceEquals(_object, value))
			{
				return;
			}

			_object = value;
			_isObjectWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("object", value);
		}
	}

	/// <summary>
	/// Updates the helper's geometry to match the dimensions of the object, including any children.
	/// </summary>
	public void Update()
	{
		RecordCall("update");
	}

	/// <summary>Updates the wireframe box for the passed object.</summary>
	/// <param name="object">The 3D object to create the helper for.</param>
	public void SetFromObject(Object3D @object)
	{
		RecordCall("setFromObject", @object);
	}

	/// <summary>
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.BoxHelper</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_object.AttachTo(batch);

		base.EmitCreate(batch);
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that. A replayed
	/// value that is itself a mirrored object is attached first, so its create op reaches the batch
	/// before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isObjectWritten)
		{
			_object.AttachTo(batch);
			batch.Set(Handle, "object", ThreeValue.Encode(_object));
		}
	}
}
