using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// The physically based material: metalness and roughness decide how a surface reacts to light, and
/// the result looks right under any lighting rather than only the one it was tuned for. This is the
/// material to reach for unless you know you want another.
/// </summary>
public sealed class ThreeMeshStandardMaterial : ThreeMaterialNode<MeshStandardMaterial>
{
	/// <summary>Colour of the surface. Left alone when not supplied, so the material stays white.</summary>
	[Parameter] public Color? Color { get; set; }

	/// <summary>
	/// How rough the surface is, from 0 for a mirror to 1 for a fully diffuse one.
	/// </summary>
	[Parameter] public float Roughness { get; set; } = 1f;

	/// <summary>How metallic the surface is, from 0 for a dielectric to 1 for a metal.</summary>
	[Parameter] public float Metalness { get; set; }

	/// <summary>Colour the surface gives off by itself, unaffected by any light.</summary>
	[Parameter] public Color? Emissive { get; set; }

	/// <summary>How strongly <see cref="Emissive"/> glows.</summary>
	[Parameter] public float EmissiveIntensity { get; set; } = 1f;

	/// <summary>Whether to draw only the edges of each triangle.</summary>
	[Parameter] public bool IsWireframe { get; set; }

	/// <summary>Whether each face is shaded flat rather than smoothed across its vertices.</summary>
	[Parameter] public bool IsFlatShaded { get; set; }

	/// <summary>Writes the shared material parameters, then the ones specific to this material.</summary>
	/// <param name="target">The mirrored material to write into.</param>
	protected override void ApplyParameters(MeshStandardMaterial target)
	{
		base.ApplyParameters(target);

		if (Color is { } color)
		{
			target.Color.Set(color.R, color.G, color.B);
		}

		if (Emissive is { } emissive)
		{
			target.Emissive.Set(emissive.R, emissive.G, emissive.B);
		}

		target.Roughness = Roughness;
		target.Metalness = Metalness;
		target.EmissiveIntensity = EmissiveIntensity;
		target.Wireframe = IsWireframe;
		target.FlatShading = IsFlatShaded;
	}

	/// <summary>Builds the material.</summary>
	/// <returns>The mirrored material.</returns>
	protected override MeshStandardMaterial CreateThreeObject()
	{
		return new MeshStandardMaterial();
	}
}
