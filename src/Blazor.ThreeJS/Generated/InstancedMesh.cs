// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A special version of <c>Mesh</c> with instanced rendering support. The JavaScript-side
/// <c>THREE.InstancedMesh</c>.
/// </summary>
/// <remarks>
/// Use <see cref="InstancedMesh"/> if you have to render a large number of objects with the same
/// geometry and material(s) but with different world transformations The usage of
/// <see cref="InstancedMesh"/> will help you to reduce the number of draw calls and thus improve
/// the overall rendering performance in your application.
/// </remarks>
/// <seealso href="https://threejs.org/examples/#webgl_instancing_dynamic">WebGL / instancing / dynamic</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_instancing_performance">WebGL / instancing / performance</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_instancing_scatter">WebGL / instancing / scatter</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_instancing_raycast">WebGL / instancing / raycast</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/InstancedMesh">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/InstancedMesh.js">Source</seealso>
public sealed class InstancedMesh : Mesh
{
	private readonly BufferGeometry? _geometry;
	private readonly Material? _material;
	private readonly int _count;
	private DataTexture? _morphTexture;
	private bool _isMorphTextureWritten;

	/// <summary>Create a new instance of <see cref="InstancedMesh"/>.</summary>
	/// <param name="geometry">An instance of <see cref="BufferGeometry"/>.</param>
	/// <param name="material">
	/// A single or an array of <see cref="Material"/>. Default is a new
	/// <see cref="MeshBasicMaterial"/>.
	/// </param>
	/// <param name="count">The **maximum** number of instances of this Mesh.</param>
	public InstancedMesh(BufferGeometry? geometry, Material? material, int count)
	{
		_geometry = geometry;
		_material = material;
		_count = count;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.InstancedMesh</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "InstancedMesh"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.InstancedMesh</c>: geometry, material, count.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_geometry, _material, _count]; }
	}

	/// <summary>
	/// Represents the morph target weights of all instances. You have to set its <c>.needsUpdate</c>
	/// flag to true if you modify instanced data via <c>.setMorphAt</c>. Writing it records a
	/// <c>morphTexture</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public DataTexture? MorphTexture
	{
		get { return _morphTexture; }
		set
		{
			if (ReferenceEquals(_morphTexture, value))
			{
				return;
			}

			_morphTexture = value;
			_isMorphTextureWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("morphTexture", value);
		}
	}

	/// <summary>
	/// Computes the bounding box of the instanced mesh, and updates the <c>.boundingBox</c> attribute.
	/// The bounding box is not computed by the engine; it must be computed by your app. You may need to
	/// recompute the bounding box if an instance is transformed via <c>.setMatrixAt()</c>.
	/// </summary>
	public void ComputeBoundingBox()
	{
		RecordCall("computeBoundingBox");
	}

	/// <summary>
	/// Computes the bounding sphere of the instanced mesh, and updates the <c>.boundingSphere</c>
	/// attribute. The engine automatically computes the bounding sphere when it is needed, e.g., for
	/// ray casting or view frustum culling. You may need to recompute the bounding sphere if an
	/// instance is transformed via [page:.setMatrixAt]().
	/// </summary>
	public void ComputeBoundingSphere()
	{
		RecordCall("computeBoundingSphere");
	}

	/// <summary>Sets the given color to the defined instance.</summary>
	/// <param name="index">The index of an instance. Values have to be in the range <c>[0, count]</c>.</param>
	/// <param name="color">The color of a single instance.</param>
	public void SetColorAt(int index, Color color)
	{
		RecordCall("setColorAt", index, color);
	}

	/// <summary>Get the morph target weights of the defined instance.</summary>
	/// <param name="index">The index of an instance. Values have to be in the range [0, count].</param>
	/// <param name="mesh">
	/// The <c>.morphTargetInfluences</c> property of this mesh will be filled with the morph target
	/// weights of the defined instance.
	/// </param>
	public void GetMorphAt(int index, Mesh mesh)
	{
		if (Batch is not null)
		{
			mesh.AttachTo(Batch);
		}

		RecordCall("getMorphAt", index, mesh);
	}

	/// <summary>
	/// Sets the given local transformation matrix to the defined instance. Make sure you set
	/// <c>.instanceMatrix.needsUpdate()</c> flag to <c>true</c> after updating all the matrices.
	/// Negatively scaled matrices are not supported.
	/// </summary>
	/// <param name="index">The index of an instance. Values have to be in the range <c>[0, count]</c>.</param>
	/// <param name="matrix">A 4x4 matrix representing the local transformation of a single instance.</param>
	public void SetMatrixAt(int index, Matrix4 matrix)
	{
		RecordCall("setMatrixAt", index, matrix);
	}

	/// <summary>
	/// Sets the morph target weights to the defined instance. Make sure you set
	/// <c>.morphTexture</c><c>.needsUpdate</c> to true after updating all the influences.
	/// </summary>
	/// <param name="index">The index of an instance. Values have to be in the range [0, count].</param>
	/// <param name="mesh">
	/// A mesh with <c>.morphTargetInfluences</c> property containing the morph target weights of a
	/// single instance.
	/// </param>
	public void SetMorphAt(int index, Mesh mesh)
	{
		if (Batch is not null)
		{
			mesh.AttachTo(Batch);
		}

		RecordCall("setMorphAt", index, mesh);
	}

	/// <summary>Frees the GPU-related resources allocated by this instance.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.InstancedMesh</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
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

		if (_isMorphTextureWritten)
		{
			_morphTexture?.AttachTo(batch);
			batch.Set(Handle, "morphTexture", ThreeValue.Encode(_morphTexture));
		}
	}
}
