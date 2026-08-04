// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Its purpose is to make working with groups of objects syntactically clearer. The JavaScript-side
/// <c>THREE.Group</c>.
/// </summary>
/// <remarks>This is almost identical to an <see cref="Object3D">Object3D</see>.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/Group">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/Group.js">Source</seealso>
public class Group : Object3D
{
	/// <summary>Creates a new <see cref="Group"/>.</summary>
	public Group()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Group</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Group"; }
	}
}
