using Kebechet.Blazor.ThreeJS.Addons;

namespace Blazor.ThreeJS.Tests.Addons;

/// <summary>
/// Builds the node rows <c>loadGltf</c> answers with, so a test can say what the browser reported
/// without spelling out the tagged wire form of three transforms every time.
/// </summary>
internal static class LoadedNode
{
	/// <summary>
	/// Describes one node exactly as the applier's encoder would: a handle out of the browser-minted
	/// half of the space, three tagged transform components, and the type three.js gave the object.
	/// </summary>
	/// <param name="handle">The minted handle.</param>
	/// <param name="name">The node's glTF name.</param>
	/// <param name="type">three.js's own type for it.</param>
	/// <param name="position">Local position the loader gave it.</param>
	/// <param name="scale">Local scale the loader gave it.</param>
	/// <param name="isVisible">Whether the loader left it visible.</param>
	/// <returns>The row.</returns>
	public static GLTFNodeDescription Describe(
		int handle,
		string name,
		string type = "Mesh",
		float[]? position = null,
		float[]? scale = null,
		bool isVisible = true)
	{
		var localPosition = position ?? [0f, 0f, 0f];
		var localScale = scale ?? [1f, 1f, 1f];
		return new GLTFNodeDescription
		{
			Handle = handle,
			Name = name,
			Type = type,
			Position = AddonJsObjectReference.TaggedValue("Vector3", localPosition),
			Rotation = AddonJsObjectReference.TaggedValue("Euler", 0f, 0f, 0f),
			Scale = AddonJsObjectReference.TaggedValue("Vector3", localScale),
			IsVisible = isVisible
		};
	}

	/// <summary>
	/// Describes one animation clip exactly as the applier would: a handle out of the browser-minted
	/// half of the space, the clip's name, and its duration in seconds.
	/// </summary>
	/// <param name="handle">The minted handle.</param>
	/// <param name="name">The clip's name.</param>
	/// <param name="duration">The clip's duration in seconds.</param>
	/// <returns>The row.</returns>
	public static GLTFClipDescription DescribeClip(int handle, string name, float duration)
	{
		return new GLTFClipDescription
		{
			Handle = handle,
			Name = name,
			Duration = duration
		};
	}
}
