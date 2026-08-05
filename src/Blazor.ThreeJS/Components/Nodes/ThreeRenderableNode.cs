using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Base for a scene-graph component whose object is drawn from a geometry and a material — a mesh, a
/// cloud of points. Both reach the object the same two ways: as a parameter, which is what lets one
/// instance be shared between several components, or as a component written inside, which is what
/// makes the markup read as a tree.
/// <para>
/// Supply one or the other for a given slot, not both. A parameter is re-applied on every render, so
/// it would overwrite what a child component put there.
/// </para>
/// </summary>
/// <typeparam name="TObject3D">Type of the scene-graph object this component owns.</typeparam>
public abstract class ThreeRenderableNode<TObject3D> : ThreeObject3DNode<TObject3D>
	where TObject3D : Object3D
{
	/// <summary>
	/// The geometry to draw. Left alone when not supplied, so a geometry component written inside this
	/// one fills the slot instead.
	/// </summary>
	[Parameter] public BufferGeometry? Geometry { get; set; }

	/// <summary>
	/// The material to draw it with. Left alone when not supplied, so a material component written
	/// inside this one fills the slot instead.
	/// </summary>
	[Parameter] public Material? Material { get; set; }

	/// <summary>
	/// Points the object's geometry slot at <paramref name="geometry"/>. What varies between a mesh and
	/// a cloud of points is only which class declares the property, so the write is the one thing a
	/// derived component has to supply.
	/// </summary>
	/// <param name="geometry">The geometry, or <see langword="null"/> to clear the slot.</param>
	protected abstract void WriteGeometry(BufferGeometry? geometry);

	/// <summary>Points the object's material slot at <paramref name="material"/>.</summary>
	/// <param name="material">The material, or <see langword="null"/> to clear the slot.</param>
	protected abstract void WriteMaterial(Material? material);

	/// <summary>Applies the transform and flags, then the two slots this component adds.</summary>
	/// <param name="target">The mirrored object to write into.</param>
	protected override void ApplyParameters(TObject3D target)
	{
		base.ApplyParameters(target);

		if (Geometry is not null)
		{
			WriteGeometry(Geometry);
		}

		if (Material is not null)
		{
			WriteMaterial(Material);
		}
	}

	/// <summary>
	/// Routes a geometry or a material written inside this component into the matching slot, and
	/// anything else — another scene-graph node — into the scene graph.
	/// </summary>
	/// <param name="child">The component whose object is being slotted in.</param>
	protected override void AttachChild(ThreeNode child)
	{
		switch (child.MirroredObject)
		{
			case BufferGeometry geometry:
				WriteGeometry(geometry);
				return;
			case Material material:
				WriteMaterial(material);
				return;
			default:
				base.AttachChild(child);
				return;
		}
	}

	/// <summary>
	/// Clears whichever slot the removed component filled. A slot cleared while this component lives on
	/// is left holding nothing, which is what the markup now says: three.js will refuse to draw the
	/// object, rather than go on drawing it with a geometry that has just been released.
	/// </summary>
	/// <param name="child">The component whose object is being removed.</param>
	protected override void DetachChild(ThreeNode child)
	{
		switch (child.MirroredObject)
		{
			case BufferGeometry _:
				WriteGeometry(null);
				return;
			case Material _:
				WriteMaterial(null);
				return;
			default:
				base.DetachChild(child);
				return;
		}
	}
}
