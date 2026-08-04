using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A renderable object combining geometry and a material, the JavaScript-side <c>THREE.Mesh</c>.
/// Typed against the concrete <see cref="BoxGeometry"/> and <see cref="MeshStandardMaterial"/> for
/// this vertical slice; the generator in Plan 2 widens this to the <c>BufferGeometry</c>/<c>Material</c>
/// base types once they exist.
/// </summary>
public sealed class Mesh : Object3D
{
	private readonly ThreeObject _geometry;
	private readonly ThreeObject _material;

	/// <summary>
	/// Initializes a new mesh from a box geometry and a standard material.
	/// </summary>
	/// <param name="geometry">Shape of the mesh.</param>
	/// <param name="material">Appearance of the mesh.</param>
	public Mesh(BoxGeometry geometry, MeshStandardMaterial material)
	{
		_geometry = geometry;
		_material = material;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Mesh</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(Mesh); }
	}

	/// <summary>
	/// Attaches the geometry and material first, then emits this mesh's create op referencing both
	/// by handle, so the applier never constructs a mesh before its dependencies exist. Attaching
	/// rather than just emitting is what lets the geometry or material be shared with another mesh:
	/// <see cref="ThreeObject.AttachTo"/> is idempotent, so a dependency already attached by an
	/// earlier mesh is not created twice, and either dependency keeps recording further property
	/// writes after this point instead of silently discarding them.
	/// </summary>
	/// <param name="batch">Batch to record the create ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_geometry.AttachTo(batch);
		_material.AttachTo(batch);
		batch.Create(Handle, ThreeTypeName, [ThreeValue.Encode(_geometry), ThreeValue.Encode(_material)]);
	}
}
