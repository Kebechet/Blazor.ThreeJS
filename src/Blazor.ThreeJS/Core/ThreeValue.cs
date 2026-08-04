using System.Text.Json.Serialization;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Encodes C# values into the wire form understood by <c>three-interop.js</c>. Math values become
/// a tagged array so the applier can write them into an existing three.js instance instead of
/// allocating, and scene objects become a handle reference.
/// </summary>
public static class ThreeValue
{
	/// <summary>
	/// Converts a value into its wire representation. Math types (<see cref="Vector3"/>,
	/// <see cref="Euler"/>, <see cref="Quaternion"/>, <see cref="Color"/>) become a
	/// <see cref="TaggedValue"/>, <see cref="ThreeObject"/> instances become a
	/// <see cref="HandleReference"/>, and everything else passes through unchanged.
	/// </summary>
	/// <param name="value">The value to encode.</param>
	/// <returns>The wire-ready representation of <paramref name="value"/>.</returns>
	public static object? Encode(object? value)
	{
		switch (value)
		{
			case null:
				return null;
			case Vector3 vector:
				return new TaggedValue { Tag = ThreeWireFormat.Vector3Tag, Values = vector.ToArray() };
			case Euler euler:
				return new TaggedValue { Tag = ThreeWireFormat.EulerTag, Values = euler.ToArray(), Order = (byte) euler.Order };
			case Quaternion quaternion:
				return new TaggedValue { Tag = ThreeWireFormat.QuaternionTag, Values = quaternion.ToArray() };
			case Color color:
				return new TaggedValue { Tag = ThreeWireFormat.ColorTag, Values = color.ToArray() };
			case ThreeObject threeObject:
				return new HandleReference { Handle = threeObject.Handle };
			default:
				return value;
		}
	}

	/// <summary>
	/// Wire representation of a math value: a type tag plus its raw components, so the JavaScript
	/// applier can write into an existing three.js instance in place.
	/// </summary>
	public sealed class TaggedValue
	{
		/// <summary>One of the <see cref="ThreeWireFormat"/> tag constants identifying the math type.</summary>
		[JsonPropertyName("$t")]
		public required string Tag { get; init; }

		/// <summary>The raw component values, e.g. [x, y, z] for a vector.</summary>
		[JsonPropertyName("v")]
		public required float[] Values { get; init; }

		/// <summary>Rotation order, only set when <see cref="Tag"/> is <see cref="ThreeWireFormat.EulerTag"/>.</summary>
		[JsonPropertyName("o")]
		public byte? Order { get; init; }
	}

	/// <summary>
	/// Wire representation of a reference to another mirrored object, resolved by the applier
	/// through its own handle table rather than by re-sending the object's data.
	/// </summary>
	public sealed class HandleReference
	{
		/// <summary>Handle of the referenced object.</summary>
		[JsonPropertyName("$ref")]
		public required int Handle { get; init; }
	}
}
