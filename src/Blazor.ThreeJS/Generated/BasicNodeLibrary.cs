// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This version of a node library represents a basic version just focusing on lights and tone
/// mapping techniques. The JavaScript-side <c>THREE.BasicNodeLibrary</c>.
/// </summary>
public sealed class BasicNodeLibrary : ThreeObject
{
	/// <summary>Initializes a new <see cref="BasicNodeLibrary"/>.</summary>
	public BasicNodeLibrary()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>BasicNodeLibrary</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal BasicNodeLibrary(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BasicNodeLibrary</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "BasicNodeLibrary"; }
	}
}
