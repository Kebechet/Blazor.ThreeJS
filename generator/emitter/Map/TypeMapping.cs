using Blazor.ThreeJS.Emitter.Emit;

namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// The outcome of resolving one IR type reference. Either it maps onto a concrete C# type, or it is
/// skipped and carries the reason why — there is deliberately no third state, so nothing can reach
/// generated code without either a mapping or a recorded justification.
/// </summary>
internal sealed class TypeMapping
{
	/// <summary>Whether the reference resolved onto a C# type.</summary>
	public bool IsMapped
	{
		get { return CSharpTypeName is not null; }
	}

	/// <summary>Resolved C# type name, without any nullable annotation.</summary>
	public string? CSharpTypeName { get; init; }

	/// <summary>What the resolved type is, and therefore what has to exist for it to compile.</summary>
	public required TypeMappingKind Kind { get; init; }

	/// <summary>
	/// True when the declared type admits <c>null</c> in three.js itself (<c>Material | null</c>), as
	/// opposed to a C# optional parameter that happens to default to null.
	/// </summary>
	public bool IsExplicitlyNullable { get; init; }

	/// <summary>Type that must be generated for this mapping to compile: a wrapped class or an enum.</summary>
	public string? RequiredGeneratedTypeName { get; init; }

	/// <summary>Why the reference was skipped, in a form a reader of the coverage table can act on.</summary>
	public string? SkipReason { get; init; }

	/// <summary>Which family of unsupported thing this was, for grouping the coverage table.</summary>
	public SkipCategory SkipCategory { get; init; }

	/// <summary>How the numeric type was chosen, on a <c>number</c> mapping.</summary>
	public NumericResolution? Numeric { get; init; }

	/// <summary>
	/// On a <see cref="TypeMappingKind.Sequence"/>, how the element type resolved. An array is only as
	/// expressible as its elements, and readability in particular does not follow from the array: an
	/// array of handle-backed objects can be sent but not read back.
	/// </summary>
	public TypeMapping? ElementMapping { get; init; }

	/// <summary>Builds a successful mapping.</summary>
	/// <param name="cSharpTypeName">Resolved C# type name.</param>
	/// <param name="kind">What the resolved type is.</param>
	/// <param name="requiredGeneratedTypeName">Type that has to be generated alongside, if any.</param>
	/// <param name="isExplicitlyNullable">Whether three.js itself admits null here.</param>
	/// <param name="numeric">Numeric basis, on a <c>number</c> mapping.</param>
	/// <returns>The mapping.</returns>
	public static TypeMapping Mapped(
		string cSharpTypeName,
		TypeMappingKind kind,
		string? requiredGeneratedTypeName = null,
		bool isExplicitlyNullable = false,
		NumericResolution? numeric = null,
		TypeMapping? elementMapping = null)
	{
		return new TypeMapping
		{
			CSharpTypeName = cSharpTypeName,
			Kind = kind,
			RequiredGeneratedTypeName = requiredGeneratedTypeName,
			IsExplicitlyNullable = isExplicitlyNullable,
			Numeric = numeric,
			ElementMapping = elementMapping
		};
	}

	/// <summary>Builds a skip.</summary>
	/// <param name="category">Family of unsupported thing.</param>
	/// <param name="reason">Why it cannot be mirrored, stated concretely.</param>
	/// <returns>The mapping.</returns>
	public static TypeMapping Skipped(SkipCategory category, string reason)
	{
		return new TypeMapping
		{
			Kind = TypeMappingKind.Skipped,
			SkipCategory = category,
			SkipReason = reason
		};
	}
}

/// <summary>
/// What one parameter's declared type resolves to: the C# types it is emitted as, and the arms it
/// lost on the way. The two travel together because a caller of
/// <see cref="TypeMapper.MapAlternatives"/> owes a record of the second whenever it uses the first.
/// </summary>
internal sealed class TypeAlternatives
{
	/// <summary>One mapping per emitted arm, in declaration order. Never empty.</summary>
	public required IReadOnlyList<TypeMapping> Arms { get; init; }

	/// <summary>
	/// Arms of the declared union the mirror could not express, each with the obstacle. Empty for
	/// everything but a union that reached C# as an overload set with fewer arms than it was declared
	/// with.
	/// </summary>
	public IReadOnlyList<DroppedAlternative> DroppedArms { get; init; } = [];
}

/// <summary>One arm of a declared union that no emitted overload stands for, and why.</summary>
internal sealed class DroppedAlternative
{
	/// <summary>The arm as three.js declares it.</summary>
	public required string TypeText { get; init; }

	/// <summary>Why it could not be mapped.</summary>
	public required string Reason { get; init; }

	/// <summary>Family the reason belongs to.</summary>
	public required SkipCategory Category { get; init; }
}

/// <summary>The five things an IR type reference is allowed to resolve to.</summary>
internal enum TypeMappingKind : byte
{
	/// <summary>A C# primitive: <c>float</c>, <c>int</c>, <c>bool</c>, <c>string</c>, <c>void</c>.</summary>
	Primitive,

	/// <summary>One of the five hand-ported math types, which are never regenerated.</summary>
	HandWrittenMathType,

	/// <summary>A generated wrapper class, referenced by handle on the wire.</summary>
	GeneratedWrapperClass,

	/// <summary>A generated enum over three.js's numeric constants.</summary>
	GeneratedEnum,

	/// <summary>
	/// One of the hand-written typed arrays. Encoded by value like a math type, but not owned or
	/// mutated in place: three.js reassigns a typed array rather than writing into it, so there is no
	/// live instance for the mirror to hang a change callback off.
	/// </summary>
	HandWrittenTypedArray,

	/// <summary>
	/// A C# array. Encoded element by element, so what it can carry — and whether it can be read back
	/// at all — is decided by <see cref="TypeMapping.ElementMapping"/> rather than by the array itself.
	/// </summary>
	Sequence,

	/// <summary>Nothing in C# mirrors it; <see cref="TypeMapping.SkipReason"/> says what and why.</summary>
	Skipped
}

/// <summary>
/// Why something could not be mirrored. These are the rows of the coverage table's
/// "what is not covered" half, so each one has to name a concrete obstacle rather than say
/// "unsupported".
/// </summary>
internal enum SkipCategory : byte
{
	/// <summary>Not skipped.</summary>
	None,

	/// <summary>A TypeScript lib or DOM type. C# cannot hold a browser object and the wire has no encoding for one.</summary>
	DomOrLibType,

	/// <summary>Declared under <c>src/nodes/**</c>, the TSL / WebGPU node stack, outside the extracted surface.</summary>
	NodeStackType,

	/// <summary>An options / parameters bag passed to a constructor. Its fields are settable properties instead.</summary>
	OptionsInterface,

	/// <summary>A three.js math value type with no C# mirror; only five are hand-ported.</summary>
	MathValueType,

	/// <summary>
	/// A tuple, which has no wire encoding, or an array whose elements have none. The encoder does walk
	/// a sequence element by element, so an array itself is not the obstacle — what is in it is.
	/// </summary>
	CollectionType,

	/// <summary>A JavaScript callback. The wire format carries no callback channel.</summary>
	CallbackType,

	/// <summary>A group of string-valued constants, which a C# enum cannot carry over this wire format.</summary>
	StringConstantGroup,

	/// <summary>A type alias that is neither a constant group nor a known special case.</summary>
	UnmappedTypeAlias,

	/// <summary>
	/// A union of more than one real alternative in a position that holds one type. A required parameter
	/// takes one overload per arm instead; a property or a return type has nowhere to put the second.
	/// </summary>
	UnmappedUnion,

	/// <summary>A TypeScript type-syntax form with no C# equivalent (conditional, mapped, indexed access…).</summary>
	UnmappedTypeSyntax,

	/// <summary>A literal type — three.js's <c>isMesh: true</c> runtime type tags — which C# has only inside an enum.</summary>
	LiteralType,

	/// <summary>An anonymous object literal type, which has no name to give a C# type.</summary>
	AnonymousObjectType,

	/// <summary>Declared <c>any</c> / <c>unknown</c>, or with no type at all, so there is nothing to express.</summary>
	UntypedValue,

	/// <summary>The class is abstract, so it has no constructor to mirror.</summary>
	AbstractClass,

	/// <summary>The class declares more than one constructor, and C# overload emission is not implemented.</summary>
	ConstructorOverloads,

	/// <summary>The class is declared but never exported, so the applier cannot reach it on <c>THREE</c>.</summary>
	NotExported,

	/// <summary>The types export it, but the shipped three.js bundle has no such runtime value to construct.</summary>
	AbsentFromShippedBundle,

	/// <summary>Two classes share a name, and a C# namespace holds one type of a given name.</summary>
	DuplicateClassName,

	/// <summary>A required parameter follows an optional one, which C# forbids.</summary>
	RequiredAfterOptional,

	/// <summary>A type parameter with neither a default nor a constraint, so erasure has nothing to erase to.</summary>
	UnerasableTypeParameter,

	/// <summary>An in-scope class that is itself not emitted, so there is no C# type to reference.</summary>
	UnwrappedClass,

	/// <summary>Declared in another package, or elsewhere in <c>@types/three</c> outside the scanned surface.</summary>
	ExternalType,

	/// <summary>The TypeScript checker could not resolve the name at all.</summary>
	UnresolvedType,

	/// <summary>The member is not part of the mirrored surface at all: static, non-public, or <c>@internal</c>.</summary>
	NotInstanceApi,

	/// <summary>
	/// Its result is neither a value the read and get ops carry — numbers, booleans, strings, the
	/// tagged math types — nor a single object a handle could name. An op marked <c>n:true</c> mints a
	/// handle for one object the browser answered with; an array of them would need a handle per
	/// element, which nothing in the wire format describes.
	/// </summary>
	NoHandleForResult,

	/// <summary>A member the constructor already takes under the same name, so the two would collide.</summary>
	ShadowedByConstructorParameter,

	/// <summary>The package provides the class by hand, so the generator does not emit it.</summary>
	HandWritten,

	/// <summary>Its C# base requires constructor arguments the generated class has nothing to supply.</summary>
	UnreachableBaseConstructor,

	/// <summary>A rest parameter, including the rest-union-tuple pseudo-overload form.</summary>
	RestParameter
}
