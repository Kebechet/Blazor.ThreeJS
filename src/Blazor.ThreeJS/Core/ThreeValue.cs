using System.Text.Json;
using System.Text.Json.Serialization;
using Kebechet.Blazor.ThreeJS.Math;

namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// Encodes C# values into the wire form understood by <c>three-interop.js</c>. Math values become
/// a tagged array so the applier can write them into an existing three.js instance instead of
/// allocating, scene objects become a handle reference, and a constructor argument the caller never
/// supplied becomes a sentinel the applier turns back into JavaScript's <c>undefined</c>.
/// </summary>
internal static class ThreeValue
{
	/// <summary>
	/// The one "argument not supplied" sentinel. A singleton because <see cref="TrimUnspecifiedTail"/>
	/// recognises it by reference: an unsupplied argument is a position in the list, not a value a
	/// caller can construct or compare.
	/// </summary>
	public static readonly UnspecifiedValue Unspecified = new();

	/// <summary>
	/// Options the untagged half of <see cref="Decode{TValue}"/> deserializes under. Web defaults match
	/// what Blazor's own JS interop uses, so a value read back lands on the same C# types a value sent
	/// out came from. Declared here rather than taken from <c>JsonSerializerOptions.Web</c>, which does
	/// not exist before .NET 9 and this package targets back to .NET 6.
	/// </summary>
	private static readonly JsonSerializerOptions _readOptions = new(JsonSerializerDefaults.Web);

	/// <summary>
	/// Converts a value into its wire representation. Math types (<see cref="Vector3"/>,
	/// <see cref="Euler"/>, <see cref="Quaternion"/>, <see cref="Color"/>, <see cref="Matrix4"/>)
	/// become a <see cref="TaggedValue"/>, <see cref="ThreeObject"/> instances become a
	/// <see cref="HandleReference"/>, <see cref="Unspecified"/> stays the sentinel the applier turns
	/// back into <c>undefined</c>, an <see cref="Enum"/> value is cast to its numeric backing
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
			case UnspecifiedValue unspecified:
				return unspecified;
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
	/// Turns a value the applier read back off a three.js object into the C# type the query declares.
	/// The inverse of <see cref="Encode"/>: a <c>$t</c>-tagged object rebuilds the math value it names,
	/// and everything else — a number, a boolean, a string, an enum's numeric value — deserializes
	/// straight onto <typeparamref name="TValue"/>.
	/// </summary>
	/// <typeparam name="TValue">C# type the query declares it returns.</typeparam>
	/// <param name="element">The raw JSON the applier sent back, absent when the read produced no value.</param>
	/// <returns>The decoded value, or the C# default when the read produced <c>null</c> or <c>undefined</c>.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when the applier sent back a math value of a tag <typeparamref name="TValue"/> cannot hold.
	/// A silent <see langword="default"/> there would be a fabricated answer, which is the one outcome a
	/// read must never produce.
	/// </exception>
	/// <exception cref="NotSupportedException">Thrown for a <c>$t</c> tag this decoder has no arm for.</exception>
	public static TValue Decode<TValue>(JsonElement? element)
	{
		if (element is not { } value || value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
		{
			return default!;
		}

		if (value.ValueKind != JsonValueKind.Object || !value.TryGetProperty(ThreeWireFormat.TagKey, out var tag))
		{
			return value.Deserialize<TValue>(_readOptions)!;
		}

		var mathValue = DecodeMathValue(tag.GetString(), value);
		if (mathValue is TValue typedMathValue)
		{
			return typedMathValue;
		}

		throw new InvalidOperationException(
			$"The applier read back a '{tag.GetString()}' value, which cannot be held as '{typeof(TValue).FullName}'. " +
			$"The query's declared return type and the value three.js actually produced have diverged.");
	}

	/// <summary>
	/// Substitutes <see cref="Unspecified"/> for a <see langword="null"/> that means "the caller did
	/// not supply this constructor argument", leaving every supplied value — including a deliberate
	/// <see langword="null"/> on a parameter three.js declares nullable — untouched.
	/// </summary>
	/// <param name="value">The argument as the generated class holds it.</param>
	/// <returns>The value, or the sentinel when it was not supplied.</returns>
	public static object? OrUnspecified(object? value)
	{
		return value ?? Unspecified;
	}

	/// <summary>
	/// Drops the trailing <see cref="Unspecified"/> sentinels from a constructor argument list.
	/// Passing the sentinel and shortening the list are the same instruction — both leave the
	/// parameter <c>undefined</c> — so the shorter form wins for the tail, and the sentinel carries
	/// only the unsupplied arguments that have a supplied one after them.
	/// </summary>
	/// <param name="args">Constructor arguments in three.js parameter order.</param>
	/// <returns>The argument list with its unsupplied tail removed.</returns>
	public static object?[] TrimUnspecifiedTail(object?[] args)
	{
		var count = args.Length;
		while (count > 0 && ReferenceEquals(args[count - 1], Unspecified))
		{
			count--;
		}

		return args[..count];
	}

	/// <summary>
	/// Rebuilds one of the five hand-written math types from its tagged wire form.
	/// </summary>
	/// <param name="tag">The <c>$t</c> tag naming which math type was encoded.</param>
	/// <param name="value">The whole tagged object, read for its components and rotation order.</param>
	/// <returns>The reconstructed math value.</returns>
	/// <exception cref="NotSupportedException">Thrown for a tag with no arm here.</exception>
	private static object DecodeMathValue(string? tag, JsonElement value)
	{
		var components = value.GetProperty(ThreeWireFormat.ValuesKey);
		switch (tag)
		{
			case ThreeWireFormat.Vector3Tag:
				return new Vector3(Component(components, 0), Component(components, 1), Component(components, 2));
			case ThreeWireFormat.EulerTag:
				var order = value.TryGetProperty(ThreeWireFormat.OrderKey, out var encodedOrder)
					? (EulerOrder) encodedOrder.GetByte()
					: EulerOrder.XYZ;

				return new Euler(Component(components, 0), Component(components, 1), Component(components, 2), order);
			case ThreeWireFormat.QuaternionTag:
				return new Quaternion(Component(components, 0), Component(components, 1), Component(components, 2), Component(components, 3));
			case ThreeWireFormat.ColorTag:
				return new Color(Component(components, 0), Component(components, 1), Component(components, 2));
			case ThreeWireFormat.Matrix4Tag:
				// Written straight into Elements rather than through Set: the wire carries the components
				// column-major, which is how Elements already stores them, while Set takes them in visual
				// row-major reading order and transposes. Routing through Set would silently transpose
				// every matrix read back.
				var matrix = new Matrix4();
				for (var index = 0; index < matrix.Elements.Length; index++)
				{
					matrix.Elements[index] = Component(components, index);
				}

				return matrix;
			default:
				throw new NotSupportedException(
					$"{nameof(ThreeValue)}.{nameof(Decode)} has no arm for the '{tag}' wire tag. " +
					$"Add one here and a matching encode case in three-interop.js.");
		}
	}

	/// <summary>Reads one component of a tagged math value's array.</summary>
	/// <param name="components">The <c>v</c> array of a tagged value.</param>
	/// <param name="index">Position to read.</param>
	/// <returns>The component as a <see cref="float"/>.</returns>
	private static float Component(JsonElement components, int index)
	{
		return components[index].GetSingle();
	}

	/// <summary>
	/// Wire representation of an argument the caller never supplied, which the applier decodes to
	/// JavaScript's <c>undefined</c> so three.js applies its own parameter default.
	/// </summary>
	internal sealed class UnspecifiedValue
	{
		/// <summary>
		/// Always <see langword="true"/>. The <see cref="ThreeWireFormat.UndefinedKey"/> key is what
		/// identifies the sentinel; the value only makes it a well-formed JSON object.
		/// </summary>
		[JsonPropertyName(ThreeWireFormat.UndefinedKey)]
		public bool IsUnspecified
		{
			get { return true; }
		}
	}

	/// <summary>
	/// Wire representation of a math value: a type tag plus its raw components, so the JavaScript
	/// applier can write into an existing three.js instance in place.
	/// </summary>
	internal sealed class TaggedValue
	{
		/// <summary>One of the <see cref="ThreeWireFormat"/> tag constants identifying the math type.</summary>
		[JsonPropertyName(ThreeWireFormat.TagKey)]
		public required string Tag { get; init; }

		/// <summary>The raw component values, e.g. [x, y, z] for a vector.</summary>
		[JsonPropertyName(ThreeWireFormat.ValuesKey)]
		public required float[] Values { get; init; }

		/// <summary>
		/// Rotation order, only set when <see cref="Tag"/> is <see cref="ThreeWireFormat.EulerTag"/>,
		/// and omitted from the payload for every other tag. The applier already reads it
		/// defensively (<c>value.o ?? 0</c>), so absence and an explicit null are equivalent to it.
		/// </summary>
		[JsonPropertyName(ThreeWireFormat.OrderKey)]
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
		[JsonPropertyName(ThreeWireFormat.HandleReferenceKey)]
		public required int Handle { get; init; }
	}
}
