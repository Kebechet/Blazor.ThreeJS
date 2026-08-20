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

	/// <summary>
	/// Adopts an existing JavaScript-side <c>AnimationObjectGroup</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal AnimationObjectGroup(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AnimationObjectGroup</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AnimationObjectGroup"; }
	}

	/// <summary>Adds an arbitrary number of objects to this animation group.</summary>
	/// <param name="args">The 3D objects to add.</param>
	public void Add(params Object3D?[] args)
	{
		RecordCall("add", args);
	}

	/// <summary>Removes an arbitrary number of objects to this animation group.</summary>
	/// <param name="args">The 3D objects to remove.</param>
	public void Remove(params Object3D?[] args)
	{
		RecordCall("remove", args);
	}

	/// <summary>Deallocates all memory resources for the passed 3D objects of this animation group.</summary>
	/// <param name="args">The 3D objects to uncache.</param>
	public void Uncache(params Object3D?[] args)
	{
		RecordCall("uncache", args);
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isAnimationObjectGroup</c> held.
	/// </summary>
	/// <returns>The value <c>isAnimationObjectGroup</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsAnimationObjectGroupAsync()
	{
		return GetAsync<bool>("isAnimationObjectGroup");
	}

	/// <summary>
	/// The UUID of the 3D object. Read-only in three.js, so it is read on demand rather than mirrored:
	/// records a get op, sends it behind every write already pending, and completes with the value
	/// <c>uuid</c> held.
	/// </summary>
	/// <returns>The value <c>uuid</c> held, once the JavaScript side has answered.</returns>
	public Task<string> UuidAsync()
	{
		return GetAsync<string>("uuid");
	}
}
