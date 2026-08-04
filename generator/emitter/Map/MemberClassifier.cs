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
	private readonly Dictionary<string, List<IrAugmentedDeclaration>> _augmentationsByTargetName;

	/// <summary>Builds a classifier.</summary>
	/// <param name="ir">The parsed IR, read for its module augmentations.</param>
	/// <param name="mapper">Type mapper.</param>
	public MemberClassifier(IrRoot ir, TypeMapper mapper)
	{
		_mapper = mapper;
		_augmentationsByTargetName = [];
		foreach (var augmentation in ir.ModuleAugmentations)
		{
			foreach (var augmented in augmentation.Augments)
			{
				if (!_augmentationsByTargetName.TryGetValue(augmented.Name, out var list))
				{
					list = [];
					_augmentationsByTargetName[augmented.Name] = list;
				}

				list.Add(augmented);
			}
		}
	}

	/// <summary>
	/// Classifies every member of one class, including members merged in by a module augmentation.
	/// A class's real member set is its own declaration plus every <c>declare module</c> block
	/// targeting it, and dropping those silently would put the mirror out of step with the runtime.
	/// </summary>
	/// <param name="irClass">Class to classify.</param>
	/// <returns>One row per member, in declaration order, declared members first.</returns>
	public IReadOnlyList<ClassifiedMember> Classify(IrClass irClass)
	{
		var rows = new List<ClassifiedMember>();
		foreach (var property in irClass.Properties)
		{
			rows.Add(ClassifyProperty(irClass, property, MemberOrigin.Declared));
		}

		foreach (var method in irClass.Methods)
		{
			rows.Add(ClassifyMethod(irClass, method, MemberOrigin.Declared));
		}

		if (!_augmentationsByTargetName.TryGetValue(irClass.Name, out var augmentations))
		{
			return rows;
		}

		foreach (var augmented in augmentations)
		{
			foreach (var property in augmented.Properties)
			{
				rows.Add(ClassifyProperty(irClass, property, MemberOrigin.ModuleAugmentation));
			}

			foreach (var method in augmented.Methods)
			{
				rows.Add(ClassifyMethod(irClass, method, MemberOrigin.ModuleAugmentation));
			}
		}

		return rows;
	}

	private ClassifiedMember ClassifyProperty(IrClass irClass, IrProperty property, MemberOrigin origin)
	{
		var row = new ClassifiedMember
		{
			ClassName = irClass.Name,
			File = irClass.File,
			MemberName = property.Name,
			MemberKind = ClassifiedMemberKind.Property,
			Origin = origin,
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
			TypeParameters = irClass.TypeParameters
		});

		if (!mapping.IsMapped)
		{
			return Skip(row, mapping.SkipReason!, mapping.SkipCategory);
		}

		row.CSharpTypeName = mapping.CSharpTypeName;
		if (!property.IsReadonly)
		{
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
			$"read-only in three.js (declared `{property.Type?.Text ?? "?"}`), and the wire format has no read op — C# could neither write it nor observe it",
			SkipCategory.ReadOnlyWithoutReadChannel);
	}

	private ClassifiedMember ClassifyMethod(IrClass irClass, IrMethod method, MemberOrigin origin)
	{
		var row = new ClassifiedMember
		{
			ClassName = irClass.Name,
			File = irClass.File,
			MemberName = method.Name,
			MemberKind = ClassifiedMemberKind.Method,
			Origin = origin,
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

		foreach (var parameter in signature.Parameters)
		{
			if (parameter.IsRest)
			{
				var isPseudoOverload = parameter.Type is { Kind: "union" } union &&
					union.Types.All(x => x.Kind == "tuple");

				return Skip(
					row,
					isPseudoOverload
						? $"parameter '{parameter.Name}' is a rest-union-tuple pseudo-overload (`{parameter.Type!.Text}`), which is one TypeScript signature standing for several C# overloads"
						: $"parameter '{parameter.Name}' is a rest parameter (`{parameter.Type?.Text ?? "?"}`)",
					SkipCategory.RestParameter);
			}

			var parameterMapping = _mapper.Map(parameter.Type, new TypeMappingContext
			{
				MemberName = parameter.Name,
				NumericKind = parameter.NumericKind,
				TypeParameters = irClass.TypeParameters
			});

			if (!parameterMapping.IsMapped)
			{
				return Skip(row, $"parameter '{parameter.Name}': {parameterMapping.SkipReason}", parameterMapping.SkipCategory);
			}
		}

		if (IsFluentOrVoid(irClass, signature.ReturnType))
		{
			row.Bucket = MemberBucket.Command;
			return row;
		}

		var returnMapping = _mapper.Map(signature.ReturnType, new TypeMappingContext
		{
			MemberName = method.Name,
			NumericKind = signature.ReturnNumericKind,
			TypeParameters = irClass.TypeParameters
		});

		if (!returnMapping.IsMapped)
		{
			return Skip(row, $"return type: {returnMapping.SkipReason}", returnMapping.SkipCategory);
		}

		row.CSharpTypeName = returnMapping.CSharpTypeName;
		row.Bucket = MemberBucket.AsyncQuery;
		return row;
	}

	/// <summary>
	/// Whether a return type means "no value comes back": <c>void</c>, or the fluent
	/// <c>this</c> / declaring-class self-return three.js uses for chaining.
	/// </summary>
	private static bool IsFluentOrVoid(IrClass irClass, IrType? returnType)
	{
		if (returnType is null)
		{
			return true;
		}

		if (returnType is { Kind: "primitive", Name: "void" or "undefined" or "this" })
		{
			return true;
		}

		return returnType is { Kind: "reference" } && string.Equals(returnType.Name, irClass.Name, StringComparison.Ordinal);
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

	/// <summary>Whether it was declared on the class or merged in by a module augmentation.</summary>
	public required MemberOrigin Origin { get; init; }

	/// <summary>Which of the four buckets it fell into.</summary>
	public required MemberBucket Bucket { get; set; }

	/// <summary>Resolved C# type of the state, or of the query's result.</summary>
	public string? CSharpTypeName { get; set; }

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

	/// <summary>A method whose result the caller needs back, which needs a read op the wire format does not have.</summary>
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

	/// <summary>Merged in by a <c>declare module</c> block elsewhere in the package.</summary>
	ModuleAugmentation
}
