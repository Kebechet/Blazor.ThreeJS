using System.Text;
using System.Text.Json;
using Blazor.ThreeJS.Emitter;
using Blazor.ThreeJS.Emitter.Emit;
using Blazor.ThreeJS.Emitter.Ir;
using Blazor.ThreeJS.Emitter.Map;

var mode = args.FirstOrDefault() ?? "--write";
if (mode is not ("--write" or "--check" or "--project"))
{
	Console.Error.WriteLine("Usage: dotnet run --project generator/emitter -- [--write|--check|--project <dir>]");
	Console.Error.WriteLine("  --write        regenerate the committed output (default)");
	Console.Error.WriteLine("  --check        regenerate in memory and fail if it differs from the committed output");
	Console.Error.WriteLine("  --project DIR  write every emittable class and enum to DIR, so the claim that they");
	Console.Error.WriteLine("                 are emittable can be checked by compiling them. Never writes into the repository.");
	return 2;
}

var repositoryRoot = FindRepositoryRoot();
var irPath = Path.Combine(repositoryRoot, "generator", "three-api.json");
if (!File.Exists(irPath))
{
	Console.Error.WriteLine($"IR not found at '{irPath}'. Run `npm run extract` first.");
	return 2;
}

var ir = JsonSerializer.Deserialize<IrRoot>(File.ReadAllText(irPath), IrSerialization.Options)
	?? throw new InvalidOperationException($"'{irPath}' did not deserialize into an IR root.");

var enums = new EnumCatalog(ir);
var mapper = new TypeMapper(ir, enums);
var constructorMapper = new ConstructorMapper();
var scope = new EmissionScope(ir, mapper, constructorMapper);
var classifier = new MemberClassifier(ir, mapper);
var coverage = new CoverageReport(ir, scope, enums, mapper, classifier);

var emitter = new ClassEmitter(ir, mapper, constructorMapper, classifier);
var audit = new EmissionAudit();
var emittedFiles = new List<EmittedFile>();

foreach (var className in EmitterConfig.EmittedClassNames)
{
	emittedFiles.Add(emitter.Emit(emitter.GetClass(className), audit));
}

ProjectNumericsOverEmittableClasses(coverage, emitter, audit);

emittedFiles.Add(new EmittedFile
{
	RelativePath = "generator/emitter-audit.md",
	Contents = audit.Render(EmitterConfig.EmittedClassNames, ir.Meta?.TypesVersion ?? "unknown")
});

emittedFiles.Add(new EmittedFile
{
	RelativePath = "generator/api-coverage.md",
	Contents = coverage.RenderMarkdown()
});

emittedFiles.Add(new EmittedFile
{
	RelativePath = "generator/api-coverage.json",
	Contents = coverage.RenderJson()
});

if (mode == "--project")
{
	var projectionDirectory = args.Skip(1).FirstOrDefault();
	if (projectionDirectory is null)
	{
		Console.Error.WriteLine("--project needs an output directory.");
		return 2;
	}

	// The projection is a throwaway corpus of roughly 200 files. Writing it into the repository would
	// look exactly like a full run having landed, so the guard is enforced rather than documented.
	var fullProjectionPath = Path.GetFullPath(projectionDirectory);
	if (fullProjectionPath.StartsWith(Path.GetFullPath(repositoryRoot), StringComparison.OrdinalIgnoreCase))
	{
		Console.Error.WriteLine($"--project refuses to write inside the repository ('{fullProjectionPath}'). Point it at a scratch directory.");
		return 2;
	}

	return WriteProjection(fullProjectionPath, coverage, emitter, enums, mapper);
}

return mode == "--check"
	? Check(repositoryRoot, emittedFiles)
	: Write(repositoryRoot, emittedFiles);

/// <summary>
/// Emits every class the mapper calls emittable but the allowlist has not reached yet, purely to
/// collect its numeric typing calls. Those are the decisions the upstream JSDoc did not make for us,
/// and they are worth reviewing before a full run rather than 190 files in. Which classes are
/// emittable is not decided here — <c>EmissionScope</c> owns that, and <c>api-coverage.md</c> reports it.
/// </summary>
static void ProjectNumericsOverEmittableClasses(CoverageReport coverage, ClassEmitter emitter, EmissionAudit audit)
{
	foreach (var result in coverage.EmittableClasses)
	{
		if (EmitterConfig.EmittedClassNames.Contains(result.Class.Name))
		{
			continue;
		}

		var projectionAudit = new EmissionAudit(NumericAuditScope.Projected);
		emitter.Emit(result.Class, projectionAudit);
		audit.AdoptNumerics(projectionAudit.NumericEntries);
	}
}

/// <summary>
/// Writes every class and enum the mapper says is emittable into a scratch directory, so "N classes
/// are emittable" can be checked by compiling them rather than taken on trust. Classes whose names
/// are already hand-written are left out: they would collide with the real ones in the same
/// namespace, and the hand-written ones are what the projection compiles against.
/// </summary>
static int WriteProjection(
	string outputDirectory,
	CoverageReport coverage,
	ClassEmitter emitter,
	EnumCatalog enums,
	TypeMapper mapper)
{
	Directory.CreateDirectory(outputDirectory);
	foreach (var staleFile in Directory.EnumerateFiles(outputDirectory, "*.cs"))
	{
		File.Delete(staleFile);
	}

	var written = 0;
	var failed = 0;
	var sealedBaseCollisions = new List<string>();
	foreach (var result in coverage.EmittableClasses)
	{
		if (EmitterConfig.ExistingCSharpTypeNames.Contains(result.Class.Name))
		{
			continue;
		}

		try
		{
			var emittedFile = emitter.Emit(result.Class, new EmissionAudit(NumericAuditScope.Projected));

			// A generated class whose nearest mirrored ancestor is one of Plan 1's sealed hand-written
			// leaves cannot compile alongside them. That is a property of the hand-written types, not of
			// the mapping, so the projection names them instead of silently pretending they compile.
			var baseTypeName = emitter.ResolveBaseTypeName(result.Class);
			if (EmitterConfig.SealedHandWrittenClassNames.Contains(baseTypeName))
			{
				sealedBaseCollisions.Add($"{result.Class.Name} : {baseTypeName}");
				continue;
			}

			File.WriteAllText(Path.Combine(outputDirectory, $"{result.Class.Name}.cs"), emittedFile.Contents);
			written++;
		}
		catch (UnsupportedMemberException exception)
		{
			Console.Error.WriteLine($"PROJECTION MISMATCH {result.Class.Name}: {exception.Reason}");
			failed++;
		}
	}

	if (sealedBaseCollisions.Count > 0)
	{
		Console.WriteLine($"skipped {sealedBaseCollisions.Count} class(es) whose base is a sealed hand-written type: {string.Join(", ", sealedBaseCollisions)}");
	}

	var enumEmitter = new EnumEmitter(coverage.Ir);
	foreach (var generatedEnum in enums.Generatable.Where(x => mapper.RequiredEnumNames.Contains(x.Name)))
	{
		if (EmitterConfig.ExistingCSharpTypeNames.Contains(generatedEnum.Name))
		{
			continue;
		}

		File.WriteAllText(Path.Combine(outputDirectory, $"{generatedEnum.Name}.cs"), enumEmitter.Emit(generatedEnum).Contents);
		written++;
	}

	Console.WriteLine($"projected {written} file(s) into {outputDirectory}");
	if (failed == 0)
	{
		return 0;
	}

	Console.Error.WriteLine($"{failed} class(es) the scope calls emittable were refused by the emitter — the two disagree.");
	return 1;
}

static int Write(string repositoryRoot, List<EmittedFile> emittedFiles)
{
	foreach (var emittedFile in emittedFiles)
	{
		var absolutePath = Path.Combine(repositoryRoot, emittedFile.RelativePath.Replace('/', Path.DirectorySeparatorChar));
		Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
		File.WriteAllText(absolutePath, emittedFile.Contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
		Console.WriteLine($"wrote {emittedFile.RelativePath}");
	}

	return 0;
}

/// <summary>
/// The golden check. Regenerates in memory and compares against what is committed, so any change to
/// the emitter that changes its output fails here until the new output is reviewed and committed.
/// </summary>
static int Check(string repositoryRoot, List<EmittedFile> emittedFiles)
{
	var hasDrifted = false;
	foreach (var emittedFile in emittedFiles)
	{
		var absolutePath = Path.Combine(repositoryRoot, emittedFile.RelativePath.Replace('/', Path.DirectorySeparatorChar));
		if (!File.Exists(absolutePath))
		{
			Console.Error.WriteLine($"MISSING {emittedFile.RelativePath} — the emitter produces it but it is not committed.");
			hasDrifted = true;
			continue;
		}

		// Line endings are normalized rather than compared: the repository has no .gitattributes and
		// core.autocrlf is enabled on Windows, so the committed file can arrive with CRLF on one
		// machine and LF on another without a single generated character having changed.
		var committed = NormalizeLineEndings(File.ReadAllText(absolutePath));
		var regenerated = NormalizeLineEndings(emittedFile.Contents);
		if (committed == regenerated)
		{
			Console.WriteLine($"ok      {emittedFile.RelativePath}");
			continue;
		}

		hasDrifted = true;
		Console.Error.WriteLine($"DRIFT   {emittedFile.RelativePath}");
		ReportFirstDifference(committed, regenerated);
	}

	if (!hasDrifted)
	{
		return 0;
	}

	Console.Error.WriteLine();
	Console.Error.WriteLine("The emitter's output no longer matches the committed files. Review the change, then run");
	Console.Error.WriteLine("`npm run emit` to accept it.");

	return 1;
}

static void ReportFirstDifference(string committed, string regenerated)
{
	var committedLines = committed.Split('\n');
	var regeneratedLines = regenerated.Split('\n');
	var lineCount = System.Math.Max(committedLines.Length, regeneratedLines.Length);

	for (var index = 0; index < lineCount; index++)
	{
		var committedLine = index < committedLines.Length ? committedLines[index] : "<end of file>";
		var regeneratedLine = index < regeneratedLines.Length ? regeneratedLines[index] : "<end of file>";
		if (committedLine == regeneratedLine)
		{
			continue;
		}

		Console.Error.WriteLine($"        first difference at line {index + 1}:");
		Console.Error.WriteLine($"          committed:   {committedLine}");
		Console.Error.WriteLine($"          regenerated: {regeneratedLine}");
		return;
	}
}

static string NormalizeLineEndings(string text)
{
	return text.Replace("\r\n", "\n");
}

/// <summary>
/// Walks up from the executing assembly until it finds the repository root, so the tool works the
/// same whether it is launched from the repository root or from its own directory.
/// </summary>
static string FindRepositoryRoot()
{
	var directory = new DirectoryInfo(AppContext.BaseDirectory);
	while (directory is not null)
	{
		if (Directory.Exists(Path.Combine(directory.FullName, "generator")) &&
			Directory.Exists(Path.Combine(directory.FullName, "src")))
		{
			return directory.FullName;
		}

		directory = directory.Parent;
	}

	throw new InvalidOperationException("Could not locate the repository root above the executing assembly.");
}
