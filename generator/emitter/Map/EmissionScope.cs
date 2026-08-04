using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// Decides which of the IR's 309 classes are in the mirrored surface at all, and then which of those
/// can actually be emitted.
/// <para>
/// Emittability is a fixpoint, not a single pass: a class whose constructor takes another wrapped
/// class is only emittable if that one is. The set therefore starts as "every in-surface class" and
/// shrinks until it stops changing, which is also what lets two classes that reference each other
/// both stay in (neither blocks the other).
/// </para>
/// </summary>
internal sealed class EmissionScope
{
	private readonly Dictionary<string, ClassScopeResult> _resultsByName = new(StringComparer.Ordinal);
	private readonly Dictionary<string, ClassScopeResult> _anyResultByName = new(StringComparer.Ordinal);
	private readonly Dictionary<string, IrClass> _classesByName;
	private readonly List<ClassScopeResult> _results = [];

	/// <summary>Every class, with the verdict reached for it, ordered by name then file.</summary>
	public IReadOnlyList<ClassScopeResult> Results
	{
		get { return _results; }
	}

	/// <summary>Builds the scope. The constructor mapping is run repeatedly until the verdicts settle.</summary>
	/// <param name="ir">The parsed IR.</param>
	/// <param name="mapper">Type mapper, which asks this scope back about wrapped classes.</param>
	/// <param name="constructorMapper">Constructor mapping, shared with the emitter so both agree.</param>
	public EmissionScope(IrRoot ir, TypeMapper mapper, ConstructorMapper constructorMapper)
	{
		mapper.AttachScope(this);

		// Four names are declared in two files each. The file whose basename is the class name is the
		// primary declaration (`WebGPURenderer.d.ts` over `WebGPURenderer.Nodes.d.ts`); plain path
		// order would pick whichever sorts first, which is arbitrary and picked the wrong one.
		var orderedClasses = ir.Classes
			.OrderBy(x => x.Name, StringComparer.Ordinal)
			.ThenBy(x => IsPrimaryDeclarationFile(x) ? 0 : 1)
			.ThenBy(x => x.File, StringComparer.Ordinal)
			.ToList();

		foreach (var irClass in orderedClasses)
		{
			var result = new ClassScopeResult
			{
				Class = irClass,
				Status = ClassScopeStatus.Emittable
			};

			ApplySurfaceRules(result, irClass);
			_results.Add(result);
			_anyResultByName.TryAdd(irClass.Name, result);
			if (result.Status != ClassScopeStatus.Emittable)
			{
				continue;
			}

			// A duplicate name would produce two files with the same path and silently drop one, which
			// is the live hazard the IR schema warns about. The first by (name, file) order wins.
			if (!_resultsByName.TryAdd(irClass.Name, result))
			{
				result.Status = ClassScopeStatus.Blocked;
				result.Reason = $"another class named `{irClass.Name}` is declared in `{_resultsByName[irClass.Name].Class.File}`, and a C# namespace holds one type of a given name";
				result.Category = SkipCategory.DuplicateClassName;
			}
		}

		_classesByName = [];
		foreach (var irClass in ir.Classes)
		{
			_classesByName.TryAdd(irClass.Name, irClass);
		}

		var hasChanged = true;
		while (hasChanged)
		{
			hasChanged = false;
			foreach (var result in _results.Where(x => x.Status == ClassScopeStatus.Emittable))
			{
				var constructorMapping = constructorMapper.Map(result.Class, mapper);
				if (!constructorMapping.IsMapped)
				{
					Block(result, constructorMapping.RefusalReason!, constructorMapping.RefusalCategory);
					_resultsByName.Remove(result.Class.Name);
					hasChanged = true;
					continue;
				}

				result.Constructor = constructorMapping;
			}

			foreach (var result in _results.Where(x => x.Status == ClassScopeStatus.Emittable))
			{
				if (DescribeUnreachableBaseConstructor(result.Class) is not { } reason)
				{
					continue;
				}

				Block(result, reason, SkipCategory.UnreachableBaseConstructor);
				_resultsByName.Remove(result.Class.Name);
				hasChanged = true;
			}
		}
	}

	/// <summary>
	/// Why a class cannot chain to its C# base's constructor, if it cannot. A generated class only
	/// carries its own three.js constructor arguments, so it has nothing to pass to a base that
	/// requires some — and inventing values for them would put a base's fields in a state three.js
	/// never produced.
	/// </summary>
	/// <param name="irClass">Class to test.</param>
	/// <returns>The obstacle, or <see langword="null"/> when the base is reachable.</returns>
	private string? DescribeUnreachableBaseConstructor(IrClass irClass)
	{
		var baseName = irClass.Extends?.Name;
		while (baseName is not null)
		{
			if (EmitterConfig.HandWrittenClassNames.Contains(baseName))
			{
				return null;
			}

			if (_resultsByName.TryGetValue(baseName, out var baseResult) && baseResult.Status == ClassScopeStatus.Emittable)
			{
				var required = baseResult.Constructor?.Parameters.Where(x => !x.IsOptional).ToList() ?? [];
				if (required.Count == 0)
				{
					return null;
				}

				return $"its C# base `{baseName}` has a constructor requiring {string.Join(", ", required.Select(x => $"`{x.ThreeName}`"))}, and a generated class carries only its own constructor arguments — it has nothing to chain with";
			}

			baseName = _classesByName.TryGetValue(baseName, out var baseClass)
				? baseClass.Extends?.Name
				: null;
		}

		return null;
	}

	/// <summary>Whether a class is emitted, and therefore usable as a C# type in another signature.</summary>
	/// <param name="name">Class name.</param>
	/// <returns><see langword="true"/> when the class is in the emitted set.</returns>
	public bool IsEmittable(string name)
	{
		return _resultsByName.TryGetValue(name, out var result) && result.Status == ClassScopeStatus.Emittable;
	}

	/// <summary>Explains why a class is not emitted, for a skip reason that names the real obstacle.</summary>
	/// <param name="name">Class name.</param>
	/// <returns>The recorded reason, or a fallback when the name is not a class at all.</returns>
	public string DescribeExclusion(string name)
	{
		if (!_anyResultByName.TryGetValue(name, out var result))
		{
			return "it is not a class in the IR";
		}

		return result.Reason ?? "it is not emitted";
	}

	/// <summary>Which family the exclusion reason belongs to, so a skip inherits the real obstacle.</summary>
	/// <param name="name">Class name.</param>
	/// <returns>The recorded category, or <see cref="SkipCategory.UnwrappedClass"/> when there is none.</returns>
	public SkipCategory DescribeExclusionCategory(string name)
	{
		if (!_anyResultByName.TryGetValue(name, out var result) || result.Category == SkipCategory.None)
		{
			return SkipCategory.UnwrappedClass;
		}

		return result.Category;
	}

	/// <summary>Whether a declaration's file is named after the class, marking it the primary of a duplicate pair.</summary>
	/// <param name="irClass">Class to test.</param>
	/// <returns><see langword="true"/> when the file basename equals the class name.</returns>
	private static bool IsPrimaryDeclarationFile(IrClass irClass)
	{
		var basename = irClass.File.Split('/').Last();
		var declarationSuffixIndex = basename.IndexOf('.');
		if (declarationSuffixIndex < 0)
		{
			return false;
		}

		return string.Equals(basename[..declarationSuffixIndex], irClass.Name, StringComparison.Ordinal);
	}

	/// <summary>
	/// The rules that decide whether a class is part of the mirrored surface at all, before anything
	/// about its constructor is considered.
	/// </summary>
	private static void ApplySurfaceRules(ClassScopeResult result, IrClass irClass)
	{
		if (EmitterConfig.HandWrittenClassNames.Contains(irClass.Name))
		{
			Exclude(
				result,
				"hand-written by the runtime. It carries scene-graph behaviour — attachment, the transform, pre-attach state replay — rather than surface, so the generated classes derive from it instead of replacing it",
				SkipCategory.HandWritten);
			return;
		}

		if (EmitterConfig.ExcludedSourcePrefixes.FirstOrDefault(x => irClass.File.StartsWith(x, StringComparison.Ordinal)) is { } excludedPrefix)
		{
			Exclude(result, $"renderer internals under `{excludedPrefix}**`; no consumer instantiates them and emitting them would inflate the coverage table", SkipCategory.UnwrappedClass);
			return;
		}

		if (irClass.File.StartsWith(EmitterConfig.MathSourcePrefix, StringComparison.Ordinal))
		{
			Exclude(
				result,
				$"a `{EmitterConfig.MathSourcePrefix}**` value type. The mirror represents math values by value, encoded inline on the wire, not as handle-backed objects. " +
				$"Five are hand-written ({string.Join(", ", EmitterConfig.MathTypeNames.Order(StringComparer.Ordinal).Select(x => $"`{x}`"))}) and are never regenerated; " +
				$"giving the rest a representation is a public-API decision, not a mapping one",
				SkipCategory.MathValueType);
			return;
		}

		if (!irClass.IsExported)
		{
			Block(result, "three.js's public barrel does not re-export it as a value — either nothing re-exports it, or it is exported `type`-only — so it is not reachable on the `THREE` namespace the applier looks names up on", SkipCategory.NotExported);
			return;
		}

		if (!irClass.IsRuntimeExport)
		{
			Block(result, "the types re-export it but the shipped three.js bundle carries no such runtime value, so constructing it would throw `Unknown three.js type`", SkipCategory.AbsentFromShippedBundle);
			return;
		}

		if (!CSharpIdentifier.IsValid(irClass.Name))
		{
			Block(result, "the export name is not a usable C# identifier", SkipCategory.UnmappedTypeSyntax);
			return;
		}

		if (irClass.IsAbstract)
		{
			Block(result, "the class is abstract, so it has no constructor to mirror", SkipCategory.AbstractClass);
			return;
		}
	}

	private static void Exclude(ClassScopeResult result, string reason, SkipCategory category)
	{
		result.Status = ClassScopeStatus.OutOfSurface;
		result.Reason = reason;
		result.Category = category;
	}

	private static void Block(ClassScopeResult result, string reason, SkipCategory category)
	{
		result.Status = ClassScopeStatus.Blocked;
		result.Reason = reason;
		result.Category = category;
	}
}

/// <summary>The verdict reached for one class.</summary>
internal sealed class ClassScopeResult
{
	/// <summary>The class this verdict is about.</summary>
	public required IrClass Class { get; init; }

	/// <summary>Emittable, deliberately out of the mirrored surface, or blocked on something.</summary>
	public required ClassScopeStatus Status { get; set; }

	/// <summary>Why, when the status is not <see cref="ClassScopeStatus.Emittable"/>.</summary>
	public string? Reason { get; set; }

	/// <summary>Family the reason belongs to, for grouping.</summary>
	public SkipCategory Category { get; set; }

	/// <summary>The resolved constructor, present once the class is known to be emittable.</summary>
	public MappedConstructor? Constructor { get; set; }
}

/// <summary>Where a class stands relative to emission.</summary>
internal enum ClassScopeStatus : byte
{
	/// <summary>The emitter can produce this class today.</summary>
	Emittable,

	/// <summary>Deliberately not mirrored — renderer plumbing or a math value type.</summary>
	OutOfSurface,

	/// <summary>In the surface, but something about it cannot be mirrored exactly yet.</summary>
	Blocked
}

/// <summary>Whether a three.js name can be used verbatim as a C# identifier.</summary>
internal static class CSharpIdentifier
{
	/// <summary>
	/// The C# reserved keywords, in full rather than only the ones three.js happens to collide with
	/// today (<c>object</c> on the helpers, <c>event</c> and <c>params</c> elsewhere). A partial list
	/// would turn a future upstream rename into a compile error in generated code.
	/// </summary>
	private static readonly IReadOnlySet<string> _reservedWords = new HashSet<string>(StringComparer.Ordinal)
	{
		"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
		"const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
		"explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
		"implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new",
		"null", "object", "operator", "out", "override", "params", "private", "protected", "public",
		"readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static",
		"string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
		"unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
	};

	/// <summary>Prefixes a name with <c>@</c> when it is a C# keyword, so it can be used as an identifier.</summary>
	/// <param name="name">Name to escape.</param>
	/// <returns>The name, escaped if it had to be.</returns>
	public static string Escape(string name)
	{
		return _reservedWords.Contains(name)
			? "@" + name
			: name;
	}

	/// <summary>Tests a name against the identifier rules the generator relies on.</summary>
	/// <param name="name">Name to test.</param>
	/// <returns><see langword="true"/> when the name can be emitted as-is.</returns>
	public static bool IsValid(string name)
	{
		if (name.Length == 0)
		{
			return false;
		}

		if (!char.IsLetter(name[0]) && name[0] != '_')
		{
			return false;
		}

		return name.All(x => char.IsLetterOrDigit(x) || x == '_');
	}
}
