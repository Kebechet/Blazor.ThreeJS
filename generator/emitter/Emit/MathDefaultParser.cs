using System.Globalization;

namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Reads the component tuple three.js documents as a math value's default — <c>@default (1,1,1)</c>
/// on a material's colour — into C# constructor arguments.
/// <para>
/// This matters more than it looks. The mirror constructs its own instance for every math-typed
/// property, and a colour constructed at the type's own default is black where three.js's is white.
/// Nothing would report it: the value is only wrong once the caller touches the property and the
/// mirror replays what it thinks the other components were.
/// </para>
/// </summary>
internal static class MathDefaultParser
{
	/// <summary>Number of components each math type's component constructor takes.</summary>
	private static readonly IReadOnlyDictionary<string, int> _componentCountsByTypeName = new Dictionary<string, int>(StringComparer.Ordinal)
	{
		["Color"] = 3,
		["Vector3"] = 3,
		["Quaternion"] = 4
	};

	/// <summary>
	/// Parses a documented default into constructor arguments, or returns <see langword="null"/> when
	/// it is not a component tuple of the right arity for the type.
	/// </summary>
	/// <param name="documentedDefault">Verbatim default text from the JSDoc, if any.</param>
	/// <param name="cSharpTypeName">The math type being constructed.</param>
	/// <returns>Float literals in constructor order, or <see langword="null"/>.</returns>
	public static IReadOnlyList<string>? TryParseComponents(string? documentedDefault, string cSharpTypeName)
	{
		if (documentedDefault is null || !_componentCountsByTypeName.TryGetValue(cSharpTypeName, out var componentCount))
		{
			return null;
		}

		var text = documentedDefault.Trim().Trim('`').Trim();
		if (!text.StartsWith('(') || !text.EndsWith(')'))
		{
			return null;
		}

		var parts = text[1..^1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length != componentCount)
		{
			return null;
		}

		var components = new List<string>(componentCount);
		foreach (var part in parts)
		{
			if (!double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ||
				!double.IsFinite(value) ||
				System.Math.Abs(value) > float.MaxValue)
			{
				return null;
			}

			components.Add(value.ToString("R", CultureInfo.InvariantCulture) + "f");
		}

		return components;
	}
}
