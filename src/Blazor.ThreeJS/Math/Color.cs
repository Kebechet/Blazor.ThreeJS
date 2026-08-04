namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// RGB colour with channels in the 0..1 range, matching three.js rather than 0..255.
/// </summary>
public sealed class Color
{
	private float _r = 1f;
	private float _g = 1f;
	private float _b = 1f;

	/// <summary>
	/// Raised whenever any channel changes. Set by an owning material or mesh so that writing
	/// <c>material.Color.R = 0.5f</c> marks the owner dirty without the owner observing each channel.
	/// </summary>
	internal Action? OnChange { get; set; }

	/// <summary>
	/// Initializes a new color with all channels set to 1.0 (white).
	/// </summary>
	public Color()
	{
	}

	/// <summary>
	/// Initializes a new color with the given channel values.
	/// </summary>
	/// <param name="r">The red channel, in the range 0..1.</param>
	/// <param name="g">The green channel, in the range 0..1.</param>
	/// <param name="b">The blue channel, in the range 0..1.</param>
	public Color(float r, float g, float b)
	{
		_r = r;
		_g = g;
		_b = b;
	}

	/// <summary>
	/// Gets a new white color instance (1, 1, 1). Each access constructs a new instance,
	/// so callers cannot accidentally share or alias a mutable preset.
	/// </summary>
	public static Color White
	{
		get { return new Color(1f, 1f, 1f); }
	}

	/// <summary>
	/// Gets a new black color instance (0, 0, 0). Each access constructs a new instance,
	/// so callers cannot accidentally share or alias a mutable preset.
	/// </summary>
	public static Color Black
	{
		get { return new Color(0f, 0f, 0f); }
	}

	/// <summary>
	/// Gets a new red color instance (1, 0, 0). Each access constructs a new instance,
	/// so callers cannot accidentally share or alias a mutable preset.
	/// </summary>
	public static Color Red
	{
		get { return new Color(1f, 0f, 0f); }
	}

	/// <summary>
	/// Gets a new green color instance (0, 1, 0). Each access constructs a new instance,
	/// so callers cannot accidentally share or alias a mutable preset.
	/// </summary>
	public static Color Green
	{
		get { return new Color(0f, 1f, 0f); }
	}

	/// <summary>
	/// Gets a new blue color instance (0, 0, 1). Each access constructs a new instance,
	/// so callers cannot accidentally share or alias a mutable preset.
	/// </summary>
	public static Color Blue
	{
		get { return new Color(0f, 0f, 1f); }
	}

	/// <summary>
	/// Gets or sets the red channel. The value is in the range 0..1, not 0..255.
	/// Setting this channel triggers the <c>OnChange</c> callback, unless the value is unchanged.
	/// </summary>
	public float R
	{
		get { return _r; }
		set
		{
			if (_r == value)
			{
				return;
			}

			_r = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Gets or sets the green channel. The value is in the range 0..1, not 0..255.
	/// Setting this channel triggers the <c>OnChange</c> callback, unless the value is unchanged.
	/// </summary>
	public float G
	{
		get { return _g; }
		set
		{
			if (_g == value)
			{
				return;
			}

			_g = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Gets or sets the blue channel. The value is in the range 0..1, not 0..255.
	/// Setting this channel triggers the <c>OnChange</c> callback, unless the value is unchanged.
	/// </summary>
	public float B
	{
		get { return _b; }
		set
		{
			if (_b == value)
			{
				return;
			}

			_b = value;
			OnChange?.Invoke();
		}
	}

	/// <summary>
	/// Sets all three channels and triggers the <c>OnChange</c> callback once (not per channel).
	/// Writing the values this color already holds changes nothing and raises nothing, so a consumer
	/// loop that reassigns unchanged state every frame costs no interop.
	/// </summary>
	/// <param name="r">The new red channel, in the range 0..1.</param>
	/// <param name="g">The new green channel, in the range 0..1.</param>
	/// <param name="b">The new blue channel, in the range 0..1.</param>
	/// <returns>This color, for method chaining.</returns>
	public Color Set(float r, float g, float b)
	{
		if (_r == r && _g == g && _b == b)
		{
			return this;
		}

		_r = r;
		_g = g;
		_b = b;
		OnChange?.Invoke();
		return this;
	}

	/// <summary>
	/// Sets the color from a hexadecimal RGB value. The hex value is interpreted as 0xRRGGBB,
	/// where each channel in the hex representation (0..255) is divided by 255 to produce the 0..1 range.
	/// Triggers the <c>OnChange</c> callback.
	/// </summary>
	/// <param name="hex">A hexadecimal RGB value, e.g. 0xff0000 for pure red.</param>
	/// <returns>This color, for method chaining.</returns>
	public Color SetHex(int hex)
	{
		return Set(
			((hex >> 16) & 0xff) / 255f,
			((hex >> 8) & 0xff) / 255f,
			(hex & 0xff) / 255f);
	}

	/// <summary>
	/// Gets the color as a hexadecimal RGB value. Each channel is clamped to 0..1, rounded to the nearest integer
	/// in the 0..255 range, and packed into 0xRRGGBB format.
	/// </summary>
	/// <returns>A hexadecimal RGB value, e.g. 0xff0000 for pure red.</returns>
	public int GetHex()
	{
		var r = (int) MathF.Round(System.Math.Clamp(_r, 0f, 1f) * 255f);
		var g = (int) MathF.Round(System.Math.Clamp(_g, 0f, 1f) * 255f);
		var b = (int) MathF.Round(System.Math.Clamp(_b, 0f, 1f) * 255f);
		return (r << 16) | (g << 8) | b;
	}

	/// <summary>
	/// Creates a new color initialized from a hexadecimal RGB value.
	/// </summary>
	/// <param name="hex">A hexadecimal RGB value, e.g. 0xff0000 for pure red.</param>
	/// <returns>A new color with channels set to the values decoded from the hex representation.</returns>
	public static Color FromHex(int hex)
	{
		var color = new Color();
		color.SetHex(hex);
		return color;
	}

	/// <summary>
	/// Extracts the channels of this color into an array.
	/// </summary>
	/// <returns>A new array containing [r, g, b] with values in the 0..1 range.</returns>
	public float[] ToArray()
	{
		return [_r, _g, _b];
	}
}
