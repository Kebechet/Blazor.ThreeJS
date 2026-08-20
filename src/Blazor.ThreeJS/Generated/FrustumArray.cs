// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// FrustumArray is used to determine if an object is visible in at least one camera from an array
/// of cameras. This is particularly useful for multi-view renderers. The JavaScript-side
/// <c>THREE.FrustumArray</c>.
/// </summary>
public sealed class FrustumArray : ThreeObject
{
	private CoordinateSystem _coordinateSystem = CoordinateSystem.WebGLCoordinateSystem;
	private bool _isCoordinateSystemWritten;

	/// <summary>Initializes a new <see cref="FrustumArray"/>.</summary>
	public FrustumArray()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>FrustumArray</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal FrustumArray(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.FrustumArray</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "FrustumArray"; }
	}

	/// <summary>
	/// The coordinate system to use. Writing it records a <c>coordinateSystem</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public CoordinateSystem CoordinateSystem
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

	/// <summary>Computes and caches a frustum for each camera of the given array camera.</summary>
	/// <param name="cameraArray">The array camera whose sub-cameras define the frustums.</param>
	public void SetFromArrayCamera(ArrayCamera cameraArray)
	{
		RecordCall("setFromArrayCamera", cameraArray);
	}

	/// <summary>Copies the values of the given frustum array to this instance.</summary>
	/// <param name="source">The frustum array to copy.</param>
	public void Copy(FrustumArray source)
	{
		RecordCall("copy", source);
	}

	/// <summary>
	/// Returns <c>true</c> if the 3D object's bounding sphere is intersecting any cached frustum.
	/// <c>FrustumArray#setFromArrayCamera</c> must be called once per render before this method.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>intersectsObject</c> returned.
	/// </summary>
	/// <param name="object">The 3D object to test.</param>
	/// <returns>The value <c>intersectsObject</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> IntersectsObjectAsync(Object3D @object)
	{
		return RecordRead<bool>("intersectsObject", @object);
	}

	/// <summary>
	/// Returns <c>true</c> if the given sprite is intersecting any cached frustum.
	/// <c>FrustumArray#setFromArrayCamera</c> must be called once per render before this method.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>intersectsSprite</c> returned.
	/// </summary>
	/// <param name="sprite">The sprite to test.</param>
	/// <returns>The value <c>intersectsSprite</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> IntersectsSpriteAsync(Sprite sprite)
	{
		return RecordRead<bool>("intersectsSprite", sprite);
	}

	/// <summary>
	/// Returns <c>true</c> if the given bounding sphere is intersecting any cached frustum.
	/// <c>FrustumArray#setFromArrayCamera</c> must be called once per render before this method.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>intersectsSphere</c> returned.
	/// </summary>
	/// <param name="sphere">The bounding sphere to test.</param>
	/// <returns>The value <c>intersectsSphere</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> IntersectsSphereAsync(Sphere sphere)
	{
		return RecordRead<bool>("intersectsSphere", sphere);
	}

	/// <summary>
	/// Returns <c>true</c> if the given bounding box is intersecting any cached frustum.
	/// <c>FrustumArray#setFromArrayCamera</c> must be called once per render before this method.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>intersectsBox</c> returned.
	/// </summary>
	/// <param name="box">The bounding box to test.</param>
	/// <returns>The value <c>intersectsBox</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> IntersectsBoxAsync(Box3 box)
	{
		return RecordRead<bool>("intersectsBox", box);
	}

	/// <summary>
	/// Returns <c>true</c> if the given point lies within any cached frustum.
	/// <c>FrustumArray#setFromArrayCamera</c> must be called once per render before this method.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>containsPoint</c> returned.
	/// </summary>
	/// <param name="point">The point to test.</param>
	/// <returns>The value <c>containsPoint</c> returned, once the JavaScript side has answered.</returns>
	public Task<bool> ContainsPointAsync(Vector3 point)
	{
		return RecordRead<bool>("containsPoint", point);
	}

	/// <summary>
	/// Returns a new frustum array with copied values from this instance. Records a read op, sends it
	/// behind every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<FrustumArray?> CloneAsync()
	{
		return RecordReadObject<FrustumArray>("clone", (adoptedBatch, adoptedHandle) => new FrustumArray(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.FrustumArray</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isCoordinateSystemWritten)
		{
			batch.Set(Handle, "coordinateSystem", ThreeValue.Encode(_coordinateSystem));
		}
	}
}
