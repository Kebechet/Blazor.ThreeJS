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
			IsStatic = property.IsStatic,
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
			// A read-only property holding a three.js class the generator does not mirror still has a
			// value worth handing back: the get op registers the object and answers with a handle, which
			// becomes an untyped `Primitive`. A writable one stays skipped — a setter needs an encodable
			// value, and a handle the mirror cannot type is not one C# could construct to write.
			if (property.IsReadonly && IsThreeObjectShape(property.Type))
			{
				row.Bucket = MemberBucket.AsyncQuery;
				row.IsPropertyRead = true;
				row.IsUntypedObjectResult = true;
				return row;
			}

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

		// Read-only and not a math value: C# cannot hold it as mirrored state, because three.js is the
		// only side that ever assigns it. The get op reads a property directly, so it can still be read
		// back on demand — as a query rather than a property, since the value is three.js's to change
		// and a C# property would imply the mirror knew it without asking.
		if (!IsReadable(mapping))
		{
			// A three.js object comes back by handle rather than by value: the get op marked `n:true` asks
			// the applier to register it and answer with a reference. Where a mirrored class describes it
			// the read adopts it into that class; where none does, the untyped wrapper carries it; and an
			// array of objects is neither, because one handle cannot name several objects.
			if (mapping.Kind != TypeMappingKind.GeneratedWrapperClass)
			{
				if (IsThreeObjectShape(property.Type))
				{
					row.Bucket = MemberBucket.AsyncQuery;
					row.IsPropertyRead = true;
					row.IsUntypedObjectResult = true;
					return row;
				}

				return Skip(
					row,
					$"read-only in three.js and typed `{property.Type?.Text ?? "?"}`, which is neither a value the get op carries nor a three.js object a handle could name",
					SkipCategory.NoHandleForResult);
			}

			row.IsAdoptedResult = true;
		}

		row.Bucket = MemberBucket.AsyncQuery;
		row.IsPropertyRead = true;
		return row;
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
			IsStatic = method.IsStatic,
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

		// A promise-returning method is read through its awaited type from here on. three.js's WebGPU
		// renderer resolves its `*Async` methods when the GPU is done rather than when the call returns,
		// so what the caller wants back is what the promise settles to.
		var returnType = UnwrapAwaited(signature.ReturnType, out var isAwaited);
		if (IsVoid(returnType))
		{
			// An awaited void still has to be a query. `clearAsync` answers nothing, but it answers
			// *later*, and recording it as a fire-and-forget call op would take away the only thing its
			// name promises: a point at which the work has finished.
			if (isAwaited)
			{
				row.IsAwaitedVoidResult = true;
				row.Bucket = MemberBucket.AsyncQuery;
				return row;
			}

			row.Bucket = MemberBucket.Command;
			return row;
		}

		if (IsSelfReturn(irClass, member.DeclaringName, returnType))
		{
			// A `this`-returning method with arguments is three.js's fluent mutator (`copy(source)`), and
			// recording it as a call op is exact. A `this`-returning method with none has nothing to
			// mutate itself with, so the return value is the whole point of calling it.
			//
			// Except `clone`, which takes an argument and still allocates: `Object3D.clone(recursive)` is
			// the one signature in the snapshot where both are true. Recording it as a call op would build
			// a three.js object and drop the only reference to it, behind a C# method whose own upstream
			// documentation promises a return value. The parameter count cannot tell the two apart, so the
			// name is what does — see IsAllocatingSelfReturn.
			if (signature.Parameters.Count > 0 && !IsAllocatingSelfReturn(method.Name))
			{
				row.Bucket = MemberBucket.Command;
				return row;
			}

			// The return value is the result, so it has to come back. Which object it actually is stays
			// the applier's call rather than this one's: `clone` allocates and `BufferGeometry.center`
			// returns the receiver, and both say `this`. An object the applier already has a handle for
			// answers with that handle, and only a genuinely new one is registered - so a
			// receiver-returning method costs no second mirror of one object.
			row.CSharpTypeName = irClass.Name;
			row.IsAdoptedResult = true;
			row.Bucket = MemberBucket.AsyncQuery;
			return row;
		}

		var returnMapping = _mapper.Map(returnType, new TypeMappingContext
		{
			MemberName = method.Name,
			NumericKind = signature.ReturnNumericKind,
			TypeParameters = member.TypeParameters
		});

		if (!returnMapping.IsMapped)
		{
			// The mapper refused the return type, but a three.js class is still an object the read op can
			// register and answer with a handle for. The parameters all mapped — the method mapper's own
			// refusal above is still a skip, because an argument has to be encodable to be sent at all.
			if (IsThreeObjectShape(returnType))
			{
				row.IsUntypedObjectResult = true;
				row.Bucket = MemberBucket.AsyncQuery;
				return row;
			}

			return Skip(row, $"return type: {returnMapping.SkipReason}", returnMapping.SkipCategory);
		}

		if (!IsReadable(returnMapping))
		{
			// A mirrored class comes back by handle rather than by value: the applier registers the
			// object and answers with a reference, which the read adopts into the declared C# type.
			// Anything else genuinely has nowhere to go.
			if (returnMapping.Kind != TypeMappingKind.GeneratedWrapperClass)
			{
				if (IsThreeObjectShape(returnType))
				{
					row.IsUntypedObjectResult = true;
					row.Bucket = MemberBucket.AsyncQuery;
					return row;
				}

				return Skip(
					row,
					$"returns `{returnMapping.CSharpTypeName}`, which is neither a value the read op carries nor a three.js object a handle could name",
					SkipCategory.NoHandleForResult);
			}

			row.IsAdoptedResult = true;
		}

		row.CSharpTypeName = returnMapping.CSharpTypeName;
		row.ReturnMapping = returnMapping;
		row.Bucket = MemberBucket.AsyncQuery;
		return row;
	}

	/// <summary>
	/// Whether a return type can travel back over the read op. The op carries <b>values</b>: a primitive
	/// passes through as itself, an enum as the same numeric backing value the write path already sends,
	/// a hand-written math type as the same <c>$t</c>-tagged form C# encodes in the other direction, and
	/// a typed array as its <c>$ta</c>-tagged components.
	/// <para>
	/// A generated wrapper class cannot: it is mirrored by handle, and serializing its public shape
	/// would hand C# a plausible bag of numbers, which is the one outcome a read must never produce —
	/// so <c>three-interop.js</c> refuses it at runtime too rather than trusting this rule alone. That
	/// is not the end of it: an op marked <c>n:true</c> asks the applier to register the object and
	/// answer with a reference, which is the channel every object-valued query here travels on. This
	/// predicate decides only whether the <b>value</b> channel will carry the result.
	/// </para>
	/// <para>
	/// An array is readable exactly when its elements are, which is not the same as being sendable: the
	/// encoder writes an array of handles happily, but reading one back would need a handle minted for
	/// every element.
	/// </para>
	/// </summary>
	/// <param name="returnMapping">The method's resolved return type.</param>
	/// <returns><see langword="true"/> when a value of that type can be read back.</returns>
	private static bool IsReadable(TypeMapping returnMapping)
	{
		if (returnMapping.Kind == TypeMappingKind.Sequence)
		{
			return returnMapping.ElementMapping is { } element && IsReadable(element);
		}

		return returnMapping.Kind is
			TypeMappingKind.Primitive or
			TypeMappingKind.GeneratedEnum or
			TypeMappingKind.HandWrittenMathType or
			TypeMappingKind.HandWrittenTypedArray;
	}

	/// <summary>
	/// Whether a declared type names a three.js <b>object</b> — something the applier can register under
	/// a handle and answer a <c>$ref</c> for, even where no C# type mirrors it. Read off the declaration
	/// rather than off the mapping, because this is asked exactly where the mapping failed.
	/// <para>
	/// ⚠️ A math class never qualifies, and neither does anything declared under
	/// <see cref="EmitterConfig.MathSourcePrefix"/>. The applier answers a math value as a tagged value
	/// rather than as a reference, so a member declared to return one would decode a
	/// <c>ThreeObjectReference</c> out of a <c>$t</c>-tagged tuple and fault. The exclusion is
	/// correctness, not taste.
	/// </para>
	/// </summary>
	/// <param name="type">Declared type, absent on a signature with none.</param>
	/// <returns><see langword="true"/> when a value of that type is a handle-able object.</returns>
	private static bool IsThreeObjectShape(IrType? type)
	{
		if (type is null)
		{
			return false;
		}

		if (type.Kind == "reference")
		{
			return IsClassArm(type);
		}

		// `Foo | null` is one object or none, which is what a nullable result already means. A union of
		// several classes qualifies too, and only here: a typed result would have to pick one arm, but
		// every arm answers as the same untyped wrapper, so nothing is narrowed by taking them together.
		if (type.Kind == "union")
		{
			var arms = type.Types ?? [];
			return arms.Any(IsClassArm) && arms.All(x => IsClassArm(x) || IsNullishArm(x));
		}

		return false;
	}

	/// <summary>Whether one union arm is an in-scope three.js class that is not a math value.</summary>
	/// <param name="type">The arm.</param>
	/// <returns><see langword="true"/> when it names such a class.</returns>
	private static bool IsClassArm(IrType type)
	{
		return type is { Kind: "reference", Target: { RefKind: "class", Origin: "in-scope" } }
			&& !EmitterConfig.MathTypeNames.Contains(type.Name ?? string.Empty)
			&& type.Target.File?.StartsWith(EmitterConfig.MathSourcePrefix, StringComparison.Ordinal) != true;
	}

	/// <summary>Whether one union arm is the absence of a value rather than an alternative to it.</summary>
	/// <param name="type">The arm.</param>
	/// <returns><see langword="true"/> for <c>null</c> and <c>undefined</c>.</returns>
	private static bool IsNullishArm(IrType type)
	{
		return type is { Kind: "primitive", Name: "null" or "undefined" };
	}

	/// <summary>
	/// The type a promise-returning method actually answers with, and whether it was one.
	/// <para>
	/// A read op already answers on a <c>Task</c>, so a <c>Promise&lt;T&gt;</c> and a <c>T</c> reach the
	/// caller as the same C# signature; what differs is when. The applier waits for the promise before
	/// filling in the answer, which is what makes <c>readRenderTargetPixelsAsync</c> hand back pixels
	/// that have actually been read rather than a handle to a pending Promise.
	/// </para>
	/// <para>
	/// Only the outer <c>Promise</c> is removed, and only once: a nested one would be three.js
	/// promising a promise, which nothing in the snapshot does, and unwrapping to a fixpoint would
	/// quietly flatten it if it ever did.
	/// </para>
	/// </summary>
	/// <param name="returnType">Declared return type, absent on a signature with none.</param>
	/// <param name="isAwaited">Whether the declared type was a promise.</param>
	/// <returns>The awaited type, or the declared one unchanged.</returns>
	private static IrType? UnwrapAwaited(IrType? returnType, out bool isAwaited)
	{
		if (returnType is { Kind: "reference", Name: "Promise", TypeArguments: [{ } awaited] })
		{
			isAwaited = true;
			return awaited;
		}

		isAwaited = false;
		return returnType;
	}

	/// <summary>Whether a return type means no value comes back at all.</summary>
	/// <param name="returnType">Declared return type, absent on a signature with none.</param>
	/// <returns><see langword="true"/> for <c>void</c> and <c>undefined</c>.</returns>
	private static bool IsVoid(IrType? returnType)
	{
		return returnType is null || returnType is { Kind: "primitive", Name: "void" or "undefined" };
	}

	/// <summary>
	/// Whether a self-returning method builds a new object rather than mutating and returning the
	/// receiver, and therefore has to hand its result back however many arguments it takes.
	/// <para>
	/// ⚠️ Read off the name, which is the only thing that knows. The declaration of
	/// <c>clone(recursive?: boolean): this</c> and of <c>copy(source: T): this</c> are the same shape,
	/// and only one of them allocates; TypeScript has no way to say which. <c>clone</c> is three.js's
	/// universal spelling for the allocating one — 57 members in the current snapshot, every one of
	/// them returning a fresh object — so matching the name is exact here rather than a heuristic.
	/// </para>
	/// <para>
	/// Deliberately just this one name. Widening it to <c>toX</c>, <c>getX</c> or anything else would be
	/// guessing at semantics the declaration does not carry, and the cost of guessing wrong is a query
	/// that mints a handle for the receiver on every call.
	/// </para>
	/// </summary>
	/// <param name="name">Three.js method name.</param>
	/// <returns><see langword="true"/> when the method allocates what it returns.</returns>
	private static bool IsAllocatingSelfReturn(string name)
	{
		return string.Equals(name, "clone", StringComparison.Ordinal);
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

	/// <summary>
	/// True when three.js declares the member <c>static</c>. Recorded because
	/// <see cref="SkipCategory.NotInstanceApi"/> covers the protected and private members too, and a
	/// report that called the whole bucket static would overstate how much of it is out of reach for
	/// want of a handle rather than for want of being public.
	/// </summary>
	public bool IsStatic { get; init; }

	/// <summary>True for state three.js exposes read-only but the applier writes into in place.</summary>
	public bool IsWrittenInPlace { get; set; }

	/// <summary>
	/// True when an <see cref="MemberBucket.AsyncQuery"/> row reads a property rather than invoking a
	/// method, which is the difference between the get op and the read op. Both answer with a value on
	/// the same correlated channel, so only the op kind and the absence of arguments differ.
	/// </summary>
	public bool IsPropertyRead { get; set; }

	/// <summary>
	/// True when an <see cref="MemberBucket.AsyncQuery"/> row answers with a handle the mirror adopts
	/// rather than with a value. The result is a three.js object, which has no wire form of its own.
	/// </summary>
	public bool IsAdoptedResult { get; set; }

	/// <summary>
	/// True when an <see cref="MemberBucket.AsyncQuery"/> row answers with a three.js object that has no
	/// mirrored C# type to be. The handle still comes back, so the object is reachable — as an untyped
	/// <c>Primitive</c> whose members are named the way three.js names them.
	/// </summary>
	public bool IsUntypedObjectResult { get; set; }

	/// <summary>
	/// Whether this query answers nothing, and exists only so the caller can wait for the work to
	/// finish. Emitted as a bare <c>Task</c>: three.js's <c>clearAsync</c> and friends resolve when the
	/// GPU is done, and that instant is the whole value of calling them.
	/// </summary>
	public bool IsAwaitedVoidResult { get; set; }

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
