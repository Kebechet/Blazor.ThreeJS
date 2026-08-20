// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A representation of mesh, line, or point geometry Includes vertex positions, face indices,
/// normals, colors, UVs, and custom attributes within buffers, reducing the cost of passing all
/// this data to the GPU. The JavaScript-side <c>THREE.BufferGeometry</c>.
/// </summary>
/// <remarks>To read and edit data in BufferGeometry attributes, see <c>BufferAttribute</c> documentation.</remarks>
/// <seealso href="https://threejs.org/examples/#webgl_buffergeometry">Mesh with non-indexed faces</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_buffergeometry_indexed">Mesh with indexed faces</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_buffergeometry_lines">Lines</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_buffergeometry_lines_indexed">Indexed Lines</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_buffergeometry_custom_attributes_particles">Particles</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_buffergeometry_rawshader">Raw Shaders</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/BufferGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/BufferGeometry.js">Source</seealso>
public class BufferGeometry : EventDispatcher
{
	private int _id;
	private string _uuid = string.Empty;
	private string _name = string.Empty;
	private BufferAttribute? _index = null;
	private IndirectStorageBufferAttribute? _indirect;
	private float _indirectOffset;
	private bool _morphTargetsRelative = false;
	private bool _isIdWritten;
	private bool _isUuidWritten;
	private bool _isNameWritten;
	private bool _isIndexWritten;
	private bool _isIndirectWritten;
	private bool _isIndirectOffsetWritten;
	private bool _isMorphTargetsRelativeWritten;
	private bool _isBoundingBoxWritten;
	private bool _isBoundingSphereWritten;

	/// <summary>
	/// Bounding box for the <c>BufferGeometry</c>, which can be calculated with
	/// <c>.computeBoundingBox()</c>. Mirrored as an instance this object owns: mutating it records a
	/// write of <c>boundingBox</c>.
	/// </summary>
	public Box3 BoundingBox { get; }

	/// <summary>
	/// Bounding sphere for the <c>BufferGeometry</c>, which can be calculated with
	/// <c>.computeBoundingSphere()</c>. Mirrored as an instance this object owns: mutating it records a
	/// write of <c>boundingSphere</c>.
	/// </summary>
	public Sphere BoundingSphere { get; }

	/// <summary>This creates a new <c>BufferGeometry</c> object.</summary>
	public BufferGeometry()
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
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>BufferGeometry</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal BufferGeometry(ThreeBatch batch, int handle)
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

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BufferGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "BufferGeometry"; }
	}

	/// <summary>
	/// Unique number for this <c>BufferGeometry</c> instance. Writing it records a <c>id</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public int Id
	{
		get { return _id; }
		set
		{
			if (_id == value)
			{
				return;
			}

			_id = value;
			_isIdWritten = true;
			RecordSet("id", value);
		}
	}

	/// <summary>
	/// <see href="http://en.wikipedia.org/wiki/Universally_unique_identifier">UUID</see> of this object
	/// instance. Writing it records a <c>uuid</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public string Uuid
	{
		get { return _uuid; }
		set
		{
			if (_uuid == value)
			{
				return;
			}

			_uuid = value;
			_isUuidWritten = true;
			RecordSet("uuid", value);
		}
	}

	/// <summary>
	/// Optional name for this <c>BufferGeometry</c> instance. Writing it records a <c>name</c> property
	/// write once this object is attached; writing the value already held records nothing.
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
	/// Allows for vertices to be re-used across multiple triangles; this is called using "indexed
	/// triangles". Each triangle is associated with the indices of three vertices. This attribute
	/// therefore stores the index of each vertex for each triangular face. If this attribute is not
	/// set, the <c>renderer</c> assumes that each three contiguous positions represent a single
	/// triangle. Writing it records a <c>index</c> property write once this object is attached; writing
	/// the value already held records nothing.
	/// </summary>
	public BufferAttribute? Index
	{
		get { return _index; }
		set
		{
			if (ReferenceEquals(_index, value))
			{
				return;
			}

			_index = value;
			_isIndexWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("index", value);
		}
	}

	/// <summary>
	/// The <c>indirect</c> property of the JavaScript-side object. Writing it records a <c>indirect</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public IndirectStorageBufferAttribute? Indirect
	{
		get { return _indirect; }
		set
		{
			if (ReferenceEquals(_indirect, value))
			{
				return;
			}

			_indirect = value;
			_isIndirectWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("indirect", value);
		}
	}

	/// <summary>
	/// The <c>indirectOffset</c> property of the JavaScript-side object. Writing it records a
	/// <c>indirectOffset</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float IndirectOffset
	{
		get { return _indirectOffset; }
		set
		{
			if (_indirectOffset == value)
			{
				return;
			}

			_indirectOffset = value;
			_isIndirectOffsetWritten = true;
			RecordSet("indirectOffset", value);
		}
	}

	/// <summary>
	/// Used to control the morph target behavior; when set to true, the morph target data is treated as
	/// relative offsets, rather than as absolute positions/normals. Writing it records a
	/// <c>morphTargetsRelative</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public bool MorphTargetsRelative
	{
		get { return _morphTargetsRelative; }
		set
		{
			if (_morphTargetsRelative == value)
			{
				return;
			}

			_morphTargetsRelative = value;
			_isMorphTargetsRelativeWritten = true;
			RecordSet("morphTargetsRelative", value);
		}
	}

	/// <summary>
	/// Set the <c>.index</c> buffer. This writes the same three.js state as <see cref="Index"/> and the
	/// mirror does not learn from it: afterwards <c>Index</c> still reports its previous value, and
	/// writing that value back records nothing at all. Where the property exists, write the property.
	/// This overload takes <c>index</c> as <c>BufferAttribute?</c> out of three.js's <c>BufferAttribute
	/// | number[] | null</c>.
	/// </summary>
	/// <param name="index">Value forwarded to the <c>index</c> argument.</param>
	public void SetIndex(BufferAttribute? index)
	{
		RecordCall("setIndex", index);
	}

	/// <summary>
	/// Set the <c>.index</c> buffer. This writes the same three.js state as <see cref="Index"/> and the
	/// mirror does not learn from it: afterwards <c>Index</c> still reports its previous value, and
	/// writing that value back records nothing at all. Where the property exists, write the property.
	/// This overload takes <c>index</c> as <c>int[]?</c> out of three.js's <c>BufferAttribute |
	/// number[] | null</c>.
	/// </summary>
	/// <param name="index">Value forwarded to the <c>index</c> argument.</param>
	public void SetIndex(int[]? index)
	{
		RecordCall("setIndex", (object?) index);
	}

	/// <summary>
	/// Records a call to <c>setIndirect</c> on the JavaScript-side object. This writes the same
	/// three.js state as <see cref="Indirect"/> and the mirror does not learn from it: afterwards
	/// <c>Indirect</c> still reports its previous value, and writing that value back records nothing at
	/// all. Where the property exists, write the property.
	/// </summary>
	/// <param name="indirect">Value forwarded to the <c>indirect</c> argument.</param>
	/// <param name="indirectOffset">Value forwarded to the <c>indirectOffset</c> argument.</param>
	public void SetIndirect(IndirectStorageBufferAttribute? indirect, float indirectOffset)
	{
		RecordCall("setIndirect", indirect, indirectOffset);
	}

	/// <summary>Adds a group to this geometry.</summary>
	/// <param name="start">Value forwarded to the <c>start</c> argument.</param>
	/// <param name="count">Value forwarded to the <c>count</c> argument.</param>
	/// <param name="materialIndex">Value forwarded to the <c>materialIndex</c> argument.</param>
	public void AddGroup(float start, int count, int materialIndex)
	{
		RecordCall("addGroup", start, count, materialIndex);
	}

	/// <summary>Clears all groups.</summary>
	public void ClearGroups()
	{
		RecordCall("clearGroups");
	}

	/// <summary>Set the <c>.drawRange</c> property.</summary>
	/// <param name="start">Value forwarded to the <c>start</c> argument.</param>
	/// <param name="count">is the number of vertices or indices to render.</param>
	public void SetDrawRange(float start, int count)
	{
		RecordCall("setDrawRange", start, count);
	}

	/// <summary>Applies the matrix transform to the geometry.</summary>
	/// <param name="matrix">Value forwarded to the <c>matrix</c> argument.</param>
	public void ApplyMatrix4(Matrix4 matrix)
	{
		RecordCall("applyMatrix4", matrix);
	}

	/// <summary>Applies the rotation represented by the quaternion to the geometry.</summary>
	/// <param name="quaternion">Value forwarded to the <c>quaternion</c> argument.</param>
	public void ApplyQuaternion(Quaternion quaternion)
	{
		RecordCall("applyQuaternion", quaternion);
	}

	/// <summary>
	/// Rotate the geometry about the X axis. This is typically done as a one time operation, and not
	/// during a loop.
	/// </summary>
	/// <param name="angle">radians.</param>
	public void RotateX(float angle)
	{
		RecordCall("rotateX", angle);
	}

	/// <summary>Rotate the geometry about the Y axis.</summary>
	/// <param name="angle">radians.</param>
	public void RotateY(float angle)
	{
		RecordCall("rotateY", angle);
	}

	/// <summary>Rotate the geometry about the Z axis.</summary>
	/// <param name="angle">radians.</param>
	public void RotateZ(float angle)
	{
		RecordCall("rotateZ", angle);
	}

	/// <summary>Translate the geometry.</summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public void Translate(float x, float y, float z)
	{
		RecordCall("translate", x, y, z);
	}

	/// <summary>Scale the geometry data.</summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public void Scale(float x, float y, float z)
	{
		RecordCall("scale", x, y, z);
	}

	/// <summary>Rotates the geometry to face a point in space.</summary>
	/// <param name="vector">A world vector to look at.</param>
	public void LookAt(Vector3 vector)
	{
		RecordCall("lookAt", vector);
	}

	/// <summary>
	/// Defines a geometry by creating a <c>position</c> attribute based on the given array of points.
	/// The array can hold instances of <see cref="Vector2"/> or <see cref="Vector3"/>. When using
	/// two-dimensional data, the <c>z</c> coordinate for all vertices is set to <c>0</c>. If the method
	/// is used with an existing <c>position</c> attribute, the vertex data are overwritten with the
	/// data from the array. The length of the array must match the vertex count. This overload takes
	/// <c>points</c> as <c>Vector3[]</c> out of three.js's <c>Vector3[] | Vector2[]</c>.
	/// </summary>
	/// <param name="points">Value forwarded to the <c>points</c> argument.</param>
	public void SetFromPoints(Vector3[] points)
	{
		RecordCall("setFromPoints", (object?) points);
	}

	/// <summary>
	/// Defines a geometry by creating a <c>position</c> attribute based on the given array of points.
	/// The array can hold instances of <see cref="Vector2"/> or <see cref="Vector3"/>. When using
	/// two-dimensional data, the <c>z</c> coordinate for all vertices is set to <c>0</c>. If the method
	/// is used with an existing <c>position</c> attribute, the vertex data are overwritten with the
	/// data from the array. The length of the array must match the vertex count. This overload takes
	/// <c>points</c> as <c>Vector2[]</c> out of three.js's <c>Vector3[] | Vector2[]</c>.
	/// </summary>
	/// <param name="points">Value forwarded to the <c>points</c> argument.</param>
	public void SetFromPoints(Vector2[] points)
	{
		RecordCall("setFromPoints", (object?) points);
	}

	/// <summary>
	/// Computes the bounding box of the geometry, and updates the <c>.boundingBox</c> attribute. The
	/// bounding box is not computed by the engine; it must be computed by your app. You may need to
	/// recompute the bounding box if the geometry vertices are modified.
	/// </summary>
	public void ComputeBoundingBox()
	{
		RecordCall("computeBoundingBox");
	}

	/// <summary>
	/// Computes the bounding sphere of the geometry, and updates the <c>.boundingSphere</c> attribute.
	/// The engine automatically computes the bounding sphere when it is needed, e.g., for ray casting
	/// or view frustum culling. You may need to recompute the bounding sphere if the geometry vertices
	/// are modified.
	/// </summary>
	public void ComputeBoundingSphere()
	{
		RecordCall("computeBoundingSphere");
	}

	/// <summary>
	/// Calculates and adds a tangent attribute to this geometry. The computation is only supported for
	/// indexed geometries and if position, normal, and uv attributes are defined.
	/// </summary>
	public void ComputeTangents()
	{
		RecordCall("computeTangents");
	}

	/// <summary>
	/// Computes vertex normals for the given vertex data. For indexed geometries, the method sets each
	/// vertex normal to be the average of the face normals of the faces that share that vertex. For
	/// non-indexed geometries, vertices are not shared, and the method sets each vertex normal to be
	/// the same as the face normal.
	/// </summary>
	public void ComputeVertexNormals()
	{
		RecordCall("computeVertexNormals");
	}

	/// <summary>Every normal vector in a geometry will have a magnitude of 1.</summary>
	public void NormalizeNormals()
	{
		RecordCall("normalizeNormals");
	}

	/// <summary>Copies another BufferGeometry to this BufferGeometry.</summary>
	/// <param name="source">Value forwarded to the <c>source</c> argument.</param>
	public void Copy(BufferGeometry source)
	{
		RecordCall("copy", source);
	}

	/// <summary>Frees the GPU-related resources allocated by this instance.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// A Read-only _string_ to check if <c>this</c> object type. Read-only in three.js, so it is read
	/// on demand rather than mirrored: records a get op, sends it behind every write already pending,
	/// and completes with the value <c>type</c> held.
	/// </summary>
	/// <returns>The value <c>type</c> held, once the JavaScript side has answered.</returns>
	public Task<string> TypeAsync()
	{
		return GetAsync<string>("type");
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="BufferGeometry"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isBufferGeometry</c> held.
	/// </summary>
	/// <returns>The value <c>isBufferGeometry</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsBufferGeometryAsync()
	{
		return GetAsync<bool>("isBufferGeometry");
	}

	/// <summary>
	/// Return the <c>.index</c> buffer. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>getIndex</c> returned.
	/// </summary>
	/// <returns>The value <c>getIndex</c> returned, once the JavaScript side has answered.</returns>
	public Task<BufferAttribute?> GetIndexAsync()
	{
		return RecordReadObject<BufferAttribute>("getIndex", (adoptedBatch, adoptedHandle) => new BufferAttribute(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Reads <c>getIndirect</c> back from the JavaScript-side object. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>getIndirect</c> returned.
	/// </summary>
	/// <returns>The value <c>getIndirect</c> returned, once the JavaScript side has answered.</returns>
	public Task<IndirectStorageBufferAttribute?> GetIndirectAsync()
	{
		return RecordReadObject<IndirectStorageBufferAttribute>("getIndirect", (adoptedBatch, adoptedHandle) => new IndirectStorageBufferAttribute(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Center the geometry based on the bounding box. Records a read op, sends it behind every write
	/// already pending, and completes with what <c>center</c> returned.
	/// </summary>
	/// <returns>The value <c>center</c> returned, once the JavaScript side has answered.</returns>
	public Task<BufferGeometry?> CenterAsync()
	{
		return RecordReadObject<BufferGeometry>("center", (adoptedBatch, adoptedHandle) => new BufferGeometry(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Return a non-index version of an indexed BufferGeometry. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>toNonIndexed</c> returned.
	/// </summary>
	/// <returns>The value <c>toNonIndexed</c> returned, once the JavaScript side has answered.</returns>
	public Task<BufferGeometry?> ToNonIndexedAsync()
	{
		return RecordReadObject<BufferGeometry>("toNonIndexed", (adoptedBatch, adoptedHandle) => new BufferGeometry(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Creates a clone of this BufferGeometry. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<BufferGeometry?> CloneAsync()
	{
		return RecordReadObject<BufferGeometry>("clone", (adoptedBatch, adoptedHandle) => new BufferGeometry(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.BufferGeometry</c>, then replays every property written before
	/// this object was attached. A replayed value that is itself a mirrored object is attached first,
	/// so its create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isIdWritten)
		{
			batch.Set(Handle, "id", ThreeValue.Encode(_id));
		}

		if (_isUuidWritten)
		{
			batch.Set(Handle, "uuid", ThreeValue.Encode(_uuid));
		}

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isIndexWritten)
		{
			_index?.AttachTo(batch);
			batch.Set(Handle, "index", ThreeValue.Encode(_index));
		}

		if (_isIndirectWritten)
		{
			_indirect?.AttachTo(batch);
			batch.Set(Handle, "indirect", ThreeValue.Encode(_indirect));
		}

		if (_isIndirectOffsetWritten)
		{
			batch.Set(Handle, "indirectOffset", ThreeValue.Encode(_indirectOffset));
		}

		if (_isMorphTargetsRelativeWritten)
		{
			batch.Set(Handle, "morphTargetsRelative", ThreeValue.Encode(_morphTargetsRelative));
		}

		if (_isBoundingBoxWritten)
		{
			batch.Set(Handle, "boundingBox", ThreeValue.Encode(BoundingBox));
		}

		if (_isBoundingSphereWritten)
		{
			batch.Set(Handle, "boundingSphere", ThreeValue.Encode(BoundingSphere));
		}
	}
}
