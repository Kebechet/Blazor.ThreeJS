using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Light applied equally to every point in the scene, the JavaScript-side <c>THREE.AmbientLight</c>.
/// </summary>
public sealed class AmbientLight : Object3D
{
	private readonly Color _color;
	private readonly float _intensity;

	/// <summary>
	/// Initializes a new ambient light.
	/// </summary>
	/// <param name="color">Light color. Defaults to white when <see langword="null"/>.</param>
	/// <param name="intensity">Light intensity.</param>
	public AmbientLight(Color? color = null, float intensity = 1f)
	{
		_color = color ?? Color.White;
		_intensity = intensity;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AmbientLight</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(AmbientLight); }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.AmbientLight</c>: color as a hex integer, and intensity.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_color.GetHex(), _intensity]; }
	}
}
