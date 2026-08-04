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

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.EventDispatcher</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "EventDispatcher"; }
	}
}
