// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Create a smooth **3D** spline curve from a series of points using the
/// <see href="https://en.wikipedia.org/wiki/Centripetal_Catmull-Rom_spline">Catmull-Rom</see>
/// algorithm. The JavaScript-side <c>THREE.CatmullRomCurve3</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl_geometry_extrude_splines">WebGL / geometry / extrude / splines</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/curves/CatmullRomCurve3">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/curves/CatmullRomCurve3.js">Source</seealso>
public sealed class CatmullRomCurve3 : ThreeObject
{
	private bool _closed = false;
	private float _tension;
	private int _arcLengthDivisions = 200;
	private bool _isClosedWritten;
	private bool _isTensionWritten;
	private bool _isArcLengthDivisionsWritten;

	/// <summary>This constructor creates a new <see cref="CatmullRomCurve3"/>.</summary>
	public CatmullRomCurve3()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CatmullRomCurve3</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CatmullRomCurve3"; }
	}

	/// <summary>
	/// The curve will loop back onto itself when this is true. Writing it records a <c>closed</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Closed
	{
		get { return _closed; }
		set
		{
			if (_closed == value)
			{
				return;
			}

			_closed = value;
			_isClosedWritten = true;
			RecordSet("closed", value);
		}
	}

	/// <summary>
	/// When <c>.curveType</c> is <c>catmullrom</c>, defines catmullrom's tension. Writing it records a
	/// <c>tension</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float Tension
	{
		get { return _tension; }
		set
		{
			if (_tension == value)
			{
				return;
			}

			_tension = value;
			_isTensionWritten = true;
			RecordSet("tension", value);
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
	/// Emits the create op for <c>THREE.CatmullRomCurve3</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isClosedWritten)
		{
			batch.Set(Handle, "closed", ThreeValue.Encode(_closed));
		}

		if (_isTensionWritten)
		{
			batch.Set(Handle, "tension", ThreeValue.Encode(_tension));
		}

		if (_isArcLengthDivisionsWritten)
		{
			batch.Set(Handle, "arcLengthDivisions", ThreeValue.Encode(_arcLengthDivisions));
		}
	}
}
