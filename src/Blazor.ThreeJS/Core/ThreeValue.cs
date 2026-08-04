using System.Text.Json.Serialization;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Encodes C# values into the wire form understood by <c>three-interop.js</c>. Math values become
/// a tagged array so the applier can write them into an existing three.js instance instead of
/// allocating, and scene objects become a handle reference.
/// </summary>
internal static class ThreeValue
{
	/// <summary>
	/// Converts a value into its wire representation. Math types (<see cref="Vector3"/>,
	/// <see cref="Euler"/>, <see cref="Quaternion"/>, <see cref="Color"/>, <see cref="Matrix4"/>)
	/// become a <see cref="TaggedValue"/>, <see cref="ThreeObject"/> instances become a
	/// <see cref="HandleReference"/>, an <see cref="Enum"/> value is cast to its numeric backing
	/// value so a future <c>JsonStringEnumConverter</c> reaching these options cannot silently turn
	/// it into its member name, and primitives, <see cref="string"/> and <see langword="null"/> pass
	/// through unchanged.
	/// </summary>
	/// <param name="value">The value to encode.</param>
	/// <returns>The wire-ready representation of <paramref name="value"/>.</returns>
	/// <exception cref="NotSupportedException">
	/// Thrown for a reference type with no encoding arm. Such a value has no wire contract, so
	/// passing it through would ship its serialized public shape and the applier would assign that
	/// plain object over a live three.js instance without raising anything.
	/// </exception>
	public static object? Encode(object? value)
	{
		switch (value)
		{
			case null:
				return null;
			case Vector3 vector:
				return new TaggedValue { Tag = ThreeWireFormat.Vector3Tag, Values = vector.ToArray() };
			case Euler euler:
				// The hand-cast here and the generic Enum arm below only agree because EulerOrder is
				// byte-backed like TaggedValue.Order. Widening EulerOrder's backing type without
				// widening Order would truncate the value on this path while the Enum arm kept it.
				return new TaggedValue { Tag = ThreeWireFormat.EulerTag, Values = euler.ToArray(), Order = (byte) euler.Order };
			case Quaternion quaternion:
				return new TaggedValue { Tag = ThreeWireFormat.QuaternionTag, Values = quaternion.ToArray() };
			case Color color:
				return new TaggedValue { Tag = ThreeWireFormat.ColorTag, Values = color.ToArray() };
			case Matrix4 matrix:
				return new TaggedValue { Tag = ThreeWireFormat.Matrix4Tag, Values = matrix.ToArray() };
			case ThreeObject threeObject:
				return new HandleReference { Handle = threeObject.Handle };
			case Enum enumValue:
				return Convert.ChangeType(enumValue, enumValue.GetTypeCode());
			default:
				if (value is string || value.GetType().IsValueType)
				{
					return value;
				}

				throw new NotSupportedException(
					$"{nameof(ThreeValue)}.{nameof(Encode)} has no encoding for '{value.GetType().FullName}'. " +
					$"Passing it through would serialize its public shape as a plain JSON object, which the applier " +
					$"would then assign over the three.js instance — a silent corruption with no error anywhere. " +
					$"Add an {nameof(Encode)} arm, a {nameof(ThreeWireFormat)} tag, and a matching decode case in three-interop.js.");
		}
	}

	/// <summary>
	/// Wire representation of a math value: a type tag plus its raw components, so the JavaScript
	/// applier can write into an existing three.js instance in place.
	/// </summary>
	internal sealed class TaggedValue
	{
		/// <summary>One of the <see cref="ThreeWireFormat"/> tag constants identifying the math type.</summary>
		[JsonPropertyName("$t")]
		public required string Tag { get; init; }

		/// <summary>The raw component values, e.g. [x, y, z] for a vector.</summary>
		[JsonPropertyName("v")]
		public required float[] Values { get; init; }

		/// <summary>
		/// Rotation order, only set when <see cref="Tag"/> is <see cref="ThreeWireFormat.EulerTag"/>,
		/// and omitted from the payload for every other tag. The applier already reads it
		/// defensively (<c>value.o ?? 0</c>), so absence and an explicit null are equivalent to it.
		/// </summary>
		[JsonPropertyName("o")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public byte? Order { get; init; }
	}

	/// <summary>
	/// Wire representation of a reference to another mirrored object, resolved by the applier
	/// through its own handle table rather than by re-sending the object's data.
	/// </summary>
	internal sealed class HandleReference
	{
		/// <summary>Handle of the referenced object.</summary>
		[JsonPropertyName("$ref")]
		public required int Handle { get; init; }
	}
}
