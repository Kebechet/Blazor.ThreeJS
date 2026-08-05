using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Light that reaches every surface in the scene equally, from no direction at all. It casts no
/// shadows and models none of the shape of an object; it is what stops the sides facing away from
/// every other light from going pure black.
/// </summary>
public sealed class ThreeAmbientLight : ThreeObject3DNode<AmbientLight>
{
	/// <summary>
	/// Colour of the light. three.js takes it as a constructor argument and the mirror exposes no
	/// property for it, so it is fixed for the life of the component — put an <c>@key</c> on the
	/// component if it has to change.
	/// </summary>
	[Parameter] public Color? Color { get; set; }

	/// <summary>Strength of the light.</summary>
	[Parameter] public float Intensity { get; set; } = 1f;

	/// <summary>The colour, compared by its packed value so an equal colour built afresh each render is not a change.</summary>
	protected override (string Name, object? Value)[] ConstructionParameters
	{
		get { return [(nameof(Color), Color?.GetHex())]; }
	}

	/// <summary>Builds the light.</summary>
	/// <returns>The mirrored light.</returns>
	protected override AmbientLight CreateThreeObject()
	{
		return new AmbientLight(Color, Intensity);
	}

	/// <summary>Applies the transform and flags, then the intensity.</summary>
	/// <param name="target">The mirrored light to write into.</param>
	protected override void ApplyParameters(AmbientLight target)
	{
		base.ApplyParameters(target);

		target.Intensity = Intensity;
	}
}
