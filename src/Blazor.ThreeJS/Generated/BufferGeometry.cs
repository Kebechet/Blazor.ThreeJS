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
	private float _indirectOffset;
	private bool _morphTargetsRelative = false;
	private bool _isIdWritten;
	private bool _isUuidWritten;
	private bool _isNameWritten;
	private bool _isIndirectOffsetWritten;
	private bool _isMorphTargetsRelativeWritten;

	/// <summary>This creates a new <c>BufferGeometry</c> object.</summary>
	public BufferGeometry()
	{
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
		if (Batch is not null)
		{
			source.AttachTo(Batch);
		}

		RecordCall("copy", source);
	}

	/// <summary>Frees the GPU-related resources allocated by this instance.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.BufferGeometry</c>, then replays every property written before
	/// this object was attached.
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

		if (_isIndirectOffsetWritten)
		{
			batch.Set(Handle, "indirectOffset", ThreeValue.Encode(_indirectOffset));
		}

		if (_isMorphTargetsRelativeWritten)
		{
			batch.Set(Handle, "morphTargetsRelative", ThreeValue.Encode(_morphTargetsRelative));
		}
	}
}
