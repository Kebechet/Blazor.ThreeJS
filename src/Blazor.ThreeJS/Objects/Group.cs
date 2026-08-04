namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A plain container for other objects, the JavaScript-side <c>THREE.Group</c>. Carries no state or
/// behavior beyond what <see cref="Object3D"/> already provides — grouping objects to move, rotate,
/// scale, or show/hide them together is its only purpose.
/// </summary>
public sealed class Group : Object3D
{
	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Group</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(Group); }
	}
}
