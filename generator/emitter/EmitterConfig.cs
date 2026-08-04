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
	/// Source-path prefixes whose classes are never mirrored. These are the renderer's own internals:
	/// in scope by the literal "everything in <c>src/</c> outside <c>src/nodes/</c>" rule, but nothing
	/// a consumer instantiates, and emitting them would put roughly a hundred classes of plumbing into
	/// the coverage table as if they were API.
	/// <para>
	/// The consumer-facing renderer types are not under either prefix — <c>WebGLRenderer</c> and every
	/// <c>WebGL*RenderTarget</c> live directly in <c>src/renderers/</c> — so they survive this rule.
	/// <see cref="ConsumerFacingRendererClassNames"/> pins that, so a future upstream file move shows
	/// up as a failed expectation rather than as a silently smaller API.
	/// </para>
	/// </summary>
	public static readonly IReadOnlyList<string> ExcludedSourcePrefixes = ["src/renderers/webgl/", "src/renderers/webxr/"];

	/// <summary>
	/// Renderer types that must stay mirrored despite <see cref="ExcludedSourcePrefixes"/>. Checked
	/// rather than special-cased: if one of these ever moves under an excluded prefix, the coverage
	/// report says so.
	/// </summary>
	public static readonly IReadOnlyList<string> ConsumerFacingRendererClassNames =
	[
		"WebGL3DRenderTarget",
		"WebGLArrayRenderTarget",
		"WebGLCubeRenderTarget",
		"WebGLRenderTarget",
		"WebGLRenderer"
	];

	/// <summary>
	/// Source prefix of three.js's math types. Everything under it is a by-value type, encoded inline
	/// on the wire rather than referenced by handle, so it is out of the generated surface entirely:
	/// five are hand-written (<see cref="MathTypeNames"/>) and giving the rest a C# representation is
	/// a public-API decision rather than a mapping one.
	/// </summary>
	public const string MathSourcePrefix = "src/math/";

	/// <summary>
	/// The alias three.js uses wherever a colour may be given as a <c>Color</c>, a CSS string or a hex
	/// number. The mirror exposes only <c>Color</c>, which covers the other two through
	/// <c>Color.FromHex</c> and reaches the browser as a real <c>THREE.Color</c>. This is the single
	/// most common non-numeric constructor parameter in the snapshot.
	/// </summary>
	public const string ColorRepresentationAliasName = "ColorRepresentation";

	/// <summary>Name of the hand-written colour type.</summary>
	public const string ColorTypeName = "Color";

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
	/// Hand-written classes declared <c>sealed</c>, which three.js nonetheless subclasses
	/// (<c>ArrayCamera extends PerspectiveCamera</c>, <c>BatchedMesh extends Mesh</c>,
	/// <c>ClippingGroup extends Group</c>). Plan 1 sealed them because only leaves were needed. A
	/// generated subclass cannot derive from one, so the projection reports them rather than pretending
	/// they compile; unsealing them is a public-API change, not a mapping decision.
	/// </summary>
	public static readonly IReadOnlySet<string> SealedHandWrittenClassNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"AmbientLight",
		"BoxGeometry",
		"DirectionalLight",
		"Group",
		"Mesh",
		"MeshStandardMaterial",
		"PerspectiveCamera",
		"Points",
		"PointsMaterial",
		"Scene"
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
