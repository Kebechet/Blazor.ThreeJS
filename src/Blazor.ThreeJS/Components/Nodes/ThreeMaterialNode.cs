using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Base for a component that supplies a material. Carries the parameters every three.js material has;
/// what makes one material differ from another — how it reacts to light, whether it has a colour at
/// all — belongs on the component for that material.
/// <para>
/// A material component fills the material slot of whatever it is written inside. It is not a
/// scene-graph node and cannot be written directly in a <see cref="ThreeCanvas"/>.
/// </para>
/// </summary>
/// <typeparam name="TMaterial">Type of the material this component owns.</typeparam>
public abstract class ThreeMaterialNode<TMaterial> : ThreeNode<TMaterial>
	where TMaterial : Material
{
	/// <summary>
	/// How opaque the material is, from 0 to 1. Values below 1 only take effect when
	/// <see cref="IsTransparent"/> is set.
	/// </summary>
	[Parameter] public float Opacity { get; set; } = 1f;

	/// <summary>Whether the material is rendered through the transparency pass at all.</summary>
	[Parameter] public bool IsTransparent { get; set; }

	/// <summary>Which faces of a surface are drawn.</summary>
	[Parameter] public Side Side { get; set; } = Side.FrontSide;

	/// <summary>Writes the parameters every material shares.</summary>
	/// <param name="target">The mirrored material to write into.</param>
	protected override void ApplyParameters(TMaterial target)
	{
		base.ApplyParameters(target);

		target.Opacity = Opacity;
		target.Transparent = IsTransparent;
		target.Side = Side;
	}
}
