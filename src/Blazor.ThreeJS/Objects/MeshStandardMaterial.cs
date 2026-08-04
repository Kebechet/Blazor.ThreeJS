using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A physically-based material driven by roughness and metalness, the JavaScript-side
/// <c>THREE.MeshStandardMaterial</c>.
/// </summary>
public sealed class MeshStandardMaterial : ThreeObject
{
	private float _roughness = 1f;
	private float _metalness = 0f;
	private Side _side = Side.FrontSide;

	/// <summary>Base color of the material.</summary>
	public Color Color { get; }

	/// <summary>
	/// Initializes a new material with white color, full roughness, and no metalness.
	/// </summary>
	public MeshStandardMaterial()
	{
		Color = Color.White;
		Color.OnChange = () => RecordSet("color", Color);
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.MeshStandardMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(MeshStandardMaterial); }
	}

	/// <summary>
	/// Gets or sets how rough the surface appears, from 0 (mirror-like) to 1 (fully diffuse). Setting
	/// this property records a property write once this material is attached to a batch — directly,
	/// or indirectly when the owning <see cref="Mesh"/> is attached. Writing the value already held
	/// records nothing, so reassigning unchanged state every frame costs no interop.
	/// </summary>
	public float Roughness
	{
		get { return _roughness; }
		set
		{
			if (_roughness == value)
			{
				return;
			}

			_roughness = value;
			RecordSet("roughness", value);
		}
	}

	/// <summary>
	/// Gets or sets how metallic the surface appears, from 0 (dielectric) to 1 (fully metallic).
	/// Setting this property records a property write once this material is attached to a batch —
	/// directly, or indirectly when the owning <see cref="Mesh"/> is attached. Writing the value
	/// already held records nothing, so reassigning unchanged state every frame costs no interop.
	/// </summary>
	public float Metalness
	{
		get { return _metalness; }
		set
		{
			if (_metalness == value)
			{
				return;
			}

			_metalness = value;
			RecordSet("metalness", value);
		}
	}

	/// <summary>
	/// Gets or sets which face(s) of the mesh's triangles this material renders. Setting this
	/// property records a property write once this material is attached to a batch — directly, or
	/// indirectly when the owning <see cref="Mesh"/> is attached. Writing the value already held
	/// records nothing, so reassigning unchanged state every frame costs no interop.
	/// </summary>
	public Side Side
	{
		get { return _side; }
		set
		{
			if (_side == value)
			{
				return;
			}

			_side = value;
			RecordSet("side", value);
		}
	}

	/// <summary>
	/// Emits the create op plus color, roughness, metalness, and side, so these properties are set on
	/// the JavaScript side even when they still hold their default value at attach time.
	/// </summary>
	/// <param name="batch">Batch to record the create op and property writes into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);
		batch.Set(Handle, "color", ThreeValue.Encode(Color));
		batch.Set(Handle, "roughness", _roughness);
		batch.Set(Handle, "metalness", _metalness);
		batch.Set(Handle, "side", ThreeValue.Encode(_side));
	}
}
