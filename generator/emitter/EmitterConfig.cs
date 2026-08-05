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
	/// Wire-encoder call that turns an argument the caller left unspecified into the <c>$undef</c>
	/// sentinel. Written out rather than derived, because the emitter compiles separately from the
	/// package and cannot <c>nameof</c> a runtime member.
	/// </summary>
	public const string OrUnspecifiedCall = "ThreeValue.OrUnspecified";

	/// <summary>Wire-encoder call that drops the unsupplied tail of a constructor argument list.</summary>
	public const string TrimUnspecifiedTailCall = "ThreeValue.TrimUnspecifiedTail";

	/// <summary>
	/// Suffix a query's C# name carries that three.js's own does not. The only rename the mirror makes:
	/// a query returns a <c>Task&lt;T&gt;</c>, and a method that hands back a task without saying so
	/// reads as a synchronous call at every call site. It also keeps a query from colliding with a
	/// same-named property on the same type.
	/// </summary>
	public const string QueryMethodSuffix = "Async";

	/// <summary>
	/// Classes the runtime provides by hand, which the generator therefore does not emit. They are
	/// still mirrored types: a generated class derives from one, and the surface resolver subtracts
	/// their members so the same three.js member is declared in exactly one C# type.
	/// <para>
	/// <c>Object3D</c> is the whole list. It carries the scene-graph machinery — parent/child
	/// attachment, the transform, and the pre-attach state replay — which is behaviour rather than
	/// surface, and which two plans went into hardening.
	/// </para>
	/// </summary>
	public static readonly IReadOnlySet<string> HandWrittenClassNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"Object3D"
	};

	/// <summary>
	/// The three.js members of <c>Object3D</c> the hand-written class actually implements.
	/// <para>
	/// The surface resolver subtracts every <c>Object3D</c> member from all ~100 descendants, so a
	/// member the hand-written class does not implement is on no C# type at all. That gap is the
	/// largest one in the mirror, and the report can only name it if it is told what is implemented —
	/// there is nothing in the IR that knows. Reviewed against
	/// <c>src/Blazor.ThreeJS/Objects/Object3D.cs</c>: a name listed here that the class does not carry
	/// would make the report understate the gap, which is the direction that matters.
	/// </para>
	/// </summary>
	public static readonly IReadOnlySet<string> HandWrittenObject3DMemberNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"add",
		"castShadow",
		"children",
		"customDepthMaterial",
		"customDistanceMaterial",
		"frustumCulled",
		"layers",
		"lookAt",
		"matrixAutoUpdate",
		"matrixWorldAutoUpdate",
		"matrixWorldNeedsUpdate",
		"name",
		"pivot",
		"position",
		"quaternion",
		"receiveShadow",
		"remove",
		"renderOrder",
		"rotation",
		"scale",
		"up",
		"visible"
	};

	/// <summary>
	/// Addons the package wraps by hand. They live in <c>examples/jsm</c>, which the extractor never
	/// reads, so nothing in the IR knows about them - and the coverage table would otherwise go on
	/// saying that none of the addons is wrapped, which stopped being true the moment these shipped.
	/// <para>
	/// Each name is the export the vendored addon module actually carries, which
	/// <c>tests/wire-format.test.mjs</c> pins by importing and constructing both against the vendored
	/// files. A rename upstream fails there rather than only making this table wrong.
	/// </para>
	/// </summary>
	public static readonly IReadOnlyList<string> HandWrittenAddonClassNames = ["GLTFLoader", "OrbitControls"];

	/// <summary>
	/// Root of the scene graph. A generated class that descends from it replays its state through
	/// <c>EmitState</c>, which <c>Object3D.AttachTo</c> calls after the create op; one that does not
	/// has no such hook and replays from <c>EmitCreate</c> instead.
	/// </summary>
	public const string SceneGraphBaseTypeName = "Object3D";

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
	/// C# types the package provides by hand, which generated code may reference without generating
	/// them.
	/// </summary>
	public static readonly IReadOnlySet<string> ExistingCSharpTypeNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"Color",
		"Euler",
		"Matrix4",
		"Object3D",
		"Quaternion",
		"ThreeObject",
		"Vector3"
	};

	/// <summary>
	/// Every C# type name that will exist in the package once this run lands: the hand-written ones
	/// above, plus every class and enum being generated. A <c>{@link X}</c> marker naming one is
	/// rewritten as a resolvable <c>&lt;see cref="X"/&gt;</c>; anything else becomes
	/// <c>&lt;c&gt;X&lt;/c&gt;</c>, because an unresolvable <c>cref</c> is a CS1574 warning and with
	/// <c>GenerateDocumentationFile</c> on across five target frameworks it multiplies by five.
	/// <para>
	/// Mutable because it cannot be known before the emission scope has reached its fixpoint — which
	/// classes are emittable is what decides it — and it is written exactly once, by
	/// <see cref="RegisterGeneratedTypeNames"/>, before any file is emitted.
	/// </para>
	/// </summary>
	public static IReadOnlySet<string> KnownCSharpTypeNames { get; private set; } = ExistingCSharpTypeNames;

	/// <summary>Records the names this run will generate, so documentation crefs to them resolve.</summary>
	/// <param name="generatedTypeNames">Class and enum names about to be emitted.</param>
	public static void RegisterGeneratedTypeNames(IEnumerable<string> generatedTypeNames)
	{
		var names = new HashSet<string>(ExistingCSharpTypeNames, StringComparer.Ordinal);
		names.UnionWith(generatedTypeNames);
		KnownCSharpTypeNames = names;
	}

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

	/// <summary>
	/// Value sets that are numeric, and therefore generatable, but are not part of the API this package
	/// ships. Keyed by three.js name, valued by the reason, which is reproduced in the coverage report
	/// so the exclusion is reviewable rather than silent.
	/// </summary>
	public static readonly IReadOnlyDictionary<string, string> ExcludedEnumNames = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["GPUColorWriteFlags"] = "a WebGPU construct, absent from the WebGL bundle this package ships (`THREE.GPUColorWriteFlags === undefined`). " +
			"Consistent with the standing exclusion of the WebGPU / TSL stack; it is numeric, so nothing but this rule would have kept it out"
	};

	/// <summary>
	/// Hand-written math types that cannot report a change. <c>Matrix4</c> hands its <c>Elements</c>
	/// array out directly, so <c>m.Elements[0] = 1f</c> is a legal mutation nothing can observe — there
	/// is no hook to hang a property write off. A matrix-typed property is therefore not mirrored at
	/// all, rather than mirrored as state that silently stops tracking.
	/// </summary>
	public static readonly IReadOnlySet<string> MathTypeNamesWithoutChangeNotification = new HashSet<string>(StringComparer.Ordinal)
	{
		"Matrix4"
	};

	/// <summary>
	/// Opening marker of the README's generated coverage region. A fixed format contract: the emitter
	/// writes between these two lines and <c>--check</c> reads between them, so they are declared once
	/// and never spelled out at either end.
	/// </summary>
	public const string ReadmeCoverageBeginMarker = "<!-- coverage:begin - generated by `npm run emit`; edit generator/emitter/Map/CoverageReport.cs, not this section -->";

	/// <summary>Closing marker of the README's generated coverage region.</summary>
	public const string ReadmeCoverageEndMarker = "<!-- coverage:end -->";

	/// <summary>Column budget for wrapped documentation text, excluding indentation and the <c>/// </c> prefix.</summary>
	public const int DocumentationWrapColumn = 96;

	/// <summary>Column budget for a declaration before its parameter list is broken onto separate lines.</summary>
	public const int DeclarationWrapColumn = 120;
}
