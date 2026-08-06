// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A IES version of <see cref="SpotLight"/>. Can only be used with <see cref="WebGPURenderer"/>.
/// The JavaScript-side <c>THREE.IESSpotLight</c>.
/// </summary>
public sealed class IESSpotLight : SpotLight
{
	private Texture? _iesMap = null;
	private bool _isIesMapWritten;

	/// <summary>Initializes a new <see cref="IESSpotLight"/>.</summary>
	public IESSpotLight()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>IESSpotLight</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal IESSpotLight(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.IESSpotLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "IESSpotLight"; }
	}

	/// <summary>
	/// The IES map. It's a lookup table that stores normalized attenuation factors (0.0 to 1.0) that
	/// represent the light's intensity at a specific angle. Writing it records a <c>iesMap</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Texture? IesMap
	{
		get { return _iesMap; }
		set
		{
			if (ReferenceEquals(_iesMap, value))
			{
				return;
			}

			_iesMap = value;
			_isIesMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("iesMap", value);
		}
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that. A replayed
	/// value that is itself a mirrored object is attached first, so its create op reaches the batch
	/// before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isIesMapWritten)
		{
			_iesMap?.AttachTo(batch);
			batch.Set(Handle, "iesMap", ThreeValue.Encode(_iesMap));
		}
	}
}
