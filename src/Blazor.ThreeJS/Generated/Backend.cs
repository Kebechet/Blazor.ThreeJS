// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.Backend</c>.</summary>
public abstract class Backend : ThreeObject
{
	private Renderer? _renderer;
	private bool _isRendererWritten;

	/// <summary>Initializes a new <see cref="Backend"/>.</summary>
	protected Backend()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Backend</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Backend(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Backend</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Backend"; }
	}

	/// <summary>
	/// The <c>renderer</c> property of the JavaScript-side object. Writing it records a <c>renderer</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Renderer? Renderer
	{
		get { return _renderer; }
		set
		{
			if (ReferenceEquals(_renderer, value))
			{
				return;
			}

			_renderer = value;
			_isRendererWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("renderer", value);
		}
	}

	/// <summary>Records a call to <c>init</c> on the JavaScript-side object.</summary>
	/// <param name="renderer">Value forwarded to the <c>renderer</c> argument.</param>
	public void Init(Renderer renderer)
	{
		RecordCall("init", renderer);
	}

	/// <summary>
	/// Reads <c>coordinateSystem</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>coordinateSystem</c> held.
	/// </summary>
	/// <returns>The value <c>coordinateSystem</c> held, once the JavaScript side has answered.</returns>
	public Task<CoordinateSystem> CoordinateSystemAsync()
	{
		return GetAsync<CoordinateSystem>("coordinateSystem");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Backend</c>, then replays every property written before this
	/// object was attached. A replayed value that is itself a mirrored object is attached first, so its
	/// create op reaches the batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isRendererWritten)
		{
			_renderer?.AttachTo(batch);
			batch.Set(Handle, "renderer", ThreeValue.Encode(_renderer));
		}
	}
}
