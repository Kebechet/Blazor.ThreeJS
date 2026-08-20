// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A special version of <see cref="Mesh"/> with multi draw batch rendering support. Use BatchedMesh
/// if you have to render a large number of objects with the same material but with different
/// geometries or world transformations. The usage of BatchedMesh will help you to reduce the number
/// of draw calls and thus improve the overall rendering performance in your application. If the
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/WEBGL_multi_draw">WEBGL_multi_draw
/// extension</see> is not supported then a less performant fallback is used. The JavaScript-side
/// <c>THREE.BatchedMesh</c>.
/// </summary>
public sealed class BatchedMesh : Mesh
{
	private readonly int _maxInstanceCount;
	private readonly int _maxVertexCount;
	private readonly int? _maxIndexCount;
	private readonly Material? _material;
	private bool _perObjectFrustumCulled = true;
	private bool _sortObjects = true;
	private bool _isBoundingBoxWritten;
	private bool _isBoundingSphereWritten;
	private bool _isPerObjectFrustumCulledWritten;
	private bool _isSortObjectsWritten;

	/// <summary>
	/// This bounding box encloses all instances of the <see cref="BatchedMesh"/>. Can be calculated
	/// with <c>.computeBoundingBox()</c>. Mirrored as an instance this object owns: mutating it records
	/// a write of <c>boundingBox</c>.
	/// </summary>
	public Box3 BoundingBox { get; }

	/// <summary>
	/// This bounding sphere encloses all instances of the <see cref="BatchedMesh"/>. Can be calculated
	/// with <c>.computeBoundingSphere()</c>. Mirrored as an instance this object owns: mutating it
	/// records a write of <c>boundingSphere</c>.
	/// </summary>
	public Sphere BoundingSphere { get; }

	/// <summary>Initializes a new <see cref="BatchedMesh"/>.</summary>
	/// <param name="maxInstanceCount">the max number of individual geometries planned to be added.</param>
	/// <param name="maxVertexCount">the max number of vertices to be used by all geometries.</param>
	/// <param name="maxIndexCount">the max number of indices to be used by all geometries.</param>
	/// <param name="material">
	/// an instance of <see cref="Material"/>. Default is a new <see cref="MeshBasicMaterial"/>.
	/// </param>
	public BatchedMesh(int maxInstanceCount, int maxVertexCount, int? maxIndexCount = null, Material? material = null)
		: base(material: material)
	{
		_maxInstanceCount = maxInstanceCount;
		_maxVertexCount = maxVertexCount;
		_maxIndexCount = maxIndexCount;
		_material = material;

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
	/// Adopts an existing JavaScript-side <c>BatchedMesh</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal BatchedMesh(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_maxInstanceCount = default!;
		_maxVertexCount = default!;

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

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BatchedMesh</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "BatchedMesh"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.BatchedMesh</c>: maxInstanceCount, maxVertexCount,
	/// maxIndexCount, material. An argument the caller left unspecified travels as the wire's
	/// not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js applies its
	/// own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				_maxInstanceCount,
				_maxVertexCount,
				ThreeValue.OrUnspecified(_maxIndexCount),
				ThreeValue.OrUnspecified(_material)
			]);
		}
	}

	/// <summary>
	/// If true then the individual objects within the <see cref="BatchedMesh"/> are frustum culled.
	/// Writing it records a <c>perObjectFrustumCulled</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public bool PerObjectFrustumCulled
	{
		get { return _perObjectFrustumCulled; }
		set
		{
			if (_perObjectFrustumCulled == value)
			{
				return;
			}

			_perObjectFrustumCulled = value;
			_isPerObjectFrustumCulledWritten = true;
			RecordSet("perObjectFrustumCulled", value);
		}
	}

	/// <summary>
	/// If true then the individual objects within the <see cref="BatchedMesh"/> are sorted to improve
	/// overdraw-related artifacts. If the material is marked as "transparent" objects are rendered back
	/// to front and if not then they are rendered front to back. Writing it records a
	/// <c>sortObjects</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool SortObjects
	{
		get { return _sortObjects; }
		set
		{
			if (_sortObjects == value)
			{
				return;
			}

			_sortObjects = value;
			_isSortObjectsWritten = true;
			RecordSet("sortObjects", value);
		}
	}

	/// <summary>
	/// Computes the bounding box, updating <c>.boundingBox</c> attribute. Bounding boxes aren't
	/// computed by default. They need to be explicitly computed, otherwise they are <c>null</c>.
	/// </summary>
	public void ComputeBoundingBox()
	{
		RecordCall("computeBoundingBox");
	}

	/// <summary>
	/// Computes the bounding sphere, updating <c>.boundingSphere</c> attribute. Bounding spheres aren't
	/// computed by default. They need to be explicitly computed, otherwise they are <c>null</c>.
	/// </summary>
	public void ComputeBoundingSphere()
	{
		RecordCall("computeBoundingSphere");
	}

	/// <summary>
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Sets the given color to the defined geometry instance. This overload takes <c>color</c> as
	/// <c>Color</c> out of three.js's <c>Color | Vector4</c>.
	/// </summary>
	/// <param name="instanceId">The id of the instance to set the color of.</param>
	/// <param name="color">The color to set the instance to. Use a <c>Vector4</c> to also define alpha.</param>
	public void SetColorAt(float instanceId, Color color)
	{
		RecordCall("setColorAt", instanceId, color);
	}

	/// <summary>
	/// Sets the given color to the defined geometry instance. This overload takes <c>color</c> as
	/// <c>Vector4</c> out of three.js's <c>Color | Vector4</c>.
	/// </summary>
	/// <param name="instanceId">The id of the instance to set the color of.</param>
	/// <param name="color">The color to set the instance to. Use a <c>Vector4</c> to also define alpha.</param>
	public void SetColorAt(float instanceId, Vector4 color)
	{
		RecordCall("setColorAt", instanceId, color);
	}

	/// <summary>
	/// Sets the given local transformation matrix to the defined instance. Negatively scaled matrices
	/// are not supported.
	/// </summary>
	/// <param name="instanceId">The id of an instance to set the matrix of.</param>
	/// <param name="matrix">A 4x4 matrix representing the local transformation of a single instance.</param>
	public void SetMatrixAt(float instanceId, Matrix4 matrix)
	{
		RecordCall("setMatrixAt", instanceId, matrix);
	}

	/// <summary>Sets the visibility of the instance at the given index.</summary>
	/// <param name="instanceId">The id of the instance to set the visibility of.</param>
	/// <param name="visible">A boolean value indicating the visibility state.</param>
	public void SetVisibleAt(float instanceId, bool visible)
	{
		RecordCall("setVisibleAt", instanceId, visible);
	}

	/// <summary>Sets the geometryIndex of the instance at the given index.</summary>
	/// <param name="instanceId">The id of the instance to set the geometryIndex of.</param>
	/// <param name="geometryId">The geometryIndex to be use by the instance.</param>
	public void SetGeometryIdAt(float instanceId, float geometryId)
	{
		RecordCall("setGeometryIdAt", instanceId, geometryId);
	}

	/// <summary>Records a call to <c>deleteGeometry</c> on the JavaScript-side object.</summary>
	/// <param name="geometryId">
	/// The id of a geometry to remove from the [name] that was previously added via "addGeometry". Any
	/// instances referencing this geometry will also be removed as a side effect.
	/// </param>
	public void DeleteGeometry(float geometryId)
	{
		RecordCall("deleteGeometry", geometryId);
	}

	/// <summary>Removes an existing instance from the BatchedMesh using the given instanceId.</summary>
	/// <param name="instanceId">
	/// The id of an instance to remove from the BatchedMesh that was previously added via
	/// "addInstance".
	/// </param>
	public void DeleteInstance(float instanceId)
	{
		RecordCall("deleteInstance", instanceId);
	}

	/// <summary>
	/// Resizes the available space in BatchedMesh's vertex and index buffer attributes to the provided
	/// sizes. If the provided arguments shrink the geometry buffers but there is not enough unused
	/// space at the end of the geometry attributes then an error is thrown.
	/// </summary>
	/// <param name="maxVertexCount">
	/// the max number of vertices to be used by all unique geometries to resize to.
	/// </param>
	/// <param name="maxIndexCount">
	/// the max number of indices to be used by all unique geometries to resize to.
	/// </param>
	public void SetGeometrySize(int maxVertexCount, int maxIndexCount)
	{
		RecordCall("setGeometrySize", maxVertexCount, maxIndexCount);
	}

	/// <summary>
	/// Resizes the necessary buffers to support the provided number of instances. If the provided
	/// arguments shrink the number of instances but there are not enough unused ids at the end of the
	/// list then an error is thrown.
	/// </summary>
	/// <param name="maxInstanceCount">
	/// the max number of individual instances that can be added and rendered by the BatchedMesh.
	/// </param>
	public void SetInstanceCount(int maxInstanceCount)
	{
		RecordCall("setInstanceCount", maxInstanceCount);
	}

	/// <summary>
	/// The maximum number of individual geometries that can be stored in the <see cref="BatchedMesh"/>.
	/// Read only. Read-only in three.js, so it is read on demand rather than mirrored: records a get
	/// op, sends it behind every write already pending, and completes with the value
	/// <c>maxInstanceCount</c> held.
	/// </summary>
	/// <returns>The value <c>maxInstanceCount</c> held, once the JavaScript side has answered.</returns>
	public Task<int> MaxInstanceCountAsync()
	{
		return GetAsync<int>("maxInstanceCount");
	}

	/// <summary>
	/// Reads <c>instanceCount</c> back from the JavaScript-side object. Read-only in three.js, so it is
	/// read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>instanceCount</c> held.
	/// </summary>
	/// <returns>The value <c>instanceCount</c> held, once the JavaScript side has answered.</returns>
	public Task<int> InstanceCountAsync()
	{
		return GetAsync<int>("instanceCount");
	}

	/// <summary>
	/// Reads <c>unusedVertexCount</c> back from the JavaScript-side object. Read-only in three.js, so
	/// it is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>unusedVertexCount</c> held.
	/// </summary>
	/// <returns>The value <c>unusedVertexCount</c> held, once the JavaScript side has answered.</returns>
	public Task<int> UnusedVertexCountAsync()
	{
		return GetAsync<int>("unusedVertexCount");
	}

	/// <summary>
	/// Reads <c>unusedIndexCount</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>unusedIndexCount</c> held.
	/// </summary>
	/// <returns>The value <c>unusedIndexCount</c> held, once the JavaScript side has answered.</returns>
	public Task<int> UnusedIndexCountAsync()
	{
		return GetAsync<int>("unusedIndexCount");
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="BatchedMesh"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isBatchedMesh</c> held.
	/// </summary>
	/// <returns>The value <c>isBatchedMesh</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsBatchedMeshAsync()
	{
		return GetAsync<bool>("isBatchedMesh");
	}

	/// <summary>
	/// Get the color of the defined geometry. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getColorAt</c> returned.
	/// </summary>
	/// <param name="instanceId">The id of an instance to get the color of.</param>
	/// <param name="color">The target object that is used to store the method's result.</param>
	/// <returns>The value <c>getColorAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<Color> GetColorAtAsync(float instanceId, Color color)
	{
		return RecordRead<Color>("getColorAt", instanceId, color);
	}

	/// <summary>
	/// Get the local transformation matrix of the defined instance. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>getMatrixAt</c> returned.
	/// </summary>
	/// <param name="instanceId">The id of an instance to get the matrix of.</param>
	/// <param name="target">
	/// This 4x4 matrix will be set to the local transformation matrix of the defined instance.
	/// </param>
	/// <returns>The value <c>getMatrixAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<Matrix4> GetMatrixAtAsync(float instanceId, Matrix4 target)
	{
		return RecordRead<Matrix4>("getMatrixAt", instanceId, target);
	}

	/// <summary>
	/// Get whether the given instance is marked as "visible" or not. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>getVisibleAt</c> returned.
	/// </summary>
	/// <param name="instanceId">The id of an instance to get the visibility state of.</param>
	/// <returns>The value <c>getVisibleAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> GetVisibleAtAsync(float instanceId)
	{
		return RecordRead<bool>("getVisibleAt", instanceId);
	}

	/// <summary>
	/// Get the range representing the subset of triangles related to the attached geometry, indicating
	/// the starting offset and count, or <c>null</c> if invalid. Return an object of the form: { start:
	/// Integer, count: Integer }. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>getGeometryRangeAt</c> returned.
	/// </summary>
	/// <param name="geometryId">The id of the geometry to get the range of.</param>
	/// <param name="target">Optional target object to copy the range in to.</param>
	/// <returns>The value <c>getGeometryRangeAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<BatchedMeshGeometryRange> GetGeometryRangeAtAsync(float geometryId, BatchedMeshGeometryRange target)
	{
		return RecordRead<BatchedMeshGeometryRange>("getGeometryRangeAt", geometryId, target);
	}

	/// <summary>
	/// Get the geometryIndex of the defined instance. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>getGeometryIdAt</c> returned.
	/// </summary>
	/// <param name="instanceId">The id of an instance to get the geometryIndex of.</param>
	/// <returns>The value <c>getGeometryIdAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetGeometryIdAtAsync(float instanceId)
	{
		return RecordRead<float>("getGeometryIdAt", instanceId);
	}

	/// <summary>
	/// Adds the given geometry to the <see cref="BatchedMesh"/> and returns the associated index
	/// referring to it. Records a read op, sends it behind every write already pending, and completes
	/// with what <c>addGeometry</c> returned.
	/// </summary>
	/// <param name="geometry">The geometry to add into the <see cref="BatchedMesh"/>.</param>
	/// <param name="reservedVertexRange">
	/// Optional parameter specifying the amount of vertex buffer space to reserve for the added
	/// geometry. This is necessary if it is planned to set a new geometry at this index at a later time
	/// that is larger than the original geometry. Defaults to the length of the given geometry vertex
	/// buffer.
	/// </param>
	/// <param name="reservedIndexRange">
	/// Optional parameter specifying the amount of index buffer space to reserve for the added
	/// geometry. This is necessary if it is planned to set a new geometry at this index at a later time
	/// that is larger than the original geometry. Defaults to the length of the given geometry index
	/// buffer.
	/// </param>
	/// <returns>The value <c>addGeometry</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> AddGeometryAsync(BufferGeometry geometry, float reservedVertexRange, float reservedIndexRange)
	{
		return RecordRead<float>("addGeometry", geometry, reservedVertexRange, reservedIndexRange);
	}

	/// <summary>
	/// Adds a new instance to the <see cref="BatchedMesh"/> using the geometry of the given geometryId
	/// and returns a new id referring to the new instance to be used by other functions. Records a read
	/// op, sends it behind every write already pending, and completes with what <c>addInstance</c>
	/// returned.
	/// </summary>
	/// <param name="geometryId">
	/// The id of a previously added geometry via "addGeometry" to add into the
	/// <see cref="BatchedMesh"/> to render.
	/// </param>
	/// <returns>The value <c>addInstance</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> AddInstanceAsync(float geometryId)
	{
		return RecordRead<float>("addInstance", geometryId);
	}

	/// <summary>
	/// Replaces the geometry at <c>geometryId</c> with the provided geometry. Throws an error if there
	/// is not enough space reserved for geometry. Calling this will change all instances that are
	/// rendering that geometry. Records a read op, sends it behind every write already pending, and
	/// completes with what <c>setGeometryAt</c> returned.
	/// </summary>
	/// <param name="geometryId">Which geometry id to replace with this geometry.</param>
	/// <param name="geometry">The geometry to substitute at the given geometry id.</param>
	/// <returns>The value <c>setGeometryAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> SetGeometryAtAsync(float geometryId, BufferGeometry geometry)
	{
		return RecordRead<float>("setGeometryAt", geometryId, geometry);
	}

	/// <summary>
	/// Repacks the sub geometries in [name] to remove any unused space remaining from previously
	/// deleted geometry, freeing up space to add new geometry. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>optimize</c> returned.
	/// </summary>
	/// <returns>The value <c>optimize</c> returned, once the JavaScript side has answered.</returns>
	public Task<BatchedMesh?> OptimizeAsync()
	{
		return RecordReadObject<BatchedMesh>("optimize", (adoptedBatch, adoptedHandle) => new BatchedMesh(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Reads <c>getBoundingBoxAt</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>getBoundingBoxAt</c> returned.
	/// </summary>
	/// <param name="geometryId">Value forwarded to the <c>geometryId</c> argument.</param>
	/// <param name="target">Value forwarded to the <c>target</c> argument.</param>
	/// <returns>The value <c>getBoundingBoxAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<Box3> GetBoundingBoxAtAsync(float geometryId, Box3 target)
	{
		return RecordRead<Box3>("getBoundingBoxAt", geometryId, target);
	}

	/// <summary>
	/// Reads <c>getBoundingSphereAt</c> back from the JavaScript-side object. Records a read op, sends
	/// it behind every write already pending, and completes with what <c>getBoundingSphereAt</c>
	/// returned.
	/// </summary>
	/// <param name="geometryId">Value forwarded to the <c>geometryId</c> argument.</param>
	/// <param name="target">Value forwarded to the <c>target</c> argument.</param>
	/// <returns>The value <c>getBoundingSphereAt</c> returned, once the JavaScript side has answered.</returns>
	public Task<Sphere> GetBoundingSphereAtAsync(float geometryId, Sphere target)
	{
		return RecordRead<Sphere>("getBoundingSphereAt", geometryId, target);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.BatchedMesh</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_material?.AttachTo(batch);

		base.EmitCreate(batch);
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isBoundingBoxWritten)
		{
			batch.Set(Handle, "boundingBox", ThreeValue.Encode(BoundingBox));
		}

		if (_isBoundingSphereWritten)
		{
			batch.Set(Handle, "boundingSphere", ThreeValue.Encode(BoundingSphere));
		}

		if (_isPerObjectFrustumCulledWritten)
		{
			batch.Set(Handle, "perObjectFrustumCulled", ThreeValue.Encode(_perObjectFrustumCulled));
		}

		if (_isSortObjectsWritten)
		{
			batch.Set(Handle, "sortObjects", ThreeValue.Encode(_sortObjects));
		}
	}
}
