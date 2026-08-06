// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The helper is an object to define grids. Grids are two-dimensional arrays of lines. The
/// JavaScript-side <c>THREE.GridHelper</c>.
/// </summary>
public sealed class GridHelper : LineSegments
{
	private readonly float _size;
	private readonly float _divisions;
	private readonly Color? _color1;
	private readonly Color? _color2;

	/// <summary>Constructs a new grid helper.</summary>
	/// <param name="size">The size of the grid.</param>
	/// <param name="divisions">The number of divisions across the grid.</param>
	/// <param name="color1">The color of the center line.</param>
	/// <param name="color2">The color of the lines of the grid.</param>
	public GridHelper(float size = 10f, float divisions = 10f, Color? color1 = null, Color? color2 = null)
	{
		_size = size;
		_divisions = divisions;
		_color1 = color1;
		_color2 = color2;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>GridHelper</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal GridHelper(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_size = default!;
		_divisions = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.GridHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "GridHelper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.GridHelper</c>: size, divisions, color1, color2. An
	/// argument the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed
	/// when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_size,
				_divisions,
				ThreeValue.OrUnspecified(_color1),
				ThreeValue.OrUnspecified(_color2)
			]);
		}
	}

	/// <summary>
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}
}
