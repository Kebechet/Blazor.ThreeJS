// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A <c>BufferAttribute</c> for
/// <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Uint16Array:">Uint16Array</see>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/TypedArray#typedarray_objects">TypedArray</see>.
/// The JavaScript-side <c>THREE.Uint16BufferAttribute</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/bufferAttributeTypes/BufferAttributeTypes">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/BufferAttribute.js">Source</seealso>
public sealed class Uint16BufferAttribute : BufferAttribute
{
	private readonly Uint16Array _array;
	private readonly float _itemSize;
	private readonly bool _normalized;

	/// <summary>This creates a new <c>Uint16BufferAttribute</c> object.</summary>
	/// <param name="array">
	/// This can be a typed or untyped (normal) array or an integer length. An array value will be
	/// converted to <c>Uint16Array</c>. If a length is given a new <c>TypedArray</c> will created,
	/// initialized with all elements set to zero.
	/// </param>
	/// <param name="itemSize">
	/// the number of values of the <c>array</c> that should be associated with a particular vertex. For
	/// instance, if this attribute is storing a 3-component vector (such as a _position_, _normal_, or
	/// _color_), then itemSize should be <c>3</c>.
	/// </param>
	/// <param name="normalized">
	/// Applies to integer data only. Indicates how the underlying data in the buffer maps to the values
	/// in the GLSL code. For instance, if <c>array</c> is an instance of <c>UInt16Array</c>, and
	/// <c>normalized</c> is true, the values <c>0</c> - <c>+65535</c> in the array data will be mapped
	/// to <c>0.0f</c> - <c>+1.0f</c> in the GLSL attribute. An <c>Int16Array</c> (signed) would map
	/// from <c>-32768</c> - <c>+32767</c> to <c>-1.0f</c> - <c>+1.0f</c>. If normalized is false, the
	/// values will be converted to floats unmodified, i.e. <c>32767</c> becomes <c>32767.0f</c>.
	/// </param>
	public Uint16BufferAttribute(Uint16Array array, float itemSize, bool normalized = false)
		: base(array: array, itemSize: itemSize, normalized: normalized)
	{
		_array = array;
		_itemSize = itemSize;
		_normalized = normalized;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Uint16BufferAttribute</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Uint16BufferAttribute(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_array = default!;
		_itemSize = default!;
		_normalized = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Uint16BufferAttribute</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Uint16BufferAttribute"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Uint16BufferAttribute</c>: array, itemSize,
	/// normalized.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_array, _itemSize, _normalized]; }
	}
}
