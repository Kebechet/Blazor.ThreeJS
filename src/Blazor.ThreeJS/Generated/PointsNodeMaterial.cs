// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Node material version of <see cref="PointsMaterial"/>. This material can be used in two ways: -
/// By rendering point primitives with <see cref="Points"/>. Since WebGPU only supports point
/// primitives with a pixel size of <c>1</c>, it's not possible to define a size. - By rendering
/// point primitives with <c>Sprites</c>. In this case, size is honored, see
/// <c>PointsNodeMaterial#sizeNode</c>. The JavaScript-side <c>THREE.PointsNodeMaterial</c>.
/// </summary>
public sealed class PointsNodeMaterial : SpriteNodeMaterial
{
	private float _size = 1f;
	private bool _isSizeWritten;

	/// <summary>Initializes a new <see cref="PointsNodeMaterial"/>.</summary>
	public PointsNodeMaterial()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PointsNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PointsNodeMaterial"; }
	}

	/// <summary>
	/// Defines the size of the points in pixels. Might be capped if the value exceeds hardware
	/// dependent parameters like
	/// [gl.ALIASED_POINT_SIZE_RANGE](https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/getParamete).
	/// Writing it records a <c>size</c> property write once this object is attached; writing the value
	/// already held records nothing.
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
			_isSizeWritten = true;
			RecordSet("size", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.PointsNodeMaterial</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isSizeWritten)
		{
			batch.Set(Handle, "size", ThreeValue.Encode(_size));
		}
	}
}
