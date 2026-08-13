// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A mesh that has a <c>Skeleton</c> with <see cref="Bone">bones</see> that can then be used to
/// animate the vertices of the geometry. The JavaScript-side <c>THREE.SkinnedMesh</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/SkinnedMesh">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/SkinnedMesh.js">Source</seealso>
public sealed class SkinnedMesh : Mesh
{
	private readonly BufferGeometry? _geometry;
	private readonly Material? _material;
	private readonly bool? _useVertexTexture;
	private BindMode _bindMode = BindMode.AttachedBindMode;
	private Skeleton? _skeleton;
	private bool _isBindModeWritten;
	private bool _isBoundingBoxWritten;
	private bool _isBoundingSphereWritten;
	private bool _isSkeletonWritten;

	/// <summary>
	/// The bounding box of the SkinnedMesh. Can be calculated with <c>.computeBoundingBox()</c>.
	/// Mirrored as an instance this object owns: mutating it records a write of <c>boundingBox</c>.
	/// </summary>
	public Box3 BoundingBox { get; }

	/// <summary>
	/// The bounding box of the SkinnedMesh. Can be calculated with <c>.computeBoundingSphere()</c>.
	/// Mirrored as an instance this object owns: mutating it records a write of <c>boundingSphere</c>.
	/// </summary>
	public Sphere BoundingSphere { get; }

	/// <summary>Create a new instance of <see cref="SkinnedMesh"/>.</summary>
	/// <param name="geometry">
	/// An instance of <c>BufferGeometry</c>. Default <c><c>new THREE.BufferGeometry()</c></c>.
	/// </param>
	/// <param name="material">
	/// A single or an array of <c>Material</c>. Default <c><c>new THREE.MeshBasicMaterial()</c></c>.
	/// </param>
	/// <param name="useVertexTexture">Value forwarded to the <c>useVertexTexture</c> constructor argument.</param>
	public SkinnedMesh(BufferGeometry? geometry = null, Material? material = null, bool? useVertexTexture = null)
	{
		_geometry = geometry;
		_material = material;
		_useVertexTexture = useVertexTexture;

		BoundingBox = new Box3();
		BoundingBox.OnChange = () =>
		{
			_isBoundingBoxWritten = true;
			RecordSet("boundingBox", BoundingBox);
		};

		BoundingSphere = new Sphere();
		BoundingSphere.OnChange = () =>
		{
			_isBoundingSphereWritten = true;
			RecordSet("boundingSphere", BoundingSphere);
		};
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>SkinnedMesh</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal SkinnedMesh(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		BoundingBox = new Box3();
		BoundingBox.OnChange = () =>
		{
			_isBoundingBoxWritten = true;
			RecordSet("boundingBox", BoundingBox);
		};

		BoundingSphere = new Sphere();
		BoundingSphere.OnChange = () =>
		{
			_isBoundingSphereWritten = true;
			RecordSet("boundingSphere", BoundingSphere);
		};

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.SkinnedMesh</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "SkinnedMesh"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.SkinnedMesh</c>: geometry, material,
	/// useVertexTexture. An argument the caller left unspecified travels as the wire's not-supplied
	/// sentinel, or is trimmed when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_geometry),
				ThreeValue.OrUnspecified(_material),
				ThreeValue.OrUnspecified(_useVertexTexture)
			]);
		}
	}

	/// <summary>
	/// Either <c>AttachedBindMode</c> or <c>DetachedBindMode</c>. <c>AttachedBindMode</c> means the
	/// skinned mesh shares the same world space as the skeleton. This is not true when using
	/// <c>DetachedBindMode</c> which is useful when sharing a skeleton across multiple skinned meshes.
	/// Writing it records a <c>bindMode</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public BindMode BindMode
	{
		get { return _bindMode; }
		set
		{
			if (_bindMode == value)
			{
				return;
			}

			_bindMode = value;
			_isBindModeWritten = true;
			RecordSet("bindMode", value);
		}
	}

	/// <summary>
	/// <c>Skeleton</c> representing the bone hierarchy of the skinned mesh. Writing it records a
	/// <c>skeleton</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public Skeleton? Skeleton
	{
		get { return _skeleton; }
		set
		{
			if (ReferenceEquals(_skeleton, value))
			{
				return;
			}

			_skeleton = value;
			_isSkeletonWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("skeleton", value);
		}
	}

	/// <summary>Bind a skeleton to the skinned mesh.</summary>
	/// <param name="skeleton"><c>Skeleton</c> created from a <see cref="Bone">Bones</see> tree.</param>
	/// <param name="bindMatrix"><c>Matrix4</c> that represents the base transform of the skeleton.</param>
	public void Bind(Skeleton skeleton, Matrix4 bindMatrix)
	{
		RecordCall("bind", skeleton, bindMatrix);
	}

	/// <summary>
	/// Computes the bounding box of the skinned mesh, and updates the <c>.boundingBox</c> attribute.
	/// The bounding box is not computed by the engine; it must be computed by your app. If the skinned
	/// mesh is animated, the bounding box should be recomputed per frame.
	/// </summary>
	public void ComputeBoundingBox()
	{
		RecordCall("computeBoundingBox");
	}

	/// <summary>
	/// Computes the bounding sphere of the skinned mesh, and updates the <c>.boundingSphere</c>
	/// attribute. The bounding sphere is automatically computed by the engine when it is needed, e.g.,
	/// for ray casting and view frustum culling. If the skinned mesh is animated, the bounding sphere
	/// should be recomputed per frame.
	/// </summary>
	public void ComputeBoundingSphere()
	{
		RecordCall("computeBoundingSphere");
	}

	/// <summary>This method sets the skinned mesh in the rest pose (resets the pose).</summary>
	public void Pose()
	{
		RecordCall("pose");
	}

	/// <summary>Normalizes the skin weights.</summary>
	public void NormalizeSkinWeights()
	{
		RecordCall("normalizeSkinWeights");
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="SkinnedMesh"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isSkinnedMesh</c> held.
	/// </summary>
	/// <returns>The value <c>isSkinnedMesh</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsSkinnedMeshAsync()
	{
		return GetAsync<bool>("isSkinnedMesh");
	}

	/// <summary>
	/// Applies the bone transform associated with the given index to the given position vector. Records
	/// a read op, sends it behind every write already pending, and completes with what
	/// <c>applyBoneTransform</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <param name="vector">Value forwarded to the <c>vector</c> argument.</param>
	/// <returns>The value <c>applyBoneTransform</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector3> ApplyBoneTransformAsync(int index, Vector3 vector)
	{
		return RecordRead<Vector3>("applyBoneTransform", index, vector);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.SkinnedMesh</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_geometry?.AttachTo(batch);
		_material?.AttachTo(batch);

		base.EmitCreate(batch);
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that. A replayed
	/// value that is itself a mirrored object is attached first, so its create op reaches the batch
	/// before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isBindModeWritten)
		{
			batch.Set(Handle, "bindMode", ThreeValue.Encode(_bindMode));
		}

		if (_isBoundingBoxWritten)
		{
			batch.Set(Handle, "boundingBox", ThreeValue.Encode(BoundingBox));
		}

		if (_isBoundingSphereWritten)
		{
			batch.Set(Handle, "boundingSphere", ThreeValue.Encode(BoundingSphere));
		}

		if (_isSkeletonWritten)
		{
			_skeleton?.AttachTo(batch);
			batch.Set(Handle, "skeleton", ThreeValue.Encode(_skeleton));
		}
	}
}
