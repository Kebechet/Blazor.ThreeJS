// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates a three-dimensional texture from raw data, with parameters to divide it into width,
/// height, and depth. The JavaScript-side <c>THREE.Data3DTexture</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl2_materials_texture3d">WebGL2 / materials / texture3d</seealso>
/// <seealso href="https://threejs.org/examples/#webgl2_materials_texture3d_partialupdate">WebGL2 / materials / texture3d / partialupdate</seealso>
/// <seealso href="https://threejs.org/examples/#webgl2_volume_cloud">WebGL2 / volume / cloud</seealso>
/// <seealso href="https://threejs.org/examples/#webgl2_volume_perlin">WebGL2 / volume / perlin</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/textures/Data3DTexture">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/textures/Data3DTexture.js">Source</seealso>
public sealed class Data3DTexture : Texture
{
	private readonly TypedArray? _data;
	private readonly float _width;
	private readonly float _height;
	private readonly float _depth;
	private Wrapping _wrapR;
	private bool _isWrapRWritten;

	/// <summary>Create a new instance of <see cref="Data3DTexture"/>.</summary>
	/// <param name="data">
	/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/ArrayBufferView">ArrayBufferView</see>
	/// of the texture.
	/// </param>
	/// <param name="width">Width of the texture.</param>
	/// <param name="height">Height of the texture.</param>
	/// <param name="depth">Depth of the texture.</param>
	public Data3DTexture(TypedArray? data = null, float width = 1f, float height = 1f, float depth = 1f)
	{
		_data = data;
		_width = width;
		_height = height;
		_depth = depth;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Data3DTexture</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Data3DTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_width = default!;
		_height = default!;
		_depth = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Data3DTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Data3DTexture"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Data3DTexture</c>: data, width, height, depth. An
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
	public Wrapping WrapR
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
	/// Read-only flag to check if a given object is of type <see cref="Data3DTexture"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isData3DTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isData3DTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsData3DTextureAsync()
	{
		return GetAsync<bool>("isData3DTexture");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Data3DTexture</c>, then replays every property written before
	/// this object was attached.
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
