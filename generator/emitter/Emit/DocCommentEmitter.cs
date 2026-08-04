using System.Text;
using System.Text.RegularExpressions;
using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Turns the IR's captured JSDoc into C# XML documentation. Every public member has to carry a doc
/// comment: <c>GenerateDocumentationFile</c> is on and the package targets five frameworks, so one
/// undocumented member is five CS1591 warnings.
/// </summary>
internal static class DocCommentEmitter
{
	/// <summary>Longest a single-line doc comment may be, measured in columns including indentation.</summary>
	private const int SingleLineMaxColumn = 116;

	private const string DocPrefix = "/// ";

	private static readonly Regex _linkMarkerPattern = new(
		@"\{@link\s+([^\s}|]+)\s*(?:\|)?\s*([^}]*)\}",
		RegexOptions.Compiled);

	private static readonly Regex _codeSpanPattern = new(@"`([^`]+)`", RegexOptions.Compiled);

	private static readonly Regex _whitespacePattern = new(@"\s+", RegexOptions.Compiled);

	/// <summary>
	/// Trailing JSDoc fragments that only restate what the C# signature already says. Upstream writes
	/// "… Optional; Expects a `Float`. Default `1`" because TypeScript cannot express any of it;
	/// <c>float width = 1f</c> expresses all three, so carrying the text across would be noise.
	/// </summary>
	private static readonly Regex[] _redundantTrailingFragments =
	[
		new(@"\bDefaults?\s+(?:to\s+)?`[^`]*`\s*$", RegexOptions.Compiled),
		new(@"\bDefaults?\s+(?:to\s+)?[^\s.;]+\s*$", RegexOptions.Compiled),
		new(@"\bExpects\s+an?\s+`?(?:Float|Integer)`?\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
		new(@"\bOptional\s*$", RegexOptions.Compiled)
	];

	/// <summary>
	/// Writes a <c>&lt;summary&gt;</c> block, collapsing to one line when it fits.
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="text">Already-rendered XML content.</param>
	public static void WriteSummary(CSharpWriter writer, string text)
	{
		WriteElement(writer, "summary", text);
	}

	/// <summary>Writes a <c>&lt;remarks&gt;</c> block.</summary>
	/// <param name="writer">Destination.</param>
	/// <param name="text">Already-rendered XML content.</param>
	public static void WriteRemarks(CSharpWriter writer, string text)
	{
		WriteElement(writer, "remarks", text);
	}

	/// <summary>Writes a <c>&lt;param&gt;</c> block for one constructor or method parameter.</summary>
	/// <param name="writer">Destination.</param>
	/// <param name="parameterName">C# parameter name.</param>
	/// <param name="text">Already-rendered XML content.</param>
	public static void WriteParam(CSharpWriter writer, string parameterName, string text)
	{
		WriteElement(writer, $"param name=\"{parameterName}\"", text, closingTagName: "param");
	}

	/// <summary>
	/// Writes one <c>&lt;seealso href="…"&gt;</c> per <c>@see</c> link, which is how the official
	/// three.js documentation URL survives into IntelliSense.
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="references">Links captured from the JSDoc.</param>
	public static void WriteSeeAlso(CSharpWriter writer, IEnumerable<IrSeeReference> references)
	{
		foreach (var reference in references)
		{
			if (string.IsNullOrWhiteSpace(reference.Url))
			{
				continue;
			}

			var label = string.IsNullOrWhiteSpace(reference.Label)
				? reference.Url
				: reference.Label;

			writer.WriteLine($"{DocPrefix}<seealso href=\"{XmlEscape(reference.Url)}\">{XmlEscape(label)}</seealso>");
		}
	}

	/// <summary>
	/// Converts one piece of JSDoc prose into XML doc content: escapes XML-significant characters,
	/// rewrites <c>{@link …}</c> markers, and turns backtick spans into <c>&lt;c&gt;</c>.
	/// </summary>
	/// <param name="text">Raw JSDoc text.</param>
	/// <returns>XML-safe documentation content, whitespace collapsed onto one logical line.</returns>
	public static string RenderInline(string text)
	{
		var collapsed = _whitespacePattern.Replace(text, " ").Trim();
		var escaped = XmlEscape(collapsed);
		var linked = _linkMarkerPattern.Replace(escaped, RenderLinkMarker);

		return _codeSpanPattern.Replace(linked, "<c>$1</c>");
	}

	/// <summary>
	/// Strips the trailing JSDoc fragments a C# signature already encodes, then restores sentence
	/// punctuation. Only the tail is touched, so the description itself is never reworded.
	/// </summary>
	/// <param name="text">Raw JSDoc parameter text.</param>
	/// <returns>The description with the redundant tail removed.</returns>
	public static string StripRedundantTail(string text)
	{
		var remaining = text.Trim();
		var hasStripped = true;
		while (hasStripped)
		{
			hasStripped = false;
			remaining = remaining.TrimEnd(' ', '\t', '\n', '\r', '.', ';', ',');
			foreach (var pattern in _redundantTrailingFragments)
			{
				var match = pattern.Match(remaining);
				if (!match.Success)
				{
					continue;
				}

				remaining = remaining[..match.Index];
				hasStripped = true;
				break;
			}
		}

		return EnsureSentenceEnd(remaining.TrimEnd(' ', '\t', '.', ';', ','));
	}

	/// <summary>Appends a full stop when the text does not already end in terminal punctuation.</summary>
	/// <param name="text">Text to terminate.</param>
	/// <returns>The text ending in <c>.</c>, <c>!</c> or <c>?</c>.</returns>
	public static string EnsureSentenceEnd(string text)
	{
		var trimmed = text.TrimEnd();
		if (trimmed.Length == 0)
		{
			return trimmed;
		}

		if (trimmed.EndsWith('.') || trimmed.EndsWith('!') || trimmed.EndsWith('?'))
		{
			return trimmed;
		}

		return trimmed + ".";
	}

	/// <summary>Escapes the three characters that cannot appear literally in XML doc content.</summary>
	/// <param name="text">Raw text.</param>
	/// <returns>XML-safe text.</returns>
	public static string XmlEscape(string text)
	{
		return text
			.Replace("&", "&amp;")
			.Replace("<", "&lt;")
			.Replace(">", "&gt;");
	}

	/// <summary>
	/// Rewrites one <c>{@link Target label}</c> marker. A <c>cref</c> is only emitted when the target
	/// is a type that actually exists in the package — an unresolvable <c>cref</c> is a CS1574
	/// warning, multiplied by every target framework.
	/// </summary>
	/// <param name="match">The matched marker.</param>
	/// <returns>Replacement XML.</returns>
	private static string RenderLinkMarker(Match match)
	{
		var target = match.Groups[1].Value.Trim();
		var label = match.Groups[2].Value.Trim();

		if (target.StartsWith("http://", StringComparison.Ordinal) || target.StartsWith("https://", StringComparison.Ordinal))
		{
			return $"<see href=\"{target}\">{(label.Length == 0 ? target : label)}</see>";
		}

		if (!EmitterConfig.ExistingCSharpTypeNames.Contains(target))
		{
			return $"<c>{(label.Length == 0 ? target : label)}</c>";
		}

		if (label.Length == 0)
		{
			return $"<see cref=\"{target}\"/>";
		}

		return $"<see cref=\"{target}\">{label}</see>";
	}

	/// <summary>
	/// Writes an XML doc element on one line when it fits inside <see cref="SingleLineMaxColumn"/>,
	/// and as an opening tag, wrapped body and closing tag otherwise.
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="openingTag">Tag text including any attributes, without angle brackets.</param>
	/// <param name="content">Already-rendered XML content.</param>
	/// <param name="closingTagName">Tag name for the closing tag; defaults to <paramref name="openingTag"/>.</param>
	private static void WriteElement(CSharpWriter writer, string openingTag, string content, string? closingTagName = null)
	{
		var closingTag = closingTagName ?? openingTag;
		var singleLine = $"{DocPrefix}<{openingTag}>{content}</{closingTag}>";
		if (writer.IndentColumn + singleLine.Length <= SingleLineMaxColumn)
		{
			writer.WriteLine(singleLine);
			return;
		}

		writer.WriteLine($"{DocPrefix}<{openingTag}>");
		foreach (var line in Wrap(content, EmitterConfig.DocumentationWrapColumn))
		{
			writer.WriteLine(DocPrefix + line);
		}

		writer.WriteLine($"{DocPrefix}</{closingTag}>");
	}

	/// <summary>
	/// Greedy word wrap over <see cref="TokenizeKeepingTagsWhole"/>, so an XML tag is never broken
	/// across lines.
	/// </summary>
	/// <param name="content">Text to wrap.</param>
	/// <param name="width">Maximum line length in characters.</param>
	/// <returns>The wrapped lines.</returns>
	private static List<string> Wrap(string content, int width)
	{
		var lines = new List<string>();
		var current = new StringBuilder();
		foreach (var word in TokenizeKeepingTagsWhole(content))
		{
			if (current.Length > 0 && current.Length + 1 + word.Length > width)
			{
				lines.Add(current.ToString());
				current.Clear();
			}

			if (current.Length > 0)
			{
				current.Append(' ');
			}

			current.Append(word);
		}

		if (current.Length > 0)
		{
			lines.Add(current.ToString());
		}

		return lines;
	}

	/// <summary>
	/// Splits on spaces, except that a tag carrying an attribute (<c>&lt;see cref="X"/&gt;</c>,
	/// <c>&lt;seealso href="…"&gt;</c>) is one token. Wrapping inside it would leave
	/// <c>&lt;see</c> on one <c>///</c> line and <c>cref="X"/&gt;</c> on the next, which parses but
	/// reads as a broken comment. Every bare <c>&lt;</c> reaching here opens a real tag:
	/// <see cref="RenderInline"/> has already escaped the prose ones to <c>&amp;lt;</c>.
	/// </summary>
	/// <param name="content">Already-rendered XML content.</param>
	/// <returns>The tokens to wrap over.</returns>
	private static IEnumerable<string> TokenizeKeepingTagsWhole(string content)
	{
		var openTag = new StringBuilder();
		foreach (var word in content.Split(' ', StringSplitOptions.RemoveEmptyEntries))
		{
			if (openTag.Length > 0)
			{
				openTag.Append(' ');
				openTag.Append(word);
				if (word.Contains('>'))
				{
					yield return openTag.ToString();
					openTag.Clear();
				}

				continue;
			}

			if (word.Contains('<') && !word.Contains('>'))
			{
				openTag.Append(word);
				continue;
			}

			yield return word;
		}

		if (openTag.Length > 0)
		{
			yield return openTag.ToString();
		}
	}
}
