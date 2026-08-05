// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Every level is associated with an object, and rendering can be switched between them at the
/// distances specified. The JavaScript-side <c>THREE.LOD</c>.
/// </summary>
/// <remarks>
/// Typically you would create, say, three meshes, one for far away (low detail), one for mid range
/// (medium detail) and one for close up (high detail).
/// </remarks>
/// <seealso href="https://threejs.org/examples/#webgl_lod">webgl / {@link LOD</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/LOD">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/LOD.js">Source</seealso>
public sealed class LOD : Object3D
{
	private bool _autoUpdate = true;
	private bool _isAutoUpdateWritten;

	/// <summary>Creates a new <see cref="LOD"/>.</summary>
	public LOD()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LOD</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LOD"; }
	}

	/// <summary>
	/// Whether the <see cref="LOD"/> object is updated automatically by the renderer per frame or not.
	/// If set to <c>false</c>, you have to call <c>.update()</c> in the render loop by yourself.
	/// Writing it records a <c>autoUpdate</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public bool AutoUpdate
	{
		get { return _autoUpdate; }
		set
		{
			if (_autoUpdate == value)
			{
				return;
			}

			_autoUpdate = value;
			_isAutoUpdateWritten = true;
			RecordSet("autoUpdate", value);
		}
	}

	/// <summary>
	/// Adds a mesh that will display at a certain distance and greater. Typically the further away the
	/// distance, the lower the detail on the mesh.
	/// </summary>
	/// <param name="object">The Object3D to display at this level.</param>
	/// <param name="distance">The distance at which to display this level of detail.</param>
	/// <param name="hysteresis">
	/// Threshold used to avoid flickering at LOD boundaries, as a fraction of distance.
	/// </param>
	public void AddLevel(Object3D @object, float distance = 0f, float hysteresis = 0f)
	{
		RecordCall("addLevel", @object, distance, hysteresis);
	}

	/// <summary>
	/// Set the visibility of each <c>level</c>'s <c>object</c> based on distance from the
	/// <c>camera</c>.
	/// </summary>
	/// <param name="camera">Value forwarded to the <c>camera</c> argument.</param>
	public void Update(Camera camera)
	{
		RecordCall("update", camera);
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

		if (_isAutoUpdateWritten)
		{
			batch.Set(Handle, "autoUpdate", ThreeValue.Encode(_autoUpdate));
		}
	}
}
