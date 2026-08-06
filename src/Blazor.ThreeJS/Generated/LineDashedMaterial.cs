// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A material for rendering line primitives. Materials define the appearance of renderable 3D
/// objects. The JavaScript-side <c>THREE.LineDashedMaterial</c>.
/// </summary>
public sealed class LineDashedMaterial : LineBasicMaterial
{
	private float _scale = 1f;
	private float _dashSize = 3f;
	private float _gapSize = 1f;
	private float _dashOffset = 0f;
	private bool _isScaleWritten;
	private bool _isDashSizeWritten;
	private bool _isGapSizeWritten;
	private bool _isDashOffsetWritten;

	/// <summary>Initializes a new <see cref="LineDashedMaterial"/>.</summary>
	public LineDashedMaterial()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>LineDashedMaterial</c> under the handle the browser minted
	/// for it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal LineDashedMaterial(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LineDashedMaterial</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LineDashedMaterial"; }
	}

	/// <summary>
	/// The scale of the dashed part of a line. Writing it records a <c>scale</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public float Scale
	{
		get { return _scale; }
		set
		{
			if (_scale == value)
			{
				return;
			}

			_scale = value;
			_isScaleWritten = true;
			RecordSet("scale", value);
		}
	}

	/// <summary>
	/// The size of the dash. This is both the gap with the stroke. Writing it records a <c>dashSize</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public float DashSize
	{
		get { return _dashSize; }
		set
		{
			if (_dashSize == value)
			{
				return;
			}

			_dashSize = value;
			_isDashSizeWritten = true;
			RecordSet("dashSize", value);
		}
	}

	/// <summary>
	/// The size of the gap. Writing it records a <c>gapSize</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float GapSize
	{
		get { return _gapSize; }
		set
		{
			if (_gapSize == value)
			{
				return;
			}

			_gapSize = value;
			_isGapSizeWritten = true;
			RecordSet("gapSize", value);
		}
	}

	/// <summary>
	/// The dash offset. Writing it records a <c>dashOffset</c> property write once this object is
	/// attached; writing the value already held records nothing.
	/// </summary>
	public float DashOffset
	{
		get { return _dashOffset; }
		set
		{
			if (_dashOffset == value)
			{
				return;
			}

			_dashOffset = value;
			_isDashOffsetWritten = true;
			RecordSet("dashOffset", value);
		}
	}

	/// <summary>
	/// This flag can be used for type testing. Read-only in three.js, so it is read on demand rather
	/// than mirrored: records a get op, sends it behind every write already pending, and completes with
	/// the value <c>isLineDashedMaterial</c> held.
	/// </summary>
	/// <returns>The value <c>isLineDashedMaterial</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsLineDashedMaterialAsync()
	{
		return GetAsync<bool>("isLineDashedMaterial");
	}

	/// <summary>
	/// Emits the create op for <c>THREE.LineDashedMaterial</c>, then replays every property written
	/// before this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isScaleWritten)
		{
			batch.Set(Handle, "scale", ThreeValue.Encode(_scale));
		}

		if (_isDashSizeWritten)
		{
			batch.Set(Handle, "dashSize", ThreeValue.Encode(_dashSize));
		}

		if (_isGapSizeWritten)
		{
			batch.Set(Handle, "gapSize", ThreeValue.Encode(_gapSize));
		}

		if (_isDashOffsetWritten)
		{
			batch.Set(Handle, "dashOffset", ThreeValue.Encode(_dashOffset));
		}
	}
}
