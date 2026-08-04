// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This node material is an experimental extension of <see cref="MeshPhysicalNodeMaterial"/> that
/// implements a Subsurface scattering (SSS) term. The JavaScript-side
/// <c>THREE.MeshSSSNodeMaterial</c>.
/// </summary>
public sealed class MeshSSSNodeMaterial : MeshPhysicalNodeMaterial
{
	/// <summary>Initializes a new <see cref="MeshSSSNodeMaterial"/>.</summary>
	public MeshSSSNodeMaterial()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshSSSNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshSSSNodeMaterial"; }
	}
}
