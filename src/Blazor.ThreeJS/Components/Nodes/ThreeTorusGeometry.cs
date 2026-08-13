using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A ring of circular cross-section lying in the XY plane - a doughnut. <see cref="Radius"/> is how
/// far the ring reaches from the origin and <see cref="Tube"/> is how thick it is.
/// <para>
/// Every parameter here is a three.js constructor argument with no property behind it: the vertices
/// are built once and cannot be re-dimensioned. Put an <c>@key</c> on the component if the shape has
/// to change, and a fresh geometry is built instead.
/// </para>
/// </summary>
public sealed class ThreeTorusGeometry : ThreeNode<TorusGeometry>
{
	/// <summary>Distance from the origin to the centre of the tube.</summary>
	[Parameter] public float Radius { get; set; } = 1f;

	/// <summary>Radius of the tube itself, so the whole shape spans <c>Radius + Tube</c>.</summary>
	[Parameter] public float Tube { get; set; } = 0.4f;

	/// <summary>Number of faces around the tube's cross-section.</summary>
	[Parameter] public int RadialSegments { get; set; } = 12;

	/// <summary>Number of segments around the ring. This is the one that shows when the torus is large.</summary>
	[Parameter] public int TubularSegments { get; set; } = 48;

	/// <summary>
	/// How far around the ring the tube runs, in radians, so a part-turn gives an arc rather than a
	/// closed loop. Left unset, three.js closes the ring.
	/// </summary>
	[Parameter] public float? Arc { get; set; }

	/// <summary>Everything this geometry is built from, since none of it is writable afterwards.</summary>
	protected override (string Name, object? Value)[] ConstructionParameters
	{
		get
		{
			return
			[
				(nameof(Radius), Radius),
				(nameof(Tube), Tube),
				(nameof(RadialSegments), RadialSegments),
				(nameof(TubularSegments), TubularSegments),
				(nameof(Arc), Arc)
			];
		}
	}

	/// <summary>Builds the geometry.</summary>
	/// <returns>The mirrored geometry.</returns>
	protected override TorusGeometry CreateThreeObject()
	{
		return new TorusGeometry(Radius, Tube, RadialSegments, TubularSegments, Arc);
	}
}
