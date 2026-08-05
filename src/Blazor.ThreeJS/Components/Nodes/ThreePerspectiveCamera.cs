using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A camera that renders the way an eye sees: things further away look smaller. The first camera
/// component in the markup is the one the canvas renders through.
/// </summary>
public sealed class ThreePerspectiveCamera : ThreeObject3DNode<PerspectiveCamera>
{
	/// <summary>Vertical field of view, in degrees.</summary>
	[Parameter] public float Fov { get; set; } = 50f;

	/// <summary>
	/// Aspect ratio of the frustum, width over height. The renderer keeps this in step with the canvas
	/// as it resizes, so a value supplied here is only the starting one.
	/// </summary>
	[Parameter] public float Aspect { get; set; } = 1f;

	/// <summary>Near plane: nothing closer than this is drawn.</summary>
	[Parameter] public float Near { get; set; } = 0.1f;

	/// <summary>Far plane: nothing further than this is drawn.</summary>
	[Parameter] public float Far { get; set; } = 2000f;

	/// <summary>Zoom factor applied on top of the field of view.</summary>
	[Parameter] public float Zoom { get; set; } = 1f;

	private (float Fov, float Aspect, float Near, float Far, float Zoom)? _appliedFrustum;

	/// <summary>Builds the camera from the frustum parameters.</summary>
	/// <returns>The mirrored camera.</returns>
	protected override PerspectiveCamera CreateThreeObject()
	{
		return new PerspectiveCamera(Fov, Aspect, Near, Far);
	}

	/// <summary>
	/// Applies the transform and flags, then the frustum — and, when the frustum changed, the
	/// recalculation three.js needs to be told to do. Writing <c>fov</c> alone changes nothing on screen:
	/// three.js derives the projection matrix once and only rebuilds it when asked, so a component that
	/// mirrored the property without the call would leave the markup and the picture disagreeing.
	/// <para>
	/// The recalculation is guarded on the values actually changing rather than left to the mirror's own
	/// write-elision, because it is a method call: the batch never coalesces one, so an unguarded call
	/// would put an op in every single render.
	/// </para>
	/// </summary>
	/// <param name="target">The mirrored camera to write into.</param>
	protected override void ApplyParameters(PerspectiveCamera target)
	{
		base.ApplyParameters(target);

		var frustum = (Fov, Aspect, Near, Far, Zoom);
		if (_appliedFrustum == frustum)
		{
			return;
		}

		_appliedFrustum = frustum;
		target.Fov = Fov;
		target.Aspect = Aspect;
		target.Near = Near;
		target.Far = Far;
		target.Zoom = Zoom;
		target.UpdateProjectionMatrix();
	}
}
