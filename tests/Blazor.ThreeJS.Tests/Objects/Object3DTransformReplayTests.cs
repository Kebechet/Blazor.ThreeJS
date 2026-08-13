using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.JSInterop;
using NSubstitute;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Objects;

/// <summary>
/// What <see cref="Object3D"/> replays when it attaches.
/// <para>
/// The mirror used to replay <c>position</c>, <c>rotation</c>, <c>scale</c> and <c>visible</c>
/// unconditionally, on the reasoning that its own defaults matched three.js's. They do not:
/// <c>HemisphereLight</c>, <c>DirectionalLight</c> and <c>SpotLight</c> are constructed at
/// <c>(0, 1, 0)</c>, because for those three the position <em>is</em> the direction they light from.
/// Replaying an unwritten origin over that left a zero-length direction, and a scene lit only by a
/// hemisphere light rendered pure black - with no exception and nothing on the console.
/// </para>
/// </summary>
public class Object3DTransformReplayTests
{
	private static ThreeContext CreateContext()
	{
		return new ThreeContext(Substitute.For<IJSObjectReference>(), contextId: 1);
	}

	private static IReadOnlyList<string> AttachedMembers(ThreeContext context, ThreeObject root)
	{
		context.Attach(root);
		return context.Batch
			.Drain()
			.Where(x => x.Kind == ThreeOpKind.Set && x.Handle == root.Handle)
			.Select(x => x.Member!)
			.ToList();
	}

	/// <summary>
	/// The regression itself. A light nobody positioned must reach the browser with no <c>position</c>
	/// op at all, so three.js's own <c>(0, 1, 0)</c> survives.
	/// </summary>
	[Fact]
	public void EmitState_LightWithNoTransformWritten_ReplaysNoTransformOps()
	{
		// Arrange
		var context = CreateContext();
		var light = new HemisphereLight(Color.White, Color.White, 1f);

		// Act
		var members = AttachedMembers(context, light);

		// Assert
		members.ShouldNotContain("position");
		members.ShouldNotContain("rotation");
		members.ShouldNotContain("scale");
		members.ShouldNotContain("visible");
	}

	/// <summary>
	/// The other half: a transform the caller did write before attaching still has to arrive, which is
	/// the whole reason replay exists.
	/// </summary>
	[Fact]
	public void EmitState_TransformWrittenBeforeAttach_ReplaysIt()
	{
		// Arrange
		var context = CreateContext();
		var light = new DirectionalLight(Color.White, 1f);
		light.Position.Set(4f, 6f, 4f);
		light.IsVisible = false;

		// Act
		var members = AttachedMembers(context, light);

		// Assert
		members.ShouldContain("position");
		members.ShouldContain("visible");
		members.ShouldNotContain("rotation");
		members.ShouldNotContain("scale");
	}

	/// <summary>
	/// Writing one component rather than the whole vector counts as writing it - the mirror tracks the
	/// transform through each value's own change callback, not through an assignment to the property.
	/// </summary>
	[Fact]
	public void EmitState_SingleTransformComponentWritten_ReplaysThatTransform()
	{
		// Arrange
		var context = CreateContext();
		var group = new Group();
		group.Scale.X = 2f;

		// Act
		var members = AttachedMembers(context, group);

		// Assert
		members.ShouldContain("scale");
		members.ShouldNotContain("position");
	}
}
