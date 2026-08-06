// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.UniformsGroup</c>.</summary>
/// <seealso href="https://threejs.org/examples/#webgl2_ubo">WebGL2 / UBO</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/UniformsGroup.js">Source</seealso>
public sealed class UniformsGroup : EventDispatcher
{
	private float _id;
	private Usage _usage;
	private bool _isIdWritten;
	private bool _isUsageWritten;

	/// <summary>Initializes a new <see cref="UniformsGroup"/>.</summary>
	public UniformsGroup()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>UniformsGroup</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal UniformsGroup(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.UniformsGroup</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "UniformsGroup"; }
	}

	/// <summary>
	/// The <c>id</c> property of the JavaScript-side object. Writing it records a <c>id</c> property
	/// write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Id
	{
		get { return _id; }
		set
		{
			if (_id == value)
			{
				return;
			}

			_id = value;
			_isIdWritten = true;
			RecordSet("id", value);
		}
	}

	/// <summary>
	/// The <c>usage</c> property of the JavaScript-side object. Writing it records a <c>usage</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public Usage Usage
	{
		get { return _usage; }
		set
		{
			if (_usage == value)
			{
				return;
			}

			_usage = value;
			_isUsageWritten = true;
			RecordSet("usage", value);
		}
	}

	/// <summary>Records a call to <c>setName</c> on the JavaScript-side object.</summary>
	/// <param name="name">Value forwarded to the <c>name</c> argument.</param>
	public void SetName(string name)
	{
		RecordCall("setName", name);
	}

	/// <summary>
	/// Records a call to <c>setUsage</c> on the JavaScript-side object. This writes the same three.js
	/// state as <see cref="Usage"/> and the mirror does not learn from it: afterwards <c>Usage</c>
	/// still reports its previous value, and writing that value back records nothing at all. Where the
	/// property exists, write the property.
	/// </summary>
	/// <param name="value">Value forwarded to the <c>value</c> argument.</param>
	public void SetUsage(Usage value)
	{
		RecordCall("setUsage", value);
	}

	/// <summary>Records a call to <c>dispose</c> on the JavaScript-side object.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>Records a call to <c>copy</c> on the JavaScript-side object.</summary>
	/// <param name="source">Value forwarded to the <c>source</c> argument.</param>
	public void Copy(UniformsGroup source)
	{
		RecordCall("copy", source);
	}

	/// <summary>
	/// Reads <c>isUniformsGroup</c> back from the JavaScript-side object. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>isUniformsGroup</c> held.
	/// </summary>
	/// <returns>The value <c>isUniformsGroup</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsUniformsGroupAsync()
	{
		return GetAsync<bool>("isUniformsGroup");
	}

	/// <summary>
	/// Reads <c>clone</c> back from the JavaScript-side object. Records a read op, sends it behind
	/// every write already pending, and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<UniformsGroup?> CloneAsync()
	{
		return RecordReadObject<UniformsGroup>("clone", (adoptedBatch, adoptedHandle) => new UniformsGroup(adoptedBatch, adoptedHandle));
	}

	/// <summary>
	/// Emits the create op for <c>THREE.UniformsGroup</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isIdWritten)
		{
			batch.Set(Handle, "id", ThreeValue.Encode(_id));
		}

		if (_isUsageWritten)
		{
			batch.Set(Handle, "usage", ThreeValue.Encode(_usage));
		}
	}
}
