// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Represents a positional audio object. ```js // create an AudioListener and add it to the camera
/// const listener = new THREE.AudioListener(); camera.add( listener ); // create the
/// PositionalAudio object (passing in the listener) const sound = new THREE.PositionalAudio(
/// listener ); // load a sound and set it as the PositionalAudio object's buffer const audioLoader
/// = new THREE.AudioLoader(); audioLoader.load( 'sounds/song.ogg', function( buffer ) {
/// sound.setBuffer( buffer ); sound.setRefDistance( 20 ); sound.play(); }); // create an object for
/// the sound to play from const sphere = new THREE.SphereGeometry( 20, 32, 16 ); const material =
/// new THREE.MeshPhongMaterial( { color: 0xff2200 } ); const mesh = new THREE.Mesh( sphere,
/// material ); scene.add( mesh ); // finally add the sound to the mesh mesh.add( sound );. The
/// JavaScript-side <c>THREE.PositionalAudio</c>.
/// </summary>
public sealed class PositionalAudio : Audio
{
	private readonly AudioListener _listener;

	/// <summary>Initializes a new <see cref="PositionalAudio"/>.</summary>
	/// <param name="listener">The global audio listener.</param>
	public PositionalAudio(AudioListener listener)
		: base(listener: listener)
	{
		_listener = listener;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>PositionalAudio</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal PositionalAudio(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_listener = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PositionalAudio</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PositionalAudio"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.PositionalAudio</c>: listener.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_listener]; }
	}

	/// <summary>
	/// Defines the reference distance for reducing volume as the audio source moves further from the
	/// listener – i.e. the distance at which the volume reduction starts taking effect.
	/// </summary>
	/// <param name="value">The reference distance to set.</param>
	public void SetRefDistance(float value)
	{
		RecordCall("setRefDistance", value);
	}

	/// <summary>Defines how quickly the volume is reduced as the source moves away from the listener.</summary>
	/// <param name="value">The rolloff factor.</param>
	public void SetRolloffFactor(float value)
	{
		RecordCall("setRolloffFactor", value);
	}

	/// <summary>
	/// Defines which algorithm to use to reduce the volume of the audio source as it moves away from
	/// the listener. Read [the spec](https://www.w3.org/TR/webaudio-1.1/#enumdef-distancemodeltype) for
	/// more details.
	/// </summary>
	/// <param name="value">The distance model to set.</param>
	public void SetDistanceModel(DistanceModel value)
	{
		RecordCall("setDistanceModel", value);
	}

	/// <summary>
	/// Defines the maximum distance between the audio source and the listener, after which the volume
	/// is not reduced any further. This value is used only by the <c>linear</c> distance model.
	/// </summary>
	/// <param name="value">The max distance.</param>
	public void SetMaxDistance(float value)
	{
		RecordCall("setMaxDistance", value);
	}

	/// <summary>Sets the directional cone in which the audio can be listened.</summary>
	/// <param name="coneInnerAngle">
	/// An angle, in degrees, of a cone inside of which there will be no volume reduction.
	/// </param>
	/// <param name="coneOuterAngle">
	/// An angle, in degrees, of a cone outside of which the volume will be reduced by a constant value,
	/// defined by the <c>coneOuterGain</c> parameter.
	/// </param>
	/// <param name="coneOuterGain">
	/// The amount of volume reduction outside the cone defined by the <c>coneOuterAngle</c>. When set
	/// to <c>0</c>, no sound can be heard.
	/// </param>
	public void SetDirectionalCone(float coneInnerAngle, float coneOuterAngle, float coneOuterGain)
	{
		RecordCall("setDirectionalCone", coneInnerAngle, coneOuterAngle, coneOuterGain);
	}

	/// <summary>
	/// Returns the current reference distance. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getRefDistance</c> returned.
	/// </summary>
	/// <returns>The value <c>getRefDistance</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetRefDistanceAsync()
	{
		return RecordRead<float>("getRefDistance");
	}

	/// <summary>
	/// Returns the current rolloff factor. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getRolloffFactor</c> returned.
	/// </summary>
	/// <returns>The value <c>getRolloffFactor</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetRolloffFactorAsync()
	{
		return RecordRead<float>("getRolloffFactor");
	}

	/// <summary>
	/// Returns the current distance model. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getDistanceModel</c> returned.
	/// </summary>
	/// <returns>The value <c>getDistanceModel</c> returned, once the JavaScript side has answered.</returns>
	public Task<DistanceModel> GetDistanceModelAsync()
	{
		return RecordRead<DistanceModel>("getDistanceModel");
	}

	/// <summary>
	/// Returns the current max distance. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getMaxDistance</c> returned.
	/// </summary>
	/// <returns>The value <c>getMaxDistance</c> returned, once the JavaScript side has answered.</returns>
	public Task<float> GetMaxDistanceAsync()
	{
		return RecordRead<float>("getMaxDistance");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.PositionalAudio</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_listener.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
