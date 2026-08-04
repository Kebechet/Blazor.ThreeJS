// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class is used to convert a series of paths to an array of shapes. It is specifically used
/// in context of fonts and SVG. The JavaScript-side <c>THREE.ShapePath</c>.
/// </summary>
public sealed class ShapePath : ThreeObject
{
	private string _type = string.Empty;
	private Path? _currentPath = null;
	private bool _isTypeWritten;
	private bool _isColorWritten;
	private bool _isCurrentPathWritten;

	/// <summary>
	/// The color of the shape. Mirrored as an instance this object owns: mutating it records a write of
	/// <c>color</c>.
	/// </summary>
	public Color Color { get; }

	/// <summary>Initializes a new <see cref="ShapePath"/>.</summary>
	public ShapePath()
	{
		Color = new Color();
		Color.OnChange = () =>
		{
			_isColorWritten = true;
			RecordSet("color", Color);
		};
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ShapePath</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ShapePath"; }
	}

	/// <summary>
	/// The <c>type</c> property of the JavaScript-side object. Writing it records a <c>type</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public string Type
	{
		get { return _type; }
		set
		{
			if (_type == value)
			{
				return;
			}

			_type = value;
			_isTypeWritten = true;
			RecordSet("type", value);
		}
	}

	/// <summary>
	/// The current path that is being generated. Writing it records a <c>currentPath</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Path? CurrentPath
	{
		get { return _currentPath; }
		set
		{
			if (ReferenceEquals(_currentPath, value))
			{
				return;
			}

			_currentPath = value;
			_isCurrentPathWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("currentPath", value);
		}
	}

	/// <summary>Creates a new path and moves it current point to the given one.</summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void MoveTo(float x, float y)
	{
		RecordCall("moveTo", x, y);
	}

	/// <summary>
	/// Adds an instance of <see cref="LineCurve"/> to the path by connecting the current point with the
	/// given one.
	/// </summary>
	/// <param name="x">The x coordinate of the end point.</param>
	/// <param name="y">The y coordinate of the end point.</param>
	public void LineTo(float x, float y)
	{
		RecordCall("lineTo", x, y);
	}

	/// <summary>
	/// Adds an instance of <see cref="QuadraticBezierCurve"/> to the path by connecting the current
	/// point with the given one.
	/// </summary>
	/// <param name="aCPx">The x coordinate of the control point.</param>
	/// <param name="aCPy">The y coordinate of the control point.</param>
	/// <param name="aX">The x coordinate of the end point.</param>
	/// <param name="aY">The y coordinate of the end point.</param>
	public void QuadraticCurveTo(float aCPx, float aCPy, float aX, float aY)
	{
		RecordCall("quadraticCurveTo", aCPx, aCPy, aX, aY);
	}

	/// <summary>
	/// Adds an instance of <see cref="CubicBezierCurve"/> to the path by connecting the current point
	/// with the given one.
	/// </summary>
	/// <param name="aCP1x">The x coordinate of the first control point.</param>
	/// <param name="aCP1y">The y coordinate of the first control point.</param>
	/// <param name="aCP2x">The x coordinate of the second control point.</param>
	/// <param name="aCP2y">The y coordinate of the second control point.</param>
	/// <param name="aX">The x coordinate of the end point.</param>
	/// <param name="aY">The y coordinate of the end point.</param>
	public void BezierCurveTo(float aCP1x, float aCP1y, float aCP2x, float aCP2y, float aX, float aY)
	{
		RecordCall("bezierCurveTo", aCP1x, aCP1y, aCP2x, aCP2y, aX, aY);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.ShapePath</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isTypeWritten)
		{
			batch.Set(Handle, "type", ThreeValue.Encode(_type));
		}

		if (_isColorWritten)
		{
			batch.Set(Handle, "color", ThreeValue.Encode(Color));
		}

		if (_isCurrentPathWritten)
		{
			batch.Set(Handle, "currentPath", ThreeValue.Encode(_currentPath));
		}
	}
}
