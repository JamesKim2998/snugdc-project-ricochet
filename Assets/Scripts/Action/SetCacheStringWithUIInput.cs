using UnityEngine;
using System.Collections;

public class SetCacheStringWithUIInput : MonoBehaviour
{
	public UIInput input;

	public bool local = true;
	public string group;
	public string key;
	
	public void Set()
	{
		if (local == false) 
		{
			Debug.Log("Not supported!");
			return;
		}
		
		if (group == null)
		{
			Global.LocalCache ().SetString(key, input.value);
		}
		else
		{
			Global.LocalCache ().SetString(group, key, input.value);
		}
	}
}

