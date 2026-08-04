using System.Globalization;
using System.Text.RegularExpressions;
using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Emits one three.js class as a C# <c>ThreeObject</c> subclass. The shape it produces is the one
/// the hand-written classes settled on: readonly backing fields, an optional-argument constructor,
/// a literal <c>ThreeTypeName</c>, and <c>ConstructorArgs</c> in three.js parameter order.
/// </summary>
internal sealed class ClassEmitter
{
	private static readonly Regex _identifierPattern = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

	private readonly IrRoot _ir;
	private readonly Dictionary<string, IrClass> _classesByName;
	private readonly HashSet<string> _baseClassNames;

	/// <summary>Builds an emitter over one IR snapshot.</summary>
	/// <param name="ir">The parsed IR.</param>
	public ClassEmitter(IrRoot ir)
	{
		_ir = ir;
		_classesByName = [];
		foreach (var irClass in ir.Classes)
		{
			_classesByName.TryAdd(irClass.Name, irClass);
		}

		_baseClassNames = ir.Classes
			.Select(x => x.Extends?.Name)
			.Where(x => x is not null)
			.Select(x => x!)
			.ToHashSet(StringComparer.Ordinal);
	}

	/// <summary>Looks up a class by its declared name.</summary>
	/// <param name="name">Declared class name.</param>
	/// <returns>The IR entry.</returns>
	/// <exception cref="InvalidOperationException">Thrown when no class of that name is in scope.</exception>
	public IrClass GetClass(string name)
	{
		if (!_classesByName.TryGetValue(name, out var irClass))
		{
			throw new InvalidOperationException($"'{name}' is not a class in the IR.");
		}

		return irClass;
	}

	/// <summary>
	/// Emits the C# source for one class, recording every inferred decision into
	/// <paramref name="audit"/>.
	/// </summary>
	/// <param name="irClass">Class to emit.</param>
	/// <param name="audit">Collector for numeric inferences and skipped members.</param>
	/// <returns>The generated file.</returns>
	/// <exception cref="UnsupportedMemberException">Thrown when the class cannot be mirrored exactly.</exception>
	public EmittedFile Emit(IrClass irClass, EmissionAudit audit)
	{
		var threeTypeName = ResolveThreeTypeName(irClass);
		var constructorParameters = ResolveConstructorParameters(irClass, threeTypeName, audit);
		var baseTypeName = ResolveBaseTypeName(irClass);

		var writer = new CSharpWriter();
		WriteFileHeader(writer);
		writer.WriteLine();
		writer.WriteLine($"using {EmitterConfig.CoreNamespace};");
		writer.WriteLine();
		writer.WriteLine($"namespace {EmitterConfig.GeneratedNamespace};");
		writer.WriteLine();

		WriteClassDocumentation(writer, irClass, threeTypeName, audit);

		var sealedModifier = _baseClassNames.Contains(irClass.Name)
			? string.Empty
			: "sealed ";

		writer.WriteLine($"public {sealedModifier}class {threeTypeName} : {baseTypeName}");
		writer.WriteLine("{");
		writer.Indent();

		foreach (var parameter in constructorParameters)
		{
			writer.WriteLine($"private readonly {parameter.CSharpTypeName} {parameter.FieldName};");
		}

		if (constructorParameters.Count > 0)
		{
			writer.WriteLine();
		}

		WriteConstructor(writer, irClass, threeTypeName, constructorParameters);
		writer.WriteLine();
		WriteThreeTypeName(writer, threeTypeName);

		if (constructorParameters.Count > 0)
		{
			writer.WriteLine();
			WriteConstructorArgs(writer, threeTypeName, constructorParameters);
		}

		writer.Outdent();
		writer.WriteLine("}");

		RecordSkippedMembers(irClass, audit);

		return new EmittedFile
		{
			RelativePath = $"src/Blazor.ThreeJS/Generated/{threeTypeName}.cs",
			Contents = writer.ToSource()
		};
	}

	/// <summary>
	/// Walks the three.js base chain and returns the nearest ancestor that has a C# mirror, falling
	/// back to <c>ThreeObject</c>. The hierarchy is deliberately flattened rather than invented:
	/// <c>BoxGeometry extends BufferGeometry</c>, and until <c>BufferGeometry</c> is wrapped there is
	/// no honest intermediate type to derive from.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <returns>C# base type name.</returns>
	private string ResolveBaseTypeName(IrClass irClass)
	{
		var currentBaseName = irClass.Extends?.Name;
		while (currentBaseName is not null)
		{
			if (EmitterConfig.ExistingCSharpTypeNames.Contains(currentBaseName))
			{
				return currentBaseName;
			}

			currentBaseName = _classesByName.TryGetValue(currentBaseName, out var baseClass)
				? baseClass.Extends?.Name
				: null;
		}

		return EmitterConfig.RootBaseTypeName;
	}

	/// <summary>
	/// Maps the three.js constructor signature onto C# parameters, refusing anything that would need
	/// a guess.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="threeTypeName">Export name, used in refusal messages.</param>
	/// <param name="audit">Collector for numeric inferences.</param>
	/// <returns>The resolved parameters, in three.js order.</returns>
	/// <exception cref="UnsupportedMemberException">Thrown for a signature the emitter cannot mirror.</exception>
	private static List<EmittedParameter> ResolveConstructorParameters(IrClass irClass, string threeTypeName, EmissionAudit audit)
	{
		if (!irClass.IsExported)
		{
			throw UnsupportedMemberException.For(threeTypeName, "the class is never exported, so it is not reachable on the THREE namespace the applier looks names up on");
		}

		if (!_identifierPattern.IsMatch(threeTypeName))
		{
			throw UnsupportedMemberException.For(threeTypeName, "the export name is not a usable C# identifier");
		}

		if (irClass.IsAbstract)
		{
			throw UnsupportedMemberException.For(threeTypeName, "the class is abstract, so it has no constructor to mirror");
		}

		if (irClass.Constructors.Count > 1)
		{
			throw UnsupportedMemberException.For(threeTypeName, $"{irClass.Constructors.Count} constructor overloads; C# overload emission is not implemented");
		}

		if (irClass.Constructors.Count == 0)
		{
			return [];
		}

		var parameters = new List<EmittedParameter>();
		var hasSeenOptional = false;
		foreach (var irParameter in irClass.Constructors[0].Parameters)
		{
			if (irParameter.IsRest)
			{
				throw UnsupportedMemberException.For(threeTypeName, $"parameter '{irParameter.Name}' is a rest parameter");
			}

			if (irParameter.Type is not { Kind: "primitive", Name: "number" })
			{
				var typeText = irParameter.Type?.Text ?? "<missing>";
				throw UnsupportedMemberException.For(threeTypeName, $"parameter '{irParameter.Name}' is typed '{typeText}', and only 'number' is mapped so far");
			}

			var resolution = NumericKindResolver.Resolve(irParameter.Name, irParameter.NumericKind);
			audit.RecordNumeric(threeTypeName, irClass.File, irParameter.Name, resolution);

			string? defaultLiteral = null;
			if (irParameter.IsOptional)
			{
				if (irParameter.DefaultValue is null)
				{
					throw UnsupportedMemberException.For(
						threeTypeName,
						$"parameter '{irParameter.Name}' is optional but undocumented, so three.js's own default is unknown; " +
						$"emitting a C# default would send a concrete value where JavaScript expects 'undefined'");
				}

				defaultLiteral = RenderDefaultLiteral(threeTypeName, irParameter.Name, irParameter.DefaultValue, resolution.CSharpTypeName);
				hasSeenOptional = true;
			}
			else if (hasSeenOptional)
			{
				throw UnsupportedMemberException.For(threeTypeName, $"required parameter '{irParameter.Name}' follows an optional one, which C# forbids");
			}

			parameters.Add(new EmittedParameter
			{
				Name = ToCamelCase(irParameter.Name),
				FieldName = "_" + ToCamelCase(irParameter.Name),
				ThreeName = irParameter.Name,
				CSharpTypeName = resolution.CSharpTypeName,
				DefaultLiteral = defaultLiteral,
				Documentation = irParameter.Doc
			});
		}

		return parameters;
	}

	/// <summary>
	/// Converts a documented JavaScript default into a C# literal of the resolved type. A default the
	/// emitter cannot parse (<c>Math.PI</c>, an object literal) is a refusal rather than a guess.
	/// </summary>
	/// <param name="threeTypeName">Class being emitted, for the refusal message.</param>
	/// <param name="parameterName">Parameter being defaulted.</param>
	/// <param name="documentedDefault">Verbatim default text from the JSDoc.</param>
	/// <param name="cSharpTypeName">Resolved C# type.</param>
	/// <returns>The C# literal.</returns>
	/// <exception cref="UnsupportedMemberException">Thrown when the default is not a plain number.</exception>
	private static string RenderDefaultLiteral(string threeTypeName, string parameterName, string documentedDefault, string cSharpTypeName)
	{
		var text = documentedDefault.Trim().Trim('`');
		switch (cSharpTypeName)
		{
			case NumericKindResolver.IntegerTypeName:
				if (!long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integerValue))
				{
					throw UnsupportedMemberException.For(threeTypeName, $"parameter '{parameterName}' documents the default '{documentedDefault}', which is not an integer literal");
				}

				return integerValue.ToString(CultureInfo.InvariantCulture);
			case NumericKindResolver.FloatTypeName:
				if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
				{
					throw UnsupportedMemberException.For(threeTypeName, $"parameter '{parameterName}' documents the default '{documentedDefault}', which is not a numeric literal");
				}

				return floatValue.ToString("R", CultureInfo.InvariantCulture) + "f";
			default:
				throw new NotImplementedException($"Unhandled C# numeric type '{cSharpTypeName}'.");
		}
	}

	/// <summary>Writes the provenance header. See <see cref="WriteFileHeader"/> for why it is not an auto-generated marker.</summary>
	/// <param name="writer">Destination.</param>
	private void WriteFileHeader(CSharpWriter writer)
	{
		var typesPackage = _ir.Meta?.TypesPackage ?? "@types/three";
		var typesVersion = _ir.Meta?.TypesVersion ?? "unknown";

		// Deliberately a plain comment rather than an "<auto-generated/>" marker. That marker makes
		// Roslyn treat the file as generated code and stop reporting *analyzer* diagnostics in it, so
		// adding an analyzer later would silently stop covering the generated half of the package.
		// CS1591 itself is a compiler diagnostic and does still fire either way, which is what keeps
		// "every generated public member is documented" an enforced property rather than a hope.
		writer.WriteLine($"// Generated from {typesPackage}@{typesVersion} by generator/emitter. Do not edit by hand.");
		writer.WriteLine("// Re-run `npm run emit` after changing the emitter or generator/three-api.json.");
	}

	/// <summary>Writes the class-level documentation block.</summary>
	/// <param name="writer">Destination.</param>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="threeTypeName">Export name.</param>
	/// <param name="audit">Collector, for recording documentation the emitter drops.</param>
	private static void WriteClassDocumentation(CSharpWriter writer, IrClass irClass, string threeTypeName, EmissionAudit audit)
	{
		var summary = irClass.Doc?.Summary is { Length: > 0 } rawSummary
			? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawSummary))
			: $"The JavaScript-side <c>THREE.{threeTypeName}</c>.";

		if (irClass.Doc?.Summary is { Length: > 0 })
		{
			summary += $" The JavaScript-side <c>THREE.{threeTypeName}</c>.";
		}

		DocCommentEmitter.WriteSummary(writer, summary);

		if (irClass.Doc?.Remarks is { Length: > 0 } remarks)
		{
			DocCommentEmitter.WriteRemarks(writer, DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(remarks)));
		}

		DocCommentEmitter.WriteSeeAlso(writer, irClass.Doc?.See ?? []);

		if (irClass.Doc?.Examples.Count > 0)
		{
			audit.RecordSkippedMember(threeTypeName, "@example", $"{irClass.Doc.Examples.Count} TypeScript example block(s), which would be misleading in C# documentation");
		}
	}

	/// <summary>Writes the constructor, its documentation, and the field assignments.</summary>
	/// <param name="writer">Destination.</param>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="threeTypeName">Export name.</param>
	/// <param name="parameters">Resolved parameters.</param>
	private static void WriteConstructor(CSharpWriter writer, IrClass irClass, string threeTypeName, List<EmittedParameter> parameters)
	{
		var constructorSummary = irClass.Constructors.FirstOrDefault()?.Doc?.Summary is { Length: > 0 } rawSummary
			? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawSummary))
			: $"Initializes a new <see cref=\"{threeTypeName}\"/>.";

		DocCommentEmitter.WriteSummary(writer, constructorSummary);

		foreach (var parameter in parameters)
		{
			var text = parameter.Documentation is { Length: > 0 } documentation
				? DocCommentEmitter.RenderInline(DocCommentEmitter.StripRedundantTail(documentation))
				: $"Value forwarded to the <c>{parameter.ThreeName}</c> constructor argument.";

			DocCommentEmitter.WriteParam(writer, parameter.Name, text);
		}

		var declaredParameters = parameters
			.Select(x => x.DefaultLiteral is null
				? $"{x.CSharpTypeName} {x.Name}"
				: $"{x.CSharpTypeName} {x.Name} = {x.DefaultLiteral}")
			.ToList();

		var singleLine = $"public {threeTypeName}({string.Join(", ", declaredParameters)})";
		if (writer.IndentColumn + singleLine.Length <= EmitterConfig.DeclarationWrapColumn)
		{
			writer.WriteLine(singleLine);
		}
		else
		{
			writer.WriteLine($"public {threeTypeName}(");
			writer.Indent();
			foreach (var (index, declaredParameter) in declaredParameters.Index())
			{
				var isLast = index == declaredParameters.Count - 1;
				writer.WriteLine(isLast
					? declaredParameter + ")"
					: declaredParameter + ",");
			}

			writer.Outdent();
		}

		writer.WriteLine("{");
		writer.Indent();
		foreach (var parameter in parameters)
		{
			writer.WriteLine($"{parameter.FieldName} = {parameter.Name};");
		}

		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>
	/// Writes <c>ThreeTypeName</c>. The value is a string literal, never <c>nameof</c>: it is a wire
	/// token the JavaScript applier looks up on the <c>THREE</c> namespace, so renaming the C# type
	/// must not silently change the protocol.
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="threeTypeName">Export name.</param>
	private static void WriteThreeTypeName(CSharpWriter writer, string threeTypeName)
	{
		DocCommentEmitter.WriteSummary(writer, $"Name of the corresponding three.js constructor, <c>THREE.{threeTypeName}</c>.");
		writer.WriteLine("protected override string ThreeTypeName");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine($"get {{ return \"{threeTypeName}\"; }}");
		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>Writes <c>ConstructorArgs</c>, forwarding the backing fields in three.js parameter order.</summary>
	/// <param name="writer">Destination.</param>
	/// <param name="threeTypeName">Export name.</param>
	/// <param name="parameters">Resolved parameters.</param>
	private static void WriteConstructorArgs(CSharpWriter writer, string threeTypeName, List<EmittedParameter> parameters)
	{
		var parameterList = string.Join(", ", parameters.Select(x => x.ThreeName));
		DocCommentEmitter.WriteSummary(writer, $"Constructor arguments forwarded to <c>THREE.{threeTypeName}</c>: {parameterList}.");
		writer.WriteLine("protected override object?[] ConstructorArgs");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine($"get {{ return [{string.Join(", ", parameters.Select(x => x.FieldName))}]; }}");
		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>
	/// Records every member the emitter left out, so the gap between the generated class and the real
	/// three.js class is visible instead of implied.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="audit">Collector.</param>
	private static void RecordSkippedMembers(IrClass irClass, EmissionAudit audit)
	{
		var threeTypeName = irClass.ExportName ?? irClass.Name;
		foreach (var property in irClass.Properties)
		{
			var reason = property switch
			{
				{ IsStatic: true } => "static",
				{ IsReadonly: true } => "read-only in three.js, and reads never leave C#",
				{ Visibility: not null } => $"visibility '{property.Visibility}'",
				_ => "settable property emission is not implemented yet"
			};

			audit.RecordSkippedMember(threeTypeName, $"property {property.Name}", reason);
		}

		foreach (var method in irClass.Methods)
		{
			var reason = method switch
			{
				{ IsStatic: true } => "static",
				{ Visibility: not null } => $"visibility '{method.Visibility}'",
				_ => "method emission is not implemented yet"
			};

			audit.RecordSkippedMember(threeTypeName, $"method {method.Name}", reason);
		}
	}

	/// <summary>
	/// Resolves the name three.js exports the class under, which is both the C# type name and the wire
	/// token. Every <c>exportName</c> in the current snapshot is the literal <c>default</c>, from
	/// <c>export default class X</c> — that names the module's default binding, not the symbol on the
	/// <c>THREE</c> namespace, so the declared name wins over it.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <returns>The three.js export name.</returns>
	private static string ResolveThreeTypeName(IrClass irClass)
	{
		if (irClass.ExportName is not null && irClass.ExportName != "default")
		{
			return irClass.ExportName;
		}

		return irClass.Name;
	}

	/// <summary>Lower-cases the first character so a three.js parameter name reads as a C# parameter.</summary>
	/// <param name="name">Three.js parameter name.</param>
	/// <returns>The camelCased name.</returns>
	private static string ToCamelCase(string name)
	{
		if (name.Length == 0 || char.IsLower(name[0]))
		{
			return name;
		}

		return char.ToLowerInvariant(name[0]) + name[1..];
	}
}

/// <summary>One constructor parameter, resolved from the IR into C# terms.</summary>
internal sealed class EmittedParameter
{
	/// <summary>C# parameter name.</summary>
	public required string Name { get; init; }

	/// <summary>Backing field name, underscore-prefixed.</summary>
	public required string FieldName { get; init; }

	/// <summary>Original three.js parameter name, used in documentation.</summary>
	public required string ThreeName { get; init; }

	/// <summary>Resolved C# type.</summary>
	public required string CSharpTypeName { get; init; }

	/// <summary>C# default literal, or <see langword="null"/> for a required parameter.</summary>
	public string? DefaultLiteral { get; init; }

	/// <summary>Raw JSDoc text for this parameter.</summary>
	public string? Documentation { get; init; }
}

/// <summary>A generated file and where it belongs in the repository.</summary>
internal sealed class EmittedFile
{
	/// <summary>Repository-relative POSIX path.</summary>
	public required string RelativePath { get; init; }

	/// <summary>File contents, LF-terminated.</summary>
	public required string Contents { get; init; }
}
