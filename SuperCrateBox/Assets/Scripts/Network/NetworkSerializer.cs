using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

public static class NetworkSerializer 
{
	public static string Serialize<T>(IEnumerable<T> _enumrable)
	{
		var _bf = new BinaryFormatter();
		var _os = new MemoryStream();

		foreach (var _enumerator in _enumrable) 
			_bf.Serialize(_os, _enumerator);

		return Convert.ToBase64String (_os.GetBuffer ());
	}
	
	public static IEnumerator<T> Deserialize<T> (string _data) 
	{
		var _bf = new BinaryFormatter ();
		var _is = new MemoryStream(Convert.FromBase64String(_data));
		yield return (T) _bf.Deserialize (_is);
	}
}
