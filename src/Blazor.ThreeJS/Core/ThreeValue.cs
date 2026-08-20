using System.Text.Json;
using System.Text.Json.Serialization;
using Kebechet.Blazor.ThreeJS.Math;
using Kebechet.Blazor.ThreeJS.Objects;

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
	/// Converts a value into its wire representation. A hand-written math type (<see cref="Vector3"/>,
	/// <see cref="Box3"/>, <see cref="Frustum"/> and the rest of <c>Math/</c>)
	/// becomes a <see cref="TaggedValue"/>, <see cref="ThreeObject"/> instances become a
	/// <see cref="HandleReference"/>, <see cref="Unspecified"/> stays the sentinel the applier turns
	/// back into <c>undefined</c>, and primitives, <see cref="string"/> and <see langword="null"/> pass
	/// through unchanged.
	/// <para>
	/// An <see cref="Enum"/> takes whichever form three.js itself uses: the numeric backing value for
	/// most sets — cast explicitly, so a future <c>JsonStringEnumConverter</c> reaching these options
	/// cannot silently turn it into its member name — and the token for the sets three.js spells as
	/// strings, which carry no meaning in their C# value at all.
	/// </para>
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
			case Vector2 vector2:
				return new TaggedValue { Tag = ThreeWireFormat.Vector2Tag, Values = vector2.ToArray() };
			case Vector4 vector4:
				return new TaggedValue { Tag = ThreeWireFormat.Vector4Tag, Values = vector4.ToArray() };
			case Matrix3 matrix3:
				return new TaggedValue { Tag = ThreeWireFormat.Matrix3Tag, Values = matrix3.ToArray() };
			case Box2 box2:
				return new TaggedValue { Tag = ThreeWireFormat.Box2Tag, Values = box2.ToArray() };
			case Box3 box3:
				return new TaggedValue { Tag = ThreeWireFormat.Box3Tag, Values = box3.ToArray() };
			case Sphere sphere:
				return new TaggedValue { Tag = ThreeWireFormat.SphereTag, Values = sphere.ToArray() };
			case Plane plane:
				return new TaggedValue { Tag = ThreeWireFormat.PlaneTag, Values = plane.ToArray() };
			case Ray ray:
				return new TaggedValue { Tag = ThreeWireFormat.RayTag, Values = ray.ToArray() };
			case Line3 line:
				return new TaggedValue { Tag = ThreeWireFormat.Line3Tag, Values = line.ToArray() };
			case Triangle triangle:
				return new TaggedValue { Tag = ThreeWireFormat.TriangleTag, Values = triangle.ToArray() };
			case Spherical spherical:
				return new TaggedValue { Tag = ThreeWireFormat.SphericalTag, Values = spherical.ToArray() };
			case Cylindrical cylindrical:
				return new TaggedValue { Tag = ThreeWireFormat.CylindricalTag, Values = cylindrical.ToArray() };
			case Frustum frustum:
				return new TaggedValue { Tag = ThreeWireFormat.FrustumTag, Values = frustum.ToArray() };
			case SphericalHarmonics3 sphericalHarmonics:
				return new TaggedValue { Tag = ThreeWireFormat.SphericalHarmonics3Tag, Values = sphericalHarmonics.ToArray() };
			case ThreeObject threeObject:
				return new HandleReference { Handle = threeObject.Handle };
			case IThreeStructure structure:
				return new StructureValue
				{
					Members = structure.ToWireMembers().ToDictionary(x => x.Key, x => Encode(x.Value))
				};
			// Ahead of the sequence arm, which a dictionary would otherwise fall into as a sequence of
			// key-value pairs. three.js declares these as `{ [key: string]: T }`, which is a plain object
			// and travels as one.
			case System.Collections.IDictionary map:
				return new StructureValue { Members = EncodeDictionary(map) };
			case UnspecifiedValue unspecified:
				return unspecified;
			case Enum enumValue:
				// three.js spells some of its closed sets as strings — `ColorSpace` is `"srgb"`, not a
				// number — and for those the C# value is only a position. Sending the number would be
				// silently wrong: three.js compares it against its own strings, matches nothing, and
				// carries on with whatever default it had.
				if (ThreeStringEnum.TokenFor(enumValue) is { } token)
				{
					return token;
				}

				return Convert.ChangeType(enumValue, enumValue.GetTypeCode());
			case TypedArray typedArray:
				return new TypedArrayValue
				{
					ConstructorName = typedArray.JavaScriptConstructorName,
					Values = typedArray.Components
				};
			case float single when NonFiniteToken(single) is { } singleToken:
				return new NonFiniteValue { Token = singleToken };
			case double precise when NonFiniteToken(precise) is { } preciseToken:
				return new NonFiniteValue { Token = preciseToken };
			case string text:
				// Ahead of the sequence arm below, which a string would otherwise fall into as a
				// sequence of characters.
				return text;
			case System.Collections.IEnumerable sequence:
				return EncodeSequence(sequence);
			default:
				if (value.GetType().IsValueType)
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
	public static TValue Decode<TValue>(JsonElement? element, ThreeContext? context = null)
	{
		if (element is not { } value || value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
		{
			return default!;
		}

		// Element by element, because the plain deserializer cannot see through a tagged value: an array
		// of Vector2 arrives as [{"$t":"Vector2","v":[…]}, …], and deserializing that onto Vector2[]
		// binds nothing and yields an array of zeroed instances — a fabricated answer with no error.
		if (value.ValueKind == JsonValueKind.Array && typeof(TValue).IsArray)
		{
			return (TValue) DecodeArray(typeof(TValue).GetElementType()!, value, context);
		}

		// A string-valued enum comes back as the token three.js compares against, which the plain
		// deserializer cannot bind to a numeric C# enum. Ahead of the arm below because a token is a
		// JSON string, and because `GLSLVersion` spells one of its tokens `"100"` — left to a converter
		// that reads numeric strings, that would bind to whichever member happened to be value 100.
		if (value.ValueKind == JsonValueKind.String)
		{
			var enumType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
			if (enumType.IsEnum && ThreeStringEnum.FromToken(enumType, value.GetString()!) is { } decodedEnum)
			{
				return (TValue) decodedEnum;
			}
		}

		if (value.ValueKind != JsonValueKind.Object)
		{
			return value.Deserialize<TValue>(_readOptions)!;
		}

		if (value.TryGetProperty(ThreeWireFormat.NonFiniteKey, out var nonFinite))
		{
			var scalar = ParseComponentToken(nonFinite.GetString());
			return typeof(TValue) == typeof(float)
				? (TValue) (object) (float) scalar
				: (TValue) Convert.ChangeType(scalar, typeof(TValue));
		}

		if (value.TryGetProperty(ThreeWireFormat.TypedArrayKey, out var constructorName))
		{
			return (TValue) DecodeTypedArray(constructorName.GetString(), value);
		}

		if (value.TryGetProperty(ThreeWireFormat.StructureKey, out var members)
			&& typeof(TValue).IsGenericType
			&& typeof(TValue).GetGenericTypeDefinition() == typeof(Dictionary<,>))
		{
			// Entry by entry rather than deserialized whole, so a value that is itself tagged - a math
			// value, a typed array - is decoded rather than bound to a bag of its wire fields.
			var valueType = typeof(TValue).GetGenericArguments()[1];
			var map = (System.Collections.IDictionary) Activator.CreateInstance(typeof(TValue))!;
			foreach (var entry in members.EnumerateObject())
			{
				map[entry.Name] = DecodeMember(valueType, entry.Value, context);
			}

			return (TValue) map;
		}

		if (value.TryGetProperty(ThreeWireFormat.StructureKey, out members)
			&& typeof(IThreeStructure).IsAssignableFrom(typeof(TValue)))
		{
			// Built and filled rather than deserialized, because the generated record knows three.js's
			// own member names and the plain deserializer knows only the C# ones. `new()` is enough of a
			// constraint to state here: every implementation is a generated record with one.
			var blank = (IThreeStructure) Activator.CreateInstance(typeof(TValue))!;
			return (TValue) blank.FromWireMembers(
				members.EnumerateObject().ToDictionary(x => x.Name, x => x.Value, StringComparer.Ordinal),
				context);
		}

		if (!value.TryGetProperty(ThreeWireFormat.TagKey, out var tag))
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
	/// Encodes each element of a sequence, so an array crosses the wire as a JSON array of already
	/// encoded values rather than as its own serialized shape.
	/// <para>
	/// Elements go through <see cref="Encode"/> individually, which is what lets an array of mirrored
	/// objects arrive as an array of <c>$ref</c> handles and an array of math values as an array of
	/// tagged values. It also means an element with no encoding throws with its own type named, rather
	/// than the array's.
	/// </para>
	/// </summary>
	/// <param name="sequence">The sequence to encode.</param>
	/// <returns>The encoded elements, in order.</returns>
	/// <summary>
	/// Encodes a dictionary's entries, keyed by the string three.js indexes them by.
	/// </summary>
	/// <param name="map">The dictionary to encode.</param>
	/// <returns>The encoded entries.</returns>
	/// <exception cref="NotSupportedException">Thrown when a key is not a string, which no three.js index signature declares.</exception>
	private static IReadOnlyDictionary<string, object?> EncodeDictionary(System.Collections.IDictionary map)
	{
		var encoded = new Dictionary<string, object?>(StringComparer.Ordinal);
		foreach (System.Collections.DictionaryEntry entry in map)
		{
			if (entry.Key is not string key)
			{
				throw new NotSupportedException(
					$"A dictionary keyed by '{entry.Key.GetType().FullName}' has no wire encoding. " +
					$"Every index signature three.js declares is keyed by string, and a JSON object has no other kind of key.");
			}

			encoded[key] = Encode(entry.Value);
		}

		return encoded;
	}

	private static object?[] EncodeSequence(System.Collections.IEnumerable sequence)
	{
		var encoded = new List<object?>();
		foreach (var element in sequence)
		{
			encoded.Add(Encode(element));
		}

		return encoded.ToArray();
	}

	/// <summary>
	/// Substitutes <see cref="Unspecified"/> for a <see langword="null"/> that means "the caller did
	/// not supply this constructor argument", leaving every supplied value — including a deliberate
	/// <see langword="null"/> on a parameter three.js declares nullable — untouched.
	/// </summary>
	/// <param name="value">The argument as the generated class holds it.</param>
	/// <returns>The value, or the sentinel when it was not supplied.</returns>
	/// <summary>
	/// Decodes one value of a runtime-known type, by calling <see cref="Decode{TValue}"/> through it.
	/// Only the dictionary path needs this: every other decode knows its type at compile time.
	/// </summary>
	/// <param name="valueType">The type to decode into.</param>
	/// <param name="element">The wire value.</param>
	/// <param name="context">Context a nested mirrored object is adopted into.</param>
	/// <returns>The decoded value.</returns>
	private static object? DecodeMember(Type valueType, JsonElement element, ThreeContext? context)
	{
		return typeof(ThreeValue)
			.GetMethod(nameof(Decode), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!
			.MakeGenericMethod(valueType)
			.Invoke(null, [(JsonElement?) element, context]);
	}

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
	/// Rebuilds one of the hand-written math types from its tagged wire form.
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
			case ThreeWireFormat.Vector2Tag:
				return new Vector2(Component(components, 0), Component(components, 1));
			case ThreeWireFormat.Vector4Tag:
				return new Vector4(Component(components, 0), Component(components, 1), Component(components, 2), Component(components, 3));
			case ThreeWireFormat.Matrix3Tag:
				// Column-major on the wire, which is how Elements already stores them. See the Matrix4
				// arm above for why this must not route through Set.
				return new Matrix3().FromArray(Components(components, 9));
			case ThreeWireFormat.Box2Tag:
				return new Box2().FromArray(Components(components, 4));
			case ThreeWireFormat.Box3Tag:
				return new Box3().FromArray(Components(components, 6));
			case ThreeWireFormat.SphereTag:
				return new Sphere().FromArray(Components(components, 4));
			case ThreeWireFormat.PlaneTag:
				return new Plane().FromArray(Components(components, 4));
			case ThreeWireFormat.RayTag:
				return new Ray().FromArray(Components(components, 6));
			case ThreeWireFormat.Line3Tag:
				return new Line3().FromArray(Components(components, 6));
			case ThreeWireFormat.TriangleTag:
				return new Triangle().FromArray(Components(components, 9));
			case ThreeWireFormat.SphericalTag:
				return new Spherical().FromArray(Components(components, 3));
			case ThreeWireFormat.CylindricalTag:
				return new Cylindrical().FromArray(Components(components, 3));
			case ThreeWireFormat.FrustumTag:
				return new Frustum().FromArray(Components(components, Frustum.PlaneCount * 4));
			case ThreeWireFormat.SphericalHarmonics3Tag:
				return new SphericalHarmonics3().FromArray(Components(components, SphericalHarmonics3.CoefficientCount * 3));
			default:
				throw new NotSupportedException(
					$"{nameof(ThreeValue)}.{nameof(Decode)} has no arm for the '{tag}' wire tag. " +
					$"Add one here and a matching encode case in three-interop.js.");
		}
	}

	/// <summary>
	/// Rebuilds a C# array from a JSON array, decoding each element the same way a lone value is
	/// decoded so a tagged element becomes the math value it names.
	/// </summary>
	/// <param name="elementType">C# type of one element.</param>
	/// <param name="array">The JSON array the applier sent back.</param>
	/// <returns>The decoded array, boxed as <see cref="object"/> for the generic caller to cast.</returns>
	private static object DecodeArray(Type elementType, JsonElement array, ThreeContext? context)
	{
		var decoded = Array.CreateInstance(elementType, array.GetArrayLength());
		var index = 0;
		foreach (var element in array.EnumerateArray())
		{
			decoded.SetValue(DecodeElement(elementType, element, context), index);
			index++;
		}

		return decoded;
	}

	/// <summary>Decodes one element of an array: a tagged math value, or anything the plain deserializer handles.</summary>
	/// <param name="elementType">C# type of the element.</param>
	/// <param name="element">The element as it arrived.</param>
	/// <returns>The decoded element.</returns>
	private static object? DecodeElement(Type elementType, JsonElement element, ThreeContext? context)
	{
		if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(ThreeWireFormat.TagKey, out var tag))
		{
			return DecodeMathValue(tag.GetString(), element);
		}

		// Back through the full decode rather than deserialized here, so an element that is itself a
		// structure, a dictionary or a typed array is bound the way it would be on its own. Plain
		// deserialization binds none of those: a `$o`-tagged intersection has no field called `$o` to
		// match, so every member would come back at its C# default.
		if (element.ValueKind == JsonValueKind.Object)
		{
			return DecodeMember(elementType, element, context);
		}

		return element.Deserialize(elementType, _readOptions);
	}

	/// <summary>
	/// Rebuilds a typed array from its <c>$ta</c>-tagged wire form, as the C# class matching the
	/// JavaScript constructor the applier named.
	/// </summary>
	/// <param name="constructorName">JavaScript constructor name carried on the wire.</param>
	/// <param name="value">The whole tagged object, read for its components.</param>
	/// <returns>The reconstructed typed array.</returns>
	/// <exception cref="NotSupportedException">Thrown for a constructor name with no C# class here.</exception>
	private static object DecodeTypedArray(string? constructorName, JsonElement value)
	{
		var components = value.GetProperty(ThreeWireFormat.ValuesKey);
		var elements = new double[components.GetArrayLength()];
		for (var index = 0; index < elements.Length; index++)
		{
			elements[index] = Component(components, index);
		}

		return constructorName switch
		{
			nameof(Float32Array) => new Float32Array(Array.ConvertAll(elements, x => (float) x)),
			nameof(Float64Array) => new Float64Array(elements),
			nameof(Int8Array) => new Int8Array(Array.ConvertAll(elements, x => (sbyte) x)),
			nameof(Int16Array) => new Int16Array(Array.ConvertAll(elements, x => (short) x)),
			nameof(Int32Array) => new Int32Array(Array.ConvertAll(elements, x => (int) x)),
			nameof(Uint8Array) => new Uint8Array(Array.ConvertAll(elements, x => (byte) x)),
			nameof(Uint8ClampedArray) => new Uint8ClampedArray(Array.ConvertAll(elements, x => (byte) x)),
			nameof(Uint16Array) => new Uint16Array(Array.ConvertAll(elements, x => (ushort) x)),
			nameof(Uint32Array) => new Uint32Array(Array.ConvertAll(elements, x => (uint) x)),
			_ => throw new NotSupportedException(
				$"{nameof(ThreeValue)}.{nameof(Decode)} has no arm for the '{constructorName}' typed array. " +
				$"Add one here and a matching class in {nameof(TypedArray)}.cs.")
		};
	}

	/// <summary>Reads the leading components of a tagged math value's array.</summary>
	/// <param name="components">The <c>v</c> array of a tagged value.</param>
	/// <param name="count">How many components the tag's type is built from.</param>
	/// <returns>The components as a <see cref="float"/> array.</returns>
	private static float[] Components(JsonElement components, int count)
	{
		var values = new float[count];
		for (var index = 0; index < count; index++)
		{
			values[index] = Component(components, index);
		}

		return values;
	}

	/// <summary>
	/// Reads one component of a tagged math value's array. A component arrives as a JSON string when
	/// it is not finite, because JSON has no numeric form for one - see
	/// <see cref="ThreeWireFormat.PositiveInfinityToken"/>.
	/// </summary>
	/// <param name="components">The <c>v</c> array of a tagged value.</param>
	/// <param name="index">Position to read.</param>
	/// <returns>The component as a <see cref="float"/>.</returns>
	/// <exception cref="NotSupportedException">Thrown for a string component that names no known token.</exception>
	private static float Component(JsonElement components, int index)
	{
		var component = components[index];
		if (component.ValueKind != JsonValueKind.String)
		{
			return component.GetSingle();
		}

		return component.GetString() switch
		{
			ThreeWireFormat.PositiveInfinityToken => float.PositiveInfinity,
			ThreeWireFormat.NegativeInfinityToken => float.NegativeInfinity,
			ThreeWireFormat.NotANumberToken => float.NaN,
			var token => throw new NotSupportedException(
				$"A tagged math value carried the component '{token}', which is not a number and names no " +
				$"non-finite token. The wire format's component encoding has diverged between the two sides.")
		};
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
		[JsonConverter(typeof(ComponentArrayConverter))]
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
	/// Writes a tagged value's component array, spelling a non-finite component as a string.
	/// <para>
	/// Without this every component would go through <c>Utf8JsonWriter.WriteNumberValue</c>, which
	/// throws <c>ArgumentException</c> on an infinity or a NaN rather than producing invalid JSON. That
	/// is not a hypothetical: three.js seeds an empty <see cref="Box3"/> at ±infinity, so a
	/// default-constructed one would fail on its first flush. See
	/// <see cref="ThreeWireFormat.PositiveInfinityToken"/> for the tokens and why the applier can read
	/// them back with a plain <c>Number(...)</c>.
	/// </para>
	/// </summary>
	private sealed class ComponentArrayConverter : JsonConverter<float[]>
	{
		/// <summary>Reads a component array back. Present to satisfy the base class; nothing deserializes
		/// a <see cref="TaggedValue"/>, because a value read back is decoded from its
		/// <see cref="JsonElement"/> by <see cref="Decode{TValue}"/> instead.</summary>
		/// <param name="reader">The JSON reader.</param>
		/// <param name="typeToConvert">The requested type.</param>
		/// <param name="options">Serializer options.</param>
		/// <returns>The decoded component array.</returns>
		public override float[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var values = new List<float>();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				values.Add(reader.TokenType == JsonTokenType.String
					? ParseToken(reader.GetString())
					: reader.GetSingle());
			}

			return values.ToArray();
		}

		/// <summary>Writes a component array, spelling non-finite components as strings.</summary>
		/// <param name="writer">The JSON writer.</param>
		/// <param name="values">The components to write.</param>
		/// <param name="options">Serializer options.</param>
		public override void Write(Utf8JsonWriter writer, float[] values, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (var value in values)
			{
				WriteComponent(writer, value);
			}

			writer.WriteEndArray();
		}

		private static float ParseToken(string? token)
		{
			return (float) ParseComponentToken(token);
		}
	}

	/// <summary>
	/// Writes a typed array's components. Separate from <see cref="ComponentArrayConverter"/> only
	/// because the element type differs — a typed array widens every element to <see cref="double"/>
	/// so one wire encoding covers all nine — and both delegate to the same writer so the non-finite
	/// spelling cannot drift between them.
	/// </summary>
	private sealed class TypedArrayComponentConverter : JsonConverter<double[]>
	{
		/// <summary>Reads a component array back. Present to satisfy the base class; nothing deserializes
		/// a <see cref="TypedArrayValue"/>.</summary>
		/// <param name="reader">The JSON reader.</param>
		/// <param name="typeToConvert">The requested type.</param>
		/// <param name="options">Serializer options.</param>
		/// <returns>The decoded component array.</returns>
		public override double[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var values = new List<double>();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				values.Add(reader.TokenType == JsonTokenType.String
					? ParseComponentToken(reader.GetString())
					: reader.GetDouble());
			}

			return values.ToArray();
		}

		/// <summary>Writes a component array, spelling non-finite components as strings.</summary>
		/// <param name="writer">The JSON writer.</param>
		/// <param name="values">The components to write.</param>
		/// <param name="options">Serializer options.</param>
		public override void Write(Utf8JsonWriter writer, double[] values, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (var value in values)
			{
				WriteComponent(writer, value);
			}

			writer.WriteEndArray();
		}
	}

	/// <summary>
	/// Writes one <see cref="float"/> component.
	/// <para>
	/// ⚠️ Deliberately not routed through the <see cref="double"/> overload. <c>Utf8JsonWriter</c> writes
	/// the shortest text that round-trips the type it is given, and widening first defeats that:
	/// <c>0.3f</c> as a double is <c>0.30000001192092896</c>, which is nineteen bytes of payload per
	/// component and a different wire byte sequence.
	/// </para>
	/// </summary>
	/// <param name="writer">The JSON writer.</param>
	/// <param name="value">The component to write.</param>
	private static void WriteComponent(Utf8JsonWriter writer, float value)
	{
		if (NonFiniteToken(value) is { } token)
		{
			writer.WriteStringValue(token);
			return;
		}

		writer.WriteNumberValue(value);
	}

	/// <summary>Writes one <see cref="double"/> component. See the <see cref="float"/> overload for why
	/// the two do not share their number write.</summary>
	/// <param name="writer">The JSON writer.</param>
	/// <param name="value">The component to write.</param>
	private static void WriteComponent(Utf8JsonWriter writer, double value)
	{
		if (NonFiniteToken(value) is { } token)
		{
			writer.WriteStringValue(token);
			return;
		}

		writer.WriteNumberValue(value);
	}

	/// <summary>
	/// Names the token a non-finite value travels as, or <see langword="null"/> when the value is a
	/// number JSON can carry. The single owner of that rule, so the component converters cannot spell
	/// it differently. Taking a double covers float too: widening preserves both infinities and NaN.
	/// </summary>
	/// <param name="value">The component to classify.</param>
	/// <returns>The token, or <see langword="null"/> for a finite value.</returns>
	private static string? NonFiniteToken(double value)
	{
		if (double.IsPositiveInfinity(value))
		{
			return ThreeWireFormat.PositiveInfinityToken;
		}

		if (double.IsNegativeInfinity(value))
		{
			return ThreeWireFormat.NegativeInfinityToken;
		}

		return double.IsNaN(value)
			? ThreeWireFormat.NotANumberToken
			: null;
	}

	/// <summary>Turns a non-finite component token back into the value it names.</summary>
	/// <param name="token">The token read off the wire.</param>
	/// <returns>The non-finite value.</returns>
	/// <exception cref="NotSupportedException">Thrown for a string that names no known token.</exception>
	private static double ParseComponentToken(string? token)
	{
		return token switch
		{
			ThreeWireFormat.PositiveInfinityToken => double.PositiveInfinity,
			ThreeWireFormat.NegativeInfinityToken => double.NegativeInfinity,
			ThreeWireFormat.NotANumberToken => double.NaN,
			_ => throw new NotSupportedException(
				$"A tagged value carried the component '{token}', which is not a number and names no " +
				$"non-finite token. The wire format's component encoding has diverged between the two sides.")
		};
	}

	/// <summary>
	/// Wire representation of a lone non-finite number. See <see cref="ThreeWireFormat.NonFiniteKey"/>
	/// for why it is tagged rather than sent as a bare string.
	/// </summary>
	internal sealed class NonFiniteValue
	{
		/// <summary>One of the non-finite tokens naming which value this is.</summary>
		[JsonPropertyName(ThreeWireFormat.NonFiniteKey)]
		public required string Token { get; init; }
	}

	/// <summary>
	/// Wire representation of a typed array: the JavaScript constructor to rebuild it with, plus its
	/// elements. See <see cref="ThreeWireFormat.TypedArrayKey"/> for why a plain JSON array will not do.
	/// </summary>
	internal sealed class TypedArrayValue
	{
		/// <summary>Name of the JavaScript typed-array constructor, resolved off the global object.</summary>
		[JsonPropertyName(ThreeWireFormat.TypedArrayKey)]
		public required string ConstructorName { get; init; }

		/// <summary>The elements, widened to double.</summary>
		[JsonPropertyName(ThreeWireFormat.ValuesKey)]
		[JsonConverter(typeof(TypedArrayComponentConverter))]
		public required double[] Values { get; init; }
	}

	/// <summary>
	/// Wire representation of a plain data object: three.js's own member names, each carrying an
	/// already-encoded value. See <see cref="ThreeWireFormat.StructureKey"/> for why it is tagged.
	/// </summary>
	internal sealed class StructureValue
	{
		/// <summary>The members, keyed by three.js's own name for each.</summary>
		[JsonPropertyName(ThreeWireFormat.StructureKey)]
		public required IReadOnlyDictionary<string, object?> Members { get; init; }
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
