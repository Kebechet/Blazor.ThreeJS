using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Light graded between two colours by which way a surface faces: <see cref="SkyColor"/> straight up,
/// <see cref="GroundColor"/> straight down. It is the cheap way to suggest an outdoor scene, where the
/// sky lights the tops of things and the ground bounces its own colour back up.
/// <para>
/// ⚠️ Leave <see cref="ThreeObject3DNode{TObject3D}.Position"/> alone unless you mean it. three.js
/// builds this light at <c>(0, 1, 0)</c> and that vector <em>is</em> the axis the gradient runs along,
/// so moving it to the origin leaves a zero-length direction and no light at all.
/// </para>
/// </summary>
public sealed class ThreeHemisphereLight : ThreeObject3DNode<HemisphereLight>
{
	/// <summary>
	/// Colour arriving from above. three.js takes it as a constructor argument and the mirror exposes
	/// no property for it, so it is fixed for the life of the component — put an <c>@key</c> on the
	/// component if it has to change.
	/// </summary>
	[Parameter] public Color? SkyColor { get; set; }

	/// <summary>Colour arriving from below, constructor-only for the same reason as <see cref="SkyColor"/>.</summary>
	[Parameter] public Color? GroundColor { get; set; }

	/// <summary>Strength of the light.</summary>
	[Parameter] public float Intensity { get; set; } = 1f;

	/// <summary>
	/// Both colours, compared by their packed values so an equal colour built afresh each render is not
	/// a change.
	/// </summary>
	protected override (string Name, object? Value)[] ConstructionParameters
	{
		get
		{
			return
			[
				(nameof(SkyColor), SkyColor?.GetHex()),
				(nameof(GroundColor), GroundColor?.GetHex())
			];
		}
	}

	/// <summary>Builds the light.</summary>
	/// <returns>The mirrored light.</returns>
	protected override HemisphereLight CreateThreeObject()
	{
		return new HemisphereLight(SkyColor, GroundColor, Intensity);
	}

	/// <summary>Applies the transform and flags, then the intensity.</summary>
	/// <param name="target">The mirrored light to write into.</param>
	protected override void ApplyParameters(HemisphereLight target)
	{
		base.ApplyParameters(target);

		target.Intensity = Intensity;
	}
}
