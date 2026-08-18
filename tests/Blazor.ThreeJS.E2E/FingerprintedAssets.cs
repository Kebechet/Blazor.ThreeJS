using System.Text.RegularExpressions;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// Shared by every test that asserts a specific static asset was fetched
/// (<see cref="GltfFigureTests"/>, <see cref="GltfCompressionTests"/>): matching a logical source path
/// against what the SDK actually serves it as.
/// </summary>
internal static class FingerprintedAssets
{
	/// <summary>
	/// Matches a static asset by its logical path, with or without the content fingerprint the SDK
	/// puts before the extension.
	/// </summary>
	/// <remarks>
	/// .NET 10 publishes static web assets fingerprinted — <c>three-interop.js</c> is served as
	/// <c>three-interop.bmy27co6t7.js</c> — and rewrites the import map so a plain
	/// <c>import './three-interop.js'</c> still resolves. The interop module's own relative imports go
	/// through that map too, which is what keeps the vendored addon siblings reachable; matching on
	/// the un-fingerprinted name alone would find nothing.
	/// </remarks>
	/// <param name="logicalPath">Path as it is written in source, for example <c>/a/b/thing.js</c>.</param>
	public static Regex UrlPattern(string logicalPath)
	{
		var extension = Path.GetExtension(logicalPath);
		var withoutExtension = logicalPath[..^extension.Length];
		return new Regex($"{Regex.Escape(withoutExtension)}(\\.[a-z0-9]+)?{Regex.Escape(extension)}$");
	}
}
