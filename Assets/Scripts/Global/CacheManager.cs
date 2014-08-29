using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CacheManager
{
	public Dictionary<string, int> Int;
	public Dictionary<string, string> String;
	public Action postBeforeClear;
	public Action postAfterClear;
	
	public CacheManager()
	{
		Int = new Dictionary<string, int> ();
		String = new Dictionary<string, string> ();
	}

	public void Clear()
	{
		if (postBeforeClear != null) postBeforeClear();
		Int.Clear ();
		String.Clear ();
		if (postAfterClear != null) postAfterClear();
	}

	string MakeKey(string _group, string _key)
	{
		return _group + "_" + _key;
	}

	public int? GetInt(string _key) 
	{
		int _value;
		if (Int.TryGetValue(_key, out _value))
		{
			return _value;
		}
		else
		{
			return null;
		}
	}

	public int? GetInt(string _group, string _key)
	{
		int _value;
		if (Int.TryGetValue(MakeKey(_group, _key), out _value))
		{
			return _value;
		}
		else
		{
			return null;
		}
	}

	public void SetInt(string _key, int _value)
	{
		Int [_key] = _value;
	}

	public void SetInt(string _group, string _key, int _value)
	{
		Int [_group + "_" + _key] = _value;
	}
	
	public string GetString(string _key) 
	{
		string _value;
		if (String.TryGetValue(_key, out _value))
		{
			return _value;
		}
		else
		{
			return null;
		}
	}
	
	public string GetString(string _group, string _key)
	{
		string _value;
		if (String.TryGetValue(MakeKey(_group, _key), out _value))
		{
			return _value;
		}
		else
		{
			return null;
		}
	}
	
	public void SetString(string _key, string _value)
	{
		String [_key] = _value;
	}
	
	public void SetString(string _group, string _key, string _value)
	{
		String [MakeKey(_group, _key)] = _value;
	}
}

