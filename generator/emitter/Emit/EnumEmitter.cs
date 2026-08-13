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

		// three.js spells a few of these sets as strings. For those the C# value is only a position and
		// carries no meaning upstream, so the wire sends the token instead — see `ThreeStringEnum`.
		var wireNote = generatedEnum.IsStringValued
			? " Encoded on the wire as the string three.js compares against, not as the C# value, which is only a position."
			: " Encoded on the wire as the numeric value three.js itself uses, not as the member name.";

		DocCommentEmitter.WriteSummary(writer, summary + wireNote);

		writer.WriteLine($"public enum {generatedEnum.Name} : {generatedEnum.BackingTypeName}");
		writer.WriteLine("{");
		writer.Indent();

		foreach (var (index, member) in generatedEnum.Members.Index())
		{
			// A constant group's members are exported on the THREE namespace individually
			// (`THREE.FrontSide`); a real TypeScript enum's are reached through it (`MOUSE.LEFT`), and
			// the WebGPU ones are not on the WebGL bundle at all — hence no `THREE.` prefix there.
			var upstreamSpelling = generatedEnum.Source == EnumSource.DeclaredEnum
				? $"Matches <c>{generatedEnum.Name}.{member.Name}</c> in three.js."
				: $"Matches <c>THREE.{member.Name}</c>.";

			var memberSummary = member.Doc?.Summary is { Length: > 0 } rawMemberSummary
				? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawMemberSummary))
				: upstreamSpelling;

			if (member.Token is { } token)
			{
				memberSummary += token.Length > 0
					? $" Sent as <c>\"{token}\"</c>."
					: " Sent as the empty string, which is what three.js uses for this.";
			}

			if (member.AliasOf is { } aliasOf)
			{
				// A cref only resolves against an unescaped identifier, so a member three.js happens to
				// spell as a C# keyword is named in plain code font instead of linked. An unresolvable
				// cref is a CS1574 warning multiplied by every target framework.
				var aliasReference = CSharpIdentifier.Escape(aliasOf) == aliasOf
					? $"<see cref=\"{aliasOf}\"/>"
					: $"<c>{aliasOf}</c>";

				memberSummary += $" An alternative spelling three.js gives the same value as {aliasReference}.";
			}

			DocCommentEmitter.WriteSummary(writer, memberSummary);

			var value = member.AliasOf is { } aliasedName
				? CSharpIdentifier.Escape(aliasedName)
				: member.Value.ToString(CultureInfo.InvariantCulture);

			var isLast = index == generatedEnum.Members.Count - 1;
			writer.WriteLine(isLast
				? $"{member.DeclarationName} = {value}"
				: $"{member.DeclarationName} = {value},");

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
