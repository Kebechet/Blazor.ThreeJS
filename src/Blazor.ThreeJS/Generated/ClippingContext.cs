// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Represents the state that is used to perform clipping via clipping planes. There is a default
/// clipping context for each render context. When the scene holds instances of
/// <c>ClippingGroup</c>, there will be a context for each group. The JavaScript-side
/// <c>THREE.ClippingContext</c>.
/// </summary>
public sealed class ClippingContext : ThreeObject
{
	private readonly ClippingContext? _parentContext;
	private bool? _clipIntersection = null;
	private string _cacheKey = string.Empty;
	private bool _shadowPass = false;
	private bool _isClipIntersectionWritten;
	private bool _isCacheKeyWritten;
	private bool _isShadowPassWritten;

	/// <summary>Constructs a new clipping context.</summary>
	/// <param name="parentContext">A reference to the parent clipping context.</param>
	public ClippingContext(ClippingContext? parentContext = null)
	{
		_parentContext = parentContext;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.ClippingContext</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "ClippingContext"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.ClippingContext</c>: parentContext. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_parentContext)]); }
	}

	/// <summary>
	/// Whether the intersection of the clipping planes is used to clip objects, rather than their
	/// union. Writing it records a <c>clipIntersection</c> property write once this object is attached;
	/// writing the value already held records nothing.
	/// </summary>
	public bool? ClipIntersection
	{
		get { return _clipIntersection; }
		set
		{
			if (_clipIntersection == value)
			{
				return;
			}

			_clipIntersection = value;
			_isClipIntersectionWritten = true;
			RecordSet("clipIntersection", value);
		}
	}

	/// <summary>
	/// The clipping context's cache key. Writing it records a <c>cacheKey</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public string CacheKey
	{
		get { return _cacheKey; }
		set
		{
			if (_cacheKey == value)
			{
				return;
			}

			_cacheKey = value;
			_isCacheKeyWritten = true;
			RecordSet("cacheKey", value);
		}
	}

	/// <summary>
	/// Whether the shadow pass is active or not. Writing it records a <c>shadowPass</c> property write
	/// once this object is attached; writing the value already held records nothing.
	/// </summary>
	public bool ShadowPass
	{
		get { return _shadowPass; }
		set
		{
			if (_shadowPass == value)
			{
				return;
			}

			_shadowPass = value;
			_isShadowPassWritten = true;
			RecordSet("shadowPass", value);
		}
	}

	/// <summary>Updates the root clipping context of a scene.</summary>
	/// <param name="scene">The scene.</param>
	/// <param name="camera">The camera that is used to render the scene.</param>
	public void UpdateGlobal(Scene scene, Camera camera)
	{
		if (Batch is not null)
		{
			scene.AttachTo(Batch);
		}

		if (Batch is not null)
		{
			camera.AttachTo(Batch);
		}

		RecordCall("updateGlobal", scene, camera);
	}

	/// <summary>Updates the clipping context.</summary>
	/// <param name="parentContext">The parent context.</param>
	/// <param name="clippingGroup">The clipping group this context belongs to.</param>
	public void Update(ClippingContext parentContext, ClippingGroup clippingGroup)
	{
		if (Batch is not null)
		{
			parentContext.AttachTo(Batch);
		}

		if (Batch is not null)
		{
			clippingGroup.AttachTo(Batch);
		}

		RecordCall("update", parentContext, clippingGroup);
	}

	/// <summary>Returns a clipping context for the given clipping group.</summary>
	/// <param name="clippingGroup">The clipping group.</param>
	public void GetGroupContext(ClippingGroup clippingGroup)
	{
		if (Batch is not null)
		{
			clippingGroup.AttachTo(Batch);
		}

		RecordCall("getGroupContext", clippingGroup);
	}

	/// <summary>
	/// Attaches the objects <c>THREE.ClippingContext</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_parentContext?.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isClipIntersectionWritten)
		{
			batch.Set(Handle, "clipIntersection", ThreeValue.Encode(_clipIntersection));
		}

		if (_isCacheKeyWritten)
		{
			batch.Set(Handle, "cacheKey", ThreeValue.Encode(_cacheKey));
		}

		if (_isShadowPassWritten)
		{
			batch.Set(Handle, "shadowPass", ThreeValue.Encode(_shadowPass));
		}
	}
}
