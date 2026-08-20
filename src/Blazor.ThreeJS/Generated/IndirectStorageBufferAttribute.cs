// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.IndirectStorageBufferAttribute</c>.</summary>
public sealed class IndirectStorageBufferAttribute : BufferAttribute
{
	private readonly TypedArray _array;
	private readonly float _itemSize;

	/// <summary>Initializes a new <see cref="IndirectStorageBufferAttribute"/>.</summary>
	/// <param name="array">Value forwarded to the <c>array</c> constructor argument.</param>
	/// <param name="itemSize">Value forwarded to the <c>itemSize</c> constructor argument.</param>
	public IndirectStorageBufferAttribute(TypedArray array, float itemSize)
		: base(array: array, itemSize: itemSize)
	{
		_array = array;
		_itemSize = itemSize;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>IndirectStorageBufferAttribute</c> under the handle the
	/// browser minted for it. No create op is emitted: the object already exists, and this mirror's job
	/// is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal IndirectStorageBufferAttribute(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_array = default!;
		_itemSize = default!;

		Batch = batch;
	}

	/// <summary>
	/// Name of the corresponding three.js constructor, <c>THREE.IndirectStorageBufferAttribute</c>.
	/// </summary>
	protected override string ThreeTypeName
	{
		get { return "IndirectStorageBufferAttribute"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.IndirectStorageBufferAttribute</c>: array, itemSize.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_array, _itemSize]; }
	}

	/// <summary>
	/// Reads <c>isIndirectStorageBufferAttribute</c> back from the JavaScript-side object. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isIndirectStorageBufferAttribute</c>
	/// held.
	/// </summary>
	/// <returns>
	/// The value <c>isIndirectStorageBufferAttribute</c> held, once the JavaScript side has answered.
	/// </returns>
	public Task<bool> IsIndirectStorageBufferAttributeAsync()
	{
		return GetAsync<bool>("isIndirectStorageBufferAttribute");
	}

	/// <summary>
	/// Reads <c>isStorageBufferAttribute</c> back from the JavaScript-side object. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isStorageBufferAttribute</c> held.
	/// </summary>
	/// <returns>The value <c>isStorageBufferAttribute</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsStorageBufferAttributeAsync()
	{
		return GetAsync<bool>("isStorageBufferAttribute");
	}
}
