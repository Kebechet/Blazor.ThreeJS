// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Creates a visual aid consisting of a spherical mesh for a given <see cref="HemisphereLight"/>.
/// When the hemisphere light is transformed or its light properties are changed, it's necessary to
/// call the <c>update()</c> method of the respective helper. The JavaScript-side
/// <c>THREE.HemisphereLightHelper</c>.
/// </summary>
public sealed class HemisphereLightHelper : Object3D
{
	private HemisphereLight _light;
	private readonly float _size;
	private readonly Color? _color;
	private MeshBasicMaterial? _material;
	private bool _isLightWritten;
	private bool _isMaterialWritten;

	/// <summary>Constructs a new hemisphere light helper.</summary>
	/// <param name="light">The light to be visualized.</param>
	/// <param name="size">The size of the mesh used to visualize the light.</param>
	/// <param name="color">The helper's color. If not set, the helper will take the color of the light.</param>
	public HemisphereLightHelper(HemisphereLight light, float size = 1f, Color? color = null)
	{
		_light = light;
		_size = size;
		_color = color;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>HemisphereLightHelper</c> under the handle the browser
	/// minted for it. No create op is emitted: the object already exists, and this mirror's job is to
	/// name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal HemisphereLightHelper(ThreeBatch batch, int handle)
		: base(handle)
	{
		_light = default!;
		_size = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.HemisphereLightHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "HemisphereLightHelper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.HemisphereLightHelper</c>: light, size, color. An
	/// argument the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed
	/// when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([_light, _size, ThreeValue.OrUnspecified(_color)]); }
	}

	/// <summary>
	/// The light being visualized. Writing it records a <c>light</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public HemisphereLight Light
	{
		get { return _light; }
		set
		{
			if (ReferenceEquals(_light, value))
			{
				return;
			}

			_light = value;
			_isLightWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("light", value);
		}
	}

	/// <summary>
	/// The <c>material</c> property of the JavaScript-side object. Writing it records a <c>material</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public MeshBasicMaterial? Material
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

	/// <summary>
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>Updates the helper to match the position and direction of the light being visualized.</summary>
	public void Update()
	{
		RecordCall("update");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.HemisphereLightHelper</c> is constructed from, so their create ops
	/// reach the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_light.AttachTo(batch);

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

		if (_isLightWritten)
		{
			_light.AttachTo(batch);
			batch.Set(Handle, "light", ThreeValue.Encode(_light));
		}

		if (_isMaterialWritten)
		{
			_material?.AttachTo(batch);
			batch.Set(Handle, "material", ThreeValue.Encode(_material));
		}
	}
}
