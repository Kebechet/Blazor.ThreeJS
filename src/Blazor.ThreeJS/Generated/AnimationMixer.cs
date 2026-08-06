// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// <c>AnimationMixer</c> is a player for animations on a particular object in the scene. When
/// multiple objects in the scene are animated independently, one <c>AnimationMixer</c> may be used
/// for each object. The JavaScript-side <c>THREE.AnimationMixer</c>.
/// </summary>
public sealed class AnimationMixer : EventDispatcher
{
	private readonly ThreeObject _root;
	private float _time = 0f;
	private float _timeScale = 1f;
	private bool _isTimeWritten;
	private bool _isTimeScaleWritten;

	/// <summary>Constructs a new animation mixer.</summary>
	/// <param name="root">The object whose animations shall be played by this mixer.</param>
	public AnimationMixer(ThreeObject root)
	{
		_root = root;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>AnimationMixer</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal AnimationMixer(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_root = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AnimationMixer</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AnimationMixer"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.AnimationMixer</c>: root.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_root]; }
	}

	/// <summary>
	/// The global mixer time (in seconds; starting with <c>0</c> on the mixer's creation). Writing it
	/// records a <c>time</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public float Time
	{
		get { return _time; }
		set
		{
			if (_time == value)
			{
				return;
			}

			_time = value;
			_isTimeWritten = true;
			RecordSet("time", value);
		}
	}

	/// <summary>
	/// A scaling factor for the global time. Note: Setting this member to <c>0</c> and later back to
	/// <c>1</c> is a possibility to pause/unpause all actions that are controlled by this mixer.
	/// Writing it records a <c>timeScale</c> property write once this object is attached; writing the
	/// value already held records nothing.
	/// </summary>
	public float TimeScale
	{
		get { return _timeScale; }
		set
		{
			if (_timeScale == value)
			{
				return;
			}

			_timeScale = value;
			_isTimeScaleWritten = true;
			RecordSet("timeScale", value);
		}
	}

	/// <summary>
	/// Advances the global mixer time and updates the animation. This is usually done in the render
	/// loop by passing the delta time from <see cref="Clock"/> or <see cref="Timer"/>.
	/// </summary>
	/// <param name="deltaTime">The delta time in seconds.</param>
	public void Update(float deltaTime)
	{
		RecordCall("update", deltaTime);
	}

	/// <summary>
	/// Sets the global mixer to a specific time and updates the animation accordingly. This is useful
	/// when you need to jump to an exact time in an animation. The input parameter will be scaled by
	/// <c>AnimationMixer#timeScale</c>. This writes the same three.js state as <see cref="Time"/> and
	/// the mirror does not learn from it: afterwards <c>Time</c> still reports its previous value, and
	/// writing that value back records nothing at all. Where the property exists, write the property.
	/// </summary>
	/// <param name="time">The time to set in seconds.</param>
	public void SetTime(float time)
	{
		RecordCall("setTime", time);
	}

	/// <summary>
	/// Deallocates all memory resources for a clip. Before using this method make sure to call
	/// <c>AnimationAction#stop</c> for all related actions.
	/// </summary>
	/// <param name="clip">The clip to uncache.</param>
	public void UncacheClip(AnimationClip clip)
	{
		RecordCall("uncacheClip", clip);
	}

	/// <summary>
	/// Deallocates all memory resources for a root object. Before using this method make sure to call
	/// <c>AnimationAction#stop</c> for all related actions or alternatively
	/// <c>AnimationMixer#stopAllAction</c> when the mixer operates on a single root.
	/// </summary>
	/// <param name="root">The root object to uncache.</param>
	public void UncacheRoot(ThreeObject root)
	{
		RecordCall("uncacheRoot", root);
	}

	/// <summary>
	/// Returns an instance of <see cref="AnimationAction"/> for the passed clip. If an action fitting
	/// the clip and root parameters doesn't yet exist, it will be created by this method. Calling this
	/// method several times with the same clip and root parameters always returns the same action.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>clipAction</c> returned.
	/// </summary>
	/// <param name="clip">An animation clip or alternatively the name of the animation clip.</param>
	/// <param name="optionalRoot">An alternative root object.</param>
	/// <param name="blendMode">The blend mode.</param>
	/// <returns>The value <c>clipAction</c> returned, once the JavaScript side has answered.</returns>
	public Task<AnimationAction?> ClipActionAsync(
		AnimationClip clip,
		ThreeObject optionalRoot,
		AnimationBlendMode blendMode)
	{
		return RecordReadObject<AnimationAction>("clipAction", (adoptedBatch, adoptedHandle) => new AnimationAction(adoptedBatch, adoptedHandle), clip, optionalRoot, blendMode);
	}

	/// <summary>
	/// Deactivates all previously scheduled actions on this mixer. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>stopAllAction</c> returned.
	/// </summary>
	/// <returns>The value <c>stopAllAction</c> returned, once the JavaScript side has answered.</returns>
	public Task<AnimationMixer?> StopAllActionAsync()
	{
		return RecordReadObject<AnimationMixer>("stopAllAction", (adoptedBatch, adoptedHandle) => new AnimationMixer(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Returns this mixer's root object. Records a read op, sends it behind every write already
	/// pending, and completes with what <c>getRoot</c> returned.
	/// </summary>
	/// <returns>The value <c>getRoot</c> returned, once the JavaScript side has answered.</returns>
	public Task<ThreeObject?> GetRootAsync()
	{
		return RecordReadObject<ThreeObject>("getRoot", (adoptedBatch, adoptedHandle) => new Primitive(adoptedBatch, adoptedHandle, "ThreeObject"));
	}

	/// <summary>
	/// Attaches the objects <c>THREE.AnimationMixer</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_root.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isTimeWritten)
		{
			batch.Set(Handle, "time", ThreeValue.Encode(_time));
		}

		if (_isTimeScaleWritten)
		{
			batch.Set(Handle, "timeScale", ThreeValue.Encode(_timeScale));
		}
	}
}
