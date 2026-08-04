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
