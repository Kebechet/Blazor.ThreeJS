using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;

namespace Kebechet.Blazor.ThreeJS.Addons;

/// <summary>
/// A node of a graph the browser built and C# only names: one object out of a loaded glTF file.
/// Behaves like any other <see cref="Object3D"/> once you have it — move it, hide it, add it to a
/// scene, subscribe to its <see cref="Object3D.OnClick"/> — but it was never created from this side,
/// and that shows in two places worth knowing about.
/// <para>
/// <b>What the mirror knows.</b> The loader set this node's transform, not C#, so
/// <see cref="Object3D.Position"/>, <see cref="Object3D.Rotation"/>, <see cref="Object3D.Scale"/>,
/// <see cref="Object3D.IsVisible"/> and <see cref="Object3D.Name"/> are seeded from what the browser
/// reported at load time and are accurate the moment you receive them. Writes after that are
/// mirrored exactly as they are on any other object. Everything the mirror was <b>not</b> told —
/// the geometry, the material, the textures the file brought in — has no handle and no C# object at
/// all: those stay entirely on the JavaScript side.
/// </para>
/// <para>
/// <b>The tree is not mirrored.</b> <see cref="Object3D.Children"/> is empty on every loaded node,
/// including the root, because C# did not build the graph and calling
/// <see cref="Object3D.Add(Object3D)"/> to rebuild it would re-parent nodes three.js has already
/// placed. The graph is intact in the browser and renders correctly; use
/// <see cref="GLTFModel.FindNode"/> and <see cref="GLTFModel.Nodes"/> to reach a node by name
/// instead of by walking children.
/// </para>
/// </summary>
public sealed class LoadedObject3D : Object3D
{
	private readonly string _threeTypeName;

	/// <summary>
	/// three.js's own <c>type</c> for this object, as the browser reported it: <c>Mesh</c>,
	/// <c>Group</c>, <c>Bone</c>, <c>Object3D</c> and so on. What a glTF node becomes is the loader's
	/// decision, so this is read rather than assumed.
	/// </summary>
	public string ThreeType
	{
		get { return _threeTypeName; }
	}

	/// <inheritdoc/>
	protected override string ThreeTypeName
	{
		get { return _threeTypeName; }
	}

	/// <summary>
	/// Adopts one node of a loaded graph: takes the handle the browser minted for it and seeds the
	/// mirror with the state the browser reported.
	/// <para>
	/// The seeding happens before <see cref="ThreeObject.Batch"/> is assigned, which is what keeps it
	/// silent. <c>RecordSet</c> is a no-op while there is no batch, so writing the loader's own values
	/// into the mirror records nothing — the object already holds them, and sending them back would be
	/// a round trip that could only ever confirm what the browser just said.
	/// </para>
	/// </summary>
	/// <param name="batch">The batch this node's later writes record into.</param>
	/// <param name="description">What the browser reported about the node.</param>
	internal LoadedObject3D(ThreeBatch batch, GLTFNodeDescription description)
		: base(description.Handle)
	{
		_threeTypeName = description.Type;

		var loadedRotation = DecodeRequired<Euler>(description.Rotation, nameof(description.Rotation));
		Position.Copy(DecodeRequired<Vector3>(description.Position, nameof(description.Position)));
		Rotation.Set(loadedRotation.X, loadedRotation.Y, loadedRotation.Z, loadedRotation.Order);
		Scale.Copy(DecodeRequired<Vector3>(description.Scale, nameof(description.Scale)));
		Name = description.Name;
		IsVisible = description.IsVisible;

		Batch = batch;
	}

	/// <summary>
	/// Decodes one component of the transform the browser reported, and fails loudly when it is
	/// missing. A missing component would leave the mirror holding C#'s own default for a node three.js
	/// has already placed somewhere else, and every later read of that property would be wrong with
	/// nothing to say so.
	/// </summary>
	/// <typeparam name="TValue">The math type the component decodes to.</typeparam>
	/// <param name="element">The tagged value as it arrived.</param>
	/// <param name="member">Name of the component, for the failure message.</param>
	/// <returns>The decoded value.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the component was absent from the payload.</exception>
	private static TValue DecodeRequired<TValue>(JsonElement? element, string member)
		where TValue : class
	{
		var value = ThreeValue.Decode<TValue>(element);
		if (value is null)
		{
			throw new InvalidOperationException(
				$"The browser reported a loaded node without its '{member}'. Every node row carries a full transform, " +
				$"so this is a wire-format disagreement with three-interop.js rather than a property the file left out.");
		}

		return value;
	}

	/// <summary>
	/// Refuses to emit a create op. Unreachable through <see cref="Object3D.AttachTo"/>, which returns
	/// early on an object that already has a batch, and this one has had one since it was constructed.
	/// <para>
	/// It throws rather than doing nothing because the only way to reach it is a rebuild — recreating
	/// the scene from the C# mirror after a lost WebGL context — and the mirror cannot rebuild this
	/// object. It holds the node's transform and nothing else; <c>new THREE.Mesh()</c> would produce an
	/// empty mesh with the right position, which renders as nothing and reports no error. Loading the
	/// file again is the only honest recovery, and that is a decision for the caller who knows the URL.
	/// </para>
	/// </summary>
	/// <param name="batch">Batch the create op would have been recorded into.</param>
	/// <exception cref="InvalidOperationException">Always.</exception>
	internal override void EmitCreate(ThreeBatch batch)
	{
		throw new InvalidOperationException(
			$"'{Name}' (handle {Handle}) is a '{ThreeType}' the browser loaded, so it cannot be created from the C# mirror. " +
			$"The mirror holds this node's transform and nothing else — not its geometry, material or textures — so a create op " +
			$"would build an empty object of the right shape in the right place. Load the file again instead.");
	}
}
