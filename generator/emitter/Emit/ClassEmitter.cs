using Blazor.ThreeJS.Emitter.Ir;
using Blazor.ThreeJS.Emitter.Map;

namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Emits one three.js class as a C# <c>ThreeObject</c> subclass. The shape it produces is the one
/// the hand-written classes settled on: readonly backing fields, an optional-argument constructor,
/// a literal <c>ThreeTypeName</c>, and <c>ConstructorArgs</c> in three.js parameter order.
/// </summary>
internal sealed class ClassEmitter
{
	private readonly IrRoot _ir;
	private readonly TypeMapper _mapper;
	private readonly ConstructorMapper _constructorMapper;
	private readonly MemberClassifier _classifier;
	private readonly Dictionary<string, IrClass> _classesByName;
	private readonly HashSet<string> _baseClassNames;

	/// <summary>Builds an emitter over one IR snapshot.</summary>
	/// <param name="ir">The parsed IR.</param>
	/// <param name="mapper">Type mapper, already attached to the emission scope.</param>
	/// <param name="constructorMapper">Constructor mapping, shared with the emission scope.</param>
	/// <param name="classifier">Member classifier, so the audit and the coverage report cannot disagree.</param>
	public ClassEmitter(IrRoot ir, TypeMapper mapper, ConstructorMapper constructorMapper, MemberClassifier classifier)
	{
		_ir = ir;
		_mapper = mapper;
		_constructorMapper = constructorMapper;
		_classifier = classifier;
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
		var constructor = ResolveConstructor(irClass, threeTypeName, audit);
		var constructorParameters = constructor.Parameters;
		var baseTypeName = ResolveBaseTypeName(irClass);

		var writer = new CSharpWriter();
		WriteFileHeader(writer);
		writer.WriteLine();
		writer.WriteLine($"using {EmitterConfig.CoreNamespace};");

		// The hand-written math types live in their own namespace, so referencing one pulls in a second
		// using. Emitted only when it is actually needed: an unused using is a warning under
		// TreatWarningsAsErrors on a consumer that turns IDE0005 on.
		if (constructorParameters.Any(x => x.Mapping.Kind == TypeMappingKind.HandWrittenMathType))
		{
			writer.WriteLine($"using {EmitterConfig.MathNamespace};");
		}

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
			WriteConstructorArgs(writer, threeTypeName, constructor);
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
	public string ResolveBaseTypeName(IrClass irClass)
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
	/// Resolves the constructor through the shared <see cref="ConstructorMapper"/> and records every
	/// numeric call and every dropped parameter, turning a refusal into the emitter's own exception so
	/// the caller sees one failure mode.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="threeTypeName">Export name, used in refusal messages.</param>
	/// <param name="audit">Collector for numeric inferences and dropped parameters.</param>
	/// <returns>The resolved constructor.</returns>
	/// <exception cref="UnsupportedMemberException">Thrown for a signature the emitter cannot mirror.</exception>
	private MappedConstructor ResolveConstructor(IrClass irClass, string threeTypeName, EmissionAudit audit)
	{
		if (!irClass.IsExported)
		{
			throw UnsupportedMemberException.For(threeTypeName, "the class is never exported, so it is not reachable on the THREE namespace the applier looks names up on");
		}

		if (!CSharpIdentifier.IsValid(threeTypeName))
		{
			throw UnsupportedMemberException.For(threeTypeName, "the export name is not a usable C# identifier");
		}

		if (irClass.IsAbstract)
		{
			throw UnsupportedMemberException.For(threeTypeName, "the class is abstract, so it has no constructor to mirror");
		}

		var constructor = _constructorMapper.Map(irClass, _mapper);
		if (!constructor.IsMapped)
		{
			throw UnsupportedMemberException.For(threeTypeName, constructor.RefusalReason!);
		}

		foreach (var parameter in constructor.Parameters)
		{
			if (parameter.Mapping.Numeric is { } numeric)
			{
				audit.RecordNumeric(threeTypeName, irClass.File, parameter.ThreeName, numeric);
			}
		}

		foreach (var droppedParameter in constructor.DroppedParameters)
		{
			audit.RecordSkippedMember(threeTypeName, $"constructor parameter {droppedParameter.Name}", droppedParameter.Reason);
		}

		return constructor;
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
	private static void WriteConstructor(CSharpWriter writer, IrClass irClass, string threeTypeName, IReadOnlyList<MappedParameter> parameters)
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
				? $"{x.CSharpTypeName} {x.DeclarationName}"
				: $"{x.CSharpTypeName} {x.DeclarationName} = {x.DefaultLiteral}")
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
			writer.WriteLine($"{parameter.FieldName} = {parameter.DeclarationName};");
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

	/// <summary>
	/// Writes <c>ConstructorArgs</c>, forwarding the backing fields in three.js parameter order.
	/// <para>
	/// A parameter the caller left unspecified is forwarded as the <c>$undef</c> sentinel, which the
	/// applier decodes to JavaScript's <c>undefined</c> so three.js applies its own default. A JSON
	/// <c>null</c> could not: <c>f(a = 1)</c> called as <c>f(null)</c> yields <c>null</c>, not
	/// <c>1</c>. Trailing sentinels are then trimmed, which says the same thing in fewer bytes and
	/// keeps <c>arguments.length</c> equal to what a hand-written JavaScript call would produce.
	/// </para>
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="threeTypeName">Export name.</param>
	/// <param name="constructor">Resolved constructor.</param>
	private static void WriteConstructorArgs(CSharpWriter writer, string threeTypeName, MappedConstructor constructor)
	{
		var parameters = constructor.Parameters;
		var parameterList = string.Join(", ", parameters.Select(x => x.ThreeName));
		var summary = $"Constructor arguments forwarded to <c>THREE.{threeTypeName}</c>: {parameterList}.";
		if (constructor.HasUnspecifiedNullable)
		{
			summary += " An argument the caller left unspecified travels as the wire's not-supplied " +
				"sentinel, or is trimmed when nothing supplied follows it, so three.js applies its own default.";
		}

		DocCommentEmitter.WriteSummary(writer, summary);
		writer.WriteLine("protected override object?[] ConstructorArgs");
		writer.WriteLine("{");
		writer.Indent();

		if (!constructor.HasUnspecifiedNullable)
		{
			writer.WriteLine($"get {{ return [{string.Join(", ", parameters.Select(x => x.FieldName))}]; }}");
			writer.Outdent();
			writer.WriteLine("}");
			return;
		}

		var arguments = parameters
			.Select(x => x.IsUnspecifiedNullable
				? $"{EmitterConfig.OrUnspecifiedCall}({x.FieldName})"
				: x.FieldName)
			.ToList();

		var singleLine = $"get {{ return {EmitterConfig.TrimUnspecifiedTailCall}([{string.Join(", ", arguments)}]); }}";
		if (writer.IndentColumn + singleLine.Length <= EmitterConfig.DeclarationWrapColumn)
		{
			writer.WriteLine(singleLine);
			writer.Outdent();
			writer.WriteLine("}");
			return;
		}

		writer.WriteLine("get");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine($"return {EmitterConfig.TrimUnspecifiedTailCall}(");
		writer.WriteLine("[");
		writer.Indent();
		foreach (var (index, argument) in arguments.Index())
		{
			writer.WriteLine(index == arguments.Count - 1
				? argument
				: argument + ",");
		}

		writer.Outdent();
		writer.WriteLine("]);");
		writer.Outdent();
		writer.WriteLine("}");
		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>
	/// Records every member the emitter left out, so the gap between the generated class and the real
	/// three.js class is visible instead of implied.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="audit">Collector.</param>
	private void RecordSkippedMembers(IrClass irClass, EmissionAudit audit)
	{
		var threeTypeName = ResolveThreeTypeName(irClass);
		foreach (var member in _classifier.Classify(irClass))
		{
			var kind = member.MemberKind == ClassifiedMemberKind.Property ? "property" : "method";
			var reason = member.Bucket switch
			{
				MemberBucket.Skipped => member.SkipReason!,
				MemberBucket.MirroredState => "classified as mirrored state; property emission lands in a later task",
				MemberBucket.Command => "classified as a command; method emission lands in a later task",
				MemberBucket.AsyncQuery => "classified as an async query; the wire format has no read op yet",
				_ => throw new NotImplementedException($"Unhandled {nameof(MemberBucket)} '{member.Bucket}'.")
			};

			audit.RecordSkippedMember(threeTypeName, $"{kind} {member.MemberName}", reason);
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

}

/// <summary>A generated file and where it belongs in the repository.</summary>
internal sealed class EmittedFile
{
	/// <summary>Repository-relative POSIX path.</summary>
	public required string RelativePath { get; init; }

	/// <summary>File contents, LF-terminated.</summary>
	public required string Contents { get; init; }
}
