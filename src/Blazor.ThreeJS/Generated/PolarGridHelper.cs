// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This helper is an object to define polar grids. Grids are two-dimensional arrays of lines. The
/// JavaScript-side <c>THREE.PolarGridHelper</c>.
/// </summary>
public sealed class PolarGridHelper : LineSegments
{
	private readonly float _radius;
	private readonly float _sectors;
	private readonly float _rings;
	private readonly float _divisions;
	private readonly Color? _color1;
	private readonly Color? _color2;

	/// <summary>Constructs a new polar grid helper.</summary>
	/// <param name="radius">The radius of the polar grid. This can be any positive number.</param>
	/// <param name="sectors">
	/// The number of sectors the grid will be divided into. This can be any positive integer.
	/// </param>
	/// <param name="rings">The number of rings. This can be any positive integer.</param>
	/// <param name="divisions">
	/// The number of line segments used for each circle. This can be any positive integer.
	/// </param>
	/// <param name="color1">The first color used for grid elements.</param>
	/// <param name="color2">The second color used for grid elements.</param>
	public PolarGridHelper(
		float radius = 10f,
		float sectors = 16f,
		float rings = 16f,
		float divisions = 64f,
		Color? color1 = null,
		Color? color2 = null)
	{
		_radius = radius;
		_sectors = sectors;
		_rings = rings;
		_divisions = divisions;
		_color1 = color1;
		_color2 = color2;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PolarGridHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PolarGridHelper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.PolarGridHelper</c>: radius, sectors, rings,
	/// divisions, color1, color2. An argument the caller left unspecified travels as the wire's
	/// not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js applies its
	/// own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_radius,
				_sectors,
				_rings,
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
