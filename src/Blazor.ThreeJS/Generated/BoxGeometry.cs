// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// <see cref="BoxGeometry"/> is a geometry class for a rectangular cuboid with a given 'width',
/// 'height', and 'depth'. The JavaScript-side <c>THREE.BoxGeometry</c>.
/// </summary>
/// <remarks>On creation, the cuboid is centred on the origin, with each edge parallel to one of the axes.</remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/BoxGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/BoxGeometry.js">Source</seealso>
public sealed class BoxGeometry : BufferGeometry
{
	private readonly float _width;
	private readonly float _height;
	private readonly float _depth;
	private readonly int _widthSegments;
	private readonly int _heightSegments;
	private readonly int _depthSegments;

	/// <summary>Create a new instance of <see cref="BoxGeometry"/>.</summary>
	/// <param name="width">Width; that is, the length of the edges parallel to the X axis.</param>
	/// <param name="height">Height; that is, the length of the edges parallel to the Y axis.</param>
	/// <param name="depth">Depth; that is, the length of the edges parallel to the Z axis.</param>
	/// <param name="widthSegments">Number of segmented rectangular faces along the width of the sides.</param>
	/// <param name="heightSegments">Number of segmented rectangular faces along the height of the sides.</param>
	/// <param name="depthSegments">Number of segmented rectangular faces along the depth of the sides.</param>
	public BoxGeometry(
		float width = 1f,
		float height = 1f,
		float depth = 1f,
		int widthSegments = 1,
		int heightSegments = 1,
		int depthSegments = 1)
	{
		_width = width;
		_height = height;
		_depth = depth;
		_widthSegments = widthSegments;
		_heightSegments = heightSegments;
		_depthSegments = depthSegments;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>BoxGeometry</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal BoxGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_width = default!;
		_height = default!;
		_depth = default!;
		_widthSegments = default!;
		_heightSegments = default!;
		_depthSegments = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BoxGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "BoxGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.BoxGeometry</c>: width, height, depth,
	/// widthSegments, heightSegments, depthSegments.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_width, _height, _depth, _widthSegments, _heightSegments, _depthSegments]; }
	}

	/// <summary>
	/// An object with a property for each of the constructor parameters. Read-only in three.js, so it
	/// is read on demand rather than mirrored: records a get op, sends it behind every write already
	/// pending, and completes with the value <c>parameters</c> held.
	/// </summary>
	/// <returns>The value <c>parameters</c> held, once the JavaScript side has answered.</returns>
	public Task<BoxGeometryParameters> ParametersAsync()
	{
		return GetAsync<BoxGeometryParameters>("parameters");
	}
}
