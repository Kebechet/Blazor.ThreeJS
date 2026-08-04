// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This type of camera can be used in order to efficiently render a scene with a predefined set of
/// cameras. This is an important performance aspect for rendering VR scenes. An instance of
/// <c>ArrayCamera</c> always has an array of sub cameras. It's mandatory to define for each sub
/// camera the <c>viewport</c> property which determines the part of the viewport that is rendered
/// with this camera. The JavaScript-side <c>THREE.ArrayCamera</c>.
/// </summary>
public sealed class ArrayCamera : PerspectiveCamera
{
	/// <summary>Constructs a new array camera.</summary>
	public ArrayCamera()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ArrayCamera</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ArrayCamera"; }
	}
}
