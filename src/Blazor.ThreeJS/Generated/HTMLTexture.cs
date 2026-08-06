// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.HTMLTexture</c>.</summary>
public sealed class HTMLTexture : Texture
{
	/// <summary>Initializes a new <see cref="HTMLTexture"/>.</summary>
	public HTMLTexture()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>HTMLTexture</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal HTMLTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.HTMLTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "HTMLTexture"; }
	}

	/// <summary>
	/// Reads <c>isHTMLTexture</c> back from the JavaScript-side object. Read-only in three.js, so it is
	/// read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isHTMLTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isHTMLTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsHTMLTextureAsync()
	{
		return GetAsync<bool>("isHTMLTexture");
	}
}
