// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

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
	private string _uuid = string.Empty;
	private bool _isUuidWritten;

	/// <summary>Creates a <see cref="Shape"/> from the points.</summary>
	public Shape()
	{
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Shape</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Shape"; }
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
	}
}
