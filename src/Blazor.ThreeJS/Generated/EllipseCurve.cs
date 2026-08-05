// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>Creates a 2d curve in the shape of an ellipse. The JavaScript-side <c>THREE.EllipseCurve</c>.</summary>
/// <remarks>Setting the <c>xRadius</c> equal to the <c>yRadius</c> will result in a circle.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/curves/EllipseCurve">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/curves/EllipseCurve.js">Source</seealso>
public class EllipseCurve : ThreeObject
{
	private float _aX;
	private float _aY;
	private float _xRadius;
	private float _yRadius;
	private float _aStartAngle;
	private float? _aEndAngle;
	private bool _aClockwise;
	private float _aRotation;
	private int _arcLengthDivisions = 200;
	private bool _isAXWritten;
	private bool _isAYWritten;
	private bool _isXRadiusWritten;
	private bool _isYRadiusWritten;
	private bool _isAStartAngleWritten;
	private bool _isAEndAngleWritten;
	private bool _isAClockwiseWritten;
	private bool _isARotationWritten;
	private bool _isArcLengthDivisionsWritten;

	/// <summary>This constructor creates a new <see cref="EllipseCurve"/>.</summary>
	/// <param name="aX">The X center of the ellipse. Expects a <c>Float</c>. Default is <c>0</c>.</param>
	/// <param name="aY">The Y center of the ellipse. Expects a <c>Float</c>. Default is <c>0</c>.</param>
	/// <param name="xRadius">
	/// The radius of the ellipse in the x direction. Expects a <c>Float</c>. Default is <c>1</c>.
	/// </param>
	/// <param name="yRadius">
	/// The radius of the ellipse in the y direction. Expects a <c>Float</c>. Default is <c>1</c>.
	/// </param>
	/// <param name="aStartAngle">
	/// The start angle of the curve in radians starting from the positive X axis. Default is <c>0</c>.
	/// </param>
	/// <param name="aEndAngle">
	/// The end angle of the curve in radians starting from the positive X axis. Default is <c>2 x
	/// Math.PI</c>.
	/// </param>
	/// <param name="aClockwise">Whether the ellipse is drawn clockwise. Default is <c>false</c>.</param>
	/// <param name="aRotation">
	/// The rotation angle of the ellipse in radians, counterclockwise from the positive X axis. Default
	/// is <c>0</c>.
	/// </param>
	public EllipseCurve(
		float aX = 0f,
		float aY = 0f,
		float xRadius = 1f,
		float yRadius = 1f,
		float aStartAngle = 0f,
		float? aEndAngle = null,
		bool aClockwise = false,
		float aRotation = 0f)
	{
		_aX = aX;
		_aY = aY;
		_xRadius = xRadius;
		_yRadius = yRadius;
		_aStartAngle = aStartAngle;
		_aEndAngle = aEndAngle;
		_aClockwise = aClockwise;
		_aRotation = aRotation;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.EllipseCurve</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "EllipseCurve"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.EllipseCurve</c>: aX, aY, xRadius, yRadius,
	/// aStartAngle, aEndAngle, aClockwise, aRotation. An argument the caller left unspecified travels
	/// as the wire's not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js
	/// applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_aX,
				_aY,
				_xRadius,
				_yRadius,
				_aStartAngle,
				ThreeValue.OrUnspecified(_aEndAngle),
				_aClockwise,
				_aRotation
			]);
		}
	}

	/// <summary>
	/// The X center of the ellipse. Writing it records a <c>aX</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float AX
	{
		get { return _aX; }
		set
		{
			if (_aX == value)
			{
				return;
			}

			_aX = value;
			_isAXWritten = true;
			RecordSet("aX", value);
		}
	}

	/// <summary>
	/// The Y center of the ellipse. Writing it records a <c>aY</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float AY
	{
		get { return _aY; }
		set
		{
			if (_aY == value)
			{
				return;
			}

			_aY = value;
			_isAYWritten = true;
			RecordSet("aY", value);
		}
	}

	/// <summary>
	/// The radius of the ellipse in the x direction. Writing it records a <c>xRadius</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float XRadius
	{
		get { return _xRadius; }
		set
		{
			if (_xRadius == value)
			{
				return;
			}

			_xRadius = value;
			_isXRadiusWritten = true;
			RecordSet("xRadius", value);
		}
	}

	/// <summary>
	/// The radius of the ellipse in the y direction. Writing it records a <c>yRadius</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float YRadius
	{
		get { return _yRadius; }
		set
		{
			if (_yRadius == value)
			{
				return;
			}

			_yRadius = value;
			_isYRadiusWritten = true;
			RecordSet("yRadius", value);
		}
	}

	/// <summary>
	/// The start angle of the curve in radians starting from the middle right side. Writing it records
	/// a <c>aStartAngle</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float AStartAngle
	{
		get { return _aStartAngle; }
		set
		{
			if (_aStartAngle == value)
			{
				return;
			}

			_aStartAngle = value;
			_isAStartAngleWritten = true;
			RecordSet("aStartAngle", value);
		}
	}

	/// <summary>
	/// The end angle of the curve in radians starting from the middle right side. Writing it records a
	/// <c>aEndAngle</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float? AEndAngle
	{
		get { return _aEndAngle; }
		set
		{
			if (_aEndAngle == value)
			{
				return;
			}

			_aEndAngle = value;
			_isAEndAngleWritten = true;
			RecordSet("aEndAngle", value);
		}
	}

	/// <summary>
	/// Whether the ellipse is drawn clockwise. Writing it records a <c>aClockwise</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool AClockwise
	{
		get { return _aClockwise; }
		set
		{
			if (_aClockwise == value)
			{
				return;
			}

			_aClockwise = value;
			_isAClockwiseWritten = true;
			RecordSet("aClockwise", value);
		}
	}

	/// <summary>
	/// The rotation angle of the ellipse in radians, counterclockwise from the positive X axis
	/// (optional). Writing it records a <c>aRotation</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public float ARotation
	{
		get { return _aRotation; }
		set
		{
			if (_aRotation == value)
			{
				return;
			}

			_aRotation = value;
			_isARotationWritten = true;
			RecordSet("aRotation", value);
		}
	}

	/// <summary>
	/// This value determines the amount of divisions when calculating the cumulative segment lengths of
	/// a <c>Curve</c> via <c>.getLengths</c>. To ensure precision when using methods like
	/// <c>.getSpacedPoints</c>, it is recommended to increase <c>.arcLengthDivisions</c> if the
	/// <c>Curve</c> is very large. Writing it records a <c>arcLengthDivisions</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public int ArcLengthDivisions
	{
		get { return _arcLengthDivisions; }
		set
		{
			if (_arcLengthDivisions == value)
			{
				return;
			}

			_arcLengthDivisions = value;
			_isArcLengthDivisionsWritten = true;
			RecordSet("arcLengthDivisions", value);
		}
	}

	/// <summary>Update the cumulative segment distance cache.</summary>
	public void UpdateArcLengths()
	{
		RecordCall("updateArcLengths");
	}

	/// <summary>
	/// Get total <c>Curve</c> arc length. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getLength</c> returned.
	/// </summary>
	/// <returns>The value <c>getLength</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetLengthAsync()
	{
		return RecordRead<float>("getLength");
	}

	/// <summary>
	/// Given u in the range <c>[ 0, 1 ]</c>,. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getUtoTmapping</c> returned.
	/// </summary>
	/// <param name="u"></param>
	/// <param name="distance"></param>
	/// <returns>The value <c>getUtoTmapping</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetUtoTmappingAsync(float u, float distance)
	{
		return RecordRead<float>("getUtoTmapping", u, distance);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.EllipseCurve</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isAXWritten)
		{
			batch.Set(Handle, "aX", ThreeValue.Encode(_aX));
		}

		if (_isAYWritten)
		{
			batch.Set(Handle, "aY", ThreeValue.Encode(_aY));
		}

		if (_isXRadiusWritten)
		{
			batch.Set(Handle, "xRadius", ThreeValue.Encode(_xRadius));
		}

		if (_isYRadiusWritten)
		{
			batch.Set(Handle, "yRadius", ThreeValue.Encode(_yRadius));
		}

		if (_isAStartAngleWritten)
		{
			batch.Set(Handle, "aStartAngle", ThreeValue.Encode(_aStartAngle));
		}

		if (_isAEndAngleWritten)
		{
			batch.Set(Handle, "aEndAngle", ThreeValue.Encode(_aEndAngle));
		}

		if (_isAClockwiseWritten)
		{
			batch.Set(Handle, "aClockwise", ThreeValue.Encode(_aClockwise));
		}

		if (_isARotationWritten)
		{
			batch.Set(Handle, "aRotation", ThreeValue.Encode(_aRotation));
		}

		if (_isArcLengthDivisionsWritten)
		{
			batch.Set(Handle, "arcLengthDivisions", ThreeValue.Encode(_arcLengthDivisions));
		}
	}
}
