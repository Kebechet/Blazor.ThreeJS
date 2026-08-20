// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates a texture based on data in compressed form, for example from a
/// <see href="https://en.wikipedia.org/wiki/DirectDraw_Surface">DDS</see> file. The JavaScript-side
/// <c>THREE.CompressedTexture</c>.
/// </summary>
/// <remarks>For use with the <c>CompressedTextureLoader</c>.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/textures/CompressedTexture">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/textures/CompressedTexture.js">Source</seealso>
public class CompressedTexture : Texture
{
	private readonly CompressedTextureMipmap[] _mipmaps;
	private readonly float _width;
	private readonly float _height;

	/// <summary>This creates a new <c>CompressedTexture</c> object.</summary>
	/// <param name="mipmaps">
	/// The mipmaps array should contain objects with data, width and height. The mipmaps should be of
	/// the correct format and type.
	/// </param>
	/// <param name="width">The width of the biggest mipmap.</param>
	/// <param name="height">The height of the biggest mipmap.</param>
	public CompressedTexture(CompressedTextureMipmap[] mipmaps, float width, float height)
	{
		_mipmaps = mipmaps;
		_width = width;
		_height = height;
	}

	/// <summary>
	/// Initializes a <c>CompressedTexture</c> whose subclass supplies its constructor arguments on the
	/// JavaScript side. The arguments this class declares are left unknown, because that is what they
	/// are: three.js was given them and C# was not.
	/// </summary>
	protected CompressedTexture()
	{
		_mipmaps = default!;
		_width = default!;
		_height = default!;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CompressedTexture</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CompressedTexture(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_mipmaps = default!;
		_width = default!;
		_height = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CompressedTexture</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CompressedTexture"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.CompressedTexture</c>: mipmaps, width, height.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_mipmaps, _width, _height]; }
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="CompressedTexture"/>. Read-only
	/// in three.js, so it is read on demand rather than mirrored: records a get op, sends it behind
	/// every write already pending, and completes with the value <c>isCompressedTexture</c> held.
	/// </summary>
	/// <returns>The value <c>isCompressedTexture</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsCompressedTextureAsync()
	{
		return GetAsync<bool>("isCompressedTexture");
	}
}
