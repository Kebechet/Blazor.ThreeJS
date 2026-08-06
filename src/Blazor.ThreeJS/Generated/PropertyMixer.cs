// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Buffered scene graph property that allows weighted accumulation; used internally. The
/// JavaScript-side <c>THREE.PropertyMixer</c>.
/// </summary>
public sealed class PropertyMixer : ThreeObject
{
	private PropertyBinding _binding;
	private readonly string _typeName;
	private float _valueSize;
	private float _cumulativeWeight = 0f;
	private float _cumulativeWeightAdditive = 0f;
	private int _useCount = 0;
	private int _referenceCount = 0;
	private bool _isBindingWritten;
	private bool _isValueSizeWritten;
	private bool _isCumulativeWeightWritten;
	private bool _isCumulativeWeightAdditiveWritten;
	private bool _isUseCountWritten;
	private bool _isReferenceCountWritten;

	/// <summary>Constructs a new property mixer.</summary>
	/// <param name="binding">The property binding.</param>
	/// <param name="typeName">The keyframe track type name.</param>
	/// <param name="valueSize">The keyframe track value size.</param>
	public PropertyMixer(PropertyBinding binding, string typeName, float valueSize)
	{
		_binding = binding;
		_typeName = typeName;
		_valueSize = valueSize;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>PropertyMixer</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal PropertyMixer(ThreeBatch batch, int handle)
		: base(handle)
	{
		_binding = default!;
		_typeName = default!;
		_valueSize = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PropertyMixer</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PropertyMixer"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.PropertyMixer</c>: binding, typeName, valueSize.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_binding, _typeName, _valueSize]; }
	}

	/// <summary>
	/// The property binding. Writing it records a <c>binding</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public PropertyBinding Binding
	{
		get { return _binding; }
		set
		{
			if (ReferenceEquals(_binding, value))
			{
				return;
			}

			_binding = value;
			_isBindingWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("binding", value);
		}
	}

	/// <summary>
	/// The keyframe track value size. Writing it records a <c>valueSize</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public float ValueSize
	{
		get { return _valueSize; }
		set
		{
			if (_valueSize == value)
			{
				return;
			}

			_valueSize = value;
			_isValueSizeWritten = true;
			RecordSet("valueSize", value);
		}
	}

	/// <summary>
	/// Accumulated weight of the property binding. Writing it records a <c>cumulativeWeight</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float CumulativeWeight
	{
		get { return _cumulativeWeight; }
		set
		{
			if (_cumulativeWeight == value)
			{
				return;
			}

			_cumulativeWeight = value;
			_isCumulativeWeightWritten = true;
			RecordSet("cumulativeWeight", value);
		}
	}

	/// <summary>
	/// Accumulated additive weight of the property binding. Writing it records a
	/// <c>cumulativeWeightAdditive</c> property write once this object is attached; writing the value
	/// already held records nothing.
	/// </summary>
	public float CumulativeWeightAdditive
	{
		get { return _cumulativeWeightAdditive; }
		set
		{
			if (_cumulativeWeightAdditive == value)
			{
				return;
			}

			_cumulativeWeightAdditive = value;
			_isCumulativeWeightAdditiveWritten = true;
			RecordSet("cumulativeWeightAdditive", value);
		}
	}

	/// <summary>
	/// Number of active keyframe tracks currently using this property binding. Writing it records a
	/// <c>useCount</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public int UseCount
	{
		get { return _useCount; }
		set
		{
			if (_useCount == value)
			{
				return;
			}

			_useCount = value;
			_isUseCountWritten = true;
			RecordSet("useCount", value);
		}
	}

	/// <summary>
	/// Number of keyframe tracks referencing this property binding. Writing it records a
	/// <c>referenceCount</c> property write once this object is attached; writing the value already
	/// held records nothing.
	/// </summary>
	public int ReferenceCount
	{
		get { return _referenceCount; }
		set
		{
			if (_referenceCount == value)
			{
				return;
			}

			_referenceCount = value;
			_isReferenceCountWritten = true;
			RecordSet("referenceCount", value);
		}
	}

	/// <summary>Accumulates data in the <c>incoming</c> region into <c>accu&lt;i&gt;</c>.</summary>
	/// <param name="accuIndex">The accumulation index.</param>
	/// <param name="weight">The weight.</param>
	public void Accumulate(int accuIndex, float weight)
	{
		RecordCall("accumulate", accuIndex, weight);
	}

	/// <summary>Accumulates data in the <c>incoming</c> region into <c>add</c>.</summary>
	/// <param name="weight">The weight.</param>
	public void AccumulateAdditive(float weight)
	{
		RecordCall("accumulateAdditive", weight);
	}

	/// <summary>Applies the state of <c>accu&lt;i&gt;</c> to the binding when accus differ.</summary>
	/// <param name="accuIndex">The accumulation index.</param>
	public void Apply(int accuIndex)
	{
		RecordCall("apply", accuIndex);
	}

	/// <summary>Remembers the state of the bound property and copy it to both accus.</summary>
	public void SaveOriginalState()
	{
		RecordCall("saveOriginalState");
	}

	/// <summary>
	/// Applies the state previously taken via <c>PropertyMixer#saveOriginalState</c> to the binding.
	/// </summary>
	public void RestoreOriginalState()
	{
		RecordCall("restoreOriginalState");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.PropertyMixer</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own. A
	/// replayed value that is itself a mirrored object is attached first, so its create op reaches the
	/// batch before the write that references it by handle.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_binding.AttachTo(batch);

		base.EmitCreate(batch);

		if (_isBindingWritten)
		{
			_binding.AttachTo(batch);
			batch.Set(Handle, "binding", ThreeValue.Encode(_binding));
		}

		if (_isValueSizeWritten)
		{
			batch.Set(Handle, "valueSize", ThreeValue.Encode(_valueSize));
		}

		if (_isCumulativeWeightWritten)
		{
			batch.Set(Handle, "cumulativeWeight", ThreeValue.Encode(_cumulativeWeight));
		}

		if (_isCumulativeWeightAdditiveWritten)
		{
			batch.Set(Handle, "cumulativeWeightAdditive", ThreeValue.Encode(_cumulativeWeightAdditive));
		}

		if (_isUseCountWritten)
		{
			batch.Set(Handle, "useCount", ThreeValue.Encode(_useCount));
		}

		if (_isReferenceCountWritten)
		{
			batch.Set(Handle, "referenceCount", ThreeValue.Encode(_referenceCount));
		}
	}
}
