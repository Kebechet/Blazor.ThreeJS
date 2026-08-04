// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The values three.js accepts for <c>GPUColorWriteFlags</c>. Encoded on the wire as the numeric
/// value three.js itself uses, not as the member name.
/// </summary>
public enum GPUColorWriteFlags : byte
{
	/// <summary>Matches <c>GPUColorWriteFlags.None</c> in three.js.</summary>
	None = 0,

	/// <summary>Matches <c>GPUColorWriteFlags.Red</c> in three.js.</summary>
	Red = 1,

	/// <summary>Matches <c>GPUColorWriteFlags.Green</c> in three.js.</summary>
	Green = 2,

	/// <summary>Matches <c>GPUColorWriteFlags.Blue</c> in three.js.</summary>
	Blue = 4,

	/// <summary>Matches <c>GPUColorWriteFlags.Alpha</c> in three.js.</summary>
	Alpha = 8,

	/// <summary>Matches <c>GPUColorWriteFlags.All</c> in three.js.</summary>
	All = 15
}
