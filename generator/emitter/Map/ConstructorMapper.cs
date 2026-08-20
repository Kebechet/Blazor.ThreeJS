using System.Globalization;
using Blazor.ThreeJS.Emitter.Emit;
using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// Maps a three.js constructor signature onto a C# one. Shared by the emission-scope fixpoint and by
/// the emitter itself, so "is this class emittable" and "what does the emitted constructor look like"
/// can never answer differently.
/// </summary>
internal sealed class ConstructorMapper
{
	private readonly IReadOnlyDictionary<string, IrClass> _classesByName;

	/// <summary>Builds a mapper over the class index, which the inherited-constructor walk needs.</summary>
	/// <param name="ir">The extracted API surface.</param>
	public ConstructorMapper(IrRoot ir)
	{
		_classesByName = ir.Classes
			.GroupBy(x => x.Name, StringComparer.Ordinal)
			.ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
	}

	/// <summary>Maps the constructor of one class.</summary>
	/// <param name="irClass">Class whose constructor is being mapped.</param>
	/// <param name="mapper">Type mapper.</param>
	/// <returns>The mapped constructor, or a refusal naming what could not be mirrored.</returns>
	public MappedConstructor Map(IrClass irClass, TypeMapper mapper)
	{
		var declaring = ResolveDeclaringClass(irClass);
		if (declaring is null)
		{
			return MappedConstructor.Mapped([], [[]], [], []);
		}

		var selected = SelectSubsumingConstructor(declaring.Constructors);
		if (selected is null)
		{
			return MappedConstructor.Refused(
				$"{declaring.Constructors.Count} constructor overloads, none of which subsumes the others, so one C# constructor cannot stand for them all",
				SkipCategory.ConstructorOverloads);
		}

		var parameters = new List<MappedParameter>();
		var armsPerPosition = new List<IReadOnlyList<MappedParameter>>();
		var dropped = new List<DroppedParameter>();
		var isTailDropped = false;

		foreach (var irParameter in selected.Parameters)
		{
			if (isTailDropped)
			{
				if (!irParameter.IsOptional && !irParameter.IsRest)
				{
					return MappedConstructor.Refused(
						$"required parameter '{irParameter.Name}' follows a dropped optional one, so it could not be passed in its own position",
						SkipCategory.RequiredAfterOptional);
				}

				dropped.Add(new DroppedParameter
				{
					Name = irParameter.Name,
					TypeText = irParameter.Type?.Text ?? "<none>",
					Reason = "an earlier optional parameter was dropped, so this one can no longer be passed in its own position",
					Category = SkipCategory.RequiredAfterOptional
				});
				continue;
			}

			if (irParameter.IsRest)
			{
				dropped.Add(new DroppedParameter
				{
					Name = irParameter.Name,
					TypeText = irParameter.Type?.Text ?? "<none>",
					Reason = "a rest parameter; three.js applies its own behaviour when it receives none",
					Category = SkipCategory.RestParameter
				});
				isTailDropped = true;
				continue;
			}

			var context = new TypeMappingContext
			{
				MemberName = irParameter.Name,
				NumericKind = irParameter.NumericKind,
				TypeParameters = irClass.TypeParameters
			};

			var alternatives = ResolveAlternatives(mapper, irParameter, context);
			var mapping = alternatives.Arms[0];
			if (!mapping.IsMapped || mapping.CSharpTypeName == "void")
			{
				var reason = mapping.SkipReason ?? "the parameter has no type";
				if (!irParameter.IsOptional)
				{
					return MappedConstructor.Refused(
						$"required parameter '{irParameter.Name}' cannot be mapped: {reason}",
						mapping.SkipCategory);
				}

				dropped.Add(new DroppedParameter
				{
					Name = irParameter.Name,
					TypeText = irParameter.Type?.Text ?? "<none>",
					Reason = reason,
					Category = mapping.SkipCategory
				});

				isTailDropped = true;
				continue;
			}

			var arms = alternatives.Arms
				.Select(x => BuildParameter(irParameter, x, alternatives))
				.ToList();

			armsPerPosition.Add(arms);
			parameters.Add(ToStorage(arms));
		}

		var firstOptionalIndex = parameters.FindIndex(x => x.IsOptional);
		if (firstOptionalIndex >= 0 && parameters.Index().Any(x => !x.Item.IsOptional && x.Index > firstOptionalIndex))
		{
			return MappedConstructor.Refused(
				"a required parameter follows an optional one, which C# forbids",
				SkipCategory.RequiredAfterOptional);
		}

		// A parameter emitted as `T? x = null` means "unspecified". Trimming the argument list covers
		// every unspecified parameter at the end; the ones counted here have a supplied parameter after
		// them, so trimming cannot reach them and they depend on the `$undef` sentinel arriving as a
		// real JavaScript undefined. Counted because it measures how much of the emitted surface that
		// one wire feature holds up.
		var middlePositionHazards = parameters
			.Index()
			.Where(x => x.Item.IsUnspecifiedNullable && x.Index < parameters.Count - 1)
			.Select(x => x.Item.ThreeName)
			.ToList();

		return MappedConstructor.Mapped(parameters, ExpandOverloads(armsPerPosition), dropped, middlePositionHazards);
	}

	/// <summary>
	/// The class whose constructor <paramref name="irClass"/> actually has. A class that declares none
	/// inherits its base's, in TypeScript and in JavaScript alike, so <c>PositionalAudio</c> — which
	/// declares no constructor and extends <c>Audio</c> — takes an <c>AudioListener</c> however little
	/// its own declaration says so.
	/// <para>
	/// Reading an absent constructor as a parameterless one would emit <c>new THREE.PositionalAudio()</c>
	/// and leave three.js without the listener it needs, which is a silent failure rather than a
	/// narrowing: the C# call compiles, the op applies, and the object is wrong.
	/// </para>
	/// </summary>
	/// <param name="irClass">Class whose constructor is wanted.</param>
	/// <returns>
	/// The nearest class up the chain that declares one, or <see langword="null"/> when nothing on the
	/// chain does and the class is genuinely parameterless.
	/// </returns>
	private IrClass? ResolveDeclaringClass(IrClass irClass)
	{
		var current = irClass;
		while (current is not null)
		{
			if (current.Constructors.Count > 0)
			{
				return current;
			}

			var baseName = current.Extends?.Name;
			current = baseName is not null && _classesByName.TryGetValue(baseName, out var baseClass)
				? baseClass
				: null;
		}

		return null;
	}

	/// <summary>
	/// Resolves a parameter's type into the arms it is emitted as: one each for a required parameter
	/// whose declared type unions several types the mirror can express, and exactly one for everything
	/// else.
	/// <para>
	/// ⚠️ An <b>optional</b> parameter is never expanded, and the reason is not taste. Optionality is
	/// resolved so that everything after a parameter carrying a C# default carries one too, so every
	/// overload of a member with an optional union parameter accepts the very same shortest call — the
	/// one that omits the argument. That call would be CS0121-ambiguous in all of them, which takes away
	/// a call site that compiles today. Dropping the parameter instead is what the mapper already does
	/// with an optional parameter it cannot map, and it loses only the argument.
	/// </para>
	/// </summary>
	/// <param name="mapper">Type mapper.</param>
	/// <param name="irParameter">The three.js parameter.</param>
	/// <param name="context">Scope the type resolves against.</param>
	/// <returns>The emitted arms and the ones left out, never empty of arms.</returns>
	public static TypeAlternatives ResolveAlternatives(TypeMapper mapper, IrParameter irParameter, TypeMappingContext context)
	{
		if (DocumentedTypedArray(irParameter) is { } typedArrayName)
		{
			return new TypeAlternatives
			{
				Arms = [TypeMapping.Mapped(typedArrayName, TypeMappingKind.HandWrittenTypedArray)],
				DroppedArms =
				[
					new DroppedAlternative
					{
						TypeText = irParameter.Type?.Text ?? "<none>",
						Reason = $"three.js converts this argument to a `{typedArrayName}` whatever it is given, so the mirror asks for one rather than for a shape it would then have to convert with a precision the caller never chose",
						Category = SkipCategory.MathValueType
					}
				]
			};
		}

		return irParameter.IsOptional
			? new TypeAlternatives { Arms = [mapper.Map(irParameter.Type, context)] }
			: mapper.MapAlternatives(irParameter.Type, context);
	}

	/// <summary>
	/// The typed array three.js says it will build out of this argument, whatever it is handed.
	/// <para>
	/// <c>Int16BufferAttribute</c> declares <c>array</c> as a number sequence and documents that "an
	/// array value will be converted to <c>Int16Array</c>". Both halves are true, and only the second
	/// says what the object ends up holding - so the mirror asks for the <c>Int16Array</c>. Taking a
	/// <c>float[]</c> instead would mean converting it in C# to reach the base, at a precision the
	/// caller never chose and with out-of-range behaviour that is C#'s rather than JavaScript's.
	/// </para>
	/// <para>
	/// Read from the declaration rather than derived from the class name, which would have been wrong
	/// exactly once: <c>Float16BufferAttribute</c> converts to a <c>Uint16Array</c>. It is also what
	/// makes this self-correcting - if upstream stops documenting the conversion, the parameter goes
	/// back to its declared union and the class back to blocked, which is visible in the coverage table.
	/// </para>
	/// <para>
	/// ⚠️ A narrowing: three.js also accepts a plain array and an integer length here, and neither is
	/// emitted. Recorded as a dropped alternative so it reaches the coverage report.
	/// </para>
	/// </summary>
	/// <param name="irParameter">The three.js parameter.</param>
	/// <returns>The C# typed-array type name, or <see langword="null"/> when nothing documents one.</returns>
	private static string? DocumentedTypedArray(IrParameter irParameter)
	{
		if (irParameter.Doc is not { Length: > 0 } summary)
		{
			return null;
		}

		var match = TypedArrayConversionPattern.Match(summary);
		return match.Success && EmitterConfig.TypedArrayTypeNames.Contains(match.Groups[1].Value)
			? match.Groups[1].Value
			: null;
	}

	/// <summary>three.js's own wording for the array it builds out of whatever this argument is.</summary>
	private static readonly System.Text.RegularExpressions.Regex TypedArrayConversionPattern =
		new(@"will be converted to `(\w+)`", System.Text.RegularExpressions.RegexOptions.Compiled);

	/// <summary>
	/// Builds one C# parameter for one arm of its declared type.
	/// </summary>
	/// <param name="irParameter">The three.js parameter.</param>
	/// <param name="mapping">The arm this parameter is being written for.</param>
	/// <param name="alternatives">Every arm and every dropped one, carried through for the storage view and the report.</param>
	/// <returns>The parameter, as one overload declares it.</returns>
	private static MappedParameter BuildParameter(IrParameter irParameter, TypeMapping mapping, TypeAlternatives alternatives)
	{
		var defaultLiteral = irParameter.IsOptional
			? RenderDefaultLiteral(irParameter.DefaultValue, mapping)
			: null;

		// An optional parameter with no expressible default still needs a C# default, and the only
		// honest one is null: "the caller did not supply this", which ConstructorArgs then forwards
		// as the `$undef` sentinel or trims off rather than sending as a JSON null.
		var isUnspecifiedNullable = irParameter.IsOptional && defaultLiteral is null;
		if (isUnspecifiedNullable)
		{
			defaultLiteral = "null";
		}

		return new MappedParameter
		{
			Name = ToCamelCase(irParameter.Name),
			DeclarationName = CSharpIdentifier.Escape(ToCamelCase(irParameter.Name)),
			FieldName = "_" + ToCamelCase(irParameter.Name),
			ThreeName = irParameter.Name,
			Mapping = mapping,
			Alternatives = alternatives.Arms,
			DroppedAlternatives = alternatives.DroppedArms,
			DeclaredTypeText = irParameter.Type?.Text,
			CSharpTypeName = isUnspecifiedNullable || mapping.IsExplicitlyNullable
				? mapping.CSharpTypeName + "?"
				: mapping.CSharpTypeName!,
			DefaultLiteral = defaultLiteral,
			IsOptional = irParameter.IsOptional,
			IsUnspecifiedNullable = isUnspecifiedNullable,
			DocumentedDefault = irParameter.DefaultValue,
			Documentation = irParameter.Doc
		};
	}

	/// <summary>
	/// Collapses a parameter's arms into the one form the object stores.
	/// <para>
	/// A widened parameter's backing field is <c>object?</c>, because the several overloads write
	/// different C# types into the same slot and <c>ConstructorArgs</c> forwards whatever is in it —
	/// <c>ThreeValue.Encode</c> dispatches on the runtime type, so the field never has to be the
	/// declared one. Only the field widens; every constructor a caller sees stays strongly typed.
	/// </para>
	/// <para>
	/// ⚠️ No emitted class reaches the widening branch today, and base-constructor chaining is why it
	/// is still here rather than why it is unused. Every class whose constructor has a multi-arm
	/// parameter — the nine <c>*BufferAttribute</c> subclasses, <c>StorageBufferAttribute</c>,
	/// <c>StorageInstancedBufferAttribute</c> — declares its <c>array</c> as something the base's
	/// <c>TypedArray</c> slot cannot hold, so <c>EmissionScope.DescribeChainToBase</c> blocks them on
	/// that instead. Give any of them an arm whose type matches the base's and they emit, at which point
	/// the widened field is what holds whichever arm the caller's overload took.
	/// </para>
	/// </summary>
	/// <param name="arms">The parameter as each of its arms declares it.</param>
	/// <returns>The storage view of the parameter.</returns>
	private static MappedParameter ToStorage(IReadOnlyList<MappedParameter> arms)
	{
		// Only a required parameter is ever expanded, so no arm here can be an unspecified nullable and
		// the flag carries over from the first arm unchanged.
		return arms.Count == 1
			? arms[0]
			: arms[0] with { CSharpTypeName = EmitterConfig.UnionStorageTypeName };
	}

	/// <summary>
	/// Turns a per-position list of arms into the overload set that covers them: the cartesian product
	/// across parameter positions.
	/// <para>
	/// Every signature it produces is distinct, and nothing here has to check that.
	/// <see cref="TypeMapper.MapAlternatives"/> has already dropped any arm whose C# type another arm of
	/// the same parameter produced — <c>Iterable&lt;number&gt;</c> and <c>ArrayLike&lt;number&gt;</c> are
	/// both <c>float[]</c>, and all three arms of <c>PositionalAudio.setDistanceModel</c> are
	/// <c>string</c> — and positions are independent, so two different arm tuples always differ in at
	/// least one position's type. A dedup filter here could not fire; it would only look like a guard.
	/// </para>
	/// <para>
	/// ⚠️ The product is multiplicative, not additive: two two-arm parameters are four declarations of
	/// the same member, and two three-arm ones would be nine.
	/// <see cref="EmitterConfig.UnionOverloadBudget"/> is what that is measured against, and
	/// <c>api-coverage.md</c> prints the largest set produced beside it so growth is visible in a
	/// generated document rather than only in a diff.
	/// </para>
	/// </summary>
	/// <param name="armsPerPosition">Arms of each parameter, in three.js parameter order.</param>
	/// <returns>One parameter list per signature, the all-first-arm one first.</returns>
	public static IReadOnlyList<IReadOnlyList<MappedParameter>> ExpandOverloads(IReadOnlyList<IReadOnlyList<MappedParameter>> armsPerPosition)
	{
		IReadOnlyList<IReadOnlyList<MappedParameter>> overloads = [[]];
		foreach (var arms in armsPerPosition)
		{
			overloads = overloads
				.SelectMany(prefix => arms.Select(arm => (IReadOnlyList<MappedParameter>)[.. prefix, arm]))
				.ToList();
		}

		return overloads;
	}

	/// <summary>
	/// Picks the one overload every other overload is a valid call of, or <see langword="null"/> when
	/// no such overload exists.
	/// <para>
	/// Overloading is how the three.js types spell a signature that gained a parameter: <c>Texture</c>
	/// declares its nine legacy arguments as one overload and the current ten, all optional, as
	/// another. Those are not two constructors — they are one, written twice, and emitting both would
	/// be a duplicate C# signature (CS0111) because the arguments differ only in optionality. Taking
	/// the widest is therefore exact rather than lossy: every call the narrower overload accepts, the
	/// wider one accepts too.
	/// </para>
	/// <para>
	/// Genuinely different overloads — ones taking unrelated types — have no subsuming member and are
	/// still refused, because silently picking one of those would drop half the constructor's API.
	/// </para>
	/// </summary>
	/// <param name="constructors">Every declared constructor overload.</param>
	/// <returns>The subsuming overload, or <see langword="null"/> when the overloads genuinely differ.</returns>
	private static IrSignature? SelectSubsumingConstructor(IReadOnlyList<IrSignature> constructors)
	{
		if (constructors.Count == 1)
		{
			return constructors[0];
		}

		return constructors.FirstOrDefault(candidate =>
			constructors.All(other => ReferenceEquals(candidate, other) || Subsumes(candidate, other)));
	}

	/// <summary>
	/// Whether every call of <paramref name="other"/> is also a valid call of <paramref name="candidate"/>:
	/// it takes at least as many parameters, agrees on the declared type at every shared position, and
	/// makes optional everything the other leaves out or leaves optional.
	/// </summary>
	/// <param name="candidate">The overload being tested as the wider one.</param>
	/// <param name="other">The overload it must accept every call of.</param>
	/// <returns><see langword="true"/> when the candidate subsumes the other.</returns>
	private static bool Subsumes(IrSignature candidate, IrSignature other)
	{
		if (candidate.Parameters.Count < other.Parameters.Count)
		{
			return false;
		}

		for (var index = 0; index < other.Parameters.Count; index++)
		{
			var candidateParameter = candidate.Parameters[index];
			var otherParameter = other.Parameters[index];
			if (candidateParameter.Type?.Text != otherParameter.Type?.Text)
			{
				return false;
			}

			// An argument the other overload lets the caller omit has to be omittable here too, or a
			// call that compiles against the other would not compile against this one.
			if (otherParameter.IsOptional && !candidateParameter.IsOptional)
			{
				return false;
			}
		}

		// Anything beyond the other overload's arity has to be omittable, for the same reason.
		return candidate.Parameters
			.Skip(other.Parameters.Count)
			.All(x => x.IsOptional || x.IsRest);
	}

	/// <summary>
	/// Renders a documented default as a C# literal, or returns <see langword="null"/> to say the
	/// parameter has to be emitted as an unspecified nullable instead. Returning null is deliberately
	/// the fallback for everything unparseable (<c>Math.PI</c>, <c>Texture.DEFAULT_MAPPING</c>): not
	/// passing the argument at all lets three.js apply its own default, which is exactly right, where
	/// inventing a literal would send a value the upstream might change under us.
	/// </summary>
	/// <param name="documentedDefault">Verbatim default text from the JSDoc, if any.</param>
	/// <param name="mapping">The parameter's resolved type.</param>
	/// <returns>The C# literal, or <see langword="null"/>.</returns>
	private static string? RenderDefaultLiteral(string? documentedDefault, TypeMapping mapping)
	{
		if (documentedDefault is null || mapping.IsExplicitlyNullable)
		{
			return null;
		}

		var text = documentedDefault.Trim().Trim('`').Trim();
		switch (mapping.CSharpTypeName)
		{
			case NumericKindResolver.IntegerTypeName:
				if (!long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integerValue) ||
					integerValue is < int.MinValue or > int.MaxValue)
				{
					return null;
				}

				return integerValue.ToString(CultureInfo.InvariantCulture);
			case NumericKindResolver.FloatTypeName:
				if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
				{
					return null;
				}

				// `Raycaster`'s `far` documents its default as `Infinity`, which parses happily and then
				// formats back as the word "Infinity" — not a C# literal. Anything outside the range a
				// float literal can hold falls through to an unspecified nullable instead, which is also
				// the more faithful answer: three.js applies its own default.
				if (!double.IsFinite(floatValue) || System.Math.Abs(floatValue) > float.MaxValue)
				{
					return null;
				}

				return floatValue.ToString("R", CultureInfo.InvariantCulture) + "f";
			case "bool":
				if (string.Equals(text, "true", StringComparison.OrdinalIgnoreCase))
				{
					return "true";
				}

				return string.Equals(text, "false", StringComparison.OrdinalIgnoreCase)
					? "false"
					: null;
			default:
				return null;
		}
	}

	/// <summary>Lower-cases the first character so a three.js parameter name reads as a C# parameter.</summary>
	/// <param name="name">Three.js parameter name.</param>
	/// <returns>The camelCased name.</returns>
	public static string ToCamelCase(string name)
	{
		if (name.Length == 0 || char.IsLower(name[0]))
		{
			return name;
		}

		return char.ToLowerInvariant(name[0]) + name[1..];
	}
}

/// <summary>A three.js constructor resolved into C# terms, or the reason it could not be.</summary>
internal sealed class MappedConstructor
{
	/// <summary>Whether the signature could be mirrored.</summary>
	public required bool IsMapped { get; init; }

	/// <summary>
	/// Parameters that reached the C# signature, in three.js order, as the object <b>stores</b> them.
	/// One entry per three.js parameter however many overloads declare it, so the backing fields, the
	/// argument list and the attachment of object-valued arguments all have a single answer.
	/// </summary>
	public IReadOnlyList<MappedParameter> Parameters { get; init; } = [];

	/// <summary>
	/// The constructors a caller sees, one per distinct C# signature. More than one only where a
	/// parameter's declared type unions several types the mirror can express separately.
	/// </summary>
	public IReadOnlyList<IReadOnlyList<MappedParameter>> Overloads { get; init; } = [];

	/// <summary>Parameters left out, each with the reason, so the narrowing is visible.</summary>
	public IReadOnlyList<DroppedParameter> DroppedParameters { get; init; } = [];

	/// <summary>
	/// Whole declarations left out, described as they would have been written. A union-armed parameter
	/// produces one constructor per arm, and where the class chains to a base only some of those arms
	/// can satisfy it - so the arms that cannot are dropped rather than blocking the class, and are
	/// named here so the narrowing reaches the coverage report instead of only the diff.
	/// </summary>
	public IReadOnlyList<string> DroppedOverloads { get; init; } = [];

	/// <summary>Builds a copy carrying a narrowed overload set.</summary>
	/// <param name="overloads">The declarations that survive.</param>
	/// <param name="droppedOverloads">The ones that do not, described.</param>
	/// <returns>The narrowed mapping.</returns>
	public MappedConstructor WithOverloads(
		IReadOnlyList<IReadOnlyList<MappedParameter>> overloads,
		IReadOnlyList<string> droppedOverloads)
	{
		return new MappedConstructor
		{
			IsMapped = IsMapped,
			Parameters = Parameters,
			Overloads = overloads,
			DroppedParameters = DroppedParameters,
			DroppedOverloads = droppedOverloads,
			MiddlePositionUnspecifiedParameters = MiddlePositionUnspecifiedParameters,
			RefusalReason = RefusalReason,
			RefusalCategory = RefusalCategory
		};
	}

	/// <summary>
	/// Names of unspecified-nullable parameters that are not last, so trimming cannot reach them and
	/// the <c>$undef</c> sentinel is what preserves three.js's own default for them.
	/// </summary>
	public IReadOnlyList<string> MiddlePositionUnspecifiedParameters { get; init; } = [];

	/// <summary>Why the signature was refused, when it was.</summary>
	public string? RefusalReason { get; init; }

	/// <summary>Family the refusal belongs to.</summary>
	public SkipCategory RefusalCategory { get; init; }

	/// <summary>Whether any parameter is emitted as an unspecified nullable, so the args need the sentinel.</summary>
	public bool HasUnspecifiedNullable
	{
		get { return Parameters.Any(x => x.IsUnspecifiedNullable); }
	}

	/// <summary>Builds a successful mapping.</summary>
	/// <param name="parameters">Parameters that reached the signature, as the object stores them.</param>
	/// <param name="overloads">The constructors a caller sees, one per distinct C# signature.</param>
	/// <param name="dropped">Parameters left out.</param>
	/// <param name="middlePositionUnspecifiedParameters">Unspecified nullables that trimming cannot protect.</param>
	/// <returns>The mapping.</returns>
	public static MappedConstructor Mapped(
		IReadOnlyList<MappedParameter> parameters,
		IReadOnlyList<IReadOnlyList<MappedParameter>> overloads,
		IReadOnlyList<DroppedParameter> dropped,
		IReadOnlyList<string> middlePositionUnspecifiedParameters)
	{
		return new MappedConstructor
		{
			IsMapped = true,
			Parameters = parameters,
			Overloads = overloads,
			DroppedParameters = dropped,
			MiddlePositionUnspecifiedParameters = middlePositionUnspecifiedParameters
		};
	}

	/// <summary>Builds a refusal.</summary>
	/// <param name="reason">What could not be mirrored.</param>
	/// <param name="category">Family the reason belongs to.</param>
	/// <returns>The refusal.</returns>
	public static MappedConstructor Refused(string reason, SkipCategory category)
	{
		return new MappedConstructor
		{
			IsMapped = false,
			RefusalReason = reason,
			RefusalCategory = category
		};
	}
}

/// <summary>One constructor parameter, resolved from the IR into C# terms.</summary>
internal sealed record MappedParameter
{
	/// <summary>
	/// C# parameter name, unescaped. This is what an XML <c>&lt;param name="…"/&gt;</c> has to say, since
	/// the identifier of <c>@object</c> is <c>object</c>.
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// The name as it is written in the signature and in the field assignment, escaped with <c>@</c>
	/// when three.js's parameter name is a C# keyword (<c>object</c> on the helpers).
	/// </summary>
	public required string DeclarationName { get; init; }

	/// <summary>Backing field name, underscore-prefixed.</summary>
	public required string FieldName { get; init; }

	/// <summary>Original three.js parameter name, used in documentation and on the wire.</summary>
	public required string ThreeName { get; init; }

	/// <summary>The resolved type, with its basis.</summary>
	public required TypeMapping Mapping { get; init; }

	/// <summary>
	/// Every type the declared one resolves to. One entry for all but a genuine multi-type union, where
	/// each entry is an overload of its own and the backing field widens to hold any of them.
	/// </summary>
	public IReadOnlyList<TypeMapping> Alternatives { get; init; } = [];

	/// <summary>
	/// Arms of the declared union no overload stands for. A narrowing, so it is reported rather than
	/// only implied by the arms that are there.
	/// </summary>
	public IReadOnlyList<DroppedAlternative> DroppedAlternatives { get; init; } = [];

	/// <summary>Declared type verbatim, so an overload can say which arm of a union it takes.</summary>
	public string? DeclaredTypeText { get; init; }

	/// <summary>Whether the declared type resolves to more than one C# type, and so to more than one overload.</summary>
	public bool HasSeveralAlternatives
	{
		get { return Alternatives.Count > 1; }
	}

	/// <summary>C# type as written in the signature, including any nullable annotation.</summary>
	public required string CSharpTypeName { get; init; }

	/// <summary>C# default literal, or <see langword="null"/> when the parameter is required or unspecified-nullable.</summary>
	public string? DefaultLiteral { get; init; }

	/// <summary>Whether three.js declares the parameter optional.</summary>
	public required bool IsOptional { get; init; }

	/// <summary>
	/// Whether the parameter is emitted as <c>T? x = null</c> meaning "not supplied", so its argument
	/// has to be trimmed rather than sent as JSON null.
	/// </summary>
	public required bool IsUnspecifiedNullable { get; init; }

	/// <summary>Documented default text, kept for the audit even when it could not be rendered.</summary>
	public string? DocumentedDefault { get; init; }

	/// <summary>Raw JSDoc text for this parameter.</summary>
	public string? Documentation { get; init; }
}

/// <summary>A constructor parameter the mirror does not expose, and why.</summary>
internal sealed class DroppedParameter
{
	/// <summary>Three.js parameter name.</summary>
	public required string Name { get; init; }

	/// <summary>Declared type, verbatim.</summary>
	public required string TypeText { get; init; }

	/// <summary>Why it is not exposed.</summary>
	public required string Reason { get; init; }

	/// <summary>Family the reason belongs to.</summary>
	public required SkipCategory Category { get; init; }
}
