using Kebechet.Blazor.ThreeJS.Objects;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A node with no appearance of its own, there to move everything written inside it at once. Its
/// transform applies to the whole subtree, so a group is what turns several meshes into one thing to
/// position, rotate and hide.
/// </summary>
public sealed class ThreeGroup : ThreeObject3DNode<Group>
{
	/// <summary>Builds the group.</summary>
	/// <returns>The mirrored group.</returns>
	protected override Group CreateThreeObject()
	{
		return new Group();
	}
}
