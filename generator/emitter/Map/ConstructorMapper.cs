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
	/// <summary>Maps the constructor of one class.</summary>
	/// <param name="irClass">Class whose constructor is being mapped.</param>
	/// <param name="mapper">Type mapper.</param>
	/// <returns>The mapped constructor, or a refusal naming what could not be mirrored.</returns>
	public MappedConstructor Map(IrClass irClass, TypeMapper mapper)
	{
		if (irClass.Constructors.Count > 1)
		{
			return MappedConstructor.Refused(
				$"{irClass.Constructors.Count} constructor overloads; C# overload emission is not implemented",
				SkipCategory.ConstructorOverloads);
		}

		if (irClass.Constructors.Count == 0)
		{
			return MappedConstructor.Mapped([], [], []);
		}

		var parameters = new List<MappedParameter>();
		var dropped = new List<DroppedParameter>();
		var isTailDropped = false;

		foreach (var irParameter in irClass.Constructors[0].Parameters)
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

			var mapping = mapper.Map(irParameter.Type, context);
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

			parameters.Add(new MappedParameter
			{
				Name = ToCamelCase(irParameter.Name),
				DeclarationName = CSharpIdentifier.Escape(ToCamelCase(irParameter.Name)),
				FieldName = "_" + ToCamelCase(irParameter.Name),
				ThreeName = irParameter.Name,
				Mapping = mapping,
				CSharpTypeName = isUnspecifiedNullable || mapping.IsExplicitlyNullable
					? mapping.CSharpTypeName + "?"
					: mapping.CSharpTypeName!,
				DefaultLiteral = defaultLiteral,
				IsOptional = irParameter.IsOptional,
				IsUnspecifiedNullable = isUnspecifiedNullable,
				DocumentedDefault = irParameter.DefaultValue,
				Documentation = irParameter.Doc
			});
		}

		var required = parameters
			.Where(x => !x.IsOptional)
			.ToList();

		var firstOptionalIndex = parameters.FindIndex(x => x.IsOptional);
		if (firstOptionalIndex >= 0 && required.Any(x => parameters.IndexOf(x) > firstOptionalIndex))
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

		return MappedConstructor.Mapped(parameters, dropped, middlePositionHazards);
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

	/// <summary>Parameters that reached the C# signature, in three.js order.</summary>
	public IReadOnlyList<MappedParameter> Parameters { get; init; } = [];

	/// <summary>Parameters left out, each with the reason, so the narrowing is visible.</summary>
	public IReadOnlyList<DroppedParameter> DroppedParameters { get; init; } = [];

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
	/// <param name="parameters">Parameters that reached the signature.</param>
	/// <param name="dropped">Parameters left out.</param>
	/// <param name="middlePositionUnspecifiedParameters">Unspecified nullables that trimming cannot protect.</param>
	/// <returns>The mapping.</returns>
	public static MappedConstructor Mapped(
		IReadOnlyList<MappedParameter> parameters,
		IReadOnlyList<DroppedParameter> dropped,
		IReadOnlyList<string> middlePositionUnspecifiedParameters)
	{
		return new MappedConstructor
		{
			IsMapped = true,
			Parameters = parameters,
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
internal sealed class MappedParameter
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
