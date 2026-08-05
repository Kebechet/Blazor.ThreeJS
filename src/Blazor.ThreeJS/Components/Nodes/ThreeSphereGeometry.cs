using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A sphere, or a slice of one. Written inside a <see cref="ThreeMesh"/>, it fills that mesh's
/// geometry slot.
/// <para>
/// Every parameter here is a three.js constructor argument with no property behind it: the vertices
/// are built once and cannot be re-dimensioned. Put an <c>@key</c> on the component if the shape has
/// to change, and a fresh geometry is built instead.
/// </para>
/// </summary>
public sealed class ThreeSphereGeometry : ThreeNode<SphereGeometry>
{
	/// <summary>Radius of the sphere.</summary>
	[Parameter] public float Radius { get; set; } = 1f;

	/// <summary>Number of horizontal segments. More makes it rounder and heavier.</summary>
	[Parameter] public int WidthSegments { get; set; } = 32;

	/// <summary>Number of vertical segments.</summary>
	[Parameter] public int HeightSegments { get; set; } = 16;

	/// <summary>Horizontal angle the surface starts at, in radians.</summary>
	[Parameter] public float PhiStart { get; set; }

	/// <summary>
	/// Horizontal angle the surface sweeps through, in radians. Left out, three.js sweeps the full
	/// circle.
	/// </summary>
	[Parameter] public float? PhiLength { get; set; }

	/// <summary>Vertical angle the surface starts at, in radians.</summary>
	[Parameter] public float ThetaStart { get; set; }

	/// <summary>
	/// Vertical angle the surface sweeps through, in radians. Left out, three.js sweeps from pole to
	/// pole.
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
				(nameof(WidthSegments), WidthSegments),
				(nameof(HeightSegments), HeightSegments),
				(nameof(PhiStart), PhiStart),
				(nameof(PhiLength), PhiLength),
				(nameof(ThetaStart), ThetaStart),
				(nameof(ThetaLength), ThetaLength)
			];
		}
	}

	/// <summary>Builds the geometry.</summary>
	/// <returns>The mirrored geometry.</returns>
	protected override SphereGeometry CreateThreeObject()
	{
		return new SphereGeometry(Radius, WidthSegments, HeightSegments, PhiStart, PhiLength, ThetaStart, ThetaLength);
	}
}
