using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

public static class NetworkSerializer 
{
	public static string Serialize<T> (T _data) 
	{
		var _bf = new BinaryFormatter();
		var _os = new MemoryStream();
		_bf.Serialize(_os, _data);
		return Convert.ToBase64String (_os.GetBuffer ());
	}
	
	public static void Deserialize<T> (string _data, out T _out)
	{
		var _bf = new BinaryFormatter ();
		var _is = new MemoryStream(Convert.FromBase64String(_data));
		_out = (T) _bf.Deserialize(_is);
	}
}
