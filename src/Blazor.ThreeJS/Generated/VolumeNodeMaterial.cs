// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>Volume node material. The JavaScript-side <c>THREE.VolumeNodeMaterial</c>.</summary>
public sealed class VolumeNodeMaterial : NodeMaterial
{
	private float _steps = 25f;
	private bool _isStepsWritten;

	/// <summary>Constructs a new volume node material.</summary>
	public VolumeNodeMaterial()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.VolumeNodeMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "VolumeNodeMaterial"; }
	}

	/// <summary>
	/// Number of steps used for raymarching. Writing it records a <c>steps</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float Steps
	{
		get { return _steps; }
		set
		{
			if (_steps == value)
			{
				return;
			}

			_steps = value;
			_isStepsWritten = true;
			RecordSet("steps", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.VolumeNodeMaterial</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isStepsWritten)
		{
			batch.Set(Handle, "steps", ThreeValue.Encode(_steps));
		}
	}
}
