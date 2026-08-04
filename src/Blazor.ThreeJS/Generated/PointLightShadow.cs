// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Represents the shadow configuration of point lights. The JavaScript-side
/// <c>THREE.PointLightShadow</c>.
/// </summary>
public sealed class PointLightShadow : ThreeObject
{
	private Camera? _camera;
	private float _intensity = 1f;
	private float _bias = 0f;
	private float _normalBias = 0f;
	private float _radius = 1f;
	private float _blurSamples = 8f;
	private TextureDataType _mapType = TextureDataType.UnsignedByteType;
	private RenderTarget? _map = null;
	private RenderTarget? _mapPass = null;
	private bool _autoUpdate = true;
	private bool _needsUpdate = false;
	private bool _isCameraWritten;
	private bool _isIntensityWritten;
	private bool _isBiasWritten;
	private bool _isNormalBiasWritten;
	private bool _isRadiusWritten;
	private bool _isBlurSamplesWritten;
	private bool _isMapTypeWritten;
	private bool _isMapWritten;
	private bool _isMapPassWritten;
	private bool _isAutoUpdateWritten;
	private bool _isNeedsUpdateWritten;

	/// <summary>Constructs a new point light shadow.</summary>
	public PointLightShadow()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PointLightShadow</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PointLightShadow"; }
	}

	/// <summary>
	/// The light's view of the world. Writing it records a <c>camera</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public Camera? Camera
	{
		get { return _camera; }
		set
		{
			if (ReferenceEquals(_camera, value))
			{
				return;
			}

			_camera = value;
			_isCameraWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("camera", value);
		}
	}

	/// <summary>
	/// The intensity of the shadow. The default is <c>1</c>. Valid values are in the range <c>[0,
	/// 1]</c>. Writing it records a <c>intensity</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public float Intensity
	{
		get { return _intensity; }
		set
		{
			if (_intensity == value)
			{
				return;
			}

			_intensity = value;
			_isIntensityWritten = true;
			RecordSet("intensity", value);
		}
	}

	/// <summary>
	/// Shadow map bias, how much to add or subtract from the normalized depth when deciding whether a
	/// surface is in shadow. The default is <c>0</c>. Very tiny adjustments here (in the order of
	/// <c>0.0001</c>) may help reduce artifacts in shadows. Writing it records a <c>bias</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Bias
	{
		get { return _bias; }
		set
		{
			if (_bias == value)
			{
				return;
			}

			_bias = value;
			_isBiasWritten = true;
			RecordSet("bias", value);
		}
	}

	/// <summary>
	/// Defines how much the position used to query the shadow map is offset along the object normal.
	/// The default is <c>0</c>. Increasing this value can be used to reduce shadow acne especially in
	/// large scenes where light shines onto geometry at a shallow angle. The cost is that shadows may
	/// appear distorted. Writing it records a <c>normalBias</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float NormalBias
	{
		get { return _normalBias; }
		set
		{
			if (_normalBias == value)
			{
				return;
			}

			_normalBias = value;
			_isNormalBiasWritten = true;
			RecordSet("normalBias", value);
		}
	}

	/// <summary>
	/// Setting this to values greater than 1 will blur the edges of the shadow. High values will cause
	/// unwanted banding effects in the shadows - a greater map size will allow for a higher value to be
	/// used here before these effects become visible. The property has no effect when the shadow map
	/// type is <c>BasicShadowMap</c>. Writing it records a <c>radius</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float Radius
	{
		get { return _radius; }
		set
		{
			if (_radius == value)
			{
				return;
			}

			_radius = value;
			_isRadiusWritten = true;
			RecordSet("radius", value);
		}
	}

	/// <summary>
	/// The amount of samples to use when blurring a VSM shadow map. Writing it records a
	/// <c>blurSamples</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public float BlurSamples
	{
		get { return _blurSamples; }
		set
		{
			if (_blurSamples == value)
			{
				return;
			}

			_blurSamples = value;
			_isBlurSamplesWritten = true;
			RecordSet("blurSamples", value);
		}
	}

	/// <summary>
	/// The type of shadow texture. The default is <c>UnsignedByteType</c>. Writing it records a
	/// <c>mapType</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public TextureDataType MapType
	{
		get { return _mapType; }
		set
		{
			if (_mapType == value)
			{
				return;
			}

			_mapType = value;
			_isMapTypeWritten = true;
			RecordSet("mapType", value);
		}
	}

	/// <summary>
	/// The depth map generated using the internal camera; a location beyond a pixel's depth is in
	/// shadow. Computed internally during rendering. Writing it records a <c>map</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public RenderTarget? Map
	{
		get { return _map; }
		set
		{
			if (ReferenceEquals(_map, value))
			{
				return;
			}

			_map = value;
			_isMapWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("map", value);
		}
	}

	/// <summary>
	/// The distribution map generated using the internal camera; an occlusion is calculated based on
	/// the distribution of depths. Computed internally during rendering. Writing it records a
	/// <c>mapPass</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public RenderTarget? MapPass
	{
		get { return _mapPass; }
		set
		{
			if (ReferenceEquals(_mapPass, value))
			{
				return;
			}

			_mapPass = value;
			_isMapPassWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("mapPass", value);
		}
	}

	/// <summary>
	/// Enables automatic updates of the light's shadow. If you do not require dynamic lighting /
	/// shadows, you may set this to <c>false</c>. Writing it records a <c>autoUpdate</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool AutoUpdate
	{
		get { return _autoUpdate; }
		set
		{
			if (_autoUpdate == value)
			{
				return;
			}

			_autoUpdate = value;
			_isAutoUpdateWritten = true;
			RecordSet("autoUpdate", value);
		}
	}

	/// <summary>
	/// When set to <c>true</c>, shadow maps will be updated in the next <c>render</c> call. If you have
	/// set <c>LightShadow#autoUpdate</c> to <c>false</c>, you will need to set this property to
	/// <c>true</c> and then make a render call to update the light's shadow. Writing it records a
	/// <c>needsUpdate</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public bool NeedsUpdate
	{
		get { return _needsUpdate; }
		set
		{
			if (_needsUpdate == value)
			{
				return;
			}

			_needsUpdate = value;
			_isNeedsUpdateWritten = true;
			RecordSet("needsUpdate", value);
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

	/// <summary>
	/// Emits the create op for <c>THREE.PointLightShadow</c>, then replays every property written
	/// before this object was attached. A replayed value that is itself a mirrored object is attached
	/// first, so its create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isCameraWritten)
		{
			_camera?.AttachTo(batch);
			batch.Set(Handle, "camera", ThreeValue.Encode(_camera));
		}

		if (_isIntensityWritten)
		{
			batch.Set(Handle, "intensity", ThreeValue.Encode(_intensity));
		}

		if (_isBiasWritten)
		{
			batch.Set(Handle, "bias", ThreeValue.Encode(_bias));
		}

		if (_isNormalBiasWritten)
		{
			batch.Set(Handle, "normalBias", ThreeValue.Encode(_normalBias));
		}

		if (_isRadiusWritten)
		{
			batch.Set(Handle, "radius", ThreeValue.Encode(_radius));
		}

		if (_isBlurSamplesWritten)
		{
			batch.Set(Handle, "blurSamples", ThreeValue.Encode(_blurSamples));
		}

		if (_isMapTypeWritten)
		{
			batch.Set(Handle, "mapType", ThreeValue.Encode(_mapType));
		}

		if (_isMapWritten)
		{
			_map?.AttachTo(batch);
			batch.Set(Handle, "map", ThreeValue.Encode(_map));
		}

		if (_isMapPassWritten)
		{
			_mapPass?.AttachTo(batch);
			batch.Set(Handle, "mapPass", ThreeValue.Encode(_mapPass));
		}

		if (_isAutoUpdateWritten)
		{
			batch.Set(Handle, "autoUpdate", ThreeValue.Encode(_autoUpdate));
		}

		if (_isNeedsUpdateWritten)
		{
			batch.Set(Handle, "needsUpdate", ThreeValue.Encode(_needsUpdate));
		}
	}
}
