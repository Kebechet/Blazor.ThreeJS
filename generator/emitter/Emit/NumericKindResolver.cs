namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Decides the C# type of a three.js <c>number</c>. TypeScript erases integers and floats to the
/// same type, so the IR only carries a kind where the upstream JSDoc stated one — 40 of 315 files.
/// Everything else is unspecified, and this is where the project-wide default is applied.
/// </summary>
internal static class NumericKindResolver
{
	/// <summary>C# type emitted for a value the resolver treats as floating point.</summary>
	public const string FloatTypeName = "float";

	/// <summary>C# type emitted for a value the resolver treats as a whole number.</summary>
	public const string IntegerTypeName = "int";

	private static readonly string[] _integerNameSuffixes = ["Segments", "Count", "Index"];
	private static readonly string[] _integerExactNames = ["count", "index"];

	/// <summary>
	/// Resolves the C# type for a numeric member, recording how the call was made so it can be
	/// audited. Explicit JSDoc wins; failing that an unambiguous integer name wins; failing that the
	/// value is float, because three.js is a graphics library and WebGL is float32 throughout.
	/// </summary>
	/// <param name="memberName">Name of the parameter or property being typed.</param>
	/// <param name="irNumericKind">The IR's <c>numericKind</c>, or <see langword="null"/> when unspecified.</param>
	/// <returns>The resolved C# type plus the reason it was chosen.</returns>
	public static NumericResolution Resolve(string memberName, string? irNumericKind)
	{
		switch (irNumericKind)
		{
			case "float":
				return new NumericResolution { CSharpTypeName = FloatTypeName, Basis = NumericBasis.DocumentedFloat };
			case "integer":
				return new NumericResolution { CSharpTypeName = IntegerTypeName, Basis = NumericBasis.DocumentedInteger };
			case null:
				break;
			default:
				throw new NotImplementedException($"Unhandled IR numericKind '{irNumericKind}' on '{memberName}'.");
		}

		if (HasIntegerName(memberName))
		{
			return new NumericResolution { CSharpTypeName = IntegerTypeName, Basis = NumericBasis.NameHeuristicInteger };
		}

		return new NumericResolution { CSharpTypeName = FloatTypeName, Basis = NumericBasis.DefaultedFloat };
	}

	/// <summary>
	/// Whether a name is an unambiguous integer indicator. Deliberately narrow — the override only
	/// fires on suffixes that cannot describe a continuous quantity. Note the match is case-sensitive,
	/// so a parameter named exactly <c>segments</c> is not caught by the <c>Segments</c> suffix.
	/// </summary>
	/// <param name="memberName">Name to test.</param>
	/// <returns><see langword="true"/> when the name indicates a whole number.</returns>
	private static bool HasIntegerName(string memberName)
	{
		if (_integerExactNames.Contains(memberName, StringComparer.Ordinal))
		{
			return true;
		}

		return _integerNameSuffixes.Any(x => memberName.EndsWith(x, StringComparison.Ordinal));
	}
}

/// <summary>The outcome of typing one numeric member.</summary>
internal sealed class NumericResolution
{
	/// <summary>C# type to emit, <c>float</c> or <c>int</c>.</summary>
	public required string CSharpTypeName { get; init; }

	/// <summary>Why that type was chosen.</summary>
	public required NumericBasis Basis { get; init; }

	/// <summary>Whether a human needs to review this call, i.e. it was not stated by the upstream docs.</summary>
	public bool IsInferred
	{
		get { return Basis is NumericBasis.NameHeuristicInteger or NumericBasis.DefaultedFloat; }
	}
}

/// <summary>Why a numeric member got the C# type it did.</summary>
internal enum NumericBasis : byte
{
	/// <summary>The upstream JSDoc says <c>Expects a Float</c>.</summary>
	DocumentedFloat,

	/// <summary>The upstream JSDoc says <c>Expects a Integer</c>.</summary>
	DocumentedInteger,

	/// <summary>Unspecified upstream, overridden to <c>int</c> by an unambiguous integer name.</summary>
	NameHeuristicInteger,

	/// <summary>Unspecified upstream, left at the project-wide <c>float</c> default.</summary>
	DefaultedFloat
}
