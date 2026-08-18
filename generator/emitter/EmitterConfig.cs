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
	/// Concrete wrapper a query answers with when three.js hands back an object no generated class
	/// mirrors. It lives in <see cref="GeneratedNamespace"/> too, so naming it costs no <c>using</c>.
	/// </summary>
	public const string UntypedObjectTypeName = "Primitive";

	/// <summary>
	/// Wire-encoder call that turns an argument the caller left unspecified into the <c>$undef</c>
	/// sentinel. Written out rather than derived, because the emitter compiles separately from the
	/// package and cannot <c>nameof</c> a runtime member.
	/// </summary>
	public const string OrUnspecifiedCall = "ThreeValue.OrUnspecified";

	/// <summary>Wire-encoder call that drops the unsupplied tail of a constructor argument list.</summary>
	public const string TrimUnspecifiedTailCall = "ThreeValue.TrimUnspecifiedTail";

	/// <summary>
	/// Backing-field type for a constructor argument several overloads write different C# types into.
	/// <para>
	/// Only the private slot widens. Every constructor a caller sees keeps the arm's own type, and
	/// <c>ConstructorArgs</c> forwards the slot unchanged because <c>ThreeValue.Encode</c> dispatches on
	/// the runtime type — a boxed <c>float</c> and a <c>float[]</c> encode as themselves either way.
	/// </para>
	/// </summary>
	public const string UnionStorageTypeName = "object?";

	/// <summary>
	/// How many overloads one member may reasonably be expanded into before the set stops reading as an
	/// API and starts reading as noise. Not enforced — nothing sensible happens by refusing to emit a
	/// member three.js declares — but <c>api-coverage.md</c> prints the largest set produced beside it,
	/// so a union that grows upstream is a visible figure in a generated document rather than a silent
	/// wall of near-identical declarations. The product is multiplicative: two three-arm parameters on
	/// one member would be nine.
	/// </summary>
	public const int UnionOverloadBudget = 4;

	/// <summary>
	/// Suffix a query's C# name carries that three.js's own does not. The only rename the mirror makes:
	/// a query returns a <c>Task&lt;T&gt;</c>, and a method that hands back a task without saying so
	/// reads as a synchronous call at every call site. It also keeps a query from colliding with a
	/// same-named property on the same type.
	/// </summary>
	public const string QueryMethodSuffix = "Async";

	/// <summary>
	/// Classes the runtime provides by hand, which the generator therefore does not emit as a class of
	/// its own. They are still mirrored types: a generated class derives from one, and the surface
	/// resolver subtracts their members so the same three.js member is declared in exactly one C# type.
	/// <para>
	/// <c>Object3D</c> is the whole list. It carries the scene-graph machinery — parent/child
	/// attachment, the transform, and the pre-attach state replay — which is behaviour rather than
	/// surface, and which two plans went into hardening.
	/// </para>
	/// <para>
	/// A name here may also be in <see cref="HybridClassNames"/>, which is not a contradiction: the
	/// behaviour stays hand-written and a generated partial supplies the surface beside it.
	/// </para>
	/// </summary>
	public static readonly IReadOnlySet<string> HandWrittenClassNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"Object3D"
	};

	/// <summary>
	/// Hand-written classes that also get a generated partial carrying their command and query surface.
	/// <para>
	/// The hand-written half owns everything that is behaviour — the constructor, the create op, the
	/// mirrored transform and its pre-attach replay — and the generated half owns everything that is
	/// only a signature over the wire. Splitting them this way is what stops the largest gap in the
	/// mirror (every <c>Object3D</c> member is subtracted from ~100 descendants, so one the base does
	/// not carry is on no C# type at all) from being a hand-maintained list nobody refreshes.
	/// </para>
	/// <para>
	/// ⚠️ Mirrored state is deliberately <b>not</b> in the generated half. It would need a replay hook,
	/// and the hand-written half already overrides <c>EmitState</c> — two replays of the same object
	/// would write the same properties twice.
	/// </para>
	/// </summary>
	public static readonly IReadOnlySet<string> HybridClassNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"Object3D"
	};

	/// <summary>
	/// The three.js members of <c>Object3D</c> the hand-written half of the partial implements, and
	/// which the generated half therefore leaves alone.
	/// <para>
	/// Load-bearing in both directions. It is the emission exclusion for
	/// <c>Generated/Object3D.cs</c> — a name missing here would be declared twice on one type and fail
	/// the build — and it is what lets the coverage report say which half of the partial each member
	/// landed on, since nothing in the IR knows that a C# file exists. Reviewed against
	/// <c>src/Blazor.ThreeJS/Objects/Object3D.cs</c>.
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
	/// The consumer-facing renderer types are not under either prefix — <c>WebGPURenderer</c> lives in
	/// <c>src/renderers/webgpu/</c> and the render targets in <c>src/renderers/common/</c> — so they
	/// survive this rule. <see cref="ConsumerFacingRendererClassNames"/> pins that, so a future upstream
	/// file move shows up as a failed expectation rather than as a silently smaller API.
	/// </para>
	/// <para>
	/// ⚠️ <c>src/renderers/webgl/</c> is now dead weight rather than a live renderer: this package ships
	/// three.js's WebGPU build, which does not export <c>WebGLRenderer</c> at all. Its 25 classes are
	/// kept excluded for the same reason they always were, and the extractor now also reports them
	/// unexported, so nothing could reach them either way.
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
		"CubeRenderTarget",
		"RenderTarget",
		"WebGL3DRenderTarget",
		"WebGLArrayRenderTarget",
		"WebGLRenderTarget",
		"WebGPURenderer"
	];

	/// <summary>
	/// Source prefix of three.js's math types. Everything under it is a by-value type, encoded inline
	/// on the wire rather than referenced by handle, so it is out of the generated surface entirely:
	/// the hand-written ones (<see cref="MathTypeNames"/>) ship, and giving the rest a C# representation
	/// is a public-API decision rather than a mapping one.
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
	/// three.js's structural stand-ins for its own math types, keyed by interface name and valued by
	/// the hand-written C# type that satisfies them.
	/// <para>
	/// <c>Vector3Like</c> is <c>{ x: number; y: number; z: number }</c> — the shape a real
	/// <c>Vector3</c> has, declared separately so a caller can pass a plain object literal. The mirror
	/// has no plain object literals, so the concrete type is the only thing it could ever send, and
	/// what arrives is a genuine <c>THREE.Vector3</c>: strictly more than the parameter asks for.
	/// Mapping them is therefore exact rather than a narrowing, which is why they are here and the
	/// serialization shapes (<c>CurveJSON</c>, <c>LightShadowJSON</c>) are not.
	/// </para>
	/// </summary>
	public static readonly IReadOnlyDictionary<string, string> StructuralMathInterfaceNames = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["Vector2Like"] = "Vector2",
		["Vector3Like"] = "Vector3",
		["Vector4Like"] = "Vector4",
		["QuaternionLike"] = "Quaternion"
	};

	/// <summary>
	/// C# types the package provides by hand, which generated code may reference without generating
	/// them.
	/// </summary>
	public static readonly IReadOnlySet<string> ExistingCSharpTypeNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"Box2",
		"Box3",
		"Color",
		"Cylindrical",
		"Euler",
		"Frustum",
		"Line3",
		"Matrix3",
		"Matrix4",
		"Object3D",
		"Plane",
		"Quaternion",
		"Ray",
		"Sphere",
		"Spherical",
		"SphericalHarmonics3",
		"ThreeObject",
		"Triangle",
		"Vector2",
		"Vector3",
		"Vector4"
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
	/// JavaScript's typed arrays, each of which the package hand-writes a C# class for in the core
	/// namespace. They resolve ahead of the lib-type refusal because three.js hands one straight to
	/// WebGL — a <c>BufferAttribute</c>'s vertex data, a <c>DataTexture</c>'s pixels — so a plain array
	/// cannot stand in for one. The names are JavaScript's own global constructors, which is what the
	/// applier resolves them by.
	/// <para>
	/// <c>BigInt64Array</c> and <c>BigUint64Array</c> are deliberately absent: three.js's own
	/// <c>TypedArray</c> alias does not include them, so nothing in the surface can ask for one.
	/// </para>
	/// </summary>
	public static readonly IReadOnlySet<string> TypedArrayTypeNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"Float32Array",
		"Float64Array",
		"Int8Array",
		"Int16Array",
		"Int32Array",
		"Uint8Array",
		"Uint8ClampedArray",
		"Uint16Array",
		"Uint32Array"
	};

	/// <summary>Name of the hand-written abstract base every typed array derives from.</summary>
	public const string TypedArrayBaseTypeName = "TypedArray";

	/// <summary>
	/// Blocked classes whose capability is reachable another way, and how.
	/// <para>
	/// A blocked class is not automatically a lost feature, and the coverage table said nothing about
	/// the difference. Most of what is blocked here is an abstract base whose concrete subclasses all
	/// generate, or a convenience subclass that only rearranges arguments —
	/// <c>new Float32BufferAttribute(values, 3)</c> is <c>new BufferAttribute(new Float32Array(values), 3)</c>,
	/// verified equal on the runtime bundle rather than assumed.
	/// </para>
	/// <para>
	/// The report renders only entries whose class is still blocked, and names any entry that is not,
	/// so a note cannot outlive the limitation it describes.
	/// </para>
	/// </summary>
	public static readonly IReadOnlyDictionary<string, string> BlockedClassWorkarounds = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["Curve"] = "abstract in three.js; every concrete curve (`LineCurve`, `SplineCurve`, `CatmullRomCurve3`, …) generates",
		["KeyframeTrack"] = "abstract in three.js; all six concrete tracks (`VectorKeyframeTrack`, `NumberKeyframeTrack`, …) generate",
		["Light"] = "abstract in three.js; every concrete light generates",
		["Controls"] = "abstract in three.js; `OrbitControls` ships as a hand-written addon",
		["DataTextureLoader"] = "abstract in three.js; concrete loaders generate",
		["Float16BufferAttribute"] = "`new BufferAttribute(new Uint16Array(values), itemSize)` — the subclass only wraps the array",
		["Float32BufferAttribute"] = "`new BufferAttribute(new Float32Array(values), itemSize)` — the subclass only wraps the array",
		["Int8BufferAttribute"] = "`new BufferAttribute(new Int8Array(values), itemSize)` — the subclass only wraps the array",
		["Int16BufferAttribute"] = "`new BufferAttribute(new Int16Array(values), itemSize)` — the subclass only wraps the array",
		["Int32BufferAttribute"] = "`new BufferAttribute(new Int32Array(values), itemSize)` — the subclass only wraps the array",
		["Uint8BufferAttribute"] = "`new BufferAttribute(new Uint8Array(values), itemSize)` — the subclass only wraps the array",
		["Uint8ClampedBufferAttribute"] = "`new BufferAttribute(new Uint8ClampedArray(values), itemSize)` — the subclass only wraps the array",
		["Uint16BufferAttribute"] = "`new BufferAttribute(new Uint16Array(values), itemSize)` — the subclass only wraps the array",
		["Uint32BufferAttribute"] = "`new BufferAttribute(new Uint32Array(values), itemSize)` — the subclass only wraps the array",
		["PositionalAudio"] = "`new PrimitiveObject3D(\"PositionalAudio\", audioListener)` — its C# base needs constructor arguments a generated subclass cannot supply",
		["InstancedBufferAttribute"] = "`new Primitive(\"InstancedBufferAttribute\", array, itemSize)` — same base-constructor limitation",
		["InstancedInterleavedBuffer"] = "`new Primitive(\"InstancedInterleavedBuffer\", array, stride)` — same base-constructor limitation",
		["VideoTexture"] = "`new Primitive(\"VideoTexture\", videoElement)` — it takes an `HTMLVideoElement`, which C# never holds",
		["GLBufferAttribute"] = "`new Primitive(\"GLBufferAttribute\", buffer, type, itemSize, elementSize, count)` — it takes a raw WebGL buffer",
		["Uniform"] = "`new Primitive(\"Uniform\", value)` — its `value` is declared `any`",
		["PMREMGenerator"] = "`new Primitive(\"PMREMGenerator\", renderer)` — two three.js classes share this name",
		["Source"] = "`new Primitive(\"Source\", data)`",
		["CompressedTexture"] = "`new Primitive(\"CompressedTexture\", mipmaps, width, height)` — compressed formats need data C# does not produce",
		["CompressedArrayTexture"] = "`new Primitive(\"CompressedArrayTexture\", mipmaps, width, height, depth)`",
		["CompressedCubeTexture"] = "`new Primitive(\"CompressedCubeTexture\", images, format, type)`"
	};

	/// <summary>
	/// TypeScript's structural array interfaces. A plain JavaScript array satisfies all of them, and a
	/// plain JavaScript array is what the sequence encoder produces, so a parameter declared with one
	/// takes a C# array exactly rather than approximately.
	/// <para>
	/// They resolve ahead of the lib-type refusal because they are shapes rather than browser objects:
	/// <c>ArrayLike&lt;number&gt;</c> is how every <c>KeyframeTrack</c> declares its times and values,
	/// and refusing it blocked the entire animation stack over something the wire already carries.
	/// </para>
	/// </summary>
	public static readonly IReadOnlySet<string> StructuralSequenceTypeNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"ArrayLike",
		"Iterable",
		"ReadonlyArray"
	};

	/// <summary>
	/// Concrete type to build when a read answers with a handle whose declared type cannot itself be
	/// constructed, keyed by that declared type.
	/// <para>
	/// <c>Object3D</c> is abstract in C# — it is the scene-graph base every mirrored node derives from,
	/// not a node. A method declared to return one (<c>LOD.getObjectForDistance</c>) still has to answer
	/// with something, and the escape-hatch scene-graph wrapper is exactly that: it satisfies the
	/// declared type and carries three.js's own name for what actually came back.
	/// </para>
	/// </summary>
	public static readonly IReadOnlyDictionary<string, string> AdoptionSubstituteTypeNames = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["Object3D"] = "PrimitiveObject3D",

		// The root of the mirror, which a union of mirrored classes resolves to. Also abstract, and its
		// concrete escape-hatch wrapper is what a read of one answers with.
		[RootBaseTypeName] = "Primitive"
	};

	/// <summary>
	/// Hand-written math types, which live in a different namespace from the generated classes and so
	/// pull in an extra <c>using</c> when referenced.
	/// </summary>
	public static readonly IReadOnlySet<string> MathTypeNames = new HashSet<string>(StringComparer.Ordinal)
	{
		"Box2",
		"Box3",
		"Color",
		"Cylindrical",
		"Euler",
		"Frustum",
		"Line3",
		"Matrix3",
		"Matrix4",
		"Plane",
		"Quaternion",
		"Ray",
		"Sphere",
		"Spherical",
		"SphericalHarmonics3",
		"Triangle",
		"Vector2",
		"Vector3",
		"Vector4"
	};

	/// <summary>
	/// Value sets that are numeric, and therefore generatable, but are not part of the API this package
	/// ships. Keyed by three.js name, valued by the reason, which is reproduced in the coverage report
	/// so the exclusion is reviewable rather than silent.
	/// </summary>
	public static readonly IReadOnlyDictionary<string, string> ExcludedEnumNames;

	/// <summary>
	/// Enums synthesised from the string-literal unions three.js writes inline rather than behind a
	/// named type, keyed by the token set so every member declaring the same set shares one C# enum.
	/// <para>
	/// The names are curated rather than derived from the member, because the member is not a reliable
	/// guide: <c>MeshBasicMaterial.wireframeLinecap</c> is declared <c>"round" | "bevel" | "miter"</c>
	/// upstream — the <i>join</i> values, not the cap ones — so deriving a name from it would produce a
	/// <c>WireframeLinecap</c> enum holding join tokens and mislead every reader. Naming the set after
	/// what the set is keeps that upstream quirk from spreading into this API.
	/// </para>
	/// <para>
	/// A union whose token set is not listed here stays refused, so a new one upstream surfaces in the
	/// coverage report as a decision to make rather than being auto-named into the public surface.
	/// </para>
	/// </summary>
	public static readonly IReadOnlyList<(string Name, string[] Tokens)> SynthesisedStringEnums =
	[
		("LineJoin", ["round", "bevel", "miter"]),
		("LineCap", ["butt", "round", "square"])
	];

	/// <summary>
	/// The renderer's own WebGPU descriptor vocabulary. Every one of these is string-valued and so
	/// would generate now that the wire carries tokens — but no member of the emitted surface is typed
	/// by any of them, because they describe pipeline state the backend builds internally and this
	/// package never hands a consumer. Emitting them would add roughly two hundred enum members to the
	/// public API that nothing can be passed to.
	/// </summary>
	private static readonly string[] _webGpuDescriptorEnumNames =
	[
		"GPUAddressMode", "GPUBlendFactor", "GPUBlendOperation", "GPUBufferBindingType",
		"GPUCompareFunction", "GPUCullMode", "GPUFeatureMap", "GPUFeatureName", "GPUFilterMode",
		"GPUFrontFace", "GPUIndexFormat", "GPUInputStepMode", "GPULoadOp", "GPUPrimitiveTopology",
		"GPUSamplerBindingType", "GPUStencilOperation", "GPUStorageTextureAccess", "GPUStoreOp",
		"GPUTextureAspect", "GPUTextureDimension", "GPUTextureFormat", "GPUTextureSampleType",
		"GPUTextureViewDimension", "GPUVertexFormat"
	];

	// A static constructor rather than a field initialiser: those run in declaration order, so
	// building the dictionary where it is declared would read `_webGpuDescriptorEnumNames` before its
	// own initialiser had run and see null.
	static EmitterConfig()
	{
		ExcludedEnumNames = BuildExcludedEnumNames();
	}

	private static Dictionary<string, string> BuildExcludedEnumNames()
	{
		var excluded = new Dictionary<string, string>(StringComparer.Ordinal)
		{
			["EulerOrder"] = "already hand-written as `Kebechet.Blazor.ThreeJS.Math.EulerOrder`, because `Euler` is a " +
				"hand-written math value rather than a generated class. Its rotation order crosses inside the tagged " +
				"Euler value as an index the applier maps through `EULER_ORDERS`, so a second enum here would be a " +
				"duplicate name carrying a different wire form",

			["GPUColorWriteFlags"] = "declared by @types/three but not exported by the bundle this package ships " +
				"(`THREE.GPUColorWriteFlags === undefined`), so a member typed by it could never resolve at runtime. " +
				"It is numeric, so nothing but this rule would have kept it out"
		};

		foreach (var name in _webGpuDescriptorEnumNames)
		{
			excluded[name] = "part of the renderer's internal WebGPU descriptor vocabulary; no member of the emitted " +
				"surface is typed by it, so generating it would add public enum members nothing can be passed to";
		}

		return excluded;
	}

	/// <summary>
	/// Hand-written math types that cannot report a change. Both matrices hand their <c>Elements</c>
	/// array out directly, so <c>m.Elements[0] = 1f</c> is a legal mutation nothing can observe — there
	/// is no hook to hang a property write off. A matrix-typed property is therefore not mirrored at
	/// all, rather than mirrored as state that silently stops tracking.
	/// <para>
	/// Every other hand-written math type routes its components through property setters, including
	/// the composite ones: <c>Box3</c>, <c>Frustum</c> and the rest hang a callback off each child they
	/// own, which is why they copy their constructor arguments instead of retaining them.
	/// </para>
	/// </summary>
	public static readonly IReadOnlySet<string> MathTypeNamesWithoutChangeNotification = new HashSet<string>(StringComparer.Ordinal)
	{
		"Matrix3",
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
