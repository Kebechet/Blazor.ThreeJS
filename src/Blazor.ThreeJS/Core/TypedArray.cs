namespace Kebechet.Blazor.ThreeJS.Core;

/// <summary>
/// A block of numbers destined for one of JavaScript's typed arrays.
/// <para>
/// three.js hands these straight to WebGL — a <c>BufferAttribute</c>'s vertex data, a
/// <c>DataTexture</c>'s pixels — which is why a plain JSON array will not do: the applier has to
/// rebuild the exact constructor the GPU upload expects. Each subclass names one, so a signature
/// asking for <see cref="Float32Array"/> cannot be satisfied with <see cref="Uint16Array"/>.
/// </para>
/// <para>
/// Components are held as <see cref="double"/> whatever the subclass's own element type is. That is
/// lossless for all nine: every one of them has elements a double represents exactly, and JSON
/// numbers are doubles regardless, so widening here costs nothing and keeps one wire encoding for the
/// whole family.
/// </para>
/// </summary>
public abstract class TypedArray
{
	/// <summary>
	/// Name of the JavaScript constructor the applier rebuilds this as, resolved off the global object.
	/// A wire token, not a code reference: it is the exact global name, so it must not be derived from
	/// the C# type name even where the two happen to agree.
	/// </summary>
	internal abstract string JavaScriptConstructorName { get; }

	/// <summary>The elements, widened to double for the wire. See the type remarks for why that is lossless.</summary>
	internal abstract double[] Components { get; }

	/// <summary>How many elements this array carries.</summary>
	public abstract int Length { get; }

	/// <summary>Constructs a typed array. Not publicly derivable: the nine subclasses are the whole set
	/// JavaScript defines, and a tenth would name a constructor no runtime has.</summary>
	private protected TypedArray()
	{
	}
}

/// <summary>32-bit floating point elements. The default for vertex positions, normals and UVs.</summary>
public sealed class Float32Array : TypedArray
{
	private readonly float[] _values;

	/// <summary>The elements, in their own precision.</summary>
	public IReadOnlyList<float> Values
	{
		get { return _values; }
	}

	/// <inheritdoc />
	public override int Length
	{
		get { return _values.Length; }
	}

	/// <summary>Wraps the given elements.</summary>
	/// <param name="values">Elements, copied so a later mutation cannot change a payload already sent.</param>
	public Float32Array(params float[] values)
	{
		_values = (float[]) values.Clone();
	}

	internal override string JavaScriptConstructorName
	{
		get { return "Float32Array"; }
	}

	internal override double[] Components
	{
		get { return Array.ConvertAll(_values, x => (double) x); }
	}
}

/// <summary>64-bit floating point elements.</summary>
public sealed class Float64Array : TypedArray
{
	private readonly double[] _values;

	/// <summary>The elements.</summary>
	public IReadOnlyList<double> Values
	{
		get { return _values; }
	}

	/// <inheritdoc />
	public override int Length
	{
		get { return _values.Length; }
	}

	/// <summary>Wraps the given elements.</summary>
	/// <param name="values">Elements, copied so a later mutation cannot change a payload already sent.</param>
	public Float64Array(params double[] values)
	{
		_values = (double[]) values.Clone();
	}

	internal override string JavaScriptConstructorName
	{
		get { return "Float64Array"; }
	}

	internal override double[] Components
	{
		get { return _values; }
	}
}

/// <summary>Signed 8-bit integer elements.</summary>
public sealed class Int8Array : TypedArray
{
	private readonly sbyte[] _values;

	/// <summary>The elements.</summary>
	public IReadOnlyList<sbyte> Values
	{
		get { return _values; }
	}

	/// <inheritdoc />
	public override int Length
	{
		get { return _values.Length; }
	}

	/// <summary>Wraps the given elements.</summary>
	/// <param name="values">Elements, copied so a later mutation cannot change a payload already sent.</param>
	public Int8Array(params sbyte[] values)
	{
		_values = (sbyte[]) values.Clone();
	}

	internal override string JavaScriptConstructorName
	{
		get { return "Int8Array"; }
	}

	internal override double[] Components
	{
		get { return Array.ConvertAll(_values, x => (double) x); }
	}
}

/// <summary>Unsigned 8-bit integer elements. The usual choice for eight-bit-per-channel pixel data.</summary>
public sealed class Uint8Array : TypedArray
{
	private readonly byte[] _values;

	/// <summary>The elements.</summary>
	public IReadOnlyList<byte> Values
	{
		get { return _values; }
	}

	/// <inheritdoc />
	public override int Length
	{
		get { return _values.Length; }
	}

	/// <summary>Wraps the given elements.</summary>
	/// <param name="values">Elements, copied so a later mutation cannot change a payload already sent.</param>
	public Uint8Array(params byte[] values)
	{
		_values = (byte[]) values.Clone();
	}

	internal override string JavaScriptConstructorName
	{
		get { return "Uint8Array"; }
	}

	internal override double[] Components
	{
		get { return Array.ConvertAll(_values, x => (double) x); }
	}
}

/// <summary>
/// Unsigned 8-bit integer elements that saturate rather than wrap when written out of range. Distinct
/// from <see cref="Uint8Array"/> on the JavaScript side, so it is distinct here.
/// </summary>
public sealed class Uint8ClampedArray : TypedArray
{
	private readonly byte[] _values;

	/// <summary>The elements.</summary>
	public IReadOnlyList<byte> Values
	{
		get { return _values; }
	}

	/// <inheritdoc />
	public override int Length
	{
		get { return _values.Length; }
	}

	/// <summary>Wraps the given elements.</summary>
	/// <param name="values">Elements, copied so a later mutation cannot change a payload already sent.</param>
	public Uint8ClampedArray(params byte[] values)
	{
		_values = (byte[]) values.Clone();
	}

	internal override string JavaScriptConstructorName
	{
		get { return "Uint8ClampedArray"; }
	}

	internal override double[] Components
	{
		get { return Array.ConvertAll(_values, x => (double) x); }
	}
}

/// <summary>Signed 16-bit integer elements.</summary>
public sealed class Int16Array : TypedArray
{
	private readonly short[] _values;

	/// <summary>The elements.</summary>
	public IReadOnlyList<short> Values
	{
		get { return _values; }
	}

	/// <inheritdoc />
	public override int Length
	{
		get { return _values.Length; }
	}

	/// <summary>Wraps the given elements.</summary>
	/// <param name="values">Elements, copied so a later mutation cannot change a payload already sent.</param>
	public Int16Array(params short[] values)
	{
		_values = (short[]) values.Clone();
	}

	internal override string JavaScriptConstructorName
	{
		get { return "Int16Array"; }
	}

	internal override double[] Components
	{
		get { return Array.ConvertAll(_values, x => (double) x); }
	}
}

/// <summary>Unsigned 16-bit integer elements. The usual choice for a geometry's index buffer.</summary>
public sealed class Uint16Array : TypedArray
{
	private readonly ushort[] _values;

	/// <summary>The elements.</summary>
	public IReadOnlyList<ushort> Values
	{
		get { return _values; }
	}

	/// <inheritdoc />
	public override int Length
	{
		get { return _values.Length; }
	}

	/// <summary>Wraps the given elements.</summary>
	/// <param name="values">Elements, copied so a later mutation cannot change a payload already sent.</param>
	public Uint16Array(params ushort[] values)
	{
		_values = (ushort[]) values.Clone();
	}

	internal override string JavaScriptConstructorName
	{
		get { return "Uint16Array"; }
	}

	internal override double[] Components
	{
		get { return Array.ConvertAll(_values, x => (double) x); }
	}
}

/// <summary>Signed 32-bit integer elements.</summary>
public sealed class Int32Array : TypedArray
{
	private readonly int[] _values;

	/// <summary>The elements.</summary>
	public IReadOnlyList<int> Values
	{
		get { return _values; }
	}

	/// <inheritdoc />
	public override int Length
	{
		get { return _values.Length; }
	}

	/// <summary>Wraps the given elements.</summary>
	/// <param name="values">Elements, copied so a later mutation cannot change a payload already sent.</param>
	public Int32Array(params int[] values)
	{
		_values = (int[]) values.Clone();
	}

	internal override string JavaScriptConstructorName
	{
		get { return "Int32Array"; }
	}

	internal override double[] Components
	{
		get { return Array.ConvertAll(_values, x => (double) x); }
	}
}

/// <summary>Unsigned 32-bit integer elements. An index buffer past 65 535 vertices needs this one.</summary>
public sealed class Uint32Array : TypedArray
{
	private readonly uint[] _values;

	/// <summary>The elements.</summary>
	public IReadOnlyList<uint> Values
	{
		get { return _values; }
	}

	/// <inheritdoc />
	public override int Length
	{
		get { return _values.Length; }
	}

	/// <summary>Wraps the given elements.</summary>
	/// <param name="values">Elements, copied so a later mutation cannot change a payload already sent.</param>
	public Uint32Array(params uint[] values)
	{
		_values = (uint[]) values.Clone();
	}

	internal override string JavaScriptConstructorName
	{
		get { return "Uint32Array"; }
	}

	internal override double[] Components
	{
		get { return Array.ConvertAll(_values, x => (double) x); }
	}
}
