using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A material for rendering a <see cref="Points"/> object, the JavaScript-side
/// <c>THREE.PointsMaterial</c>.
/// </summary>
public sealed class PointsMaterial : ThreeObject
{
	private float _size = 1f;

	/// <summary>Base color of the points.</summary>
	public Color Color { get; }

	/// <summary>
	/// Initializes a new points material with white color and default size.
	/// </summary>
	public PointsMaterial()
	{
		Color = Color.White;
		Color.OnChange = () => RecordSet("color", Color);
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PointsMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(PointsMaterial); }
	}

	/// <summary>
	/// Gets or sets the point size. Setting this property records a property write once this
	/// material is attached to a batch — directly, or indirectly when the owning <see cref="Points"/>
	/// is attached. Writing the value already held records nothing, so reassigning unchanged state
	/// every frame costs no interop.
	/// </summary>
	public float Size
	{
		get { return _size; }
		set
		{
			if (_size == value)
			{
				return;
			}

			_size = value;
			RecordSet("size", value);
		}
	}

	/// <summary>
	/// Emits the create op plus color and size, so these properties are set on the JavaScript side
	/// even when they still hold their default value at attach time.
	/// </summary>
	/// <param name="batch">Batch to record the create op and property writes into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);
		batch.Set(Handle, "color", ThreeValue.Encode(Color));
		batch.Set(Handle, "size", _size);
	}
}
