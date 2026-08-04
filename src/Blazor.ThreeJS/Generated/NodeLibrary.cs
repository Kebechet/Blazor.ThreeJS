// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The purpose of a node library is to assign node implementations to existing library features. In
/// <c>WebGPURenderer</c> lights, materials which are not based on <c>NodeMaterial</c> as well as
/// tone mapping techniques are implemented with node-based modules. The JavaScript-side
/// <c>THREE.NodeLibrary</c>.
/// </summary>
public class NodeLibrary : ThreeObject
{
	/// <summary>Initializes a new <see cref="NodeLibrary"/>.</summary>
	public NodeLibrary()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.NodeLibrary</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "NodeLibrary"; }
	}
}
