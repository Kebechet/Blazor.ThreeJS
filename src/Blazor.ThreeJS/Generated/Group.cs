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

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Group</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Group(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Group</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Group"; }
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="Group"/>. Read-only in three.js,
	/// so it is read on demand rather than mirrored: records a get op, sends it behind every write
	/// already pending, and completes with the value <c>isGroup</c> held.
	/// </summary>
	/// <returns>The value <c>isGroup</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsGroupAsync()
	{
		return GetAsync<bool>("isGroup");
	}
}
