// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// This class generates a Prefiltered, Mipmapped Radiance Environment Map (PMREM) from a cubeMap
/// environment texture. The JavaScript-side <c>THREE.PMREMGenerator</c>.
/// </summary>
/// <remarks>
/// This allows different levels of blur to be quickly accessed based on material roughness Unlike a
/// traditional mipmap chain, it only goes down to the LOD_MIN level (above), and then creates extra
/// even more filtered 'mips' at the same LOD_MIN resolution, associated with higher roughness
/// levels In this way we maintain resolution to smoothly interpolate diffuse lighting while
/// limiting sampling computation. Note: The minimum <c>MeshStandardMaterial</c>'s roughness depends
/// on the size of the provided texture If your render has small dimensions or the shiny parts have
/// a lot of curvature, you may still be able to get away with a smaller texture size. | texture
/// size | minimum roughness | |--------------|--------------------| | 16 | 0.21 | | 32 | 0.15 | |
/// 64 | 0.11 | | 128 | 0.076 | | 256 | 0.054 | | 512 | 0.038 | | 1024 | 0.027 |.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/extras/PMREMGenerator">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/extras/PMREMGenerator.js">Source</seealso>
public sealed class PMREMGenerator : ThreeObject
{
	private readonly WebGLRenderer _renderer;

	/// <summary>This constructor creates a new PMREMGenerator.</summary>
	/// <param name="renderer">Value forwarded to the <c>renderer</c> constructor argument.</param>
	public PMREMGenerator(WebGLRenderer renderer)
	{
		_renderer = renderer;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.PMREMGenerator</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "PMREMGenerator"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.PMREMGenerator</c>: renderer.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_renderer]; }
	}

	/// <summary>Pre-compiles the cubemap shader.</summary>
	public void CompileCubemapShader()
	{
		RecordCall("compileCubemapShader");
	}

	/// <summary>Pre-compiles the equirectangular shader.</summary>
	public void CompileEquirectangularShader()
	{
		RecordCall("compileEquirectangularShader");
	}

	/// <summary>Frees the GPU-related resources allocated by this instance.</summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.PMREMGenerator</c> is constructed from, so their create ops reach
	/// the batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_renderer.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
