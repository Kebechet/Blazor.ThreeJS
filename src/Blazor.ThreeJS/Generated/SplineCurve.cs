// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Create a smooth **2D** spline curve from a series of points. The JavaScript-side
/// <c>THREE.SplineCurve</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/curves/SplineCurve">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/curves/SplineCurve.js">Source</seealso>
public sealed class SplineCurve : ThreeObject
{
	private int _arcLengthDivisions = 200;
	private bool _isArcLengthDivisionsWritten;

	/// <summary>This constructor creates a new <see cref="SplineCurve"/>.</summary>
	public SplineCurve()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.SplineCurve</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "SplineCurve"; }
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
	/// Emits the create op for <c>THREE.SplineCurve</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isArcLengthDivisionsWritten)
		{
			batch.Set(Handle, "arcLengthDivisions", ThreeValue.Encode(_arcLengthDivisions));
		}
	}
}
