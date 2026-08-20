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

	/// <summary>Tag written for an encoded <see cref="Vector2"/> value.</summary>
	public const string Vector2Tag = "Vector2";

	/// <summary>Tag written for an encoded <see cref="Vector4"/> value.</summary>
	public const string Vector4Tag = "Vector4";

	/// <summary>Tag written for an encoded <see cref="Matrix3"/> value.</summary>
	public const string Matrix3Tag = "Matrix3";

	/// <summary>Tag written for an encoded <see cref="Matrix2"/> value.</summary>
	public const string Matrix2Tag = "Matrix2";

	/// <summary>Tag written for an encoded <see cref="Box2"/> value.</summary>
	public const string Box2Tag = "Box2";

	/// <summary>Tag written for an encoded <see cref="Box3"/> value.</summary>
	public const string Box3Tag = "Box3";

	/// <summary>Tag written for an encoded <see cref="Sphere"/> value.</summary>
	public const string SphereTag = "Sphere";

	/// <summary>Tag written for an encoded <see cref="Plane"/> value.</summary>
	public const string PlaneTag = "Plane";

	/// <summary>Tag written for an encoded <see cref="Ray"/> value.</summary>
	public const string RayTag = "Ray";

	/// <summary>Tag written for an encoded <see cref="Line3"/> value.</summary>
	public const string Line3Tag = "Line3";

	/// <summary>Tag written for an encoded <see cref="Triangle"/> value.</summary>
	public const string TriangleTag = "Triangle";

	/// <summary>Tag written for an encoded <see cref="Spherical"/> value.</summary>
	public const string SphericalTag = "Spherical";

	/// <summary>Tag written for an encoded <see cref="Cylindrical"/> value.</summary>
	public const string CylindricalTag = "Cylindrical";

	/// <summary>Tag written for an encoded <see cref="Frustum"/> value.</summary>
	public const string FrustumTag = "Frustum";

	/// <summary>Tag written for an encoded <see cref="SphericalHarmonics3"/> value.</summary>
	public const string SphericalHarmonics3Tag = "SphericalHarmonics3";

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

	/// <summary>
	/// Handle every context registers its own <c>WebGPURenderer</c> under.
	/// <para>
	/// Reserved rather than minted, so C# can address the renderer without a round trip to ask what
	/// handle it got. The applier seeds its own allocator below this value, so no loaded glTF node or
	/// other browser-made object can ever be given it. It is negative because the browser made the
	/// renderer — <c>createContext</c> builds it before C# exists — and negative handles are that
	/// side's half of the space.
	/// </para>
	/// </summary>
	public const int RendererHandle = -1;

	/// <summary>
	/// Key naming which JavaScript typed array a component list should be rebuilt as, e.g.
	/// <c>{"$ta":"Float32Array","v":[…]}</c>. A plain JSON array cannot carry this: three.js hands a
	/// <c>BufferAttribute</c>'s array straight to WebGL, which needs the real typed array rather than
	/// an <c>Array</c> of numbers.
	/// </summary>
	public const string TypedArrayKey = "$ta";

	/// <summary>
	/// Key of a lone non-finite number, <c>{"$n":"Infinity"}</c>. A tagged math value carries its
	/// components as strings under <see cref="ValuesKey"/>; a scalar has no such array to hide in, and
	/// sending the bare string would be indistinguishable from a genuine string value —
	/// <c>Set("name", "Infinity")</c> would become a number. <c>AnimationAction.repetitions</c> is
	/// three.js's own documented use of infinity, so this is ordinary rather than exotic.
	/// </summary>
	public const string NonFiniteKey = "$n";

	/// <summary>
	/// Key of a plain data object, <c>{"$o":{"start":0,"count":3}}</c>. three.js describes some of what
	/// it hands back with an interface rather than a class - a geometry group, a batched-mesh range -
	/// and those are values with no identity, so a handle would be the wrong shape for them.
	/// <para>
	/// Tagged rather than sent bare, because the applier refuses to serialize a three.js <em>instance</em>
	/// and that refusal is load-bearing: a <c>Mesh</c> flattened into JSON would reach C# as a plausible
	/// bag of numbers. The tag is what says "this really is a plain object", and the applier only writes
	/// it for a value whose prototype is <c>Object.prototype</c> or none.
	/// </para>
	/// </summary>
	public const string StructureKey = "$o";

	/// <summary>
	/// Wire token for <see cref="float.PositiveInfinity"/> inside a tagged value's component array.
	/// <para>
	/// JSON has no numeric form for a non-finite value, and the two runtimes fail differently rather
	/// than loudly: <c>Utf8JsonWriter</c> throws <c>ArgumentException</c> on the way out, while
	/// JavaScript's <c>JSON.stringify(Infinity)</c> silently yields <c>null</c> on the way back. A
	/// default <see cref="Box3"/> is exactly this case - three.js seeds an empty box at ±infinity - so
	/// a component is carried as a string whenever it is not finite, and turned back into a number on
	/// both sides. The spelling matches JavaScript's own <c>String(Infinity)</c>, so the applier
	/// converts with a plain <c>Number(...)</c>.
	/// </para>
	/// </summary>
	public const string PositiveInfinityToken = "Infinity";

	/// <summary>Wire token for <see cref="float.NegativeInfinity"/>. See <see cref="PositiveInfinityToken"/>.</summary>
	public const string NegativeInfinityToken = "-Infinity";

	/// <summary>Wire token for <see cref="float.NaN"/>. See <see cref="PositiveInfinityToken"/>.</summary>
	public const string NotANumberToken = "NaN";
}
