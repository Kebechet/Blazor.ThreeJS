// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>MOUSE</c>. Encoded on the wire as the numeric value three.js
/// itself uses, not as the member name.
/// </summary>
public enum MOUSE : byte
{
	/// <summary>Matches <c>MOUSE.LEFT</c> in three.js.</summary>
	LEFT = 0,

	/// <summary>Matches <c>MOUSE.MIDDLE</c> in three.js.</summary>
	MIDDLE = 1,

	/// <summary>Matches <c>MOUSE.RIGHT</c> in three.js.</summary>
	RIGHT = 2,

	/// <summary>
	/// Matches <c>MOUSE.ROTATE</c> in three.js. An alternative spelling three.js gives the same value
	/// as <see cref="LEFT"/>.
	/// </summary>
	ROTATE = LEFT,

	/// <summary>
	/// Matches <c>MOUSE.DOLLY</c> in three.js. An alternative spelling three.js gives the same value as
	/// <see cref="MIDDLE"/>.
	/// </summary>
	DOLLY = MIDDLE,

	/// <summary>
	/// Matches <c>MOUSE.PAN</c> in three.js. An alternative spelling three.js gives the same value as
	/// <see cref="RIGHT"/>.
	/// </summary>
	PAN = RIGHT
}
