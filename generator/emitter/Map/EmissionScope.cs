using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// Decides which of the IR's 309 classes are in the mirrored surface at all, and then which of those
/// can actually be emitted.
/// <para>
/// Emittability is a fixpoint, not a single pass: a class whose constructor takes another wrapped
/// class is only emittable if that one is. The set therefore starts as "every in-surface class" and
/// shrinks until it stops changing, which is also what lets two classes that reference each other
/// both stay in (neither blocks the other).
/// </para>
/// </summary>
internal sealed class EmissionScope
{
	private readonly Dictionary<string, ClassScopeResult> _resultsByName = new(StringComparer.Ordinal);
	private readonly Dictionary<string, ClassScopeResult> _anyResultByName = new(StringComparer.Ordinal);
	private readonly Dictionary<string, IrClass> _classesByName;
	private readonly List<ClassScopeResult> _results = [];

	/// <summary>Every class, with the verdict reached for it, ordered by name then file.</summary>
	public IReadOnlyList<ClassScopeResult> Results
	{
		get { return _results; }
	}

	/// <summary>Builds the scope. The constructor mapping is run repeatedly until the verdicts settle.</summary>
	/// <param name="ir">The parsed IR.</param>
	/// <param name="mapper">Type mapper, which asks this scope back about wrapped classes.</param>
	/// <param name="constructorMapper">Constructor mapping, shared with the emitter so both agree.</param>
	public EmissionScope(IrRoot ir, TypeMapper mapper, ConstructorMapper constructorMapper)
	{
		mapper.AttachScope(this);

		// Four names are declared in two files each, and only the first of each pair is emitted, so this
		// order decides which declaration's surface becomes the C# type.
		//
		// ⚠️ The primary-file rule below does less than it looks like it does. IsPrimaryDeclarationFile
		// cuts a basename at its *first* dot, so `WebGPURenderer.Nodes.d.ts` scores primary exactly as
		// `WebGPURenderer.d.ts` does, and the tie falls through to ordinal file order — which puts
		// `.Nodes` first. The emitted `WebGPURenderer` is therefore the `.Nodes` declaration's, and its
		// `library` is the one the flattened `Renderer` ancestor declares (`NodeLibrary`, skipped as
		// NotExported) rather than the `StandardNodeLibrary` the other declaration gives it — which is an
		// emitted class, and would have been mirrored state. The losing declaration is not silent about
		// it: it gets a blocked row naming the file that won.
		//
		// Left as it stands deliberately. Tightening the rule would change emitted code, which is a
		// change to make on its own rather than inside a comment fix.
		var orderedClasses = ir.Classes
			.OrderBy(x => x.Name, StringComparer.Ordinal)
			.ThenBy(x => IsPrimaryDeclarationFile(x) ? 0 : 1)
			.ThenBy(x => x.File, StringComparer.Ordinal)
			.ToList();

		foreach (var irClass in orderedClasses)
		{
			var result = new ClassScopeResult
			{
				Class = irClass,
				Status = ClassScopeStatus.Emittable
			};

			ApplySurfaceRules(result, irClass);
			_results.Add(result);
			_anyResultByName.TryAdd(irClass.Name, result);
			if (result.Status != ClassScopeStatus.Emittable)
			{
				continue;
			}

			// A duplicate name would produce two files with the same path and silently drop one, which
			// is the live hazard the IR schema warns about. The first by (name, file) order wins.
			if (!_resultsByName.TryAdd(irClass.Name, result))
			{
				result.Status = ClassScopeStatus.Blocked;
				result.Reason = $"another class named `{irClass.Name}` is declared in `{_resultsByName[irClass.Name].Class.File}`, and a C# namespace holds one type of a given name";
				result.Category = SkipCategory.DuplicateClassName;
			}
		}

		_classesByName = [];
		foreach (var irClass in ir.Classes)
		{
			_classesByName.TryAdd(irClass.Name, irClass);
		}

		var hasChanged = true;
		while (hasChanged)
		{
			hasChanged = false;
			foreach (var result in _results.Where(x => x.Status == ClassScopeStatus.Emittable))
			{
				var constructorMapping = constructorMapper.Map(result.Class, mapper);
				if (!constructorMapping.IsMapped)
				{
					Block(result, constructorMapping.RefusalReason!, constructorMapping.RefusalCategory);
					_resultsByName.Remove(result.Class.Name);
					hasChanged = true;
					continue;
				}

				result.Constructor = constructorMapping;
			}

			foreach (var result in _results.Where(x => x.Status == ClassScopeStatus.Emittable))
			{
				if (DescribeUnreachableBaseConstructor(result) is not { } reason)
				{
					continue;
				}

				Block(result, reason, SkipCategory.UnreachableBaseConstructor);
				_resultsByName.Remove(result.Class.Name);
				hasChanged = true;
			}
		}
	}

	/// <summary>
	/// Works out what a class passes to its C# base's constructor, and records it on the result.
	/// <para>
	/// three.js writes a subclass's constructor as its base's arguments plus its own — <c>Float32BufferAttribute</c>
	/// and <c>BufferAttribute</c> both start with <c>array, itemSize</c> — so the arguments to chain are
	/// the ones the two declarations share by name. Matching on the three.js name rather than on
	/// position is what lets an argument the base takes and the subclass does not (<c>EllipseCurve</c>'s
	/// <c>xRadius</c>, which <c>ArcCurve</c> does not declare) be left to its own default instead of
	/// silently receiving the value of whatever sat in that position.
	/// </para>
	/// <para>
	/// A base argument the subclass does not declare is only skippable when the base made it optional.
	/// Where it did not, there is nothing honest to pass — inventing a value would put the base's half
	/// of the mirror in a state three.js never produced — and the class stays blocked.
	/// </para>
	/// </summary>
	/// <param name="result">The class being tested, whose chain is set when one exists.</param>
	/// <returns>The obstacle, or <see langword="null"/> when the base constructor is reachable.</returns>
	private string? DescribeUnreachableBaseConstructor(ClassScopeResult result)
	{
		var irClass = result.Class;
		var baseName = irClass.Extends?.Name;
		while (baseName is not null)
		{
			if (EmitterConfig.HandWrittenClassNames.Contains(baseName))
			{
				return null;
			}

			if (_resultsByName.TryGetValue(baseName, out var baseResult) && baseResult.Status == ClassScopeStatus.Emittable)
			{
				return DescribeChainToBase(result, baseName, baseResult);
			}

			baseName = _classesByName.TryGetValue(baseName, out var baseClass)
				? baseClass.Extends?.Name
				: null;
		}

		return null;
	}

	/// <summary>
	/// Matches a class's constructor against its base's, one overload at a time.
	/// <para>
	/// Per overload rather than per class, because a parameter whose declared type unions several
	/// mappable ones is stored widened — as <c>object?</c> — and the widened slot is never what a base
	/// parameter wants. What has to line up is what each <em>declaration</em> takes, and every one of
	/// them has to line up: an overload whose <c>: base(…)</c> would not compile is not an overload that
	/// can be emitted, so one unchainable arm blocks the class rather than quietly disappearing.
	/// </para>
	/// </summary>
	/// <param name="result">The class being tested, whose chain is set when one exists.</param>
	/// <param name="baseName">Name of the nearest generated base.</param>
	/// <param name="baseResult">That base's own verdict, carrying its mapped constructor.</param>
	/// <returns>The obstacle, or <see langword="null"/> when every overload can chain.</returns>
	private static string? DescribeChainToBase(ClassScopeResult result, string baseName, ClassScopeResult baseResult)
	{
		var baseOverloads = baseResult.Constructor?.Overloads ?? [];
		if (baseOverloads.All(x => x.Count == 0))
		{
			result.BaseChains = [];
			return null;
		}

		string? obstacle = null;
		var kept = new List<IReadOnlyList<MappedParameter>>();
		var chains = new List<IReadOnlyList<BaseChainArgument>>();
		var droppedOverloads = new List<string>();
		foreach (var overload in result.Constructor?.Overloads ?? [])
		{
			IReadOnlyList<BaseChainArgument>? chained = null;
			foreach (var baseOverload in baseOverloads)
			{
				if (TryChainOverload(overload, baseOverload, baseName, out var chain, out var reason))
				{
					chained = chain;
					break;
				}

				obstacle ??= reason;
			}

			if (chained is null)
			{
				// A declaration that cannot satisfy the base is dropped rather than taken as a verdict on
				// the class. `Float32BufferAttribute` takes either a sequence or a length, and only the
				// sequence has something to give `BufferAttribute`'s `array` - so the class exists with the
				// constructor that works instead of not existing at all. Reported, because a caller who
				// wanted the other one has to find out from the coverage table rather than from a compiler
				// error with no explanation.
				droppedOverloads.Add($"({string.Join(", ", overload.Select(x => $"{x.CSharpTypeName} {x.Name}"))})");
				continue;
			}

			kept.Add(overload);
			chains.Add(chained);
		}

		// Every declaration failed. The base's constructor arguments are therefore ones three.js supplies
		// on its own side and C# never sees - `WebGPURenderer` builds its own backend inside
		// `super(new WebGPUBackend(parameters), parameters)` - so the class chains to the base's
		// no-argument form, which says the mirror was not told rather than pretending it was.
		//
		// ⚠️ Safe only because the base call feeds the mirror and nothing else. What reaches the browser
		// is this class's own `ConstructorArgs` under its own `ThreeTypeName`, and a required argument of
		// *its* that could not be mapped has already blocked it before this point. So an object built
		// this way is complete on the JavaScript side; what is unknown is only what C# can say about the
		// half of it the base declares.
		if (kept.Count == 0)
		{
			result.BaseChains = (result.Constructor?.Overloads ?? []).Select(_ => (IReadOnlyList<BaseChainArgument>) []).ToList();
			result.UninformedBaseChain = true;
			return null;
		}

		if (droppedOverloads.Count > 0 && result.Constructor is { } constructor)
		{
			result.Constructor = constructor.WithOverloads(kept, droppedOverloads);
		}

		// One chain per surviving declaration, in the same order. ⚠️ Not one chain for the class: two
		// declarations of the same constructor can need different arguments for the same base parameter.
		// `Float32BufferAttribute` takes a sequence or a length, and `new Float32Array(sequence)` is what
		// three.js builds from the first - while the same expression applied to the second would wrap the
		// length itself as a one-element array, which is a value three.js never holds.
		result.BaseChains = chains;
		return null;
	}

	/// <summary>
	/// Matches one declaration against one of the base's, by three.js parameter name.
	/// <para>
	/// Matching on the name rather than on position is what lets an argument the base takes and the
	/// subclass does not — <c>EllipseCurve</c>'s <c>xRadius</c>, which <c>ArcCurve</c> never declares —
	/// keep its own default instead of silently receiving whatever sat in that position. Types have to be
	/// identical, not merely convertible: a base slot holds what the mirror believes the JavaScript
	/// object's own property holds, and a widened or narrowed value would make the inherited property
	/// report something three.js never assigned.
	/// </para>
	/// </summary>
	/// <param name="overload">The subclass declaration being matched.</param>
	/// <param name="baseOverload">The base declaration being matched against.</param>
	/// <param name="baseName">Name of the base, for the obstacle text.</param>
	/// <param name="chain">The arguments to forward, when they all line up.</param>
	/// <param name="reason">What stopped the match, when one did.</param>
	/// <returns>Whether this declaration can chain to that one.</returns>
	private static bool TryChainOverload(
		IReadOnlyList<MappedParameter> overload,
		IReadOnlyList<MappedParameter> baseOverload,
		string baseName,
		out IReadOnlyList<BaseChainArgument> chain,
		out string? reason)
	{
		var own = overload.ToDictionary(x => x.ThreeName, StringComparer.Ordinal);
		var matched = new List<BaseChainArgument>();
		foreach (var baseParameter in baseOverload)
		{
			if (!own.TryGetValue(baseParameter.ThreeName, out var ownParameter))
			{
				if (baseParameter.IsOptional)
				{
					continue;
				}

				chain = [];
				reason = $"its C# base `{baseName}` requires `{baseParameter.ThreeName}`, which this class does not declare, so there is no value to chain that three.js would itself have produced";
				return false;
			}

			var expression = ChainExpression(ownParameter, baseParameter);
			if (expression is null)
			{
				if (baseParameter.IsOptional)
				{
					continue;
				}

				chain = [];
				reason = $"its C# base `{baseName}` requires `{baseParameter.ThreeName}` as `{baseParameter.CSharpTypeName}`, and this class declares it as `{ownParameter.CSharpTypeName}`, so the value it holds cannot stand in for the base's";
				return false;
			}

			matched.Add(new BaseChainArgument
			{
				ParameterName = baseParameter.DeclarationName,
				ArgumentName = ownParameter.DeclarationName,
				Expression = expression
			});
		}

		chain = matched;
		reason = null;
		return true;
	}

	/// <summary>
	/// How one of this class's parameters is written as an argument to the base's, or
	/// <see langword="null"/> when it cannot stand in for it.
	/// <para>
	/// Usually the parameter itself, under an identical type. The one substitution is a parameter this
	/// class declares as <c>T? x = null</c> — "the caller did not supply this" — against a base that
	/// declares it as <c>T</c> with a default. Not supplying it means three.js applies its own default,
	/// and that default is exactly what the base parameter's literal already holds, having come from the
	/// same documentation. Coalescing to it puts the base's field where three.js will put the JavaScript
	/// object's, which is the whole point of chaining.
	/// </para>
	/// <para>
	/// ⚠️ Only a <em>documented</em> default qualifies. A parameter with no expressible default is the
	/// one emitted as <c>T? x = null</c> in the first place, so a base slot in that state has no literal
	/// to coalesce to and is left alone rather than filled with a C# zero the mirror would then report
	/// as fact.
	/// </para>
	/// </summary>
	/// <param name="ownParameter">This class's parameter.</param>
	/// <param name="baseParameter">The base parameter it would be passed to.</param>
	/// <returns>The C# expression to write, or <see langword="null"/> when there is none.</returns>
	private static string? ChainExpression(MappedParameter ownParameter, MappedParameter baseParameter)
	{
		if (string.Equals(ownParameter.CSharpTypeName, baseParameter.CSharpTypeName, StringComparison.Ordinal))
		{
			return ownParameter.DeclarationName;
		}

		if (ownParameter.IsUnspecifiedNullable
			&& baseParameter.DefaultLiteral is { } literal
			&& string.Equals(ownParameter.CSharpTypeName, baseParameter.CSharpTypeName + "?", StringComparison.Ordinal))
		{
			return $"{ownParameter.DeclarationName} ?? {literal}";
		}

		// A concrete typed array where the base holds the abstract one. Identity is the rule because a
		// widened or narrowed *value* would misreport what three.js holds; this is neither. The object
		// handed up is the same object, under the base type it already derives from, and it is exactly
		// what `BufferAttribute.array` will contain.
		if (ownParameter.Mapping.Kind == TypeMappingKind.HandWrittenTypedArray
			&& baseParameter.Mapping.Kind == TypeMappingKind.HandWrittenTypedArray
			&& string.Equals(baseParameter.CSharpTypeName, EmitterConfig.TypedArrayBaseTypeName, StringComparison.Ordinal))
		{
			return ownParameter.DeclarationName;
		}

		return null;
	}

	/// <summary>
	/// Whether some emitted class chains to this one with nothing to give it, so it needs a no-argument
	/// constructor for that subclass to call.
	/// <para>
	/// Emitted only where a subclass actually needs it, rather than on every class that has one: an
	/// extra way to build an object with its own declared arguments left unknown is not something to
	/// offer unasked.
	/// </para>
	/// </summary>
	/// <param name="name">Class name.</param>
	/// <returns>Whether the no-argument constructor has to be written.</returns>
	public bool NeedsUninformedConstructor(string name)
	{
		return _results.Any(x =>
			x.Status == ClassScopeStatus.Emittable
			&& x.UninformedBaseChain
			&& NearestGeneratedBaseName(x.Class) == name);
	}

	/// <summary>The nearest ancestor this scope emits, which is the C# base a class chains to.</summary>
	/// <param name="irClass">Class to walk up from.</param>
	/// <returns>That ancestor's name, or <see langword="null"/> when the C# base is hand-written.</returns>
	private string? NearestGeneratedBaseName(IrClass irClass)
	{
		var baseName = irClass.Extends?.Name;
		while (baseName is not null)
		{
			if (EmitterConfig.HandWrittenClassNames.Contains(baseName))
			{
				return null;
			}

			if (_resultsByName.TryGetValue(baseName, out var baseResult) && baseResult.Status == ClassScopeStatus.Emittable)
			{
				return baseName;
			}

			baseName = _classesByName.TryGetValue(baseName, out var baseClass) ? baseClass.Extends?.Name : null;
		}

		return null;
	}

	/// <summary>
	/// The constructor this scope settled on, which is not always what mapping the class again would
	/// produce: chaining to a base can drop a declaration the mapper had no reason to. Read by the
	/// emitter so the two agree, which is what this type's own summary promises.
	/// </summary>
	/// <param name="name">Class name.</param>
	/// <returns>The resolved constructor, or <see langword="null"/> when the class is not emitted.</returns>
	public MappedConstructor? ConstructorFor(string name)
	{
		return _resultsByName.TryGetValue(name, out var result) ? result.Constructor : null;
	}

	public IReadOnlyList<IReadOnlyList<BaseChainArgument>> BaseChainsFor(string name)
	{
		return _resultsByName.TryGetValue(name, out var result)
			? result.BaseChains ?? []
			: [];
	}

	/// <summary>Whether a class is emitted, and therefore usable as a C# type in another signature.</summary>
	/// <param name="name">Class name.</param>
	/// <returns><see langword="true"/> when the class is in the emitted set.</returns>
	public bool IsEmittable(string name)
	{
		return _resultsByName.TryGetValue(name, out var result) && result.Status == ClassScopeStatus.Emittable;
	}

	/// <summary>Explains why a class is not emitted, for a skip reason that names the real obstacle.</summary>
	/// <param name="name">Class name.</param>
	/// <returns>The recorded reason, or a fallback when the name is not a class at all.</returns>
	public string DescribeExclusion(string name)
	{
		if (!_anyResultByName.TryGetValue(name, out var result))
		{
			return "it is not a class in the IR";
		}

		return result.Reason ?? "it is not emitted";
	}

	/// <summary>Which family the exclusion reason belongs to, so a skip inherits the real obstacle.</summary>
	/// <param name="name">Class name.</param>
	/// <returns>The recorded category, or <see cref="SkipCategory.UnwrappedClass"/> when there is none.</returns>
	public SkipCategory DescribeExclusionCategory(string name)
	{
		if (!_anyResultByName.TryGetValue(name, out var result) || result.Category == SkipCategory.None)
		{
			return SkipCategory.UnwrappedClass;
		}

		return result.Category;
	}

	/// <summary>
	/// Whether a declaration's file is named after the class, ranking it ahead of one that is not.
	/// <para>
	/// ⚠️ The basename is cut at its <b>first</b> dot, so this is true of <c>WebGPURenderer.Nodes.d.ts</c>
	/// as well as of <c>WebGPURenderer.d.ts</c> — a pair like that is not separated here and falls through
	/// to file order. See the ordering in the constructor for what that costs.
	/// </para>
	/// </summary>
	/// <param name="irClass">Class to test.</param>
	/// <returns><see langword="true"/> when the basename up to its first dot equals the class name.</returns>
	private static bool IsPrimaryDeclarationFile(IrClass irClass)
	{
		var basename = irClass.File.Split('/').Last();
		var declarationSuffixIndex = basename.IndexOf('.');
		if (declarationSuffixIndex < 0)
		{
			return false;
		}

		return string.Equals(basename[..declarationSuffixIndex], irClass.Name, StringComparison.Ordinal);
	}

	/// <summary>
	/// The rules that decide whether a class is part of the mirrored surface at all, before anything
	/// about its constructor is considered.
	/// </summary>
	private static void ApplySurfaceRules(ClassScopeResult result, IrClass irClass)
	{
		if (EmitterConfig.HandWrittenClassNames.Contains(irClass.Name))
		{
			var hybridNote = EmitterConfig.HybridClassNames.Contains(irClass.Name)
				? ". A generated `partial` beside it does carry its command and query surface, which is why this is an exclusion from *emitting the type* rather than from the mirror"
				: string.Empty;

			Exclude(
				result,
				$"hand-written by the runtime. It carries scene-graph behaviour — attachment, the transform, pre-attach state replay — rather than surface, so the generated classes derive from it instead of replacing it{hybridNote}",
				SkipCategory.HandWritten);
			return;
		}

		if (EmitterConfig.ExcludedSourcePrefixes.FirstOrDefault(x => irClass.File.StartsWith(x, StringComparison.Ordinal)) is { } excludedPrefix)
		{
			Exclude(result, $"renderer internals under `{excludedPrefix}**`; no consumer instantiates them and emitting them would inflate the coverage table", SkipCategory.UnwrappedClass);
			return;
		}

		if (irClass.File.StartsWith(EmitterConfig.MathSourcePrefix, StringComparison.Ordinal)
			&& !EmitterConfig.HandleBackedMathClassNames.Contains(irClass.Name))
		{
			Exclude(
				result,
				$"a `{EmitterConfig.MathSourcePrefix}**` value type. The mirror represents math values by value, encoded inline on the wire, not as handle-backed objects. " +
				$"{EmitterConfig.MathTypeNames.Count} are hand-written ({string.Join(", ", EmitterConfig.MathTypeNames.Order(StringComparer.Ordinal).Select(x => $"`{x}`"))}) and are never regenerated; " +
				$"giving the rest a representation is a public-API decision, not a mapping one",
				SkipCategory.MathValueType);
			return;
		}

		if (!irClass.IsExported)
		{
			Block(result, "three.js's public barrel does not re-export it as a value — either nothing re-exports it, or it is exported `type`-only — so it is not reachable on the `THREE` namespace the applier looks names up on", SkipCategory.NotExported);
			return;
		}

		if (!irClass.IsRuntimeExport)
		{
			Block(result, "the types re-export it but the shipped three.js bundle carries no such runtime value, so constructing it would throw `Unknown three.js type`", SkipCategory.AbsentFromShippedBundle);
			return;
		}

		if (!CSharpIdentifier.IsValid(irClass.Name))
		{
			Block(result, "the export name is not a usable C# identifier", SkipCategory.UnmappedTypeSyntax);
			return;
		}

		// Abstract classes are emitted, as abstract C# classes. They are never constructed - the applier
		// is never asked to `new THREE.Light()` and could not - but they are named: every concrete light
		// is a `Light`, and without the type the hierarchy is a row of unrelated leaves hanging off
		// ThreeObject. Their constructor is still mapped, because a subclass chains to it, and is emitted
		// `protected`, which makes "cannot be constructed" a fact the compiler holds.
		//
		// ⚠️ Except a *generic* one, which stays blocked. A type parameter erases to its default or its
		// constraint, and on an abstract base that is weaker than what each concrete subclass erases it
		// to: `Curve<TVector>.getPoint` returns `TVector`, which is `Vector2 | Vector3` on the base and
		// plainly `Vector3` on `CatmullRomCurve3`. Emitting the base moves the member onto it, where it
		// no longer maps - and the ten concrete curves lose six methods each that flattening was giving
		// them. Measured: 67 members across 17 classes. Until the erasure follows the subclass, the base
		// is worth less than the flattening it replaces.
		if (irClass.IsAbstract && irClass.TypeParameters is { Count: > 0 })
		{
			Block(
				result,
				"the class is abstract and generic, so emitting it would move its members onto a type parameter erased more weakly than each concrete subclass erases it - the subclasses carry them instead",
				SkipCategory.AbstractClass);
		}
	}

	private static void Exclude(ClassScopeResult result, string reason, SkipCategory category)
	{
		result.Status = ClassScopeStatus.OutOfSurface;
		result.Reason = reason;
		result.Category = category;
	}

	private static void Block(ClassScopeResult result, string reason, SkipCategory category)
	{
		result.Status = ClassScopeStatus.Blocked;
		result.Reason = reason;
		result.Category = category;
	}
}

/// <summary>The verdict reached for one class.</summary>
internal sealed class ClassScopeResult
{
	/// <summary>The class this verdict is about.</summary>
	public required IrClass Class { get; init; }

	/// <summary>Emittable, deliberately out of the mirrored surface, or blocked on something.</summary>
	public required ClassScopeStatus Status { get; set; }

	/// <summary>Why, when the status is not <see cref="ClassScopeStatus.Emittable"/>.</summary>
	public string? Reason { get; set; }

	/// <summary>Family the reason belongs to, for grouping.</summary>
	public SkipCategory Category { get; set; }

	/// <summary>The resolved constructor, present once the class is known to be emittable.</summary>
	public MappedConstructor? Constructor { get; set; }

	/// <summary>
	/// What each of this class's constructor declarations passes to its generated base, in the base's
	/// parameter order, one entry per surviving overload. Empty entries where the base takes no
	/// arguments; <see langword="null"/> when the base is hand-written or there is no generated base at
	/// all, and the emitted constructors therefore chain implicitly.
	/// </summary>
	public IReadOnlyList<IReadOnlyList<BaseChainArgument>>? BaseChains { get; set; }

	/// <summary>
	/// Whether this class has nothing at all to give its generated base, and chains to the base's
	/// no-argument form. The base needs that form to exist, which is what
	/// <see cref="EmissionScope.NeedsUninformedConstructor"/> answers for the base.
	/// </summary>
	public bool UninformedBaseChain { get; set; }
}

/// <summary>
/// One argument a generated constructor forwards to its base, named on both sides. Written as a C#
/// named argument, so a base parameter the subclass does not declare keeps its own default rather than
/// shifting the ones after it along.
/// </summary>
internal sealed class BaseChainArgument
{
	/// <summary>
	/// The base constructor's parameter, as the name of a C# named argument. Escaped, because the
	/// argument name has to be spelled the way the parameter is declared — <c>@object:</c>, not
	/// <c>object:</c>.
	/// </summary>
	public required string ParameterName { get; init; }

	/// <summary>This constructor's own parameter, used to tell which overloads declare it.</summary>
	public required string ArgumentName { get; init; }

	/// <summary>The C# expression written as the argument, usually the parameter itself.</summary>
	public required string Expression { get; init; }
}

/// <summary>Where a class stands relative to emission.</summary>
internal enum ClassScopeStatus : byte
{
	/// <summary>The emitter can produce this class today.</summary>
	Emittable,

	/// <summary>Deliberately not mirrored — renderer plumbing or a math value type.</summary>
	OutOfSurface,

	/// <summary>In the surface, but something about it cannot be mirrored exactly yet.</summary>
	Blocked
}

/// <summary>Whether a three.js name can be used verbatim as a C# identifier.</summary>
internal static class CSharpIdentifier
{
	/// <summary>
	/// The C# reserved keywords, in full rather than only the ones three.js happens to collide with
	/// today (<c>object</c> on the helpers, <c>event</c> and <c>params</c> elsewhere). A partial list
	/// would turn a future upstream rename into a compile error in generated code.
	/// </summary>
	private static readonly IReadOnlySet<string> _reservedWords = new HashSet<string>(StringComparer.Ordinal)
	{
		"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
		"const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
		"explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
		"implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new",
		"null", "object", "operator", "out", "override", "params", "private", "protected", "public",
		"readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static",
		"string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
		"unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
	};

	/// <summary>Prefixes a name with <c>@</c> when it is a C# keyword, so it can be used as an identifier.</summary>
	/// <param name="name">Name to escape.</param>
	/// <returns>The name, escaped if it had to be.</returns>
	public static string Escape(string name)
	{
		return _reservedWords.Contains(name)
			? "@" + name
			: name;
	}

	/// <summary>Tests a name against the identifier rules the generator relies on.</summary>
	/// <param name="name">Name to test.</param>
	/// <returns><see langword="true"/> when the name can be emitted as-is.</returns>
	public static bool IsValid(string name)
	{
		if (name.Length == 0)
		{
			return false;
		}

		if (!char.IsLetter(name[0]) && name[0] != '_')
		{
			return false;
		}

		return name.All(x => char.IsLetterOrDigit(x) || x == '_');
	}
}
