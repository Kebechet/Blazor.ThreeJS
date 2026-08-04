namespace Kebechet.Blazor.ThreeJS.Math;

/// <summary>
/// Three-component vector. Mirrors three.js semantics: mutators mutate in place and return
/// <c>this</c> for chaining, so upstream three.js documentation and examples translate directly.
/// </summary>
public sealed class Vector3
{
	private float _x;
	private float _y;
	private float _z;

	/// <summary>
	/// Raised whenever any component changes. Set by an owning <c>Object3D</c> so that writing
	/// <c>mesh.Position.X</c> marks the owner dirty without the owner observing each component.
	/// </summary>
	internal Action? OnChange { get; set; }

	public Vector3()
	{
	}

	public Vector3(float x, float y, float z)
	{
		_x = x;
		_y = y;
		_z = z;
	}

	public float X
	{
		get { return _x; }
		set
		{
			_x = value;
			OnChange?.Invoke();
		}
	}

	public float Y
	{
		get { return _y; }
		set
		{
			_y = value;
			OnChange?.Invoke();
		}
	}

	public float Z
	{
		get { return _z; }
		set
		{
			_z = value;
			OnChange?.Invoke();
		}
	}

	public Vector3 Set(float x, float y, float z)
	{
		_x = x;
		_y = y;
		_z = z;
		OnChange?.Invoke();
		return this;
	}

	public Vector3 Copy(Vector3 other)
	{
		return Set(other._x, other._y, other._z);
	}

	public Vector3 Add(Vector3 other)
	{
		return Set(_x + other._x, _y + other._y, _z + other._z);
	}

	public Vector3 Sub(Vector3 other)
	{
		return Set(_x - other._x, _y - other._y, _z - other._z);
	}

	public Vector3 MultiplyScalar(float scalar)
	{
		return Set(_x * scalar, _y * scalar, _z * scalar);
	}

	public float LengthSq()
	{
		return (_x * _x) + (_y * _y) + (_z * _z);
	}

	public float Length()
	{
		return MathF.Sqrt(LengthSq());
	}

	public Vector3 Normalize()
	{
		var length = Length();
		if (length == 0f)
		{
			return this;
		}

		return MultiplyScalar(1f / length);
	}

	public float Dot(Vector3 other)
	{
		return (_x * other._x) + (_y * other._y) + (_z * other._z);
	}

	public Vector3 Cross(Vector3 other)
	{
		return Set(
			(_y * other._z) - (_z * other._y),
			(_z * other._x) - (_x * other._z),
			(_x * other._y) - (_y * other._x));
	}

	public float DistanceTo(Vector3 other)
	{
		var deltaX = _x - other._x;
		var deltaY = _y - other._y;
		var deltaZ = _z - other._z;
		return MathF.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
	}

	public float[] ToArray()
	{
		return new float[] { _x, _y, _z };
	}

	public Vector3 FromArray(float[] values)
	{
		return Set(values[0], values[1], values[2]);
	}

	public Vector3 Clone()
	{
		return new Vector3(_x, _y, _z);
	}
}
