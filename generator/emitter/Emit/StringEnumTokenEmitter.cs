using Blazor.ThreeJS.Emitter.Ir;
using Blazor.ThreeJS.Emitter.Map;

namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Emits the lookup that turns a string-valued enum into the token three.js compares against, and
/// back again.
/// <para>
/// It exists as generated code rather than as reflection over an attribute because this package
/// targets Blazor WebAssembly, where the linker is free to trim metadata nothing statically
/// references — a reflective lookup would compile, pass every desktop test, and return null in a
/// trimmed browser build. A generated switch is a static reference to every member it names.
/// </para>
/// </summary>
internal sealed class StringEnumTokenEmitter
{
	/// <summary>Name of the generated class, which <c>ThreeValue</c> calls into.</summary>
	public const string ClassName = "ThreeStringEnum";

	private readonly IrRoot _ir;

	/// <summary>Builds an emitter over one IR snapshot, for the provenance header.</summary>
	/// <param name="ir">The parsed IR.</param>
	public StringEnumTokenEmitter(IrRoot ir)
	{
		_ir = ir;
	}

	/// <summary>Emits the lookup covering every string-valued enum the catalog generated.</summary>
	/// <param name="stringValuedEnums">The string-valued enums, in the order they are declared.</param>
	/// <returns>The generated file.</returns>
	public EmittedFile Emit(IReadOnlyList<GeneratedEnum> stringValuedEnums)
	{
		var writer = new CSharpWriter();
		writer.WriteLine($"// Generated from {_ir.Meta?.TypesPackage ?? "@types/three"}@{_ir.Meta?.TypesVersion ?? "unknown"} by generator/emitter. Do not edit by hand.");
		writer.WriteLine("// Re-run `npm run emit` after changing the emitter or generator/three-api.json.");
		writer.WriteLine();
		writer.WriteLine($"namespace {EmitterConfig.GeneratedNamespace};");
		writer.WriteLine();

		DocCommentEmitter.WriteSummary(
			writer,
			"Maps the enums three.js spells as strings between their C# values and the tokens the " +
			"browser compares against. Generated rather than reflective so a trimmed WebAssembly build " +
			"cannot lose the mapping.");

		writer.WriteLine($"internal static class {ClassName}");
		writer.WriteLine("{");
		writer.Indent();

		WriteTokenFor(writer, stringValuedEnums);
		writer.WriteLine();
		WriteFromToken(writer, stringValuedEnums);

		foreach (var stringValuedEnum in stringValuedEnums)
		{
			writer.WriteLine();
			WriteTokenForOne(writer, stringValuedEnum);
			writer.WriteLine();
			WriteFromTokenForOne(writer, stringValuedEnum);
		}

		writer.Outdent();
		writer.WriteLine("}");

		return new EmittedFile
		{
			RelativePath = $"src/Blazor.ThreeJS/Generated/{ClassName}.cs",
			Contents = writer.ToSource()
		};
	}

	private static void WriteTokenFor(CSharpWriter writer, IReadOnlyList<GeneratedEnum> stringValuedEnums)
	{
		DocCommentEmitter.WriteSummary(
			writer,
			"The token a value crosses the wire as, or <see langword=\"null\"/> when its enum is one of " +
			"the numeric ones and the number is already what three.js wants.");
		writer.WriteLine("/// <param name=\"value\">Any enum value.</param>");
		writer.WriteLine("/// <returns>The token, or <see langword=\"null\"/> for a numeric enum.</returns>");
		writer.WriteLine("public static string? TokenFor(Enum value)");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine("return value switch");
		writer.WriteLine("{");
		writer.Indent();

		foreach (var stringValuedEnum in stringValuedEnums)
		{
			var parameterName = ConstructorMapper.ToCamelCase(stringValuedEnum.Name);
			writer.WriteLine($"{stringValuedEnum.Name} {parameterName} => TokenFor({parameterName}),");
		}

		writer.WriteLine("_ => null");
		writer.Outdent();
		writer.WriteLine("};");
		writer.Outdent();
		writer.WriteLine("}");
	}

	private static void WriteFromToken(CSharpWriter writer, IReadOnlyList<GeneratedEnum> stringValuedEnums)
	{
		DocCommentEmitter.WriteSummary(
			writer,
			"The value a token names, for reading a string-valued enum back out of the browser.");
		writer.WriteLine("/// <param name=\"enumType\">The enum the caller is expecting.</param>");
		writer.WriteLine("/// <param name=\"token\">The token the browser sent.</param>");
		writer.WriteLine("/// <returns>The boxed value, or <see langword=\"null\"/> when the type is not string-valued or the token is unknown.</returns>");
		writer.WriteLine("public static object? FromToken(Type enumType, string token)");
		writer.WriteLine("{");
		writer.Indent();

		foreach (var stringValuedEnum in stringValuedEnums)
		{
			writer.WriteLine($"if (enumType == typeof({stringValuedEnum.Name}))");
			writer.WriteLine("{");
			writer.Indent();
			writer.WriteLine($"return {stringValuedEnum.Name}FromToken(token);");
			writer.Outdent();
			writer.WriteLine("}");
			writer.WriteLine();
		}

		writer.WriteLine("return null;");
		writer.Outdent();
		writer.WriteLine("}");
	}

	private static void WriteTokenForOne(CSharpWriter writer, GeneratedEnum stringValuedEnum)
	{
		var parameterName = ConstructorMapper.ToCamelCase(stringValuedEnum.Name);
		DocCommentEmitter.WriteSummary(writer, $"The token three.js compares a <see cref=\"{stringValuedEnum.Name}\"/> against.");
		writer.WriteLine($"/// <param name=\"{parameterName}\">The value to send.</param>");
		writer.WriteLine("/// <returns>The token.</returns>");
		writer.WriteLine($"private static string TokenFor({stringValuedEnum.Name} {parameterName})");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine($"return {parameterName} switch");
		writer.WriteLine("{");
		writer.Indent();

		// Aliases share their canonical member's value, so a second arm naming one would be an
		// unreachable duplicate the compiler rejects.
		foreach (var member in stringValuedEnum.Members.Where(x => x.AliasOf is null))
		{
			writer.WriteLine($"{stringValuedEnum.Name}.{member.DeclarationName} => {Quote(member.Token ?? string.Empty)},");
		}

		writer.WriteLine($"_ => throw new NotImplementedException($\"No three.js token is known for {stringValuedEnum.Name} '{{{parameterName}}}'.\")");
		writer.Outdent();
		writer.WriteLine("};");
		writer.Outdent();
		writer.WriteLine("}");
	}

	private static void WriteFromTokenForOne(CSharpWriter writer, GeneratedEnum stringValuedEnum)
	{
		DocCommentEmitter.WriteSummary(writer, $"The <see cref=\"{stringValuedEnum.Name}\"/> a token names.");
		writer.WriteLine("/// <param name=\"token\">The token the browser sent.</param>");
		writer.WriteLine("/// <returns>The value, or <see langword=\"null\"/> when three.js sent something this build does not know.</returns>");
		writer.WriteLine($"private static object? {stringValuedEnum.Name}FromToken(string token)");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine("return token switch");
		writer.WriteLine("{");
		writer.Indent();

		foreach (var member in stringValuedEnum.Members.Where(x => x.AliasOf is null))
		{
			writer.WriteLine($"{Quote(member.Token ?? string.Empty)} => {stringValuedEnum.Name}.{member.DeclarationName},");
		}

		writer.WriteLine("_ => null");
		writer.Outdent();
		writer.WriteLine("};");
		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>
	/// Writes a token as a C# string literal. three.js's tokens are plain lowercase words today, so the
	/// escaping never fires — it is here because a token is upstream data and a future one containing a
	/// quote would otherwise emit source that does not compile.
	/// </summary>
	/// <param name="token">The token to quote.</param>
	/// <returns>The literal, including its quotes.</returns>
	private static string Quote(string token)
	{
		return "\"" + token.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal) + "\"";
	}
}
