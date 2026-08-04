using System.Text.Json;
using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// Resolves the two shapes three.js uses for a closed set of values into one C# enum concept:
/// a type alias unioning <c>typeof</c> of loose <c>export const</c>s (36 of them, the common case),
/// and a real TypeScript <c>enum</c> (27, almost all WebGPU).
/// <para>
/// A group is only generatable when every member resolves to a <b>numeric</b> literal, because
/// <c>ThreeValue.Encode</c> sends a C# enum as its numeric backing value. A string-valued group such
/// as <c>ColorSpace</c> would arrive in the browser as <c>0</c> where three.js expects <c>"srgb"</c>,
/// so it is refused instead of silently mistyped.
/// </para>
/// </summary>
internal sealed class EnumCatalog
{
	private readonly Dictionary<string, GeneratedEnum> _generatableByName = new(StringComparer.Ordinal);
	private readonly Dictionary<string, string> _refusalsByName = new(StringComparer.Ordinal);

	/// <summary>Every enum the catalog can generate, ordered by name.</summary>
	public IReadOnlyList<GeneratedEnum> Generatable
	{
		get
		{
			return _generatableByName.Values
				.OrderBy(x => x.Name, StringComparer.Ordinal)
				.ToList();
		}
	}

	/// <summary>Names the catalog refused, with the reason, ordered by name.</summary>
	public IReadOnlyList<KeyValuePair<string, string>> Refusals
	{
		get
		{
			return _refusalsByName
				.OrderBy(x => x.Key, StringComparer.Ordinal)
				.ToList();
		}
	}

	/// <summary>Builds the catalog from one IR snapshot.</summary>
	/// <param name="ir">The parsed IR.</param>
	public EnumCatalog(IrRoot ir)
	{
		var constantsByName = new Dictionary<string, IrConstant>(StringComparer.Ordinal);
		foreach (var constant in ir.Constants)
		{
			constantsByName.TryAdd(constant.Name, constant);
		}

		foreach (var alias in ir.TypeAliases)
		{
			if (alias.ConstantGroup is not { Count: > 0 } group)
			{
				continue;
			}

			if (EmitterConfig.ExcludedEnumNames.TryGetValue(alias.Name, out var aliasExclusion))
			{
				_refusalsByName[alias.Name] = aliasExclusion;
				continue;
			}

			AddFromConstantGroup(alias, group, constantsByName);
		}

		foreach (var irEnum in ir.Enums)
		{
			if (EmitterConfig.ExcludedEnumNames.TryGetValue(irEnum.Name, out var exclusion))
			{
				_refusalsByName[irEnum.Name] = exclusion;
				continue;
			}

			AddFromDeclaredEnum(irEnum);
		}
	}

	/// <summary>Looks up a generatable enum by the three.js name of its alias or enum declaration.</summary>
	/// <param name="name">Alias or enum name, e.g. <c>Wrapping</c>.</param>
	/// <param name="generatedEnum">The resolved enum when it is generatable.</param>
	/// <returns><see langword="true"/> when a C# enum can be generated for that name.</returns>
	public bool TryGet(string name, out GeneratedEnum? generatedEnum)
	{
		return _generatableByName.TryGetValue(name, out generatedEnum);
	}

	/// <summary>Looks up why a name that looks like an enum was refused.</summary>
	/// <param name="name">Alias or enum name.</param>
	/// <returns>The refusal reason, or <see langword="null"/> when the name was never a candidate.</returns>
	public string? GetRefusal(string name)
	{
		return _refusalsByName.GetValueOrDefault(name);
	}

	private void AddFromConstantGroup(IrTypeAlias alias, List<string> group, Dictionary<string, IrConstant> constantsByName)
	{
		var values = new List<(string Name, long Value, IrDoc? Doc)>();
		foreach (var constantName in group)
		{
			if (!constantsByName.TryGetValue(constantName, out var constant))
			{
				Refuse(alias.Name, $"constant `{constantName}` is not in the IR, so its value is unknown");
				return;
			}

			if (constant.Type is not { Kind: "literal" } literal || literal.Value is not { } value)
			{
				Refuse(alias.Name, $"constant `{constantName}` has no literal value in the IR (declared type `{constant.Type?.Text ?? "<none>"}`)");
				return;
			}

			if (value.ValueKind == JsonValueKind.String)
			{
				Refuse(
					alias.Name,
					$"the group is string-valued (`{constantName}` = \"{value.GetString()}\"); a C# enum is sent over the wire as its " +
					$"numeric backing value, so it would arrive as a number where three.js expects the string");
				return;
			}

			if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt64(out var numericValue))
			{
				Refuse(alias.Name, $"constant `{constantName}` has a non-integral value `{value}`");
				return;
			}

			values.Add((constantName, numericValue, constant.Doc));
		}

		Add(alias.Name, alias.File, alias.Doc, values, EnumSource.ConstantGroup);
	}

	private void AddFromDeclaredEnum(IrEnum irEnum)
	{
		var values = new List<(string Name, long Value, IrDoc? Doc)>();
		foreach (var member in irEnum.Members)
		{
			if (member.Value is not { } value)
			{
				Refuse(irEnum.Name, $"member `{member.Name}` has no computed value in the IR");
				return;
			}

			if (value.ValueKind == JsonValueKind.String)
			{
				Refuse(
					irEnum.Name,
					$"the enum is string-valued (`{member.Name}` = \"{value.GetString()}\"); a C# enum is sent over the wire as its " +
					$"numeric backing value, so it would arrive as a number where three.js expects the string");
				return;
			}

			if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt64(out var numericValue))
			{
				Refuse(irEnum.Name, $"member `{member.Name}` has a non-integral value `{value}`");
				return;
			}

			values.Add((member.Name, numericValue, member.Doc));
		}

		Add(irEnum.Name, irEnum.File, null, values, EnumSource.DeclaredEnum);
	}

	/// <summary>
	/// Turns a resolved value list into a C# enum, collapsing repeated values into explicit aliases.
	/// Both <c>MOUSE</c> (<c>LEFT</c> and <c>ROTATE</c> are 0) and <c>MinificationTextureFilter</c>
	/// (four deprecated <c>MipMap</c> spellings duplicate the <c>Mipmap</c> ones) hit this: C# rejects
	/// two members declared with the same literal unless the second names the first.
	/// </summary>
	private void Add(string name, string file, IrDoc? doc, List<(string Name, long Value, IrDoc? Doc)> values, EnumSource source)
	{
		if (values.Count == 0)
		{
			Refuse(name, "the group is empty");
			return;
		}

		if (!CSharpIdentifier.IsValid(name))
		{
			Refuse(name, "the name is not a usable C# identifier");
			return;
		}

		var members = new List<GeneratedEnumMember>();
		var canonicalByValue = new Dictionary<long, string>();
		foreach (var (memberName, value, memberDoc) in values)
		{
			if (!CSharpIdentifier.IsValid(memberName))
			{
				Refuse(name, $"member `{memberName}` is not a usable C# identifier");
				return;
			}

			// C# rejects a member that repeats its enclosing type's name outright, and `@`-escaping does
			// not help — the two identifiers still compare equal.
			if (string.Equals(memberName, name, StringComparison.Ordinal))
			{
				Refuse(name, $"member `{memberName}` repeats the enum's own name, which C# does not allow");
				return;
			}

			if (canonicalByValue.TryGetValue(value, out var canonicalName))
			{
				members.Add(new GeneratedEnumMember
				{
					Name = memberName,
					DeclarationName = CSharpIdentifier.Escape(memberName),
					Value = value,
					AliasOf = canonicalName,
					Doc = memberDoc
				});
				continue;
			}

			canonicalByValue[value] = memberName;
			members.Add(new GeneratedEnumMember
			{
				Name = memberName,
				DeclarationName = CSharpIdentifier.Escape(memberName),
				Value = value,
				Doc = memberDoc
			});
		}

		_generatableByName[name] = new GeneratedEnum
		{
			Name = name,
			File = file,
			Doc = doc,
			Source = source,
			BackingTypeName = ResolveBackingTypeName(members),
			Members = members
		};
	}

	/// <summary>
	/// Picks the narrowest backing type that holds every value, matching the hand-written enums'
	/// <c>: byte</c> convention wherever three.js's numbers are small enough. three.js's constants are
	/// mostly WebGL enum values in the thousands, so most land on <c>ushort</c>; the type is reported
	/// per enum in <c>api-coverage.md</c> rather than left to be discovered from the generated source.
	/// </summary>
	/// <param name="members">The enum's resolved members.</param>
	/// <returns>The C# backing type keyword.</returns>
	private static string ResolveBackingTypeName(IReadOnlyList<GeneratedEnumMember> members)
	{
		var lowest = members.Min(x => x.Value);
		var highest = members.Max(x => x.Value);

		if (lowest >= 0)
		{
			if (highest <= byte.MaxValue)
			{
				return "byte";
			}

			if (highest <= ushort.MaxValue)
			{
				return "ushort";
			}

			return highest <= uint.MaxValue
				? "uint"
				: "long";
		}

		if (lowest >= sbyte.MinValue && highest <= sbyte.MaxValue)
		{
			return "sbyte";
		}

		if (lowest >= short.MinValue && highest <= short.MaxValue)
		{
			return "short";
		}

		return lowest >= int.MinValue && highest <= int.MaxValue
			? "int"
			: "long";
	}

	private void Refuse(string name, string reason)
	{
		_refusalsByName[name] = reason;
	}
}

/// <summary>A C# enum the generator can produce from three.js's constants.</summary>
internal sealed class GeneratedEnum
{
	/// <summary>C# and three.js name, e.g. <c>Wrapping</c>.</summary>
	public required string Name { get; init; }

	/// <summary>Declaring file, relative to the types package root.</summary>
	public required string File { get; init; }

	/// <summary>JSDoc attached to the alias, when there is any.</summary>
	public IrDoc? Doc { get; init; }

	/// <summary>Which of three.js's two closed-set shapes this came from.</summary>
	public required EnumSource Source { get; init; }

	/// <summary>Narrowest backing type that holds every value; see <see cref="EnumCatalog"/>.</summary>
	public required string BackingTypeName { get; init; }

	/// <summary>Members in declaration order.</summary>
	public required IReadOnlyList<GeneratedEnumMember> Members { get; init; }
}

/// <summary>One member of a generated enum.</summary>
internal sealed class GeneratedEnumMember
{
	/// <summary>Member name, kept exactly as three.js spells it.</summary>
	public required string Name { get; init; }

	/// <summary>
	/// The name as written in the declaration, <c>@</c>-escaped when three.js spells the member as a
	/// C# keyword. <see cref="Name"/> is what documentation and a <c>cref</c> have to say.
	/// </summary>
	public required string DeclarationName { get; init; }

	/// <summary>Numeric value three.js gives it.</summary>
	public required long Value { get; init; }

	/// <summary>Set when an earlier member already declared this value, which C# only allows as an alias.</summary>
	public string? AliasOf { get; init; }

	/// <summary>JSDoc attached to the constant or member.</summary>
	public IrDoc? Doc { get; init; }
}

/// <summary>Which three.js shape a generated enum was recovered from.</summary>
internal enum EnumSource : byte
{
	/// <summary>A type alias unioning <c>typeof</c> of loose <c>export const</c>s.</summary>
	ConstantGroup,

	/// <summary>A real TypeScript <c>enum</c> declaration.</summary>
	DeclaredEnum
}
