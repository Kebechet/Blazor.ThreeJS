using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// The root of a declarative scene: owns the <see cref="Scene"/> the markup builds into, remembers
/// which camera it is rendered through, and is the slot the components written directly inside a
/// <see cref="ThreeCanvas"/> attach to.
/// <para>
/// Rendered by <see cref="ThreeCanvas"/> around its child content and never written by a consumer,
/// which is why it is internal. It exists as a component rather than as a plain object because it has
/// to render — cascading the two values the nodes need, and taking part in the render loop so that a
/// batch left pending by a removed subtree still gets flushed when no node survives to flush it.
/// </para>
/// </summary>
internal sealed class ThreeSceneRoot : ComponentBase, IThreeSlot
{
	/// <summary>The declarative scene, as the consumer wrote it inside the canvas.</summary>
	[Parameter] public RenderFragment? ChildContent { get; set; }

	private readonly Scene _scene = new();
	private readonly List<Camera> _cameras = [];
	private Camera? _renderedCamera;
	private bool _isActive;

	/// <summary>
	/// The context this scene records into, once <see cref="ThreeCanvas"/> has one. Null until then,
	/// which is what a node's flush checks: a node's first render completes before the JavaScript-side
	/// context exists, and there is nothing to flush into yet.
	/// </summary>
	public ThreeContext? Context { get; private set; }

	/// <summary>The scene object the markup builds into.</summary>
	public Scene Scene
	{
		get { return _scene; }
	}

	/// <summary>Whether the scene has been attached to a context.</summary>
	bool IThreeSlot.IsSlotAttached
	{
		get { return _scene.IsAttached; }
	}

	/// <summary>Adds a top-level scene-graph node to the scene.</summary>
	/// <param name="child">The component whose object is being slotted in.</param>
	/// <exception cref="InvalidOperationException">Thrown when the child's object does not belong in a scene graph.</exception>
	void IThreeSlot.AttachChild(ThreeNode child)
	{
		if (child.MirroredObject is not Object3D childObject)
		{
			throw new InvalidOperationException(
				$"A '{child.MirroredObject.GetType().Name}' cannot be written directly inside a '{nameof(ThreeCanvas)}'. " +
				$"Only scene-graph objects — meshes, lights, cameras, groups — go in the scene; a geometry or a material belongs inside the " +
				$"component that uses it.");
		}

		_scene.Add(childObject);
	}

	/// <summary>Takes a top-level node back out of the scene.</summary>
	/// <param name="child">The component whose object is being removed.</param>
	void IThreeSlot.DetachChild(ThreeNode child)
	{
		if (child.MirroredObject is Object3D childObject)
		{
			_scene.Remove(childObject);
		}
	}

	/// <summary>
	/// Records a camera the scene could be rendered through. The first registered one wins: a scene has
	/// exactly one active camera, and quietly switching to whichever camera component happened to
	/// initialize last would make the rendered view depend on markup order in a way nothing announces.
	/// The rest wait in registration order, so that removing the active camera promotes the next one
	/// rather than freezing the canvas on a handle the browser has retired.
	/// </summary>
	/// <param name="camera">A camera component's mirrored camera.</param>
	public void RegisterCamera(Camera camera)
	{
		if (!_cameras.Contains(camera))
		{
			_cameras.Add(camera);
		}
	}

	/// <summary>
	/// Takes a disposed camera component's camera out of the running. When it was the active one, the
	/// next render's activation pass re-points the renderer at the next registered camera — see
	/// <see cref="ActivateWhenReadyAsync"/> — and with no camera left, rendering pauses until one
	/// appears, which is also what an empty scene does.
	/// </summary>
	/// <param name="camera">The camera being removed.</param>
	public void UnregisterCamera(Camera camera)
	{
		_cameras.Remove(camera);
	}

	/// <summary>
	/// Takes the context the canvas created and starts rendering the scene if it is ready to be. Called
	/// once, by <see cref="ThreeCanvas"/>, after the first render — which is what makes the attach reach
	/// a complete graph and emit it in dependency order, exactly as the imperative path does.
	/// </summary>
	/// <param name="threeContext">The context the canvas created.</param>
	/// <exception cref="InvalidOperationException">Thrown when the markup declared a scene but no camera.</exception>
	public async Task BuildAsync(ThreeContext threeContext)
	{
		Context = threeContext;
		await ActivateWhenReadyAsync();
	}

	/// <summary>
	/// Starts rendering the scene the first time it has something to render and a camera to render it
	/// through, then drains anything the render left pending.
	/// <para>
	/// Activation is retried on every render rather than done once, because markup written as
	/// <c>@if (_isLoaded)</c> is empty on the first render and complete on a later one — and refusing to
	/// build a scene that has not appeared yet would make conditional content unusable.
	/// </para>
	/// <para>
	/// The flush covers the case no node can: a render whose only effect was to remove the last node,
	/// leaving the detach and release ops in the batch with nothing left to send them.
	/// </para>
	/// </summary>
	/// <param name="firstRender">Whether this is the first time the component has rendered.</param>
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (Context is not { } threeContext)
		{
			return;
		}

		await ActivateWhenReadyAsync();
		await threeContext.FlushAsync();
	}

	/// <summary>
	/// Attaches the scene and the camera and tells the renderer to draw them, then keeps the renderer
	/// pointed at a live camera on every render after that.
	/// <para>
	/// A scene with objects in it and no camera is a mistake and says so; a scene with nothing in it at
	/// all is markup that has not produced its content yet, and simply waits.
	/// </para>
	/// <para>
	/// Once active, this watches for the active camera changing under it: conditional markup that swaps
	/// one camera component for another disposes the camera the renderer draws through, and without
	/// re-pointing the renderer here the canvas would freeze on the retired handle with no error
	/// anywhere. A swap that leaves no camera at all pauses rendering — the browser-side loop skips a
	/// frame whose camera handle resolves to nothing — until a camera component appears again.
	/// </para>
	/// </summary>
	/// <returns>A task that completes once the renderer points at the current camera, or immediately if the scene is not ready.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the scene has objects in it but no camera.</exception>
	private async Task ActivateWhenReadyAsync()
	{
		if (Context is not { } threeContext)
		{
			return;
		}

		var activeCamera = _cameras.FirstOrDefault();
		if (!_isActive)
		{
			if (activeCamera is null)
			{
				if (_scene.Children.Any())
				{
					throw new InvalidOperationException(
						$"The scene written inside this '{nameof(ThreeCanvas)}' has objects in it but no camera, so there is no point of view to " +
						$"render it from. Add a camera component — '{nameof(ThreePerspectiveCamera)}' or '{nameof(ThreeOrthographicCamera)}' — to " +
						$"the markup.");
				}

				return;
			}

			_isActive = true;
			_renderedCamera = activeCamera;
			await threeContext.SetActiveSceneAsync(_scene, activeCamera);
			return;
		}

		if (activeCamera is not null && !ReferenceEquals(activeCamera, _renderedCamera))
		{
			_renderedCamera = activeCamera;
			await threeContext.SetActiveSceneAsync(_scene, activeCamera);
		}
	}

	/// <summary>
	/// Cascades itself twice: once as the scene root every node reaches for its context and its camera
	/// registration, once as the slot the top-level nodes attach to. Both are <c>IsFixed</c> — this
	/// component is both for as long as it exists.
	/// </summary>
	/// <param name="builder">Builder for this component's render tree.</param>
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenComponent<CascadingValue<ThreeSceneRoot>>(0);
		builder.AddAttribute(1, "Value", this);
		builder.AddAttribute(2, "IsFixed", true);
		builder.AddAttribute(3, "ChildContent", (RenderFragment) BuildSlotCascade);
		builder.CloseComponent();
	}

	/// <summary>Cascades this component as the slot, around the consumer's markup.</summary>
	/// <param name="builder">Builder for the cascade's child content.</param>
	private void BuildSlotCascade(RenderTreeBuilder builder)
	{
		builder.OpenComponent<CascadingValue<IThreeSlot>>(0);
		builder.AddAttribute(1, "Value", this);
		builder.AddAttribute(2, "IsFixed", true);
		builder.AddAttribute(3, "ChildContent", ChildContent);
		builder.CloseComponent();
	}
}
