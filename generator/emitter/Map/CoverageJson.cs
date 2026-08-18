namespace Blazor.ThreeJS.Emitter.Map;

/// <summary>
/// The machine-readable half of the coverage output. This is a **fixed format contract** consumed by
/// whatever generates the README's coverage table, so property names here are wire tokens: renaming
/// one changes the artifact, not just the code.
/// </summary>
internal sealed class CoverageJson
{
	/// <summary>Package the API surface was read from.</summary>
	public required string TypesPackage { get; init; }

	/// <summary>Exact pinned version of that package.</summary>
	public required string TypesVersion { get; init; }

	/// <summary>Headline counts, which is all the coverage table itself needs.</summary>
	public required CoverageTotalsJson Totals { get; init; }

	/// <summary>Per-class verdicts.</summary>
	public required List<CoverageClassJson> Classes { get; init; }

	/// <summary>Member skip counts grouped by obstacle.</summary>
	public required List<CoverageSkipCategoryJson> MemberSkipReasons { get; init; }

	/// <summary>Every enum the generator emits, referenced by a mapped member or not.</summary>
	public required List<CoverageEnumJson> GeneratedEnums { get; init; }

	/// <summary>Value sets that look like enums but cannot be generated, with the reason.</summary>
	public required List<CoverageRefusedEnumJson> RefusedEnums { get; init; }

	/// <summary>Per-member classification.</summary>
	public required List<CoverageMemberJson> Members { get; init; }
}

/// <summary>Headline counts.</summary>
internal sealed class CoverageTotalsJson
{
	/// <summary>Classes in the IR.</summary>
	public required int Classes { get; init; }

	/// <summary>Classes the emitter can produce today.</summary>
	public required int EmittableClasses { get; init; }

	/// <summary>Classes deliberately outside the mirrored surface.</summary>
	public required int OutOfSurfaceClasses { get; init; }

	/// <summary>Classes in the surface that are blocked on something.</summary>
	public required int BlockedClasses { get; init; }

	/// <summary>
	/// Classes the shipped bundle exports, and which are therefore constructible by name through the
	/// untyped escape hatch whether or not they are generated. A superset of
	/// <see cref="EmittableClasses"/>.
	/// </summary>
	public required int ReachableClasses { get; init; }

	/// <summary>Declared members across every class, including augmented ones.</summary>
	public required int Members { get; init; }

	/// <summary>Members classified as mirrored state.</summary>
	public required int MirroredState { get; init; }

	/// <summary>Members classified as commands.</summary>
	public required int Commands { get; init; }

	/// <summary>Members classified as async queries.</summary>
	public required int AsyncQueries { get; init; }

	/// <summary>
	/// The subset of <see cref="AsyncQueries"/> whose answer is a three.js object no generated class
	/// mirrors, so it comes back as an untyped <c>Primitive</c> under its own handle. Counted on its own
	/// because it is the one query shape the typed surface cannot describe: it moves as the generator
	/// covers more classes, and a covered class turns its callers typed without changing this total's
	/// meaning.
	/// </summary>
	public required int UntypedObjectQueries { get; init; }

	/// <summary>Members skipped, with a reason recorded against each.</summary>
	public required int SkippedMembers { get; init; }

	/// <summary>
	/// Members belonging to a class the generator emits. The denominator that can actually move:
	/// covering more of the API means covering more of this, not of <see cref="Members"/>.
	/// <para>
	/// ⚠️ <see cref="MirroredState"/>, <see cref="Commands"/> and <see cref="AsyncQueries"/> are counted
	/// over <b>every</b> class, including blocked ones whose members are classified but never emitted.
	/// Summing those three overstates what ships; <see cref="GeneratedMembers"/> is the honest numerator
	/// and this is its denominator.
	/// </para>
	/// </summary>
	public required int ReachableMembers { get; init; }

	/// <summary>
	/// Members that actually reach generated C#: classified into a bucket <b>and</b> sitting on a class
	/// the generator emits.
	/// </summary>
	public required int GeneratedMembers { get; init; }

	/// <summary>
	/// Members belonging to a class that is not emitted, which no mapping rule can expose because there
	/// is no C# type to put them on. For the largest blocking reason, <c>NotExported</c>, there is no
	/// runtime value either — those classes are absent from the shipped three.js bundle, so even the
	/// escape hatch's <c>THREE[name]</c> lookup would fail.
	/// </summary>
	public required int StrandedMembers { get; init; }
}

/// <summary>One class's verdict.</summary>
internal sealed class CoverageClassJson
{
	/// <summary>Three.js class name.</summary>
	public required string Name { get; init; }

	/// <summary>Declaring file, relative to the types package root.</summary>
	public required string File { get; init; }

	/// <summary><c>emittable</c>, <c>outOfSurface</c> or <c>blocked</c>.</summary>
	public required string Status { get; init; }

	/// <summary>
	/// Whether the shipped three.js bundle puts this name on <c>THREE</c>, which is exactly what the
	/// applier resolves a create op against — so it is also whether an untyped <c>Primitive</c> can
	/// construct it. Independent of <see cref="Status"/>: a class can be unreachable by the generator
	/// and still reachable by name, and a class the bundle does not export is reachable by nothing.
	/// </summary>
	public required bool IsReachable { get; init; }

	/// <summary>Why, when the status is not <c>emittable</c>.</summary>
	public string? Reason { get; init; }

	/// <summary>Obstacle family the reason belongs to.</summary>
	public string? Category { get; init; }

	/// <summary>Parameters that reached the C# constructor.</summary>
	public required int ConstructorParameterCount { get; init; }

	/// <summary>Parameters left out of the C# constructor, each with a reason.</summary>
	public required List<CoverageDroppedParameterJson> DroppedConstructorParameters { get; init; }

	/// <summary>Unspecified nullables that trailing-null trimming cannot protect.</summary>
	public required List<string> MiddlePositionUnspecifiedParameters { get; init; }
}

/// <summary>A constructor parameter the mirror does not expose.</summary>
internal sealed class CoverageDroppedParameterJson
{
	/// <summary>Three.js parameter name.</summary>
	public required string Name { get; init; }

	/// <summary>Declared type, verbatim.</summary>
	public required string Type { get; init; }

	/// <summary>Why it is not exposed.</summary>
	public required string Reason { get; init; }

	/// <summary>Obstacle family.</summary>
	public required string Category { get; init; }
}

/// <summary>Skip count for one obstacle family.</summary>
internal sealed class CoverageSkipCategoryJson
{
	/// <summary>Obstacle family.</summary>
	public required string Category { get; init; }

	/// <summary>Members skipped for it.</summary>
	public required int Members { get; init; }
}

/// <summary>One generated enum.</summary>
internal sealed class CoverageEnumJson
{
	/// <summary>Enum name, matching three.js.</summary>
	public required string Name { get; init; }

	/// <summary><c>ConstantGroup</c> when inferred from loose constants, <c>DeclaredEnum</c> when three.js declares one.</summary>
	public required string Source { get; init; }

	/// <summary>Member count, aliases included.</summary>
	public required int Members { get; init; }

	/// <summary>C# backing type.</summary>
	public required string BackingType { get; init; }

	/// <summary>Whether a mapped member's type resolves to it today.</summary>
	public required bool IsReferenced { get; init; }
}

/// <summary>One value set that cannot become a C# enum.</summary>
internal sealed class CoverageRefusedEnumJson
{
	/// <summary>Alias or enum name.</summary>
	public required string Name { get; init; }

	/// <summary>Why it was refused.</summary>
	public required string Reason { get; init; }
}

/// <summary>One member's classification.</summary>
internal sealed class CoverageMemberJson
{
	/// <summary>Declaring class.</summary>
	public required string Class { get; init; }

	/// <summary>Member name.</summary>
	public required string Member { get; init; }

	/// <summary><c>Property</c> or <c>Method</c>.</summary>
	public required string Kind { get; init; }

	/// <summary><c>Declared</c> or <c>ModuleAugmentation</c>.</summary>
	public required string Origin { get; init; }

	/// <summary>Which of the four buckets it fell into.</summary>
	public required string Bucket { get; init; }

	/// <summary>Resolved C# type, when it has one.</summary>
	public string? Type { get; init; }

	/// <summary>Why it was skipped, when it was.</summary>
	public string? Reason { get; init; }

	/// <summary>Obstacle family.</summary>
	public string? Category { get; init; }
}
