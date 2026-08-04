using System.Text.Json.Serialization;

namespace Blazor.ThreeJS.Emitter.Ir;

/// <summary>
/// The subset of <c>generator/three-api.json</c> the emitter reads. Shapes follow
/// <c>generator/IR-SCHEMA.md</c>; every optional field is nullable or defaults to an empty
/// collection, because the IR omits empty values rather than writing <c>null</c>.
/// </summary>
internal sealed class IrRoot
{
	/// <summary>Provenance of the snapshot, reproduced in the header of every generated file.</summary>
	public IrMeta? Meta { get; set; }

	/// <summary>Every class in scope. Not unique by name — key by <c>Name</c> plus <c>File</c>.</summary>
	public List<IrClass> Classes { get; set; } = [];

	/// <summary>Declared interfaces, most of them the <c>*Parameters</c> / <c>*Options</c> bags a constructor takes.</summary>
	public List<IrInterface> Interfaces { get; set; } = [];

	/// <summary>Real TypeScript <c>enum</c> declarations, as opposed to the loose constants below.</summary>
	public List<IrEnum> Enums { get; set; } = [];

	/// <summary>Loose <c>export const X: &lt;literal&gt;</c> declarations, the raw material of the generated enums.</summary>
	public List<IrConstant> Constants { get; set; } = [];

	/// <summary>Type aliases; the ones carrying a <c>constantGroup</c> are what turns loose constants into enums.</summary>
	public List<IrTypeAlias> TypeAliases { get; set; } = [];

	/// <summary>
	/// <c>declare module "…"</c> declaration merging. A class's real member set is its own entry plus
	/// any augmentation targeting it, so these are merged rather than ignored.
	/// </summary>
	public List<IrModuleAugmentation> ModuleAugmentations { get; set; } = [];
}

/// <summary>Provenance block of the IR.</summary>
internal sealed class IrMeta
{
	/// <summary>The npm package the API surface was read from, e.g. <c>@types/three</c>.</summary>
	public string? TypesPackage { get; set; }

	/// <summary>Exact pinned version of <see cref="TypesPackage"/>.</summary>
	public string? TypesVersion { get; set; }

	/// <summary>Directory inside the types package the snapshot was taken from, e.g. <c>src</c>.</summary>
	public string? SourceRoot { get; set; }

	/// <summary>How much of each kind of declaration the snapshot holds.</summary>
	public IrCounts? Counts { get; set; }

	/// <summary>
	/// Directories under <c>src/</c> the extractor never parsed, counted rather than asserted so the
	/// coverage table can state how large each exclusion is.
	/// </summary>
	public List<IrExcludedDirectory> ExcludedDirectories { get; set; } = [];

	/// <summary>The addon modules, which live outside <c>src/</c> and are never extracted.</summary>
	public IrExcludedDirectory? Addons { get; set; }

	/// <summary>How the extractor resolved what three.js publishes, and what the shipped bundle carries.</summary>
	public IrPublicSurface? PublicSurface { get; set; }
}

/// <summary>
/// The barrel graph the extractor walked and the bundle it checked the result against — the two
/// facts behind <see cref="IrClass.IsExported"/> and <see cref="IrClass.IsRuntimeExport"/>.
/// </summary>
internal sealed class IrPublicSurface
{
	/// <summary>The barrel file the walk started from, e.g. <c>src/Three.d.ts</c>.</summary>
	public string? Barrel { get; set; }

	/// <summary>Repository-relative path of the three.js bundle the package ships and looks names up on.</summary>
	public string? RuntimeBundle { get; set; }
}

/// <summary>
/// Declaration counts for the snapshot. Only the ones a report quotes are modelled; the rest of the
/// block is ignored by the reader, as everywhere else in this model.
/// </summary>
internal sealed class IrCounts
{
	/// <summary>
	/// Top-level exported functions. Not classes, and not wrapped — quoted so the coverage table can
	/// say so, because conflating the two is what once made the class total look like 374.
	/// </summary>
	public int Functions { get; set; }
}

/// <summary>A directory of declarations deliberately left out of the snapshot, with its size.</summary>
internal sealed class IrExcludedDirectory
{
	/// <summary>POSIX path relative to the types package root.</summary>
	public required string Path { get; set; }

	/// <summary>How many <c>.d.ts</c> files it holds.</summary>
	public int Files { get; set; }

	/// <summary>How many class declarations it holds.</summary>
	public int Classes { get; set; }
}

/// <summary>A single three.js class declaration.</summary>
internal sealed class IrClass
{
	/// <summary>Declared class name, e.g. <c>BoxGeometry</c>.</summary>
	public required string Name { get; set; }

	/// <summary>POSIX path of the declaring file, relative to the types package root.</summary>
	public required string File { get; set; }

	/// <summary>
	/// Whether three.js's public barrel (<c>src/Three.d.ts</c>) re-exports the class as a value. False
	/// covers both "no barrel reaches it" and "the barrel exports it <c>type</c>-only", which is how
	/// <c>@types/three</c> spells a class three.js keeps internal.
	/// </summary>
	public bool IsExported { get; set; }

	/// <summary>
	/// Whether the shipped three.js bundle actually puts the name on <c>THREE</c>. The types can claim
	/// an export the runtime does not have — <c>SourceJSON</c> is declared <c>export class</c> where
	/// every other JSON shape is an <c>interface</c> — and <c>three-interop.js</c> resolves constructors
	/// against the bundle, not against the types.
	/// </summary>
	public bool IsRuntimeExport { get; set; }

	/// <summary>
	/// Export name when it differs from <see cref="Name"/>. In the current snapshot this is always the
	/// literal <c>default</c> (<c>export default class X</c>), which names the module's default
	/// binding rather than the symbol three.js re-exports, so it is not the wire token.
	/// </summary>
	public string? ExportName { get; set; }

	/// <summary>True when the class cannot be instantiated directly.</summary>
	public bool IsAbstract { get; set; }

	/// <summary>
	/// Declared type parameters. Erased for v1: a reference to one resolves to its default, failing
	/// that its constraint, so <c>Mesh&lt;TGeometry&gt;</c> maps as if it were <c>Mesh</c> taking a
	/// <c>BufferGeometry</c>.
	/// </summary>
	public List<IrTypeParameter> TypeParameters { get; set; } = [];

	/// <summary>Base class, absent for a root class.</summary>
	public IrType? Extends { get; set; }

	/// <summary>JSDoc attached to the class declaration.</summary>
	public IrDoc? Doc { get; set; }

	/// <summary>Constructor overloads in declaration order.</summary>
	public List<IrSignature> Constructors { get; set; } = [];

	/// <summary>Declared properties, including accessors.</summary>
	public List<IrProperty> Properties { get; set; } = [];

	/// <summary>Declared methods, with overloads grouped under one entry.</summary>
	public List<IrMethod> Methods { get; set; } = [];
}

/// <summary>One call signature: a constructor, or a single method overload.</summary>
internal sealed class IrSignature
{
	/// <summary>Positional parameters in declaration order.</summary>
	public List<IrParameter> Parameters { get; set; } = [];

	/// <summary>Return type; absent on a constructor.</summary>
	public IrType? ReturnType { get; set; }

	/// <summary><c>float</c> or <c>integer</c> when the <c>@returns</c> text says so; absent means unspecified.</summary>
	public string? ReturnNumericKind { get; set; }

	/// <summary>JSDoc attached to this signature.</summary>
	public IrDoc? Doc { get; set; }
}

/// <summary>A single parameter of a signature.</summary>
internal sealed class IrParameter
{
	/// <summary>Parameter name as declared in TypeScript.</summary>
	public required string Name { get; set; }

	/// <summary>Declared type.</summary>
	public IrType? Type { get; set; }

	/// <summary>True when the parameter is marked <c>?</c> or carries an initializer.</summary>
	public bool IsOptional { get; set; }

	/// <summary>True for a <c>...rest</c> parameter.</summary>
	public bool IsRest { get; set; }

	/// <summary><c>float</c> or <c>integer</c> when the JSDoc says so; absent means unspecified.</summary>
	public string? NumericKind { get; set; }

	/// <summary>Documented default, verbatim from the JSDoc, as source text.</summary>
	public string? DefaultValue { get; set; }

	/// <summary>Plain-text parameter documentation, not a <see cref="IrDoc"/>.</summary>
	public string? Doc { get; set; }
}

/// <summary>A declared property or accessor.</summary>
internal sealed class IrProperty
{
	/// <summary>Property name.</summary>
	public required string Name { get; set; }

	/// <summary>Declared type.</summary>
	public IrType? Type { get; set; }

	/// <summary>True for a static member.</summary>
	public bool IsStatic { get; set; }

	/// <summary>True for a <c>readonly</c> field or a getter with no setter.</summary>
	public bool IsReadonly { get; set; }

	/// <summary><c>get</c>, <c>set</c> or <c>get-set</c>; absent for a plain field.</summary>
	public string? Accessor { get; set; }

	/// <summary><c>protected</c> or <c>private</c>; absent means public.</summary>
	public string? Visibility { get; set; }

	/// <summary><c>float</c> or <c>integer</c> when the JSDoc says so; absent means unspecified.</summary>
	public string? NumericKind { get; set; }

	/// <summary>Documented default, verbatim from the JSDoc, as source text.</summary>
	public string? DefaultValue { get; set; }

	/// <summary>True when the property is declared <c>?</c>.</summary>
	public bool IsOptional { get; set; }

	/// <summary>True for an abstract member, which has no implementation to mirror.</summary>
	public bool IsAbstract { get; set; }

	/// <summary>JSDoc attached to the property.</summary>
	public IrDoc? Doc { get; set; }
}

/// <summary>A declared method, with every overload grouped under one entry.</summary>
internal sealed class IrMethod
{
	/// <summary>Method name.</summary>
	public required string Name { get; set; }

	/// <summary>True for a static member.</summary>
	public bool IsStatic { get; set; }

	/// <summary><c>protected</c> or <c>private</c>; absent means public.</summary>
	public string? Visibility { get; set; }

	/// <summary>True for an abstract member, which has no implementation to mirror.</summary>
	public bool IsAbstract { get; set; }

	/// <summary>Overloads in declaration order.</summary>
	public List<IrSignature> Overloads { get; set; } = [];
}

/// <summary>A type node, modelled from syntax rather than from the TypeScript checker.</summary>
internal sealed class IrType
{
	/// <summary>Discriminator: <c>primitive</c>, <c>reference</c>, <c>union</c>, <c>array</c>, and so on.</summary>
	public required string Kind { get; set; }

	/// <summary>Source text of the type, whitespace-collapsed. A faithful fallback for anything unmodelled.</summary>
	public required string Text { get; set; }

	/// <summary>Name of a <c>primitive</c> or <c>reference</c> node.</summary>
	public string? Name { get; set; }

	/// <summary><c>string</c>, <c>number</c>, <c>boolean</c> or <c>other</c> on a <c>literal</c> node.</summary>
	public string? LiteralKind { get; set; }

	/// <summary>Value of a <c>literal</c> node.</summary>
	public System.Text.Json.JsonElement? Value { get; set; }

	/// <summary>Element type of an <c>array</c>, <c>optional</c> or <c>rest</c> node.</summary>
	public IrType? Element { get; set; }

	/// <summary>Members of a <c>union</c> or <c>intersection</c> node.</summary>
	public List<IrType> Types { get; set; } = [];

	/// <summary>Members of a <c>tuple</c> node.</summary>
	public List<IrType> Elements { get; set; } = [];

	/// <summary>Type arguments of a <c>reference</c> node, e.g. the <c>Vector3</c> in <c>Curve&lt;Vector3&gt;</c>.</summary>
	public List<IrType> TypeArguments { get; set; } = [];

	/// <summary>What a <c>reference</c> name resolves to, and where it lives.</summary>
	public IrTypeTarget? Target { get; set; }
}

/// <summary>One declared type parameter of a class, interface or signature.</summary>
internal sealed class IrTypeParameter
{
	/// <summary>Parameter name, e.g. <c>TGeometry</c>.</summary>
	public required string Name { get; set; }

	/// <summary>The <c>extends</c> bound, when one is declared.</summary>
	public IrType? Constraint { get; set; }

	/// <summary>The <c>=</c> default, when one is declared. Preferred over the constraint when erasing.</summary>
	public IrType? Default { get; set; }
}

/// <summary>
/// A declared interface. three.js gives a class its property surface by declaration merging —
/// <c>export interface MeshStandardMaterial extends MeshStandardMaterialProperties {}</c> next to the
/// class of the same name — so an interface's members are part of the class's real member set, not
/// supporting detail.
/// </summary>
internal sealed class IrInterface
{
	/// <summary>Declared interface name.</summary>
	public required string Name { get; set; }

	/// <summary>POSIX path of the declaring file, relative to the types package root.</summary>
	public required string File { get; set; }

	/// <summary>Interfaces this one extends. An array, because an interface may extend several.</summary>
	public List<IrType> Extends { get; set; } = [];

	/// <summary>Declared properties, including accessors.</summary>
	public List<IrProperty> Properties { get; set; } = [];

	/// <summary>Declared methods, with overloads grouped under one entry.</summary>
	public List<IrMethod> Methods { get; set; } = [];

	/// <summary>Declared type parameters, in scope for every member declared here.</summary>
	public List<IrTypeParameter> TypeParameters { get; set; } = [];
}

/// <summary>A real TypeScript <c>enum</c> declaration.</summary>
internal sealed class IrEnum
{
	/// <summary>Declared enum name.</summary>
	public required string Name { get; set; }

	/// <summary>POSIX path of the declaring file, relative to the types package root.</summary>
	public required string File { get; set; }

	/// <summary>Members in declaration order.</summary>
	public List<IrEnumMember> Members { get; set; } = [];
}

/// <summary>One member of a real TypeScript <c>enum</c>.</summary>
internal sealed class IrEnumMember
{
	/// <summary>Member name.</summary>
	public required string Name { get; set; }

	/// <summary>Checker-computed constant value: a number or a string.</summary>
	public System.Text.Json.JsonElement? Value { get; set; }

	/// <summary>JSDoc attached to the member.</summary>
	public IrDoc? Doc { get; set; }
}

/// <summary>A loose <c>export const X: &lt;literal&gt;</c> declaration.</summary>
internal sealed class IrConstant
{
	/// <summary>Constant name, e.g. <c>FrontSide</c>.</summary>
	public required string Name { get; set; }

	/// <summary>POSIX path of the declaring file, relative to the types package root.</summary>
	public required string File { get; set; }

	/// <summary>Declared type, normally a literal node carrying the value.</summary>
	public IrType? Type { get; set; }

	/// <summary>JSDoc attached to the constant.</summary>
	public IrDoc? Doc { get; set; }
}

/// <summary>A type alias. The ones with a <see cref="ConstantGroup"/> are the enum candidates.</summary>
internal sealed class IrTypeAlias
{
	/// <summary>Alias name, e.g. <c>Side</c>.</summary>
	public required string Name { get; set; }

	/// <summary>POSIX path of the declaring file, relative to the types package root.</summary>
	public required string File { get; set; }

	/// <summary>
	/// Names of the in-scope constants the alias unions, present only when every member of the union
	/// is a <c>typeof</c> of one. This is the grouping signal for turning loose constants into enums.
	/// </summary>
	public List<string>? ConstantGroup { get; set; }

	/// <summary>The aliased type.</summary>
	public IrType? Type { get; set; }

	/// <summary>JSDoc attached to the alias.</summary>
	public IrDoc? Doc { get; set; }
}

/// <summary>One <c>declare module "…" { … }</c> block.</summary>
internal sealed class IrModuleAugmentation
{
	/// <summary>File the augmentation is declared in.</summary>
	public required string File { get; set; }

	/// <summary>Where the augmented declaration lives, when the target resolved in scope.</summary>
	public string? TargetFile { get; set; }

	/// <summary>Declarations this block merges members into.</summary>
	public List<IrAugmentedDeclaration> Augments { get; set; } = [];
}

/// <summary>One declaration a module augmentation merges members into.</summary>
internal sealed class IrAugmentedDeclaration
{
	/// <summary>Name of the augmented class or interface.</summary>
	public required string Name { get; set; }

	/// <summary>Base interfaces the augmentation merges onto the target's <c>extends</c> clause.</summary>
	public List<IrType> Extends { get; set; } = [];

	/// <summary>Properties added by the augmentation.</summary>
	public List<IrProperty> Properties { get; set; } = [];

	/// <summary>Methods added by the augmentation.</summary>
	public List<IrMethod> Methods { get; set; } = [];
}

/// <summary>Resolution result for a <c>reference</c> type node.</summary>
internal sealed class IrTypeTarget
{
	/// <summary><c>class</c>, <c>interface</c>, <c>typeAlias</c>, <c>unresolved</c>, and so on.</summary>
	public string? RefKind { get; set; }

	/// <summary><c>in-scope</c>, <c>excluded</c>, <c>package</c>, <c>lib</c>, <c>external</c> or <c>unresolved</c>.</summary>
	public string? Origin { get; set; }

	/// <summary>Declaring file, present for <c>in-scope</c> and <c>excluded</c> targets.</summary>
	public string? File { get; set; }
}

/// <summary>Parsed JSDoc for a declaration. Every field is omitted by the extractor when empty.</summary>
internal sealed class IrDoc
{
	/// <summary>Leading description. Keeps <c>{@link …}</c> markers verbatim for the emitter to rewrite.</summary>
	public string? Summary { get; set; }

	/// <summary>Text of the <c>@remarks</c> tag.</summary>
	public string? Remarks { get; set; }

	/// <summary>Fenced <c>@example</c> blocks, in TypeScript.</summary>
	public List<string> Examples { get; set; } = [];

	/// <summary>Text of the <c>@returns</c> tag.</summary>
	public string? Returns { get; set; }

	/// <summary>Text of the <c>@defaultValue</c> or <c>@default</c> tag, backticks stripped.</summary>
	public string? DefaultValue { get; set; }

	/// <summary>Links from <c>@see</c> tags, most usefully the official documentation URL.</summary>
	public List<IrSeeReference> See { get; set; } = [];

	/// <summary>True when the declaration carries <c>@deprecated</c>.</summary>
	public bool IsDeprecated { get; set; }

	/// <summary>Text of the <c>@deprecated</c> tag.</summary>
	public string? Deprecated { get; set; }

	/// <summary>True when the declaration carries <c>@internal</c>. Kept, not filtered — the emitter decides.</summary>
	public bool IsInternal { get; set; }
}

/// <summary>One <c>@see</c> link.</summary>
internal sealed class IrSeeReference
{
	/// <summary>Target URL.</summary>
	public string? Url { get; set; }

	/// <summary>Link text, e.g. <c>Official Documentation</c>.</summary>
	public string? Label { get; set; }
}

/// <summary>Shared serializer settings for reading the IR.</summary>
internal static class IrSerialization
{
	/// <summary>
	/// The IR is camelCase and carries fields this model does not declare, so unmapped members are
	/// ignored rather than treated as an error.
	/// </summary>
	public static readonly System.Text.Json.JsonSerializerOptions Options = new()
	{
		PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = false,
		ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
		NumberHandling = JsonNumberHandling.AllowReadingFromString
	};
}
