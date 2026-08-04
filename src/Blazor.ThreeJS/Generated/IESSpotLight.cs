// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A IES version of <see cref="SpotLight"/>. Can only be used with <see cref="WebGPURenderer"/>.
/// The JavaScript-side <c>THREE.IESSpotLight</c>.
/// </summary>
public sealed class IESSpotLight : SpotLight
{
	/// <summary>Initializes a new <see cref="IESSpotLight"/>.</summary>
	public IESSpotLight()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.IESSpotLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "IESSpotLight"; }
	}
}
