using Kebechet.Blazor.ThreeJS.Objects;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A triangular polygon mesh — the workhorse of a three.js scene. Write a geometry component and a
/// material component inside it, or hand it instances through
/// <see cref="ThreeRenderableNode{TObject3D}.Geometry"/> and
/// <see cref="ThreeRenderableNode{TObject3D}.Material"/>.
/// </summary>
public sealed class ThreeMesh : ThreeRenderableNode<Mesh>
{
	/// <summary>Builds the mesh, with whatever geometry and material were supplied as parameters.</summary>
	/// <returns>The mirrored mesh.</returns>
	protected override Mesh CreateThreeObject()
	{
		return new Mesh(Geometry, Material);
	}

	/// <summary>Points the mesh at a geometry.</summary>
	/// <param name="geometry">The geometry, or <see langword="null"/> to clear the slot.</param>
	protected override void WriteGeometry(BufferGeometry? geometry)
	{
		Object.Geometry = geometry;
	}

	/// <summary>Points the mesh at a material.</summary>
	/// <param name="material">The material, or <see langword="null"/> to clear the slot.</param>
	protected override void WriteMaterial(Material? material)
	{
		Object.Material = material;
	}
}
