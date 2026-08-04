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
		NumericResolution? numeric = null)
	{
		return new TypeMapping
		{
			CSharpTypeName = cSharpTypeName,
			Kind = kind,
			RequiredGeneratedTypeName = requiredGeneratedTypeName,
			IsExplicitlyNullable = isExplicitlyNullable,
			Numeric = numeric
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

	/// <summary>An array or tuple. The wire encoder has no array arm.</summary>
	CollectionType,

	/// <summary>A JavaScript callback. The wire format carries no callback channel.</summary>
	CallbackType,

	/// <summary>A group of string-valued constants, which a C# enum cannot carry over this wire format.</summary>
	StringConstantGroup,

	/// <summary>A type alias that is neither a constant group nor a known special case.</summary>
	UnmappedTypeAlias,

	/// <summary>A union of more than one real alternative, which C# cannot express as one parameter.</summary>
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

	/// <summary>Read-only in three.js, and the wire format has no read channel.</summary>
	ReadOnlyWithoutReadChannel,

	/// <summary>A rest parameter, including the rest-union-tuple pseudo-overload form.</summary>
	RestParameter
}
