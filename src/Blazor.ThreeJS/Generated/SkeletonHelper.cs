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
	private bool _isRootWritten;

	/// <summary>Constructs a new skeleton helper.</summary>
	/// <param name="object">
	/// Usually an instance of <see cref="SkinnedMesh"/>. However, any 3D object can be used if it
	/// represents a hierarchy of bones (see <see cref="Bone"/>).
	/// </param>
	public SkeletonHelper(Object3D @object)
	{
		_object = @object;
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
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isRootWritten)
		{
			batch.Set(Handle, "root", ThreeValue.Encode(_root));
		}
	}
}
