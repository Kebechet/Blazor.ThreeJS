using Kebechet.Blazor.ThreeJS.Objects;
using Microsoft.AspNetCore.Components;

namespace Kebechet.Blazor.ThreeJS.Components;

/// <summary>
/// A camera with no perspective: an object keeps its size however far away it is. The first camera
/// component in the markup is the one the canvas renders through.
/// </summary>
public sealed class ThreeOrthographicCamera : ThreeObject3DNode<OrthographicCamera>
{
	/// <summary>Left plane of the camera's frustum.</summary>
	[Parameter] public float Left { get; set; } = -1f;

	/// <summary>Right plane of the camera's frustum.</summary>
	[Parameter] public float Right { get; set; } = 1f;

	/// <summary>Top plane of the camera's frustum.</summary>
	[Parameter] public float Top { get; set; } = 1f;

	/// <summary>Bottom plane of the camera's frustum.</summary>
	[Parameter] public float Bottom { get; set; } = -1f;

	/// <summary>Near plane: nothing closer than this is drawn.</summary>
	[Parameter] public float Near { get; set; } = 0.1f;

	/// <summary>Far plane: nothing further than this is drawn.</summary>
	[Parameter] public float Far { get; set; } = 2000f;

	/// <summary>Zoom factor applied on top of the frustum.</summary>
	[Parameter] public float Zoom { get; set; } = 1f;

	private (float Left, float Right, float Top, float Bottom, float Near, float Far, float Zoom)? _appliedFrustum;

	/// <summary>Builds the camera from the frustum parameters.</summary>
	/// <returns>The mirrored camera.</returns>
	protected override OrthographicCamera CreateThreeObject()
	{
		return new OrthographicCamera(Left, Right, Top, Bottom, Near, Far);
	}

	/// <summary>
	/// Applies the transform and flags, then the frustum and — when it changed — the recalculation
	/// three.js has to be told to do. See <see cref="ThreePerspectiveCamera"/> for why the call is
	/// guarded rather than left to the mirror.
	/// </summary>
	/// <param name="target">The mirrored camera to write into.</param>
	protected override void ApplyParameters(OrthographicCamera target)
	{
		base.ApplyParameters(target);

		var frustum = (Left, Right, Top, Bottom, Near, Far, Zoom);
		if (_appliedFrustum == frustum)
		{
			return;
		}

		_appliedFrustum = frustum;
		target.Left = Left;
		target.Right = Right;
		target.Top = Top;
		target.Bottom = Bottom;
		target.Near = Near;
		target.Far = Far;
		target.Zoom = Zoom;
		target.UpdateProjectionMatrix();
	}
}
