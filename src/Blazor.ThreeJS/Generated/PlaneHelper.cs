// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A helper object to visualize an instance of <see cref="Plane"/>. The JavaScript-side
/// <c>THREE.PlaneHelper</c>.
/// </summary>
public sealed class PlaneHelper : Line
{
	private readonly Plane _plane;
	private float _size;
	private readonly Color? _hex;
	private bool _isSizeWritten;

	/// <summary>Constructs a new plane helper.</summary>
	/// <param name="plane">The plane to be visualized.</param>
	/// <param name="size">The side length of plane helper.</param>
	/// <param name="hex">The helper's color.</param>
	public PlaneHelper(Plane plane, float size = 1f, Color? hex = null)
	{
		_plane = plane;
		_size = size;
		_hex = hex;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>PlaneHelper</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal PlaneHelper(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_plane = default!;
		_size = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PlaneHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PlaneHelper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.PlaneHelper</c>: plane, size, hex. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([_plane, _size, ThreeValue.OrUnspecified(_hex)]); }
	}

	/// <summary>
	/// The side length of plane helper. Writing it records a <c>size</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float Size
	{
		get { return _size; }
		set
		{
			if (_size == value)
			{
				return;
			}

			_size = value;
			_isSizeWritten = true;
			RecordSet("size", value);
		}
	}

	/// <summary>Updates the helper to match the position and direction of the light being visualized.</summary>
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

		if (_isSizeWritten)
		{
			batch.Set(Handle, "size", ThreeValue.Encode(_size));
		}
	}
}
