#if !NET7_0_OR_GREATER

namespace System.Runtime.CompilerServices;

/// <summary>
/// Compile-time marker the C# compiler requires to emit <c>required</c> members. Present in the
/// BCL from .NET 7; defined here so the net6.0 target can use <c>required</c> too. Purely a
/// marker — it has no runtime behaviour.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed class RequiredMemberAttribute : Attribute
{
}

/// <summary>
/// Compile-time marker the C# compiler emits alongside <c>required</c> members so older compilers
/// refuse to consume them unsafely. Present in the BCL from .NET 7; defined here for net6.0.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute : Attribute
{
	public CompilerFeatureRequiredAttribute(string featureName)
	{
		FeatureName = featureName;
	}

	public string FeatureName { get; }

	public bool IsOptional { get; init; }
}

#endif
