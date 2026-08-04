// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="ShadowMaterial"/>. The JavaScript-side
/// <c>THREE.ShadowNodeMaterial</c>.
/// </summary>
public sealed class ShadowNodeMaterial : NodeMaterial
{
	private bool _isColorWritten;

	/// <summary>
	/// Color of the material. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>Constructs a new shadow node material.</summary>
	public ShadowNodeMaterial()
	{
		Color = new Color(0f, 0f, 0f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ShadowNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ShadowNodeMaterial"; }
	}

	/// <summary>
	/// Emits the create op for <c>THREE.ShadowNodeMaterial</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isColorWritten)
		{
			batch.Set(Handle, "color", ThreeValue.Encode(Color));
		}
	}
}
