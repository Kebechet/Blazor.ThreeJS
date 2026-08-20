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
	/// <summary>
	/// Appended to a replay method's summary when at least one replayed property holds another mirrored
	/// object. Says why the replay attaches before it writes, which is otherwise the one line in the
	/// method a reader cannot derive from the surrounding code.
	/// </summary>
	private const string ReplayAttachmentSentence = " A replayed value that is itself a mirrored object is attached first, so its create op reaches the batch before the write that references it by handle.";

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
		WriteConstructor(writer, irClass, threeTypeName, baseTypeName, constructor, surface.Properties, _scope.BaseChainFor(irClass.Name));
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
			WriteCommand(writer, command, surface.Properties);
		}

		foreach (var query in surface.Queries)
		{
			writer.WriteLine();
			WriteQuery(writer, query);
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
	/// Emits the generated half of a class the runtime also hand-writes: a partial declaration carrying
	/// the commands and queries three.js declares on it, so the hand-written half can keep the
	/// behaviour without also owing the surface.
	/// <para>
	/// Only commands and queries. Mirrored state is the hand-written half's, because replaying it needs
	/// an <c>EmitState</c> override and that half already has one — two would write the same property
	/// twice. For the same reason nothing structural is emitted here: no constructor, no
	/// <c>ThreeTypeName</c>, no <c>ConstructorArgs</c>, and no base type, all of which the hand-written
	/// declaration supplies.
	/// </para>
	/// <para>
	/// The member set is whatever <see cref="MemberClassifier"/> yields, minus the names the
	/// hand-written half implements (<see cref="EmitterConfig.HandWrittenObject3DMemberNames"/>).
	/// Nothing is hand-picked, so a member three.js adds upstream arrives here on its own.
	/// </para>
	/// </summary>
	/// <param name="irClass">The hand-written class to supply a generated partial for.</param>
	/// <returns>The generated file.</returns>
	/// <exception cref="UnsupportedMemberException">
	/// Thrown when two emitted members would claim the same C# name. The normal path can record such a
	/// collision and carry on because the losing member is only absent from one generated class; here
	/// it would be absent from the base of the whole scene graph, which is worth failing the run over.
	/// </exception>
	public EmittedFile EmitHybridPartial(IrClass irClass)
	{
		var threeTypeName = ResolveThreeTypeName(irClass);
		var surface = ResolveHybridSurface(irClass, threeTypeName);

		var writer = new CSharpWriter();
		WriteFileHeader(writer);
		writer.WriteLine();

		// The core namespace is not imported unconditionally the way it is for a whole class: this half
		// declares no constructor and no replay, so it names a core type only when a signature does.
		if (UsesCoreTypes(surface))
		{
			writer.WriteLine($"using {EmitterConfig.CoreNamespace};");
		}

		if (UsesMathTypes([], surface))
		{
			writer.WriteLine($"using {EmitterConfig.MathNamespace};");
		}

		writer.WriteLine();
		writer.WriteLine($"namespace {EmitterConfig.GeneratedNamespace};");
		writer.WriteLine();

		WriteHybridPartialDocumentation(writer, threeTypeName);

		// No base type: the hand-written part declares it, and repeating it here would only be a second
		// place for it to be wrong. The modifiers do have to match that declaration exactly.
		writer.WriteLine($"public abstract partial class {threeTypeName}");
		writer.WriteLine("{");
		writer.Indent();

		var hasWrittenMember = false;
		foreach (var command in surface.Commands)
		{
			if (hasWrittenMember)
			{
				writer.WriteLine();
			}

			WriteCommand(writer, command, surface.Properties);
			hasWrittenMember = true;
		}

		foreach (var query in surface.Queries)
		{
			if (hasWrittenMember)
			{
				writer.WriteLine();
			}

			WriteQuery(writer, query);
			hasWrittenMember = true;
		}

		writer.Outdent();
		writer.WriteLine("}");

		return new EmittedFile
		{
			RelativePath = $"src/Blazor.ThreeJS/Generated/{threeTypeName}.cs",
			Contents = writer.ToSource()
		};
	}

	/// <summary>
	/// The commands and queries a hybrid partial emits: every classified member of those two buckets
	/// except the ones the hand-written half already declares.
	/// </summary>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="threeTypeName">Export name, which also claims its own C# name.</param>
	/// <returns>The emitted surface, with no properties in it.</returns>
	/// <exception cref="UnsupportedMemberException">Thrown when two members claim the same C# name.</exception>
	private EmittedSurface ResolveHybridSurface(IrClass irClass, string threeTypeName)
	{
		var takenCSharpNames = new HashSet<string>(StringComparer.Ordinal) { threeTypeName };
		var commands = new List<EmittedCommand>();
		var queries = new List<EmittedQuery>();

		var classified = _classifier.Classify(irClass).ToList();
		var cSharpNames = ResolveCSharpNames(classified);
		foreach (var member in classified)
		{
			if (member.Bucket is not (MemberBucket.Command or MemberBucket.AsyncQuery))
			{
				continue;
			}

			if (EmitterConfig.HandWrittenObject3DMemberNames.Contains(member.MemberName))
			{
				continue;
			}

			var cSharpName = cSharpNames[member.MemberName];

			if (!takenCSharpNames.Add(cSharpName))
			{
				throw UnsupportedMemberException.For(
					threeTypeName,
					$"its member `{member.MemberName}` wants the C# name `{cSharpName}`, which another member of the same partial already holds");
			}

			if (member.Bucket == MemberBucket.Command)
			{
				commands.Add(BuildCommand(member, cSharpName));
				continue;
			}

			queries.Add(BuildQuery(member, cSharpName));
		}

		return new EmittedSurface { Properties = [], Commands = commands, Queries = queries };
	}

	/// <summary>
	/// Writes the documentation block above a hybrid partial's declaration.
	/// <para>
	/// Parameterised by the type name, but not by the type. The warning below earns its place by naming
	/// which members go stale and what each one leaves stale, and the ones it names are
	/// <c>Object3D</c>'s. Deriving that list from the surface being emitted would reduce it to "a command
	/// leaves something unspecified stale", which is the generic warning a reader already skips — so the
	/// prose stays specific, and a second hybrid class fails the emit here rather than silently being
	/// documented with the first one's members.
	/// </para>
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="threeTypeName">Export name.</param>
	/// <exception cref="InvalidOperationException">
	/// Thrown when <see cref="EmitterConfig.HybridClassNames"/> holds more than one class.
	/// </exception>
	private static void WriteHybridPartialDocumentation(CSharpWriter writer, string threeTypeName)
	{
		if (EmitterConfig.HybridClassNames.Count > 1)
		{
			throw new InvalidOperationException(
				$"The prose below names `Object3D`'s members literally — `RotateX`, `Position`, `Attach`, `ClearAsync` and the rest — " +
				$"while this method is parameterised only by the type name. " +
				$"{nameof(EmitterConfig)}.{nameof(EmitterConfig.HybridClassNames)} now holds " +
				$"{string.Join(", ", EmitterConfig.HybridClassNames.Order(StringComparer.Ordinal))}, so that prose would be emitted onto a class it does not describe. " +
				$"Give this method a per-class body, or derive the named members from the surface being emitted, before adding a second hybrid class.");
		}

		DocCommentEmitter.WriteSummary(
			writer,
			$"The generated half of <c>{threeTypeName}</c>: the commands and queries <c>THREE.{threeTypeName}</c> declares, " +
			$"beside the hand-written half that owns the scene-graph behaviour. See the hand-written part for what this type is. " +
			$"<para>" +
			$"⚠️ <b>Every command here leaves the mirror stale, and so do the queries that mutate.</b> A command records a call and " +
			$"reads nothing back, so the state three.js changes on its side goes on being reported by C# as whatever it was " +
			$"before. One that writes the transform (<c>RotateX</c>, <c>TranslateOnAxis</c>, <c>ApplyMatrix4</c> and their kind) " +
			$"leaves <c>Position</c>, <c>Rotation</c>, <c>Scale</c> and <c>Quaternion</c> reporting their pre-call values, and " +
			$"writing one of those values back then records nothing at all, because the mirror sees the value it already holds. " +
			$"One that changes the scene graph (<c>Attach</c>, <c>Copy</c>) leaves <c>Children</c> reporting the parentage the " +
			$"mirror last arranged itself — and so do <c>RemoveFromParentAsync</c> and <c>ClearAsync</c>, which are queries only " +
			$"because three.js hands the changed object back: what they answer with is a handle, not a refreshed mirror. Both " +
			$"answer the receiver, which always exists, so the nullable <c>Task&lt;Object3D?&gt;</c> they declare never actually " +
			$"resolves null. Where a property or a hand-written method expresses what you want, use that; where you want the " +
			$"command, treat what it wrote as three.js's from then on." +
			$"</para>");
	}

	/// <summary>
	/// Whether an emitted surface names a type from the core namespace, and therefore needs it
	/// imported. Only the typed arrays and the mirror root live there; everything else a signature can
	/// name is a generated class in this file's own namespace or a hand-written math value.
	/// </summary>
	/// <param name="surface">The surface being emitted.</param>
	/// <returns><see langword="true"/> when the core namespace has to be imported.</returns>
	private static bool UsesCoreTypes(EmittedSurface surface)
	{
		// No property arm: a hybrid surface never carries one, because mirrored state stays with the
		// hand-written half.
		return surface.Commands.Any(command => command.Overloads.Any(overload => overload.Any(x => NamesCoreType(x.Mapping)))) ||
			surface.Queries.Any(query => query.ReturnTypeName == EmitterConfig.RootBaseTypeName ||
				EmitterConfig.TypedArrayTypeNames.Contains(query.ReturnTypeName) ||
				NamesCoreType(query.ReturnMapping) ||
				query.Overloads.Any(overload => overload.Any(x => NamesCoreType(x.Mapping))));
	}

	/// <summary>Whether a mapping puts a core-namespace type's name in the emitted source.</summary>
	/// <param name="mapping">The resolved type, absent where the member carries none.</param>
	/// <returns><see langword="true"/> when the core namespace has to be imported.</returns>
	private static bool NamesCoreType(TypeMapping? mapping)
	{
		if (mapping is null)
		{
			return false;
		}

		return mapping.Kind == TypeMappingKind.HandWrittenTypedArray || NamesCoreType(mapping.ElementMapping);
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
	/// Whether a mapping is an array whose elements are themselves mirrored objects. Such an array
	/// encodes to one handle reference per element, so every element needs attaching before the op that
	/// carries it — the same obligation a single-valued reference has, spread over the elements.
	/// <para>
	/// Kept apart from the single-valued test because the two attach differently: one is
	/// <c>_field?.AttachTo(batch)</c>, the other <c>AttachEach(batch, _field)</c>, and reading the
	/// array's own kind alone would answer <see cref="TypeMappingKind.Sequence"/> for an array of
	/// numbers just as readily.
	/// </para>
	/// </summary>
	/// <param name="mapping">The resolved type.</param>
	/// <returns><see langword="true"/> when this is an array of generated wrapper classes.</returns>
	private static bool IsSequenceOfMirroredObjects(TypeMapping mapping)
	{
		return mapping.Kind == TypeMappingKind.Sequence
			&& mapping.ElementMapping?.Kind == TypeMappingKind.GeneratedWrapperClass;
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
			throw UnsupportedMemberException.For(threeTypeName, "three.js's public barrel does not re-export it as a value, so it is not reachable on the THREE namespace the applier looks names up on");
		}

		if (!irClass.IsRuntimeExport)
		{
			throw UnsupportedMemberException.For(threeTypeName, "the shipped three.js bundle carries no such runtime value, so constructing it would throw Unknown three.js type");
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

		RecordDroppedAlternatives(audit, threeTypeName, "constructor", constructor.Parameters);
		return constructor;
	}

	/// <summary>
	/// Records the arms of a parameter's declared union that no emitted overload takes. An overload set
	/// is additive, so the arms that mapped need no note — but an arm that did not is part of the
	/// declared type the generated member does not accept, and this package narrows nothing without
	/// saying so.
	/// </summary>
	/// <param name="audit">Collector.</param>
	/// <param name="threeTypeName">Export name of the class being emitted.</param>
	/// <param name="memberDescription">The member the parameters belong to, e.g. <c>method set</c>.</param>
	/// <param name="parameters">Parameters of one signature; every overload carries the same arm record.</param>
	private static void RecordDroppedAlternatives(
		EmissionAudit audit,
		string threeTypeName,
		string memberDescription,
		IReadOnlyList<MappedParameter> parameters)
	{
		foreach (var parameter in parameters)
		{
			foreach (var dropped in parameter.DroppedAlternatives)
			{
				audit.RecordSkippedMember(
					threeTypeName,
					$"{memberDescription} parameter {parameter.ThreeName}, arm {dropped.TypeText}",
					$"one arm of `{parameter.DeclaredTypeText}` that no emitted overload takes: {dropped.Reason}");
			}
		}
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
		var queries = new List<EmittedQuery>();

		var classified = _classifier.Classify(irClass).ToList();
		var cSharpNames = ResolveCSharpNames(classified);
		foreach (var member in classified)
		{
			var kind = member.MemberKind == ClassifiedMemberKind.Property ? "property" : "method";
			if (member.Bucket == MemberBucket.Skipped)
			{
				audit.RecordSkippedMember(threeTypeName, $"{kind} {member.MemberName}", member.SkipReason!);
				continue;
			}

			var cSharpName = cSharpNames[member.MemberName];

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

			RecordDroppedAlternatives(audit, threeTypeName, $"{kind} {member.MemberName}", member.Method?.Overloads.FirstOrDefault() ?? []);

			if (member.Bucket == MemberBucket.Command)
			{
				commands.Add(BuildCommand(member, cSharpName));
				continue;
			}

			if (member.Bucket == MemberBucket.AsyncQuery)
			{
				queries.Add(BuildQuery(member, cSharpName));
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
				// A widened constructor slot is `object?`, and a property writing through it would have to
				// be `object?` too — a public member the caller gets no help from. The constructor keeps
				// the slot; the value is still writable through the escape hatch's `Set`.
				if (parameter.HasSeveralAlternatives)
				{
					audit.RecordSkippedMember(
						threeTypeName,
						$"property {member.MemberName}",
						$"the constructor takes it as `{parameter.DeclaredTypeText ?? "a union"}` and emits one overload per arm, so its backing field holds any of them — a property over that field could only be typed `object`");

					takenCSharpNames.Remove(cSharpName);
					continue;
				}

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
			var cSharpTypeName = ResolvePropertyTypeName(member.Mapping, isOwnedMathValue);
			var defaultLiteral = isOwnedMathValue
				? null
				: MethodMapper.RenderDefaultLiteral(ResolveDocumentedDefault(member), member.Mapping);

			properties.Add(BuildProperty(member, cSharpName, fieldName, cSharpTypeName, defaultLiteral, isOwnedMathValue));
		}

		return new EmittedSurface { Properties = properties, Commands = commands, Queries = queries };
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
			Overloads = member.Method!.Overloads,
			Documentation = member.Method.Signature?.Doc
		};
	}

	private static EmittedQuery BuildQuery(ClassifiedMember member, string cSharpName)
	{
		// A property read has no IR method behind it, so its parameters are empty by construction rather
		// than by mapping — the get op names a property and takes nothing.
		//
		// An untyped object result has no resolved C# type by definition, since the mapping is what
		// failed, so the wrapper it answers with is named here rather than carried from the map.
		if (member.IsPropertyRead)
		{
			return new EmittedQuery
			{
				ThreeName = member.MemberName,
				CSharpName = cSharpName,
				Overloads = [[]],
				ReturnTypeName = member.IsUntypedObjectResult ? EmitterConfig.UntypedObjectTypeName : member.CSharpTypeName!,
				ReturnMapping = member.Mapping,
				IsPropertyRead = true,
				IsAdoptedResult = member.IsAdoptedResult,
				IsUntypedObjectResult = member.IsUntypedObjectResult,
				IsAwaitedVoidResult = member.IsAwaitedVoidResult,
				Documentation = member.Property?.Doc
			};
		}

		return new EmittedQuery
		{
			ThreeName = member.MemberName,
			CSharpName = cSharpName,
			Overloads = member.Method!.Overloads,
			ReturnTypeName = member.IsUntypedObjectResult ? EmitterConfig.UntypedObjectTypeName : member.CSharpTypeName!,
			ReturnMapping = member.ReturnMapping,
			IsAdoptedResult = member.IsAdoptedResult,
			IsUntypedObjectResult = member.IsUntypedObjectResult,
			IsAwaitedVoidResult = member.IsAwaitedVoidResult,
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
	/// <summary>
	/// Names the C# type of a mirrored property.
	/// </summary>
	/// <param name="mapping">The resolved type of the three.js member.</param>
	/// <param name="isOwnedMathValue">
	/// Whether the property is a math value this object constructs and watches. Such a property is
	/// never null even where three.js declares it nullable (<c>BufferGeometry.boundingBox</c> is
	/// <c>Box3 | null</c> until something computes it), because the mirror has to hold an instance from
	/// the start to hang its change callback off. Annotating it nullable would both misstate the
	/// mirror's own invariant and emit <c>new Box3?()</c>, which does not compile.
	/// </param>
	/// <returns>The type name, with a nullable annotation where one belongs.</returns>
	private static string ResolvePropertyTypeName(TypeMapping mapping, bool isOwnedMathValue = false)
	{
		if (isOwnedMathValue)
		{
			return mapping.CSharpTypeName!;
		}

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
	/// <para>
	/// An array-typed field starts empty rather than null. Empty is the honest initial state — the
	/// mirror holds no elements until the caller supplies some — where null would be a second, useless
	/// way of saying the same thing that every consumer would have to test for.
	/// </para>
	/// </summary>
	/// <param name="cSharpTypeName">Field type as written.</param>
	/// <returns>The initializer literal, or <see langword="null"/> when none is needed.</returns>
	private static string? DefaultInitializer(string cSharpTypeName)
	{
		if (cSharpTypeName == "string")
		{
			return "string.Empty";
		}

		if (cSharpTypeName.EndsWith("[]", StringComparison.Ordinal))
		{
			return "[]";
		}

		// A typed array starts empty for the same reason an array does. Its constructor takes its
		// elements as a `params` list, so the no-argument form is the empty one.
		return EmitterConfig.TypedArrayTypeNames.Contains(cSharpTypeName)
			? $"new {cSharpTypeName}()"
			: null;
	}

	private bool UsesMathTypes(IReadOnlyList<MappedParameter> constructorParameters, EmittedSurface surface)
	{
		return constructorParameters.Any(x => x.Alternatives.Any(NamesMathType)) ||
			surface.Properties.Any(x => NamesMathType(x.Mapping)) ||
			surface.Commands.Any(command => command.Overloads.Any(overload => overload.Any(x => NamesMathType(x.Mapping)))) ||
			surface.Queries.Any(query => EmitterConfig.MathTypeNames.Contains(query.ReturnTypeName) ||
				NamesMathType(query.ReturnMapping) ||
				query.Overloads.Any(overload => overload.Any(x => NamesMathType(x.Mapping))));
	}

	/// <summary>
	/// Whether a mapping puts a math type's name in the emitted source, and therefore needs the math
	/// namespace imported. Recurses through arrays: <c>Vector3[]</c> spells <c>Vector3</c> just as
	/// plainly as <c>Vector3</c> does, and only the element mapping knows that.
	/// </summary>
	/// <param name="mapping">The resolved type, absent where the member carries none.</param>
	/// <returns><see langword="true"/> when the math namespace has to be imported.</returns>
	private static bool NamesMathType(TypeMapping? mapping)
	{
		if (mapping is null)
		{
			return false;
		}

		return mapping.Kind == TypeMappingKind.HandWrittenMathType || NamesMathType(mapping.ElementMapping);
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

	/// <summary>
	/// Writes one constructor per emitted overload, each with its own documentation and field
	/// assignments, then the adoption constructor.
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="irClass">Class being emitted.</param>
	/// <param name="threeTypeName">Export name.</param>
	/// <param name="baseTypeName">C# base, which decides which adoption constructor to chain to.</param>
	/// <param name="constructor">Resolved constructor, with its storage view and its overloads.</param>
	/// <param name="properties">Emitted properties, for wiring the owned math values.</param>
	private static void WriteConstructor(
		CSharpWriter writer,
		IrClass irClass,
		string threeTypeName,
		string baseTypeName,
		MappedConstructor constructor,
		IReadOnlyList<EmittedProperty> properties,
		IReadOnlyList<BaseChainArgument> baseChain)
	{
		var constructorSummary = irClass.Constructors.FirstOrDefault()?.Doc?.Summary is { Length: > 0 } rawSummary
			? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawSummary))
			: $"Initializes a new <see cref=\"{threeTypeName}\"/>.";

		foreach (var (index, parameters) in constructor.Overloads.Index())
		{
			if (index > 0)
			{
				writer.WriteLine();
			}

			DocCommentEmitter.WriteSummary(writer, constructorSummary + DescribeArmChoice(parameters));

			foreach (var parameter in parameters)
			{
				var text = parameter.Documentation is { Length: > 0 } documentation
					? DocCommentEmitter.RenderInline(DocCommentEmitter.StripRedundantTail(documentation))
					: $"Value forwarded to the <c>{parameter.ThreeName}</c> constructor argument.";

				DocCommentEmitter.WriteParam(writer, parameter.Name, text);
			}

			WriteDeclaration(writer, $"public {threeTypeName}", parameters);
			WriteBaseChain(writer, baseChain, parameters);

			writer.WriteLine("{");
			writer.Indent();
			foreach (var parameter in parameters)
			{
				writer.WriteLine($"{parameter.FieldName} = {parameter.DeclarationName};");
			}

			WriteOwnedMathValueSetup(writer, properties, hasPrecedingStatements: parameters.Count > 0);

			writer.Outdent();
			writer.WriteLine("}");
		}

		WriteAdoptionConstructor(writer, threeTypeName, baseTypeName, constructor.Parameters, properties);
	}

	/// <summary>
	/// The C# name each classified member takes, resolved for the whole class at once.
	/// <para>
	/// A query is renamed with an <c>Async</c> suffix, because it hands back a <c>Task</c> and a method
	/// that does so without saying it reads as a synchronous call. three.js has its own <c>*Async</c>
	/// methods though — the WebGPU renderer's are half of its API — and appending to those produces
	/// <c>ClearAsyncAsync</c>, which says the same thing twice. Such a member keeps three.js's own name.
	/// </para>
	/// <para>
	/// ⚠️ Unless another member of the same class would then have the same name. <c>hasFeature</c> and
	/// <c>hasFeatureAsync</c> are two real methods of <c>WebGPURenderer</c>, and both want
	/// <c>HasFeatureAsync</c>; the doubled form is ugly, but it is a name, whereas dropping one of two
	/// methods three.js offers is a missing feature. Resolved across the whole member list rather than
	/// as each one is reached, so which of the two yields does not depend on classification order.
	/// </para>
	/// </summary>
	/// <param name="members">Every classified member of the class.</param>
	/// <returns>The C# name to use, keyed by the three.js member name.</returns>
	private static Dictionary<string, string> ResolveCSharpNames(IReadOnlyList<ClassifiedMember> members)
	{
		var mechanical = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (var member in members)
		{
			mechanical[member.MemberName] = member.Bucket == MemberBucket.AsyncQuery
				? ToPascalCase(member.MemberName) + EmitterConfig.QueryMethodSuffix
				: ToPascalCase(member.MemberName);
		}

		var contested = mechanical.Values
			.Concat(members.Select(x => ToPascalCase(x.MemberName)))
			.GroupBy(x => x, StringComparer.Ordinal)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key)
			.ToHashSet(StringComparer.Ordinal);

		var resolved = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (var member in members)
		{
			var shortened = ToPascalCase(member.MemberName);
			resolved[member.MemberName] = member.Bucket == MemberBucket.AsyncQuery
				&& member.MemberName.EndsWith(EmitterConfig.QueryMethodSuffix, StringComparison.Ordinal)
				&& !contested.Contains(shortened)
					? shortened
					: mechanical[member.MemberName];
		}

		return resolved;
	}

	/// <summary>
	/// Writes the <c>: base(…)</c> clause, when this class shares constructor arguments with its
	/// generated base.
	/// <para>
	/// Without it the base half of the object holds nothing: <c>ArcCurve</c> takes an <c>aX</c>, stores
	/// it in a field of its own, and leaves <c>EllipseCurve.AX</c> — the property a caller reads it back
	/// through — reporting zero. The wire was always right, because <c>ConstructorArgs</c> is the
	/// subclass's own; it is the mirror that was not.
	/// </para>
	/// <para>
	/// Written as named arguments so a base parameter this class does not declare keeps its own default
	/// instead of being filled by whatever came next in the list.
	/// </para>
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="baseChain">Arguments to forward, in the base's parameter order.</param>
	/// <param name="parameters">Parameters of the overload being written.</param>
	private static void WriteBaseChain(
		CSharpWriter writer,
		IReadOnlyList<BaseChainArgument> baseChain,
		IReadOnlyList<MappedParameter> parameters)
	{
		// An overload's arms carry the same three.js names as the storage view the chain was computed
		// against, but not necessarily the same C# types — a widened arm cannot stand in for a base
		// parameter that wanted the narrow one. Chaining only what this overload actually declares keeps
		// the emitted call to the base honest per overload rather than per class.
		var declared = parameters
			.Select(x => x.DeclarationName)
			.ToHashSet(StringComparer.Ordinal);

		var arguments = baseChain
			.Where(x => declared.Contains(x.ArgumentName))
			.Select(x => $"{x.ParameterName}: {x.Expression}")
			.ToList();

		if (arguments.Count == 0)
		{
			return;
		}

		writer.Indent();
		writer.WriteLine($": base({string.Join(", ", arguments)})");
		writer.Outdent();
	}

	/// <summary>
	/// Writes a declaration and its parameter list, breaking onto one line per parameter when the
	/// single-line form runs past the column budget.
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="header">Everything before the opening parenthesis.</param>
	/// <param name="parameters">Parameters of this overload.</param>
	private static void WriteDeclaration(CSharpWriter writer, string header, IReadOnlyList<MappedParameter> parameters)
	{
		var declaredParameters = parameters
			.Select(x => x.DefaultLiteral is null
				? $"{x.CSharpTypeName} {x.DeclarationName}"
				: $"{x.CSharpTypeName} {x.DeclarationName} = {x.DefaultLiteral}")
			.ToList();

		var singleLine = $"{header}({string.Join(", ", declaredParameters)})";
		if (writer.IndentColumn + singleLine.Length <= EmitterConfig.DeclarationWrapColumn)
		{
			writer.WriteLine(singleLine);
			return;
		}

		writer.WriteLine($"{header}(");
		writer.Indent();
		foreach (var (index, declaredParameter) in declaredParameters.Index())
		{
			writer.WriteLine(index == declaredParameters.Count - 1
				? declaredParameter + ")"
				: declaredParameter + ",");
		}

		writer.Outdent();
	}

	/// <summary>
	/// Renders the arguments of a recorded call, leading comma included, or an empty string when the
	/// member takes none.
	/// <para>
	/// ⚠️ A lone array argument is cast to <c>object?</c>, and the cast is load-bearing rather than
	/// decorative. The record helpers take <c>params object?[]</c>, and array covariance makes
	/// <c>Vector2[]</c> convertible to <c>object?[]</c> — so <c>RecordCall("setFromPoints", points)</c>
	/// binds the array as the whole parameter array and three.js receives one argument per point
	/// instead of one array. The cast forces the expanded form, which is the call that was written.
	/// </para>
	/// </summary>
	/// <param name="parameters">Parameters of this overload.</param>
	/// <returns>The argument list to append to the helper call.</returns>
	private static string RenderRecordedArguments(IReadOnlyList<MappedParameter> parameters)
	{
		if (parameters.Count == 0)
		{
			return string.Empty;
		}

		if (parameters is [{ Mapping.Kind: TypeMappingKind.Sequence } sole])
		{
			return $", (object?) {sole.DeclarationName}";
		}

		return ", " + string.Join(", ", parameters.Select(x => x.DeclarationName));
	}

	/// <summary>
	/// The sentence that tells one overload of a union-armed member apart from its siblings. Without it
	/// several declarations carry the same upstream summary and nothing in the documentation says why
	/// there is more than one.
	/// </summary>
	/// <param name="parameters">Parameters of this overload.</param>
	/// <returns>The sentence, or an empty string when this member has only the one form.</returns>
	private static string DescribeArmChoice(IReadOnlyList<MappedParameter> parameters)
	{
		var armed = parameters
			.Where(x => x.HasSeveralAlternatives && x.DeclaredTypeText is { Length: > 0 })
			.Select(x => $"<c>{x.Name}</c> as <c>{DocCommentEmitter.RenderInline(x.CSharpTypeName)}</c> " +
				$"out of three.js's <c>{DocCommentEmitter.RenderInline(x.DeclaredTypeText!)}</c>")
			.ToList();

		if (armed.Count == 0)
		{
			return string.Empty;
		}

		return $" This overload takes {string.Join(", and ", armed)}.";
	}

	/// <summary>
	/// Writes the constructor that names an object the browser already made rather than building one.
	/// <para>
	/// The batch is taken here rather than assigned later so the object is attached from the moment it
	/// exists: <c>AttachTo</c> returns early on an already-attached object, which is what stops
	/// <c>EmitCreate</c> ever running for it. Creating it a second time is exactly the failure this
	/// prevents — the JavaScript object is already there, and a create op would replace it with a
	/// default-constructed one.
	/// </para>
	/// <para>
	/// Not public: a handle only means anything against the JavaScript object table it came from, so
	/// minting one is this assembly's business.
	/// </para>
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="threeTypeName">Class being emitted.</param>
	/// <param name="baseTypeName">C# base, which decides which adoption constructor to chain to.</param>
	/// <param name="parameters">Constructor parameters, whose backing fields adoption cannot fill.</param>
	/// <param name="properties">Emitted properties, for the owned math values that still need wiring.</param>
	private static void WriteAdoptionConstructor(
		CSharpWriter writer,
		string threeTypeName,
		string baseTypeName,
		IReadOnlyList<MappedParameter> parameters,
		IReadOnlyList<EmittedProperty> properties)
	{
		writer.WriteLine();
		DocCommentEmitter.WriteSummary(
			writer,
			$"Adopts an existing JavaScript-side <c>{threeTypeName}</c> under the handle the browser minted for it. " +
			$"No create op is emitted: the object already exists, and this mirror's job is to name it.");

		DocCommentEmitter.WriteParam(writer, "batch", "Batch this object's writes record into.");
		DocCommentEmitter.WriteParam(writer, "handle", "Negative handle the JavaScript side registered the object under.");

		// The hand-written bases take the handle alone and have no batch to be given; a generated base
		// carries this same two-argument constructor, so the chain forwards both.
		var isHandWrittenBase = baseTypeName == EmitterConfig.RootBaseTypeName ||
			EmitterConfig.HandWrittenClassNames.Contains(baseTypeName);

		writer.WriteLine($"internal {threeTypeName}(ThreeBatch batch, int handle)");
		writer.Indent();
		writer.WriteLine(isHandWrittenBase ? ": base(handle)" : ": base(batch, handle)");
		writer.Outdent();
		writer.WriteLine("{");
		writer.Indent();

		// A constructor argument three.js requires is unknown to an adopted mirror: the browser built
		// the object, so the real value is on that side and was never sent here. The field is written
		// as unknown rather than left unassigned, because leaving it would warn (CS8618) about a
		// non-nullable field that the public constructor does fill.
		var unknownFields = parameters
			.Where(x => !x.CSharpTypeName.EndsWith("?", StringComparison.Ordinal))
			.ToList();

		foreach (var parameter in unknownFields)
		{
			writer.WriteLine($"{parameter.FieldName} = default!;");
		}

		WriteOwnedMathValueSetup(writer, properties, hasPrecedingStatements: unknownFields.Any());

		if (properties.Any(x => x.IsOwnedMathValue) || unknownFields.Any())
		{
			writer.WriteLine();
		}

		writer.WriteLine("Batch = batch;");
		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>Writes the owned math values' construction and change hooks, shared by both constructors.</summary>
	/// <param name="writer">Destination.</param>
	/// <param name="properties">Emitted properties.</param>
	/// <param name="hasPrecedingStatements">Whether a blank line is needed before the first block.</param>
	private static void WriteOwnedMathValueSetup(CSharpWriter writer, IReadOnlyList<EmittedProperty> properties, bool hasPrecedingStatements)
	{
		foreach (var (index, property) in properties.Where(x => x.IsOwnedMathValue).Index())
		{
			if (index > 0 || hasPrecedingStatements)
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
		else if (IsSequenceOfMirroredObjects(property.Mapping))
		{
			// AttachEach tolerates both nulls, so the guard the single-valued arm needs is inside it
			// rather than around the call.
			writer.WriteLine("AttachEach(Batch, value);");
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

	private static void WriteCommand(CSharpWriter writer, EmittedCommand command, IReadOnlyList<EmittedProperty> properties)
	{
		var summary = command.Documentation?.Summary is { Length: > 0 } rawSummary
			? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawSummary))
			: $"Records a call to <c>{command.ThreeName}</c> on the JavaScript-side object.";

		if (FindShadowedProperty(command, properties) is { } shadowedProperty)
		{
			summary += $" This writes the same three.js state as <see cref=\"{shadowedProperty.CSharpName}\"/> " +
				$"and the mirror does not learn from it: afterwards <c>{shadowedProperty.CSharpName}</c> still reports its previous value, " +
				$"and writing that value back records nothing at all. Where the property exists, write the property.";
		}

		foreach (var (index, parameters) in command.Overloads.Index())
		{
			if (index > 0)
			{
				writer.WriteLine();
			}

			DocCommentEmitter.WriteSummary(writer, summary + DescribeArmChoice(parameters));
			foreach (var parameter in parameters)
			{
				var text = parameter.Documentation is { Length: > 0 } documentation
					? DocCommentEmitter.RenderInline(DocCommentEmitter.StripRedundantTail(documentation))
					: $"Value forwarded to the <c>{parameter.ThreeName}</c> argument.";

				DocCommentEmitter.WriteParam(writer, parameter.Name, text);
			}

			WriteDeclaration(writer, $"public void {command.CSharpName}", parameters);

			writer.WriteLine("{");
			writer.Indent();

			// An argument that is itself a mirrored object has to exist on the JavaScript side before the
			// call that references it by handle, and RecordCall attaches it. Emitting that here instead
			// would only cover the case where this object already has a batch: a command invoked before
			// the attach is held and replayed later, and nothing at this call site can attach anything
			// then. One owner for the invariant is what keeps the two paths from drifting.
			writer.WriteLine($"RecordCall(\"{command.ThreeName}\"{RenderRecordedArguments(parameters)});");
			writer.Outdent();
			writer.WriteLine("}");
		}
	}

	/// <summary>
	/// The mirrored property a command writes behind the mirror's back, when there is one. three.js
	/// spells such a pair <c>setX</c> / <c>x</c>, and the command records a call while the property's
	/// backing field keeps the value it had — which the property's own write-elision then turns into a
	/// silently dropped write. Same class only: a pair split across a base and its subclass is documented
	/// on the base, where both members are declared together.
	/// </summary>
	/// <param name="command">The command being emitted.</param>
	/// <param name="properties">The mirrored properties this class emits.</param>
	/// <returns>The shadowed property, or <see langword="null"/> when the command shadows nothing.</returns>
	private static EmittedProperty? FindShadowedProperty(EmittedCommand command, IReadOnlyList<EmittedProperty> properties)
	{
		const string setterPrefix = "set";
		if (!command.ThreeName.StartsWith(setterPrefix, StringComparison.Ordinal) || command.ThreeName.Length <= setterPrefix.Length)
		{
			return null;
		}

		var shadowedName = char.ToLowerInvariant(command.ThreeName[setterPrefix.Length]) + command.ThreeName[(setterPrefix.Length + 1)..];
		return properties.FirstOrDefault(x => string.Equals(x.ThreeName, shadowedName, StringComparison.Ordinal));
	}

	/// <summary>
	/// Writes one query: a method that records a read op and awaits the value three.js sends back.
	/// <para>
	/// The C# name carries an <c>Async</c> suffix that three.js's own name does not, which is the one
	/// place the mirror renames a member. It has to: the return type is a <c>Task&lt;T&gt;</c>, and a
	/// method that hands back a task without saying so reads as a synchronous call at every call site.
	/// </para>
	/// </summary>
	/// <param name="writer">Destination.</param>
	/// <param name="query">The query being emitted.</param>
	private static void WriteQuery(CSharpWriter writer, EmittedQuery query)
	{
		var summary = query.Documentation?.Summary is { Length: > 0 } rawSummary
			? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawSummary))
			: $"Reads <c>{query.ThreeName}</c> back from the JavaScript-side object.";

		if (query.IsUntypedObjectResult)
		{
			// The one query shape whose result the mirror cannot describe. Saying so in the summary is the
			// point: the caller gets a working object and no compiler help with its members.
			summary += query.IsPropertyRead
				? $" Holds a three.js object no generated class mirrors: records a get op, sends it behind every write already pending, and completes with whatever <c>{query.ThreeName}</c> held, under its own handle, as an untyped <see cref=\"{EmitterConfig.UntypedObjectTypeName}\"/>."
				: $" Answers with a three.js object no generated class mirrors: records a read op, sends it behind every write already pending, and completes with what <c>{query.ThreeName}</c> returned, under its own handle, as an untyped <see cref=\"{EmitterConfig.UntypedObjectTypeName}\"/>.";

			summary += " The mirror learns nothing from it — its members are reached by their three.js names, and nothing here checks them.";
		}
		else if (query.IsAwaitedVoidResult)
		{
			summary += $" Answers nothing, and is awaited for when rather than for what: records a read op, sends it behind every write already pending, and completes once the promise <c>{query.ThreeName}</c> returned has settled.";
		}
		else
		{
			summary += query.IsPropertyRead
				? $" Read-only in three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every write already pending, and completes with the value <c>{query.ThreeName}</c> held."
				: $" Records a read op, sends it behind every write already pending, and completes with what <c>{query.ThreeName}</c> returned.";
		}

		// An object result is nullable: three.js answers with nothing when the member held no object,
		// and a non-nullable signature would make that indistinguishable from an object at handle zero.
		var returnTypeName = query switch
		{
			{ IsAwaitedVoidResult: true } => "Task",
			{ IsAdoptedResult: true } or { IsUntypedObjectResult: true } => $"Task<{query.ReturnTypeName}?>",
			_ => $"Task<{query.ReturnTypeName}>"
		};

		foreach (var (index, parameters) in query.Overloads.Index())
		{
			if (index > 0)
			{
				writer.WriteLine();
			}

			DocCommentEmitter.WriteSummary(writer, summary + DescribeArmChoice(parameters));
			foreach (var parameter in parameters)
			{
				var text = parameter.Documentation is { Length: > 0 } documentation
					? DocCommentEmitter.RenderInline(DocCommentEmitter.StripRedundantTail(documentation))
					: $"Value forwarded to the <c>{parameter.ThreeName}</c> argument.";

				DocCommentEmitter.WriteParam(writer, parameter.Name, text);
			}

			if (query.IsUntypedObjectResult)
			{
				DocCommentEmitter.WriteReturns(writer, query.IsPropertyRead
					? $"The object <c>{query.ThreeName}</c> held, under its own handle, or <see langword=\"null\"/> when it held none."
					: $"The object <c>{query.ThreeName}</c> returned, under its own handle, or <see langword=\"null\"/> when it returned none.");
			}
			else if (query.IsAwaitedVoidResult)
			{
				DocCommentEmitter.WriteReturns(writer, $"A task that completes once <c>{query.ThreeName}</c> has finished.");
			}
			else
			{
				DocCommentEmitter.WriteReturns(writer, query.IsPropertyRead
					? $"The value <c>{query.ThreeName}</c> held, once the JavaScript side has answered."
					: $"The value <c>{query.ThreeName}</c> returned, once the JavaScript side has answered.");
			}

			WriteDeclaration(writer, $"public {returnTypeName} {query.CSharpName}", parameters);

			writer.WriteLine("{");
			writer.Indent();

			var arguments = RenderRecordedArguments(parameters);
			if (query.IsUntypedObjectResult)
			{
				// No adopter lambda, because there is no type to adopt into: the escape hatch's own helpers
				// answer with the wrapper that names three.js's runtime type instead of asserting a C# one.
				writer.WriteLine(query.IsPropertyRead
					? $"return GetObjectAsync(\"{query.ThreeName}\");"
					: $"return CallObjectAsync(\"{query.ThreeName}\"{arguments});");
			}
			else if (query.IsAdoptedResult)
			{
				// The lambda is what supplies the concrete type: the helper resolves a handle this context
				// already mirrors and only calls this for one it has never seen.
				var adopt = EmitterConfig.AdoptionSubstituteTypeNames.TryGetValue(query.ReturnTypeName, out var substitute)
					? $"(adoptedBatch, adoptedHandle) => new {substitute}(adoptedBatch, adoptedHandle, \"{query.ReturnTypeName}\")"
					: $"(adoptedBatch, adoptedHandle) => new {query.ReturnTypeName}(adoptedBatch, adoptedHandle)";
				// The type argument is spelled out rather than inferred: where the adopter is a substitute
				// for an abstract declared type, inference would take the substitute and produce a
				// `Task<PrimitiveObject3D?>` that does not convert to the declared `Task<Object3D?>`.
				writer.WriteLine(query.IsPropertyRead
					? $"return RecordGetObject<{query.ReturnTypeName}>(\"{query.ThreeName}\", {adopt});"
					: $"return RecordReadObject<{query.ReturnTypeName}>(\"{query.ThreeName}\", {adopt}{arguments});");
			}
			else if (query.IsAwaitedVoidResult)
			{
				// Read through the value channel and the value discarded, because the answer is the
				// completion rather than anything in it. `Task<object?>` returns as the bare `Task` the
				// signature declares, so no second helper is needed to say "nothing came back".
				writer.WriteLine($"return RecordRead<object?>(\"{query.ThreeName}\"{arguments});");
			}
			else if (query.IsPropertyRead)
			{
				writer.WriteLine($"return GetAsync<{query.ReturnTypeName}>(\"{query.ThreeName}\");");
			}
			else
			{
				// RecordRead owns attaching any argument that is itself a mirrored object, for the same reason
				// RecordCall does: one owner for the invariant is what keeps the two paths from drifting.
				writer.WriteLine($"return RecordRead<{query.ReturnTypeName}>(\"{query.ThreeName}\"{arguments});");
			}

			writer.Outdent();
			writer.WriteLine("}");
		}
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
			.Where(x => x.Alternatives.Any(alternative =>
				alternative.Kind == TypeMappingKind.GeneratedWrapperClass
				|| IsSequenceOfMirroredObjects(alternative)))
			.ToList();

		var isSceneGraphType = IsSceneGraphType(irClass);
		var hasReplay = properties.Count > 0;
		var hasReplayedReference = properties.Any(x =>
			x.Mapping.Kind == TypeMappingKind.GeneratedWrapperClass
			|| IsSequenceOfMirroredObjects(x.Mapping));

		if (dependencies.Count > 0 || (hasReplay && !isSceneGraphType))
		{
			var createSummary = dependencies.Count > 0
				? $"Attaches the objects <c>THREE.{threeTypeName}</c> is constructed from, so their create ops reach the batch before the one that references them by handle, then emits this object's own."
				: $"Emits the create op for <c>THREE.{threeTypeName}</c>, then replays every property written before this object was attached.";

			if (hasReplay && !isSceneGraphType && hasReplayedReference)
			{
				createSummary += ReplayAttachmentSentence;
			}

			writer.WriteLine();
			DocCommentEmitter.WriteSummary(writer, createSummary);
			DocCommentEmitter.WriteParam(writer, "batch", "Batch to record the ops into.");
			writer.WriteLine("internal override void EmitCreate(ThreeBatch batch)");
			writer.WriteLine("{");
			writer.Indent();
			foreach (var dependency in dependencies)
			{
				// A widened slot holds whichever arm the caller's overload took, so the attach has to test
				// what is actually in it. Only the arms that are mirrored objects need attaching, and only
				// those answer the type test.
				if (dependency.HasSeveralAlternatives)
				{
					writer.WriteLine($"({dependency.FieldName} as {EmitterConfig.RootBaseTypeName})?.AttachTo(batch);");
					continue;
				}

				if (IsSequenceOfMirroredObjects(dependency.Mapping))
				{
					writer.WriteLine($"AttachEach(batch, {dependency.FieldName});");
					continue;
				}

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

		var stateSummary = "Replays every property written before this object was attached, so construction order never matters to the caller. A property the caller never wrote is left alone: three.js's own default is the truth for it, and the mirror has never read anything back to improve on that.";
		if (hasReplayedReference)
		{
			stateSummary += ReplayAttachmentSentence;
		}

		writer.WriteLine();
		DocCommentEmitter.WriteSummary(writer, stateSummary);
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

			// A replayed value that is itself a mirrored object travels as a handle reference, and the
			// write being replayed is one the caller made before this object had a batch — so its setter
			// had nothing to attach the value to. Attaching here rather than emitting the create op
			// directly is what keeps a shared instance from being created twice.
			if (property.Mapping.Kind == TypeMappingKind.GeneratedWrapperClass)
			{
				writer.WriteLine(property.CSharpTypeName.EndsWith('?')
					? $"{value}?.AttachTo(batch);"
					: $"{value}.AttachTo(batch);");
			}
			else if (IsSequenceOfMirroredObjects(property.Mapping))
			{
				writer.WriteLine($"AttachEach(batch, {value});");
			}

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

	/// <summary>Queries — the methods whose result is read back — in resolution order.</summary>
	public required IReadOnlyList<EmittedQuery> Queries { get; init; }
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

	/// <summary>One parameter list per emitted overload.</summary>
	public required IReadOnlyList<IReadOnlyList<MappedParameter>> Overloads { get; init; }

	/// <summary>Upstream JSDoc for the signature.</summary>
	public IrDoc? Documentation { get; init; }
}

/// <summary>One query — a method whose return value is read back — resolved into C# terms.</summary>
internal sealed class EmittedQuery
{
	/// <summary>Method name as three.js spells it, and the wire token.</summary>
	public required string ThreeName { get; init; }

	/// <summary>C# method name, which carries the <c>Async</c> suffix the returned task calls for.</summary>
	public required string CSharpName { get; init; }

	/// <summary>One parameter list per emitted overload.</summary>
	public required IReadOnlyList<IReadOnlyList<MappedParameter>> Overloads { get; init; }

	/// <summary>C# type of the value read back, without the surrounding task.</summary>
	public required string ReturnTypeName { get; init; }

	/// <summary>How that type resolved, which is what knows whether it names a math type through an array.</summary>
	public TypeMapping? ReturnMapping { get; init; }

	/// <summary>
	/// True when this reads a read-only property through the get op rather than invoking a method
	/// through the read op.
	/// </summary>
	public bool IsPropertyRead { get; init; }

	/// <summary>True when the result is a three.js object adopted by handle rather than a value.</summary>
	public bool IsAdoptedResult { get; init; }

	/// <summary>
	/// True when the result is a three.js object no generated class mirrors, so it comes back by handle
	/// as an untyped <c>Primitive</c> rather than as the declared type.
	/// </summary>
	public bool IsUntypedObjectResult { get; init; }

	/// <summary>Whether the query answers nothing and is emitted as a bare <c>Task</c>.</summary>
	public bool IsAwaitedVoidResult { get; init; }

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
