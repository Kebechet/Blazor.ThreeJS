using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A material that ignores lighting entirely and draws its colour flat. Useful for a wireframe, a
/// helper, or anything that should read as a diagram rather than an object in the scene — and the one
/// material that shows up in a scene with no lights in it.
/// </summary>
public sealed class ThreeMeshBasicMaterial : ThreeMaterialNode<MeshBasicMaterial>
{
	/// <summary>Colour of the surface. Left alone when not supplied, so the material stays white.</summary>
	[Parameter] public Color? Color { get; set; }

	/// <summary>Whether to draw only the edges of each triangle.</summary>
	[Parameter] public bool IsWireframe { get; set; }

	/// <summary>Writes the shared material parameters, then the ones specific to this material.</summary>
	/// <param name="target">The mirrored material to write into.</param>
	protected override void ApplyParameters(MeshBasicMaterial target)
	{
		base.ApplyParameters(target);

		if (Color is { } color)
		{
			target.Color.Set(color.R, color.G, color.B);
		}

		target.Wireframe = IsWireframe;
	}

	/// <summary>Builds the material.</summary>
	/// <returns>The mirrored material.</returns>
	protected override MeshBasicMaterial CreateThreeObject()
	{
		return new MeshBasicMaterial();
	}
}
