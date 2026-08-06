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

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Bone</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Bone(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Bone</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Bone"; }
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="Bone"/>. Read-only in three.js,
	/// so it is read on demand rather than mirrored: records a get op, sends it behind every write
	/// already pending, and completes with the value <c>isBone</c> held.
	/// </summary>
	/// <returns>The value <c>isBone</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsBoneAsync()
	{
		return GetAsync<bool>("isBone");
	}
}
