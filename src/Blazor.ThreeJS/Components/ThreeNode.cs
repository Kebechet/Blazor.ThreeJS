using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// Base for every declarative component that owns one mirrored three.js object. Builds the object
/// when the component initializes, puts it into the slot its position in the render tree names,
/// writes its parameters into the mirror, and releases it when the component is disposed.
/// <para>
/// Nothing here talks to the browser directly. A component writes into the C# mirror exactly as
/// imperative code does, and the batch decides what that costs: a re-render that changes no parameter
/// records no op, and a flush with nothing pending makes no interop call. That is what keeps a scene
/// graph — which is nested <see cref="RenderFragment"/> child content, and therefore re-renders on
/// every parent render — from costing a message per frame.
/// </para>
/// </summary>
public abstract class ThreeNode : ComponentBase, IDisposable
{
	/// <summary>
	/// The nearest enclosing slot, cascaded by whichever component or scene root encloses this one.
	/// </summary>
	[CascadingParameter] private IThreeSlot? _parentSlot { get; set; }

	/// <summary>
	/// The scene root that owns this subtree, cascaded by <see cref="ThreeCanvas"/>. Carries the
	/// context to flush into and the camera registration.
	/// </summary>
	[CascadingParameter] private ThreeSceneRoot? _sceneRoot { get; set; }

	private bool _isRegisteredWithParent;
	private bool _isDisposed;

	/// <summary>The mirrored object this component owns.</summary>
	internal abstract ThreeObject MirroredObject { get; }

	/// <summary>
	/// Whether this component has been disposed, so a child detaching from it knows not to record an op
	/// naming a handle the applier has already retired.
	/// </summary>
	internal bool IsDisposed
	{
		get { return _isDisposed; }
	}

	/// <summary>
	/// Builds the mirrored object and, when the slot it belongs to has not itself been attached yet,
	/// puts it in straight away.
	/// <para>
	/// Registering here rather than later is what gives the initial build the same op order the
	/// imperative path produces. Nothing is attached during the first render, so a child slotting into
	/// its parent only touches the C# mirror; the whole graph is attached once, at the root, after the
	/// render batch — by which time every geometry is on its mesh and every mesh is in its group. The
	/// attach then walks it dependency-first, so a geometry's create op precedes the mesh's.
	/// </para>
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Thrown when the component is not inside a <see cref="ThreeCanvas"/>, which is the only thing
	/// that cascades a slot to build into.
	/// </exception>
	protected override void OnInitialized()
	{
		if (_parentSlot is null || _sceneRoot is null)
		{
			throw new InvalidOperationException(
				$"'{GetType().Name}' has no enclosing scene to build into. A declarative three.js component has to be written inside a " +
				$"'{nameof(ThreeCanvas)}', which is what owns the scene, the camera and the context every node records into.");
		}

		BuildMirroredObject();
		if (MirroredObject is Camera camera)
		{
			_sceneRoot.RegisterCamera(camera);
		}

		if (!_parentSlot.IsSlotAttached)
		{
			RegisterWithParent();
		}
	}

	/// <summary>
	/// Writes this component's parameters into the mirror, on the first render and on every one after
	/// it. Every parameter is written every time and the mirror decides what that costs: each of its
	/// setters compares against the value it already holds and records nothing when they match, so a
	/// re-render in which one parameter changed records exactly that one op, and a re-render in which
	/// none did records none at all.
	/// </summary>
	protected override void OnParametersSet()
	{
		ApplyParameters();
	}

	/// <summary>
	/// Completes a late registration and flushes.
	/// <para>
	/// A component added to a scene that is already running registers here rather than in
	/// <see cref="OnInitialized"/>, because its parent would otherwise emit the create op before the
	/// component's own children had a chance to fill its slots — a mesh created without its geometry.
	/// By the time this runs the whole new subtree has rendered, so the attach that registration
	/// triggers walks a complete object.
	/// </para>
	/// <para>
	/// The flush is unconditional and costs nothing when there is nothing to send. Every node flushes
	/// rather than only the root, because a node can re-render without the root re-rendering — a
	/// consumer's own component sitting between them is enough — and the writes of that render would
	/// otherwise sit in the batch until something unrelated happened to flush them. The first flush of
	/// a batch drains everything pending, so the others make no interop call.
	/// </para>
	/// </summary>
	/// <param name="firstRender">Whether this is the first time the component has rendered.</param>
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !_isRegisteredWithParent)
		{
			RegisterWithParent();
		}

		if (_sceneRoot?.Context is not { } threeContext)
		{
			return;
		}

		await threeContext.FlushAsync();
	}

	/// <summary>
	/// Takes this component's object out of its parent slot and releases it. Ordering across a removed
	/// subtree is safe in both directions: a parent that has already been disposed refuses the detach,
	/// so no op ever names a retired handle, and a release op tolerates a handle that is already gone.
	/// </summary>
	public void Dispose()
	{
		if (_isDisposed)
		{
			return;
		}

		_isDisposed = true;
		ReleaseParameterSubscriptions();
		if (_isRegisteredWithParent)
		{
			_parentSlot?.DetachChild(this);
		}

		ReleaseMirroredObject();
	}

	/// <summary>
	/// Builds the mirrored object this component owns. Called once, before any parameter is applied,
	/// with every <see cref="ParameterAttribute"/> already bound — so a constructor argument can be read
	/// straight off a parameter.
	/// </summary>
	protected abstract void BuildMirroredObject();

	/// <summary>
	/// Writes this component's parameters into the mirrored object. Called on every render; see
	/// <see cref="OnParametersSet"/> for why writing them all every time is what makes an unchanged
	/// render free.
	/// </summary>
	protected abstract void ApplyParameters();

	/// <summary>
	/// Releases anything this component subscribed the mirrored object to, before the object itself is
	/// released. Empty here; overridden where a parameter carries a subscription rather than a value.
	/// </summary>
	protected virtual void ReleaseParameterSubscriptions()
	{
	}

	/// <summary>
	/// Records the release of the mirrored object, if this component ever got as far as building one. A
	/// component whose <see cref="OnInitialized"/> threw is still disposed, and reaching for an object it
	/// never built would replace the failure the consumer needs to see with one about this class.
	/// </summary>
	protected abstract void ReleaseMirroredObject();

	/// <summary>
	/// Puts this component's object into its parent slot, once.
	/// </summary>
	private void RegisterWithParent()
	{
		if (_parentSlot is not { } parentSlot)
		{
			return;
		}

		_isRegisteredWithParent = true;
		parentSlot.AttachChild(this);
	}
}

/// <summary>
/// Base for a declarative component owning a mirrored object of a known type, which is what lets a
/// component write to the object's own properties rather than through the untyped escape hatch.
/// </summary>
/// <typeparam name="TThreeObject">Type of the mirrored object this component owns.</typeparam>
public abstract class ThreeNode<TThreeObject> : ThreeNode
	where TThreeObject : ThreeObject
{
	private TThreeObject? _object;
	private (string Name, object? Value)[] _constructedParameters = [];

	/// <summary>
	/// The mirrored object this component owns. Available from <see cref="ThreeNode.OnParametersSet"/>
	/// onwards; reading it earlier is a defect in a derived component rather than something a consumer
	/// can reach, so it throws rather than answering with a placeholder.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown before the component has initialized.</exception>
	public TThreeObject Object
	{
		get
		{
			return _object ?? throw new InvalidOperationException(
				$"'{GetType().Name}' has not built its '{typeof(TThreeObject).Name}' yet. The object is built when the component initializes, " +
				$"so nothing before that lifecycle stage can read it.");
		}
	}

	/// <summary>The mirrored object, as the untyped base sees it.</summary>
	internal override ThreeObject MirroredObject
	{
		get { return Object; }
	}

	/// <summary>
	/// Constructs the mirrored object from the parameters bound so far. Called once per component
	/// instance, so a three.js constructor argument that has no property behind it is fixed for the life
	/// of the component; changing such a parameter afterwards is refused rather than silently ignored.
	/// </summary>
	/// <returns>The object this component will own.</returns>
	protected abstract TThreeObject CreateThreeObject();

	/// <summary>
	/// The parameters <see cref="CreateThreeObject"/> passes to a three.js constructor and that no
	/// three.js property can carry afterwards — a box's width, a sphere's segment count. Named so the
	/// failure a change to one produces can say which one it was. Empty by default, for a component
	/// whose object takes no such argument.
	/// </summary>
	protected virtual (string Name, object? Value)[] ConstructionParameters
	{
		get { return []; }
	}

	/// <summary>
	/// Writes the parameters that correspond to writable three.js properties into
	/// <paramref name="target"/>. Empty here, for a component whose object has none.
	/// </summary>
	/// <param name="target">The mirrored object to write into.</param>
	protected virtual void ApplyParameters(TThreeObject target)
	{
	}

	/// <summary>Builds the object through <see cref="CreateThreeObject"/> and records what it was built with.</summary>
	protected sealed override void BuildMirroredObject()
	{
		_object = CreateThreeObject();
		_constructedParameters = ConstructionParameters;
	}

	/// <summary>Applies the parameters to the object this component owns.</summary>
	protected sealed override void ApplyParameters()
	{
		ThrowIfConstructionParametersChanged();
		ApplyParameters(Object);
	}

	/// <summary>Releases the object, when one was built.</summary>
	protected sealed override void ReleaseMirroredObject()
	{
		_object?.Release();
	}

	/// <summary>
	/// Refuses a change to a parameter the three.js constructor consumed and no property exposes. Such a
	/// value only ever existed as an argument to a constructor that has already run, so nothing could
	/// carry the new one across — and keeping the old one quietly would leave the scene disagreeing with
	/// the markup with nothing anywhere to say so.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when a construction parameter changed.</exception>
	private void ThrowIfConstructionParametersChanged()
	{
		foreach (var (constructed, current) in _constructedParameters.Zip(ConstructionParameters))
		{
			if (Equals(constructed.Value, current.Value))
			{
				continue;
			}

			throw new InvalidOperationException(
				$"'{current.Name}' on '{GetType().Name}' changed from '{constructed.Value}' to '{current.Value}', and three.js takes it as a " +
				$"constructor argument with no property behind it — the object built with the old value cannot be told the new one. Put an " +
				$"'@key' on the component whose value tracks this parameter, so a change builds a fresh object instead of trying to reconfigure " +
				$"one that cannot be.");
		}
	}
}
