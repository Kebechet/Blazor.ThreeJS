// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A continuous line. The JavaScript-side <c>THREE.Line</c>.</summary>
/// <remarks>
/// This is nearly the same as <c>LineSegments</c>, the only difference is that it is rendered using
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/drawElements">gl.LINE_STRIP</see>
/// instead of
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/drawElements">gl.LINES</see>.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/Line">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/Line.js">Source</seealso>
public class Line : Object3D
{
	private BufferGeometry? _geometry;
	private Material? _material;
	private bool _isGeometryWritten;
	private bool _isMaterialWritten;

	/// <summary>Create a new instance of <see cref="Line"/>.</summary>
	/// <param name="geometry">
	/// Vertices representing the <see cref="Line"/> segment(s). Default <c><c>new
	/// THREE.BufferGeometry()</c></c>.
	/// </param>
	/// <param name="material">Material for the line. Default <c><c>new THREE.LineBasicMaterial()</c></c>.</param>
	public Line(BufferGeometry? geometry = null, Material? material = null)
	{
		_geometry = geometry;
		_material = material;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Line</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Line"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Line</c>: geometry, material. An argument the caller
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
	/// Vertices representing the <see cref="Line"/> segment(s). Writing it records a <c>geometry</c>
	/// property write once this object is attached; writing the value already held records nothing.
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
	/// Material for the line. Writing it records a <c>material</c> property write once this object is
	/// attached; writing the value already held records nothing.
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
	/// Attaches the objects <c>THREE.Line</c> is constructed from, so their create ops reach the batch
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
