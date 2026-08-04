namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// A camera that renders a scene using perspective projection, matching how the human eye sees.
/// </summary>
public sealed class PerspectiveCamera : Object3D
{
	private readonly float _fov;
	private readonly float _aspect;
	private readonly float _near;
	private readonly float _far;

	/// <summary>
	/// Initializes a new perspective camera.
	/// </summary>
	/// <param name="fov">Vertical field of view, in degrees.</param>
	/// <param name="aspect">Aspect ratio, typically the canvas width divided by its height.</param>
	/// <param name="near">Distance to the near clipping plane.</param>
	/// <param name="far">Distance to the far clipping plane.</param>
	public PerspectiveCamera(float fov = 50f, float aspect = 1f, float near = 0.1f, float far = 2000f)
	{
		_fov = fov;
		_aspect = aspect;
		_near = near;
		_far = far;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PerspectiveCamera</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return nameof(PerspectiveCamera); }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.PerspectiveCamera</c>: fov, aspect, near, far.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_fov, _aspect, _near, _far]; }
	}

	/// <summary>
	/// Orients this camera to face the given point in world space.
	/// </summary>
	/// <param name="x">X coordinate of the point to look at.</param>
	/// <param name="y">Y coordinate of the point to look at.</param>
	/// <param name="z">Z coordinate of the point to look at.</param>
	public void LookAt(float x, float y, float z)
	{
		RecordCall("lookAt", x, y, z);
	}
}
