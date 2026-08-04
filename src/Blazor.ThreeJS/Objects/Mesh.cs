using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A renderable object combining geometry and a material, the JavaScript-side <c>THREE.Mesh</c>.
/// Typed against <see cref="BoxGeometry"/> and <see cref="MeshStandardMaterial"/>, the geometry and
/// material types this release wraps.
/// </summary>
public sealed class Mesh : Object3D
{
	private readonly BoxGeometry _geometry;
	private MeshStandardMaterial _material;

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
	/// Gets or sets the appearance of this mesh. Setting this property attaches the incoming material
	/// first — a no-op if it is already attached to this batch, per <see cref="ThreeObject.AttachTo"/>
	/// — so its create op always reaches the batch before the property write that references it by
	/// handle. Assigning the material this mesh already holds records nothing.
	/// </summary>
	public MeshStandardMaterial Material
	{
		get { return _material; }
		set
		{
			if (ReferenceEquals(_material, value))
			{
				return;
			}

			_material = value;
			if (Batch is not null)
			{
				_material.AttachTo(Batch);
			}

			RecordSet("material", _material);
		}
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
