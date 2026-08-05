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

	/// <summary>glTF model loaded by the browser, with orbit controls attached.</summary>
	public const string FigureWithOrbitControls = "addons-gltfloader-and-orbitcontrols--load-and-orbit-a-model";

	/// <summary>Every story the demo publishes.</summary>
	public static IReadOnlyList<string> All =>
	[
		ImperativeRotatingCube,
		ImperativeClickToRecolour,
		DeclarativeRotatingCube,
		DeclarativeClickToRecolour,
		FigureWithOrbitControls
	];
}
