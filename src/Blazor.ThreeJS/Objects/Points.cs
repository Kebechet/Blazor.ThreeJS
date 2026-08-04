using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A renderable object that draws each vertex of its geometry as a point, the JavaScript-side
/// <c>THREE.Points</c>. Proves the geometry/material dependency-before-owner ordering seen on
/// <see cref="Mesh"/> is not specific to that type.
/// </summary>
public sealed class Points : Object3D
{
	private readonly ThreeObject _geometry;
	private readonly ThreeObject _material;

	/// <summary>
	/// Initializes a new points object from a geometry and a points material.
	/// </summary>
	/// <param name="geometry">
	/// Vertices to render as points. <see cref="BoxGeometry"/> is a <c>BufferGeometry</c> in
	/// three.js, so it renders as points at the box's corners.
	/// </param>
	/// <param name="material">Appearance of the points.</param>
	public Points(BoxGeometry geometry, PointsMaterial material)
	{
		_geometry = geometry;
		_material = material;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Points</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(Points); }
	}

	/// <summary>
	/// Attaches the geometry and material first, then emits this points object's create op
	/// referencing both by handle, so the applier never constructs it before its dependencies exist.
	/// See <see cref="Mesh.EmitCreate"/> for why attaching rather than just emitting matters.
	/// </summary>
	/// <param name="batch">Batch to record the create ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_geometry.AttachTo(batch);
		_material.AttachTo(batch);
		batch.Create(Handle, ThreeTypeName, [ThreeValue.Encode(_geometry), ThreeValue.Encode(_material)]);
	}
}
