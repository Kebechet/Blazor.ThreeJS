// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A special type of camera that is positioned in 3D space to render its surroundings into a cube
/// render target. The render target can then be used as an environment map for rendering realtime
/// reflections in your scene. The JavaScript-side <c>THREE.CubeCamera</c>.
/// </summary>
public sealed class CubeCamera : Object3D
{
	private readonly float _near;
	private readonly float _far;
	private readonly CubeRenderTarget _renderTarget;
	private CoordinateSystem? _coordinateSystem = null;
	private float _activeMipmapLevel = 0f;
	private bool _isCoordinateSystemWritten;
	private bool _isActiveMipmapLevelWritten;

	/// <summary>Constructs a new cube camera.</summary>
	/// <param name="near">The camera's near plane.</param>
	/// <param name="far">The camera's far plane.</param>
	/// <param name="renderTarget">The cube render target.</param>
	public CubeCamera(float near, float far, CubeRenderTarget renderTarget)
	{
		_near = near;
		_far = far;
		_renderTarget = renderTarget;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>CubeCamera</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal CubeCamera(ThreeBatch batch, int handle)
		: base(handle)
	{
		_near = default!;
		_far = default!;
		_renderTarget = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.CubeCamera</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "CubeCamera"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.CubeCamera</c>: near, far, renderTarget.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_near, _far, _renderTarget]; }
	}

	/// <summary>
	/// The current active coordinate system. Writing it records a <c>coordinateSystem</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public CoordinateSystem? CoordinateSystem
	{
		get { return _coordinateSystem; }
		set
		{
			if (_coordinateSystem == value)
			{
				return;
			}

			_coordinateSystem = value;
			_isCoordinateSystemWritten = true;
			RecordSet("coordinateSystem", value);
		}
	}

	/// <summary>
	/// The current active mipmap level. Writing it records a <c>activeMipmapLevel</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float ActiveMipmapLevel
	{
		get { return _activeMipmapLevel; }
		set
		{
			if (_activeMipmapLevel == value)
			{
				return;
			}

			_activeMipmapLevel = value;
			_isActiveMipmapLevelWritten = true;
			RecordSet("activeMipmapLevel", value);
		}
	}

	/// <summary>Must be called when the coordinate system of the cube camera is changed.</summary>
	public void UpdateCoordinateSystem()
	{
		RecordCall("updateCoordinateSystem");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.CubeCamera</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_renderTarget.AttachTo(batch);

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

		if (_isCoordinateSystemWritten)
		{
			batch.Set(Handle, "coordinateSystem", ThreeValue.Encode(_coordinateSystem));
		}

		if (_isActiveMipmapLevelWritten)
		{
			batch.Set(Handle, "activeMipmapLevel", ThreeValue.Encode(_activeMipmapLevel));
		}
	}
}
