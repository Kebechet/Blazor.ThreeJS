using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Light that shines uniformly along a single direction from infinitely far away, approximating a
/// sun. The JavaScript-side <c>THREE.DirectionalLight</c>.
/// </summary>
public sealed class DirectionalLight : Object3D
{
	private readonly Color _color;
	private readonly float _intensity;

	/// <summary>
	/// Initializes a new directional light.
	/// </summary>
	/// <param name="color">Light color. Defaults to white when <see langword="null"/>.</param>
	/// <param name="intensity">Light intensity.</param>
	public DirectionalLight(Color? color = null, float intensity = 1f)
	{
		_color = color ?? Color.White;
		_intensity = intensity;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.DirectionalLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(DirectionalLight); }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.DirectionalLight</c>: color as a hex integer, and intensity.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_color.GetHex(), _intensity]; }
	}
}
