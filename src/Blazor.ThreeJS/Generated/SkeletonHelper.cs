// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A helper object to assist with visualizing a <see cref="Skeleton"/>. The JavaScript-side
/// <c>THREE.SkeletonHelper</c>.
/// </summary>
public sealed class SkeletonHelper : LineSegments
{
	private readonly Object3D _object;
	private Object3D? _root;
	private Bone?[] _bones = [];
	private bool _isRootWritten;
	private bool _isBonesWritten;

	/// <summary>Constructs a new skeleton helper.</summary>
	/// <param name="object">
	/// Usually an instance of <see cref="SkinnedMesh"/>. However, any 3D object can be used if it
	/// represents a hierarchy of bones (see <see cref="Bone"/>).
	/// </param>
	public SkeletonHelper(Object3D @object)
	{
		_object = @object;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>SkeletonHelper</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal SkeletonHelper(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_object = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.SkeletonHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "SkeletonHelper"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.SkeletonHelper</c>: object.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_object]; }
	}

	/// <summary>
	/// The object being visualized. Writing it records a <c>root</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public Object3D? Root
	{
		get { return _root; }
		set
		{
			if (ReferenceEquals(_root, value))
			{
				return;
			}

			_root = value;
			_isRootWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("root", value);
		}
	}

	/// <summary>
	/// The list of bones that the helper visualizes. Writing it records a <c>bones</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Bone?[] Bones
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

	/// <summary>Defines the colors of the helper.</summary>
	/// <param name="color1">The first line color for each bone.</param>
	/// <param name="color2">The second line color for each bone.</param>
	public void SetColors(Color color1, Color color2)
	{
		RecordCall("setColors", color1, color2);
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
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isSkeletonHelper</c> held.
	/// </summary>
	/// <returns>The value <c>isSkeletonHelper</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsSkeletonHelperAsync()
	{
		return GetAsync<bool>("isSkeletonHelper");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.SkeletonHelper</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_object.AttachTo(batch);

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

		if (_isRootWritten)
		{
			_root?.AttachTo(batch);
			batch.Set(Handle, "root", ThreeValue.Encode(_root));
		}

		if (_isBonesWritten)
		{
			AttachEach(batch, _bones);
			batch.Set(Handle, "bones", ThreeValue.Encode(_bones));
		}
	}
}
