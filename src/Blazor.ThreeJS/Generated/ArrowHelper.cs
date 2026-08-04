// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>An 3D arrow object for visualizing directions. The JavaScript-side <c>THREE.ArrowHelper</c>.</summary>
public sealed class ArrowHelper : Object3D
{
	private readonly Vector3? _dir;
	private readonly Vector3? _origin;
	private readonly float _length;
	private readonly Color? _color;
	private readonly float? _headLength;
	private readonly float? _headWidth;
	private Line? _line;
	private Mesh? _cone;
	private bool _isLineWritten;
	private bool _isConeWritten;

	/// <summary>Constructs a new arrow helper.</summary>
	/// <param name="dir">The (normalized) direction vector.</param>
	/// <param name="origin">Point at which the arrow starts.</param>
	/// <param name="length">Length of the arrow in world units.</param>
	/// <param name="color">Color of the arrow.</param>
	/// <param name="headLength">times 0.2] - The length of the head of the arrow.</param>
	/// <param name="headWidth">times 0.2] - The width of the head of the arrow.</param>
	public ArrowHelper(
		Vector3? dir = null,
		Vector3? origin = null,
		float length = 1f,
		Color? color = null,
		float? headLength = null,
		float? headWidth = null)
	{
		_dir = dir;
		_origin = origin;
		_length = length;
		_color = color;
		_headLength = headLength;
		_headWidth = headWidth;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ArrowHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ArrowHelper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ArrowHelper</c>: dir, origin, length, color,
	/// headLength, headWidth. An argument the caller left unspecified travels as the wire's
	/// not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js applies its
	/// own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_dir),
				ThreeValue.OrUnspecified(_origin),
				_length,
				ThreeValue.OrUnspecified(_color),
				ThreeValue.OrUnspecified(_headLength),
				ThreeValue.OrUnspecified(_headWidth)
			]);
		}
	}

	/// <summary>
	/// The line part of the arrow helper. Writing it records a <c>line</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public Line? Line
	{
		get { return _line; }
		set
		{
			if (ReferenceEquals(_line, value))
			{
				return;
			}

			_line = value;
			_isLineWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("line", value);
		}
	}

	/// <summary>
	/// The cone part of the arrow helper. Writing it records a <c>cone</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public Mesh? Cone
	{
		get { return _cone; }
		set
		{
			if (ReferenceEquals(_cone, value))
			{
				return;
			}

			_cone = value;
			_isConeWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("cone", value);
		}
	}

	/// <summary>Sets the direction of the helper.</summary>
	/// <param name="dir">The normalized direction vector.</param>
	public void SetDirection(Vector3 dir)
	{
		RecordCall("setDirection", dir);
	}

	/// <summary>Sets the length of the helper.</summary>
	/// <param name="length">Length of the arrow in world units.</param>
	/// <param name="headLength">times 0.2] - The length of the head of the arrow.</param>
	/// <param name="headWidth">times 0.2] - The width of the head of the arrow.</param>
	public void SetLength(float length, float headLength, float headWidth)
	{
		RecordCall("setLength", length, headLength, headWidth);
	}

	/// <summary>Sets the color of the helper.</summary>
	/// <param name="color">The color to set.</param>
	public void SetColor(Color color)
	{
		RecordCall("setColor", color);
	}

	/// <summary>
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isLineWritten)
		{
			batch.Set(Handle, "line", ThreeValue.Encode(_line));
		}

		if (_isConeWritten)
		{
			batch.Set(Handle, "cone", ThreeValue.Encode(_cone));
		}
	}
}
