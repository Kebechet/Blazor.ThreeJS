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
				return MapArray(type, context);
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
				return MapLiteral(type, context);
			default:
				return TypeMapping.Skipped(SkipCategory.UnmappedTypeSyntax, $"`{type.Text}` is a TypeScript `{type.Kind}` type, which has no C# equivalent");
		}
	}

	/// <summary>
	/// The one mapping every arm of a union agrees on, or <see langword="null"/> when they disagree or
	/// any of them cannot be mapped at all.
	/// <para>
	/// Compared on the C# type name rather than on the mapping, because that is what the emitted
	/// signature carries: two arms that produce the same name are indistinguishable to a caller, so
	/// choosing between them is not a choice. An arm that cannot be mapped disqualifies the union
	/// outright — agreement among the arms that happen to work would be agreement about a surface that
	/// is missing part of what the caller may pass.
	/// </para>
	/// </summary>
	/// <param name="alternatives">The union's arms, already stripped of <c>null</c> and <c>undefined</c>.</param>
	/// <param name="context">Scope the arms resolve against.</param>
	/// <returns>The agreed mapping, or <see langword="null"/>.</returns>
	private TypeMapping? TryTakeAgreedMapping(List<IrType> alternatives, TypeMappingContext context)
	{
		TypeMapping? agreed = null;
		foreach (var alternative in alternatives)
		{
			var mapping = Map(alternative, context);
			if (!mapping.IsMapped)
			{
				return null;
			}

			if (agreed is null)
			{
				agreed = mapping;
				continue;
			}

			if (!string.Equals(agreed.CSharpTypeName, mapping.CSharpTypeName, StringComparison.Ordinal))
			{
				return null;
			}
		}

		return agreed;
	}

	/// <summary>
	/// The string literals a union is made of, or <see langword="null"/> when any arm is something
	/// else. A single non-string arm disqualifies the whole union: the set would no longer be closed,
	/// and an enum standing for it would silently drop whatever that arm allowed.
	/// </summary>
	/// <param name="alternatives">The union's arms, already stripped of <c>null</c> and <c>undefined</c>.</param>
	/// <returns>The tokens, or <see langword="null"/>.</returns>
	internal static IReadOnlyList<string>? TryTakeStringLiteralTokens(IReadOnlyList<IrType> alternatives)
	{
		var tokens = new List<string>();
		foreach (var alternative in alternatives)
		{
			if (alternative is not { Kind: "literal" } literal ||
				literal.Value is not { ValueKind: System.Text.Json.JsonValueKind.String } value ||
				value.GetString() is not { } token)
			{
				return null;
			}

			tokens.Add(token);
		}

		return tokens;
	}

	/// <summary>
	/// Maps a single literal type to the C# type of the value it pins.
	/// <para>
	/// Every one of these in the three.js surface is a runtime type tag — <c>isMesh: true</c>,
	/// <c>isBufferGeometry: true</c> — declared as the literal <c>true</c> rather than as
	/// <c>boolean</c>. C# cannot express "always true", but it does not need to: the tag is read-only,
	/// so it comes back over the get op as the <see langword="bool"/> it is. Narrowing the type to
	/// <see langword="bool"/> loses only the guarantee, not the value.
	/// </para>
	/// </summary>
	private static TypeMapping MapLiteral(IrType type, TypeMappingContext context)
	{
		switch (type.LiteralKind)
		{
			case "boolean":
				return TypeMapping.Mapped("bool", TypeMappingKind.Primitive);
			case "string":
				return TypeMapping.Mapped("string", TypeMappingKind.Primitive);
			case "number":
				var resolution = NumericKindResolver.Resolve(context.MemberName, context.NumericKind);
				return TypeMapping.Mapped(resolution.CSharpTypeName, TypeMappingKind.Primitive, numeric: resolution);
			default:
				return TypeMapping.Skipped(
					SkipCategory.LiteralType,
					$"`{type.Text}` is a `{type.LiteralKind ?? "?"}` literal, which pins a value C# has no type for");
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

	/// <summary>
	/// Maps <c>T[]</c> onto <c>T[]</c>, resolving the element type and carrying its mapping through.
	/// The wire encoder walks a sequence element by element, so an array is expressible exactly when
	/// its elements are — an array of handles included, which is how <c>Skeleton.bones</c> travels.
	/// </summary>
	private TypeMapping MapArray(IrType type, TypeMappingContext context)
	{
		if (type.Element is null)
		{
			return TypeMapping.Skipped(SkipCategory.CollectionType, $"`{type.Text}` is an array whose element type the IR does not carry");
		}

		var element = Map(type.Element, context);
		if (!element.IsMapped)
		{
			return TypeMapping.Skipped(element.SkipCategory, $"`{type.Text}` is an array whose element type cannot be mapped: {element.SkipReason}");
		}

		if (element.CSharpTypeName == "void")
		{
			return TypeMapping.Skipped(SkipCategory.CollectionType, $"`{type.Text}` is an array of `void`, which has no elements to carry");
		}

		// The element's own nullable annotation is kept: `(Material | null)[]` really can hold nulls,
		// and dropping it would let a caller pass an array C# thinks is non-null into a slot that is not.
		var elementTypeName = element.IsExplicitlyNullable || element.Kind == TypeMappingKind.GeneratedWrapperClass
			? element.CSharpTypeName + "?"
			: element.CSharpTypeName;

		return TypeMapping.Mapped(elementTypeName + "[]", TypeMappingKind.Sequence, element.RequiredGeneratedTypeName, elementMapping: element);
	}

	private TypeMapping MapReference(IrType type, TypeMappingContext context)
	{
		var name = type.Name ?? type.Text;
		var target = type.Target;

		// Ahead of the origin switch, which would otherwise refuse these as lib types. They are lib
		// types — but the package hand-writes a C# class for each, because three.js hands a typed array
		// straight to WebGL and nothing else can stand in for one.
		if (EmitterConfig.TypedArrayTypeNames.Contains(name))
		{
			return TypeMapping.Mapped(name, TypeMappingKind.HandWrittenTypedArray);
		}

		// Structural array interfaces, which a plain JavaScript array satisfies — and a plain JavaScript
		// array is exactly what the sequence encoder produces. `ArrayLike<number>` is what every
		// keyframe track declares its times and values as, so refusing it as a lib type blocked the
		// whole animation stack over a shape the wire already carries.
		if (EmitterConfig.StructuralSequenceTypeNames.Contains(name) && type.TypeArguments is [{ } elementType])
		{
			return MapArray(new IrType { Kind = "array", Text = type.Text, Element = elementType }, context);
		}

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
		// A structural stand-in for a math type three.js also has a class for. The mirror can only ever
		// send the class, which satisfies the interface, so this resolves rather than being refused as
		// a shape with no C# equivalent.
		if (EmitterConfig.StructuralMathInterfaceNames.TryGetValue(name, out var mathTypeName))
		{
			return TypeMapping.Mapped(mathTypeName, TypeMappingKind.HandWrittenMathType);
		}

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

		// three.js's own alias for "any of the nine typed arrays", which it spells as a union. The
		// package's hand-written base is exactly that set, so the union resolves to it rather than
		// being refused for having more than one arm.
		if (string.Equals(name, EmitterConfig.TypedArrayBaseTypeName, StringComparison.Ordinal))
		{
			return TypeMapping.Mapped(EmitterConfig.TypedArrayBaseTypeName, TypeMappingKind.HandWrittenTypedArray);
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
	/// resolving it to one arbitrary arm would silently narrow the API — see
	/// <see cref="MapAlternatives"/>, which is what a parameter position asks instead.
	/// </summary>
	private TypeMapping MapUnion(IrType type, TypeMappingContext context)
	{
		return TryReduceUnion(type, context, out var alternatives, out _)
			?? RefuseUnion(type, alternatives);
	}

	/// <summary>
	/// Resolves a type in a position that can carry several C# signatures, answering with one mapping
	/// per distinct arm rather than with a refusal.
	/// <para>
	/// A parameter is the only such position. C# overloads on parameters, so a union of genuinely
	/// different types is expressible there — as several methods — where a property or a return type
	/// has exactly one type and has to keep refusing. Only the top level of the parameter's own type is
	/// expanded: a union nested inside an array (<c>ArrayLike&lt;number | string | boolean&gt;</c>)
	/// would need one overload per element type of a sequence the encoder writes element by element,
	/// which is a different question and stays refused.
	/// </para>
	/// <para>
	/// Arms that cannot be mapped are dropped rather than disqualifying the union, because an overload
	/// set is additive: the arms that do map are signatures a caller gains, and the ones that do not are
	/// no worse off than under the refusal. ⚠️ A dropped arm is still a narrowing, so it comes back in
	/// <see cref="TypeAlternatives.DroppedArms"/> and is reported — nothing about the declared type may
	/// be lost without a recorded reason, and an arm that silently disappears is exactly that.
	/// </para>
	/// <para>
	/// Arms are deduplicated by C# type name for the same reason <see cref="TryTakeAgreedMapping"/>
	/// compares on it — two arms that produce the same name are the same signature, and emitting both is
	/// CS0111. A duplicate is not a narrowing: the signature it wanted is already there.
	/// </para>
	/// </summary>
	/// <param name="type">The IR type node, or <see langword="null"/> when the declaration had none.</param>
	/// <param name="context">Declaring member and class.</param>
	/// <returns>
	/// One mapping per distinct arm, in declaration order, beside the arms that were left out. A
	/// single-element list for everything that is not a genuine multi-type union, and a single refusal
	/// when no arm maps.
	/// </returns>
	public TypeAlternatives MapAlternatives(IrType? type, TypeMappingContext context)
	{
		if (type is not { Kind: "union" })
		{
			return new TypeAlternatives { Arms = [Map(type, context)] };
		}

		if (TryReduceUnion(type, context, out var alternatives, out var hasNullArm) is { } reduced)
		{
			return new TypeAlternatives { Arms = [reduced] };
		}

		var arms = new List<TypeMapping>();
		var dropped = new List<DroppedAlternative>();
		var takenTypeNames = new HashSet<string>(StringComparer.Ordinal);
		foreach (var alternative in alternatives)
		{
			var mapping = Map(alternative, context);
			if (!mapping.IsMapped || mapping.CSharpTypeName == "void")
			{
				dropped.Add(new DroppedAlternative
				{
					TypeText = alternative.Text,
					Reason = mapping.SkipReason ?? "the arm carries no type",
					Category = mapping.SkipCategory
				});

				continue;
			}

			if (!takenTypeNames.Add(mapping.CSharpTypeName!))
			{
				continue;
			}

			arms.Add(hasNullArm && !mapping.IsExplicitlyNullable
				? TypeMapping.Mapped(
					mapping.CSharpTypeName!,
					mapping.Kind,
					mapping.RequiredGeneratedTypeName,
					isExplicitlyNullable: true,
					numeric: mapping.Numeric,
					elementMapping: mapping.ElementMapping)
				: mapping);
		}

		// No arm mapped, so this is a refusal rather than a narrowing: the refusal reason names the whole
		// union, and listing its arms a second time as "dropped" would double-count the same loss.
		return arms.Count == 0
			? new TypeAlternatives { Arms = [RefuseUnion(type, alternatives)] }
			: new TypeAlternatives { Arms = arms, DroppedArms = dropped };
	}

	private static TypeMapping RefuseUnion(IrType type, IReadOnlyList<IrType> alternatives)
	{
		return TypeMapping.Skipped(
			SkipCategory.UnmappedUnion,
			$"`{type.Text}` unions {alternatives.Count} distinct types; C# cannot express that as one parameter and picking one arm would narrow the API silently");
	}

	/// <summary>
	/// Collapses a union onto the one C# type it really is, when it is one. Answers <see langword="null"/>
	/// for a genuinely heterogeneous union, handing back the arms it was left with so the caller can
	/// either refuse them or turn them into overloads.
	/// </summary>
	/// <param name="type">The union node.</param>
	/// <param name="context">Declaring member and class.</param>
	/// <param name="alternatives">The union's arms, stripped of <c>null</c> and <c>undefined</c>.</param>
	/// <param name="hasNullArm">Whether the declaration admitted <c>null</c> or <c>undefined</c>.</param>
	/// <returns>The collapsed mapping, or <see langword="null"/>.</returns>
	private TypeMapping? TryReduceUnion(
		IrType type,
		TypeMappingContext context,
		out List<IrType> alternatives,
		out bool hasNullArm)
	{
		alternatives = type.Types
			.Where(x => x is not { Kind: "primitive", Name: "null" } and not { Kind: "primitive", Name: "undefined" })
			.ToList();

		hasNullArm = alternatives.Count != type.Types.Count;

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

		// Arms that all map to one C# type are not a choice at all — they are TypeScript spelling the
		// same thing more than once. `number[] | ArrayLike<number>` is a sequence of numbers written
		// twice, and `string | "BufferGeometry"` is a string with one of its own values named beside it.
		// Neither narrows anything, so neither is recorded as a narrowing: the mapping that comes out is
		// exactly what the declared type meant.
		//
		// ⚠️ A union of nothing but string literals is excluded, even though every arm does map to
		// `string`. That is the closed set an enum stands for, and agreeing on `string` would both undo
		// the enums already synthesised for those sets and stop any new set upstream from ever reaching
		// the coverage report as a decision to make — it would just quietly become a string.
		if (alternatives.Count > 1 &&
			TryTakeStringLiteralTokens(alternatives) is null &&
			TryTakeAgreedMapping(alternatives, context) is { CSharpTypeName: { } agreedTypeName } agreedMapping)
		{
			return TypeMapping.Mapped(
				agreedTypeName,
				agreedMapping.Kind,
				agreedMapping.RequiredGeneratedTypeName,
				agreedMapping.IsExplicitlyNullable || hasNullArm,
				agreedMapping.Numeric,
				agreedMapping.ElementMapping);
		}

		// A union whose arms are all mirrored classes is still one thing on the wire: every arm travels
		// as a handle, and the applier does not care which class the handle names. So it resolves to the
		// base they all share rather than being refused — `Scene.fog` is `Fog | FogExp2`, and without
		// this the property does not exist at all, which is a worse answer than one that accepts either.
		// Weaker than the declared type — nothing stops a caller assigning a Mesh to `Scene.fog` — so it
		// is recorded as a narrowing and listed in the README's narrowings section with the arms it
		// stands for.
		if (alternatives.Count > 1 && alternatives.All(x => IsMirroredClass(x, context)))
		{
			MultiValueNarrowings.Add(type.Text);
			return TypeMapping.Mapped(
				EmitterConfig.RootBaseTypeName,
				TypeMappingKind.GeneratedWrapperClass,
				isExplicitlyNullable: hasNullArm);
		}

		// A union of nothing but string literals is a closed set of values, not a choice between types,
		// so it is the same thing a named alias like `ColorSpace` describes and resolves to the same
		// kind of C# enum. Only the sets this package names resolve; the rest fall through and are
		// refused below rather than being given a name derived from whichever member happened to be
		// read first.
		if (alternatives.Count > 1 && TryTakeStringLiteralTokens(alternatives) is { } tokens &&
			_enums.TryGetByTokenSet(tokens, out var synthesisedEnum) && synthesisedEnum is not null)
		{
			return TypeMapping.Mapped(
				synthesisedEnum.Name,
				TypeMappingKind.GeneratedEnum,
				requiredGeneratedTypeName: synthesisedEnum.Name,
				isExplicitlyNullable: hasNullArm);
		}

		if (alternatives.Count != 1)
		{
			return null;
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
	/// Whether a union arm is a class this mirror represents by handle, which is what makes an
	/// all-class union expressible as their shared base.
	/// </summary>
	/// <param name="alternative">One arm of the union.</param>
	/// <param name="context">Mapping context, for type-parameter erasure inside the arm.</param>
	/// <returns><see langword="true"/> when the arm resolves to a handle-backed class.</returns>
	private bool IsMirroredClass(IrType alternative, TypeMappingContext context)
	{
		if (alternative.Kind != "reference" || alternative.Target?.RefKind != "class")
		{
			return false;
		}

		return Map(alternative, context).Kind == TypeMappingKind.GeneratedWrapperClass;
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
