// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class can be used to define an exponential squared fog, which gives a clear view near the
/// camera and a faster than exponentially densening fog farther from the camera. The
/// JavaScript-side <c>THREE.FogExp2</c>.
/// </summary>
public sealed class FogExp2 : ThreeObject
{
	private readonly Color _color;
	private float _density;
	private string _name = string.Empty;
	private bool _isNameWritten;
	private bool _isDensityWritten;

	/// <summary>Constructs a new fog.</summary>
	/// <param name="color">The fog's color.</param>
	/// <param name="density">Defines how fast the fog will grow dense.</param>
	public FogExp2(Color color, float density = 0.00025f)
	{
		_color = color;
		_density = density;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>FogExp2</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal FogExp2(ThreeBatch batch, int handle)
		: base(handle)
	{
		_color = default!;
		_density = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.FogExp2</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "FogExp2"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.FogExp2</c>: color, density.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_color, _density]; }
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
	/// Defines how fast the fog will grow dense. Writing it records a <c>density</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Density
	{
		get { return _density; }
		set
		{
			if (_density == value)
			{
				return;
			}

			_density = value;
			_isDensityWritten = true;
			RecordSet("density", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isFogExp2</c> held.
	/// </summary>
	/// <returns>The value <c>isFogExp2</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsFogExp2Async()
	{
		return GetAsync<bool>("isFogExp2");
	}

	/// <summary>
	/// Returns a new fog with copied values from this instance. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<FogExp2?> CloneAsync()
	{
		return RecordReadObject<FogExp2>("clone", (adoptedBatch, adoptedHandle) => new FogExp2(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Serializes the fog into JSON. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>toJSON</c> returned.
	/// </summary>
	/// <returns>The value <c>toJSON</c> returned, once the JavaScript side has answered.</returns>
	public Task<FogExp2JSON> ToJSONAsync()
	{
		return RecordRead<FogExp2JSON>("toJSON");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.FogExp2</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isDensityWritten)
		{
			batch.Set(Handle, "density", ThreeValue.Encode(_density));
		}
	}
}
