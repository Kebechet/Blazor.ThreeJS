using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Blazor.ThreeJS.E2E;

/// <summary>
/// Runs the demo storybook as a child process for the lifetime of the suite and serves it over
/// loopback, so the tests see the same static assets a consumer's browser would rather than the
/// compile-time view of them.
/// </summary>
internal sealed class DemoServer : IAsyncDisposable
{
	/// <summary>Project file of the WebAssembly host, relative to the repository root.</summary>
	public static readonly string[] WebAssemblyHost = ["demo", "Blazor.ThreeJS.Demo.Wasm", "Blazor.ThreeJS.Demo.Wasm.csproj"];

	/// <summary>Project file of the Blazor Server host, relative to the repository root.</summary>
	public static readonly string[] ServerHost = ["demo", "Blazor.ThreeJS.Demo.Server", "Blazor.ThreeJS.Demo.Server.csproj"];

	private readonly string[] _projectPath;

	/// <summary>Builds a runner for one of the two demo hosts.</summary>
	/// <param name="projectPath">Project file, as path segments below the repository root.</param>
	public DemoServer(string[] projectPath)
	{
		_projectPath = projectPath;
	}

	/// <summary>
	/// How long the demo is given to build and start listening. It is built from source on a cold
	/// checkout, which on a CI runner is minutes rather than seconds.
	/// </summary>
	private static readonly TimeSpan StartTimeout = TimeSpan.FromMinutes(5);

	/// <summary>Gap between readiness polls while the demo is still starting.</summary>
	private static readonly TimeSpan ReadinessPollInterval = TimeSpan.FromMilliseconds(250);

	/// <summary>Lines the demo wrote to stdout or stderr, kept so a failed start can explain itself.</summary>
	private readonly ConcurrentQueue<string> _outputLines = new();

	private Process? _process;

	/// <summary>Root URL the demo is listening on, for example <c>http://127.0.0.1:51234</c>.</summary>
	public string BaseUrl { get; private set; } = string.Empty;

	/// <summary>
	/// Starts the demo on a free loopback port and returns once it answers an HTTP request. Leaves
	/// nothing running if the start fails: the caller's own teardown is what stops the process, and
	/// this method only ever adds to what that has to clean up.
	/// </summary>
	public async Task StartAsync()
	{
		var repositoryRoot = FindRepositoryRoot();
		BaseUrl = $"http://127.0.0.1:{ReserveTcpPort()}";

		var startInfo = new ProcessStartInfo("dotnet")
		{
			WorkingDirectory = repositoryRoot,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true
		};
		startInfo.ArgumentList.Add("run");
		startInfo.ArgumentList.Add("--project");
		startInfo.ArgumentList.Add(Path.Combine([repositoryRoot, .. _projectPath]));
		startInfo.ArgumentList.Add("--configuration");
		startInfo.ArgumentList.Add("Release");
		startInfo.ArgumentList.Add("--no-launch-profile");
		startInfo.ArgumentList.Add("--urls");
		startInfo.ArgumentList.Add(BaseUrl);
		startInfo.Environment["DOTNET_NOLOGO"] = "1";

		var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
		process.OutputDataReceived += CaptureOutput;
		process.ErrorDataReceived += CaptureOutput;
		_process = process;

		if (!process.Start())
		{
			throw new InvalidOperationException("Could not start the demo storybook process.");
		}

		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		await WaitForReadyAsync();
	}

	/// <summary>
	/// Polls the root URL until the demo answers, the demo dies, or the start timeout elapses.
	/// </summary>
	private async Task WaitForReadyAsync()
	{
		using var client = new HttpClient();
		var deadline = DateTime.UtcNow + StartTimeout;
		while (DateTime.UtcNow < deadline)
		{
			if (_process is { HasExited: true })
			{
				throw new InvalidOperationException($"The demo exited before it was ready.{Environment.NewLine}{RecentOutput}");
			}

			// Only a connection failure means "not listening yet"; anything else is a real answer and
			// should surface rather than be retried until the timeout hides it.
			try
			{
				using var response = await client.GetAsync(BaseUrl);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					return;
				}
			}
			catch (HttpRequestException)
			{
			}

			await Task.Delay(ReadinessPollInterval);
		}

		throw new TimeoutException($"The demo was not ready after {StartTimeout}.{Environment.NewLine}{RecentOutput}");
	}

	/// <summary>Records a line of the demo's output for a failure message.</summary>
	private void CaptureOutput(object sender, DataReceivedEventArgs args)
	{
		if (!string.IsNullOrWhiteSpace(args.Data))
		{
			_outputLines.Enqueue(args.Data);
		}
	}

	/// <summary>The tail of the demo's output, which is where a start failure explains itself.</summary>
	private string RecentOutput
	{
		get
		{
			return string.Join(Environment.NewLine, _outputLines.TakeLast(100));
		}
	}

	/// <summary>
	/// Stops the demo, whole process tree included. <c>dotnet run</c> is a launcher: killing it alone
	/// leaves the app it started holding the port, and the next run then fails to bind.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		var process = _process;
		_process = null;
		if (process is null)
		{
			return;
		}

		// HasExited can flip between the test and the kill, and Kill then throws rather than
		// no-opping. The process is gone either way, which is all this method wanted.
		try
		{
			if (!process.HasExited)
			{
				process.Kill(entireProcessTree: true);
				await process.WaitForExitAsync();
			}
		}
		catch (InvalidOperationException)
		{
		}

		process.Dispose();
	}

	/// <summary>
	/// Asks the OS for a free loopback port by binding to port 0 and releasing it again.
	/// </summary>
	private static int ReserveTcpPort()
	{
		var listener = new TcpListener(IPAddress.Loopback, 0);
		listener.Start();
		var port = ((IPEndPoint) listener.LocalEndpoint).Port;
		listener.Stop();
		return port;
	}

	/// <summary>
	/// Walks up from the test binary until it finds the checkout, identified by the two files this
	/// suite actually needs rather than by a directory name.
	/// </summary>
	private static string FindRepositoryRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null)
		{
			if (File.Exists(Path.Combine([directory.FullName, .. WebAssemblyHost])) &&
				File.Exists(Path.Combine(directory.FullName, "src", "Blazor.ThreeJS.slnx")))
			{
				return directory.FullName;
			}

			directory = directory.Parent;
		}

		throw new DirectoryNotFoundException("Could not locate the Blazor.ThreeJS repository root.");
	}
}
