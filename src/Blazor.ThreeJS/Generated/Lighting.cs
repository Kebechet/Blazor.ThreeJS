// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This renderer module manages the lights nodes which are unique per scene and camera combination.
/// The lights node itself is later configured in the render list with the actual lights from the
/// scene. The JavaScript-side <c>THREE.Lighting</c>.
/// </summary>
public sealed class Lighting : ThreeObject
{
	private bool _enabled = true;
	private bool _isEnabledWritten;

	/// <summary>Initializes a new <see cref="Lighting"/>.</summary>
	public Lighting()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Lighting</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Lighting"; }
	}

	/// <summary>
	/// Whether this lighting manager is enabled or not. Writing it records a <c>enabled</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Enabled
	{
		get { return _enabled; }
		set
		{
			if (_enabled == value)
			{
				return;
			}

			_enabled = value;
			_isEnabledWritten = true;
			RecordSet("enabled", value);
		}
	}

	/// <summary>
	/// Saves the current lights of the scene's lights node so they can be restored in
	/// <c>Lighting#finishRender</c>. Must be paired with a <c>finishRender()</c> call to avoid memory
	/// leaks. Nested render calls might mutate the lights array so a save/restore is required for each
	/// render call.
	/// </summary>
	/// <param name="scene">The scene.</param>
	public void BeginRender(Scene scene)
	{
		if (Batch is not null)
		{
			scene.AttachTo(Batch);
		}

		RecordCall("beginRender", scene);
	}

	/// <summary>Restores the lights saved by the matching <c>Lighting#beginRender</c> call.</summary>
	/// <param name="scene">The scene.</param>
	public void FinishRender(Scene scene)
	{
		if (Batch is not null)
		{
			scene.AttachTo(Batch);
		}

		RecordCall("finishRender", scene);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Lighting</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isEnabledWritten)
		{
			batch.Set(Handle, "enabled", ThreeValue.Encode(_enabled));
		}
	}
}
