// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A class containing utility functions for images. The JavaScript-side <c>THREE.ImageUtils</c>.</summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/ImageUtils">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/ImageUtils.js">Source</seealso>
public sealed class ImageUtils : ThreeObject
{
	/// <summary>Initializes a new <see cref="ImageUtils"/>.</summary>
	public ImageUtils()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ImageUtils</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ImageUtils(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ImageUtils</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ImageUtils"; }
	}
}
