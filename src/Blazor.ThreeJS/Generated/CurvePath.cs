// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Curved Path - a curve path is simply a array of connected curves, but retains the api of a
/// curve. The JavaScript-side <c>THREE.CurvePath</c>.
/// </summary>
/// <remarks>
/// A <see cref="CurvePath"/> is simply an array of connected curves, but retains the api of a
/// curve.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/core/CurvePath">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/core/CurvePath.js">Source</seealso>
public class CurvePath : ThreeObject
{
	private bool _autoClose = false;
	private int _arcLengthDivisions = 200;
	private bool _isAutoCloseWritten;
	private bool _isArcLengthDivisionsWritten;

	/// <summary>The constructor take no parameters.</summary>
	public CurvePath()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CurvePath</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CurvePath"; }
	}

	/// <summary>
	/// Whether or not to automatically close the path. Writing it records a <c>autoClose</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool AutoClose
	{
		get { return _autoClose; }
		set
		{
			if (_autoClose == value)
			{
				return;
			}

			_autoClose = value;
			_isAutoCloseWritten = true;
			RecordSet("autoClose", value);
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
	/// Emits the create op for <c>THREE.CurvePath</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isAutoCloseWritten)
		{
			batch.Set(Handle, "autoClose", ThreeValue.Encode(_autoClose));
		}

		if (_isArcLengthDivisionsWritten)
		{
			batch.Set(Handle, "arcLengthDivisions", ThreeValue.Encode(_arcLengthDivisions));
		}
	}
}
