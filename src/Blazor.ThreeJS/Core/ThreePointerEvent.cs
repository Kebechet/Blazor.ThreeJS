using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Where a pointer event met the object it was raised on. Handed to <see cref="Object3D.OnClick"/>
/// by the JavaScript applier, which computed it by casting a ray through the pointer.
/// <para>
/// Which object was hit is not on here: the event is raised on that object, so a handler already has
/// it. What it could not have worked out for itself is where on the object the ray landed, and that
/// is what these two carry.
/// </para>
/// </summary>
public sealed class ThreePointerEvent
{
	/// <summary>
	/// Point in world space where the ray met the object's geometry — not the object's origin, and not
	/// a screen coordinate.
	/// </summary>
	public required Vector3 Point { get; init; }

	/// <summary>Distance in world units from the camera to <see cref="Point"/>.</summary>
	public required float Distance { get; init; }
}
