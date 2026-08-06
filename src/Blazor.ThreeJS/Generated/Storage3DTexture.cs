// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This special type of texture is intended for compute shaders. It can be used to compute the data
/// of a texture with a compute shader. Note: This type of texture can only be used with
/// <c>WebGPURenderer</c> and a WebGPU backend. The JavaScript-side <c>THREE.Storage3DTexture</c>.
/// </summary>
public sealed class Storage3DTexture : Texture
{
	private readonly float _width;
	private readonly float _height;
	private readonly float _depth;
	private Wrapping _wrapR;
	private bool _is3DTexture;
	private bool _isWrapRWritten;
	private bool _isIs3DTextureWritten;

	/// <summary>Constructs a new storage texture.</summary>
	/// <param name="width">The storage texture's width.</param>
	/// <param name="height">The storage texture's height.</param>
	/// <param name="depth">The storage texture's depth.</param>
	public Storage3DTexture(float width = 1f, float height = 1f, float depth = 1f)
	{
		_width = width;
		_height = height;
		_depth = depth;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Storage3DTexture</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Storage3DTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_width = default!;
		_height = default!;
		_depth = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Storage3DTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Storage3DTexture"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.Storage3DTexture</c>: width, height, depth.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_width, _height, _depth]; }
	}

	/// <summary>
	/// This defines how the texture is wrapped in the depth direction and corresponds to *W* in UVW
	/// mapping. Writing it records a <c>wrapR</c> property write once this object is attached; writing
	/// the value already held records nothing.
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
	/// Indicates whether this texture is a 3D texture. Writing it records a <c>is3DTexture</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool Is3DTexture
	{
		get { return _is3DTexture; }
		set
		{
			if (_is3DTexture == value)
			{
				return;
			}

			_is3DTexture = value;
			_isIs3DTextureWritten = true;
			RecordSet("is3DTexture", value);
		}
	}

	/// <summary>Records a call to <c>setSize</c> on the JavaScript-side object.</summary>
	/// <param name="width">Value forwarded to the <c>width</c> argument.</param>
	/// <param name="height">Value forwarded to the <c>height</c> argument.</param>
	/// <param name="depth">Value forwarded to the <c>depth</c> argument.</param>
	public void SetSize(float width, float height, float depth)
	{
		RecordCall("setSize", width, height, depth);
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isStorageTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isStorageTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsStorageTextureAsync()
	{
		return GetAsync<bool>("isStorageTexture");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Storage3DTexture</c>, then replays every property written
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

		if (_isIs3DTextureWritten)
		{
			batch.Set(Handle, "is3DTexture", ThreeValue.Encode(_is3DTexture));
		}
	}
}
