// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A 2D <see cref="Path"/> representation. The JavaScript-side <c>THREE.Path</c>.</summary>
/// <remarks>
/// The class provides methods for creating paths and contours of 2D shapes similar to the 2D Canvas
/// API.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/core/Path">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/core/Path.js">Source</seealso>
public class Path : CurvePath
{
	private readonly Vector2[]? _points;
	private bool _isCurrentPointWritten;

	/// <summary>
	/// The current offset of the path. Any new <c>Curve</c> added will start here. Mirrored as an
	/// instance this object owns: mutating it records a write of <c>currentPoint</c>.
	/// </summary>
	public Vector2 CurrentPoint { get; }

	/// <summary>Creates a <see cref="Path"/> from the points.</summary>
	/// <param name="points">Array of <see cref="Vector2">Vector2s</see>.</param>
	public Path(Vector2[]? points = null)
	{
		_points = points;

		CurrentPoint = new Vector2();
		CurrentPoint.OnChange = () =>
		{
			_isCurrentPointWritten = true;
			RecordSet("currentPoint", CurrentPoint);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Path</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Path(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		CurrentPoint = new Vector2();
		CurrentPoint.OnChange = () =>
		{
			_isCurrentPointWritten = true;
			RecordSet("currentPoint", CurrentPoint);
		};

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Path</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Path"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Path</c>: points. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_points)]); }
	}

	/// <summary>Adds an absolutely positioned <c>EllipseCurve</c> to the path.</summary>
	/// <param name="aX"></param>
	/// <param name="aY">X, The absolute center of the arc.</param>
	/// <param name="aRadius">The radius of the arc.</param>
	/// <param name="aStartAngle">The start angle in radians.</param>
	/// <param name="aEndAngle">The end angle in radians.</param>
	/// <param name="aClockwise">Sweep the arc clockwise.</param>
	public void Absarc(float aX, float aY, float aRadius, float aStartAngle, float aEndAngle, bool aClockwise = false)
	{
		RecordCall("absarc", aX, aY, aRadius, aStartAngle, aEndAngle, aClockwise);
	}

	/// <summary>Adds an absolutely positioned <c>EllipseCurve</c> to the path.</summary>
	/// <param name="aX"></param>
	/// <param name="aY">X, The absolute center of the ellipse.</param>
	/// <param name="xRadius">The radius of the ellipse in the x axis.</param>
	/// <param name="yRadius">The radius of the ellipse in the y axis.</param>
	/// <param name="aStartAngle">The start angle in radians.</param>
	/// <param name="aEndAngle">The end angle in radians.</param>
	/// <param name="aClockwise">Sweep the ellipse clockwise.</param>
	/// <param name="aRotation">
	/// The rotation angle of the ellipse in radians, counterclockwise from the positive X axis.
	/// </param>
	public void Absellipse(
		float aX,
		float aY,
		float xRadius,
		float yRadius,
		float aStartAngle,
		float aEndAngle,
		bool aClockwise = false,
		float aRotation = 0f)
	{
		RecordCall("absellipse", aX, aY, xRadius, yRadius, aStartAngle, aEndAngle, aClockwise, aRotation);
	}

	/// <summary>Adds an <c>EllipseCurve</c> to the path, positioned relative to <c>.currentPoint</c>.</summary>
	/// <param name="aX"></param>
	/// <param name="aY">X, The center of the arc offset from the last call.</param>
	/// <param name="aRadius">The radius of the arc.</param>
	/// <param name="aStartAngle">The start angle in radians.</param>
	/// <param name="aEndAngle">The end angle in radians.</param>
	/// <param name="aClockwise">Sweep the arc clockwise.</param>
	public void Arc(float aX, float aY, float aRadius, float aStartAngle, float aEndAngle, bool aClockwise = false)
	{
		RecordCall("arc", aX, aY, aRadius, aStartAngle, aEndAngle, aClockwise);
	}

	/// <summary>
	/// This creates a bezier curve from <c>.currentPoint</c> with (cp1X, cp1Y) and (cp2X, cp2Y) as
	/// control points and updates <c>.currentPoint</c> to x and y.
	/// </summary>
	/// <param name="aCP1x"></param>
	/// <param name="aCP1y"></param>
	/// <param name="aCP2x"></param>
	/// <param name="aCP2y"></param>
	/// <param name="aX"></param>
	/// <param name="aY"></param>
	public void BezierCurveTo(float aCP1x, float aCP1y, float aCP2x, float aCP2y, float aX, float aY)
	{
		RecordCall("bezierCurveTo", aCP1x, aCP1y, aCP2x, aCP2y, aX, aY);
	}

	/// <summary>Adds an <c>EllipseCurve</c> to the path, positioned relative to <c>.currentPoint</c>.</summary>
	/// <param name="aX"></param>
	/// <param name="aY">X, The center of the ellipse offset from the last call.</param>
	/// <param name="xRadius">The radius of the ellipse in the x axis.</param>
	/// <param name="yRadius">The radius of the ellipse in the y axis.</param>
	/// <param name="aStartAngle">The start angle in radians.</param>
	/// <param name="aEndAngle">The end angle in radians.</param>
	/// <param name="aClockwise">Sweep the ellipse clockwise.</param>
	/// <param name="aRotation">
	/// The rotation angle of the ellipse in radians, counterclockwise from the positive X axis.
	/// </param>
	public void Ellipse(
		float aX,
		float aY,
		float xRadius,
		float yRadius,
		float aStartAngle,
		float aEndAngle,
		bool aClockwise = false,
		float aRotation = 0f)
	{
		RecordCall("ellipse", aX, aY, xRadius, yRadius, aStartAngle, aEndAngle, aClockwise, aRotation);
	}

	/// <summary>Connects a <c>LineCurve</c> from <c>.currentPoint</c> to x, y onto the path.</summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public void LineTo(float x, float y)
	{
		RecordCall("lineTo", x, y);
	}

	/// <summary>Move the <c>.currentPoint</c> to x, y.</summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public void MoveTo(float x, float y)
	{
		RecordCall("moveTo", x, y);
	}

	/// <summary>
	/// Creates a quadratic curve from <c>.currentPoint</c> with cpX and cpY as control point and
	/// updates <c>.currentPoint</c> to x and y.
	/// </summary>
	/// <param name="aCPx"></param>
	/// <param name="aCPy"></param>
	/// <param name="aX"></param>
	/// <param name="aY"></param>
	public void QuadraticCurveTo(float aCPx, float aCPy, float aX, float aY)
	{
		RecordCall("quadraticCurveTo", aCPx, aCPy, aX, aY);
	}

	/// <summary>Points are added to the <c>curves</c> array as <c>LineCurves</c>.</summary>
	/// <param name="vectors">Value forwarded to the <c>vectors</c> argument.</param>
	public void SetFromPoints(Vector2[] vectors)
	{
		RecordCall("setFromPoints", (object?) vectors);
	}

	/// <summary>Connects a new <c>SplineCurve</c> onto the path.</summary>
	/// <param name="pts">An array of <see cref="Vector2">Vector2's</see>.</param>
	public void SplineThru(Vector2[] pts)
	{
		RecordCall("splineThru", (object?) pts);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Path</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isCurrentPointWritten)
		{
			batch.Set(Handle, "currentPoint", ThreeValue.Encode(CurrentPoint));
		}
	}
}
