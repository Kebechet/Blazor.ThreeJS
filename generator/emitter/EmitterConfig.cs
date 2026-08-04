namespace Blazor.ThreeJS.Emitter;

/// <summary>
/// The decisions the emitter cannot read out of the IR: which classes to emit, what namespace they
/// land in, and which C# types already exist to be referenced.
/// </summary>
internal static class EmitterConfig
{
	/// <summary>Root namespace of the package the generated code compiles into.</summary>
	public const string RootNamespace = "Kebechet.Blazor.ThreeJS";

	/// <summary>Namespace holding the runtime base types the generated code derives from.</summary>
	public const string CoreNamespace = RootNamespace + ".Core";

	/// <summary>Namespace holding the hand-written math value types.</summary>
	public const string MathNamespace = RootNamespace + ".Math";

	/// <summary>
	/// Namespace every generated class currently lands in. Provisional: mirroring three.js's own
	/// directory layout into sub-namespaces is a call for the full run, and moving a public type
	/// between namespaces is a breaking change, so the flat layout the hand-written classes already
	/// use is kept until that call is made.
	/// </summary>
	public const string GeneratedNamespace = RootNamespace + ".Objects";

	/// <summary>Base class for a three.js class whose own base has no C# mirror yet.</summary>
	public const string RootBaseTypeName = "ThreeObject";

	/// <summary>
	/// Classes the emitter is allowed to emit, keyed by the three.js export name. Deliberately an
	/// allowlist rather than "everything in the IR": roughly a third of the 309 classes are renderer
	/// internals, and the emitter refuses anything it cannot model exactly (see
	/// <see cref="Emit.UnsupportedMemberException"/>) rather than guessing.
	/// </summary>
	public static readonly IReadOnlyList<string> EmittedClassNames = ["BoxGeometry"];

	/// <summary>
	/// C# types that already exist in the package, so a <c>{@link X}</c> marker naming one can be
	/// rewritten as a resolvable <c>&lt;see cref="X"/&gt;</c>. Anything else becomes
	/// <c>&lt;c&gt;X&lt;/c&gt;</c> — an unresolvable cref is a CS1574 warning, and with
	/// <c>GenerateDocumentationFile</c> on across five target frameworks that multiplies by five.
	/// </summary>
	public static readonly IReadOnlySet<string> ExistingCSharpTypeNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"AmbientLight",
		"BoxGeometry",
		"Color",
		"DirectionalLight",
		"Euler",
		"Group",
		"Matrix4",
		"Mesh",
		"MeshStandardMaterial",
		"Object3D",
		"PerspectiveCamera",
		"Points",
		"PointsMaterial",
		"Quaternion",
		"Scene",
		"Side",
		"ThreeObject",
		"Vector3"
	};

	/// <summary>
	/// Hand-written math types, which live in a different namespace from the generated classes and so
	/// pull in an extra <c>using</c> when referenced.
	/// </summary>
	public static readonly IReadOnlySet<string> MathTypeNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"Color",
		"Euler",
		"Matrix4",
		"Quaternion",
		"Vector3"
	};

	/// <summary>Column budget for wrapped documentation text, excluding indentation and the <c>/// </c> prefix.</summary>
	public const int DocumentationWrapColumn = 96;

	/// <summary>Column budget for a declaration before its parameter list is broken onto separate lines.</summary>
	public const int DeclarationWrapColumn = 120;
}
