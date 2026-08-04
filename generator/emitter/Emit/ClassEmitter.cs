using Blazor.ThreeJS.Emitter.Ir;
using Blazor.ThreeJS.Emitter.Map;

namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Emits one three.js class as a C# <c>ThreeObject</c> subclass. The shape it produces is the one the
/// hand-written classes settled on: backing fields, an optional-argument constructor, a literal
/// <c>ThreeTypeName</c>, <c>ConstructorArgs</c> in three.js parameter order, a property per piece of
/// mirrored state, a method per command, and a replay of everything the caller wrote before the
/// object was attached.
/// </summary>
internal sealed class ClassEmitter
{
	private readonly IrRoot _ir;
	private readonly TypeMapper _mapper;
	private readonly ConstructorMapper _constructorMapper;
	private readonly MemberClassifier _classifier;
	private readonly EmissionScope _scope;
	private readonly Dictionary<string, IrClass> _classesByName;
	private readonly HashSet<string> _baseClassNames;

	/// <summary>Builds an emitter over one IR snapshot.</summary>
	/// <param name="ir">The parsed IR.</param>
	/// <param name="mapper">Type mapper, already attached to the emission scope.</param>
	/// <param name="constructorMapper">Constructor mapping, shared with the emission scope.</param>
	/// <param name="classifier">Member classifier, so the audit and the coverage report cannot disagree.</param>
	/// <param name="scope">Emission scope, for deciding which ancestors have a C# type of their own.</param>
	public ClassEmitter(
		IrRoot ir,
		TypeMapper mapper,
		ConstructorMapper constructorMapper,
		MemberClassifier classifier,
		EmissionScope scope)
	{
		_ir = ir;
		_mapper = mapper;
		_constructorMapper = constructorMapper;
		_classifier = classifier;
		_scope = scope;
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
		var surface = ResolveEmittedSurface(irClass, threeTypeName, constructorParameters, audit);
		var fields = ResolveFields(constructorParameters, surface.Properties);

		var writer = new CSharpWriter();
		WriteFileHeader(writer);
		writer.WriteLine();
		writer.WriteLine($"using {EmitterConfig.CoreNamespace};");

		// The hand-written math types live in their own namespace, so referencing one pulls in a second
		// using. Emitted only when it is actually needed: an unused using is a warning under
		// TreatWarningsAsErrors on a consumer that turns IDE0005 on.
		if (UsesMathTypes(constructorParameters, surface))
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

		WriteFields(writer, fields, surface.Properties);
		WriteOwnedMathProperties(writer, surface.Properties);
		WriteConstructor(writer, irClass, threeTypeName, constructorParameters, surface.Properties);
		writer.WriteLine();
		WriteThreeTypeName(writer, threeTypeName);

		if (constructorParameters.Count > 0)
		{
			writer.WriteLine();
			WriteConstructorArgs(writer, threeTypeName, constructor);
		}

		foreach (var property in surface.Properties.Where(x => !x.IsOwnedMathValue))
		{
			writer.WriteLine();
			WriteProperty(writer, threeTypeName, property);
		}

		foreach (var command in surface.Commands)
		{
			writer.WriteLine();
			WriteCommand(writer, command);
		}

		WriteAttachmentAndReplay(writer, irClass, threeTypeName, constructorParameters, surface.Properties);

		writer.Outdent();
		writer.WriteLine("}");

		return new EmittedFile
		{
			RelativePath = $"src/Blazor.ThreeJS/Generated/{threeTypeName}.cs",
			Contents = writer.ToSource()
		};
	}

	/// <summary>
	/// Walks the three.js base chain and returns the nearest ancestor that has a C# type of its own,
	/// falling back to <c>ThreeObject</c>. An ancestor with no mirror is not invented: its members are
	/// folded into this class by the surface resolver instead.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <returns>C# base type name.</returns>
	public string ResolveBaseTypeName(IrClass irClass)
	{
		var currentBaseName = irClass.Extends?.Name;
		while (currentBaseName is not null)
		{
			if (IsMirrored(currentBaseName))
			{
				return currentBaseName;
			}

			currentBaseName = _classesByName.TryGetValue(currentBaseName, out var baseClass)
				? baseClass.Extends?.Name
				: null;
		}

		return EmitterConfig.RootBaseTypeName;
	}

	/// <summary>Whether a class name resolves to a C# type — one being generated, or a hand-written one.</summary>
	/// <param name="name">Three.js class name.</param>
	/// <returns><see langword="true"/> when C# has a type for it.</returns>
	private bool IsMirrored(string name)
	{
		return _scope.IsEmittable(name) || EmitterConfig.HandWrittenClassNames.Contains(name);
	}

	/// <summary>
	/// Whether the emitted class descends from the hand-written scene-graph root. Those replay their
	/// state from <c>EmitState</c>, which <c>Object3D.AttachTo</c> calls after the create op and after
	/// the transform has been replayed; everything else has no such hook and replays from
	/// <c>EmitCreate</c>.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <returns><see langword="true"/> when <c>Object3D</c> is on the C# base chain.</returns>
	private bool IsSceneGraphType(IrClass irClass)
	{
		var baseTypeName = ResolveBaseTypeName(irClass);
		while (baseTypeName != EmitterConfig.RootBaseTypeName)
		{
			if (baseTypeName == EmitterConfig.SceneGraphBaseTypeName)
			{
				return true;
			}

			if (!_classesByName.TryGetValue(baseTypeName, out var baseClass))
			{
				return false;
			}

			baseTypeName = ResolveBaseTypeName(baseClass);
		}

		return false;
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

	/// <summary>
	/// Turns the classified member set into the properties and commands this class actually emits,
	/// recording every member left out and why.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="threeTypeName">Export name.</param>
	/// <param name="constructorParameters">Resolved constructor parameters, which claim their names first.</param>
	/// <param name="audit">Collector.</param>
	/// <returns>The emitted surface.</returns>
	private EmittedSurface ResolveEmittedSurface(
		IrClass irClass,
		string threeTypeName,
		IReadOnlyList<MappedParameter> constructorParameters,
		EmissionAudit audit)
	{
		var parametersByThreeName = constructorParameters.ToDictionary(x => x.ThreeName, StringComparer.Ordinal);
		var takenCSharpNames = new HashSet<string>(StringComparer.Ordinal) { threeTypeName };
		var properties = new List<EmittedProperty>();
		var commands = new List<EmittedCommand>();

		foreach (var member in _classifier.Classify(irClass))
		{
			var kind = member.MemberKind == ClassifiedMemberKind.Property ? "property" : "method";
			if (member.Bucket == MemberBucket.Skipped)
			{
				audit.RecordSkippedMember(threeTypeName, $"{kind} {member.MemberName}", member.SkipReason!);
				continue;
			}

			if (member.Bucket == MemberBucket.AsyncQuery)
			{
				audit.RecordSkippedMember(threeTypeName, $"{kind} {member.MemberName}", "returns a value, and the wire format has create, set, call, add, remove and dispose — no op reads anything back");
				continue;
			}

			var cSharpName = ToPascalCase(member.MemberName);
			if (!takenCSharpNames.Add(cSharpName))
			{
				audit.RecordSkippedMember(
					threeTypeName,
					$"{kind} {member.MemberName}",
					$"its C# name `{cSharpName}` is already taken on this type, and C# holds one member of a given name");
				continue;
			}

			var fencedBlocks = DocCommentEmitter.CountFencedCodeBlocks(member.Property?.Doc?.Summary) +
				DocCommentEmitter.CountFencedCodeBlocks(member.Method?.Signature?.Doc?.Summary);

			if (fencedBlocks > 0)
			{
				audit.RecordSkippedMember(threeTypeName, $"fenced code in the {kind} {member.MemberName} summary", $"{fencedBlocks} JavaScript block(s) written inline in the prose, which would be misleading in C# documentation");
			}

			if (member.Bucket == MemberBucket.Command)
			{
				commands.Add(BuildCommand(member, cSharpName));
				continue;
			}

			var isOwnedMathValue = member.Mapping!.Kind == TypeMappingKind.HandWrittenMathType;
			if (isOwnedMathValue && EmitterConfig.MathTypeNamesWithoutChangeNotification.Contains(member.Mapping.CSharpTypeName!))
			{
				audit.RecordSkippedMember(
					threeTypeName,
					$"property {member.MemberName}",
					$"`{member.Mapping.CSharpTypeName}` hands its components out as a mutable array, so a change to it cannot be observed and there is nothing to record a property write from");

				takenCSharpNames.Remove(cSharpName);
				continue;
			}

			if (parametersByThreeName.TryGetValue(member.MemberName, out var parameter))
			{
				if (isOwnedMathValue)
				{
					// A math value is mirrored as an instance this object owns and watches for changes,
					// which cannot also be the constructor argument the caller may have left unspecified.
					// The constructor wins: it is the form three.js itself documents for these.
					audit.RecordSkippedMember(
						threeTypeName,
						$"property {member.MemberName}",
						"the constructor already takes it, and a math value is mirrored as an instance this object owns rather than as a settable field");
					takenCSharpNames.Remove(cSharpName);
					continue;
				}

				properties.Add(BuildProperty(member, cSharpName, parameter.FieldName, parameter.CSharpTypeName, null, isOwnedMathValue: false));
				continue;
			}

			var fieldName = "_" + ConstructorMapper.ToCamelCase(member.MemberName);
			var cSharpTypeName = ResolvePropertyTypeName(member.Mapping);
			var defaultLiteral = isOwnedMathValue
				? null
				: MethodMapper.RenderDefaultLiteral(ResolveDocumentedDefault(member), member.Mapping);

			properties.Add(BuildProperty(member, cSharpName, fieldName, cSharpTypeName, defaultLiteral, isOwnedMathValue));
		}

		return new EmittedSurface { Properties = properties, Commands = commands };
	}

	private static EmittedProperty BuildProperty(
		ClassifiedMember member,
		string cSharpName,
		string fieldName,
		string cSharpTypeName,
		string? defaultLiteral,
		bool isOwnedMathValue)
	{
		return new EmittedProperty
		{
			ThreeName = member.MemberName,
			CSharpName = cSharpName,
			FieldName = fieldName,
			WrittenFieldName = $"_is{cSharpName}Written",
			CSharpTypeName = cSharpTypeName,
			Mapping = member.Mapping!,
			DefaultLiteral = defaultLiteral,
			IsOwnedMathValue = isOwnedMathValue,
			Documentation = member.Property?.Doc,
			DocumentedMathDefault = isOwnedMathValue ? ResolveDocumentedDefault(member) : null
		};
	}

	private static EmittedCommand BuildCommand(ClassifiedMember member, string cSharpName)
	{
		return new EmittedCommand
		{
			ThreeName = member.MemberName,
			CSharpName = cSharpName,
			Parameters = member.Method!.Parameters,
			Documentation = member.Method.Signature?.Doc
		};
	}

	/// <summary>
	/// C# type of a mirrored property. A reference to another wrapped class is always nullable: the
	/// mirror has no instance to give it until the caller supplies one, and a non-nullable field with
	/// nothing to initialise it to is a compile warning that says the same thing less clearly.
	/// </summary>
	/// <param name="mapping">The property's resolved type.</param>
	/// <returns>The C# type as written.</returns>
	private static string ResolvePropertyTypeName(TypeMapping mapping)
	{
		if (mapping.Kind == TypeMappingKind.GeneratedWrapperClass || mapping.IsExplicitlyNullable)
		{
			return mapping.CSharpTypeName + "?";
		}

		return mapping.CSharpTypeName!;
	}

	private static string? ResolveDocumentedDefault(ClassifiedMember member)
	{
		return member.Property?.DefaultValue ?? member.Property?.Doc?.DefaultValue;
	}

	/// <summary>
	/// Merges the constructor's backing fields with the properties' into one list, so a property that
	/// three.js also takes as a constructor argument writes through the same field rather than holding
	/// a second copy that the two could disagree on.
	/// </summary>
	/// <param name="constructorParameters">Resolved constructor parameters.</param>
	/// <param name="properties">Emitted properties.</param>
	/// <returns>One field per name, in constructor-then-property order.</returns>
	private static List<EmittedField> ResolveFields(
		IReadOnlyList<MappedParameter> constructorParameters,
		IReadOnlyList<EmittedProperty> properties)
	{
		var reassignedFieldNames = properties
			.Where(x => !x.IsOwnedMathValue)
			.Select(x => x.FieldName)
			.ToHashSet(StringComparer.Ordinal);

		var fields = new List<EmittedField>();
		var declaredNames = new HashSet<string>(StringComparer.Ordinal);
		foreach (var parameter in constructorParameters)
		{
			declaredNames.Add(parameter.FieldName);
			fields.Add(new EmittedField
			{
				Name = parameter.FieldName,
				CSharpTypeName = parameter.CSharpTypeName,
				IsReadonly = !reassignedFieldNames.Contains(parameter.FieldName),
				InitializerLiteral = null
			});
		}

		foreach (var property in properties)
		{
			if (property.IsOwnedMathValue || !declaredNames.Add(property.FieldName))
			{
				continue;
			}

			fields.Add(new EmittedField
			{
				Name = property.FieldName,
				CSharpTypeName = property.CSharpTypeName,
				IsReadonly = false,
				InitializerLiteral = property.DefaultLiteral ?? DefaultInitializer(property.CSharpTypeName)
			});
		}

		return fields;
	}

	/// <summary>
	/// Initializer for a field the upstream documents no default for. Only non-nullable reference types
	/// need one at all; every other field's C# default is as good an answer as the mirror has.
	/// </summary>
	/// <param name="cSharpTypeName">Field type as written.</param>
	/// <returns>The initializer literal, or <see langword="null"/> when none is needed.</returns>
	private static string? DefaultInitializer(string cSharpTypeName)
	{
		return cSharpTypeName == "string"
			? "string.Empty"
			: null;
	}

	private bool UsesMathTypes(IReadOnlyList<MappedParameter> constructorParameters, EmittedSurface surface)
	{
		return constructorParameters.Any(x => x.Mapping.Kind == TypeMappingKind.HandWrittenMathType) ||
			surface.Properties.Any(x => x.Mapping.Kind == TypeMappingKind.HandWrittenMathType) ||
			surface.Commands.Any(command => command.Parameters.Any(x => x.Mapping.Kind == TypeMappingKind.HandWrittenMathType));
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

		var fencedBlocks = DocCommentEmitter.CountFencedCodeBlocks(irClass.Doc?.Summary) +
			DocCommentEmitter.CountFencedCodeBlocks(irClass.Doc?.Remarks);

		if (fencedBlocks > 0)
		{
			audit.RecordSkippedMember(threeTypeName, "fenced code in the class summary", $"{fencedBlocks} JavaScript block(s) written inline in the prose, which would be misleading in C# documentation");
		}
	}

	private static void WriteFields(CSharpWriter writer, IReadOnlyList<EmittedField> fields, IReadOnlyList<EmittedProperty> properties)
	{
		foreach (var field in fields)
		{
			var readonlyModifier = field.IsReadonly ? "readonly " : string.Empty;
			var initializer = field.InitializerLiteral is null
				? string.Empty
				: $" = {field.InitializerLiteral}";

			writer.WriteLine($"private {readonlyModifier}{field.CSharpTypeName} {field.Name}{initializer};");
		}

		// One flag per mirrored property, set the moment the caller writes it. The replay reads these
		// rather than the fields themselves: a value the caller never set is not state the mirror knows,
		// and writing our guess at three.js's default over three.js's actual default would be a silent
		// correction of the library by a mirror that has never read anything back from it.
		foreach (var property in properties)
		{
			writer.WriteLine($"private bool {property.WrittenFieldName};");
		}

		if (fields.Count > 0 || properties.Count > 0)
		{
			writer.WriteLine();
		}
	}

	private static void WriteOwnedMathProperties(CSharpWriter writer, IReadOnlyList<EmittedProperty> properties)
	{
		foreach (var property in properties.Where(x => x.IsOwnedMathValue))
		{
			WritePropertyDocumentation(writer, property, isOwnedMathValue: true);
			writer.WriteLine($"public {property.CSharpTypeName} {property.CSharpName} {{ get; }}");
			writer.WriteLine();
		}
	}

	/// <summary>Writes the constructor, its documentation, and the field assignments.</summary>
	/// <param name="writer">Destination.</param>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="threeTypeName">Export name.</param>
	/// <param name="parameters">Resolved parameters.</param>
	/// <param name="properties">Emitted properties, for wiring the owned math values.</param>
	private static void WriteConstructor(
		CSharpWriter writer,
		IrClass irClass,
		string threeTypeName,
		IReadOnlyList<MappedParameter> parameters,
		IReadOnlyList<EmittedProperty> properties)
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

		foreach (var (index, property) in properties.Where(x => x.IsOwnedMathValue).Index())
		{
			if (index > 0 || parameters.Count > 0)
			{
				writer.WriteLine();
			}

			writer.WriteLine($"{property.CSharpName} = {RenderMathConstruction(property)};");
			writer.WriteLine($"{property.CSharpName}.OnChange = () =>");
			writer.WriteLine("{");
			writer.Indent();
			writer.WriteLine($"{property.WrittenFieldName} = true;");
			writer.WriteLine($"RecordSet(\"{property.ThreeName}\", {property.CSharpName});");
			writer.Outdent();
			writer.WriteLine("};");
		}

		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>
	/// Builds the initial value of an owned math property. three.js documents these as a component
	/// tuple (<c>@default (1,1,1)</c>), which is renderable; anything else falls back to the type's own
	/// default, because inventing components would be a value three.js never agreed to.
	/// </summary>
	/// <param name="property">The property being initialised.</param>
	/// <returns>A C# construction expression.</returns>
	private static string RenderMathConstruction(EmittedProperty property)
	{
		var components = MathDefaultParser.TryParseComponents(property.DocumentedMathDefault, property.CSharpTypeName);
		return components is null
			? $"new {property.CSharpTypeName}()"
			: $"new {property.CSharpTypeName}({string.Join(", ", components)})";
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

	private static void WriteProperty(CSharpWriter writer, string threeTypeName, EmittedProperty property)
	{
		WritePropertyDocumentation(writer, property, isOwnedMathValue: false);
		writer.WriteLine($"public {property.CSharpTypeName} {property.CSharpName}");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine($"get {{ return {property.FieldName}; }}");
		writer.WriteLine("set");
		writer.WriteLine("{");
		writer.Indent();

		var isReferenceIdentity = property.Mapping.Kind == TypeMappingKind.GeneratedWrapperClass;
		var unchangedTest = isReferenceIdentity
			? $"ReferenceEquals({property.FieldName}, value)"
			: $"{property.FieldName} == value";

		writer.WriteLine($"if ({unchangedTest})");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine("return;");
		writer.Outdent();
		writer.WriteLine("}");
		writer.WriteLine();
		writer.WriteLine($"{property.FieldName} = value;");
		writer.WriteLine($"{property.WrittenFieldName} = true;");

		if (isReferenceIdentity)
		{
			writer.WriteLine("if (Batch is not null && value is not null)");
			writer.WriteLine("{");
			writer.Indent();
			writer.WriteLine("value.AttachTo(Batch);");
			writer.Outdent();
			writer.WriteLine("}");
			writer.WriteLine();
		}

		writer.WriteLine($"RecordSet(\"{property.ThreeName}\", value);");
		writer.Outdent();
		writer.WriteLine("}");
		writer.Outdent();
		writer.WriteLine("}");
	}

	private static void WritePropertyDocumentation(CSharpWriter writer, EmittedProperty property, bool isOwnedMathValue)
	{
		var summary = property.Documentation?.Summary is { Length: > 0 } rawSummary
			? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawSummary))
			: $"The <c>{property.ThreeName}</c> property of the JavaScript-side object.";

		summary += isOwnedMathValue
			? $" Mirrored as an instance this object owns: mutating it records a write of <c>{property.ThreeName}</c>."
			: $" Writing it records a <c>{property.ThreeName}</c> property write once this object is attached; writing the value already held records nothing.";

		DocCommentEmitter.WriteSummary(writer, summary);
	}

	private static void WriteCommand(CSharpWriter writer, EmittedCommand command)
	{
		var summary = command.Documentation?.Summary is { Length: > 0 } rawSummary
			? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawSummary))
			: $"Records a call to <c>{command.ThreeName}</c> on the JavaScript-side object.";

		DocCommentEmitter.WriteSummary(writer, summary);
		foreach (var parameter in command.Parameters)
		{
			var text = parameter.Documentation is { Length: > 0 } documentation
				? DocCommentEmitter.RenderInline(DocCommentEmitter.StripRedundantTail(documentation))
				: $"Value forwarded to the <c>{parameter.ThreeName}</c> argument.";

			DocCommentEmitter.WriteParam(writer, parameter.Name, text);
		}

		var declaredParameters = command.Parameters
			.Select(x => x.DefaultLiteral is null
				? $"{x.CSharpTypeName} {x.DeclarationName}"
				: $"{x.CSharpTypeName} {x.DeclarationName} = {x.DefaultLiteral}")
			.ToList();

		var declaration = $"public void {command.CSharpName}({string.Join(", ", declaredParameters)})";
		if (writer.IndentColumn + declaration.Length <= EmitterConfig.DeclarationWrapColumn)
		{
			writer.WriteLine(declaration);
		}
		else
		{
			writer.WriteLine($"public void {command.CSharpName}(");
			writer.Indent();
			foreach (var (index, declaredParameter) in declaredParameters.Index())
			{
				writer.WriteLine(index == declaredParameters.Count - 1
					? declaredParameter + ")"
					: declaredParameter + ",");
			}

			writer.Outdent();
		}

		writer.WriteLine("{");
		writer.Indent();

		// An argument that is itself a mirrored object has to exist on the JavaScript side before the
		// call that references it by handle. Attaching rather than emitting its create op directly is
		// what keeps a shared instance from being created twice.
		var attachedParameters = command.Parameters
			.Where(x => x.Mapping.Kind == TypeMappingKind.GeneratedWrapperClass)
			.ToList();

		foreach (var parameter in attachedParameters)
		{
			var guard = parameter.CSharpTypeName.EndsWith('?')
				? $"if (Batch is not null && {parameter.DeclarationName} is not null)"
				: "if (Batch is not null)";

			writer.WriteLine(guard);
			writer.WriteLine("{");
			writer.Indent();
			writer.WriteLine($"{parameter.DeclarationName}.AttachTo(Batch);");
			writer.Outdent();
			writer.WriteLine("}");
			writer.WriteLine();
		}

		var arguments = command.Parameters.Count == 0
			? string.Empty
			: ", " + string.Join(", ", command.Parameters.Select(x => x.DeclarationName));

		writer.WriteLine($"RecordCall(\"{command.ThreeName}\"{arguments});");
		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>
	/// Writes the two attachment hooks: one that gets this object's dependencies onto the JavaScript
	/// side before the create op that references them by handle, and one that replays the state the
	/// caller wrote before this object was attached.
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="threeTypeName">Export name.</param>
	/// <param name="constructorParameters">Resolved constructor parameters.</param>
	/// <param name="properties">Emitted properties.</param>
	private void WriteAttachmentAndReplay(
		CSharpWriter writer,
		IrClass irClass,
		string threeTypeName,
		IReadOnlyList<MappedParameter> constructorParameters,
		IReadOnlyList<EmittedProperty> properties)
	{
		var dependencies = constructorParameters
			.Where(x => x.Mapping.Kind == TypeMappingKind.GeneratedWrapperClass)
			.ToList();

		var isSceneGraphType = IsSceneGraphType(irClass);
		var hasReplay = properties.Count > 0;

		if (dependencies.Count > 0 || (hasReplay && !isSceneGraphType))
		{
			writer.WriteLine();
			DocCommentEmitter.WriteSummary(
				writer,
				dependencies.Count > 0
					? $"Attaches the objects <c>THREE.{threeTypeName}</c> is constructed from, so their create ops reach the batch before the one that references them by handle, then emits this object's own."
					: $"Emits the create op for <c>THREE.{threeTypeName}</c>, then replays every property written before this object was attached.");

			DocCommentEmitter.WriteParam(writer, "batch", "Batch to record the ops into.");
			writer.WriteLine("internal override void EmitCreate(ThreeBatch batch)");
			writer.WriteLine("{");
			writer.Indent();
			foreach (var dependency in dependencies)
			{
				writer.WriteLine(dependency.CSharpTypeName.EndsWith('?')
					? $"{dependency.FieldName}?.AttachTo(batch);"
					: $"{dependency.FieldName}.AttachTo(batch);");
			}

			if (dependencies.Count > 0)
			{
				writer.WriteLine();
			}

			writer.WriteLine("base.EmitCreate(batch);");
			if (hasReplay && !isSceneGraphType)
			{
				WriteReplayBody(writer, properties);
			}

			writer.Outdent();
			writer.WriteLine("}");
		}

		if (!hasReplay || !isSceneGraphType)
		{
			return;
		}

		writer.WriteLine();
		DocCommentEmitter.WriteSummary(
			writer,
			"Replays every property written before this object was attached, so construction order never matters to the caller. A property the caller never wrote is left alone: three.js's own default is the truth for it, and the mirror has never read anything back to improve on that.");

		DocCommentEmitter.WriteParam(writer, "batch", "Batch to record the property writes into.");
		writer.WriteLine("internal override void EmitState(ThreeBatch batch)");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine("base.EmitState(batch);");
		WriteReplayBody(writer, properties);
		writer.Outdent();
		writer.WriteLine("}");
	}

	private static void WriteReplayBody(CSharpWriter writer, IReadOnlyList<EmittedProperty> properties)
	{
		foreach (var property in properties)
		{
			writer.WriteLine();
			writer.WriteLine($"if ({property.WrittenFieldName})");
			writer.WriteLine("{");
			writer.Indent();
			var value = property.IsOwnedMathValue
				? property.CSharpName
				: property.FieldName;

			writer.WriteLine($"batch.Set(Handle, \"{property.ThreeName}\", ThreeValue.Encode({value}));");
			writer.Outdent();
			writer.WriteLine("}");
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

	/// <summary>Upper-cases the first character so a three.js member name reads as a C# member.</summary>
	/// <param name="name">Three.js member name.</param>
	/// <returns>The PascalCased name.</returns>
	private static string ToPascalCase(string name)
	{
		if (name.Length == 0 || char.IsUpper(name[0]))
		{
			return name;
		}

		return char.ToUpperInvariant(name[0]) + name[1..];
	}
}

/// <summary>The members one class emits, once names have been resolved against the constructor.</summary>
internal sealed class EmittedSurface
{
	/// <summary>Mirrored state, in resolution order.</summary>
	public required IReadOnlyList<EmittedProperty> Properties { get; init; }

	/// <summary>Commands, in resolution order.</summary>
	public required IReadOnlyList<EmittedCommand> Commands { get; init; }
}

/// <summary>One backing field of a generated class.</summary>
internal sealed class EmittedField
{
	/// <summary>Field name, underscore-prefixed.</summary>
	public required string Name { get; init; }

	/// <summary>C# type as written.</summary>
	public required string CSharpTypeName { get; init; }

	/// <summary>Whether nothing ever reassigns it after construction.</summary>
	public required bool IsReadonly { get; init; }

	/// <summary>Initializer literal, when the field needs one.</summary>
	public string? InitializerLiteral { get; init; }
}

/// <summary>One piece of mirrored state, resolved into C# terms.</summary>
internal sealed class EmittedProperty
{
	/// <summary>Member name as three.js spells it, and the wire token.</summary>
	public required string ThreeName { get; init; }

	/// <summary>C# property name.</summary>
	public required string CSharpName { get; init; }

	/// <summary>Backing field name, shared with the constructor when three.js takes the same value there.</summary>
	public required string FieldName { get; init; }

	/// <summary>Name of the flag recording whether the caller has ever written this property.</summary>
	public required string WrittenFieldName { get; init; }

	/// <summary>C# type as written, including any nullable annotation.</summary>
	public required string CSharpTypeName { get; init; }

	/// <summary>The resolved type, with its basis.</summary>
	public required TypeMapping Mapping { get; init; }

	/// <summary>C# literal the field starts at, when the upstream documents an expressible default.</summary>
	public string? DefaultLiteral { get; init; }

	/// <summary>
	/// Whether this is a math value the object owns and watches, rather than a field it reassigns.
	/// three.js declares these mutable in place and the applier writes a decoded math value into the
	/// live instance, so the mirror holds one instance for the object's lifetime.
	/// </summary>
	public required bool IsOwnedMathValue { get; init; }

	/// <summary>Upstream JSDoc for the property.</summary>
	public IrDoc? Documentation { get; init; }

	/// <summary>Documented default text, kept verbatim so a math tuple default can be parsed from it.</summary>
	public string? DocumentedMathDefault { get; init; }
}

/// <summary>One command, resolved into C# terms.</summary>
internal sealed class EmittedCommand
{
	/// <summary>Method name as three.js spells it, and the wire token.</summary>
	public required string ThreeName { get; init; }

	/// <summary>C# method name.</summary>
	public required string CSharpName { get; init; }

	/// <summary>Parameters that reached the C# signature.</summary>
	public required IReadOnlyList<MappedParameter> Parameters { get; init; }

	/// <summary>Upstream JSDoc for the signature.</summary>
	public IrDoc? Documentation { get; init; }
}

/// <summary>A generated file and where it belongs in the repository.</summary>
internal sealed class EmittedFile
{
	/// <summary>Repository-relative POSIX path.</summary>
	public required string RelativePath { get; init; }

	/// <summary>File contents, LF-terminated.</summary>
	public required string Contents { get; init; }
}
