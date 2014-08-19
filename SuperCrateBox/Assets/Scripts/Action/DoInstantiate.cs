using UnityEngine;
using System.Collections;

public class DoInstantiate : MonoBehaviour 
{
	public GameObject prefab;
	public GameObject parent;

	void Start () {
		if (prefab == null
		    || parent == null)
		{
			Debug.LogError("Missing component!");
			return;
		}
	}

	public void Instantiate() 
	{
		var _go = Instantiate(prefab) as GameObject;
		var _scale = _go.transform.localScale;
		_go.transform.parent = parent.transform;
		_go.transform.localScale = _scale;
	}
}
