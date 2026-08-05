using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Base for a declarative component whose object belongs in the scene graph — a mesh, a light, a
/// camera, a group. Carries the transform and the per-object rendering flags every such object has,
/// raises clicks, and offers itself as the slot for whatever is written inside it.
/// </summary>
/// <typeparam name="TObject3D">Type of the scene-graph object this component owns.</typeparam>
public abstract class ThreeObject3DNode<TObject3D> : ThreeNode<TObject3D>, IThreeSlot
	where TObject3D : Object3D
{
	/// <summary>
	/// What is written inside this component. Every scene-graph node written in here is added as a
	/// child of this one; a geometry or a material reaches whichever slot this component offers for it.
	/// </summary>
	[Parameter] public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Position relative to the parent object. Left alone when not supplied, so the object keeps
	/// three.js's own origin.
	/// </summary>
	[Parameter] public Vector3? Position { get; set; }

	/// <summary>
	/// Rotation relative to the parent object, as Euler angles. Left alone when not supplied.
	/// </summary>
	[Parameter] public Euler? Rotation { get; set; }

	/// <summary>Scale relative to the parent object. Left alone when not supplied.</summary>
	[Parameter] public Vector3? Scale { get; set; }

	/// <summary>Optional name for the object, which does not have to be unique.</summary>
	[Parameter] public string? Name { get; set; }

	/// <summary>Whether the object is rendered.</summary>
	[Parameter] public bool IsVisible { get; set; } = true;

	/// <summary>Whether the object is rendered into the shadow map.</summary>
	[Parameter] public bool CastShadow { get; set; }

	/// <summary>Whether the object's material receives shadows.</summary>
	[Parameter] public bool ReceiveShadow { get; set; }

	/// <summary>
	/// Raised when a click's ray meets this object's own geometry. Supplying a handler is what opts the
	/// object into hit-testing on the browser side, and removing it opts it back out, so a scene where
	/// nothing handles a click is never hit-tested at all.
	/// <para>
	/// Only this object is tested. A handler on a <c>&lt;ThreeGroup&gt;</c> does not make the meshes
	/// inside it clickable — put it on the components that carry the geometry you want clicked.
	/// </para>
	/// </summary>
	[Parameter] public EventCallback<ThreePointerEvent> OnClick { get; set; }

	private Action<ThreePointerEvent>? _pointerHandler;

	/// <summary>Whether the object behind this slot has already reached a context.</summary>
	bool IThreeSlot.IsSlotAttached
	{
		get { return Object.IsAttached; }
	}

	/// <summary>Puts a child's object into this component's slot for it.</summary>
	/// <param name="child">The component whose object is being slotted in.</param>
	void IThreeSlot.AttachChild(ThreeNode child)
	{
		AttachChild(child);
	}

	/// <summary>Takes a child's object back out, unless this component is already being torn down.</summary>
	/// <param name="child">The component whose object is being removed.</param>
	void IThreeSlot.DetachChild(ThreeNode child)
	{
		// A parent disposed before its child records nothing here. Its own release op has already
		// retired its handle on the browser side, so a detach op naming it would be an unknown-handle
		// failure — and there is nothing left to detach the child from anyway.
		if (IsDisposed)
		{
			return;
		}

		DetachChild(child);
	}

	/// <summary>
	/// Puts a child's object into this component. Adds it to the scene graph, which is what every
	/// scene-graph node does with a scene-graph child; a component with a slot of its own — a mesh's
	/// geometry, a mesh's material — overrides this and falls back here for the rest.
	/// </summary>
	/// <param name="child">The component whose object is being slotted in.</param>
	/// <exception cref="InvalidOperationException">Thrown when the child's object does not belong in a scene graph.</exception>
	protected virtual void AttachChild(ThreeNode child)
	{
		if (child.MirroredObject is not Object3D childObject)
		{
			throw BuildUnslottableChildFailure(child);
		}

		Object.Add(childObject);
	}

	/// <summary>Takes a child's object back out of the scene graph.</summary>
	/// <param name="child">The component whose object is being removed.</param>
	protected virtual void DetachChild(ThreeNode child)
	{
		if (child.MirroredObject is Object3D childObject)
		{
			Object.Remove(childObject);
		}
	}

	/// <summary>
	/// The failure a slot reports for a child it has nowhere to put, naming both types and what this
	/// component does accept.
	/// </summary>
	/// <param name="child">The component that cannot be slotted in.</param>
	/// <returns>The exception to throw.</returns>
	protected InvalidOperationException BuildUnslottableChildFailure(ThreeNode child)
	{
		return new InvalidOperationException(
			$"'{GetType().Name}' has nowhere to put a '{child.MirroredObject.GetType().Name}' written inside it. " +
			$"It accepts scene-graph objects, which become its children. Write the component somewhere its object has a slot.");
	}

	/// <summary>
	/// Writes the transform, the name, and the rendering flags, then brings the click subscription into
	/// line with the handler currently supplied.
	/// </summary>
	/// <param name="target">The mirrored object to write into.</param>
	protected override void ApplyParameters(TObject3D target)
	{
		if (Position is { } position)
		{
			target.Position.Set(position.X, position.Y, position.Z);
		}

		if (Rotation is { } rotation)
		{
			target.Rotation.Set(rotation.X, rotation.Y, rotation.Z, rotation.Order);
		}

		if (Scale is { } scale)
		{
			target.Scale.Set(scale.X, scale.Y, scale.Z);
		}

		if (Name is { } name)
		{
			target.Name = name;
		}

		target.IsVisible = IsVisible;
		target.CastShadow = CastShadow;
		target.ReceiveShadow = ReceiveShadow;
		SyncPointerSubscription(target);
	}

	/// <summary>Unsubscribes the click handler before the object is released.</summary>
	protected override void ReleaseParameterSubscriptions()
	{
		if (_pointerHandler is null)
		{
			return;
		}

		Object.OnClick -= _pointerHandler;
		_pointerHandler = null;
	}

	/// <summary>
	/// Renders whatever is written inside this component, with this component offered as the slot for
	/// it. <c>IsFixed</c> because a component is its own slot for as long as it exists, so there is
	/// nothing for the cascade to notify anyone about.
	/// </summary>
	/// <param name="builder">Builder for this component's render tree.</param>
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		if (ChildContent is null)
		{
			return;
		}

		builder.OpenComponent<CascadingValue<IThreeSlot>>(0);
		builder.AddAttribute(1, "Value", this);
		builder.AddAttribute(2, "IsFixed", true);
		builder.AddAttribute(3, "ChildContent", ChildContent);
		builder.CloseComponent();
	}

	/// <summary>
	/// Subscribes or unsubscribes the mirror's click event to match whether a handler is supplied,
	/// through one handler instance that lives as long as the subscription — so a re-render that
	/// supplies the same handler again changes nothing and records no op.
	/// </summary>
	/// <param name="target">The mirrored object to subscribe on.</param>
	private void SyncPointerSubscription(TObject3D target)
	{
		var isSubscribed = _pointerHandler is not null;
		if (OnClick.HasDelegate == isSubscribed)
		{
			return;
		}

		if (!isSubscribed)
		{
			_pointerHandler = RaisePointerEvent;
			target.OnClick += _pointerHandler;
			return;
		}

		target.OnClick -= _pointerHandler;
		_pointerHandler = null;
	}

	/// <summary>
	/// Hands a hit the browser reported to the supplied handler.
	/// <para>
	/// The returned task is not awaited, and cannot be: the mirror raises clicks through a synchronous
	/// <see cref="Action"/>, which is what lets a handler change the scene without awaiting anything. A
	/// synchronous handler therefore throws straight through this call and out through the interop call
	/// that delivered the hit; an asynchronous one is dispatched to its own component, which is what
	/// re-renders it and reports its failure.
	/// </para>
	/// </summary>
	/// <param name="pointerEvent">Where the ray met this object.</param>
	private void RaisePointerEvent(ThreePointerEvent pointerEvent)
	{
		_ = OnClick.InvokeAsync(pointerEvent);
	}
}
