// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A class for displaying <see cref="Points"/>. The JavaScript-side <c>THREE.Points</c>.</summary>
/// <remarks>
/// The <see cref="Points"/> are rendered by the <c>WebGLRenderer</c> using
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/drawElements">gl.POINTS</see>.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/Points">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/Points.js">Source</seealso>
public sealed class Points : Object3D
{
	private BufferGeometry? _geometry;
	private Material? _material;
	private float[]? _morphTargetInfluences;
	private Dictionary<string, float>? _morphTargetDictionary;
	private bool _isMorphTargetInfluencesWritten;
	private bool _isMorphTargetDictionaryWritten;
	private bool _isGeometryWritten;
	private bool _isMaterialWritten;

	/// <summary>Create a new instance of <see cref="Points"/>.</summary>
	/// <param name="geometry">
	/// An instance of <c>BufferGeometry</c>. Default <c><c>new THREE.BufferGeometry()</c></c>.
	/// </param>
	/// <param name="material">
	/// A single or an array of <c>Material</c>. Default <c><c>new THREE.PointsMaterial()</c></c>.
	/// </param>
	public Points(BufferGeometry? geometry = null, Material? material = null)
	{
		_geometry = geometry;
		_material = material;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Points</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Points(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Points</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Points"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Points</c>: geometry, material. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
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
	/// An array of weights typically from <c>0-1</c> that specify how much of the morph is applied.
	/// Writing it records a <c>morphTargetInfluences</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public float[]? MorphTargetInfluences
	{
		get { return _morphTargetInfluences; }
		set
		{
			if (_morphTargetInfluences == value)
			{
				return;
			}

			_morphTargetInfluences = value;
			_isMorphTargetInfluencesWritten = true;
			RecordSet("morphTargetInfluences", value);
		}
	}

	/// <summary>
	/// A dictionary of morphTargets based on the <c>morphTarget.name</c> property. Writing it records a
	/// <c>morphTargetDictionary</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public Dictionary<string, float>? MorphTargetDictionary
	{
		get { return _morphTargetDictionary; }
		set
		{
			if (_morphTargetDictionary == value)
			{
				return;
			}

			_morphTargetDictionary = value;
			_isMorphTargetDictionaryWritten = true;
			RecordSet("morphTargetDictionary", value);
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
	/// An instance of <c>Material</c>, defining the object's appearance. Writing it records a
	/// <c>material</c> property write once this object is attached; writing the value already held
	/// records nothing.
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
	/// Read-only flag to check if a given object is of type <see cref="Points"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isPoints</c> held.
	/// </summary>
	/// <returns>The value <c>isPoints</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsPointsAsync()
	{
		return GetAsync<bool>("isPoints");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.Points</c> is constructed from, so their create ops reach the
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

		if (_isMorphTargetInfluencesWritten)
		{
			batch.Set(Handle, "morphTargetInfluences", ThreeValue.Encode(_morphTargetInfluences));
		}

		if (_isMorphTargetDictionaryWritten)
		{
			batch.Set(Handle, "morphTargetDictionary", ThreeValue.Encode(_morphTargetDictionary));
		}

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
