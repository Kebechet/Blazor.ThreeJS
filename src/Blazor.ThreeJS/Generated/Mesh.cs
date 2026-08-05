// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Class representing triangular <see href="https://en.wikipedia.org/wiki/Polygon_mesh">polygon
/// mesh</see> based objects. The JavaScript-side <c>THREE.Mesh</c>.
/// </summary>
/// <remarks>Also serves as a base for other classes such as <c>SkinnedMesh</c>, <c>InstancedMesh</c>.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/Mesh">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/Mesh.js">Source</seealso>
public class Mesh : Object3D
{
	private BufferGeometry? _geometry;
	private Material? _material;
	private bool _isGeometryWritten;
	private bool _isMaterialWritten;

	/// <summary>Create a new instance of <see cref="Mesh"/>.</summary>
	/// <param name="geometry">
	/// An instance of <c>BufferGeometry</c>. Default <c><c>new THREE.BufferGeometry()</c></c>.
	/// </param>
	/// <param name="material">
	/// A single or an array of <c>Material</c>. Default <c><c>new THREE.MeshBasicMaterial()</c></c>.
	/// </param>
	public Mesh(BufferGeometry? geometry = null, Material? material = null)
	{
		_geometry = geometry;
		_material = material;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Mesh</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Mesh"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Mesh</c>: geometry, material. An argument the caller
	/// left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_geometry),
				ThreeValue.OrUnspecified(_material)
			]);
		}
	}

	/// <summary>
	/// An instance of <c>BufferGeometry</c> (or derived classes), defining the object's structure.
	/// Writing it records a <c>geometry</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public BufferGeometry? Geometry
	{
		get { return _geometry; }
		set
		{
			if (ReferenceEquals(_geometry, value))
			{
				return;
			}

			_geometry = value;
			_isGeometryWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("geometry", value);
		}
	}

	/// <summary>
	/// An instance of material derived from the <c>Material</c> base class or an array of materials,
	/// defining the object's appearance. Writing it records a <c>material</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public Material? Material
	{
		get { return _material; }
		set
		{
			if (ReferenceEquals(_material, value))
			{
				return;
			}

			_material = value;
			_isMaterialWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("material", value);
		}
	}

	/// <summary>Updates the morphTargets to have no influence on the object.</summary>
	public void UpdateMorphTargets()
	{
		RecordCall("updateMorphTargets");
	}

	/// <summary>
	/// Get the local-space position of the vertex at the given index, taking into account the current
	/// animation state of both morph targets and skinning. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>getVertexPosition</c> returned.
	/// </summary>
	/// <param name="index"></param>
	/// <param name="target">Value forwarded to the <c>target</c> argument.</param>
	/// <returns>The value <c>getVertexPosition</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector3> GetVertexPositionAsync(int index, Vector3 target)
	{
		return RecordRead<Vector3>("getVertexPosition", index, target);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.Mesh</c> is constructed from, so their create ops reach the batch
	/// before the one that references them by handle, then emits this object's own.
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

		if (_isGeometryWritten)
		{
			_geometry?.AttachTo(batch);
			batch.Set(Handle, "geometry", ThreeValue.Encode(_geometry));
		}

		if (_isMaterialWritten)
		{
			_material?.AttachTo(batch);
			batch.Set(Handle, "material", ThreeValue.Encode(_material));
		}
	}
}
