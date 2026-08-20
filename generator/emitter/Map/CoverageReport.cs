using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Blazor.ThreeJS.Emitter.Emit;
using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// Renders the type-mapping and member-classification results as committed generator output: a
/// markdown report for a human, and a JSON document the README's coverage table can be generated
/// from. Both are produced from the same in-memory model, so the prose and the numbers cannot drift.
/// </summary>
internal sealed class CoverageReport
{
	private readonly IrRoot _ir;
	private readonly EmissionScope _scope;
	private readonly EnumCatalog _enums;
	private readonly TypeMapper _mapper;
	private readonly List<ClassifiedMember> _members;

	/// <summary>
	/// A public member of a generated class: a <c>public</c> declaration at class-body indentation.
	/// Counted from the emitted text rather than from the model so the README's figure is the one a
	/// reader reproduces with <c>grep -c "^\tpublic " src/Blazor.ThreeJS/Generated/*.cs</c>.
	/// </summary>
	private static readonly Regex _publicMemberPattern = new(@"^\tpublic ", RegexOptions.Compiled | RegexOptions.Multiline);

	/// <summary>An enum member: a name and its literal at enum-body indentation.</summary>
	private static readonly Regex _enumMemberPattern = new(@"^\t[A-Za-z_][A-Za-z0-9_]* = ", RegexOptions.Compiled | RegexOptions.Multiline);

	private static readonly Regex _enumFilePattern = new(@"^public enum ", RegexOptions.Compiled | RegexOptions.Multiline);

	private static readonly JsonSerializerOptions _coverageJsonOptions = new()
	{
		WriteIndented = true,
		IndentSize = 2,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	/// <summary>The IR this report was built from, so a caller can reuse its provenance.</summary>
	public IrRoot Ir
	{
		get { return _ir; }
	}

	/// <summary>Every class the emitter could produce today, ordered by name.</summary>
	public IReadOnlyList<ClassScopeResult> EmittableClasses
	{
		get
		{
			return _scope.Results
				.Where(x => x.Status == ClassScopeStatus.Emittable)
				.ToList();
		}
	}

	/// <summary>Builds a report from a completed mapping run.</summary>
	/// <param name="ir">The parsed IR.</param>
	/// <param name="scope">Emission scope, already resolved to a fixpoint.</param>
	/// <param name="enums">Catalog of generatable enums.</param>
	/// <param name="mapper">Type mapper, read for which enums are actually referenced.</param>
	/// <param name="classifier">Member classifier.</param>
	public CoverageReport(IrRoot ir, EmissionScope scope, EnumCatalog enums, TypeMapper mapper, MemberClassifier classifier)
	{
		_ir = ir;
		_scope = scope;
		_enums = enums;
		_mapper = mapper;
		_members = [];

		// One declaration per class name, where every class figure in this document counts one per
		// declaration. Four names are declared in two files each, and the machinery behind a member row is
		// keyed by name: `ClassSurfaceResolver` caches a resolved surface under the name, and
		// `EmissionScope` lets at most one declaration of a name reach C#. So classifying the second
		// declaration re-reports the first one's surface verbatim — the same members, on the same class
		// name, counted twice. A class row is about a declaration and there are two of those; a member row
		// is about a surface and there is one.
		foreach (var irClass in scope.Results.Select(x => x.Class).DistinctBy(x => x.Name, StringComparer.Ordinal))
		{
			_members.AddRange(classifier.Classify(irClass));
		}
	}

	/// <summary>Renders the human-readable report.</summary>
	/// <returns>Markdown, LF-terminated.</returns>
	public string RenderMarkdown()
	{
		var builder = new StringBuilder();
		AppendHeader(builder);
		AppendMappingRules(builder);
		AppendSurfaceResolution(builder);
		AppendReplayPolicy(builder);
		AppendClassCoverage(builder);
		AppendMemberClassification(builder);
		AppendSkipList(builder);
		AppendEnums(builder);
		AppendHazards(builder);
		return builder.ToString();
	}

	/// <summary>
	/// Renders the README's coverage section — the claim a consumer reads on nuget.org before
	/// installing. Generated rather than written by hand for one reason: a hand-maintained coverage
	/// claim drifts towards flattery, and this one has to survive being checked. Every figure here comes
	/// from the same model as <c>api-coverage.md</c>, and the section ends with how to reproduce each.
	/// </summary>
	/// <param name="emittedFiles">Everything this run produces, read for the shipped member counts.</param>
	/// <returns>Markdown, LF-terminated, to splice between the README's coverage markers.</returns>
	public string RenderReadmeSection(IReadOnlyList<EmittedFile> emittedFiles)
	{
		var builder = new StringBuilder();
		AppendReadmeHeadline(builder, emittedFiles);
		AppendReadmeClassTable(builder);
		AppendReadmeExclusions(builder);
		AppendReadmeReadChannel(builder);
		AppendBlockedClassWorkarounds(builder);
		AppendReadmeNarrowings(builder);
		AppendReadmeEscapeHatch(builder);
		AppendReadmeMeasurement(builder);
		return builder.ToString();
	}

	/// <summary>Renders the machine-readable report the coverage table is generated from.</summary>
	/// <returns>JSON, LF-terminated, 2-space indented.</returns>
	public string RenderJson()
	{
		var classes = _scope.Results
			.Select(x => new CoverageClassJson
			{
				Name = x.Class.Name,
				File = x.Class.File,
				Status = DescribeStatus(x.Status),
				IsReachable = x.Class.IsRuntimeExport,
				Reason = x.Reason,
				Category = x.Category == SkipCategory.None ? null : x.Category.ToString(),
				ConstructorParameterCount = x.Constructor?.Parameters.Count ?? 0,
				DroppedConstructorParameters = x.Constructor?.DroppedParameters
					.Select(dropped => new CoverageDroppedParameterJson
					{
						Name = dropped.Name,
						Type = dropped.TypeText,
						Reason = dropped.Reason,
						Category = dropped.Category.ToString()
					})
					.ToList() ?? [],
				MiddlePositionUnspecifiedParameters = x.Constructor?.MiddlePositionUnspecifiedParameters.ToList() ?? [],
				DroppedConstructorUnionArms = RenderDroppedArms(x.Constructor?.Parameters ?? [])
			})
			.ToList();

		var document = new CoverageJson
		{
			TypesPackage = _ir.Meta?.TypesPackage ?? "@types/three",
			TypesVersion = _ir.Meta?.TypesVersion ?? "unknown",
			Totals = new CoverageTotalsJson
			{
				Classes = _scope.Results.Count,
				EmittableClasses = _scope.Results.Count(x => x.Status == ClassScopeStatus.Emittable),
				OutOfSurfaceClasses = _scope.Results.Count(x => x.Status == ClassScopeStatus.OutOfSurface),
				BlockedClasses = _scope.Results.Count(x => x.Status == ClassScopeStatus.Blocked),
				ReachableClasses = ReachableClassCount(),
				Members = _members.Count,
				MirroredState = CountBucket(MemberBucket.MirroredState),
				Commands = CountBucket(MemberBucket.Command),
				AsyncQueries = CountBucket(MemberBucket.AsyncQuery),
				UntypedObjectQueries = _members.Count(x => x.IsUntypedObjectResult),
				SkippedMembers = CountBucket(MemberBucket.Skipped),
				ReachableMembers = EmittableMembers().Count(),
				GeneratedMembers = EmittableMembers().Count(x => x.Bucket != MemberBucket.Skipped),
				StrandedMembers = _members.Count - EmittableMembers().Count()
			},
			Classes = classes,
			MemberSkipReasons = SkipCountsByCategory()
				.Select(x => new CoverageSkipCategoryJson { Category = x.Key.ToString(), Members = x.Value })
				.ToList(),
			GeneratedEnums = _enums.Generatable
				.Select(x => new CoverageEnumJson
				{
					Name = x.Name,
					Source = x.Source.ToString(),
					Members = x.Members.Count,
					BackingType = x.BackingTypeName,
					IsReferenced = _mapper.RequiredEnumNames.Contains(x.Name)
				})
				.ToList(),
			RefusedEnums = _enums.Refusals
				.Select(x => new CoverageRefusedEnumJson { Name = x.Key, Reason = x.Value })
				.ToList(),
			Members = _members
				.Select(x => new CoverageMemberJson
				{
					Class = x.ClassName,
					Member = x.MemberName,
					Kind = x.MemberKind.ToString(),
					Origin = x.Origin.ToString(),
					Bucket = x.Bucket.ToString(),
					Type = x.CSharpTypeName,
					IsUntypedObject = x.IsUntypedObjectResult ? true : null,
					Reason = x.SkipReason,
					Category = x.SkipCategory == SkipCategory.None ? null : x.SkipCategory.ToString(),
					DroppedUnionArms = RenderDroppedArms(x.Method?.Overloads.FirstOrDefault() ?? [])
				})
				.ToList()
		};

		var json = JsonSerializer.Serialize(document, _coverageJsonOptions);
		return json.ReplaceLineEndings("\n") + "\n";
	}

	/// <summary>
	/// The dropped-arm rows for one parameter list, or <see langword="null"/> when it lost none — the
	/// JSON omits the key rather than writing an empty list on every one of the thousands of rows that
	/// never had a union.
	/// </summary>
	/// <param name="parameters">Parameters of one signature.</param>
	/// <returns>The rows, or <see langword="null"/>.</returns>
	private static List<CoverageDroppedArmJson>? RenderDroppedArms(IReadOnlyList<MappedParameter> parameters)
	{
		var rows = parameters
			.SelectMany(parameter => parameter.DroppedAlternatives.Select(dropped => new CoverageDroppedArmJson
			{
				Parameter = parameter.ThreeName,
				DeclaredType = parameter.DeclaredTypeText ?? "<none>",
				Arm = dropped.TypeText,
				Reason = dropped.Reason,
				Category = dropped.Category.ToString()
			}))
			.ToList();

		return rows.Count == 0 ? null : rows;
	}

	private void AppendReadmeHeadline(StringBuilder builder, IReadOnlyList<EmittedFile> emittedFiles)
	{
		var generatedFiles = emittedFiles
			.Where(x => x.RelativePath.EndsWith(".cs", StringComparison.Ordinal))
			.ToList();

		var classFiles = generatedFiles
			.Where(x => !_enumFilePattern.IsMatch(x.Contents))
			.ToList();

		// The hybrid partials are counted apart from the class total rather than folded into it. Their
		// members are real and grep finds them in `Generated/`, but the type they land on is not one of
		// the classes the headline claims — attributing them to it would be the one figure in this
		// section that overstates.
		var hybridPaths = EmitterConfig.HybridClassNames
			.Select(x => $"src/Blazor.ThreeJS/Generated/{x}.cs")
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		var hybridMemberCount = classFiles
			.Where(x => hybridPaths.Contains(x.RelativePath))
			.Sum(x => _publicMemberPattern.Matches(x.Contents).Count);

		var publicMemberCount = classFiles
			.Where(x => !hybridPaths.Contains(x.RelativePath))
			.Sum(x => _publicMemberPattern.Matches(x.Contents).Count);

		var enumMemberCount = generatedFiles
			.Where(x => _enumFilePattern.IsMatch(x.Contents))
			.Sum(x => _enumMemberPattern.Matches(x.Contents).Count);

		var emittableCount = _scope.Results.Count(x => x.Status == ClassScopeStatus.Emittable);
		AppendLine(builder, $"Generated from `{_ir.Meta?.TypesPackage}@{_ir.Meta?.TypesVersion}`: **{emittableCount} of three.js's {_scope.Results.Count} core classes** and");
		AppendLine(builder, $"**{_enums.Generatable.Count}** of its constant groups, carrying {publicMemberCount} public members and {enumMemberCount} enum members. Property names,");
		AppendLine(builder, "constructor argument order and documentation are three.js's own rather than a paraphrase - and so is");
		AppendLine(builder, "everything below, which is what that same generator run measured about itself.");
		AppendLine(builder);
		if (hybridMemberCount > 0)
		{
			var hybridNames = string.Join(", ", EmitterConfig.HybridClassNames.Order(StringComparer.Ordinal).Select(x => $"`{x}`"));
			AppendLine(builder, $"A further {hybridMemberCount} public members are generated onto {hybridNames}, which is hand-written and so is **not**");
			AppendLine(builder, $"one of those {emittableCount} classes - its command and query surface is emitted as the other half of a");
			AppendLine(builder, $"`partial class`. So `Generated/**` carries {publicMemberCount + hybridMemberCount} public members in all, and this headline claims");
			AppendLine(builder, "only the ones that sit on a class the generator made.");
			AppendLine(builder);
		}

		AppendLine(builder, $"A further {ReachableClassCount() - emittableCount} classes have no generated type and are still **reachable** untyped, by name -");
		AppendLine(builder, "[how](#reaching-what-is-not-generated). What is out of reach is what three.js does not export at all.");
		AppendLine(builder);
	}

	private void AppendReadmeClassTable(StringBuilder builder)
	{
		var byStatus = _scope.Results
			.GroupBy(x => x.Status)
			.ToDictionary(x => x.Key, x => x.Count());

		AppendLine(builder, "### Classes");
		AppendLine(builder);
		AppendLine(builder, "| | classes |");
		AppendLine(builder, "|---|---|");
		AppendLine(builder, $"| **generated** | **{byStatus.GetValueOrDefault(ClassScopeStatus.Emittable)}** |");
		AppendLine(builder, $"| blocked on something the mirror cannot express yet | {byStatus.GetValueOrDefault(ClassScopeStatus.Blocked)} |");
		AppendLine(builder, $"| deliberately out of the mirrored surface | {byStatus.GetValueOrDefault(ClassScopeStatus.OutOfSurface)} |");
		AppendLine(builder, $"| **three.js core total** | **{_scope.Results.Count}** |");
		AppendLine(builder);
		AppendLine(builder, "The blocked ones, by what blocks them:");
		AppendLine(builder);
		AppendLine(builder, "| obstacle | classes |");
		AppendLine(builder, "|---|---|");
		var blockedByCategory = _scope.Results
			.Where(x => x.Status == ClassScopeStatus.Blocked)
			.GroupBy(x => x.Category)
			.Select(x => new { Category = x.Key, Count = x.Count() })
			.OrderByDescending(x => x.Count)
			.ThenBy(x => x.Category.ToString(), StringComparer.Ordinal);

		foreach (var group in blockedByCategory)
		{
			AppendLine(builder, $"| {DescribeCategory(group.Category)} | {group.Count} |");
		}

		AppendLine(builder);
	}

	/// <summary>
	/// The two exclusions a consumer is most likely to be caught out by — the addons and the node stack
	/// are not in the class total at all, so a percentage computed from that total would overstate the
	/// coverage. Stated as its own table for exactly that reason.
	/// </summary>
	private void AppendReadmeExclusions(StringBuilder builder)
	{
		AppendLine(builder, "### Not wrapped, and not counted above");
		AppendLine(builder);
		AppendLine(builder, "These are outside the class total, because the extractor never reads them:");
		AppendLine(builder);
		AppendLine(builder, "| | classes | |");
		AppendLine(builder, "|---|---|---|");
		if (_ir.Meta?.Addons is { } addons)
		{
			var handWrittenAddons = string.Join(", ", EmitterConfig.HandWrittenAddonClassNames.Select(x => $"`{x}`"));
			var unwrappedAddonCount = addons.Classes - EmitterConfig.HandWrittenAddonClassNames.Count;
			AppendLine(builder, $"| addons (`{addons.Path}`) | {addons.Classes} | **{EmitterConfig.HandWrittenAddonClassNames.Count} wrapped by hand**: {handWrittenAddons}, each vendored as its own static asset beside the bundle. The other {unwrappedAddonCount} are not wrapped - no post-processing passes, no exporters, no other controls. `DRACOLoader` and `KTX2Loader` are vendored too, but only as decoder dependencies `GLTFLoader` wires in when `GLTFLoadOptions` opts a load into one, never as a class a consumer constructs directly. The generator reads none of them either way, which is why they sit outside the class total |");
		}

		foreach (var excluded in _ir.Meta?.ExcludedDirectories ?? [])
		{
			AppendLine(builder, $"| the TSL / WebGPU node stack (`{excluded.Path}`) | {excluded.Classes} | the shipped bundle **does** carry them - the renderer is `WebGPURenderer` and every material it draws is a node graph - but they are outside the surface the extractor reads, and deliberately so: TSL's operators are grafted onto node prototypes at runtime and its typing lives in TypeScript generics no C# signature carries, so a mirror of it would be a lossy shadow. `ThreeContext.LoadNodeAsync` reaches the real thing instead |");
		}

		AppendLine(builder);
		AppendLine(builder, "And inside the total, deliberately out of the mirrored surface:");
		AppendLine(builder);

		var rendererInternalCount = _scope.Results
			.Count(x => EmitterConfig.ExcludedSourcePrefixes.Any(prefix => x.Class.File.StartsWith(prefix, StringComparison.Ordinal)));

		var mathCount = _scope.Results
			.Count(x => x.Status == ClassScopeStatus.OutOfSurface && x.Category == SkipCategory.MathValueType);

		// Named from the scope rather than from a written-out list, so a type that is ported - or one
		// that upstream adds and nobody ports - moves between the two halves of this sentence on its own.
		var unportedMathTypeNames = _scope.Results
			.Where(x => x.Status == ClassScopeStatus.OutOfSurface && x.Category == SkipCategory.MathValueType)
			.Select(x => x.Class.Name)
			.Where(x => !EmitterConfig.MathTypeNames.Contains(x))
			.Order(StringComparer.Ordinal)
			.ToList();

		var handWrittenNames = string.Join(", ", EmitterConfig.HandWrittenClassNames.Order(StringComparer.Ordinal).Select(x => $"`{x}`"));
		var mathTypeNames = string.Join(", ", EmitterConfig.MathTypeNames.Order(StringComparer.Ordinal).Select(x => $"`{x}`"));
		var unportedNames = unportedMathTypeNames.Any()
			? string.Join(", ", unportedMathTypeNames.Select(x => $"`{x}`"))
			: "none";

		var prefixes = string.Join(", ", EmitterConfig.ExcludedSourcePrefixes.Select(x => $"`{x}**`"));
		var rendererTypes = string.Join(", ", EmitterConfig.ConsumerFacingRendererClassNames.Select(x => $"`{x}`"));

		AppendLine(builder, "| | classes | |");
		AppendLine(builder, "|---|---|---|");
		AppendLine(builder, $"| renderer internals ({prefixes}) | {rendererInternalCount} | the types consumers actually name ({rendererTypes}) are outside those directories and are generated |");
		AppendLine(builder, $"| `{EmitterConfig.MathSourcePrefix}**` value types | {mathCount} | {EmitterConfig.MathTypeNames.Count} of them ship, hand-ported ({mathTypeNames}); the other {unportedMathTypeNames.Count} do not: {unportedNames}. A math value is arithmetic rather than a signature: the generator has their members but not their behaviour, so each one waits on a hand port |");
		AppendLine(builder, $"| {handWrittenNames} | {EmitterConfig.HandWrittenClassNames.Count} | **hybrid**: hand-written for behaviour, generated for surface. The hand-written part carries the scene-graph machinery - attachment, the transform, pre-attach state replay; a generated `partial` beside it carries three.js's `Object3D` commands and queries (`RotateX`, `Attach`, `GetObjectByNameAsync`, …). Not counted as generated above, because no generator makes the type itself |");
		AppendLine(builder);
	}

	/// <summary>
	/// What the read op reaches and what it still does not. Stated in the README rather than left to
	/// the coverage report, because "can I get a value back" is the first thing a consumer asks and the
	/// honest answer is "some of them" — a figure that reads worse than the previous "none" did but is
	/// the one that survives being checked.
	/// <para>
	/// Every figure here is counted from the classification rather than written down, so the section
	/// cannot claim a channel is shut after the generator has opened it. It said exactly that once.
	/// </para>
	/// </summary>
	private void AppendReadmeReadChannel(StringBuilder builder)
	{
		var emittableMembers = EmittableMembers().ToList();
		var queries = emittableMembers.Where(x => x.Bucket == MemberBucket.AsyncQuery).ToList();
		var adoptedCount = queries.Count(x => x.IsAdoptedResult);
		var untypedObjectCount = queries.Count(x => x.IsUntypedObjectResult);
		// An awaited void is a query that answers nothing: three.js's `clearAsync` and friends resolve
		// when the GPU is done, and the completion is the whole answer. Counted apart from the value
		// queries, which would otherwise say a value comes back where none does.
		var awaitedVoidQueries = queries.Where(x => x.IsAwaitedVoidResult).ToList();
		var valueQueries = queries.Where(x => !x.IsAdoptedResult && !x.IsUntypedObjectResult && !x.IsAwaitedVoidResult).ToList();
		var valuePropertyCount = valueQueries.Count(x => x.IsPropertyRead);
		var valueMethodCount = valueQueries.Count - valuePropertyCount;
		var noHandleCount = emittableMembers.Count(x => x.SkipCategory == SkipCategory.NoHandleForResult);
		var callbackCount = emittableMembers.Count(x => x.SkipCategory == SkipCategory.CallbackType);
		var domCount = emittableMembers.Count(x => x.SkipCategory == SkipCategory.DomOrLibType);
		// Split rather than labelled "static", which this bucket is only mostly made of: `NotInstanceApi`
		// also carries the protected and private members, and calling the whole figure static overstates
		// how much of it a handle-addressable mirror could ever have reached.
		var notInstanceApi = emittableMembers
			.Where(x => x.SkipCategory == SkipCategory.NotInstanceApi)
			.ToList();

		var staticCount = notInstanceApi.Count;
		var staticMemberCount = notInstanceApi.Count(x => x.IsStatic);
		var collectionCount = emittableMembers.Count(x =>
			x.MemberKind == ClassifiedMemberKind.Method &&
			x.SkipCategory == SkipCategory.CollectionType);

		AppendLine(builder, "### ⚠️ What reads back, and what does not");
		AppendLine(builder);
		AppendLine(builder, "Two of the wire format's op kinds answer: **read** invokes a three.js method, and **get** reads a");
		AppendLine(builder, "property. Both travel inside the batch they were recorded in, so either always observes the writes made");
		AppendLine(builder, "before it, and both are generated as `…Async` methods returning a task.");
		AppendLine(builder);
		AppendLine(builder, $"They answer in two ways. A **value** comes back as itself - numbers, booleans, strings, and the {EmitterConfig.MathTypeNames.Count}");
		AppendLine(builder, "hand-written math types, tagged exactly as they are sent in the other direction. An **object** cannot:");
		AppendLine(builder, "serializing one would hand C# a plausible bag of numbers instead of a value. So the applier registers it");
		AppendLine(builder, "under a handle of its own and answers with a reference to that handle instead, which is what makes");
		AppendLine(builder, "`renderer.shadowMap` and `mesh.CloneAsync()` reachable at all.");
		AppendLine(builder);
		AppendLine(builder, $"On the generated classes that reaches **{queries.Count} members**:");
		AppendLine(builder);
		AppendLine(builder, $"- **{valueQueries.Count} answer with a value** - {Pluralize(valueMethodCount, "method", "methods")} (focal length and effective field of view, elapsed time,");
		AppendLine(builder, $"  curve lengths, instance matrices and colours, vertex positions, layer tests) and {valuePropertyCount} read-only");
		AppendLine(builder, "  properties (`uuid`, `instanceCount`, and three.js's own `isMesh`-style type tags). A read-only property");
		AppendLine(builder, "  is read on demand rather than mirrored, because three.js is the only side that ever assigns it: a C#");
		AppendLine(builder, "  property would imply the mirror knew the value without asking.");
		AppendLine(builder, $"- **{adoptedCount} answer with a mirrored object** - `Task<T?>` over the generated type, adopted under the handle the");
		AppendLine(builder, "  applier registered it beneath. A handle this context already mirrors resolves back to that same C#");
		AppendLine(builder, "  object rather than to a second wrapper of it - which is what makes a method returning its own");
		AppendLine(builder, "  receiver safe - and `null` means the member genuinely held none.");
		AppendLine(builder, $"- **{untypedObjectCount} answer with an object no generated class mirrors** - `Task<Primitive?>`, the same untyped wrapper");
		AppendLine(builder, "  the escape hatch hands out. The handle is real and writable; nothing type-checks the members you name");
		AppendLine(builder, "  on it. Adoption dedupes here on the same terms as above, and a handle this context mirrors as");
		AppendLine(builder, "  something *other* than a `Primitive` faults instead of being wrapped a second time - that mirror is the");
		AppendLine(builder, "  better answer and the caller is already holding it.");
		if (awaitedVoidQueries.Count > 0)
		{
			AppendLine(builder, $"- **{awaitedVoidQueries.Count} answer nothing at all** - a bare `Task`, awaited for *when* rather than for what.");
			AppendLine(builder, "  three.js declares these as returning a promise (`renderer.clearAsync`, `renderer.waitForGPU`), and the");
			AppendLine(builder, "  promise settles when the GPU has finished rather than when the call returned - so the applier waits");
			AppendLine(builder, "  for it before answering the row. Recording them as call ops would apply just as well and complete");
			AppendLine(builder, "  immediately, which is the one thing their name says they do not do.");
		}

		AppendLine(builder);
		AppendLine(builder, "What remains out of reach is out for reasons a handle does not fix:");
		AppendLine(builder);
		AppendLine(builder, $"- **{Pluralize(callbackCount, "member", "members")} taking or returning a JavaScript callback** - the wire carries ops in one direction only,");
		AppendLine(builder, "  so there is nothing to call back into C# with.");
		AppendLine(builder, $"- **{Pluralize(domCount, "member", "members")} typed as a DOM or TypeScript lib type** - C# holds no `HTMLCanvasElement` to hand over,");
		AppendLine(builder, "  and a handle names a three.js object rather than an arbitrary browser one.");
		AppendLine(builder, $"- **{Pluralize(staticCount, "member", "members")} that are not instance API** - {staticMemberCount} of them static, which the mirror has no handle to");
		AppendLine(builder, $"  address because a static belongs to the class rather than to any object it holds, and {staticCount - staticMemberCount}");
		AppendLine(builder, "  declared `protected` or `private`, which three.js does not offer a consumer in the first place.");
		if (collectionCount > 0)
		{
			AppendLine(builder, $"- **{Pluralize(collectionCount, "method", "methods")} returning or taking an array** - which is why **`Raycaster.intersectObjects` is still not");
			AppendLine(builder, "  callable**: it answers with `Intersection[]`, and the wire has no array encoding in either direction.");
		}

		if (noHandleCount > 0)
		{
			AppendLine(builder, $"- **{Pluralize(noHandleCount, "member", "members")} whose result is neither** - an array of objects, say, which would need a handle minted per");
			AppendLine(builder, "  element rather than one for the result.");
		}

		AppendLine(builder);
		AppendLine(builder, "A read is caller-initiated and costs one interop call. An idle scene still costs **zero** - nothing polls,");
		AppendLine(builder, "and no callback runs per frame.");
		AppendLine(builder);
	}

	/// <summary>
	/// Lists the blocked classes whose capability is still reachable, and how. A blocked class is not
	/// the same as a lost feature, and a table that only counts them implies it is.
	/// </summary>
	private void AppendBlockedClassWorkarounds(StringBuilder builder)
	{
		// Counted per result, like every other class figure in this document. Four three.js names are
		// declared in two files each, so counting distinct names instead would put a different total here
		// than the class table three paragraphs up — one document, two answers for the same question. The
		// member rows are the one place that does count per name, and say why at the constructor.
		var blocked = _scope.Results
			.Where(x => x.Status == ClassScopeStatus.Blocked)
			.ToList();

		var blockedNames = blocked
			.Select(x => x.Class.Name)
			.ToHashSet(StringComparer.Ordinal);

		// Disjoint by construction: a workaround only counts for a class the bundle actually exports,
		// because one naming a class no runtime has would not work anyway.
		var withWorkaround = blocked
			.Where(x => x.Class.IsRuntimeExport && EmitterConfig.BlockedClassWorkarounds.ContainsKey(x.Class.Name))
			.ToList();

		var stillBlocked = EmitterConfig.BlockedClassWorkarounds
			.Where(x => withWorkaround.Any(result => string.Equals(result.Class.Name, x.Key, StringComparison.Ordinal)))
			.OrderBy(x => x.Key, StringComparer.Ordinal)
			.ToList();

		var noLongerBlocked = EmitterConfig.BlockedClassWorkarounds.Keys
			.Where(x => !blockedNames.Contains(x))
			.Order(StringComparer.Ordinal)
			.ToList();

		var absentFromBundle = blocked.Count(x => !x.Class.IsRuntimeExport);
		var unaccounted = blocked.Count - absentFromBundle - withWorkaround.Count;

		AppendLine(builder, "### Blocked, but still reachable");
		AppendLine(builder);
		AppendLine(builder, $"The {blocked.Count} blocked classes account for themselves as follows:");
		AppendLine(builder);
		AppendLine(builder, $"- **{absentFromBundle} are absent from the shipped three.js bundle.** Not a mapping decision: `THREE[name]` is");
		AppendLine(builder, "  `undefined` for every one of them, so nothing could construct them — not a generated class, and not the");
		AppendLine(builder, "  escape hatch either.");
		AppendLine(builder, $"- **{withWorkaround.Count} lose no capability**, listed below. They are abstract bases whose concrete subclasses all");
		AppendLine(builder, "  generate, convenience subclasses that only rearrange constructor arguments, or classes the untyped");
		AppendLine(builder, "  escape hatch constructs by name.");
		if (unaccounted > 0)
		{
			AppendLine(builder, $"- **{unaccounted} are neither**, and are a genuine gap nobody has written a route to yet.");
		}

		AppendLine(builder);
		AppendLine(builder, "A blocked class is therefore not automatically a missing feature, and a count on its own implies it is.");
		AppendLine(builder);
		AppendLine(builder, "| class | how to get the same result |");
		AppendLine(builder, "|---|---|");
		foreach (var (name, workaround) in stillBlocked)
		{
			AppendLine(builder, $"| `{name}` | {workaround} |");
		}

		AppendLine(builder);

		// Named rather than silently dropped: a note that outlives the limitation it describes sends a
		// reader to the escape hatch for a class that now has a generated type.
		if (noLongerBlocked.Any())
		{
			var names = string.Join(", ", noLongerBlocked.Select(x => $"`{x}`"));
			AppendLine(builder, $"⚠️ {noLongerBlocked.Count} entries above describe classes that are no longer blocked and should be removed from");
			AppendLine(builder, $"`EmitterConfig.BlockedClassWorkarounds`: {names}.");
			AppendLine(builder);
		}
	}

	private void AppendReadmeNarrowings(StringBuilder builder)
	{
		var droppedClassCount = EmittableClasses.Count(x => x.Constructor is { DroppedParameters.Count: > 0 });
		var droppedParameterCount = EmittableClasses.Sum(x => x.Constructor?.DroppedParameters.Count ?? 0);
		// Scoped to methods that are actually emitted. Across all 309 classes 31 methods declare several
		// overloads, but a method that is skipped or waiting on a read op is not narrowed by taking the
		// first signature — it is not there at all, and is already counted elsewhere.
		var overloadedCount = EmittableMembers().Count(x => x.OverloadCount > 1 && x.Bucket == MemberBucket.Command);
		var emittableCount = _scope.Results.Count(x => x.Status == ClassScopeStatus.Emittable);

		AppendLine(builder, "### Where a generated type is narrower than three.js");
		AppendLine(builder);
		AppendLine(builder, $"- **{droppedClassCount} of the {emittableCount} generated classes have a narrower constructor.** {droppedParameterCount} trailing optional parameters");
		AppendLine(builder, "  whose type does not map are dropped; calling the JavaScript constructor with fewer arguments is what");
		AppendLine(builder, "  three.js is built for, so the result is a faithful subset rather than a guess. A **required** parameter");
		AppendLine(builder, "  that does not map blocks the whole class instead.");
		if (overloadedCount > 0)
		{
			AppendLine(builder, $"- **{overloadedCount} generated methods declare more than one *TypeScript* overload upstream, and only the first");
			AppendLine(builder, "  signature is emitted.** Unrelated to the arm overloads below, which come from one signature.");
		}

		AppendLine(builder, "- **A colour is a `Color`.** three.js also accepts a CSS string or a hex number wherever a colour is");
		AppendLine(builder, "  taken; the hex form is covered by `Color.FromHex`, the string form is not exposed.");
		if (_mapper.MultiValueNarrowings.Count > 0)
		{
			AppendLine(builder, $"- **`T | T[]` maps to `T`** in {_mapper.MultiValueNarrowings.Count} declared types, so a mesh with several materials is not expressible.");
		}

		var overloaded = UnionOverloadedMembers();
		if (overloaded.Count > 0)
		{
			AppendLine(builder, $"- **A union in a required parameter becomes one overload per arm** - {Pluralize(overloaded.Count, "member", "members")} carry");
			AppendLine(builder, $"  {overloaded.Sum(x => x.OverloadCount)} signatures between them, so `BufferGeometry.SetIndex` takes either a `BufferAttribute` or an");
			AppendLine(builder, "  `int[]`. Two costs: an **optional** union parameter is dropped instead, because every overload would");
			AppendLine(builder, "  accept the same argument-omitting call and none could win it; and where two arms are reference types");
			AppendLine(builder, "  anything that converts to both is ambiguous (CS0121) — `SetIndex(null)` and `SetFromPoints([])` do not");
			AppendLine(builder, "  compile, and need a cast (`SetIndex((int[]?) null)`) or a named argument.");
		}

		AppendLine(builder);
	}

	/// <summary>
	/// The other half of the coverage claim, and the reason every limit stated above is a limit of the
	/// <b>typed</b> surface rather than of the package. A partial typed surface with an escape hatch
	/// reads "this many typed, this many reachable"; the same surface without one reads "this many, and
	/// you are stuck" - and only one of those is what a consumer actually gets.
	/// <para>
	/// The word "reachable" is load-bearing and is not stretched: it means the shipped bundle exports
	/// the name, so the applier can construct it. A class three.js keeps to itself is reachable by
	/// nothing at all, and is counted on its own line rather than folded in.
	/// </para>
	/// </summary>
	/// <param name="builder">Destination.</param>
	private void AppendReadmeEscapeHatch(StringBuilder builder)
	{
		var emittableCount = _scope.Results.Count(x => x.Status == ClassScopeStatus.Emittable);
		var reachableCount = ReachableClassCount();
		var untypedCount = reachableCount - emittableCount;
		var unreachableCount = _scope.Results.Count - reachableCount;

		AppendLine(builder, "### Reaching what is not generated");
		AppendLine(builder);
		AppendLine(builder, "Everything above is a limit of the **typed** surface. None of it is a limit of the package, because a");
		AppendLine(builder, "class the generator refuses is still a class three.js has:");
		AppendLine(builder);
		AppendLine(builder, "- **`Primitive` / `PrimitiveObject3D`** construct any class the shipped bundle exports, by its three.js");
		AppendLine(builder, "  name - the same `new THREE[name](…)` the applier runs for a generated one.");
		AppendLine(builder, "- **`Set` / `Call` / `CallAsync` / `GetAsync`** reach any member of any object you hold, generated or");
		AppendLine(builder, "  not, by its three.js name. `GetAsync` reads a **property**, which is what puts the read-only ones above");
		AppendLine(builder, "  within reach.");
		AppendLine(builder, "- **`GetObjectAsync` / `CallObjectAsync`** are those two again for a member whose answer is an **object**:");
		AppendLine(builder, "  the applier registers it and hands back a `Primitive` you can write to, which is how a nested object no");
		AppendLine(builder, "  dotted path addresses - `renderer.shadowMap` - is reached.");
		AppendLine(builder);
		AppendLine(builder, "| | classes |");
		AppendLine(builder, "|---|---|");
		AppendLine(builder, $"| **generated, and typed** | **{emittableCount}** |");
		AppendLine(builder, $"| reachable by name, untyped | {untypedCount} |");
		AppendLine(builder, $"| **reachable at all** | **{reachableCount}** |");
		AppendLine(builder, $"| not exported by three.js, so reachable by nothing | {unreachableCount} |");
		AppendLine(builder, $"| **three.js core total** | **{_scope.Results.Count}** |");
		AppendLine(builder);
		AppendLine(builder, $"⚠️ The last row is not a gap this package can close. Those {unreachableCount} classes are ones three.js's own barrel does");
		AppendLine(builder, "not publish as values, so `THREE[name]` is `undefined` in the browser and there is nothing to construct -");
		AppendLine(builder, "by this package or by any other consumer of the same bundle. They are counted separately rather than");
		AppendLine(builder, "folded into the claim.");
		AppendLine(builder);
		AppendLine(builder, "⚠️ **The escape hatch is sharper than the typed surface, on purpose.** It bypasses the generated types,");
		AppendLine(builder, "so it bypasses what they know:");
		AppendLine(builder);
		AppendLine(builder, "- **Nothing checks the names.** A misspelled type, member or argument list is three.js's to reject, and it");
		AppendLine(builder, "  does so when the batch runs - through `OnError` for a write, and by faulting the task for a read.");
		AppendLine(builder, "- **The mirror does not learn from a raw `Set`.** `mesh.Set(\"visible\", false)` leaves `mesh.IsVisible`");
		AppendLine(builder, "  reporting `true`, and the next typed write of `true` then records nothing. Where a typed property exists,");
		AppendLine(builder, "  use it.");
		AppendLine(builder, "- **A raw `Set` made before the object is attached replays after every typed property**, whichever order the");
		AppendLine(builder, "  two were written in. A typed property is replayed from its field, which does not know when it was set.");
		AppendLine(builder, "- **⚠️ A lone array argument needs an `(object?)` cast.** `Call`, `CallAsync`, `CallObjectAsync`,");
		AppendLine(builder, "  `new Primitive(…)`, `new PrimitiveObject3D(…)` and `ThreeContext.LoadNodeAsync` all take");
		AppendLine(builder, "  `params object?[]`, and C# array covariance makes a **reference-type** array convertible to it — so");
		AppendLine(builder, "  `Call(\"setFromPoints\", points)` binds `points` as the whole argument list and three.js receives one");
		AppendLine(builder, "  argument per point. Write `Call(\"setFromPoints\", (object?) points)`. No overload can fix this: the");
		AppendLine(builder, "  non-expanded form wins on an identity conversion, so it would still be chosen. A **value-type** array");
		AppendLine(builder, "  (`float[]`, `int[]`) is unaffected, having no covariant conversion to `object?[]`. The generated");
		AppendLine(builder, "  classes carry the cast already; this is a limit of the escape hatch, and of the workaround column above.");
		AppendLine(builder);
		AppendLine(builder, "What it does **not** bypass: an object-valued write still attaches the object it references before the");
		AppendLine(builder, "op that names it, a call recorded before an attach is still replayed rather than dropped, writes still");
		AppendLine(builder, "coalesce per member and a call is still a barrier to that coalescing, and a value with no wire encoding is");
		AppendLine(builder, "still refused rather than shipped as a plain object. Those are properties of the batch, and the escape");
		AppendLine(builder, "hatch goes through it rather than around it.");
		AppendLine(builder);
	}

	private void AppendReadmeMeasurement(StringBuilder builder)
	{
		var functionCount = _ir.Meta?.Counts?.Functions ?? 0;
		AppendLine(builder, "### How this was measured");
		AppendLine(builder);
		AppendLine(builder, $"- **Classes**: every class declaration under `{_ir.Meta?.TypesPackage}@{_ir.Meta?.TypesVersion}`'s `{_ir.Meta?.SourceRoot}/`, minus the excluded");
		AppendLine(builder, $"  directories above, extracted into `generator/three-api.json`. three.js also exports {functionCount} top-level");
		AppendLine(builder, "  functions, which are not classes, are not counted in the total, and are not wrapped either.");
		AppendLine(builder, $"  `npm run extract:check` fails if that snapshot differs from what `{_ir.Meta?.TypesPackage}` says today.");
		AppendLine(builder, $"- **Generated is a class you can construct**: every one of them is a constructor on the `{_ir.Meta?.PublicSurface?.RuntimeBundle}`");
		AppendLine(builder, "  bundle that ships in this package, which `tests/wire-format.test.mjs` asserts name by name. A class");
		AppendLine(builder, "  three.js declares in its types but does not put on `THREE` is **blocked**, not counted.");
		AppendLine(builder, "- **Generated**: the files in `src/Blazor.ThreeJS/Generated/`, one per class or enum. `npm run emit:check`");
		AppendLine(builder, "  fails if any of them differs from what the generator produces today, or if one is left behind.");
		AppendLine(builder, $"- **Reachable is a name the bundle exports**: the extractor imports `{_ir.Meta?.PublicSurface?.RuntimeBundle}`");
		AppendLine(builder, "  and records, per class, whether three.js puts that name on `THREE` - the runtime itself rather than a");
		AppendLine(builder, "  second reading of the types. `tests/wire-format.test.mjs` asserts the figure from **both** sides: every");
		AppendLine(builder, "  class called reachable is a constructor on that bundle, and no class it leaves out is one, so the number");
		AppendLine(builder, "  can neither overstate nor understate itself.");
		AppendLine(builder, "- **Public members**: `grep -c \"^\\tpublic \" src/Blazor.ThreeJS/Generated/*.cs`, summed over the class files.");
		AppendLine(builder, "  The headline splits that sum, because one of those files is the generated half of a hand-written");
		AppendLine(builder, "  class rather than a class the generator made: its members are counted, and counted separately.");
		AppendLine(builder, "- **Everything else**: `generator/api-coverage.json`, written by the run that wrote this section. The");
		AppendLine(builder, "  per-class and per-member detail behind every figure, including each blocked class and each skipped");
		AppendLine(builder, "  member with its obstacle named, is in [`generator/api-coverage.md`](generator/api-coverage.md).");
		AppendLine(builder);
		AppendLine(builder, "This section is generated by `npm run emit` and rewritten in place between its markers. Editing it by");
		AppendLine(builder, "hand is pointless: the next run overwrites it, and `npm run emit:check` fails until it matches.");
	}

	private void AppendHeader(StringBuilder builder)
	{
		AppendLine(builder, "# API coverage");
		AppendLine(builder);
		AppendLine(builder, $"Generated by `generator/emitter` from `generator/three-api.json` (`{_ir.Meta?.TypesPackage}@{_ir.Meta?.TypesVersion}`).");
		AppendLine(builder, "Re-run `npm run emit` to refresh; `generator/api-coverage.json` carries the same data for the README table.");
		AppendLine(builder);
		AppendLine(builder, "This file answers two questions and nothing else: **which of three.js's classes the generator can");
		AppendLine(builder, "produce**, and **what happens to every member it declares**. Anything not covered says why, in terms of");
		AppendLine(builder, "the specific obstacle rather than the word \"unsupported\".");
		AppendLine(builder);
	}

	private void AppendMappingRules(StringBuilder builder)
	{
		AppendLine(builder, "## How a TypeScript type becomes a C# type");
		AppendLine(builder);
		AppendLine(builder, "Every type reference in the snapshot resolves to exactly one of five outcomes. There is no sixth,");
		AppendLine(builder, "so nothing reaches generated code without either a mapping or a recorded reason.");
		AppendLine(builder);
		AppendLine(builder, "| outcome | what it covers |");
		AppendLine(builder, "|---|---|");
		AppendLine(builder, "| C# primitive | `number` (`float` / `int`, see the numeric policy in `emitter-audit.md`), `boolean`, `string`, `void` |");
		AppendLine(builder, "| hand-written math type | `Vector3`, `Euler`, `Quaternion`, `Matrix4`, `Color` — encoded inline on the wire, never regenerated |");
		AppendLine(builder, "| generated wrapper class | any emitted class, passed by handle |");
		AppendLine(builder, "| generated enum | a type alias unioning `typeof` of numeric constants, or a numeric TypeScript `enum` |");
		AppendLine(builder, "| **skipped** | everything else, with the obstacle named |");
		AppendLine(builder);
		AppendLine(builder, "Seven rules are worth stating explicitly because they are judgement calls, not mechanics:");
		AppendLine(builder);
		AppendLine(builder, "1. **`ColorRepresentation` maps to `Color`.** three.js accepts `Color | string | number` wherever a colour");
		AppendLine(builder, "   is taken; the mirror exposes only `Color`, which reaches the browser as a real `THREE.Color` and covers");
		AppendLine(builder, "   the hex form through `Color.FromHex`. This is a **narrowing**: the CSS-string spelling is not exposed.");
		AppendLine(builder, "2. **Type parameters are erased to their default, failing that their constraint.** The C# object model is");
		AppendLine(builder, "   non-generic by design, so `Mesh<TGeometry = BufferGeometry>` maps as if the parameter were written out.");
		AppendLine(builder, "3. **An optional parameter whose type does not map is dropped, and everything after it with it.** Calling");
		AppendLine(builder, "   the JavaScript constructor with fewer arguments is exactly what three.js is built for, so the emitted");
		AppendLine(builder, "   class is a faithful subset rather than a guess. A **required** parameter that does not map blocks the");
		AppendLine(builder, "   whole class instead. Every dropped parameter is listed below.");
		AppendLine(builder, "4. **`src/math/**` is out of the generated surface.** Math values are by-value types encoded inline, not");
		AppendLine(builder, "   handle-backed objects; five are hand-written and the rest would need a representation decision first.");
		AppendLine(builder, "5. **`T | T[]` maps to `T`.** This is not a choice between two types; it is one type plus three.js's");
		AppendLine(builder, "   convenience form for supplying several. `Mesh.material` is declared `Material | Material[]`, and");
		AppendLine(builder, "   refusing it would leave a mesh with no material at all. A **narrowing**: the multi-material form is");
		AppendLine(builder, "   not exposed. A union of genuinely different types is a real choice, and rule 7 is where it goes.");
		AppendLine(builder, "6. **A method parameter is optional only when a real default can be written for it.** The `$undef`");
		AppendLine(builder, "   sentinel is constructor-arguments-only, so a method has no way to say \"not supplied\". Optionality is");
		AppendLine(builder, "   therefore resolved right to left: the moment a parameter cannot carry a default, every parameter");
		AppendLine(builder, "   before it becomes required. Emitting an optional three.js parameter as a required C# one is always");
		AppendLine(builder, "   safe; inventing a default would send a value three.js never agreed to.");
		AppendLine(builder, "7. **A required parameter whose type is a genuine union becomes one overload per arm.** A parameter is");
		AppendLine(builder, "   the only position C# lets a union through, because C# overloads on parameters — so");
		AppendLine(builder, "   `BufferGeometry.setIndex`, declared `BufferAttribute | number[] | null`, emits `SetIndex(BufferAttribute?)`");
		AppendLine(builder, "   beside `SetIndex(int[]?)`. An arm that does not map is left out rather than blocking the member, and");
		AppendLine(builder, "   arms that resolve to the same C# type are emitted once — `Iterable<number>` and `ArrayLike<number>`");
		AppendLine(builder, "   are both `float[]`, and declaring that signature twice would not compile. A **property** or a");
		AppendLine(builder, "   **return type** holds one type and has nowhere to put the second, so there the union stays refused.");
		AppendLine(builder);
		AppendLine(builder, "   ⚠️ Two things this costs the caller. An **optional** parameter is deliberately *not* expanded: every");
		AppendLine(builder, "   overload would accept the same argument-omitting call and it would be ambiguous (CS0121) in all of");
		AppendLine(builder, "   them, so an optional union parameter is dropped exactly as any other optional parameter the mapper");
		AppendLine(builder, "   cannot map. And where two arms are **reference types**, any argument that converts to both is");
		AppendLine(builder, "   ambiguous — a bare `null`, and an empty collection expression where both arms are arrays. So");
		AppendLine(builder, "   `geometry.SetIndex(null)` and `geometry.SetFromPoints([])` do not compile; a cast");
		AppendLine(builder, "   (`SetIndex((int[]?) null)`, `SetFromPoints((Vector3[]) [])`) or a named argument picks the arm.");
		AppendLine(builder);

		if (_mapper.MultiValueNarrowings.Count > 0)
		{
			AppendLine(builder, "Declared types narrowed by rule 5, each of which lost its multi-value form:");
			AppendLine(builder);
			foreach (var narrowed in _mapper.MultiValueNarrowings.Order(StringComparer.Ordinal))
			{
				AppendLine(builder, $"- `{narrowed}`");
			}

			AppendLine(builder);
		}

		var overloaded = UnionOverloadedMembers();
		if (overloaded.Count > 0)
		{
			var extraOverloads = overloaded.Sum(x => x.OverloadCount - 1);
			var largestSet = overloaded.Max(x => x.OverloadCount);
			AppendLine(builder, $"Emitted members that rule 7 gives more than one signature — {overloaded.Count} of them, carrying");
			AppendLine(builder, $"{extraOverloads} overloads beyond the one a single-typed parameter would have produced. A list rather");
			AppendLine(builder, "than a table, because every entry contains the `|` a table cell would split on:");
			AppendLine(builder);
			foreach (var member in overloaded)
			{
				AppendLine(builder, $"- `{member.Member}` — `{member.DeclaredTypeText}` → {member.OverloadCount} overloads");
			}

			AppendLine(builder);
			AppendLine(builder, $"The largest set is **{largestSet}**, against a budget of {EmitterConfig.UnionOverloadBudget}. The expansion is a cartesian product across");
			AppendLine(builder, "parameters, so it multiplies rather than adds: two three-arm parameters on one member would be nine");
			AppendLine(builder, "near-identical declarations. The figure is printed rather than enforced, because refusing to emit a");
			AppendLine(builder, "member three.js declares would be the worse answer — but it makes upstream growth visible here.");
			AppendLine(builder);
			if (largestSet > EmitterConfig.UnionOverloadBudget)
			{
				AppendLine(builder, $"⚠️ The budget is exceeded. Decide whether that member should still be expanded arm by arm.");
				AppendLine(builder);
			}
		}

		AppendDroppedUnionArms(builder);
	}

	/// <summary>
	/// Arms of a declared union that reached no overload. The other half of rule 7, and the half that is
	/// a loss: an arm that maps is a signature the caller gains, an arm that does not is part of the
	/// declared type the mirror does not carry — and this package does not narrow anything without
	/// recording why.
	/// </summary>
	/// <param name="builder">Destination.</param>
	private void AppendDroppedUnionArms(StringBuilder builder)
	{
		var rows = DroppedUnionArms();
		AppendLine(builder, "Arms of a declared union that no overload takes, across the whole snapshot rather than only the");
		AppendLine(builder, "emitted classes. Each is a narrowing of a member that does exist, so it is listed rather than left to");
		AppendLine(builder, "be inferred from an overload count:");
		AppendLine(builder);
		if (rows.Count == 0)
		{
			AppendLine(builder, "None.");
			AppendLine(builder);
			return;
		}

		foreach (var row in rows)
		{
			AppendLine(builder, $"- `{row.Member}` — `{row.ArmText}` out of `{row.DeclaredTypeText}`: {row.Reason}");
		}

		AppendLine(builder);
	}

	/// <summary>
	/// The emitted members a union in a required parameter turns into an overload set, read off the
	/// signatures the emitter actually produces rather than off the unions the mapper saw. A union on a
	/// class that ends up blocked, or on a member skipped for its return type, produces no overloads at
	/// all, and listing it here would claim a surface that is not there.
	/// </summary>
	/// <returns>One row per member, ordered by member then by declared type.</returns>
	private List<UnionOverloadedMember> UnionOverloadedMembers()
	{
		var rows = new List<UnionOverloadedMember>();
		foreach (var result in EmittableClasses.Where(x => x.Constructor is { Overloads.Count: > 1 }))
		{
			rows.Add(new UnionOverloadedMember
			{
				Member = $"{result.Class.Name}(…)",
				DeclaredTypeText = DescribeArmedParameters(result.Constructor!.Parameters),
				OverloadCount = result.Constructor.Overloads.Count
			});
		}

		foreach (var member in EmittedSurfaceMembers().Where(x => x.Method is { Overloads.Count: > 1 }))
		{
			rows.Add(new UnionOverloadedMember
			{
				Member = $"{member.ClassName}.{member.MemberName}",
				DeclaredTypeText = DescribeArmedParameters(member.Method!.Overloads[0]),
				OverloadCount = member.Method.Overloads.Count
			});
		}

		return [.. rows.OrderBy(x => x.Member, StringComparer.Ordinal).ThenBy(x => x.DeclaredTypeText, StringComparer.Ordinal)];
	}

	/// <summary>
	/// Every arm of every declared union that reached no C# overload, over the whole snapshot: the
	/// constructors and the methods, whether or not the declaring class is emitted.
	/// </summary>
	/// <returns>One row per dropped arm, ordered by member.</returns>
	private List<DroppedUnionArm> DroppedUnionArms()
	{
		var rows = new List<DroppedUnionArm>();
		foreach (var result in _scope.Results)
		{
			rows.AddRange(DescribeDroppedArms($"{result.Class.Name}(…)", result.Constructor?.Parameters ?? []));
		}

		foreach (var member in _members)
		{
			rows.AddRange(DescribeDroppedArms($"{member.ClassName}.{member.MemberName}", member.Method?.Overloads.FirstOrDefault() ?? []));
		}

		return [.. rows.OrderBy(x => x.Member, StringComparer.Ordinal).ThenBy(x => x.ArmText, StringComparer.Ordinal)];
	}

	private static IEnumerable<DroppedUnionArm> DescribeDroppedArms(string member, IReadOnlyList<MappedParameter> parameters)
	{
		return parameters.SelectMany(parameter => parameter.DroppedAlternatives.Select(dropped => new DroppedUnionArm
		{
			Member = member,
			DeclaredTypeText = $"{parameter.ThreeName}: {parameter.DeclaredTypeText}",
			ArmText = dropped.TypeText,
			Reason = dropped.Reason
		}));
	}

	private static string DescribeArmedParameters(IReadOnlyList<MappedParameter> parameters)
	{
		return string.Join(", ", parameters
			.Where(x => x.HasSeveralAlternatives)
			.Select(x => $"{x.ThreeName}: {x.DeclaredTypeText}"));
	}

	/// <summary>
	/// Every classified member that lands on a generated file: the emittable classes' members, plus the
	/// hybrid classes', whose generated half is a file of the same kind even though the type itself is
	/// hand-written.
	/// </summary>
	/// <returns>The members, in classification order.</returns>
	private IEnumerable<ClassifiedMember> EmittedSurfaceMembers()
	{
		var names = EmittableClasses
			.Select(x => x.Class.Name)
			.ToHashSet(StringComparer.Ordinal);

		names.UnionWith(EmitterConfig.HybridClassNames);
		return _members.Where(x => names.Contains(x.ClassName));
	}

	/// <summary>
	/// How a class's member set is worked out, which is not the same thing as the members its own
	/// declaration lists. This is the single largest determinant of how much API the mirror exposes.
	/// </summary>
	private void AppendSurfaceResolution(StringBuilder builder)
	{
		var byOrigin = _members
			.GroupBy(x => x.Origin)
			.ToDictionary(x => x.Key, x => x.Count());

		AppendLine(builder, "## How a class's member set is worked out");
		AppendLine(builder);
		AppendLine(builder, "three.js gives its classes their property surface through **declaration merging**: the class declaration");
		AppendLine(builder, "carries little more than a constructor, and an `export interface X extends XProperties {}` alongside it");
		AppendLine(builder, "supplies everything else. Reading only class-declared members produces a `MeshStandardMaterial` with no");
		AppendLine(builder, "`color`, no `roughness` and no `side` — the largest body of mirrored state in the library, invisible.");
		AppendLine(builder);
		AppendLine(builder, "Three things are resolved, in this order:");
		AppendLine(builder);
		AppendLine(builder, "1. **Interface inheritance.** Everything reachable through the same-named interface and its `extends`");
		AppendLine(builder, "   chain is pulled in, including any `declare module` block augmenting one of them.");
		AppendLine(builder, "2. **Ancestor flattening.** An ancestor with no C# type of its own — the abstract `Light`, say — has its");
		AppendLine(builder, "   members folded into the class rather than lost with it.");
		AppendLine(builder, "3. **Base subtraction.** Members the nearest mirrored ancestor already carries are removed, because C#");
		AppendLine(builder, "   inheritance provides them. Without this the same three.js member would be re-declared on every");
		AppendLine(builder, "   subclass, hiding the base member each time.");
		AppendLine(builder);
		AppendLine(builder, "| where the member came from | members |");
		AppendLine(builder, "|---|---|");
		foreach (var origin in Enum.GetValues<MemberOrigin>())
		{
			AppendLine(builder, $"| {DescribeOrigin(origin)} | {byOrigin.GetValueOrDefault(origin)} |");
		}

		AppendLine(builder);
		AppendHandWrittenBaseSurface(builder);
	}

	/// <summary>
	/// How the scene-graph base's surface is split between its hand-written and its generated half, and
	/// what neither half carries. Its members are subtracted from every descendant, so anything both
	/// halves leave out is on no C# type at all — the one place in this report where a member can go
	/// missing without any of the skip rules below having fired.
	/// </summary>
	private void AppendHandWrittenBaseSurface(StringBuilder builder)
	{
		var members = _members
			.Where(x => EmitterConfig.HandWrittenClassNames.Contains(x.ClassName))
			.ToList();

		var mirrorable = members
			.Where(x => x.Bucket is MemberBucket.MirroredState or MemberBucket.Command or MemberBucket.AsyncQuery)
			.ToList();

		var handWritten = mirrorable
			.Where(x => EmitterConfig.HandWrittenObject3DMemberNames.Contains(x.MemberName))
			.ToList();

		// Exactly the predicate `ClassEmitter.ResolveHybridSurface` emits on, so this table cannot claim a
		// member the generated partial does not carry, or miss one it does.
		var generated = mirrorable
			.Where(x => !EmitterConfig.HandWrittenObject3DMemberNames.Contains(x.MemberName))
			.Where(x => x.Bucket is MemberBucket.Command or MemberBucket.AsyncQuery)
			.ToList();

		var missing = mirrorable
			.Except(handWritten)
			.Except(generated)
			.ToList();

		AppendLine(builder, "⚠️ **`Object3D` is a hybrid, and its members are subtracted from every descendant.** The hand-written half");
		AppendLine(builder, "(`src/Blazor.ThreeJS/Objects/Object3D.cs`) carries the scene-graph machinery — attachment, the transform,");
		AppendLine(builder, "the pre-attach state replay — which is behaviour rather than surface. The generated half");
		AppendLine(builder, "(`src/Blazor.ThreeJS/Generated/Object3D.cs`) is the other part of the same `partial class`, and carries the");
		AppendLine(builder, "commands and queries, which are surface rather than behaviour. Subtracting the pair's members from each of");
		AppendLine(builder, "the ~100 descendants is right, because re-declaring them everywhere would be worse — but it means a member");
		AppendLine(builder, "**neither** half carries is on **no C# type at all**, without any skip rule below having fired.");
		AppendLine(builder);
		var handWrittenProperties = Pluralize(handWritten.Count(x => x.Bucket == MemberBucket.MirroredState), "property", "properties");
		var handWrittenMethods = Pluralize(handWritten.Count(x => x.Bucket != MemberBucket.MirroredState), "method", "methods");
		var generatedCommands = Pluralize(generated.Count(x => x.Bucket == MemberBucket.Command), "command", "commands");
		var generatedQueries = Pluralize(generated.Count(x => x.Bucket == MemberBucket.AsyncQuery), "query", "queries");
		AppendLine(builder, $"Of the {mirrorable.Count} `Object3D` members that could be mirrored, the hand-written half implements {handWritten.Count}");
		AppendLine(builder, $"({handWrittenProperties} and {handWrittenMethods}) and the generated half emits {generated.Count}");
		AppendLine(builder, $"({generatedCommands} and {generatedQueries}), leaving {missing.Count}.");
		AppendLine(builder);
		AppendLine(builder, "⚠️ The generated half emits **no mirrored state**, which is why what remains is almost all of that bucket.");
		AppendLine(builder, "Replaying a property needs an `EmitState` override, and the hand-written half already has one; a second");
		AppendLine(builder, "would write the same property twice on every attach. `parent` is in here for that reason rather than any");
		AppendLine(builder, "other — three.js declares it writable, so it classifies as state — and it is still readable through");
		AppendLine(builder, "`GetObjectAsync(\"parent\")`, which answers with a handle rather than trying to hold one.");
		AppendLine(builder);
		if (missing.Count == 0)
		{
			AppendLine(builder, "None.");
			AppendLine(builder);
			return;
		}

		AppendLine(builder, "| member | bucket | type |");
		AppendLine(builder, "|---|---|---|");
		foreach (var member in missing.OrderBy(x => x.Bucket.ToString(), StringComparer.Ordinal).ThenBy(x => x.MemberName, StringComparer.Ordinal))
		{
			AppendLine(builder, $"| `{member.MemberName}` | {member.Bucket} | `{member.CSharpTypeName ?? "—"}` |");
		}

		AppendLine(builder);
		AppendLine(builder, "Closing the rest means giving the generated half a replay hook the hand-written one cooperates with,");
		AppendLine(builder, "rather than the two of them both overriding `EmitState`.");
		AppendLine(builder);
	}

	/// <summary>
	/// The replay policy, which decides what actually reaches the browser when an object is attached.
	/// Stated here because it is a behavioural contract rather than a mapping rule, and because the
	/// obvious alternative is actively unsafe.
	/// </summary>
	private static void AppendReplayPolicy(StringBuilder builder)
	{
		AppendLine(builder, "## What gets replayed on attach");
		AppendLine(builder);
		AppendLine(builder, "A generated class replays **only the properties the caller actually wrote**. Each mirrored property");
		AppendLine(builder, "carries a flag set on first write, and `EmitCreate` / `EmitState` replay the ones that are set, so a");
		AppendLine(builder, "value written before the object was attached is never lost and construction order never matters.");
		AppendLine(builder);
		AppendLine(builder, "⚠️ **Replaying every property unconditionally would corrupt objects, not just cost bytes.** The mirror");
		AppendLine(builder, "has no read channel, so a property the caller never touched holds the emitter's *guess* at three.js's");
		AppendLine(builder, "default — the documented one where it is expressible, and the C# zero value where it is not.");
		AppendLine(builder, "`Material.stencilWriteMask` and `stencilFuncMask` document `0xff`, which is not a C# integer literal, so");
		AppendLine(builder, "the field starts at `0`; replaying that on every attach would silently disable stencil writes on every");
		AppendLine(builder, "material in the scene. Writing only what the caller wrote makes the mirror unable to be wrong about a");
		AppendLine(builder, "value it was never told.");
		AppendLine(builder);
		AppendLine(builder, "A math-typed property is mirrored as an instance the object owns and watches, so mutating it in place");
		AppendLine(builder, "(`material.Color.SetHex(…)`) counts as a write. `Matrix4` is the exception: it hands its components out");
		AppendLine(builder, "as a mutable array, so a change to it cannot be observed, and matrix-typed properties are not mirrored.");
		AppendLine(builder);
	}

	private static string DescribeOrigin(MemberOrigin origin)
	{
		return origin switch
		{
			MemberOrigin.Declared => "declared on the class itself",
			MemberOrigin.InterfaceInheritance => "reached through the interface three.js merges into the class",
			MemberOrigin.FlattenedAncestor => "folded in from an ancestor with no C# type of its own",
			MemberOrigin.ModuleAugmentation => "merged in by a `declare module` block",
			_ => throw new NotImplementedException($"Unhandled {nameof(MemberOrigin)} '{origin}'.")
		};
	}

	private void AppendClassCoverage(StringBuilder builder)
	{
		var byStatus = _scope.Results
			.GroupBy(x => x.Status)
			.ToDictionary(x => x.Key, x => x.Count());

		AppendLine(builder, "## Classes");
		AppendLine(builder);
		AppendLine(builder, "| status | classes |");
		AppendLine(builder, "|---|---|");
		AppendLine(builder, $"| emittable | {byStatus.GetValueOrDefault(ClassScopeStatus.Emittable)} |");
		AppendLine(builder, $"| deliberately out of the mirrored surface | {byStatus.GetValueOrDefault(ClassScopeStatus.OutOfSurface)} |");
		AppendLine(builder, $"| blocked | {byStatus.GetValueOrDefault(ClassScopeStatus.Blocked)} |");
		AppendLine(builder, $"| **total** | **{_scope.Results.Count}** |");
		AppendLine(builder);
		AppendLine(builder, $"Emittability and **reachability** are different questions. {ReachableClassCount()} of these classes are names the shipped");
		AppendLine(builder, "bundle puts on `THREE`, so the untyped `Primitive` can construct any of them whether or not the generator");
		AppendLine(builder, $"produced a type for it. The other {_scope.Results.Count - ReachableClassCount()} are exported by nothing and reachable by nothing, which is why");
		AppendLine(builder, "they are never folded into a coverage claim.");
		AppendLine(builder);

		AppendLine(builder, "### Out of the mirrored surface, by rule");
		AppendLine(builder);
		AppendGroupedReasons(builder, _scope.Results.Where(x => x.Status == ClassScopeStatus.OutOfSurface));

		var missingRendererTypes = EmitterConfig.ConsumerFacingRendererClassNames
			.Where(x => !_scope.IsEmittable(x))
			.ToList();

		AppendLine(builder, "The consumer-facing renderer types are checked against the exclusion rather than special-cased:");
		AppendLine(builder);
		foreach (var name in EmitterConfig.ConsumerFacingRendererClassNames)
		{
			var status = _scope.IsEmittable(name)
				? "emittable"
				: $"**not emittable** — {_scope.DescribeExclusion(name)}";

			AppendLine(builder, $"- `{name}`: {status}");
		}

		AppendLine(builder);
		if (missingRendererTypes.Count > 0)
		{
			AppendLine(builder, $"⚠️ {missingRendererTypes.Count} of them are not emitted. That is a coverage hole in the types consumers actually use.");
			AppendLine(builder);
		}

		AppendLine(builder, "### Blocked, by cause");
		AppendLine(builder);
		AppendGroupedReasons(builder, _scope.Results.Where(x => x.Status == ClassScopeStatus.Blocked));

		AppendLine(builder, "<details><summary>Every blocked class</summary>");
		AppendLine(builder);
		AppendLine(builder, "| class | why | file |");
		AppendLine(builder, "|---|---|---|");
		foreach (var result in _scope.Results.Where(x => x.Status == ClassScopeStatus.Blocked))
		{
			AppendLine(builder, $"| `{result.Class.Name}` | {result.Reason} | `{result.Class.File}` |");
		}

		AppendLine(builder);
		AppendLine(builder, "</details>");
		AppendLine(builder);

		AppendLine(builder, "<details><summary>Every emittable class</summary>");
		AppendLine(builder);
		AppendLine(builder, "| class | constructor parameters | dropped | file |");
		AppendLine(builder, "|---|---|---|---|");
		foreach (var result in EmittableClasses)
		{
			var dropped = result.Constructor?.DroppedParameters ?? [];
			var droppedText = dropped.Count == 0
				? "—"
				: string.Join(", ", dropped.Select(x => $"`{x.Name}`"));

			AppendLine(builder, $"| `{result.Class.Name}` | {result.Constructor?.Parameters.Count ?? 0} | {droppedText} | `{result.Class.File}` |");
		}

		AppendLine(builder);
		AppendLine(builder, "</details>");
		AppendLine(builder);
	}

	private void AppendMemberClassification(StringBuilder builder)
	{
		AppendLine(builder, "## Members");
		AppendLine(builder);
		AppendLine(builder, "Every property and method of every class in the IR, in one of four buckets. Classification is");
		AppendLine(builder, "independent of whether the declaring class is emitted, so the same rows answer both \"how much of");
		AppendLine(builder, "three.js is reachable at all\" and \"how much of what we mirror is state\".");
		AppendLine(builder);
		AppendLine(builder, "| bucket | what it means | all classes | emittable classes |");
		AppendLine(builder, "|---|---|---|---|");
		AppendBucketRow(builder, MemberBucket.MirroredState, "state C# holds and writes through on change");
		AppendBucketRow(builder, MemberBucket.Command, "a method recorded as a call op, returning nothing or `this`");
		AppendBucketRow(builder, MemberBucket.AsyncQuery, "a method whose result the caller needs back");
		AppendBucketRow(builder, MemberBucket.Skipped, "not mirrored; see the skip list below");
		AppendLine(builder, $"| **total** | | **{_members.Count}** | **{EmittableMembers().Count()}** |");
		AppendLine(builder);
		var emittableQueries = EmittableMembers().Where(x => x.Bucket == MemberBucket.AsyncQuery).ToList();
		var emittablePropertyReadCount = emittableQueries.Count(x => x.IsPropertyRead);
		AppendLine(builder, "Two op kinds answer: **read**, which invokes a method, and **get**, which reads a property.");
		AppendLine(builder, $"{emittableQueries.Count} of the async queries above sit on an emitted class and are generated as `…Async` methods, {emittablePropertyReadCount} of");
		AppendLine(builder, "them over the get op rather than the read op. Both kinds answer with a value where one can travel, and");
		AppendLine(builder, "with a handle to an object where it cannot.");
		AppendLine(builder);

		var overloadedMethods = _members
			.Where(x => x.OverloadCount > 1)
			.ToList();

		AppendLine(builder, $"⚠️ **{overloadedMethods.Count} methods declare more than one TypeScript overload, and only the first is classified.** Each");
		AppendLine(builder, "stands for several C# overloads; the classification says what the first signature is, not how many methods");
		AppendLine(builder, "a full run would emit. Rule 7's arm overloads are a different thing — those come from one signature whose");
		AppendLine(builder, "parameter unions several types, and are emitted in full.");
		AppendLine(builder);

		var augmented = _members
			.Where(x => x.Origin == MemberOrigin.ModuleAugmentation)
			.ToList();

		AppendLine(builder, "### Members merged in from module augmentations");
		AppendLine(builder);
		AppendLine(builder, $"A class's real member set is its own declaration plus every `declare module` block targeting it.");
		AppendLine(builder, $"{_ir.ModuleAugmentations.Count} augmentations exist and contribute {augmented.Count} members, which are classified here rather");
		AppendLine(builder, "than silently dropped.");
		AppendLine(builder);
		if (augmented.Count == 0)
		{
			AppendLine(builder, "None.");
			AppendLine(builder);
			return;
		}

		AppendLine(builder, "| class | member | bucket | why |");
		AppendLine(builder, "|---|---|---|---|");
		foreach (var member in augmented)
		{
			AppendLine(builder, $"| `{member.ClassName}` | `{member.MemberName}` | {member.Bucket} | {member.SkipReason ?? "mapped"} |");
		}

		AppendLine(builder);
	}

	private void AppendSkipList(StringBuilder builder)
	{
		AppendLine(builder, "## Skip list");
		AppendLine(builder);
		AppendLine(builder, "Every member the mirror does not express, grouped by the obstacle. This is the \"not covered\" half of");
		AppendLine(builder, "the README's coverage table.");
		AppendLine(builder);
		AppendLine(builder, "| obstacle | members | what it is |");
		AppendLine(builder, "|---|---|---|");
		foreach (var (category, count) in SkipCountsByCategory())
		{
			AppendLine(builder, $"| `{category}` | {count} | {DescribeCategory(category)} |");
		}

		AppendLine(builder);

		var skipped = _members
			.Where(x => x.Bucket == MemberBucket.Skipped)
			.ToList();

		AppendLine(builder, $"<details><summary>Every skipped member ({skipped.Count})</summary>");
		AppendLine(builder);
		AppendLine(builder, "| class | member | obstacle | why |");
		AppendLine(builder, "|---|---|---|---|");
		foreach (var member in skipped)
		{
			var kind = member.MemberKind == ClassifiedMemberKind.Property ? "property" : "method";
			AppendLine(builder, $"| `{member.ClassName}` | `{kind} {member.MemberName}` | `{member.SkipCategory}` | {member.SkipReason} |");
		}

		AppendLine(builder);
		AppendLine(builder, "</details>");
		AppendLine(builder);
	}

	private void AppendEnums(StringBuilder builder)
	{
		var generatable = _enums.Generatable;
		var referencedCount = generatable.Count(x => _mapper.RequiredEnumNames.Contains(x.Name));
		var inferredCount = generatable.Count(x => x.Source == EnumSource.ConstantGroup);

		AppendLine(builder, "## Enums");
		AppendLine(builder);
		AppendLine(builder, "three.js closes a value set in two ways, and both become one C# enum:");
		AppendLine(builder);
		AppendLine(builder, "- a real TypeScript `enum` — a direct translation, nothing is inferred;");
		AppendLine(builder, "- loose `export const`s **grouped by a type alias** that unions `typeof` of each one. The grouping is");
		AppendLine(builder, "  three.js's own (`type Side = typeof FrontSide | typeof BackSide | typeof DoubleSide`), read out of the");
		AppendLine(builder, "  declaration rather than guessed from a name prefix. A constant no alias groups stays a constant: an");
		AppendLine(builder, "  invented grouping would be worse than none.");
		AppendLine(builder);
		AppendLine(builder, "A group is only generatable when every value is **numeric**, because the wire encoder sends a C# enum");
		AppendLine(builder, "as its numeric backing value — a string-valued group would arrive as a number where three.js expects");
		AppendLine(builder, "the string. The backing type is the narrowest that holds every value, so three.js's small flag sets stay");
		AppendLine(builder, "`byte` and its WebGL constants land on `ushort`.");
		AppendLine(builder);
		AppendLine(builder, $"**{generatable.Count} generated**: {inferredCount} inferred from a constant group, {generatable.Count - inferredCount} from a real TypeScript `enum`.");
		AppendLine(builder, $"{referencedCount} are referenced by a mapped member today; the rest are emitted anyway, because an enum is a");
		AppendLine(builder, "leaf type whose availability should not move with the class surface.");
		AppendLine(builder);
		AppendLine(builder, "| enum | source | members | aliases | backing type | referenced |");
		AppendLine(builder, "|---|---|---|---|---|---|");
		foreach (var generatedEnum in generatable)
		{
			var aliases = generatedEnum.Members.Count(x => x.AliasOf is not null);
			var source = generatedEnum.Source == EnumSource.ConstantGroup
				? "constant group"
				: "TypeScript `enum`";

			var isReferenced = _mapper.RequiredEnumNames.Contains(generatedEnum.Name)
				? "yes"
				: "no";

			AppendLine(builder, $"| `{generatedEnum.Name}` | {source} | {generatedEnum.Members.Count} | {aliases} | `{generatedEnum.BackingTypeName}` | {isReferenced} |");
		}

		AppendLine(builder);

		var duplicateValueEnums = generatable
			.Where(x => x.Members.Any(member => member.AliasOf is not null))
			.ToList();

		AppendLine(builder, "### Duplicate values");
		AppendLine(builder);
		AppendLine(builder, "three.js gives several members the same number — `MOUSE.LEFT` and `MOUSE.ROTATE` are both `0`, and");
		AppendLine(builder, "`MinificationTextureFilter` carries four deprecated `MipMap` spellings of its `Mipmap` values. C# rejects");
		AppendLine(builder, "two members declared with the same literal, so the second names the first instead. No member is dropped:");
		AppendLine(builder);
		foreach (var generatedEnum in duplicateValueEnums)
		{
			var aliases = generatedEnum.Members
				.Where(x => x.AliasOf is not null)
				.Select(x => $"`{x.Name}` = `{x.AliasOf}`");

			AppendLine(builder, $"- `{generatedEnum.Name}`: {string.Join(", ", aliases)}");
		}

		AppendLine(builder);
		AppendUngroupedConstants(builder);
		AppendLine(builder, "### Refused");
		AppendLine(builder);
		AppendLine(builder, "| name | why |");
		AppendLine(builder, "|---|---|");
		foreach (var (name, reason) in _enums.Refusals)
		{
			AppendLine(builder, $"| `{name}` | {reason} |");
		}

		AppendLine(builder);
	}

	/// <summary>
	/// Constants that no type alias groups. They stay loose constants on purpose: the only grouping
	/// signal the generator trusts is three.js's own alias, and inventing one — by name prefix, by
	/// value range, by declaration adjacency — would produce an enum three.js does not agree exists.
	/// </summary>
	/// <param name="builder">Destination.</param>
	private void AppendUngroupedConstants(StringBuilder builder)
	{
		var groupedNames = _ir.TypeAliases
			.SelectMany(x => x.ConstantGroup ?? [])
			.ToHashSet(StringComparer.Ordinal);

		var ungrouped = _ir.Constants
			.Where(x => !groupedNames.Contains(x.Name))
			.ToList();

		var ungroupedValues = ungrouped
			.Where(x => x.Type is { Kind: "literal" })
			.ToList();

		AppendLine(builder, "### Constants left ungrouped");
		AppendLine(builder);
		AppendLine(builder, $"{ungrouped.Count} of the {_ir.Constants.Count} exported constants belong to no alias, so no enum claims them. Grouping them");
		AppendLine(builder, "would mean inventing a set three.js never declared, and an enum is a promise that its values are");
		AppendLine(builder, "exhaustive and mutually exclusive — a promise only the upstream alias is in a position to make.");
		AppendLine(builder);
		AppendLine(builder, $"Most are not value sets at all: only {ungroupedValues.Count} of the {ungrouped.Count} are a literal. The rest are namespace objects");
		AppendLine(builder, "(`MathUtils`, `ShaderChunk`, `UniformsLib`, `Cache`), which no grouping rule would have reached.");
		AppendLine(builder);
		if (ungrouped.Count == 0)
		{
			AppendLine(builder, "None.");
			AppendLine(builder);
			return;
		}

		AppendLine(builder, $"<details><summary>Every ungrouped constant ({ungrouped.Count})</summary>");
		AppendLine(builder);
		AppendLine(builder, "| constant | value | file |");
		AppendLine(builder, "|---|---|---|");
		foreach (var constant in ungrouped)
		{
			// The declared text is printed only for literals. Several of these constants are whole
			// namespace objects whose type text runs to four thousand characters of inline members,
			// which would drown the table without saying anything a reader needs.
			var value = constant.Type is { Kind: "literal", Text.Length: > 0 }
				? $"`{constant.Type.Text}`"
				: $"not a literal — a `{constant.Type?.Kind ?? "untyped"}` type";

			AppendLine(builder, $"| `{constant.Name}` | {value} | `{constant.File}` |");
		}

		AppendLine(builder);
		AppendLine(builder, "</details>");
		AppendLine(builder);
	}

	private void AppendHazards(StringBuilder builder)
	{
		var withHazards = EmittableClasses
			.Where(x => x.Constructor is { MiddlePositionUnspecifiedParameters.Count: > 0 })
			.ToList();

		var hazardCount = withHazards.Sum(x => x.Constructor!.MiddlePositionUnspecifiedParameters.Count);

		AppendLine(builder, "## Unspecified arguments in a middle position");
		AppendLine(builder);
		AppendLine(builder, "An optional parameter whose three.js default the types do not state is emitted as `T? x = null`,");
		AppendLine(builder, "meaning \"not supplied\". `ConstructorArgs` trims the unsupplied **tail** off the argument list, so");
		AppendLine(builder, "three.js applies its own default to it.");
		AppendLine(builder);
		AppendLine(builder, "Trimming only reaches the end. A JSON `null` is not JavaScript's `undefined`: `function f(a = 1) {}`");
		AppendLine(builder, "called as `f(null)` yields `null`, not `1`. So an unspecified parameter with a supplied one after it");
		AppendLine(builder, "cannot be trimmed and must not be sent as null either — it travels as the `$undef` sentinel");
		AppendLine(builder, "(`ThreeWireFormat.UndefinedKey`), which `three-interop.js` decodes to a real `undefined`. The");
		AppendLine(builder, "round trip is pinned end to end by `tests/wire-format.test.mjs` against the vendored three.js.");
		AppendLine(builder);
		AppendLine(builder, $"{withHazards.Count} emittable classes carry {hazardCount} such parameters. They are the measure of how much");
		AppendLine(builder, "of the emitted surface that one wire feature holds up.");
		AppendLine(builder);
		if (withHazards.Count == 0)
		{
			AppendLine(builder, "None.");
			AppendLine(builder);
			return;
		}

		AppendLine(builder, "<details><summary>Every affected class</summary>");
		AppendLine(builder);
		AppendLine(builder, "| class | parameters that depend on the `$undef` sentinel |");
		AppendLine(builder, "|---|---|");
		foreach (var result in withHazards)
		{
			var names = string.Join(", ", result.Constructor!.MiddlePositionUnspecifiedParameters.Select(x => $"`{x}`"));
			AppendLine(builder, $"| `{result.Class.Name}` | {names} |");
		}

		AppendLine(builder);
		AppendLine(builder, "</details>");
		AppendLine(builder);
	}

	/// <summary>
	/// How many classes the shipped bundle puts on <c>THREE</c>, which is what the applier resolves a
	/// create op against and therefore what the untyped escape hatch can construct. Read off the IR's
	/// own runtime-export flag, which the extractor produced by importing the bundle itself rather than
	/// by reading the types a second time.
	/// </summary>
	/// <returns>The number of reachable classes.</returns>
	private int ReachableClassCount()
	{
		return _scope.Results.Count(x => x.Class.IsRuntimeExport);
	}

	private IEnumerable<ClassifiedMember> EmittableMembers()
	{
		var emittableNames = EmittableClasses
			.Select(x => x.Class.Name)
			.ToHashSet(StringComparer.Ordinal);

		return _members.Where(x => emittableNames.Contains(x.ClassName));
	}

	private void AppendBucketRow(StringBuilder builder, MemberBucket bucket, string description)
	{
		var overall = CountBucket(bucket);
		var inEmittable = EmittableMembers().Count(x => x.Bucket == bucket);
		AppendLine(builder, $"| {bucket} | {description} | {overall} | {inEmittable} |");
	}

	private int CountBucket(MemberBucket bucket)
	{
		return _members.Count(x => x.Bucket == bucket);
	}

	private List<KeyValuePair<SkipCategory, int>> SkipCountsByCategory()
	{
		return _members
			.Where(x => x.Bucket == MemberBucket.Skipped)
			.GroupBy(x => x.SkipCategory)
			.Select(x => new KeyValuePair<SkipCategory, int>(x.Key, x.Count()))
			.OrderByDescending(x => x.Value)
			.ThenBy(x => x.Key.ToString(), StringComparer.Ordinal)
			.ToList();
	}

	private static void AppendGroupedReasons(StringBuilder builder, IEnumerable<ClassScopeResult> results)
	{
		var grouped = results
			.GroupBy(x => x.Category)
			.Select(x => new { Category = x.Key, Count = x.Count(), Example = x.First().Reason })
			.OrderByDescending(x => x.Count)
			.ThenBy(x => x.Category.ToString(), StringComparer.Ordinal)
			.ToList();

		if (grouped.Count == 0)
		{
			AppendLine(builder, "None.");
			AppendLine(builder);
			return;
		}

		AppendLine(builder, "| obstacle | classes | example reason |");
		AppendLine(builder, "|---|---|---|");
		foreach (var group in grouped)
		{
			AppendLine(builder, $"| `{group.Category}` | {group.Count} | {group.Example} |");
		}

		AppendLine(builder);
	}

	private static string DescribeStatus(ClassScopeStatus status)
	{
		return status switch
		{
			ClassScopeStatus.Emittable => "emittable",
			ClassScopeStatus.OutOfSurface => "outOfSurface",
			ClassScopeStatus.Blocked => "blocked",
			_ => throw new NotImplementedException($"Unhandled {nameof(ClassScopeStatus)} '{status}'.")
		};
	}

	private static string DescribeCategory(SkipCategory category)
	{
		return category switch
		{
			SkipCategory.None => "not skipped",
			SkipCategory.DomOrLibType => "a TypeScript lib or DOM type; C# holds no browser object and the wire has no encoding for one",
			SkipCategory.NodeStackType => "declared under `src/nodes/**`, the TSL / WebGPU node stack outside the extracted surface",
			SkipCategory.OptionsInterface => "a structural interface — an options bag or an event map — with no C# type to be",
			SkipCategory.MathValueType => "a `src/math/**` value type that is not one of the hand-written ones",
			SkipCategory.CollectionType => "a tuple, which has no wire encoding, or an array whose elements have none — `ThreeValue.Encode` does walk a sequence element by element, so an array is exactly as encodable as what is in it",
			SkipCategory.CallbackType => "a JavaScript callback; the wire format carries ops in one direction only",
			SkipCategory.StringConstantGroup => "a group of string-valued constants, which a C# enum cannot carry over this wire format",
			SkipCategory.UnmappedTypeAlias => "a type alias that is neither a constant group nor a rename of a mapped type",
			SkipCategory.UnmappedUnion => "a union of several real alternatives in a position that holds one type — a property or a return type, since a required parameter becomes one overload per arm",
			SkipCategory.UnmappedTypeSyntax => "a TypeScript type form with no C# equivalent",
			SkipCategory.LiteralType => "a literal type — three.js's `isMesh`-style runtime type tags",
			SkipCategory.AnonymousObjectType => "an anonymous object literal type with no name to give a C# type",
			SkipCategory.UntypedValue => "declared `any` / `unknown`, or with no type at all",
			SkipCategory.AbstractClass => "the class is abstract, so it has no constructor to mirror",
			SkipCategory.ConstructorOverloads => "the class declares more than one constructor",
			SkipCategory.NotExported => "three.js's public barrel does not re-export it as a value, so the applier cannot reach it on `THREE`",
			SkipCategory.AbsentFromShippedBundle => "the types re-export it but the shipped three.js bundle has no such runtime value to construct",
			SkipCategory.DuplicateClassName => "two classes share a name, and a C# namespace holds one type of a given name",
			SkipCategory.RequiredAfterOptional => "a required parameter follows an optional one, which C# forbids",
			SkipCategory.UnerasableTypeParameter => "a type parameter with neither a default nor a constraint to erase to",
			SkipCategory.UnwrappedClass => "an in-scope class that is itself not emitted",
			SkipCategory.ExternalType => "declared outside the scanned `src/` surface",
			SkipCategory.UnresolvedType => "the TypeScript checker could not resolve the name",
			SkipCategory.NotInstanceApi => "static, non-public or `@internal` — not part of the mirrored instance API",
			SkipCategory.NoHandleForResult => "its result is neither a value the read op carries nor one object a handle could name — an array of objects needs a handle per element, not one for the result",
			SkipCategory.ShadowedByConstructorParameter => "the constructor already takes it under the same name",
			SkipCategory.HandWritten => "the package provides the class by hand, and the generated classes derive from it",
			SkipCategory.UnreachableBaseConstructor => "its C# base requires constructor arguments the generated class has nothing to supply",
			SkipCategory.RestParameter => "a rest parameter, including the rest-union-tuple pseudo-overload form",
			_ => throw new NotImplementedException($"Unhandled {nameof(SkipCategory)} '{category}'.")
		};
	}

	/// <summary>One emitted member that a union in a required parameter gives several signatures.</summary>
	private sealed class UnionOverloadedMember
	{
		/// <summary>Member as the report names it, qualified by its class.</summary>
		public required string Member { get; init; }

		/// <summary>The declared union, or unions, the overloads stand for.</summary>
		public required string DeclaredTypeText { get; init; }

		/// <summary>How many signatures are emitted for it.</summary>
		public required int OverloadCount { get; init; }
	}

	/// <summary>One arm of a declared union that reached no C# overload.</summary>
	private sealed class DroppedUnionArm
	{
		/// <summary>Member as the report names it, qualified by its class.</summary>
		public required string Member { get; init; }

		/// <summary>The parameter and its whole declared union.</summary>
		public required string DeclaredTypeText { get; init; }

		/// <summary>The arm that was left out.</summary>
		public required string ArmText { get; init; }

		/// <summary>Why it could not be mapped.</summary>
		public required string Reason { get; init; }
	}

	/// <summary>Renders a count with the right noun, so a generated sentence never reads "1 methods".</summary>
	/// <param name="count">How many.</param>
	/// <param name="singular">Noun for one.</param>
	/// <param name="plural">Noun for any other count, including zero.</param>
	/// <returns>The count followed by the noun.</returns>
	private static string Pluralize(int count, string singular, string plural)
	{
		return count == 1
			? $"{count} {singular}"
			: $"{count} {plural}";
	}

	private static void AppendLine(StringBuilder builder, string text = "")
	{
		builder.Append(text);
		builder.Append('\n');
	}
}
