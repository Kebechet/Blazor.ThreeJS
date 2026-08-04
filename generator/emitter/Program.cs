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
var methodMapper = new MethodMapper();
var scope = new EmissionScope(ir, mapper, constructorMapper);
var surfaces = new ClassSurfaceResolver(ir, name => scope.IsEmittable(name) || EmitterConfig.HandWrittenClassNames.Contains(name));
var classifier = new MemberClassifier(surfaces, mapper, methodMapper);
var coverage = new CoverageReport(ir, scope, enums, mapper, classifier);

var emittedClassNames = coverage.EmittableClasses
	.Select(x => x.Class.Name)
	.ToList();

// Documentation crefs are resolved against the names this run will produce, so a `{@link Material}`
// marker on one generated class points at the generated `Material` rather than degrading to plain
// text. Registered before the first file is emitted, because the first file may carry such a marker.
EmitterConfig.RegisterGeneratedTypeNames(emittedClassNames.Concat(enums.Generatable.Select(x => x.Name)));

var emitter = new ClassEmitter(ir, mapper, constructorMapper, classifier, scope);
var audit = new EmissionAudit();
var emittedFiles = new List<EmittedFile>();

foreach (var result in coverage.EmittableClasses)
{
	emittedFiles.Add(emitter.Emit(result.Class, audit));
}

// Every generatable enum is emitted, not only the ones a currently emittable class happens to
// reference. An enum is a leaf: it carries no handle, no wire op and no dependency on the class
// surface, so gating it on which classes are emittable today would only churn the committed set as
// that surface grows. What is or is not generatable is EnumCatalog's call, and api-coverage.md
// reports both halves.
var enumEmitter = new EnumEmitter(ir);
foreach (var generatedEnum in enums.Generatable)
{
	emittedFiles.Add(enumEmitter.Emit(generatedEnum));
}

emittedFiles.Add(new EmittedFile
{
	RelativePath = "generator/emitter-audit.md",
	Contents = audit.Render(emittedClassNames, ir.Meta?.TypesVersion ?? "unknown")
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

	return WriteProjection(fullProjectionPath, emittedFiles);
}

return mode == "--check"
	? Check(repositoryRoot, emittedFiles)
	: Write(repositoryRoot, emittedFiles);

/// <summary>
/// Writes the generated C# into a scratch directory rather than into the package, so the corpus can
/// be compiled in isolation without touching what is committed.
/// </summary>
static int WriteProjection(string outputDirectory, List<EmittedFile> emittedFiles)
{
	Directory.CreateDirectory(outputDirectory);
	foreach (var staleFile in Directory.EnumerateFiles(outputDirectory, "*.cs"))
	{
		File.Delete(staleFile);
	}

	var written = 0;
	foreach (var emittedFile in emittedFiles.Where(x => x.RelativePath.EndsWith(".cs", StringComparison.Ordinal)))
	{
		File.WriteAllText(Path.Combine(outputDirectory, Path.GetFileName(emittedFile.RelativePath)), emittedFile.Contents);
		written++;
	}

	Console.WriteLine($"projected {written} file(s) into {outputDirectory}");
	return 0;
}

static int Write(string repositoryRoot, List<EmittedFile> emittedFiles)
{
	foreach (var staleFile in FindStaleGeneratedFiles(repositoryRoot, emittedFiles))
	{
		File.Delete(Path.Combine(repositoryRoot, staleFile.Replace('/', Path.DirectorySeparatorChar)));
		Console.WriteLine($"deleted {staleFile}");
	}

	foreach (var emittedFile in emittedFiles)
	{
		var absolutePath = Path.Combine(repositoryRoot, emittedFile.RelativePath.Replace('/', Path.DirectorySeparatorChar));
		Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
		File.WriteAllText(absolutePath, emittedFile.Contents, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
	}

	Console.WriteLine($"wrote {emittedFiles.Count} file(s)");
	return 0;
}

/// <summary>
/// Committed generated files this run no longer produces. Without this, a class that stops being
/// emittable — an upstream rename, a mapping rule tightened — leaves its file behind, still compiled
/// into the package and still passing the golden check, because the check only compares the files the
/// emitter does produce.
/// </summary>
/// <param name="repositoryRoot">Repository root.</param>
/// <param name="emittedFiles">Everything this run produces.</param>
/// <returns>Repository-relative paths to delete.</returns>
static List<string> FindStaleGeneratedFiles(string repositoryRoot, List<EmittedFile> emittedFiles)
{
	var generatedDirectory = Path.Combine(repositoryRoot, "src", "Blazor.ThreeJS", "Generated");
	if (!Directory.Exists(generatedDirectory))
	{
		return [];
	}

	var expected = emittedFiles
		.Select(x => x.RelativePath)
		.ToHashSet(StringComparer.OrdinalIgnoreCase);

	return Directory.EnumerateFiles(generatedDirectory, "*.cs")
		.Select(x => "src/Blazor.ThreeJS/Generated/" + Path.GetFileName(x))
		.Where(x => !expected.Contains(x))
		.OrderBy(x => x, StringComparer.Ordinal)
		.ToList();
}

/// <summary>
/// The golden check. Regenerates in memory and compares against what is committed, so any change to
/// the emitter that changes its output fails here until the new output is reviewed and committed.
/// </summary>
static int Check(string repositoryRoot, List<EmittedFile> emittedFiles)
{
	var hasDrifted = false;
	foreach (var staleFile in FindStaleGeneratedFiles(repositoryRoot, emittedFiles))
	{
		Console.Error.WriteLine($"STALE   {staleFile} — it is committed but the emitter no longer produces it.");
		hasDrifted = true;
	}

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
			continue;
		}

		hasDrifted = true;
		Console.Error.WriteLine($"DRIFT   {emittedFile.RelativePath}");
		ReportFirstDifference(committed, regenerated);
	}

	if (!hasDrifted)
	{
		Console.WriteLine($"ok      {emittedFiles.Count} generated file(s) match what is committed");
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
