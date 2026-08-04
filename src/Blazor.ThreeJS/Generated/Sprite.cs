// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A <see cref="Sprite"/> is a plane that always faces towards the camera, generally with a
/// partially transparent texture applied. The JavaScript-side <c>THREE.Sprite</c>.
/// </summary>
/// <remarks>Sprites do not cast shadows, setting <c>castShadow = true</c> will have no effect.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/Sprite">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/Sprite.js">Source</seealso>
public sealed class Sprite : Object3D
{
	private SpriteMaterial? _material;
	private BufferGeometry? _geometry;
	private bool _isGeometryWritten;
	private bool _isMaterialWritten;

	/// <summary>Creates a new Sprite.</summary>
	/// <param name="material">
	/// An instance of <c>SpriteMaterial</c>. Default <c><c>new SpriteMaterial()</c></c>, _with white
	/// color_.
	/// </param>
	public Sprite(SpriteMaterial? material = null)
	{
		_material = material;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Sprite</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Sprite"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.Sprite</c>: material. An argument the caller left
	/// unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing supplied
	/// follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return ThreeValue.TrimUnspecifiedTail([ThreeValue.OrUnspecified(_material)]); }
	}

	/// <summary>
	/// The <c>geometry</c> property of the JavaScript-side object. Writing it records a <c>geometry</c>
	/// property write once this object is attached; writing the value already held records nothing.
	/// </summary>
	public BufferGeometry? Geometry
	{
		get { return _geometry; }
		set
		{
			if (ReferenceEquals(_geometry, value))
			{
				return;
			}

			_geometry = value;
			_isGeometryWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("geometry", value);
		}
	}

	/// <summary>
	/// An instance of <c>SpriteMaterial</c>, defining the object's appearance. Writing it records a
	/// <c>material</c> property write once this object is attached; writing the value already held
	/// records nothing.
	/// </summary>
	public SpriteMaterial? Material
	{
		get { return _material; }
		set
		{
			if (ReferenceEquals(_material, value))
			{
				return;
			}

			_material = value;
			_isMaterialWritten = true;
			if (Batch is not null && value is not null)
			{
				value.AttachTo(Batch);
			}

			RecordSet("material", value);
		}
	}

	/// <summary>
	/// Attaches the objects <c>THREE.Sprite</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_material?.AttachTo(batch);

		base.EmitCreate(batch);
	}

	/// <summary>
	/// Replays every property written before this object was attached, so construction order never
	/// matters to the caller. A property the caller never wrote is left alone: three.js's own default
	/// is the truth for it, and the mirror has never read anything back to improve on that.
	/// </summary>
	/// <param name="batch">Batch to record the property writes into.</param>
	internal override void EmitState(ThreeBatch batch)
	{
		base.EmitState(batch);

		if (_isGeometryWritten)
		{
			batch.Set(Handle, "geometry", ThreeValue.Encode(_geometry));
		}

		if (_isMaterialWritten)
		{
			batch.Set(Handle, "material", ThreeValue.Encode(_material));
		}
	}
}
