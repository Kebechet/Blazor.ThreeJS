using Blazor.ThreeJS.Emitter.Emit;
using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// Resolves an IR type node onto a C# type, or refuses it with a reason. Every reference in the
/// snapshot goes through here, so the set of things the mirror can express is defined in exactly one
/// place and the coverage table is a rendering of this file's decisions rather than a parallel list.
/// </summary>
internal sealed class TypeMapper
{
	private readonly EnumCatalog _enums;
	private readonly Dictionary<string, IrTypeAlias> _aliasesByName;
	private EmissionScope? _scope;

	/// <summary>Enums this mapper resolved references to, so only what is used has to be generated.</summary>
	public HashSet<string> RequiredEnumNames { get; } = new(StringComparer.Ordinal);

	/// <summary>
	/// Declared types of the form <c>T | T[]</c> that were narrowed to their single-value arm, so the
	/// coverage report can state which parts of the API lost their multi-value form.
	/// </summary>
	public HashSet<string> MultiValueNarrowings { get; } = new(StringComparer.Ordinal);

	/// <summary>Builds a mapper over one IR snapshot.</summary>
	/// <param name="ir">The parsed IR.</param>
	/// <param name="enums">Catalog of generatable enums.</param>
	public TypeMapper(IrRoot ir, EnumCatalog enums)
	{
		_enums = enums;
		_aliasesByName = new Dictionary<string, IrTypeAlias>(StringComparer.Ordinal);
		foreach (var alias in ir.TypeAliases)
		{
			_aliasesByName.TryAdd(alias.Name, alias);
		}
	}

	/// <summary>
	/// Hands the mapper the emission scope. Split from the constructor because the two are mutually
	/// dependent: the scope decides emittability by asking this mapper about constructor parameters,
	/// and this mapper decides whether a class reference resolves by asking the scope.
	/// </summary>
	/// <param name="scope">The scope being built.</param>
	public void AttachScope(EmissionScope scope)
	{
		_scope = scope;
	}

	/// <summary>Resolves a type node.</summary>
	/// <param name="type">The IR type node, or <see langword="null"/> when the declaration had none.</param>
	/// <param name="context">Declaring member and class, needed for numeric typing and type-parameter erasure.</param>
	/// <returns>The mapping, successful or skipped.</returns>
	public TypeMapping Map(IrType? type, TypeMappingContext context)
	{
		if (type is null)
		{
			return TypeMapping.Skipped(SkipCategory.UntypedValue, "the declaration carries no type");
		}

		switch (type.Kind)
		{
			case "primitive":
				return MapPrimitive(type, context);
			case "reference":
				return MapReference(type, context);
			case "union":
				return MapUnion(type, context);
			case "array":
				return TypeMapping.Skipped(
					SkipCategory.CollectionType,
					$"`{type.Text}` is an array, and `ThreeValue.Encode` has no array arm — an array argument has no wire encoding");
			case "tuple":
			case "namedTupleMember":
				return TypeMapping.Skipped(SkipCategory.CollectionType, $"`{type.Text}` is a tuple, which has no wire encoding");
			case "function":
			case "constructorType":
				return TypeMapping.Skipped(
					SkipCategory.CallbackType,
					$"`{type.Text}` is a JavaScript callback, and the wire format carries ops in one direction only — there is no channel to call back into C#");
			case "object":
				return TypeMapping.Skipped(SkipCategory.AnonymousObjectType, $"`{type.Text}` is an anonymous object literal type with no named C# equivalent");
			case "literal":
				return TypeMapping.Skipped(
					SkipCategory.LiteralType,
					$"`{type.Text}` is a literal type — three.js's `isMesh`-style runtime type tag — which C# has no equivalent for outside an enum member");
			default:
				return TypeMapping.Skipped(SkipCategory.UnmappedTypeSyntax, $"`{type.Text}` is a TypeScript `{type.Kind}` type, which has no C# equivalent");
		}
	}

	private static TypeMapping MapPrimitive(IrType type, TypeMappingContext context)
	{
		switch (type.Name)
		{
			case "number":
				var resolution = NumericKindResolver.Resolve(context.MemberName, context.NumericKind);
				return TypeMapping.Mapped(resolution.CSharpTypeName, TypeMappingKind.Primitive, numeric: resolution);
			case "boolean":
				return TypeMapping.Mapped("bool", TypeMappingKind.Primitive);
			case "string":
				return TypeMapping.Mapped("string", TypeMappingKind.Primitive);
			case "void":
			case "undefined":
				return TypeMapping.Mapped("void", TypeMappingKind.Primitive);
			case "this":
				return TypeMapping.Skipped(SkipCategory.UnmappedTypeSyntax, "`this` is only meaningful as a fluent return type, which the caller handles separately");
			default:
				return TypeMapping.Skipped(SkipCategory.UntypedValue, $"`{type.Name}` carries no type information a C# signature could express");
		}
	}

	private TypeMapping MapReference(IrType type, TypeMappingContext context)
	{
		var name = type.Name ?? type.Text;
		var target = type.Target;

		switch (target?.Origin)
		{
			case "lib":
				return TypeMapping.Skipped(
					SkipCategory.DomOrLibType,
					$"`{type.Text}` is a TypeScript lib type; C# holds no browser object and the wire format has no encoding for one");
			case "excluded":
				return TypeMapping.Skipped(
					SkipCategory.NodeStackType,
					$"`{type.Text}` is declared under `src/nodes/**`, the TSL / WebGPU node stack that is outside the extracted API surface");
			case "external":
				return TypeMapping.Skipped(SkipCategory.ExternalType, $"`{type.Text}` is declared in another package");
			case "package":
				return TypeMapping.Skipped(SkipCategory.ExternalType, $"`{type.Text}` is declared elsewhere in `@types/three`, outside the scanned `src/` surface");
			case "unresolved":
			case null:
				return TypeMapping.Skipped(SkipCategory.UnresolvedType, $"`{type.Text}` did not resolve to any declaration");
		}

		switch (target.RefKind)
		{
			case "typeParameter":
				return MapTypeParameter(name, context);
			case "class":
				return MapClassReference(name, type, context);
			case "interface":
				return MapInterfaceReference(name, type);
			case "enum":
			case "typeAlias":
				return MapNamedValueSet(name, type, context);
			default:
				return TypeMapping.Skipped(SkipCategory.UnmappedTypeSyntax, $"`{type.Text}` resolves to a `{target.RefKind}`, which is not a type the mirror can express");
		}
	}

	/// <summary>
	/// Erases a type parameter to its default, failing that its constraint. The C# mirror is
	/// non-generic by design, so <c>Mesh&lt;TGeometry = BufferGeometry&gt;</c> maps exactly as if the
	/// parameter had been written out.
	/// </summary>
	private TypeMapping MapTypeParameter(string name, TypeMappingContext context)
	{
		var typeParameter = context.TypeParameters.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));
		var erased = typeParameter?.Default ?? typeParameter?.Constraint;
		if (erased is null)
		{
			return TypeMapping.Skipped(
				SkipCategory.UnerasableTypeParameter,
				$"type parameter `{name}` has neither a default nor a constraint, so erasing it leaves nothing to map to");
		}

		if (context.HasErased(name))
		{
			return TypeMapping.Skipped(SkipCategory.UnerasableTypeParameter, $"type parameter `{name}` erases to itself");
		}

		return Map(erased, context.WithErased(name));
	}

	private TypeMapping MapClassReference(string name, IrType type, TypeMappingContext context)
	{
		if (EmitterConfig.MathTypeNames.Contains(name))
		{
			return TypeMapping.Mapped(name, TypeMappingKind.HandWrittenMathType);
		}

		if (_scope is not null && _scope.IsEmittable(name))
		{
			return TypeMapping.Mapped(name, TypeMappingKind.GeneratedWrapperClass, requiredGeneratedTypeName: name);
		}

		if (EmitterConfig.ExistingCSharpTypeNames.Contains(name) && context.MayUseHandWrittenClasses)
		{
			return TypeMapping.Mapped(name, TypeMappingKind.GeneratedWrapperClass);
		}

		var exclusion = _scope?.DescribeExclusion(name) ?? "the emission scope has not been built";
		var category = _scope?.DescribeExclusionCategory(name) ?? SkipCategory.UnwrappedClass;
		return TypeMapping.Skipped(category, $"`{type.Text}` is not an emitted class: {exclusion}");
	}

	private static TypeMapping MapInterfaceReference(string name, IrType type)
	{
		var isOptionsBag = name.EndsWith("Parameters", StringComparison.Ordinal) ||
			name.EndsWith("Options", StringComparison.Ordinal);

		if (isOptionsBag)
		{
			return TypeMapping.Skipped(
				SkipCategory.OptionsInterface,
				$"`{type.Text}` is an options bag. Every field it carries is also a settable property on the constructed object, " +
				$"so the mirror expresses them as properties rather than as one anonymous constructor argument");
		}

		return TypeMapping.Skipped(
			SkipCategory.OptionsInterface,
			$"`{type.Text}` is an interface, and the mirror has no representation for a structural type — only for classes it constructs by handle");
	}

	private TypeMapping MapNamedValueSet(string name, IrType type, TypeMappingContext context)
	{
		if (string.Equals(name, EmitterConfig.ColorRepresentationAliasName, StringComparison.Ordinal))
		{
			return TypeMapping.Mapped(EmitterConfig.ColorTypeName, TypeMappingKind.HandWrittenMathType);
		}

		// The catalog is asked before the hand-written names, so a value set that exists in both places
		// resolves through the generated enum and is counted as referenced. `Side` is the only one
		// today; asking the hand-written list first would keep it out of RequiredEnumNames permanently,
		// so the coverage table would report it unreferenced however many members came to use it.
		if (_enums.TryGet(name, out var generatedEnum) && generatedEnum is not null)
		{
			RequiredEnumNames.Add(generatedEnum.Name);
			return TypeMapping.Mapped(generatedEnum.Name, TypeMappingKind.GeneratedEnum, requiredGeneratedTypeName: generatedEnum.Name);
		}

		if (EmitterConfig.ExistingCSharpTypeNames.Contains(name) && !EmitterConfig.MathTypeNames.Contains(name))
		{
			return TypeMapping.Mapped(name, TypeMappingKind.GeneratedEnum);
		}

		if (_enums.GetRefusal(name) is { } refusal)
		{
			var category = refusal.Contains("string-valued", StringComparison.Ordinal)
				? SkipCategory.StringConstantGroup
				: SkipCategory.UnmappedTypeAlias;

			return TypeMapping.Skipped(category, $"`{name}` cannot become a C# enum: {refusal}");
		}

		if (_aliasesByName.TryGetValue(name, out var alias) && alias.Type is { } aliased)
		{
			// Aliases that are a plain rename of a mappable type are followed through; the alias only
			// exists in TypeScript, so refusing it would refuse a type the mirror can already express.
			if (aliased.Kind is "reference" or "primitive")
			{
				return Map(aliased, context);
			}

			return TypeMapping.Skipped(
				SkipCategory.UnmappedTypeAlias,
				$"`{name}` aliases `{aliased.Text}`, which is neither a group of numeric constants nor a type the mirror expresses");
		}

		return TypeMapping.Skipped(SkipCategory.UnmappedTypeAlias, $"`{type.Text}` is a named value set the mapper has no rule for");
	}

	/// <summary>
	/// Maps a union. Only the <c>T | null</c> / <c>T | undefined</c> shape survives, because it is the
	/// one union C# can express as a single parameter. Anything wider is a genuine overload set and
	/// resolving it to one arbitrary arm would silently narrow the API.
	/// </summary>
	private TypeMapping MapUnion(IrType type, TypeMappingContext context)
	{
		var alternatives = type.Types
			.Where(x => x is not { Kind: "primitive", Name: "null" } and not { Kind: "primitive", Name: "undefined" })
			.ToList();

		var hasNullArm = alternatives.Count != type.Types.Count;

		// `T | T[]` is not a choice between two types — it is one type, with three.js's convenience form
		// for supplying several of it. `Material | Material[]` is what `Mesh.material` is declared as,
		// and refusing it leaves a mesh with no material at all. The single-value arm is taken and the
		// multi-value form is recorded as a narrowing; picking an arm of a genuinely heterogeneous union
		// stays refused below, because there the choice would be between different things.
		if (alternatives.Count == 2 && TryTakeSingleValueArm(alternatives) is { } singleValueArm)
		{
			MultiValueNarrowings.Add(type.Text);
			alternatives = [singleValueArm];
		}

		if (alternatives.Count != 1)
		{
			return TypeMapping.Skipped(
				SkipCategory.UnmappedUnion,
				$"`{type.Text}` unions {alternatives.Count} distinct types; C# cannot express that as one parameter and picking one arm would narrow the API silently");
		}

		var mapping = Map(alternatives[0], context);
		if (!mapping.IsMapped || !hasNullArm)
		{
			return mapping;
		}

		return TypeMapping.Mapped(
			mapping.CSharpTypeName!,
			mapping.Kind,
			mapping.RequiredGeneratedTypeName,
			isExplicitlyNullable: true,
			numeric: mapping.Numeric);
	}

	/// <summary>
	/// Returns the single-value arm of a <c>T | T[]</c> union, or <see langword="null"/> when the two
	/// arms are not the same type in singular and plural form.
	/// </summary>
	/// <param name="alternatives">The union's two non-null arms.</param>
	/// <returns>The scalar arm, when the union is one type spelled two ways.</returns>
	private static IrType? TryTakeSingleValueArm(IReadOnlyList<IrType> alternatives)
	{
		var arrayArm = alternatives.FirstOrDefault(x => x.Kind == "array");
		if (arrayArm?.Element is null)
		{
			return null;
		}

		var scalarArm = alternatives.First(x => !ReferenceEquals(x, arrayArm));
		return string.Equals(arrayArm.Element.Text, scalarArm.Text, StringComparison.Ordinal)
			? scalarArm
			: null;
	}
}

/// <summary>
/// What the mapper needs to know about where a type reference appears: the member's name and
/// documented numeric kind, and the type parameters in scope for erasure.
/// </summary>
internal sealed class TypeMappingContext
{
	private readonly IReadOnlySet<string> _erasedTypeParameterNames;

	/// <summary>Name of the parameter, property or method the type belongs to.</summary>
	public required string MemberName { get; init; }

	/// <summary><c>float</c> / <c>integer</c> from the JSDoc, or <see langword="null"/> when unspecified.</summary>
	public string? NumericKind { get; init; }

	/// <summary>Type parameters declared by the enclosing class, used to erase a reference to one.</summary>
	public IReadOnlyList<IrTypeParameter> TypeParameters { get; init; } = [];

	/// <summary>
	/// Whether a reference may resolve to a hand-written class that the generator does not itself
	/// emit. True for members of generated classes, which compile alongside the hand-written ones.
	/// </summary>
	public bool MayUseHandWrittenClasses { get; init; } = true;

	/// <summary>Creates a context.</summary>
	/// <param name="erasedTypeParameterNames">Type parameters already erased on this path, to stop a cycle.</param>
	public TypeMappingContext(IReadOnlySet<string>? erasedTypeParameterNames = null)
	{
		_erasedTypeParameterNames = erasedTypeParameterNames ?? new HashSet<string>(StringComparer.Ordinal);
	}

	/// <summary>Whether a type parameter has already been erased on the current resolution path.</summary>
	/// <param name="name">Type parameter name.</param>
	/// <returns><see langword="true"/> when erasing it again would loop.</returns>
	public bool HasErased(string name)
	{
		return _erasedTypeParameterNames.Contains(name);
	}

	/// <summary>Returns a copy of this context that has erased one more type parameter.</summary>
	/// <param name="name">Type parameter just erased.</param>
	/// <returns>The extended context.</returns>
	public TypeMappingContext WithErased(string name)
	{
		var erased = new HashSet<string>(_erasedTypeParameterNames, StringComparer.Ordinal) { name };
		return new TypeMappingContext(erased)
		{
			MemberName = MemberName,
			NumericKind = NumericKind,
			TypeParameters = TypeParameters,
			MayUseHandWrittenClasses = MayUseHandWrittenClasses
		};
	}
}
