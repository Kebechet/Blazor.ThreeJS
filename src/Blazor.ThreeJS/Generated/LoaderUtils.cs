// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.LoaderUtils</c>.</summary>
public sealed class LoaderUtils : ThreeObject
{
	/// <summary>Initializes a new <see cref="LoaderUtils"/>.</summary>
	public LoaderUtils()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>LoaderUtils</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal LoaderUtils(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LoaderUtils</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LoaderUtils"; }
	}

	/// <summary>
	/// Reads <c>extractUrlBase</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>extractUrlBase</c> returned.
	/// </summary>
	/// <param name="url">Value forwarded to the <c>url</c> argument.</param>
	/// <returns>The value <c>extractUrlBase</c> returned, once the JavaScript side has answered.</returns>
	public static Task<string> ExtractUrlBaseAsync(ThreeContext context, string url)
	{
		return context.CallStaticAsync<string>("LoaderUtils", "extractUrlBase", url);
	}

	/// <summary>
	/// Reads <c>resolveURL</c> back from the JavaScript-side object. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>resolveURL</c> returned.
	/// </summary>
	/// <param name="url">Value forwarded to the <c>url</c> argument.</param>
	/// <param name="path">Value forwarded to the <c>path</c> argument.</param>
	/// <returns>The value <c>resolveURL</c> returned, once the JavaScript side has answered.</returns>
	public static Task<string> ResolveURLAsync(ThreeContext context, string url, string path)
	{
		return context.CallStaticAsync<string>("LoaderUtils", "resolveURL", url, path);
	}
}
