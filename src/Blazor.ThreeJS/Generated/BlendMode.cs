// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.BlendMode</c>.</summary>
public sealed class BlendMode : ThreeObject
{
	private Blending _blending;
	private BlendingSrcFactor _blendSrc;
	private BlendingDstFactor _blendDst;
	private BlendingEquation _blendEquation;
	private BlendingSrcFactor? _blendSrcAlpha;
	private BlendingDstFactor? _blendDstAlpha;
	private BlendingEquation? _blendEquationAlpha;
	private bool _premultiplyAlpha;
	private bool _isBlendingWritten;
	private bool _isBlendSrcWritten;
	private bool _isBlendDstWritten;
	private bool _isBlendEquationWritten;
	private bool _isBlendSrcAlphaWritten;
	private bool _isBlendDstAlphaWritten;
	private bool _isBlendEquationAlphaWritten;
	private bool _isPremultiplyAlphaWritten;

	/// <summary>Initializes a new <see cref="BlendMode"/>.</summary>
	/// <param name="blending">Value forwarded to the <c>blending</c> constructor argument.</param>
	public BlendMode(Blending blending)
	{
		_blending = blending;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>BlendMode</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal BlendMode(ThreeBatch batch, int handle)
		: base(handle)
	{
		_blending = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BlendMode</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "BlendMode"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.BlendMode</c>: blending.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_blending]; }
	}

	/// <summary>
	/// The <c>blending</c> property of the JavaScript-side object. Writing it records a <c>blending</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Blending Blending
	{
		get { return _blending; }
		set
		{
			if (_blending == value)
			{
				return;
			}

			_blending = value;
			_isBlendingWritten = true;
			RecordSet("blending", value);
		}
	}

	/// <summary>
	/// The <c>blendSrc</c> property of the JavaScript-side object. Writing it records a <c>blendSrc</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public BlendingSrcFactor BlendSrc
	{
		get { return _blendSrc; }
		set
		{
			if (_blendSrc == value)
			{
				return;
			}

			_blendSrc = value;
			_isBlendSrcWritten = true;
			RecordSet("blendSrc", value);
		}
	}

	/// <summary>
	/// The <c>blendDst</c> property of the JavaScript-side object. Writing it records a <c>blendDst</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public BlendingDstFactor BlendDst
	{
		get { return _blendDst; }
		set
		{
			if (_blendDst == value)
			{
				return;
			}

			_blendDst = value;
			_isBlendDstWritten = true;
			RecordSet("blendDst", value);
		}
	}

	/// <summary>
	/// The <c>blendEquation</c> property of the JavaScript-side object. Writing it records a
	/// <c>blendEquation</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public BlendingEquation BlendEquation
	{
		get { return _blendEquation; }
		set
		{
			if (_blendEquation == value)
			{
				return;
			}

			_blendEquation = value;
			_isBlendEquationWritten = true;
			RecordSet("blendEquation", value);
		}
	}

	/// <summary>
	/// The <c>blendSrcAlpha</c> property of the JavaScript-side object. Writing it records a
	/// <c>blendSrcAlpha</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public BlendingSrcFactor? BlendSrcAlpha
	{
		get { return _blendSrcAlpha; }
		set
		{
			if (_blendSrcAlpha == value)
			{
				return;
			}

			_blendSrcAlpha = value;
			_isBlendSrcAlphaWritten = true;
			RecordSet("blendSrcAlpha", value);
		}
	}

	/// <summary>
	/// The <c>blendDstAlpha</c> property of the JavaScript-side object. Writing it records a
	/// <c>blendDstAlpha</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public BlendingDstFactor? BlendDstAlpha
	{
		get { return _blendDstAlpha; }
		set
		{
			if (_blendDstAlpha == value)
			{
				return;
			}

			_blendDstAlpha = value;
			_isBlendDstAlphaWritten = true;
			RecordSet("blendDstAlpha", value);
		}
	}

	/// <summary>
	/// The <c>blendEquationAlpha</c> property of the JavaScript-side object. Writing it records a
	/// <c>blendEquationAlpha</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public BlendingEquation? BlendEquationAlpha
	{
		get { return _blendEquationAlpha; }
		set
		{
			if (_blendEquationAlpha == value)
			{
				return;
			}

			_blendEquationAlpha = value;
			_isBlendEquationAlphaWritten = true;
			RecordSet("blendEquationAlpha", value);
		}
	}

	/// <summary>
	/// The <c>premultiplyAlpha</c> property of the JavaScript-side object. Writing it records a
	/// <c>premultiplyAlpha</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public bool PremultiplyAlpha
	{
		get { return _premultiplyAlpha; }
		set
		{
			if (_premultiplyAlpha == value)
			{
				return;
			}

			_premultiplyAlpha = value;
			_isPremultiplyAlphaWritten = true;
			RecordSet("premultiplyAlpha", value);
		}
	}

	/// <summary>Records a call to <c>copy</c> on the JavaScript-side object.</summary>
	/// <param name="source">Value forwarded to the <c>source</c> argument.</param>
	public void Copy(BlendMode source)
	{
		RecordCall("copy", source);
	}

	/// <summary>
	/// Reads <c>clone</c> back from the JavaScript-side object. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<BlendMode?> CloneAsync()
	{
		return RecordReadObject<BlendMode>("clone", (adoptedBatch, adoptedHandle) => new BlendMode(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.BlendMode</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isBlendingWritten)
		{
			batch.Set(Handle, "blending", ThreeValue.Encode(_blending));
		}

		if (_isBlendSrcWritten)
		{
			batch.Set(Handle, "blendSrc", ThreeValue.Encode(_blendSrc));
		}

		if (_isBlendDstWritten)
		{
			batch.Set(Handle, "blendDst", ThreeValue.Encode(_blendDst));
		}

		if (_isBlendEquationWritten)
		{
			batch.Set(Handle, "blendEquation", ThreeValue.Encode(_blendEquation));
		}

		if (_isBlendSrcAlphaWritten)
		{
			batch.Set(Handle, "blendSrcAlpha", ThreeValue.Encode(_blendSrcAlpha));
		}

		if (_isBlendDstAlphaWritten)
		{
			batch.Set(Handle, "blendDstAlpha", ThreeValue.Encode(_blendDstAlpha));
		}

		if (_isBlendEquationAlphaWritten)
		{
			batch.Set(Handle, "blendEquationAlpha", ThreeValue.Encode(_blendEquationAlpha));
		}

		if (_isPremultiplyAlphaWritten)
		{
			batch.Set(Handle, "premultiplyAlpha", ThreeValue.Encode(_premultiplyAlpha));
		}
	}
}
