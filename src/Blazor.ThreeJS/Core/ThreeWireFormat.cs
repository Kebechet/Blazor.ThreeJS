using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Fixed wire-format contract shared with <c>three-interop.js</c>. Every literal here appears on
/// both sides of the interop boundary, so changing one without changing the JavaScript applier
/// breaks the protocol silently. These are transport tokens, not code references: they must NOT
/// be derived from type names via <c>nameof</c>, because renaming a C# type would then change the
/// wire format invisibly.
/// </summary>
public static class ThreeWireFormat
{
	/// <summary>Tag written for an encoded <see cref="Vector3"/> value.</summary>
	public const string Vector3Tag = "Vector3";

	/// <summary>Tag written for an encoded <see cref="Euler"/> value.</summary>
	public const string EulerTag = "Euler";

	/// <summary>Tag written for an encoded <see cref="Quaternion"/> value.</summary>
	public const string QuaternionTag = "Quaternion";

	/// <summary>Tag written for an encoded <see cref="Color"/> value.</summary>
	public const string ColorTag = "Color";

	/// <summary>Tag written for an encoded <see cref="Matrix4"/> value.</summary>
	public const string Matrix4Tag = "Matrix4";
}
