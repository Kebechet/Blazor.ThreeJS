// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A class containing utility functions for shapes. The JavaScript-side <c>THREE.ShapeUtils</c>.</summary>
/// <remarks>
/// Note that these are all linear functions so it is necessary to calculate separately for x, y
/// (and z, w if present) components of a vector.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/ShapeUtils">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/ShapeUtils.js">Source</seealso>
public sealed class ShapeUtils : ThreeObject
{
	/// <summary>Initializes a new <see cref="ShapeUtils"/>.</summary>
	public ShapeUtils()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ShapeUtils</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ShapeUtils(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ShapeUtils</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ShapeUtils"; }
	}
}
