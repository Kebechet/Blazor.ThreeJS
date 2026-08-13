using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A cone standing on the Y axis, centred on the origin: a cylinder whose top has closed to a point.
/// <para>
/// Every parameter here is a three.js constructor argument with no property behind it: the vertices
/// are built once and cannot be re-dimensioned. Put an <c>@key</c> on the component if the shape has
/// to change, and a fresh geometry is built instead.
/// </para>
/// </summary>
public sealed class ThreeConeGeometry : ThreeNode<ConeGeometry>
{
	/// <summary>Radius of the base.</summary>
	[Parameter] public float Radius { get; set; } = 1f;

	/// <summary>Height along the Y axis, from the base to the point.</summary>
	[Parameter] public float Height { get; set; } = 1f;

	/// <summary>Number of faces around the circumference. Higher is rounder and costs more vertices.</summary>
	[Parameter] public int RadialSegments { get; set; } = 32;

	/// <summary>Number of bands stacked up the side. One is enough unless the surface is being deformed.</summary>
	[Parameter] public int HeightSegments { get; set; } = 1;

	/// <summary>Leaves the base off, so the cone is a hollow shell.</summary>
	[Parameter] public bool OpenEnded { get; set; }

	/// <summary>Angle around the Y axis where the surface starts, in radians.</summary>
	[Parameter] public float ThetaStart { get; set; }

	/// <summary>
	/// How far around the Y axis the surface runs, in radians. Left unset, three.js sweeps the full
	/// turn rather than this component inventing a value for it.
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
				(nameof(Height), Height),
				(nameof(RadialSegments), RadialSegments),
				(nameof(HeightSegments), HeightSegments),
				(nameof(OpenEnded), OpenEnded),
				(nameof(ThetaStart), ThetaStart),
				(nameof(ThetaLength), ThetaLength)
			];
		}
	}

	/// <summary>Builds the geometry.</summary>
	/// <returns>The mirrored geometry.</returns>
	protected override ConeGeometry CreateThreeObject()
	{
		return new ConeGeometry(Radius, Height, RadialSegments, HeightSegments, OpenEnded, ThetaStart, ThetaLength);
	}
}
