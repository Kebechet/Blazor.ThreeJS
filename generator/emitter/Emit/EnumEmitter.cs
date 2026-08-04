using System.Globalization;
using Blazor.ThreeJS.Emitter.Ir;
using Blazor.ThreeJS.Emitter.Map;

namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Emits one three.js value set as a C# enum. Member names are kept exactly as three.js spells them,
/// including the deprecated <c>MipMap</c> variants, because the name is what a consumer reads in the
/// three.js documentation and a tidier spelling would just be a second thing to learn.
/// </summary>
internal sealed class EnumEmitter
{
	private readonly IrRoot _ir;

	/// <summary>Builds an emitter over one IR snapshot, for the provenance header.</summary>
	/// <param name="ir">The parsed IR.</param>
	public EnumEmitter(IrRoot ir)
	{
		_ir = ir;
	}

	/// <summary>Emits the C# source for one enum.</summary>
	/// <param name="generatedEnum">The resolved enum.</param>
	/// <returns>The generated file.</returns>
	public EmittedFile Emit(GeneratedEnum generatedEnum)
	{
		var writer = new CSharpWriter();
		writer.WriteLine($"// Generated from {_ir.Meta?.TypesPackage ?? "@types/three"}@{_ir.Meta?.TypesVersion ?? "unknown"} by generator/emitter. Do not edit by hand.");
		writer.WriteLine("// Re-run `npm run emit` after changing the emitter or generator/three-api.json.");
		writer.WriteLine();
		writer.WriteLine($"namespace {EmitterConfig.GeneratedNamespace};");
		writer.WriteLine();

		var summary = generatedEnum.Doc?.Summary is { Length: > 0 } rawSummary
			? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawSummary))
			: $"The values three.js accepts for <c>{generatedEnum.Name}</c>.";

		DocCommentEmitter.WriteSummary(
			writer,
			summary + " Encoded on the wire as the numeric value three.js itself uses, not as the member name.");

		writer.WriteLine($"public enum {generatedEnum.Name} : {generatedEnum.BackingTypeName}");
		writer.WriteLine("{");
		writer.Indent();

		foreach (var (index, member) in generatedEnum.Members.Index())
		{
			var memberSummary = member.Doc?.Summary is { Length: > 0 } rawMemberSummary
				? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawMemberSummary))
				: $"Matches <c>THREE.{member.Name}</c>.";

			if (member.AliasOf is { } aliasOf)
			{
				memberSummary += $" An alternative spelling three.js gives the same value as <see cref=\"{aliasOf}\"/>.";
			}

			DocCommentEmitter.WriteSummary(writer, memberSummary);

			var value = member.AliasOf ?? member.Value.ToString(CultureInfo.InvariantCulture);
			var isLast = index == generatedEnum.Members.Count - 1;
			writer.WriteLine(isLast
				? $"{member.Name} = {value}"
				: $"{member.Name} = {value},");

			if (!isLast)
			{
				writer.WriteLine();
			}
		}

		writer.Outdent();
		writer.WriteLine("}");

		return new EmittedFile
		{
			RelativePath = $"src/Blazor.ThreeJS/Generated/{generatedEnum.Name}.cs",
			Contents = writer.ToSource()
		};
	}
}
