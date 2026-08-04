// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This version of a node library represents a basic version just focusing on lights and tone
/// mapping techniques. The JavaScript-side <c>THREE.BasicNodeLibrary</c>.
/// </summary>
public sealed class BasicNodeLibrary : NodeLibrary
{
	/// <summary>Initializes a new <see cref="BasicNodeLibrary"/>.</summary>
	public BasicNodeLibrary()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BasicNodeLibrary</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "BasicNodeLibrary"; }
	}
}
