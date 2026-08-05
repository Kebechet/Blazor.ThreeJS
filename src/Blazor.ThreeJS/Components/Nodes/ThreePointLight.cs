using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Light radiating in every direction from a single point, the way a bare bulb does. Unlike a
/// directional light its <see cref="ThreeObject3DNode{TObject3D}.Position"/> is a real place, and how
/// far its light carries is what <see cref="Distance"/> and <see cref="Decay"/> describe.
/// </summary>
public sealed class ThreePointLight : ThreeObject3DNode<PointLight>
{
	/// <summary>
	/// Colour of the light. three.js takes it as a constructor argument and the mirror exposes no
	/// property for it, so it is fixed for the life of the component — put an <c>@key</c> on the
	/// component if it has to change.
	/// </summary>
	[Parameter] public Color? Color { get; set; }

	/// <summary>Strength of the light.</summary>
	[Parameter] public float Intensity { get; set; } = 1f;

	/// <summary>
	/// How far the light reaches before it has faded to nothing. Zero, the default, means it never
	/// stops.
	/// </summary>
	[Parameter] public float Distance { get; set; }

	/// <summary>How sharply the light dims with distance. Two is what physically correct falloff needs.</summary>
	[Parameter] public float Decay { get; set; } = 2f;

	/// <summary>The colour, compared by its packed value so an equal colour built afresh each render is not a change.</summary>
	protected override (string Name, object? Value)[] ConstructionParameters
	{
		get { return [(nameof(Color), Color?.GetHex())]; }
	}

	/// <summary>Builds the light.</summary>
	/// <returns>The mirrored light.</returns>
	protected override PointLight CreateThreeObject()
	{
		return new PointLight(Color, Intensity, Distance, Decay);
	}

	/// <summary>Applies the transform and flags, then the intensity and the falloff.</summary>
	/// <param name="target">The mirrored light to write into.</param>
	protected override void ApplyParameters(PointLight target)
	{
		base.ApplyParameters(target);

		target.Intensity = Intensity;
		target.Distance = Distance;
		target.Decay = Decay;
	}
}
