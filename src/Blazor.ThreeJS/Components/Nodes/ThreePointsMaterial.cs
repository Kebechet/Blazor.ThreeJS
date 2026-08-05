using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// The material a <see cref="ThreePoints"/> cloud is drawn with: one square per vertex, sized in world
/// units or in pixels depending on <see cref="HasSizeAttenuation"/>.
/// </summary>
public sealed class ThreePointsMaterial : ThreeMaterialNode<PointsMaterial>
{
	/// <summary>Colour of each point. Left alone when not supplied, so the material stays white.</summary>
	[Parameter] public Color? Color { get; set; }

	/// <summary>How big each point is drawn.</summary>
	[Parameter] public float Size { get; set; } = 1f;

	/// <summary>
	/// Whether a point shrinks with distance the way a solid object does. Turn it off and every point
	/// keeps the same size on screen however far away it is.
	/// </summary>
	[Parameter] public bool HasSizeAttenuation { get; set; } = true;

	/// <summary>Writes the shared material parameters, then the ones specific to this material.</summary>
	/// <param name="target">The mirrored material to write into.</param>
	protected override void ApplyParameters(PointsMaterial target)
	{
		base.ApplyParameters(target);

		if (Color is { } color)
		{
			target.Color.Set(color.R, color.G, color.B);
		}

		target.Size = Size;
		target.SizeAttenuation = HasSizeAttenuation;
	}

	/// <summary>Builds the material.</summary>
	/// <returns>The mirrored material.</returns>
	protected override PointsMaterial CreateThreeObject()
	{
		return new PointsMaterial();
	}
}
