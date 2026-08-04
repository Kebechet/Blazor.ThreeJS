// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>The JavaScript-side <c>THREE.SourceJSON</c>.</summary>
public sealed class SourceJSON : ThreeObject
{
	private string _uuid = string.Empty;
	private bool _isUuidWritten;

	/// <summary>Initializes a new <see cref="SourceJSON"/>.</summary>
	public SourceJSON()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.SourceJSON</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "SourceJSON"; }
	}

	/// <summary>
	/// The <c>uuid</c> property of the JavaScript-side object. Writing it records a <c>uuid</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public string Uuid
	{
		get { return _uuid; }
		set
		{
			if (_uuid == value)
			{
				return;
			}

			_uuid = value;
			_isUuidWritten = true;
			RecordSet("uuid", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.SourceJSON</c>, then replays every property written before this
	/// object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isUuidWritten)
		{
			batch.Set(Handle, "uuid", ThreeValue.Encode(_uuid));
		}
	}
}
