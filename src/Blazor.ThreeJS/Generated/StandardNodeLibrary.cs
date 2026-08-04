// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This version of a node library represents the standard version used in
/// <see cref="WebGPURenderer"/>. It maps lights, tone mapping techniques and materials to
/// node-based implementations. The JavaScript-side <c>THREE.StandardNodeLibrary</c>.
/// </summary>
public sealed class StandardNodeLibrary : NodeLibrary
{
	/// <summary>Initializes a new <see cref="StandardNodeLibrary"/>.</summary>
	public StandardNodeLibrary()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.StandardNodeLibrary</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "StandardNodeLibrary"; }
	}
}
