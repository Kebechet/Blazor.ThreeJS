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

	/// <summary>Grid of every geometry that builds from numbers alone.</summary>
	public const string CatalogueGeometries = "catalogue-geometries--parametric-geometries";

	/// <summary>The same sphere under each mesh material.</summary>
	public const string CatalogueMaterials = "catalogue-materials--mesh-materials";

	/// <summary>One scene lit by each light type in turn.</summary>
	public const string CatalogueLights = "catalogue-lights--light-types";

	/// <summary>The same boxes through a perspective and an orthographic projection.</summary>
	public const string CatalogueCameras = "catalogue-cameras--perspective-and-orthographic";

	/// <summary>Points, line segments, a sprite and an instanced mesh.</summary>
	public const string CatalogueObjectTypes = "catalogue-object-types--non-mesh-objects";

	/// <summary>Scene and light helpers drawn together.</summary>
	public const string CatalogueHelpers = "catalogue-helpers--scene-and-light-helpers";

	/// <summary>Live interop counters against an orbiting, then C#-driven, scene.</summary>
	public const string InteropBudget = "architecture-interop-budget--an-idle-scene-costs-nothing";

	/// <summary>Classes with no generated wrapper, reached by name.</summary>
	public const string EscapeHatch = "architecture-escape-hatch--reaching-what-the-mirror-does-not-wrap";

	/// <summary>The three ways a mirrored C# value stops matching the browser.</summary>
	public const string MirrorAuthority = "architecture-mirror-authority--when-the-mirror-is-stale";

	/// <summary>Every story the demo publishes.</summary>
	public static IReadOnlyList<string> All =>
	[
		ImperativeRotatingCube,
		ImperativeClickToRecolour,
		DeclarativeRotatingCube,
		DeclarativeClickToRecolour,
		DeclarativeNodes,
		FigureWithOrbitControls,
		CatalogueGeometries,
		CatalogueMaterials,
		CatalogueLights,
		CatalogueCameras,
		CatalogueObjectTypes,
		CatalogueHelpers,
		InteropBudget,
		EscapeHatch,
		MirrorAuthority
	];
}
