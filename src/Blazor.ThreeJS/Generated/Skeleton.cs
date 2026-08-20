// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Use an array of <see cref="Bone">bones</see> to create a <see cref="Skeleton"/> that can be used
/// by a <c>SkinnedMesh</c>. The JavaScript-side <c>THREE.Skeleton</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/Skeleton">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/Skeleton.js">Source</seealso>
public sealed class Skeleton : ThreeObject
{
	private Bone?[]? _bones;
	private Matrix4[]? _boneInverses;
	private string _uuid = string.Empty;
	private Float32Array? _boneMatrices;
	private DataTexture? _boneTexture;
	private float _frame;
	private bool _isUuidWritten;
	private bool _isBonesWritten;
	private bool _isBoneInversesWritten;
	private bool _isBoneMatricesWritten;
	private bool _isBoneTextureWritten;
	private bool _isFrameWritten;

	/// <summary>Creates a new Skeleton.</summary>
	/// <param name="bones">The array of <c>bones</c>.</param>
	/// <param name="boneInverses">An array of <c>Matrix4s</c>.</param>
	public Skeleton(Bone?[]? bones = null, Matrix4[]? boneInverses = null)
	{
		_bones = bones;
		_boneInverses = boneInverses;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Skeleton</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Skeleton(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Skeleton</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Skeleton"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Skeleton</c>: bones, boneInverses. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_bones),
				ThreeValue.OrUnspecified(_boneInverses)
			]);
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
	/// The array of <c>Bones</c>. Writing it records a <c>bones</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public Bone?[]? Bones
	{
		get { return _bones; }
		set
		{
			if (_bones == value)
			{
				return;
			}

			_bones = value;
			_isBonesWritten = true;
			AttachEach(Batch, value);

			RecordSet("bones", value);
		}
	}

	/// <summary>
	/// An array of <see cref="Matrix4">Matrix4s</see> that represent the inverse of the
	/// <c>matrixWorld</c> of the individual bones. Writing it records a <c>boneInverses</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Matrix4[]? BoneInverses
	{
		get { return _boneInverses; }
		set
		{
			if (_boneInverses == value)
			{
				return;
			}

			_boneInverses = value;
			_isBoneInversesWritten = true;
			RecordSet("boneInverses", value);
		}
	}

	/// <summary>
	/// The array buffer holding the bone data when using a vertex texture. Writing it records a
	/// <c>boneMatrices</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public Float32Array? BoneMatrices
	{
		get { return _boneMatrices; }
		set
		{
			if (_boneMatrices == value)
			{
				return;
			}

			_boneMatrices = value;
			_isBoneMatricesWritten = true;
			RecordSet("boneMatrices", value);
		}
	}

	/// <summary>
	/// The <c>DataTexture</c> holding the bone data when using a vertex texture. Writing it records a
	/// <c>boneTexture</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public DataTexture? BoneTexture
	{
		get { return _boneTexture; }
		set
		{
			if (ReferenceEquals(_boneTexture, value))
			{
				return;
			}

			_boneTexture = value;
			_isBoneTextureWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("boneTexture", value);
		}
	}

	/// <summary>
	/// The <c>frame</c> property of the JavaScript-side object. Writing it records a <c>frame</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Frame
	{
		get { return _frame; }
		set
		{
			if (_frame == value)
			{
				return;
			}

			_frame = value;
			_isFrameWritten = true;
			RecordSet("frame", value);
		}
	}

	/// <summary>Records a call to <c>init</c> on the JavaScript-side object.</summary>
	public void Init()
	{
		RecordCall("init");
	}

	/// <summary>Generates the <c>boneInverses</c> array if not provided in the constructor.</summary>
	public void CalculateInverses()
	{
		RecordCall("calculateInverses");
	}

	/// <summary>Returns the skeleton to the base pose.</summary>
	public void Pose()
	{
		RecordCall("pose");
	}

	/// <summary>Updates the <c>boneMatrices</c> and <c>boneTexture</c> after changing the bones.</summary>
	public void Update()
	{
		RecordCall("update");
	}

	/// <summary>Frees the GPU-related resources allocated by this instance.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Computes an instance of <c>DataTexture</c> in order to pass the bone data more efficiently to
	/// the shader. Records a read op, sends it behind every write already pending, and completes with
	/// what <c>computeBoneTexture</c> returned.
	/// </summary>
	/// <returns>The value <c>computeBoneTexture</c> returned, once the JavaScript side has answered.</returns>
	public Task<Skeleton?> ComputeBoneTextureAsync()
	{
		return RecordReadObject<Skeleton>("computeBoneTexture", (adoptedBatch, adoptedHandle) => new Skeleton(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns a clone of this <see cref="Skeleton"/> object. Records a read op, sends it behind every
	/// write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<Skeleton?> CloneAsync()
	{
		return RecordReadObject<Skeleton>("clone", (adoptedBatch, adoptedHandle) => new Skeleton(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Searches through the skeleton's bone array and returns the first with a matching name. Records a
	/// read op, sends it behind every write already pending, and completes with what
	/// <c>getBoneByName</c> returned.
	/// </summary>
	/// <param name="name">String to match to the Bone's <c>.name</c> property.</param>
	/// <returns>The value <c>getBoneByName</c> returned, once the JavaScript side has answered.</returns>
	public Task<Bone?> GetBoneByNameAsync(string name)
	{
		return RecordReadObject<Bone>("getBoneByName", (adoptedBatch, adoptedHandle) => new Bone(adoptedBatch, adoptedHandle), name);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.Skeleton</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own. A replayed
	/// value that is itself a mirrored object is attached first, so its create op reaches the batch
	/// before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		AttachEach(batch, _bones);

		base.EmitCreate(batch);

		if (_isUuidWritten)
		{
			batch.Set(Handle, "uuid", ThreeValue.Encode(_uuid));
		}

		if (_isBonesWritten)
		{
			AttachEach(batch, _bones);
			batch.Set(Handle, "bones", ThreeValue.Encode(_bones));
		}

		if (_isBoneInversesWritten)
		{
			batch.Set(Handle, "boneInverses", ThreeValue.Encode(_boneInverses));
		}

		if (_isBoneMatricesWritten)
		{
			batch.Set(Handle, "boneMatrices", ThreeValue.Encode(_boneMatrices));
		}

		if (_isBoneTextureWritten)
		{
			_boneTexture?.AttachTo(batch);
			batch.Set(Handle, "boneTexture", ThreeValue.Encode(_boneTexture));
		}

		if (_isFrameWritten)
		{
			batch.Set(Handle, "frame", ThreeValue.Encode(_frame));
		}
	}
}
