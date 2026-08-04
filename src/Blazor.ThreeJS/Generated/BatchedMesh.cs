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
	private bool _isPerObjectFrustumCulledWritten;
	private bool _isSortObjectsWritten;

	/// <summary>Initializes a new <see cref="BatchedMesh"/>.</summary>
	/// <param name="maxInstanceCount">the max number of individual geometries planned to be added.</param>
	/// <param name="maxVertexCount">the max number of vertices to be used by all geometries.</param>
	/// <param name="maxIndexCount">the max number of indices to be used by all geometries.</param>
	/// <param name="material">
	/// an instance of <see cref="Material"/>. Default is a new <see cref="MeshBasicMaterial"/>.
	/// </param>
	public BatchedMesh(int maxInstanceCount, int maxVertexCount, int? maxIndexCount = null, Material? material = null)
	{
		_maxInstanceCount = maxInstanceCount;
		_maxVertexCount = maxVertexCount;
		_maxIndexCount = maxIndexCount;
		_material = material;
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
