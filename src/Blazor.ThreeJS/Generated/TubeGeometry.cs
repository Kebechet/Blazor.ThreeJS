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
	private readonly ThreeObject? _path;
	private readonly int _tubularSegments;
	private readonly float _radius;
	private readonly int _radialSegments;
	private readonly bool _closed;
	private Vector3[] _tangents = [];
	private Vector3[] _normals = [];
	private Vector3[] _binormals = [];
	private bool _isTangentsWritten;
	private bool _isNormalsWritten;
	private bool _isBinormalsWritten;

	/// <summary>Create a new instance of <see cref="TubeGeometry"/>.</summary>
	/// <param name="path">
	/// A 3D path that inherits from the <c>Curve</c> base class. Default <c>new
	/// THREE.QuadraticBezierCurve3(new Vector3(-1, -1, 0 ), new Vector3(-1, 1, 0), new Vector3(1, 1,
	/// 0))</c>.
	/// </param>
	/// <param name="tubularSegments">The number of segments that make up the tube.</param>
	/// <param name="radius">The radius of the tube.</param>
	/// <param name="radialSegments">The number of segments that make up the cross-section.</param>
	/// <param name="closed">Is the tube open or closed.</param>
	public TubeGeometry(
		ThreeObject? path = null,
		int tubularSegments = 64,
		float radius = 1f,
		int radialSegments = 8,
		bool closed = false)
	{
		_path = path;
		_tubularSegments = tubularSegments;
		_radius = radius;
		_radialSegments = radialSegments;
		_closed = closed;
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
		_tubularSegments = default!;
		_radius = default!;
		_radialSegments = default!;
		_closed = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.TubeGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "TubeGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.TubeGeometry</c>: path, tubularSegments, radius,
	/// radialSegments, closed. An argument the caller left unspecified travels as the wire's
	/// not-supplied sentinel, or is trimmed when nothing supplied follows it, so three.js applies its
	/// own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_path),
				_tubularSegments,
				_radius,
				_radialSegments,
				_closed
			]);
		}
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
	/// An object with a property for each of the constructor parameters. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>parameters</c> held.
	/// </summary>
	/// <returns>The value <c>parameters</c> held, once the JavaScript side has answered.</returns>
	public Task<TubeGeometryParameters> ParametersAsync()
	{
		return GetAsync<TubeGeometryParameters>("parameters");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.TubeGeometry</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_path?.AttachTo(batch);

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
