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
public sealed class BoxGeometry : ThreeObject
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
}
