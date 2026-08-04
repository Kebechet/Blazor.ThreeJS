using System.Globalization;
using Blazor.ThreeJS.Emitter.Emit;
using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// Maps a three.js method signature onto a C# one. Separate from <see cref="ConstructorMapper"/>
/// because the two differ on exactly one point, and it matters: a constructor argument the caller
/// leaves unspecified travels as the <c>$undef</c> sentinel, whereas a method has no such channel —
/// the sentinel is constructor-args-only, so a method parameter is emitted optional only when a real
/// default can be written for it.
/// </summary>
internal sealed class MethodMapper
{
	/// <summary>Maps the first overload of a method.</summary>
	/// <param name="method">Method being mapped.</param>
	/// <param name="typeParameters">Type parameters in scope where the method was declared.</param>
	/// <param name="mapper">Type mapper.</param>
	/// <returns>The mapped signature, or a refusal naming what could not be mirrored.</returns>
	public MappedMethod Map(IrMethod method, IReadOnlyList<IrTypeParameter> typeParameters, TypeMapper mapper)
	{
		var signature = method.Overloads.FirstOrDefault();
		if (signature is null)
		{
			return MappedMethod.Refused("the method has no signature in the IR", SkipCategory.UnmappedTypeSyntax);
		}

		var parameters = new List<MappedParameter>();
		var dropped = new List<DroppedParameter>();
		var isTailDropped = false;

		foreach (var irParameter in signature.Parameters)
		{
			if (isTailDropped)
			{
				dropped.Add(new DroppedParameter
				{
					Name = irParameter.Name,
					TypeText = irParameter.Type?.Text ?? "<none>",
					Reason = "an earlier parameter was dropped, so this one can no longer be passed in its own position",
					Category = SkipCategory.RequiredAfterOptional
				});
				continue;
			}

			if (irParameter.IsRest)
			{
				var isPseudoOverload = irParameter.Type is { Kind: "union" } union && union.Types.All(x => x.Kind == "tuple");
				return MappedMethod.Refused(
					isPseudoOverload
						? $"parameter '{irParameter.Name}' is a rest-union-tuple pseudo-overload (`{irParameter.Type!.Text}`), which is one TypeScript signature standing for several C# overloads"
						: $"parameter '{irParameter.Name}' is a rest parameter (`{irParameter.Type?.Text ?? "?"}`)",
					SkipCategory.RestParameter);
			}

			var mapping = mapper.Map(irParameter.Type, new TypeMappingContext
			{
				MemberName = irParameter.Name,
				NumericKind = irParameter.NumericKind,
				TypeParameters = typeParameters
			});

			if (!mapping.IsMapped || mapping.CSharpTypeName == "void")
			{
				var reason = mapping.SkipReason ?? "the parameter has no type";
				if (!irParameter.IsOptional)
				{
					return MappedMethod.Refused($"parameter '{irParameter.Name}': {reason}", mapping.SkipCategory);
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

			var name = ConstructorMapper.ToCamelCase(irParameter.Name);
			parameters.Add(new MappedParameter
			{
				Name = name,
				DeclarationName = CSharpIdentifier.Escape(name),
				FieldName = "_" + name,
				ThreeName = irParameter.Name,
				Mapping = mapping,
				CSharpTypeName = mapping.IsExplicitlyNullable
					? mapping.CSharpTypeName + "?"
					: mapping.CSharpTypeName!,
				DefaultLiteral = irParameter.IsOptional
					? RenderDefaultLiteral(irParameter.DefaultValue, mapping)
					: null,
				IsOptional = irParameter.IsOptional,
				IsUnspecifiedNullable = false,
				DocumentedDefault = irParameter.DefaultValue,
				Documentation = irParameter.Doc
			});
		}

		if (signature.Parameters.Count > 0 && parameters.Count == 0)
		{
			return MappedMethod.Refused(
				"every parameter was dropped, so the emitted call would pass none of the arguments the method exists to take",
				dropped.FirstOrDefault()?.Category ?? SkipCategory.UnmappedTypeSyntax);
		}

		return MappedMethod.Mapped(ResolveOptionalTail(parameters), dropped, signature);
	}

	/// <summary>
	/// Decides which parameters keep their C# default. A parameter may only be optional when every
	/// parameter after it is too, so optionality is resolved right to left: the moment one cannot carry
	/// a default, every parameter before it becomes required. Emitting an optional three.js parameter
	/// as required is always safe — the caller passes a real value — where inventing a default would
	/// send one three.js never agreed to.
	/// </summary>
	/// <param name="parameters">Parameters in declaration order.</param>
	/// <returns>The same parameters, with optionality settled.</returns>
	private static List<MappedParameter> ResolveOptionalTail(List<MappedParameter> parameters)
	{
		var isTailOptional = true;
		var resolved = new MappedParameter[parameters.Count];
		for (var index = parameters.Count - 1; index >= 0; index--)
		{
			var parameter = parameters[index];
			isTailOptional = isTailOptional && parameter.IsOptional && parameter.DefaultLiteral is not null;
			resolved[index] = isTailOptional
				? parameter
				: new MappedParameter
				{
					Name = parameter.Name,
					DeclarationName = parameter.DeclarationName,
					FieldName = parameter.FieldName,
					ThreeName = parameter.ThreeName,
					Mapping = parameter.Mapping,
					CSharpTypeName = parameter.CSharpTypeName,
					DefaultLiteral = null,
					IsOptional = false,
					IsUnspecifiedNullable = false,
					DocumentedDefault = parameter.DocumentedDefault,
					Documentation = parameter.Documentation
				};
		}

		return [.. resolved];
	}

	/// <summary>
	/// Renders a documented default as a C# literal, or returns <see langword="null"/> when it cannot
	/// be written out. Shared shape with <see cref="ConstructorMapper"/>, extended with the enum case:
	/// three.js documents a default like <c>FrontSide</c>, which names a member of the very enum the
	/// parameter is typed by.
	/// </summary>
	/// <param name="documentedDefault">Verbatim default text from the JSDoc, if any.</param>
	/// <param name="mapping">The parameter's resolved type.</param>
	/// <returns>The C# literal, or <see langword="null"/>.</returns>
	public static string? RenderDefaultLiteral(string? documentedDefault, TypeMapping mapping)
	{
		if (documentedDefault is null)
		{
			return null;
		}

		var text = documentedDefault.Trim().Trim('`').Trim();
		if (mapping.IsExplicitlyNullable)
		{
			return string.Equals(text, "null", StringComparison.Ordinal)
				? "null"
				: null;
		}

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
			case "string":
				return null;
			default:
				return mapping.Kind == TypeMappingKind.GeneratedEnum && CSharpIdentifier.IsValid(text)
					? $"{mapping.CSharpTypeName}.{text}"
					: null;
		}
	}
}

/// <summary>A three.js method resolved into C# terms, or the reason it could not be.</summary>
internal sealed class MappedMethod
{
	/// <summary>Whether the signature could be mirrored.</summary>
	public required bool IsMapped { get; init; }

	/// <summary>Parameters that reached the C# signature, in three.js order.</summary>
	public IReadOnlyList<MappedParameter> Parameters { get; init; } = [];

	/// <summary>Parameters left out, each with the reason.</summary>
	public IReadOnlyList<DroppedParameter> DroppedParameters { get; init; } = [];

	/// <summary>The signature the mapping was read from.</summary>
	public IrSignature? Signature { get; init; }

	/// <summary>Why the signature was refused, when it was.</summary>
	public string? RefusalReason { get; init; }

	/// <summary>Family the refusal belongs to.</summary>
	public SkipCategory RefusalCategory { get; init; }

	/// <summary>Builds a successful mapping.</summary>
	/// <param name="parameters">Parameters that reached the signature.</param>
	/// <param name="dropped">Parameters left out.</param>
	/// <param name="signature">The signature that was mapped.</param>
	/// <returns>The mapping.</returns>
	public static MappedMethod Mapped(IReadOnlyList<MappedParameter> parameters, IReadOnlyList<DroppedParameter> dropped, IrSignature signature)
	{
		return new MappedMethod
		{
			IsMapped = true,
			Parameters = parameters,
			DroppedParameters = dropped,
			Signature = signature
		};
	}

	/// <summary>Builds a refusal.</summary>
	/// <param name="reason">What could not be mirrored.</param>
	/// <param name="category">Family the reason belongs to.</param>
	/// <returns>The refusal.</returns>
	public static MappedMethod Refused(string reason, SkipCategory category)
	{
		return new MappedMethod
		{
			IsMapped = false,
			RefusalReason = reason,
			RefusalCategory = category
		};
	}
}
