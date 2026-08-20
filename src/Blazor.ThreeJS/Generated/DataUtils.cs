// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.DataUtils</c>.</summary>
public sealed class DataUtils : ThreeObject
{
	/// <summary>Initializes a new <see cref="DataUtils"/>.</summary>
	public DataUtils()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>DataUtils</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal DataUtils(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.DataUtils</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "DataUtils"; }
	}

	/// <summary>
	/// Reads <c>toHalfFloat</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>toHalfFloat</c> returned.
	/// </summary>
	/// <param name="val">Value forwarded to the <c>val</c> argument.</param>
	/// <returns>The value <c>toHalfFloat</c> returned, once the JavaScript side has answered.</returns>
	public static Task<float> ToHalfFloatAsync(ThreeContext context, float val)
	{
		return context.CallStaticAsync<float>("DataUtils", "toHalfFloat", val);
	}

	/// <summary>
	/// Reads <c>fromHalfFloat</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>fromHalfFloat</c> returned.
	/// </summary>
	/// <param name="val">Value forwarded to the <c>val</c> argument.</param>
	/// <returns>The value <c>fromHalfFloat</c> returned, once the JavaScript side has answered.</returns>
	public static Task<float> FromHalfFloatAsync(ThreeContext context, float val)
	{
		return context.CallStaticAsync<float>("DataUtils", "fromHalfFloat", val);
	}
}
