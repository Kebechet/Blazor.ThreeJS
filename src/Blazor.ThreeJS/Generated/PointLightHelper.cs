// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This displays a helper object consisting of a spherical mesh for visualizing an instance of
/// <see cref="PointLight"/>. The JavaScript-side <c>THREE.PointLightHelper</c>.
/// </summary>
public sealed class PointLightHelper : Mesh
{
	private PointLight _light;
	private readonly float _sphereSize;
	private readonly Color? _color;
	private bool _isLightWritten;

	/// <summary>Constructs a new point light helper.</summary>
	/// <param name="light">The light to be visualized.</param>
	/// <param name="sphereSize">The size of the sphere helper.</param>
	/// <param name="color">The helper's color. If not set, the helper will take the color of the light.</param>
	public PointLightHelper(PointLight light, float sphereSize = 1f, Color? color = null)
	{
		_light = light;
		_sphereSize = sphereSize;
		_color = color;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PointLightHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PointLightHelper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.PointLightHelper</c>: light, sphereSize, color. An
	/// argument the caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed
	/// when nothing supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([_light, _sphereSize, ThreeValue.OrUnspecified(_color)]); }
	}

	/// <summary>
	/// The light being visualized. Writing it records a <c>light</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public PointLight Light
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
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>Updates the helper to match the position of the light being visualized.</summary>
	public void Update()
	{
		RecordCall("update");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.PointLightHelper</c> is constructed from, so their create ops
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
	}
}
