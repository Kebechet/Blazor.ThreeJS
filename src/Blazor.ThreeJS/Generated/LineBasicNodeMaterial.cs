// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="LineBasicMaterial"/>. The JavaScript-side
/// <c>THREE.LineBasicNodeMaterial</c>.
/// </summary>
public sealed class LineBasicNodeMaterial : NodeMaterial
{
	private float _linewidth = 1f;
	private bool _isColorWritten;
	private bool _isLinewidthWritten;

	/// <summary>
	/// Color of the material. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>Constructs a new line basic node material.</summary>
	public LineBasicNodeMaterial()
	{
		Color = new Color(1f, 1f, 1f);
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LineBasicNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LineBasicNodeMaterial"; }
	}

	/// <summary>
	/// Controls line thickness or lines. Can only be used with <c>SVGRenderer</c>. WebGL and WebGPU
	/// ignore this setting and always render line primitives with a width of one pixel. Writing it
	/// records a <c>linewidth</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float Linewidth
	{
		get { return _linewidth; }
		set
		{
			if (_linewidth == value)
			{
				return;
			}

			_linewidth = value;
			_isLinewidthWritten = true;
			RecordSet("linewidth", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.LineBasicNodeMaterial</c>, then replays every property written
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

		if (_isLinewidthWritten)
		{
			batch.Set(Handle, "linewidth", ThreeValue.Encode(_linewidth));
		}
	}
}
