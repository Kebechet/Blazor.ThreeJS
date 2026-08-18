using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Addons;
using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.JSInterop;
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

		// The trailing null is the progress reference the caller did not ask for. Passed rather than
		// omitted, because the applier's parameter list is positional and a missing argument would make
		// the URL land in it.
		invocation.Arguments.ShouldBe([4, ModelUrl, null]);
	}

	[Fact]
	public async Task GLTFLoader_LoadWithoutProgress_PassesNoReference()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 4);

		// Act
		await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Assert
		// Nothing to keep alive in the browser's reference table when nobody is listening.
		var invocation = module.Invocations.Single(x => x.Identifier == "loadGltf");
		invocation.Arguments.Last().ShouldBeNull();
	}

	[Fact]
	public async Task GLTFLoader_LoadWithProgress_PassesAReferenceTheBrowserCanReportTo()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 4);
		var reports = new List<GltfLoadProgress>();

		// Act
		await new GLTFLoader(context).LoadAsync(ModelUrl, new Progress<GltfLoadProgress>(reports.Add));

		// Assert
		var invocation = module.Invocations.Single(x => x.Identifier == "loadGltf");
		invocation.Arguments.Last().ShouldBeOfType<DotNetObjectReference<GltfProgressReporter>>();
	}

	[Theory]
	[InlineData(512L, 2048L, 0.25d)]
	[InlineData(2048L, 2048L, 1d)]
	public void GltfLoadProgress_TotalKnown_ReportsAFraction(long loaded, long total, double expected)
	{
		// Arrange & Act
		var progress = new GltfLoadProgress { BytesLoaded = loaded, BytesTotal = total };

		// Assert
		progress.Fraction.ShouldBe(expected);
	}

	[Fact]
	public void GltfLoadProgress_TotalUnknown_ReportsNoFraction()
	{
		// Arrange & Act
		var progress = new GltfLoadProgress { BytesLoaded = 512L, BytesTotal = null };

		// Assert
		// Not zero: a server that streams without a Content-Length reports no total, and answering
		// "0% forever" would drive a determinate progress bar that never moves. Null says "indeterminate".
		progress.Fraction.ShouldBeNull();
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
	public async Task GLTFLoader_Load_SurfacesEveryClipTheBrowserReported()
	{
		// Arrange
		var module = new AddonJsObjectReference
		{
			LoadResponse = new GLTFLoadResponse
			{
				Nodes = [LoadedNode.Describe(handle: -1, "Figure", "Group")],
				Animations = [LoadedNode.DescribeClip(handle: -2, "Spin", 1.5f)]
			}
		};
		var context = new ThreeContext(module, contextId: 1);

		// Act
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Assert
		model.Animations.Count.ShouldBe(1);
		model.Animations[0].Name.ShouldBe("Spin");
		model.Animations[0].Duration.ShouldBe(1.5f);
	}

	[Fact]
	public async Task GLTFLoader_LoadWithNoAnimations_AnswersAnEmptyClipList()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);

		// Act
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Assert
		model.Animations.ShouldBeEmpty();
	}

	[Fact]
	public async Task GLTFLoader_LoadWithClips_RecordsNoOpsWhileAdoptingThem()
	{
		// Arrange
		var module = new AddonJsObjectReference
		{
			LoadResponse = new GLTFLoadResponse
			{
				Nodes = [LoadedNode.Describe(handle: -1, "Figure", "Group")],
				Animations = [LoadedNode.DescribeClip(handle: -2, "Spin", 1.5f)]
			}
		};
		var context = new ThreeContext(module, contextId: 1);

		// Act
		await new GLTFLoader(context).LoadAsync(ModelUrl);
		await context.FlushAsync();

		// Assert
		// Adopting a clip costs nothing to say, the same way seeding a node's transform does: every
		// value came from the browser's own answer, so sending any of it back would only confirm it.
		module.AppliedBatches.ShouldBeEmpty();
	}

	[Fact]
	public async Task GLTFLoader_FindClipWithNoMatch_AnswersNull()
	{
		// Arrange
		var module = new AddonJsObjectReference { LoadResponse = FigureResponse() };
		var context = new ThreeContext(module, contextId: 1);
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);

		// Act
		var clip = model.FindClip("missing");

		// Assert
		clip.ShouldBeNull();
	}

	[Fact]
	public async Task GLTFLoader_FoundClip_ExposesTheHandleTheBrowserMintedForIt()
	{
		// Arrange
		var module = new AddonJsObjectReference
		{
			LoadResponse = new GLTFLoadResponse
			{
				Nodes = [LoadedNode.Describe(handle: -1, "Figure", "Group")],
				Animations = [LoadedNode.DescribeClip(handle: -2, "Spin", 1.5f)]
			}
		};
		var context = new ThreeContext(module, contextId: 1);
		var model = await new GLTFLoader(context).LoadAsync(ModelUrl);
		var clip = model.FindClip("Spin").ShouldNotBeNull();

		// A clip is exercised the same way any adopted AnimationClip is: handed to a mixer already
		// attached to this context. What matters here is only that Clip.Handle is the browser-minted
		// one, and that ClipActionAsync sends it as a $ref rather than a value - not what the mixer
		// answers, which is out of scope for a fake module that answers nothing.
		var mixer = new AnimationMixer(model.Scene);
		context.Attach(mixer);

		// Act
		await mixer.ClipActionAsync(clip.Clip, model.Scene, AnimationBlendMode.NormalAnimationBlendMode);

		// Assert
		var op = module.AllOps.Single(x => x.Kind == ThreeOpKind.Read && x.Member == "clipAction");
		op.Args.ShouldNotBeNull();
		op.Args!.OfType<ThreeValue.HandleReference>().ShouldContain(x => x.Handle == clip.Clip.Handle);
		clip.Clip.Handle.ShouldBe(-2);
	}

	[Fact]
	public void GLTFLoadResponse_DeserializedWithoutAnAnimationsKey_AnswersWithAnEmptyList()
	{
		// Arrange
		// The shape an old browser tab, running JavaScript cached from before this feature shipped,
		// would still send - no "a" key at all rather than an empty array for one.
		const string json = """
			{"n":[{"h":-1,"n":"Figure","t":"Group","p":{"$t":"Vector3","v":[0,0,0]},"r":{"$t":"Euler","v":[0,0,0]},"s":{"$t":"Vector3","v":[1,1,1]},"v":true}]}
			""";

		// Act
		var response = JsonSerializer.Deserialize<GLTFLoadResponse>(json);

		// Assert
		response.ShouldNotBeNull();
		response.Animations.ShouldBeEmpty();
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
