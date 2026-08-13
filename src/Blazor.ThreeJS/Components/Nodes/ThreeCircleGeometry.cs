using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A flat disc in the XY plane, built as a fan of triangles from its centre. Facing +Z, so it is
/// invisible edge-on and from behind unless the material draws both sides.
/// <para>
/// Every parameter here is a three.js constructor argument with no property behind it: the vertices
/// are built once and cannot be re-dimensioned. Put an <c>@key</c> on the component if the shape has
/// to change, and a fresh geometry is built instead.
/// </para>
/// </summary>
public sealed class ThreeCircleGeometry : ThreeNode<CircleGeometry>
{
	/// <summary>Radius of the disc.</summary>
	[Parameter] public float Radius { get; set; } = 1f;

	/// <summary>Number of triangles in the fan. Higher is rounder at the rim.</summary>
	[Parameter] public int Segments { get; set; } = 32;

	/// <summary>Angle where the disc starts, in radians.</summary>
	[Parameter] public float ThetaStart { get; set; }

	/// <summary>
	/// How far around the disc runs, in radians, so a part-turn gives a pie slice. Left unset, three.js
	/// sweeps the full turn rather than this component inventing a value for it.
	/// </summary>
	[Parameter] public float? ThetaLength { get; set; }

	/// <summary>Everything this geometry is built from, since none of it is writable afterwards.</summary>
	protected override (string Name, object? Value)[] ConstructionParameters
	{
		get
		{
			return
			[
				(nameof(Radius), Radius),
				(nameof(Segments), Segments),
				(nameof(ThetaStart), ThetaStart),
				(nameof(ThetaLength), ThetaLength)
			];
		}
	}

	/// <summary>Builds the geometry.</summary>
	/// <returns>The mirrored geometry.</returns>
	protected override CircleGeometry CreateThreeObject()
	{
		return new CircleGeometry(Radius, Segments, ThetaStart, ThetaLength);
	}
}
