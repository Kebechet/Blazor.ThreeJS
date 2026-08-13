using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A flat annulus in the XY plane - a disc with a hole in it. Facing +Z, so it is invisible edge-on
/// and from behind unless the material draws both sides.
/// <para>
/// Every parameter here is a three.js constructor argument with no property behind it: the vertices
/// are built once and cannot be re-dimensioned. Put an <c>@key</c> on the component if the shape has
/// to change, and a fresh geometry is built instead.
/// </para>
/// </summary>
public sealed class ThreeRingGeometry : ThreeNode<RingGeometry>
{
	/// <summary>Radius of the hole.</summary>
	[Parameter] public float InnerRadius { get; set; } = 0.5f;

	/// <summary>Radius of the outer rim.</summary>
	[Parameter] public float OuterRadius { get; set; } = 1f;

	/// <summary>Number of segments around the ring. Higher is rounder at both edges.</summary>
	[Parameter] public int ThetaSegments { get; set; } = 32;

	/// <summary>Number of bands between the hole and the rim. One is enough for a flat ring.</summary>
	[Parameter] public int PhiSegments { get; set; } = 1;

	/// <summary>Angle where the ring starts, in radians.</summary>
	[Parameter] public float ThetaStart { get; set; }

	/// <summary>
	/// How far around the ring runs, in radians, so a part-turn gives an arc. Left unset, three.js
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
				(nameof(InnerRadius), InnerRadius),
				(nameof(OuterRadius), OuterRadius),
				(nameof(ThetaSegments), ThetaSegments),
				(nameof(PhiSegments), PhiSegments),
				(nameof(ThetaStart), ThetaStart),
				(nameof(ThetaLength), ThetaLength)
			];
		}
	}

	/// <summary>Builds the geometry.</summary>
	/// <returns>The mirrored geometry.</returns>
	protected override RingGeometry CreateThreeObject()
	{
		return new RingGeometry(InnerRadius, OuterRadius, ThetaSegments, PhiSegments, ThetaStart, ThetaLength);
	}
}
