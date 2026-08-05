using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A flat rectangle in the XY plane — a floor, a wall, a backdrop. Written inside a
/// <see cref="ThreeMesh"/>, it fills that mesh's geometry slot.
/// <para>
/// Every parameter here is a three.js constructor argument with no property behind it: the vertices
/// are built once and cannot be re-dimensioned. Put an <c>@key</c> on the component if the shape has
/// to change, and a fresh geometry is built instead.
/// </para>
/// </summary>
public sealed class ThreePlaneGeometry : ThreeNode<PlaneGeometry>
{
	/// <summary>Width along the X axis.</summary>
	[Parameter] public float Width { get; set; } = 1f;

	/// <summary>Height along the Y axis.</summary>
	[Parameter] public float Height { get; set; } = 1f;

	/// <summary>Number of segmented faces along the width.</summary>
	[Parameter] public int WidthSegments { get; set; } = 1;

	/// <summary>Number of segmented faces along the height.</summary>
	[Parameter] public int HeightSegments { get; set; } = 1;

	/// <summary>Everything this geometry is built from, since none of it is writable afterwards.</summary>
	protected override (string Name, object? Value)[] ConstructionParameters
	{
		get
		{
			return
			[
				(nameof(Width), Width),
				(nameof(Height), Height),
				(nameof(WidthSegments), WidthSegments),
				(nameof(HeightSegments), HeightSegments)
			];
		}
	}

	/// <summary>Builds the geometry.</summary>
	/// <returns>The mirrored geometry.</returns>
	protected override PlaneGeometry CreateThreeObject()
	{
		return new PlaneGeometry(Width, Height, WidthSegments, HeightSegments);
	}
}
