// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Use an array of <see cref="Bone">bones</see> to create a <see cref="Skeleton"/> that can be used
/// by a <c>SkinnedMesh</c>. The JavaScript-side <c>THREE.Skeleton</c>.
/// </summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/Skeleton">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/Skeleton.js">Source</seealso>
public sealed class Skeleton : ThreeObject
{
	private string _uuid = string.Empty;
	private DataTexture? _boneTexture;
	private float _frame;
	private bool _isUuidWritten;
	private bool _isBoneTextureWritten;
	private bool _isFrameWritten;

	/// <summary>Creates a new Skeleton.</summary>
	public Skeleton()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Skeleton</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Skeleton"; }
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
	/// Emits the create op for <c>THREE.Skeleton</c>, then replays every property written before this
	/// object was attached. A replayed value that is itself a mirrored object is attached first, so its
	/// create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isUuidWritten)
		{
			batch.Set(Handle, "uuid", ThreeValue.Encode(_uuid));
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
