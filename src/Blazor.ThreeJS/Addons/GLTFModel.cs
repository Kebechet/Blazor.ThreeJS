namespace Kebechet.Blazor.ThreeJS.Addons;

/// <summary>
/// The result of loading one glTF or GLB file: the graph's root, the nodes of it the mirror knows by
/// name, and the animation clips the file brought along.
/// <para>
/// <b>How much is mirrored, and why not all of it.</b> A C# object is built for the root and for
/// every <i>named</i> descendant, and for nothing else. A name is glTF's own way of addressing a
/// node — it is what an artist sets when they mean a part to be reachable — whereas an unnamed node
/// can only be identified by its position in a traversal, which changes the next time the file is
/// exported. Mirroring those would hand the caller an identifier that quietly stops meaning the same
/// thing. The cost of a load is therefore set by how much of the file its author chose to name, not
/// by how much geometry it contains: a hundred-thousand-triangle figure with six named parts mirrors
/// seven objects. A file that names all of its nodes mirrors all of them, and
/// <see cref="Nodes"/>.Count says so.
/// </para>
/// <para>
/// Everything not mirrored is still there and still renders — the graph lives in the browser and the
/// renderer walks it whole. What the unmirrored part cannot do is be addressed from C#.
/// </para>
/// </summary>
public sealed class GLTFModel
{
	/// <summary>
	/// Root of the loaded graph. Add this to a scene to show the model, and move, rotate or hide it to
	/// move, rotate or hide the whole thing.
	/// </summary>
	public LoadedObject3D Scene { get; }

	/// <summary>
	/// Every named node beneath <see cref="Scene"/>, in the order the browser traversed them. Empty
	/// when the file names nothing below its root.
	/// </summary>
	public IReadOnlyList<LoadedObject3D> Nodes { get; }

	/// <summary>
	/// Every animation clip the file brought along, in the order the browser reported them. Empty when
	/// the file carries none. Play one through an <c>AnimationMixer</c> attached to <see cref="Scene"/>
	/// (or to whichever node the clip's tracks target) and <c>AnimationMixer.ClipActionAsync</c>.
	/// </summary>
	public IReadOnlyList<LoadedAnimationClip> Animations { get; }

	/// <summary>
	/// Builds the model over the nodes and clips the browser reported.
	/// </summary>
	/// <param name="scene">The loaded root.</param>
	/// <param name="nodes">Its named descendants.</param>
	/// <param name="animations">Its animation clips.</param>
	internal GLTFModel(LoadedObject3D scene, IReadOnlyList<LoadedObject3D> nodes, IReadOnlyList<LoadedAnimationClip> animations)
	{
		Scene = scene;
		Nodes = nodes;
		Animations = animations;
	}

	/// <summary>
	/// Finds a named node beneath <see cref="Scene"/>, or <see langword="null"/> when the file has no
	/// such name. Matching is ordinal and case-sensitive, because a glTF name is data rather than an
	/// identifier the mirror is free to normalize.
	/// <para>
	/// The first match wins. glTF does not require names to be unique, and the loader only makes them
	/// unique within one file; a caller who needs every match can filter <see cref="Nodes"/>.
	/// </para>
	/// </summary>
	/// <param name="name">The node's glTF name.</param>
	/// <returns>The node, or <see langword="null"/> when nothing carries that name.</returns>
	public LoadedObject3D? FindNode(string name)
	{
		return Nodes.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));
	}

	/// <summary>
	/// Finds an animation clip by name, or <see langword="null"/> when the file carries no such clip.
	/// Matching is ordinal and case-sensitive, for the same reason <see cref="FindNode"/>'s is: a glTF
	/// name is data rather than an identifier the mirror is free to normalize.
	/// <para>
	/// The first match wins. glTF does not require animation names to be unique, and a caller who needs
	/// every match can filter <see cref="Animations"/>.
	/// </para>
	/// </summary>
	/// <param name="name">The clip's name.</param>
	/// <returns>The clip, or <see langword="null"/> when nothing carries that name.</returns>
	public LoadedAnimationClip? FindClip(string name)
	{
		return Animations.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));
	}

	/// <summary>
	/// Releases everything this load brought in: the geometries, materials and textures the browser
	/// retains for the graph, and every handle minted for the file's nodes and clips. Without this, a
	/// loaded model stays resident for the life of the canvas even after its scene node is removed —
	/// the browser has no way to know C# is done with it.
	/// <para>
	/// Remove <see cref="Scene"/> from whatever it was added to first; the release does not detach it.
	/// The op travels with the next flush, like every other recorded instruction. Afterwards every
	/// mirror this model handed out — <see cref="Scene"/>, <see cref="Nodes"/>,
	/// <see cref="Animations"/> — is spent: a write records nothing and a read fails at the call site.
	/// Calling this twice is a no-op the second time.
	/// </para>
	/// </summary>
	public void Unload()
	{
		// One dispose op, for the root: the applier releases the whole graph off it and retires every
		// handle it minted for the file. The node and clip mirrors are spent locally rather than each
		// recording a dispose of their own, which would name handles the applier drops in that same
		// sweep.
		Scene.RetireHandle();
		Scene.RetireLocally();
		foreach (var node in Nodes)
		{
			node.RetireLocally();
		}

		foreach (var animation in Animations)
		{
			animation.Clip.RetireLocally();
		}
	}
}
