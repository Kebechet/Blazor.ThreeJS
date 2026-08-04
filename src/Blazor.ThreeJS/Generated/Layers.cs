// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A <c>Layers</c> object assigns an <c>Object3D</c> to 1 or more of 32 layers numbered <c>0</c> to
/// <c>31</c> - internally the layers are stored as a
/// <see href="https://en.wikipedia.org/wiki/Mask_(computing)">bit mask</see>, and by default all
/// Object3Ds are a member of layer <c>0</c>. The JavaScript-side <c>THREE.Layers</c>.
/// </summary>
/// <remarks>
/// This can be used to control visibility - an object must share a layer with a
/// <see cref="Camera">camera</see> to be visible when that camera's view is rendered. All classes
/// that inherit from <c>Object3D</c> have an <c>Object3D.layers</c> property which is an instance
/// of this class.
/// </remarks>
/// <seealso href="https://threejs.org/examples/#webgl_layers">WebGL / layers</seealso>
/// <seealso href="https://threejs.org/examples/#webxr_vr_layers">Webxr / vr / layers</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/Layers">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/Layers.js">Source</seealso>
public sealed class Layers : ThreeObject
{
	private int _mask;
	private bool _isMaskWritten;

	/// <summary>Create a new Layers object, with membership initially set to layer 0.</summary>
	public Layers()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Layers</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Layers"; }
	}

	/// <summary>
	/// A bit mask storing which of the 32 layers this layers object is currently a member of. Writing
	/// it records a <c>mask</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public int Mask
	{
		get { return _mask; }
		set
		{
			if (_mask == value)
			{
				return;
			}

			_mask = value;
			_isMaskWritten = true;
			RecordSet("mask", value);
		}
	}

	/// <summary>Set membership to <c>layer</c>, and remove membership all other layers.</summary>
	/// <param name="layer">An integer from 0 to 31.</param>
	public void Set(float layer)
	{
		RecordCall("set", layer);
	}

	/// <summary>Add membership of this <c>layer</c>.</summary>
	/// <param name="layer">An integer from 0 to 31.</param>
	public void Enable(float layer)
	{
		RecordCall("enable", layer);
	}

	/// <summary>Add membership to all layers.</summary>
	public void EnableAll()
	{
		RecordCall("enableAll");
	}

	/// <summary>Toggle membership of <c>layer</c>.</summary>
	/// <param name="layer">An integer from 0 to 31.</param>
	public void Toggle(float layer)
	{
		RecordCall("toggle", layer);
	}

	/// <summary>Remove membership of this <c>layer</c>.</summary>
	/// <param name="layer">An integer from 0 to 31.</param>
	public void Disable(float layer)
	{
		RecordCall("disable", layer);
	}

	/// <summary>Remove membership from all layers.</summary>
	public void DisableAll()
	{
		RecordCall("disableAll");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Layers</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isMaskWritten)
		{
			batch.Set(Handle, "mask", ThreeValue.Encode(_mask));
		}
	}
}
