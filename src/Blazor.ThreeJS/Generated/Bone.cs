// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A <see cref="Bone"/> which is part of a <c>Skeleton</c>. The JavaScript-side <c>THREE.Bone</c>.
/// </summary>
/// <remarks>
/// The skeleton in turn is used by the <c>SkinnedMesh</c> Bones are almost identical to a blank
/// <c>Object3D</c>.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/Bone">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/Bone.js">Source</seealso>
public sealed class Bone : Object3D
{
	/// <summary>Creates a new <see cref="Bone"/>.</summary>
	public Bone()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Bone</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Bone"; }
	}
}
