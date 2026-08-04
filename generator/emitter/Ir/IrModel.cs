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
}

/// <summary>Provenance block of the IR.</summary>
internal sealed class IrMeta
{
	/// <summary>The npm package the API surface was read from, e.g. <c>@types/three</c>.</summary>
	public string? TypesPackage { get; set; }

	/// <summary>Exact pinned version of <see cref="TypesPackage"/>.</summary>
	public string? TypesVersion { get; set; }
}

/// <summary>A single three.js class declaration.</summary>
internal sealed class IrClass
{
	/// <summary>Declared class name, e.g. <c>BoxGeometry</c>.</summary>
	public required string Name { get; set; }

	/// <summary>POSIX path of the declaring file, relative to the types package root.</summary>
	public required string File { get; set; }

	/// <summary>False for a class declared but never exported, which the applier therefore cannot construct.</summary>
	public bool IsExported { get; set; }

	/// <summary>
	/// Export name when it differs from <see cref="Name"/>. In the current snapshot this is always the
	/// literal <c>default</c> (<c>export default class X</c>), which names the module's default
	/// binding rather than the symbol three.js re-exports, so it is not the wire token.
	/// </summary>
	public string? ExportName { get; set; }

	/// <summary>True when the class cannot be instantiated directly.</summary>
	public bool IsAbstract { get; set; }

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

	/// <summary>Element type of an <c>array</c>, <c>optional</c> or <c>rest</c> node.</summary>
	public IrType? Element { get; set; }

	/// <summary>Members of a <c>union</c> or <c>intersection</c> node.</summary>
	public List<IrType> Types { get; set; } = [];

	/// <summary>What a <c>reference</c> name resolves to, and where it lives.</summary>
	public IrTypeTarget? Target { get; set; }
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
