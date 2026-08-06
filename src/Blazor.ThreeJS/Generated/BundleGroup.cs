// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A specialized group which enables applications access to the Render Bundle API of WebGPU. The
/// group with all its descendant nodes are considered as one render bundle and processed as such by
/// the renderer. This module is only fully supported by <c>WebGPURenderer</c> with a WebGPU
/// backend. With a WebGL backend, the group can technically be rendered but without any performance
/// improvements. The JavaScript-side <c>THREE.BundleGroup</c>.
/// </summary>
public sealed class BundleGroup : Group
{
	private bool _needsUpdate = false;
	private bool _isNeedsUpdateWritten;

	/// <summary>Initializes a new <see cref="BundleGroup"/>.</summary>
	public BundleGroup()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>BundleGroup</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal BundleGroup(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BundleGroup</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "BundleGroup"; }
	}

	/// <summary>
	/// Set this property to <c>true</c> when the bundle group has changed. Writing it records a
	/// <c>needsUpdate</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool NeedsUpdate
	{
		get { return _needsUpdate; }
		set
		{
			if (_needsUpdate == value)
			{
				return;
			}

			_needsUpdate = value;
			_isNeedsUpdateWritten = true;
			RecordSet("needsUpdate", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isBundleGroup</c> held.
	/// </summary>
	/// <returns>The value <c>isBundleGroup</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsBundleGroupAsync()
	{
		return GetAsync<bool>("isBundleGroup");
	}

	/// <summary>
	/// The bundle group's version. Read-only in three.js, so it is read on demand rather than mirrored:
	/// records a get op, sends it behind every write already pending, and completes with the value
	/// <c>version</c> held.
	/// </summary>
	/// <returns>The value <c>version</c> held, once the JavaScript side has answered.</returns>
	public Task<float> VersionAsync()
	{
		return GetAsync<float>("version");
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

		if (_isNeedsUpdateWritten)
		{
			batch.Set(Handle, "needsUpdate", ThreeValue.Encode(_needsUpdate));
		}
	}
}
