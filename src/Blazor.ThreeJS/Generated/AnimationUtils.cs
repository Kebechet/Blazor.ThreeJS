// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A class with various methods to assist with animations. The JavaScript-side
/// <c>THREE.AnimationUtils</c>.
/// </summary>
public sealed class AnimationUtils : ThreeObject
{
	/// <summary>Initializes a new <see cref="AnimationUtils"/>.</summary>
	public AnimationUtils()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>AnimationUtils</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal AnimationUtils(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AnimationUtils</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AnimationUtils"; }
	}
}
