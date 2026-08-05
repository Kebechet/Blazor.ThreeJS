using Kebechet.Blazor.ThreeJS.Addons;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Shouldly;

namespace Blazor.ThreeJS.Tests.Addons;

/// <summary>
/// Covers what C# does with a graph the browser built: which nodes it mirrors, what each mirror
/// knows, and that adopting them costs nothing. That a real glTF actually loads is pinned end to end
/// against the vendored addon by <c>tests/wire-format.test.mjs</c>.
/// </summary>
public class GLTFLoaderTests
{
	private const string ModelUrl = "models/figure.gltf";

	[Fact]
	public async Task GLTFLoader_Load_AsksTheBrowserForTheUrlOnThisContext()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 4);

		// Act
		await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Assert
		var invocation = module.Invocations.Single(x => x.Identifier == "loadGltf");
		invocation.Arguments.ShouldBe([4, ModelUrl]);
	}

	[Fact]
	public async Task GLTFLoader_Load_MirrorsTheRootAndEveryNamedNodeTheBrowserReported()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);

		// Act
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Assert
		model.Scene.Name.ShouldBe("Figure");
		model.Scene.ThreeType.ShouldBe("Group");
		model.Nodes.Select(x => x.Name).ShouldBe(["Head", "Torso"]);
	}

	[Fact]
	public async Task GLTFLoader_Load_RecordsNoOpsWhileSeedingTheMirror()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);

		// Act
		await new GLTFLoader(context).LoadAsync(ModelUrl);
		await context.FlushAsync();

		// Assert
		// Seeding writes the loader's own values into the mirror. Every one of them is already true of
		// the object in the browser, so sending any of them back would be a round trip that could only
		// confirm what the browser just said.
		module.AppliedBatches.ShouldBeEmpty();
	}

	[Fact]
	public async Task GLTFLoader_LoadedNode_ReportsTheTransformTheLoaderGaveIt()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);

		// Act
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Assert
		var head = model.FindNode("Head").ShouldNotBeNull();
		head.Position.ToArray().ShouldBe([0f, 0.95f, 0f]);
		head.Scale.ToArray().ShouldBe([0.3f, 0.32f, 0.3f]);
		head.ThreeType.ShouldBe("Mesh");
		head.IsVisible.ShouldBeTrue();
	}

	[Fact]
	public async Task GLTFLoader_LoadedNodeTheLoaderHid_ReportsItselfHidden()
	{
		// Arrange
		var module = new AddonJsObjectReference
		{
			LoadResponse = new GLTFLoadResponse
			{
				Nodes =
				[
					LoadedNode.Describe(handle: -1, "Figure", "Group"),
					LoadedNode.Describe(handle: -2, "Hidden", isVisible: false)
				]
			}
		};

		var context = new ThreeContext(module, contextId: 1);

		// Act
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Assert
		model.FindNode("Hidden").ShouldNotBeNull().IsVisible.ShouldBeFalse();
	}

	[Fact]
	public async Task GLTFLoader_LoadedRootAddedToAScene_RecordsAnAddAndNoCreateForIt()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);
		var scene = new Scene();
		context.Attach(scene);

		// Act
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);
		scene.Add(model.Scene);
		await context.FlushAsync();

		// Assert
		var ops = module.AllOps;
		ops.ShouldContain(x => x.Kind == ThreeOpKind.Add && x.Handle == scene.Handle && x.ChildHandle == model.Scene.Handle);
		ops.ShouldNotContain(x => x.Kind == ThreeOpKind.Create && x.Handle == model.Scene.Handle);
	}

	[Fact]
	public async Task GLTFLoader_WriteToALoadedNode_RecordsASetOnTheMintedHandle()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Act
		var head = model.FindNode("Head").ShouldNotBeNull();
		head.Position.Set(1f, 2f, 3f);
		await context.FlushAsync();

		// Assert
		var setOp = module.AllOps.Single(x => x.Kind == ThreeOpKind.Set);
		setOp.Handle.ShouldBe(head.Handle);
		setOp.Member.ShouldBe("position");
	}

	[Fact]
	public async Task GLTFLoader_WriteToALoadedNodeThatChangesNothing_RecordsNoOp()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Act
		// The seeded value, written again. Nothing changed, so nothing may cross the wire - the same
		// guard every mirrored property has, now standing on a value the browser supplied.
		model.FindNode("Head").ShouldNotBeNull().Position.Set(0f, 0.95f, 0f);
		await context.FlushAsync();

		// Assert
		module.AppliedBatches.ShouldBeEmpty();
	}

	[Fact]
	public async Task GLTFLoader_ClickSubscriptionOnALoadedNode_OptsItIntoHitTesting()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);
		var head = model.FindNode("Head").ShouldNotBeNull();
		ThreePointerEvent? raisedEvent = null;

		// Act
		head.OnClick += pointerEvent => raisedEvent = pointerEvent;
		await context.FlushAsync();
		context.DispatchPointerEvent(head.Handle, new ThreePointerEvent { Point = new Vector3(1f, 2f, 3f), Distance = 4f });

		// Assert
		module.AllOps.ShouldContain(x => x.Kind == ThreeOpKind.Pick && x.Handle == head.Handle && Equals(x.Value, true));
		raisedEvent.ShouldNotBeNull().Distance.ShouldBe(4f);
	}

	[Fact]
	public async Task GLTFLoader_LoadedNodeAttachedToASecondContext_Throws()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);
		var otherContext = new ThreeContext(new AddonJsObjectReference(), contextId: 2);
		var otherScene = new Scene();
		otherContext.Attach(otherScene);

		// Act
		var exception = Record.Exception(() => otherScene.Add(model.Scene));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task GLTFLoader_LoadedNodeAskedToEmitItsCreateOp_Throws()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Act
		// The rebuild path a lost WebGL context would take. The mirror holds this node's transform and
		// nothing else, so a create op would produce an empty object of the right shape in the right
		// place - which renders as nothing and reports no error.
		var exception = Record.Exception(() => model.Scene.EmitCreate(context.Batch));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public async Task GLTFLoader_LoadWithNoUrl_Throws(string url)
	{
		// Arrange
		var context = new ThreeContext(new AddonJsObjectReference(), contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => new GLTFLoader(context).LoadAsync(url));

		// Assert
		exception.ShouldBeOfType<ArgumentException>();
	}

	[Fact]
	public async Task GLTFLoader_LoadAnsweredWithNoNodes_Throws()
	{
		// Arrange
		var context = new ThreeContext(new AddonJsObjectReference(), contextId: 1);

		// Act
		var exception = await Record.ExceptionAsync(() => new GLTFLoader(context).LoadAsync(ModelUrl));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	[Fact]
	public async Task GLTFLoader_LoadAnsweredWithoutATransform_Throws()
	{
		// Arrange
		var module = new AddonJsObjectReference
		{
			LoadResponse = new GLTFLoadResponse
			{
				Nodes = [new GLTFNodeDescription { Handle = -1, Name = "Figure", Type = "Group", IsVisible = true }]
			}
		};

		var context = new ThreeContext(module, contextId: 1);

		// Act
		// A missing component would leave the mirror holding C#'s own default for a node three.js has
		// already placed somewhere else, with nothing anywhere to say the reading is wrong.
		var exception = await Record.ExceptionAsync(() => new GLTFLoader(context).LoadAsync(ModelUrl));

		// Assert
		exception.ShouldBeOfType<InvalidOperationException>();
	}

	/// <summary>
	/// The shape the browser reports for the demo's own figure, cut down to the root and two parts.
	/// </summary>
	/// <returns>The load response.</returns>
	private static GLTFLoadResponse FigureResponse()
	{
		return new GLTFLoadResponse
		{
			Nodes =
			[
				LoadedNode.Describe(handle: -1, "Figure", "Group"),
				LoadedNode.Describe(handle: -2, "Head", position: [0f, 0.95f, 0f], scale: [0.3f, 0.32f, 0.3f]),
				LoadedNode.Describe(handle: -3, "Torso", position: [0f, 0.35f, 0f], scale: [0.52f, 0.72f, 0.3f])
			]
		};
	}
}
