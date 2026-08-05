using Bunit;
using Kebechet.Blazor.ThreeJS.Components;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Components;

public class DeclarativeSceneTests
{
	[Fact]
	public async Task DeclarativeScene_FirstRender_CreatesEveryDependencyBeforeWhateverReferencesIt()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		host.Scene = builder =>
		{
			BuildCamera(builder, 0);
			BuildMesh(builder, 10, null);
		};

		// Act
		host.Render();

		// Assert
		var ops = host.Module.AllOps;
		ops.ShouldNotBeEmpty();
		ThrowIfAnyOpNamesAnUncreatedHandle(ops);

		var geometryCreateIndex = IndexOfCreate(ops, nameof(BoxGeometry));
		var materialCreateIndex = IndexOfCreate(ops, nameof(MeshStandardMaterial));
		var meshCreateIndex = IndexOfCreate(ops, nameof(Mesh));
		geometryCreateIndex.ShouldBeLessThan(meshCreateIndex);
		materialCreateIndex.ShouldBeLessThan(meshCreateIndex);

		var meshHandle = ops.ElementAt(meshCreateIndex).Handle;
		var addIndex = ops.Select((x, index) => (x, index)).First(x => x.x.Kind == ThreeOpKind.Add && x.x.ChildHandle == meshHandle).index;
		meshCreateIndex.ShouldBeLessThan(addIndex);
	}

	[Fact]
	public async Task DeclarativeScene_ReRenderedWithNothingChanged_MakesNoInteropCall()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		host.Scene = builder =>
		{
			BuildCamera(builder, 0);

			// Every value here is allocated afresh on each render, which is what a consumer writing
			// `Position="new(0, 1, 0)"` or `Color="Color.Red"` in markup actually produces. Blazor cannot
			// tell such a parameter has not changed, so none of these components can be short-circuited:
			// each one re-renders and re-applies every parameter, every time.
			builder.OpenComponent<ThreeMesh>(10);
			builder.AddAttribute(11, "Position", new Vector3(0f, 1f, 0f));
			builder.AddAttribute(12, "ChildContent", (RenderFragment) (meshBuilder =>
			{
				meshBuilder.OpenComponent<ThreeBoxGeometry>(0);
				meshBuilder.CloseComponent();
				meshBuilder.OpenComponent<ThreeMeshStandardMaterial>(2);
				meshBuilder.AddAttribute(3, "Color", new Color(1f, 0f, 0f));
				meshBuilder.CloseComponent();
			}));
			builder.CloseComponent();
		};
		host.Render();
		var callsAfterBuild = host.Module.CallCount;
		var batchesAfterBuild = host.Module.AppliedBatches.Count;

		// Act
		for (var renderIndex = 0; renderIndex < 5; renderIndex++)
		{
			await host.ForceRenderAsync();
		}

		// Assert
		// Non-vacuously: the mesh and the material really did re-render, at least once per forced render,
		// so the whole parameter-application path ran again every time and still produced nothing. A tree
		// that had quietly skipped re-rendering would pass the interop assertions below for the wrong
		// reason.
		host.RootComponent.RenderCount.ShouldBeGreaterThan(1);
		host.RootComponent.FindComponent<ThreeMesh>().RenderCount.ShouldBeGreaterThan(5);
		host.RootComponent.FindComponent<ThreeMeshStandardMaterial>().RenderCount.ShouldBeGreaterThan(5);
		host.Module.AppliedBatches.Count.ShouldBe(batchesAfterBuild);
		host.Module.CallCount.ShouldBe(callsAfterBuild);
	}

	[Fact]
	public async Task DeclarativeScene_OneParameterChanged_RecordsThatWriteAndNothingElse()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		host.Scene = builder =>
		{
			BuildCamera(builder, 0);
			BuildMesh(builder, 10, host.MeshPosition);
		};
		host.Render();
		var batchesAfterBuild = host.Module.AppliedBatches.Count;

		// Act
		host.MeshPosition = new Vector3(0f, 2f, 0f);
		await host.ForceRenderAsync();

		// Assert
		host.Module.AppliedBatches.Count.ShouldBe(batchesAfterBuild + 1);
		var ops = host.Module.AppliedBatches.Last();
		ops.Count.ShouldBe(1);
		var op = ops.First();
		op.Kind.ShouldBe(ThreeOpKind.Set);
		op.Member.ShouldBe("position");
	}

	[Fact]
	public async Task DeclarativeScene_SubtreeRemoved_DetachesAndReleasesWithoutNamingAReleasedHandle()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		host.IsSubtreeRendered = true;
		host.Scene = builder =>
		{
			BuildCamera(builder, 0);
			if (!host.IsSubtreeRendered)
			{
				return;
			}

			builder.OpenComponent<ThreeGroup>(10);
			builder.AddAttribute(11, "ChildContent", (RenderFragment) (groupBuilder => BuildMesh(groupBuilder, 20, null)));
			builder.CloseComponent();
		};
		host.Render();
		var sceneHandle = host.Module.AllOps.First(x => x.Kind == ThreeOpKind.Create && x.Type == nameof(Scene)).Handle;
		var groupHandle = host.Module.AllOps.First(x => x.Kind == ThreeOpKind.Create && x.Type == nameof(Group)).Handle;
		var meshHandle = host.Module.AllOps.First(x => x.Kind == ThreeOpKind.Create && x.Type == nameof(Mesh)).Handle;
		var geometryHandle = host.Module.AllOps.First(x => x.Kind == ThreeOpKind.Create && x.Type == nameof(BoxGeometry)).Handle;
		var materialHandle = host.Module.AllOps.First(x => x.Kind == ThreeOpKind.Create && x.Type == nameof(MeshStandardMaterial)).Handle;
		var opsBeforeRemoval = host.Module.AllOps.Count;

		// Act
		host.IsSubtreeRendered = false;
		await host.ForceRenderAsync();

		// Assert
		var teardownOps = host.Module.AllOps.Skip(opsBeforeRemoval).ToList();
		ThrowIfAnyOpNamesAnUncreatedHandle(host.Module.AllOps);

		// Blazor disposes the subtree top-down, not bottom-up: the group goes first, then the mesh under
		// it, then that mesh's geometry and material. The group's detach therefore has to be the first op
		// — it is the only moment at which both the scene and the group are still live — and everything
		// below it must record no detach at all, since the slot it would detach from has already been
		// released. Pinned as an exact sequence, because the failure the design is guarding against is an
		// extra op rather than a missing one.
		var teardownShape = teardownOps
			.Select(x => $"{x.Kind}(handle {x.Handle}, child {x.ChildHandle})")
			.ToList();

		teardownShape.ShouldBe(
		[
			$"{ThreeOpKind.Remove}(handle {sceneHandle}, child {groupHandle})",
			$"{ThreeOpKind.Dispose}(handle {groupHandle}, child 0)",
			$"{ThreeOpKind.Dispose}(handle {meshHandle}, child 0)",
			$"{ThreeOpKind.Dispose}(handle {geometryHandle}, child 0)",
			$"{ThreeOpKind.Dispose}(handle {materialHandle}, child 0)"
		]);
	}

	[Fact]
	public async Task DeclarativeScene_MeshAddedToARunningScene_StillCreatesItsGeometryFirst()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		host.IsSubtreeRendered = false;
		host.Scene = builder =>
		{
			BuildCamera(builder, 0);
			if (!host.IsSubtreeRendered)
			{
				return;
			}

			BuildMesh(builder, 10, null);
		};
		host.Render();
		var opsBeforeAddition = host.Module.AllOps.Count;

		// Act
		host.IsSubtreeRendered = true;
		await host.ForceRenderAsync();

		// Assert
		var additionOps = host.Module.AllOps.Skip(opsBeforeAddition).ToList();
		ThrowIfAnyOpNamesAnUncreatedHandle(host.Module.AllOps);
		IndexOfCreate(additionOps, nameof(BoxGeometry)).ShouldBeLessThan(IndexOfCreate(additionOps, nameof(Mesh)));
		IndexOfCreate(additionOps, nameof(MeshStandardMaterial)).ShouldBeLessThan(IndexOfCreate(additionOps, nameof(Mesh)));
	}

	[Fact]
	public async Task DeclarativeScene_ClickHandlerSupplied_OptsTheObjectInAndRaisesTheHandler()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		var hits = new List<ThreePointerEvent>();
		host.Scene = builder =>
		{
			BuildCamera(builder, 0);
			builder.OpenComponent<ThreeMesh>(10);
			builder.AddAttribute(11, "OnClick", EventCallback.Factory.Create<ThreePointerEvent>(host, hits.Add));
			builder.CloseComponent();
		};
		host.Render();

		// Act
		var pickOp = host.Module.AllOps.First(x => x.Kind == ThreeOpKind.Pick);
		await host.RootComponent.InvokeAsync(() => host.Canvas.DispatchPointerEventAsync(pickOp.Handle, 1f, 2f, 3f, 4f));

		// Assert
		pickOp.Value.ShouldBe(true);
		hits.Count.ShouldBe(1);
		hits.First().Distance.ShouldBe(4f);
	}

	[Fact]
	public async Task DeclarativeScene_ConstructionOnlyParameterChanged_ThrowsNamingIt()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		host.BoxWidth = 1f;
		host.Scene = builder =>
		{
			BuildCamera(builder, 0);
			builder.OpenComponent<ThreeMesh>(10);
			builder.AddAttribute(11, "ChildContent", (RenderFragment) (meshBuilder =>
			{
				meshBuilder.OpenComponent<ThreeBoxGeometry>(0);
				meshBuilder.AddAttribute(1, "Width", host.BoxWidth);
				meshBuilder.CloseComponent();
			}));
			builder.CloseComponent();
		};
		host.Render();

		// Act
		host.BoxWidth = 2f;
		await Record.ExceptionAsync(() => host.ForceRenderAsync());
		var exception = await host.WaitForUnhandledExceptionAsync();

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
		exception.Message.ShouldContain(nameof(ThreeBoxGeometry.Width));
		exception.Message.ShouldContain("@key");
	}

	[Fact]
	public async Task DeclarativeScene_MaterialWrittenDirectlyInTheCanvas_ThrowsNamingWhatBelongsThere()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		host.Scene = builder =>
		{
			BuildCamera(builder, 0);
			builder.OpenComponent<ThreeMeshStandardMaterial>(10);
			builder.CloseComponent();
		};

		// Act
		var exception = Record.Exception(() => host.Render());

		// Assert
		exception.ShouldNotBeNull();
		exception.Message.ShouldContain(nameof(MeshStandardMaterial));
	}

	[Fact]
	public async Task DeclarativeScene_NoCameraDeclared_ThrowsNamingTheCameraComponents()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		host.Scene = builder => BuildMesh(builder, 0, null);

		// Act
		var exception = Record.Exception(() => host.Render());

		// Assert
		exception.ShouldNotBeNull();
		exception.Message.ShouldContain(nameof(ThreePerspectiveCamera));
	}

	[Fact]
	public async Task DeclarativeScene_ContentAppearsAfterTheFirstRender_StartsRenderingWhenTheCameraArrives()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();
		host.IsSubtreeRendered = false;
		host.Scene = builder =>
		{
			if (!host.IsSubtreeRendered)
			{
				return;
			}

			BuildCamera(builder, 0);
			BuildMesh(builder, 10, null);
		};
		host.Render();
		host.Module.SetActiveSceneCallCount.ShouldBe(0);

		// Act
		host.IsSubtreeRendered = true;
		await host.ForceRenderAsync();

		// Assert
		host.Module.SetActiveSceneCallCount.ShouldBe(1);
		ThrowIfAnyOpNamesAnUncreatedHandle(host.Module.AllOps);
		IndexOfCreate(host.Module.AllOps, nameof(BoxGeometry)).ShouldBeLessThan(IndexOfCreate(host.Module.AllOps, nameof(Mesh)));
	}

	[Fact]
	public async Task DeclarativeScene_NodeWrittenOutsideACanvas_ThrowsNamingTheCanvas()
	{
		// Arrange
		using var bunitContext = new BunitContext();

		// Act
		var exception = Record.Exception(() => bunitContext.Render<ThreeGroup>(builder =>
		{
			builder.OpenComponent<ThreeGroup>(0);
			builder.CloseComponent();
		}));

		// Assert
		exception.ShouldNotBeNull();
		exception.Message.ShouldContain(nameof(ThreeCanvas));
	}

	[Fact]
	public async Task DeclarativeScene_CanvasWithNoChildContent_BuildsNothingAndCostsNoInterop()
	{
		// Arrange
		await using var host = new DeclarativeSceneHost();

		// Act
		host.Render();

		// Assert
		host.Module.AppliedBatches.ShouldBeEmpty();
		host.Module.SetActiveSceneCallCount.ShouldBe(0);
	}

	/// <summary>
	/// Renders a camera whose parameters never change, so it contributes nothing to a later batch.
	/// </summary>
	/// <param name="builder">Builder for the scene fragment.</param>
	/// <param name="sequence">Sequence number to open the component at.</param>
	private static void BuildCamera(RenderTreeBuilder builder, int sequence)
	{
		builder.OpenComponent<ThreePerspectiveCamera>(sequence);
		builder.AddAttribute(sequence + 1, "Fov", 75f);
		builder.CloseComponent();
	}

	/// <summary>Renders a mesh with a geometry and a material written inside it.</summary>
	/// <param name="builder">Builder for the scene fragment.</param>
	/// <param name="sequence">Sequence number to open the component at.</param>
	/// <param name="position">Position to give the mesh, or <see langword="null"/> to leave it alone.</param>
	private static void BuildMesh(RenderTreeBuilder builder, int sequence, Vector3? position)
	{
		builder.OpenComponent<ThreeMesh>(sequence);
		builder.AddAttribute(sequence + 1, "Position", position);
		builder.AddAttribute(sequence + 2, "ChildContent", (RenderFragment) (meshBuilder =>
		{
			meshBuilder.OpenComponent<ThreeBoxGeometry>(0);
			meshBuilder.CloseComponent();
			meshBuilder.OpenComponent<ThreeMeshStandardMaterial>(2);
			meshBuilder.CloseComponent();
		}));
		builder.CloseComponent();
	}

	/// <summary>Position of the first create op for a three.js type.</summary>
	/// <param name="ops">Ops to search.</param>
	/// <param name="threeTypeName">Name of the three.js type.</param>
	/// <returns>The index of the create op.</returns>
	private static int IndexOfCreate(IReadOnlyList<ThreeOp> ops, string threeTypeName)
	{
		return ops
			.Select((x, index) => (Op: x, Index: index))
			.First(x => x.Op.Kind == ThreeOpKind.Create && x.Op.Type == threeTypeName)
			.Index;
	}

	/// <summary>
	/// The property the applier enforces and every ordering claim in this file reduces to: no op may
	/// name a handle that has not been created yet, or one that has already been released.
	/// <c>resolveHandle</c> throws <c>Unknown handle</c> for both, so an op that breaks this would fail
	/// in the browser with only an <c>OnError</c> to show for it.
	/// </summary>
	/// <param name="ops">Every op sent so far, in order.</param>
	private static void ThrowIfAnyOpNamesAnUncreatedHandle(IReadOnlyList<ThreeOp> ops)
	{
		var liveHandles = new HashSet<int>();
		foreach (var op in ops)
		{
			if (op.Kind == ThreeOpKind.Create)
			{
				foreach (var referencedHandle in ReferencedHandles(op))
				{
					liveHandles.ShouldContain(referencedHandle, $"a '{op.Type}' create op references handle {referencedHandle}, which nothing has created yet");
				}

				liveHandles.Add(op.Handle);
				continue;
			}

			liveHandles.ShouldContain(op.Handle, $"a '{op.Kind}' op targets handle {op.Handle}, which is not live at that point in the batch");
			foreach (var referencedHandle in ReferencedHandles(op))
			{
				liveHandles.ShouldContain(referencedHandle, $"a '{op.Kind}' op references handle {referencedHandle}, which is not live at that point in the batch");
			}

			if (op.Kind == ThreeOpKind.Dispose)
			{
				liveHandles.Remove(op.Handle);
			}
		}
	}

	/// <summary>Handles an op names besides its own target.</summary>
	/// <param name="op">The op to inspect.</param>
	/// <returns>Every other handle the applier will have to resolve for it.</returns>
	private static IEnumerable<int> ReferencedHandles(ThreeOp op)
	{
		if (op.ChildHandle != 0)
		{
			yield return op.ChildHandle;
		}

		if (op.Value is ThreeValue.HandleReference valueReference)
		{
			yield return valueReference.Handle;
		}

		foreach (var argument in op.Args ?? [])
		{
			if (argument is ThreeValue.HandleReference argumentReference)
			{
				yield return argumentReference.Handle;
			}
		}
	}
}
