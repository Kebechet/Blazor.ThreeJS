// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Helper object to assist with visualizing a <see cref="DirectionalLight"/>'s effect on the scene.
/// This consists of a plane and a line representing the light's position and direction. When the
/// directional light or its target are transformed or light properties are changed, it's necessary
/// to call the <c>update()</c> method of the respective helper. The JavaScript-side
/// <c>THREE.DirectionalLightHelper</c>.
/// </summary>
public sealed class DirectionalLightHelper : Object3D
{
	private DirectionalLight _light;
	private readonly float _size;
	private readonly Color? _color;
	private Line? _lightPlane;
	private Line? _targetLine;
	private bool _isLightWritten;
	private bool _isLightPlaneWritten;
	private bool _isTargetLineWritten;

	/// <summary>Constructs a new directional light helper.</summary>
	/// <param name="light">The light to be visualized.</param>
	/// <param name="size">The dimensions of the plane.</param>
	/// <param name="color">The helper's color. If not set, the helper will take the color of the light.</param>
	public DirectionalLightHelper(DirectionalLight light, float size = 1f, Color? color = null)
	{
		_light = light;
		_size = size;
		_color = color;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.DirectionalLightHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "DirectionalLightHelper"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.DirectionalLightHelper</c>: light, size, color. An
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
	public DirectionalLight Light
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
	/// Contains the line showing the location of the directional light. Writing it records a
	/// <c>lightPlane</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public Line? LightPlane
	{
		get { return _lightPlane; }
		set
		{
			if (ReferenceEquals(_lightPlane, value))
			{
				return;
			}

			_lightPlane = value;
			_isLightPlaneWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("lightPlane", value);
		}
	}

	/// <summary>
	/// Represents the target line of the directional light. Writing it records a <c>targetLine</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Line? TargetLine
	{
		get { return _targetLine; }
		set
		{
			if (ReferenceEquals(_targetLine, value))
			{
				return;
			}

			_targetLine = value;
			_isTargetLineWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("targetLine", value);
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
	/// Attaches the objects <c>THREE.DirectionalLightHelper</c> is constructed from, so their create
	/// ops reach the batch before the one that references them by handle, then emits this object's own.
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

		if (_isLightPlaneWritten)
		{
			batch.Set(Handle, "lightPlane", ThreeValue.Encode(_lightPlane));
		}

		if (_isTargetLineWritten)
		{
			batch.Set(Handle, "targetLine", ThreeValue.Encode(_targetLine));
		}
	}
}
