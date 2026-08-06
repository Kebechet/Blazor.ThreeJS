// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>Alias for <c>EllipseCurve</c>. The JavaScript-side <c>THREE.ArcCurve</c>.</summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/curves/ArcCurve">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/curves/ArcCurve.js">Source</seealso>
public sealed class ArcCurve : EllipseCurve
{
	private readonly float _aX;
	private readonly float _aY;
	private readonly float? _aRadius;
	private readonly float _aStartAngle;
	private readonly float? _aEndAngle;
	private readonly bool _aClockwise;

	/// <summary>This constructor creates a new <see cref="ArcCurve"/>.</summary>
	/// <param name="aX">The X center of the ellipse. Expects a <c>Float</c>. Default is <c>0</c>.</param>
	/// <param name="aY">The Y center of the ellipse. Expects a <c>Float</c>. Default is <c>0</c>.</param>
	/// <param name="aRadius">Value forwarded to the <c>aRadius</c> constructor argument.</param>
	/// <param name="aStartAngle">
	/// The start angle of the curve in radians starting from the positive X axis. Default is <c>0</c>.
	/// </param>
	/// <param name="aEndAngle">
	/// The end angle of the curve in radians starting from the positive X axis. Default is <c>2 x
	/// Math.PI</c>.
	/// </param>
	/// <param name="aClockwise">Whether the ellipse is drawn clockwise. Default is <c>false</c>.</param>
	public ArcCurve(
		float aX = 0f,
		float aY = 0f,
		float? aRadius = null,
		float aStartAngle = 0f,
		float? aEndAngle = null,
		bool aClockwise = false)
	{
		_aX = aX;
		_aY = aY;
		_aRadius = aRadius;
		_aStartAngle = aStartAngle;
		_aEndAngle = aEndAngle;
		_aClockwise = aClockwise;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ArcCurve</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ArcCurve(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_aX = default!;
		_aY = default!;
		_aStartAngle = default!;
		_aClockwise = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ArcCurve</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ArcCurve"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ArcCurve</c>: aX, aY, aRadius, aStartAngle,
	/// aEndAngle, aClockwise. An argument the caller left unspecified travels as the wire's
	/// not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js applies its
	/// own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_aX,
				_aY,
				ThreeValue.OrUnspecified(_aRadius),
				_aStartAngle,
				ThreeValue.OrUnspecified(_aEndAngle),
				_aClockwise
			]);
		}
	}
}
