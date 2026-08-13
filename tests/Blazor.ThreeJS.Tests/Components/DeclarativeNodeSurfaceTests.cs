using Kebechet.Blazor.ThreeJS.Components;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Components;

/// <summary>
/// The set of declarative components the package ships.
/// <para>
/// A floor rather than a description. <em>Components/Declarative nodes</em> in the demo says it shows
/// "every node the package ships" and lists them by name, and nothing about adding a component makes
/// that story update itself — so the claim would quietly stop being true. Adding a node here fails
/// this test, which is the reminder to put it in the story too.
/// </para>
/// </summary>
public class DeclarativeNodeSurfaceTests
{
	[Fact]
	public void DeclarativeNodes_PackageInspected_AreExactlyTheOnesTheDemoShows()
	{
		// Arrange & Act
		var nodeNames = typeof(ThreeNode).Assembly
			.GetExportedTypes()
			.Where(x => !x.IsAbstract && typeof(ThreeNode).IsAssignableFrom(x))
			.Select(x => x.Name)
			.OrderBy(x => x, StringComparer.Ordinal)
			.ToList();

		// Assert
		nodeNames.ShouldBe([
			"ThreeAmbientLight",
			"ThreeBoxGeometry",
			"ThreeCircleGeometry",
			"ThreeConeGeometry",
			"ThreeCylinderGeometry",
			"ThreeDirectionalLight",
			"ThreeGroup",
			"ThreeHemisphereLight",
			"ThreeMesh",
			"ThreeMeshBasicMaterial",
			"ThreeMeshStandardMaterial",
			"ThreeOrthographicCamera",
			"ThreePerspectiveCamera",
			"ThreePlaneGeometry",
			"ThreePointLight",
			"ThreePoints",
			"ThreePointsMaterial",
			"ThreeRingGeometry",
			"ThreeSphereGeometry",
			"ThreeSpotLight",
			"ThreeTorusGeometry",
			"ThreeTorusKnotGeometry"
		]);
	}
}
