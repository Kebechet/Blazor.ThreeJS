// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using Kebechet.Blazor.ThreeJS.Core;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// An axis object to visualize the 3 axes in a simple way. The X axis is red. The Y axis is green.
/// The Z axis is blue. The JavaScript-side <c>THREE.AxesHelper</c>.
/// </summary>
public sealed class AxesHelper : LineSegments
{
	private readonly float _size;

	/// <summary>Constructs a new axes helper.</summary>
	/// <param name="size">Size of the lines representing the axes.</param>
	public AxesHelper(float size = 1f)
	{
		_size = size;
	}

	/// <summary>
	/// Adopts an existing JavaScript-side <c>AxesHelper</c> under the handle the browser minted for it.
	/// No create op is emitted: the object already exists, and this mirror's job is to name it.
	/// </summary>
	/// <param name="batch">Batch this object's writes record into.</param>
	/// <param name="handle">Negative handle the JavaScript side registered the object under.</param>
	internal AxesHelper(ThreeBatch batch, int handle)
		: base(batch, handle)
	{
		_size = default!;

		Batch = batch;
	}

	/// <summary>Name of the corresponding three.js constructor, <c>THREE.AxesHelper</c>.</summary>
	protected override string ThreeTypeName
	{
		get { return "AxesHelper"; }
	}

	/// <summary>Constructor arguments forwarded to <c>THREE.AxesHelper</c>: size.</summary>
	protected override object?[] ConstructorArgs
	{
		get { return [_size]; }
	}

	/// <summary>Defines the colors of the axes helper.</summary>
	/// <param name="xAxisColor">The color for the x axis.</param>
	/// <param name="yAxisColor">The color for the y axis.</param>
	/// <param name="zAxisColor">The color for the z axis.</param>
	public void SetColors(Color xAxisColor, Color yAxisColor, Color zAxisColor)
	{
		RecordCall("setColors", xAxisColor, yAxisColor, zAxisColor);
	}

	/// <summary>
	/// Frees the GPU-related resources allocated by this instance. Call this method whenever this
	/// instance is no longer used in your app.
	/// </summary>
	public void Dispose()
	{
		RecordCall("dispose");
	}
}
