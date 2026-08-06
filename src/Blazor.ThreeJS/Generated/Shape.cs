// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Defines an arbitrary 2d <see cref="Shape"/> plane using paths with optional holes. The
/// JavaScript-side <c>THREE.Shape</c>.
/// </summary>
/// <remarks>
/// It can be used with <c>ExtrudeGeometry</c>, <c>ShapeGeometry</c>, to get points, or to get
/// triangulated faces.
/// </remarks>
/// <seealso href="https://threejs.org/examples/#webgl_geometry_shapes">geometry / shapes</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_geometry_extrude_shapes">geometry / extrude / shapes</seealso>
/// <seealso href="https://threejs.org/examples/#webgl_geometry_extrude_shapes2">geometry / extrude / shapes2</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/core/Shape">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/core/Shape.js">Source</seealso>
public sealed class Shape : Path
{
	private readonly Vector2[]? _points;
	private string _uuid = string.Empty;
	private Path?[] _holes = [];
	private bool _isUuidWritten;
	private bool _isHolesWritten;

	/// <summary>Creates a <see cref="Shape"/> from the points.</summary>
	/// <param name="points">Array of <see cref="Vector2">Vector2s</see>.</param>
	public Shape(Vector2[]? points = null)
	{
		_points = points;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Shape</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Shape(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Shape</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Shape"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Shape</c>: points. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_points)]); }
	}

	/// <summary>
	/// <see href="http://en.wikipedia.org/wiki/Universally_unique_identifier">UUID</see> of this object
	/// instance. Writing it records a <c>uuid</c> property write once this object is attached; writing
	/// the value already held records nothing.
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
	/// An array of <see cref="Path">paths</see> that define the holes in the shape. Writing it records
	/// a <c>holes</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public Path?[] Holes
	{
		get { return _holes; }
		set
		{
			if (_holes == value)
			{
				return;
			}

			_holes = value;
			_isHolesWritten = true;
			RecordSet("holes", value);
		}
	}

	/// <summary>
	/// Get an array of <see cref="Vector2">Vector2's</see> that represent the holes in the shape.
	/// Records a read op, sends it behind every write already pending, and completes with what
	/// <c>getPointsHoles</c> returned.
	/// </summary>
	/// <param name="divisions">The fineness of the result.</param>
	/// <returns>The value <c>getPointsHoles</c> returned, once the JavaScript side has answered.</returns>
	public Task<Vector2[][]> GetPointsHolesAsync(int divisions)
	{
		return RecordRead<Vector2[][]>("getPointsHoles", divisions);
	}

	/// <summary>
	/// Emits the create op for <c>THREE.Shape</c>, then replays every property written before this
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

		if (_isHolesWritten)
		{
			batch.Set(Handle, "holes", ThreeValue.Encode(_holes));
		}
	}
}
