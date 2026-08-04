using System.Text;

namespace Blazor.ThreeJS.Emitter.Emit;

/// <summary>
/// Accumulates generated C# source. Indents with tabs and terminates every line with <c>\n</c>, so
/// output is byte-identical on Windows and Linux and the golden comparison cannot fail on line
/// endings alone.
/// </summary>
internal sealed class CSharpWriter
{
	/// <summary>Columns a single tab occupies, used only for deciding where to wrap.</summary>
	public const int TabWidth = 4;

	private readonly StringBuilder _builder = new();
	private int _indentLevel;

	/// <summary>Visual column the current indentation ends at.</summary>
	public int IndentColumn
	{
		get { return _indentLevel * TabWidth; }
	}

	/// <summary>Increases indentation by one level.</summary>
	public void Indent()
	{
		_indentLevel++;
	}

	/// <summary>Decreases indentation by one level.</summary>
	public void Outdent()
	{
		if (_indentLevel == 0)
		{
			throw new InvalidOperationException($"{nameof(Outdent)} was called more times than {nameof(Indent)}.");
		}

		_indentLevel--;
	}

	/// <summary>Writes an empty line, with no trailing indentation.</summary>
	public void WriteLine()
	{
		_builder.Append('\n');
	}

	/// <summary>Writes one indented line.</summary>
	/// <param name="text">Line content, without indentation or a line terminator.</param>
	public void WriteLine(string text)
	{
		_builder.Append('\t', _indentLevel);
		_builder.Append(text);
		_builder.Append('\n');
	}

	/// <summary>Returns the accumulated source.</summary>
	/// <returns>The generated file contents.</returns>
	public string ToSource()
	{
		return _builder.ToString();
	}
}
