using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Base for every object that can be placed in a scene graph: meshes, lights, cameras, and the
/// scene itself. Carries the transform three.js objects share and the parent/child relationships
/// between them.
/// </summary>
public abstract class Object3D : ThreeObject
{
	private readonly List<Object3D> _children = [];
	private bool _isVisible = true;

	/// <summary>Position relative to the parent object.</summary>
	public Vector3 Position { get; }

	/// <summary>Rotation relative to the parent object, expressed as Euler angles.</summary>
	public Euler Rotation { get; }

	/// <summary>Scale relative to the parent object.</summary>
	public Vector3 Scale { get; }

	/// <summary>Child objects attached via <see cref="Add"/>.</summary>
	public IReadOnlyList<Object3D> Children
	{
		get { return _children; }
	}

	/// <summary>
	/// Gets or sets whether this object is rendered. Setting this property records a property write
	/// once <see cref="ThreeObject.Batch"/> is attached, unless the value is unchanged — writing the
	/// value already held costs no interop, so a per-frame toggle that stays put is free.
	/// </summary>
	public bool IsVisible
	{
		get { return _isVisible; }
		set
		{
			if (_isVisible == value)
			{
				return;
			}

			_isVisible = value;
			RecordSet("visible", value);
		}
	}

	/// <summary>
	/// Initializes the transform with position at the origin, no rotation, and unit scale, and wires
	/// each component's <c>OnChange</c> callback to record a property write.
	/// </summary>
	protected Object3D()
	{
		Position = new Vector3();
		Rotation = new Euler();
		Scale = new Vector3(1f, 1f, 1f);

		Position.OnChange = () => RecordSet("position", Position);
		Rotation.OnChange = () => RecordSet("rotation", Rotation);
		Scale.OnChange = () => RecordSet("scale", Scale);
	}

	/// <summary>
	/// Adds a child object to this object. If this object is already attached to a batch, the child
	/// is attached too and the parent/child relationship is recorded immediately; otherwise the
	/// relationship is replayed when this object is later attached.
	/// </summary>
	/// <param name="child">The object to add as a child.</param>
	public void Add(Object3D child)
	{
		_children.Add(child);
		if (Batch is not null)
		{
			child.AttachTo(Batch);
			Batch.Add(Handle, child.Handle);
		}
	}

	/// <summary>
	/// Attaches this object and its entire subtree to a batch: emits the create op, replays every
	/// property already set on this object, then attaches each child in turn. Idempotent — a second
	/// call on an already-attached object is a no-op. Internal because the only entry point a
	/// consumer needs is <see cref="ThreeContext.Attach"/>, which calls this on the root object on
	/// their behalf. Overrides <see cref="ThreeObject.AttachTo"/> to layer transform replay and child
	/// attachment on top of the base create-op guard.
	/// </summary>
	/// <param name="batch">The batch to attach this object to.</param>
	internal override void AttachTo(ThreeBatch batch)
	{
		if (Batch is not null)
		{
			return;
		}

		base.AttachTo(batch);
		EmitState(batch);

		foreach (var child in _children)
		{
			child.AttachTo(batch);
			batch.Add(Handle, child.Handle);
		}
	}

	/// <summary>
	/// Replays properties that were written before this object was attached, so construction order
	/// never matters to the caller.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal virtual void EmitState(ThreeBatch batch)
	{
		batch.Set(Handle, "position", ThreeValue.Encode(Position));
		batch.Set(Handle, "rotation", ThreeValue.Encode(Rotation));
		batch.Set(Handle, "scale", ThreeValue.Encode(Scale));
		batch.Set(Handle, "visible", _isVisible);
	}
}
