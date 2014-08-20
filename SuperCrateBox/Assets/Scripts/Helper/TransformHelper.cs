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

	public static void SetParentLocal(GameObject _child, GameObject _parent )
	{
		var _position = _child.transform.localPosition;
		var _scale = _child.transform.localScale;
		_child.transform.parent = _parent.transform;
		_child.transform.localPosition = _position;
		_child.transform.localScale = _scale;
	}

	public static void SetParentWithoutScale(GameObject _child, GameObject _parent )
	{
		var _scale = _child.transform.localScale;
		_child.transform.parent = _parent.transform;
		_child.transform.localScale = _scale;
	}
}

