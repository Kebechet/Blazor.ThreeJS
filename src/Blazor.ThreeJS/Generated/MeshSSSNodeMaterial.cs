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

	/// <summary>
	/// Adopts an existing JavaScript-side <c>MeshSSSNodeMaterial</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal MeshSSSNodeMaterial(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshSSSNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "MeshSSSNodeMaterial"; }
	}

	/// <summary>
	/// Whether the lighting model should use SSS or not. Read-only in three.js, so it is read on demand
	/// rather than mirrored: records a get op, sends it behind every write already pending, and
	/// completes with the value <c>useSSS</c> held.
	/// </summary>
	/// <returns>The value <c>useSSS</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> UseSSSAsync()
	{
		return GetAsync<bool>("useSSS");
	}
}
