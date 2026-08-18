using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Base for every object that can be placed in a scene graph: meshes, lights, cameras, and the
/// scene itself. Carries the transform three.js objects share, the parent/child relationships
/// between them, and the per-object rendering flags — shadows, layers, culling, render order.
/// <para>
/// Hand-written rather than generated, because the attachment machinery and the pre-attach state
/// replay below are behaviour rather than surface. Its members are subtracted from every generated
/// descendant, so a three.js member missing from this type is missing from the whole mirror.
/// </para>
/// <para>
/// Half of it <em>is</em> generated: <c>Generated/Object3D.cs</c> is the other part of this partial
/// and carries three.js's <c>Object3D</c> commands and queries — <c>RotateX</c>, <c>Attach</c>,
/// <c>GetObjectByNameAsync</c> and the rest — which are surface rather than behaviour. What neither
/// half carries is listed in <c>generator/api-coverage.md</c>.
/// </para>
/// </summary>
public abstract partial class Object3D : ThreeObject
{
	private readonly List<Object3D> _children = [];
	private bool _isVisible = true;
	private string _name = string.Empty;
	private bool _castShadow;
	private bool _receiveShadow;
	private bool _frustumCulled = true;
	private float _renderOrder;
	private bool _matrixAutoUpdate = true;
	private bool _matrixWorldAutoUpdate = true;
	private bool _matrixWorldNeedsUpdate;
	private Layers? _layers;
	private Material? _customDepthMaterial;
	private Material? _customDistanceMaterial;
	private bool _isPositionWritten;
	private bool _isRotationWritten;
	private bool _isScaleWritten;
	private bool _isVisibleWritten;
	private bool _isNameWritten;
	private bool _isUpWritten;
	private bool _isQuaternionWritten;
	private bool _isPivotWritten;
	private bool _isCastShadowWritten;
	private bool _isReceiveShadowWritten;
	private bool _isFrustumCulledWritten;
	private bool _isRenderOrderWritten;
	private bool _isMatrixAutoUpdateWritten;
	private bool _isMatrixWorldAutoUpdateWritten;
	private bool _isMatrixWorldNeedsUpdateWritten;
	private bool _isLayersWritten;
	private bool _isCustomDepthMaterialWritten;
	private bool _isCustomDistanceMaterialWritten;
	private Action<ThreePointerEvent>? _onClick;

	/// <summary>Position relative to the parent object.</summary>
	public Vector3 Position { get; } = new();

	/// <summary>Rotation relative to the parent object, expressed as Euler angles.</summary>
	public Euler Rotation { get; } = new();

	/// <summary>Scale relative to the parent object.</summary>
	public Vector3 Scale { get; } = new(1f, 1f, 1f);

	/// <summary>
	/// Object's local rotation as a quaternion. three.js keeps this and <see cref="Rotation"/> in step
	/// with each other, so writing either one moves the object; the wire writes into the instance
	/// three.js already holds, which is what triggers that. Mirrored as an instance this object owns:
	/// mutating it records a write of <c>quaternion</c>.
	/// </summary>
	public Quaternion Quaternion { get; } = new();

	/// <summary>
	/// The object's up direction, used by <see cref="LookAt"/> to decide the orientation of the result.
	/// Mirrored as an instance this object owns: mutating it records a write of <c>up</c>.
	/// </summary>
	public Vector3 Up { get; } = new(0f, 1f, 0f);

	/// <summary>
	/// The pivot point for rotation and scale transformations. When set, rotation and scale are applied
	/// around this point instead of the object's origin. Mirrored as an instance this object owns:
	/// mutating it records a write of <c>pivot</c>.
	/// <para>
	/// three.js starts this as <c>null</c>, meaning "no pivot", and C# has no null to mirror it with —
	/// so the instance here reads <c>(0, 0, 0)</c> until you touch it, and nothing is sent until you do.
	/// Mutating it is what gives the three.js object a pivot; there is no way back to <c>null</c>.
	/// </para>
	/// </summary>
	public Vector3 Pivot { get; } = new();

	/// <summary>Child objects attached via <see cref="Add"/>.</summary>
	public IReadOnlyList<Object3D> Children
	{
		get { return _children; }
	}

	/// <summary>
	/// Gets or sets whether this object is rendered. Setting this property records a property write
	/// once <see cref="ThreeObject.Batch"/> is attached, unless the value is unchanged — writing the
	/// value already held costs no interop, so a per-frame toggle that stays put is free.
	/// </summary>
	public bool IsVisible
	{
		get { return _isVisible; }
		set
		{
			if (_isVisible == value)
			{
				return;
			}

			_isVisible = value;
			_isVisibleWritten = true;
			RecordSet("visible", value);
		}
	}

	/// <summary>
	/// Optional name of the object, which does not need to be unique. Writing it records a <c>name</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public string Name
	{
		get { return _name; }
		set
		{
			if (_name == value)
			{
				return;
			}

			_name = value;
			_isNameWritten = true;
			RecordSet("name", value);
		}
	}

	/// <summary>
	/// Whether the object gets rendered into the shadow map. Writing it records a <c>castShadow</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool CastShadow
	{
		get { return _castShadow; }
		set
		{
			if (_castShadow == value)
			{
				return;
			}

			_castShadow = value;
			_isCastShadowWritten = true;
			RecordSet("castShadow", value);
		}
	}

	/// <summary>
	/// Whether the material receives shadows. Writing it records a <c>receiveShadow</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool ReceiveShadow
	{
		get { return _receiveShadow; }
		set
		{
			if (_receiveShadow == value)
			{
				return;
			}

			_receiveShadow = value;
			_isReceiveShadowWritten = true;
			RecordSet("receiveShadow", value);
		}
	}

	/// <summary>
	/// When this is set, it checks every frame if the object is in the frustum of the camera before
	/// rendering the object. If set to <see langword="false"/> the object gets rendered every frame even
	/// if it is not in the frustum of the camera. Writing it records a <c>frustumCulled</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool FrustumCulled
	{
		get { return _frustumCulled; }
		set
		{
			if (_frustumCulled == value)
			{
				return;
			}

			_frustumCulled = value;
			_isFrustumCulledWritten = true;
			RecordSet("frustumCulled", value);
		}
	}

	/// <summary>
	/// Overrides the default rendering order of scene graph objects, although opaque and transparent
	/// objects remain sorted independently. Sorting is from lowest to highest; setting it on a
	/// <c>Group</c> sorts and renders all of its descendants together. Writing it records a
	/// <c>renderOrder</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float RenderOrder
	{
		get { return _renderOrder; }
		set
		{
			if (_renderOrder == value)
			{
				return;
			}

			_renderOrder = value;
			_isRenderOrderWritten = true;
			RecordSet("renderOrder", value);
		}
	}

	/// <summary>
	/// When this is set, three.js calculates the matrix of position, rotation and scale every frame, and
	/// also recalculates the world matrix. Writing it records a <c>matrixAutoUpdate</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool MatrixAutoUpdate
	{
		get { return _matrixAutoUpdate; }
		set
		{
			if (_matrixAutoUpdate == value)
			{
				return;
			}

			_matrixAutoUpdate = value;
			_isMatrixAutoUpdateWritten = true;
			RecordSet("matrixAutoUpdate", value);
		}
	}

	/// <summary>
	/// If set, the renderer checks every frame whether the object and its children need matrix updates.
	/// When it is not set, you have to maintain all matrices in the object and its children yourself.
	/// Writing it records a <c>matrixWorldAutoUpdate</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public bool MatrixWorldAutoUpdate
	{
		get { return _matrixWorldAutoUpdate; }
		set
		{
			if (_matrixWorldAutoUpdate == value)
			{
				return;
			}

			_matrixWorldAutoUpdate = value;
			_isMatrixWorldAutoUpdateWritten = true;
			RecordSet("matrixWorldAutoUpdate", value);
		}
	}

	/// <summary>
	/// When this is set, three.js calculates the world matrix in that frame and resets the property to
	/// <see langword="false"/> — on its side only, since nothing reads back, so C# goes on reporting the
	/// value it last wrote. Writing it records a <c>matrixWorldNeedsUpdate</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public bool MatrixWorldNeedsUpdate
	{
		get { return _matrixWorldNeedsUpdate; }
		set
		{
			if (_matrixWorldNeedsUpdate == value)
			{
				return;
			}

			_matrixWorldNeedsUpdate = value;
			_isMatrixWorldNeedsUpdateWritten = true;
			RecordSet("matrixWorldNeedsUpdate", value);
		}
	}

	/// <summary>
	/// The layer membership of the object. The object is only visible if it shares at least one layer
	/// with the camera in use, and layers also filter ray-intersection tests. Assigning one attaches it
	/// to the same context as this object. Writing it records a <c>layers</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public Layers? Layers
	{
		get { return _layers; }
		set
		{
			if (ReferenceEquals(_layers, value))
			{
				return;
			}

			_layers = value;
			_isLayersWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("layers", value);
		}
	}

	/// <summary>
	/// Custom depth material to be used when rendering to the depth map. Only meaningful on meshes: when
	/// shadow-casting with a directional or spot light and moving vertex positions in the vertex shader,
	/// this is what gives the shadow the same displacement. Assigning one attaches it to the same context
	/// as this object. Writing it records a <c>customDepthMaterial</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public Material? CustomDepthMaterial
	{
		get { return _customDepthMaterial; }
		set
		{
			if (ReferenceEquals(_customDepthMaterial, value))
			{
				return;
			}

			_customDepthMaterial = value;
			_isCustomDepthMaterialWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("customDepthMaterial", value);
		}
	}

	/// <summary>
	/// Same as <see cref="CustomDepthMaterial"/>, but used with a point light. Assigning one attaches it
	/// to the same context as this object. Writing it records a <c>customDistanceMaterial</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Material? CustomDistanceMaterial
	{
		get { return _customDistanceMaterial; }
		set
		{
			if (ReferenceEquals(_customDistanceMaterial, value))
			{
				return;
			}

			_customDistanceMaterial = value;
			_isCustomDistanceMaterialWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("customDistanceMaterial", value);
		}
	}

	/// <summary>
	/// Raised when the user clicks this object — this object, not the canvas: the browser casts a ray
	/// through the pointer and only an object the ray actually met raises anything. A click on empty
	/// space, or on an object nobody subscribed to, sends nothing at all.
	/// <para>
	/// Subscribing is what opts the object into hit-testing, and unsubscribing the last handler opts it
	/// back out, so hit-testing costs only what the objects with a subscriber cost. Both record an op
	/// that travels with the next flush, exactly like a property write.
	/// </para>
	/// <para>
	/// Only this object's own geometry is hit-tested. Subscribing on a parent does not make its
	/// children clickable — subscribe on the objects that have the geometry you want clicked.
	/// </para>
	/// <para>
	/// The handler runs on the renderer's synchronization context, the same one a DOM event handler
	/// runs on, so calling <c>StateHasChanged</c> from it needs no dispatching on any of the three
	/// hosting models. Scene-graph changes the handler makes are flushed for it once every handler has
	/// run, so it does not have to await anything; a handler that changes nothing costs no interop,
	/// because a flush with nothing pending makes no call.
	/// </para>
	/// </summary>
	public event Action<ThreePointerEvent>? OnClick
	{
		add
		{
			var wasSubscribed = _onClick is not null;
			_onClick += value;
			if (!wasSubscribed && _onClick is not null)
			{
				RecordPointerSubscription(true);
			}
		}
		remove
		{
			var wasSubscribed = _onClick is not null;
			_onClick -= value;
			if (wasSubscribed && _onClick is null)
			{
				RecordPointerSubscription(false);
			}
		}
	}

	/// <summary>
	/// Initializes the transform with position at the origin, no rotation, and unit scale, and wires
	/// each component's <c>OnChange</c> callback to record a property write. The values match three.js's
	/// own defaults, so an object nobody has configured mirrors one three.js just constructed.
	/// </summary>
	protected Object3D()
	{
		WireTransformChangeTracking();
	}

	/// <summary>
	/// Initializes the transform exactly as the parameterless constructor does, for an object the
	/// browser created and minted a handle for. See <see cref="ThreeObject(int)"/> for why minting is
	/// not part of the surface a consumer can reach.
	/// </summary>
	/// <param name="handle">The negative handle the JavaScript side registered the object under.</param>
	private protected Object3D(int handle)
		: base(handle)
	{
		WireTransformChangeTracking();
	}

	/// <summary>
	/// Points each transform component's <c>OnChange</c> callback at the property write it stands for,
	/// so writing <c>mesh.Position.X</c> records a <c>position</c> write without the owner watching
	/// each component.
	/// </summary>
	private void WireTransformChangeTracking()
	{
		Position.OnChange = () =>
		{
			_isPositionWritten = true;
			RecordSet("position", Position);
		};

		Rotation.OnChange = () =>
		{
			_isRotationWritten = true;
			RecordSet("rotation", Rotation);
		};

		Scale.OnChange = () =>
		{
			_isScaleWritten = true;
			RecordSet("scale", Scale);
		};

		Quaternion.OnChange = () =>
		{
			_isQuaternionWritten = true;
			RecordSet("quaternion", Quaternion);
		};

		Up.OnChange = () =>
		{
			_isUpWritten = true;
			RecordSet("up", Up);
		};

		Pivot.OnChange = () =>
		{
			_isPivotWritten = true;
			RecordSet("pivot", Pivot);
		};
	}

	/// <summary>
	/// Adds a child object to this object. If this object is already attached to a batch, the child
	/// is attached too and the parent/child relationship is recorded immediately; otherwise the
	/// relationship is replayed when this object is later attached.
	/// </summary>
	/// <param name="child">The object to add as a child.</param>
	public void Add(Object3D child)
	{
		_children.Add(child);
		if (Batch is not null)
		{
			child.AttachTo(Batch);
			Batch.Add(Handle, child.Handle);
		}
	}

	/// <summary>
	/// Detaches a child added by <see cref="Add"/>. Records the detach op once this object is attached
	/// to a batch; before that there is nothing on the JavaScript side to detach from, so the child is
	/// simply dropped from the list <see cref="AttachTo"/> would have replayed.
	/// <para>
	/// Detaching does not release the child. The child keeps its handle and its JavaScript-side object,
	/// so it can be added somewhere else; releasing it is <see cref="ThreeObject.RetireHandle"/>'s job.
	/// </para>
	/// <para>
	/// A no-op for an object that is not a child of this one, which is what lets a caller detach
	/// defensively without first checking <see cref="Children"/>.
	/// </para>
	/// </summary>
	/// <param name="child">The object to detach.</param>
	public void Remove(Object3D child)
	{
		if (!_children.Remove(child))
		{
			return;
		}

		Batch?.Remove(Handle, child.Handle);
	}

	/// <summary>
	/// Orients this object to face the given point in world space.
	/// <para>
	/// three.js declares <c>lookAt</c> on <c>Object3D</c> rather than on the camera, so every object in
	/// the graph has it. It lives here rather than in generated code because the generator erases
	/// overloads to the first signature, which upstream is the <c>Vector3</c> one — and this component
	/// spelling is the shape already published.
	/// </para>
	/// <para>
	/// ⚠️ <b>This leaves the mirror stale.</b> three.js computes the new orientation into the object's
	/// <c>rotation</c> and <c>quaternion</c>; the call records a command and nothing reads the result
	/// back, so <see cref="Rotation"/> and <see cref="Quaternion"/> go on reporting their pre-call
	/// values. Writing one of those values back therefore records <b>nothing</b> — the mirror sees the
	/// value it already holds and elides the write — and three.js keeps the <c>lookAt</c> orientation
	/// permanently. Write a different value, or set <see cref="Rotation"/> instead of calling this.
	/// Pinned by <c>Object3DTests.Object3D_LookAtThenRewritingTheRotationTheMirrorStillHolds_RecordsNothing</c>.
	/// </para>
	/// </summary>
	/// <param name="x">X coordinate of the point to look at.</param>
	/// <param name="y">Y coordinate of the point to look at.</param>
	/// <param name="z">Z coordinate of the point to look at.</param>
	public void LookAt(float x, float y, float z)
	{
		RecordCall("lookAt", x, y, z);
	}

	/// <summary>
	/// Attaches this object and its entire subtree to a batch: runs the base attach sequence — create
	/// op, then <see cref="EmitState"/>, then the commands invoked before the attach — and attaches
	/// each child in turn afterwards. Idempotent — a second call on an already-attached object is a
	/// no-op. Internal because the only entry point a consumer needs is
	/// <see cref="ThreeContext.Attach"/>, which calls this on the root object on their behalf.
	/// Overrides <see cref="ThreeObject.AttachTo"/> to layer child attachment on top of the base
	/// create-op guard.
	/// </summary>
	/// <param name="batch">The batch to attach this object to.</param>
	/// <exception cref="InvalidOperationException">
	/// Thrown when this object is already attached to a different batch. The check is repeated here
	/// rather than left to the base implementation, because this override returns before reaching it.
	/// </exception>
	internal override void AttachTo(ThreeBatch batch)
	{
		ThrowIfAttachedToAnotherBatch(batch);
		if (Batch is not null)
		{
			return;
		}

		base.AttachTo(batch);

		foreach (var child in _children)
		{
			child.AttachTo(batch);
			batch.Add(Handle, child.Handle);
		}
	}

	/// <summary>
	/// Replays properties that were written before this object was attached, so construction order
	/// never matters to the caller.
	/// <para>
	/// Every value here is replayed only when it was actually written. A value nobody set is this
	/// class's guess at a three.js default rather than something it read back, and writing a guess is
	/// how <c>HemisphereLight</c> came to render whole scenes black: three.js constructs it, and
	/// <c>DirectionalLight</c> and <c>SpotLight</c>, with <c>position</c> at <c>(0, 1, 0)</c> rather
	/// than the origin, because for those three the position <em>is</em> the direction they light
	/// from. Replaying an unwritten <c>(0, 0, 0)</c> over that left a zero-length direction and no
	/// light at all, silently — see <c>Object3DTransformReplayTests</c>.
	/// </para>
	/// <para>
	/// <c>quaternion</c> lands after <c>rotation</c> because three.js derives each from the other, and
	/// the one the caller actually wrote has to be the one applied last.
	/// </para>
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		if (_isPositionWritten)
		{
			batch.Set(Handle, "position", ThreeValue.Encode(Position));
		}

		if (_isRotationWritten)
		{
			batch.Set(Handle, "rotation", ThreeValue.Encode(Rotation));
		}

		if (_isScaleWritten)
		{
			batch.Set(Handle, "scale", ThreeValue.Encode(Scale));
		}

		if (_isVisibleWritten)
		{
			batch.Set(Handle, "visible", ThreeValue.Encode(_isVisible));
		}

		if (_isQuaternionWritten)
		{
			batch.Set(Handle, "quaternion", ThreeValue.Encode(Quaternion));
		}

		if (_isUpWritten)
		{
			batch.Set(Handle, "up", ThreeValue.Encode(Up));
		}

		if (_isPivotWritten)
		{
			batch.Set(Handle, "pivot", ThreeValue.Encode(Pivot));
		}

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isCastShadowWritten)
		{
			batch.Set(Handle, "castShadow", ThreeValue.Encode(_castShadow));
		}

		if (_isReceiveShadowWritten)
		{
			batch.Set(Handle, "receiveShadow", ThreeValue.Encode(_receiveShadow));
		}

		if (_isFrustumCulledWritten)
		{
			batch.Set(Handle, "frustumCulled", ThreeValue.Encode(_frustumCulled));
		}

		if (_isRenderOrderWritten)
		{
			batch.Set(Handle, "renderOrder", ThreeValue.Encode(_renderOrder));
		}

		if (_isMatrixAutoUpdateWritten)
		{
			batch.Set(Handle, "matrixAutoUpdate", ThreeValue.Encode(_matrixAutoUpdate));
		}

		if (_isMatrixWorldAutoUpdateWritten)
		{
			batch.Set(Handle, "matrixWorldAutoUpdate", ThreeValue.Encode(_matrixWorldAutoUpdate));
		}

		if (_isMatrixWorldNeedsUpdateWritten)
		{
			batch.Set(Handle, "matrixWorldNeedsUpdate", ThreeValue.Encode(_matrixWorldNeedsUpdate));
		}

		// The three object-typed properties attach their value first: a $ref to a handle the applier
		// has never seen is an unknown-handle failure in the browser, and a value assigned before this
		// object reached a batch has not been attached by its setter.
		if (_isLayersWritten)
		{
			_layers?.AttachTo(batch);
			batch.Set(Handle, "layers", ThreeValue.Encode(_layers));
		}

		if (_isCustomDepthMaterialWritten)
		{
			_customDepthMaterial?.AttachTo(batch);
			batch.Set(Handle, "customDepthMaterial", ThreeValue.Encode(_customDepthMaterial));
		}

		if (_isCustomDistanceMaterialWritten)
		{
			_customDistanceMaterial?.AttachTo(batch);
			batch.Set(Handle, "customDistanceMaterial", ThreeValue.Encode(_customDistanceMaterial));
		}

		// A handler subscribed before the attach recorded nothing at the time, for the same reason a
		// property write did: there was no batch to record into. This is where it lands.
		if (_onClick is not null)
		{
			batch.Pick(Handle, true);
			batch.Context?.RegisterPointerTarget(this);
		}
	}

	/// <summary>
	/// Raises <see cref="OnClick"/> for a hit the browser reported. Called only by
	/// <see cref="ThreeContext"/>, which resolves the handle in the callback back to this object, and
	/// only for an object that opted in — an object with no subscriber is not in the applier's
	/// candidate set, so no ray can reach it.
	/// </summary>
	/// <param name="pointerEvent">Where the ray met this object.</param>
	internal void RaiseClick(ThreePointerEvent pointerEvent)
	{
		_onClick?.Invoke(pointerEvent);
	}

	/// <summary>
	/// Records an opt-in or opt-out with both halves of the machinery: the op that tells the applier
	/// which objects to hit-test, and the context's own handle-to-object table, which is what turns
	/// the handle in an incoming callback back into the object whose event to raise.
	/// <para>
	/// A no-op before this object is attached — there is no batch to record into and no context to
	/// register with. <see cref="EmitState"/> replays the subscription once there is.
	/// </para>
	/// </summary>
	/// <param name="isPointerTarget">Whether the object is opting in or out.</param>
	private void RecordPointerSubscription(bool isPointerTarget)
	{
		var batch = Batch;
		if (batch is null)
		{
			return;
		}

		batch.Pick(Handle, isPointerTarget);
		if (batch.Context is not { } context)
		{
			return;
		}

		if (isPointerTarget)
		{
			context.RegisterPointerTarget(this);
			return;
		}

		context.UnregisterPointerTarget(this);
	}
}
