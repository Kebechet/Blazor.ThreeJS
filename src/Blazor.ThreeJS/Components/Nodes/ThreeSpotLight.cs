using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Light thrown in a cone from a point, the way a stage lamp is. It points from its
/// <see cref="ThreeObject3DNode{TObject3D}.Position"/> at the light's target, which three.js places at
/// the origin unless something moves it.
/// </summary>
public sealed class ThreeSpotLight : ThreeObject3DNode<SpotLight>
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

	/// <summary>
	/// Half-width of the cone, in radians, up to half a turn. Left unset, three.js picks its own
	/// default rather than this component inventing one.
	/// <para>
	/// Constructor-only, like <see cref="Color"/>: three.js takes an angle when the light is built and
	/// the mirror exposes no property for it, so changing this rebuilds the light.
	/// </para>
	/// </summary>
	[Parameter] public float? Angle { get; set; }

	/// <summary>
	/// How much of the cone's edge is blurred away, from 0 for a hard rim to 1 for an entirely soft one.
	/// </summary>
	[Parameter] public float Penumbra { get; set; }

	/// <summary>How sharply the light dims with distance. Two is what physically correct falloff needs.</summary>
	[Parameter] public float Decay { get; set; } = 2f;

	/// <summary>
	/// The two values three.js only accepts when the light is built: the colour, compared by its packed
	/// value so an equal colour built afresh each render is not a change, and the cone angle.
	/// </summary>
	protected override (string Name, object? Value)[] ConstructionParameters
	{
		get
		{
			return
			[
				(nameof(Color), Color?.GetHex()),
				(nameof(Angle), Angle)
			];
		}
	}

	/// <summary>Builds the light.</summary>
	/// <returns>The mirrored light.</returns>
	protected override SpotLight CreateThreeObject()
	{
		return new SpotLight(Color, Intensity, Distance, Angle, Penumbra, Decay);
	}

	/// <summary>Applies the transform and flags, then everything the light lets you change afterwards.</summary>
	/// <param name="target">The mirrored light to write into.</param>
	protected override void ApplyParameters(SpotLight target)
	{
		base.ApplyParameters(target);

		target.Intensity = Intensity;
		target.Distance = Distance;
		target.Penumbra = Penumbra;
		target.Decay = Decay;
	}
}
