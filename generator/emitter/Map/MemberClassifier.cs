using Blazor.ThreeJS.Emitter.Ir;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// Sorts every declared member of every class into the four buckets the design defines: state the
/// mirror holds and writes through, a command it records, a query that would have to read back from
/// JavaScript, or a skip with a reason.
/// <para>
/// The classification is deliberately independent of whether the declaring class is emitted, so the
/// coverage table can say both "of the API we mirror, this much is state" and "of the whole of
/// three.js, this much is reachable at all".
/// </para>
/// </summary>
internal sealed class MemberClassifier
{
	private readonly TypeMapper _mapper;
	private readonly ClassSurfaceResolver _surfaces;
	private readonly MethodMapper _methods;

	/// <summary>Builds a classifier.</summary>
	/// <param name="surfaces">Resolver that turns a class into the member set it owns.</param>
	/// <param name="mapper">Type mapper.</param>
	/// <param name="methods">Method mapping, shared with the emitter so the two cannot disagree.</param>
	public MemberClassifier(ClassSurfaceResolver surfaces, TypeMapper mapper, MethodMapper methods)
	{
		_mapper = mapper;
		_surfaces = surfaces;
		_methods = methods;
	}

	/// <summary>
	/// Classifies every member one class owns: its own declarations, everything reachable through the
	/// interface three.js merges into it, members folded in from ancestors that have no C# mirror, and
	/// members merged in by a <c>declare module</c> block. Members the mirrored base already carries
	/// are not classified here — they belong to that base.
	/// </summary>
	/// <param name="irClass">Class to classify.</param>
	/// <returns>One row per member, in resolution order.</returns>
	public IReadOnlyList<ClassifiedMember> Classify(IrClass irClass)
	{
		var rows = new List<ClassifiedMember>();
		foreach (var member in _surfaces.Resolve(irClass).Members)
		{
			rows.Add(member.Property is { } property
				? ClassifyProperty(irClass, property, member)
				: ClassifyMethod(irClass, member.Method!, member));
		}

		return rows;
	}

	private ClassifiedMember ClassifyProperty(IrClass irClass, IrProperty property, SurfaceMember member)
	{
		var row = new ClassifiedMember
		{
			ClassName = irClass.Name,
			File = irClass.File,
			MemberName = property.Name,
			MemberKind = ClassifiedMemberKind.Property,
			Origin = member.Origin,
			DeclaringName = member.DeclaringName,
			Property = property,
			Bucket = MemberBucket.Skipped
		};

		if (NotInstanceApiReason(property.Name, property.IsStatic, property.Visibility, property.IsAbstract, property.Doc) is { } notApiReason)
		{
			return Skip(row, notApiReason, SkipCategory.NotInstanceApi);
		}

		var mapping = _mapper.Map(property.Type, new TypeMappingContext
		{
			MemberName = property.Name,
			NumericKind = property.NumericKind,
			TypeParameters = member.TypeParameters
		});

		if (!mapping.IsMapped)
		{
			return Skip(row, mapping.SkipReason!, mapping.SkipCategory);
		}

		row.CSharpTypeName = mapping.CSharpTypeName;
		row.Mapping = mapping;
		if (!property.IsReadonly)
		{
			row.IsWrittenInPlace = mapping.Kind == TypeMappingKind.HandWrittenMathType;
			row.Bucket = MemberBucket.MirroredState;
			return row;
		}

		// three.js declares its math members read-only because you mutate them in place rather than
		// reassign them, and the applier's `assign` copies a decoded math value into the live
		// instance. So a read-only Vector3 really is mirrored state; a read-only anything else is a
		// value C# could neither write nor read back.
		if (mapping.Kind == TypeMappingKind.HandWrittenMathType)
		{
			row.Bucket = MemberBucket.MirroredState;
			row.IsWrittenInPlace = true;
			return row;
		}

		return Skip(
			row,
			$"read-only in three.js (declared `{property.Type?.Text ?? "?"}`), and the read op invokes a method — a property has nothing to route through it, and exposing one as an async method would change the shape of the mirrored API rather than its coverage",
			SkipCategory.ReadOnlyProperty);
	}

	private ClassifiedMember ClassifyMethod(IrClass irClass, IrMethod method, SurfaceMember member)
	{
		var row = new ClassifiedMember
		{
			ClassName = irClass.Name,
			File = irClass.File,
			MemberName = method.Name,
			MemberKind = ClassifiedMemberKind.Method,
			Origin = member.Origin,
			DeclaringName = member.DeclaringName,
			OverloadCount = method.Overloads.Count,
			Bucket = MemberBucket.Skipped
		};

		if (NotInstanceApiReason(method.Name, method.IsStatic, method.Visibility, method.IsAbstract, method.Overloads.FirstOrDefault()?.Doc) is { } notApiReason)
		{
			return Skip(row, notApiReason, SkipCategory.NotInstanceApi);
		}

		var signature = method.Overloads.FirstOrDefault();
		if (signature is null)
		{
			return Skip(row, "the method has no signature in the IR", SkipCategory.UnmappedTypeSyntax);
		}

		var mappedMethod = _methods.Map(method, member.TypeParameters, _mapper);
		if (!mappedMethod.IsMapped)
		{
			return Skip(row, mappedMethod.RefusalReason!, mappedMethod.RefusalCategory);
		}

		row.Method = mappedMethod;
		if (IsVoid(signature.ReturnType))
		{
			row.Bucket = MemberBucket.Command;
			return row;
		}

		if (IsSelfReturn(irClass, member.DeclaringName, signature.ReturnType))
		{
			// A `this`-returning method with arguments is three.js's fluent mutator (`copy(source)`), and
			// recording it as a call op is exact. A `this`-returning method with none has nothing to
			// mutate itself with, so the return value is the whole point of calling it — `clone()` is the
			// case, and it allocates a JavaScript object the wire format has no way to hand back a handle
			// for. Emitting it as a void command would silently create and discard an object.
			if (signature.Parameters.Count > 0)
			{
				row.Bucket = MemberBucket.Command;
				return row;
			}

			return Skip(
				row,
				"takes no arguments and returns its own type, so the return value is the result — it is a JavaScript object, and no op hands back a handle for one the browser created",
				SkipCategory.NoHandleForResult);
		}

		var returnMapping = _mapper.Map(signature.ReturnType, new TypeMappingContext
		{
			MemberName = method.Name,
			NumericKind = signature.ReturnNumericKind,
			TypeParameters = member.TypeParameters
		});

		if (!returnMapping.IsMapped)
		{
			return Skip(row, $"return type: {returnMapping.SkipReason}", returnMapping.SkipCategory);
		}

		if (!IsReadable(returnMapping))
		{
			return Skip(
				row,
				$"returns `{returnMapping.CSharpTypeName}`, a handle-backed object — the read op carries values, and no op mints a handle for an object JavaScript created",
				SkipCategory.NoHandleForResult);
		}

		row.CSharpTypeName = returnMapping.CSharpTypeName;
		row.ReturnMapping = returnMapping;
		row.Bucket = MemberBucket.AsyncQuery;
		return row;
	}

	/// <summary>
	/// Whether a return type can travel back over the read op. The op carries <b>values</b>: a primitive
	/// passes through as itself, an enum as the same numeric backing value the write path already sends,
	/// and one of the five hand-written math types as the same <c>$t</c>-tagged form C# encodes in the
	/// other direction.
	/// <para>
	/// A generated wrapper class cannot: it is mirrored by handle, and no op mints a handle for an
	/// object JavaScript created. Serializing its public shape instead would hand C# a plausible bag of
	/// numbers, which is the one outcome a read must never produce, so <c>three-interop.js</c> refuses
	/// it at runtime too rather than trusting this rule alone.
	/// </para>
	/// </summary>
	/// <param name="returnMapping">The method's resolved return type.</param>
	/// <returns><see langword="true"/> when a value of that type can be read back.</returns>
	private static bool IsReadable(TypeMapping returnMapping)
	{
		return returnMapping.Kind is TypeMappingKind.Primitive or TypeMappingKind.GeneratedEnum or TypeMappingKind.HandWrittenMathType;
	}

	/// <summary>Whether a return type means no value comes back at all.</summary>
	/// <param name="returnType">Declared return type, absent on a signature with none.</param>
	/// <returns><see langword="true"/> for <c>void</c> and <c>undefined</c>.</returns>
	private static bool IsVoid(IrType? returnType)
	{
		return returnType is null || returnType is { Kind: "primitive", Name: "void" or "undefined" };
	}

	/// <summary>
	/// Whether a return type is the declaring type itself — TypeScript's polymorphic <c>this</c>, or a
	/// reference back to the class or interface the member was read from.
	/// </summary>
	/// <param name="irClass">Class being classified.</param>
	/// <param name="declaringName">Class or interface the member was declared on.</param>
	/// <param name="returnType">Declared return type.</param>
	/// <returns><see langword="true"/> when the method returns its own type.</returns>
	private static bool IsSelfReturn(IrClass irClass, string declaringName, IrType? returnType)
	{
		if (returnType is { Kind: "primitive", Name: "this" })
		{
			return true;
		}

		return returnType is { Kind: "reference" } &&
			(string.Equals(returnType.Name, irClass.Name, StringComparison.Ordinal) ||
				string.Equals(returnType.Name, declaringName, StringComparison.Ordinal));
	}

	private static string? NotInstanceApiReason(string name, bool isStatic, string? visibility, bool isAbstract, IrDoc? doc)
	{
		if (isStatic)
		{
			return "static; the mirror models instances, and a static write has no handle to address";
		}

		if (visibility is not null)
		{
			return $"declared `{visibility}`, so it is not part of the public API";
		}

		if (isAbstract)
		{
			return "abstract, so there is no implementation to mirror";
		}

		if (doc?.IsInternal == true)
		{
			return "marked `@internal` upstream, so it is not public API";
		}

		if (!CSharpIdentifier.IsValid(name))
		{
			return "the member name is not a usable C# identifier";
		}

		return null;
	}

	private static ClassifiedMember Skip(ClassifiedMember row, string reason, SkipCategory category)
	{
		row.Bucket = MemberBucket.Skipped;
		row.SkipReason = reason;
		row.SkipCategory = category;
		return row;
	}
}

/// <summary>One classified member.</summary>
internal sealed class ClassifiedMember
{
	/// <summary>Three.js class the member belongs to.</summary>
	public required string ClassName { get; init; }

	/// <summary>Declaring file, relative to the types package root.</summary>
	public required string File { get; init; }

	/// <summary>Member name as three.js spells it.</summary>
	public required string MemberName { get; init; }

	/// <summary>Whether it is a property or a method.</summary>
	public required ClassifiedMemberKind MemberKind { get; init; }

	/// <summary>How the member reached the class it is classified under.</summary>
	public required MemberOrigin Origin { get; init; }

	/// <summary>Class or interface the member was read from, which is not always the class it lands on.</summary>
	public required string DeclaringName { get; init; }

	/// <summary>Which of the four buckets it fell into.</summary>
	public required MemberBucket Bucket { get; set; }

	/// <summary>Resolved C# type of the state, or of the query's result.</summary>
	public string? CSharpTypeName { get; set; }

	/// <summary>The full type mapping behind <see cref="CSharpTypeName"/>, on a mapped member.</summary>
	public TypeMapping? Mapping { get; set; }

	/// <summary>The resolved return type, on a query whose result can be read back.</summary>
	public TypeMapping? ReturnMapping { get; set; }

	/// <summary>The resolved signature, on a method the mapper accepted.</summary>
	public MappedMethod? Method { get; set; }

	/// <summary>The IR declaration behind a property row, read for its documentation and default.</summary>
	public IrProperty? Property { get; init; }

	/// <summary>True for state three.js exposes read-only but the applier writes into in place.</summary>
	public bool IsWrittenInPlace { get; set; }

	/// <summary>Number of overloads, on a method.</summary>
	public int OverloadCount { get; init; }

	/// <summary>Why it was skipped, when it was.</summary>
	public string? SkipReason { get; set; }

	/// <summary>Family the skip reason belongs to.</summary>
	public SkipCategory SkipCategory { get; set; }
}

/// <summary>The four buckets every member falls into.</summary>
internal enum MemberBucket : byte
{
	/// <summary>State the C# object holds and writes through on change.</summary>
	MirroredState,

	/// <summary>A method recorded as a call op and applied on the JavaScript side.</summary>
	Command,

	/// <summary>A method whose result the caller needs back, recorded as a read op and awaited.</summary>
	AsyncQuery,

	/// <summary>Not mirrored; <see cref="ClassifiedMember.SkipReason"/> says why.</summary>
	Skipped
}

/// <summary>Whether a classified member is a property or a method.</summary>
internal enum ClassifiedMemberKind : byte
{
	/// <summary>A property or accessor.</summary>
	Property,

	/// <summary>A method.</summary>
	Method
}

/// <summary>Where a member came from.</summary>
internal enum MemberOrigin : byte
{
	/// <summary>Declared on the class itself.</summary>
	Declared,

	/// <summary>
	/// Reached through the interface three.js declaration-merges into the class, or one that interface
	/// extends. This is where every material property lives.
	/// </summary>
	InterfaceInheritance,

	/// <summary>
	/// Declared on an ancestor that has no C# mirror of its own — the abstract <c>Light</c>, say — so it
	/// is folded into this class rather than lost with the ancestor.
	/// </summary>
	FlattenedAncestor,

	/// <summary>Merged in by a <c>declare module</c> block elsewhere in the package.</summary>
	ModuleAugmentation
}
