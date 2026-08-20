using Blazor.ThreeJS.Emitter.Ir;
using Blazor.ThreeJS.Emitter.Map;

namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Emits one structural interface as a C# record.
/// <para>
/// three.js describes some of what it hands back with a shape rather than a class:
/// <c>BufferGeometry.groups</c> is a <c>GeometryGroup[]</c>, and a group is
/// <c>{ start, count, materialIndex? }</c>. A value with no identity, which is why it is a record and
/// not a handle-backed class - two groups holding the same numbers are the same group, and neither
/// side keeps a reference to one.
/// </para>
/// <para>
/// The wire form is written out rather than discovered by reflection, because the emitter already
/// knows every member and its type. Both directions use three.js's own member names: the object is
/// assigned straight onto a three.js instance, or read straight off one.
/// </para>
/// </summary>
internal sealed class StructureEmitter
{
	private readonly IrRoot _ir;
	private readonly TypeMapper _mapper;

	/// <summary>Builds an emitter over one IR snapshot.</summary>
	/// <param name="ir">The parsed IR, for the provenance header.</param>
	/// <param name="mapper">Type mapper, which resolves each member's C# type.</param>
	public StructureEmitter(IrRoot ir, TypeMapper mapper)
	{
		_ir = ir;
		_mapper = mapper;
	}

	/// <summary>Emits the C# source for one structure.</summary>
	/// <param name="irInterface">The interface to stand for.</param>
	/// <returns>The generated file.</returns>
	public EmittedFile Emit(IrInterface irInterface)
	{
		var members = irInterface.Properties
			.Select(x => new StructureMember(x, _mapper.Map(x.Type, new TypeMappingContext
			{
				MemberName = x.Name,
				NumericKind = x.NumericKind
			})))
			.ToList();

		var writer = new CSharpWriter();
		writer.WriteLine($"// Generated from {_ir.Meta?.TypesPackage ?? "@types/three"}@{_ir.Meta?.TypesVersion ?? "unknown"} by generator/emitter. Do not edit by hand.");
		writer.WriteLine("// Re-run `npm run emit` after changing the emitter or generator/three-api.json.");
		writer.WriteLine();
		writer.WriteLine("using System.Text.Json;");
		writer.WriteLine($"using {EmitterConfig.CoreNamespace};");
		if (members.Any(x => EmitterConfig.MathTypeNames.Contains(x.BareTypeName)))
		{
			writer.WriteLine($"using {EmitterConfig.MathNamespace};");
		}

		writer.WriteLine();
		writer.WriteLine($"namespace {EmitterConfig.GeneratedNamespace};");
		writer.WriteLine();

		var summary = $"The shape three.js calls <c>{irInterface.Name}</c>.";

		DocCommentEmitter.WriteSummary(
			writer,
			summary +
			" A plain value rather than a handle-backed object: three.js declares it as a shape, and nothing on either side keeps a reference to one. " +
			"It travels as its own members, under three.js's names for them.");

		writer.WriteLine($"public sealed record {irInterface.Name} : IThreeStructure");
		writer.WriteLine("{");
		writer.Indent();

		foreach (var (index, member) in members.Index())
		{
			if (index > 0)
			{
				writer.WriteLine();
			}

			var memberSummary = member.Property.Doc?.Summary is { Length: > 0 } rawMember
				? DocCommentEmitter.EnsureSentenceEnd(DocCommentEmitter.RenderInline(rawMember))
				: $"three.js's <c>{member.Property.Name}</c>.";

			DocCommentEmitter.WriteSummary(writer, memberSummary);
			writer.WriteLine($"public {member.CSharpTypeName} {member.CSharpName} {{ get; init; }}");
		}

		WriteToWireMembers(writer, members);
		WriteFromWireMembers(writer, irInterface.Name, members);

		writer.Outdent();
		writer.WriteLine("}");

		return new EmittedFile
		{
			RelativePath = $"src/Blazor.ThreeJS/Generated/{irInterface.Name}.cs",
			Contents = writer.ToSource()
		};
	}

	/// <summary>Writes the outbound half: this value's members, under three.js's own names.</summary>
	/// <param name="writer">Destination.</param>
	/// <param name="members">The resolved members.</param>
	private static void WriteToWireMembers(CSharpWriter writer, IReadOnlyList<StructureMember> members)
	{
		writer.WriteLine();
		DocCommentEmitter.WriteSummary(
			writer,
			"This value's members, keyed by three.js's name for each. " +
			"An optional member left unset is omitted rather than sent as null, so three.js applies its own default the way it would for an object literal that never mentioned it.");

		DocCommentEmitter.WriteReturns(writer, "The members to send.");
		writer.WriteLine("IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine("var members = new Dictionary<string, object?>(StringComparer.Ordinal);");
		foreach (var (index, member) in members.Index())
		{
			if (!member.IsOptional)
			{
				writer.WriteLine($"members[\"{member.Property.Name}\"] = {member.CSharpName};");
				continue;
			}

			if (index > 0)
			{
				writer.WriteLine();
			}

			writer.WriteLine($"if ({member.CSharpName} is not null)");
			writer.WriteLine("{");
			writer.Indent();
			writer.WriteLine($"members[\"{member.Property.Name}\"] = {member.CSharpName};");
			writer.Outdent();
			writer.WriteLine("}");
		}

		writer.WriteLine();
		writer.WriteLine("return members;");
		writer.Outdent();
		writer.WriteLine("}");
	}

	/// <summary>Writes the inbound half: build this value from what the applier answered with.</summary>
	/// <param name="writer">Destination.</param>
	/// <param name="typeName">Name of the record being written.</param>
	/// <param name="members">The resolved members.</param>
	private static void WriteFromWireMembers(CSharpWriter writer, string typeName, IReadOnlyList<StructureMember> members)
	{
		writer.WriteLine();
		DocCommentEmitter.WriteSummary(
			writer,
			$"Builds a <c>{typeName}</c> from the members the applier sent back. " +
			"A member three.js did not carry keeps this instance's own value, which for the blank instance the decoder builds is the C# default - and an absent optional member is exactly that.");

		DocCommentEmitter.WriteParam(writer, "members", "The decoded members, keyed by three.js's name for each.");
		DocCommentEmitter.WriteReturns(writer, "The value those members describe.");
		writer.WriteLine("IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members)");
		writer.WriteLine("{");
		writer.Indent();
		writer.WriteLine($"return new {typeName}");
		writer.WriteLine("{");
		writer.Indent();
		foreach (var (index, member) in members.Index())
		{
			var comma = index == members.Count - 1 ? string.Empty : ",";
			writer.WriteLine(
				$"{member.CSharpName} = members.TryGetValue(\"{member.Property.Name}\", out var {member.LocalName})" +
				$" ? ThreeValue.Decode<{member.CSharpTypeName}>({member.LocalName})" +
				$" : {member.CSharpName}{comma}");
		}

		writer.Outdent();
		writer.WriteLine("};");
		writer.Outdent();
		writer.WriteLine("}");
	}
}

/// <summary>One member of a structure, with its type resolved.</summary>
/// <param name="Property">The three.js declaration.</param>
/// <param name="Mapping">Its resolved C# type.</param>
internal sealed record StructureMember(IrProperty Property, TypeMapping Mapping)
{
	/// <summary>Whether three.js declares the member optional, so it may be omitted in both directions.</summary>
	public bool IsOptional
	{
		get { return Property.IsOptional || Mapping.IsExplicitlyNullable; }
	}

	/// <summary>The C# type as written, nullable when three.js allows the member to be absent.</summary>
	public string CSharpTypeName
	{
		get { return IsOptional ? Mapping.CSharpTypeName + "?" : Mapping.CSharpTypeName!; }
	}

	/// <summary>The type without its nullable annotation, for deciding which usings are needed.</summary>
	public string BareTypeName
	{
		get { return Mapping.CSharpTypeName!.TrimEnd('?').TrimEnd('[', ']'); }
	}

	/// <summary>PascalCased member name.</summary>
	public string CSharpName
	{
		get { return char.ToUpperInvariant(Property.Name[0]) + Property.Name[1..]; }
	}

	/// <summary>Name of the local the decode block binds the raw element to.</summary>
	public string LocalName
	{
		get { return char.ToLowerInvariant(Property.Name[0]) + Property.Name[1..] + "Element"; }
	}
}
