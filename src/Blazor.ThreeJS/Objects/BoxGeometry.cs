using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Rectangular cuboid geometry, the JavaScript-side <c>THREE.BoxGeometry</c>.
/// </summary>
public sealed class BoxGeometry : ThreeObject
{
	private readonly float _width;
	private readonly float _height;
	private readonly float _depth;

	/// <summary>
	/// Initializes a new box geometry.
	/// </summary>
	/// <param name="width">Size along the X axis.</param>
	/// <param name="height">Size along the Y axis.</param>
	/// <param name="depth">Size along the Z axis.</param>
	public BoxGeometry(float width = 1f, float height = 1f, float depth = 1f)
	{
		_width = width;
		_height = height;
		_depth = depth;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.BoxGeometry</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(BoxGeometry); }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.BoxGeometry</c>: width, height, depth.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_width, _height, _depth]; }
	}
}
