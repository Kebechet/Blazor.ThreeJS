using Kebechet.Blazor.ThreeJS.Core;
using Microsoft.JSInterop;

namespace Kebechet.Blazor.ThreeJS.Addons;

/// <summary>
/// Loads glTF and GLB models into a <see cref="ThreeContext"/>, wrapping three.js's own
/// <c>GLTFLoader</c> addon.
/// <para>
/// The file is fetched and parsed by the browser, which is the only place it can be: parsing glTF
/// means building buffers, geometries, materials and textures, and none of those have a wire form.
/// What crosses back is a description of the graph — one row per mirrored node — so the objects the
/// browser created can be named from C#. Those rows are the <b>only</b> thing C# learns about the
/// file; see <see cref="GLTFModel"/> for what is mirrored and <see cref="LoadedObject3D"/> for what
/// each mirror knows.
/// </para>
/// <para>
/// The addon module is fetched on the first load and not before, so an application that never loads
/// a model never downloads the loader either.
/// </para>
/// <para>
/// Compressed-mesh and compressed-texture extensions (<c>KHR_draco_mesh_compression</c>,
/// <c>KHR_texture_basisu</c>) are <b>not</b> wired up: they need a decoder addon and its worker
/// scripts, which are separate assets again. A file using one fails the load with the browser's own
/// message rather than silently loading without its geometry.
/// </para>
/// </summary>
public sealed class GLTFLoader
{
	private readonly ThreeContext _context;

	/// <summary>
	/// Creates a loader that loads into <paramref name="context"/>. The loader holds no state of its
	/// own between loads, so one per context, per page, or per call are all equivalent.
	/// </summary>
	/// <param name="context">The context the loaded graph is registered against.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
	public GLTFLoader(ThreeContext context)
	{
		if (context is null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		_context = context;
	}

	/// <summary>
	/// Loads a model and returns the mirror over the graph the browser built.
	/// <para>
	/// The URL is fetched by the browser, so it is resolved against the page rather than against
	/// anything on the server: a relative one works exactly as it would in an <c>&lt;img src&gt;</c>.
	/// A <c>.gltf</c> file's own external references — its <c>.bin</c> buffer, its textures — are
	/// resolved relative to the URL given here, which is three.js's behaviour and not this wrapper's.
	/// </para>
	/// <para>
	/// Unlike a flush, a failure here is <b>not</b> swallowed. A model that could not be fetched or
	/// parsed faults the returned task with the browser's own message, as does a load that outlives
	/// its circuit: a caller awaiting a model must not be handed an empty one.
	/// </para>
	/// </summary>
	/// <param name="url">URL of the <c>.gltf</c> or <c>.glb</c> file, as the browser will fetch it.</param>
	/// <param name="progress">
	/// Receives the browser's own fetch progress while the file downloads, or <see langword="null"/> to
	/// ask for none. Reported a handful of times per load rather than continuously, and a report that
	/// cannot be delivered — a circuit that went away mid-download — is dropped rather than failing the
	/// load.
	/// </param>
	/// <returns>The loaded model, already attached to this loader's context.</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="url"/> is <see langword="null"/>, empty, or whitespace.</exception>
	/// <exception cref="InvalidOperationException">Thrown when the browser answered without a root node.</exception>
	public async Task<GLTFModel> LoadAsync(string url, IProgress<GltfLoadProgress>? progress = null)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			throw new ArgumentException("A glTF URL is required; the browser has nothing to fetch without one.", nameof(url));
		}

		// Disposed however the load ends. An undisposed reference keeps the reporter, and everything it
		// closes over, alive in the browser's reference table for as long as the circuit lives.
		using var progressReference = progress is null
			? null
			: DotNetObjectReference.Create(new GltfProgressReporter(progress));

		var response = await _context.LoadGltfAsync(url, progressReference);
		if (!response.Nodes.Any())
		{
			throw new InvalidOperationException(
				$"The browser loaded '{url}' but reported no nodes for it. Every load produces at least the root of the graph, " +
				$"so this is a wire-format disagreement with three-interop.js rather than an empty file.");
		}

		var mirroredNodes = response.Nodes
			.Select(x => new LoadedObject3D(_context.Batch, x))
			.ToList();

		var namedDescendants = mirroredNodes
			.Skip(1)
			.ToList();

		return new GLTFModel(mirroredNodes.First(), namedDescendants);
	}
}
