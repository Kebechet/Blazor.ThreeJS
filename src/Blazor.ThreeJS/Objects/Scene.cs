namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Root of a scene graph. Its handle is paired with a camera's handle to tell the renderer what to
/// draw each frame.
/// </summary>
public sealed class Scene : Object3D
{
	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Scene</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(Scene); }
	}
}
