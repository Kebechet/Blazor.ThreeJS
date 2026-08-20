// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.RenderPipeline</c>.</summary>
public class RenderPipeline : ThreeObject
{
	private Renderer _renderer;
	private bool _outputColorTransform;
	private bool _needsUpdate;
	private bool _isRendererWritten;
	private bool _isOutputColorTransformWritten;
	private bool _isNeedsUpdateWritten;

	/// <summary>Initializes a new <see cref="RenderPipeline"/>.</summary>
	/// <param name="renderer">Value forwarded to the <c>renderer</c> constructor argument.</param>
	public RenderPipeline(Renderer renderer)
	{
		_renderer = renderer;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>RenderPipeline</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal RenderPipeline(ThreeBatch batch, int handle)
		: base(handle)
	{
		_renderer = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.RenderPipeline</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "RenderPipeline"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.RenderPipeline</c>: renderer.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_renderer]; }
	}

	/// <summary>
	/// The <c>renderer</c> property of the JavaScript-side object. Writing it records a <c>renderer</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Renderer Renderer
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

	/// <summary>
	/// The <c>outputColorTransform</c> property of the JavaScript-side object. Writing it records a
	/// <c>outputColorTransform</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public bool OutputColorTransform
	{
		get { return _outputColorTransform; }
		set
		{
			if (_outputColorTransform == value)
			{
				return;
			}

			_outputColorTransform = value;
			_isOutputColorTransformWritten = true;
			RecordSet("outputColorTransform", value);
		}
	}

	/// <summary>
	/// The <c>needsUpdate</c> property of the JavaScript-side object. Writing it records a
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

	/// <summary>Records a call to <c>render</c> on the JavaScript-side object.</summary>
	public void Render()
	{
		RecordCall("render");
	}

	/// <summary>Records a call to <c>dispose</c> on the JavaScript-side object.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Reads <c>renderAsync</c> back from the JavaScript-side object. Answers nothing, and is awaited
	/// for when rather than for what: records a read op, sends it behind every write already pending,
	/// and completes once the promise <c>renderAsync</c> returned has settled.
	/// </summary>
	/// <returns>A task that completes once <c>renderAsync</c> has finished.</returns>
	public Task RenderAsync()
	{
		return RecordRead<object?>("renderAsync");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.RenderPipeline</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own. A
	/// replayed value that is itself a mirrored object is attached first, so its create op reaches the
	/// batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_renderer.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isRendererWritten)
		{
			_renderer.AttachTo(batch);
			batch.Set(Handle, "renderer", ThreeValue.Encode(_renderer));
		}

		if (_isOutputColorTransformWritten)
		{
			batch.Set(Handle, "outputColorTransform", ThreeValue.Encode(_outputColorTransform));
		}

		if (_isNeedsUpdateWritten)
		{
			batch.Set(Handle, "needsUpdate", ThreeValue.Encode(_needsUpdate));
		}
	}
}
