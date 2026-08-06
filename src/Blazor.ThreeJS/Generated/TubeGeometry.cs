// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>Creates a tube that extrudes along a 3d curve. The JavaScript-side <c>THREE.TubeGeometry</c>.</summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/TubeGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/TubeGeometry.js">Source</seealso>
public sealed class TubeGeometry : BufferGeometry
{
	private Vector3[] _tangents = [];
	private Vector3[] _normals = [];
	private Vector3[] _binormals = [];
	private bool _isTangentsWritten;
	private bool _isNormalsWritten;
	private bool _isBinormalsWritten;

	/// <summary>Create a new instance of <see cref="TubeGeometry"/>.</summary>
	public TubeGeometry()
	{
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>TubeGeometry</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal TubeGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.TubeGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "TubeGeometry"; }
	}

	/// <summary>
	/// An array of <c>Vector3</c> tangents. Writing it records a <c>tangents</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public Vector3[] Tangents
	{
		get { return _tangents; }
		set
		{
			if (_tangents == value)
			{
				return;
			}

			_tangents = value;
			_isTangentsWritten = true;
			RecordSet("tangents", value);
		}
	}

	/// <summary>
	/// An array of <c>Vector3</c> normals. Writing it records a <c>normals</c> property write once this
	/// object is attached; writing the value already held records nothing.
	/// </summary>
	public Vector3[] Normals
	{
		get { return _normals; }
		set
		{
			if (_normals == value)
			{
				return;
			}

			_normals = value;
			_isNormalsWritten = true;
			RecordSet("normals", value);
		}
	}

	/// <summary>
	/// An array of <c>Vector3</c> binormals. Writing it records a <c>binormals</c> property write once
	/// this object is attached; writing the value already held records nothing.
	/// </summary>
	public Vector3[] Binormals
	{
		get { return _binormals; }
		set
		{
			if (_binormals == value)
			{
				return;
			}

			_binormals = value;
			_isBinormalsWritten = true;
			RecordSet("binormals", value);
		}
	}

	/// <summary>
	/// Emits the create op for <c>THREE.TubeGeometry</c>, then replays every property written before
	/// this object was attached.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		base.EmitCreate(batch);

		if (_isTangentsWritten)
		{
			batch.Set(Handle, "tangents", ThreeValue.Encode(_tangents));
		}

		if (_isNormalsWritten)
		{
			batch.Set(Handle, "normals", ThreeValue.Encode(_normals));
		}

		if (_isBinormalsWritten)
		{
			batch.Set(Handle, "binormals", ThreeValue.Encode(_binormals));
		}
	}
}
