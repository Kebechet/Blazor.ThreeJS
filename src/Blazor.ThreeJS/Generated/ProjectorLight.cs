// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A projector light version of <see cref="SpotLight"/>. Can only be used with
/// <see cref="WebGPURenderer"/>. The JavaScript-side <c>THREE.ProjectorLight</c>.
/// </summary>
public sealed class ProjectorLight : SpotLight
{
	private float? _aspect = null;
	private bool _isAspectWritten;

	/// <summary>Initializes a new <see cref="ProjectorLight"/>.</summary>
	public ProjectorLight()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ProjectorLight</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ProjectorLight(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ProjectorLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ProjectorLight"; }
	}

	/// <summary>
	/// Aspect ratio of the light. Set to <c>null</c> to use the texture aspect ratio. Writing it
	/// records a <c>aspect</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float? Aspect
	{
		get { return _aspect; }
		set
		{
			if (_aspect == value)
			{
				return;
			}

			_aspect = value;
			_isAspectWritten = true;
			RecordSet("aspect", value);
		}
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

		if (_isAspectWritten)
		{
			batch.Set(Handle, "aspect", ThreeValue.Encode(_aspect));
		}
	}
}
