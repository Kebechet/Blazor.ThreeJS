// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A special version of the Group object that defines clipping planes for descendant objects.
/// ClippingGroups can be nested, with clipping planes accumulating by type: intersection or union.
/// The JavaScript-side <c>THREE.ClippingGroup</c>.
/// </summary>
public sealed class ClippingGroup : Group
{
	private Plane[] _clippingPlanes = [];
	private bool _enabled;
	private bool _clipIntersection;
	private bool _clipShadows;
	private bool _isClippingPlanesWritten;
	private bool _isEnabledWritten;
	private bool _isClipIntersectionWritten;
	private bool _isClipShadowsWritten;

	/// <summary>Initializes a new <see cref="ClippingGroup"/>.</summary>
	public ClippingGroup()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>ClippingGroup</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal ClippingGroup(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ClippingGroup</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ClippingGroup"; }
	}

	/// <summary>
	/// User-defined clipping planes specified as THREE.Plane objects in world space. These planes apply
	/// to the objects that are children of this ClippingGroup. Points in space whose signed distance to
	/// the plane is negative are clipped (not rendered). See the webgpu_clipping example. Default is
	/// <c>[]</c>. Writing it records a <c>clippingPlanes</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public Plane[] ClippingPlanes
	{
		get { return _clippingPlanes; }
		set
		{
			if (_clippingPlanes == value)
			{
				return;
			}

			_clippingPlanes = value;
			_isClippingPlanesWritten = true;
			RecordSet("clippingPlanes", value);
		}
	}

	/// <summary>
	/// Determines if the clipping planes defined by this object are applied. Default is <c>true</c>.
	/// Writing it records a <c>enabled</c> property write once this object is attached; writing the
	/// value already held records nothing.
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
	/// Changes the behavior of clipping planes so that only their intersection is clipped, rather than
	/// their union. Default is <c>false</c>. Writing it records a <c>clipIntersection</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool ClipIntersection
	{
		get { return _clipIntersection; }
		set
		{
			if (_clipIntersection == value)
			{
				return;
			}

			_clipIntersection = value;
			_isClipIntersectionWritten = true;
			RecordSet("clipIntersection", value);
		}
	}

	/// <summary>
	/// Defines whether to clip shadows according to the clipping planes specified by this
	/// ClippingGroup. Default is <c>false</c>. Writing it records a <c>clipShadows</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool ClipShadows
	{
		get { return _clipShadows; }
		set
		{
			if (_clipShadows == value)
			{
				return;
			}

			_clipShadows = value;
			_isClipShadowsWritten = true;
			RecordSet("clipShadows", value);
		}
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type ClippingGroup. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isClippingGroup</c> held.
	/// </summary>
	/// <returns>The value <c>isClippingGroup</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsClippingGroupAsync()
	{
		return GetAsync<bool>("isClippingGroup");
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

		if (_isClippingPlanesWritten)
		{
			batch.Set(Handle, "clippingPlanes", ThreeValue.Encode(_clippingPlanes));
		}

		if (_isEnabledWritten)
		{
			batch.Set(Handle, "enabled", ThreeValue.Encode(_enabled));
		}

		if (_isClipIntersectionWritten)
		{
			batch.Set(Handle, "clipIntersection", ThreeValue.Encode(_clipIntersection));
		}

		if (_isClipShadowsWritten)
		{
			batch.Set(Handle, "clipShadows", ThreeValue.Encode(_clipShadows));
		}
	}
}
