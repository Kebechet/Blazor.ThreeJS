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

	/// <summary>Used for parsing AOS keyframe formats.</summary>
	/// <param name="jsonKeys">A list of JSON keyframes.</param>
	/// <param name="times">This array will be filled with keyframe times by this method.</param>
	/// <param name="values">This array will be filled with keyframe values by this method.</param>
	/// <param name="valuePropertyName">The name of the property to use.</param>
	public void FlattenJSON(float[] jsonKeys, float[] times, float[] values, string valuePropertyName)
	{
		RecordCall("flattenJSON", jsonKeys, times, values, valuePropertyName);
	}

	/// <summary>
	/// Returns <c>true</c> if the given object is a typed array. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>isTypedArray</c> returned.
	/// </summary>
	/// <param name="context">
	/// Context the call belongs to; a static has no object of its own to record through.
	/// </param>
	/// <param name="object">The object to check.</param>
	/// <returns>The value <c>isTypedArray</c> returned, once the JavaScript side has answered.</returns>
	public static Task<bool> IsTypedArrayAsync(ThreeContext context, object? @object)
	{
		return context.CallStaticAsync<bool>("AnimationUtils", "isTypedArray", @object);
	}

	/// <summary>
	/// Returns an array by which times and values can be sorted. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>getKeyframeOrder</c> returned.
	/// </summary>
	/// <param name="context">
	/// Context the call belongs to; a static has no object of its own to record through.
	/// </param>
	/// <param name="times">The keyframe time values.</param>
	/// <returns>The value <c>getKeyframeOrder</c> returned, once the JavaScript side has answered.</returns>
	public static Task<float[]> GetKeyframeOrderAsync(ThreeContext context, float[] times)
	{
		return context.CallStaticAsync<float[]>("AnimationUtils", "getKeyframeOrder", (object?) times);
	}

	/// <summary>
	/// Sorts the given array by the previously computed order via <c>getKeyframeOrder()</c>. Records a
	/// read op, sends it behind every write already pending, and completes with what <c>sortedArray</c>
	/// returned.
	/// </summary>
	/// <param name="context">
	/// Context the call belongs to; a static has no object of its own to record through.
	/// </param>
	/// <param name="values">The values to sort.</param>
	/// <param name="stride">The stride.</param>
	/// <param name="order">The sort order.</param>
	/// <returns>The value <c>sortedArray</c> returned, once the JavaScript side has answered.</returns>
	public static Task<float[]> SortedArrayAsync(ThreeContext context, float[] values, float stride, float[] order)
	{
		return context.CallStaticAsync<float[]>("AnimationUtils", "sortedArray", values, stride, order);
	}

	/// <summary>
	/// Creates a new clip, containing only the segment of the original clip between the given frames.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>subclip</c> returned.
	/// </summary>
	/// <param name="context">
	/// Context the call belongs to; a static has no object of its own to record through.
	/// </param>
	/// <param name="sourceClip">The values to sort.</param>
	/// <param name="name">The name of the clip.</param>
	/// <param name="startFrame">The start frame.</param>
	/// <param name="endFrame">The end frame.</param>
	/// <param name="fps">The FPS.</param>
	/// <returns>The value <c>subclip</c> returned, once the JavaScript side has answered.</returns>
	public static Task<AnimationClip?> SubclipAsync(
		ThreeContext context,
		AnimationClip sourceClip,
		string name,
		float startFrame,
		float endFrame,
		float fps = 30f)
	{
		return context.CallStaticAsync<AnimationClip>("AnimationUtils", "subclip", sourceClip, name, startFrame, endFrame, fps);
	}

	/// <summary>
	/// Converts the keyframes of the given animation clip to an additive format. Records a read op,
	/// sends it behind every write already pending, and completes with what <c>makeClipAdditive</c>
	/// returned.
	/// </summary>
	/// <param name="context">
	/// Context the call belongs to; a static has no object of its own to record through.
	/// </param>
	/// <param name="targetClip">The clip to make additive.</param>
	/// <param name="referenceFrame">The reference frame.</param>
	/// <param name="referenceClip">The reference clip.</param>
	/// <param name="fps">The FPS.</param>
	/// <returns>The value <c>makeClipAdditive</c> returned, once the JavaScript side has answered.</returns>
	public static Task<AnimationClip?> MakeClipAdditiveAsync(
		ThreeContext context,
		AnimationClip targetClip,
		float referenceFrame,
		AnimationClip referenceClip,
		float fps = 30f)
	{
		return context.CallStaticAsync<AnimationClip>("AnimationUtils", "makeClipAdditive", targetClip, referenceFrame, referenceClip, fps);
	}
}
