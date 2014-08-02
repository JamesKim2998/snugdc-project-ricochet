using UnityEngine;
using System.Collections;

public class SetCacheString : MonoBehaviour
{
	public bool local = true;
	public string group;
	public string key;
	public string value;

	public void Set()
	{
		if (local == false) 
		{
			Debug.Log("Not supported!");
			return;
		}

		if (group == null)
		{
			Global.LocalCache ().SetString(key, value);
		}
		else
		{
			Global.LocalCache ().SetString(group, key, value);
		}

	}
}

