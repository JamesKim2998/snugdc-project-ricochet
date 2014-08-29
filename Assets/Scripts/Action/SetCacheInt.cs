using UnityEngine;
using System.Collections;

public class SetCacheInt : MonoBehaviour
{
	public bool local = true;
	public string group;
	public string key;
	public int value;
	
	public void Set()
	{
		if (local == false) 
		{
			Debug.Log("Not supported!");
			return;
		}
		
		if (group == null)
		{
			Global.LocalCache ().SetInt(key, value);
		}
		else
		{
			Global.LocalCache ().SetInt(group, key, value);
		}
	}
}

