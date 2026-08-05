using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Somewhere a declarative child component can put the mirrored object it owns. Cascaded down the
/// render tree, so a component finds its slot the same way any Blazor component finds an ancestor's
/// state: the nearest enclosing one wins.
/// <para>
/// This is what makes the render tree the scene graph. <c>&lt;ThreeMesh&gt;</c> offers itself as the
/// slot for its own children, so a <c>&lt;ThreeBoxGeometry&gt;</c> written inside it reaches the
/// mesh's geometry rather than the scene, and a <c>&lt;ThreeMesh&gt;</c> written inside a
/// <c>&lt;ThreeGroup&gt;</c> is added to the group rather than the scene. Nothing declares where it
/// belongs; where it is written is where it goes.
/// </para>
/// <para>
/// Internal because a slot answers with <see cref="ThreeObject"/>-level state that only this assembly
/// can act on. Deriving from <see cref="ThreeNode{TThreeObject}"/> is the supported way to add a
/// component of your own; implementing a slot is not part of the surface yet.
/// </para>
/// </summary>
internal interface IThreeSlot
{
	/// <summary>
	/// Whether the object behind this slot has already reached a <see cref="ThreeContext"/>.
	/// <para>
	/// This is what decides <b>when</b> a child registers, and therefore the order the ops come out in.
	/// A slot that is still detached takes its children immediately, because nothing is emitted until
	/// the whole graph is attached at the root — which is what puts a geometry's create op ahead of the
	/// mesh that references it. A slot that is already attached takes them only once the child's own
	/// subtree has rendered, so the child is complete before its create op is emitted.
	/// </para>
	/// </summary>
	bool IsSlotAttached { get; }

	/// <summary>Puts a child's mirrored object into this slot.</summary>
	/// <param name="child">The component whose object is being slotted in.</param>
	/// <exception cref="InvalidOperationException">
	/// Thrown when this slot has nowhere to put an object of that kind — a material inside a
	/// <c>&lt;ThreeGroup&gt;</c>, for instance.
	/// </exception>
	void AttachChild(ThreeNode child);

	/// <summary>
	/// Takes a child's mirrored object back out, because the component that owned it is being disposed.
	/// A no-op once this slot is itself being torn down: the ops it would record would name a handle the
	/// applier has already retired.
	/// </summary>
	/// <param name="child">The component whose object is being removed.</param>
	void DetachChild(ThreeNode child);
}
