namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Which face(s) of a mesh's triangles a material renders, matching three.js's <c>THREE.Side</c>
/// numeric constants exactly. <see cref="Core.ThreeValue.Encode"/> encodes this enum as its numeric
/// backing value rather than its member name, which is what three.js expects on the wire.
/// </summary>
public enum Side : byte
{
	/// <summary>Render only the front face of each triangle. Matches <c>THREE.FrontSide</c>.</summary>
	FrontSide = 0,

	/// <summary>Render only the back face of each triangle. Matches <c>THREE.BackSide</c>.</summary>
	BackSide = 1,

	/// <summary>Render both faces of each triangle. Matches <c>THREE.DoubleSide</c>.</summary>
	DoubleSide = 2
}
