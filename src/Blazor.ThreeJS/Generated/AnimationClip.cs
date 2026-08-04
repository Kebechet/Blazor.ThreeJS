// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A reusable set of keyframe tracks which represent an animation. The JavaScript-side
/// <c>THREE.AnimationClip</c>.
/// </summary>
public sealed class AnimationClip : ThreeObject
{
	private string? _name;
	private float _duration;
	private AnimationBlendMode _blendMode;
	private bool _isNameWritten;
	private bool _isDurationWritten;
	private bool _isBlendModeWritten;

	/// <summary>
	/// Constructs a new animation clip. Note: Instead of instantiating an AnimationClip directly with
	/// the constructor, you can use the static interface of this class for creating clips. In most
	/// cases though, animation clips will automatically be created by loaders when importing animated
	/// 3D assets.
	/// </summary>
	/// <param name="name">The clip's name.</param>
	/// <param name="duration">
	/// The clip's duration in seconds. If a negative value is passed, the duration will be calculated
	/// from the passed keyframes.
	/// </param>
	public AnimationClip(string? name = null, float duration = -1f)
	{
		_name = name;
		_duration = duration;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AnimationClip</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AnimationClip"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.AnimationClip</c>: name, duration. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_name), _duration]); }
	}

	/// <summary>
	/// The clip's name. Writing it records a <c>name</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public string? Name
	{
		get { return _name; }
		set
		{
			if (_name == value)
			{
				return;
			}

			_name = value;
			_isNameWritten = true;
			RecordSet("name", value);
		}
	}

	/// <summary>
	/// The clip's duration in seconds. Writing it records a <c>duration</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float Duration
	{
		get { return _duration; }
		set
		{
			if (_duration == value)
			{
				return;
			}

			_duration = value;
			_isDurationWritten = true;
			RecordSet("duration", value);
		}
	}

	/// <summary>
	/// Defines how the animation is blended/combined when two or more animations are simultaneously
	/// played. Writing it records a <c>blendMode</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public AnimationBlendMode BlendMode
	{
		get { return _blendMode; }
		set
		{
			if (_blendMode == value)
			{
				return;
			}

			_blendMode = value;
			_isBlendModeWritten = true;
			RecordSet("blendMode", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.AnimationClip</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isNameWritten)
		{
			batch.Set(Handle, "name", ThreeValue.Encode(_name));
		}

		if (_isDurationWritten)
		{
			batch.Set(Handle, "duration", ThreeValue.Encode(_duration));
		}

		if (_isBlendModeWritten)
		{
			batch.Set(Handle, "blendMode", ThreeValue.Encode(_blendMode));
		}
	}
}
