using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
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
		foreach (var irClass in ir.Classes.OrderBy(x => x.Name, StringComparer.Ordinal).ThenBy(x => x.File, StringComparer.Ordinal))
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
		AppendClassCoverage(builder);
		AppendMemberClassification(builder);
		AppendSkipList(builder);
		AppendEnums(builder);
		AppendHazards(builder);
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
				MiddlePositionUnspecifiedParameters = x.Constructor?.MiddlePositionUnspecifiedParameters.ToList() ?? []
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
				Members = _members.Count,
				MirroredState = CountBucket(MemberBucket.MirroredState),
				Commands = CountBucket(MemberBucket.Command),
				AsyncQueries = CountBucket(MemberBucket.AsyncQuery),
				SkippedMembers = CountBucket(MemberBucket.Skipped)
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
					Reason = x.SkipReason,
					Category = x.SkipCategory == SkipCategory.None ? null : x.SkipCategory.ToString()
				})
				.ToList()
		};

		var json = JsonSerializer.Serialize(document, _coverageJsonOptions);
		return json.ReplaceLineEndings("\n") + "\n";
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

	private static void AppendMappingRules(StringBuilder builder)
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
		AppendLine(builder, "Four rules are worth stating explicitly because they are judgement calls, not mechanics:");
		AppendLine(builder);
		AppendLine(builder, "1. **`ColorRepresentation` maps to `Color`.** three.js accepts `Color | string | number` wherever a colour");
		AppendLine(builder, "   is taken; the mirror exposes only `Color`, which reaches the browser as a real `THREE.Color` and covers");
		AppendLine(builder, "   the hex form through `Color.FromHex`. This is a **narrowing**: the CSS-string spelling is not exposed.");
		AppendLine(builder, "2. **Type parameters are erased to their default, failing that their constraint.** The C# object model is");
		AppendLine(builder, "   non-generic by design, so `Mesh<TGeometry = BufferGeometry>` maps as if the parameter were written out.");
		AppendLine(builder, "3. **An optional parameter whose type does not map is dropped, and everything after it with it.** Calling");
		AppendLine(builder, "   the JavaScript constructor with fewer arguments is exactly what three.js is built for, so the emitted");
		AppendLine(builder, "   class is a faithful subset rather than a guess. A **required** parameter that does not map blocks the");
		AppendLine(builder, "   whole class instead. Every dropped parameter is listed below — this is the one rule that narrows the");
		AppendLine(builder, "   public API relative to three.js.");
		AppendLine(builder, "4. **`src/math/**` is out of the generated surface.** Math values are by-value types encoded inline, not");
		AppendLine(builder, "   handle-backed objects; five are hand-written and the rest would need a representation decision first.");
		AppendLine(builder);
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
		AppendSealedBaseCollisions(builder);
	}

	/// <summary>
	/// Emittable classes that three.js derives from one of Plan 1's hand-written leaves. They map
	/// cleanly; they just cannot compile next to a <c>sealed</c> base. Recorded here because it is a
	/// property of the hand-written types rather than of the mapping, and it has to be settled before
	/// a full run can land.
	/// </summary>
	private void AppendSealedBaseCollisions(StringBuilder builder)
	{
		var collisions = EmittableClasses
			.Where(x => x.Class.Extends?.Name is { } baseName && EmitterConfig.SealedHandWrittenClassNames.Contains(baseName))
			.ToList();

		AppendLine(builder, "### Emittable, but their base is a `sealed` hand-written class");
		AppendLine(builder);
		AppendLine(builder, "Plan 1 sealed the hand-written classes because only leaves were needed. three.js subclasses several");
		AppendLine(builder, "of them, so these map cleanly but cannot compile alongside the current hand-written types. Unsealing");
		AppendLine(builder, "them is a public-API change rather than a mapping decision.");
		AppendLine(builder);
		if (collisions.Count == 0)
		{
			AppendLine(builder, "None.");
			AppendLine(builder);
			return;
		}

		AppendLine(builder, "| class | sealed base |");
		AppendLine(builder, "|---|---|");
		foreach (var result in collisions)
		{
			AppendLine(builder, $"| `{result.Class.Name}` | `{result.Class.Extends!.Name}` |");
		}

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
		AppendLine(builder, "⚠️ **No async query is emittable yet.** The wire format has five op kinds — create, set, call, add,");
		AppendLine(builder, "remove, dispose — and none of them reads a value back. Every member in that bucket is classified and");
		AppendLine(builder, "waiting on a read op, not on a mapping.");
		AppendLine(builder);

		var overloadedMethods = _members
			.Where(x => x.OverloadCount > 1)
			.ToList();

		AppendLine(builder, $"⚠️ **{overloadedMethods.Count} methods declare more than one overload, and only the first is classified.** Each stands");
		AppendLine(builder, "for several C# overloads; the classification says what the first signature is, not how many methods a");
		AppendLine(builder, "full run would emit.");
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
			SkipCategory.MathValueType => "a `src/math/**` value type beyond the five that are hand-written",
			SkipCategory.CollectionType => "an array or tuple; `ThreeValue.Encode` has no array arm",
			SkipCategory.CallbackType => "a JavaScript callback; the wire format carries ops in one direction only",
			SkipCategory.StringConstantGroup => "a group of string-valued constants, which a C# enum cannot carry over this wire format",
			SkipCategory.UnmappedTypeAlias => "a type alias that is neither a constant group nor a rename of a mapped type",
			SkipCategory.UnmappedUnion => "a union of several real alternatives, which one C# parameter cannot express",
			SkipCategory.UnmappedTypeSyntax => "a TypeScript type form with no C# equivalent",
			SkipCategory.LiteralType => "a literal type — three.js's `isMesh`-style runtime type tags",
			SkipCategory.AnonymousObjectType => "an anonymous object literal type with no name to give a C# type",
			SkipCategory.UntypedValue => "declared `any` / `unknown`, or with no type at all",
			SkipCategory.AbstractClass => "the class is abstract, so it has no constructor to mirror",
			SkipCategory.ConstructorOverloads => "the class declares more than one constructor",
			SkipCategory.NotExported => "the class is never exported, so the applier cannot reach it on `THREE`",
			SkipCategory.DuplicateClassName => "two classes share a name, and a C# namespace holds one type of a given name",
			SkipCategory.RequiredAfterOptional => "a required parameter follows an optional one, which C# forbids",
			SkipCategory.UnerasableTypeParameter => "a type parameter with neither a default nor a constraint to erase to",
			SkipCategory.UnwrappedClass => "an in-scope class that is itself not emitted",
			SkipCategory.ExternalType => "declared outside the scanned `src/` surface",
			SkipCategory.UnresolvedType => "the TypeScript checker could not resolve the name",
			SkipCategory.NotInstanceApi => "static, non-public or `@internal` — not part of the mirrored instance API",
			SkipCategory.ReadOnlyWithoutReadChannel => "read-only in three.js, and the wire format has no read op",
			SkipCategory.RestParameter => "a rest parameter, including the rest-union-tuple pseudo-overload form",
			_ => throw new NotImplementedException($"Unhandled {nameof(SkipCategory)} '{category}'.")
		};
	}

	private static void AppendLine(StringBuilder builder, string text = "")
	{
		builder.Append(text);
		builder.Append('\n');
	}
}
