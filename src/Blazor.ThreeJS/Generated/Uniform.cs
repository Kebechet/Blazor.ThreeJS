// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// Uniforms are global GLSL variables. They are passed to shader programs. The JavaScript-side
/// <c>THREE.Uniform</c>.
/// </summary>
/// <seealso href="https://threejs.org/examples/#webgl_nodes_materials_instance_uniform">WebGL2 / nodes / materials / instance / uniform</seealso>
/// <seealso href="https://threejs.org/examples/#webgpu_instance_uniform">WebGPU / instance / uniform</seealso>
/// <seealso href="https://threejs.org/docs/index.html#api/en/core/Uniform">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/core/Uniform.js">Source</seealso>
public sealed class Uniform : ThreeObject
{
	private readonly object? _value;

	/// <summary>Create a new instance of <c>Uniform</c>.</summary>
	/// <param name="value">
	/// An object containing the value to set up the uniform. It's type must be one of the Uniform Types
	/// described above.
	/// </param>
	public Uniform(object? value)
	{
		_value = value;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>Uniform</c> under the handle the browser minted for it. No
	/// create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal Uniform(ThreeBatch batch, int handle)
		: base(handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.Uniform</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "Uniform"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.Uniform</c>: value.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_value]; }
	}

	/// <summary>
	/// Returns a clone of this uniform. Records a read op, sends it behind every write already pending,
	/// and completes with what <c>clone</c> returned.
	/// </summary>
	/// <returns>The value <c>clone</c> returned, once the JavaScript side has answered.</returns>
	public Task<Uniform?> CloneAsync()
	{
		return RecordReadObject<Uniform>("clone", (adoptedBatch, adoptedHandle) => new Uniform(adoptedBatch, adoptedHandle));
	}
}
