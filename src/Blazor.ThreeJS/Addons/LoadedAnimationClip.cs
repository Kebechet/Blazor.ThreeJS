using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;

namespace Kebechet.Blazor.ThreeJS.Addons;

/// <summary>
/// One animation clip a loaded glTF file brought along: the mirrored <see cref="AnimationClip"/>, and
/// the name and duration the browser reported for it.
/// <para>
/// <b>Composition, not inheritance.</b> This wraps an <see cref="AnimationClip"/> rather than being
/// one, because the generated class's <see cref="AnimationClip.Name"/> and
/// <see cref="AnimationClip.Duration"/> are constructor-argument state - the adopt constructor at
/// <c>Generated/AnimationClip.cs</c> has no way to seed them without recording a property write, the
/// way <see cref="LoadedObject3D"/> seeds a node's transform before it has a <see cref="ThreeBatch"/>
/// to record into. Reporting them here instead, as plain get-only properties read once at load time,
/// avoids that round trip entirely: the browser already told us both values in the load response.
/// </para>
/// </summary>
public sealed class LoadedAnimationClip
{
	/// <summary>
	/// The mirrored clip, for passing to <see cref="AnimationMixer.ClipActionAsync"/> or anywhere else
	/// an <see cref="AnimationClip"/> is expected.
	/// </summary>
	public AnimationClip Clip { get; }

	/// <summary>The clip's name, as three.js built it from the glTF animation.</summary>
	public string Name { get; }

	/// <summary>The clip's duration in seconds.</summary>
	public float Duration { get; }

	/// <summary>
	/// Adopts one clip the browser reported, under the handle it minted.
	/// </summary>
	/// <param name="batch">The batch <see cref="Clip"/>'s later writes record into.</param>
	/// <param name="description">What the browser reported about the clip.</param>
	internal LoadedAnimationClip(ThreeBatch batch, GLTFClipDescription description)
	{
		Clip = new AnimationClip(batch, description.Handle);
		Name = description.Name;
		Duration = description.Duration;
	}
}
