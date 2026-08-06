// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A class for generating plane geometries. The JavaScript-side <c>THREE.PlaneGeometry</c>.</summary>
/// <seealso href="https://threejs.org/docs/index.html#api/en/geometries/PlaneGeometry">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/geometries/PlaneGeometry.js">Source</seealso>
public sealed class PlaneGeometry : BufferGeometry
{
	private readonly float _width;
	private readonly float _height;
	private readonly int _widthSegments;
	private readonly int _heightSegments;

	/// <summary>Create a new instance of <see cref="PlaneGeometry"/>.</summary>
	/// <param name="width">Width along the X axis.</param>
	/// <param name="height">Height along the Y axis.</param>
	/// <param name="widthSegments">Number of segmented faces along the width of the sides.</param>
	/// <param name="heightSegments">Number of segmented faces along the height of the sides.</param>
	public PlaneGeometry(float width = 1f, float height = 1f, int widthSegments = 1, int heightSegments = 1)
	{
		_width = width;
		_height = height;
		_widthSegments = widthSegments;
		_heightSegments = heightSegments;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>PlaneGeometry</c> under the handle the browser minted for
	/// it. No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal PlaneGeometry(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_width = default!;
		_height = default!;
		_widthSegments = default!;
		_heightSegments = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PlaneGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PlaneGeometry"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.PlaneGeometry</c>: width, height, widthSegments,
	/// heightSegments.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_width, _height, _widthSegments, _heightSegments]; }
	}
}
