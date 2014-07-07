using UnityEngine;
using System.Collections;

public static class TransformHelper
{
	public static Vector2 DegToVector(float _degree) 
	{
		return new Vector2(
			Mathf.Cos(Mathf.Deg2Rad * _degree), 
			Mathf.Sin(Mathf.Deg2Rad * _degree));
	}

	public static float VectorToRad(Vector2 _vector)
	{
		return Mathf.Atan2(_vector.y, _vector.x);
	}

	public static float VectorToDeg(Vector2 _vector)
	{
		return Mathf.Rad2Deg * VectorToRad(_vector);
	}
}

