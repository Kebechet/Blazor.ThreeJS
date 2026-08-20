// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>A continuous line that connects back to the start. The JavaScript-side <c>THREE.LineLoop</c>.</summary>
/// <remarks>
/// This is nearly the same as <c>Line</c>, the only difference is that it is rendered using
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/drawElements">gl.LINE_LOOP</see>
/// instead of
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/WebGLRenderingContext/drawElements">gl.LINE_STRIP</see>,
/// which draws a straight line to the next vertex, and connects the last vertex back to the first.
/// </remarks>
/// <seealso href="https://threejs.org/docs/index.html#api/en/objects/LineLoop">Official Documentation</seealso>
/// <seealso href="https://github.com/mrdoob/three.js/blob/master/src/objects/LineLoop.js">Source</seealso>
public sealed class LineLoop : Line
{
	private readonly BufferGeometry? _geometry;
	private readonly Material? _material;

	/// <summary>Create a new instance of <see cref="LineLoop"/>.</summary>
	/// <param name="geometry">
	/// List of vertices representing points on the line loop. Default <c><c>new
	/// THREE.BufferGeometry()</c></c>.
	/// </param>
	/// <param name="material">Material for the line. Default <c><c>new THREE.LineBasicMaterial()</c></c>.</param>
	public LineLoop(BufferGeometry? geometry = null, Material? material = null)
		: base(geometry: geometry, material: material)
	{
		_geometry = geometry;
		_material = material;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>LineLoop</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal LineLoop(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.LineLoop</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "LineLoop"; }
	}

	/// <summary>
	/// Constructor arguments forwarded to <c>THREE.LineLoop</c>: geometry, material. An argument the
	/// caller left unspecified travels as the wire's not-supplied sentinel, or is trimmed when nothing
	/// supplied follows it, so three.js applies its own default.
	/// </summary>
	protected override object?[] ConstructorArgs
	{
		get
		{
			return ThreeValue.TrimUnspecifiedTail(
			[
				ThreeValue.OrUnspecified(_geometry),
				ThreeValue.OrUnspecified(_material)
			]);
		}
	}

	/// <summary>
	/// Read-only flag to check if a given object is of type <see cref="LineLoop"/>. Read-only in
	/// three.js, so it is read on demand rather than mirrored: records a get op, sends it behind every
	/// write already pending, and completes with the value <c>isLineLoop</c> held.
	/// </summary>
	/// <returns>The value <c>isLineLoop</c> held, once the JavaScript side has answered.</returns>
	public Task<bool> IsLineLoopAsync()
	{
		return GetAsync<bool>("isLineLoop");
	}

	/// <summary>
	/// Attaches the objects <c>THREE.LineLoop</c> is constructed from, so their create ops reach the
	/// batch before the one that references them by handle, then emits this object's own.
	/// </summary>
	/// <param name="batch">Batch to record the ops into.</param>
	internal override void EmitCreate(ThreeBatch batch)
	{
		_geometry?.AttachTo(batch);
		_material?.AttachTo(batch);

		base.EmitCreate(batch);
	}
}
