using UnityEngine;
using System.Collections;

public static class ColorHelper
{
	public static Vector3 ColorToVector(Color _color)
	{
		return new Vector3 (_color.r, _color.g, _color.b);
	}

	public static Color VectorToColor(Vector3 _vec)
	{
		var _color = new Color (_vec.x, _vec.y, _vec.z);
		_color.a = 255;
		return _color;
	}
}

