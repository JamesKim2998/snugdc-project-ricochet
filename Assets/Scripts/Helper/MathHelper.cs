using UnityEngine;
using System.Collections;

public static class MathHelper 
{
	public static int Mod(int _value, int _mod)
	{
		return (_value % _mod + _mod) % _mod;
	}

	public static float Mod(float _value, float _mod)
	{
		return (_value % _mod + _mod) % _mod;
	}
}

