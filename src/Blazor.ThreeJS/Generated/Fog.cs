// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class can be used to define a linear fog that grows linearly denser with the distance. The
/// JavaScript-side <c>THREE.Fog</c>.
/// </summary>
public sealed class Fog : ThreeObject
{
	private readonly Color _color;
	private float _near;
	private float _far;
	private string _name = string.Empty;
	private bool _isNameWritten;
	private bool _isNearWritten;
	private bool _isFarWritten;

	/// <summary>Constructs a new fog.</summary>
	/// <param name="color">The fog's color.</param>
	/// <param name="near">The minimum distance to start applying fog.</param>
	/// <param name="far">The maximum distance at which fog stops being calculated and applied.</param>
	public Fog(Color color, float near = 1f, float far = 1000f)
	{
		_color = color;
		_near = near;
		_far = far;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Fog</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Fog(ThreeBatch batch, int handle)
		: base(handle)
	{
		_color = default!;
		_near = default!;
		_far = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Fog</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Fog"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.Fog</c>: color, near, far.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_color, _near, _far]; }
	}

	/// <summary>
	/// The name of the fog. Writing it records a <c>name</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public string Name
	{
		get { return _name; }
		set
		{
			if (_name == value)
			{
				return;
			}

			_name = value;
			_isNameWritten = true;
			RecordSet("name", value);
		}
	}

	/// <summary>
	/// The minimum distance to start applying fog. Objects that are less than <c>near</c> units from
	/// the active camera won't be affected by fog. Writing it records a <c>near</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Near
	{
		get { return _near; }
		set
		{
			if (_near == value)
			{
				return;
			}

			_near = value;
			_isNearWritten = true;
			RecordSet("near", value);
		}
	}

	/// <summary>
	/// The maximum distance at which fog stops being calculated and applied. Objects that are more than
	/// <c>far</c> units away from the active camera won't be affected by fog. Writing it records a
	/// <c>far</c> property write once this object is attached; writing the value already held records
	/// nothing.
	/// </summary>
	public float Far
	{
		get { return _far; }
		set
		{
			if (_far == value)
			{
				return;
			}

			_far = value;
			_isFarWritten = true;
			RecordSet("far", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isFog</c> held.
	/// </summary>
	/// <returns>The value <c>isFog</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsFogAsync()
	{
		return GetAsync<bool>("isFog");
	}

	/// <summary>
	/// Returns a new fog with copied values from this instance. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<Fog?> CloneAsync()
	{
		return RecordReadObject<Fog>("clone", (adoptedBatch, adoptedHandle) => new Fog(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Fog</c>, then replays every property written before this object
	/// was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isNearWritten)
		{
			batch.Set(Handle, "near", ThreeValue.Encode(_near));
		}

		if (_isFarWritten)
		{
			batch.Set(Handle, "far", ThreeValue.Encode(_far));
		}
	}
}
