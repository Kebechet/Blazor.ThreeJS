using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Fixed wire-format contract shared with <c>three-interop.js</c>. Every literal here appears on
/// both sides of the interop boundary, so changing one without changing the JavaScript applier
/// breaks the protocol silently. These are transport tokens, not code references: they must NOT
/// be derived from type names via <c>nameof</c>, because renaming a C# type would then change the
/// wire format invisibly.
/// </summary>
internal static class ThreeWireFormat
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

	/// <summary>
	/// Key carrying which of the tags above a math value was encoded under. Read by both sides now:
	/// the applier switches on it to build a three.js instance, and the C# decoder switches on it to
	/// rebuild a math value read back from a query.
	/// </summary>
	public const string TagKey = "$t";

	/// <summary>Key carrying a tagged math value's raw components, e.g. [x, y, z] for a vector.</summary>
	public const string ValuesKey = "v";

	/// <summary>Key carrying an <see cref="Euler"/>'s rotation order, omitted under every other tag.</summary>
	public const string OrderKey = "o";

	/// <summary>Key of a reference to another mirrored object, resolved through the applier's handle table.</summary>
	public const string HandleReferenceKey = "$ref";

	/// <summary>
	/// Key of the "this argument was not supplied" sentinel, <c>{"$undef":true}</c>, which the applier
	/// decodes to JavaScript's <c>undefined</c>. JSON <c>null</c> cannot say this: a JavaScript default
	/// only applies to <c>undefined</c>, so <c>function f(a = 1) {}</c> called as <c>f(null)</c> yields
	/// <c>null</c>, not <c>1</c>. This is the only value that lets three.js apply its own default to an
	/// argument that has a supplied argument after it.
	/// </summary>
	public const string UndefinedKey = "$undef";
}
