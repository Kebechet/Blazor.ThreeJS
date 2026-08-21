namespace Blazor.ThreeJS.E2E;

/// <summary>
/// The BlazingStory ids of every story in the demo. BlazingStory derives an id from the
/// <c>[Stories]</c> path and the story name, so these strings are the only place the suite depends on
/// how the demo is organised — and <c>Sidebar_ShellOpened_ListsEveryStoryExactlyOnce</c> checks the
/// list against what the shell actually publishes.
/// </summary>
internal static class Stories
{
	/// <summary>Imperative cube scene, animated from C#.</summary>
	public const string ImperativeRotatingCube = "components-threecanvas--rotating-cube";

	/// <summary>Imperative cube scene whose outer cubes recolour on a click.</summary>
	public const string ImperativeClickToRecolour = "components-threecanvas--click-to-recolour";

	/// <summary>The same rotating cube written as a component tree.</summary>
	public const string DeclarativeRotatingCube = "components-declarative-scene--rotating-cube";

	/// <summary>The same clickable cubes written as a component tree.</summary>
	public const string DeclarativeClickToRecolour = "components-declarative-scene--click-to-recolour";

	/// <summary>Every declarative node the package ships, in one scene.</summary>
	public const string DeclarativeNodes = "components-declarative-nodes--every-node-the-package-ships";

	/// <summary>glTF model loaded by the browser, with orbit controls attached.</summary>
	public const string FigureWithOrbitControls = "addons-gltfloader-and-orbitcontrols--load-and-orbit-a-model";

	/// <summary>A glTF model playing the animation clip its own file brought along.</summary>
	public const string AnimatedModel = "addons-gltfloader-animations--play-a-clip-the-file-brought-along";

	/// <summary>A Draco-compressed glTF model, loaded only once the caller opts a DRACOLoader in.</summary>
	public const string CompressedModel = "addons-gltfloader-compression--load-a-draco-compressed-model";

	/// <summary>Grid of every geometry that builds from numbers alone.</summary>
	public const string CatalogueGeometries = "catalogue-geometries--parametric-geometries";

	/// <summary>The same sphere under each mesh material.</summary>
	public const string CatalogueMaterials = "catalogue-materials--mesh-materials";

	/// <summary>One cube painted per face, through the geometry's own groups and a material list.</summary>
	public const string CatalogueMultiMaterial = "catalogue-materials--one-mesh-six-materials";

	/// <summary>One scene lit by each light type in turn.</summary>
	public const string CatalogueLights = "catalogue-lights--light-types";

	/// <summary>The same boxes through a perspective and an orthographic projection.</summary>
	public const string CatalogueCameras = "catalogue-cameras--perspective-and-orthographic";

	/// <summary>Points, line segments, a sprite and an instanced mesh.</summary>
	public const string CatalogueObjectTypes = "catalogue-object-types--non-mesh-objects";

	/// <summary>Scene and light helpers drawn together.</summary>
	public const string CatalogueHelpers = "catalogue-helpers--scene-and-light-helpers";

	/// <summary>A mesh built in C# from a typed-array vertex buffer and index buffer.</summary>
	public const string CatalogueCustomGeometry = "catalogue-custom-geometry--a-mesh-built-from-raw-vertex-data";

	/// <summary>A Vector2 spline and a Vector3 Catmull-Rom curve, sampled by three.js.</summary>
	public const string CatalogueCurves = "catalogue-curves--two-dimensional-and-three-dimensional-curves";

	/// <summary>The generated <c>Object3D</c> commands, beside the mirrored state they leave stale.</summary>
	public const string CatalogueTransformCommands = "catalogue-transform-commands--driving-a-mesh-with-commands";

	/// <summary>A DataTexture whose pixels are generated in C#.</summary>
	public const string CatalogueTextures = "catalogue-textures--a-texture-built-from-a-byte-buffer";

	/// <summary>Box3, Plane and a read-back Sphere, drawn where three.js can show them.</summary>
	public const string CatalogueMathValues = "catalogue-math-values--bounds-planes-and-spheres";

	/// <summary>Every way a value comes back from three.js, beside a mirrored one.</summary>
	public const string ReadingState = "architecture-reading-state-back--asking-three-js-what-it-holds";

	/// <summary>The adopted WebGPURenderer: tone mapping, exposure and clear colour from C#.</summary>
	public const string Renderer = "architecture-the-renderer--reaching-the-renderer-itself";

	/// <summary>Members whose answer is a three.js object, typed and untyped, adopted by handle.</summary>
	public const string ObjectResults = "architecture-object-results--when-the-answer-is-an-object";

	/// <summary>Shadow casting, reached by adopting the renderer's nested shadow map.</summary>
	public const string CatalogueShadows = "catalogue-shadows--casting-and-receiving-shadows";

	/// <summary>Linear and exponential fog, assigned through a union-typed scene property.</summary>
	public const string CatalogueFog = "catalogue-fog--linear-and-exponential-fog";

	/// <summary>A keyframe clip built in C# and played through an AnimationMixer.</summary>
	public const string CatalogueAnimation = "catalogue-animation--a-keyframe-clip-built-in-code";

	/// <summary>TSL node shading loaded from a JavaScript module, with its uniform driven from C#.</summary>
	public const string CatalogueShaders = "catalogue-shaders--custom-shading-with-tsl";

	/// <summary>Live interop counters against an orbiting, then C#-driven, scene.</summary>
	public const string InteropBudget = "architecture-interop-budget--an-idle-scene-costs-nothing";

	/// <summary>Classes with no generated wrapper, reached by name.</summary>
	public const string EscapeHatch = "architecture-escape-hatch--reaching-what-the-mirror-does-not-wrap";

	/// <summary>The three ways a mirrored C# value stops matching the browser.</summary>
	public const string MirrorAuthority = "architecture-mirror-authority--when-the-mirror-is-stale";

	/// <summary>Manual: a declarative camera swapped in place, and rendering pausing with none at all.</summary>
	public const string CameraSwitching = "components-camera-switching--swap-the-active-camera-in-place";

	/// <summary>Manual: replacing OrbitControls spends the old wrapper, and detaching it spares the live set.</summary>
	public const string OrbitControlsReplace = "addons-orbitcontrols-lifecycle--replace-a-set-then-detach-the-stale-one";

	/// <summary>Manual: MaxDistance clamped to a finite bound and restored to three.js's infinite default.</summary>
	public const string OrbitControlsInfinity = "addons-orbitcontrols-lifecycle--clamp-the-zoom-then-restore-infinity";

	/// <summary>Manual: a glTF model loaded and unloaded in cycles, with the browser's memory watched.</summary>
	public const string ModelUnload = "addons-gltfloader-unload--load-and-unload-a-model";

	/// <summary>Manual: static factories and raycast hits whose answers are objects, end to end.</summary>
	public const string StaticsAndRaycasts = "architecture-statics-and-raycasts--static-factories-and-raycast-hits";

	/// <summary>Every story the demo publishes.</summary>
	public static IReadOnlyList<string> All =>
	[
		ImperativeRotatingCube,
		ImperativeClickToRecolour,
		DeclarativeRotatingCube,
		DeclarativeClickToRecolour,
		DeclarativeNodes,
		FigureWithOrbitControls,
		AnimatedModel,
		CompressedModel,
		CatalogueGeometries,
		CatalogueMaterials,
		CatalogueMultiMaterial,
		CatalogueLights,
		CatalogueCameras,
		CatalogueObjectTypes,
		CatalogueHelpers,
		CatalogueCustomGeometry,
		CatalogueCurves,
		CatalogueTransformCommands,
		CatalogueTextures,
		CatalogueMathValues,
		CatalogueShadows,
		CatalogueFog,
		CatalogueAnimation,
		CatalogueShaders,
		ReadingState,
		Renderer,
		ObjectResults,
		InteropBudget,
		EscapeHatch,
		MirrorAuthority,
		CameraSwitching,
		OrbitControlsReplace,
		OrbitControlsInfinity,
		ModelUnload,
		StaticsAndRaycasts
	];
}
