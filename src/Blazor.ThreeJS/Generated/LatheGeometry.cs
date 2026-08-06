// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates meshes with axial symmetry like vases. The JavaScript-side <c>THREE.LatheGeometry</c>.
/// </summary>
/// <remarks>The lathe rotates around the Y axis.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/LatheGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/LatheGeometry.js">Source</seealso>
public sealed class LatheGeometry : BufferGeometry
{
	private readonly Vector2[]? _points;
	private readonly int _segments;
	private readonly float _phiStart;
	private readonly float? _phiLength;

	/// <summary>This creates a <see cref="LatheGeometry"/> based on the parameters.</summary>
	/// <param name="points">
	/// Array of Vector2s. The x-coordinate of each point must be greater than zero. Default <c>[new
	/// Vector2(0, -0.5), new Vector2(0.5, 0), new Vector2(0, 0.5)]</c> _which creates a simple diamond
	/// shape_.
	/// </param>
	/// <param name="segments">The number of circumference segments to generate.</param>
	/// <param name="phiStart">The starting angle in radians.</param>
	/// <param name="phiLength">
	/// The radian (0 to 2*PI) range of the lathed section 2*PI is a closed lathe, less than 2PI is a
	/// portion.
	/// </param>
	public LatheGeometry(Vector2[]? points = null, int segments = 12, float phiStart = 0f, float? phiLength = null)
	{
		_points = points;
		_segments = segments;
		_phiStart = phiStart;
		_phiLength = phiLength;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>LatheGeometry</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal LatheGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_segments = default!;
		_phiStart = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LatheGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LatheGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.LatheGeometry</c>: points, segments, phiStart,
	/// phiLength. An argument the caller left unspecified travels as the wire's not-supplied sentinel,
	/// or is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_points),
				_segments,
				_phiStart,
				ThreeValue.OrUnspecified(_phiLength)
			]);
		}
	}
}
