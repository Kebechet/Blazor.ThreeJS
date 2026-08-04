// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This displays a cone shaped helper object for a <see cref="SpotLight"/>. When the spot light or
/// its target are transformed or light properties are changed, it's necessary to call the
/// <c>update()</c> method of the respective helper. The JavaScript-side
/// <c>THREE.SpotLightHelper</c>.
/// </summary>
public sealed class SpotLightHelper : Object3D
{
	private SpotLight _light;
	private readonly Color? _color;
	private LineSegments? _cone;
	private bool _isLightWritten;
	private bool _isConeWritten;

	/// <summary>Constructs a new spot light helper.</summary>
	/// <param name="light">The light to be visualized.</param>
	/// <param name="color">The helper's color. If not set, the helper will take the color of the light.</param>
	public SpotLightHelper(SpotLight light, Color? color = null)
	{
		_light = light;
		_color = color;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.SpotLightHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "SpotLightHelper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.SpotLightHelper</c>: light, color. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([_light, ThreeValue.OrUnspecified(_color)]); }
	}

	/// <summary>
	/// The light being visualized. Writing it records a <c>light</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public SpotLight Light
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
	/// The <c>cone</c> property of the JavaScript-side object. Writing it records a <c>cone</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public LineSegments? Cone
	{
		get { return _cone; }
		set
		{
			if (ReferenceEquals(_cone, value))
			{
				return;
			}

			_cone = value;
			_isConeWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("cone", value);
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
	/// Attaches the objects <c>THREE.SpotLightHelper</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
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
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isLightWritten)
		{
			batch.Set(Handle, "light", ThreeValue.Encode(_light));
		}

		if (_isConeWritten)
		{
			batch.Set(Handle, "cone", ThreeValue.Encode(_cone));
		}
	}
}
