using System.Text.Json.Serialization;

namespace Kebechet.Blazor.ThreeJS.Addons;

/// <summary>
/// Opts a <see cref="GLTFLoader"/> load into the compressed-asset extensions three.js's own
/// <c>GLTFLoader</c> does not decode on its own: <c>KHR_draco_mesh_compression</c> and
/// <c>KHR_texture_basisu</c>. Left at its defaults (both <see langword="false"/>), a file using either
/// extension rejects the load with the browser's own message rather than silently loading without its
/// geometry or textures.
/// <para>
/// Each flag costs a decoder module fetched only when set: the DRACOLoader/draco decoder pair for
/// <see cref="IsDracoEnabled"/>, the KTX2Loader/basis transcoder pair for <see cref="IsKtx2Enabled"/>.
/// A load that opts into neither fetches neither.
/// </para>
/// </summary>
public sealed class GLTFLoadOptions
{
	/// <summary>
	/// Wires a DRACOLoader into the load, so a mesh compressed with
	/// <c>KHR_draco_mesh_compression</c> decodes instead of rejecting the load.
	/// </summary>
	public bool IsDracoEnabled { get; init; }

	/// <summary>
	/// Wires a KTX2Loader into the load, so a texture compressed with <c>KHR_texture_basisu</c>
	/// transcodes instead of rejecting the load.
	/// </summary>
	public bool IsKtx2Enabled { get; init; }
}

/// <summary>
/// The wire form of <see cref="GLTFLoadOptions"/> that crosses to <c>loadGltf</c>/<c>loadGltfInto</c>
/// in <c>three-interop.js</c>. Kept separate from the public type so the short, lowercase JSON names
/// the JavaScript side reads (<c>draco</c>/<c>ktx2</c>) are a fact about the wire format rather than
/// about the property names a consumer sees.
/// </summary>
internal sealed class GLTFLoadOptionsDto
{
	/// <summary>Mirrors <see cref="GLTFLoadOptions.IsDracoEnabled"/>.</summary>
	[JsonPropertyName("draco")]
	public bool Draco { get; init; }

	/// <summary>Mirrors <see cref="GLTFLoadOptions.IsKtx2Enabled"/>.</summary>
	[JsonPropertyName("ktx2")]
	public bool Ktx2 { get; init; }

	/// <summary>
	/// Converts a caller's <see cref="GLTFLoadOptions"/> to its wire form, or answers
	/// <see langword="null"/> for a <see langword="null"/> input so a caller who asked for no options
	/// sends none rather than a DTO of all-false flags.
	/// </summary>
	/// <param name="options">The caller's options, or <see langword="null"/>.</param>
	/// <returns>The wire form, or <see langword="null"/>.</returns>
	public static GLTFLoadOptionsDto? FromOptions(GLTFLoadOptions? options)
	{
		return options is null
			? null
			: new GLTFLoadOptionsDto { Draco = options.IsDracoEnabled, Ktx2 = options.IsKtx2Enabled };
	}
}
