using Kebechet.Blazor.ThreeJS.Objects;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A cloud of points, drawn one per vertex of its geometry. Takes a geometry and a material exactly as
/// <see cref="ThreeMesh"/> does; what differs is only that three.js draws the vertices instead of the
/// faces between them.
/// </summary>
public sealed class ThreePoints : ThreeRenderableNode<Points>
{
	/// <summary>Builds the point cloud, with whatever geometry and material were supplied as parameters.</summary>
	/// <returns>The mirrored point cloud.</returns>
	protected override Points CreateThreeObject()
	{
		return new Points(Geometry, Material);
	}

	/// <summary>Points the cloud at a geometry.</summary>
	/// <param name="geometry">The geometry, or <see langword="null"/> to clear the slot.</param>
	protected override void WriteGeometry(BufferGeometry? geometry)
	{
		Object.Geometry = geometry;
	}

	/// <summary>Points the cloud at a material.</summary>
	/// <param name="material">The material, or <see langword="null"/> to clear the slot.</param>
	protected override void WriteMaterial(Material? material)
	{
		Object.Material = material;
	}
}
