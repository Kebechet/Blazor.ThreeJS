namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Thrown when the IR describes something the emitter cannot mirror exactly. Refusing is deliberate:
/// a guessed constructor argument reaches the browser as a wrong number with nothing on the C# side
/// to signal it, so the emitter stops and names the member instead of producing plausible output.
/// </summary>
internal sealed class UnsupportedMemberException : Exception
{
	/// <summary>Three.js class the refused member belongs to.</summary>
	public required string ClassName { get; init; }

	/// <summary>The member, and why the emitter would have had to guess.</summary>
	public required string Reason { get; init; }

	/// <summary>Creates the exception.</summary>
	/// <param name="message">Full message, including class and reason.</param>
	public UnsupportedMemberException(string message)
		: base(message)
	{
	}

	/// <summary>Builds a refusal for a named class and reason.</summary>
	/// <param name="className">Three.js class being emitted.</param>
	/// <param name="reason">What the emitter cannot model.</param>
	/// <returns>The exception to throw.</returns>
	public static UnsupportedMemberException For(string className, string reason)
	{
		return new UnsupportedMemberException($"{className}: {reason}")
		{
			ClassName = className,
			Reason = reason
		};
	}
}
