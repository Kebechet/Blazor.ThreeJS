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

		var armsPerPosition = new List<IReadOnlyList<MappedParameter>>();
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
				// A rest-union-tuple is one TypeScript signature standing for several C# overloads, and
				// there is no single parameter list to write for it.
				if (irParameter.Type is { Kind: "union" } union && union.Types.All(x => x.Kind == "tuple"))
				{
					return MappedMethod.Refused(
						$"parameter '{irParameter.Name}' is a rest-union-tuple pseudo-overload (`{irParameter.Type!.Text}`), which is one TypeScript signature standing for several C# overloads",
						SkipCategory.RestParameter);
				}

				// An ordinary rest parameter is a C# `params` array, and the two mean the same thing:
				// `group.Add(a, b, c)` reaches three.js as three arguments.
				//
				// ⚠️ It relies on the array covariance the escape hatch warns about, and here that is the
				// wanted behaviour rather than the hazard. `RecordCall` takes `params object?[]`, so
				// handing it an `Object3D[]` binds the array *as* the argument list - which is precisely
				// what a rest parameter is. The `(object?)` cast the sequence path adds would defeat it.
				var restMapping = mapper.Map(irParameter.Type, new TypeMappingContext
				{
					MemberName = irParameter.Name,
					NumericKind = irParameter.NumericKind,
					TypeParameters = typeParameters
				});

				if (!restMapping.IsMapped || restMapping.Kind != TypeMappingKind.Sequence)
				{
					return MappedMethod.Refused(
						$"parameter '{irParameter.Name}' is a rest parameter (`{irParameter.Type?.Text ?? "?"}`)",
						SkipCategory.RestParameter);
				}

				var restName = ConstructorMapper.ToCamelCase(irParameter.Name);
				armsPerPosition.Add([
					new MappedParameter
					{
						Name = restName,
						DeclarationName = CSharpIdentifier.Escape(restName),
						FieldName = "_" + restName,
						ThreeName = irParameter.Name,
						Mapping = restMapping,
						Alternatives = [restMapping],
						DeclaredTypeText = irParameter.Type?.Text,
						CSharpTypeName = restMapping.CSharpTypeName!,
						IsOptional = false,
						IsUnspecifiedNullable = false,
						IsRest = true,
						Documentation = irParameter.Doc
					}
				]);

				continue;
			}

			var alternatives = ConstructorMapper.ResolveAlternatives(mapper, irParameter, new TypeMappingContext
			{
				MemberName = irParameter.Name,
				NumericKind = irParameter.NumericKind,
				TypeParameters = typeParameters
			});

			var mapping = alternatives.Arms[0];
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
			armsPerPosition.Add(alternatives.Arms
				.Select(arm => new MappedParameter
				{
					Name = name,
					DeclarationName = CSharpIdentifier.Escape(name),
					FieldName = "_" + name,
					ThreeName = irParameter.Name,
					Mapping = arm,
					Alternatives = alternatives.Arms,
					DroppedAlternatives = alternatives.DroppedArms,
					DeclaredTypeText = irParameter.Type?.Text,
					CSharpTypeName = arm.IsExplicitlyNullable
						? arm.CSharpTypeName + "?"
						: arm.CSharpTypeName!,
					DefaultLiteral = irParameter.IsOptional
						? RenderDefaultLiteral(irParameter.DefaultValue, arm)
						: null,
					IsOptional = irParameter.IsOptional,
					IsUnspecifiedNullable = false,
					DocumentedDefault = irParameter.DefaultValue,
					Documentation = irParameter.Doc
				})
				.ToList());
		}

		if (signature.Parameters.Count > 0 && armsPerPosition.Count == 0)
		{
			return MappedMethod.Refused(
				"every parameter was dropped, so the emitted call would pass none of the arguments the method exists to take",
				dropped.FirstOrDefault()?.Category ?? SkipCategory.UnmappedTypeSyntax);
		}

		var overloads = ConstructorMapper.ExpandOverloads(armsPerPosition)
			.Select(x => (IReadOnlyList<MappedParameter>)ResolveOptionalTail([.. x]))
			.ToList();

		return MappedMethod.Mapped(overloads, dropped, signature);
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
				: parameter with { DefaultLiteral = null, IsOptional = false, IsUnspecifiedNullable = false };
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

	/// <summary>
	/// The methods a caller sees, one per distinct C# signature, each carrying the parameters that
	/// reached it in three.js order. More than one only where a parameter's declared type unions several
	/// types the mirror can express separately.
	/// </summary>
	public IReadOnlyList<IReadOnlyList<MappedParameter>> Overloads { get; init; } = [];

	/// <summary>Parameters left out, each with the reason.</summary>
	public IReadOnlyList<DroppedParameter> DroppedParameters { get; init; } = [];

	/// <summary>The signature the mapping was read from.</summary>
	public IrSignature? Signature { get; init; }

	/// <summary>Why the signature was refused, when it was.</summary>
	public string? RefusalReason { get; init; }

	/// <summary>Family the refusal belongs to.</summary>
	public SkipCategory RefusalCategory { get; init; }

	/// <summary>Builds a successful mapping.</summary>
	/// <param name="overloads">The methods a caller sees, one per distinct C# signature.</param>
	/// <param name="dropped">Parameters left out.</param>
	/// <param name="signature">The signature that was mapped.</param>
	/// <returns>The mapping.</returns>
	public static MappedMethod Mapped(
		IReadOnlyList<IReadOnlyList<MappedParameter>> overloads,
		IReadOnlyList<DroppedParameter> dropped,
		IrSignature signature)
	{
		return new MappedMethod
		{
			IsMapped = true,
			Overloads = overloads,
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
