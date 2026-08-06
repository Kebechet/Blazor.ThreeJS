// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>JavaScript events for custom objects. The JavaScript-side <c>THREE.EventDispatcher</c>.</summary>
/// <seealso href="https://github.com/mrdoob/eventdispatcher.js">mrdoob EventDispatcher on GitHub</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/EventDispatcher">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/EventDispatcher.js">Source</seealso>
public class EventDispatcher : ThreeObject
{
	/// <summary>Creates <c>EventDispatcher</c> object.</summary>
	public EventDispatcher()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>EventDispatcher</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal EventDispatcher(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.EventDispatcher</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "EventDispatcher"; }
	}
}
