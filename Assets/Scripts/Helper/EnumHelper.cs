using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class EnumHelper 
{
	public static bool TryParse<T>(string _value, out T _enum)
	{
		_enum = (T) Enum.Parse(typeof(T), _value);
		return _enum != null;
	}

	public static IEnumerable<T> GetValues<T>() 
	{
		return Enum.GetValues(typeof(T)).Cast<T>();
	}
}

