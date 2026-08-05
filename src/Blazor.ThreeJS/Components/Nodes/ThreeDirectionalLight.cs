using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Light travelling in parallel rays from infinitely far away, the way sunlight does. Its
/// <see cref="ThreeObject3DNode{TObject3D}.Position"/> sets the direction it comes from rather than a
/// place it sits, since the rays are parallel and the source has no location.
/// </summary>
public sealed class ThreeDirectionalLight : ThreeObject3DNode<DirectionalLight>
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
	/// Object the light points at. Leave it out and the light points at the scene's origin, which is
	/// three.js's own default target.
	/// </summary>
	[Parameter] public Object3D? Target { get; set; }

	/// <summary>The colour, compared by its packed value so an equal colour built afresh each render is not a change.</summary>
	protected override (string Name, object? Value)[] ConstructionParameters
	{
		get { return [(nameof(Color), Color?.GetHex())]; }
	}

	/// <summary>Builds the light.</summary>
	/// <returns>The mirrored light.</returns>
	protected override DirectionalLight CreateThreeObject()
	{
		return new DirectionalLight(Color, Intensity);
	}

	/// <summary>Applies the transform and flags, then the intensity and the target.</summary>
	/// <param name="target">The mirrored light to write into.</param>
	protected override void ApplyParameters(DirectionalLight target)
	{
		base.ApplyParameters(target);

		target.Intensity = Intensity;
		if (Target is not null)
		{
			target.Target = Target;
		}
	}
}
