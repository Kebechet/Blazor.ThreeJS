using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A tube following a knot that winds around a torus. <see cref="P"/> and <see cref="Q"/> are what
/// make it a knot rather than a ring: they count the windings in each direction, and coprime values
/// give a single strand that never meets itself.
/// <para>
/// Every parameter here is a three.js constructor argument with no property behind it: the vertices
/// are built once and cannot be re-dimensioned. Put an <c>@key</c> on the component if the shape has
/// to change, and a fresh geometry is built instead.
/// </para>
/// </summary>
public sealed class ThreeTorusKnotGeometry : ThreeNode<TorusKnotGeometry>
{
	/// <summary>Distance from the origin to the centre of the tube.</summary>
	[Parameter] public float Radius { get; set; } = 1f;

	/// <summary>Radius of the tube itself.</summary>
	[Parameter] public float Tube { get; set; } = 0.4f;

	/// <summary>Number of segments along the length of the knot. This is what keeps a long path smooth.</summary>
	[Parameter] public int TubularSegments { get; set; } = 64;

	/// <summary>Number of faces around the tube's cross-section.</summary>
	[Parameter] public int RadialSegments { get; set; } = 8;

	/// <summary>How many times the strand winds around the axis of symmetry.</summary>
	[Parameter] public int P { get; set; } = 2;

	/// <summary>How many times the strand winds around a circle inside the torus.</summary>
	[Parameter] public int Q { get; set; } = 3;

	/// <summary>Everything this geometry is built from, since none of it is writable afterwards.</summary>
	protected override (string Name, object? Value)[] ConstructionParameters
	{
		get
		{
			return
			[
				(nameof(Radius), Radius),
				(nameof(Tube), Tube),
				(nameof(TubularSegments), TubularSegments),
				(nameof(RadialSegments), RadialSegments),
				(nameof(P), P),
				(nameof(Q), Q)
			];
		}
	}

	/// <summary>Builds the geometry.</summary>
	/// <returns>The mirrored geometry.</returns>
	protected override TorusKnotGeometry CreateThreeObject()
	{
		return new TorusKnotGeometry(Radius, Tube, TubularSegments, RadialSegments, P, Q);
	}
}
