using System.Text;
using System.Text.Json;
using Blazor.ThreeJS.Emitter;
using Blazor.ThreeJS.Emitter.Emit;
using Blazor.ThreeJS.Emitter.Ir;

var mode = args.FirstOrDefault() ?? "--write";
if (mode is not ("--write" or "--check"))
{
	Console.Error.WriteLine("Usage: dotnet run --project generator/emitter -- [--write|--check]");
	Console.Error.WriteLine("  --write  regenerate the committed output (default)");
	Console.Error.WriteLine("  --check  regenerate in memory and fail if it differs from the committed output");
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

var emitter = new ClassEmitter(ir);
var audit = new EmissionAudit();
var emittedFiles = new List<EmittedFile>();

foreach (var className in EmitterConfig.EmittedClassNames)
{
	emittedFiles.Add(emitter.Emit(emitter.GetClass(className), audit));
}

ProjectRefusalsOverWholeIr(ir, emitter, audit);

emittedFiles.Add(new EmittedFile
{
	RelativePath = "generator/emitter-audit.md",
	Contents = audit.Render(EmitterConfig.EmittedClassNames, ir.Meta?.TypesVersion ?? "unknown")
});

return mode == "--check"
	? Check(repositoryRoot, emittedFiles)
	: Write(repositoryRoot, emittedFiles);

/// <summary>
/// Runs every class in the IR through the same constructor mapping, without emitting, so the audit
/// can report what the full run is still blocked on instead of discovering it 309 files in.
/// </summary>
static void ProjectRefusalsOverWholeIr(IrRoot ir, ClassEmitter emitter, EmissionAudit audit)
{
	foreach (var irClass in ir.Classes)
	{
		if (EmitterConfig.EmittedClassNames.Contains(irClass.Name))
		{
			continue;
		}

		var projectionAudit = new EmissionAudit(NumericAuditScope.Projected);
		try
		{
			emitter.Emit(irClass, projectionAudit);
		}
		catch (UnsupportedMemberException exception)
		{
			audit.RecordRefusedClass(irClass.Name, irClass.File, exception.Reason);
			continue;
		}

		audit.AdoptNumerics(projectionAudit.NumericEntries);
	}
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
