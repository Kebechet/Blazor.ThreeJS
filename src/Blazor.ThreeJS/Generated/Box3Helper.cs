// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A helper object to visualize an instance of <see cref="Box3"/>. The JavaScript-side
/// <c>THREE.Box3Helper</c>.
/// </summary>
public sealed class Box3Helper : LineSegments
{
	private readonly Box3 _box;
	private readonly Color? _color;

	/// <summary>Constructs a new box3 helper.</summary>
	/// <param name="box">The box to visualize.</param>
	/// <param name="color">The box's color.</param>
	public Box3Helper(Box3 box, Color? color = null)
	{
		_box = box;
		_color = color;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Box3Helper</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Box3Helper(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_box = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Box3Helper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Box3Helper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Box3Helper</c>: box, color. An argument the caller
	/// left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([_box, ThreeValue.OrUnspecified(_color)]); }
	}

	/// <summary>
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}
}
