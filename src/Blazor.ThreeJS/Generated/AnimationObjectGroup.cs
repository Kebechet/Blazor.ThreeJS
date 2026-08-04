// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A group of objects that receives a shared animation state. Usage: - Add objects you would
/// otherwise pass as 'root' to the constructor or the .clipAction method of AnimationMixer. -
/// Instead pass this object as 'root'. - You can also add and remove objects later when the mixer
/// is running. Note: - Objects of this class appear as one object to the mixer, so cache control of
/// the individual objects must be done on the group. Limitation: - The animated properties must be
/// compatible among the all objects in the group. - A single property can either be controlled
/// through a target group or directly, but not both. The JavaScript-side
/// <c>THREE.AnimationObjectGroup</c>.
/// </summary>
public sealed class AnimationObjectGroup : ThreeObject
{
	/// <summary>Constructs a new animation group.</summary>
	public AnimationObjectGroup()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AnimationObjectGroup</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AnimationObjectGroup"; }
	}
}
