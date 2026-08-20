// Generated from @types/three@0.185.3 by generator/emitter. Do not edit by hand.
// Re-run `npm run emit` after changing the emitter or generator/three-api.json.

using System.Text.Json;
using Kebechet.Blazor.ThreeJS.Core;

namespace Kebechet.Blazor.ThreeJS.Objects;

/// <summary>
/// The shape three.js calls <c>OrthographicCameraView</c>. A plain value rather than a
/// handle-backed object: three.js declares it as a shape, and nothing on either side keeps a
/// reference to one. It travels as its own members, under three.js's names for them.
/// </summary>
public sealed record OrthographicCameraView : IThreeStructure
{
	/// <summary>three.js's <c>enabled</c>.</summary>
	public bool Enabled { get; init; }

	/// <summary>three.js's <c>fullWidth</c>.</summary>
	public float FullWidth { get; init; }

	/// <summary>three.js's <c>fullHeight</c>.</summary>
	public float FullHeight { get; init; }

	/// <summary>three.js's <c>offsetX</c>.</summary>
	public float OffsetX { get; init; }

	/// <summary>three.js's <c>offsetY</c>.</summary>
	public float OffsetY { get; init; }

	/// <summary>three.js's <c>width</c>.</summary>
	public float Width { get; init; }

	/// <summary>three.js's <c>height</c>.</summary>
	public float Height { get; init; }

	/// <summary>
	/// This value's members, keyed by three.js's name for each. An optional member left unset is
	/// omitted rather than sent as null, so three.js applies its own default the way it would for an
	/// object literal that never mentioned it.
	/// </summary>
	/// <returns>The members to send.</returns>
	IReadOnlyDictionary<string, object?> IThreeStructure.ToWireMembers()
	{
		var members = new Dictionary<string, object?>(StringComparer.Ordinal);
		members["enabled"] = Enabled;
		members["fullWidth"] = FullWidth;
		members["fullHeight"] = FullHeight;
		members["offsetX"] = OffsetX;
		members["offsetY"] = OffsetY;
		members["width"] = Width;
		members["height"] = Height;

		return members;
	}

	/// <summary>
	/// Builds a <c>OrthographicCameraView</c> from the members the applier sent back. A member three.js
	/// did not carry keeps this instance's own value, which for the blank instance the decoder builds
	/// is the C# default - and an absent optional member is exactly that.
	/// </summary>
	/// <param name="members">The decoded members, keyed by three.js's name for each.</param>
	/// <returns>The value those members describe.</returns>
	IThreeStructure IThreeStructure.FromWireMembers(IReadOnlyDictionary<string, JsonElement> members)
	{
		return new OrthographicCameraView
		{
			Enabled = members.TryGetValue("enabled", out var enabledElement) ? ThreeValue.Decode<bool>(enabledElement) : Enabled,
			FullWidth = members.TryGetValue("fullWidth", out var fullWidthElement) ? ThreeValue.Decode<float>(fullWidthElement) : FullWidth,
			FullHeight = members.TryGetValue("fullHeight", out var fullHeightElement) ? ThreeValue.Decode<float>(fullHeightElement) : FullHeight,
			OffsetX = members.TryGetValue("offsetX", out var offsetXElement) ? ThreeValue.Decode<float>(offsetXElement) : OffsetX,
			OffsetY = members.TryGetValue("offsetY", out var offsetYElement) ? ThreeValue.Decode<float>(offsetYElement) : OffsetY,
			Width = members.TryGetValue("width", out var widthElement) ? ThreeValue.Decode<float>(widthElement) : Width,
			Height = members.TryGetValue("height", out var heightElement) ? ThreeValue.Decode<float>(heightElement) : Height
		};
	}
}
