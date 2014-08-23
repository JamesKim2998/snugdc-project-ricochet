using UnityEngine;
using System.Collections;

public static class ComponentHelper
{
	public static T AddComponentIfNotExists<T>(GameObject _go) where T: Component
	{
		var _component = _go.GetComponent<T>();
		if (_component == null) _component = _go.AddComponent<T>();
		return _component;
	}

	public static void AssignComponentIfNotExists<T>( GameObject _go, ref T _component ) where T: Component
	{
		if (_component != null) return;
		_component = AddComponentIfNotExists<T>(_go);
	}
}

