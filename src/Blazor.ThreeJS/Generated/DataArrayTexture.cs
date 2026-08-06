// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates an array of textures directly from raw data, width and height and depth. The
/// JavaScript-side <c>THREE.DataArrayTexture</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl2_materials_texture2darray">WebGL2 / materials / texture2darray</seealso>
/// <seealso href="https://threejs.org/examples/#webgl2_rendertarget_texture2darray">WebGL2 / rendertarget / texture2darray</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/textures/DataArrayTexture">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/textures/DataArrayTexture.js">Source</seealso>
public sealed class DataArrayTexture : Texture
{
	private readonly TypedArray? _data;
	private readonly float _width;
	private readonly float _height;
	private readonly float _depth;
	private bool _wrapR;
	private bool _isWrapRWritten;

	/// <summary>This creates a new <c>DataArrayTexture</c> object.</summary>
	/// <param name="data">
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/ArrayBufferView">ArrayBufferView</see>
	/// of the texture.
	/// </param>
	/// <param name="width">Width of the texture.</param>
	/// <param name="height">Height of the texture.</param>
	/// <param name="depth">Depth of the texture.</param>
	public DataArrayTexture(TypedArray? data = null, float width = 1f, float height = 1f, float depth = 1f)
	{
		_data = data;
		_width = width;
		_height = height;
		_depth = depth;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>DataArrayTexture</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal DataArrayTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_width = default!;
		_height = default!;
		_depth = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.DataArrayTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "DataArrayTexture"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.DataArrayTexture</c>: data, width, height, depth. An
	/// argument the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed
	/// when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_data), _width, _height, _depth]); }
	}

	/// <summary>
	/// The <c>wrapR</c> property of the JavaScript-side object. Writing it records a <c>wrapR</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool WrapR
	{
		get { return _wrapR; }
		set
		{
			if (_wrapR == value)
			{
				return;
			}

			_wrapR = value;
			_isWrapRWritten = true;
			RecordSet("wrapR", value);
		}
	}

	/// <summary>
	/// Describes that a specific layer of the texture needs to be updated. Normally when
	/// <c>Texture.needsUpdate</c> is set to true, the entire compressed texture array is sent to the
	/// GPU. Marking specific layers will only transmit subsets of all mipmaps associated with a
	/// specific depth in the array which is often much more performant.
	/// </summary>
	/// <param name="layerIndex">Value forwarded to the <c>layerIndex</c> argument.</param>
	public void AddLayerUpdate(int layerIndex)
	{
		RecordCall("addLayerUpdate", layerIndex);
	}

	/// <summary>Resets the layer updates registry. See <c>DataArrayTexture.addLayerUpdate</c>.</summary>
	public void ClearLayerUpdates()
	{
		RecordCall("clearLayerUpdates");
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="DataArrayTexture"/>. Read-only
	/// in three.js, so it is read on demand rather than mirrored: records a get op, sends it behind
	/// every write already pending, and completes with the value <c>isDataArrayTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isDataArrayTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsDataArrayTextureAsync()
	{
		return GetAsync<bool>("isDataArrayTexture");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.DataArrayTexture</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isWrapRWritten)
		{
			batch.Set(Handle, "wrapR", ThreeValue.Encode(_wrapR));
		}
	}
}
